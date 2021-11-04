using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Linq;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    internal class OrderToSupplierSummaryViewModel : ListItemViewModelBase
    {
        public OrderToSupplierSummaryViewModel(OrdersToSupplier order) : base(new OpenOrderToSupplierDetailViewCommand())
        {
            Update(order);
        }

        #region プロパティ
        public string OrderID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        private string _id;

        public string SupplierName
        {
            get { return _supplier; }
            set { SetProperty(ref _supplier, value); }
        }
        private string _supplier;

        public string OrderParts
        {
            get { return _parts; }
            set { SetProperty(ref _parts, value); }
        }
        private string _parts;

        public string ArrivalDateText
        {
            get { return _arrival.ToString("yyyyMMdd"); }
        }
        public DateTime ArrivalDate
        {
            get { return _arrival; }
            set { SetProperty(ref _arrival, value); }
        }
        private DateTime _arrival;

        public string OrderedDateText
        {
            get { return _ordered.ToString("yyyyMMdd"); }
        }

        public DateTime OrderedDate
        {
            get { return _ordered; }
            set { SetProperty(ref _ordered, value); }
        }
        private DateTime _ordered;
        #endregion // プロパティ

        public void Update(OrdersToSupplier order)
        {
            _id = order.ID;
            _supplier = MemorieDeFleursUIModel.Instance.FindSupplier(order.Supplier).Name;
            _parts = string.Join(", ", order.Details.Select(i => $"{i.PartsCode} x{i.LotCount}"));
            _arrival = order.DeliveryDate;
            _ordered = order.OrderDate;

            RaisePropertyChanged(nameof(ActionVisivility), nameof(OrderID), nameof(SupplierName), nameof(OrderParts), nameof(ArrivalDate));
        }
    }
}
