
using MemorieDeFleurs.Models.Entities;

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MemorieDeFleurs.Logging

{
    /// <summary>
    /// 在庫アクションの操作ログ
    /// </summary>
    public static class InventoryActionLogger
    {
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
        [Conditional("DEBUG")]
        public static void DEBUGLOG_InventoryActionChanged(InventoryAction newAction, int used, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var oldQuantity = newAction.Quantity - used;
            var oldRemain = newAction.Remain + used;

            var txtQuantity = oldQuantity == newAction.Quantity ? "(same)" : $"-> {newAction.Quantity}";
            var txtRemain = oldRemain == newAction.Remain ? "(same)" : $"-> {newAction.Remain}";

            LogUtil.Debug($"Update: {newAction.ToString("h")}=[quantity={oldQuantity} {txtQuantity}, remain={oldRemain} {txtRemain}", caller, path, line);
        }

        /// <summary>
        /// 在庫アクションの変更ログを出力する
        /// </summary>
        /// <param name="newAction">変更後の在庫アクション</param>
        /// <param name="oldAction">変更前の数量と残数を参照するための在庫アクション</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_InventoryActionChanged(InventoryAction newAction, InventoryAction oldAction, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var txtQuantity = newAction.Quantity == oldAction.Quantity ? $"{newAction.Quantity} (same)" : $"{oldAction.Quantity} -> {newAction.Quantity}";
            var txtRemain = newAction.Remain == oldAction.Remain ? $"{newAction.Remain} (same)" : $"{oldAction.Remain} -> {newAction.Remain}";

            LogUtil.Debug($"Update; {newAction.ToString("h")}=[quantity:={txtQuantity}, remain:={txtRemain}]", caller, path, line);
        }

        /// <summary>
        /// 在庫アクションの削除ログを出力する
        /// </summary>
        /// <param name="action">削除した在庫アクション</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_InventoryActionRemoved(InventoryAction action, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            LogUtil.Debug($"Removed: {action.ToString("L")}", caller, path, line);
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
            LogUtil.Debug($"Created: {action.ToString("L")}", caller, path, line);
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

            LogUtil.Debug($"Compare: {action.ToString("h")}.Remain({action.Remain}) {operatorString} {quantity}", caller, path, line);
        }

        /// <summary>
        /// 基準日にこのロットへ全量戻せるかどうかの判定ログを出力する
        /// </summary>
        /// <param name="action">比較対象アクション</param>
        /// <param name="quantityToReturn">在庫に戻す数量</param>
        /// <param name="caller">【通常は省略】呼び出し元情報メソッド名。呼び出し元がプロパティの setter/getter の時はそのプロパティ名</param>
        /// <param name="path">【通常は省略】呼び出し元ファイルのパス</param>
        /// <param name="line">【通常は省略】このメソッドが呼び出された、path中の行番号</param>
        [Conditional("DEBUG")]
        public static void DEBUGLOG_ComparationOfInventoryUsedAndReturns(InventoryAction action, int quantityToReturn, [CallerMemberName] string caller = "", [CallerFilePath] string path="", [CallerLineNumber] int line = 0)
        {
            var operatorString = action.Quantity >= quantityToReturn ? ">=" : "<";
            LogUtil.Debug($"Comparet: {action.ToString("h")}.Quantity({action.Quantity}) {operatorString} {quantityToReturn}", caller, path, line);
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
        public static void DEBUGLOG_ComparationOfInventoryQuantityAndPreviousRemain(InventoryAction action, int remain, [CallerMemberName] string caller = "", [CallerFilePath] string path = "", [CallerLineNumber] int line = 0)
        {
            var operatorString = action.Quantity <= remain ? "<=" : ">";

            LogUtil.Debug($"Compare: {action.ToString("h")}.Quantity({action.Quantity}) {operatorString} {remain}", caller, path, line);
        }
    }
}
