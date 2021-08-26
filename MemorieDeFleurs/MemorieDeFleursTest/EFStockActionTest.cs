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
    public class EFStockActionTest
    {
        private static string TestDBFile = "./testdata/db/MemorieDeFleurs.db";

        private SqliteConnection TestDB { get; set; }

        private MemorieDeFleursDbContext DbContext { get; set; }

        public EFStockActionTest()
        {
            TestDB = CreateDBConnection(TestDBFile);
        }

        private static string CodeKey = "@code";
        private static string NameKey = "@name";
        private static string ExpectedCode = "BA001";
        private static string ExpectedName = "薔薇(赤)";

        [TestInitialize]
        public void Setup()
        {
            // DbContext を継承したクラスのコンストラクタを適宜用意することで、
            // 接続文字列や DbConnection を DbContext に渡すことができる。
            TestDB.Open();

            using (var cmd = TestDB.CreateCommand())
            {
                cmd.CommandText = $"insert into BOUQUET_PARTS values ( {CodeKey}, {NameKey}, 1, 100, 3, 0 )";
                cmd.Parameters.AddWithValue(CodeKey, ExpectedCode);
                cmd.Parameters.AddWithValue(NameKey, ExpectedName);
                cmd.ExecuteNonQuery();
            }

            DbContext = new MemorieDeFleursDbContext(TestDB);
        }

        [TestCleanup]
        public void TearDown()
        {
            // テーブル全削除はORマッピングフレームワークが持つ「DBを隠蔽する」意図にそぐわないため
            // DbContext.Customers.Clear() のような操作は用意されていない。
            // DbConnection 経由かDbContext.Database.ExecuteSqlRaw() を使い、DELETEまたはTRUNCATE文を発行すること。
            DbContext.Database.ExecuteSqlRaw("delete from STOCK_ACTIONS");
            DbContext.Dispose();
            TestDB.Close();
        }

        private SqliteConnection CreateDBConnection(string dbFileName)
        {
            var builder = new SqliteConnectionStringBuilder();

            builder.DataSource = dbFileName;
            builder.ForeignKeys = true;
            builder.Mode = SqliteOpenMode.ReadWrite;

            return new SqliteConnection(builder.ToString());
        }

        [TestMethod]
        public void CanAddStockAction()
        {
            var code = ExpectedCode;

            StockAction action = new StockAction()
            {
                ACTION_DATE = 20040330,
                ACTION = StockActionType.SCHEDULED_TO_ARRIVE,
                BOUQUET_PARTS_CODE = ExpectedCode,
                ARRIVAL_DATE = 20040330,
                LOT_NO = 1,
                QUANTITY = 200,
                REMAIN = 200
            };

            DbContext.StockActions.Add(action);
            DbContext.SaveChanges();

            Assert.AreEqual(1, DbContext.StockActions
                .Count(x => x.BOUQUET_PARTS_CODE == ExpectedCode));
            Assert.IsTrue(DbContext.StockActions
                .Where(x => x.BOUQUET_PARTS_CODE == ExpectedCode)
                .All(x => x.BouquetPart.NAME == ExpectedName));
        }
    }
}
