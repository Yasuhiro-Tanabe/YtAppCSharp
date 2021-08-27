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
    /// 発注処理 (仕入先への注文) による在庫管理
    /// </summary>
    [TestClass]
    public class StockActionLogicTest : MemorieDeFleursDbContextTestBase
    {
        private int ExpectedSupplerCode { get; set; }
        private string ExpectedPartCode { get; set; }

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

            ExpectedSupplerCode = s.Code;
            ExpectedPartCode = p.Code;

            LogUtil.Debug($"DateMaster: from {Model.DateMaster.FirstDate} to {Model.DateMaster.LastDate}");
            Model.DateMaster.Fill(20200401, 20200531);
        }

        private void CleanupDb(object sender, EventArgs unused)
        {
            ClearAll();
            var date = new DateUtil(TestDB);
            LogUtil.Debug($"Datemaster: from {date.FirstDate} to {date.LastDate}");
        }

        /// <summary>
        /// 発注時に所定の在庫アクションが登録されるかどうかのテスト
        /// </summary>
        [TestMethod]
        public void ScheduledToAllivalStockActionShallBeCreated()
        {
            var supplier = Model.SupplierModel.Find(ExpectedSupplerCode);
            var part = Model.BouquetModel.Find(ExpectedPartCode);
            var orderDate = 20200425;
            var arrivalDate = 20200430;
            var numLot = 2;

            var orderLotNo = Model.SupplierModel.Order(orderDate, part, numLot, arrivalDate);

            AssertStockAction(StockActionType.SCHEDULED_TO_ARRIVE, arrivalDate, arrivalDate, part.Code, orderLotNo, numLot * part.QuantitiesPerLot, numLot * part.QuantitiesPerLot);

#if false

            var order = new {
                Supplier = Model.SupplierModel.Find(ExpectedSupplerCode),
                Part = Model.BouquetModel.Find(ExpectedPartCode),
                OrderDate = 20200425,
                OrderBody = new List<object> {
                    new {
                        Arrival = 20200430,
                        Lot = 2
                    },

                    new {
                        Arrival = 20200501,
                        Lot = 3
                    },

                    new {
                        Arrival = 20200502,
                        Lot = 2
                    },

                    new {
                        Arrival = 20200503,
                        Lot = 2
                    },

                    new {
                        Arrival = 20200506,
                        Lot = 1
                    },
                }
            };

            foreach(var o in order.OrderBody)
            {

            }
#endif
        }

        private void AssertStockAction(StockActionType type, int targetDate, int arrivalDate, string partCode, int lotno, int quantity, int remain)
        {
            var key = $"基準日={targetDate}, アクション={type.ToString()}, 花コード={partCode}, 在庫ロット番号={lotno}, 入荷日={arrivalDate}";

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
    }
}
