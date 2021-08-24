using DDLGenerator.Models.Logging;

using System;
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

        /// <summary>
        /// コマンドの実行可否を判定する
        /// </summary>
        /// <param name="parameter">コマンド送信元</param>
        /// <returns>コマンド実行可否：実行可能の時真</returns>
        public bool CanExecute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#CanExecute() called. parameter={parameter?.GetType().Name}");
            return true;
        }

        /// <summary>
        /// コマンドを実行する
        /// </summary>
        /// <param name="parameter">コマンド送信元</param>
        public void Execute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");
            Application.Current.Shutdown();
        }
    }
}
