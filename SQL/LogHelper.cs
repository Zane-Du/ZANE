//using log4net;
//using log4net.Config;

//namespace RMR.Utils
//{
//    public enum LogEnumLevel
//    {
//        Debug = 0,
//        Info,
//        Warn,
//        Error,
//        Fatal
//    }

//    public static class LogHelper
//    {
//        private static readonly ILog log = LogManager.GetLogger(typeof(LogHelper));

//        public static void InitLogHelper()
//        {
//            // Specify the full or relative path to your custom log4net configuration file
//            string configFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}log4net\\log4net.config";   // e.g., "C:\\path\\to\\log4net.config"

//            // Configure log4net using the specified config file
//            if (File.Exists(configFilePath))
//            {
//                XmlConfigurator.Configure(new FileInfo(configFilePath));
//            }
//            else
//            {
//                Console.WriteLine($"Log4net configuration file not found: {configFilePath}");
//            }
//        }

//        public static void LogMsg(this string message, LogEnumLevel logEnumLevel)
//        {

//            switch (logEnumLevel)
//            {
//                case LogEnumLevel.Debug:
//                    log.Debug(message);
//                    break;
//                case LogEnumLevel.Info:
//                    log.Info(message);
//                    break;
//                case LogEnumLevel.Warn:
//                    log.Warn(message);
//                    break;
//                case LogEnumLevel.Error:
//                    log.Error(message);
//                    break;
//                case LogEnumLevel.Fatal:
//                    log.Fatal(message);
//                    break;

//            }
//        }
//    }
//}
