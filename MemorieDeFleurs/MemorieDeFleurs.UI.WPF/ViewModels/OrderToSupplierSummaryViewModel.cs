using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Linq;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    internal class OrderToSupplierSummaryViewModel : ListItemViewModelBase, IReloadable, IDialogCaller
    {
        public OrderToSupplierSummaryViewModel(OrdersToSupplier order) : base(new OpenDetailViewCommand())
        {
            OrderNo = order.ID;
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
        /// 仕入先名称
        /// </summary>
        public string SupplierName
        {
            get { return _supplier; }
            set { SetProperty(ref _supplier, value); }
        }
        private string _supplier;

        /// <summary>
        /// 発注明細 (一覧表示用)
        /// </summary>
        public string OrderParts
        {
            get { return _parts; }
            set { SetProperty(ref _parts, value); }
        }
        private string _parts;

        /// <summary>
        /// 納品予定日 (表示用)
        /// </summary>
        public string ArrivalDateText
        {
            get { return _arrival.ToString("yyyyMMdd"); }
        }

        /// <summary>
        /// 納品予定日
        /// </summary>
        public DateTime ArrivalDate
        {
            get { return _arrival; }
            set
            {
                SetProperty(ref _arrival, value);
                RaisePropertyChanged(nameof(ArrivalDateText));
            }
        }
        private DateTime _arrival;

        /// <summary>
        /// 発注日 (表示用)
        /// </summary>
        public string OrderedDateText
        {
            get { return _ordered.ToString("yyyyMMdd"); }
        }

        /// <summary>
        /// 発注日
        /// </summary>
        public DateTime OrderedDate
        {
            get { return _ordered; }
            set
            {
                SetProperty(ref _ordered, value);
                RaisePropertyChanged(nameof(OrderedDateText));
            }
        }
        private DateTime _ordered;
        #endregion // プロパティ

        #region IReloadabe
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            if(string.IsNullOrWhiteSpace(OrderNo))
            {
                throw new ApplicationException("発注番号が指定されていません。");
            }
            else
            {
                var order = MemorieDeFleursUIModel.Instance.FindOrderToSupplier(OrderNo);
                if(order == null)
                {
                    throw new ApplicationException($"該当する発注情報がありません：{OrderNo}");
                }
                else
                {
                    Update(order);
                }
            }
        }
        private void Update(OrdersToSupplier order)
        {
            SupplierName = MemorieDeFleursUIModel.Instance.FindSupplier(order.Supplier).Name;
            OrderParts = string.Join(", ", order.Details.Select(i => $"{i.PartsCode} x{i.LotCount}"));
            ArrivalDate = order.DeliveryDate;
            OrderedDate = order.OrderDate;
        }
        #endregion // IReloadable

        #region IDialogCaller
        /// <inheritdoc/>
        public OpenDialogCommand OpenDialog { get; } = new OpenDialogCommand();

        /// <inheritdoc/>
        public NotificationObject DialogViewModel
        {
            get
            {
                var vm = new OrderToSupplierDetailViewModel() { OrderNo = OrderNo };
                vm.UpdateProperties();
                return vm;
            }
        }
        #endregion // IDialogCaller
    }
}
