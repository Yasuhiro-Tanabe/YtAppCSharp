using MemorieDeFleurs.Databese.SQLite;

using System;
using System.Collections.Generic;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// 日別在庫アクション検証
    /// </summary>
    public class ActionDateStockActionValidator : List<ExpectedStockAction>
    {
        /// <summary>
        /// この検証器の呼び出し元
        /// </summary>
        private LotStockActionValidator Parent { get; set; }

        /// <summary>
        /// 検証器を作成する
        /// </summary>
        /// <param name="p">呼び出し元の検証器</param>
        public ActionDateStockActionValidator(LotStockActionValidator p)
        {
            Parent = p;
        }

        /// <summary>
        /// 入荷予定の期待値を登録する
        /// </summary>
        /// <param name="arrived">入荷本数</param>
        /// <returns>自分自身</returns>
        public ActionDateStockActionValidator Arrived(int arrived)
        {
            Add(ExpectedStockAction.CreateArrivedAction(arrived));
            return this;
        }

        /// <summary>
        /// 使用予定の期待値を登録する
        /// </summary>
        /// <param name="used">使用量</param>
        /// <param name="remain">残数</param>
        /// <returns>自分自身</returns>
        public ActionDateStockActionValidator Used(int used, int remain)
        {
            Add(ExpectedStockAction.CreateUsedAction(used, remain));
            return this;
        }

        public ActionDateStockActionValidator OutOfStock(int lacked)
        {
            Add(ExpectedStockAction.CreateOutOfStockAction(lacked));
            return this;
        }

        /// <summary>
        /// 破棄予定の期待値を登録する
        /// </summary>
        /// <param name="discarded">破棄数</param>
        /// <returns>自分自身</returns>
        public ActionDateStockActionValidator Discarded(int discarded)
        {
            Add(ExpectedStockAction.CreateDiscardAction(discarded));
            return this;
        }

        /// <summary>
        /// 呼び出し元のロットの在庫アクション検証器に制御を戻す
        /// </summary>
        /// <returns>ロットの在庫アクション検証器</returns>
        public PartStockActionValidator End()
        {
            return Parent.End();
        }

        public ActionDateStockActionValidator At(DateTime date)
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
            ForEach(a => a.AssertExists(context, actionDate, partsCode, lotNo, arrivedDate));
        }
    }
}
