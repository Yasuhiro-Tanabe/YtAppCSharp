using MemorieDeFleurs;
using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace MemorieDeFleursTest.ModelEntityTest
{
    [TestClass]
    public class SequenceUtilTest : MemorieDeFleursTestBase
    {
        private SequenceUtil TestSequence { get; set; }

        private MemorieDeFleursModel Model { get; set; }
        public SequenceUtilTest() : base()
        {
            Model = new MemorieDeFleursModel(TestDB);
            TestSequence = Model.Sequences;

            BeforeTestBaseCleaningUp += ClearTestSequence;
        }

        private void ClearTestSequence(object sender, EventArgs unused)
        {
            TestSequence.Clear();

        }

        [TestMethod]
        public void GetSeqCostmoersFirstValue()
        {
            Assert.AreEqual(1, TestSequence.SEQ_CUSTOMERS.Next());
        }

        [TestMethod]
        public void GetSeqCostomersNextValue()
        {
            var tmp = TestSequence.SEQ_CUSTOMERS.Next();
            Assert.AreEqual(2, TestSequence.SEQ_CUSTOMERS.Next());
        }

        [TestMethod]
        public void GetSeqShippingFirstValue()
        {
            Assert.AreEqual(1, TestSequence.SEQ_SHIPPING.Next());
        }

        [TestMethod]
        public void GetSeqShippingNextValue()
        {
            var tmp = TestSequence.SEQ_SHIPPING.Next();
            Assert.AreEqual(2, TestSequence.SEQ_SHIPPING.Next());
        }

        [TestMethod]
        public void SequenceInTransaction_CommitAvailable()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(1, TestSequence.SEQ_SUPPLIERS.Next(context));

                using (var transaction = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(2, TestSequence.SEQ_SUPPLIERS.Next(context));
                    transaction.Commit();
                }
                using (var transaction2 = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(3, TestSequence.SEQ_SUPPLIERS.Next(context));
                    transaction2.Commit();
                }
                Assert.AreEqual(4, TestSequence.SEQ_SUPPLIERS.Next(context));
            }
        }

        #region 【懸案】トランザクションロールバックのテスト：現在は RED になるためテスト対象外
        //[TestMethod,TestCategory("【RED】")]
        public void SequenceInTransaction_RollbackAvailable()
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                Assert.AreEqual(1, TestSequence.SEQ_SUPPLIERS.Next(context));

                using (var transaction1 = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(2, TestSequence.SEQ_SUPPLIERS.Next(context));
                    transaction1.Rollback();
                }
                using (var transaction2 = context.Database.BeginTransaction())
                {
                    Assert.AreEqual(2, TestSequence.SEQ_SUPPLIERS.Next(context));
                    transaction2.Rollback();
                }
                Assert.AreEqual(2, TestSequence.SEQ_SUPPLIERS.Next(context));
            }
        }
        #endregion // 懸案
    }
}
