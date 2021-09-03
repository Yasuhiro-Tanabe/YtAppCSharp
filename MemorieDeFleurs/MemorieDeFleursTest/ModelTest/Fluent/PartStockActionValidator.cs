using MemorieDeFleurs.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// 単品在庫アクション検証器
    /// </summary>
    public class PartStockActionValidator : Dictionary<Tuple<DateTime, int>, LotStockActionValidator>
    {
        /// <summary>
        /// この検証器の呼び出し元
        /// </summary>
        private StockActionsValidator Parent { get; set; }

        private LotStockActionValidator CurrentChild { get; set; } = null;

        /// <summary>
        /// 検証器を作成する
        /// </summary>
        /// <param name="p">呼び出し元の検証器</param>
        public PartStockActionValidator(StockActionsValidator p)
        {
            Parent = p;
        }

        /// <summary>
        /// ロットの在庫アクション検証器を生成する：生成するだけで制御は移さない。
        /// </summary>
        /// <param name="arrivedDate">入荷予定日</param>
        /// <param name="lotNo">ロット番号</param>
        /// <returns>自分自身</returns>
        public PartStockActionValidator Lot(DateTime arrivedDate, int lotNo)
        {
            LotStockActionValidator validator;
            var key = Tuple.Create(arrivedDate, lotNo);
            if (!TryGetValue(key, out validator))
            {
                validator = new LotStockActionValidator(this);
                Add(key, validator);
            }

            CurrentChild = validator;
            return this;
        }

        /// <summary>
        /// ロットの在庫アクション検証器を生成する：生成するだけで制御は移さない。
        /// </summary>
        /// <param name="arrivalDate">入荷予定日</param>
        /// <param name="findLotNumber">入荷予定日からロット番号を特定するためのメソッドまたはデレゲート</param>
        /// <returns>自分自身</returns>
        public PartStockActionValidator Lot(DateTime arrivalDate, Func<DateTime, int> findLotNumber)
        {
            return Lot(arrivalDate, findLotNumber(arrivalDate));
        }

        /// <summary>
        /// ロットの在庫アクション検証器に制御を移す
        /// </summary>
        /// <returns>ロットの在庫アクション検証器</returns>
        public LotStockActionValidator Begin()
        {
            if (CurrentChild == null)
            {
                throw new InvalidOperationException($"Call {nameof(Lot)}() before calling {nameof(Begin)}().");
            }
            return CurrentChild;
        }

        /// <summary>
        /// 呼び出し元の在庫アクション検証器に制御を戻す
        /// </summary>
        /// <returns>在庫アクション検証器</returns>
        public StockActionsValidator End()
        {
            return Parent;
        }

        /// <summary>
        /// データベース上の在庫アクションのうちこの検証器に登録されている各ロットの在庫アクションが、
        /// 期待値通りに登録されているかどうかを検証する
        /// </summary>
        /// <param name="context">検証対象データベース</param>
        /// <param name="partsCode">検証対象の単品</param>
        public void AssertAll(MemorieDeFleursDbContext context, string partsCode)
        {
            this.All(kv => { kv.Value.AssertAll(context, partsCode, kv.Key.Item1, kv.Key.Item2); return true; });
        }
    }
}
