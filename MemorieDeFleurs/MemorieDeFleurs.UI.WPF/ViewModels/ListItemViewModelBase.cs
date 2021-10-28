using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
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
        /// コマンドボタンの可視性
        /// </summary>
        public Visibility ActionVisivility { get { return _isVisible ? Visibility.Visible : Visibility.Collapsed; } }
        private bool _isVisible = false;
        #endregion // プロパティ

        #region コマンド
        public ICommand Remove { get; } = new DeleteFromDatabase();
        public ICommand Detail { get; private set; }
        #endregion // コマンド

        public void ShowCommandButtons()
        {
            _isVisible = true;
            RaisePropertyChanged(nameof(ActionVisivility));
        }

        public void HideCommandButtons()
        {
            _isVisible = false;
            RaisePropertyChanged(nameof(ActionVisivility));
        }

        public void RemoveMe()
        {
            LogUtil.DEBULOG_MethodCalled();
            SelectedItemRemoving?.Invoke(this, null);
            RaisePropertyChanged(nameof(RemoveMe));
        }

        public void OpenDetailView()
        {
            LogUtil.DEBULOG_MethodCalled();
            DetailViewOpening?.Invoke(this, null);
        }

        protected void Update(string key)
        {
            _key = key;
            _isVisible = false;
            RaisePropertyChanged(nameof(Key), nameof(ActionVisivility));
        }
    }
}
