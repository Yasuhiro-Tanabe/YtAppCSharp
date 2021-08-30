using MemorieDeFleurs.Models;
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
        /// 発注時に所定の在庫アクションが登録されるかどうかのテスト
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

        [TestMethod]
        public void CanAddManyOrdersToSingleSupplier()
        {
            var expectedSupplier = Model.SupplierModel.Find(ExpectedSupplerCode);
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var expectedCountOfOrders = InitialOrderLotNumbers.Count;
            var expectedCountOfScheduledToUseStockActions = expectedPart.ExpiryDate * expectedCountOfOrders;
            
            Assert.AreEqual(expectedCountOfOrders, expectedCountOfOrders, $"注文数と発注ロット数の不一致：仕入先={expectedSupplier.Name}, 花コード={expectedPart.Name}");
            AssertAllLotNumbersAreUnique(InitialOrderLotNumbers);
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_ARRIVE);
            AssertStockActionCount(expectedCountOfScheduledToUseStockActions, StockActionType.SCHEDULED_TO_USE);
            AssertStockActionCount(expectedCountOfOrders, StockActionType.SCHEDULED_TO_DISCARD);
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
    }
}
