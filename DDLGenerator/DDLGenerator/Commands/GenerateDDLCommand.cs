using DDLGenerator.Models.Logging;
using DDLGenerator.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DDLGenerator.Commands
{
    class GenerateDDLCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// コマンドが割り当てられたビューモデル
        /// </summary>
        private MainWindowViewModel _vm;

        public GenerateDDLCommand(MainWindowViewModel vm)
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


            Models.DDLGenerator.Generate(_vm.TableDefinitionFilePath, _vm.OutputDdlFilePath);
            LogUtil.Info("ファイル出力完了");
        }
    }
}
