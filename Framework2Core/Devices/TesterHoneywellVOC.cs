using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core {
    /// <summary>
    /// 具体设备类：霍尼韦尔 VOC 测试仪，使用串口通讯，接收指令并返回读数（气体浓度）
    /// 可配置的属性包括：串口连接参数、读取次数、读取间隔
    /// </summary>
    public class TesterHoneywellVOC : AbstractDevice {

        #region 1. 字段、普通属性

        // 字段：串口，用于通讯。默认参数为 9600、8、1、N；读取超时 500ms
        private SerialPort _serialPort = new SerialPort() {
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            StopBits = StopBits.One,
            Parity = Parity.None,
            ReadTimeout = 500
        };

        // 字段：用于读写串口时，锁线程
        private object _lockObj = new object();

        /// <summary>
        /// 重写父类的属性：测试仪的串口是否连接且打开
        /// </summary>
        public override bool IsConnected {
            get {
                return _serialPort.IsOpen;
            }
        }
        #endregion


        #region 2. 构造函数

        /// <summary>
        /// 带参实例构造函数：提供测试仪的设备名，加载并设置参数，打开串口
        /// </summary>
        /// <param name="deviceName"></param>
        public TesterHoneywellVOC(string deviceName) : base(deviceName) {
            OpenTester();   //加载并设置参数，打开串口
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
        /// 默认值为 9600
        /// </summary>
        [IniConfig]
        public BaudrateOptions 波特率 {
            get { return _baudrate; }
            set {
                _baudrate = value;
                _serialPort.BaudRate = Convert.ToInt32(_baudrate.Text); //根据 Text 属性设置波特率
            }
        }
        private BaudrateOptions _baudrate = new BaudrateOptions() { Text = "9600" };


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
        /// 可配置的属性：在一次测试中，读取测试仪示数的次数。默认值为 10
        /// </summary>
        [IniConfig]
        public int 读取次数 { get; set; } = 10;

        /// <summary>
        ///  可配置的属性：在一次测试中，读取测试仪示数的时间间隔。单位 ms，默认值为 200
        /// </summary>
        [IniConfig]
        public int 读取间隔 { get; set; } = 200;

        #endregion


        #region 4. 主要功能：重写 Open / Close：打开、关闭串口

        /// <summary>
        /// 主要功能：重写父类方法：加载配置，并打开串口
        /// </summary>
        public override void Open() {
            //开串口
            if (!IsConnected) {
                OpenTester();
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：关闭串口
        /// </summary>
        public override void Close() {           
            //关串口
            if (IsConnected) {
                CloseTester();
            }
        }


        // 私有方法：加载并设置参数，打开串口
        private void OpenTester() {
            try {
                //串口关闭时，才能修改参数
                if (!_serialPort.IsOpen) {
                    //设置参数
                    LoadConfigs(); //读取本地配置，设置参数：串口参数
                    
                    _serialPort.Open(); //打开串口 

                    //连接后，保存参数
                    SaveConfigs(); //保存本地配置
                }
            }
            catch (Exception ex) {
                ShowException("打开测试仪串口失败！", ex);
            }
        }


        // 私有方法：关闭串口，释放相关资源
        private void CloseTester() {
            try {
                _serialPort.Close(); //关闭串口
                _serialPort.Dispose(); //释放串口资源
            }
            catch (Exception ex) {
                ShowException("关闭测试仪串口失败！", ex);
            }
        }

        #endregion


        #region 4. 主要功能：读取测试数据

        /// <summary>
        /// 主要功能：方法：读取 VOC 测试数据多次，并取出第二大的值。
        /// 读取的次数以及间隔，均为可配置的属性
        /// </summary>
        /// <param name="res">输出参数：测试仪返回的结果。如果测试失败，返回 -1.0</param>
        /// <param name="orignalData">输出参数：测试仪返回的原始数据</param>
        /// <returns>测试成功，返回 True；测试失败，返回 False</returns>

        public bool ReadTestResult(out double res, out string orignalData) {            
            
            string cmd = "R"; // 指令

            lock (_lockObj) {
                try {
                    double[] dResults = new double[读取次数];

                    // 读取10次测试仪数据
                    for (int i = 0; i < dResults.Length; i++) {
                        _serialPort.Write(cmd); //发送串口命令
                        string strRes = ReadSerialPortResponse(); //读取串口返回值
                                                                  //格式：00000000
                        if (!string.IsNullOrEmpty(strRes)) {
                            strRes = strRes.Trim();
                            double.TryParse(strRes, out dResults[i]); //解析字符串为 double 类型
                        }
                        Thread.Sleep(读取间隔);
                    }

                    orignalData = string.Join(", ", dResults); //拼接各组数据为字符串

                    Array.Sort(dResults); //升序排列
                    res = dResults[dResults.Length - 2]; //返回第二大的数据
                    return true;
                }
                catch (Exception ex) {
                    LocalLogSave.WriteErrorLog("测试仪 [" + DeviceName + "] 读取测试数据失败！\r\n异常信息:\r\n" + ex.ToString());
                }

                res = -1.0;
                orignalData = "";
                return false;
            }

        }

        #endregion


        #region 6. 私有方法：读取串口返回数据

        // 私有方法：读取串口返回数据
        private string ReadSerialPortResponse() {
            string res = null;
            try {
                res = _serialPort.ReadTo("\r"); //读取串口返回值，到 \r 停止
            }
            catch (Exception ex) {
                LocalLogSave.WriteErrorLog("测试仪 [" + DeviceName + "] 读取串口返回数据失败！\r\n异常信息:\r\n" + ex.ToString());
                res = null;
            }
            finally {
                //即使在 try、catch 中 return，依旧会执行 finally 中的语句
                _serialPort.DiscardInBuffer();  // 读取完毕，清空接受缓存区
            }

            return res;
        }

        #endregion
        
    } //class
}// namespace
