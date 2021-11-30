using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// メイン画面のビューモデル
    /// </summary>
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
        public string Message { get { return LogUtil.Appender?.Notification; } }

        /// <summary>
        /// タブコントロール内に現在表示されているビューに対応するビューモデルの一覧
        /// </summary>
        public ObservableCollection<TabItemControlViewModelBase> TabItemControlCollection { get; } = new ObservableCollection<TabItemControlViewModelBase>();

        /// <summary>
        /// 現在選択中のタブアイテム
        /// </summary>
        public TabItemControlViewModelBase CurrentItem
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }
        private TabItemControlViewModelBase _item;
        #endregion // プロパティ

        #region コマンド
        /// <summary>
        /// 終了メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand Exit { get; } = new ExitCommand();
        /// <summary>
        /// 単品登録メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenPartsDetailView { get; } = new OpenDetailViewCommand<BouquetPartsDetailViewModel>();
        /// <summary>
        /// 単品一覧メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenPartsListView { get; } = new OpenListViewCommand<BouquetPartsListViewModel>();
        /// <summary>
        /// 商品登録メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenBouquetDetailView { get; } = new OpenDetailViewCommand<BouquetDetailViewModel>();
        /// <summary>
        /// 商品一覧メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenBouquetListView { get; } = new OpenListViewCommand<BouquetListViewModel>();
        /// <summary>
        /// 仕入先一覧メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenSupplierListView { get; } = new OpenListViewCommand<SupplierListViewModel>();
        /// <summary>
        /// 仕入先登録メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenSupplierDetailView { get; } = new OpenDetailViewCommand<SupplierDetailViewModel>();
        /// <summary>
        /// 得意先一覧メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenCustomerListView { get; } = new OpenListViewCommand<CustomerListViewModel>();
        /// <summary>
        /// 得意先登録メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenCustomerDetailView { get; } = new OpenDetailViewCommand<CustomerDetailViewModel>();
        /// <summary>
        /// 仕入先発注一覧メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenOrderToSupplierListView { get; } = new OpenListViewCommand<OrderToSupplierListViewModel>();
        /// <summary>
        /// 仕入先発注登録メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenOrderToSupplierDetailView { get; } = new OpenDetailViewCommand<OrderToSupplierDetailViewModel>();
        /// <summary>
        /// 得意先受注一覧メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenOrderFromCustomerListView { get; } = new OpenListViewCommand<OrderFromCustomerListViewModel>();
        /// <summary>
        /// 得意先受注登録メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenOrderFromCustomerDetailView { get; } = new OpenDetailViewCommand<OrderFromCustomerDetailViewModel>();
        /// <summary>
        /// 在庫推移表メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenInventoryTransactionView { get; } = new OpenInventoryTransitionViewCommand();
        /// <summary>
        /// 加工指示書メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenProcessingInstructionsPreview { get; } = new OpenProcessingInstructionsPreviewCommand();
        /// <summary>
        /// 入荷検品メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenOrderToSupplierInspectionList { get; } = new OpenListViewCommand<OrderToSupplierInspectionListViewModel>();
        /// <summary>
        /// 単品在庫は器メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand OpenInventoryDiscardView { get; } = new OpenInventoryDiscardViewCommand();
        /// <summary>
        /// SQLite新規メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand SQLiteNew { get; } = new SQLiteNewDbCommand();
        /// <summary>
        /// SQLite読込メニュー選択時に実行するコマンド
        /// </summary>
        public ICommand SQLiteLoad { get; } = new SQLiteOpenDbFileCommand();
        /// <summary>
        /// SQLite保存メニュー選択時に事項するコマンド
        /// </summary>
        public ICommand SQLiteSave { get; } = new SQLiteSaveToDbFileCommand();
        #endregion // コマンド

        #region ビューの生成・切替
        /// <summary>
        /// タブ要素ビューモデルを内部管理しているコレクションに登録する
        /// 
        /// 登録したコレクションのプロパティ変更イベントを発行することで、
        /// 画面内の <see cref="TabControl"/> が新しいタブ要素を画面に表示する。
        /// </summary>
        /// <param name="vm"></param>
        public void OpenTabItem(TabItemControlViewModelBase vm)
        {
            var found = TabItemControlCollection.SingleOrDefault(item => item.Header == vm.Header);
            if(found == null)
            {
                TabItemControlCollection.Add(vm);
                found = vm;
                found.TabItemControlClosing += CloseTabItem;

                if (found is ListViewModelBase) { (found as ListViewModelBase).DetailViewOpening += OpenDetailView; }
            }
            CurrentItem = found;
        }

        /// <summary>
        /// 各タブ要素ビューモデルの <see cref="TabItemControlViewModelBase.TabItemControlClosing"/> イベントを購読するイベントハンドラ
        /// 
        /// タブ要素ビューモデルを内部管理しているコレクションから除外する
        /// 
        /// 削除後のコレクションのプロパティ変更イベントを発行することで、
        /// 画面内の <see cref="TabControl"/> が新しいタブ要素を画面に表示する。
        /// </summary>
        /// <param name="sender">イベント発行元：タブ要素ビューモデル</param>
        /// <param name="unused">イベントパラメータ：このイベントハンドラでは使用しない</param>
        public void CloseTabItem(object sender, EventArgs unused)
        {
            var vm = sender as TabItemControlViewModelBase;
            if(vm != null)
            {
                var found = FindTabItem(vm.Header);
                if (found != null)
                {
                    TabItemControlCollection.Remove(found);

                    if (found is ListViewModelBase) { (found as ListViewModelBase).DetailViewOpening -= OpenDetailView; }
                }
                CurrentItem = TabItemControlCollection.FirstOrDefault();
                vm.TabItemControlClosing -= CloseTabItem;
            }
        }

        /// <summary>
        /// タブ要素コレクション中のタブ要素ビューモデルを検索する
        /// </summary>
        /// <param name="header">タブ要素名：<see cref="TabItemControlViewModelBase.Header"/>に表示している文字列</param>
        /// <returns>コレクション中の、header と同じタブ要素名を持つタブ要所ビューモデル</returns>
        public TabItemControlViewModelBase FindTabItem(string header)
        {
            return TabItemControlCollection.SingleOrDefault(item => item.Header == header);
        }
        #endregion // ビューの生成・切替

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViiewModel() : base()
        {
            LogUtil.Appender.PropertyChanged += RefreshLogMessage;
        }

        private void RefreshLogMessage(object unused1, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(LogUtil.Appender.Notification))
            {
                RaisePropertyChanged(nameof(Message));
            }
        }

        private void OpenDetailView(object sender, EventArgs unused)
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod(sender.GetType().Name);
                if (sender is ListViewModelBase)
                {
                    CurrentItem = (sender as ListViewModelBase).OpenDetailTabItem(this);
                    RaisePropertyChanged(nameof(TabItemControlCollection), nameof(CurrentItem));
                }
                else
                {
                    throw new NotImplementedException($"Invalid sender: {GetType().Name}.{nameof(OpenDetailView)}({sender.GetType().Name}, unused)");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(sender.GetType().Name);
            }
        }
    }
}
