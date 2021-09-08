using DDLGenerator.Models.Logging;
using DDLGenerator.Models.Writers;

using SpreadsheetLight;

using System.IO;

namespace DDLGenerator.Models
{
    /// <summary>
    /// テーブル定義スクリプト生成器：
    /// </summary>
    public class DDLGenerator
    {
        public static void Generate(string input, string output)
        {
            var generator = new DDLGenerator();
            generator.GenerateDDL(input, output);

        }

        public void GenerateDDL(string input, string output)
        {
            LogUtil.Debug($"GenerateDDL() called. input={input}, output={output}");
            if(string.IsNullOrWhiteSpace(input))
            {
                throw new FileNotFoundException("Input file name is empty or white space.");
            }
            if (string.IsNullOrWhiteSpace(output))
            {
                throw new FileNotFoundException("Output file name is empty or white space.");
            }

            if (!File.Exists(input))
            {
                throw new FileNotFoundException(input);
            }

            try
            {
                using (var doc = new SLDocument(input))
                {
                    var parser = new TableDefinitionWorksheetParser(doc);
                    parser.Parse();
                    LogUtil.Info($"テーブル定義書 '{Path.GetFileName(input)}' の解析完了");

                    if(parser.IsFoundTableDefinitions())
                    {
                        var writer = new DDLWriter(output);
                        writer.WriteTables(parser.TableDefinitions);
                        LogUtil.Info($"スクリプト '{Path.GetFileName(output)}' の出力完了");
                    }
                }
            }
            catch(IOException eio)
            {
                LogUtil.Warn("テーブル定義スクリプトの作成に失敗しました：" + eio.Message);
            }
        }

    }
}
