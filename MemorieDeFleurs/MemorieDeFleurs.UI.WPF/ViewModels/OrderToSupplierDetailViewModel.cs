using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class OrderToSupplierDetailViewModel : DetailViewModelBase
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
        public string ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        private string _id;

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
        /// 発注日 (表示専用)
        /// </summary>
        public string OrderDateText
        {
            get { return _orderDate.ToShortDateString(); }
        }
        /// <summary>
        /// 発注日
        /// </summary>
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

        public string OrderPartsText
        {
            get { return _partsText; }
        }
        private string _partsText;

        /// <summary>
        /// 仕入先一覧
        /// </summary>
        public ObservableCollection<SupplierSummaryViewModel> Suppliers { get; } = new ObservableCollection<SupplierSummaryViewModel>();
        public SupplierSummaryViewModel SelectedSupplier { get; set; }

        /// <summary>
        /// 仕入先が提供している単品のうち <see cref="OrderParts"/> に登録されていないもの一覧
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> SupplyParts { get; } = new ObservableCollection<PartsListItemViewModel>();
        public PartsListItemViewModel SelectedSupplyParts { get; set; }

        /// <summary>
        /// 仕入対象単品の一覧
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> OrderParts { get; } = new ObservableCollection<PartsListItemViewModel>();
        public PartsListItemViewModel SelectedOrderParts { get; set; }

        /// <summary>
        /// 商品構成編集中に表示するコントロールの可視性
        /// </summary>
        public Visibility EditingModeVisivility { get { return _editing ? Visibility.Visible : Visibility.Collapsed; } }
        private bool _editing;

        #endregion // プロパティ

        #region コマンド
        public ICommand Edit { get; } = new EditPartsListCommand();
        public ICommand Fix { get; } = new FixPartsListCommand();
        public ICommand Append { get; } = new AddToListItemCommand();
        public ICommand Remove { get; } = new RemoveFromListItemCommand();
        public ICommand Order { get; }
        public ICommand Cancel { get; }
        #endregion // コマンド

        public void Update(OrdersToSupplier order)
        {
            _id = order.ID;
            _orderDate = order.OrderDate;
            _arrivalDate = order.DeliveryDate;

            LoadSupplierList();
            SelectedSupplier = Suppliers.SingleOrDefault(s => s.SupplierCode == order.Supplier);

            OrderParts.Clear();
            foreach (var item in order.Details)
            {
                OrderParts.Add(new PartsListItemViewModel(item));
            }

            _partsText = string.Join(", ", OrderParts.Select(p => $"{p.PartsCode} x{p.Quantity}"));
            _editing = false;
            RaisePropertyChanged(nameof(ID), nameof(SupplierCode), nameof(SupplierName), nameof(OrderDateText), nameof(ArrivalDate));
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

        public override void Update()
        {
            if(string.IsNullOrWhiteSpace(_id))
            {
                throw new ApplicationException("発注番号が指定されていません。");
            }
            else
            {
                var found = MemorieDeFleursUIModel.Instance.FindOrderToSupplier(_id);
                if(found == null)
                {
                    throw new ApplicationException($"該当する発注がありません：{_id}");
                }
                else
                {
                    Update(found);
                }
            }
        }


        public void EditOrderParts()
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
            _editing = true;

            RaisePropertyChanged(nameof(SupplyParts), nameof(SelectedOrderParts), nameof(SelectedSupplyParts), nameof(OrderParts), nameof(EditingModeVisivility));
        }
        public void FixOrderParts()
        {
            _editing = false;
            RaisePropertyChanged(nameof(EditingModeVisivility), nameof(OrderPartsText));
        }

        public void AppendToOrderParts()
        {

        }

        public void RemoveFromOrderParts()
        {

        }

        public void OrderMe()
        {
            if(string.IsNullOrWhiteSpace(_id))
            {

            }
            else
            {
                throw new ApplicationException("すでに発注済です。");
            }
        }
        public void CancelMe()
        {
            if(string.IsNullOrWhiteSpace(_id))
            {
                throw new ApplicationException("発注されていません。");
            }
            else
            {

            }
        }

        public override void SaveToDatabase()
        {
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

            var days = (ArrivalDate - DateTime.Today).Days;
            foreach(var parts in OrderParts)
            {
                if(days < parts.LeadTime)
                {
                    ex.Append($"{parts.PartsName}({parts.PartsCode}) の発注リードタイムは本日から {parts.LeadTime} 日です。納品予定日 {ArrivalDate:yyyy/MM/dd} に間に合いません。");
                }
            }

            if(ex.ValidationErrors.Count >0) { throw ex; }
        }
    }
}
