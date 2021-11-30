using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 入荷検品一覧画面内に一覧表示する各検品対象受注情報のビューモデル
    /// </summary>
    public class OrderToSupplierInspectionSummaryViewModel : ListItemViewModelBase, IDialogCaller
    {
        /// <summary>
        /// この受注情報の検品が終了したことを通知する
        /// </summary>
        public event EventHandler Inspected;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderToSupplierInspectionSummaryViewModel() : base(null)
        {
            OpenDialog.DialogClosing += NotifyInspected;
        }

        private void NotifyInspected(object sender, DialogClosingEventArgs args)
        {
            if (args.Status == DialogClosingStatus.OK)
            {
                Inspected?.Invoke(this, null);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="order">表示する受注情報</param>
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
        /// <inheritdoc/>
        public OpenDialogCommand OpenDialog { get; } = new OpenDialogCommand();

        /// <inheritdoc/>
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
