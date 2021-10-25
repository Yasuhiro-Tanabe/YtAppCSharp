using log4net;
using log4net.Config;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MemorieDeFleurs.Logging

{
    public static class LogUtil
    {
        private static ILog _logger;

        private static IndentString Indent { get; set; } = new IndentString();

        static string ConfigFileName { get; } = "./log4net.config";

        static LogUtil()
        {
            var stream = new FileStream(ConfigFileName, FileMode.Open, FileAccess.Read);
            XmlConfigurator.Configure(stream);
            _logger = LogManager.GetLogger(typeof(LogUtil).Name);
        }

        /// <summary>
        /// WPFコントロールにログ出力を通知するためのアペンダ
        /// </summary>
        public static NotifyAppender Appender
        {
            get
            {
                return LogManager.GetCurrentLoggers()
                    .SelectMany(log => log.Logger.Repository.GetAppenders())
                    .FirstOrDefault(a => a is NotifyAppender)
                    as NotifyAppender;
            }
        }

        #region 基本のログ出力メソッド
        public static void Debug(string msg, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) => _logger.DebugFormat("{0}{1} ({2},{3}:{4})", Indent, msg, caller, Path.GetFileName(path), line);
        public static void DebugFormat(string fmt, params object[] args) => _logger.DebugFormat(fmt, args);
        public static void Info(string msg) => _logger.Info(msg);
        public static void InfoFormat(string fmt, params object[] args) => _logger.InfoFormat(fmt, args);
        public static void Warn(string msg) => _logger.Warn(msg);
        public static void WarnFormat(string fmt, params object[] args) => _logger.WarnFormat(fmt, args);
        public static void Error(string msg) => _logger.Error(msg);
        public static void ErrorFormat(string fmt, params object[] args) => _logger.ErrorFormat(fmt, args);
        public static void Fatal(string msg) => _logger.Fatal(msg);
        public static void FatalFormat(string fmt, params object[] args) => _logger.FatalFormat(fmt, args);

        public static bool IsDebugEnabled { get { return _logger.IsDebugEnabled; } }
        public static bool IsInfoEnabled { get { return _logger.IsInfoEnabled; } }
        public static bool IsWarnEnabled { get { return _logger.IsWarnEnabled; } }
        public static bool IsErrorEnabled { get { return _logger.IsErrorEnabled; } }
        public static bool IsFatalEnabled { get { return _logger.IsFatalEnabled; } }
        #endregion

        #region デバッグ用の拡張ログ出力
        public static void DebugWithoutLineNumber(string msg) => _logger.Debug($"{Indent}{msg}");
        /// <summary>
        /// メソッド開始ログを出力する
        /// </summary>
        /// <param name="args">【省略可】メソッドの引数をログ出力したい場合、その文字列を適宜整形したもの</param>
        /// <param name="msg">【省略可】メソッド引数ではない追加文言をログ出力したい場合、その文字列</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_BeginMethod(string args = "", string msg = "", [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var argument = string.IsNullOrWhiteSpace(args) ? "()" : $"( {args} )";
            var message = string.IsNullOrWhiteSpace(msg) ? "" : $" {msg}";

            LogUtil.DebugFormat($"{Indent}[BEGIN] {caller}{argument}{message} ({Path.GetFileName(path)}:{line})");
            Indent++;
        }

        /// <summary>
        /// メソッド終了ログを出力する
        /// </summary>
        /// <param name="args">【省略可】メソッドの引数をログ出力したい場合、その文字列を適宜整形したもの</param>
        /// <param name="msg">【省略可】メソッド引数ではない追加文言をログ出力したい場合、その文字列</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_EndMethod(string args = "", string msg = "", [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var argument = string.IsNullOrWhiteSpace(args) ? "()" : $"( {args} )";
            var message = string.IsNullOrWhiteSpace(msg) ? string.Empty : $" {msg}";

            Indent--;
            LogUtil.DebugFormat($"{Indent}[END] {caller}{argument}{message} ({Path.GetFileName(path)}:{line})");
        }

        /// <summary>
        /// テスト開始ログを、通常のメソッド開始終了ログとは別フォーマットで出力する
        /// </summary>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_BeginTest([CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.DebugFormat("========== {0} [BEGIN] ========== ({1}:{2})", caller, Path.GetFileName(path), line);
        }

        /// <summary>
        /// テスト終了ログを、通常のメソッド開始終了ログとは別フォーマットで出力する
        /// </summary>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_EndTest([CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.DebugFormat("========== {0} [END] ========== ({1}:{2})", caller, Path.GetFileName(path), line);
        }
        #endregion
    }
}
