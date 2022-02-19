using YasT.Framework.Logging;
using DDLGenerator.Models.Parsers;
using DDLGenerator.Models.Writers;

using SpreadsheetLight;

using System;
using System.IO;

namespace DDLGenerator.Models
{
    /// <summary>
    /// テーブル定義スクリプト生成器：
    /// </summary>
    public class DDLGenerator
    {
        public IDDLParser Parser { get; set; }
        public IDDLWriter Writer { get; set; }

        public void GenerateDDL()
        {
            LogUtil.Debug($"GenerateDDL() called. Parser={Parser.GetType().Name}, Writer={Writer.GetType().Name}");

            if(Parser == null)
            {
                LogUtil.Error("システムエラー：Parserが指定されていない");
                return;
            }
            if(Writer == null)
            {
                LogUtil.Error("システムエラー：テーブル定義出力方法が指定されていない");
                return;
            }

            try
            {
                Parser.Validate();
                Writer.Validate();

                Parser.Parse();

                if (Parser.TableDefinitions.Count > 0)
                {
                    Writer.WriteTables(Parser.TableDefinitions);
                }
            }
            catch (FileNotFoundException ef)
            {
                // IOException のサブクラスなので先にチェックする
                LogUtil.Warn("ファイルが見つかりません：" + ef.Message);
            }
            catch (IOException eio)
            {
                LogUtil.Warn("テーブル定義スクリプトの作成に失敗しました：" + eio.Message);
            }
            catch(ApplicationException eapp)
            {
                LogUtil.Warn("テーブル定義スクリプトの作成に失敗しました：" + eapp.Message);
            }
            catch(Exception e)
            {
                LogUtil.Error($"想定外のエラー {e.GetType().Name}: {e.Message}");
            }
        }
    }
}
