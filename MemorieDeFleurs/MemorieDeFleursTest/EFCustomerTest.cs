using MemorieDeFleurs.Entities;
using MemorieDeFleurs.Models;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace MemorieDeFleursTest
{
    /// <summary>
    /// EntityFrameworkCore の試行：
    /// DbContext を直接使って CUSTOMERS テーブルを操作するテスト
    /// </summary>
    [TestClass]
    public class EFCustomerTest
    {
        private static string TestDBFile = "./testdata/db/MemorieDeFleurs.db";

        private SqliteConnection TestDB { get; set; }

        private MemorieDeFleursDbContext DbContext { get; set; }

        public EFCustomerTest()
        {
            TestDB = CreateDBConnection(TestDBFile);
        }

        [TestInitialize]
        public void Setup()
        {
            // DbContext を継承したクラスのコンストラクタを適宜用意することで、
            // 接続文字列や DbConnection を DbContext に渡すことができる。
            TestDB.Open();
            DbContext = new MemorieDeFleursDbContext(TestDB);
        }

        [TestCleanup]
        public void TearDown()
        {
            // テーブル全削除はORマッピングフレームワークが持つ「DBを隠蔽する」意図にそぐわないため
            // DbContext.Customers.Clear() のような操作は用意されていない。
            // DbConnection 経由かDbContext.Database.ExecuteSqlRaw() を使い、DELETEまたはTRUNCATE文を発行すること。
            DbContext.Database.ExecuteSqlRaw("delete from CUSTOMERS");
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
        public void CanAddCustomer()
        {
            var customer = new Customer()
            {
                ID = 1,
                NAME = "Test User",
                E_MAIL = "foo@localdomain",
                CARD_NO = "0000111122223333",
                STATUS = 0
            };

            // Add/Remove したら SaveChanges を忘れないこと
            DbContext.Customers.Add(customer);
            DbContext.SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void CannotAddNoNamedCustomer()
        {
            var customer = new Customer()
            {
                ID = 1,
                NAME = null,
                E_MAIL = "foo@localdomain",
                CARD_NO = "0000111122223333",
                STATUS = 0
            };

            // NOT NULL や PRIMARY KEY などの制約違反が発生するのは
            // Add ではなく SaveChanges のタイミング
            DbContext.Customers.Add(customer);
            DbContext.SaveChanges();
        }

        [TestMethod]
        public void CanAddTwoOrMoreCustomersByDifferentID()
        {
            var customer1 = new Customer()
            {
                ID = 1,
                NAME = "Test User",
                E_MAIL = "foo@localdomain",
                CARD_NO = "0000111122223333",
                STATUS = 0
            };
            var customer2 = new Customer()
            {
                ID = 2,
                NAME = "Test User2",
                E_MAIL = "bar@localdomain",
                CARD_NO = "1234567890123456",
                STATUS = 0
            };
            var customer3 = new Customer()
            {
                ID = 3,
                NAME = "Test User3",
                E_MAIL = "hoge@localdomain",
                CARD_NO = "1122334455667788",
                STATUS = 0
            };

            DbContext.Customers.Add(customer1);
            DbContext.Customers.Add(customer2);
            DbContext.Customers.Add(customer3);
            DbContext.SaveChanges();
        }

        [TestMethod,ExpectedException(typeof(InvalidOperationException))]
        public void CannotAddCustomersWithSameID()
        {
            var customer1 = new Customer()
            {
                ID = 1,
                NAME = "Test User",
                E_MAIL = "foo@localdomain",
                CARD_NO = "0000111122223333",
                STATUS = 0
            };
            var customer2 = new Customer()
            {
                ID = 1,
                NAME = "Test User2",
                E_MAIL = "bar@localdomain",
                CARD_NO = "1234567890123456",
                STATUS = 0
            };

            DbContext.Customers.Add(customer1);
            DbContext.Customers.Add(customer2);
            DbContext.SaveChanges();
        }
    }
}
