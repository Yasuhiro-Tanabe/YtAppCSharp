using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;

using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    internal class DialogViewModel : NotificationObject
    {
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
        public IDialogUser ViewModel
        {
            get { return _vm; }
            set { SetProperty(ref _vm, value); }
        }
        private IDialogUser _vm;
        #endregion // プロパティ

        #region コマンド
        public ICommand Loaded { get; } = new ReloadDetailCommand();
        public ICommand Ok { get; } = new DialogOkCommand();
        public ICommand Cancel { get; } = new DialogCancelCommand();
        #endregion // コマンド

        public void UpdateDialogParameter()
        {
            LogUtil.DEBUGLOG_BeginMethod();
            var param = new DialogParameter();
            ViewModel.FillDialogParameters(param);

            DialogTitle = param.DialogTitle;
            OkContent = param.OkContent;
            CancelContent = param.CancelContent;
            RaisePropertyChanged(nameof(ViewModel));
            ViewModel.OnDialogOpened();
            LogUtil.DEBUGLOG_EndMethod();
        }
    }
}
