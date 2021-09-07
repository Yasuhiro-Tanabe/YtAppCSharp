using MemorieDeFleurs.Models;
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

            //入荷予定と破棄予定の各在庫アクションが正しい日付、登録数、破棄数で登録されている
            AssertStockAction(StockActionType.SCHEDULED_TO_ARRIVE, arrivalDate, arrivalDate, ExpectedPart.Code, expectedLotNumber, expectedQuantity, expectedQuantity);
            AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, discardDate, arrivalDate, ExpectedPart.Code, expectedLotNumber, expectedQuantity, 0);
            foreach(var d in Enumerable.Range(0, ExpectedPart.ExpiryDate).Select(i => arrivalDate.AddDays(i)))
            {
                // 入荷予定日当日～破棄予定日当日 の間の使用予定在庫アクションが登録されている
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, d, arrivalDate, ExpectedPart.Code, expectedLotNumber, 0, expectedQuantity);
            }
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
            var expected = new {
                LotNo = InitialOrders[DateConst.April30th][0].LotNo,
                Arrival = DateConst.April30th,
                PreviousDay = DateConst.April30th,
                Today = DateConst.May1st,
                NextDay = DateConst.May2nd,
                InitialQuantity = InitialOrders[DateConst.April30th][0].InitialQuantity,
                Used = 60,
                Remain = InitialOrders[DateConst.April30th][0].InitialQuantity - 60

            };
            var another = new
            {
                LotNo = InitialOrders[DateConst.May1st][0].LotNo,
                Arrival = DateConst.May1st,
                Today = DateConst.May1st,
                NextDay = DateConst.May2nd,
                InitialQuantity = InitialOrders[DateConst.May1st][0].InitialQuantity,
                Used = 0,
                Remain = InitialOrders[DateConst.May1st][0].InitialQuantity
            };

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, expected.Today, expected.Used);

            Assert.AreEqual(expected.Remain + another.Remain, actualRemain);

            // 引当てされた同一ロットに属する前日、当日、翌日の在庫アクションの数量/残数が意図通りに変化している
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expected.PreviousDay, expected.Arrival, ExpectedPart.Code, expected.LotNo, 0, expected.InitialQuantity);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expected.Today, expected.Arrival, ExpectedPart.Code, expected.LotNo, expected.Used, expected.Remain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expected.NextDay, expected.Arrival, ExpectedPart.Code, expected.LotNo, 0, expected.Remain);

            // 引当てされたのとは別ロットに影響が出ていない
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, another.Today, another.Arrival, ExpectedPart.Code, another.LotNo, 0, another.Remain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, another.NextDay, another.Arrival, ExpectedPart.Code, another.LotNo, 0, another.Remain);
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
            var expectedUsedDate = DateConst.April30th;
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = InitialOrders[expectedArrivalDate][0].LotNo;
            var quantity = 20;
            var expectedRemain = 180;

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, expectedUsedDate, quantity);

            // 加工当日分残数は正しいか
            Assert.AreEqual(expectedRemain, actualRemain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expectedUsedDate, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, quantity, expectedRemain);

            // 翌日から破棄予定日までの残数に反映されているか (翌日からなので列挙の要素数は「品質維持可能日数-1」個)
            foreach(var day in Enumerable.Range(1, ExpectedPart.ExpiryDate-1).Select(i => expectedArrivalDate.AddDays(i)))
            {
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, day, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, 0, expectedRemain);
            }

            // 破棄された数はあっているか
            var discardDate = expectedArrivalDate.AddDays(ExpectedPart.ExpiryDate);
            AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, discardDate, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, expectedRemain, 0);
        }

        [TestMethod]
        public void AllStocksInTheDayIsUsed()
        {
            var expectedUsedDate = DateConst.April30th;
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = InitialOrders[expectedArrivalDate][0].LotNo;
            var quantity = 200;
            var expectedRemain = 0;

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, expectedUsedDate, quantity);

            // 加工当日分残数は正しいか
            Assert.AreEqual(expectedRemain, actualRemain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expectedUsedDate, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, quantity, expectedRemain);

            // 翌日から破棄予定日までの残数に反映されているか (翌日からなので列挙の要素数は「品質維持可能日数-1」個)
            foreach (var day in Enumerable.Range(1, ExpectedPart.ExpiryDate - 1).Select(i => expectedArrivalDate.AddDays(i)))
            {
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, day, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, 0, expectedRemain);
            }

            // 破棄された数はあっているか
            var discardDate = expectedArrivalDate.AddDays(ExpectedPart.ExpiryDate);
            AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, discardDate, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, expectedRemain, 0);
        }

        [TestMethod]
        public void NotEnoughStocksInTheDay_andOutofStockRecordGenerated()
        {
            var expectedUsedDate = DateConst.April30th;
            var expectedArrivalDate = expectedUsedDate;
            var expectedLotNumber = InitialOrders[expectedArrivalDate][0].LotNo;
            var quantity = 220;
            var expectedUsedQuantity = 200;
            var expectedOutOfStock = 20;

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, expectedUsedDate, quantity);

            // 加工当日分残数は正しいか
            Assert.AreEqual(-expectedOutOfStock, actualRemain);
            AssertStockAction(StockActionType.SCHEDULED_TO_USE, expectedUsedDate, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, expectedUsedQuantity, 0);

            // 翌日から破棄予定日までの残数に反映されているか (翌日からなので列挙の要素数は「品質維持可能日数-1」個)
            foreach (var day in Enumerable.Range(1, ExpectedPart.ExpiryDate - 1).Select(i => expectedArrivalDate.AddDays(i)))
            {
                AssertStockAction(StockActionType.SCHEDULED_TO_USE, day, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, 0, 0);
            }

            // 破棄された数はあっているか
            var discardDate = expectedArrivalDate.AddDays(ExpectedPart.ExpiryDate);
            AssertStockAction(StockActionType.SCHEDULED_TO_DISCARD, discardDate, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, 0, 0);

            // 在庫不足の在庫アクションが登録されているか
            AssertStockAction(StockActionType.OUT_OF_STOCK, expectedArrivalDate, expectedArrivalDate, ExpectedPart.Code, expectedLotNumber, expectedOutOfStock, -expectedOutOfStock);

        }

        /// <summary>
        /// 一つの在庫アクションの残数では使用数を賄えないとき、同一日付の別在庫アクションからも使用数不足分を引くことができる
        /// </summary>
        [TestMethod]
        public void CanRemoveFromTwoOrMoreStockActions()
        {
            var actionDate = DateConst.May2nd;
            var expected = new
            {
                First = new
                {
                    LotNo = InitialOrders[DateConst.April30th][0].LotNo,
                    Arrived = DateConst.April30th,
                    Previous = actionDate.AddDays(-1),
                    Today = actionDate,
                    Next = actionDate.AddDays(1),
                    Discard = DateConst.April30th.AddDays(ExpectedPart.ExpiryDate),
                    Initial = 200,
                    Used = 200,
                    Remain = 0
                },
                Second = new
                {
                    LotNo = InitialOrders[DateConst.May1st][0].LotNo,
                    Arrived = DateConst.May1st,
                    Previous = actionDate.AddDays(-1),
                    Today = actionDate,
                    Next = actionDate.AddDays(1),
                    Discard = DateConst.May1st.AddDays(ExpectedPart.ExpiryDate),
                    Initial = 300,
                    Used = 80,
                    Remain = 220
                },
                Third = new
                {
                    LotNo = InitialOrders[DateConst.May2nd][0].LotNo,
                    Arrived = DateConst.May2nd,
                    Today = actionDate,
                    Next = actionDate.AddDays(1),
                    Discard = DateConst.May2nd.AddDays(ExpectedPart.ExpiryDate),
                    Initial = 200,
                    Used = 0,
                    Remain = 200
                }
            };

            var allUsed = expected.First.Used + expected.Second.Used;
            var expectedRemain = expected.First.Remain + expected.Second.Remain + expected.Third.Initial;

            var actualRemain = Model.BouquetModel.UseBouquetPart(ExpectedPart, actionDate, allUsed);

            Assert.AreEqual(expectedRemain, actualRemain);

            // 前々日(4/30)入荷分の在庫アクションについて、数量・残数、破棄数が正しく更新されているか
            {
                var lot = expected.First.LotNo;
                var arrived = expected.First.Arrived;
                var part = ExpectedPart.Code;
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
                var part = ExpectedPart.Code;
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
                var part = ExpectedPart.Code;
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
            var expectedPart = Model.BouquetModel.Find(ExpectedPart.Code);

            var test = new Dictionary<DateTime, int>()
            {
                { DateConst.April30th, 20 },
                { DateConst.May1st, 50 },
                { DateConst.May2nd, 80 },
                { DateConst.May3rd, 20 },
                { DateConst.May4th, 400 },
                { DateConst.May5th, 170 },
                { DateConst.May6th, 40 }
            };
            var expected = new List<Tuple<DateTime, List<Tuple<DateTime, ExpectedStockAction>>>>() {
                Tuple.Create(
                    DateConst.April30th,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(DateConst.April30th, ExpectedStockAction.CreateArrivedAction(200)),
                        Tuple.Create(DateConst.April30th, ExpectedStockAction.CreateUsedAction(20, 180)),
                        Tuple.Create(DateConst.May1st, ExpectedStockAction.CreateUsedAction(50, 130)),
                        Tuple.Create(DateConst.May2nd, ExpectedStockAction.CreateUsedAction(80, 50)),
                        Tuple.Create(DateConst.May3rd, ExpectedStockAction.CreateUsedAction(20, 30)),
                        Tuple.Create(DateConst.May3rd, ExpectedStockAction.CreateDiscardAction(30))
                    }),
                Tuple.Create(
                    DateConst.May1st,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(DateConst.May1st, ExpectedStockAction.CreateArrivedAction(300)),
                        Tuple.Create(DateConst.May1st, ExpectedStockAction.CreateUsedAction(0, 300)),
                        Tuple.Create(DateConst.May2nd, ExpectedStockAction.CreateUsedAction(0, 300)),
                        Tuple.Create(DateConst.May3rd, ExpectedStockAction.CreateUsedAction(0, 300)),
                        Tuple.Create(DateConst.May4th, ExpectedStockAction.CreateUsedAction(300, 0)),
                        Tuple.Create(DateConst.May4th, ExpectedStockAction.CreateDiscardAction(0))
                    }),
                Tuple.Create(
                    DateConst.May2nd,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(DateConst.May2nd, ExpectedStockAction.CreateArrivedAction(200)  ),
                        Tuple.Create(DateConst.May2nd, ExpectedStockAction.CreateUsedAction(0, 200)  ),
                        Tuple.Create(DateConst.May3rd, ExpectedStockAction.CreateUsedAction(0, 200)  ),
                        Tuple.Create(DateConst.May4th, ExpectedStockAction.CreateUsedAction(100, 100)),
                        Tuple.Create(DateConst.May5th, ExpectedStockAction.CreateUsedAction(100, 0)  ),
                        Tuple.Create(DateConst.May5th, ExpectedStockAction.CreateDiscardAction(0)    )
                    }),
                Tuple.Create(
                    DateConst.May3rd,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(DateConst.May3rd, ExpectedStockAction.CreateArrivedAction(200) ),
                        Tuple.Create(DateConst.May3rd, ExpectedStockAction.CreateUsedAction(0, 200) ),
                        Tuple.Create(DateConst.May4th, ExpectedStockAction.CreateUsedAction(0, 200) ),
                        Tuple.Create(DateConst.May5th, ExpectedStockAction.CreateUsedAction(70, 130)),
                        Tuple.Create(DateConst.May6th, ExpectedStockAction.CreateUsedAction(40, 90) ),
                        Tuple.Create(DateConst.May6th, ExpectedStockAction.CreateDiscardAction(90)  )
                    }),
                Tuple.Create(
                    DateConst.May6th,
                    new List<Tuple<DateTime,ExpectedStockAction>>()
                    {
                        Tuple.Create(DateConst.May6th, ExpectedStockAction.CreateArrivedAction(100)),
                        Tuple.Create(DateConst.May6th, ExpectedStockAction.CreateUsedAction(0, 100)),
                        Tuple.Create(DateConst.May7th, ExpectedStockAction.CreateUsedAction(0, 100)),
                        Tuple.Create(DateConst.May8th, ExpectedStockAction.CreateUsedAction(0, 100)),
                        Tuple.Create(DateConst.May9th, ExpectedStockAction.CreateUsedAction(0, 100)),
                        Tuple.Create(DateConst.May9th, ExpectedStockAction.CreateDiscardAction(100))
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
                var lotNo = InitialOrders[arrived][0].LotNo;
                foreach(var a in e.Item2)
                {
                    var date = a.Item1;
                    var type = a.Item2.Type;
                    var quantity = a.Item2.Quantity;
                    var remain = a.Item2.Remain;
                    AssertStockAction(type, date, arrived, ExpectedPart.Code, lotNo, quantity, remain);
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
