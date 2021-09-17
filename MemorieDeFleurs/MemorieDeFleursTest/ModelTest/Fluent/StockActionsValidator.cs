using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        private IDictionary<StockActionType, int> ExpectedStockActionCount { get; } = new Dictionary<StockActionType, int>();

        private SqliteConnection CurrentConnection { get; set; }

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
        /// 特定の在庫アクションタイプが期待個数データベースに登録されていることを確認する
        /// </summary>
        /// <param name="type">在庫アクションタイプ</param>
        /// <param name="expected">期待値(個数)</param>
        /// <returns>自分自身</returns>
        public StockActionsValidator StockActionCountShallBe(StockActionType type, int expected)
        {
            ExpectedStockActionCount[type] = expected;
            return this;
        }

        /// <summary>
        /// データベース上の在庫アクションが自分自身に登録されているすべての期待値と一致するかどうかを検証する
        /// </summary>
        /// <param name="context">検証対象データベース</param>
        private void AssertAll(MemorieDeFleursDbContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.All(kv => { kv.Value.AssertAll(context, kv.Key); return true; });

            foreach(var expected in ExpectedStockActionCount)
            {
                Assert.AreEqual(expected.Value, context.StockActions.Count(act => act.Action == expected.Key), $"Type: {expected.Key}");

            }
        }

        public StockActionsValidator TargetDBIs(SqliteConnection connection)
        {
            CurrentConnection = connection;
            return this;
        }

        public void AssertAll()
        {
            if(CurrentConnection is null)
            {
                throw new InvalidOperationException("TargetDB is undefined. call TargetDbIs() before calling AssertAll()");
            }

            using (var context = new MemorieDeFleursDbContext(CurrentConnection))
            {
                AssertAll(context);
            }

        }
    }
}
