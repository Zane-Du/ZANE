using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwinCAT.Ads;

namespace Framework2Core
{
    /// <summary>
    /// 具体设备类：倍福 PLC，使用 TwinCAT AdsClient 通讯，可读写信号点（bool）、数据（int、float）、字符串（string）等
    /// 可配置的属性包括：网口连接参数、心跳时间、信号字典：《信号名, 变量标签》
    /// </summary>
    public class PLCBeckhoffAds : AbstractDevice
    {

        #region 1. 字段、普通属性

        // 字段：倍福 PLC 通讯器：Wincat AdsClient
        public TcAdsClient _adsClient = new TcAdsClient();

        // 字段：用于读写网口时，锁线程
        private object _lockObj = new object();

        // 字段：计时器。当在运行状态时，定时向 PLC 写入心跳信号。默认计时间隔为 4 秒        
        private static System.Timers.Timer _timer = new System.Timers.Timer() { Interval = 4000 };

        // 字段：句柄字典《变量名，变量句柄》：根据变量名，找到变量的句柄
        public Dictionary<string, int> 句柄字典 = new Dictionary<string, int>();

        /// <summary>
        /// 重写父类的属性：PLC AdsClient 是否连接上，并且成功创建了变量句柄
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return _adsClient.IsConnected && _isConnected;
            }
        }
        private bool _isConnected = false;

        /// <summary>
        /// 重写父类的属性：PLC 是否正在运行：定时发送心跳信号
        /// </summary>
        public override bool IsRunning
        {
            get
            {
                return _timer.Enabled;
            }
        }

        #endregion


        #region 2. 构造函数
        private bool beat = false;
        /// <summary>
        /// 带参实例构造函数：提供 PLC 的设备名，加载并设置参数，打开 AdsClient；初始化计时器事件
        /// </summary>
        /// <param name="deviceName">PLC 的设备名</param>
        public PLCBeckhoffAds(string deviceName) : base(deviceName)
        {
            OpenPLC(); //加载并设置参数，连接 PLC

            if (IsConnected)
            {
                _timer.Elapsed += new System.Timers.ElapsedEventHandler((obj, e) =>
                {
                    if (IsConnected && IsRunning)
                    { //adsClient 已连接，并且正在运行中
                        beat = !beat;
                        WriteValue<bool>("心跳信号", beat); //向心跳信号变量写入 true
                    }
                });


            }
            //初始化计时结束的事件处理器    
        }

        #endregion


        #region 3. 可配置的属性

        /// <summary>
        /// 可配置的属性：网口的 IP 地址，默认值为 169.254.100.1.1.1
        /// </summary>
        [IniConfig]
        public string IP地址
        {
            get { return addrIP; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    addrIP = value;
            }
        }

        private string addrIP = "169.254.100.1.1.1";
        /// <summary>
        /// 可配置的属性：网口的端口号，默认值为 801
        /// </summary>
        [IniConfig]
        public int 端口号
        {
            get { return port; }
            set
            {
                if (value > 0)
                    port = value;
            }
        }


        private int port = 801;
        /// <summary>
        /// 可配置的属性：串口在读取字符串时的最大长度。默认值为 100
        /// </summary>
        [IniConfig]
        public int 读取字符串长度
        {
            get { return strLength; }
            set
            {
                if (value > 0)
                    strLength = value;
            }
        }

        private int strLength = 100;
        /// <summary>
        /// 可配置的属性：心跳时间，在运行状态时，定时写入心跳信号，单位 ms。默认值为 4000
        /// </summary>
        [IniConfig]
        public int 心跳时间
        {
            get { return (int)_timer.Interval; }
            set { _timer.Interval = value; }
        }


        /// <summary>
        /// 可配置的属性：信号字典：《信号名, 变量标签》。用于读写 PLC 信号和数据。
        /// 默认添加心跳信号，变量标签为 .bPLCCommunicationPC_ToPLC
        /// </summary>
        [IniConfig]
        public Dictionary<string, string> 信号字典 { get; set; } = new Dictionary<string, string>();

        #endregion


        #region 4. 主要功能：重写 Open / Close：打开、关闭 AdsClient

        /// <summary>
        /// 主要功能：重写父类方法：加载配置，并打开 PLC AdsClient
        /// </summary>
        public override void Open()
        {
            //开 PLC AdsClient
            if (!IsConnected)
            {
                OpenPLC();
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：停止心跳信号，并关闭串口
        /// </summary>
        public override void Close()
        {
            //停止运行：关定时器
            if (IsRunning)
            {
                StopRunning();
            }

            //关 PLC AdsClient
            if (IsConnected)
            {
                ClosePLC();
            }
        }


        // 私有方法：加载并设置参数，连接网口；遍历信号字典，为各信号创建句柄
        private bool OpenPLC()
        {
            bool isSucce = false;
            try
            {
                //AdsClient 断开时，才能修改参数
                if (!_adsClient.IsConnected)
                {
                    //设置参数
                    LoadConfigs(); //读取本地配置，设置参数：网口参数、心跳时间、信号字典
                    if (!PingAccessable(IP地址)) return false;
                    _adsClient.Connect(IP地址, 端口号); //连接网口
                    if (!_adsClient.IsConnected) return false;
                    //遍历信号字典，为各信号创建句柄
                    句柄字典 = new Dictionary<string, int>();
                    //_isConnected = true; //先置 true，如果创建句柄时发生异常，就置 false
                    _isConnected = _adsClient.IsConnected;
                    if (!_isConnected)
                    {
                        return isSucce;
                    }
                    else
                    {
                        //信号字典默认添加心跳信号：.bPLCCommunicationPC_ToPLC
                        if (!信号字典.ContainsKey("心跳信号"))
                        {
                            信号字典.Add("心跳信号", ".PcHeart[1]");
                        }

                        foreach (var name in 信号字典.Keys)
                        {
                            var label = 信号字典[name]; //找到变量的标签
                            int handle = -1;
                            try
                            {
                                label = label.Replace(';', ','); //数组变量的逗号，在配置文件中写为分号，这里还原
                                handle = _adsClient.CreateVariableHandle(label); //为变量标签创建句柄
                            }
                            catch (Exception ex)
                            {
                                ShowException($"PLC 创建变量 [{label}] 的句柄失败！", ex);

                            }

                            //添加到句柄字典
                            句柄字典.Add(name, handle);
                        }
                        isSucce = true;
                        //连接后，保存参数
                        SaveConfigs(); //保存本地配置

                    }
                }

            }
            catch (Exception ex)
            {
                ShowException("打开 PLC AdsClient 失败！", ex);
            }
            return isSucce;
        }


        // 私有方法：关闭 PLC，释放相关资源
        private void ClosePLC()
        {
            try
            {
                _adsClient.Disconnect();//断开连接
                _adsClient?.Dispose(); //释放资源
            }
            catch (Exception ex)
            {
                ShowException("关闭 PLC AdsClient失败！", ex);
            }
        }

        #endregion


        #region 4. 主要功能：重写 Start / StopRunning：发送、停止心跳信号

        /// <summary>
        /// 主要功能：重写父类方法：开始运行：开定时器，发送心跳信号
        /// </summary>
        public override void StartRunning()
        {
            //打开 PLC
            if (!IsConnected)
            {
                Open();
            }

            //开定时器，发送心跳信号
            if (!IsRunning)
            {
                _timer.Enabled = true;
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：停止运行：关定时器，停止心跳信号
        /// </summary>
        public override void StopRunning()
        {
            if (IsRunning)
            {
                _timer.Enabled = false; //关定时器
            }
        }
        #endregion


        #region 4. 主要功能：按照名称，读变量

        /// <summary>
        /// 主要功能：泛型方法：从 PLC 指定名称的标签读取变量的值。可使用类型：bool、int、float
        /// </summary>
        /// <param name="variableName">读取变量的名称</param>
        /// <returns>读取到的值</returns>
        public T ReadValue<T>(string variableName) where T : struct
        {

            //如果PLC已连接
            if (IsConnected)
            {
                //lock (_lockObj)
                //{
                try
                {
                    var handle = GetVariableHandle(variableName); //根据变量名，找到变量的句柄

                    //判断：句柄>0
                    if (handle != -1)
                    {
                        return (T)_adsClient.ReadAny(handle, typeof(T)); //读取值
                    }
                }
                catch (Exception ex)
                {
                    LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取变量 " + variableName + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                }
                //}// lock
            }// if (IsConnected)

            return default(T);
        }


        /// <summary>
        /// 主要功能：方法：从 PLC 指定名称的标签读取字符串变量的值。字符串长度由可配置的参数 [读取字符串长度] 设置。
        /// </summary>
        /// <param name="variableName">读取字符串变量的名称</param>
        /// <returns>读取到的字符串</returns>
        public string ReadString(string variableName)
        {

            //如果PLC已连接
            if (IsConnected)
            {
                lock (_lockObj)
                {
                    try
                    {
                        var handle = GetVariableHandle(variableName); //根据变量名，找到变量的句柄

                        //判断：句柄>0
                        if (handle != -1)
                        {
                            var res = _adsClient.ReadAny(handle, typeof(String), new int[] { 读取字符串长度 });
                            return res.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 读取字符串变量 " + variableName + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return "";
        }

        #endregion


        #region 4. 主要功能：按照名称，写变量

        /// <summary>
        /// 主要功能：泛型方法：向 PLC 指定名称的标签写入变量的值。可使用类型：bool、int、float。
        /// 如果写入成功，返回 true
        /// </summary>
        /// <param name="variableName">写入变量的名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteValue<T>(string variableName, T value) where T : struct
        {

            //如果PLC已连接
            if (IsConnected)
            {
                //lock (_lockObj)
                //{
                try
                {
                    var handle = GetVariableHandle(variableName); //根据变量名，找到变量的句柄

                    //判断：句柄>0
                    if (handle != -1)
                    {
                        _adsClient.WriteAny(handle, value); //写入值
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 写入变量 " + variableName + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                }
                //}// lock
            }// if (IsConnected)

            return false;
        }


        /// <summary>
        /// 主要功能：方法：向 PLC 指定名称的标签写入字符串变量的值。
        /// 如果写入成功，返回 true
        /// </summary>
        /// <param name="variableName">写入字符串变量的名称</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否写入成功</returns>
        public bool WriteString(string variableName, string value)
        {

            //如果PLC已连接
            if (IsConnected)
            {
                lock (_lockObj)
                {
                    try
                    {
                        var handle = GetVariableHandle(variableName); //根据变量名，找到变量的句柄

                        //判断：
                        if (handle != -1)
                        {
                            _adsClient.WriteAny(handle, value, new int[] { value.Length }); //写入值
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("PLC [" + DeviceName + "] 写入字符串变量 " + variableName + " 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return false;
        }

        #endregion


        #region 6. 私有方法：内部调用

        // 私有方法：根据变量名，从字典中找到变量的句柄。如果没找到，返回 -1
        private int GetVariableHandle(string variableName)
        {
            int handle = -1;
            if (句柄字典.ContainsKey(variableName))
            {
                handle = 句柄字典[variableName]; //根据变量名，找到变量的句柄
            }
            return handle;
        }

        #endregion

    }// class

}// namespace
