using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 得意先受注一覧画面のビューモデル
    /// </summary>
    public class OrderFromCustomerListViewModel : ListViewModelBase, IReloadable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderFromCustomerListViewModel() : base("得意先受注一覧") { }

        #region プロパティ
        /// <summary>
        /// 表示中の注文一覧の開始日
        /// </summary>
        public DateTime From
        {
            get { return _from; }
            set { SetProperty(ref _from, value); }
        }
        private DateTime _from;

        /// <summary>
        /// 表示中の注文一覧の終了日
        /// </summary>
        public DateTime To
        {
            get { return _to; }
            set { SetProperty(ref _to, value); }
        }
        private DateTime _to;

        /// <summary>
        /// 得意先一覧：注文一覧絞り込み用
        /// </summary>
        public ObservableCollection<CustomerSummaryViewModel> Customers { get; } = new ObservableCollection<CustomerSummaryViewModel>();

        /// <summary>
        /// 現在選択中の得意先：「全得意先」を含む
        /// </summary>
        public CustomerSummaryViewModel SelectedCustomer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }
        private CustomerSummaryViewModel _customer;

        /// <summary>
        /// 得意先からの注文一覧
        /// </summary>
        public ObservableCollection<OrderFromCustomerSummaryViewModel> Orders { get; } = new ObservableCollection<OrderFromCustomerSummaryViewModel>();

        /// <summary>
        /// 現在選択中の得意先からの注文
        /// </summary>
        public OrderFromCustomerSummaryViewModel SelectedOrder
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }
        private OrderFromCustomerSummaryViewModel _order;
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
                if (SelectedCustomer == null)
                {
                    UpdateCustomers();
                    UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersFromCustomer());

                    if (Orders.Count > 0)
                    {
                        From = Orders.FirstOrDefault().OrderDate;
                        To = Orders.LastOrDefault().OrderDate;
                    }
                }
                else
                {
                    if (To < From)
                    {
                        To = From;
                    }

                    if (SelectedCustomer.CustomerID < 1)
                    {
                        // すべての得意先を選択した
                        UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersFromCustomer(From, To));
                    }
                    else
                    {
                        // 特定の仕入先を選択した
                        UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersFromCustomer(From, To, SelectedCustomer.CustomerID));
                    }
                }
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }
        private void UpdateCustomers()
        {
            Customers.Clear();
            Customers.Add(new CustomerSummaryViewModel() { CustomerID = -1, CustomerName = "すべての得意先" });
            foreach (var customer in MemorieDeFleursUIModel.Instance.FindAllCustomers())
            {
                Customers.Add(new CustomerSummaryViewModel(customer));
            }
            SelectedCustomer = Customers[0];
        }
        private void UpdateOrders(IEnumerable<OrderFromCustomer> orders)
        {
            Orders.Clear();
            foreach (var order in orders)
            {
                var item = new OrderFromCustomerSummaryViewModel(order);
                Subscribe(item);
                Orders.Add(item);
            }
            SelectedOrder = null;
        }
        #endregion // IReloadable

        /// <inheritdoc/>
        public override DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM)
        {
            OrderFromCustomerDetailViewModel detail = null;
            try
            {
                LogUtil.DEBUGLOG_BeginMethod(mainVM.GetType().Name);
                detail = mainVM.FindTabItem(OrderFromCustomerDetailViewModel.Name) as OrderFromCustomerDetailViewModel;
                if (detail == null)
                {
                    detail = new OrderFromCustomerDetailViewModel();
                }

                detail.OrderNo = SelectedOrder.OrderNo;
                detail.UpdateProperties();

                mainVM.OpenTabItem(detail);
                return detail;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name, $"{detail?.GetType()?.Name} opened.");
            }
        }

        /// <inheritdoc/>
        protected override void RemoveSelectedItem(object sender)
        {
            MemorieDeFleursUIModel.Instance.CancelOrderFromCustomer(SelectedOrder.OrderNo);
            UpdateProperties();
        }

    }
}
