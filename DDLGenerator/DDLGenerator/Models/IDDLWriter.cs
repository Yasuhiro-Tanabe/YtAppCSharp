using System.Collections.Generic;

namespace DDLGenerator.Models
{
    public interface IDDLWriter
    {
        /// <summary>
        /// <see cref="IDDLParser"/> 実行結果を基にテーブル定義ファイルを出力する。
        /// </summary>
        /// <param name="tables">テーブル一覧：<see cref="IDDLParser"/>の解析結果</param>
        public void WriteTables(IList<TableDefinition> tables);

        /// <summary>
        /// <see cref="IDDLParser"/> 呼出前に、ライターに必要な情報が登録されているかどうかを検証する。
        /// 
        /// 不足な情報がある等検証エラーは、何らかの例外としてスローする。
        /// </summary>
        public void Validate();

    }
}
