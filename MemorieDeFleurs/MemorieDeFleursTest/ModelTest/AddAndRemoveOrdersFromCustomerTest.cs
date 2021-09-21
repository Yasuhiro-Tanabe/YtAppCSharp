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
        private IDictionary<DateTime, int> InitialStocks { get; } = new Dictionary<DateTime, int>();

        #region CurrentStock
        private class StockCalcurator
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
                protected IQueryable<StockAction> FindStockActionByDate(DateTime d)
                {
                    if (DbContext == null)
                    {
                        using (var context = new MemorieDeFleursDbContext(DbConnection))
                        {
                            // 一回DBとの接続が切れるので、コピーを生成してから返す
                            return FindStockActionByDate(context, d).ToList().AsQueryable();
                        }
                    }
                    else
                    {
                        // 同一トランザクション内からの呼出を想定しておりDBとの接続は切れない。そのまま返す。
                        return FindStockActionByDate(DbContext, d);
                    }
                }

                private IQueryable<StockAction> FindStockActionByDate(MemorieDeFleursDbContext context, DateTime d)
                {
                    return context.StockActions
                        .Where(act => act.PartsCode == PartCode)
                        .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
                        .Where(act => act.ActionDate == d);
                }
            }

            public class QuantityCalculator : Base
            {
                public QuantityCalculator(SqliteConnection connection, BouquetPart part) : base(connection, part) { }
                public QuantityCalculator(MemorieDeFleursDbContext context, BouquetPart part) : base(context, part) { }

                public int this[DateTime d] {
                    get
                    { return FindStockActionByDate(d)
                            .Sum(act => act.Quantity);
                    }
                }

                public int this[DateTime d, int lotNo] {
                    get
                    {
                        return FindStockActionByDate(d)
                            .Where(act => act.StockLotNo == lotNo)
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
                        return FindStockActionByDate(d)
                            .Sum(act => act.Remain);
                    }
                }

                public int this[DateTime d, int lotNo]
                {
                    get
                    {
                        return FindStockActionByDate(d)
                            .Where(act => act.StockLotNo == lotNo)
                            .Single().Remain;
                    }
                }
            }
            public QuantityCalculator Quantity { get; private set; }
            public RemainCalculator Remain { get; private set; }

            public StockCalcurator(SqliteConnection connection, BouquetPart part)
            {
                Quantity = new QuantityCalculator(connection, part);
                Remain = new RemainCalculator(connection, part);
            }

            public StockCalcurator(MemorieDeFleursDbContext context, BouquetPart part)
            {
                Quantity = new QuantityCalculator(context, part);
                Remain = new RemainCalculator(context, part);
            }

        }
        #endregion // CurrentStock



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
                    InitialStocks.Add(d, context.StockActions
                        .Where(act => act.PartsCode == ExpectedPart.Code)
                        .Where(act => act.Action == StockActionType.SCHEDULED_TO_USE)
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
        public void OneOrderUpdatesCurrentStock()
        {
            LogUtil.Debug($"===== {nameof(OneOrderUpdatesCurrentStock)} [Begin] =====");
            var CurrentStock = new StockCalcurator(TestDB, ExpectedPart);
            var lot = InitialOrders[DateConst.April30th][0].LotNo;

            // Order() 後に CurrentStock の値が変わるので期待値をあらかじめ計算する
            var May2nd = DateConst.May2nd;
            var used = ExpectedBouquet.PartsList.Single(p => p.PartsCode == ExpectedPart.Code).Quantity;
            var expectedMay2Quantity = CurrentStock.Quantity[May2nd, lot] + used;
            var expectedMay2Remain = CurrentStock.Remain[May2nd, lot] - used;

            // お届け日は在庫消費日の翌日
            var orderNo = Model.CustomerModel.Order(DateConst.May1st, ExpectedBouquet, ExpectedShippingAddress, May2nd.AddDays(1));

            // 受注番号が正しく生成されている
            Assert.IsNotNull(orderNo);
            Assert.AreEqual($"{DateConst.May1st.ToString("yyyyMMdd")}-000001", orderNo);

            // 受注結果通りに在庫が減っている
            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.April30th, lot).Begin()
                    .At(May2nd).Used(expectedMay2Quantity, expectedMay2Remain)
                    .End()
                .End()
                .StockActionCountShallBe(StockActionType.OUT_OF_STOCK, 0)
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.Debug($"===== {nameof(OneOrderUpdatesCurrentStock)} [End] =====");
        }

        [TestMethod]
        public void CanRollbackCurrentOrder()
        {
            LogUtil.DEBUGLOG_BeginMethod(msg: "===== TEST BEGIN =====");
            var lot = InitialOrders[DateConst.May6th][0].LotNo;
            var currentStock = new StockCalcurator(TestDB, ExpectedPart);
            var expectedRemain = currentStock.Remain[DateConst.May8th, lot]; // お届け日前日(=発送日)の当日残：注文がロールバックされるので当日残に増減はないはず

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

            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May6th, lot).Begin()
                    .At(DateConst.May8th).Used(0, expectedRemain)
                    .At(DateConst.May9th).Used(0, expectedRemain).Discarded(expectedRemain)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndMethod(msg: "===== TEST END =====");
        }
    }
}
