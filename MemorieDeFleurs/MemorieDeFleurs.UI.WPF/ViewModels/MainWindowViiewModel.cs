using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ICommand Exit { get; } = new ExitCommand();
        public ICommand OpenPartsDetailView { get; } = new OpenPartsDetailViewCommand();
        public ICommand OpenPartsListView { get; } = new OpenPartListViewCommand();
        public ICommand OpenBouquetDetailView { get; } = new OpenBouquetDetailViewCommand();
        public ICommand OpenBouquetListView { get; } = new OpenBouquetListViewCommand();
        public ICommand OpenSupplierListView { get; } = new OpenSupplierListViewCommand();
        public ICommand OpenSupplierDetailView { get; } = new OpenSupplierDetailViewCommand();
        public ICommand OpenCustomerListView { get; } = new OpenCustomerListViewCommand();
        public ICommand OpenCustomerDetailView { get; } = new OpenCustomerDetailViewCommand();
        public ICommand OpenOrderToSupplierListView { get; } = new OpenOrderToSupplierListViewCommand();
        public ICommand OpenOrderToSupplierDetailView { get; } = new OpenOrderToSupplierDetailViewCommand();
        public ICommand OpenOrderFromCustomerListView { get; } = new OpenOrderFromCustomerListViewCommand();
        public ICommand OpenOrderFromCustomerDetailView { get; } = new OpenOrderFromCustomerDetailViewCommand();
        public ICommand OpenInventoryTransactionView { get; } = new OpenInventoryTransitionViewCommand();
        public ICommand SQLiteLoad { get; } = new SQLiteOpenDbFileCommand();
        public ICommand SQLiteSave { get; } = new SQLiteSaveToDbFileCommand();
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

                if (found is ListViewModelBase) { (found as ListViewModelBase).DetailViewOpening += OpenDetailView; }
            }
            CurrentItem = found;
        }

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

        public TabItemControlViewModelBase FindTabItem(string header)
        {
            return TabItemControlCollection.SingleOrDefault(item => item.Header == header);
        }
        #endregion // ビューの生成・切替

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
