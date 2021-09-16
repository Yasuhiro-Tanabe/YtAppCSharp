using DDLGenerator.Models.Logging;
using DDLGenerator.ViewModels;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DDLGenerator.Commands
{
    class SelectOutputFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return IsExecutable;
        }

        public void Execute(object parameter)
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
                if (parameter is SQLiteDDLViewModel)
                {
                    var vm = parameter as SQLiteDDLViewModel;
                    vm.OutputDdlFilePath = dialog.FileName;

                }
                LogUtil.Info("出力ファイル:" + dialog.FileName);
            }
            else
            {
                LogUtil.Warn($"Unexpected View: {parameter.GetType().Name}");
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
