using DDLGenerator.Models.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DDLGenerator.Commands
{
    class SelectTableDefinitionFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#CanExecute() called. parameter={parameter?.GetType().Name}");
            return true;
        }

        public void Execute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");

            LogUtil.Info("入力ファイル：");
        }
    }
}
