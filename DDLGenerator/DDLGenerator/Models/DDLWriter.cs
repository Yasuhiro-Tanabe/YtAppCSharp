using DDLGenerator.Models.Logging;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DDLGenerator.Models
{
    /// <summary>
    /// データ定義スクリプトへの書き込み器
    /// </summary>
    class DDLWriter : StreamWriter
    {
        public DDLWriter(Stream stream) : this(stream, Encoding.UTF8) { }
        public DDLWriter(Stream stream, Encoding encoding) : base(stream, encoding) { }

        public void WriteTables(IList<TableDefinition> tables)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var tbl in tables)
            {
                var countPK = tbl.Rows.Count(r => r.IsPrimaryKey);
                LogUtil.Info($"Processing table {tbl.TableName}...");
                LogUtil.Debug($"Table: {tbl.TableName}, NumRows={tbl.Rows.Count}, countPK={countPK}");


                WriteLine($"-- テーブル：{tbl.TableName} ({tbl.TableComment})");
                WriteLine($"DROP TABLE IF EXISTS {tbl.TableName};");
                WriteLine($"CREATE TABLE {tbl.TableName} (");

                builder.Clear();
                var numCols = 0;
                foreach (var row in tbl.Rows)
                {
                    LogUtil.Debug($"     Row: {row.RowName} ({row.RowComment})");
                    if (numCols > 0) { builder.AppendLine(","); }

                    builder.Append($"    {row.RowName.ToUpper()}");
                    var typeName = row.DataType.ToUpper();
                    if (typeName.StartsWith("CHAR") || typeName.StartsWith("VARCHAR"))
                    {
                        builder.Append($" {typeName}({row.DataLength})");
                    }
                    else
                    {
                        builder.Append($" {typeName}");
                    }

                    if (row.IsNotNull) { builder.Append(" NOT NULL"); }

                    if (countPK < 2 && row.IsPrimaryKey)
                    {
                        // プライマリキーがこのテーブルに1つしか無いときは、その列の定義中に PRIMARY KEY と記載する。
                        builder.Append(" PRIMARY KEY");
                    }

                    if (!string.IsNullOrWhiteSpace(row.ForeignKeyTableName))
                    {
                        builder.Append($" REFERENCES {row.ForeignKeyTableName}({row.ForeignKeyColumnName})");
                    }

                    builder.Append($"     /* {row.RowComment} */");
                    numCols++;
                }

                if (countPK > 2)
                {
                    // 複数列でプライマリキーを構成する場合は、各列に PRIMARY KEY をつけず列定義の後に PRIMERY KEY ( K1, K2, ... ) と記載する
                    builder.AppendLine(",");
                    builder.Append("  PRIMARY KEY (");
                    tbl.Rows
                        .Where(_r => _r.IsPrimaryKey)
                        .Select(_r => _r.RowName)
                        .ToList()
                        .ForEach(s => builder.Append($" {s},"));

                    builder.Remove(builder.Length - 1, 1);
                    builder.Append(" )");
                }

                builder.AppendLine();
                builder.AppendLine(");");


                var str = builder.ToString();
                LogUtil.Debug($"=== Created SQL ====\n{str}");
                WriteLine(str);
                WriteLine();

                Flush();
            }
        }
    }
}
