using log4net;
using log4net.Config;

using MemorieDeFleurs.Models.Entities;

using System.Diagnostics;
using System.IO;
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
        public static void Debug(string msg, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            if(_logger.IsDebugEnabled)
            {
                _logger.Debug($"{msg} ({caller},{Path.GetFileName(path)}:{line}");
            }
        }
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

        #region デバッグ用の拡張ログ出力
        [Conditional("DEBUG")]
        public static void DEBUGLOG_BeginMethod(string args = "", string msg = "", [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var b = new StringBuilder().Append($"{Indent}[BEGIN] ").Append(caller)
                .Append(string.IsNullOrWhiteSpace(args) ? "()" : $"( {args} )");

            if (!string.IsNullOrWhiteSpace(msg))
            {
                b.Append(' ').Append(msg);
            }

            LogUtil.Debug(b.ToString(), caller, path, line);
            Indent++;
        }

        [Conditional("DEBUG")]
        public static void DEBUGLOG_EndMethod(string args = "", string msg = "", [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            Indent--;

            var b = new StringBuilder().Append($"{Indent}[END] ").Append(caller)
                .Append(string.IsNullOrWhiteSpace(args) ? "()" : $"( {args} )");

            if (!string.IsNullOrWhiteSpace(msg))
            {
                b.Append(' ').Append(msg);
            }

            LogUtil.Debug(b.ToString(), caller, path, line);
        }

        [Conditional("DEBUG")]
        public static void DEBUGLOG_StockActionQuantityChanged(StockAction oldAction, int newQuantity, int newRemain, [CallerMemberName] string calledFrom = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var builder = new StringBuilder()
                .Append(LogUtil.Indent).Append(oldAction.PartsCode).Append('.').Append(oldAction.Action)
                .Append("[lot=").Append(oldAction.StockLotNo)
                .AppendFormat(", day={0:yyyyMMdd}", oldAction.ActionDate);

            if (newQuantity == oldAction.Quantity)
            {
                builder.AppendFormat(", quantity={0} (same)", oldAction.Quantity);
            }
            else
            {
                builder.AppendFormat(", quantity={0}->{1}", oldAction.Quantity, newQuantity);
            }

            if (newRemain == oldAction.Remain)
            {
                builder.AppendFormat(", remain={0} (same)", oldAction.Remain);
            }
            else
            {
                builder.AppendFormat(", remain={0}->{1}", oldAction.Remain, newRemain);
            }

            builder.Append(" ]");

            LogUtil.Debug(builder.ToString(), calledFrom, path, line);
        }
        #endregion
    }
}
