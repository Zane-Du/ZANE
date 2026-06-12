using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core
{
    /// <summary>
    /// 具体设备类：LISHWEI 数字式光源控制器，使用串口通讯，接收指令并控制各通道的亮度 0-255
    /// 可配置的属性包括：串口连接参数、光源通道数
    /// </summary>
    public class LightControllerCST : AbstractDevice
    {

        #region 1. 字段、普通属性

        // 字段：串口，用于通讯。默认参数为 19200、8、1、N；读取超时 500ms
        private readonly SerialPort _serialPort = new SerialPort()
        {
            PortName = "COM1",
            BaudRate = 19200,
            DataBits = 8,
            StopBits = StopBits.One,
            Parity = Parity.None,
            ReadTimeout = 500
        };

        // 字段：用于读写串口时，锁线程
        private readonly object _lockObj = new object();

        /// <summary>
        /// 重写父类的属性：测试仪的串口是否连接且打开
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return _serialPort.IsOpen;
            }
        }

        public static Dictionary<string, LightControllerCST> _dicLightControllerCST = new Dictionary<string, LightControllerCST>();

        #endregion


        #region 2. 构造函数

        /// <summary>
        /// 带参实例构造函数：提供测试仪的设备名，加载并设置参数，打开串口
        /// </summary>
        /// <param name="deviceName"></param>
        public LightControllerCST(string deviceName) : base(deviceName)
        {
            OpenLightController();   //加载并设置参数，打开串口
            if (IsConnected)
            {
                _dicLightControllerCST[deviceName] = this;
            }
        }

        #endregion


        #region 3. 可配置的属性

        /// <summary>
        /// 可配置的属性：串口的端口名，默认值为 COM1
        /// </summary>
        [IniConfig]
        public string 端口名
        {
            get { return _serialPort.PortName; }
            set
            {
                if (value.StartsWith("COM"))
                { //设置端口名，以 COM 开头
                    _serialPort.PortName = value;
                }
            }
        }


        /// <summary>
        /// 可配置的属性：串口的波特率。使用可选项类型 BaudrateOptions，可选值包括：9600、19200、38400、57600、115200。
        /// 默认值为 9600
        /// </summary>
        [IniConfig]
        public BaudrateOptions 波特率
        {
            get { return _baudrate; }
            set
            {
                _baudrate = value;
                _serialPort.BaudRate = Convert.ToInt32(_baudrate.Text); //根据 Text 属性设置波特率
            }
        }
        private BaudrateOptions _baudrate = new BaudrateOptions() { Text = "9600" };


        /// <summary>
        /// 可配置的属性：串口的数据位，可选值包括：6、7、8。默认值为 8
        /// </summary>
        [IniConfig]
        public int 数据位
        {
            get { return _serialPort.DataBits; }
            set
            {
                if (value > 5 && value < 9)
                { //限定数据位范围：6，7，8
                    _serialPort.DataBits = value; //设置数据位
                }
            }
        }


        /// <summary>
        /// 可配置的属性：串口的停止位，默认值为 One
        /// </summary>
        [IniConfig]
        public StopBits 停止位
        {
            get { return _serialPort.StopBits; }
            set
            {
                if (value > 0)
                    _serialPort.StopBits = value;
            }
        }


        /// <summary>
        /// 可配置的属性：串口的校验位，默认值为 None
        /// </summary>
        [IniConfig]
        public Parity 校验位
        {
            get { return _serialPort.Parity; }
            set
            {
                if (value > 0)
                    _serialPort.Parity = value;
            }
        }


        /// <summary>
        /// 可配置的属性：串口读取超时。读取等待超过此时间，视为未接收到数据。单位 ms，默认值为 500
        /// </summary>
        [IniConfig]
        public int 读取超时
        {
            get { return _serialPort.ReadTimeout; }
            set
            {
                if (value > 0)
                    _serialPort.ReadTimeout = value;
            }
        }


        /// <summary>
        ///  可配置的属性：光源控制器所支持的光源数量。默认值为 4
        /// </summary>
        [IniConfig]
        public int 通道数
        {
            get { return channnelCount; }
            set
            {
                if (value > 0)
                    channnelCount = value;
            }
        }
        private int channnelCount = 4;

        /// <summary>
        ///  可配置的属性：每个通道的亮度
        /// </summary>
        [IniConfig]
        public int[] 通道亮度 { get; set; } = new int[] { 255, 255, 255, 255, 255, 255, 255, 255 };
        #endregion


        #region 4. 主要功能：重写 Open / Close：打开、关闭串口

        /// <summary>
        /// 主要功能：重写父类方法：加载配置，并打开串口
        /// </summary>
        public override void Open()
        {
            //开串口
            if (!IsConnected)
            {
                OpenLightController();
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：关闭串口
        /// </summary>
        public override void Close()
        {
            //关串口
            if (IsConnected)
            {
                CloseLightController();
            }
        }


        // 私有方法：加载并设置参数，打开串口
        private string OpenLightController()
        {
            string errorMessage = string.Empty;
            try
            {
                //串口关闭时，才能修改参数
                if (!_serialPort.IsOpen)
                {
                    LoadConfigs(); //读取本地配置，设置参数：串口参数
                    _serialPort.Open(); //打开串口 
                    OpenAllLights();//打开所有通道亮度

                    //连接后，保存参数
                    SaveConfigs(); //保存本地配置
                    CloseAllLights();
                }
                return errorMessage;
            }
            catch (Exception ex)
            {
                errorMessage = $"打开光源控制器{DeviceName}串口失败！" + ex.Message;
                return errorMessage;
            }
        }


        // 私有方法：关闭串口，释放相关资源
        private void CloseLightController()
        {
            try
            {
                CloseAllLights(); //关闭所有光源
                _serialPort.Close(); //关闭串口
                _serialPort.Dispose(); //释放串口资源
            }
            catch (Exception ex)
            {
                ShowException("关闭光源控制器串口失败！", ex);
            }
        }

        #endregion


        #region 4. 主要功能：打开、关闭光源

        /// <summary>
        /// 主要功能：方法：调节单个光源的亮度为设定值，并把其他光源亮度置 0。
        /// 通道索引从 0 开始：通道0-A、通道1-B、通道2-C、通道3-D...
        /// </summary>
        /// <param name="channel">通道索引，从 0 开始</param>
        /// <param name="bright">设定亮度值，范围 0-255</param>
        /// <returns>是否设置成功</returns>
        public bool AdjustChannel(int channel, int bright)
        {
            if (channel >= 通道数)
            {
                return false;
            }

            if (bright > 255 || bright < 0)
            {
                return false;
            }

            int[] allBrights = new int[通道数]; //所有通道的亮度数组
            allBrights[channel] = bright; //将指定通道 channel 的光源亮度，设定为 bright

            return SendCommand(allBrights); //根据亮度数组生成控制指令，并发送
        }

        /// <summary>
        /// 主要功能：方法：调节单个光源的亮度为设定值，并把其他光源亮度不处理。
        /// 通道索引从 0 开始：通道0-A、通道1-B、通道2-C、通道3-D...
        /// </summary>
        /// <param name="channel">通道索引，从 0 开始</param>
        /// <param name="bright">设定亮度值，范围 0-255</param>
        /// <returns>是否设置成功</returns>
        public bool AdjustChannelSet(int channel, int bright)
        {
            if (channel >= 通道数)
            {
                return false;
            }

            if (bright > 255 || bright < 0)
            {
                return false;
            }

            return SendCommand(channel, bright); //根据亮度数组生成控制指令，并发送
        }

        public string AjustMultipleChannelSet(int[] arrChannels, int[] arrBrights)
        {
            return SendCommand(arrChannels, arrBrights);
        }

        /// <summary>
        /// 主要功能：方法：调节多个光源的亮度为设定值，并把其他光源置 0。
        /// 通道索引从 0 开始：通道 0-A、通道 1-B、通道 2-C、通道 3-D...
        /// </summary>
        /// <param name="arrChannels">通道索引数组，从 0 开始，例如 [0, 1, 3]</param>
        /// <param name="arrBrights">设定亮度值数组，范围 0-255，例如 [50, 100, 155]</param>
        /// <returns>是否设置成功</returns>
        public bool AdjustMultipleChannels(int[] arrChannels, int[] arrBrights)
        {

            int countChannels = arrChannels.Length; //通道数组长度
            int countBrights = arrBrights.Length; //亮度数组长度

            if (countChannels != countBrights)
            {
                return false;
            }

            int[] allBrights = new int[通道数]; //所有通道的亮度数组，初始化亮度为 0

            for (int i = 0; i < countChannels; i++)
            {
                var channel = arrChannels[i]; //通道索引
                var bright = arrBrights[i]; //设定亮度

                if (channel >= 通道数)
                {
                    return false;
                }

                if (bright > 255 || bright < 0)
                {
                    return false;
                }

                allBrights[channel] = bright; //将索引为 channel 的通道的亮度，更新为设定值 bright
            }

            return SendCommand(allBrights); //根据亮度数组生成控制指令，并发送
        }


        /// <summary>
        /// 主要功能：方法：关闭所有光源
        /// </summary>
        /// <returns>是否设置成功</returns>
        public bool CloseAllLights()
        {

            int[] allBrights = new int[通道数]; //所有通道的亮度数组，初始化亮度为 0
            return SendCommand(allBrights); //根据亮度数组生成控制指令，并发送
        }


        /// <summary>
        /// 主要功能：方法：打开所有光源
        /// </summary>
        /// <returns>是否设置成功</returns>
        public bool OpenAllLights()
        {

            int[] allBrights = new int[通道数]; //所有通道的亮度数组
            for (int i = 0; i < allBrights.Length; i++)
            {
                allBrights[i] = 255; //每个通道亮度设置为 155
            }
            return SendCommand(allBrights); //根据亮度数组生成控制指令，并发送
        }

        #endregion


        #region 6. 私有方法：生成控制指令，读取串口返回数据

        // 私有方法：根据亮度数组生成控制指令，并发送
        private bool SendCommand(int[] allBrights)
        {

            //如果光源控制器已连接
            if (IsConnected)
            {

                // 第1步：生成指令
                StringBuilder sb = new StringBuilder();
                sb.Append("$");//与CST的区别
                for (int i = 0; i < allBrights.Length; i++)
                {
                    sb.Append("S");

                    //各通道对应的符号：通道0-A、通道1-B、通道2-C、通道3-D...
                    char channelSymbol = Convert.ToChar('A' + i);
                    sb.Append(channelSymbol);

                    //设定亮度："D4"：4位十进制数，例如 0155
                    int bright = allBrights[i];
                    //sb.Append(bright.ToString("D4"));//CST
                    sb.Append(bright.ToString("D3"));
                    if (i != allBrights.Length - 1)
                    {
                        sb.Append("&");
                    }
                }
                sb.Append("#");
                string cmd = sb.ToString();

                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(cmd); //发送串口命令
                        Thread.Sleep(20); //等待20ms 开关光源
                        string strRes = ReadSerialPortResponse(); //读取串口返回值
                        if (!string.IsNullOrEmpty(strRes))
                        {
                            return true; //收到返回值不为空，设置成功
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("光源控制器 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return false;
        }


        // 私有方法：根据亮度数组生成控制指令，并发送
        private string SendCommand(int[] allChannels, int[] allBrights)
        {

            //如果光源控制器已连接
            if (IsConnected)
            {

                // 第1步：生成指令
                StringBuilder sb = new StringBuilder();
                sb.Append("$");//与CST的区别
                for (int i = 0; i < allChannels.Length; i++)
                {
                    sb.Append("S");
                    if (allChannels[i] >= 通道数)
                    {
                        return $"光源亮度调整错误:目标通道号[{allChannels[i]}]超过最大通道数";
                    }

                    //各通道对应的符号：通道0-A、通道1-B、通道2-C、通道3-D...
                    char channelSymbol = Convert.ToChar('A' + allChannels[i]);
                    sb.Append(channelSymbol);
                    if (i > allBrights.Length)
                    {
                        return $"光源亮度调整错误:目标通道号[{allChannels[i]}]未设置对应光源亮度";
                    }
                    //设定亮度："D4"：4位十进制数，例如 0155
                    int bright = allBrights[i];
                    if (bright > 255) bright = 255;
                    if (bright < 0) bright = 0;
                    //sb.Append(bright.ToString("D4"));
                    sb.Append(bright.ToString("D3"));
                    if (i != allBrights.Length - 1)
                    {
                        sb.Append("&");
                    }
                }
                sb.Append("#");
                string cmd = sb.ToString();

                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(cmd); //发送串口命令
                        Thread.Sleep(20); //等待20ms 开关光源
                        string strRes = ReadSerialPortResponse(); //读取串口返回值
                        if (!string.IsNullOrEmpty(strRes))
                        {
                            return ""; //收到返回值不为空，设置成功
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("光源控制器 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                        return ("光源控制器 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return "光源控制器 [" + DeviceName + "] 未连接";
        }
        private bool SendCommand(int iChannel, int iBright)
        {

            //如果光源控制器已连接
            if (IsConnected)
            {

                // 第1步：生成指令
                StringBuilder sb = new StringBuilder();

                if (iChannel < 通道数)
                {
                    sb.Append("$");//与CST的区别
                    sb.Append("S");
                    //各通道对应的符号：通道0-A、通道1-B、通道2-C、通道3-D...
                    char channelSymbol = Convert.ToChar('A' + iChannel);
                    sb.Append(channelSymbol);

                    //设定亮度："D4"：4位十进制数，例如 0155
                    int bright = iBright;
                    //sb.Append(bright.ToString("D4"));
                    sb.Append(bright.ToString("D3"));
                    sb.Append("#");
                }

                string cmd = sb.ToString();

                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(cmd); //发送串口命令
                        Thread.Sleep(10); //等待20ms 开关光源
                        string strRes = ReadSerialPortResponse(); //读取串口返回值
                        if (!string.IsNullOrEmpty(strRes))
                        {
                            return true; //收到返回值不为空，设置成功
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("光源控制器 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return false;
        }
        // 私有方法：读串口返回数据。光源返回的数据无换行符，因此读取固定长度 100
        private string ReadSerialPortResponse()
        {
            string res = null;
            try
            {
                byte[] buffer = new byte[100]; //光源返回的数据无换行符，因此改为读取固定长度 100
                int length = _serialPort.Read(buffer, 0, 100); //返回有效长度
                res = Encoding.Default.GetString(buffer, 0, length); //将返回的byte数组编码为string
            }
            catch (Exception ex)
            {
                LocalLogSave.WriteErrorLog("光源控制器 [" + DeviceName + "] 读取串口返回数据失败！\r\n异常信息:\r\n" + ex.ToString());
                res = null;
            }
            finally
            {
                //即使在 try、catch 中 return，依旧会执行 finally 中的语句
                _serialPort.DiscardInBuffer();  // 读取完毕，清空接收缓存区
            }

            return res;
        }

        #endregion

    }
}
