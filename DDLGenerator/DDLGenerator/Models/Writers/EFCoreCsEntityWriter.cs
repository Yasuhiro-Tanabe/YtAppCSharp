using YasT.Framework.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDLGenerator.Models.Writers
{
    /// <summary>
    /// Entity Framework Core 用のエンティティクラスを出力するライター
    /// </summary>
    public class EFCoreCsEntityWriter : IDDLWriter
    {
        /// <summary>
        /// 出力先フォルダ
        /// </summary>
        public string OutputPath { get; set; }

        public enum ActionIfClassFileExists 
        {
            ForceOverwrite,
            ValidateBeforeStart,
            AbortBeforeWritingTheFile,
            SkipTheFile
        }

        public string NameSpace { get; set; } = "Sample.EFCore.Entities";

        public ActionIfClassFileExists WriterActionMode { get; set; } = ActionIfClassFileExists.ForceOverwrite;

        public void WriteTables(IList<TableDefinition> tables)
        {
            ValidateAllFilesAreNotExists(tables);
            foreach (var t in tables)
            {
                WriteEntityClassFile(t);
            }
        }

        private void ValidateAllFilesAreNotExists(IList<TableDefinition> tables)
        {
            if (WriterActionMode == ActionIfClassFileExists.ValidateBeforeStart)
            {
                foreach (var className in tables.Select(t => ConvertToUpperCammel(t.TableName)))
                {
                    if (File.Exists(Path.Combine(OutputPath, $"{className}.cs")))
                    {
                        throw new ApplicationException($"File '{className}.cs' is already exists in the folder {OutputPath}.");
                    }
                }
            }
        }

        private void WriteEntityClassFile(TableDefinition table)
        {
            var className = ConvertToUpperCammel(table.TableName);

            LogUtil.Debug($"WriteEntityClassFile(table {table.TableName} -> {className}.cs)");

            using(var file = File.OpenWrite(Path.Combine(OutputPath, $"{className}.cs")))
            using (var writer = new StreamWriter(file))
            {
                LogUtil.Debug($"Opened: {file.Name}");
                var pkCount = table.Rows.Count(r => r.IsPrimaryKey);
                var builder = new StringBuilder();
                builder
                    .AppendLine("using System.ComponentModel.DataAnnotations;")
                    .AppendLine("using System.ComponentModel.DataAnnotations.Schema;")
                    .AppendLine()
                    .AppendLine($"namespace {NameSpace}")
                    .AppendLine("{");
                
                Indent++;
                builder
                    .AppendLine($"{Indent}/// <summary>")
                    .AppendLine($"{Indent}/// {table.TableComment}")
                    .AppendLine($"{Indent}/// </summary>")
                    .AppendLine($"{Indent}[Table(\"{table.TableName}\")]")
                    .AppendLine($"{Indent}public class {ConvertToUpperCammel(table.TableName)}")
                    .AppendLine($"{Indent}{{");
                
                Indent++;
                var i = 0;
                foreach(var row in table.Rows)
                {
                    var propertyName = ConvertToUpperCammel(row.RowName);
                    if(i > 0) { builder.AppendLine(); }
                    builder
                        .AppendLine($"{Indent}/// <summary>")
                        .AppendLine($"{Indent}/// {row.RowComment}")
                        .AppendLine($"{Indent}/// </summary>");

                    if(pkCount == 1 && row.IsPrimaryKey)
                    {
                        builder.AppendLine($"{Indent}[Key,Column(\"{row.RowName}\")]");
                    }
                    else
                    {
                        builder.AppendLine($"{Indent}[Column(\"{row.RowName}\")]");
                    }

                    builder
                        .AppendLine($"{Indent}public {CovnertDataType(row.DataType)} {propertyName} {{ get; set; }}");
                    LogUtil.Debug($"Written[{i}]: '{propertyName}' <-- '{row.RowName}'");
                    i++;
                }

                Indent--;
                builder.AppendLine($"{Indent}}}");
                Indent--;
                builder.AppendLine($"{Indent}}}");

                writer.WriteLine(builder.ToString());
                writer.Flush();
            }

            LogUtil.Debug($"File '{className}.cs' created.");
            LogUtil.Info($"テーブル[{table.TableName}]からクラスファイル '{className}.cs' を生成終了");
        }

        /// <summary>
        /// アンダースコア "_" 区切りの名前を Upper Cammel Case の名前に変換する
        /// </summary>
        /// <param name="underscore_delimited_name"></param>
        /// <returns></returns>
        private string ConvertToUpperCammel(string underscore_delimited_name)
        {
            return string.Join("", underscore_delimited_name.Split("_").Select(s => s.ToUpper()[0] + s.ToLower().Substring(1)));
        }

        private IDictionary<string, string> DataTypeName = new Dictionary<string, string>()
        {
            { "INT", "int" },
            { "TEXT", "string" },
        };

        private string CovnertDataType(string dbDataType)
        {
            return DataTypeName[dbDataType];
        }

        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(OutputPath))
            {
                throw new ApplicationException("Output folder name is empty or white space.");
            }
            if(string.IsNullOrWhiteSpace(NameSpace))
            {
                throw new ApplicationException("Namespace of entity classes is not found.");
            }
        }

        private IndentString Indent { get; set; } = new IndentString();

        private class IndentString
        {
            private string filler = "    ";

            private int Depth { get; set; } = 0;

            public static IndentString operator++(IndentString indent)
            {
                indent.Depth++;
                return indent;
            }

            public static IndentString operator--(IndentString indent)
            {
                indent.Depth--;
                return indent;
            }

            public override string ToString()
            {
                return string.Join("", Enumerable.Range(0, Depth).Select(i => filler));
            }
        }
    }
}
