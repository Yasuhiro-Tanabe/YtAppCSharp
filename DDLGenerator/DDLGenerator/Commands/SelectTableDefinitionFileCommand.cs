using YasT.Framework.Logging;
using DDLGenerator.ViewModels;

using Microsoft.Win32;

using System;
using System.Windows.Input;

namespace DDLGenerator.Commands
{
    class SelectTableDefinitionFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#CanExecute() called. parameter={parameter?.GetType().Name}");
            return IsExecutable;
        }

        public void Execute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");

            // CommonOpenFileDialog だと開くファイルの拡張子が指定できないのでだめ。
            var dialog = new OpenFileDialog();
            dialog.Title = "データベース定義書の選択";
            dialog.Filter = "Excel ファイル (*.xlsx)|*.xlsx";
            dialog.DefaultExt = ".xlsx";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;

            var result = dialog.ShowDialog();

            if(result.HasValue && result.Value)
            {
                LogUtil.Info("入力ファイル：" + dialog.FileName);
                if(parameter is SQLiteDDLViewModel)
                {
                    LogUtil.Debug($"SQLiteDDLViewModel.TableDefinitionFilePath='{dialog.FileName}'");
                    (parameter as SQLiteDDLViewModel).TableDefinitionFilePath = dialog.FileName;
                }
                else if(parameter is EFCoreEntityViewModel)
                {
                    LogUtil.Debug($"EFCoreEntityViewModel.TableDefinitionFilePath = '{dialog.FileName}'");
                    (parameter as EFCoreEntityViewModel).TableDefinitionFilePath = dialog.FileName;
                }
                else
                {
                    LogUtil.Warn($"Unexpected View: {parameter.GetType().Name}");
                }
            }
        }

        private bool IsExecutable { get; set; } = true;

        public void OnGenerationStarted(object sender, EventArgs unused)
        {
            IsExecutable = false;
            CanExecuteChanged?.Invoke(this, null);
        }

        public void OnGenerationFinished(object sender, EventArgs unused)
        {
            IsExecutable = true;
            CanExecuteChanged?.Invoke(this, null);
        }
    }
}
