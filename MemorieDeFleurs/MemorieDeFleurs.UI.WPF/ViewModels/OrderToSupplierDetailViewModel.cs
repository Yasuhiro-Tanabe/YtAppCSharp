using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class OrderToSupplierDetailViewModel : DetailViewModelBase, IEditableAndFixable, IAppendableRemovable, IOrderable, IDialogViewModel, IPrintable, IReloadable
    {
        public static string Name { get; } = "仕入先発注詳細";

        public OrderToSupplierDetailViewModel() : base(Name)
        {
            LoadSupplierList();
            SelectedSupplier = null;
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
        /// 仕入先ID (表示専用)
        /// </summary>
        public int SupplierCode
        {
            get { return SelectedSupplier == null ? 0 : SelectedSupplier.SupplierCode; }
        }

        /// <summary>
        /// 仕入先名称 (表示専用)
        /// </summary>
        public string SupplierName
        {
            get { return SelectedSupplier == null ? string.Empty : SelectedSupplier.SupplierName; }
        }

        /// <summary>
        /// 発注日
        /// </summary>
        public DateTime OrderDate
        {
            get { return _orderDate; }
            set { SetProperty(ref _orderDate, value); }
        }
        private DateTime _orderDate = DateTime.Today;

        /// <summary>
        /// 納品希望日
        /// </summary>
        public DateTime ArrivalDate
        {
            get { return _arrivalDate; }
            set { SetProperty(ref _arrivalDate, value); }
        }
        private DateTime _arrivalDate = DateTime.Today;

        /// <summary>
        /// 発注する単品一覧 (表示専用)
        /// </summary>
        public string OrderPartsText
        {
            get { return _partsText; }
            private set { SetProperty(ref _partsText, value); }
        }
        private string _partsText;

        /// <summary>
        /// 仕入先一覧
        /// </summary>
        public ObservableCollection<SupplierSummaryViewModel> Suppliers { get; } = new ObservableCollection<SupplierSummaryViewModel>();

        /// <summary>
        /// 選択中の仕入先
        /// </summary>
        public SupplierSummaryViewModel SelectedSupplier
        {
            get { return _supplier; }
            set { SetProperty(ref _supplier, value); }
        }
        private SupplierSummaryViewModel _supplier;

        /// <summary>
        /// 仕入先が提供している単品のうち <see cref="OrderParts"/> に登録されていないもの一覧
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> SupplyParts { get; } = new ObservableCollection<PartsListItemViewModel>();

        /// <summary>
        /// 現在選択中の <see cref="OrderParts"/> 未登録単品
        /// </summary>
        public PartsListItemViewModel SelectedSupplyParts
        {
            get { return _supplingParts; }
            set { SetProperty(ref _supplingParts, value); }
        }
        private PartsListItemViewModel _supplingParts;

        /// <summary>
        /// 仕入対象単品の一覧
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> OrderParts { get; } = new ObservableCollection<PartsListItemViewModel>();

        /// <summary>
        /// 現在選択中の仕入対象単品
        /// </summary>
        public PartsListItemViewModel SelectedOrderParts
        {
            get { return _orderParts; }
            set { SetProperty(ref _orderParts, value); }
        }
        private PartsListItemViewModel _orderParts;

        /// <summary>
        /// 商品構成編集中かどうか
        /// </summary>
        public bool IsEditing
        {
            get { return _editing; }
            set { SetProperty(ref _editing, value); }
        }
        private bool _editing;

        /// <summary>
        /// 印刷日
        /// </summary>
        public DateTime PrintDate
        {
            get { return _print; }
            set { SetProperty(ref _print, value); }
        }
        private DateTime _print = DateTime.Today;


        #endregion // プロパティ

        #region コマンド
        public ICommand Edit { get; } = new EditCommand();
        public ICommand Fix { get; } = new FixCommand();
        public ICommand Append { get; } = new AppendToListCommand();
        public ICommand Remove { get; } = new RemoveFromListCommand();
        public ICommand Order { get; } = new OrderCommand();
        public ICommand Cancel { get; } = new CancelOrderCommand();
        public ICommand ChangeArrivalDate { get; } = new ChangeArrivalDateCommand();
        public ICommand PreviewPrint { get; } = new OpenDialogCommand();
        public ICommand Print { get; } = new PrintCommand();
        #endregion // コマンド

        #region IReloadable
        /// <inheritdoc/>
        public void UpdateProperties()
        {
            if (string.IsNullOrWhiteSpace(OrderNo))
            {
                throw new ApplicationException("発注番号が指定されていません。");
            }
            else
            {
                var found = MemorieDeFleursUIModel.Instance.FindOrderToSupplier(OrderNo);
                if (found == null)
                {
                    throw new ApplicationException($"該当する発注がありません：{OrderNo}");
                }
                else
                {
                    Update(found);
                    LogUtil.DEBUGLOG_MethodCalled(msg: $"Order={OrderNo}, {OrderPartsText}");
                }
            }
        }
        private void Update(OrdersToSupplier order)
        {
            OrderNo = order.ID;
            OrderDate = order.OrderDate;
            ArrivalDate = order.DeliveryDate;

            LoadSupplierList();
            SelectedSupplier = Suppliers.SingleOrDefault(s => s.SupplierCode == order.Supplier);

            OrderParts.Clear();
            foreach (var item in order.Details)
            {
                OrderParts.Add(new PartsListItemViewModel(item));
            }

            OrderPartsText = string.Join(", ", OrderParts.Select(p => $"{p.PartsCode} x{p.Quantity}"));
            IsEditing = false;

            IsDirty = false;
        }
        private void LoadSupplierList()
        {
            Suppliers.Clear();
            foreach (var s in MemorieDeFleursUIModel.Instance.FindAllSuppliers())
            {
                Suppliers.Add(new SupplierSummaryViewModel(s));
            }
        }
        #endregion // IReloadable

        #region IEditableFixable
        public void OpenEditView()
        {
            if(SelectedSupplier == null)
            {
                throw new ApplicationException("仕入先が指定されていません。");
            }

            var supplier = MemorieDeFleursUIModel.Instance.FindSupplier(SelectedSupplier.SupplierCode);

            SupplyParts.Clear();
            foreach (var item in supplier.SupplyParts)
            {
                if(OrderParts.SingleOrDefault(p => p.PartsCode == item.PartCode) == null)
                {
                    // OrderParts に登録されていない単品のみ登録する
                    SupplyParts.Add(new PartsListItemViewModel(item.Part));
                }
            }

            SelectedOrderParts = null;
            SelectedSupplyParts = null;
            IsEditing = true;
        }

        public void FixEditing()
        {
            IsEditing = false;
            OrderPartsText = string.Join(", ", OrderParts.Select(p => $"{p.PartsCode} x{p.Quantity}"));
        }
        #endregion // IEditableFixable

        #region IAddableRemovable
        public void AppendToList()
        {
            // SupplyParts から OrderParts への移動
            var parts = SelectedSupplyParts;

            OrderParts.Add(parts);
            SelectedOrderParts = parts;

            SupplyParts.Remove(parts);
            SelectedSupplyParts = null;
        }

        public void RemoveFromList()
        {
            // OrderParts から SupplyParts への移動
            var parts = SelectedSupplyParts;

            SupplyParts.Add(parts);
            SelectedSupplyParts = parts;

            OrderParts.Remove(parts);
            SelectedOrderParts = null;
        }
        #endregion // IAddableRemovable

        #region IOrderable
        public void OrderMe()
        {
            if(string.IsNullOrWhiteSpace(_no))
            {
                var order = new OrdersToSupplier()
                {
                    OrderDate = OrderDate,
                    DeliveryDate = ArrivalDate,
                    Supplier = SupplierCode
                };
                foreach(var parts in OrderParts)
                {
                    order.Details.Add(new OrderDetailsToSupplier() { PartsCode = parts.PartsCode, LotCount = parts.Quantity });
                }
                OrderNo = MemorieDeFleursUIModel.Instance.Order(order);
                UpdateProperties();
            }
            else
            {
                throw new ApplicationException("すでに発注済です。");
            }
        }

        public void CancelMe()
        {
            if(string.IsNullOrWhiteSpace(_no))
            {
                throw new ApplicationException("発注されていません。");
            }
            else
            {
                MemorieDeFleursUIModel.Instance.CancelOrderToSupplier(OrderNo);
                Clear.Execute(this);
            }
        }

        public void ChangeMyArrivalDate()
        {
            if (string.IsNullOrWhiteSpace(_no))
            {
                throw new ApplicationException("発注されていません。");
            }
            else
            {
                MemorieDeFleursUIModel.Instance.ChangeArrivalDateOfOrderToSupplier(OrderNo, ArrivalDate);
            }
        }
        #endregion // IOrderable

        public override void SaveToDatabase()
        {
            // 使用しない：「発注」、「納期変更」、発注取消」で操作させる。
            base.SaveToDatabase();
        }

        public override void Validate()
        {
            var ex = new ValidateFailedException();
            if(SupplierCode < 1)
            {
                ex.Append("仕入先が選択されていません。");
            }
            if(OrderParts.Count == 0)
            {
                ex.Append("仕入れる単品がありません。");
            }

            var days = (ArrivalDate - OrderDate).Days;
            foreach(var parts in OrderParts)
            {
                if(days < parts.LeadTime)
                {
                    ex.Append($"{parts.PartsName}({parts.PartsCode}) の発注リードタイムは本日から {parts.LeadTime} 日です。納品予定日 {ArrivalDate:yyyy/MM/dd} に間に合いません。");
                }
            }

            if(ex.ValidationErrors.Count >0) { throw ex; }
        }

        public override void ClearProperties()
        {
            OrderNo = string.Empty;
            OrderPartsText = string.Empty;
            OrderDate = DateTime.Today;
            ArrivalDate = DateTime.Today;

            SelectedSupplier = null;
            LoadSupplierList();

            SelectedOrderParts = null;
            OrderParts.Clear();

            SelectedSupplyParts = null;
            SupplyParts.Clear();

            IsEditing = false;

            IsDirty = false;
        }

        #region IDialogViewModel
        public void FillDialogParameters(DialogParameter param)
        {
            param.DialogTitle = "発注書印刷";
            param.OkContent = "印刷";
            param.CancelContent = "キャンセル";
        }

        public void DialogOK()
        {
            LogUtil.DEBUGLOG_MethodCalled(msg: $"Order={OrderNo}");
            Print.Execute(this);
        }

        public void DialogCancel()
        {
            LogUtil.DEBUGLOG_MethodCalled(msg: $"Order={OrderNo}");
        }
        #endregion // IDialogViewModel

        #region IPrintable
        public void ValidateBeforePrinting() { }
        #endregion // IPrintable
    }
}
