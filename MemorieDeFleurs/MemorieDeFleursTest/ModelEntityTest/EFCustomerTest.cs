using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

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
                context.Database.ExecuteSqlRaw("delete from SHIPPING_ADDRESS");
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

        [TestMethod]
        public void RelationOfCustomerAndShippingAddress()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                context.Customers.Add(new Customer() { ID = 2, EmailAddress = "user2@localdomain", Name = "ユーザ2", CardNo = "1234123412341234" });
                context.Customers.Add(new Customer() { ID = 3, EmailAddress = "user3@localdomain", Name = "ユーザ3", CardNo = "1234567890123450" });

                var date = new DateTime(DateConst.Year, 1, 1);
                context.ShippingAddresses.Add(new ShippingAddress() { ID = 7, CustomerID = 2, Address1 = "住所2-1", Name = "友人A", LatestOrderDate = date });
                context.ShippingAddresses.Add(new ShippingAddress() { ID = 8, CustomerID = 2, Address1 = "住所2-2", Name = "友人B", LatestOrderDate = date });
                context.ShippingAddresses.Add(new ShippingAddress() { ID = 9, CustomerID = 2, Address1 = "住所2-3", Name = "友人C", LatestOrderDate = date });

                context.ShippingAddresses.Add(new ShippingAddress() { ID = 14, CustomerID = 3, Address1 = "住所3-1-1", Name = "友人V", LatestOrderDate = date });
                context.ShippingAddresses.Add(new ShippingAddress() { ID = 15, CustomerID = 3, Address1 = "住所3-1-2", Name = "友人W", LatestOrderDate = date });
                context.ShippingAddresses.Add(new ShippingAddress() { ID = 16, CustomerID = 3, Address1 = "住所3-1-3", Name = "友人X", LatestOrderDate = date });
                context.ShippingAddresses.Add(new ShippingAddress() { ID = 17, CustomerID = 3, Address1 = "住所3-1-4", Name = "友人Y", LatestOrderDate = date });

                context.SaveChanges();
            }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                // 関連を正しくトラッキングするためには Customer と ShippingAddress の両方を取得する必要あり
                var customer = context.Customers.Find(2);
                var addresses = context.ShippingAddresses.Where(addr => addr.CustomerID == customer.ID).ToList();

                Assert.IsNotNull(customer.ShippingAddresses);
                Assert.AreEqual(3, customer.ShippingAddresses.Count);
                Assert.AreEqual(customer.ID, customer.ShippingAddresses[0].CustomerID);
            }
        }
    }
}
