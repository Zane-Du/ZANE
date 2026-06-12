using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;

namespace Framework2Core
{
  /// <summary>
  /// 抽象类：设备抽象类，定义了设备通用的属性和方法，包括设备名、参数配置、打开、关闭、运行、停止等。
  /// 带参实例构造函数需提供设备名。
  /// </summary>
  public abstract class AbstractDevice
  {

    #region 1. 普通属性

    /// <summary>
    /// 抽象类的只读属性：设备名，作为 Ini 配置文件的 section。仅可在构造函数中初始化
    /// </summary>
    public string DeviceName { get; }


    /// <summary>
    /// 抽象类的只读属性：是否连接并打开，可在子类中重写。仅可在构造函数中初始化
    /// </summary>
    public virtual bool IsConnected { get; }


    /// <summary>
    /// 抽象类的只读属性：是否正在运行，可在子类中重写。仅可在构造函数中初始化
    /// </summary>
    public virtual bool IsRunning { get; set; }

    #endregion


    #region 2. 构造函数

    /// <summary>
    /// 实例构造函数：提供设备名，作为 Ini 配置文件的 section
    /// </summary>
    /// <param name="deviceName">设备名</param>
    public AbstractDevice(string deviceName)
    {
      DeviceName = deviceName; // 初始化设备名
    }

    #endregion


    #region 3. 加载、保存配置

    /// <summary>
    /// 抽象类的方法：加载所有配置，可在子类中重写
    /// </summary>
    public virtual void LoadConfigs()
    {
      this.LoadObjConfigsFromIni(DeviceName);
    }

    /// <summary>
    /// 抽象类的方法：保存所有配置，可在子类中重写
    /// </summary>
    public virtual void SaveConfigs()
    {
      this.SaveObjConfigsToIni(DeviceName);
    }

    /// <summary>
    /// 抽象类的方法：打开设置窗口，配置参数，可在子类中重写
    /// </summary>
    public virtual void SetConfigs()
    {
      this.SetObjConfigs(DeviceName);
    }

    #endregion


    #region 4.主要功能：打开、关闭、运行、停止设备

    /// <summary>
    /// 抽象类的方法：打开并连接设备，可在子类中重写
    /// </summary>
    public virtual void Open() { }


    /// <summary>
    /// 抽象类的方法：断开连接并关闭设备，可在子类中重写
    /// </summary>
    public virtual void Close() { }


    /// <summary>
    /// 抽象类的方法：设备开始运行，可在子类中重写
    /// </summary>
    public virtual void StartRunning() { }


    /// <summary>
    /// 抽象类的方法：设备停止运行，可在子类中重写
    /// </summary>
    public virtual void StopRunning() { }

    #endregion


    #region 4. 功能：弹窗显示异常

    /// <summary>
    /// 功能：仅子类可调用的方法：开新线程，弹窗显示异常
    /// </summary>
    /// <param name="foreword">异常主题</param>
    /// <param name="ex">被显示的异常</param>
    protected void ShowException(string foreword, Exception ex)
    {
      // 开新线程，避免阻塞当前线程
      Task.Run(() =>
      {
        MessageBox.Show($">> {foreword}\r\n\r\n异常信息：\r\n{ex}", $"{this.GetType().Name} 类的 [{DeviceName}] 异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
      });
    }

    #endregion


    public bool PingAccessable(string hostNameOrAddress,int tmOut=500)
    {
      bool rst = false;
      Computer c = new Computer();
      try
      {

        if (hostNameOrAddress.Length > 13)
        {
          hostNameOrAddress = hostNameOrAddress.Substring(0, hostNameOrAddress.Length - 4);
          //hostNameOrAddress = hostNameOrAddress.Replace(".3.1.1", ".3");                  
          // hostNameOrAddress = hostNameOrAddress.
        }
        rst = c.Network.Ping(hostNameOrAddress, tmOut);
        return rst;
      }
      catch
      {
        return rst;
      }

    }

    #region 设备行为信息事件
    /// <summary>
    /// 用于日志显示记录设备行为信息
    /// 设备对象初始化后对事件绑定
    /// add  202109 by DC.Feng
    /// </summary>
  
    public event EventHandler<NotifyEventArgs> Notify;
    public void OnNotify(string msg,MessageType msType)
    {
      if (string.IsNullOrEmpty(msg)) return;

      EventHandler<NotifyEventArgs> tmp = Notify;
      if (tmp != null) tmp(this, new NotifyEventArgs(msg, msType));    
    }
    #endregion

  }// class

}// namespace
