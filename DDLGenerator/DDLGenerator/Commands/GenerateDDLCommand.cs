using DDLGenerator.Models.Logging;
using DDLGenerator.ViewModels;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace DDLGenerator.Commands
{
    class GenerateDDLCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// コマンドが割り当てられたビューモデル
        /// </summary>
        private SQLiteDDLViewModel _vm;

        public GenerateDDLCommand(SQLiteDDLViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#CanExecute() called. parameter={parameter?.GetType().Name}");
            return true;
        }

        public void Execute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");

            Task.Run(() => Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                Models.DDLGenerator.Generate(_vm.TableDefinitionFilePath, _vm.OutputDdlFilePath);
                LogUtil.Info("ファイル出力完了");
            }));
        }
    }
}
