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
            _logger.DebugFormat("{0} ({1},{2}:{3})", msg, caller, Path.GetFileName(path), line);
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

            LogUtil.Debug($"{Indent}[BEGIN] {caller}{argument}{message}", caller, path, line);
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
            LogUtil.Debug($"{Indent}[END] {caller}{argument}{message}", caller, path, line);
        }

        /// <summary>
        /// 在庫アクションの変更ログを出力する
        /// </summary>
        /// <param name="oldAction">変更前の在庫アクションオブジェクト</param>
        /// <param name="newQuantity">変更後の数量</param>
        /// <param name="newRemain">変更後の残数</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_StockActionQuantityChanged(StockAction oldAction, int newQuantity, int newRemain, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var qnew = newQuantity == oldAction.Quantity ? "(same)" : $"-> {newQuantity}";
            var rnew = newRemain == oldAction.Remain ? $"(same)" : $"-> {newRemain}";

            LogUtil.Debug($"{Indent}Update: {oldAction.ToString("h")}=[quantity={oldAction.Quantity}{qnew}, remain={oldAction.Remain}{rnew}]", caller, path, line);
        }
        #endregion
    }
}
