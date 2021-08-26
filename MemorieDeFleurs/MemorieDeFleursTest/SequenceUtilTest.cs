using MemorieDeFleurs;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class SequenceUtilTest
    {
        private static string TestDBFile = "./testdata/db/MemorieDeFleurs.db";

        private SqliteConnection TestDB { get; set; }

        private SequenceUtil TestSequence { get; set; }


        public SequenceUtilTest()
        {
            TestDB = CreateDBConnection(TestDBFile);
            TestSequence = new SequenceUtil(TestDB);
        }

        private SqliteConnection CreateDBConnection(string dbFileName)
        {
            var builder = new SqliteConnectionStringBuilder();

            builder.DataSource = dbFileName;
            builder.ForeignKeys = true;
            builder.Mode = SqliteOpenMode.ReadWrite;

            LogUtil.Debug($"CreateConnection({dbFileName})=>DataSource={builder.ToString()}");
            return new SqliteConnection(builder.ToString());
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
