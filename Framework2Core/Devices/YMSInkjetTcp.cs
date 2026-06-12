using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework2Core
{
    public class YMSInkjetTcp : AbstractDevice
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
        public static Dictionary<string, YMSInkjetTcp> _dic_Name_YMS = new Dictionary<string, YMSInkjetTcp>();

        /// <summary>
        /// 普通字段：用于将当前日期与1970年1月1日的时间间隔，转换到0~9,A~Z(除字母"I"和字母"O")的34进制
        /// </summary>
        public static List<string> dayCode = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        //普通字段：用于条码校验时计算时间间隔
        public static DateTime dateStart = DateTime.Parse("1970/1/1");

        #endregion

        #region 2. 构造函数

        /// <summary>
        /// 带参实例构造函数：提供机器人的设备名，加载并设置参数，打开网口
        /// </summary>
        /// <param name="deviceName"></param>
        public YMSInkjetTcp(string deviceName) : base(deviceName)
        {
            OpenYMSInkjet();  //加载并设置参数，打开网口
            if (IsConnected)
            {
                _dic_Name_YMS.Add(deviceName, this); //将此机器人注册到静态列表中，后续可直接通过名称访问机器人
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

        [IniConfig]
        public int Pack码最小长度 { get; set; } = 8;

        [IniConfig]
        public bool 屏蔽喷码校验 { get; set; } = false;

        [IniConfig("BarcodeCheckMode")]
        public 校验方式 CellPack校验方式 { get; set; } = 校验方式.转DOM校验;

        [IniConfig]
        public string 固定条码 { get; set; }
        [IniConfig]
        public bool 喷码系统2 { get; set; } = false;

        [IniConfig]
        public bool 喷码系统2调试 { get; set; } = false;

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
                OpenYMSInkjet();
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
                CloseYMSInkjet();
            }
        }


        // 私有方法：加载并设置参数，打开网口
        private void OpenYMSInkjet()
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
                        ShowException("连接喷码机网口失败！", ex);
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
                ShowException("打开喷码机网口失败！", ex);
            }
        }


        // 私有方法：关闭网口，释放相关资源
        private void CloseYMSInkjet()
        {
            try
            {
                _netStream.Close(); //关闭流对象
                _tcpClient.Close(); //关闭连接并释放                
            }
            catch (Exception ex)
            {
                ShowException("关闭喷码机网口失败！", ex);
            }
        }

        #endregion

        #region 4.主要功能：发送和读取喷码机反馈
        public string SendCode(string cmd, out string PackCode)
        {
            PackCode = "";
            cmd += 喷码系统2 ? (喷码系统2调试 ? ";4" : ";1") : "";
            return GetResponseAndParseResult(cmd + "\r\n", out PackCode);
        }
        #endregion

        #region 6.私有方法：发送指令，读取网口返回数据，并解析指令
        private string GetResponseAndParseResult(string cmd, out string PackCode)
        {
            PackCode = "";
            string strRes = SendCommand(cmd); //发送指令，等待响应

            //判断返回字符串是否合法：不为空，并且包含 "OK"
            if (!string.IsNullOrEmpty(strRes))
            {
                if (strRes.Contains("01,"))
                {
                    PackCode = strRes.Replace("01,", "");
                    return "";
                }
                else if (strRes.Contains("02,"))
                {
                    strRes = SendCommand(cmd); //发送指令，等待响应
                    if (string.IsNullOrEmpty(strRes))
                    {
                        return "喷码机 [" + DeviceName + "] 发送指令 [" + cmd + "] 失败！\r\n异常信息:\r\n" + "喷码机返回为空";
                    }
                    if (strRes.Contains("01,"))
                    {
                        PackCode = strRes.Replace("01,", "");
                        return "";
                    }
                    else
                    {
                        return "喷码机返回NG,如有异常请查看喷码机软件";
                    }
                }
            }

            return "喷码机 [" + DeviceName + "] 发送指令 [" + cmd + "] 失败！\r\n异常信息:\r\n" + "喷码机返回为空";
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
                        LocalLogSave.WriteErrorLog("喷码机 [" + DeviceName + "] 发送指令 [" + cmd + "] 失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return ""; //发生错误，返回空字符串
        }


        public string CheckCellPack(string cellCode, string packCode)
        {
            if (CellPack校验方式 == 校验方式.不校验) return "";
            if (packCode.Length < Pack码最小长度)
            {
                return $"Pack码长度校验异常：当前Pack码长度[{packCode.Length}]小于设定的Pack码最小长度";
            }
            try
            {

                //1、校验PACK码的生产日期
                string strPackDate = packCode.Substring(3, 3);
                string dateCode = ParseDaysToDOM(DateTime.Now);
                if (strPackDate != dateCode)
                {
                    //DBAInfoCtrl.AppendInformation("SendCELL Pack日期校验异常 Pack码日期: " + strPackDate + "  当前日期： " + dateCode);
                    return "SendCELL Pack日期校验异常 Pack码日期: " + strPackDate + "  当前日期： " + dateCode;
                }

                //校验Pack码和Cell码的生产日期代码
                string check1 = cellCode.Substring(3, 3);
                string check2 = packCode.Substring(packCode.Length - 3, 3);
                //将cell码上的DOM码转换为具体日期
                DateTime dateDom = ReverseDOMToDays(check1);
                
                string yww = ParseDomToYWW(dateDom);
                if (yww == check2)
                {
                    return "";
                }
                else
                {
                    return "SendCELL Cell码日期校验异常 CELL码日期: " + yww + "  喷码日期： " + check2;
                }
            }
            catch (Exception ex)
            {
                return "SendCell异常: " + ex.ToString();
            }
        }

        //将当前日期与1970年1月1日的间隔天数，转换到0~9，A~Z（去除字母"I"和字母"O"）的34进制字母数字型
        public string ParseDaysToDOM(DateTime dateNow)
        {
            TimeSpan timeSpan = dateNow - dateStart;
            int Days = timeSpan.Days;
            string strResult = "";
            List<int> days = new List<int>();
            int 除数 = Days;
            int 商 = 0;
            int 余数 = 0;
            do
            {
                商 = 除数 / 34;
                余数 = 除数 % 34;
                days.Add(余数);
                除数 = 商;
            } while (商 != 0);
            days.Reverse();
            foreach (int item in days)
            {
                strResult = strResult + dayCode[item].ToString();
            }
            return strResult;
        }
        /// <summary>
        /// 方法：将Cell码的DOM数据转换为具体日期
        /// </summary>
        /// <param name="strDay"></param>
        /// <returns></returns>
        public DateTime ReverseDOMToDays(string strDay)
        {
            strDay = strDay.ToUpper();
            double dResult = 0;
            List<int> numList = new List<int>();
            for (int i = 0; i < strDay.Length; i++)
            {
                numList.Add(dayCode.IndexOf(strDay[i].ToString()));
            }
            numList.Reverse();
            for (int i = 0; i < numList.Count; i++)
            {
                dResult += (double)numList[i] * Math.Pow(34, i);
            }
            return dateStart.AddDays(dResult);

        }

        public string ParseDomToYWW(DateTime dateDom)
        {
            string strDateYear = dateDom.Year.ToString();      //获取年份
            DateTime dateOfFiretDay = Convert.ToDateTime((strDateYear + "-" + "1" + "-" + "1"));     //获取当年的第一天
            int dateTimeOfFirstDay = Convert.ToInt32(dateOfFiretDay.DayOfWeek);      //获取第一天是星期几，星期天为0，星期一是1
            double d = (dateDom.DayOfYear + dateTimeOfFirstDay) / 7.0;
            int weekOfYear = Convert.ToInt32(Math.Ceiling(d));      //转换成日期是今年第几周
                                                                    //满52进1年，没有53周
            if (weekOfYear > 52)
            {
                dateDom = dateDom.AddYears(1);
                strDateYear = dateDom.Year.ToString();      //获取年份
                weekOfYear -= 52;
            }
            string yww = strDateYear.Substring(strDateYear.Length - 1, 1) + weekOfYear.ToString("00");
            return yww;
        }
        #endregion
    }

    #region 辅助类
    public enum 校验方式
    {
        [IniConfig("ParseToDom")]
        转DOM校验=0,
        [IniConfig("PlainCode")]
        明码校验 = 1,
        [IniConfig("UnCheckCode")]
        不校验 = 2,
    }
    #endregion
}
