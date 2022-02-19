using log4net;
using log4net.Config;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace YasT.Framework.Logging
{
    /// <summary>
    /// 汎用ログユーティリティ
    /// 
    /// 初期化や終了のことを考えずに log4net (http://logging.apache.org/log4net/) のログ出力機能を使うためのユーティリティメソッド集
    /// </summary>
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
        public static NotifyAppender? Appender
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
        /// <summary>
        /// DEBUG レベルのログを出力する
        /// </summary>
        /// <param name="msg">ログ出力メッセージ</param>
        /// <param name="caller">(通常は省略)このメソッドの呼び出し元メソッド/プロパティ名</param>
        /// <param name="path">(通常は省略)このメソッドの呼び出し元メソッド/プロパティが記述されているファイル名</param>
        /// <param name="line">(通常は省略)このログメソッドを記述したファイル内の行番号</param>
        public static void Debug(string msg, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) => _logger.DebugFormat("{0}{1} ({2},{3}:{4})", Indent, msg, caller, Path.GetFileName(path), line);

        /// <summary>
        /// DEBUG レベルのを出力する
        /// 
        /// フォーマットの記述方法は <see cref="string.Format(string, object?[])"/> 参照。
        /// </summary>
        /// <param name="fmt">ログ出力フォーマット</param>
        /// <param name="args">フォーマットに埋め込むパラメータ、fmt に記述した可変部分の数に合わせて指定する</param>
        public static void DebugFormat(string fmt, params object[] args) => _logger.DebugFormat(fmt, args);

        /// <summary>
        /// 例外の内容をDEBUG レベルのログとして出力する
        /// </summary>
        /// <param name="ex">出力する例外</param>
        /// <param name="caller">(通常は省略)このメソッドの呼び出し元メソッド/プロパティ名</param>
        /// <param name="path">(通常は省略)このメソッドの呼び出し元メソッド/プロパティが記述されているファイル名</param>
        /// <param name="line">(通常は省略)このログメソッドを記述したファイル内の行番号</param>
        public static void Debug(Exception ex, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0) => Debug(ex.ToString(), caller, path, line);

        /// <summary>
        /// INFO レベルのログを出力する
        /// </summary>
        /// <param name="msg">出力メッセージ</param>
        public static void Info(string msg) => _logger.Info(msg);

        /// <summary>
        /// 例外の内容を INFO レベルのログとして出力する
        /// </summary>
        /// <param name="ex">出力する例外</param>
        public static void Info(Exception ex) => Info(ex.ToString());

        /// <summary>
        /// INFO レベルのログを出力する
        /// 
        /// フォーマットの記述方法は <see cref="string.Format(string, object?[])"/> 参照。
        /// </summary>
        /// <param name="fmt">ログ出力フォーマット</param>
        /// <param name="args">フォーマットに埋め込むパラメータ、fmt に記述した可変部分の数に合わせて指定する</param>
        public static void InfoFormat(string fmt, params object[] args) => _logger.InfoFormat(fmt, args);

        /// <summary>
        /// WARN レベルのログを出力する
        /// </summary>
        /// <param name="msg">出力メッセージ</param>
        public static void Warn(string msg) => _logger.Warn(msg);

        /// <summary>
        /// 例外の内容を WARN レベルのログとして出力する
        /// </summary>
        /// <param name="ex">出力する例外</param>
        public static void Warn(Exception ex) => Warn(ex.ToString());

        /// <summary>
        /// WARN レベルのログを出力する
        /// 
        /// フォーマットの記述方法は <see cref="string.Format(string, object?[])"/> 参照。
        /// </summary>
        /// <param name="fmt">ログ出力フォーマット</param>
        /// <param name="args">フォーマットに埋め込むパラメータ、fmt に記述した可変部分の数に合わせて指定する</param>
        public static void WarnFormat(string fmt, params object[] args) => _logger.WarnFormat(fmt, args);

        /// <summary>
        /// ERROR レベルのログを出力する
        /// </summary>
        /// <param name="msg">出力メッセージ</param>
        public static void Error(string msg) => _logger.Error(msg);

        /// <summary>
        /// 例外の内容を ERROR レベルのログとして出力する
        /// </summary>
        /// <param name="ex">出力する例外</param>
        public static void Error(Exception ex) => Error(ex.ToString());

        /// <summary>
        /// ERROR レベルのログを出力する
        /// 
        /// フォーマットの記述方法は <see cref="string.Format(string, object?[])"/> 参照。
        /// </summary>
        /// <param name="fmt">ログ出力フォーマット</param>
        /// <param name="args">フォーマットに埋め込むパラメータ、fmt に記述した可変部分の数に合わせて指定する</param>
        public static void ErrorFormat(string fmt, params object[] args) => _logger.ErrorFormat(fmt, args);

        /// <summary>
        /// FATAL レベルのログを出力する
        /// </summary>
        /// <param name="msg">出力メッセージ</param>
        public static void Fatal(string msg) => _logger.Fatal(msg);

        /// <summary>
        /// 例外の内容を FATAL レベルのログとして出力する
        /// </summary>
        /// <param name="ex">出力する例外</param>
        public static void Fatal(Exception ex) => Fatal(ex.ToString());

        /// <summary>
        /// FATAL レベルのログを出力する
        /// 
        /// フォーマットの記述方法は <see cref="string.Format(string, object?[])"/> 参照。
        /// </summary>
        /// <param name="fmt">ログ出力フォーマット</param>
        /// <param name="args">フォーマットに埋め込むパラメータ、fmt に記述した可変部分の数に合わせて指定する</param>
        public static void FatalFormat(string fmt, params object[] args) => _logger.FatalFormat(fmt, args);

        /// <summary>
        /// 現在の設定で DEBUG レベルのログが出力されるかどうか
        /// </summary>
        public static bool IsDebugEnabled { get { return _logger.IsDebugEnabled; } }

        /// <summary>
        /// 現在の設定で INFO レベルのログが出力されるかどうか
        /// </summary>
        public static bool IsInfoEnabled { get { return _logger.IsInfoEnabled; } }

        /// <summary>
        /// 現在の設定で WARN レベルのログが出力されるかどうか
        /// </summary>
        public static bool IsWarnEnabled { get { return _logger.IsWarnEnabled; } }

        /// <summary>
        /// 現在の設定で ERROR レベルのログが出力されるかどうか
        /// </summary>
        public static bool IsErrorEnabled { get { return _logger.IsErrorEnabled; } }

        /// <summary>
        /// 現在の設定で FATAL レベルのログが出力されるかどうか
        /// </summary>
        public static bool IsFatalEnabled { get { return _logger.IsFatalEnabled; } }
        #endregion

        #region デバッグ用の拡張ログ出力
        /// <summary>
        /// 呼び出し元情報を含まないデバッグログを出力する。
        /// </summary>
        /// <param name="msg">ログ出力メッセージ</param>
        [Conditional("DEBUGLOG")]
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
        /// 
        /// このメソッドを呼び出すとインデントレベルが一つ下がる(左に移動する)。
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
        /// メソッド呼出ログを実行する：メソッド開始終了ログを両方出力するのは冗長になりすぎる場合の大体メソッド
        /// 
        /// このメソッドを呼び出してもインデントレベルは変動しない。
        /// </summary>
        /// <param name="args"></param>
        /// <param name="msg"></param>
        /// <param name="caller"></param>
        /// <param name="path"></param>
        /// <param name="line"></param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_MethodCalled(string args="", string msg="", [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var argument = string.IsNullOrWhiteSpace(args) ? "()" : $"( {args} )";
            var message = string.IsNullOrWhiteSpace(msg) ? "" : $" {msg}";

            LogUtil.DebugFormat($"{Indent}[CALLED] {caller}{argument}{message} ({Path.GetFileName(path)}:{line})");
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
