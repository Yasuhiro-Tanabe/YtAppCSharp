using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

using System;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IDialogCaller"/> の実装に必要な、ダイアログを開くコマンド
    /// </summary>
    public class OpenDialogCommand : CommandBase
    {
        /// <summary>
        /// ダイアログが閉じようとしていることを通知する
        /// </summary>
        public event EventHandler<DialogClosingEventArgs> DialogClosing;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenDialogCommand() : base()
        {
            AddAction(typeof(IDialogCaller), Open);
        }

        private void Open(object parameter)
        {
            var vm = new DialogViewModel() { ViewModel = (parameter as IDialogCaller).DialogViewModel };
            vm.DialogClosing += NotifyDialogClosing;

            var dialog = new DialogWindow() { DataContext = vm };

            dialog.ShowDialog();
        }

        private void NotifyDialogClosing(object sender, DialogClosingEventArgs args)
        {
            DialogClosing?.Invoke(this, args);
        }
    }
}
