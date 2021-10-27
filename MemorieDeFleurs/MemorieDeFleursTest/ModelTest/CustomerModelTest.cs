using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class CustomerModelTest : MemorieDeFleursModelTestBase
    {
        [TestMethod]
        public void CanAddCustomerViaModel()
        {
            var expected = Model.CustomerModel.GetCustomerBuilder()
                .NameIs("蘇我幸恵")
                .PasswordIs("sogayukie12345")
                .EmailAddressIs("ysoga@localdomain")
                .CardNoIs("9876543210123210")
                .Create();

            var actual = Model.CustomerModel.FindCustomer(expected.ID);

            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.Password, actual.Password);
        }

        [TestMethod]
        public void CanAddShippingAddressFromCustomerViaBuilder()
        {
            var customer = Model.CustomerModel.GetCustomerBuilder()
                .NameIs("蘇我幸恵")
                .PasswordIs("sogayukie12345")
                .EmailAddressIs("ysoga@localdomain")
                .CardNoIs("9876543210123210")
                .Create();

            var expected = Model.CustomerModel.GetShippingAddressBuilder()
                .From(customer)
                .To("ピアノ生徒1")
                .AddressIs("東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                .Create();

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var actualShippingAddress = context.ShippingAddresses.Single();
                var actualCustomer = context.Customers.Find(customer.ID);

                Assert.AreEqual(expected.Name, actualShippingAddress.Name);
                Assert.IsNotNull(actualShippingAddress.Customer);
                Assert.AreEqual(customer.ID, actualShippingAddress.Customer.ID);

                Assert.IsNotNull(actualCustomer.ShippingAddresses);
                Assert.AreEqual(1, actualCustomer.ShippingAddresses.Count());
                Assert.AreEqual(actualShippingAddress.ID, actualCustomer.ShippingAddresses[0].ID);
            }
        }

        [TestMethod]
        public void FindAllCustomers()
        {
            var expectedCustomers = new List<Customer>()
            {
                Model.CustomerModel.GetCustomerBuilder()
                    .EmailAddressIs("ysoga@localdomain").NameIs("蘇我幸恵").CardNoIs("1234123412341234").Create(),
                Model.CustomerModel.GetCustomerBuilder()
                    .EmailAddressIs("user2@localdomain").NameIs("ユーザー2").CardNoIs("4321432143214321").Create(),
            };
            var ba001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();
            var ht001 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses("BA001", 4)
                .Create();
            var supplier = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .Create();

            var actualCustomers = Model.CustomerModel.FindAllCustomers();

            Assert.AreEqual(expectedCustomers.Count, actualCustomers.Count(), "得意先登録数が合わない");
            foreach(var expected in expectedCustomers)
            {
                Assert.AreEqual(1, actualCustomers.Count(c => c.ID == expected.ID), $"得意先{expected.ID}({expected.Name}) の登録数が合わない");
            }
        }

        [TestMethod]
        public void RemoveCusomer_SucceededWhenTheCustomerHasNoOrdersYet()
        {
            var expectedCustomers = new List<Customer>()
            {
                Model.CustomerModel.GetCustomerBuilder()
                    .EmailAddressIs("ysoga@localdomain").NameIs("蘇我幸恵")
                    .SendTo("ピアノ生徒1", "東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                    .CardNoIs("1234123412341234").Create(),
                Model.CustomerModel.GetCustomerBuilder()
                    .SendTo("友人A", "住所2-1")
                    .EmailAddressIs("user2@localdomain").NameIs("ユーザー2").CardNoIs("4321432143214321").Create(),
            };
            var targetCustomer = Model.CustomerModel.GetCustomerBuilder()
                    .EmailAddressIs("user3@localdomain").NameIs("ユーザー3").CardNoIs("9876543212345678")
                    .SendTo("友人V",  "住所3-1-1") // 発注履歴がないのにお届け先が登録されていることはないはずだが、念のため。
                    .Create();

            var ba001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();
            var ht001 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses("BA001", 4)
                .Create();
            var supplier = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .Create();

            var lot = Model.SupplierModel.Order(DateConst.April20th, supplier.Code, DateConst.April30th, Tuple.Create("BA001", 1));
            var orders = new List<string>()
            {
                Model.CustomerModel.Order(DateConst.April25th, ht001, expectedCustomers[0].ShippingAddresses[0], DateConst.May1st),
                Model.CustomerModel.Order(DateConst.April25th, ht001, expectedCustomers[1].ShippingAddresses[0], DateConst.May2nd),
            };

            Model.CustomerModel.RemoveCustomer(targetCustomer.ID);

            var actualCustomers = Model.CustomerModel.FindAllCustomers();

            Assert.AreEqual(expectedCustomers.Count, actualCustomers.Count(), "得意先の登録件数が合わない");
            Assert.IsNull(expectedCustomers.SingleOrDefault(c => c.ID == targetCustomer.ID), $"削除したはずの得意先{targetCustomer.ID}({targetCustomer.Name}) が登録されている");
        }

        [TestMethod]
        public void RemoveCustomer_FailedWhenTheCustomerHasOrders()
        {
            var expectedCustomers = new List<Customer>()
            {
                Model.CustomerModel.GetCustomerBuilder()
                    .EmailAddressIs("ysoga@localdomain").NameIs("蘇我幸恵")
                    .SendTo("ピアノ生徒1", "東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                    .CardNoIs("1234123412341234").Create(),
                Model.CustomerModel.GetCustomerBuilder()
                    .SendTo("友人A", "住所2-1")
                    .EmailAddressIs("user2@localdomain").NameIs("ユーザー2").CardNoIs("4321432143214321").Create(),
            };

            var ba001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();
            var ht001 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses("BA001", 4)
                .Create();
            var supplier = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .Create();

            var lot = Model.SupplierModel.Order(DateConst.April20th, supplier.Code, DateConst.April30th, Tuple.Create("BA001", 1));
            var orders = new List<string>()
            {
                Model.CustomerModel.Order(DateConst.April25th, ht001, expectedCustomers[0].ShippingAddresses[0], DateConst.May1st),
                Model.CustomerModel.Order(DateConst.April25th, ht001, expectedCustomers[1].ShippingAddresses[0], DateConst.May2nd),
            };

            Assert.ThrowsException<ApplicationException>(() => Model.CustomerModel.RemoveCustomer(expectedCustomers[0].ID));

            // 得意先削除が失敗しているので，得意先は登録時のままであるはず
            var actualCustomers = Model.CustomerModel.FindAllCustomers();

            Assert.AreEqual(expectedCustomers.Count, actualCustomers.Count(), "得意先登録数が合わない");
            foreach (var expected in expectedCustomers)
            {
                Assert.AreEqual(1, actualCustomers.Count(c => c.ID == expected.ID), $"得意先{expected.ID}({expected.Name}) の登録数が合わない");
            }
        }
    }
}
