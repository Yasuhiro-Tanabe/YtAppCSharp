using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Linq;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class OrderToSupplierInspectionSummaryViewModel : ListItemViewModelBase, IDialogCaller
    {
        public OrderToSupplierInspectionSummaryViewModel() : base(new OpenDialogCommand()) { }

        public OrderToSupplierInspectionSummaryViewModel(OrdersToSupplier order) : this()
        {
            Update(order);
        }

        #region プロパティ
        /// <summary>
        /// 発注番号
        /// </summary>
        public string OrderNo
        {
            get { return _no; }
            set { SetProperty(ref _no, value); }
        }
        private string _no;

        /// <summary>
        /// 仕入先コード
        /// </summary>
        public int SupplierCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private int _code;

        /// <summary>
        /// 仕入先名称
        /// </summary>
        public string SupplierName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        /// <summary>
        /// 発注日
        /// </summary>
        public DateTime OrderDate
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }
        private DateTime _order;

        /// <summary>
        /// 入荷予定日
        /// </summary>
        public DateTime ArrivalDate
        {
            get { return _arrive; }
            set { SetProperty(ref _arrive, value); }
        }
        private DateTime _arrive;

        /// <summary>
        /// 発注した単品とロット数
        /// </summary>
        public string OrderParts
        {
            get { return _parts; }
            set { SetProperty(ref _parts, value); }
        }
        private string _parts;

        /// <summary>
        /// 検品済かどうか
        /// </summary>
        public bool IsInspected
        {
            get { return _inspected; }
            set { SetProperty(ref _inspected, value); }
        }
        private bool _inspected;
        #endregion // プロパティ

        private void Update(OrdersToSupplier order)
        {
            OrderNo = order.ID;
            SupplierCode = order.Supplier;
            SupplierName = MemorieDeFleursUIModel.Instance.FindSupplier(SupplierCode).Name;
            OrderDate = order.OrderDate;
            ArrivalDate = order.DeliveryDate;
            OrderParts = string.Join(", ", order.Details.Select(d => $"{d.PartsCode}x{d.LotCount}"));
            IsInspected = order.Status == OrderToSupplierStatus.ARRIVED;
        }

        #region IDialogCaller
        public NotificationObject DialogViewModel
        {
            get
            {
                var vm = new OrderToSupplierInspectionDetailViewModel() { OrderNo = OrderNo };
                vm.UpdateProperties();
                return vm;
            }
        }
        #endregion // IDialogCaller
    }
}
