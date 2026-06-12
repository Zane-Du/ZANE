using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core
{
    /// <summary>
    /// 具体设备类：雅马哈机器人，使用网口 Tcp 通讯，根据指令读写点位、运动
    /// 可配置的属性包括：网口连接参数
    /// </summary>
    public class RobotYamahaTcp : AbstractDevice
    {

        #region 1. 字段、属性

        // 字段：Tcp 客户端，用于通讯。默认 IP 地址为：192.168.0.2，端口号 23，读取超时 500ms
        private TcpClient _tcpClient = new TcpClient();

        // 字段：用于读写网口时，锁线程
        private object _lockObj = new object();

        // 字段：用于 _tcpClient 读写操作的网络流对象。在 _tcpClient 连接后创建，关闭后释放
        private NetworkStream _netStream;

        /// <summary>
        /// 重写父类的属性：机器人的网口是否连接且打开
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return _tcpClient.Connected;
            }
        }

        /// <summary>
        /// 静态字典：《机器人名，机器人》。可通过名称找到对应的机器人
        /// </summary>
        public static Dictionary<string, RobotYamahaTcp> _dic_Name_Robot = new Dictionary<string, RobotYamahaTcp>();

        #endregion


        #region 2. 构造函数

        /// <summary>
        /// 带参实例构造函数：提供机器人的设备名，加载并设置参数，打开网口
        /// </summary>
        /// <param name="deviceName"></param>
        public RobotYamahaTcp(string deviceName) : base(deviceName)
        {
            OpenRobot();  //加载并设置参数，打开网口
            if (IsConnected)
            {
                _dic_Name_Robot.Add(deviceName, this); //将此机器人注册到静态列表中，后续可直接通过名称访问机器人
            }
        }

        #endregion


        #region 3. 可配置的属性

        /// <summary>
        /// 可配置的属性：网口的 IP 地址，默认值为 192.168.0.2
        /// </summary>
        [IniConfig]
        public string IP地址 { get; set; } = "192.168.0.2";


        /// <summary>
        /// 可配置的属性：网口的端口号，默认值为 23
        /// </summary>
        [IniConfig]
        public int 端口号 { get; set; } = 23;


        /// <summary>
        /// 可配置的属性：网口读取超时。读取等待超过此时间，视为未接收到数据。单位 ms，默认值为 1000
        /// </summary>
        [IniConfig]
        public int 读取超时 { get; set; } = 1000;


        /// <summary>
        /// 可配置的属性：网口在读取结果时的缓存区最大长度。默认值为 100
        /// </summary>
        [IniConfig]
        public int 读取结果长度 { get; set; } = 100;


        /// <summary>
        /// 可配置的属性：机器人是否为左手系。 默认为 false，即右手系
        /// </summary>
        [IniConfig]
        public bool 是否左手系 { get; set; } = false;

        #endregion


        #region 4. 主要功能：重写 Open / Close：打开、关闭网口

        /// <summary>
        /// 主要功能：重写父类方法：加载配置，并打开网口
        /// </summary>
        public override void Open()
        {
            //开网口
            if (!IsConnected)
            {
                OpenRobot();
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：关闭网口
        /// </summary>
        public override void Close()
        {
            //关网口
            if (IsConnected)
            {
                CloseRobot();
            }
        }


        // 私有方法：加载并设置参数，打开网口
        private void OpenRobot()
        {
            try
            {
                //网口关闭时，才能修改参数
                if (!_tcpClient.Connected)
                {
                    //设置参数
                    LoadConfigs(); //读取本地配置，设置参数：网口参数

                    //使用异步方式
                    //会出现界面已显示，机器人还未连接情况
                    //Task.Run(new Action(()=> {
                    try
                    {
                        _tcpClient.Connect(IP地址, 端口号); //连接网口
                        _netStream = _tcpClient.GetStream(); //获取读写的流对象   
                    }
                    catch (Exception ex)
                    {
                        ShowException("连接机器人网口失败！", ex);
                        _tcpClient.Dispose();
                        _tcpClient = new TcpClient();
                    }

                    //}));
                }

                //连接后，保存参数                       
                SaveConfigs(); //保存本地配置
            }
            catch (Exception ex)
            {
                ShowException("打开机器人网口失败！", ex);
            }
        }


        // 私有方法：关闭网口，释放相关资源
        private void CloseRobot()
        {
            try
            {
                _netStream.Close(); //关闭流对象
                _tcpClient.Close(); //关闭连接并释放                
            }
            catch (Exception ex)
            {
                ShowException("关闭机器人网口失败！", ex);
            }
        }

        #endregion


        #region 4. 主要功能：读写机器人坐标

        /// <summary>
        /// 主要功能：方法：获取机器人当前位置
        /// </summary>
        /// <param name="coord">机器人当前位置：X、Y、Z、R。如果获取失败，四个坐标都为 0.0</param>
        /// <returns>是否获取成功</returns>
        public bool GetCurrentPose(out double[] coord)
        {

            string strCmd = "@?WHRXY\r\n"; //指令：获取当前位置
            return GetResponseAndParseCoord(strCmd, out coord); //发送指令，解析响应，得到坐标
        }


        /// <summary>
        /// 主要功能：方法：读取点位 Pn 的坐坐标
        /// </summary>
        /// <param name="n">要读取的点位序号：0、1、2、3...</param>
        /// <param name="coord">点位 Pn 的坐标：X、Y、Z、R。如果获取失败，四个坐标都为 0.0</param>
        /// <returns>是否读取成功</returns>
        public bool ReadPn(int n, out double[] coord)
        {

            string strCmd = $"@?P{n}\r\n"; //指令：获取 Pn 的坐标
            return GetResponseAndParseCoord(strCmd, out coord); //发送指令，解析响应，得到坐标
        }

        /// <summary>
        /// 主要功能：方法：读取点位 Pn 的坐坐标 及手系
        /// </summary>
        /// <param name="n">要读取的点位序号：0、1、2、3...</param>
        /// <param name="coord">点位 Pn 的坐标：X、Y、Z、R。如果获取失败，四个坐标都为 0.0</param>
        /// <returns>是否读取成功</returns>
        public bool ReadPnWithHand(int n, out double[] coord, out int hand)
        {

            string strCmd = $"@?P{n}\r\n"; //指令：获取 Pn 的坐标
            return GetResponseAndParseCoord(strCmd, out coord, out hand); //发送指令，解析响应，得到坐标 手系
        }
        /// <summary>
        /// 主要功能：方法：写入点 Pn 的坐标：X、Y、Z、R。点位的手系，由可配置的属性 [是否左手系] 决定
        /// </summary>
        /// <param name="n">要写入的点位序号：0、1、2、3...</param>
        /// <param name="coord">点位 Pn 的坐标：X、Y、Z、R</param>
        /// <returns>是否写入成功</returns>
        public bool WritePn(int n, double[] coord)
        {

            if (coord.Length >= 4)
            {
                // 指令格式："@Pn = X.000 Y.000 Z.000 R.000 0 0 1\r\n"
                string strCmd = $"@P{n} = {coord[0]:F3} {coord[1]:F3} {coord[2]:F3} {coord[3]:F3} 0 0 ";
                string strHand = 是否左手系 ? "2\r\n" : "1\r\n"; //左手系 2，右手系 1

                return GetResponseAndParseResult(strCmd + strHand); //发送指令，解析响应，判断是否操作成功               
            }

            return false;
        }


        /// <summary>
        /// 主要功能：方法：写入点 Pn 的坐标：X、Y、Z、R，0，0，hand。带点位的手系
        /// </summary>
        /// <param name="n">要写入的点位序号：0、1、2、3...</param>
        /// <param name="coord">点位 Pn 的坐标：X、Y、Z、R</param>
        /// /// <param name="hand">手系：0（无）1（右手系） 2（左手系）</param>
        /// <returns>是否写入成功</returns>
        public bool WritePnWithHand(int n, double[] coord, int hand)
        {

            if (coord.Length >= 4)
            {
                // 指令格式："@Pn = X.000 Y.000 Z.000 R.000 0 0 1\r\n"
                string strCmd = $"@P{n} = {coord[0]:F3} {coord[1]:F3} {coord[2]:F3} {coord[3]:F3} 0 0 ";
                string strHand = $" {hand}\r\n"; //左手系 2，右手系 1,无 0

                return GetResponseAndParseResult(strCmd + strHand); //发送指令，解析响应，判断是否操作成功               
            }

            return false;
        }

        #endregion


        #region 4. 主要功能：机器人运动至指定点位

        /// <summary>
        /// 主要功能：方法：以 PTP 运动至点 Pn，并指定其速度
        /// </summary>
        /// <param name="n">目标点位序号：0、1、2、3...</param>
        /// 
        /// <param name="speed">设定的运动速度，范围 1-100</param>
        /// <returns>是否成功执行指令</returns>
        public bool MovePToPn(int n, int speed)
        {
            if (speed > 0 && speed <= 100)
            {
                // 指令格式："@MOVE P,Pn,S=speed\r\n"
                string strCmd = $"@MOVE P,P{n},S=speed\r\n";
                return GetResponseAndParseResult(strCmd); //发送指令，解析响应，判断是否操作成功  
            }

            return false;
        }


        /// <summary>
        /// 主要功能：方法：以门型运动至点 Pn，并指定其门型高度和速度
        /// </summary>
        /// <param name="n">目标点位序号：0、1、2、3...</param>
        /// <param name="speed">设定的运动速度，范围 1-100</param>
        /// <param name="z">门型高度，范围 ≥ 0.0，默认值为 0.0</param>
        /// <returns>是否成功执行指令</returns>
        public bool MoveZToPn(int n, int speed, double z = 0.0)
        {
            if (speed > 0 && speed <= 100 && z >= 0.0)
            {
                // 指令格式："@MOVE P,Pn,Z=z,S=speed\r\n"
                string strCmd = $"@MOVE P,P{n},Z={z:F3},S=speed\r\n";
                return GetResponseAndParseResult(strCmd); //发送指令，解析响应，判断是否操作成功  
            }

            return false;
        }

        #endregion


        #region 6. 私有方法：发送指令，读取网口返回数据，并解析指令

        // 私有方法：发送指令，解析响应，判断是否操作成功
        private bool GetResponseAndParseResult(string cmd)
        {

            string strRes = SendCommand(cmd); //发送指令，等待响应

            //判断返回字符串是否合法：不为空，并且包含 "OK"
            if (!string.IsNullOrEmpty(strRes))
            {
                return strRes.Contains("OK");
            }

            return false;
        }


        // 私有方法：发送指令，解析响应，得到坐标：X、Y、Z、R
        private bool GetResponseAndParseCoord(string cmd, out double[] coord)
        {

            string strRes = SendCommand(cmd); //发送指令，等待响应

            //判断返回字符串是否合法
            if (!string.IsNullOrEmpty(strRes))
            {
                //以空格分隔
                string[] arr = strRes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //解析各元素为 double
                if (arr.Length >= 4)
                {
                    //初始化返回的数组
                    coord = new double[4]; // X、Y、Z、R
                    bool bOK0 = double.TryParse(arr[0], out coord[0]); //X
                    bool bOK1 = double.TryParse(arr[1], out coord[1]); //Y
                    bool bOK2 = double.TryParse(arr[2], out coord[2]); //Z
                    bool bOK3 = double.TryParse(arr[3], out coord[3]); //R
                    if (bOK0 && bOK1 && bOK2 && bOK3)
                    {
                        return true;
                    }
                }
            }

            // 发生错误：重置数组每个元素为 0
            coord = new double[4];
            return false;
        }

        // 私有方法：发送指令，解析响应，得到坐标：X、Y、Z、R Hand
        private bool GetResponseAndParseCoord(string cmd, out double[] coord, out int hand)
        {

            string strRes = SendCommand(cmd); //发送指令，等待响应

            //判断返回字符串是否合法
            if (!string.IsNullOrEmpty(strRes))
            {
                //以空格分隔
                string[] arr = strRes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //解析各元素为 double
                if (arr.Length >= 7)
                {
                    //初始化返回的数组
                    coord = new double[4]; // X、Y、Z、R         
                    bool bOK0 = double.TryParse(arr[0], out coord[0]); //X
                    bool bOK1 = double.TryParse(arr[1], out coord[1]); //Y
                    bool bOK2 = double.TryParse(arr[2], out coord[2]); //Z
                    bool bOK3 = double.TryParse(arr[3], out coord[3]); //R
                    bool bOK4 = int.TryParse(arr[6], out hand); //Hand
                    if (bOK0 && bOK1 && bOK2 && bOK3 && bOK4)
                    {
                        return true;
                    }
                }
            }

            // 发生错误：重置数组每个元素为 0
            hand = 0;
            coord = new double[4];
            return false;
        }
        // 私有方法：发送指令，并返回收到的结果
        private string SendCommand(string cmd)
        {

            //如果机器人已连接
            if (IsConnected)
            {
                lock (_lockObj)
                { //锁线程
                    try
                    {
                        //发送前：清空接收缓存
                        if (_netStream.DataAvailable)
                        {
                            byte[] bufRead0 = new byte[读取结果长度];
                            _netStream.Read(bufRead0, 0, 读取结果长度);
                        }

                        //向服务器发送指令
                        byte[] bufWrite = Encoding.Default.GetBytes(cmd);
                        _netStream.Write(bufWrite, 0, bufWrite.Length); //写入流
                        Thread.Sleep(50); //等待返回结果

                        //接收返回的字符串
                        _netStream.ReadTimeout = 读取超时;   //读取超时，默认1秒钟
                        byte[] bufRead = new byte[读取结果长度];
                        int bufLength = _netStream.Read(bufRead, 0, 读取结果长度); //读取流，返回有效数据长度
                        string strRes = Encoding.Default.GetString(bufRead, 0, bufLength); //截取有效数据，并编码为字符串
                        strRes = strRes.Trim().Replace("\r", "").Replace("\n", ""); //去除首位空格和换行符

                        // 接收到的响应不为空，返回
                        if (!string.IsNullOrEmpty(strRes))
                        {
                            return strRes;
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("机器人 [" + DeviceName + "] 发送指令 [" + cmd + "] 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return ""; //发生错误，返回空字符串
        }



        #endregion

    }// class

}// namespace
