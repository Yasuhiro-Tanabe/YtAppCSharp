using YasT.Framework.Logging;
using DDLGenerator.ViewModels;

using Microsoft.Win32;

using System;
using YasT.Framework.WPF;

namespace DDLGenerator.Commands
{
    public class SelectOutputFileCommand : CommandBase<SQLiteDDLViewModel>
    {
        private bool IsExecutable { get; set; } = true;

        public void OnGenerationStarted(object sender, EventArgs unused) => ToUnexecutable();

        public void OnGenerationFinished(object sender, EventArgs unused) => ToExecutable();

        protected override void Execute(SQLiteDDLViewModel parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");

            var dialog = new SaveFileDialog();
            dialog.Title = "テーブル定義スクリプトファイルの選択";
            dialog.Filter = "テーブル定義スクリプトファイル (*.sql)|*.sql";
            dialog.DefaultExt = ".sql";
            dialog.CheckPathExists = true;
            dialog.OverwritePrompt = true;
            dialog.FileName = "TableDefinitions.sql";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                parameter.OutputDdlFilePath = dialog.FileName;
                LogUtil.Info("出力ファイル:" + dialog.FileName);
            }
        }
    }
}
