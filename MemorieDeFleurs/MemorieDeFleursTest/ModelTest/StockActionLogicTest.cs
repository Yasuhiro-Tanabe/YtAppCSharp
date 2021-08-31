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

        private static class Date
        {
            public static DateTime April30th { get; } = new DateTime(2020, 4, 30);
            public static DateTime May1st { get; } = new DateTime(2020, 5, 1);
            public static DateTime May2nd { get; } = new DateTime(2020, 5, 2);

            public static DateTime May3rd { get; } = new DateTime(2020, 5, 3);

            public static DateTime May4th { get; } = new DateTime(2020, 5, 4);
            public static DateTime May5th { get; } = new DateTime(2020, 5, 5);
            public static DateTime May6th { get; } = new DateTime(2020, 5, 6);
            public static DateTime May7th { get; } = new DateTime(2020, 5, 7);
            public static DateTime May8th { get; } = new DateTime(2020, 5, 8);
            public static DateTime May9th { get; } = new DateTime(2020, 5, 9);

        }



        private IDictionary<DateTime, ISet<int>> ArrivedLotNumbers { get; } = new SortedDictionary<DateTime, ISet<int>>();

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
                    Tuple.Create(Date.April30th, 2),
                    Tuple.Create(Date.May1st, 3),
                    Tuple.Create(Date.May2nd, 2),
                    Tuple.Create(Date.May3rd, 2),
                    Tuple.Create(Date.May6th, 1)
                }
            };
            foreach (var o in orders.OrderBody)
            {
                ISet<int> lotNumbers;
                if(!ArrivedLotNumbers.TryGetValue(o.Item1, out lotNumbers))
                {
                    lotNumbers = new SortedSet<int>();
                    ArrivedLotNumbers.Add(o.Item1, lotNumbers);
                }

                lotNumbers.Add(Model.SupplierModel.Order(orders.OrderDate, p, o.Item2, o.Item1));
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
            var expectedCountOfOrders = ArrivedLotNumbers.SelectMany(i => i.Value).Count();
            var expectedCountOfScheduledToUseStockActions = (expectedPart.ExpiryDate + 1) * expectedCountOfOrders;
            
            // 登録は TestInitialize で行っている処理で代用

            Assert.AreEqual(expectedCountOfOrders, expectedCountOfOrders, $"注文数と発注ロット数の不一致：仕入先={expectedSupplier.Name}, 花コード={expectedPart.Name}");
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
            var expectedSupplier = Model.SupplierModel.Find(ExpectedSupplerCode);
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var expected = new {
                LotNo = ArrivedLotNumbers[Date.April30th].First(),
                Arrival = Date.April30th,
                PreviousDay = Date.April30th,
                Today = Date.May1st,
                NextDay = Date.May2nd,
                InitialQuantity = 200,
                Used = 60,
                Remain = 140

            };
            var another = new
            {
                LotNo = ArrivedLotNumbers[Date.May1st].First(),
                Arrival = Date.May1st,
                Today = Date.May1st,
                NextDay = Date.May2nd,
                InitialQuantity = 300,
                Used = 0,
                Remain = 300
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
            var orderCancelDate = Date.May2nd;
            var expectedCountOfOrders = ArrivedLotNumbers.SelectMany(i => i.Value).Count() - 1;
            var expectedCountOfScheduledToUseStockActions = (expectedPart.ExpiryDate + 1) * expectedCountOfOrders;

            var expectedCanceledLotNumber = ArrivedLotNumbers[orderCancelDate].First(); 

            Model.SupplierModel.CancelOrder(expectedCanceledLotNumber);
            ArrivedLotNumbers[orderCancelDate].Remove(expectedCanceledLotNumber);


            var actualCountOfOrders = ArrivedLotNumbers.SelectMany(i => i.Value).Count();
            Assert.AreEqual(expectedCountOfOrders, actualCountOfOrders, $"注文数と発注ロット数の不一致：仕入先={expectedSupplier.Name}, 花コード={expectedPart.Name}");

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
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var expectedUsedDate = Date.April30th;
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = ArrivedLotNumbers[expectedArrivalDate].First();
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
            var expectedUsedDate = Date.April30th;
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = ArrivedLotNumbers[expectedArrivalDate].First();
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
            var expectedUsedDate = Date.April30th;
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = ArrivedLotNumbers[expectedArrivalDate].First();
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

        /// <summary>
        /// 一つの在庫アクションの残数では使用数を賄えないとき、同一日付の別在庫アクションからも使用数不足分を引くことができる
        /// </summary>
        [TestMethod]
        public void CanRemoveFromTwoOrMoreStockActions()
        {
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);
            var actionDate = Date.May2nd;
            var expected = new
            {
                First = new
                {
                    LotNo = ArrivedLotNumbers[Date.April30th].First(),
                    Arrived = Date.April30th,
                    Previous = actionDate.AddDays(-1),
                    Today = actionDate,
                    Next = actionDate.AddDays(1),
                    Discard = Date.April30th.AddDays(expectedPart.ExpiryDate),
                    Initial = 200,
                    Used = 200,
                    Remain = 0
                },
                Second = new
                {
                    LotNo = ArrivedLotNumbers[Date.May1st].First(),
                    Arrived = Date.May1st,
                    Previous = actionDate.AddDays(-1),
                    Today = actionDate,
                    Next = actionDate.AddDays(1),
                    Discard = Date.May1st.AddDays(expectedPart.ExpiryDate),
                    Initial = 300,
                    Used = 80,
                    Remain = 220
                },
                Third = new
                {
                    LotNo = ArrivedLotNumbers[Date.May2nd].First(),
                    Arrived = Date.May2nd,
                    Today = actionDate,
                    Next = actionDate.AddDays(1),
                    Discard = Date.May2nd.AddDays(expectedPart.ExpiryDate),
                    Initial = 200,
                    Used = 0,
                    Remain = 200
                }
            };

            var allUsed = expected.First.Used + expected.Second.Used;
            var expectedRemain = expected.First.Remain + expected.Second.Remain + expected.Third.Initial;

            var actualRemain = Model.BouquetModel.UseBouquetPart(expectedPart, actionDate, allUsed);

            Assert.AreEqual(expectedRemain, actualRemain);

            // 前々日(4/30)入荷分の在庫アクションについて、数量・残数、破棄数が正しく更新されているか
            {
                var lot = expected.First.LotNo;
                var arrived = expected.First.Arrived;
                var part = expectedPart.Code;
                var first = expected.First;

                AssertStockAction(StockActionType.SCHEDULED_TO_USE, first.Previous, arrived, part, lot, 0, first.Initial);
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, first.Today, arrived, part, lot, first.Used, first.Remain);
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, first.Next, arrived, part, lot, 0, first.Remain);
                AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, first.Discard, arrived, part, lot, first.Remain, 0);
            }

            // 前日(5/1)入荷分の在庫アクションについて、数量・残数,破棄数が正しく更新されているか
            {
                var lot = expected.Second.LotNo;
                var arrived = expected.Second.Arrived;
                var part = expectedPart.Code;
                var second = expected.Second;

                AssertStockAction(StockActionType.SCHEDULED_TO_USE, second.Previous, arrived, part, lot, 0, second.Initial);
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, second.Today, arrived, part, lot, second.Used, second.Remain);
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, second.Next, arrived, part, lot, 0, second.Remain);
                AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, second.Discard, arrived, part, lot, second.Remain, 0);
            }

            // 前日分までで使用数の確保が完了しているので当日(5/2)入荷分の在庫アクションに影響が出ていないか
            {
                var lot = expected.Third.LotNo;
                var arrived = expected.Third.Arrived;
                var part = expectedPart.Code;
                var third = expected.Third;

                AssertStockAction(StockActionType.SCHEDULED_TO_USE, third.Today, arrived, part, lot, 0, third.Initial);
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, third.Next, arrived, part, lot, 0, third.Remain);
                AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, third.Discard, arrived, part, lot, third.Remain, 0);
            }

            // 一日の在庫はトータルすれば潤沢なので、残数不足の在庫アクションは生成されない
            AssertStockActionCount(0, StockActionType.OUT_OF_STOCK);
        }

        #region 複合テスト用サポートクラス
        private class ExpectedStockAction
        {
            public StockActionType Type { get; private set; }

            private ExpectedStockAction(StockActionType t, int q, int r)
            {
                Type = t;
                Quantity = q;
                Remain = r;
            }

            public int Quantity { get; private set; }
            public int Remain { get; private set; }

            public static ExpectedStockAction CreateArrivedAction(int arrived)
            {
                return new ExpectedStockAction(StockActionType.SCHEDULED_TO_ARRIVE, arrived, arrived);
            }

            public static ExpectedStockAction CreateUsedAction(int used, int remain)
            {
                return new ExpectedStockAction(StockActionType.SCHEDULED_TO_USE, used, remain);
            }
            public static ExpectedStockAction CreateDiscardAction(int discarded)
            {
                return new ExpectedStockAction(StockActionType.SCHEDULED_TO_DISCARD, discarded, 0);
            }
        }

        #endregion // 複合テスト用サポートクラス

        [TestMethod]
        public void CompositeTestFromApril30ToMay7th()
        {
            var expectedPart = Model.BouquetModel.Find(ExpectedPartCode);

            var test = new Dictionary<DateTime, int>()
            {
                { Date.April30th, 20 },
                { Date.May1st, 50 },
                { Date.May2nd, 80 },
                { Date.May3rd, 20 },
                { Date.May4th, 400 },
                { Date.May5th, 170 },
                { Date.May6th, 40 }
            };
            var expected = new List<Tuple<DateTime, List<Tuple<DateTime, ExpectedStockAction>>>>() {
                Tuple.Create(
                    Date.April30th,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(Date.April30th, ExpectedStockAction.CreateArrivedAction(200)),
                        Tuple.Create(Date.April30th, ExpectedStockAction.CreateUsedAction(20, 180)),
                        Tuple.Create(Date.May1st, ExpectedStockAction.CreateUsedAction(50, 130)),
                        Tuple.Create(Date.May2nd, ExpectedStockAction.CreateUsedAction(80, 50)),
                        Tuple.Create(Date.May3rd, ExpectedStockAction.CreateUsedAction(20, 30)),
                        Tuple.Create(Date.May3rd, ExpectedStockAction.CreateDiscardAction(30))
                    }),
                Tuple.Create(
                    Date.May1st,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(Date.May1st, ExpectedStockAction.CreateArrivedAction(300)),
                        Tuple.Create(Date.May1st, ExpectedStockAction.CreateUsedAction(0, 300)),
                        Tuple.Create(Date.May2nd, ExpectedStockAction.CreateUsedAction(0, 300)),
                        Tuple.Create(Date.May3rd, ExpectedStockAction.CreateUsedAction(0, 300)),
                        Tuple.Create(Date.May4th, ExpectedStockAction.CreateUsedAction(300, 0)),
                        Tuple.Create(Date.May4th, ExpectedStockAction.CreateDiscardAction(0))
                    }),
                Tuple.Create(
                    Date.May2nd,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(Date.May2nd, ExpectedStockAction.CreateArrivedAction(200)  ),
                        Tuple.Create(Date.May2nd, ExpectedStockAction.CreateUsedAction(0, 200)  ),
                        Tuple.Create(Date.May3rd, ExpectedStockAction.CreateUsedAction(0, 200)  ),
                        Tuple.Create(Date.May4th, ExpectedStockAction.CreateUsedAction(100, 100)),
                        Tuple.Create(Date.May5th, ExpectedStockAction.CreateUsedAction(100, 0)  ),
                        Tuple.Create(Date.May5th, ExpectedStockAction.CreateDiscardAction(0)    )
                    }),
                Tuple.Create(
                    Date.May3rd,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(Date.May3rd, ExpectedStockAction.CreateArrivedAction(200) ),
                        Tuple.Create(Date.May3rd, ExpectedStockAction.CreateUsedAction(0, 200) ),
                        Tuple.Create(Date.May4th, ExpectedStockAction.CreateUsedAction(0, 200) ),
                        Tuple.Create(Date.May5th, ExpectedStockAction.CreateUsedAction(70, 130)),
                        Tuple.Create(Date.May6th, ExpectedStockAction.CreateUsedAction(40, 90) ),
                        Tuple.Create(Date.May6th, ExpectedStockAction.CreateDiscardAction(90)  )
                    }),
                Tuple.Create(
                    Date.May6th,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(Date.May6th, ExpectedStockAction.CreateArrivedAction(100)),
                        Tuple.Create(Date.May6th, ExpectedStockAction.CreateUsedAction(0, 100)),
                        Tuple.Create(Date.May7th, ExpectedStockAction.CreateUsedAction(0, 100)),
                        Tuple.Create(Date.May8th, ExpectedStockAction.CreateUsedAction(0, 100)),
                        Tuple.Create(Date.May9th, ExpectedStockAction.CreateUsedAction(0, 100)),
                        Tuple.Create(Date.May9th, ExpectedStockAction.CreateDiscardAction(100))
                    })
            };

            // テスト：全加工予定の登録
            foreach(var u in test)
            {
                Model.BouquetModel.UseBouquetPart(expectedPart, u.Key, u.Value);
            }

            // 検証1：全在庫アクションが意図通り変更されていること
            foreach(var e in expected)
            {
                var arrived = e.Item1;
                var lotNo = ArrivedLotNumbers[arrived].First();
                foreach(var a in e.Item2)
                {
                    var date = a.Item1;
                    var type = a.Item2.Type;
                    var quantity = a.Item2.Quantity;
                    var remain = a.Item2.Remain;
                    AssertStockAction(type, date, arrived, ExpectedPartCode, lotNo, quantity, remain);
                }
            }

            // 検証2：在庫不足が発生していないこと
            AssertStockActionCount(0, StockActionType.OUT_OF_STOCK);
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
        private void AssertAllLotNumbersAreUnique()
        {
            // Linqで全ロット番号を一つの IEnumerable に変形する手段もあるが、それだとアサーション発生したロットの入荷日がわからないので
            // 愚直に二重ループを回す
            foreach(var i in ArrivedLotNumbers)
            {
                foreach(var j in i.Value)
                {
                    var actual = TestDBContext.StockActions.Count(a => a.Action == StockActionType.SCHEDULED_TO_ARRIVE && a.StockLotNo == j);
                    var days = TestDBContext.StockActions.Where(a => a.Action == StockActionType.SCHEDULED_TO_ARRIVE && a.StockLotNo == j).Select(a => a.ArrivalDate);
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
