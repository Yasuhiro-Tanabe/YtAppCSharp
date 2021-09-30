using MemorieDeFleurs.Databese.SQLite;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// 日別在庫アクション検証
    /// </summary>
    public class DateInventoryActionValidator
    {
        private IList<ExpectedInventoryAction> ExpectedActions { get; } = new List<ExpectedInventoryAction>();

        /// <summary>
        /// この検証器の呼び出し元
        /// </summary>
        private LotInventoryActionValidator Parent { get; set; }

        /// <summary>
        /// 検証器を作成する
        /// </summary>
        /// <param name="p">呼び出し元の検証器</param>
        public DateInventoryActionValidator(LotInventoryActionValidator p)
        {
            Parent = p;
        }

        /// <summary>
        /// 入荷予定の期待値を登録する
        /// </summary>
        /// <param name="arrived">入荷本数</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator Arrived(int arrived)
        {
            ExpectedActions.Add(ExpectedInventoryAction.CreateArrivedAction(arrived));
            return this;
        }

        /// <summary>
        /// 使用予定の期待値を登録する
        /// </summary>
        /// <param name="used">使用量</param>
        /// <param name="remain">残数</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator Used(int used, int remain)
        {
            ExpectedActions.Add(ExpectedInventoryAction.CreateUsedAction(used, remain));
            return this;
        }

        /// <summary>
        /// 在庫不足の期待値を登録する
        /// </summary>
        /// <param name="shortage">在庫不足量</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator Shortage(int shortage)
        {
            ExpectedActions.Add(ExpectedInventoryAction.CreateInventoryShortageAction(shortage));
            return this;
        }

        /// <summary>
        /// 破棄予定の期待値を登録する
        /// </summary>
        /// <param name="discarded">破棄数</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator Discarded(int discarded)
        {
            ExpectedActions.Add(ExpectedInventoryAction.CreateDiscardAction(discarded));
            return this;
        }

        /// <summary>
        /// 日別在庫検証項目登録終了マーク：
        /// 
        /// 呼び出し元のロットの在庫アクション検証器に制御を戻す
        /// </summary>
        /// <returns>ロットの在庫アクション検証器</returns>
        public PartsInventoryActionValidator END { get { return Parent.END; } }

        public DateInventoryActionValidator At(DateTime date)
        {
            return Parent.At(date);
        }

        /// <summary>
        /// データベース上の在庫アクションのうちこの検証器に登録されている各基準日の在庫アクションが、
        /// 期待値通りに登録されているかどうかを検証する
        /// </summary>
        /// <param name="context">検証対象データベース</param>
        /// <param name="partsCode">対象単品の花コード</param>
        /// <param name="arrivedDate">対象ロットの入荷予定日</param>
        /// <param name="lotNo">対象ロットのロット番号</param>
        public void AssertAll(MemorieDeFleursDbContext context, string partsCode, DateTime arrivedDate, int lotNo, DateTime actionDate)
        {
            ExpectedActions.All(a => { a.AssertExists(context, actionDate, partsCode, lotNo, arrivedDate); return true; });
        }
    }
}
