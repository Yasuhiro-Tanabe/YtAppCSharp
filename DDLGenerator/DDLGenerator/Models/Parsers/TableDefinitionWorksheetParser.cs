using DDLGenerator.Models.Logging;

using SpreadsheetLight;

using System;
using System.Collections.Generic;
using System.IO;

namespace DDLGenerator.Models.Parsers
{
    /// <summary>
    /// SpreadsheetLight クラスライブラリを使ったテーブル定義書 (Excel) の解析器
    /// </summary>
    public class TableDefinitionWorksheetParser : IDDLParser
    {
        private SLDocument Document { get; set; }

        public string DataDefinitionFileName { get; set; }

        /// <summary>
        /// データベース定義書から読み取ったテーブル定義の一覧
        /// </summary>
        public IList<TableDefinition> TableDefinitions { get; } = new List<TableDefinition>();

        /// <summary>
        /// ワークシートの解析を実施する
        /// </summary>
        public void Parse()
        {
            using(Document = new SLDocument(DataDefinitionFileName))
            {
                foreach (var sheet in Document.GetWorksheetNames())
                {
                    Document.SelectWorksheet(sheet);
                    LogUtil.Debug($"Processing WorkSheet {sheet}...");

                    if (!IsTableDefinitionWorksheet()) { continue; }

                    var table = new TableDefinition()
                    {
                        TableName = GetCellValueStringOrEmpty("B2"),
                        TableComment = GetCellValueStringOrEmpty("B1")
                    };
                    LogUtil.Info($"テーブル定義読み込み中： '{table.TableName}' ({table.TableComment})...");

                    int i = GetFirstColumDefinitionPosition();

                    // テーブル定義の取得
                    while (Document.HasCellValue($"A{i}"))
                    {
                        TableColumnDefinition row = new TableColumnDefinition()
                        {
                            RowName = GetCellValueStringOrEmpty($"B{i}"),
                            RowComment = GetCellValueStringOrEmpty($"A{i}"),
                            DataType = GetCellValueStringOrEmpty($"D{i}"),
                            DataLength = GetCellValueIntegerOrZero($"E{i}"),
                            IsNotNull = Document.HasCellValue($"F{i}"),
                            IsPrimaryKey = Document.HasCellValue($"G{i}"),
                            ForeignKeyTableName = GetCellValueStringOrEmpty($"H{i}"),
                            ForeignKeyColumnName = GetCellValueStringOrEmpty($"I{i}")
                        };

                        table.Rows.Add(row);
                        LogUtil.Debug($"Row '{row.RowName}' ({row.RowComment}, type={row.DataType}) found.");

                        i++;
                    }

                    TableDefinitions.Add(table);
                    LogUtil.Debug($"Table '{table.TableName}' added to tables.");
                }
            }
            LogUtil.Info($"テーブル定義書 '{Path.GetFileName(DataDefinitionFileName)}' の解析完了");

            Document = null;
        }

        public bool IsFoundTableDefinitions()
        {
            return (TableDefinitions.Count > 0);
        }

        /// <summary>
        /// 現在選択対象のワークシートがテーブル定義のワークシートかどうかを判定する。具体的には：
        /// </summary>
        /// <returns>テーブル定義のワークシートであるとき真、そうでないとき偽</returns>
        public bool IsTableDefinitionWorksheet()
        {
            return Document.GetCellValueAsString("A1").Equals("テーブル和名")
                && Document.GetCellValueAsString("A2").Equals("テーブル名")
                && !string.IsNullOrWhiteSpace(Document.GetCellValueAsString("B2"));
        }

        /// <summary>
        /// 列定義の1行目を示す、テーブル定義ワークシート中の行番号を返す
        /// </summary>
        /// <returns></returns>
        public int GetFirstColumDefinitionPosition()
        {
            // 列定義のヘッダ行開始位置(行番号)取得
            int i = 1;
            for (; !Document.GetCellValueAsString($"A{i}").Equals("列和名", StringComparison.Ordinal); i++) { }

            // 列定義行はヘッダ行の次の行から始まる
            i++;
            return i;
        }

        /// <summary>
        /// データベース定義書(Excel)のセルから文字列を読み込む
        /// </summary>
        /// <param name="cell">セル名：A1, B3, etc.</param>
        /// <returns>セル内の文字列、またはセルが空のとき空文字列</returns>
        private string GetCellValueStringOrEmpty(string cell)
        {
            if (Document.HasCellValue(cell))
            {
                string value = Document.GetCellValueAsString(cell);
                return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// データベース定義書(Excel)のセルから整数値を読み込む
        /// </summary>
        /// <param name="cell">セル名：A1, B3, etc.</param>
        /// <returns>セル内の数値、またはセルが空のとき0</returns>
        private int GetCellValueIntegerOrZero(string cell)
        {
            if (Document.HasCellValue(cell))
            {
                return Document.GetCellValueAsInt32(cell);
            }
            else
            {
                return 0;
            }
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(DataDefinitionFileName))
            {
                throw new FileNotFoundException("Input file name is empty or white space.");
            }
            if (!File.Exists(DataDefinitionFileName))
            {
                throw new FileNotFoundException(DataDefinitionFileName);
            }
        }
    }
}
