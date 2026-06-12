using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COMMUNICATION
{
    //默认为8 1 无校验，只需要输入串口号和波特率
    public class ModbusAsciiService : IDisposable
    {
        #region 构造函数
        /// <summary>
        /// 初始化 Modbus ASCII 服务
        /// </summary>
        /// <param name="portName">串口名称 (如 "COM1")</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="slaveId">从站地址</param>
        public ModbusAsciiService(string portName, int baudRate, byte slaveId = 1)
        {
            _portName = portName;
            _baudRate = baudRate;
            _slaveId = slaveId;
        }
        #endregion

        #region 属性和字段
        private readonly object _rwLock = new object();
        private const int BufferSize = 512; // ASCII报文较长，缓冲区适当加大

        private SerialPort _serialPort;
        private readonly string _portName;
        private readonly int _baudRate;
        private readonly byte _slaveId;
        private bool _isConnected = false;

        // 用于处理串口粘包/半包的内部缓冲
        private readonly StringBuilder _internalBuffer = new StringBuilder();

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
                    _internalBuffer.Clear();
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

        #region 核心底层：构建报文与LRC校验
        /// <summary>
        /// 计算 LRC 校验和 (Modbus ASCII 专用)
        /// </summary>
        private byte CalculateLrc(byte[] data, int length)
        {
            byte lrc = 0;
            for (int i = 0; i < length; i++)
            {
                lrc += data[i];
            }
            return (byte)(-(sbyte)lrc); // LRC 为补码
        }

        /// <summary>
        /// 构建 Modbus ASCII 读取请求 (功能码 03)
        /// </summary>
        private string BuildReadRequest(ushort address, ushort registerCount = 1)
        {
            // 原始二进制数据帧：[SlaveID][FuncCode][AddrH][AddrL][QtyH][QtyL]
            byte[] rawFrame = new byte[6];
            rawFrame[0] = _slaveId;
            rawFrame[1] = 0x03;
            rawFrame[2] = (byte)(address >> 8);
            rawFrame[3] = (byte)(address & 0xFF);
            rawFrame[4] = (byte)(registerCount >> 8);
            rawFrame[5] = (byte)(registerCount & 0xFF);

            byte lrc = CalculateLrc(rawFrame, 6);

            // 将二进制转换为 ASCII Hex 字符串，并加上 ':' 前缀和 '\r\n' 后缀
            StringBuilder sb = new StringBuilder(":");
            foreach (byte b in rawFrame)
            {
                sb.Append(b.ToString("X2"));
            }
            sb.Append(lrc.ToString("X2"));
            sb.Append("\r\n");

            return sb.ToString();
        }
        #endregion

        #region 核心：带拆包的发送+接收（通用底层方法）
        /// <summary>
        /// 发送 ASCII 请求 + 循环接收直到遇到 \r\n (解决串口半包问题)
        /// </summary>
        private async Task<string> SendAndReceiveAsciiAsync(string asciiRequest, CancellationToken ct = default)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus ASCII 串口未连接");

            await _asyncLock.WaitAsync(ct);
            try
            {
                // 清空历史残留数据
                _serialPort.DiscardInBuffer();
                _internalBuffer.Clear();

                // 发送 ASCII 报文
                byte[] sendBytes = Encoding.ASCII.GetBytes(asciiRequest);
                await _serialPort.BaseStream.WriteAsync(sendBytes, 0, sendBytes.Length, ct);

                byte[] recvBuffer = new byte[BufferSize];

                // 循环接收，直到检测到结束符 \r\n
                while (!ct.IsCancellationRequested)
                {
                    int readLen = await _serialPort.BaseStream.ReadAsync(recvBuffer, 0, recvBuffer.Length, ct);

                    if (readLen == 0)
                    {
                        Close();
                        throw new Exception("串口连接断开或超时");
                    }

                    // 将收到的字节追加到内部缓冲区
                    string chunk = Encoding.ASCII.GetString(recvBuffer, 0, readLen);
                    _internalBuffer.Append(chunk);

                    string currentData = _internalBuffer.ToString();

                    // 检查是否包含完整的响应报文 (以 \r\n 结尾)
                    int endIndex = currentData.IndexOf("\r\n", StringComparison.Ordinal);
                    if (endIndex >= 0)
                    {
                        // 提取完整的一行报文
                        string fullResponse = currentData.Substring(0, endIndex);

                        // 清理内部缓冲区中已处理的数据（防止粘包）
                        _internalBuffer.Remove(0, endIndex + 2);

                        return fullResponse;
                    }
                }

                throw new Exception("接收响应超时");
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        /// <summary>
        /// 解析 ASCII 响应字符串为二进制字节数组
        /// </summary>
        private byte[] ParseAsciiResponse(string asciiResponse)
        {
            // 去除开头的 ':'
            if (string.IsNullOrEmpty(asciiResponse) || asciiResponse[0] != ':')
                throw new Exception($"无效的 Modbus ASCII 响应: {asciiResponse}");

            string hexString = asciiResponse.Substring(1);

            // 验证 LRC 校验
            if (hexString.Length < 4)
                throw new Exception("响应报文长度不足");

            byte[] rawData = new byte[hexString.Length / 2];
            for (int i = 0; i < rawData.Length; i++)
            {
                rawData[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            // 校验 LRC (最后一个字节是 LRC)
            byte calculatedLrc = CalculateLrc(rawData, rawData.Length - 1);
            if (calculatedLrc != rawData[rawData.Length - 1])
                throw new Exception($"LRC 校验失败! 预期:{calculatedLrc:X2}, 实际:{rawData[rawData.Length - 1]:X2}");

            // 返回不含 LRC 的有效数据部分
            byte[] validData = new byte[rawData.Length - 1];
            Array.Copy(rawData, 0, validData, 0, validData.Length);
            return validData;
        }
        #endregion

        #region 业务层：读取寄存器
        public async Task<short> ReadIntRegisterAsync(ushort address)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus未连接");

            string request = BuildReadRequest(address, 1);
            string response = await SendAndReceiveAsciiAsync(request);
            byte[] data = ParseAsciiResponse(response);

            // 校验功能码和数据长度
            if (data[1] != 0x03)
                throw new Exception($"Modbus 异常码: {data[1]}");
            if (data[2] < 2)
                throw new Exception("寄存器数据长度不足");

            // 提取数据 (大端序拼接)
            short value = (short)((data[3] << 8) | data[4]);
            return value;
        }

        public async Task<float> ReadFloatRegisterAsync(ushort address)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus未连接");

            // Float 占 2 个寄存器
            string request = BuildReadRequest(address, 2);
            string response = await SendAndReceiveAsciiAsync(request);
            byte[] data = ParseAsciiResponse(response);

            // 校验功能码和数据长度
            if (data[1] != 0x03)
                throw new Exception($"Modbus 异常码: {data[1]}");
            if (data[2] < 4)
                throw new Exception("浮点数寄存器数据长度不足");

            // 提取连续的 4 个字节 (索引 3, 4, 5, 6)
            byte[] floatBytes = new byte[4];
            Array.Copy(data, 3, floatBytes, 0, 4);

            // 处理字节序 (仿照你原代码中的汇川大端序处理逻辑)
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(floatBytes);
            }

            // 针对某些特定PLC可能需要交换字序 (Word Swap)，视实际情况保留或注释
            var tempAB = floatBytes[0];
            floatBytes[0] = floatBytes[1];
            floatBytes[1] = tempAB;
            Array.Reverse(floatBytes);

            return BitConverter.ToSingle(floatBytes, 0);
        }
        #endregion

        #region 业务层：通用原始字节发送 (透传)
        /// <summary>
        /// 发送一段原始的 byte[] 数据并接收返回值
        /// 注意：此方法会自动将 byte[] 转换为 ASCII Hex 格式发送，并将响应解析回 byte[]
        /// </summary>
        /// <param name="rawData">需要发送的原始二进制数据</param>
        /// <returns>从站返回的原始二进制数据（已去除LRC校验）</returns>
        public async Task<byte[]> SendRawBytesAsync(byte[] rawData)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus未连接");

            if (rawData == null || rawData.Length == 0)
                throw new ArgumentException("发送的原始数据不能为空");

            // 1. 计算 LRC 校验和
            byte lrc = CalculateLrc(rawData, rawData.Length);

            // 2. 将原始 byte[] 拼接 LRC 后，转换为 ASCII Hex 字符串
            StringBuilder sb = new StringBuilder(":");
            foreach (byte b in rawData)
            {
                sb.Append(b.ToString("X2"));
            }
            sb.Append(lrc.ToString("X2"));
            sb.Append("\r\n");

            // 3. 调用底层方法发送并接收 ASCII 响应
            string asciiResponse = await SendAndReceiveAsciiAsync(sb.ToString());

            // 4. 解析响应为 byte[] 并返回
            return ParseAsciiResponse(asciiResponse);
        }
        #endregion












        #region 业务电子秤
        public Lock _lockObj;
        public List<string> SendCommand000()
        {
            //如果光源控制器已连接
            if (IsConnected)
            {

                byte[] byteSendCmd = new byte[4] { 0x30, 0x30, 0x31, 0x54 };
                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        //_serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        Thread.Sleep(50);
                        string saa = "";
                        List<string> strRes = ReadSerialPortResponse(out saa); //读取串口返回值
                        return strRes;
                        //if (!string.IsNullOrEmpty(strRes))
                        //{
                        //    return true; //收到返回值不为空，设置成功
                        //}
                    }
                    catch (Exception ex)
                    {
                        //LocalLogSave.WriteErrorLog("等离子清洗机 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return null;
        }

        public List<string> SendCommand3(out string jkjk)
        {
            //如果光源控制器已连接
            if (IsConnected)
            {

                byte[] byteSendCmd = new byte[4] { 0x30, 0x30, 0x31, 0x3F };

                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        //_serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        Thread.Sleep(50);
                        string saa = "";
                        List<string> strRes = ReadSerialPortResponse(out saa); //读取串口返回值
                        jkjk = saa;

                        return strRes;
                        //if (!string.IsNullOrEmpty(strRes))
                        //{
                        //    return true; //收到返回值不为空，设置成功
                        //}
                    }
                    catch (Exception ex)
                    {
                        jkjk = "";
                //        LocalLogSave.WriteErrorLog("等离子清洗机 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)
            else
            {
                jkjk = "";
            }

            return null;
        }



        // 私有方法：读串口返回数据。光源返回的数据无换行符，因此读取固定长度 100
        private List<string> ReadSerialPortResponse(out string wtData)
        {
            List<string> res = new List<string>();
            try
            {
                byte[] buffer = new byte[200]; //光源返回的数据无换行符，因此改为读取固定长度 100
                int length = _serialPort.Read(buffer, 0, 200); //返回有效长度
                for (int i = 0; i < length; i++)
                {
                    //strSendCmd += Convert.ToString(byteSendCmd[i], 16);
                    res.Add(String.Format("{0:X2}",/*(int) */buffer[i]));
                    //res += String.Format("{0:X2}",/*(int) */buffer[i]);
                }


                string asciiString = ConvertHexListToString(res);
                wtData = ExtractWTDataBySplit(asciiString);

                Console.WriteLine(wtData);
              

                int aa = 0;

            }
            catch (Exception ex)
            {
               // LocalLogSave.WriteErrorLog("光源控制器 [" + DeviceName + "] 读取串口返回数据失败！\r\n异常信息:\r\n" + ex.ToString());
                res = null;
                wtData = null;

            }
            finally
            {
                //即使在 try、catch 中 return，依旧会执行 finally 中的语句
                _serialPort.DiscardInBuffer();  // 读取完毕，清空接收缓存区

            }

            return res;
        }

        public static string ConvertHexListToString(List<string> hexList)
        {
            byte[] bytes = new byte[hexList.Count];

            for (int i = 0; i < hexList.Count; i++)
            {
                bytes[i] = Convert.ToByte(hexList[i], 16);
            }

            return Encoding.ASCII.GetString(bytes);
        }

        public static string ExtractWTDataBySplit(string input)
        {
            // 按行分割
            string[] lines = input.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("WT:"))
                {
                    // 返回 WT: 后面的内容，并去除首尾空格
                    return line.Substring(3).Trim();
                }
            }

            return null;
        }


        #endregion
    }
}