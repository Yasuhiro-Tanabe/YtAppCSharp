using MemorieDeFleurs;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class EFCoreLearningTest : MemorieDeFleursDbContextTestBase
    {
        private static string ExpectedPartCode = "BA001";

        public EFCoreLearningTest() : base()
        {
            BeforeTestBaseCleaningUp += CleanupData;
        }

        void CleanupData(object sender, EventArgs unused)
        {
            ClearAll();
        }

        [TestMethod]
        public void LearnEFCoreTransaction_RollbackIsAvailable()
        {
            using (var transaction = TestDBContext.Database.BeginTransaction())
            {
                Assert.AreEqual(0, TestDBContext.StockActions.Count(), "事前検証エラー：StockActionsが空でない");
                try
                {
                    StockAction action = new StockAction()
                    {
                        ActionDate = new DateTime(2004, 03, 30),
                        Action = StockActionType.SCHEDULED_TO_ARRIVE,
                        PartsCode = ExpectedPartCode,
                        ArrivalDate = new DateTime(2004, 03, 30),
                        StockLotNo = 1,
                        Quantity = 200,
                        Remain = 200
                    };

                    TestDBContext.StockActions.Add(action);
                    TestDBContext.SaveChanges();
                    Assert.AreEqual(1, TestDBContext.StockActions.Count(), "登録されている在庫アクション数は１つのはず");
                    throw new Exception();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }

                Assert.AreEqual(0, TestDBContext.StockActions.Count(), "ロールバック後も在庫アクションが残っている");
            }
        }

        [TestMethod]
        public void LearnEFCoreTransaction_CannotUseMultipeTransactionsInSingleDbContext()
        {
            using (var tansaction1 = TestDBContext.Database.BeginTransaction())
            {
                Assert.ThrowsException<InvalidOperationException>(() => TestDBContext.Database.BeginTransaction());
            }
        }

    }
}
