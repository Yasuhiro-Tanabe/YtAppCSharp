
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

        public DateMasterTest()
        {
            TestDB = CreateDBConnection(TestDBFile);
            EmptyDB = CreateDBConnection(EmptyDBFile);
        }

        private SQLiteConnection CreateDBConnection(string dbFileName)
        {
            var builder = new SQLiteConnectionStringBuilder();

            builder.SetDefaults = true;
            builder.DataSource = dbFileName;
            builder.ForeignKeys = true;
            builder.ReadOnly = false;

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
            TestDB.Close();
            EmptyDB.Close();
        }

        [TestMethod]
        public void NoDatesInEmptyDb()
        {
            var master = new DateMaster(EmptyDB);
            Assert.AreEqual(-1, master.FirstDate);
            Assert.AreEqual(-1, master.LastDate);
        }

    }
}
