using MemorieDeFleurs;
using MemorieDeFleurs.Logging;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class MemorieDeFleursTestBase
    {
        private static string EmptyDBFile = "./testdata/db/MemorieDeFleursEmpty.db";
        private static string TableDefinitionFile = "./testdata/db/TableDefinitions.sql";
        private IList<Tuple<string, string>> TableDefinitions = new List<Tuple<string, string>>();

        protected SqliteConnection TestDB { get; set; }
        protected SqliteConnection EmptyDB { get; private set; }
        protected SqliteConnection InMemoryDB { get; private set; }

        protected event EventHandler AfterTestBaseInitializing;
        protected event EventHandler BeforeTestBaseCleaningUp;

        protected MemorieDeFleursTestBase()
        {
            EmptyDB = CreateDBConnection(EmptyDBFile); // DBファイルを開くサンプルコード兼用
            InMemoryDB = CreateInmeoryDBConnection();
            TestDB = InMemoryDB;

            LoadTableDefinitionsFile();
        }

        private void LoadTableDefinitionsFile()
        {
            using (var stream = new StreamReader(File.OpenRead(TableDefinitionFile)))
            {
                var ddls = stream.ReadToEnd().Split(";");
                var headerComment = new Regex("--[^\n]+\n");
                var tableComment = new Regex("/\\*[^/]*\\*/");
                var spaceUntilComma = new Regex("[ \t]+,");
                var whiteSpaces = new Regex("[\\s]+");
                foreach (var ddl in ddls)
                {
                    var s1 = headerComment.Replace(ddl.Trim(), string.Empty);
                    var s2 = tableComment.Replace(s1.Trim(), string.Empty);
                    var s3 = spaceUntilComma.Replace(s2, ","); // 行頭スペースは確保したいので Trim() は行わない
                    var s4 = whiteSpaces.Replace(s3, " ");
                    if (s4.StartsWith("CREATE TABLE", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var s5 = s4.Split()[2];
                        TableDefinitions.Add(Tuple.Create(s5,s4));
                    }
                }
            }
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

        private SqliteConnection CreateInmeoryDBConnection()
        {
            var builder = new SqliteConnectionStringBuilder();

            builder.DataSource = ":memory:";
            builder.ForeignKeys = true;

            LogUtil.Debug($"CreateInmeoryDBConnection()=>DataSource={builder.ToString()}");
            return new SqliteConnection(builder.ToString());
        }

        [TestInitialize]
        public void BaseInitialize()
        {
            LogUtil.Debug("Start: MemorieDeFleursTestBase#BaseInitialize()");
            TestDB.Open();
            EmptyDB.Open();
            InMemoryDB.Open();

            foreach(var ddl in TableDefinitions)
            {
                using(var cmd = InMemoryDB.CreateCommand())
                {
                    cmd.CommandText = ddl.Item2;
                    cmd.ExecuteNonQuery();
                    LogUtil.Debug($"Table {ddl.Item1} Created.");
                }
            }

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

            InMemoryDB.Close();
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
            ClearAll(TestDB);
            ClearAll(InMemoryDB);
        }
        protected void ClearAll(SqliteConnection connection)
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

            using (var cmd = connection.CreateCommand())
            {
                foreach (var t in tables)
                {
                    cmd.CommandText = $"delete from {t}";
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// 中間状態のデータベースをファイルに保存する
        /// 
        /// デバッグ用。
        /// </summary>
        /// <param name="connection">テスト対象データベース</param>
        /// <param name="dbFileName">保存するファイル名</param>
        protected void SaveCurrentDatabaseTo(SqliteConnection connection, string dbFileName)
        {
            LogUtil.DEBUGLOG_BeginMethod(dbFileName);
            try
            {
                if (File.Exists(dbFileName))
                {
                    LogUtil.Debug($"{LogUtil.Indent}Database {dbFileName} is alerdy exists. removed.");
                    File.Delete(dbFileName);
                }

                var builder = new SqliteConnectionStringBuilder();
                builder.DataSource = dbFileName;
                builder.Mode = SqliteOpenMode.ReadWriteCreate;
                builder.ForeignKeys = true;

                var backupDb = new SqliteConnection(builder.ToString());
                connection.BackupDatabase(backupDb);
                LogUtil.Debug($"Database backuped to: {dbFileName}");

                LogUtil.DEBUGLOG_EndMethod(dbFileName, $"Saved successfully.");
            }
            catch(Exception ex)
            {
                LogUtil.DEBUGLOG_EndMethod(dbFileName, $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
