using MemorieDeFleurs;
using MemorieDeFleurs.Models.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class EFStockActionTest : MemorieDeFleursDbContextTestBase
    {
        private static string PartCodeKey = "@part";
        private static string SupplierCodeKey = "@supplier";
        private static string NameKey = "@name";
        private static string ExpectedPartCode = "BA001";
        private static string ExpectedPartName = "薔薇(赤)";
        private static int ExpectedSupplierCode = 1;
        private static string ExpectedSupplierName = "新橋園芸";
        private static string ExpectedSupplierAddress = "東京都中央区銀座";


        public EFStockActionTest() : base()
        {
            AfterTestBaseInitializing += AppendSomeObjects;
            BeforeTestBaseCleaningUp += CleanupTestData;
            
        }

        private void AppendSomeObjects(object sender, EventArgs unused)
        {
            LogUtil.Debug($"EFStockActionTest#AppendBouquetParts() is called.");
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
            var supplier = new Supplier()
            {
                Code = ExpectedSupplierCode,
                Name = ExpectedSupplierName,
                Address1 = ExpectedSupplierAddress,
            };
            TestDBContext.Suppliers.Add(supplier);
            TestDBContext.SaveChanges();
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
            LogUtil.Debug($"EFStockActionTest#CleanupTestData() is called.");

            // テーブル全削除はORマッピングフレームワークが持つ「DBを隠蔽する」意図にそぐわないため
            // DbContext.Customers.Clear() のような操作は用意されていない。
            // DbConnection 経由かDbContext.Database.ExecuteSqlRaw() を使い、DELETEまたはTRUNCATE文を発行すること。
            TestDBContext.Database.ExecuteSqlRaw("delete from STOCK_ACTIONS");
            TestDBContext.Database.ExecuteSqlRaw("delete from BOUQUET_SUPPLIERS");
            TestDBContext.Database.ExecuteSqlRaw("delete from BOUQUET_PARTS");
            TestDBContext.Database.ExecuteSqlRaw("delete from SUPPLIERS");
        }

        [TestMethod]
        public void CanAddStockAction()
        {
            var code = ExpectedPartCode;

            StockAction action = new StockAction()
            {
                ActionDate = new DateTime(2004,03,30),
                Action = StockActionType.SCHEDULED_TO_ARRIVE,
                PartsCode = ExpectedPartCode,
                ArrivalDate = new DateTime(2004,03,30),
                StockLotNo = 1,
                Quantity = 200,
                Remain = 200
            };

            TestDBContext.StockActions.Add(action);
            TestDBContext.SaveChanges();

            Assert.AreEqual(1, TestDBContext.StockActions
                .Count(x => x.PartsCode == ExpectedPartCode));
            Assert.IsTrue(TestDBContext.StockActions
                .Where(x => x.PartsCode == ExpectedPartCode)
                .All(x => x.BouquetPart.Name == ExpectedPartName));
        }

        [TestMethod]
        public void LearnEFCoreTransaction_RollbackIsAvailable()
        {
            using (var transaction = TestDBContext.Database.BeginTransaction())
            {
                Assert.AreEqual(0, TestDBContext.StockActions.Count(), "事前検証エラー：StockActionsが空でない");
                try
                {
                    StockAction action = new StockAction()
                    {
                        ActionDate = new DateTime(2004, 03, 30),
                        Action = StockActionType.SCHEDULED_TO_ARRIVE,
                        PartsCode = ExpectedPartCode,
                        ArrivalDate = new DateTime(2004, 03, 30),
                        StockLotNo = 1,
                        Quantity = 200,
                        Remain = 200
                    };

                    TestDBContext.StockActions.Add(action);
                    TestDBContext.SaveChanges();
                    Assert.AreEqual(1, TestDBContext.StockActions.Count(), "登録されている在庫アクション数は１つのはず");
                    throw new Exception();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                }

                Assert.AreEqual(0, TestDBContext.StockActions.Count(), "ロールバック後も在庫アクションが残っている");
            }
        }

        [TestMethod]
        public void LearnEFCoreTransaction_CannotUseMultipeTransactionsInSingleDbContext()
        {
            using(var tansaction1 = TestDBContext.Database.BeginTransaction())
            {
                Assert.ThrowsException<InvalidOperationException>(() => TestDBContext.Database.BeginTransaction());
            }
        }
    }
}
