using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class OrderFromCustomerListViewModel : ListViewModelBase
    {
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
        public CustomerSummaryViewModel SelectedCustomer { get; set; }

        /// <summary>
        /// 得意先からの注文一覧
        /// </summary>
        public ObservableCollection<OrderFromCustomerSummaryViewModel> Orders { get; } = new ObservableCollection<OrderFromCustomerSummaryViewModel>();

        /// <summary>
        /// 現在選択中の得意先からの注文
        /// </summary>
        public OrderFromCustomerSummaryViewModel SelectedOrder { get; set; }
        #endregion // プロパティ

        public override void LoadItems()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();
                if(SelectedCustomer == null)
                {
                    UpdateCustomers();
                    UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersFromCustomer());

                    if(Orders.Count > 0)
                    {
                        _from = Orders.FirstOrDefault().OrderDate;
                        _to = Orders.LastOrDefault().OrderDate;
                    }

                    RaisePropertyChanged(nameof(Orders), nameof(SelectedOrder), nameof(Customers), nameof(SelectedCustomer), nameof(From), nameof(To));
                }
                else
                {
                    if(_to < _from)
                    {
                        _to = _from;
                        RaisePropertyChanged(nameof(To));
                    }

                    if(SelectedCustomer.CustomerID < 1)
                    {
                        // すべての得意先を選択した
                        UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersFromCustomer(_from, _to));
                    }
                    else
                    {
                        // 特定の仕入先を選択した
                        UpdateOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersFromCustomer(_from, _to, SelectedCustomer.CustomerID));
                    }

                    RaisePropertyChanged(nameof(Orders), nameof(SelectedOrder));
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
            foreach(var customer in MemorieDeFleursUIModel.Instance.FindAllCustomers())
            {
                Customers.Add(new CustomerSummaryViewModel(customer));
            }
            SelectedCustomer = Customers[0];
        }
        private void UpdateOrders(IEnumerable<OrderFromCustomer> orders)
        {
            Orders.Clear();
            foreach(var order in orders)
            {
                var item = new OrderFromCustomerSummaryViewModel(order);
                Subscribe(item);
                Orders.Add(item);
            }
            SelectedOrder = null;
        }

        public override DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM)
        {
            OrderFromCustomerDetailViewModel detail = null;
            try
            {
                LogUtil.DEBUGLOG_BeginMethod(mainVM.GetType().Name);
                detail = mainVM.FindTabItem(OrderFromCustomerDetailViewModel.Name) as OrderFromCustomerDetailViewModel;
                if(detail == null)
                {
                    detail = new OrderFromCustomerDetailViewModel();
                    mainVM.OpenTabItem(detail);
                }

                detail.OrderNo = SelectedOrder.OrderNo;
                detail.Update();
                return detail;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name, $"{detail?.GetType()?.Name} opened.");
            }
        }

        protected override void RemoveSelectedItem(object sender)
        {
            MemorieDeFleursUIModel.Instance.CancelOrderFromCustomer(SelectedOrder.OrderNo);
            LoadItems();
        }

    }
}
