using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace MemorieDeFleursTest.ModelEntityTest
{
    /// <summary>
    /// EntityFrameworkCore の試行：
    /// DbContext を直接使って CUSTOMERS テーブルを操作するテスト
    /// </summary>
    [TestClass]
    public class EFCustomerTest : MemorieDeFleursTestBase
    {
        public EFCustomerTest() : base()
        {
            BeforeTestBaseCleaningUp += CleanupDatabase;
        }

        private void CleanupDatabase(object sender, EventArgs unused)
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                // テーブル全削除はORマッピングフレームワークが持つ「DBを隠蔽する」意図にそぐわないため
                // DbContext.Customers.Clear() のような操作は用意されていない。
                // DbConnection 経由かDbContext.Database.ExecuteSqlRaw() を使い、DELETEまたはTRUNCATE文を発行すること。
                context.Database.ExecuteSqlRaw("delete from CUSTOMERS");
            }
        }

        [TestMethod]
        public void CanAddCustomer()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var customer = new Customer()
                {
                    ID = 1,
                    Name = "Test User",
                    EmailAddress = "foo@localdomain",
                    CardNo = "0000111122223333",
                    Status = 0
                };

                // Add/Remove したら SaveChanges を忘れないこと
                context.Customers.Add(customer);
                context.SaveChanges();
            }
        }

        [TestMethod, ExpectedException(typeof(DbUpdateException))]
        public void CannotAddNoNamedCustomer()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var customer = new Customer()
                {
                    ID = 1,
                    Name = null,
                    EmailAddress = "foo@localdomain",
                    CardNo = "0000111122223333",
                    Status = 0
                };

                // NOT NULL や PRIMARY KEY などの制約違反が発生するのは
                // Add ではなく SaveChanges のタイミング
                context.Customers.Add(customer);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void CanAddTwoOrMoreCustomersByDifferentID()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var customer1 = new Customer()
                {
                    ID = 1,
                    Name = "Test User",
                    EmailAddress = "foo@localdomain",
                    CardNo = "0000111122223333",
                    Status = 0
                };
                var customer2 = new Customer()
                {
                    ID = 2,
                    Name = "Test User2",
                    EmailAddress = "bar@localdomain",
                    CardNo = "1234567890123456",
                    Status = 0
                };
                var customer3 = new Customer()
                {
                    ID = 3,
                    Name = "Test User3",
                    EmailAddress = "hoge@localdomain",
                    CardNo = "1122334455667788",
                    Status = 0
                };

                context.Customers.Add(customer1);
                context.Customers.Add(customer2);
                context.Customers.Add(customer3);
                context.SaveChanges();
            }
        }

        [TestMethod,ExpectedException(typeof(InvalidOperationException))]
        public void CannotAddCustomersWithSameID()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var customer1 = new Customer()
                {
                    ID = 1,
                    Name = "Test User",
                    EmailAddress = "foo@localdomain",
                    CardNo = "0000111122223333",
                    Status = 0
                };
                var customer2 = new Customer()
                {
                    ID = 1,
                    Name = "Test User2",
                    EmailAddress = "bar@localdomain",
                    CardNo = "1234567890123456",
                    Status = 0
                };

                context.Customers.Add(customer1);
                context.Customers.Add(customer2);
                context.SaveChanges();
            }
        }
    }
}
