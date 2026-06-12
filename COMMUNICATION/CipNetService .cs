using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZANE
{
    public class CipNetService : IDisposable
    {
        //暂时只开放ip，默认端口44818

        #region 构造函数
        /// <summary>
        /// 初始化 CIP 网络服务
        /// </summary>
        /// <param name="ip">PLC IP地址</param>
        /// <param name="port">端口号 (CIP默认 44818)</param>
        public CipNetService(string ip, int port = 44818)
        {
            _ip = ip;
            _port = port;
        }
        #endregion

        #region 属性和字段
        private readonly object _rwLock = new object();
        private const int BufferSize = 1024; // CIP响应报文通常较长，需加大缓冲

        private TcpClient _client;
        private NetworkStream _stream;
        private readonly string _ip;
        private readonly int _port;
        private bool _isConnected = false;

        // CIP 核心标识符
        private uint _sessionHandle = 0;
        private uint _targetConnectionId = 0;
        private uint _originatorConnectionId = 0;
        private ushort _sequenceCounter = 0;

        public bool IsConnected => _isConnected && _client?.Connected == true;

        // 异步锁，保证同一时间只有一个读写操作
        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);
        #endregion

        #region 打开关闭方法
        public void Close()
        {
            lock (_rwLock)
            {
                try
                {
                    if (_stream != null || _client != null)
                    {
                        // 发送注销会话报文释放资源
                        if (_sessionHandle > 0)
                        {
                            byte[] unregPacket = BuildUnregisterSessionPacket(_sessionHandle);
                            _stream?.Write(unregPacket, 0, unregPacket.Length);
                        }
                        _stream?.Close();
                        _client?.Close();
                    }
                }
                catch { }
                finally
                {
                    _isConnected = false;
                    _sessionHandle = 0;
                }
            }
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// 打开 CIP 连接（包含 TCP握手 + 注册会话 + ForwardOpen）
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            return await Task.Run(async () =>
            {
                try
                {
                    lock (_rwLock)
                    {
                        Close(); // 确保先关闭旧连接
                        _client = new TcpClient();
                    }

                    await _client.ConnectAsync(_ip, _port);
                    _stream = _client.GetStream();

                    // 1. 发送 Register Session 请求 (命令码 0x65)
                    byte[] regRequest = BuildRegisterSessionPacket();
                    byte[] regResponse = await SendAndReceiveRawAsync(regRequest);

                    // 解析会话句柄 (Session Handle 位于报文第4-7字节)
                    _sessionHandle = BitConverter.ToUInt32(regResponse, 4);
                    if (_sessionHandle == 0) throw new Exception("CIP 注册会话失败，返回无效句柄");

                    // 2. 执行 Forward Open 建立通信通道 (服务码 0x54)
                    if (!await ForwardOpenAsync())
                    {
                        throw new Exception("CIP Forward Open 建立通道失败");
                    }

                    _isConnected = true;
                    return true;
                }
                catch (Exception ex)
                {
                    _isConnected = false;
                    throw new Exception($"CIP 连接失败: {ex.Message}");
                }
            });
        }
        #endregion

        #region 核心底层：CIP 报文构建
        // 注册会话报文 (固定格式)
        private byte[] BuildRegisterSessionPacket()
        {
            byte[] packet = new byte[28];
            BitConverter.GetBytes((ushort)0x65).CopyTo(packet, 0); // Command: Register Session
            BitConverter.GetBytes((ushort)0x04).CopyTo(packet, 2); // Length
            packet[24] = 0x01; packet[25] = 0x00; packet[26] = 0x00; packet[27] = 0x00; // Protocol Version & Options
            return packet;
        }

        // 注销会话报文
        private byte[] BuildUnregisterSessionPacket(uint sessionHandle)
        {
            byte[] packet = new byte[24];
            BitConverter.GetBytes((ushort)0x66).CopyTo(packet, 0); // Command: Unregister Session
            BitConverter.GetBytes(sessionHandle).CopyTo(packet, 4);
            return packet;
        }

        // Forward Open 请求构造
        private byte[] BuildForwardOpenRequest()
        {
            List<byte> cipData = new List<byte>();
            cipData.Add(0x54); // Service: Forward Open
            cipData.Add(0x02); // Request Path Size
            cipData.AddRange(new byte[] { 0x20, 0x06, 0x24, 0x01 }); // Path: Class 0x06, Instance 0x01

            // Connection Parameters (简化版)
            cipData.AddRange(new byte[] {
                0x0A, 0xF0, 0x00, 0x00, // O->T Network Connection ID (随机)
                0x00, 0x00, 0x00, 0x00, // T->O Network Connection ID (由PLC分配)
                0x01, 0x00, 0x00, 0x00, // Originator Serial Number
                0x0A, 0xF0,             // Timeout
                0x00, 0x00, 0x00, 0x00  // Reserved
            });

            return WrapInSendUnitData(cipData.ToArray());
        }

        // 将 CIP 数据封装在 Send Unit Data (命令码 0x6F) 中
        private byte[] WrapInSendUnitData(byte[] cipMessage)
        {
            List<byte> packet = new List<byte>();
            packet.AddRange(BitConverter.GetBytes((ushort)0x6F)); // Command: Send Unit Data
            packet.AddRange(BitConverter.GetBytes((ushort)(cipMessage.Length + 24))); // Length
            packet.AddRange(BitConverter.GetBytes(_sessionHandle));

            // Interface Handle, Timeout, Item Count... (标准封装头)
            packet.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00 });
            packet.AddRange(BitConverter.GetBytes((ushort)0x0000)); // Null Item
            packet.AddRange(BitConverter.GetBytes((ushort)0x0000));
            packet.AddRange(BitConverter.GetBytes((ushort)0xB202)); // Unconnected Data Item
            packet.AddRange(BitConverter.GetBytes((ushort)cipMessage.Length));
            packet.AddRange(cipMessage);

            return packet.ToArray();
        }

        // 构建 Read Tag 请求 (服务码 0x4C)
        private byte[] BuildReadTagRequest(string tagName, ushort elementCount = 1)
        {
            List<byte> cipData = new List<byte>();
            cipData.Add(0x4C); // Service: Read Tag

            // 符号路径 (Symbolic Segment)
            byte[] tagBytes = Encoding.UTF8.GetBytes(tagName);
            if (tagBytes.Length % 2 != 0) Array.Resize(ref tagBytes, tagBytes.Length + 1); // CIP要求偶数长度

            cipData.Add((byte)((tagBytes.Length / 2) | 0x90)); // Symbolic Segment Header
            cipData.AddRange(tagBytes);
            cipData.AddRange(BitConverter.GetBytes(elementCount));

            return WrapInSendUnitData(cipData.ToArray());
        }
        #endregion

        #region 核心：带拆包的发送+接收（通用底层方法）
        /// <summary>
        /// 发送请求 + 循环接收完整 CIP/TCP 报文（解决半包/粘包）
        /// </summary>
        private async Task<byte[]> SendAndReceiveRawAsync(byte[] request, CancellationToken ct = default)
        {
            if (!IsConnected && _sessionHandle == 0) // 允许在Connect阶段调用
                throw new InvalidOperationException("CIP 未连接");

            await _asyncLock.WaitAsync(ct);
            try
            {
                byte[] recvBuffer = new byte[BufferSize];
                byte[] fullPacket = new byte[0];
                int totalReceived = 0;

                await _stream.WriteAsync(request, 0, request.Length, ct);

                while (!ct.IsCancellationRequested)
                {
                    int readLen = await _stream.ReadAsync(recvBuffer, totalReceived, BufferSize - totalReceived, ct);
                    if (readLen == 0) { Close(); throw new Exception("PLC 连接断开"); }

                    totalReceived += readLen;

                    // CIP Encapsulation Header 前8字节包含总长度信息
                    if (totalReceived >= 8)
                    {
                        // 长度字段在第2-3字节 (Length of encapsulation data)
                        ushort pktLen = BitConverter.ToUInt16(recvBuffer, 2);
                        int fullLen = 8 + pktLen;

                        if (totalReceived >= fullLen)
                        {
                            fullPacket = new byte[fullLen];
                            Array.Copy(recvBuffer, 0, fullPacket, 0, fullLen);
                            break;
                        }
                    }
                    await Task.Delay(5, ct);
                }
                return fullPacket;
            }
            finally { _asyncLock.Release(); }
        }

        // 执行 Forward Open 并解析响应
        private async Task<bool> ForwardOpenAsync()
        {
            byte[] foRequest = BuildForwardOpenRequest();
            byte[] foResponse = await SendAndReceiveRawAsync(foRequest);

            // 提取 CIP 状态码 (通常在报文的倒数部分，这里做基础校验)
            // 如果响应正常，提取 Connection ID 供后续通信使用
            // 真实项目中需严格解析 CIP 响应结构，此处简化为判断长度和关键位置
            if (foResponse.Length > 40)
            {
                _targetConnectionId = BitConverter.ToUInt32(foResponse, foResponse.Length - 12); // 粗略提取
                return true;
            }
            return false;
        }
        #endregion

        #region 业务层：读取标签
        public async Task<short> ReadIntTagAsync(string tagName)
        {
            if (!IsConnected) throw new InvalidOperationException("CIP 未连接");

            byte[] request = BuildReadTagRequest(tagName, 1);
            byte[] response = await SendAndReceiveRawAsync(request);

            // CIP Read Tag 响应: [Service(0xCC)][Status][ExtStatus][DataType][Data...]
            // 寻找数据起始位置 (简化解析逻辑)
            if (response.Length < 30) throw new Exception("CIP 响应数据长度不足");

            // 提取数据类型和数据值 (假设 INT 类型, 占2字节)
            // 实际开发中需根据 DataType 动态解析
            short value = BitConverter.ToInt16(response, response.Length - 4);
            return value;
        }

        public async Task<float> ReadFloatTagAsync(string tagName)
        {
            if (!IsConnected) throw new InvalidOperationException("CIP 未连接");

            byte[] request = BuildReadTagRequest(tagName, 1);
            byte[] response = await SendAndReceiveRawAsync(request);

            if (response.Length < 32) throw new Exception("CIP Float 响应数据长度不足");

            byte[] floatBytes = new byte[4];
            Array.Copy(response, response.Length - 6, floatBytes, 0, 4);

            // CIP 默认小端序，Windows也是小端序，通常无需反转
            return BitConverter.ToSingle(floatBytes, 0);
        }
        #endregion
    }
}