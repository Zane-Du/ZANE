using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core {
    /// <summary>
    /// 具体设备类：欧姆龙 PLC，使用串口 HostLink 协议通讯，可读写信号点（bool）、整数（ushort、short）、字符串。
    /// 可配置的属性包括：串口连接参数、心跳时间、信号字典：《信号名, 信号地址》
    /// </summary>
    public class PLCOmronHostLink : AbstractDevice {

        #region 1. 字段、普通属性

        // 字段：串口，用于通讯。默认参数为 115200、8、1、N；读取超时 500ms
        private SerialPort _serialPort = new SerialPort() {
            PortName = "COM1",
            BaudRate = 115200,
            DataBits = 8,
            StopBits = StopBits.One,
            Parity = Parity.None,
            ReadTimeout = 500
        };

        // 字段：计时器。当在运行状态时，定时向 PLC 写入心跳信号。默认计时间隔为 4 秒        
        private static System.Timers.Timer _timer = new System.Timers.Timer() { Interval = 4000 };

        // 字段：用于读写串口时，锁线程
        private object _lockObj = new object(); 

        /// <summary>
        /// 重写父类的属性：PLC 的串口是否连接且打开
        /// </summary>
        public override bool IsConnected {
            get {
                return _serialPort.IsOpen;
            }
        }


        /// <summary>
        /// 重写父类的属性：PLC 是否正在运行：定时发送心跳信号
        /// </summary>
        public override bool IsRunning {
            get {
                return _timer.Enabled;
            }
        }


        #endregion


        #region 2. 构造函数

        /// <summary>
        /// 带参实例构造函数：提供 PLC 的设备名，加载并设置参数，打开串口；初始化计时器事件
        /// </summary>
        /// <param name="deviceName">PLC 的设备名</param>
        public PLCOmronHostLink(string deviceName) : base(deviceName) {

            OpenPLC(); //加载并设置参数，打开串口

            //初始化计时结束的事件处理器
            _timer.Elapsed += new System.Timers.ElapsedEventHandler((obj, e) => {
                if (IsConnected && IsRunning) { //串口已连接，并且正在运行中
                    WriteBoolean("心跳信号", true); //向心跳信号地址写入 true
                }
            });
        }

        #endregion


        #region 3. 可配置的属性

        /// <summary>
        /// 可配置的属性：串口的端口名，默认值为 COM1
        /// </summary>
        [IniConfig]
        public string 端口名 {
            get { return _serialPort.PortName; }
            set {
                if (value.StartsWith("COM")) { //设置端口名，以 COM 开头
                    _serialPort.PortName = value;
                }
            }
        }


        /// <summary>
        /// 可配置的属性：串口的波特率。使用可选项类型 BaudrateOptions，可选值包括：9600、19200、38400、57600、115200。
        /// 默认值为 115200
        /// </summary>
        [IniConfig]
        public BaudrateOptions 波特率 {
            get { return _baudrate; }
            set {
                _baudrate = value;
                _serialPort.BaudRate = Convert.ToInt32(_baudrate.Text); //根据 Text 属性设置波特率
            }
        }
        private BaudrateOptions _baudrate = new BaudrateOptions() { Text = "115200" };


        /// <summary>
        /// 可配置的属性：串口的数据位，可选值包括：6、7、8。默认值为 8
        /// </summary>
        [IniConfig]
        public int 数据位 {
            get { return _serialPort.DataBits; }
            set {
                if (value > 5 && value < 9) { //限定数据位范围：6，7，8
                    _serialPort.DataBits = value; //设置数据位
                }
            }
        }


        /// <summary>
        /// 可配置的属性：串口的停止位，默认值为 One
        /// </summary>
        [IniConfig]
        public StopBits 停止位 {
            get { return _serialPort.StopBits; }
            set { _serialPort.StopBits = value; }
        }


        /// <summary>
        /// 可配置的属性：串口的校验位，默认值为 None
        /// </summary>
        [IniConfig]
        public Parity 校验位 {
            get { return _serialPort.Parity; }
            set { _serialPort.Parity = value; }
        }


        /// <summary>
        /// 可配置的属性：串口读取超时。读取等待超过此时间，视为未接收到数据。单位 ms，默认值为 500
        /// </summary>
        [IniConfig]
        public int 读取超时 {
            get { return _serialPort.ReadTimeout; }
            set { _serialPort.ReadTimeout = value; }
        }


        /// <summary>
        /// 可配置的属性：串口在读取字符串时的最大长度。默认值为 100
        /// </summary>
        [IniConfig]
        public int 读取字符串长度 { get; set; } = 100;


        /// <summary>
        /// 可配置的属性：心跳时间，在运行状态时，定时写入心跳信号，单位 ms。默认值为 4000
        /// </summary>
        [IniConfig]
        public int 心跳时间 {
            get { return (int)_timer.Interval; }
            set { _timer.Interval = value; }
        }


        /// <summary>
        /// 可配置的属性：信号字典：《信号名, 信号地址》。用于读写 PLC 信号和数据。
        /// 默认添加心跳信号，地址为 9700
        /// </summary>
        [IniConfig]
        public Dictionary<string, int> 信号字典 { get; set; } = new Dictionary<string, int>();

        #endregion


        #region 4. 主要功能：重写 Open / Close：打开、关闭串口

        /// <summary>
        /// 主要功能：重写父类方法：加载配置，并打开串口
        /// </summary>
        public override void Open() {
            //开串口
            if (!IsConnected) {
                OpenPLC();
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：停止心跳信号，并关闭串口
        /// </summary>
        public override void Close() {
            //停止运行：关定时器
            if (IsRunning) {
                StopRunning();
            }

            //关串口
            if (IsConnected) {
                ClosePLC();
            }
        }


        // 私有方法：加载并设置参数，打开串口
        private void OpenPLC() {
            try {
                //串口关闭时，才能修改参数
                if (!_serialPort.IsOpen) {
                    //设置参数
                    LoadConfigs(); //读取本地配置，设置参数：串口参数、心跳时间、信号字典

                    //信号字典默认添加心跳信号：9700
                    if (!信号字典.ContainsKey("心跳信号")) {
                        信号字典.Add("心跳信号", 9700);
                    }

                    _serialPort.Open(); //打开串口 

                    //连接后，保存参数
                    SaveConfigs(); //保存本地配置
                }
            }
            catch (Exception ex) {
                ShowException("打开 PLC 串口失败！", ex);
            }
        }


        // 私有方法：关闭串口，释放相关资源
        private void ClosePLC() {
            try {
                _serialPort.Close(); //关闭串口
                _serialPort.Dispose(); //释放串口资源
            }
            catch (Exception ex) {
                ShowException("关闭 PLC 串口失败！", ex);
            }
        }

        #endregion


        #region 4. 主要功能：重写 Start / StopRunning：发送、停止心跳信号

        /// <summary>
        /// 主要功能：重写父类方法：开始运行：开定时器，发送心跳信号
        /// </summary>
        public override void StartRunning() {
            //打开串口
            if (!IsConnected) {
                Open();
            }

            //开定时器，发送心跳信号
            if (!IsRunning) {
                _timer.Enabled = true;
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：停止运行：关定时器，停止心跳信号
        /// </summary>
        public override void StopRunning() {
            if (IsRunning) {
                _timer.Enabled = false; //关定时器
            }
        }
        #endregion


        #region 4. 主要功能：按照名称，读写变量：bool, ushort, short, string

        /// <summary>
        /// 主要功能：方法：从 PLC 指定名称的地址读取信号点：true 表示 ON，false 表示 OFF
        /// </summary>
        /// <param name="addressName">读取信号的名称</param>
        /// <returns>读取到的值。如果读取发生异常，返回 false</returns>
        public bool ReadBoolean(string addressName) {
            return ReadBoolean(信号字典[addressName]);
        }


        /// <summary>
        /// 主要功能：方法：向 PLC 指定名称的地址写入信号点：true 表示 ON，false 表示 OFF
        /// </summary>
        /// <param name="addressName">写入信号的名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功。如果写入成功，返回 true</returns>
        public bool WriteBoolean(string addressName, bool value) {
            return WriteBoolean(信号字典[addressName], value);
        }


        /// <summary>
        /// 主要功能：方法：从 PLC 指定名称的 DM 区读取 16 位数据，返回 ushort 类型（UInt16）
        /// </summary>
        /// <param name="address">读取数据的 DM 区名称</param>
        /// <returns>读取到的值。如果读取发生异常，返回 0</returns>
        public ushort ReadUInt16(string addressName) {
            return ReadUInt16(信号字典[addressName]);
        }


        /// <summary>
        /// 主要功能：方法：向 PLC 指定名称的 DM 区写入 16 位数据：ushort 类型（UInt16）
        /// </summary>
        /// <param name="address">写入数据的 DM 区名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功。如果写入成功，返回 true</returns>
        public bool WriteUInt16(string addressName, ushort value) {
            return WriteUInt16(信号字典[addressName], value);
        }


        /// <summary>
        /// 主要功能：方法：从 PLC 指定名称的 DM 区读取 16 位数据，返回 short 类型（Int16）
        /// </summary>
        /// <param name="address">读取数据的 DM 区名称</param>
        /// <returns>读取到的值。如果读取发生异常，返回 0</returns>
        public short ReadInt16(string addressName) {
            return ReadInt16(信号字典[addressName]);
        }


        /// <summary>
        /// 主要功能：方法：向 PLC 指定名称的 DM 区写入16位数据：short 类型（Int16）
        /// </summary>
        /// <param name="address">写入数据的 DM 区名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功。如果写入成功，返回 true</returns>
        public bool WriteInt16(string addressName, short value) {
            return WriteInt16(信号字典[addressName], value);
        }


        /// <summary>
        /// 主要功能：方法：从 PLC 指定名称的 DM 区开始读取多个 16 位数据，并转换为字符串。
        /// 读取 DM 区的数量，由可配置的参数 [读取字符串长度] 设置。
        /// </summary>
        /// <param name="address">读取数据的起始 DM 区名称</param>
        /// <returns>读取到的字符串。如果读取发生异常，返回空字符串</returns>
        public string ReadString(string addressName) {
            return ReadString(信号字典[addressName]);
        }


        /// <summary>
        /// 主要功能：方法：将字符串转换为多个 16 位数据，并写入到 PLC 指定名称的 DM 区。
        /// </summary>
        /// <param name="address">写入字符串的起始 DM 区名称</param>
        /// <param name="value">要写入的字符串</param>
        /// <returns>是否写入成功。如果写入成功，返回 true</returns>
        public bool WriteString(string addressName, string value) {
            return WriteString(信号字典[addressName], value);
        }

        #endregion


        #region 4. 功能：按照信号地址，读写变量：bool, ushort, short, string

        /// <summary>
        /// 功能：方法：从 PLC 指定地址读取信号点：true 表示 ON，false 表示 OFF
        /// </summary>
        /// <param name="address">读取信号的地址</param>
        /// <returns>读取到的值。如果读取发生异常，返回 false</returns>
        public bool ReadBoolean(int address) {

            //如果PLC已连接
            if (IsConnected) {
                
                string cmd = MakeCioBitReadCmd(address / 100, address % 100); //创建读指令，前两位通道号，后两位地址

                lock (_lockObj) {
                    try {
                        _serialPort.Write(cmd); //发送串口命令
                        string strData = ReadSerialPortResponse(); //读取串口返回值
                        if (strData != null && strData.Length > 24) {
                            return (strData[24] == '1'); //判断索引 = 24 的字符是否为1
                        }
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取 CIO 位 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return false;
        }


        /// <summary>
        /// 功能：方法：向 PLC 指定地址写入信号点：true 表示 ON，false 表示 OFF
        /// </summary>
        /// <param name="address">写入信号的地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功。如果写入成功，返回 true</returns>
        public bool WriteBoolean(int address, bool value) {

            //如果PLC已连接
            if (IsConnected) {
                
                string cmd = MakeCioBitWriteCmd(address / 100, address % 100, value); //创建写指令，前两位通道号，后两位地址

                lock (_lockObj) {
                    try {
                        _serialPort.Write(cmd); //发送串口命令
                        string strData = ReadSerialPortResponse(); //读取串口返回值  
                        return (strData != null); //收到的响应不为空，视为写入成功
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 写入 CIO 位 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return false;
        }


        /// <summary>
        /// 功能：方法：从 PLC 指定 DM 区读取 16 位数据，返回 ushort 类型（UInt16）
        /// </summary>
        /// <param name="address">读取数据的地址</param>
        /// <returns>读取到的值。如果读取发生异常，返回 0</returns>
        public ushort ReadUInt16(int address) {

            //如果PLC已连接
            if (IsConnected) {

                string cmd = MakeDMWordReadCmd(address); //创建读取 DM 区指令

                lock (_lockObj) {
                    try {
                        _serialPort.Write(cmd); //发送串口命令
                        string strData = ReadSerialPortResponse(); //读取串口返回值
                        if (strData != null && strData.Length > 26
                            && strData.StartsWith("@") && strData.EndsWith("*")) { //返回字符串以 @开头，*结尾
                                                                                   //截取有效字符，从 16 进制转换为 UInt16 返回
                            string strValid = strData.Substring(23, 4);
                            return Convert.ToUInt16(strValid, 16);
                        }
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取 DM 区 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return 0;
        }


        /// <summary>
        /// 功能：方法：向 PLC 指定 DM 区写入16位数据：ushort 类型（UInt16）
        /// </summary>
        /// <param name="address">写入数据的地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功。如果写入成功，返回 true</returns>
        public bool WriteUInt16(int address, ushort value) {

            //如果PLC已连接
            if (IsConnected) {

                string cmd = MakeDMWordWriteCmd(address, value); //创建写入 DM 区指令

                lock (_lockObj) {
                    try {
                        _serialPort.Write(cmd); //发送串口命令
                        string strData = ReadSerialPortResponse(); //读取串口返回值
                        if (strData != null
                            && strData.StartsWith("@") && strData.EndsWith("*")) { //返回字符串以 @开头，*结尾
                                                                                   //收到的响应不为空，视为写入成功
                            return true;
                        }
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取 DM 区 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return false;
        }


        /// <summary>
        /// 功能：方法：从 PLC 指定 DM 区读取 16 位数据，返回 short 类型（Int16）
        /// </summary>
        /// <param name="address">读取数据的地址</param>
        /// <returns>读取到的值。如果读取发生异常，返回 0</returns>
        public short ReadInt16(int address) {

            //如果PLC已连接
            if (IsConnected) {

                string cmd = MakeDMWordReadCmd(address); //创建读取 DM 区指令
                lock (_lockObj) {
                    try {
                        _serialPort.Write(cmd); //发送串口命令
                        string strData = ReadSerialPortResponse(); //读取串口返回值
                        if (strData != null && strData.Length > 26
                            && strData.StartsWith("@") && strData.EndsWith("*")) { //返回字符串以 @开头，*结尾
                                                                                   //截取有效字符，从 16 进制转换为 Int16 返回
                            string strValid = strData.Substring(23, 4);
                            return Convert.ToInt16(strValid, 16);
                        }
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取 DM 区 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return 0;
        }


        /// <summary>
        /// 功能：方法：向 PLC 指定 DM 区写入16位数据：short 类型（Int16）
        /// </summary>
        /// <param name="address">写入数据的地址</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功。如果写入成功，返回 true</returns>
        public bool WriteInt16(int address, short value) {

            //如果PLC已连接
            if (IsConnected) {

                string cmd = MakeDMWordWriteCmd(address, value); //创建写入 DM 区指令

                lock (_lockObj) {
                    try {
                        _serialPort.Write(cmd); //发送串口命令
                        string strData = ReadSerialPortResponse(); //读取串口返回值
                        if (strData != null
                            && strData.StartsWith("@") && strData.EndsWith("*")) { //返回字符串以 @开头，*结尾
                                                                                   //收到的响应不为空，视为写入成功
                            return true;
                        }
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取 DM 区 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return false;
        }


        /// <summary>
        /// 功能：方法：从 PLC 指定 DM 区开始读取多个 16 位数据，并转换为字符串。
        /// 读取 DM 区的数量，由可配置的参数 [读取字符串长度] 设置。
        /// </summary>
        /// <param name="address">读取数据的起始地址</param>
        /// <returns>读取到的字符串。如果读取发生异常，返回空字符串</returns>
        public string ReadString(int address) {

            //如果PLC已连接
            if (IsConnected) {

                string cmd = MakeDMStringReadCmd(address, 读取字符串长度); //创建读取 DM 区字符串指令

                lock (_lockObj) {
                    try {
                        _serialPort.Write(cmd); //发送串口命令
                        string strData = ReadSerialPortResponse(); //读取串口返回值
                        if (strData != null && strData.Length > 26
                            && strData.StartsWith("@") && strData.EndsWith("*")) { //返回字符串以 @开头，*结尾

                            string strValid = strData.Substring(23, 2 * 读取字符串长度); //截取出有效字符，长度是字符串长度的两倍

                            //转换过程：1个字16位，返回4个字符 "3132" → 分成两个16进制数：0x31，0x32 
                            //→ 转换成ASCII码（byte[]数组）：49，50 → 转换成字符：'1'，'2'
                            List<byte> listBytes = new List<byte>();
                            for (int i = 0; i < 读取字符串长度; i++) {
                                string temp = strValid.Substring(i * 2, 2);
                                byte b = Convert.ToByte(temp, 16); //两个返回的字符，转换成一个16进制数
                                if (b >= 32) { //仅保留可显示的字符
                                    listBytes.Add(b);
                                }
                            }

                            //将byte[]数组转换为string
                            string strRes = Encoding.ASCII.GetString(listBytes.ToArray()).Trim(); //去除空格
                            return strRes;
                        }
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取 DM 区 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return "";
        }


        /// <summary>
        /// 功能：方法：将字符串转换为多个 16 位数据，并写入到 PLC 指定 DM 区
        /// </summary>
        /// <param name="address">写入字符串的起始地址</param>
        /// <param name="value">要写入的字符串</param>
        /// <returns>是否写入成功。如果写入成功，返回 true</returns>
        public bool WriteString(int address, string value) {

            //如果PLC已连接
            if (IsConnected) {

                string cmd = MakeDMStringWriteCmd(address, value); //创建写入 DM 区字符串指令

                lock (_lockObj) {
                    try {
                        _serialPort.Write(cmd); //发送串口命令
                        string strRes = ReadSerialPortResponse(); //读取串口返回值
                        if (strRes != null
                            && strRes.StartsWith("@") && strRes.EndsWith("*")) { //返回字符串以 @开头，*结尾
                                                                                 //收到的响应不为空，视为写入成功
                            return true;
                        }
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 写入 DM 区 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return false;
        }

        #endregion


        #region 6. 私有方法：构建读写命令

        // 私有方法：构建 CIO 位读取命令
        private string MakeCioBitReadCmd(int channel, int bit) {
            //Hostlink协议起始代码  Hostlink单元号 头代码 响应等待时间   固定     写  CIO区 
            //         @                00           FA        0       00000000  0102  30  
            string str_comm = "@00FA00000000001013000";
            str_comm += String.Format("{0:X2}", channel);  //00+nAddress 通道
            str_comm += String.Format("{0:X2}", bit);      //nBit  开始位
            str_comm += "0001";                             //写一个位
            str_comm += MakeCheckSum(str_comm);             //计算校验和
            str_comm += "*\r";                              //结束符
            return str_comm;
        }


        // 私有方法：构建 CIO 位写入命令
        private string MakeCioBitWriteCmd(int channel, int bit, bool value) {
            //Hostlink协议起始代码  Hostlink单元号 头代码 响应等待时间   固定     写  CIO区 
            //         @                00           FA        0       00000000  0102  30  
            string str_comm = "@00FA00000000001023000";
            str_comm += String.Format("{0:X2}", channel);  //00+nAddress 通道
            str_comm += String.Format("{0:X2}", bit);      //nBit  开始位
            str_comm += "0001";                             //写一个位
            str_comm += value ? "01" : "00";                 //01置ON  00置OFF
            str_comm += MakeCheckSum(str_comm);             //计算校验和
            str_comm += "*\r";                              //结束符
            return str_comm;
        }


        // 私有方法：构建 DM 区读取一个字的命令
        private string MakeDMWordReadCmd(int address) {
            string strCmd = "@00FA000000000010182";
            string strAddress = String.Format("{0:X4}", address);
            strCmd += strAddress;
            strCmd += "00";         //地址第三个字节为0
            strCmd += "0001";       //读取 DM 区字数：一个字，对应16位
            strCmd += MakeCheckSum(strCmd);
            strCmd += "*\r";
            return strCmd;
        }


        // 私有方法：构建 DM 区写入一个字的命令，写入类型为 ushort（UInt16）
        private string MakeDMWordWriteCmd(int address, ushort value) {
            string strCmd = "@00FA000000000010282";
            string strAddress = String.Format("{0:X4}", address);
            strCmd += strAddress;
            strCmd += "00";         //地址第三个字节为0
            strCmd += "0001";       //写入 DM 区字数：一个字，对应16位
            strCmd += String.Format("{0:X4}", value); //将写入的值，转换为4位16进制数
            strCmd += MakeCheckSum(strCmd);
            strCmd += "*\r";
            return strCmd;
        }


        // 私有方法：构建 DM 区写入一个字的命令，写入类型为 short（Int16）
        private string MakeDMWordWriteCmd(int address, short value) {
            string strCmd = "@00FA000000000010282";
            string strAddress = String.Format("{0:X4}", address);
            strCmd += strAddress;
            strCmd += "00";         //地址第三个字节为0
            strCmd += "0001";       //写入 DM 区字数：一个字，对应16位
            strCmd += String.Format("{0:X4}", value); //将写入的值，转换为4位16进制数
            strCmd += MakeCheckSum(strCmd);
            strCmd += "*\r";
            return strCmd;
        }


        // 私有方法：构建 DM 区读取字符串的命令
        private string MakeDMStringReadCmd(int address, int length) {
            //length：要读取的字符串长度
            //每个字（16位）可以存放两个 ASCII 字符，因此读取的 DM 字数量 count = 字符串长度 / 2
            double count = Math.Ceiling(length * 0.5); //向上取整

            string strCmd = "@00FA000000000010182";
            string strAddress = String.Format("{0:X4}", address);
            strCmd += strAddress;
            strCmd += "00";         //地址第三个字节为0
            strCmd += String.Format("{0:X4}", (int)count); //要读取的 DM 字数量
            strCmd += MakeCheckSum(strCmd);
            strCmd += "*\r";
            return strCmd;
        }


        // 私有方法：构建 DM 区写入字符串的命令
        private string MakeDMStringWriteCmd(int address, string value) {
            //字符串转 byte[] 数组
            byte[] array = Encoding.ASCII.GetBytes(value);
            List<byte> listBytes = new List<byte>(array);
            if (listBytes.Count() % 2 != 0) {  //保证 byte 为偶数个
                listBytes.Add(0);
            }

            string strCmd = "@00FA000000000010282";
            string strAddress = String.Format("{0:X4}", address);
            strCmd += strAddress;
            strCmd += "00";         //地址第三个字节为0
            strCmd += String.Format("{0:X4}", listBytes.Count() / 2); //要写入的 DM 字数量 = 字符串长度 / 2
            foreach (byte b in listBytes) {
                strCmd += String.Format("{0:X2}", b);  //每个 byte 转换为两个字符
            }
            strCmd += MakeCheckSum(strCmd);
            strCmd += "*\r";
            return strCmd;
        }


        // 私有方法：计算校验和字符串
        private string MakeCheckSum(string cmd) {
            byte[] byteArray = Encoding.Default.GetBytes(cmd);
            byte result = byteArray[0];

            for (int i = 1; i < byteArray.Length; i++) {
                result ^= byteArray[i];
            }
            return result.ToString("X2");
        }

        #endregion


        #region 6. 私有方法：读串口返回数据

        // 私有方法：读串口返回数据，到 \r 停止
        private string ReadSerialPortResponse() {
            string res = null;
            try {
                res = _serialPort.ReadTo("\r"); //读取串口返回值，到 \r 停止
            }
            catch (Exception ex) {
                LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取串口返回数据失败！\r\n异常信息:\r\n" + ex.ToString());
                res = null;
            }
            finally {
                //即使在 try、catch 中 return，依旧会执行 finally 中的语句
                _serialPort.DiscardInBuffer();  // 读取完毕，清空接受缓存区
            }

            return res;
        }

        #endregion
        
    }// class

}// namespace
