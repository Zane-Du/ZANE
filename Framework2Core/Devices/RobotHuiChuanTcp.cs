using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core
{
  public class RobotHuichuanTcp : AbstractDevice
  {
    #region 字段、属性
    // 字段：用于读写网口时，锁线程
    private object _lockObj = new object();

    private Socket sSocket;
    private IPAddress iHostIP;
    private IPEndPoint iPoint;
    #endregion

    /// <summary>
    /// 重写父类的属性：机器人的网口是否连接且打开
    /// </summary>
    public override bool IsConnected
    {
      get
      {
        if (sSocket == null) 
          return false;
        return sSocket.Connected;
      }
    }

    /// <summary>
    /// 静态字典：《机器人名，机器人》。可通过名称找到对应的机器人
    /// </summary>
    public static Dictionary<string, RobotHuichuanTcp> _dic_Name_Robot = new Dictionary<string, RobotHuichuanTcp>();

    #region 2. 构造函数

    /// <summary>
    /// 带参实例构造函数：提供机器人的设备名，加载并设置参数，打开网口
    /// </summary>
    /// <param name="deviceName"></param>
    public RobotHuichuanTcp(string deviceName) : base(deviceName)
    {
      OpenNetPort();  //加载并设置参数，打开网口
      _dic_Name_Robot.Add(deviceName, this); //将此机器人注册到静态列表中，后续可直接通过名称访问机器人
    }

    #endregion

    #region 3. 可配置的属性


    /// <summary>
    /// 可配置的属性：网口的 IP 地址，默认值为 192.168.0.2
    /// </summary>
    [IniConfig]
    public string IP地址工控机
    {
      get
      {
        return addrIPComputer;
      }
      set
      {
        if (!string.IsNullOrEmpty(value))
          addrIPComputer = value;
      }
    }
    private string addrIPComputer = "192.168.0.2";

    /// <summary>
    /// 可配置的属性：网口的 IP 地址，默认值为 192.168.0.2
    /// </summary>
    [IniConfig]
    public string IP地址
    {
      get
      {
        return addrIP;
      }
      set
      {
        if (!string.IsNullOrEmpty(value))
          addrIP = value;
      }
    }
    private string addrIP = "192.168.0.2";

    /// <summary>
    /// 可配置的属性：网口的端口号，默认值为 23
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

    private int port = 23;
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
        OpenNetPort();
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
        CloseNetPort();
      }
    }

    private void OpenNetPort()
    {
      try
      {
        //网口关闭时，才能修改参数
        if (sSocket == null || !sSocket.Connected)
        {
          //设置参数
          LoadConfigs(); //读取本地配置，设置参数：网口参数
          iHostIP = IPAddress.Parse(IP地址);//将 IP 地址字符串转换为 IPAddress 实例
          iPoint = new IPEndPoint(iHostIP, 端口号); //用指定的地址和端口号初始化
          sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//使用指定的地址族、套接字类型和协议初始化
                                                                                                
          //IPAddress localIP = IPAddress.Parse("169.254.50.100");//指定工控机IP（工控机）
          IPAddress localIP = IPAddress.Parse(IP地址工控机);//指定工控机IP（工控机）
          //指定工控机端口（非1025即可）
          IPEndPoint localEP = new IPEndPoint(localIP, 3000);
          //指定工控机端口（非1025即可）
          IPEndPoint localEP1 = new IPEndPoint(localIP, 3001);
          //使用异步方式
          //会出现界面已显示，机器人还未连接情况
          //Task.Run(new Action(()=> {
          try
          {
            if (iHostIP.ToString() == "169.254.50.5")
            {
              sSocket.Bind(localEP);
            }
            else
            {
              sSocket.Bind(localEP1);
            }

            sSocket.Connect(IP地址, 端口号); //连接网口
          }
          catch (Exception ex)
          {
            ShowException("连接机器人网口失败！", ex);
          }

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
    private void CloseNetPort()
    {
      try
      {
        sSocket.Close(); //关闭流对象    
      }
      catch (Exception ex)
      {
        ShowException("关闭机器人网口失败！", ex);
      }
    }

    #endregion

    #region 4. 主要功能：获取机器人当前坐标

    /// <summary>
    /// 主要功能：方法：获取机器人当前位置
    /// </summary>
    /// <param name="coord">机器人当前位置：X、Y、Z、R。如果获取失败，四个坐标都为 0.0</param>
    /// <returns>是否获取成功</returns>
    public bool GetCurrentPose(out double[] coord)
    {

      string strCmd = "GetCurrentLocation,"; //指令：获取当前位置
      return GetResponseAndParseCoord(strCmd, out coord); //发送指令，解析响应，得到坐标
    }
    #endregion

    #region 4. 主要功能：发送纠偏点位
    /// <summary>
    /// 发送偏移量
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="R"></param>
    /// <returns></returns>
    public bool SendFangLiaoLocation(double[] offset)
    {
      string strCmd = offset[0].ToString("F3") + "," + offset[1].ToString("F3") + "," + offset[3].ToString("F3");
      return GetResponseAndParseResult(strCmd);
    }

    /// <summary>
    /// 发送绝对坐标 不带目标点位序号
    /// </summary>
    /// 
    public bool SendPosLocation(double[] coord)
    {
      string strCmd = coord[0].ToString("F4") + "," + coord[1].ToString("F4") + "," + coord[3].ToString("F4");
      return GetResponseAndParseResult(strCmd); //发送指令，解析响应，判断是否操作成功  
    }

    /// <summary>
    /// 发送绝对坐标
    /// </summary>
    /// <param name="n">目标点位序号：0、1、2、3...</param>
    public bool SendPosLocation(int n, double[] coord)
    {
      string strCmd = n + "," + coord[0].ToString("F4") + "," + coord[1].ToString("F4") + "," + coord[3].ToString("F4");
      return GetResponseAndParseResult(strCmd); //发送指令，解析响应，判断是否操作成功  
    }

    /// <summary>
    /// 发送料盘基准坐标以及行列和行列间距
    /// </summary>
    /// <param name="n"></param>
    /// <param name="coord">X,Y,U,机器人坐标</param>
    /// <param name="palletPara">料盘行列以及行列间距</param>
    /// <returns></returns>
    public bool SendPalletPara(int n, double[] coord, double[] palletPara)
    {
      string strCmd = n + "," + coord[0].ToString("F4") + "," + coord[1].ToString("F4") + "," + coord[3].ToString("F4") + "," + palletPara[0].ToString("f4") + "," + palletPara[1].ToString("f4") + "," + palletPara[2].ToString("f4") + "," + palletPara[3].ToString("f4");
      return GetResponseAndParseResult(strCmd); //发送指令，解析响应，判断是否操作成功  
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
            if (!sSocket.Connected) OpenNetPort();
            string strReturn = string.Empty;

            //向服务器发送指令
            byte[] bufWrite = Encoding.ASCII.GetBytes(cmd);
            sSocket.Send(bufWrite, bufWrite.Length, 0);

            byte[] receiveByte = new byte[23];
            sSocket.ReceiveTimeout = 2000;
            int iBuffer = sSocket.Receive(receiveByte, receiveByte.Length, 0);
            strReturn = Encoding.ASCII.GetString(receiveByte, 0, iBuffer);

            strReturn = strReturn.Trim().Replace("\r", "").Replace("\n", ""); //去除首位空格和换行符

            // 接收到的响应不为空，返回
            if (!string.IsNullOrEmpty(strReturn))
            {
              return strReturn;
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

    /// <summary>
    /// 发送纠偏点位
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="R"></param>
    /// <returns></returns>
    public bool SendFangLiaoLocation(int N, double X, double Y, double R)
    {
      //如果机器人已连接
      if (IsConnected)
      {
        lock (_lockObj)
        { //锁线程
          if (!sSocket.Connected) OpenNetPort();
          string strReturn = string.Empty;
          try
          {
            string strSend = "P" + N + "," + X.ToString("F3") + "," + Y.ToString("F3") + "," + R.ToString("F3");
            byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(strSend);
            sSocket.Send(sendByte, sendByte.Length, 0);

            byte[] receiveByte = new byte[23];
            sSocket.ReceiveTimeout = 2000;
            int iBuffer = sSocket.Receive(receiveByte, receiveByte.Length, 0);
            strReturn = Encoding.ASCII.GetString(receiveByte, 0, iBuffer);
          }
          catch (Exception)
          { }

          return strReturn.Contains("OK");
        }
      }
      else
      {
        MessageBox.Show("爱普生机器人未连接！");
      }
      return false;
    }
    /// <summary>
    /// 发送托盘参数
    /// </summary>
    /// <param name="PalletPara"></param>
    /// <returns></returns>
    public bool SendPalletPara1(double[] PalletPara)
    {
      //如果机器人已连接
      if (IsConnected)
      {
        lock (_lockObj)
        { //锁线程
          if (!sSocket.Connected) OpenNetPort();
          string strReturn = string.Empty;
          try
          {
            string strSend = "2," + PalletPara[0] + "," + PalletPara[1] + "," + PalletPara[2] + "," + PalletPara[3] + "," + PalletPara[4] + "," + PalletPara[5] + "," + PalletPara[6] + "," + PalletPara[7] + "," + PalletPara[8] + "," + PalletPara[9] + "," + PalletPara[10] + "," + PalletPara[11] + "," + PalletPara[12] + "," + PalletPara[13] + "," + PalletPara[14];
            byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(strSend);
            sSocket.Send(sendByte, sendByte.Length, 0);

            byte[] receiveByte = new byte[23];
            sSocket.ReceiveTimeout = 2000;
            int iBuffer = sSocket.Receive(receiveByte, receiveByte.Length, 0);
            strReturn = Encoding.ASCII.GetString(receiveByte, 0, iBuffer);
          }
          catch
          { }

          return strReturn.Contains("OK");
        }
      }
      else
      {
        MessageBox.Show("机器人未连接");
      }
      return false;
    }

    /// <summary>
    /// 获取基准放料位
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="R"></param>
    /// <returns></returns>
    public bool GetCurrentRobotLocation(out double X, out double Y, out double R)
    {
      //如果机器人已连接
      if (IsConnected)
      {
        lock (_lockObj)
        { //锁线程
          string strSend = "3";
          byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(strSend);
          sSocket.Send(sendByte, sendByte.Length, 0);

          byte[] receiveByte = new byte[1024];
          sSocket.ReceiveTimeout = 2000;
          int iBuffer = sSocket.Receive(receiveByte, receiveByte.Length, 0);
          string strReturn = Encoding.ASCII.GetString(receiveByte, 0, iBuffer);
          try
          {
            string[] strArray = strReturn.Split(new char[] { ',' });
            X = double.Parse(strArray[0].Trim());
            Y = double.Parse(strArray[1].Trim());
            R = double.Parse(strArray[2].Trim());
          }
          catch
          {
            X = 0; Y = 0; R = 0;
            return false;
          }
          return true;
        }
      }
      else
      {
        MessageBox.Show("机器人未连接");
      }

      X = 0; Y = 0; R = 0;
      return false;
    }
  }
}
