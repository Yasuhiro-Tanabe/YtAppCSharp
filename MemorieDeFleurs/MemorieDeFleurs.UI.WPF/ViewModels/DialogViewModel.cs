using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Windows.Input;

using YasT.Framework.Logging;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    internal class DialogViewModel : NotificationObject, IReloadable
    {
        public event EventHandler<DialogClosingEventArgs> DialogClosing;

        public DialogViewModel()
        {
            (Ok as DialogOkCommand).DialogClosing += DialogClosingOk;
            (Cancel as DialogCancelCommand).DialogCloing += DialogClosingCancel;
        }
        private void DialogClosingOk(object sender, EventArgs unused)
        {
            DialogClosing?.Invoke(this, new DialogClosingEventArgs(DialogClosingStatus.OK, ViewModel));
        }
        private void DialogClosingCancel(object sender, EventArgs unused)
        {
            DialogClosing?.Invoke(this, new DialogClosingEventArgs(DialogClosingStatus.CANCEL, ViewModel));
        }

        #region プロパティ
        /// <summary>
        /// ダイアログタイトル
        /// </summary>
        public string DialogTitle
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _title = "タイトル";

        /// <summary>
        /// OKボタンに表示するコンテンツ
        /// </summary>
        public object OkContent
        {
            get { return _ok; }
            set { SetProperty(ref _ok, value); }
        }
        private object _ok = "OK";

        /// <summary>
        /// CANCELボタンに表示するコンテンツ
        /// </summary>
        public object CancelContent
        {
            get { return _cancel; }
            set { SetProperty(ref _cancel, value); }
        }
        private object _cancel = "Cancel";

        /// <summary>
        /// ダイアログ中に表示するビューのビューモデル
        /// </summary>
        public NotificationObject ViewModel
        {
            get { return _vm; }
            set {
                SetProperty(ref _vm, value);
                RaisePropertyChanged(nameof(DialogUser));
            }
        }
        private NotificationObject _vm;

        /// <summary>
        /// ダイアログに関する操作を行うビューモデル：実体は <see cref="ViewModel"/> と同じオブジェクト
        /// </summary>
        public IDialogViewModel DialogUser
        {
            get { return _vm as IDialogViewModel; }
        }
        #endregion // プロパティ

        #region コマンド
        public ICommand Loaded { get; } = new ReloadCommand();
        public ICommand Ok { get; } = new DialogOkCommand();
        public ICommand Cancel { get; } = new DialogCancelCommand();
        #endregion // コマンド

        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();
                var param = new DialogParameter();
                DialogUser.FillDialogParameters(param);

                DialogTitle = param.DialogTitle;
                OkContent = param.OkContent;
                CancelContent = param.CancelContent;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }
        #endregion // IReloadable
    }
}
