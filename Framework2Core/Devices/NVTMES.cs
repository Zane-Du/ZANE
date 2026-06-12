using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Framework2Core;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Framework2Core
{
    public class NVTMES : AbstractDevice
    {
        #region 1、普通属性、字段
        //字段：MES通讯器，UDP通讯
        UdpClient udpClient = new UdpClient();

        // 字段：用于读写网口时，锁线程
        private object _lockObj = new object();

        //字段：实例化的远程端点
        IPEndPoint remote目标端口 = null;
        IPEndPoint remote本地端口 = null;
        public static System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        
        private bool _isOpen = false;
        public bool IsOpen { get => _isOpen; }

        private bool _isClosing;

        public bool IsClosing
        {
            get { return _isClosing; }
        }

        #endregion

        #region 2、构造函数
        public NVTMES(string deviceName) : base(deviceName)
        {
            this.LoadConfigs();
            this.SaveConfigs();
        }
        #endregion

        #region 3、可配置属性
        /// <summary>
        /// 可配置的属性：网口的 IP 地址，默认值为 169.254.100.1.1.1
        /// </summary>
        [IniConfig]
        public string IP地址
        {
            get { return addrIP; }
            set
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    addrIP = value;
                    try
                    {
                        if(addrIP != value)
                        {
                            remote目标端口 = new IPEndPoint(IPAddress.Parse(addrIP), 目标端口号);//实例化一个远程端点
                            remote本地端口 = new IPEndPoint(IPAddress.Parse(addrIP), 本地端口号);//实例化一个远程端点
                        }
                    }
                    catch(Exception ex)
                    {
                        ShowException($"MES设置[IP地址]失败！", ex);
                    }

                }
            }
        }

        private string addrIP = "127.0.0.1";

        /// <summary>
        /// 可配置的属性：网口的端口号，默认值为 801
        /// </summary>
        [IniConfig]
        public int 目标端口号
        {
            get { return _portSend; }
            set
            {
                if (value > 0)
                {
                    try
                    {
                        if(_portSend != value)
                        {
                            _portSend = value;
                            remote目标端口 = new IPEndPoint(IPAddress.Parse(addrIP), 目标端口号);//实例化一个远程端点
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException($"MES设置[IP地址]失败！", ex);
                    }
                }
            }
        }
        private int _portSend = 9900;

        /// <summary>
        /// 可配置的属性：网口的端口号，默认值为 801
        /// </summary>
        [IniConfig]
        public int 本地端口号
        {
            get { return _portLocal; }
            set
            {
                if (value > 0)
                {
                    try
                    {
                        if (_portLocal != value)
                        {
                            _portLocal = value;
                            remote本地端口 = new IPEndPoint(IPAddress.Parse(addrIP), 本地端口号);//实例化一个远程端点
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException($"MES设置[IP地址]失败！", ex);
                    }
                }
            }
        }
        private int _portLocal = 9910;


        [IniConfig]
        public Dictionary<string, string> MES参数上传字典 { get; set; } = new Dictionary<string, string>();

        [IniConfig]
        public Dictionary<string, string> MES参数点检字典 { get; set; } = new Dictionary<string, string>();

        [IniConfig]
        public string 设备ID { get; set; } = "";
        [IniConfig]
        public string ItemSpecificationName { get; set; } = "";
        [IniConfig]
        public int MES工位索引 { get; set; } = 10;
        #endregion

        #region 4#打开、关闭监听端口
        public override void Open()
        {

            try
            {
                if (!_isOpen)
                {
                    remote目标端口 = new IPEndPoint(IPAddress.Parse(IP地址), 目标端口号);//实例化一个远程端点
                    remote本地端口 = new IPEndPoint(IPAddress.Parse(IP地址), 本地端口号);//实例化一个远程端点
                    udpClient = new UdpClient(本地端口号);
                    _isOpen = true;
                    _isClosing = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UDP通讯实例化远程端点失败\r\n" + ex.ToString());
            }
        }

        public override void Close()
        {
            //停止运行：关定时器
            if (!_isOpen)
            {
                return;
            }
            _isClosing = true;
            SendMessage<MES_Close>(new MES_Close(), remote本地端口);
            //SendMessage("Close", remote本地端口);
            System.Threading.Thread.Sleep(500); //等待接收端口接收关闭信息
            udpClient?.Close();
            _isOpen = false;
        }
        #endregion

        #region 4.UDP发送、监听信息

        public void SendMessage<T>(T messageToSend)
        {
            lock (_lockObj)
            {
                try
                {
                    byte[] sendData = null;//要发送的字节数组
                    string JsonMsgForUser = JsonConvert.SerializeObject(messageToSend);
                    sendData = Encoding.UTF8.GetBytes(JsonMsgForUser);
                    //sendData = Encoding.Default.GetBytes(JsonMsgForUser);
                    
                    udpClient.Send(sendData, sendData.Length, remote目标端口);//将数据发送到远程端点

                    string filePath = @".\MES上传日志\";
                    if (!System.IO.Directory.Exists(filePath))
                        System.IO.Directory.CreateDirectory(filePath);  //如果文件不存在则新建

                    filePath += DateTime.Now.ToString("yyyy-MM") + ".txt";
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine(DateTime.Now.ToString("G"));
                        sw.WriteLine(remote目标端口.ToString());
                        sw.WriteLine(JsonMsgForUser);
                        sw.WriteLine("-^8^--^*^--^-^--^.^--^8^--^*^--^-^--^.^-");
                        sw.WriteLine();
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("MES发送信息失败:\r\n" + ex.Message);
                }
            }
        }

        public void SendMessage<T>(T messageToSend,IPEndPoint 目标端口)
        {
            lock (_lockObj)
            {
                try
                {
                    byte[] sendData = null;//要发送的字节数组
                    string JsonMsgForUser = JsonConvert.SerializeObject(messageToSend);
                    sendData = Encoding.UTF8.GetBytes(JsonMsgForUser);
                    //sendData = Encoding.Default.GetBytes(JsonMsgForUser);

                    udpClient.Send(sendData, sendData.Length, 目标端口);//将数据发送到远程端点

                    string filePath = @".\MES上传日志\";
                    if (!System.IO.Directory.Exists(filePath))
                        System.IO.Directory.CreateDirectory(filePath);  //如果文件不存在则新建

                    filePath += DateTime.Now.ToString("yyyy-MM") + ".txt";
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine(DateTime.Now.ToString("G"));
                        sw.WriteLine(目标端口.ToString());
                        sw.WriteLine(JsonMsgForUser);
                        sw.WriteLine("-^8^--^*^--^-^--^.^--^8^--^*^--^-^--^.^-");
                        sw.WriteLine();
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("MES发送信息失败:\r\n" + ex.Message);
                }
            }
        }

        public void SendMessage(string messageToSend)
        {
            lock (_lockObj)
            {
                try
                {
                    byte[] sendData = null;//要发送的字节数组
                    sendData = Encoding.Default.GetBytes(messageToSend);
                    udpClient.Send(sendData, sendData.Length, remote目标端口);//将数据发送到远程端点
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MES发送信息失败:\r\n" + ex.Message);
                }
            }
        }

        public void SendMessage(string messageToSend,IPEndPoint 目标端口)
        {
            lock (_lockObj)
            {
                try
                {
                    byte[] sendData = null;//要发送的字节数组
                    sendData = Encoding.UTF8.GetBytes(messageToSend);
                    udpClient?.Send(sendData, sendData.Length, 目标端口);//将数据发送到远程端点
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MES发送信息失败:\r\n" + ex.Message);
                }
            }
        }


        public string ReceiveMessage()
        {
            //不可以加锁，加锁后，会被一直占用，导致没有监听到信号，信息发送不出去
            //lock (_lockObj)
            //{
            byte[] receiveData = null;
            string receiveString = "";
            try
            {
                receiveData = udpClient.Receive(ref remote本地端口);//接收数据
                //byte数据转换为字符串数据
                //receiveString = Encoding.Default.GetString(receiveData);
                receiveString = Encoding.UTF8.GetString(receiveData);
                //udpClient.Close();//关闭连接

                string filePath = @".\MES接收日志\";
                if (!System.IO.Directory.Exists(filePath))
                    System.IO.Directory.CreateDirectory(filePath);  //如果文件不存在则新建

                filePath += DateTime.Now.ToString("yyyy-MM") + ".txt";
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(DateTime.Now.ToString("G"));
                    sw.WriteLine(remote本地端口.ToString());
                    sw.WriteLine(receiveString);
                    sw.WriteLine("-^8^--^*^--^-^--^.^--^8^--^*^--^-^--^.^-");
                    sw.WriteLine();
                }
            }
            catch (Exception ex)
            {
                ShowException($"Mes接收信息失败！", ex);
            }

            //Json字符串反序列化为具体类，并返回
            return receiveString;
            //}
        }

        

        #endregion

    }

    #region 5.辅助类
    #region 基类
    public class BaseItemData
    {
        [IniConfig]
        public string ItemTestProject { get; set; } = "";    //测试项目（匹配 WI 的名称）
        [IniConfig]
        public string ItemName { get; set; } = "";            //参数名称对应我们程序的计算名称
        [IniConfig]
        public string ItemType { get; set; } = "Craft";        //Device 设备相关、Product 产品相关、Craft 工艺相关（QCFC）、Test 测试相关（WI）
        [IniConfig]
        public string ItemIndex { get; set; } = "";
        [IniConfig]
        public string ItemSubType { get; set; } = "CCD";         //子类（PLC、CCD、ROBOT、SCADA、 ADDED、OTHER）
        [IniConfig]
        public string ItemSpecificationName { get; set; } = "";       //工艺规程名称（工序）对应我们程序的Viproccess.Name
        [IniConfig]
        public string ItemMaxValue { get; set; } = "";        //上限
        [IniConfig]
        public string ItemMinValue { get; set; } = "";        //下限
        [IniConfig]
        public string ItemSTDValue { get; set; } = "";        //标准值
        [IniConfig]
        public string ItemFixedValue { get; set; } = "";      //固定值(与前 3 项互斥)
        [IniConfig]
        public string ItemUnit { get; set; } = "";        //单位
        [IniConfig]
        public string ItemURL { get; set; } = "";         //文件、程序、模板路径（FTP）

    }
    #endregion


    #region 5.1.辅助类 MES参数下发相关类
    #region 5.1.1接收MES下发参数相关类
    public class DOWN_PARAMS
    {
        public string APIName { get; set; } = "";       //通信标识符固定为 NVT-MDC
        public string CMD { get; set; } = "";     //为指令代码集，包含 DOWN_PARAMS 参数下发指令、UPLOAD_PARAMS 参数上传指令、READ_PARAMS 参数读取指令、HEART _ CHECK 心跳检测指令。
        public string MachineNo { get; set; } = "";      //当前设备 ID
        public DOWN_PARAMS_Data Data { get; set; } = new DOWN_PARAMS_Data();       //为指令参数集合。具体含义和结构参考不同的指令集合。
    }

    public class DOWN_PARAMS_Data
    {
        public string SerialId { get; set; } = "";       //参数下发的时间戳，上位机反馈数据采集软件反馈的 SerialId 必须与接收的 SerialId 一致。
        public string Count { get; set; } = "";         //参数集合中参数的个数
        public DOWN_PARAMS_ItemData[] ItemData { get; set; }        //下发参数集合具体参见 ItemData 格式

    }
    public class DOWN_PARAMS_ItemData : BaseItemData
    {
       

    }
    #endregion

    #region 5.1.2接收MES下发参数完成反馈相关类
    public class DOWN_PARAMS_RESPOND
    {
        public string Status { get; set; } = "OK";      //固定值，参数接收与否都返回OK
        public string Message { get; set; } = "调用接口成功!";        //确认接收参数返回：调用接口成功!；拒接参数返回：人工拒绝接受!
        public string ErrorCode { get; set; } = "0000";     //确认接收返回"0000", 拒绝接收返回"0001"
        public string CMD { get; set; } = "DOWN_PARAMS";        //固定参数
        public DOWN_PARAMS_RESPOND_DATA Data { get; set; } = new DOWN_PARAMS_RESPOND_DATA();

    }
    public class DOWN_PARAMS_RESPOND_DATA
    {
        public string SerialId { get; set; } = "";      //参数下发的时间戳，上位机反馈数据采集软件反馈的 SerialId 必须与接收的 SerialId 一致。
        public string Count { get; set; } = "";         //接收到MES下发的参数集合中参数的个数

    }
    #endregion
    #endregion

    #region 5.2、软件参数上传至MES
    /// <summary>
    /// 上传参数至MES类：视觉模板或者视觉参数需要上传到 MES 系统中，用于设备发生故障文件参数丢失、设备参数化换型等。
    /// 当用户修改完之后，提示用户是否上传当前模板，用户选择上传设备视觉参数模板文件名
    /// </summary>
    public class UPLOAD_PARAMS
    {
        public string APIName { get; set; } = "NVT-MDC";       //通信标识符固定为 NVT-MDC
        public string CMD { get; set; } = "UPLOAD_PARAMS";     //为指令代码集，包含 DOWN_PARAMS 参数下发指令、UPLOAD_PARAMS 参数上传指令、READ_PARAMS 参数读取指令、HEART _ CHECK 心跳检测指令。
        public string MachineNo { get; set; } = "";      //当前设备 ID
        public UPLOAD_PARAMS_DATA Data { get; set; } = new UPLOAD_PARAMS_DATA();
    }
    public class UPLOAD_PARAMS_DATA
    {
        public string SerialId { get; set; } = "";       //参数下发的时间戳，上位机反馈数据采集软件反馈的 SerialId 必须与接收的 SerialId 一致。
        public string Count { get; set; } = "";         //参数集合中参数的个数
        public UPLOAD_PARAMS_ItemData[] ItemData { get; set; }        //下发参数集合具体参见 ItemData 格式
    }
    public class UPLOAD_PARAMS_ItemData:BaseItemData
    {
    }

    /// <summary>
    /// MES接收完参数信息后反馈的信息类
    /// </summary>
    public class UPLOAD_PARAMS_RESPOND
    {
        public string Status { get; set; } = "";      //参数接收成功返回"OK"
        public string Message { get; set; } = "";        //MES保存参数成功返回：调用接口成功!；拒接参数返回：人工拒绝接受!
        public string ErrorCode { get; set; } = "";     //MES保存参数成功返回"0000", 拒绝接收返回"0001"
        public string CMD { get; set; } = "";        //固定参数"UPLOAD_PARAMS"
        public UPLOAD_PARAMS_RESPOND_DATA Data { get; set; } = new UPLOAD_PARAMS_RESPOND_DATA();
    }

    public class UPLOAD_PARAMS_RESPOND_DATA
    {
        public string SerialId { get; set; } = "";      //参数上传的时间戳，上位机反馈数据采集软件反馈的 SerialId 必须与接收的 SerialId 一致。
        public string Count { get; set; } = "";        //上位机上传的参数集合中参数的个数
    }
    #endregion

    #region 5.3.参数点检
    #region 5.3.1接收MES参数点检指令
    /// <summary>
    /// 上位机接收MES点检指令类
    /// </summary>
    public class POINT_CHECK_PARAMS_CMD
    {
        public string APIName { get; set; } = "";
        public string CMD { get; set; } = "";       //点检固定参数："READ_PARAMS"
        public string MachineNo { get; set; } = "";     //设备ID
        public POINT_CHECK_PARAMS_DATA Data { get; set; } = new POINT_CHECK_PARAMS_DATA();
    }
    public class POINT_CHECK_PARAMS_DATA
    {
        public string SerialId { get; set; } = "";      //MES点检的时间戳，点检完成反馈的 SerialId 必须与接收的 SerialId 一致。
    }
    #endregion

    #region 5.3.2上位机反馈点检参数给MES
    /// <summary>
    /// 上位机反馈给MES点检信息类
    /// </summary>
    public class POINT_CHECK_PARAMS_RESPOND
    {
        public string Status { get; set; } = "OK";      //固定值
        public string Message { get; set; } = "调用接口成功!";       //点检成功，返回"调用接口成功!"
        public string ErrorCode { get; set; } = "0000";     //点检成功，返回"0000"
        public string CMD { get; set; } = "READ_PARAMS";    //点检反馈固定值
        public POINT_CHECK_PARAMS_RESPOND_DATA Data { get; set; } = new POINT_CHECK_PARAMS_RESPOND_DATA();
    }


    public class POINT_CHECK_PARAMS_RESPOND_DATA
    {
        public string SerialId { get; set; } = "";      //MES点检的时间戳，点检完成反馈的 SerialId 必须与接收的 SerialId 一致。
        public string Count { get; set; } = "0";         //MES点检指令中参数个数，默认为1
        public string SoftWare { get; set; } = "CCD";       //软件名称
        public string SoftWareVer { get; set; } = "V1.0.0.1";       //软件版本
        public POINT_CHECK_PARAMS_RESPOND_ItemData[] ItemData { get; set; }
    }

    public class POINT_CHECK_PARAMS_RESPOND_ItemData:BaseItemData
    {
        //public string ItemTestProject { get; set; } = "";       //测试项目（匹配 WI 的名称）
        //public string ItemName { get; set; } = "";      //参数名称
        //public string ItemType { get; set; } = "";      //Device 设备相关、Product 产品相关、Craft 工艺相关（QCFC）、Test 测试相关（WI）
        //public string ItemIndex { get; set; } = "";
        //public string ItemSubType { get; set; } = "";       //子类（PLC、CCD、ROBOT、SCADA、ADDED、OTHER）
        //public string ItemSpecificationName { get; set; } = "";         //工艺规程名称（工序）
        //public string ItemMaxValue { get; set; } = "";          //上限
        //public string ItemMinValue { get; set; } = "";          //下限
        //public string ItemSTDValue { get; set; } = "";          //标准值
        //public string ItemFixedValue { get; set; } = "";        //固定值(与前 3 项互斥)
        //public string ItemUnit { get; set; } = "mm";          //单位
        //public string ItemURL { get; set; } = "";       //文件、程序、模板路径（FTP）
    }
    #endregion

    #endregion


    #region 5.4.心跳信号
    //数据采集软件会定时发送心跳检查指令给 CCD 上位机或辅助设备上位机，CCD 上位机或辅助设备上位机收到信息回复信息即可。
    public class MES_HEART
    {
        public string APIName { get; set; }     //
        public string CMD { get; set; }     //
        public string MachineNo { get; set; }
        public MesHeartData Data { get; set; }//
    }
    public class MesHeartData
    {

    }

    //CCD 视觉软件接收到心跳信号处理完之后，正确的执行返回结果
    public class CCD_HEART
    {
        public string Status { get; set; } = "OK";
        public string Message { get; set; } = "调用接口成功!";
        public string ErrorCode { get; set; } = "0000";
        public string CMD { get; set; } = "HEART_CHECK";
        public CCDHeartData Data { get; set; } = new CCDHeartData();
    }
    public class CCDHeartData
    {

    }

    #endregion

    #region 5.5关闭端口信号
    public class MES_Close
    {
        public string CMD { get; set; } = "Close";
    }
    #endregion
    #endregion

}
