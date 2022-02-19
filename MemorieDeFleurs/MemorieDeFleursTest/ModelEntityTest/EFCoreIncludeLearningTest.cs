using MemorieDeFleurs.Databese.SQLite;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Linq;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class EFCoreIncludeLearningTest : MemorieDeFleursTestBase
    {
        public EFCoreIncludeLearningTest()
        {
            AfterTestBaseInitializing += PrepareDatabase;
            BeforeTestBaseCleaningUp += CleanupDatabase;
        }

        #region TestInitialize
        private void PrepareDatabase(object sender, EventArgs unused)
        {
            using (var reader = new StreamReader("./testdata/TestData.sql"))
            {
                var sqls = reader.ReadToEnd().Split("\n").Where(s => s.StartsWith("insert"));

                using (var transaction = TestDB.BeginTransaction())
                using (var command = TestDB.CreateCommand())
                {
                    try
                    {
                        foreach (var sql in sqls)
                        {
                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion

        #region TestTerminate
        private void CleanupDatabase(object sender, EventArgs unused)
        {
            ClearAll();
        }
        #endregion

        /// <summary>
        /// Include() を使わないと、取得したエンティティが ForeginKey 属性付きで参照している別エンティティへの参照が取れない
        /// </summary>
        [TestMethod]
        public void BouquetParts_WithoutInclude()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var parts = context.BouquetParts
                    .Single(p => p.Code == "GP001");
                Assert.AreEqual(0, parts.Suppliers.Count);
            }
        }

        /// <summary>
        /// Include() を使うと、取得したエンティティが ForeginKey 属性付きで参照している別エンティティへの参照を取得できる
        /// </summary>
        [TestMethod]
        public void BouquetParts_WithInclude()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var parts = context.BouquetParts
                    .Include(p => p.Suppliers)
                    .Single(p => p.Code == "GP001");
                Assert.AreEqual(2, parts.Suppliers.Count);
            }
        }

        [TestMethod]
        public void PartsSuppliers_WithoutInclude()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var item = context.PartsSuppliers
                    .Single(i => i.PartCode == "BA001" && i.SupplierCode == 1);

                Assert.IsNull(item.Part);
                Assert.IsNull(item.Supplier);
            }
        }

        /// <summary>
        /// ForeginKey 属性が複数ついている場合、片方だけInclude()してももう一方は該当するエンティティへの参照が取れない
        /// </summary>
        [TestMethod]
        public void PartsSuppliers_WithIncludePartWithoutIncludeSupplier()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var item = context.PartsSuppliers
                    .Include(i => i.Part)
                    .Single(i => i.PartCode == "BA001" && i.SupplierCode == 1);

                Assert.IsNotNull(item.Part);
                Assert.AreEqual("BA001", item.Part.Code);
                Assert.IsNull(item.Supplier);
            }
        }

        /// <summary>
        /// 取得するオブジェクトを先に絞り込むと、Include() したエンティティには絞り込んだエンティティのみ登録される
        /// </summary>
        [TestMethod]
        public void PartsSuppliers_WithIncludeBothPartsAndSupplier()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var item = context.PartsSuppliers
                    .Include(i => i.Part)
                    .Include(i => i.Supplier)
                    .Single(i => i.PartCode == "GP001" && i.SupplierCode == 1); // SQL発行→DBから1件だけ検出 (Linq to SQL)

                Assert.IsNotNull(item.Part);
                Assert.AreEqual("GP001", item.Part.Code);
                Assert.AreEqual(1, item.Part.Suppliers.Count());
                Assert.AreEqual(1, item.Part.Suppliers[0].SupplierCode);
                Assert.IsNotNull(item.Supplier);
                Assert.AreEqual(1, item.Supplier.Code);
                Assert.AreEqual(1, item.Supplier.SupplyParts.Count()); // 検出したものだけ？
                Assert.AreEqual("GP001", item.Supplier.SupplyParts[0].PartCode);
            }
        }

        /// <summary>
        /// 全件取得してから絞り込むと、Include() したエンティティには絞り込んだエンティティ以外も登録される
        /// </summary>
        [TestMethod]
        public void PartsSuppliers_GetAllItemsBeforeCallSingle()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var item = context.PartsSuppliers
                    .Include(i => i.Part)
                    .Include(i => i.Supplier)
                    .ToList() // SQL発行→テーブルに登録さえれている全レコードを取り込む
                    .Single(i => i.PartCode == "GP001" && i.SupplierCode == 1); // 全件取り込んだ中から絞り込み (Linq to Object)

                Assert.IsNotNull(item.Part);
                Assert.AreEqual("GP001", item.Part.Code);
                Assert.AreEqual(2, item.Part.Suppliers.Count());
                foreach(var id in new[] { 1, 2 })
                {
                    Assert.AreEqual(1, item.Part.Suppliers.Count(s => s.SupplierCode == id), $"仕入可能な仕入先{id} の登録件数がおかしい");
                }
                Assert.IsNotNull(item.Supplier);
                Assert.AreEqual(1, item.Supplier.Code);
                Assert.AreEqual(4, item.Supplier.SupplyParts.Count());
                foreach(var code in new [] { "BA001", "BA002", "BA003", "GP001" })
                {
                    Assert.AreEqual(1, item.Supplier.SupplyParts.Count(p => p.PartCode == code), $"仕入可能な単品 {code} の登録件数がおかしい");
                }
            }
        }

        [TestMethod]
        public void Bouquet_WithoutIncludePartsList()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var bouquet = context.Bouquets
                    .Single(b => b.Code == "HT002");
                Assert.AreEqual(0, bouquet.PartsList.Count);
            }
        }

        [TestMethod]
        public void Bouquet_WithIncludePartsList()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var bouquet = context.Bouquets
                    .Include(b => b.PartsList)
                    .Single(b => b.Code == "HT002");
                Assert.AreEqual(3, bouquet.PartsList.Count);
                foreach(var code in new[] { "BA001", "BA003", "GP001" })
                {
                    Assert.AreEqual(1, bouquet.PartsList.Count(p => p.PartsCode == code));
                }
            }
        }

        /// <summary>
        /// Include().ThenInclude() で Bouquet.PartsList の各要素について対向の単品情報を取得することができる
        /// </summary>
        [TestMethod]
        public void Bouquet_WithIncludeThenInclude()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var bouquet = context.Bouquets
                    .Include(b => b.PartsList)
                    .ThenInclude(p => p.Part)
                    .Single(b => b.Code == "HT002");
                Assert.AreEqual(3, bouquet.PartsList.Count);
                foreach (var code in new[] { "BA001", "BA003", "GP001" })
                {
                    Assert.AreEqual(1, bouquet.PartsList.Count(p => p.PartsCode == code));
                }
                foreach(var item in bouquet.PartsList)
                {
                    Assert.IsNotNull(item.Bouquet);
                    Assert.IsNotNull(item.Part);
                    Assert.AreEqual(item.PartsCode, item.Part.Code);
                }
            }
        }
    }
}
