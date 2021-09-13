using log4net;
using log4net.Config;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace MemorieDeFleurs.Logging

{
    public static class LogUtil
    {
        private static ILog _logger;

        public static IndentString Indent { get; set; } = new IndentString();

        static LogUtil()
        {
            BasicConfigurator.Configure();
            _logger = LogManager.GetLogger(typeof(LogUtil).Name);
        }

        #region 基本のログ出力メソッド
        public static void Debug(string msg) => _logger.Debug(msg);
        public static void DebugFormat(string fmt, params object[] args) => _logger.DebugFormat(fmt, args);
        public static void Info(string msg) => _logger.Info(msg);
        public static void InfoFormat(string fmt, params object[] args) => _logger.InfoFormat(fmt, args);
        public static void Warn(string msg) => _logger.Warn(msg);
        public static void WarnFormat(string fmt, params object[] args) => _logger.WarnFormat(fmt, args);
        public static void Error(string msg) => _logger.Error(msg);
        public static void ErrorFormat(string fmt, params object[] args) => _logger.ErrorFormat(fmt, args);
        public static void Fatal(string msg) => _logger.Fatal(msg);
        public static void FatalFormat(string fmt, params object[] args) => _logger.FatalFormat(fmt, args);

        #endregion

    }
}
