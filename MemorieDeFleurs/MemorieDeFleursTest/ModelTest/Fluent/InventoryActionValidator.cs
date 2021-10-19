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
    public class InventoryActionValidator
    {
        private IDictionary<string, PartsInventoryActionValidator> PartsValidators { get; } = new SortedDictionary<string, PartsInventoryActionValidator>();

        private InventoryActionValidator() { }

        /// <summary>
        /// 検証クラスのインスタンスを生成する
        /// </summary>
        /// <returns>このクラスのオブジェクト</returns>
        public static InventoryActionValidator NewInstance()
        {
            return new InventoryActionValidator();
        }

        private string CurrentPartsCode { get; set; } = null;

        private PartsInventoryActionValidator CurrentChild { get; set; } = null;

        private IDictionary<InventoryActionType, int> ExpectedInventoryActionCount { get; } = new Dictionary<InventoryActionType, int>();

        private SqliteConnection CurrentConnection { get; set; }

        /// <summary>
        /// 単品在庫アクション検証器を生成する：生成するだけで制御はこの検証器のまま。
        /// </summary>
        /// <param name="part">単品</param>
        /// <returns>単品在庫アクション検証器</returns>
        public InventoryActionValidator BouquetPartIs(BouquetPart part)
        {
            return BouquetPartIs(part.Code);
        }

        public InventoryActionValidator BouquetPartIs(string partsCode)
        {
            PartsInventoryActionValidator validator;
            if (!PartsValidators.TryGetValue(partsCode, out validator))
            {
                validator = new PartsInventoryActionValidator(this);
                PartsValidators.Add(partsCode, validator);
            }
            CurrentChild = validator;

            return this;
        }

        /// <summary>
        /// 単品在庫アクション登録開始マーク：
        /// 
        /// 単品在庫アクション検証器に制御を移す
        /// </summary>
        /// <returns>単品在庫アクション検証器</returns>
        public PartsInventoryActionValidator BEGIN
        {
            get
            {
                if (null == CurrentChild)
                {
                    throw new InvalidOperationException($"Call {nameof(BouquetPartIs)}() before calling {nameof(BEGIN)}().");
                }
                return CurrentChild;
            }
        }

        /// <summary>
        /// 特定の在庫アクションタイプが期待個数データベースに登録されていることを確認する
        /// </summary>
        /// <param name="type">在庫アクションタイプ</param>
        /// <param name="expected">期待値(個数)</param>
        /// <returns>自分自身</returns>
        public InventoryActionValidator InventoryActionCountShallBe(InventoryActionType type, int expected)
        {
            ExpectedInventoryActionCount[type] = expected;
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

            PartsValidators.All(kv => { kv.Value.AssertAll(context, kv.Key); return true; });

            foreach(var expected in ExpectedInventoryActionCount)
            {
                Assert.AreEqual(expected.Value, context.InventoryActions.Count(act => act.Action == expected.Key), $"Type: {expected.Key}");

            }
        }

        public InventoryActionValidator TargetDBIs(SqliteConnection connection)
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
