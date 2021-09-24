using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class AddAndRemoveOrdersFromCustomerTest : MemorieDeFleursTestBase
    {
        /// <summary>
        /// テストで使用する商品
        /// </summary>
        private Bouquet ExpectedBouquet { get; set; }

        /// <summary>
        /// 在庫不足検証用の、単品を大量消費する商品
        /// </summary>
        private Bouquet ExpectedBigBouquet { get; set; }

        /// <summary>
        /// テストで使用する単品
        /// </summary>
        private BouquetPart ExpectedPart { get; set; }

        /// <summary>
        /// テストで使用する得意先
        /// </summary>
        private Customer ExpectedCustomer { get; set; }

        /// <summary>
        /// テストで使用するお届け先
        /// </summary>
        private ShippingAddress ExpectedShippingAddress { get; set; }

        /// <summary>
        /// 検証対象モデル
        /// </summary>
        private MemorieDeFleursModel Model { get; set; }

        /// <summary>
        /// 在庫一覧：在庫ロット毎の、ロット番号と入荷(予定)数量。在庫アクションの検証に使用
        /// </summary>
        private TestOrder InitialOrders { get; } = new TestOrder();

        /// <summary>
        /// 在庫一覧：日々の在庫数に関する、テスト前の初期値。受注による在庫増減の期待値を計算するために使用
        /// </summary>
        private IDictionary<DateTime, int> InitialInventories { get; } = new Dictionary<DateTime, int>();

        #region InventoryCalcurator
        private class InventoryCalcurator
        {
            public class Base
            {
                protected SqliteConnection DbConnection { get; set; }

                protected MemorieDeFleursDbContext DbContext { get; set; }
                protected string PartCode { get; set; }
                public Base(MemorieDeFleursDbContext context, BouquetPart part)
                {
                    DbContext = context;
                    DbConnection = null;
                    PartCode = part.Code;
                }
                public Base(SqliteConnection connection, BouquetPart part)
                {
                    DbContext = null;
                    DbConnection = connection;
                    PartCode = part.Code;
                }
                protected IQueryable<InventoryAction> FindInventoryActionByDate(DateTime d)
                {
                    if (DbContext == null)
                    {
                        using (var context = new MemorieDeFleursDbContext(DbConnection))
                        {
                            // 一回DBとの接続が切れるので、コピーを生成してから返す
                            return FindInventoryActionByDate(context, d).ToList().AsQueryable();
                        }
                    }
                    else
                    {
                        // 同一トランザクション内からの呼出を想定しておりDBとの接続は切れない。そのまま返す。
                        return FindInventoryActionByDate(DbContext, d);
                    }
                }

                private IQueryable<InventoryAction> FindInventoryActionByDate(MemorieDeFleursDbContext context, DateTime d)
                {
                    return context.InventoryActions
                        .Where(act => act.PartsCode == PartCode)
                        .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                        .Where(act => act.ActionDate == d);
                }
            }

            public class QuantityCalculator : Base
            {
                public QuantityCalculator(SqliteConnection connection, BouquetPart part) : base(connection, part) { }
                public QuantityCalculator(MemorieDeFleursDbContext context, BouquetPart part) : base(context, part) { }

                public int this[DateTime d] {
                    get
                    { return FindInventoryActionByDate(d)
                            .Sum(act => act.Quantity);
                    }
                }

                public int this[DateTime d, int lotNo] {
                    get
                    {
                        return FindInventoryActionByDate(d)
                            .Where(act => act.InventoryLotNo == lotNo)
                            .Single()
                            .Quantity;
                    }
                }
            }

            public class RemainCalculator : Base
            {
                public RemainCalculator(SqliteConnection connection, BouquetPart part) : base(connection, part) { }
                public RemainCalculator(MemorieDeFleursDbContext context, BouquetPart part) : base(context, part) { }

                public int this[DateTime d] 
                {
                    get
                    {
                        return FindInventoryActionByDate(d)
                            .Sum(act => act.Remain);
                    }
                }

                public int this[DateTime d, int lotNo]
                {
                    get
                    {
                        return FindInventoryActionByDate(d)
                            .Where(act => act.InventoryLotNo == lotNo)
                            .Single().Remain;
                    }
                }
            }
            public QuantityCalculator Quantity { get; private set; }
            public RemainCalculator Remain { get; private set; }

            public InventoryCalcurator(SqliteConnection connection, BouquetPart part)
            {
                Quantity = new QuantityCalculator(connection, part);
                Remain = new RemainCalculator(connection, part);
            }

            public InventoryCalcurator(MemorieDeFleursDbContext context, BouquetPart part)
            {
                Quantity = new QuantityCalculator(context, part);
                Remain = new RemainCalculator(context, part);
            }

        }
        #endregion // InventoryCalcurator



        public AddAndRemoveOrdersFromCustomerTest()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;
        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDB);

            PrepareBouquet();
            PrepeareCustomer();
            PrepareInitialOrders();
            PrepareInitialUsed();

        }


        private void PrepareInitialOrders()
        {
            var orders = new
            {
                OrderDate = new DateTime(2020, 4, 25),
                OrderBody = new List<Tuple<DateTime, int>>() {
                    Tuple.Create(DateConst.April30th, 2),
                    Tuple.Create(DateConst.May1st, 3),
                    Tuple.Create(DateConst.May2nd, 2),
                    Tuple.Create(DateConst.May3rd, 2),
                    Tuple.Create(DateConst.May6th, 1)
                }
            };
            foreach (var o in orders.OrderBody)
            {
                var lotNo = Model.SupplierModel.Order(orders.OrderDate, ExpectedPart, o.Item2, o.Item1);
                InitialOrders.Append(o.Item1, lotNo, o.Item2 * ExpectedPart.QuantitiesPerLot);
            }
        }

        private void PrepareInitialUsed()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var used = new List<Tuple<DateTime, int>>()
                {
                    Tuple.Create(DateConst.April30th, 20),
                    Tuple.Create(DateConst.May1st, 50),
                    Tuple.Create(DateConst.May2nd, 80),
                    Tuple.Create(DateConst.May3rd, 20),
                    Tuple.Create(DateConst.May4th, 400),
                    Tuple.Create(DateConst.May5th, 170),
                    Tuple.Create(DateConst.May6th, 40)
                };
                foreach (var u in used)
                {
                    Model.BouquetModel.UseBouquetPart(context, ExpectedPart, u.Item1, u.Item2);
                }

                foreach (var d in Enumerable.Range(0, 10).Select(i => DateConst.April30th.AddDays(i)))
                {
                    InitialInventories.Add(d, context.InventoryActions
                        .Where(act => act.PartsCode == ExpectedPart.Code)
                        .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE)
                        .Where(act => act.ActionDate == d)
                        .Sum(act => act.Remain));
                }
            }
        }

        private void PrepeareCustomer()
        {
            ExpectedCustomer = Model.CustomerModel.GetCustomerBuilder()
                .EmailAddressIs("ysoga@localdomain")
                .NameIs("蘇我幸恵")
                .PasswordIs("sogayukie12345")
                .CardNoIs("9876543210123210")
                .Create();

            ExpectedShippingAddress = Model.CustomerModel.GetShippingAddressBuilder()
                .From(ExpectedCustomer)
                .To("ピアノ生徒1")
                .AddressIs("東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                .Create();
        }

        private void PrepareBouquet()
        {
            ExpectedPart = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();

            ExpectedBouquet = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses(ExpectedPart, 4)
                .Create();

            ExpectedBigBouquet = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HUGE0")
                .NameIs("検証用の巨大ブーケ")
                .Uses(ExpectedPart, 150)
                .Create();
        }
        #endregion // TestInitialize

        #region TestCleanup
        private void CleanupModel(object sender, EventArgs unused)
        {
            ClearAll();
        }
        #endregion // TestCleanup

        /// <summary>
        /// 引当可能：既存在庫への影響なし
        /// </summary>
        [TestMethod]
        public void OneOrderUpdatesCurrentInventory()
        {
            LogUtil.DEBUGLOG_BeginMethod(msg: "===== TEST BEGIN =====");
            var caclurator = new InventoryCalcurator(TestDB, ExpectedPart);
            var lot = InitialOrders[DateConst.April30th][0].LotNo;

            // Order() 後に在庫数が変わるので期待値をあらかじめ計算する
            var May2nd = DateConst.May2nd;
            var used = ExpectedBouquet.PartsList.Single(p => p.PartsCode == ExpectedPart.Code).Quantity;
            var expectedMay2Quantity = caclurator.Quantity[May2nd, lot] + used;
            var expectedMay2Remain = caclurator.Remain[May2nd, lot] - used;

            // お届け日は在庫消費日の翌日：AddDays()しているのはテスト目的が在庫の増減確認であり在庫引当日を基準にしているため。普通は 5/3 を直接指定する。
            var orderNo = Model.CustomerModel.Order(DateConst.May1st, ExpectedBouquet, ExpectedShippingAddress, May2nd.AddDays(1));

            // 受注番号が正しく生成されている
            Assert.IsNotNull(orderNo);
            Assert.AreEqual($"{DateConst.May1st.ToString("yyyyMMdd")}-000001", orderNo);

            // 受注情報が適切に登録されている
            var actualOrder = Model.CustomerModel.FindOrder(orderNo);
            var expectedOrder = new OrderFromCustomer()
            {
                ID = orderNo,
                CustomerID = ExpectedCustomer.ID,
                ShippingAddressID = ExpectedShippingAddress.ID,
                BouquetCode = ExpectedBouquet.Code,
                OrderDate = DateConst.May1st,
                ShippingDate = DateConst.May2nd
            };
            AssertOrder(expectedOrder, actualOrder);

            // 受注結果通りに在庫が減っている
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.April30th, lot).Begin()
                    .At(May2nd).Used(expectedMay2Quantity, expectedMay2Remain)
                    .End()
                .End()
                .InventoryActionCountShallBe(InventoryActionType.SHORTAGE, 0)
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndMethod(msg: "===== TEST END =====");
        }

        private void AssertOrder(OrderFromCustomer expected, OrderFromCustomer actual)
        {
            var orderString = $"受注番号={expected.ID}, 受注日={expected.OrderDate.ToString("yyyyMMdd")}, 商品={expected.BouquetCode}," +
                $" 発送日={expected.ShippingDate.ToString("yyyyMMdd")}, 得意先={expected.CustomerID}, お届け先={expected.ShippingAddressID}";

            Assert.IsNotNull(actual, $"DBに登録されていない: {orderString}");
            Assert.AreEqual(expected.ID, actual.ID, $"受注番号不正: {orderString}");
            Assert.AreEqual(expected.OrderDate, actual.OrderDate, $"受注日不一致: {orderString}");
            Assert.AreEqual(expected.ShippingDate, actual.ShippingDate, $"発送日不一致: {orderString}");

            Assert.AreEqual(expected.BouquetCode, actual.BouquetCode, $"花束コードが不一致: {orderString}");
            Assert.IsNotNull(actual.Bouquet, $"Bouquetが取 得できていない: {orderString}");
            Assert.AreEqual(expected.BouquetCode, actual.Bouquet.Code, $"取得したBouquetの花束コードが不一致");

            Assert.AreEqual(expected.CustomerID, actual.CustomerID, $"得意先IDが不一致: {orderString}");
            Assert.IsNotNull(actual.Customer, $"Customer が取得できていない: {orderString}");
            Assert.AreEqual(expected.CustomerID, actual.Customer.ID, $"取得した Customer のIDが不一致: {orderString}");

            Assert.AreEqual(expected.ShippingAddressID, actual.ShippingAddressID, $"お届け先IDが不一致: {orderString}");
            Assert.IsNotNull(actual.ShippingAddress, $"ShippingAddress が取得できていない: {orderString}");
            Assert.AreEqual(expected.ShippingAddressID, actual.ShippingAddress.ID, $"取得した ShippingAddress のIDが不一致: {orderString}");
        }

        [TestMethod]
        public void CanRollbackCurrentOrder()
        {
            LogUtil.DEBUGLOG_BeginMethod(msg: "===== TEST BEGIN =====");
            var lot = InitialOrders[DateConst.May6th][0].LotNo;
            var calcurator = new InventoryCalcurator(TestDB, ExpectedPart);
            var expectedRemain = calcurator.Remain[DateConst.May8th, lot]; // お届け日前日(=発送日)の当日残：注文がロールバックされるので当日残に増減はないはず

            using (var context = new MemorieDeFleursDbContext(TestDB))
            using(var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Model.CustomerModel.Order(context, DateConst.April30th, ExpectedBigBouquet, ExpectedShippingAddress, DateConst.May9th);
                    transaction.Commit();
                    Assert.Fail($"想定外の成功：在庫が足りないので {ExpectedBigBouquet.Code} の引当ができずに例外をスローするはず");
                }
                catch (NotImplementedException)
                {
                    transaction.Rollback();
                }
            }

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May6th, lot).Begin()
                    .At(DateConst.May8th).Used(0, expectedRemain)
                    .At(DateConst.May9th).Used(0, expectedRemain).Discarded(expectedRemain)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndMethod(msg: "===== TEST END =====");
        }

        [TestMethod]
        public void CancelOrder_CanCommit()
        {
            LogUtil.DEBUGLOG_BeginMethod(msg: "===== TEST BEGIN =====");
            var lot0503 = InitialOrders[DateConst.May3rd][0].LotNo;
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;

            // このテストでOrder() を呼ぶ前の状態：
            //     - 5/3ロットの5/6在庫数40，残数90、当日破棄
            //     - 5/6ロットの初期数量100、未使用のまま
            var order = Model.CustomerModel.Order(DateConst.May1st, ExpectedBouquet, ExpectedShippingAddress, DateConst.May6th.AddDays(1));

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May6th).Used(44, 86).Discarded(86)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            Model.CustomerModel.CancelOrder(order);
            LogUtil.Debug($"{LogUtil.Indent}After ordered...");
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May6th).Used(40, 90).Discarded(90)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndMethod(msg: "===== TEST END =====");
        }

        [TestMethod]
        public void CancelOrder_CanRollback()
        {
            LogUtil.DEBUGLOG_BeginMethod(msg: "===== TEST BEGIN =====");
            var lot0503 = InitialOrders[DateConst.May3rd][0].LotNo;
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;

            // このテストでOrder() を呼ぶ前の状態：
            //     - 5/3ロットの5/6在庫数40，残数90、当日破棄
            //     - 5/6ロットの初期数量100、未使用のまま
            var order = Model.CustomerModel.Order(DateConst.May1st, ExpectedBouquet, ExpectedShippingAddress, DateConst.May6th.AddDays(1));

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May6th).Used(44, 86).Discarded(86)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                Model.CustomerModel.CancelOrder(context, order);
                transaction.Rollback();
            }
            LogUtil.Debug($"{LogUtil.Indent}After ordered...");

            // CancelOrder() はロールバックされたので、在庫は Order() 実施後の状態と変わらないはず
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May6th).Used(44, 86).Discarded(86)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndMethod(msg: "===== TEST END =====");
        }

        /// <summary>
        /// 同一在庫ロット内でのお届け日変更：5/8→5/10 (発送日5/7→5/9)
        /// </summary>
        [TestMethod]
        public void ChangeArrivalDate_FromMay8thToMay10th_InventoryChangedInsideLot0506Only()
        {
            LogUtil.DEBUGLOG_BeginMethod(msg: "===== TEST BEGIN =====");
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;

            // このテストでOrder() を呼ぶ前の状態：
            //     - 5/6ロットの初期数量100、未使用
            var order = Model.CustomerModel.Order(DateConst.May1st, ExpectedBouquet, ExpectedShippingAddress, DateConst.May8th);
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(4, 96)
                    .At(DateConst.May8th).Used(0, 96)
                    .At(DateConst.May9th).Used(0, 96).Discarded(96)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            // 同一ロット内で変更
            var May10th = DateConst.May9th.AddDays(1);
            Model.CustomerModel.ChangeArrivalDate(order, May10th);
            LogUtil.Debug($"{LogUtil.Indent}After ordered...");
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(4, 96).Discarded(96)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndMethod(msg: "===== TEST END =====");
        }

        /// <summary>
        /// 同一在庫ロットをまたがるお届け日変更：5/6→5/9 (発送日5/5→5/8)
        /// </summary>
        [TestMethod]
        public void ChangeArrivalDate_FromMay6thToMay9th_InventoryChangedInsideLot0506Only()
        {
            LogUtil.DEBUGLOG_BeginMethod(msg: "===== TEST BEGIN =====");
            var lot0503 = InitialOrders[DateConst.May3rd][0].LotNo;
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;

            // このテストでOrder() を呼ぶ前の状態：
            //     - 5/3ロットの5/6在庫数40，残数90、当日破棄
            //     - 5/6ロットの初期数量100、未使用のまま
            var order = Model.CustomerModel.Order(DateConst.May1st, ExpectedBouquet, ExpectedShippingAddress, DateConst.May6th);
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May5th).Used(74, 126)
                    .At(DateConst.May6th).Used(40, 86).Discarded(86)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            Model.CustomerModel.ChangeArrivalDate(order, DateConst.May9th);

            LogUtil.Debug($"{LogUtil.Indent}After ordered...");
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May5th).Used(70, 130)
                    .At(DateConst.May6th).Used(40, 90).Discarded(90)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(4, 96)
                    .At(DateConst.May9th).Used(0, 96).Discarded(96)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndMethod(msg: "===== TEST END =====");
        }

        [TestMethod]
        public void ChangeArrivalDate_CanRollback()
        {
            LogUtil.DEBUGLOG_BeginMethod(msg: "===== TEST BEGIN =====");
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;

            // このテストでOrder() を呼ぶ前の状態：
            //     - 5/6ロットの初期数量100、未使用
            var order = Model.CustomerModel.Order(DateConst.May1st, ExpectedBouquet, ExpectedShippingAddress, DateConst.May8th);
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(4, 96)
                    .At(DateConst.May8th).Used(0, 96)
                    .At(DateConst.May9th).Used(0, 96).Discarded(96)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            using(var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                Model.CustomerModel.ChangeArrivalDate(context, order, DateConst.May9th);
                transaction.Rollback();
            }

            // ロールバックしたので在庫推移は日付変更前の状態を保っているはず
            LogUtil.Debug($"{LogUtil.Indent}After ordered...");
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(4, 96)
                    .At(DateConst.May8th).Used(0, 96)
                    .At(DateConst.May9th).Used(0, 96).Discarded(96)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndMethod(msg: "===== TEST END =====");
        }
    }
}
