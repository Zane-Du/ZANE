using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication.Profinet.Omron;

namespace Framework2Core {
    /// <summary>
    /// 具体设备类：欧姆龙 PLC，使用网口 FinsUdp 协议通讯，可读写信号点（bool）、数据（int、float）、字符串（string）等
    /// 可配置的属性包括：网口连接参数、心跳时间、信号字典：《信号名, 信号地址》
    /// </summary>
    public class PLCOmronFinsUdp : AbstractDevice {

        #region 1. 字段、普通属性

        // 字段：FinsUdp，用于通讯
        private OmronFinsUdp _finsUdp;

        // 字段：用于读写网口时，锁线程
        private object _lockObj = new object();

        // 字段：计时器。当在运行状态时，定时向 PLC 写入心跳信号。默认计时间隔为 4 秒        
        private static System.Timers.Timer _timer = new System.Timers.Timer() { Interval = 4000 };        

        /// <summary>
        /// 重写父类的属性：PLC 的网口是否连接且打开
        /// </summary>
        public override bool IsConnected {
            get {
                return true; // _finsUdp 没有 Connect 属性，默认连接上
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
        /// 带参实例构造函数：提供 PLC 的设备名，加载并设置参数，打开网口；初始化计时器事件
        /// </summary>
        /// <param name="deviceName">PLC 的设备名</param>
        public PLCOmronFinsUdp(string deviceName) : base(deviceName) {

            OpenPLC(); //加载并设置参数，打开网口

            //初始化计时结束的事件处理器
            _timer.Elapsed += new System.Timers.ElapsedEventHandler((obj, e) => {
                if (IsConnected && IsRunning) { //网口已连接，并且正在运行中
                    //WriteBoolean("心跳信号", true); //向心跳信号地址写入 true
                    WriteValue<bool>("心跳信号", true); //向心跳信号地址写入 true
                }
            });
        }

        #endregion


        #region 3. 可配置的属性

        /// <summary>
        /// 可配置的属性：网口的 IP 地址，默认值为 192.168.250.1
        /// </summary>
        [IniConfig]
        public string IP地址 { get; set; } = "192.168.250.1";


        /// <summary>
        /// 可配置的属性：网口的端口号，默认值为 9600
        /// </summary>
        [IniConfig]
        public int 端口号 { get; set; } = 9600;


        /// <summary>
        /// 可配置的属性：网口读取超时。读取等待超过此时间，视为未接收到数据。单位 ms，默认值为 500
        /// </summary>
        [IniConfig]
        public int 读取超时 { get; set; } = 500;


        /// <summary>
        /// 可配置的属性：网口在读取字符串时的最大长度。默认值为 100
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
        /// 默认添加心跳信号，地址为 C65.00
        /// </summary>
        [IniConfig]
        public Dictionary<string, string> 信号字典 { get; set; } = new Dictionary<string, string>();

        #endregion


        #region 4. 主要功能：重写 Open / Close：打开、关闭网口

        /// <summary>
        /// 主要功能：重写父类方法：加载配置，并打开网口
        /// </summary>
        public override void Open() {
            //开网口
            if (!IsConnected) {
                OpenPLC();
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：停止心跳信号，并关闭网口
        /// </summary>
        public override void Close() {
            //停止运行：关定时器
            if (IsRunning) {
                StopRunning();
            }

            //关网口
            if (IsConnected) {
                ClosePLC();
            }
        }


        // 私有方法：加载并设置参数，打开网口
        private void OpenPLC() {
            try {
                //网口关闭时，才能修改参数
                //if (!_serialPort.IsOpen) {
                    //设置参数
                    LoadConfigs(); //读取本地配置，设置参数：网口参数、心跳时间、信号字典

                    //信号字典默认添加心跳信号：65.00
                    if (!信号字典.ContainsKey("心跳信号")) {
                        信号字典.Add("心跳信号", "C65.00");
                    }
                    _finsUdp = new OmronFinsUdp(IP地址, 端口号);
                    _finsUdp.SA1 = 192;
                    _finsUdp.ReceiveTimeout = 读取超时;
                    _finsUdp.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;

                    //连接后，保存参数
                    SaveConfigs(); //保存本地配置
                //}
            }
            catch (Exception ex) {
                ShowException("打开 PLC 网口失败！", ex);
            }
        }


        // 私有方法：关闭网口，释放相关资源
        private void ClosePLC() {
            try {

            }
            catch (Exception ex) {
                ShowException("关闭 PLC 网口失败！", ex);
            }
        }

        #endregion


        #region 4. 主要功能：重写 Start / StopRunning：发送、停止心跳信号

        /// <summary>
        /// 主要功能：重写父类方法：开始运行：开定时器，发送心跳信号
        /// </summary>
        public override void StartRunning() {
            //打开网口
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


        #region 4. 主要功能：按照名称，读地址

        /// <summary>
        /// 主要功能：泛型方法：从 PLC 指定名称读取地址的值。可使用类型：bool、int、float 等
        /// </summary>
        /// <param name="addressName">读取地址的名称</param>
        /// <returns>读取到的值</returns>
        public T ReadValue<T>(string addressName) where T : struct {

            if (信号字典.ContainsKey(addressName)) {
                string address = 信号字典[addressName];
                lock (_lockObj) {
                    try {
                        object res; //装箱
                        var typeName = typeof(T).Name;
                        switch (typeName) {
                            case "Boolean":
                                res = _finsUdp.ReadBool(address).Content;
                                break;
                            case "Int16":
                                res = _finsUdp.ReadInt16(address).Content;
                                break;
                            case "Int32":
                                res = _finsUdp.ReadInt32(address).Content;
                                break;
                            case "Int64":
                                res = _finsUdp.ReadInt64(address).Content;
                                break;
                            case "UInt16":
                                res = _finsUdp.ReadUInt16(address).Content;
                                break;
                            case "UInt32":
                                res = _finsUdp.ReadUInt32(address).Content;
                                break;
                            case "UInt64":
                                res = _finsUdp.ReadUInt64(address).Content;
                                break;
                            case "Single":
                                res = _finsUdp.ReadFloat(address).Content;
                                break;
                            case "Double":
                                res = _finsUdp.ReadDouble(address).Content;
                                break;
                            default:
                                res = 0;
                                break;
                        }

                        return (T)res; //拆箱                        
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取地址 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if 信号字典.ContainsKey

            return default(T);
        }


        /// <summary>
        /// 主要功能：方法：从 PLC 指定名称读取字符串地址的值。字符串长度由可配置的参数 [读取字符串长度] 设置。
        /// </summary>
        /// <param name="addressName">读取字符串地址的名称</param>
        /// <returns>读取到的字符串</returns>
        public string ReadString(string addressName) {

            if (信号字典.ContainsKey(addressName)) {
                string address = 信号字典[addressName];
                lock (_lockObj) {
                    try {
                        return _finsUdp.ReadString(address, (ushort)读取字符串长度).Content;
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取字符串 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if 信号字典.ContainsKey

            return "";
        }

        #endregion


        #region 4. 主要功能：按照名称，写地址

        /// <summary>
        /// 主要功能：泛型方法：向 PLC 指定名称的地址写入值。可使用类型：bool、int、float 等
        /// 如果写入成功，返回 true
        /// </summary>
        /// <param name="addressName">写入地址的名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteValue<T>(string addressName, T value) where T : struct {

            if (信号字典.ContainsKey(addressName)) {
                string address = 信号字典[addressName];
                lock (_lockObj) {
                    try {
                        object objValue = value; //装箱
                        bool res = false; //写入是否成功
                        var typeName = typeof(T).Name;

                        switch (typeName) {
                            case "Boolean":
                                res = _finsUdp.Write(address, (bool)objValue).IsSuccess;
                                break;
                            case "Int16":
                                res = _finsUdp.Write(address, (short)objValue).IsSuccess;
                                break;
                            case "Int32":
                                res = _finsUdp.Write(address, (int)objValue).IsSuccess;
                                break;
                            case "Int64":
                                res = _finsUdp.Write(address, (long)objValue).IsSuccess;
                                break;
                            case "UInt16":
                                res = _finsUdp.Write(address, (ushort)objValue).IsSuccess;
                                break;
                            case "UInt32":
                                res = _finsUdp.Write(address, (uint)objValue).IsSuccess;
                                break;
                            case "UInt64":
                                res = _finsUdp.Write(address, (ulong)objValue).IsSuccess;
                                break;
                            case "Single":
                                res = _finsUdp.Write(address, (float)objValue).IsSuccess;
                                break;
                            case "Double":
                                res = _finsUdp.Write(address, (double)objValue).IsSuccess;
                                break;
                            default:
                                break;
                        }

                        return res; //返回是否写入成功
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 写入地址 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if 信号字典.ContainsKey

            return false;
        }


        /// <summary>
        /// 主要功能：方法：向 PLC 指定名称写入字符串地址的值。
        /// 如果写入成功，返回 true
        /// </summary>
        /// <param name="addressName">写入字符串地址的名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteString(string addressName, string value) {

            if (信号字典.ContainsKey(addressName)) {
                string address = 信号字典[addressName];
                lock (_lockObj) {
                    try {
                        return _finsUdp.Write(address, value).IsSuccess;
                    }
                    catch (Exception ex) {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 写入字符串 " + address + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if 信号字典.ContainsKey

            return false;
        }

        #endregion

    }// class

}// namespace
