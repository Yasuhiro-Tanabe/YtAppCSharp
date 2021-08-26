using MemorieDeFleurs;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class SequenceUtilTest : MemorieDeFleursTestBase
    {
        private SequenceUtil TestSequence { get; set; }


        public SequenceUtilTest() : base()
        {
            TestSequence = new SequenceUtil(TestDB);

            BeforeTestBaseCleaningUp += ClearTestSequence;
        }

        private void ClearTestSequence(object sender, EventArgs unused)
        {
            TestSequence.Clear();

        }

        [TestMethod]
        public void GetSeqCostmoersFirstValue()
        {
            Assert.AreEqual(1, TestSequence.SEQ_CUSTOMERS.Next);
        }

        [TestMethod]
        public void GetSeqCostomersNextValue()
        {
            var tmp = TestSequence.SEQ_CUSTOMERS.Next;
            Assert.AreEqual(2, TestSequence.SEQ_CUSTOMERS.Next);
        }

        [TestMethod]
        public void GetSeqShippingFirstValue()
        {
            Assert.AreEqual(1, TestSequence.SEQ_SHIPPING.Next);
        }

        [TestMethod]
        public void GetSeqShippingNextValue()
        {
            var tmp = TestSequence.SEQ_SHIPPING.Next;
            Assert.AreEqual(2, TestSequence.SEQ_SHIPPING.Next);
        }
    }
}
