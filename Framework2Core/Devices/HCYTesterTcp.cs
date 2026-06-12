using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Framework2Core
{
  /// <summary>
  /// 具体设备类：HCY测试仪，使用网口 Tcp 通讯，收发指令，并获取测试结果
  /// 可配置的属性包括：网口连接参数
  /// </summary>
  public class HCYTesterTcp : AbstractDevice
  {

    #region 1. 字段、属性

    // 字段：Tcp 客户端，用于通讯。默认 IP 地址为：192.168.100.250，端口号 9004，读取超时 1000ms
    private TcpClient _tcpClient = new TcpClient();

    // 字段：用于 _tcpClient 读写操作的网络流对象。在 _tcpClient 连接后创建，关闭后释放
    private NetworkStream _netStream;

    // 字段：用于读写网口时，锁线程
    private object _lockObj = new object();

    /// <summary>
    /// 重写父类的属性：扫码枪的网口是否连接且打开
    /// </summary>
    public override bool IsConnected
    {
      get
      {
        return _tcpClient.Connected;
      }
    }


    /// <summary>
    /// 重写父类的属性：测试仪是否正在运行：扫码中
    /// </summary>
    public override bool IsRunning
    {
      get
      {
        return _isRunning;
      }
    }
    private bool _isRunning = false;

    #endregion


    #region 2. 构造函数

    /// <summary>
    /// 带参实例构造函数：提供测试仪的设备名，加载并设置参数，打开网口
    /// </summary>
    /// <param name="deviceName"></param>
    public HCYTesterTcp(string deviceName) : base(deviceName)
    {
      OpenTester();  //加载并设置参数，打开网口
    }

    #endregion


    #region 3. 可配置的属性

    /// <summary>
    /// 可配置的属性：网口的 IP 地址，默认值为 192.168.100.100
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

    private string addrIP = "192.168.100.250";

    /// <summary>
    /// 可配置的属性：网口的端口号，默认值为 9004
    /// </summary>
    [IniConfig]
    public int 端口号
    {
      get
      {
        return port;
      }
      set
      {
        if (value != 0)
          port = value;
      }
    }


    private int port = 8000;
    /// <summary>
    /// 可配置的属性：网口读取超时。读取等待超过此时间，视为未接收到数据。单位 ms，默认值为 1000
    /// </summary>
    [IniConfig]
    public int 读取超时
    {
      get
      {
        return overTime;
      }
      set
      {
        if (value != 0)
          overTime = value;
      }
    }
    private int overTime = 9000;


    /// <summary>
    /// 可配置的属性：网口在读取测试仪数据时的缓存区最大长度。默认值为 100
    /// </summary>
    [IniConfig]
    public int 读取长度
    {
      get
      {
        return codeLength;
      }
      set
      {
        if (value != 0)
          codeLength = value;
      }
    }



    private int codeLength = 100;
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
        OpenTester();
      }
    }


    /// <summary>
    /// 主要功能：重写父类方法：停止测试仪，并关闭网口
    /// </summary>
    public override void Close()
    {
      //停止运行
      if (IsRunning)
      {
        StopRunning();
      }

      //关网口
      if (IsConnected)
      {
        CloseTester();
      }
    }


    // 私有方法：加载并设置参数，打开网口
    private void OpenTester()
    {
      try
      {
        //网口关闭时，才能修改参数
        if (!_tcpClient.Connected)
        {
          //设置参数
          LoadConfigs(); //读取本地配置，设置参数：网口参数
          //使用异步方式
          Task.Run(new Action(() =>
          {
            try
            {
              _tcpClient.Connect(IP地址, 端口号); //连接网口
              _netStream = _tcpClient.GetStream(); //获取读写的流对象   
            }
            catch (Exception ex)
            {
              ShowException("连接测试仪网口失败！", ex);
            }

          }));
        }

        //连接后，保存参数                       
        SaveConfigs(); //保存本地配置
      }
      catch (Exception ex)
      {
        ShowException("打开测试仪网口失败！", ex);
      }
    }


    // 私有方法：关闭网口，释放相关资源
    private void CloseTester()
    {
      try
      {
        _netStream.Close(); //关闭流对象
        _tcpClient.Close(); //关闭连接并释放                
      }
      catch (Exception ex)
      {
        ShowException("关闭测试仪网口失败！", ex);
      }
    }

    #endregion


    #region 4. 主要功能：开始运行、停止运行

    /// <summary>
    /// 主要功能：重写父类方法：开始运行：开网口
    /// </summary>
    public override void StartRunning()
    {
      //打开网口
      if (!IsConnected)
      {
        Open();
      }
    }


    /// <summary>
    /// 主要功能：重写父类方法：停止运行：停止
    /// </summary>
    public override void StopRunning()
    {
      //停止扫码
      if (IsRunning)
      {
       
      }
    }


    /// <summary>
    /// 主要功能：方法：启动测试一次
    /// 	命令格式：START:通道号,PackBarcode\r\n
    /// 	示例说明：START:CH1,PackSN,CellSN\r\n 通道1启动测试，
    /// 	HYC返回格式：
    ///     START:CH1,OK\r\n 通道1启动测试OK
    ///	    START:CH1,NG\r\n 通道1启动测试NG
    /// </summary>
    /// <returns>如果测试成功，返回测试结果；如果测试失败，停止测试并返回 ERROR</returns>
    public string TestOnce(string packBarcode="", string CellBarcode = "", string channal = "CH1")
    {

      //如果测试仪已连接
      if (IsConnected)
      {
        lock (_lockObj)
        {
          _isRunning = true; //正在扫码中
          try
          {
            //发送前：清空接收缓存
            if (_netStream.DataAvailable)
            {
              byte[] bufRead0 = new byte[读取长度];
              _netStream.Read(bufRead0, 0, 读取长度);
            }
            //向服务器发送指令
            byte[] bufWrite = Encoding.Default.GetBytes($"START:{channal},{packBarcode},{CellBarcode}\r\n"); //指令：启动测试
            _netStream.Write(bufWrite, 0, bufWrite.Length); //写入流
            Thread.Sleep(50); //等待
            if (_netStream.DataAvailable)//接收前判断有无数据
            {
              //接收返回的字符串
              _netStream.ReadTimeout = 读取超时;   //读取超时，默认1秒钟
              byte[] bufRead = new byte[读取长度];
              int bufLength = _netStream.Read(bufRead, 0, 读取长度); //读取流，返回有效数据长度
              string strRes = Encoding.Default.GetString(bufRead, 0, bufLength); //截取有效数据，并编码为字符串
              strRes = strRes.Trim().Replace("\r", "").Replace("\n", ""); //去除空格和换行符

              //判断返回字符串是否合法
              if (!(string.IsNullOrEmpty(strRes)))
              {
                _isRunning = false; //返回字符串合法，启动测试结束
                return strRes;
              }
            }
            else
              return "启动测试对方无响应 ";
          }
          catch (Exception ex)
          {
            LocalLogSave.WriteErrorLog($"测试仪 [{ DeviceName }] -》[{channal}]启动失败！\r\n异常信息:\r\n" + ex.ToString());
            return "启动测试异常";
          }
         
        }// lock
      }// if (IsConnected)

      return "ERROR"; //测试仪未连接，或发生错误，返回 ERROR
    }
    /// <summary>
    /// 获取通道状态信号
    ///   	命令格式： STATUS:通道号;
    ///   	示例说明：STATUS:CH1\r\n   查询通道1状态
    ///   
    /// 	HYC返回格式：
    /// 	 STATUS:CH1,RUN\r\n
    ///      RUN(测试中),STOP(屏蔽中),WAIT(等待测试)
    /// </summary>
    /// <param name="channal">通道号：CH1</param>
    /// <returns></returns>
    public string GetTesterStatus(string channal= "CH1")
    {
      string rst = "ERROR";
      try
      {

        //如果测试仪已连接
        if (IsConnected)
        {
          lock (_lockObj)
          {
          
            try
            {
              //发送前：清空接收缓存
              if (_netStream.DataAvailable)
              {
                byte[] bufRead0 = new byte[读取长度];
                _netStream.Read(bufRead0, 0, 读取长度);
              }
              //向服务器发送指令
              byte[] bufWrite = Encoding.Default.GetBytes($"STATUS:{channal}\r\n"); //指令：启动测试
              _netStream.Write(bufWrite, 0, bufWrite.Length); //写入流
              Thread.Sleep(50); //等待扫码

              if (_netStream.DataAvailable)
              {
                //接收返回的字符串
                _netStream.ReadTimeout = 读取超时;   //读取超时，默认1秒钟
                byte[] bufRead = new byte[读取长度];
                int bufLength = _netStream.Read(bufRead, 0, 读取长度); //读取流，返回有效数据长度
                string strRes = Encoding.Default.GetString(bufRead, 0, bufLength); //截取有效数据，并编码为字符串
                strRes = strRes.Trim().Replace("\r", "").Replace("\n", ""); //去除空格和换行符

                //判断返回字符串是否合法
                if (!(string.IsNullOrEmpty(strRes)))
                {
                  return strRes;
                }
              }
              else
                rst = "获取测试仪状态对方未响应";
            }
            catch (Exception ex)
            {
              LocalLogSave.WriteErrorLog($"测试仪 [{ DeviceName }] -》[{channal}]获取状态失败！\r\n异常信息:\r\n" + ex.ToString());
            }

          
          }// lock
        }
      }
      catch (Exception)
      { }
      return rst;
    }
    /// <summary>
    /// 获取测试结果
    /// 	命令格式：RESULT:通道号, PackSN \r\n
    /// 	示例说明：RESULT:CH1,,PackSN \r\n  询问通道1电池F8Y05134ZZJ02JN7X+044测试结果
    /// 	HYC返回格式：
    /// 	RESULT:CH1,F8Y05134ZZJ02JN7X+044,OK\r\n  通道1电池F8Y05134ZZJ02JN7X+044测试OK
    /// 	RESULT:CH1, F8Y05134ZZJ02JN7X+044,NG\r\n  通道1电池F8Y05134ZZJ02JN7X+044测试NG
    /// 	RESULT:CH1,F8Y05134ZZJ02JN7X+044,TESTING\r\n  通道1电池F8Y05134ZZJ02JN7X+044测试中
    /// </summary>
    /// <param name="channal"></param>
    /// <returns></returns>
    public string GetTestResult(string packBarcode = "", string channal = "CH1")
    {
      string rst = "ERROR";
      try
      {

        //如果测试仪已连接
        if (IsConnected )
        {
          lock (_lockObj)
          {
            try
            {
              //发送前：清空接收缓存
              if (_netStream.DataAvailable)
              {
                byte[] bufRead0 = new byte[读取长度];
                _netStream.Read(bufRead0, 0, 读取长度);
              }
              //向服务器发送指令
              byte[] bufWrite = Encoding.Default.GetBytes($"RESULT:{channal},{packBarcode} \r\n"); //指令：获取测试结果。
              _netStream.Write(bufWrite, 0, bufWrite.Length); //写入流
              Thread.Sleep(50); //等待扫码

              if (_netStream.DataAvailable)
              {
                //接收返回的字符串
                _netStream.ReadTimeout = 读取超时;   //读取超时，默认1秒钟
                byte[] bufRead = new byte[读取长度];
                int bufLength = _netStream.Read(bufRead, 0, 读取长度); //读取流，返回有效数据长度
                string strRes = Encoding.Default.GetString(bufRead, 0, bufLength); //截取有效数据，并编码为字符串
                strRes = strRes.Trim().Replace("\r", "").Replace("\n", ""); //去除空格和换行符

                //判断返回字符串是否合法
                if (!(string.IsNullOrEmpty(strRes)))
                {
                  _isRunning = false; //返回字符串合法，启动测试结束
                  return strRes;
                }
              }
              else
                rst = "获取结果数据未响应";
            }
            catch (Exception ex)
            {
              LocalLogSave.WriteErrorLog($"测试仪 [{ DeviceName }] -》[{channal}]获取测试结果失败！\r\n异常信息:\r\n" + ex.ToString());
              return "读取测试仪数据超时";
            }
          
          }// lock
        }
      }
      catch (Exception ex)
      { }
      return rst;
    }


    #endregion

  }// class

}// namespace
