using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class MemorieDeFleursTestBase
    {
        private static string TestDBFile = "./testdata/db/MemorieDeFleurs.db";
        private static string EmptyDBFile = "./testdata/db/MemorieDeFleursEmpty.db";

        protected SqliteConnection TestDB { get; set; }
        protected SqliteConnection EmptyDB { get; set; }

        protected event EventHandler AfterTestBaseInitializing;
        protected event EventHandler BeforeTestBaseCleaningUp;

        protected MemorieDeFleursTestBase()
        {
            TestDB = CreateDBConnection(TestDBFile);
            EmptyDB = CreateDBConnection(EmptyDBFile);
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
        public void BaseInitialize()
        {
            LogUtil.Debug("Start: MemorieDeFleursTestBase#BaseInitialize()");
            TestDB.Open();
            EmptyDB.Open();

            AfterTestBaseInitializing?.Invoke(this, null);
            LogUtil.Debug("Done: MemorieDeFleursTestBase#BaseInitialize()");
        }

        [TestCleanup]
        public void BaseCleanup()
        {
            LogUtil.Debug("Start: MemorieDeFleursTestBase#BaseCleanup()");
            // 登録したイベントハンドラを、登録したときと逆順に呼び出す
            if (BeforeTestBaseCleaningUp != null)
            {
                // IList<>.ForEach() を使うためには IEnumerable<>.ToList<>() が必要。
                // このときデータコピーが発生するのはうれしくない。
                foreach(var handler in BeforeTestBaseCleaningUp.GetInvocationList().Reverse().Cast<EventHandler>())
                {
                    handler(this, null);
                }
            }

            TestDB.Close();
            EmptyDB.Close();
            LogUtil.Debug("Done: MemorieDeFleursTestBase#BaseCleanup()");
        }

        /// <summary>
        /// データベース内の全データを削除する。
        /// 
        /// 常時全削除、ではないので、必要なときに呼び出すこと。
        /// </summary>
        protected void ClearAll()
        {
            // 削除対象テーブル、削除順
            var tables = new List<string>()
            {
                "STOCK_ACTIONS",
                "ORDER_DETAILS_FROM_CUSTOMER",
                "ORDER_DETAILS_TO_SUPPLIER",
                "BOUQUET_PARTS_LIST",
                "BOUQUET_SUPPLIERS",
                "ORDER_FROM_CUSTOMER",
                "ORDERS_TO_SUPPLIER",
                "SHIPPING_ADDRESS",
                "CUSTOMERS",
                "SUPPLIERS",
                "BOUQUET_SET",
                "BOUQUET_PARTS",
                "SEQUENCES",
                "DATE_MASTER",
            };

            using (var cmd = TestDB.CreateCommand())
            {
                foreach (var t in tables)
                {
                    cmd.CommandText = $"delete from {t}";
                    cmd.ExecuteNonQuery();
                }
            }

        }
    }
}
