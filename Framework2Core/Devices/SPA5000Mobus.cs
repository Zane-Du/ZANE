using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.ModBus;
using System.IO.Ports;
using System.Threading;


namespace Framework2Core
{
    /// <summary>
    /// NVT等离子清洗机
    /// </summary>
    public class SPA5000Mobus : AbstractDevice
    {

        #region 2.构造函数
        public SPA5000Mobus(string deviceName) : base(deviceName)
        {
            OpenPlasmaCleaner();
        }
        #endregion



        #region 1. 字段、普通属性

        // 字段：串口，用于通讯。默认参数为 19200、8、1、N；读取超时 500ms
        private SerialPort _serialPort = new SerialPort()
        {
            PortName = "COM4",
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

        private static ushort[] g_McRctable_16 = {
        0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
        0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
        0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
        0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
        0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
        0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
        0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
        0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
        0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
        0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
        0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
        0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
        0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
        0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
        0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
        0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
        0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
        0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
        0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
        0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
        0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
        0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
        0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
        0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
        0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
        0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
        0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
        0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
        0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
        0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
        0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
        0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040}; //Modbus协议指令

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
        private BaudrateOptions _baudrate = new BaudrateOptions() { Text = "38400" };


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
            set
            {
                if ((int)value > 0)
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
                if ((int)value > 0)
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


        #endregion


        #region 4. 主要功能：重写 Open / Close：打开、关闭串口

        /// <summary>
        /// 主要功能：重写父类方法：加载配置，并打开串口
        /// </summary>
        public override void Open()
        {
            //开串口
            if (!IsConnected)
            {
                OpenPlasmaCleaner();
            }
        }

        public  void Open2()
        {
            //开串口
            if (!IsConnected)
            {
                //try
                //{
                //    //串口关闭时，才能修改参数
                //    if (!_serialPort.IsOpen)
                //    {
                //        //设置参数
                //        //LoadConfigs(); //读取本地配置，设置参数：串口参数

                //        //不用默认打开
                //        _serialPort.Open(); //打开串口 

                //        //连接后，保存参数
                //        //SaveConfigs(); //保存本地配置
                //    }
                //}
                //catch (Exception ex)
                //{
                //    ShowException("打开等离子清洗机串口失败！", ex);
                    
                //}

              
                    if (!_serialPort.IsOpen)
                    {
                        //设置参数
                        //LoadConfigs(); //读取本地配置，设置参数：串口参数

                        //不用默认打开
                        _serialPort.Open(); //打开串口 

                        //连接后，保存参数
                        //SaveConfigs(); //保存本地配置
                    }
               
            }
        }


        /// <summary>
        /// 主要功能：重写父类方法：关闭串口
        /// </summary>
        public override void Close()
        {
            //关串口
            if (IsConnected)
            {
                ClosePlasmaCleaner();
            }
        }


        // 私有方法：加载并设置参数，打开串口
        private void OpenPlasmaCleaner()
        {
            try
            {
                //串口关闭时，才能修改参数
                if (!_serialPort.IsOpen)
                {
                    //设置参数
                    LoadConfigs(); //读取本地配置，设置参数：串口参数

                    //不用默认打开
                    //_serialPort.Open(); //打开串口 

                    //连接后，保存参数
                    SaveConfigs(); //保存本地配置
                }
            }
            catch (Exception ex)
            {
                ShowException("打开等离子清洗机串口失败！", ex);
            }
        }


        // 私有方法：关闭串口，释放相关资源
        private void ClosePlasmaCleaner()
        {
            try
            {
                _serialPort.Close(); //关闭串口
                _serialPort.Dispose(); //释放串口资源
            }
            catch (Exception ex)
            {
                ShowException("关闭等离子清洗机串口失败！", ex);
            }
        }

        #endregion

        #region 6.私有方法：生成控制指令，读取串口返回数据

        //public string GetPower()
        //{
        //    lock (_lockObj)
        //    {
        //        try
        //        {
        //            //_serialPort.Write(bArr, 0, bArr.Length);  //指令 //发送串口命令
        //            //Thread.Sleep(200);
        //            //string strRes = ReadSerialPortResponse(); //读取串口返回值

        //            return strRes;
        //        }
        //        catch (Exception ex)
        //        {
        //            LocalLogSave.WriteErrorLog("光源控制器 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
        //        }
        //    } 
        //    return null;

        //}
        public static string uiuiuiui = "";

        // 私有方法：根据亮度数组生成控制指令，并发送
        public List<string> SendCommand(string 从机码, string 功能码,string 起始地址,string 读取数量,out string strSendCmd) 
        {
            strSendCmd = "";
            //如果光源控制器已连接
            if (IsConnected)
            {

                // 第1步：生成指令
                //Byte[] cmdByte = new Byte[] {(Byte) Convert.ToInt32(从机码), 16),(Byte)Convert.ToInt32(功能码), 16),(Byte)Convert.ToInt32(起始地址), 16),(Byte)Convert.ToInt32(读取数量), 16),}
                string cmd =从机码+" "+功能码+" "+起始地址+" "+读取数量;
                string[] cmdArray = cmd.Split(new char[] { ' ' });
                //转16进制
                Byte[] cmdByte = new Byte[cmdArray.Length];
                for (int i = 0; i < cmdArray.Length; i++)
                {
                    cmdByte[i] = Convert.ToByte(cmdArray[i], 16);
                }

                //cmd = cmd.Replace(" ", "");
                //byte[] byteCmd = System.Text.Encoding.Default.GetBytes(cmd);
                ushort nCRC = CRC_GetModbus16(cmdByte, cmdByte.Length);
                
                byte[] byteSendCmd = new byte[cmdByte.Length+2];
                Array.Copy(cmdByte, 0, byteSendCmd, 0, cmdByte.Length);
                byteSendCmd[cmdByte.Length] = (byte)(nCRC % 0x100);
                byteSendCmd[cmdByte.Length+1] = (byte)(nCRC / 0x100);

                for (int i = 0; i < byteSendCmd.Length; i++)
                {
                    //strSendCmd += Convert.ToString(byteSendCmd[i], 16);
                    strSendCmd += String.Format("{0:X2}",/*(int) */byteSendCmd[i]);
                }
                
                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(byteSendCmd,0,byteSendCmd.Length); //发送串口命令
                        //_serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        Thread.Sleep(50);
                        string saa = "";
                        List<string> strRes = ReadSerialPortResponse(out saa); //读取串口返回值
                        return strRes;
                        //if (!string.IsNullOrEmpty(strRes))
                        //{
                        //    return true; //收到返回值不为空，设置成功
                        //}
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("等离子清洗机 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return null;
        }

        public List<string> SendCommand2(out string strSendCmd)
        {
            strSendCmd = "";
            //如果光源控制器已连接
            if (IsConnected)
            {

               

                //byte[] byteSendCmd = new byte[4];


                byte[] byteSendCmd = new byte[4] { 0x30, 0x30, 0x31, 0x3F };

                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        //_serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        Thread.Sleep(50);
                        string zane = "";
                        List<string> strRes = ReadSerialPortResponse(out zane); //读取串口返回值
                        string aaa = zane;


                        return strRes;
                  
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("等离子清洗机 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return null;
        }

        public List<string> SendCommand3(out string jkjk)
        {
            //如果光源控制器已连接
            if (IsConnected)
            {

                byte[] byteSendCmd = new byte[4] { 0x30, 0x30, 0x31, 0x3F };

                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        //_serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        Thread.Sleep(50);
                        string saa = "";
                        List<string> strRes = ReadSerialPortResponse(out saa); //读取串口返回值
                        jkjk= saa;

                        return strRes;
                        //if (!string.IsNullOrEmpty(strRes))
                        //{
                        //    return true; //收到返回值不为空，设置成功
                        //}
                    }
                    catch (Exception ex)
                    {
                        jkjk = "";
                        LocalLogSave.WriteErrorLog("等离子清洗机 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)
            else
            {
                jkjk = "";
            }

            return null;
        }

        public List<string> SendCommand000()
        {
            //如果光源控制器已连接
            if (IsConnected)
            {

                byte[] byteSendCmd = new byte[4] { 0x30, 0x30, 0x31, 0x54 };
                // 第2步：发送指令
                lock (_lockObj)
                {
                    try
                    {
                        _serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        //_serialPort.Write(byteSendCmd, 0, byteSendCmd.Length); //发送串口命令
                        Thread.Sleep(50);
                        string saa = "";
                        List<string> strRes = ReadSerialPortResponse(out saa); //读取串口返回值
                        return strRes;
                        //if (!string.IsNullOrEmpty(strRes))
                        //{
                        //    return true; //收到返回值不为空，设置成功
                        //}
                    }
                    catch (Exception ex)
                    {
                        LocalLogSave.WriteErrorLog("等离子清洗机 [" + DeviceName + "] 发送串口指令失败！\r\n异常信息:\r\n" + ex.ToString());
                    }
                }// lock
            }// if (IsConnected)

            return null;
        }


        // 私有方法：读串口返回数据。光源返回的数据无换行符，因此读取固定长度 100
        private List<string> ReadSerialPortResponse(out string wtData)
        {
            List<string> res = new List<string>() ;
            try
            {
                byte[] buffer = new byte[200]; //光源返回的数据无换行符，因此改为读取固定长度 100
                int length = _serialPort.Read(buffer, 0, 200); //返回有效长度
                for (int i = 0; i < length; i++)
                {
                    //strSendCmd += Convert.ToString(byteSendCmd[i], 16);
                    res.Add(String.Format("{0:X2}",/*(int) */buffer[i]));
                    //res += String.Format("{0:X2}",/*(int) */buffer[i]);
                }


                string asciiString = ConvertHexListToString(res);
                 wtData = ExtractWTDataBySplit(asciiString);
                Console.WriteLine(wtData);
                uiuiuiui = wtData;

                int aa = 0;

            }
            catch (Exception ex)
            {
                LocalLogSave.WriteErrorLog("光源控制器 [" + DeviceName + "] 读取串口返回数据失败！\r\n异常信息:\r\n" + ex.ToString());
                res = null;
                wtData = null;

            }
            finally
            {
                //即使在 try、catch 中 return，依旧会执行 finally 中的语句
                _serialPort.DiscardInBuffer();  // 读取完毕，清空接收缓存区

            }

            return res;
        }

        //public string ReadMachinePower(string 从机码)
        //{
        //    string strSendCom = "";
        //    //return SendCommand(从机码, "04", "00 00", "00 01",out strSendCom);
        //}

        /// CRC校验
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="nLength"></param>
        /// <returns></returns>
        ///       
        public static ushort CRC_GetModbus16(byte[] pData, int nLength)
        {
            ushort cRc_16 = 0xFFFF;
            byte temp;

            for (int i = 0; i < nLength; ++i)
            {
                temp = (byte)(cRc_16 & 0xFF);
                cRc_16 = (ushort)((cRc_16 >> 8) ^ g_McRctable_16[(temp ^ pData[i]) & 0xFF]);
            }
            return cRc_16;
        }


        static string ConvertHexListToString(List<string> hexList)
        {
            byte[] bytes = new byte[hexList.Count];

            for (int i = 0; i < hexList.Count; i++)
            {
                bytes[i] = Convert.ToByte(hexList[i], 16);
            }

            return Encoding.ASCII.GetString(bytes);
        }

        static string ExtractWTDataBySplit(string input)
        {
            // 按行分割
            string[] lines = input.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.StartsWith("WT:"))
                {
                    // 返回 WT: 后面的内容，并去除首尾空格
                    return line.Substring(3).Trim();
                }
            }

            return null;
        }
        #endregion


    }
}
