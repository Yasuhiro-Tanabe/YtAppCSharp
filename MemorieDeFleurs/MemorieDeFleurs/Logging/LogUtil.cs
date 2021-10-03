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

        public static bool IsDebugEnabled { get { return _logger.IsDebugEnabled; } }
        public static bool IsInfoEnabled { get { return _logger.IsInfoEnabled; } }
        public static bool IsWarnEnabled { get { return _logger.IsWarnEnabled; } }
        public static bool IsErrorEnabled { get { return _logger.IsErrorEnabled; } }
        public static bool IsFatalEnabled { get { return _logger.IsFatalEnabled; } }
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
        /// 在庫アクションの変更ログを出力する
        /// </summary>
        /// <param name="oldAction">変更前の在庫アクションオブジェクト</param>
        /// <param name="newQuantity">変更後の数量</param>
        /// <param name="newRemain">変更後の残数</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_InventoryActionQuantityChanged(InventoryAction oldAction, int newQuantity, int newRemain, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var qnew = newQuantity == oldAction.Quantity ? "(same)" : $"-> {newQuantity}";
            var rnew = newRemain == oldAction.Remain ? $"(same)" : $"-> {newRemain}";

            LogUtil.Debug($"{Indent}Update: {oldAction.ToString("h")}=[quantity={oldAction.Quantity} {qnew}, remain={oldAction.Remain} {rnew}]", caller, path, line);
        }

        /// <summary>
        /// 変更処理実施前に在庫アクションの変更ログを出力する
        /// 
        /// oldAction.Quantity が oldAction.Quantity から (oldAction.Quantity + used) に、Remain が oldAction.Remain から (oldAction.Remain － used) に
        /// 変更されたことをログ出力する。
        /// </summary>
        /// <param name="oldAction">変更前の在庫アクションオブジェクト</param>
        /// <param name="used">変更後の数量</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_InventoryActionQuantityChangingTo(InventoryAction oldAction, int used, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            DEBUGLOG_InventoryActionQuantityChanged(oldAction, oldAction.Quantity + used, oldAction.Remain - used, caller, path, line);
        }

        /// <summary>
        /// 変更処理実施後に在庫アクションの変更ログを出力する
        /// 
        /// newAction.Quantity が (newAction.Quantity － used) から newAction.Quantity に、Remain が (newAction.Remain ＋ used) から newAction.Remain に
        /// 変更されたことをログ出力する。
        /// </summary>
        /// <param name="newAction">変更後の在庫アクションオブジェクト</param>
        /// <param name="used">変更した数量：変更前の数量は内部で計算する</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        public static void DEBUGLOG_InventoryActionQuantityChanged(InventoryAction newAction, int used, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var oldQuantity = newAction.Quantity - used;
            var oldRemain = newAction.Remain + used;

            var txtQuantity = oldQuantity == newAction.Quantity ? "(same)" : $"-> {newAction.Quantity}";
            var txtRemain = oldRemain == newAction.Remain ? "(same)" : $"-> {newAction.Remain}";

            LogUtil.Debug($"{Indent}Update: {newAction.ToString("h")}=[quantity={oldQuantity} {txtQuantity}, remain={oldRemain} {txtRemain}", caller, path, line);
        }

        /// <summary>
        /// 生成/登録された在庫アクションをデバッグログ出力する
        /// </summary>
        /// <param name="action">出力対象在庫アクション</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_InventoryActionCreated(InventoryAction action, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.Debug($"{Indent}Created: {action.ToString("L")}", caller, path, line);
        }

        /// <summary>
        /// 基準日にこのロットから要求数量をすべて引当できるかどうかの判定ログを出力する
        /// </summary>
        /// <param name="action">比較対象アクション</param>
        /// <param name="quantity">要求数量</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_ComparationOfInventoryRemainAndQuantity(InventoryAction action, int quantity, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var operatorString = action.Remain >= quantity ? ">=" : "<";

            LogUtil.Debug($"{Indent}Compare: {action.ToString("h")}.Remain({action.Remain}) {operatorString} {quantity}", caller, path, line);

        }

        /// <summary>
        /// 基準日に前日残から当日要求されていた数量をすべて引当できるかどうかの判定ログを出力する
        /// </summary>
        /// <param name="action">比較対象アクション</param>
        /// <param name="remain">前日残</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_ComparisonOfInventoryQuantityAndPreviousRemain(InventoryAction action, int remain, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var operatorString = action.Quantity <= remain ? "<=" : ">";

            LogUtil.Debug($"{Indent}Compare: {action.ToString("h")}.Quantity({action.Quantity}) {operatorString} {remain}", caller, path, line);
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
