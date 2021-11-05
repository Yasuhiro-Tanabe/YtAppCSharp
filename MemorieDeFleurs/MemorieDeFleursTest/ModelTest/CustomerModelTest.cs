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

        [TestMethod]
        public void Save_NewCustomer()
        {
            var expected = new Customer()
            {
                ID = 10,
                Name = "テスト",
                EmailAddress = "test@localdomain",
                CardNo = "1234123412341234",
            };

            expected.ShippingAddresses.Add(new ShippingAddress() { CustomerID = expected.ID, ID = 1, Name = "お届け先1", Address1 = "住所1", LatestOrderDate = DateConst.April20th });
            expected.ShippingAddresses.Add(new ShippingAddress() { CustomerID = expected.ID, ID = 2, Name = "お届け先2", Address1 = "住所2", LatestOrderDate = DateConst.April20th });
            expected.ShippingAddresses.Add(new ShippingAddress() { CustomerID = expected.ID, ID = 3, Name = "お届け先3", Address1 = "住所3", LatestOrderDate = DateConst.April20th });

            Assert.IsNull(Model.CustomerModel.FindCustomer(expected.ID), $"得意先ID {expected.ID} が未使用ではない");

            Model.CustomerModel.Save(expected);
            var actual = Model.CustomerModel.FindCustomer(expected.ID);

            Assert.AreEqual(expected.Name, actual.Name, "得意先名称が一致しない");
            Assert.AreEqual(expected.ShippingAddresses.Count, actual.ShippingAddresses.Count, "お届け先登録数が一致しない");
            foreach(var address in expected.ShippingAddresses)
            {
                var found = actual.ShippingAddresses.SingleOrDefault(a => a.CustomerID == address.CustomerID && a.ID == address.ID);
                Assert.IsNotNull(found, $"お届け先 {address.ID} が登録されていない");
                Assert.AreEqual(address.Name, found.Name, $"お届け先 {address.ID} の名前が一致しない");
            }
            foreach(var address in actual.ShippingAddresses)
            {
                var found = expected.ShippingAddresses.SingleOrDefault(a => a.CustomerID == address.CustomerID && a.ID == address.ID);
                Assert.IsNotNull(found, $"登録していないはずの {address.ID} が登録されている");
                Assert.AreEqual(found.Name, address.Name, $"お届け先 {address.ID} の名前が一致しない");
            }
        }

        [TestMethod]
        public void Save_CustomerNameChanged()
        {
            var email = "test@localdomain";
            var cardNo = "1234123412341234";
            var name = new[] { "お届け先1", "お届け先2", "お届け先3" };
            var address = new[] { "住所1", "住所2", "住所3" };
            var id = new[] { 1, 2, 3 };
            var EXPECTED_NAME = "変更した名前";

            var initial = Model.CustomerModel.GetCustomerBuilder()
                .NameIs("変更前の名前").EmailAddressIs(email).CardNoIs(cardNo)
                .SendTo(name[0], address[0])
                .SendTo(name[1], address[1])
                .SendTo(name[2], address[2])
                .Create();

            var modified = new Customer()
            {
                ID = initial.ID,
                Name = EXPECTED_NAME,
                EmailAddress = email,
                CardNo = cardNo
            };
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initial.ID, ID = id[0], Name = name[0], Address1 = address[0], LatestOrderDate = DateTime.Now });
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initial.ID, ID = id[1], Name = name[1], Address1 = address[1], LatestOrderDate = DateTime.Now });
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initial.ID, ID = id[2], Name = name[2], Address1 = address[2], LatestOrderDate = DateTime.Now });

            Model.CustomerModel.Save(modified);
            var actual = Model.CustomerModel.FindCustomer(initial.ID);

            Assert.AreEqual(EXPECTED_NAME, actual.Name, "得意先名称が一致しない");
            Assert.AreEqual(name.Length, actual.ShippingAddresses.Count, "お届け先登録数が一致しない");
            for(int i = 0; i < name.Length; i++)
            {
                var found = actual.ShippingAddresses.SingleOrDefault(a => a.CustomerID == initial.ID && a.ID == id[i]);
                Assert.IsNotNull(found, $"お届け先 {id[i]} が登録されていない");
                Assert.AreEqual(name[i], found.Name, $"お届け先 {id[i]} の名前が一致しない");
            }
            foreach (var shipping in actual.ShippingAddresses)
            {
                Assert.IsTrue(id.Contains(shipping.ID), $"登録していないはずの {shipping.ID} が登録されている:Name={shipping.Name}");
            }
        }

        [TestMethod]
        public void Save_ShippingAddressAdded()
        {
            var name = "得意先1";
            var email = "test@localdomain";
            var cardNo = "1234123412341234";
            var shipName = new[] { "お届け先1", "お届け先2", "お届け先3", "お届け先4" };
            var shipAddr = new[] { "住所1", "住所2", "住所3", "住所4" };
            var shipID = new[] { 1, 2, 3, 4 };
            var MODIFIED_NAME = "変更後の名前";

            var initial = Model.CustomerModel.GetCustomerBuilder()
                .NameIs(name).EmailAddressIs(email).CardNoIs(cardNo)
                .SendTo(shipName[0], shipAddr[0])
                .SendTo(shipName[1], shipAddr[1])
                .SendTo(shipName[2], shipAddr[2])
                .Create();

            var modified = new Customer()
            {
                ID = initial.ID,
                Name = MODIFIED_NAME,
                EmailAddress = email,
                CardNo = cardNo
            };
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initial.ID, ID = shipID[0], Name = shipName[0], Address1 = shipName[0], LatestOrderDate = DateTime.Now });
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initial.ID, ID = shipID[1], Name = shipName[1], Address1 = shipName[1], LatestOrderDate = DateTime.Now });
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initial.ID, ID = shipID[2], Name = shipName[2], Address1 = shipName[2], LatestOrderDate = DateTime.Now });
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initial.ID, ID = shipID[3], Name = shipName[3], Address1 = shipAddr[3], LatestOrderDate = DateTime.Now });

            Model.CustomerModel.Save(modified);
            var actual = Model.CustomerModel.FindCustomer(initial.ID);

            Assert.AreEqual(MODIFIED_NAME, actual.Name, "得意先名称が一致しない");
            Assert.AreEqual(shipName.Length, actual.ShippingAddresses.Count, "お届け先登録数が一致しない");
            for (int i = 0; i < shipName.Length; i++)
            {
                var found = actual.ShippingAddresses.SingleOrDefault(a => a.CustomerID == initial.ID && a.ID == shipID[i]);
                Assert.IsNotNull(found, $"お届け先 {shipID[i]} が登録されていない");
                Assert.AreEqual(shipName[i], found.Name, $"お届け先 {shipID[i]} の名前が一致しない");
            }
            foreach (var shipping in actual.ShippingAddresses)
            {
                Assert.IsTrue(shipID.Contains(shipping.ID), $"登録していないはずの {shipping.ID} が登録されている:Name={shipping.Name}");
            }
        }

        [TestMethod]
        public void Save_ShippingAddressRemoved()
        {
            var name = "得意先1";
            var email = "test@localdomain";
            var cardNo = "1234123412341234";
            var MODIFIED_NAME = "変更後の名前";
            var initialID = 0;

            // 初期登録するお届け先
            var shipName = new[] { "お届け先1", "お届け先2", "お届け先3", "お届け先4" };
            var shipAddr = new[] { "住所1", "住所2", "住所3", "住所4" };
            var shipID = new[] { 1, 2, 3, 4 };

            // お届け先確認用：初期登録から「お届け先3」を除いたもの
            var newShipName = new[] { "お届け先1", "お届け先2", "お届け先4" };
            var newShipID = new[] { 1, 2, 4 };

            var initial = Model.CustomerModel.GetCustomerBuilder()
                .NameIs(name).EmailAddressIs(email).CardNoIs(cardNo)
                .SendTo(shipName[0], shipAddr[0])
                .SendTo(shipName[1], shipAddr[1])
                .SendTo(shipName[2], shipAddr[2])
                .SendTo(shipName[3], shipAddr[3])
                .Create();
            initialID = initial.ID;

            var modified = new Customer()
            {
                ID = initial.ID,
                Name = MODIFIED_NAME,
                EmailAddress = email,
                CardNo = cardNo
            };
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initialID, ID = shipID[0], Name = shipName[0], Address1 = shipName[0], LatestOrderDate = DateTime.Now });
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initialID, ID = shipID[1], Name = shipName[1], Address1 = shipName[1], LatestOrderDate = DateTime.Now });
            modified.ShippingAddresses.Add(new ShippingAddress() { CustomerID = initialID, ID = shipID[3], Name = shipName[3], Address1 = shipAddr[3], LatestOrderDate = DateTime.Now });

            Model.CustomerModel.Save(modified);

            var actual = Model.CustomerModel.FindCustomer(initialID);


            Assert.AreEqual(MODIFIED_NAME, actual.Name, "得意先名称が一致しない");
            Assert.AreEqual(newShipID.Length, actual.ShippingAddresses.Count, "お届け先登録数が一致しない");
            for (int i = 0; i < newShipID.Length; i++)
            {
                var found = actual.ShippingAddresses.SingleOrDefault(a => a.CustomerID == initialID && a.ID == newShipID[i]);
                Assert.IsNotNull(found, $"お届け先 {newShipID[i]} が登録されていない");
                Assert.AreEqual(newShipName[i], found.Name, $"お届け先 {newShipID[i]} の名前が一致しない");
            }
            foreach (var shipping in actual.ShippingAddresses)
            {
                Assert.IsTrue(newShipID.Contains(shipping.ID), $"登録していないはずの {shipping.ID} が登録されている:Name={shipping.Name}");
            }
        }
    }
}
