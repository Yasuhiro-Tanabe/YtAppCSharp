using MemorieDeFleurs.Models;

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
        public LotStockActionValidator Parent { get; private set; }

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
        /// ロットの在庫アクション検証器を生成し制御を移す
        /// </summary>
        /// <param name="arrivedDate">入荷予定日</param>
        /// <param name="lotNo">ロット番号</param>
        /// <returns>ロットの在庫アクション検証器</returns>
        public LotStockActionValidator Lot(DateTime arrivedDate, int lotNo)
        {
            return Parent.Lot(arrivedDate, lotNo);
        }

        /// <summary>
        /// ロットの在庫アクション検証器を生成し制御を移す
        /// </summary>
        /// <param name="arrivalDate">入荷予定日</param>
        /// <param name="findLotNumber">入荷予定日からロット番号を特定するためのメソッドまたはデレゲート</param>
        /// <returns>ロットの在庫アクション検証器</returns>
        public LotStockActionValidator Lot(DateTime arrivedDate, Func<DateTime, int> findLotNumber)
        {
            return Parent.Lot(arrivedDate, findLotNumber);
        }

        /// <summary>
        /// 日別在庫アクション検証器を生成し制御を移す
        /// </summary>
        /// <param name="actionDate">基準日</param>
        /// <returns>日別在庫アクション検証器</returns>
        public ActionDateStockActionValidator At(DateTime actionDate)
        {
            return Parent.At(actionDate);
        }

        /// <summary>
        /// データベース上の在庫アクションのうちこの検証器に登録されている各基準日の在庫アクションが、
        /// 期待値通りに登録されているかどうかを検証する
        /// </summary>
        /// <param name="context">検証対象データベース</param>
        public void AssertAll(MemorieDeFleursDbContext context)
        {
            Parent.AssertAll(context);
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
