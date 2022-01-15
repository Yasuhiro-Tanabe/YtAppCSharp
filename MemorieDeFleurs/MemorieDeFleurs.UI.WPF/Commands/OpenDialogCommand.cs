using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

using System;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IDialogCaller"/> の実装に必要な、ダイアログを開くコマンド
    /// </summary>
    public class OpenDialogCommand : CommandBase<IDialogCaller>
    {
        /// <summary>
        /// ダイアログが閉じようとしていることを通知する
        /// </summary>
        public event EventHandler<DialogClosingEventArgs> DialogClosing;

        private void NotifyDialogClosing(object sender, DialogClosingEventArgs args)
        {
            DialogClosing?.Invoke(this, args);
        }

        /// <inheritdoc/>
        protected override void Execute(IDialogCaller parameter)
        {
            var vm = new DialogViewModel() { ViewModel = parameter.DialogViewModel };
            vm.DialogClosing += NotifyDialogClosing;

            var dialog = new DialogWindow() { DataContext = vm };

            dialog.ShowDialog();
        }
    }
}
