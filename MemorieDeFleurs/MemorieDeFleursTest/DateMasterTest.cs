
using MemorieDeFleurs;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SQLite;

using System;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class DateMasterTest
    {
        private static string TestDBFile = "./testdata/db/MemorieDeFleurs.db";
        private static string EmptyDBFile = "./testdata/db/MemorieDeFleursEmpty.db";

        private SQLiteConnection TestDB { get; set; }
        private SQLiteConnection EmptyDB { get; set; }

        private DateMaster TestDateMaster { get; set; }
        private DateMaster EmptyDateMaster { get; set; }

        public DateMasterTest()
        {
            TestDB = CreateDBConnection(TestDBFile);
            EmptyDB = CreateDBConnection(EmptyDBFile);
            TestDateMaster = new DateMaster(TestDB);
            EmptyDateMaster = new DateMaster(EmptyDB);
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
            Assert.AreEqual(DateMaster.InvalidDate, EmptyDateMaster.FirstDate);
            Assert.AreEqual(DateMaster.InvalidDate, EmptyDateMaster.LastDate);
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
            Assert.AreEqual(DateMaster.InvalidDateIndex, TestDateMaster.FirstDate);
            Assert.AreEqual(DateMaster.InvalidDateIndex, TestDateMaster.LastDate);
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
    }
}
