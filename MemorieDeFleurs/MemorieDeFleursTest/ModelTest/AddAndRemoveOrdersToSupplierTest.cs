using MemorieDeFleurs;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelTest
{
    /// <summary>
    /// 追加発注および発注取消に関するテスト
    /// </summary>
    [TestClass]
    public class AddAndRemoveOrdersToSupplierTest : MemorieDeFleursDbContextTestBase
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


        public AddAndRemoveOrdersToSupplierTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;

        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDBContext);

            PrepareSuppliers();
            PrepareBouquetParts();
            PrepareInitialOrders();
            PrepareInitialUsed();
        }

        private void PrepareSuppliers()
        {
            ExpectedSupplier = Model.SupplierModel.Entity<Supplier>()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .Create();
        }

        private void PrepareBouquetParts()
        {
            ExpectedPart = Model.BouquetModel.Entity<BouquetModel>()
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
                Tuple.Create(Date.April30th, 20),
                Tuple.Create(Date.May1st, 50),
                Tuple.Create(Date.May2nd, 80),
                Tuple.Create(Date.May3rd, 20),
                Tuple.Create(Date.May4th, 400),
                Tuple.Create(Date.May5th, 170),
                Tuple.Create(Date.May6th, 40)
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
                    Tuple.Create(Date.April30th, 2),
                    Tuple.Create(Date.May1st, 3),
                    Tuple.Create(Date.May2nd, 2),
                    Tuple.Create(Date.May3rd, 2),
                    Tuple.Create(Date.May6th, 1)
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

        #region テストデータ生成参照用のサポートクラス
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

        private class OrderInfo
        {
            public OrderInfo(int lotNo, int initial)
            {
                LotNo = lotNo;
                InitialQuantity = initial;
            }

            public int LotNo { get; private set; }
            public int InitialQuantity { get; private set; }

            public static OrderInfo Create(int lotNo, int initial)
            {
                return new OrderInfo(lotNo, initial);
            }

        }
        private class TestOrder : SortedDictionary<DateTime, IList<OrderInfo>>
        {
            public void Append(DateTime date, int lotNo, int initial)
            {
                IList<OrderInfo> list;
                if (!TryGetValue(date, out list))
                {
                    list = new List<OrderInfo>();
                    Add(date, list);
                }

                var item = OrderInfo.Create(lotNo, initial);
                list.Add(item);
            }

            public void Remove(int lotNo)
            {
                foreach (var i in Values)
                {
                    var found = i.SingleOrDefault(j => j.LotNo == lotNo);
                    if (found != null)
                    {
                        i.Remove(found);
                        return;
                    }
                }
            }
        }
        #endregion // テストデータ生成参照用のサポートクラス

        #region テスト実行用のサポートクラス
        private class ExpectedStockAction
        {
            public StockActionType Type { get; private set; }
            public int Quantity { get; private set; }
            public int Remain { get; private set; }


            private ExpectedStockAction(StockActionType t, int q, int r)
            {
                Type = t;
                Quantity = q;
                Remain = r;
            }

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

            /// <summary>
            /// 特定の１在庫アクションが、数量や残数も含めすべて意図通り登録されているかどうかを検証する
            /// </summary>
            /// <param name="context">データベースコンテキスト</param>
            /// <param name="actionDate">基準日</param>
            /// <param name="part">花コード</param>
            /// <param name="lot">在庫ロット番号</param>
            /// <param name="arrivedDate">入荷(予定)日</param>
            public void AssertExists(MemorieDeFleursDbContext context, DateTime actionDate, string part, int lot, DateTime arrivedDate)
            {
                var key = new StringBuilder()
                    .AppendFormat("基準日={0:yyyyMMdd}", actionDate)
                    .Append(", アクション=").Append(Type)
                    .Append(", 花コード=").Append(part)
                    .Append(", 在庫ロット番号=").Append(lot)
                    .AppendFormat(", 入荷日={0:yyyyMMdd}", arrivedDate)
                    .ToString();

                var candidate = context.StockActions
                    .Where(a => a.Action == Type)
                    .Where(a => a.ActionDate == actionDate)
                    .Where(a => a.PartsCode == part)
                    .Where(a => a.StockLotNo == lot)
                    .Where(a => a.ArrivalDate == arrivedDate);

                Assert.IsNotNull(candidate, "抽出結果が null：" + key);
                Assert.AreNotEqual(0, candidate.Count(), "該当するアクションが0件：" + key);
                Assert.AreEqual(1, candidate.Count(), $"該当するアクションが {candidate.Count()} 個ある：" + key);

                var action = candidate.SingleOrDefault();

                Assert.AreEqual(Quantity, action.Quantity, "数量不一致：" + key);
                Assert.AreEqual(Remain, action.Remain, "残数不一致：" + key);
            }
        }

        private class ActionDateStockActionValidator : List<ExpectedStockAction>
        {
            public LotStockActionValidator Parent { get; private set; }

            public ActionDateStockActionValidator(LotStockActionValidator p)
            {
                Parent = p;
            }

            public ActionDateStockActionValidator Arrived(int arrived)
            {
                Add(ExpectedStockAction.CreateArrivedAction(arrived));
                return this;
            }

            public ActionDateStockActionValidator Used(int used, int remain)
            {
                Add(ExpectedStockAction.CreateUsedAction(used, remain));
                return this;
            }
            public ActionDateStockActionValidator Discarded(int discarded)
            {
                Add(ExpectedStockAction.CreateDiscardAction(discarded));
                return this;
            }

            public LotStockActionValidator Lot(DateTime arrivedDate, int lotNo)
            {
                return Parent.Lot(arrivedDate, lotNo);
            }

            public LotStockActionValidator Lot(DateTime arrivedDate, Func<DateTime, int> findLotNumber)
            {
                return Parent.Lot(arrivedDate, findLotNumber);
            }
            public ActionDateStockActionValidator At(DateTime actionDate)
            {
                return Parent.At(actionDate);
            }
            public void AssertAll(MemorieDeFleursDbContext context)
            {
                Parent.AssertAll(context);
            }

            public void AssertAll(MemorieDeFleursDbContext context, string partsCode, DateTime arrivedDate, int lotNo, DateTime actionDate)
            {
                ForEach(a => a.AssertExists(context, actionDate, partsCode, lotNo, arrivedDate));
            }
        }
        private class  LotStockActionValidator : Dictionary<DateTime, ActionDateStockActionValidator>
        {
            public PartStockActionValidator Parent { get; private set; }

            public LotStockActionValidator(PartStockActionValidator p)
            {
                Parent = p;
            }
            public DateTime CurrentActionDate { get; private set; }

            public ActionDateStockActionValidator At(DateTime actionDate)
            {
                ActionDateStockActionValidator validator;
                if(!TryGetValue(actionDate, out validator))
                {
                    validator = new ActionDateStockActionValidator(this);
                    Add(actionDate, validator);
                }

                CurrentActionDate = actionDate;
                return validator;
            }

            public LotStockActionValidator Lot(DateTime arrivedDate, int lotNo)
            {
                return Parent.Lot(arrivedDate, lotNo);
            }
            public LotStockActionValidator Lot(DateTime arrivedDate, Func<DateTime, int> findLotNumber)
            {
                return Parent.Lot(arrivedDate, findLotNumber);
            }
            public void AssertAll(MemorieDeFleursDbContext context)
            {
                Parent.AssertAll(context);
            }

            public void AssertAll(MemorieDeFleursDbContext context, string partsCode, DateTime arrivedDate, int lotNo)
            {
                this.All(kv => { kv.Value.AssertAll(context, partsCode, arrivedDate, lotNo, kv.Key); return true; });
            }

        }

        private class PartStockActionValidator : Dictionary<Tuple<DateTime,int>, LotStockActionValidator> {
            public StockActionsValidator Parent { get; private set; }

            public PartStockActionValidator(StockActionsValidator p)
            {
                Parent = p;
            }

            public LotStockActionValidator Lot(DateTime arrivedDate, int lotNo)
            {
                LotStockActionValidator validator;
                var key = Tuple.Create(arrivedDate, lotNo);
                if (!TryGetValue(key, out validator))
                {
                    validator = new LotStockActionValidator(this);
                    Add(key, validator);
                }

                return validator;
            }

            public LotStockActionValidator Lot(DateTime arrivalDate, Func<DateTime,int> findLotNumber)
            {
                return Lot(arrivalDate, findLotNumber(arrivalDate));
            }

            public void AssertAll(MemorieDeFleursDbContext context)
            {
                Parent.AssertAll(context);
            }

            public void AssertAll(MemorieDeFleursDbContext context, string partsCode)
            {
                this.All(kv => { kv.Value.AssertAll(context, partsCode, kv.Key.Item1, kv.Key.Item2); return true; });
            }
        }

        private class StockActionsValidator : Dictionary<string, PartStockActionValidator>
        {
            private StockActionsValidator() { }

            public static StockActionsValidator NewInstance()
            {
                return new StockActionsValidator();
            }

            public BouquetPart CurrentPart { get; private set; }

            #region フルーエントインタフェース
            public PartStockActionValidator BouquetPart(BouquetPart part)
            {
                PartStockActionValidator validator;
                if(!TryGetValue(part.Code, out validator))
                {
                    validator = new PartStockActionValidator(this);
                    Add(part.Code, validator);
                }
                CurrentPart = part;
                return validator;

            }

            public void AssertAll(MemorieDeFleursDbContext context)
            {
                this.All(kv => { kv.Value.AssertAll(context, kv.Key); return true; });
            }
            #endregion // フルーエントインタフェース
        }
        #endregion テスト実行用のサポートクラス

        private int FindLotNumber(DateTime arrived, int index = 0)
        {
            return InitialOrders[arrived][index].LotNo;
        }

        /// <summary>
        /// 5/4納品予定分追加発注の検証：当日以降の加工予定と入荷予定に影響を与えないこと
        /// </summary>
        [TestMethod]
        public void NewOrderWhichArrivedAt20200504()
        {
            LogUtil.Debug("=====NewOrderWhichArrivedAt20200504 [Begin]=====");
            var newLotNo = Model.SupplierModel.Order(Date.April30th, ExpectedPart, 1, Date.May4th);
            var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart)
                .Lot(Date.May1st, findLotNo)
                    .At(Date.May4th).Used(300, 0).Discarded(0)
                .Lot(Date.May2nd, findLotNo)
                    .At(Date.May4th).Used(100, 100)
                    .At(Date.May5th).Used(100, 0).Discarded(0)
                .Lot(Date.May3rd, findLotNo)
                    .At(Date.May4th).Used(0, 200)
                    .At(Date.May5th).Used(70, 130)
                    .At(Date.May6th).Used(40, 90).Discarded(90)
                .Lot(Date.May6th, findLotNo)
                    .At(Date.May6th).Arrived(100).Used(0, 100)
                    .At(Date.May7th).Used(0, 100)
                    .At(Date.May8th).Used(0, 100)
                    .At(Date.May9th).Used(0, 100).Discarded(100)
                .Lot(Date.May4th, newLotNo)
                    .At(Date.May4th).Arrived(100).Used(0, 100)
                    .At(Date.May5th).Used(0, 100)
                    .At(Date.May6th).Used(0, 100)
                    .At(Date.May7th).Used(0, 100).Discarded(100)
                .AssertAll(TestDBContext);
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
            var newLotNo = Model.SupplierModel.Order(Date.April30th, ExpectedPart, 1, Date.May2nd);
            var findLotNo = new Func<DateTime, int>(d => InitialOrders[d][0].LotNo);

            StockActionsValidator.NewInstance().BouquetPart(ExpectedPart)
                .Lot(Date.April30th, findLotNo)
                    .At(Date.May2nd).Used(80, 50)
                    .At(Date.May3rd).Used(20, 30).Discarded(30)
                .Lot(Date.May1st, findLotNo)
                    .At(Date.May2nd).Used(0, 300)
                    .At(Date.May3rd).Used(0, 300)
                    .At(Date.May4th).Used(300, 0).Discarded(0)
                .Lot(Date.May2nd, findLotNo)
                    .At(Date.May2nd).Arrived(200).Used(0, 200)
                    .At(Date.May3rd).Used(0, 200)
                    .At(Date.May4th).Used(100, 100)
                    .At(Date.May5th).Used(100, 0).Discarded(0)
                .Lot(Date.May2nd, newLotNo)
                    .At(Date.May2nd).Arrived(100).Used(0, 100)
                    .At(Date.May3rd).Used(0, 100)
                    .At(Date.May4th).Used(0, 100)
                    .At(Date.May5th).Used(70, 30).Discarded(30) // 0本→70本使用
                .Lot(Date.May3rd, findLotNo)
                    .At(Date.May3rd).Arrived(200).Used(0, 200)
                    .At(Date.May4th).Used(0, 200)
                    .At(Date.May5th).Used(0, 200)  // 70本使用→0本
                    .At(Date.May6th).Used(40, 160).Discarded(160)
                .AssertAll(TestDBContext);
            LogUtil.Debug("=====NewOrderWhichArrivedAt20200502 [End]=====");
        }
    }
}
