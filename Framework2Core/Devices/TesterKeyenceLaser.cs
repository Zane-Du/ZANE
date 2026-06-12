using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework2Core
{
  /// <summary>
  /// 具体类：基恩士激光多点传感器，使用网口 Tcp 通讯，接收指令，并返回测试结果
  /// 可配置的属性包括：网口连接参数
  /// </summary>
  public class TesterKeyenceLaser : AbstractDevice
  {

    #region 1. 字段、属性

    // 字段：Tcp 客户端，用于通讯。默认 IP 地址为：192.168.0.15，端口号 64000，读取超时 1000ms
    private TcpClient _tcpClient = new TcpClient();

    // 字段：用于 _tcpClient 读写操作的网络流对象。在 _tcpClient 连接后创建，关闭后释放
    private NetworkStream _netStream;

    // 字段：用于读写网口时，锁线程
    private object _lockObj = new object();

    /// <summary>
    /// 重写父类的属性：传感器的网口是否连接且打开
    /// </summary>
    public override bool IsConnected
    {
      get
      {
        return _tcpClient.Connected;
      }
    }

    #endregion


    #region 2. 构造函数

    /// <summary>
    /// 带参实例构造函数：提供传感器的设备名，加载并设置参数，打开网口
    /// </summary>
    /// <param name="deviceName"></param>
    public TesterKeyenceLaser(string deviceName) : base(deviceName)
    {
      OpenTester();  //加载并设置参数，打开网口
    }

    #endregion


    #region 3. 可配置的属性

    /// <summary>
    /// 可配置的属性：网口的 IP 地址，默认值为 192.168.0.15
    /// </summary>
    [IniConfig]
    public string IP地址 { get; set; } = "192.168.0.15";


    /// <summary>
    /// 可配置的属性：网口的端口号，默认值为 64000
    /// </summary>
    [IniConfig]
    public int 端口号 { get; set; } = 64000;


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
    /// 主要功能：重写父类方法：关闭网口
    /// </summary>
    public override void Close()
    {
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
              ShowException("连接激光传感器网口失败！", ex);
            }

          }));
        }

        //连接后，保存参数                       
        SaveConfigs(); //保存本地配置
      }
      catch (Exception ex)
      {
        ShowException("打开激光传感器网口失败！", ex);
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
        ShowException("关闭激光传感器网口失败！", ex);
      }
    }

    #endregion


    #region 4. 主要功能：读取测试数据

    public void TrigTest()
    {

      lock (_lockObj)
      {
        try
        {
          //发送前：清空接收缓存
          if (_netStream.DataAvailable)
          {
            byte[] bufRead0 = new byte[读取结果长度];
            _netStream.Read(bufRead0, 0, 读取结果长度);
          }

          //向服务器发送指令
          string cmd0 = "SW,01,161,+000000000\r\n"; // 指令：0
          byte[] bufWrite0 = Encoding.Default.GetBytes(cmd0);
          _netStream.Write(bufWrite0, 0, bufWrite0.Length); //写入流
          Thread.Sleep(50); //等待写入

          //发送前：清空接收缓存
          if (_netStream.DataAvailable)
          {
            byte[] bufRead0 = new byte[读取结果长度];
            _netStream.Read(bufRead0, 0, 读取结果长度);
          }

          //向服务器发送指令
          string cmd1 = "SW,01,161,+000000001\r\n"; // 指令：1
          byte[] bufWrite1 = Encoding.Default.GetBytes(cmd1);
          _netStream.Write(bufWrite1, 0, bufWrite1.Length); //写入流
          Thread.Sleep(50); //等待写入
        }
        catch (Exception ex)
        {
          LocalLogSave.WriteErrorLog("激光传感器 [" + DeviceName + "] 触发测试失败！\r\n异常信息:\r\n" + ex.ToString());
        }
      }
    }

    /// <summary>
    /// 主要功能：方法：读取激光传感器的结果，并取工具 3 的数据最为最终结果
    /// </summary>
    /// <param name="res">输出参数：工具 3 解析后的数据</param>
    /// <param name="orignalData">输出参数：传感器返回的原始数据</param>
    /// <returns></returns>
    public bool ReadTestResult(out double res, out string orignalData)
    {

      orignalData = "";
      lock (_lockObj)
      {
        try
        {
          //发送前：清空接收缓存
          if (_netStream.DataAvailable)
          {
            byte[] bufRead0 = new byte[读取结果长度];
            _netStream.Read(bufRead0, 0, 读取结果长度);
          }

          //向服务器发送指令
          string cmd = "M0\r\n"; // 指令：读取结果
          byte[] bufWrite = Encoding.Default.GetBytes(cmd);
          _netStream.Write(bufWrite, 0, bufWrite.Length); //写入流
          Thread.Sleep(100); //等待测试结果

          //接收返回的数据：M0, 工具1, 工具2, 工具3
          _netStream.ReadTimeout = 读取超时;   //读取超时，默认1秒钟
          byte[] bufRead = new byte[读取结果长度];
          int bufLength = _netStream.Read(bufRead, 0, 读取结果长度); //读取流，返回有效数据长度
          orignalData = Encoding.Default.GetString(bufRead, 0, bufLength); //截取有效数据，并编码为字符串
          string strRes = orignalData.Trim().Replace("\r", "").Replace("\n", ""); //去除空格和换行符
          var splits = strRes.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries); //按照逗号分隔
          if (splits.Length > 3)
          {
            return double.TryParse(splits[3], out res); //取工具3的结果
          }
        }
        catch (Exception ex)
        {
          LocalLogSave.WriteErrorLog("激光传感器 [" + DeviceName + "] 读取测试数据失败！\r\n异常信息:\r\n" + ex.ToString());
        }

        res = -99999998;
        return false;
      }
    }

    #endregion
  }
}
