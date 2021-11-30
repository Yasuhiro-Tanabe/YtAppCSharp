using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    internal class OrderToSupplierListViewModel : ListViewModelBase, IReloadable
    {
        public OrderToSupplierListViewModel() : base("仕入先発注一覧") { }

        #region プロパティ
        /// <summary>
        /// 仕入先発注一覧
        /// </summary>
        public ObservableCollection<OrderToSupplierSummaryViewModel> Orders { get; } = new ObservableCollection<OrderToSupplierSummaryViewModel>();

        /// <summary>
        /// 現在選択中の発注履歴
        /// </summary>
        public OrderToSupplierSummaryViewModel SelectedOrder
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }
        private OrderToSupplierSummaryViewModel _order;

        /// <summary>
        /// 現在選択中の仕入先発注一覧の開始日
        /// </summary>
        public DateTime From
        {
            get { return _from; }
            set { SetProperty(ref _from, value); }
        }
        private DateTime _from = DateTime.Today;

        /// <summary>
        /// 現在選択中の仕入先発注一覧の終了日
        /// </summary>
        public DateTime To
        {
            get { return _to; }
            set { SetProperty(ref _to, value); }
        }
        private DateTime _to = DateTime.Today;

        /// <summary>
        /// 仕入先一覧＋「全仕入先」：仕入先選択に使用する選択候補
        /// </summary>
        public ObservableCollection<SupplierSummaryViewModel> Suppliers { get; } = new ObservableCollection<SupplierSummaryViewModel>();

        /// <summary>
        /// 現在選択中の仕入先：「全仕入先」を含む
        /// </summary>
        public SupplierSummaryViewModel SelectedSupplier
        {
            get { return _supplier; }
            set { SetProperty(ref _supplier, value); }
        }
        private SupplierSummaryViewModel _supplier;
        #endregion // プロパティ

        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();
                if (SelectedSupplier == null)
                {
                    // 初期表示
                    UpdateSuppliers();
                    UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersToSupplier());

                    if (Orders.Count > 0)
                    {
                        From = Orders.FirstOrDefault().OrderedDate;
                        To = Orders.LastOrDefault().OrderedDate;
                    }
                }
                else
                {
                    if (To < From)
                    {
                        To = From;
                    }

                    if (SelectedSupplier.SupplierCode < 1)
                    {
                        // すべての仕入先を選択した
                        UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersToSupplier(From, To));
                    }
                    else
                    {
                        // 特定の仕入先を選択した
                        UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersToSupplier(From, To, SelectedSupplier.SupplierCode));
                    }

                }
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(msg: $"{SelectedSupplier.SupplierName}, {From:yyyyMMdd} ～ {To:yyyyMMdd}");
            }
        }
        #endregion // IReloadable
        private void UpdateSuppliers()
        {
            Suppliers.Clear();
            Suppliers.Add(new SupplierSummaryViewModel() { SupplierCode = -1, SupplierName = "すべての仕入先" });
            foreach (var supplier in MemorieDeFleursUIModel.Instance.FindAllSuppliers())
            {
                Suppliers.Add(new SupplierSummaryViewModel(supplier));
            }
            SelectedSupplier = Suppliers[0];
        }

        private void UpdateOrders(IEnumerable<OrdersToSupplier> foundOrders)
        {
            Orders.Clear();
            foreach (var order in foundOrders)
            {
                var summary = new OrderToSupplierSummaryViewModel(order);
                Subscribe(summary);
                Orders.Add(summary);
            }
        }

        public override DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM)
        {
            LogUtil.DEBUGLOG_BeginMethod(mainVM.GetType().Name);
            
            var detail = mainVM.FindTabItem(OrderToSupplierDetailViewModel.Name) as OrderToSupplierDetailViewModel;
            if(detail == null)
            {
                detail = new OrderToSupplierDetailViewModel();
            }

            detail.OrderNo = SelectedOrder.OrderNo;
            detail.UpdateProperties();

            mainVM.OpenTabItem(detail);
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name, $"{detail.GetType().Name} opened.");
            return detail;
        }

        protected override void RemoveSelectedItem(object sender)
        {
            MemorieDeFleursUIModel.Instance.CancelOrderToSupplier((sender as OrderToSupplierSummaryViewModel).OrderNo);
            UpdateProperties();
        }
    }
}
