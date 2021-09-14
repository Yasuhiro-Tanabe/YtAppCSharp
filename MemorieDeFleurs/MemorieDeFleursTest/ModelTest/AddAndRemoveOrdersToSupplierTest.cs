using MemorieDeFleurs;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MemorieDeFleursTest.ModelTest.Fluent;
using MemorieDeFleurs.Logging;

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
            LogUtil.Debug("=====NewOrderWhichArrivedAt20200504 [Begin]=====");

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var newLotNo = Model.SupplierModel.Order(context, DateConst.April30th, ExpectedPart, 1, DateConst.May4th);
                var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

                StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
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
                    .AssertAll(context);
            }

            LogUtil.Debug("=====NewOrderWhichArrivedAt20200504 [End]=====");
        }

        /// <summary>
        /// 5/2納品予定分の追加発注により、5/3納品予定分から払い出していた5/5加工分70本が、
        /// 今回追加発注分からの払い出しに置き換わること
        /// </summary>
        [TestMethod]
        public void NewOrderWhichArrivedAt20200502()
        {
            LogUtil.Debug("=====NewOrderWhichArrivedAt20200502 [Begin]=====");

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var newLotNo = Model.SupplierModel.Order(context, DateConst.April30th, ExpectedPart, 1, DateConst.May2nd);
                var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

                StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
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
                    .AssertAll(context);
            }

            LogUtil.Debug("=====NewOrderWhichArrivedAt20200502 [End]=====");
        }

        /// <summary>
        /// 未使用の在庫ロットを破棄しても他の在庫ロットに影響が出ない
        /// </summary>
        [TestMethod]
        public void RemoveOrderArrivedAt20200506()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var lot = InitialOrders[DateConst.May6th][0].LotNo;
                var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

                Model.SupplierModel.CancelOrder(context, lot);

                // 他在庫ロットの在庫アクションで、破棄した在庫ロットの入荷日以降の在庫アクションに変化がない
                StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                    .Lot(DateConst.May6th, lot).HasNoStockActions()
                    .Lot(DateConst.May3rd, findLotNo).Begin()
                        .At(DateConst.May6th).Used(40, 90).Discarded(90)
                        .End()
                    .End()
                    .AssertAll(context);
            }
        }

        /// <summary>
        /// 在庫ロットの破棄により在庫不足が派生する
        /// </summary>
        [TestMethod]
        public void RemoveOrderArrivedAt20200502()
        {
            LogUtil.Debug("===== RemoveOrderArrivedAt20200502 [Begin]=====");

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var lot = InitialOrders[DateConst.May2nd][0].LotNo;
                var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

                Model.SupplierModel.CancelOrder(context, lot);

                StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                    .Lot(DateConst.May2nd, lot).HasNoStockActions()
                    .Lot(DateConst.May3rd, findLotNo).Begin()
                        .At(DateConst.May4th).Used(100, 100)
                        .At(DateConst.May5th).Used(100, 0).OutOfStock(70)
                        .At(DateConst.May6th).Used(0, 0).Discarded(0)
                        .End()
                    .Lot(DateConst.May6th, findLotNo).Begin()
                        .At(DateConst.May6th).Used(40, 60)
                        .At(DateConst.May7th).Used(0, 60)
                        .At(DateConst.May8th).Used(0, 60)
                        .At(DateConst.May9th).Used(0, 60).Discarded(60)
                        .End()
                    .End()
                    .AssertAll(context);
            }

            LogUtil.Debug("===== RemoveOrderArrivedAt20200502 [End]=====");
        }

        /// <summary>
        /// 発注取消により生じた在庫不足が、追加発注により解消される
        /// 
        /// 未来入荷分からの振替も、すべて追加発注分で加工される
        /// </summary>
        [TestMethod]
        public void ChangeOrders_RemoveFrom20200502_And_AddTwoLotTo202005005()
        {
            LogUtil.Debug("===== ChangeOrders_RemoveFrom20200502_And_AddTo202005005 (order = 2 Lot (200)) [Begin]=====");

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var lot0502 = InitialOrders[DateConst.May2nd][0].LotNo;
                var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;
                var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

                LogUtil.Debug($"+++ CancelOrder({lot0502})");
                Model.SupplierModel.CancelOrder(context, lot0502);
                StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                    .Lot(DateConst.May2nd, lot0502).HasNoStockActions()
                    .Lot(DateConst.May3rd, findLotNo).Begin()
                        .At(DateConst.May4th).Used(100, 100)
                        .At(DateConst.May5th).Used(100, 0).OutOfStock(70)
                        .At(DateConst.May6th).Used(0, 0).Discarded(0)
                        .End()
                    .Lot(DateConst.May6th, findLotNo).Begin()
                        .At(DateConst.May6th).Used(40, 60)
                        .At(DateConst.May7th).Used(0, 60)
                        .At(DateConst.May8th).Used(0, 60)
                        .At(DateConst.May9th).Used(0, 60).Discarded(60)
                        .End()
                    .End()
                    .AssertAll(context);

                LogUtil.Debug($"+++ Order({DateConst.May5th.ToString("yyyyMMdd")})");
                var lot0505 = Model.SupplierModel.Order(context, DateConst.May1st, ExpectedPart, 2, DateConst.May5th);

                StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                    .Lot(DateConst.May2nd, lot0502).HasNoStockActions()
                    .Lot(DateConst.May5th, lot0505).Begin()
                        .At(DateConst.May5th).Used(70, 130)
                        .At(DateConst.May6th).Used(40, 90)
                        .At(DateConst.May7th).Used(0, 90)
                        .At(DateConst.May8th).Used(0, 90).Discarded(90).End()
                    .Lot(DateConst.May6th, lot0506).Begin()
                        .At(DateConst.May6th).Used(0, 100)
                        .At(DateConst.May7th).Used(0, 100)
                        .At(DateConst.May8th).Used(0, 100)
                        .At(DateConst.May9th).Used(0, 100).Discarded(100).End()
                    .End()
                    .StockActionCountShallBe(StockActionType.OUT_OF_STOCK, 0)
                    .AssertAll(context);
            }

            LogUtil.Debug("===== ChangeOrders_RemoveFrom20200502_And_AddTo202005005 (order = 2 Lot (200)) [End]=====");
        }

        /// <summary>
        /// 在庫不足は解消されるが、未来入荷予定分への振替はゼロにならない
        /// </summary>
        [TestMethod]
        public void ChangeOrders_RemoveFrom20200502_And_AddOneLotTo202005005()
        {
            LogUtil.Debug("===== ChangeOrders_RemoveFrom20200502_And_AddTo202005005 (order = 1 Lot (100)) [Begin]=====");

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var lot0502 = InitialOrders[DateConst.May2nd][0].LotNo;
                var lot0506 = InitialOrders[DateConst.May6th][0].LotNo;
                var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

                Model.SupplierModel.CancelOrder(context, lot0502);
                var lot0505 = Model.SupplierModel.Order(context, DateConst.May1st, ExpectedPart, 1, DateConst.May5th);

                StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                    .Lot(DateConst.May2nd, lot0502).HasNoStockActions()
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
                    .StockActionCountShallBe(StockActionType.OUT_OF_STOCK, 0)
                    .AssertAll(context);
            }

            LogUtil.Debug("===== ChangeOrders_RemoveFrom20200502_And_AddTo202005005 (order = 1 Lot (100)) [End]=====");
        }
    }
}
