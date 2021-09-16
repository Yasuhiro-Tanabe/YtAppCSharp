using System.Collections.Generic;

namespace DDLGenerator.Models
{
    /// <summary>
    /// データベースのテーブル定義
    /// </summary>
    public class TableDefinition
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// コメント：テーブル和名
        /// </summary>
        public string TableComment { get; set; }

        /// <summary>
        /// テーブル定義書に記載されたカラム定義の一覧
        /// </summary>
        public IList<TableColumnDefinition> Rows { get; } = new List<TableColumnDefinition>();
    }
}
