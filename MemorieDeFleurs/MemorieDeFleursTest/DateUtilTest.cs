
using MemorieDeFleurs;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class DateUtilTest
    {
        private static string TestDBFile = "./testdata/db/MemorieDeFleurs.db";
        private static string EmptyDBFile = "./testdata/db/MemorieDeFleursEmpty.db";

        private SqliteConnection TestDB { get; set; }
        private SqliteConnection EmptyDB { get; set; }

        private DateUtil TestDateMaster { get; set; }
        private DateUtil EmptyDateMaster { get; set; }

        public DateUtilTest()
        {
            TestDB = CreateDBConnection(TestDBFile);
            EmptyDB = CreateDBConnection(EmptyDBFile);
            TestDateMaster = new DateUtil(TestDB);
            EmptyDateMaster = new DateUtil(EmptyDB);
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
        public void Setup()
        {
            TestDB.Open();
            EmptyDB.Open();
        }

        [TestCleanup]
        public void TearDown()
        {
            TestDateMaster.Clear();
            TestDB.Close();
            EmptyDB.Close();
        }

        [TestMethod]
        public void NoDatesInEmptyDb()
        {
            Assert.AreEqual(DateUtil.InvalidDate, EmptyDateMaster.FirstDate);
            Assert.AreEqual(DateUtil.InvalidDate, EmptyDateMaster.LastDate);
        }

        [TestMethod]
        public void AddAndRemoveDateMaster()
        {
            var startDate = 20200101;
            var endDate = 20221231;

            TestDateMaster.Fill(startDate, endDate);
            Assert.AreEqual(startDate, TestDateMaster.FirstDate);
            Assert.AreEqual(endDate, TestDateMaster.LastDate);

            TestDateMaster.Clear();
            Assert.AreEqual(DateUtil.InvalidDateIndex, TestDateMaster.FirstDate);
            Assert.AreEqual(DateUtil.InvalidDateIndex, TestDateMaster.LastDate);
        }

        [TestMethod]
        public void IsValidDate()
        {
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20200101));
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20211231));
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20990630));
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20200229)); // 閏年である
            Assert.IsTrue(EmptyDateMaster.IsValidDate(20000229)); // 閏年であり、かつ西暦年%400=0なので閏日がある
        }

        [TestMethod]
        public void IsInvalidDate()
        {
            Assert.IsFalse(EmptyDateMaster.IsValidDate(20200100));
            Assert.IsFalse(EmptyDateMaster.IsValidDate(20211301));
            Assert.IsFalse(EmptyDateMaster.IsValidDate(20231131));
            Assert.IsFalse(EmptyDateMaster.IsValidDate(20210229));// 閏年ではない
            Assert.IsFalse(EmptyDateMaster.IsValidDate(21000229));// 閏年だが閏日がない(400年に3回、西暦年%400=100,200,300の年)
        }

        [TestMethod]
        public void NextDayOf20200331Is20200401()
        {
            TestDateMaster.Fill(20200101, 20201231);

            Assert.AreEqual(20200401, TestDateMaster.Add(20200331, 1));
        }

        [TestMethod]
        public void PreviousDayOf20200401Is20200331()
        {
            TestDateMaster.Fill(20200101, 20201231);

            Assert.AreEqual(20200331, TestDateMaster.Add(20200401, -1));
        }

        public void PreviousDayIsNotRegisteredDateMaster()
        {
            var startDate = 20200101;
            var endDate = 20201231;
            TestDateMaster.Fill(startDate, endDate);

            Assert.AreEqual(DateUtil.InvalidDate, TestDateMaster.Add(startDate, -1));
        }

        public void NextDayIsNotRegisteredInDateMaster()
        {
            var startDate = 20200101;
            var endDate = 20201231;
            TestDateMaster.Fill(startDate, endDate);

            Assert.AreEqual(DateUtil.InvalidDate, TestDateMaster.Add(endDate, 1));
        }
    }
}
