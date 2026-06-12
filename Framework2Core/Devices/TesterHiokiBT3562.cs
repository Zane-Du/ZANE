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
  /// 具体设备类：日置3562电压内阻测试仪，使用串口通讯，接收指令并返回电压及内阻值
  /// 可配置的属性包括：串口连接参数、读取次数、读取间隔
  /// </summary>
  public class TesterHiokiBT3562 : AbstractDevice
  {

    #region 1. 字段、普通属性

    // 字段：串口，用于通讯。默认参数为 9600、8、1、N；读取超时 500ms
    private SerialPort _serialPort = new SerialPort()
    {
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
    public override bool IsConnected
    {
      get
      {
        return _serialPort.IsOpen;
      }
    }
    #endregion


    #region 2. 构造函数

    /// <summary>
    /// 带参构造函数：提供测试仪的设备名，加载并设置参数，打开串口
    /// </summary>
    /// <param name="deviceName"></param>
    public TesterHiokiBT3562(string deviceName) : base(deviceName)
    {
      OpenTester();   //加载并设置参数，打开网口
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
      set {
        if ((int)value>0 &&(int) value<4)
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
        if ((int)value > 0 && (int)value < 5)
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
    /// 可配置的属性：在一次测试中，读取测试仪示数的次数。默认值为 5
    /// </summary>
    [IniConfig]
    public int 读取次数 { get; set; } = 5;

    /// <summary>
    ///  可配置的属性：在一次测试中，读取测试仪示数的时间间隔。单位 ms，默认值为 50
    /// </summary>
    [IniConfig]
    public int 读取间隔 { get; set; } = 50;

    #endregion


    #region 4. 主要方法：重写 Open / Close：打开、关闭串口

    /// <summary>
    /// 重写父类方法：加载配置，并打开串口
    /// </summary>
    public override void Open()
    {
      //开串口
      if (!IsConnected)
      {
        OpenTester();
      }
    }


    /// <summary>
    /// 重写父类方法：关闭串口
    /// </summary>
    public override void Close()
    {
      //关串口
      if (IsConnected)
      {
        CloseTester();
      }
    }


    // 私有方法：加载并设置参数，打开串口
    private void OpenTester()
    {
      try
      {
        //串口关闭时，才能修改参数
        if (!_serialPort.IsOpen)
        {
          //设置参数
          LoadConfigs(); //读取本地配置，设置参数：串口参数

          _serialPort.Open(); //打开串口 

          //保存参数
          SaveConfigs(); //保存本地配置
        }
      }
      catch (Exception ex)
      {
        ShowException("打开测试仪串口失败！", ex);
      }
    }


    // 私有方法：关闭串口，释放相关资源
    private void CloseTester()
    {
      try
      {
        _serialPort.Close(); //关闭串口
        _serialPort.Dispose(); //释放串口资源
      }
      catch (Exception ex)
      {
        ShowException("关闭测试仪串口失败！", ex);
      }
    }

    #endregion


    #region 4. 主要功能：读取测试数据

    /// <summary>
    /// 主要功能：读取电压内阻测试数据多次，并去掉最大值、最小值，求平均值。
    /// 读取的次数以及间隔，均为可配置的属性
    /// </summary>
    /// <param name="resOCV">输出参数：根据测试仪返回的结果，求出的平均值。如果测试失败，返回 -1.0</param>
    /// <param name="orignalOCVData">输出参数：测试仪返回的原始数据</param>
    /// <returns>测试成功，返回 True；测试失败，返回 False</returns>

    public bool ReadTestResult(out double resOCV, out double resIR, out string orignalOCVData, out string orignalIRData)
    {

      string cmd0 = ":TRIG:SOUR IMM\n"; //立即进行触发
      string cmd1 = ":INIT:CONT ON\n"; //设置为连续测量ON
      string cmd2 = ":FETCH?\n";  //读取测量值

      lock (_lockObj)
      {
        try
        {
          double[] dResultsOCV = new double[读取次数]; //OCV原始数据
          double[] dResultsIR = new double[读取次数]; //IR原始数据

          _serialPort.Write(cmd0); //发送串口命令：设置
          _serialPort.Write(cmd1);

          // 读取5次测试仪数据
          for (int i = 0; i < dResultsOCV.Length; i++)
          {
            _serialPort.Write(cmd2); //发送串口命令：读取测量值
            Thread.Sleep(100); //等待100ms 测试完成
            string strRes = ReadSerialPortResponse(); //读取串口返回值   
            if (!string.IsNullOrEmpty(strRes))
            {
              string[] splits = strRes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); //以逗号分隔字符串，并去除空格
              double.TryParse(splits[0], out dResultsOCV[i]); //解析字符串为 double 类型
              double.TryParse(splits[1], out dResultsIR[i]);
            }
            Thread.Sleep(读取间隔);
          }

          orignalOCVData = string.Join(", ", dResultsOCV); //拼接各组数据为字符串
          orignalIRData = string.Join(", ", dResultsIR);

          Array.Sort(dResultsOCV); //升序排列
          double tempOCV = 0.0;
          for (int i = 1; i < dResultsOCV.Length - 1; i++)
          {
            tempOCV += dResultsOCV[i]; //累加：去除最大和最小
          }
          resOCV = tempOCV / (dResultsOCV.Length - 2); //取平均

          Array.Sort(dResultsIR); //升序排列
          double tempIR = 0.0;
          for (int i = 1; i < dResultsIR.Length - 1; i++)
          {
            tempIR += dResultsIR[i]; //累加：去除最大和最小
          }
          resIR = tempIR / (dResultsIR.Length - 2); //取平均

          return true;
        }
        catch (Exception)
        {
          //LocalSave.WriteErrorLog("测试仪 [" + DeviceName + "] 读取测试数据失败！\r\n异常信息:\r\n" + ex.ToString());
        }

        resOCV = -1.0;
        resIR = -1.0;
        orignalOCVData = "";
        orignalIRData = "";
        return false;
      }
    }

    #endregion


    #region 6. 私有方法：内部调用

    // 私有方法：读串口返回数据
    private string ReadSerialPortResponse()
    {
      string res = null;
      try
      {
        res = _serialPort.ReadTo("\r"); //读取串口返回值，到 \r 停止
      }
      catch (Exception ex)
      {
        //LocalSave.WriteErrorLog("测试仪 [" + DeviceName + "] 读取串口返回数据失败！\r\n异常信息:\r\n" + ex.ToString());
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

  } //class
}// namespace
