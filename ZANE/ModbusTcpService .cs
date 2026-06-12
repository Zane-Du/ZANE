

using System;
using System.Net.Sockets;
using System.Threading.Tasks;

#region ZANE

#endregion


namespace ZANE
{
    public class ModbusTcpService : IDisposable
    {
        #region 弃用代码
        /// <summary>
        /// 读取单个寄存器的值
        /// </summary>
        /// <param name="address">寄存器地址（如0x3001）</param>
        //public async Task<short> ReadRegisterAsync(ushort address)
        //{
        //    if (!IsConnected)
        //        throw new InvalidOperationException("Modbus未连接");

        //    // 构建读取报文
        //    byte[] request = BuildReadRequest(address);

        //    // 发送请求
        //    await _stream.WriteAsync(request, 0, request.Length);

        //    // 接收响应
        //    byte[] buffer = new byte[256];
        //    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

        //    if (bytesRead < 9)
        //        throw new Exception("响应数据长度不足");

        //    // 解析数值（第9、10字节是数据）
        //    short value = (short)((buffer[9] << 8) | buffer[10]);
        //    return value;
        //}

        #endregion

        #region 构造函数
        public ModbusTcpService(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        #endregion

        #region 属性和字段

        private readonly object _rwLock = new object();
        private const int BufferSize = 256;

        private TcpClient _client;
        private NetworkStream _stream;
        private readonly string _ip;
        private readonly int _port;
        private bool _isConnected = false;
        private int _transactionId = 1;

        public bool IsConnected => _isConnected && _client?.Connected == true;

        #endregion

        #region 打开关闭方法
        public void Close()
        {
            lock (_rwLock)
            {
                try
                {
                    _stream?.Close();
                    _client?.Close();
                }
                catch { }
                finally
                {
                    _isConnected = false;
                }
            }
        }

        public void Dispose()
        {
            Close();
        }

        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);




        /// <summary>
        /// 打开连接（软件启动时调用）
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(_ip, _port);

                _stream = _client.GetStream();
                _isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                _isConnected = false;
                throw new Exception($"连接失败: {ex.Message}");
            }
        }

        #endregion

        private byte[] BuildReadRequest(ushort address, ushort registerCount = 1)
        {
            byte[] request = new byte[12];

            // 事务标识符（每次递增）
            request[0] = (byte)(_transactionId >> 8);
            request[1] = (byte)(_transactionId++);

            // 协议标识符（固定0）
            request[2] = 0x00;
            request[3] = 0x00;

            // 长度（固定6）
            request[4] = 0x00;
            request[5] = 0x06;

            // 单元标识符（站号）
            request[6] = 0x01;

            // 功能码（03 = 读保持寄存器）
            request[7] = 0x03;

            // 寄存器起始地址
            request[8] = (byte)(address >> 8);   // 高位
            request[9] = (byte)(address & 0xFF); // 低位

            // 🔥 核心修改：使用传入的寄存器数量，而不是硬编码的 1
            request[10] = (byte)(registerCount >> 8);   // 数量高位
            request[11] = (byte)(registerCount & 0xFF); // 数量低位

            return request;
        }




        #region 核心：带拆包的发送+接收（通用底层方法）
        /// <summary>
        /// 发送请求 + 循环接收完整Modbus TCP报文（解决半包/粘包）
        /// </summary>
        private async Task<byte[]> SendAndReceiveAsync(byte[] request, CancellationToken ct = default)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus 连接已断开");

            await _asyncLock.WaitAsync(ct);
            try
            {

                byte[] recvBuffer = new byte[BufferSize];
                byte[] fullPacket = new byte[0];
                int totalReceived = 0;

                _stream.Write(request, 0, request.Length);


                // 循环接收，直到收完一整包
                while (!ct.IsCancellationRequested)
                {
                    int readLen;

                    readLen = _stream.Read(recvBuffer, totalReceived, BufferSize - totalReceived);

                    if (readLen == 0)
                    {
                        // 对方主动断开
                        Close();
                        throw new Exception("PLC 连接断开，读取数据为0");
                    }

                    totalReceived += readLen;

                    // Modbus TCP 前6字节：事务ID(2)+协议ID(2)+长度(2)
                    if (totalReceived >= 6)
                    {
                        ushort pktLen = (ushort)((recvBuffer[4] << 8) | recvBuffer[5]);
                        int fullLen = 6 + pktLen; // 整包总长度

                        if (totalReceived >= fullLen)
                        {
                            // 收齐完整报文
                            fullPacket = new byte[fullLen];
                            Array.Copy(recvBuffer, 0, fullPacket, 0, fullLen);
                            break;
                        }
                    }

                    await Task.Delay(5, ct); // 短暂等待继续收
                }
                // 校验事务ID（请求与响应一致）
                ushort reqTranId = (ushort)((request[0] << 8) | request[1]);
                ushort resTranId = (ushort)((fullPacket[0] << 8) | fullPacket[1]);
                if (reqTranId != resTranId)
                    throw new Exception($"事务ID不匹配，请求:{reqTranId} 响应:{resTranId}");

                return fullPacket;

            }


            finally { _asyncLock.Release(); }
        }
        #endregion


        public async Task<short> ReadIntRegisterAsync(ushort address)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Modbus未连接");

            // 构建读取报文（注意：读INT时，寄存器数量必须传 1）
            byte[] request = BuildReadRequest(address, registerCount: 1);





            //await _stream.WriteAsync(request, 0, request.Length);
            //byte[] buffer = new byte[256];
            //int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

            // Modbus TCP头(9字节) + 1个寄存器数据(2字节) = 至少11字节
            //if (bytesRead < 11)
            //    throw new Exception($"响应数据长度不足，预期至少11字节，实际收到{bytesRead}字节");





            byte[] buffer = await SendAndReceiveAsync(request);
            // 校验功能码 + 数据长度
            if (buffer[7] != 0x03)
                throw new Exception($"Modbus 异常码: {buffer[7]}");
            if (buffer[8] < 2)
                throw new Exception("寄存器数据长度不足");






            // 汇川默认大端序，直接拼接高低字节即可
            short value = (short)((buffer[9] << 8) | buffer[10]);

            return value;
        }


        public async Task<float> ReadFloatRegisterAsync(ushort address)
        {


            #region 这个里面的字节转换，就像是狗屎一样

            #endregion



            if (!IsConnected)
                throw new InvalidOperationException("Modbus未连接");

            // 构建读取报文（注意：读浮点数时，数量参数应传 2，因为一个 float 占 2 个寄存器）
            byte[] request = BuildReadRequest(address, 2);








            #region 原来的写法，弃用
            //// 发送请求
            //await _stream.WriteAsync(request, 0, request.Length);

            //// 接收响应
            //byte[] buffer = new byte[256];
            //int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

            //// 浮点数占 4 个字节，加上 Modbus TCP 头部和数据头，总长度至少为 13
            //if (true)
            //{
            //    //忽略总长度校验
            //    if (bytesRead < 13)
            //    {
            //        var jk = bytesRead;
            //        int aa = 0;

            //        throw new Exception($"响应数据长度不足，预期至少13字节，实际收到{bytesRead}字节");
            //    }

            //}
            #endregion


            byte[] buffer = await SendAndReceiveAsync(request);
            // 校验功能码 + 数据长度
            if (buffer[7] != 0x03)
                throw new Exception($"Modbus 异常码: {buffer[7]}");
            if (buffer[8] < 2)
                throw new Exception("寄存器数据长度不足");








            // 1. 提取连续的 4 个字节（索引 9, 10, 11, 12）
            byte[] floatBytes = new byte[4];
            Array.Copy(buffer, 9, floatBytes, 0, 4);

            // 2. 处理字节序（Endianness）
            // 你的原代码 (buffer[9] << 8) | buffer[10] 表明设备使用的是【大端序】
            // 但 x86/x64 Windows 系统的 BitConverter 默认使用【小端序】
            // 因此必须将字节数组反转，否则解析出来的值会完全错乱
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(floatBytes);
            }

            var tempAB = floatBytes[0];
            floatBytes[0] = floatBytes[1];
            floatBytes[1] = tempAB;


            var jkjk = floatBytes;

            Array.Reverse(floatBytes);

            // 3. 按照 IEEE 754 标准将 4 个字节转换为单精度浮点数 (float)
            float value = BitConverter.ToSingle(floatBytes, 0);

            return value;

        }





    }
}