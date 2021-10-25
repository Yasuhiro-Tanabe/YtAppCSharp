using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelTest
{
    /// <summary>
    /// 商品の入荷に関連するテスト
    /// </summary>
    [TestClass]
    public class PartsArrivalTest : MemorieDeFleursModelTestBase
    {
        #region テスト用データ
        private IDictionary<string, BouquetPart> BouquetParts { get; } = new SortedDictionary<string, BouquetPart>();
        private IDictionary<string, Bouquet> Bouquets { get; } = new SortedDictionary<string, Bouquet>();

        private IDictionary<int, Customer> Customers { get; } = new SortedDictionary<int, Customer>();

        private IDictionary<int, Supplier> Suppliers { get; } = new SortedDictionary<int, Supplier>();

        private IDictionary<DateTime, IList<string>> InitialOrdersToSupplyer { get; } = new SortedDictionary<DateTime, IList<string>>();

        private IDictionary<string, TestOrder> InitialLots { get; } = new SortedDictionary<string, TestOrder>();
        #endregion // テスト用データ

        public PartsArrivalTest() :base()
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
                GetInitialOrdersFromSupplier(context);
            }
        }

        private void PrepareParts(MemorieDeFleursDbContext context)
        {
            Action<BouquetPart> add = part => BouquetParts.Add(part.Code, part);

            add(Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(10)
                .ExpiryDateIs(3)
                .Create(context));

            add(Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA002")
                .PartNameIs("薔薇(白)")
                .LeadTimeIs(1)
                .QauntityParLotIs(10)
                .ExpiryDateIs(3)
                .Create(context));

            add(Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA003")
                .PartNameIs("薔薇(ピンク)")
                .LeadTimeIs(1)
                .QauntityParLotIs(10)
                .ExpiryDateIs(3)
                .Create(context));

            add(Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("GP001")
                .PartNameIs("かすみ草")
                .LeadTimeIs(2)
                .QauntityParLotIs(5)
                .ExpiryDateIs(2)
                .Create(context));

            add(Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN001")
                .PartNameIs("カーネーション(赤)")
                .LeadTimeIs(3)
                .QauntityParLotIs(20)
                .ExpiryDateIs(5)
                .Create(context));

            add(Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN002")
                .PartNameIs("カーネーション(ピンク)")
                .LeadTimeIs(3)
                .QauntityParLotIs(20)
                .ExpiryDateIs(5)
                .Create(context));
        }

        private void PrepareBouquets(MemorieDeFleursDbContext context)
        {
            Action<Bouquet> add = b => Bouquets.Add(b.Code, b);

            add(Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses("BA001", 4)
                .Create(context));

            add(Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT002")
                .NameIs("花束-Bセット")
                .Uses("BA001", 3)
                .Uses("BA002", 5)
                .Uses("BA003", 3)
                .Uses("GP001", 6)
                .Create(context));

            add(Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT003")
                .NameIs("一輪挿し-A")
                .Uses("BA003", 1)
                .Create(context));

            add(Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT004")
                .NameIs("結婚式用ブーケ")
                .Uses("BA002", 3)
                .Uses("BA003", 5)
                .Uses("GP001", 3)
                .Uses("CN002", 3)
                .Create(context));

            add(Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT005")
                .NameIs("母の日感謝セット")
                .Uses("CN001", 6)
                .Uses("CN002", 6)
                .Create(context));

            add(Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT006")
                .NameIs("花束-Cセット")
                .Uses("BA001", 6)
                .Uses("BA002", 4)
                .Uses("BA003", 3)
                .Uses("GP001", 3)
                .Uses("CN001", 2)
                .Uses("CN002", 5)
                .Create(context));
        }

        private void PrepareCustomers(MemorieDeFleursDbContext context)
        {
            Action<Customer> add = c => Customers.Add(c.ID, c);

            add(Model.CustomerModel.GetCustomerBuilder()
                .NameIs("蘇我幸恵")
                .EmailAddressIs("ysoga@localdomain")
                .CardNoIs("9876543210123210")
                .SendTo("ピアノ生徒1", "東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                .SendTo("ピアノ生徒2", "茨城県水戸市城南2-1-20", "井門水戸ビル5階")
                .SendTo("友人A", "静岡県浜松市中区板屋町111-2", "浜松アクトタワー10階")
                .SendTo("ピアノ生徒3", "愛知県名古屋市西区上名古屋3-25-28", "第7猪村ビル4階")
                .SendTo("ピアノ生徒4", "大阪府吹田市垂水町3-9-10", "白川ビル3階")
                .SendTo("友人B", "福岡県福岡市博多区博多駅南2-8-16", "東洋マンション駅南スターオフィス2階")
                .Create(context));

            add(Model.CustomerModel.GetCustomerBuilder()
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
                .Create(context));

            add(Model.CustomerModel.GetCustomerBuilder()
                .NameIs("ユーザ3")
                .EmailAddressIs("user3@localdomain")
                .CardNoIs("1234567890123450")
                .SendTo("友人V", "住所3-1-1")
                .SendTo("友人W", "住所3-1-2")
                .SendTo("友人X", "住所3-1-3")
                .SendTo("友人Y", "住所3-1-4")
                .SendTo("友人Z", "住所3-1-5")
                .SendTo("親戚1", "住所3-1-6")
                .Create(context));
        }

        private void PrepareSuppliers(MemorieDeFleursDbContext context)
        {
            Action<Supplier> add = s => Suppliers.Add(s.Code, s);

            add(Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .EmailIs("shinbashi@localdomain")
                .PhoneNumberIs("00012345678")
                .FaxNumberIs("00012345677")
                .SupplyParts("BA001", "BA002", "BA003", "GP001")
                .Create(context));

            add(Model.SupplierModel.GetSupplierBuilder()
                .NameIs("木挽町花壇")
                .AddressIs("東京都中央区銀座四丁目121-5")
                .EmailIs("kobiki@localdomain")
                .PhoneNumberIs("09098765432")
                .FaxNumberIs("0120000000")
                .SupplyParts("GP001", "CN001", "CN002")
                .Create(context));
        }

        private void PrepareInitialOrderToSupplier(MemorieDeFleursDbContext context)
        {
            var orderDate = new DateTime(DateConst.Year, 3, 10);

            Model.SupplierModel.Order(context, orderDate, Suppliers[1], DateConst.April30th,
                new[] {
                            Tuple.Create(BouquetParts["BA001"], 2),
                            Tuple.Create(BouquetParts["BA002"], 1),
                            Tuple.Create(BouquetParts["BA003"], 1),
                            Tuple.Create(BouquetParts["GP001"], 1),
                });

            Model.SupplierModel.Order(context, orderDate, Suppliers[2], DateConst.April30th,
                new[] {
                    Tuple.Create(BouquetParts["GP001"], 2),
                    Tuple.Create(BouquetParts["CN001"], 2),
                    Tuple.Create(BouquetParts["CN002"], 2),
                });
        }

        private void PrepareOrdersFromCustomer(MemorieDeFleursDbContext context)
        {
            Model.CustomerModel.Order(context, DateConst.April30th, Bouquets["HT001"], Customers[1].ShippingAddresses[1], DateConst.May3rd);
            Model.CustomerModel.Order(context, DateConst.April30th, Bouquets["HT001"], Customers[1].ShippingAddresses[2], DateConst.May3rd);
            Model.CustomerModel.Order(context, DateConst.April30th, Bouquets["HT001"], Customers[1].ShippingAddresses[3], DateConst.May3rd);
            Model.CustomerModel.Order(context, DateConst.April30th, Bouquets["HT001"], Customers[2].ShippingAddresses[1], DateConst.May4th);
            Model.CustomerModel.Order(context, DateConst.April30th, Bouquets["HT001"], Customers[1].ShippingAddresses[4], DateConst.May4th);
        }

        private void GetInitialLotNumbers(MemorieDeFleursDbContext context)
        {
            LogUtil.DEBUGLOG_BeginMethod();
            var arrivedActions = context.InventoryActions
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE)
                .OrderBy(act => act.PartsCode)
                .ThenBy(act => act.ArrivalDate)
                .ToList();

            foreach(var grp in arrivedActions
                .GroupBy(act => act.PartsCode))
            {
                var order = new TestOrder();
                foreach(var act in grp)
                {
                    order.Append(act.ArrivalDate, act.InventoryLotNo, act.Quantity);
                }
                InitialLots.Add(grp.Key, order);
                LogUtil.DebugWithoutLineNumber($"{grp.Key}=[{order.ToString()}]");
            }
            LogUtil.DEBUGLOG_EndMethod();
        }

        private void GetInitialOrdersFromSupplier(MemorieDeFleursDbContext context)
        {
            LogUtil.DEBUGLOG_BeginMethod();

            foreach(var grp in context.OrdersToSuppliers.ToList()
                .OrderBy(order => order.DeliveryDate)
                .GroupBy(o => o.DeliveryDate))
            {
                var list = new List<string>();
                
                foreach(var order in grp)
                {
                    list.Add(order.ID);
                }

                InitialOrdersToSupplyer.Add(grp.Key, list.ToList());
                LogUtil.DebugWithoutLineNumber($"{grp.Key:yyyyMMdd}: {string.Join(", ", list)}");
            }

            LogUtil.DEBUGLOG_EndMethod();
        }
        #endregion // TestInitialize

        [TestMethod]
        public void OrdersQuantityIsChanged_OrderPartsAreNotUsed()
        {
            var order = InitialOrdersToSupplyer[DateConst.April30th][0];
            var part = BouquetParts["GP001"];
            Model.SupplierModel.ChangeArrivedQuantities(order, new[] { Tuple.Create(part, 8) });

            InventoryActionValidator.NewInstance().BouquetPartIs(part).BEGIN
                .Lot(DateConst.April30th).BEGIN
                    .At(DateConst.April30th).Arrived(8)
                    .At(DateConst.May2nd).Discarded(8)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void OrdersQuantityIsChagned_OrderPartsAreUsed()
        {
            var order = InitialOrdersToSupplyer[DateConst.April30th][0];
            var ba001 = BouquetParts["BA001"];
            var ba002 = BouquetParts["BA002"];

            Model.SupplierModel.ChangeArrivedQuantities(order, new[] { Tuple.Create(ba001, 18), Tuple.Create(ba002, 9) });

            InventoryActionValidator.NewInstance().BouquetPartIs(ba001).BEGIN
                .Lot(DateConst.April30th).BEGIN
                    .At(DateConst.April30th).Arrived(18)
                    .At(DateConst.May2nd).Used(12, 6)
                    .At(DateConst.May3rd).Used(6,0).Shortage(2).Discarded(0)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }
    }
}
