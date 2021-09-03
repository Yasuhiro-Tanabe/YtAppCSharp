using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// 在庫アクション検証器：在庫アクションの期待値を登録し、期待値通りに登録されているかどうかを検証するクラス
    /// </summary>
    public class StockActionsValidator : Dictionary<string, PartStockActionValidator>
    {
        private StockActionsValidator() { }

        /// <summary>
        /// 検証クラスのインスタンスを生成する
        /// </summary>
        /// <returns>このクラスのオブジェクト</returns>
        public static StockActionsValidator NewInstance()
        {
            return new StockActionsValidator();
        }

        private BouquetPart CurrentPart { get; set; } = null;

        private PartStockActionValidator CurrentChild { get; set; } = null;

        /// <summary>
        /// 単品在庫アクション検証器を生成する：生成するだけで制御はこの検証器のまま。
        /// </summary>
        /// <param name="part">単品</param>
        /// <returns>単品在庫アクション検証器</returns>
        public StockActionsValidator BouquetPart(BouquetPart part)
        {
            PartStockActionValidator validator;
            if (!TryGetValue(part.Code, out validator))
            {
                validator = new PartStockActionValidator(this);
                Add(part.Code, validator);
            }
            CurrentPart = part;
            CurrentChild = validator;
            return this;
        }

        /// <summary>
        /// 単品在庫アクション検証器に制御を移す
        /// </summary>
        /// <returns>単品在庫アクション検証器</returns>
        public PartStockActionValidator Begin()
        {
            if(null == CurrentChild)
            {
                throw new InvalidOperationException($"Call {nameof(BouquetPart)}() before calling {nameof(Begin)}().");
            }
            return CurrentChild;
        }

        /// <summary>
        /// データベース上の在庫アクションが自分自身に登録されているすべての期待値と一致するかどうかを検証する
        /// </summary>
        /// <param name="context">検証対象データベース</param>
        public void AssertAll(MemorieDeFleursDbContext context)
        {
            this.All(kv => { kv.Value.AssertAll(context, kv.Key); return true; });
        }
    }
}
