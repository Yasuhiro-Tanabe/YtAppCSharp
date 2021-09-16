namespace DDLGenerator.Models
{
    /// <summary>
    /// データベーステーブルの各カラム
    /// </summary>
    public class TableColumnDefinition
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string RowName { get; set; }

        /// <summary>
        /// コメント：列和名
        /// </summary>
        public string RowComment { get; set; }

        /// <summary>
        /// データ型名称
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// データ長：<see cref="DataType"/> が文字列型の時だけ有効
        /// </summary>
        public int DataLength { get; set; }

        /// <summary>
        /// 非NULLフラグ
        /// </summary>
        public bool IsNotNull { get; set; }

        /// <summary>
        /// 主キー識別フラグ
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 外部キーテーブル名
        /// </summary>
        public string ForeignKeyTableName { get; set; }

        /// <summary>
        /// 外部キー列名
        /// </summary>
        public string ForeignKeyColumnName { get; set; }
    }
}
