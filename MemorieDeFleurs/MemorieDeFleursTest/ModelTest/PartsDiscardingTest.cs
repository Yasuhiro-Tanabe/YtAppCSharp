using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class PartsDiscardingTest :MemorieDeFleursModelTestBase
    {
        #region テスト用データ
        private class PartsInfo
        {
            public BouquetPart BA001 { get; set; }
            public BouquetPart BA002 { get; set; }
            public BouquetPart BA003 { get; set; }

            public BouquetPart GP001 { get; set; }
            public BouquetPart CN001 { get; set; }
            public BouquetPart CN002 { get; set; }
        }
        private PartsInfo BouquetParts { get; } = new PartsInfo();

        private class ProductsInfo
        {
            public Bouquet HT001 { get; set; }
            public Bouquet HT002 { get; set; }
            public Bouquet HT003 { get; set; }
            public Bouquet HT004 { get; set; }

            public Bouquet HT005 { get; set; }
            public Bouquet HT006 { get; set; }
            public Bouquet HT007 { get; set; }

        }
        private ProductsInfo Bouquets { get; } = new ProductsInfo();

        private class CustomerInfo
        {
            public Customer Customer1 { get; set; }
            public Customer Customer2 { get; set; }
            public Customer Customer3 { get; set; }
        }
        private CustomerInfo Customers { get; } = new CustomerInfo();

        private class SupplierInfo
        {
            public Supplier Supplier1 { get; set; }
            public Supplier Supplier2 { get; set; }
        }
        private SupplierInfo Suppliers { get; } = new SupplierInfo();

        private IDictionary<DateTime, IList<string>> InitialOrdersToSupplyer { get; } = new SortedDictionary<DateTime, IList<string>>();

        private IDictionary<string, TestOrder> InitialLots { get; } = new SortedDictionary<string, TestOrder>();

        private IDictionary<DateTime, IList<string>> InitialOrdersFromCustomers { get; } = new SortedDictionary<DateTime, IList<string>>();
        #endregion // テスト用データ

        public PartsDiscardingTest()
        {
            AfterTestBaseInitializing += PrepareModel;
        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    PrepareParts(context);
                    PrepareBouquets(context);
                    PrepareCustomers(context);
                    PrepareSuppliers(context);
                    PrepareInitialOrderToSupplier(context);
                    PrepareOrdersFromCustomer(context);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                // 登録データ確定後に呼び出す
                GetInitialLotNumbers(context);
                GetInitialOrdersToSupplier(context);
                GetInitialOrdersFromCustomer(context);
            }
        }

        private void PrepareParts(MemorieDeFleursDbContext context)
        {
            BouquetParts.BA001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            BouquetParts.BA002 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA002")
                .PartNameIs("薔薇(白)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            BouquetParts.BA003 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA003")
                .PartNameIs("薔薇(ピンク)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            BouquetParts.GP001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("GP001")
                .PartNameIs("かすみ草")
                .LeadTimeIs(2)
                .QauntityParLotIs(50)
                .ExpiryDateIs(2)
                .Create(context);

            BouquetParts.CN001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN001")
                .PartNameIs("カーネーション(赤)")
                .LeadTimeIs(3)
                .QauntityParLotIs(50)
                .ExpiryDateIs(5)
                .Create(context);

            BouquetParts.CN002 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN002")
                .PartNameIs("カーネーション(ピンク)")
                .LeadTimeIs(3)
                .QauntityParLotIs(50)
                .ExpiryDateIs(5)
                .Create(context);
        }

        private void PrepareBouquets(MemorieDeFleursDbContext context)
        {
            Bouquets.HT001 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses("BA001", 4)
                .Create(context);

            Bouquets.HT002 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT002")
                .NameIs("花束-Bセット")
                .Uses("BA001", 3)
                .Uses("BA003", 3)
                .Uses("GP001", 6)
                .Create(context);

            Bouquets.HT003 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT003")
                .NameIs("一輪挿し-A")
                .Uses("BA003", 1)
                .Create(context);

            Bouquets.HT004 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT004")
                .NameIs("結婚式用ブーケ")
                .Uses("BA002", 3)
                .Uses("BA003", 5)
                .Uses("GP001", 3)
                .Uses("CN002", 3)
                .Create(context);

            Bouquets.HT005 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT005")
                .NameIs("母の日感謝セット")
                .Uses("CN001", 6)
                .Uses("CN002", 6)
                .Create(context);

            Bouquets.HT006 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT006")
                .NameIs("花束-Cセット")
                .Uses("BA001", 6)
                .Uses("BA002", 4)
                .Uses("BA003", 3)
                .Uses("GP001", 3)
                .Uses("CN001", 2)
                .Uses("CN002", 5)
                .Create(context);

            Bouquets.HT007 = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT007")
                .NameIs("還暦祝い60本セット")
                .Uses("BA001", 40)
                .Uses("BA002", 10)
                .Uses("BA003", 10)
                .Create(context);
        }

        private void PrepareCustomers(MemorieDeFleursDbContext context)
        {
            Customers.Customer1 = Model.CustomerModel.GetCustomerBuilder()
                .NameIs("蘇我幸恵")
                .EmailAddressIs("ysoga@localdomain")
                .CardNoIs("9876543210123210")
                .SendTo("ピアノ生徒1", "東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                .SendTo("ピアノ生徒2", "茨城県水戸市城南2-1-20", "井門水戸ビル5階")
                .SendTo("友人A", "静岡県浜松市中区板屋町111-2", "浜松アクトタワー10階")
                .SendTo("ピアノ生徒3", "愛知県名古屋市西区上名古屋3-25-28", "第7猪村ビル4階")
                .SendTo("ピアノ生徒4", "大阪府吹田市垂水町3-9-10", "白川ビル3階")
                .SendTo("友人B", "福岡県福岡市博多区博多駅南2-8-16", "東洋マンション駅南スターオフィス2階")
                .Create(context);

            Customers.Customer2 = Model.CustomerModel.GetCustomerBuilder()
                .NameIs("ユーザ2")
                .EmailAddressIs("user2@localdomain")
                .CardNoIs("1234123412341230")
                .SendTo("友人A", "住所2-1")
                .SendTo("友人B", "住所2-2", "建物2A")
                .SendTo("友人C", "住所2-3")
                .SendTo("生徒1", "住所2-4")
                .SendTo("生徒2", "住所2-5", "建物2B")
                .SendTo("生徒3", "住所2-6")
                .SendTo("生徒4", "住所2-7")
                .Create(context);

            Customers.Customer3 = Model.CustomerModel.GetCustomerBuilder()
                .NameIs("ユーザ3")
                .EmailAddressIs("user3@localdomain")
                .CardNoIs("1234567890123450")
                .SendTo("友人V", "住所3-1-1")
                .SendTo("友人W", "住所3-1-2")
                .SendTo("友人X", "住所3-1-3")
                .SendTo("友人Y", "住所3-1-4")
                .SendTo("友人Z", "住所3-1-5")
                .SendTo("親戚1", "住所3-1-6")
                .Create(context);
        }

        private void PrepareSuppliers(MemorieDeFleursDbContext context)
        {

            Suppliers.Supplier1 = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .EmailIs("shinbashi@localdomain")
                .PhoneNumberIs("00012345678")
                .FaxNumberIs("00012345677")
                .SupplyParts("BA001", "BA002", "BA003", "GP001")
                .Create(context);

            Suppliers.Supplier2 = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("木挽町花壇")
                .AddressIs("東京都中央区銀座四丁目121-5")
                .EmailIs("kobiki@localdomain")
                .PhoneNumberIs("09098765432")
                .FaxNumberIs("0120000000")
                .SupplyParts("GP001", "CN001", "CN002")
                .Create(context);
        }

        private void PrepareInitialOrderToSupplier(MemorieDeFleursDbContext context)
        {
            var orderDate = new DateTime(2020, 3, 10);

            // 仕入先1
            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier1, DateConst.April30th,
                new[] {
                            Tuple.Create(BouquetParts.BA001, 2),
                            Tuple.Create(BouquetParts.BA002, 1),
                            Tuple.Create(BouquetParts.BA003, 1),
                            Tuple.Create(BouquetParts.GP001, 1),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier1, DateConst.May1st,
                new[] {
                    Tuple.Create(BouquetParts.BA001, 3),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier1, DateConst.May2nd,
                new[] {
                    Tuple.Create(BouquetParts.BA001, 2),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier1, DateConst.May3rd,
                new[] {
                    Tuple.Create(BouquetParts.BA001, 2),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier1, DateConst.May3rd,
                new[] {
                    Tuple.Create(BouquetParts.BA002, 1),
                    Tuple.Create(BouquetParts.BA003, 2),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier1, DateConst.May3rd,
                new[] {
                    Tuple.Create(BouquetParts.BA002, 1),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier1, DateConst.May6th,
                new[] {
                    Tuple.Create(BouquetParts.BA001, 1),
                });

            // 仕入先2
            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier2, DateConst.April30th,
                new[]
                {
                    Tuple.Create(BouquetParts.CN001, 1),
                    Tuple.Create(BouquetParts.CN002, 1),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier2, DateConst.May1st,
                new[]
                {
                    Tuple.Create(BouquetParts.CN002, 1),
                });


            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier2, DateConst.May2nd,
                new[]
                {
                    Tuple.Create(BouquetParts.GP001, 1),
                    Tuple.Create(BouquetParts.CN001, 1),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier2, DateConst.May3rd,
                new[]
                {
                    Tuple.Create(BouquetParts.CN002, 1),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers.Supplier2, DateConst.May5th,
                new[]
                {
                    Tuple.Create(BouquetParts.GP001, 1),
                    Tuple.Create(BouquetParts.CN002, 1),
                });

        }

        private void PrepareOrdersFromCustomer(MemorieDeFleursDbContext context)
        {
            var orderDate = new DateTime(2020, 3, 1);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer1.ShippingAddresses[0], DateConst.May1st, "Message1");
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer1.ShippingAddresses[1], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer1.ShippingAddresses[2], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer1.ShippingAddresses[3], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer1.ShippingAddresses[4], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer1.ShippingAddresses[5], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer2.ShippingAddresses[0], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer2.ShippingAddresses[1], DateConst.May1st, "Message2");
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer2.ShippingAddresses[2], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer2.ShippingAddresses[3], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer2.ShippingAddresses[4], DateConst.May1st);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer2.ShippingAddresses[5], DateConst.May1st);

            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[6], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer3.ShippingAddresses[0], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer3.ShippingAddresses[1], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer3.ShippingAddresses[2], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer3.ShippingAddresses[3], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT003, Customers.Customer3.ShippingAddresses[4], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT003, Customers.Customer3.ShippingAddresses[5], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT003, Customers.Customer1.ShippingAddresses[0], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer1.ShippingAddresses[1], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer1.ShippingAddresses[2], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer1.ShippingAddresses[3], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[4], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[5], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer2.ShippingAddresses[0], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer2.ShippingAddresses[1], DateConst.May2nd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer2.ShippingAddresses[2], DateConst.May2nd);

            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[3], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[4], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[5], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[6], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT002, Customers.Customer3.ShippingAddresses[0], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT002, Customers.Customer3.ShippingAddresses[1], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer3.ShippingAddresses[2], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer3.ShippingAddresses[3], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer3.ShippingAddresses[4], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer3.ShippingAddresses[5], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[0], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[1], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[2], DateConst.May3rd);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer1.ShippingAddresses[3], DateConst.May3rd);

            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer1.ShippingAddresses[4], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer1.ShippingAddresses[5], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT002, Customers.Customer2.ShippingAddresses[0], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT002, Customers.Customer2.ShippingAddresses[1], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT002, Customers.Customer2.ShippingAddresses[2], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT002, Customers.Customer2.ShippingAddresses[3], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT003, Customers.Customer2.ShippingAddresses[4], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT003, Customers.Customer2.ShippingAddresses[5], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer2.ShippingAddresses[6], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer3.ShippingAddresses[0], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer3.ShippingAddresses[1], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer3.ShippingAddresses[2], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer3.ShippingAddresses[3], DateConst.May4th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT005, Customers.Customer3.ShippingAddresses[4], DateConst.May4th);

            Model.CustomerModel.Order(context, orderDate, Bouquets.HT003, Customers.Customer3.ShippingAddresses[5], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT003, Customers.Customer1.ShippingAddresses[0], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT003, Customers.Customer1.ShippingAddresses[1], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer1.ShippingAddresses[2], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer1.ShippingAddresses[3], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer1.ShippingAddresses[4], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer1.ShippingAddresses[5], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer2.ShippingAddresses[0], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer2.ShippingAddresses[1], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer2.ShippingAddresses[2], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer2.ShippingAddresses[3], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer2.ShippingAddresses[4], DateConst.May5th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer2.ShippingAddresses[5], DateConst.May5th);

            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[6], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer3.ShippingAddresses[0], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer3.ShippingAddresses[1], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer3.ShippingAddresses[2], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer3.ShippingAddresses[3], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer3.ShippingAddresses[4], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer3.ShippingAddresses[5], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[0], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[1], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[2], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer1.ShippingAddresses[3], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer1.ShippingAddresses[4], DateConst.May6th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT007, Customers.Customer1.ShippingAddresses[5], DateConst.May6th);

            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[0], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[1], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[2], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT001, Customers.Customer2.ShippingAddresses[3], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer2.ShippingAddresses[4], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer2.ShippingAddresses[5], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer2.ShippingAddresses[6], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer3.ShippingAddresses[0], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer3.ShippingAddresses[1], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT004, Customers.Customer3.ShippingAddresses[2], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer3.ShippingAddresses[3], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer3.ShippingAddresses[4], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer3.ShippingAddresses[5], DateConst.May7th);
            Model.CustomerModel.Order(context, orderDate, Bouquets.HT006, Customers.Customer1.ShippingAddresses[0], DateConst.May7th);
        }

        private void GetInitialLotNumbers(MemorieDeFleursDbContext context)
        {
            LogUtil.DEBUGLOG_BeginMethod();
            var arrivedActions = context.InventoryActions
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE)
                .OrderBy(act => act.PartsCode)
                .ThenBy(act => act.ArrivalDate)
                .ToList();

            foreach (var grp in arrivedActions
                .GroupBy(act => act.PartsCode))
            {
                var order = new TestOrder();
                foreach (var act in grp)
                {
                    order.Append(act.ArrivalDate, act.InventoryLotNo, act.Quantity);
                }
                InitialLots.Add(grp.Key, order);
                LogUtil.DebugFormat($"{LogUtil.Indent}{grp.Key}=[{order.ToString()}]");
            }
            LogUtil.DEBUGLOG_EndMethod();
        }

        private void GetInitialOrdersToSupplier(MemorieDeFleursDbContext context)
        {
            LogUtil.DEBUGLOG_BeginMethod();
            try
            {
                foreach (var grp in context.OrdersToSuppliers.ToList()
                    .OrderBy(order => order.DeliveryDate)
                    .GroupBy(o => o.DeliveryDate))
                {
                    var list = new List<string>();

                    foreach (var order in grp)
                    {
                        list.Add(order.ID);
                    }

                    InitialOrdersToSupplyer.Add(grp.Key, list.ToList());
                    LogUtil.DebugFormat($"{LogUtil.Indent}{grp.Key:yyyyMMdd}: {string.Join(", ", list)}");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }

        private void GetInitialOrdersFromCustomer(MemorieDeFleursDbContext context)
        {
            foreach (var grp in context.OrderFromCustomers.AsEnumerable().GroupBy(order => order.ShippingDate))
            {
                InitialOrdersFromCustomers.Add(grp.Key, grp.Select(order => order.ID).ToList());
            }
        }
        #endregion // TestInitialize

        [TestMethod]
        public void DiscardOneParts()
        {
            LogUtil.DEBUGLOG_BeginTest();

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            Model.BouquetModel.DiscardBouquetParts(DateConst.May9th, Tuple.Create("BA001", 100));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            var lot0506 = InitialLots["BA001"][DateConst.May6th][0].LotNo;
            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.May6th, lot0506).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                        .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .ContainsActionType(InventoryActionType.DISCARDED)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void DiscardBeforeExpiryDate()
        {
            Model.BouquetModel.DiscardBouquetParts(DateConst.May7th, Tuple.Create("BA001", 20));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            var lot0506 = InitialLots["BA001"][DateConst.May6th][0].LotNo;
            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.May6th, lot0506).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 80).Discarded(20)
                        .ContainsActionType(InventoryActionType.DISCARDED)
                    .At(DateConst.May8th).Used(0, 80)
                    .At(DateConst.May9th).Used(0, 80).Discarded(80)
                        .ContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .NotContainsActionType(InventoryActionType.DISCARDED)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void DiscardMoreThanExpectedQuantity()
        {
            Model.BouquetModel.DiscardBouquetParts(DateConst.May9th, Tuple.Create("BA001", 120));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            var lot0506 = InitialLots["BA001"][DateConst.May6th][0].LotNo;
            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.May6th, lot0506).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100).Shortage(20)
                        .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .ContainsActionType(InventoryActionType.DISCARDED)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void DiscardFromTwoLot()
        {
            Model.BouquetModel.DiscardBouquetParts(DateConst.May6th, Tuple.Create("BA001", 150));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            var lot0503 = InitialLots["BA001"][DateConst.May3rd][0].LotNo;
            var lot0506 = InitialLots["BA001"][DateConst.May6th][0].LotNo;
            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.May3rd, lot0503).BEGIN
                    .At(DateConst.May6th).Used(40, 90).Discarded(90)
                    .END
                .Lot(DateConst.May6th, lot0506).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(0, 40).Discarded(60)
                    .At(DateConst.May9th).Used(0, 40).Discarded(40)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        /// <summary>
        /// 4/30 に BA001を100本破棄→ 5/5 に在庫不足発生
        /// </summary>
        [TestMethod,TestCategory("RED")]
        public void InentoryShortageCausedToDiscard()
        {
            Model.BouquetModel.DiscardBouquetParts(DateConst.April30th, Tuple.Create("BA001", 150));
            Model.BouquetModel.DiscardBouquetParts(DateConst.May1st, Tuple.Create("BA001", 50));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            var lot0430 = InitialLots["BA001"][DateConst.April30th][0].LotNo;
            var lot0501 = InitialLots["BA001"][DateConst.May1st][0].LotNo;
            var lot0502 = InitialLots["BA001"][DateConst.May2nd][0].LotNo;
            var lot0503 = InitialLots["BA001"][DateConst.May3rd][0].LotNo;
            var lot0506 = InitialLots["BA001"][DateConst.May6th][0].LotNo;
            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.April30th, lot0430).BEGIN
                    .At(DateConst.April30th).Arrived(200).Used(20, 30).Discarded(150)
                    .At(DateConst.May1st).Used(30, 0)
                    .At(DateConst.May2nd).Used(0, 0)
                    .At(DateConst.May3rd).Used(0, 0).Discarded(0)
                    .END
                .Lot(DateConst.May1st, lot0501).BEGIN
                    .At(DateConst.May1st).Arrived(300).Used(20, 230).Discarded(50)
                    .At(DateConst.May2nd).Used(80, 150)
                    .At(DateConst.May3rd).Used(20, 130)
                    .At(DateConst.May4th).Used(130, 0).Discarded(0)
                    .END
                .Lot(DateConst.May2nd, lot0502).BEGIN
                    .At(DateConst.May2nd).Arrived(200).Used(0, 200)
                    .At(DateConst.May3rd).Used(0, 200)
                    .At(DateConst.May4th).Used(200, 0)
                    .At(DateConst.May5th).Used(0, 0).Discarded(0)
                    .END
                .Lot(DateConst.May3rd, lot0503).BEGIN
                    .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                    .At(DateConst.May4th).Used(70, 130)
                    .At(DateConst.May5th).Used(130, 0).Shortage(40)
                    .At(DateConst.May6th).Used(0, 0)
                    .END
                .Lot(DateConst.May6th, lot0506).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(40, 60)
                    .At(DateConst.May9th).Used(0, 60).Discarded(60)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }
    }
}
