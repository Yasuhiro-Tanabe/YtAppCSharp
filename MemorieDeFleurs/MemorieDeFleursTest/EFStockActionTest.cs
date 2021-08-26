using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class EFStockActionTest : MemorieDeFleursDbContextTestBase
    {
        private static string CodeKey = "@code";
        private static string NameKey = "@name";
        private static string ExpectedCode = "BA001";
        private static string ExpectedName = "薔薇(赤)";


        public EFStockActionTest() : base()
        {
            AfterTestBaseInitializing += AppendBouquetParts;
            BeforeTestBaseCleaningUp += CleanupTestData;
            
        }

        private void AppendBouquetParts(object sender, EventArgs unused)
        {
            LogUtil.Debug($"EFStockActionTest#AppendBouquetParts() is called.");
            // Entity 未作成の単品情報は、SQLを使って直接登録する
            using (var cmd = TestDB.CreateCommand())
            {
                cmd.CommandText = $"insert into BOUQUET_PARTS values ( {CodeKey}, {NameKey}, 1, 100, 3, 0 )";
                cmd.Parameters.AddWithValue(CodeKey, ExpectedCode);
                cmd.Parameters.AddWithValue(NameKey, ExpectedName);
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
        }

        [TestMethod]
        public void CanAddStockAction()
        {
            var code = ExpectedCode;

            StockAction action = new StockAction()
            {
                ActionDate = 20040330,
                Action = StockActionType.SCHEDULED_TO_ARRIVE,
                PartsCode = ExpectedCode,
                ArrivalDate = 20040330,
                StockLotNo = 1,
                Quantity = 200,
                Remain = 200
            };

            TestDBContext.StockActions.Add(action);
            TestDBContext.SaveChanges();

            Assert.AreEqual(1, TestDBContext.StockActions
                .Count(x => x.PartsCode == ExpectedCode));
            Assert.IsTrue(TestDBContext.StockActions
                .Where(x => x.PartsCode == ExpectedCode)
                .All(x => x.BouquetPart.Name == ExpectedName));
        }
    }
}
