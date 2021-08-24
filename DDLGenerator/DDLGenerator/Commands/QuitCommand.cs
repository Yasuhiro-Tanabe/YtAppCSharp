using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DDLGenerator.Commands
{
    /// <summary>
    /// 終了コマンド：アプリケーションを終了する
    /// </summary>
    public class QuitCommand : ICommand

    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
