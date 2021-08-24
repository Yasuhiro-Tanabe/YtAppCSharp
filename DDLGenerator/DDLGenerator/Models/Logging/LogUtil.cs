using log4net;
using log4net.Config;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDLGenerator.Models.Logging
{
    /// <summary>
    /// ログ出力ユーティリティ
    /// </summary>
    public static class LogUtil
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LogUtil));

        static LogUtil()
        {
            XmlConfigurator.Configure();
        }

        /// <summary>
        /// アペンダ
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

        #region Log4Net ロガー用のログ出力メソッド
        /// <summary>
        /// DEBUG レベルのログを出力する
        /// </summary>
        /// <param name="msg">出力するログメッセージ</param>
        public static void Debug(string msg)
        {
            if (_logger.IsDebugEnabled) { _logger.Debug(msg); }
        }

        /// <summary>
        /// INFO レベルのログを出力する
        /// </summary>
        /// <param name="msg">出力するログメッセージ</param>
        public static void Info(string msg)
        {
            if (_logger.IsInfoEnabled) { _logger.Info(msg); }
        }

        /// <summary>
        /// WARNレベルのログを出力する
        /// </summary>
        /// <param name="msg">出力するログメッセージ</param>
        public static void Warn(string msg)
        {
            if (_logger.IsWarnEnabled) { _logger.Warn(msg); }
        }

        /// <summary>
        /// ERROR レベルのログを出力する
        /// </summary>
        /// <param name="msg">出力するログメッセージ</param>
        public static void Error(string msg)
        {
            if (_logger.IsErrorEnabled) { _logger.Error(msg); }
        }

        /// <summary>
        /// FATAL レベルのログを出力する
        /// </summary>
        /// <param name="msg">出力するログメッセージ</param>
        public static void Fatal(string msg)
        {
            if (_logger.IsFatalEnabled) { _logger.Fatal(msg); }
        }

        /// <summary>
        /// DEBUG レベルのログを出力する：<see cref="string.Format(string, object?)"/> 総統の処理を内部で行う。
        /// </summary>
        /// <param name="fmt">ログ出力メッセージのフォーマット</param>
        /// <param name="args">フォーマットの可変部分に埋め込む引数列</param>
        public static void DebugFormat(string fmt, string[] args)
        {
            if (_logger.IsDebugEnabled) { _logger.DebugFormat(fmt, args); }
        }

        /// <summary>
        /// INFO レベルのログを出力する：<see cref="string.Format(string, object?)"/> 総統の処理を内部で行う。
        /// </summary>
        /// <param name="fmt">ログ出力メッセージのフォーマット</param>
        /// <param name="args">フォーマットの可変部分に埋め込む引数列</param>
        public static void InfoFormat(string fmt, string[] args)
        {
            if (_logger.IsInfoEnabled) { _logger.InfoFormat(fmt, args); }
        }

        /// <summary>
        /// WARN レベルのログを出力する：<see cref="string.Format(string, object?)"/> 総統の処理を内部で行う。
        /// </summary>
        /// <param name="fmt">ログ出力メッセージのフォーマット</param>
        /// <param name="args">フォーマットの可変部分に埋め込む引数列</param>
        public static void WarnFormat(string fmt, string[] args)
        {
            if (_logger.IsWarnEnabled) { _logger.WarnFormat(fmt, args); }
        }

        /// <summary>
        /// ERROR レベルのログを出力する：<see cref="string.Format(string, object?)"/> 総統の処理を内部で行う。
        /// </summary>
        /// <param name="fmt">ログ出力メッセージのフォーマット</param>
        /// <param name="args">フォーマットの可変部分に埋め込む引数列</param>
        public static void ErrorFormat(string fmt, string[] args)
        {
            if (_logger.IsErrorEnabled) { _logger.ErrorFormat(fmt, args); }
        }

        /// <summary>
        /// FATAL レベルのログを出力する：<see cref="string.Format(string, object?)"/> 総統の処理を内部で行う。
        /// </summary>
        /// <param name="fmt">ログ出力メッセージのフォーマット</param>
        /// <param name="args">フォーマットの可変部分に埋め込む引数列</param>
        public static void FatalFormat(string fmt, string[] args)
        {
            if (_logger.IsFatalEnabled) { _logger.FatalFormat(fmt, args); }
        }
        #endregion // Log4Net ロガー用のログ出力メソッド
    }
}
