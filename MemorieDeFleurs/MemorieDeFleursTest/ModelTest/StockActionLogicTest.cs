﻿using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    /// <summary>
    /// 発注処理 (仕入先への注文) による在庫管理
    /// </summary>
    [TestClass]
    public class StockActionLogicTest : MemorieDeFleursDbContextTestBase
    {
        private Supplier ExpectedSupplier { get; set; }
        private BouquetPart ExpectedPart { get; set; }

        public StockActionLogicTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupDb;
        }

        private MemorieDeFleursModel Model { get; set; }

        private TestOrder InitialOrders { get; } = new TestOrder();

        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDBContext);

            ExpectedSupplier = Model.SupplierModel.Entity<Supplier>()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .Create();
            ExpectedPart = Model.BouquetModel.Entity<BouquetModel>()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();

            var orders = new
            {
                Supplier = ExpectedSupplier,
                Part = ExpectedPart,
                OrderDate = new DateTime(2020, 4, 25),
                OrderBody = new List<Tuple<DateTime, int>>() {
                    Tuple.Create(DateConst.April30th, 2),
                    Tuple.Create(DateConst.May1st, 3),
                    Tuple.Create(DateConst.May2nd, 2),
                    Tuple.Create(DateConst.May3rd, 2),
                    Tuple.Create(DateConst.May6th, 1)
                }
            };

            foreach(var o in orders.OrderBody)
            {
                var lotNo = Model.SupplierModel.Order(orders.OrderDate, orders.Part, o.Item2, o.Item1);
                InitialOrders.Append(o.Item1, lotNo, o.Item2 * orders.Part.QuantitiesPerLot);
            }
        }

        private void CleanupDb(object sender, EventArgs unused)
        {
            ClearAll();
        }

        /// <summary>
        /// 発注時に所定の在庫アクションが所定の日数分登録されることを確認する
        /// </summary>
        [TestMethod]
        public void CanAddOneOrderToSingleSupplier()
        {
            var orderDate = new DateTime(2020, 5, 10);
            var arrivalDate = new DateTime(2020, 5, 20);
            var discardDate = arrivalDate.AddDays(ExpectedPart.ExpiryDate);
            var numLot = 2;
            var expectedQuantity = numLot * ExpectedPart.QuantitiesPerLot;

            var expectedLotNumber = Model.SupplierModel.Order(orderDate, ExpectedPart, numLot, arrivalDate);

            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(arrivalDate, expectedLotNumber).Begin()
                    .At(arrivalDate).Arrived(expectedQuantity).Used(0, expectedQuantity)
                    .At(arrivalDate.AddDays(1)).Used(0, expectedQuantity)
                    .At(arrivalDate.AddDays(2)).Used(0, expectedQuantity)
                    .At(arrivalDate.AddDays(3)).Used(0, expectedQuantity).Discarded(expectedQuantity)
                    .End()
                .End()
                .AssertAll(TestDBContext);
        }

        /// <summary>
        /// 複数の入荷(入荷日の重複なし)により必要な数の在庫アクションが登録されることを確認する
        /// </summary>
        [TestMethod]
        public void CanAddManyOrdersToSingleSupplier()
        {
            var expectedCountOfOrders = InitialOrders.SelectMany(i => i.Value).Count();
            var expectedCountOfScheduledToUseStockActions = (ExpectedPart.ExpiryDate + 1) * expectedCountOfOrders;
            
            // 登録は TestInitialize で行っている処理で代用

            Assert.AreEqual(expectedCountOfOrders, expectedCountOfOrders, $"注文数と発注ロット数の不一致：仕入先={ExpectedSupplier.Name}, 花コード={ExpectedPart.Name}");
            AssertAllLotNumbersAreUnique();
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_ARRIVE);
            AssertStockActionCount(expectedCountOfScheduledToUseStockActions, StockActionType.SCHEDULED_TO_USE);
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_DISCARD);
        }

        /// <summary>
        /// 複数ロットの入荷(予定)在庫アクションに潤沢な数量がある状態では、入荷日の一番若い在庫ロットの数量ないで加工が行われることの確認
        /// </summary>
        [TestMethod]
        public void WillBeUsedEarliestArrivedStockLotWhenTwoOrMoreLotHasEnoughQuantity()
        {
            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, DateConst.May1st, 60);
            var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                // 引当てされた同一ロットに属する前日、当日、翌日の在庫アクションの数量/残数が意図通りに変化している
                .Lot(DateConst.April30th, findLotNo).Begin()
                    .At(DateConst.April30th).Used(0, 200)
                    .At(DateConst.May1st).Used(60, 140)
                    .At(DateConst.May2nd).Used(0, 140)
                    .End()
                // 引当てされたのとは別ロットに影響が出ていない
                .Lot(DateConst.May1st, findLotNo).Begin()
                    .At(DateConst.May1st).Used(0, 300)
                    .At(DateConst.May2nd).Used(0, 300)
                    .End()
                .End()
                .AssertAll(TestDBContext);

        }

        [TestMethod]
        public void CanRemoveOneOrdersOfSingleSupplier()
        {
            var orderCancelDate = DateConst.May2nd;
            var expectedCountOfOrders = InitialOrders.SelectMany(i => i.Value).Count() - 1;
            var expectedCountOfScheduledToUseStockActions = (ExpectedPart.ExpiryDate + 1) * expectedCountOfOrders;

            var expectedCanceledLotNumber = InitialOrders[orderCancelDate][0].LotNo; 

            Model.SupplierModel.CancelOrder(expectedCanceledLotNumber);
            InitialOrders.Remove(expectedCanceledLotNumber);


            var actualCountOfOrders = InitialOrders.SelectMany(i => i.Value).Count();
            Assert.AreEqual(expectedCountOfOrders, actualCountOfOrders, $"注文数と発注ロット数の不一致：仕入先={ExpectedSupplier.Name}, 花コード={ExpectedPart.Name}");

            // 対象在庫ロットに関する在庫アクションが消えていること
            AssertNoStockActions(StockActionType.SCHEDULED_TO_ARRIVE, expectedCanceledLotNumber);
            AssertNoStockActions(StockActionType.SCHEDULED_TO_USE, expectedCanceledLotNumber);
            AssertNoStockActions(StockActionType.SCHEDULED_TO_DISCARD, expectedCanceledLotNumber);

            // ほかの在庫ロットアクションが消えていないこと
            AssertAllLotNumbersAreUnique();
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_ARRIVE);
            AssertStockActionCount(expectedCountOfScheduledToUseStockActions, StockActionType.SCHEDULED_TO_USE);
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_DISCARD);
        }

        [TestMethod]
        public void CanRemoveUsedQuantityOfPartsFromStockAction()
        {
            var lotNo = InitialOrders[DateConst.April30th][0].LotNo;

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, DateConst.April30th, 20);

            Assert.AreEqual(180, actualRemain);
            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.April30th, lotNo).Begin()
                    .At(DateConst.April30th).Used(20, 180)
                    .At(DateConst.May1st).Used(0, 180)
                    .At(DateConst.May2nd).Used(0, 180)
                    .At(DateConst.May3rd).Used(0, 180).Discarded(180)
                    .End()
                .End()
                .AssertAll(TestDBContext);
        }

        [TestMethod]
        public void AllStocksInTheDayIsUsed()
        {
            var lotNo = InitialOrders[DateConst.April30th][0].LotNo;

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, DateConst.April30th, 200);

            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.April30th, lotNo).Begin()
                    .At(DateConst.April30th).Used(200, 0)
                    .At(DateConst.May1st).Used(0, 0)
                    .At(DateConst.May2nd).Used(0, 0)
                    .At(DateConst.May3rd).Used(0, 0).Discarded(0)
                    .End()
                .End()
                .AssertAll(TestDBContext);
        }

        [TestMethod]
        public void NotEnoughStocksInTheDay_andOutofStockRecordGenerated()
        {
            var lotNo = InitialOrders[DateConst.April30th][0].LotNo;

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, DateConst.April30th, 220);

            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.April30th, lotNo).Begin()
                    .At(DateConst.April30th).Used(200, 0).OutOfStock(20)
                    .At(DateConst.May1st).Used(0, 0)
                    .At(DateConst.May2nd).Used(0, 0)
                    .At(DateConst.May3rd).Used(0, 0).Discarded(0)
                    .End()
                .End()
                .AssertAll(TestDBContext);
        }

        /// <summary>
        /// 一つの在庫アクションの残数では使用数を賄えないとき、同一日付の別在庫アクションからも使用数不足分を引くことができる
        /// </summary>
        [TestMethod]
        public void CanRemoveFromTwoOrMoreStockActions()
        {
            var lotNo = new int[]
            {
                InitialOrders[DateConst.April30th][0].LotNo,
                InitialOrders[DateConst.May1st][0].LotNo,
                InitialOrders[DateConst.May2nd][0].LotNo,
            };

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, DateConst.May2nd, 500);

            // 2ロット分の全量を消費し、ただし3ロット目は消費しない
            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.April30th, lotNo[0]).Begin()
                    .At(DateConst.May1st).Used(0, 200)
                    .At(DateConst.May2nd).Used(200, 0)
                    .At(DateConst.May3rd).Used(0, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May1st, lotNo[1]).Begin()
                    .At(DateConst.May1st).Used(0, 300)
                    .At(DateConst.May2nd).Used(300, 0)
                    .At(DateConst.May3rd).Used(0, 0)
                    .At(DateConst.May4th).Used(0, 0).Discarded(0)
                    .End()
                .Lot(DateConst.May2nd, lotNo[2]).Begin()
                    .At(DateConst.May2nd).Used(0, 200)
                    .At(DateConst.May3rd).Used(0, 200)
                    .At(DateConst.May5th).Discarded(200)
                    .End()
                .End()
                .AssertAll(TestDBContext);

            // 一日の在庫はトータルすれば潤沢なので、残数不足の在庫アクションは生成されない
            AssertStockActionCount(0, StockActionType.OUT_OF_STOCK);
        }

        [TestMethod]
        public void CompositeTestFromApril30ToMay7th()
        {
            var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

            var input = new Dictionary<DateTime, int>()
            {
                { DateConst.April30th, 20 },
                { DateConst.May1st, 50 },
                { DateConst.May2nd, 80 },
                { DateConst.May3rd, 20 },
                { DateConst.May4th, 400 },
                { DateConst.May5th, 170 },
                { DateConst.May6th, 40 }
            };
            
            var actualRemain = input.Select(u => Model.BouquetModel.UseBouquetPart(ExpectedPart, u.Key, u.Value));

            // 検証1：注文反映中に在庫不足が発生していないこと
            Assert.IsTrue(actualRemain.All(r => r > 0));

            // 検証2 全在庫アクションが意図通り登録されていること
            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart).Begin()
                .Lot(DateConst.April30th, findLotNo).Begin()
                    .At(DateConst.April30th).Arrived(200).Used(20, 180)
                    .At(DateConst.May1st).Used(50, 130)
                    .At(DateConst.May2nd).Used(80, 50)
                    .At(DateConst.May3rd).Used(20, 30).Discarded(30)
                    .End()
                .Lot(DateConst.May1st, findLotNo).Begin()
                    .At(DateConst.May1st).Arrived(300).Used(0, 300)
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
                .Lot(DateConst.May3rd, findLotNo).Begin()
                    .At(DateConst.May3rd).Arrived(200).Used(0, 200)
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
                .End()
                .AssertAll(TestDBContext);

            // 検証3：在庫不足アクションが登録されていないこと
            AssertStockActionCount(0, StockActionType.OUT_OF_STOCK);
        }

        #region 在庫アクションに関する検証用サポート関数
        /// <summary>
        /// 特定のアクションタイプを持つ在庫アクションが指定個数登録されているかどうかを検証する
        /// </summary>
        /// <param name="expected">在庫アクション数の期待値</param>
        /// <param name="type">検証対象の在庫アクションタイプ</param>
        private void AssertStockActionCount(int expected, StockActionType type)
        {
            int actual = TestDBContext.StockActions.Count(a => a.Action == type);
            Assert.AreEqual(expected, actual, $"登録されるべき在庫アクション数の不一致：アクション={type.ToString()}");
        }

        /// <summary>
        /// 各ロット毎の入荷予定在庫アクションを数え、指定ロット番号の在庫アクションが登録されているか、ロット番号に重複がないかどうかを検証する
        /// </summary>
        private void AssertAllLotNumbersAreUnique()
        {
            // Linqで全ロット番号を一つの IEnumerable に変形する手段もあるが、それだとアサーション発生したロットの入荷日がわからないので
            // 愚直に二重ループを回す
            foreach(var i in InitialOrders)
            {
                foreach(var j in i.Value)
                {
                    var actual = TestDBContext.StockActions.Count(a => a.Action == StockActionType.SCHEDULED_TO_ARRIVE && a.StockLotNo == j.LotNo);
                    var days = TestDBContext.StockActions.Where(a => a.Action == StockActionType.SCHEDULED_TO_ARRIVE && a.StockLotNo == j.LotNo).Select(a => a.ArrivalDate);
                    Assert.AreEqual(1, actual, $"ロット番号={j}, 入荷日=[{string.Join(", ", days)}]");
                }
            }
        }

        /// <summary>
        /// 指定ロット番号の、指定された在庫アクションが登録されて*いない*ことを検証する
        /// </summary>
        /// <param name="type">在庫アクションタイプ</param>
        /// <param name="lotNo">在庫ロット番号</param>
        private void AssertNoStockActions(StockActionType type, int lotNo)
        {
            Assert.AreEqual(0, TestDBContext.StockActions.Count(a => a.Action == type && a.StockLotNo == lotNo));
        }
#endregion // 在庫アクションに関する検証用サポート関数
    }
}
