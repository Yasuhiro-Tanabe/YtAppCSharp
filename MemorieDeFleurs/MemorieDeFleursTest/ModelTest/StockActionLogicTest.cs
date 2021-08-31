﻿using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

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
        private int ExpectedSupplerCode { get; set; }
        private string ExpectedPartCode { get; set; }

        private List<int> InitialOrderLotNumbers { get; } = new List<int>();

        public StockActionLogicTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupDb;
        }

        private MemorieDeFleursModel Model { get; set; }

        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDBContext);

            var s = Model.SupplierModel.Entity<Supplier>()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .Create();
            var p = Model.BouquetModel.Entity<BouquetModel>()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();

            var orders = new
            {
                Supplier = s,
                Part = p,
                OrderDate = new DateTime(2020, 4, 25),
                OrderBody = new List<Tuple<DateTime, int>>() {
                    Tuple.Create(new DateTime(2020,4,30), 2),
                    Tuple.Create(new DateTime(2020,5,1), 2),
                    Tuple.Create(new DateTime(2020,5,2), 3),
                    Tuple.Create(new DateTime(2020,5,3), 2),
                    Tuple.Create(new DateTime(2020,5,6), 1)
                }
            };
            foreach (var o in orders.OrderBody)
            {
                InitialOrderLotNumbers.Add(Model.SupplierModel.Order(orders.OrderDate, p, o.Item2, o.Item1));
            }


            ExpectedSupplerCode = s.Code;
            ExpectedPartCode = p.Code;
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
            var expectedSupplier = Model.SupplierModel.Find(ExpectedSupplerCode);
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var orderDate = new DateTime(2020, 5, 10);
            var arrivalDate = new DateTime(2020, 5, 20);
            var discardDate = arrivalDate.AddDays(expectedPart.ExpiryDate);
            var numLot = 2;
            var expectedQuantity = numLot * expectedPart.QuantitiesPerLot;

            var expectedLotNumber = Model.SupplierModel.Order(orderDate, expectedPart, numLot, arrivalDate);

            //入荷予定と破棄予定の各在庫アクションが正しい日付、登録数、破棄数で登録されている
            AssertStockAction(StockActionType.SCHEDULED_TO_ARRIVE, arrivalDate, arrivalDate, expectedPart.Code, expectedLotNumber, expectedQuantity, expectedQuantity);
            AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, discardDate, arrivalDate, expectedPart.Code, expectedLotNumber, expectedQuantity, 0);
            foreach(var d in Enumerable.Range(0, expectedPart.ExpiryDate).Select(i => arrivalDate.AddDays(i)))
            {
                // 入荷予定日当日～破棄予定日当日 の間の使用予定在庫アクションが登録されている
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, d, arrivalDate, expectedPart.Code, expectedLotNumber, 0, expectedQuantity);
            }
        }

        /// <summary>
        /// 複数の入荷(入荷日の重複なし)により必要な数の在庫アクションが登録されることを確認する
        /// </summary>
        [TestMethod]
        public void CanAddManyOrdersToSingleSupplier()
        {
            var expectedSupplier = Model.SupplierModel.Find(ExpectedSupplerCode);
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var expectedCountOfOrders = InitialOrderLotNumbers.Count;
            var expectedCountOfScheduledToUseStockActions = expectedPart.ExpiryDate * expectedCountOfOrders;
            
            // 登録は TestInitialize で行っている処理で代用

            Assert.AreEqual(expectedCountOfOrders, expectedCountOfOrders, $"注文数と発注ロット数の不一致：仕入先={expectedSupplier.Name}, 花コード={expectedPart.Name}");
            AssertAllLotNumbersAreUnique(InitialOrderLotNumbers);
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_ARRIVE);
            AssertStockActionCount(expectedCountOfScheduledToUseStockActions, StockActionType.SCHEDULED_TO_USE);
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_DISCARD);
        }

        /// <summary>
        /// 複数ロットの入荷(予定)在庫アクションに潤沢な数量がある状態では、入荷日の一番若い在庫ロットから順に引当されることの確認
        /// </summary>
        [TestMethod]
        public void WillBeUsedEarliestArrivedStockLotWhenTwoOrMoreLotHasEnoughQuantity()
        {
            var expectedSupplier = Model.SupplierModel.Find(ExpectedSupplerCode);
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);

            var expected = new {
                LotNo = InitialOrderLotNumbers[0],
                Arrival = new DateTime(2020, 4, 30),
                PreviousDay = new DateTime(2020, 4, 30),
                Today = new DateTime(2020, 5, 1),
                NextDay = new DateTime(2020, 5, 2),
                InitialQuantity = 200,
                Used = 60,
                Remain = 140

            };
            var another = new
            {
                LotNo = InitialOrderLotNumbers[1],
                Arrival = new DateTime(2020, 5, 1),
                Today = new DateTime(2020, 5, 1),
                NextDay = new DateTime(2020, 5, 2),
                InitialQuantity = 200,
                Used = 0,
                Remain = 200
            };

            var actualRemain = Model.BouquetModel.UseBouquetPart(expectedPart, expected.Today, expected.Used);

            Assert.AreEqual(expected.Remain + another.Remain, actualRemain);

            // 引当てされた同一ロットに属する前日、当日、翌日の在庫アクションの数量/残数が意図通りに変化している
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expected.PreviousDay, expected.Arrival, ExpectedPartCode, expected.LotNo, 0, expected.InitialQuantity);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expected.Today, expected.Arrival, ExpectedPartCode, expected.LotNo, expected.Used, expected.Remain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expected.NextDay, expected.Arrival, ExpectedPartCode, expected.LotNo, 0, expected.Remain);

            // 引当てされたのとは別ロットに影響が出ていない
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, another.Today, another.Arrival, ExpectedPartCode, another.LotNo, 0, another.Remain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, another.NextDay, another.Arrival, ExpectedPartCode, another.LotNo, 0, another.Remain);
        }

        [TestMethod]
        public void CanRemoveOneOrdersOfSingleSupplier()
        {
            var expectedSupplier = Model.SupplierModel.Find(ExpectedSupplerCode);
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var indexOfCancelOrder = 2; // 20200502 のオーダー
            var expectedCountOfOrders = InitialOrderLotNumbers.Count - 1;
            var expectedCountOfScheduledToUseStockActions = expectedPart.ExpiryDate * expectedCountOfOrders;

            var expectedCanceledLotNumber = InitialOrderLotNumbers[indexOfCancelOrder]; 

            Model.SupplierModel.CancelOrder(expectedCanceledLotNumber);
            InitialOrderLotNumbers.RemoveAt(indexOfCancelOrder);

            Assert.AreEqual(expectedCountOfOrders, InitialOrderLotNumbers.Count, $"注文数と発注ロット数の不一致：仕入先={expectedSupplier.Name}, 花コード={expectedPart.Name}");

            // 対象在庫ロットに関する在庫アクションが消えていること
            AssertNoStockActions(StockActionType.SCHEDULED_TO_ARRIVE, expectedCanceledLotNumber);
            AssertNoStockActions(StockActionType.SCHEDULED_TO_USE, expectedCanceledLotNumber);
            AssertNoStockActions(StockActionType.SCHEDULED_TO_DISCARD, expectedCanceledLotNumber);

            // ほかの在庫ロットアクションが消えていないこと
            AssertAllLotNumbersAreUnique(InitialOrderLotNumbers);
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_ARRIVE);
            AssertStockActionCount(expectedCountOfScheduledToUseStockActions, StockActionType.SCHEDULED_TO_USE);
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_DISCARD);
        }

        [TestMethod]
        public void CanRemoveUsedQuantityOfPartsFromStockAction()
        {
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var expectedUsedDate = new DateTime(2020, 4, 30);
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = InitialOrderLotNumbers[0];
            var quantity = 20;
            var expectedRemain = 180;

            var actualRemain = Model.BouquetModel.UseBouquetPart(expectedPart, expectedUsedDate, quantity);

            // 加工当日分残数は正しいか
            Assert.AreEqual(expectedRemain, actualRemain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expectedUsedDate, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, quantity, expectedRemain);

            // 翌日から破棄予定日までの残数に反映されているか (翌日からなので列挙の要素数は「品質維持可能日数-1」個)
            foreach(var day in Enumerable.Range(1, expectedPart.ExpiryDate-1).Select(i => expectedArrivalDate.AddDays(i)))
            {
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, day, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, 0, expectedRemain);
            }

            // 破棄された数はあっているか
            var discardDate = expectedArrivalDate.AddDays(expectedPart.ExpiryDate);
            AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, discardDate, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, expectedRemain, 0);
        }

        [TestMethod]
        public void AllStocksInTheDayIsUsed()
        {
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var expectedUsedDate = new DateTime(2020, 4, 30);
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = InitialOrderLotNumbers[0];
            var quantity = 200;
            var expectedRemain = 0;

            var actualRemain = Model.BouquetModel.UseBouquetPart(expectedPart, expectedUsedDate, quantity);

            // 加工当日分残数は正しいか
            Assert.AreEqual(expectedRemain, actualRemain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expectedUsedDate, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, quantity, expectedRemain);

            // 翌日から破棄予定日までの残数に反映されているか (翌日からなので列挙の要素数は「品質維持可能日数-1」個)
            foreach (var day in Enumerable.Range(1, expectedPart.ExpiryDate - 1).Select(i => expectedArrivalDate.AddDays(i)))
            {
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, day, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, 0, expectedRemain);
            }

            // 破棄された数はあっているか
            var discardDate = expectedArrivalDate.AddDays(expectedPart.ExpiryDate);
            AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, discardDate, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, expectedRemain, 0);
        }

        [TestMethod]
        public void NotEnoughStocksInTheDay_andOutofStockRecordGenerated()
        {
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var expectedUsedDate = new DateTime(2020, 4, 30);
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = InitialOrderLotNumbers[0];
            var quantity = 220;
            var expectedUsedQuantity = 200;
            var expectedOutOfStock = 20;

            var actualRemain = Model.BouquetModel.UseBouquetPart(expectedPart, expectedUsedDate, quantity);

            // 加工当日分残数は正しいか
            Assert.AreEqual(-expectedOutOfStock, actualRemain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expectedUsedDate, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, expectedUsedQuantity, 0);

            // 翌日から破棄予定日までの残数に反映されているか (翌日からなので列挙の要素数は「品質維持可能日数-1」個)
            foreach (var day in Enumerable.Range(1, expectedPart.ExpiryDate - 1).Select(i => expectedArrivalDate.AddDays(i)))
            {
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, day, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, 0, 0);
            }

            // 破棄された数はあっているか
            var discardDate = expectedArrivalDate.AddDays(expectedPart.ExpiryDate);
            AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, discardDate, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, 0, 0);

            // 在庫不足の在庫アクションが登録されているか
            AssertStockAction(StockActionType.OUT_OF_STOCK, expectedArrivalDate, expectedArrivalDate, ExpectedPartCode, expectedLotNumber, expectedOutOfStock, -expectedOutOfStock);

        }

        #region 在庫アクションに関する検証用サポート関数
        /// <summary>
        /// 特定の１在庫アクションが、数量や残数も含めすべて意図通り登録されているかどうかを検証する
        /// </summary>
        /// <param name="type">在庫アクション</param>
        /// <param name="targetDate">基準日</param>
        /// <param name="arrivalDate">入荷(予定)日</param>
        /// <param name="partCode">花コード</param>
        /// <param name="lotno">在庫ロット番号</param>
        /// <param name="quantity">数量</param>
        /// <param name="remain">残数</param>
        private void AssertStockAction(StockActionType type, DateTime targetDate, DateTime arrivalDate, string partCode, int lotno, int quantity, int remain)
        {
            var key = $"基準日={targetDate.ToString("yyyyMMdd")}, アクション={type.ToString()}, 花コード={partCode}, 在庫ロット番号={lotno}, 入荷日={arrivalDate.ToString("yyyyMMdd")}";

            var candidate = TestDBContext.StockActions
                .Where(a => a.Action == type)
                .Where(a => a.ActionDate == targetDate)
                .Where(a => a.PartsCode == partCode)
                .Where(a => a.StockLotNo == lotno)
                .Where(a => a.ArrivalDate == arrivalDate);

            Assert.IsNotNull(candidate, "抽出結果が null：" + key);
            Assert.AreNotEqual(0, candidate.Count(), "該当するアクションが0件：" + key);
            Assert.AreEqual(1, candidate.Count(), $"該当するアクションが {candidate.Count()} 個ある：" + key);

            var action = candidate.SingleOrDefault();

            Assert.AreEqual(quantity, action.Quantity, "数量不一致：" + key);
            Assert.AreEqual(remain, action.Remain, "残数不一致：" + key);
        }

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
        /// <param name="lotNumbers">懸賞対象ロット番号の一覧</param>
        private void AssertAllLotNumbersAreUnique(List<int> lotNumbers)
        {
            lotNumbers.ForEach(i => Assert.AreEqual(1, TestDBContext.StockActions.Count(a => a.Action == StockActionType.SCHEDULED_TO_ARRIVE && a.StockLotNo == i)));
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
