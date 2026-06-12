using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework2Core
{
  public enum MessageType
  {
     info = 1,
     error ,
     alert,
     exception,
  }
  public class NotifyEventArgs : EventArgs
  {
    public string Info { get; private set; }

    public NotifyEventArgs(string msg, MessageType msType)
    {
      Info = msg;
    }
  } 

  public interface INotify
  {
    // events
    event EventHandler<NotifyEventArgs> Notify;
  }

}
