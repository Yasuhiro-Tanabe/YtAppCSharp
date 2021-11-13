using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases
{
    public class ListItemViewModelBase : NotificationObject
    {
        public event EventHandler DetailViewOpening;
        public event EventHandler SelectedItemRemoving;

        protected ListItemViewModelBase(ICommand openDetailView) : base()
        {
            Detail = openDetailView;
        }

        #region プロパティ
        /// <summary>
        /// リスト内の要素を特定するためのキー文字列
        /// 
        /// 今のところログ専用
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }
        private string _key;

        /// <summary>
        /// ボタンを表示する/しているかどうか
        /// </summary>
        public bool IsActionVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }
        private bool _isVisible = false;
        #endregion // プロパティ

        #region コマンド
        public ICommand Remove { get; } = new DeleteFromDatabase();
        public ICommand Detail { get; private set; }
        #endregion // コマンド

        public void RemoveMe()
        {
            LogUtil.DEBUGLOG_MethodCalled();
            SelectedItemRemoving?.Invoke(this, null);
            RaisePropertyChanged(nameof(RemoveMe));
        }

        public void OpenDetailView()
        {
            LogUtil.DEBUGLOG_MethodCalled();
            DetailViewOpening?.Invoke(this, null);
        }

        protected void Update(string key)
        {
            Key = key;
            IsActionVisible = false;
        }
    }
}
