using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Data.Common;
using System.Linq;

using YasT.Framework.Logging;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class EFInventoryActionTest : MemorieDeFleursTestBase
    {
        private static string PartCodeKey = "@part";
        private static string SupplierCodeKey = "@supplier";
        private static string NameKey = "@name";
        private static string ExpectedPartCode = "BA001";
        private static string ExpectedPartName = "薔薇(赤)";
        private static int ExpectedSupplierCode = 1;
        private static string ExpectedSupplierName = "新橋園芸";
        private static string ExpectedSupplierAddress = "東京都中央区銀座";


        public EFInventoryActionTest() : base()
        {
            AfterTestBaseInitializing += AppendSomeObjects;
            BeforeTestBaseCleaningUp += CleanupTestData;
            
        }

        private void AppendSomeObjects(object sender, EventArgs unused)
        {
            LogUtil.Debug($"EFInventoryActionTest#AppendBouquetParts() is called.");
            // Entity 未作成の単品情報は、SQLを使って直接登録する
            AppendBouquetParts();
            AppendSuppliers();
            AppendPartSuppliers();
        }

        private void AppendBouquetParts()
        {
            using (var cmd = TestDB.CreateCommand())
            {
                cmd.CommandText = $"insert into BOUQUET_PARTS values ( {PartCodeKey}, {NameKey}, 1, 100, 3, 0 )";
                cmd.Parameters.AddWithValue(PartCodeKey, ExpectedPartCode);
                cmd.Parameters.AddWithValue(NameKey, ExpectedPartName);
                cmd.ExecuteNonQuery();
            }
        }

        private void AppendSuppliers()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var supplier = new Supplier()
                {
                    Code = ExpectedSupplierCode,
                    Name = ExpectedSupplierName,
                    Address1 = ExpectedSupplierAddress,
                };
                context.Suppliers.Add(supplier);
                context.SaveChanges();
            }
        }

        private void AppendPartSuppliers()
        {
            using (var cmd = TestDB.CreateCommand())
            {
                cmd.CommandText = $"insert into BOUQUET_SUPPLIERS values ( {SupplierCodeKey}, {PartCodeKey} )";
                cmd.Parameters.AddWithValue(SupplierCodeKey, ExpectedSupplierCode);
                cmd.Parameters.AddWithValue(PartCodeKey, ExpectedPartCode);
                cmd.ExecuteNonQuery();
            }
        }

        private void CleanupTestData(object sender, EventArgs unused)
        {
            LogUtil.Debug($"EFInventoryActionTest#CleanupTestData() is called.");

            // テーブル全削除はORマッピングフレームワークが持つ「DBを隠蔽する」意図にそぐわないため
            // DbContext.Customers.Clear() のような操作は用意されていない。
            // DbConnection 経由かDbContext.Database.ExecuteSqlRaw() を使い、DELETEまたはTRUNCATE文を発行すること。
            CleanupTable(TestDB, "INVENTORY_ACTIONS");
            CleanupTable(TestDB, "BOUQUET_SUPPLIERS");
            CleanupTable(TestDB, "BOUQUET_PARTS");
            CleanupTable(TestDB, "SUPPLIERS");
        }

        private void CleanupTable(DbConnection connection, string table)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"delete from {table}";
                cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void CanAddInventoryAction()
        {
            using (var context =new  MemorieDeFleursDbContext(TestDB))
            {
                var code = ExpectedPartCode;

                InventoryAction action = new InventoryAction()
                {
                    ActionDate = new DateTime(DateConst.Year, 03, 30),
                    Action = InventoryActionType.SCHEDULED_TO_ARRIVE,
                    PartsCode = ExpectedPartCode,
                    ArrivalDate = new DateTime(DateConst.Year, 03, 30),
                    InventoryLotNo = 1,
                    Quantity = 200,
                    Remain = 200
                };

                context.InventoryActions.Add(action);
                context.SaveChanges();

                Assert.AreEqual(1, context.InventoryActions
                    .Count(x => x.PartsCode == ExpectedPartCode));
                Assert.IsTrue(context.InventoryActions
                    .Where(x => x.PartsCode == ExpectedPartCode)
                    .All(x => x.BouquetPart.Name == ExpectedPartName));
            }
        }

    }
}
