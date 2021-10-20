using Microsoft.Data.Sqlite;

using System;
using System.Data.Common;
using System.IO;
using System.Text.RegularExpressions;

namespace MemorieDeFleurs.Database.SQLite
{
    /// <summary>
    /// 本番用のDB操作インタフェース
    /// 
    /// データベースとの依存関係は極力ユーザインタフェースから切り離したい
    /// </summary>
    public class MemorieDeFleursDatabseFacade
    {
        /// <summary>
        /// データベース接続オブジェクトを生成する
        /// </summary>
        /// <param name="dbFileName">データベースファイル名：ファイルが見つからないときは新規作成する</param>
        /// <returns></returns>
        public static DbConnection OpenDatabase(string dbFileName)
        {
            var builder = new SqliteConnectionStringBuilder();
            var isNewDB = !File.Exists(dbFileName);

            builder.DataSource = dbFileName;
            builder.ForeignKeys = true;
            builder.Mode = SqliteOpenMode.ReadWriteCreate;

            var conn = new SqliteConnection(builder.ToString());
            conn.Open();

            if(isNewDB)
            {
                var ddlFileName = "./Database/SQLite/TableDefinitions.sql";
                using (var stream = new StreamReader(File.OpenRead(ddlFileName)))
                using (var cmd = conn.CreateCommand())
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
                            cmd.CommandText = s4;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            return conn;
        }
    }
}
