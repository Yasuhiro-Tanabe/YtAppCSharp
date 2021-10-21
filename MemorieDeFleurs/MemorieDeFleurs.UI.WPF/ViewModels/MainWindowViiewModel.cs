using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class MainWindowViiewModel : NotificationObject
    {
        #region プロパティ
        /// <summary>
        /// ウィンドウタイトル
        /// </summary>
        public string WindowTitle
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _title = "Memorie de Fleurs 花卉受発注支援システム 管理画面";

        /// <summary>
        /// システムメッセージ表示領域に表示する文言
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        private string _message;

        /// <summary>
        /// タブコントロール内に現在表示されているビューに対応するビューモデルの一覧
        /// </summary>
        public ObservableCollection<TabItemControlViewModelBase> TabItemControlCollection { get; } = new ObservableCollection<TabItemControlViewModelBase>();

        /// <summary>
        /// 現在選択中のタブアイテム
        /// </summary>
        public TabItemControlViewModelBase CurrentItem { get; set; }
        
        /// <summary>
        /// 接続中のデータベース
        /// </summary>
        public DbConnection DbConnection
        {
            get { return _conn; }
            set { SetProperty(ref _conn, value); }
        }
        private DbConnection _conn;
        #endregion // プロパティ

        #region コマンド
        public ICommand Exit { get; } = new ExitCommand();
        public ICommand OpenPartsDetailView { get; } = new OpenPartsDetailViewCommand();
        public ICommand ConnectoToSQLiteDatabase { get; } = new ConnectToSQLiteDatabaseCommand();
        #endregion // コマンド

        #region ビューの生成・切替
        public void OpenTabItem(TabItemControlViewModelBase vm)
        {
            var found = TabItemControlCollection.SingleOrDefault(item => item.Header == vm.Header);
            if(found == null)
            {
                TabItemControlCollection.Add(vm);
                found = vm;
                found.TabItemControlClosing += CloseTabItem;
            }
            CurrentItem = found;
            RaisePropertyChanged(nameof(TabItemControlCollection), nameof(CurrentItem));
        }

        public void CloseTabItem(object sender, EventArgs unused)
        {
            var vm = sender as TabItemControlViewModelBase;
            if(vm != null)
            {
                var found = TabItemControlCollection.SingleOrDefault(item => item.Header == vm.Header);
                if (found != null)
                {
                    TabItemControlCollection.Remove(found);
                }
                CurrentItem = TabItemControlCollection.FirstOrDefault();
                RaisePropertyChanged(nameof(TabItemControlCollection), nameof(CurrentItem));
                vm.TabItemControlClosing -= CloseTabItem;
            }
        }
        #endregion // ビューの生成・切替
    }
}
