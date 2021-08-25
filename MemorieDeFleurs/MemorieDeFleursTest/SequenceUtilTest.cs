using MemorieDeFleurs;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class SequenceUtilTest
    {
        private static string TestDBFile = "./testdata/db/MemorieDeFleurs.db";

        private SQLiteConnection TestDB { get; set; }

        private SequenceUtil TestSequence { get; set; }


        public SequenceUtilTest()
        {
            TestDB = CreateDBConnection(TestDBFile);
            TestSequence = new SequenceUtil(TestDB);
        }

        private SQLiteConnection CreateDBConnection(string dbFileName)
        {
            var builder = new SQLiteConnectionStringBuilder();

            builder.SetDefaults = true;
            builder.DataSource = dbFileName;
            builder.ForeignKeys = true;
            builder.ReadOnly = false;
            builder.SyncMode = SynchronizationModes.Normal;

            LogUtil.Debug($"CreateConnection({dbFileName})=>DataSource={builder.ToString()}");
            return new SQLiteConnection(builder.ToString());
        }

        [TestInitialize]
        public void SetUp()
        {
            TestDB.Open();
        }

        [TestCleanup]
        public void TearDown()
        {
            TestSequence.Clear();
            TestDB.Close();
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
