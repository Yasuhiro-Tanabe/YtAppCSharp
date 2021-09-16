using System.Collections.Generic;

namespace DDLGenerator.Models
{
    public interface IDDLParser
    {
        /// <summary>
        /// 実装クラスのプロパティで指定されたテーブル定義ファイルを読み取り、テーブル定義一覧を構築する。
        /// </summary>
        public void Parse();

        /// <summary>
        /// <see cref="IDDLParser"/> 呼出前に、パーサーに必要な情報が登録されているかどうかを検証する。
        /// 
        /// 不足な情報がある等検証エラーは、何らかの例外としてスローする。
        /// </summary>
        public void Validate();

        /// <summary>
        /// テーブル定義一覧
        /// </summary>
        public IList<TableDefinition> TableDefinitions { get; }

    }
}
