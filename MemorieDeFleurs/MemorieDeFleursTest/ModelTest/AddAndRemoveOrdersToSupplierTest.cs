using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

using MemorieDeFleursTest.ModelTest.Fluent;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Databese.SQLite;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    /// <summary>
    /// 追加発注および発注取消に関するテスト
    /// </summary>
    [TestClass]
    public class AddAndRemoveOrdersToSupplierTest : MemorieDeFleursTestBase
    {
        /// <summary>
        /// テストで使用する仕入先
        /// </summary>
        private Supplier ExpectedSupplier { get; set; }

        /// <summary>
        /// テストで使用する単品
        /// </summary>
        private BouquetPart ExpectedPart { get; set; }

        /// <summary>
        /// 検証対象モデル
        /// </summary>
        private MemorieDeFleursModel Model { get; set; }

        /// <summary>
        /// 各ロット毎のロット番号と入荷(予定)数量
        /// </summary>
        private TestOrder InitialOrders { get; } = new TestOrder();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AddAndRemoveOrdersToSupplierTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;

        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDB);

            PrepareSuppliers();
            PrepareBouquetParts();
            PrepareInitialOrders();
            PrepareInitialUsed();
        }

        private void PrepareSuppliers()
        {
            ExpectedSupplier = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .Create();
        }

        private void PrepareBouquetParts()
        {
            ExpectedPart = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();
        }

        private void PrepareInitialUsed()
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
                Model.BouquetModel.UseBouquetPart(ExpectedPart, u.Item1, u.Item2);
            }
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
        #endregion // TestInitialize

        #region TestCleanup
        private void CleanupModel(object sender, EventArgs unused)
        {
            ClearAll();
        }
        #endregion // TesetCleanup


        /// <summary>
        /// 5/4納品予定分追加発注の検証：当日以降の加工予定と入荷予定に影響を与えないこと
        /// </summary>
        [TestMethod]
        public void NewOrderWhichArrivedAt20200504()
        {
            LogUtil.DEBUGLOG_BeginTest();

            var newLotNo = Model.SupplierModel.Order(DateConst.April30th, ExpectedPart, 1, DateConst.May4th);

            var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May1st, findLotNo).Begin()
                    .At(DateConst.May4th).Used(300, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May2nd, findLotNo).Begin()
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May3rd, findLotNo).Begin()
                    .At(DateConst.May4th).Used(0, 200)
                    .At(DateConst.May5th).Used(70, 130)
                    .At(DateConst.May6th).Used(40, 90).Discarded(90)
                    .End()
                .Lot(DateConst.May6th, findLotNo).Begin()
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                    .End()
                .Lot(DateConst.May4th, newLotNo).Begin()
                    .At(DateConst.May4th).Arrived(100).Used(0, 100)
                    .At(DateConst.May5th).Used(0, 100)
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        /// <summary>
        /// 5/2納品予定分の追加発注により、5/3納品予定分から払い出していた5/5加工分70本が、
        /// 今回追加発注分からの払い出しに置き換わること
        /// </summary>
        [TestMethod]
        public void NewOrderWhichArrivedAt20200502()
        {
            LogUtil.DEBUGLOG_BeginTest();
            var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

            var newLotNo = Model.SupplierModel.Order(DateConst.April30th, ExpectedPart, 1, DateConst.May2nd);

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.April30th, findLotNo).Begin()
                    .At(DateConst.May2nd).Used(80, 50)
                    .At(DateConst.May3rd).Used(20, 30).Discarded(30)
                    .End()
                .Lot(DateConst.May1st, findLotNo).Begin()
                    .At(DateConst.May2nd).Used(0, 300)
                    .At(DateConst.May3rd).Used(0, 300)
                    .At(DateConst.May4th).Used(300, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May2nd, findLotNo).Begin()
                    .At(DateConst.May2nd).Arrived(200).Used(0, 200)
                    .At(DateConst.May3rd).Used(0, 200)
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May2nd, newLotNo).Begin()
                    .At(DateConst.May2nd).Arrived(100).Used(0, 100)
                    .At(DateConst.May3rd).Used(0, 100)
                    .At(DateConst.May4th).Used(0, 100)
                    .At(DateConst.May5th).Used(70, 30).Discarded(30) // 0本→70本使用
                    .End()
                .Lot(DateConst.May3rd, findLotNo).Begin()
                    .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                    .At(DateConst.May4th).Used(0, 200)
                    .At(DateConst.May5th).Used(0, 200)  // 70本使用→0本
                    .At(DateConst.May6th).Used(40, 160).Discarded(160)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        /// <summary>
        /// 未使用の在庫ロットを破棄しても他の在庫ロットに影響が出ない
        /// </summary>
        [TestMethod]
        public void RemoveOrderArrivedAt20200506()
        {
            var lot = InitialOrders[DateConst.May6th][0].LotNo;
            var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

            Model.SupplierModel.CancelOrder(lot);

            // 他在庫ロットの在庫アクションで、破棄した在庫ロットの入荷日以降の在庫アクションに変化がない
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May6th, lot).HasNoInventoryActions()
                .Lot(DateConst.May3rd, findLotNo).Begin()
                    .At(DateConst.May6th).Used(40, 90).Discarded(90)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        /// <summary>
        /// 在庫ロットの破棄により在庫不足が派生する
        /// </summary>
        [TestMethod]
        public void RemoveOrderArrivedAt20200502()
        {
            LogUtil.DEBUGLOG_BeginTest();

            var lot = InitialOrders[DateConst.May2nd][0].LotNo;
            var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

            Model.SupplierModel.CancelOrder(lot);

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May2nd, lot).HasNoInventoryActions()
                .Lot(DateConst.May3rd, findLotNo).Begin()
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0).Shortage(70)
                    .At(DateConst.May6th).Used(0, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May6th, findLotNo).Begin()
                    .At(DateConst.May6th).Used(40, 60)
                    .At(DateConst.May7th).Used(0, 60)
                    .At(DateConst.May8th).Used(0, 60)
                    .At(DateConst.May9th).Used(0, 60).Discarded(60)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        /// <summary>
        /// 発注取消により生じた在庫不足が、追加発注により解消される
        /// 
        /// 未来入荷分からの振替も、すべて追加発注分で加工される
        /// </summary>
        [TestMethod]
        public void ChangeOrders_RemoveFrom20200502_And_AddTwoLotTo202005005()
        {
            LogUtil.DEBUGLOG_BeginTest();

            var lot0502 = InitialOrders[DateConst.May2nd][0].LotNo;
            var lot0503 = InitialOrders[DateConst.May3rd][0].LotNo;
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;
            var lot0505 = 0;

            LogUtil.Debug($"+++ CancelOrder({lot0502})");
            Model.SupplierModel.CancelOrder(lot0502);

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May2nd, lot0502).HasNoInventoryActions()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0).Shortage(70)
                    .At(DateConst.May6th).Used(0, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(40, 60)
                    .At(DateConst.May7th).Used(0, 60)
                    .At(DateConst.May8th).Used(0, 60)
                    .At(DateConst.May9th).Used(0, 60).Discarded(60)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.Debug($"+++ Order({DateConst.May5th.ToString("yyyyMMdd")})");
            lot0505 = Model.SupplierModel.Order(DateConst.May1st, ExpectedPart, 2, DateConst.May5th);

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May2nd, lot0502).HasNoInventoryActions()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0)
                    .At(DateConst.May6th).Used(0, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May5th, lot0505).Begin()
                    .At(DateConst.May5th).Arrived(200).Used(70, 130)
                    .At(DateConst.May6th).Used(40, 90)
                    .At(DateConst.May7th).Used(0, 90)
                    .At(DateConst.May8th).Used(0, 90).Discarded(90).End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100).End()
                .End()
                .InventoryActionCountShallBe(InventoryActionType.SHORTAGE, 0)
                .TargetDBIs(TestDB)
                .AssertAll();
            LogUtil.DEBUGLOG_EndTest();
        }

        /// <summary>
        /// 在庫不足は解消されるが、未来入荷予定分への振替はゼロにならない
        /// </summary>
        [TestMethod]
        public void ChangeOrders_RemoveFrom20200502_And_AddOneLotTo202005005()
        {
            LogUtil.DEBUGLOG_BeginTest();

            var lot0502 = InitialOrders[DateConst.May2nd][0].LotNo;
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;
            var lot0505 = 0;

            Model.SupplierModel.CancelOrder(lot0502);
            lot0505 = Model.SupplierModel.Order(DateConst.May1st, ExpectedPart, 1, DateConst.May5th);

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May2nd, lot0502).HasNoInventoryActions()
                .Lot(DateConst.May5th, lot0505).Begin()
                    .At(DateConst.May5th).Used(70, 30)
                    .At(DateConst.May6th).Used(30, 0)
                    .At(DateConst.May7th).Used(0, 0)
                    .At(DateConst.May8th).Used(0, 0).Discarded(0).End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Used(10, 90)
                    .At(DateConst.May7th).Used(0, 90)
                    .At(DateConst.May8th).Used(0, 90)
                    .At(DateConst.May9th).Used(0, 90).Discarded(90).End()
                .End()
                .InventoryActionCountShallBe(InventoryActionType.SHORTAGE, 0)
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void ChangeOrders_RemoveAndAddSameDaySameLotCount()
        {
            LogUtil.DEBUGLOG_BeginTest();

            var oldLot0502 = InitialOrders[DateConst.May2nd][0].LotNo;
            var lot0503 = InitialOrders[DateConst.May3rd][0].LotNo;
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;

            Model.SupplierModel.CancelOrder(oldLot0502);
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May2nd, oldLot0502).HasNoInventoryActions()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0).Shortage(70)
                    .At(DateConst.May6th).Used(0, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May6th, InitialOrders[DateConst.May6th][0].LotNo).Begin()
                    .At(DateConst.May6th).Arrived(100).Used(40, 60)
                    .At(DateConst.May7th).Used(0, 60)
                    .At(DateConst.May8th).Used(0, 60)
                    .At(DateConst.May9th).Used(0, 60).Discarded(60)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            var newLot0502 = Model.SupplierModel.Order(DateConst.April30th, ExpectedPart, 2, DateConst.May2nd);

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                // 5/2納品分の在庫ロットはすべて新しいロット番号に置き換わり、しかし在庫推移は変わっていないはず
                .Lot(DateConst.May2nd, oldLot0502).HasNoInventoryActions()
                .Lot(DateConst.May2nd, newLot0502).Begin()
                    .At(DateConst.May2nd).Arrived(200).Used(0, 200)
                    .At(DateConst.May3rd).Used(0, 200)
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0).Discarded(0)
                    .End()
                // 5/3,6納品分の在庫ロットはすべて初期状態と同じに戻っているはず
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                    .At(DateConst.May4th).Used(0, 200)
                    .At(DateConst.May5th).Used(70, 130)
                    .At(DateConst.May6th).Used(40, 90).Discarded(90)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void OrderInTransaction_CommitAvailable()
        {
            LogUtil.DEBUGLOG_BeginTest();
            var lot0509 = 0;
            var numLot = 1;
            var quantity = numLot * ExpectedPart.QuantitiesPerLot;

            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                lot0509 = Model.SupplierModel.Order(context, DateConst.April30th, ExpectedPart, numLot, DateConst.May9th);
                transaction.Commit();
            }

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May9th, lot0509).Begin()
                    .At(DateConst.May9th).Arrived(quantity).Used(0, quantity)
                    .At(DateConst.May9th.AddDays(1)).Used(0, quantity)
                    .At(DateConst.May9th.AddDays(2)).Used(0, quantity)
                    .At(DateConst.May9th.AddDays(3)).Used(0, quantity).Discarded(quantity)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }


        [TestMethod]
        public void OrderInTransaction_RoolbackAvailable()
        {
            LogUtil.DEBUGLOG_BeginTest();
            var lot0509 = 0;
            var numLot = 1;
            var quantity = numLot * ExpectedPart.QuantitiesPerLot;

            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                lot0509 = Model.SupplierModel.Order(context, DateConst.April30th, ExpectedPart, numLot, DateConst.May9th);
                transaction.Rollback();
            }

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May9th, lot0509).HasNoInventoryActions()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();
            Assert.AreEqual(lot0509, Model.Sequences.SEQ_INVENTORY_LOT_NUMBER.Next(), "発注がロールバックされているので、再採番したときはロールバック前に採番したロット番号が取得できるはず");
            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void OrderToSupplier()
        {
            var orderNo = Model.SupplierModel.Order(DateConst.April30th, ExpectedSupplier, DateConst.May9th, new List<Tuple<BouquetPart, int>>() { Tuple.Create(ExpectedPart, 1) });
            var expectedLotNo = InitialOrders.Count()+1; // 新規ロット番号は既存ロット数+1 のはず
            var date = Enumerable.Range(0, 4).Select(i => DateConst.May9th.AddDays(i)).ToArray();

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(1, context.OrdersToSuppliers.Count());
                Assert.AreEqual(1, context.OrderDetailsToSuppliers.Count());
            }
            Assert.AreEqual($"{DateConst.April30th.ToString("yyyyMMdd")}-000001", orderNo);
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May9th, expectedLotNo).Begin()
                    .At(date[0]).Arrived(100).Used(0, 100)
                    .At(date[1]).Used(0, 100)
                    .At(date[2]).Used(0, 100)
                    .At(date[3]).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void OrderToSupplierInTransaction_CanRollback()
        {
            var orderNo = "";
            var expectedLotNo = InitialOrders.Count() + 1;

            using (var context = new MemorieDeFleursDbContext(TestDB))
            using(var transaction = context.Database.BeginTransaction())
            {
                orderNo = Model.SupplierModel.Order(context, DateConst.April30th, ExpectedSupplier, DateConst.May9th, new List<Tuple<BouquetPart, int>>() { Tuple.Create(ExpectedPart, 1) });
                transaction.Rollback();
            }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(0, context.OrdersToSuppliers.Count());
                Assert.AreEqual(0, context.OrderDetailsToSuppliers.Count());
            }
            Assert.AreEqual($"{DateConst.April30th.ToString("yyyyMMdd")}-000001", orderNo);
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May9th, expectedLotNo).HasNoInventoryActions()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void CanelOrderToSupplier()
        {
            var orderNo = Model.SupplierModel.Order(DateConst.April30th, ExpectedSupplier, DateConst.May9th, new List<Tuple<BouquetPart, int>>() { Tuple.Create(ExpectedPart, 1) });
            var expectedLotNo = InitialOrders.Count() + 1;

            Model.SupplierModel.CancelOrder(orderNo);

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(0, context.OrdersToSuppliers.Count());
                Assert.AreEqual(0, context.OrderDetailsToSuppliers.Count());
            }
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May9th, expectedLotNo).HasNoInventoryActions()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

        }

        [TestMethod]
        public void CancelOrderToSupplierInTransaction_CanRollback()
        {
            var orderNo = Model.SupplierModel.Order(DateConst.April30th, ExpectedSupplier, DateConst.May9th, new List<Tuple<BouquetPart, int>>() { Tuple.Create(ExpectedPart, 1) });
            var expectedLotNo = InitialOrders.Count() + 1;

            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                Model.SupplierModel.CancelOrder(context, orderNo);
                transaction.Rollback();
            }

            // キャンセルを取り消したので注文は残っているはず
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(1, context.OrdersToSuppliers.Count());
                Assert.AreEqual(1, context.OrderDetailsToSuppliers.Count());
            }

            var date = Enumerable.Range(0, 4).Select(i => DateConst.May9th.AddDays(i)).ToArray();
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May9th, expectedLotNo).Begin()
                    .At(date[0]).Arrived(100).Used(0, 100)
                    .At(date[1]).Used(0, 100)
                    .At(date[2]).Used(0, 100)
                    .At(date[3]).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void ChangeArrivalDate_UnusedInventory()
        {
            LogUtil.DEBUGLOG_BeginTest();
            var orderNo = Model.SupplierModel.Order(DateConst.May1st, ExpectedSupplier, DateConst.May7th, new List<Tuple<BouquetPart,int>>() { Tuple.Create(ExpectedPart, 1) });
            var lotNo = InitialOrders.Count() + 1;

            Model.SupplierModel.ChangeArrivalDate(orderNo, DateConst.May9th);

            var days = Enumerable.Range(0, ExpectedPart.ExpiryDate + 1).Select(i => DateConst.May9th.AddDays(i)).ToArray();

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May7th, lotNo).HasNoInventoryActions()
                .Lot(DateConst.May9th, lotNo).Begin()
                    .At(days[0]).Arrived(100).Used(0, 100)
                    .At(days[1]).Used(0, 100)
                    .At(days[2]).Used(0, 100)
                    .At(days[3]).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();
            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void ChangeArrivalDate_CanRollback()
        {
            LogUtil.DEBUGLOG_BeginTest();
            var orderNo = Model.SupplierModel.Order(DateConst.May1st, ExpectedSupplier, DateConst.May7th, new List<Tuple<BouquetPart, int>>() { Tuple.Create(ExpectedPart, 1) });
            var lotNo = InitialOrders.Count() + 1;

            using(var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                Model.SupplierModel.ChangeArrivalDate(context, orderNo, DateConst.May9th);
                transaction.Rollback();
            }

            var days = Enumerable.Range(0, ExpectedPart.ExpiryDate + 1).Select(i => DateConst.May7th.AddDays(i)).ToArray();

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May7th, lotNo).Begin()
                    .At(days[0]).Arrived(100).Used(0, 100)
                    .At(days[1]).Used(0, 100)
                    .At(days[2]).Used(0, 100)
                    .At(days[3]).Used(0, 100).Discarded(100)
                    .End()
                .Lot(DateConst.May9th, lotNo).HasNoInventoryActions()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();
            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void ChangeArrivalDate_InventoryUsed()
        {
            LogUtil.DEBUGLOG_BeginTest();

            var oldLot0502 = InitialOrders[DateConst.May2nd][0].LotNo;
            var lot0503 = InitialOrders[DateConst.May3rd][0].LotNo;
            var newLot0502 = InitialOrders.Count() + 1;

            // 5/2 のロットに発注番号を割り当てるため一度発注取消→再発注す：在庫ロット番号が変わるので注意。
            Model.SupplierModel.CancelOrder(oldLot0502);


            var orderNo = Model.SupplierModel.Order(DateConst.April30th, ExpectedSupplier, DateConst.May2nd,
                new List<Tuple<BouquetPart, int>>() { Tuple.Create(ExpectedPart, 2) });

            var days = Enumerable.Range(0, ExpectedPart.ExpiryDate + 1).Select(i => DateConst.May9th.AddDays(i)).ToArray();

            Model.SupplierModel.ChangeArrivalDate(orderNo, DateConst.May9th);

            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                // 5/2 のロットは、新旧両方とも存在しないはず
                .Lot(DateConst.May2nd, oldLot0502).HasNoInventoryActions()
                .Lot(DateConst.May2nd, newLot0502).HasNoInventoryActions()
                // 5/2 に再登録したロットは5/9～に移動しているはず
                .Lot(DateConst.May9th, newLot0502).Begin()
                    .At(days[0]).Arrived(200).Used(0, 200)
                    .At(days[1]).Used(0, 200)
                    .At(days[2]).Used(0, 200)
                    .At(days[3]).Used(0, 200).Discarded(200)
                    .End()
                // 5/3ロットの在庫推移が変わっている＝納品日変更により5/2のロットが注文取り消しされたのと同じ状態になるはず
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May3rd).Arrived(200).Used(0,200)
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0).Shortage(70)
                    .At(DateConst.May6th).Used(0, 0).Discarded(0)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void ChangeArrivalDateInTransaction_CanRollback()
        {
            LogUtil.DEBUGLOG_BeginTest();

            var oldLot0502 = InitialOrders[DateConst.May2nd][0].LotNo;
            var newLot0502 = InitialOrders.Count() + 1;
            var lot0503 = InitialOrders[DateConst.May3rd][0].LotNo;
            var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;

            // 5/2 のロットに発注番号を割り当てるため一度発注取消→再発注す：在庫ロット番号が変わるので注意。
            Model.SupplierModel.CancelOrder(oldLot0502);
            var orderNo = Model.SupplierModel.Order(DateConst.April30th, ExpectedSupplier, DateConst.May2nd,
                new List<Tuple<BouquetPart, int>>() { Tuple.Create(ExpectedPart, 2) });

            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                Model.SupplierModel.ChangeArrivalDate(context, orderNo, DateConst.May9th);
                transaction.Rollback();
            }

            // 5/2納品分の在庫ロットはすべて新しいロット番号に置き換わり、しかし在庫推移は初期状態と同じはず
            InventoryActionValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.May2nd, oldLot0502).HasNoInventoryActions()
                .Lot(DateConst.May2nd, newLot0502).Begin()
                    .At(DateConst.May2nd).Arrived(200).Used(0, 200)
                    .At(DateConst.May3rd).Used(0, 200)
                    .At(DateConst.May4th).Used(100, 100)
                    .At(DateConst.May5th).Used(100, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May3rd, lot0503).Begin()
                    .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                    .At(DateConst.May4th).Used(0, 200)
                    .At(DateConst.May5th).Used(70, 130)
                    .At(DateConst.May6th).Used(40, 90).Discarded(90)
                    .End()
                .Lot(DateConst.May6th, lot0506).Begin()
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                    .End()
                .End()
                .TargetDBIs(TestDB)
                .AssertAll();
        }
    }
}
