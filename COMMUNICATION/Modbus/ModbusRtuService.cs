using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace COMMUNICATION
{
    //默认为8 1 无校验，只需要输入串口号和波特率
    public class ModbusRtuService : IDisposable
    {
        #region 构造函数
        /// <summary>
        /// 初始化 Modbus RTU 服务
        /// </summary>
        /// <param name="portName">串口名称 (如 "COM1")</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="slaveId">从站地址</param>
        public ModbusRtuService(string portName, int baudRate, byte slaveId = 1)
        {
            _portName = portName;
            _baudRate = baudRate;
            _slaveId = slaveId;
        }
        #endregion

        #region 属性和字段
        private readonly object _rwLock = new object();
        private const int BufferSize = 256;

        private SerialPort _serialPort;
        private readonly string _portName;
        private readonly int _baudRate;
        private readonly byte _slaveId;
        private bool _isConnected = false;

        // 内部接收缓冲
        private byte[] _internalBuffer = new byte[0];

        public bool IsConnected => _isConnected && _serialPort?.IsOpen == true;

        // 异步锁，保证同一时间只有一个读写操作（串口是半双工）
        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);
        #endregion

        #region 打开关闭方法
        public void Close()
        {
            lock (_rwLock)
            {
                try
                {
                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        _serialPort.DiscardInBuffer();
                        _serialPort.DiscardOutBuffer();
                        _serialPort.Close();
                        _serialPort.Dispose();
                        _serialPort = null;
                    }
                }
                catch { }
                finally
                {
                    _isConnected = false;
                    _internalBuffer = new byte[0];
                }
            }
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// 打开串口连接
        /// </summary>
        public Task<bool> ConnectAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    lock (_rwLock)
                    {
                        Close(); // 确保先关闭旧连接
                        // 默认参数: 8数据位, 无校验, 1停止位 (8-N-1)
                        _serialPort = new SerialPort(_portName, _baudRate, Parity.None, 8, StopBits.One);
                        _serialPort.ReadTimeout = 1000;
                        _serialPort.WriteTimeout = 1000;
                        _serialPort.Open();
                        _isConnected = true;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _isConnected = false;
                    throw new Exception($"串口连接失败: {ex.Message}");
                }
            });
        }
        #endregion

        #region 核心底层：构建报文与CRC16校验
        /// <summary>
        /// 计算 CRC16 校验码 (Modbus RTU 标准多项式 0xA001)
        /// </summary>
        private ushort CalculateCrc16(byte[] data, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }

        /// <summary>
        /// 构建 Modbus RTU 读取请求 (功能码 03)
        /// </summary>
        private byte[] BuildReadRequest(ushort address, ushort registerCount = 1)
        {
            // RTU 二进制帧：[SlaveID][FuncCode][AddrH][AddrL][QtyH][QtyL][CRCLow][CRCHigh]
            byte[] rawFrame = new byte[6];
            rawFrame[0] = _slaveId;
            rawFrame[1] = 0x03;
            rawFrame[2] = (byte)(address >> 8);
            rawFrame[3] = (byte)(address & 0xFF);
            rawFrame[4] = (byte)(registerCount >> 8);
            rawFrame[5] = (byte)(registerCount & 0xFF);

            ushort crc = CalculateCrc16(rawFrame, 6);

            // 拼接完整报文
            byte[] request = new byte[8];
            Array.Copy(rawFrame, request, 6);
            request[6] = (byte)(crc & 0xFF);       // CRC 低字节在前
            request[7] = (byte)((crc >> 8) & 0xFF); // CRC 高字节在后
            return request;
        }
        #endregion

        #region 核心：带拆包的发送+接收（通用底层方法）
        /// <summary>
        /// 发送 RTU 请求 + 循环接收直到凑齐完整响应帧 (解决串口半包问题)
        /// </summary>
        private async Task<byte[]> SendAndReceiveRtuAsync(byte[] rtuRequest, CancellationToken ct = default)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus RTU 串口未连接");

            await _asyncLock.WaitAsync(ct);
            try
            {
                // 清空历史残留数据
                _serialPort.DiscardInBuffer();
                _internalBuffer = new byte[0];

                // 发送 RTU 二进制报文
                await _serialPort.BaseStream.WriteAsync(rtuRequest, 0, rtuRequest.Length, ct);

                byte[] recvBuffer = new byte[BufferSize];

                // 根据波特率计算 3.5 字符超时时间 (例如 9600bps 约 4ms)
                // 这里使用一个保守的轮询等待策略，避免阻塞UI线程
                DateTime lastRxTime = DateTime.Now;
                int expectedLength = 0;

                while (!ct.IsCancellationRequested)
                {
                    if (_serialPort.BytesToRead > 0)
                    {
                        int readLen = await _serialPort.BaseStream.ReadAsync(recvBuffer, 0, recvBuffer.Length, ct);

                        if (readLen == 0)
                        {
                            Close();
                            throw new Exception("串口连接断开或超时");
                        }

                        // 追加到内部缓冲区
                        int oldLen = _internalBuffer.Length;
                        Array.Resize(ref _internalBuffer, oldLen + readLen);
                        Array.Copy(recvBuffer, 0, _internalBuffer, oldLen, readLen);

                        lastRxTime = DateTime.Now; // 更新最后接收时间

                        // 如果还没解析出预期长度，尝试解析前几个字节获取数据长度
                        // Modbus RTU 读寄存器响应格式: [Addr(1)][Func(1)][ByteCount(1)][Data(N)][CRC(2)]
                        if (expectedLength == 0 && _internalBuffer.Length >= 3 && _internalBuffer[1] == 0x03)
                        {
                            byte byteCount = _internalBuffer[2];
                            expectedLength = 3 + byteCount + 2; // 总长度 = 地址(1)+功能(1)+字节数(1)+数据(N)+CRC(2)
                        }
                    }

                    // 判断是否收齐完整报文
                    if (expectedLength > 0 && _internalBuffer.Length >= expectedLength)
                    {
                        byte[] fullPacket = new byte[expectedLength];
                        Array.Copy(_internalBuffer, fullPacket, expectedLength);
                        return fullPacket;
                    }

                    // 如果没有新数据，且超过了静默时间（防粘包/判断帧结束），抛出异常或返回当前数据
                    // 这里简单处理：如果已经收到了足够的数据但迟迟没收齐，或者长时间没收到新数据
                    if ((DateTime.Now - lastRxTime).TotalMilliseconds > 50 && _internalBuffer.Length > 0)
                    {
                        // 超过50ms没有新数据到达，认为一帧已经结束
                        if (expectedLength > 0 && _internalBuffer.Length >= expectedLength)
                        {
                            byte[] fullPacket = new byte[expectedLength];
                            Array.Copy(_internalBuffer, fullPacket, expectedLength);
                            return fullPacket;
                        }

                        // 如果连基本长度都不够就超时了，说明通信异常
                        throw new Exception($"RTU 接收超时或不完整，已收到 {_internalBuffer.Length} 字节");
                    }

                    await Task.Delay(2, ct); // 短暂让出CPU
                }

                throw new OperationCanceledException("接收响应被取消");
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        /// <summary>
        /// 校验 RTU 响应帧的 CRC
        /// </summary>
        private void VerifyRtuResponse(byte[] response)
        {
            if (response.Length < 4)
                throw new Exception("RTU 响应报文长度不足");

            // 提取报文中的 CRC
            ushort receivedCrc = (ushort)(response[response.Length - 2] | (response[response.Length - 1] << 8));

            // 计算有效数据的 CRC
            ushort calculatedCrc = CalculateCrc16(response, response.Length - 2);

            if (receivedCrc != calculatedCrc)
                throw new Exception($"RTU CRC 校验失败! 预期:{calculatedCrc:X4}, 实际:{receivedCrc:X4}");
        }
        #endregion

        #region 业务层：读取寄存器
        public async Task<short> ReadIntRegisterAsync(ushort address)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus未连接");

            byte[] request = BuildReadRequest(address, 1);
            byte[] response = await SendAndReceiveRtuAsync(request);

            VerifyRtuResponse(response);

            // 校验功能码和数据长度
            if (response[1] != 0x03)
                throw new Exception($"Modbus 异常码: {response[1]}");
            if (response[2] < 2)
                throw new Exception("寄存器数据长度不足");

            // 提取数据 (大端序拼接)
            short value = (short)((response[3] << 8) | response[4]);
            return value;
        }

        public async Task<float> ReadFloatRegisterAsync(ushort address)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus未连接");

            // Float 占 2 个寄存器
            byte[] request = BuildReadRequest(address, 2);
            byte[] response = await SendAndReceiveRtuAsync(request);

            VerifyRtuResponse(response);

            // 校验功能码和数据长度
            if (response[1] != 0x03)
                throw new Exception($"Modbus 异常码: {response[1]}");
            if (response[2] < 4)
                throw new Exception("浮点数寄存器数据长度不足");

            // 提取连续的 4 个字节 (索引 3, 4, 5, 6)
            byte[] floatBytes = new byte[4];
            Array.Copy(response, 3, floatBytes, 0, 4);

            // 完美继承你原代码中的大端序及 Word Swap 处理逻辑
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(floatBytes);
            }

            var tempAB = floatBytes[0];
            floatBytes[0] = floatBytes[1];
            floatBytes[1] = tempAB;
            Array.Reverse(floatBytes);

            return BitConverter.ToSingle(floatBytes, 0);
        }
        #endregion
    }
}