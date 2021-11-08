using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class OrderFromCustomerSummaryViewModel : ListItemViewModelBase
    {
        public OrderFromCustomerSummaryViewModel() : base(new OpenOrderFromCustomerDetailViewCommand()) { }

        #region プロパティ
        /// <summary>
        /// 受注番号
        /// </summary>
        public string OrderNo
        {
            get { return _orderNo; }
            set { SetProperty(ref _orderNo, value); }
        }
        private string _orderNo;

        /// <summary>
        /// 得意先名称
        /// </summary>
        public string CustomerName
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }
        private string _customer;

        /// <summary>
        /// お届け先名称
        /// </summary>
        public string ShippingTo
        {
            get { return _shippingTo; }
            set { SetProperty(ref _shippingTo, value); }
        }
        private string _shippingTo;

        /// <summary>
        /// 受注日(表示用)
        /// </summary>
        public string OrderDateText { get { return _ordered.ToString("yyyy/MM/dd"); } }

        /// <summary>
        /// 受注日 (データベースとのやりとり等で使用する)
        /// </summary>
        public DateTime OrderDate
        {
            get { return _ordered; }
            set { SetProperty(ref _ordered, value); }
        }
        private DateTime _ordered;

        /// <summary>
        /// お届け日
        /// 
        /// 内部で保持している発送日の翌日
        /// </summary>
        public string ArrivalDateText { get { return _shipping.AddDays(1).ToString("yyyy/MM/dd"); } }

        /// <summary>
        /// 発送日
        /// </summary>
        public DateTime ShippingDate
        {
            get { return _shipping; }
            set { SetProperty(ref _shipping, value); }
        }
        private DateTime _shipping;

        /// <summary>
        /// お届け商品名
        /// </summary>
        public string BouquetName
        {
            get { return _bouquet; }
            set { SetProperty(ref _bouquet, value); }
        }
        private string _bouquet;
        #endregion // プロパティ


        public OrderFromCustomerSummaryViewModel(OrderFromCustomer order) : this()
        {
            Update(order);
        }

        private void Update(OrderFromCustomer order)
        {
            _orderNo = order.ID;
            _customer = order.Customer.Name;
            _shippingTo = order.ShippingAddress.Name;
            _shipping = order.ShippingDate;
            _bouquet = order.Bouquet.Name;
            _ordered = order.OrderDate;

            RaisePropertyChanged(nameof(OrderNo), nameof(CustomerName), nameof(ShippingTo), nameof(ArrivalDateText), nameof(ShippingDate),
                nameof(BouquetName), nameof(OrderDateText), nameof(OrderDate));
        }
    }
}
