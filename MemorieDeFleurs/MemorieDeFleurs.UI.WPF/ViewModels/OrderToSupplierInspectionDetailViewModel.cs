using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 入荷検品ダイアログ内で表示するビューモデル
    /// </summary>
    public class OrderToSupplierInspectionDetailViewModel : DetailViewModelBase, IReloadable, IDialogViewModel
    {
        /// <summary>
        /// ビューモデルの名称：<see cref="TabItemControlViewModelBase.Header"/> や <see cref="MainWindowViiewModel.FindTabItem(string)"/> に渡すクラス定数として使用する。
        /// </summary>
        public static string Name { get; } = "検品";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderToSupplierInspectionDetailViewModel() : base(Name) { }

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
            get { return _ordered; }
            set { SetProperty(ref _ordered, value); }
        }
        private DateTime _ordered;

        /// <summary>
        /// 検品実施日
        /// </summary>
        public DateTime InspectionDate
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }
        private DateTime _date = DateTime.Today;

        /// <summary>
        /// 検品済かどうかの判定フラグ
        /// </summary>
        public bool IsInspected
        {
            get { return _inspected; }
            set { SetProperty(ref _inspected, value); }
        }
        private bool _inspected;

        /// <summary>
        /// 仕入先発注明細
        /// </summary>
        public ObservableCollection<InspectionPartsListItemViewModel> Parts { get; } = new ObservableCollection<InspectionPartsListItemViewModel>();
        #endregion // プロパティ

        #region IDialogViewModel
        /// <inheritdoc/>
        public void DialogCancel()
        {
            // 何もしない
        }

        /// <inheritdoc/>
        public void DialogOK()
        {
            if(!IsInspected)
            {
                var changed = Parts.Where(d => d.IsQuantityChanged).Select(d => Tuple.Create(d.BouquetPart, d.ActualQuantity));
                MemorieDeFleursUIModel.Instance.InspectArrivedOrder(OrderNo, InspectionDate, changed);
            }
        }

        /// <inheritdoc/>
        public void FillDialogParameters(DialogParameter param)
        {
            param.DialogTitle = $"検品：{OrderNo}";
            param.OkContent = "確定";
            param.CancelContent = "キャンセル";
        }
        #endregion // IDialogViewModel

        #region IReloadable
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
                var found = MemorieDeFleursUIModel.Instance.FindOrderToSupplier(OrderNo);
                if(found == null)
                {
                    throw new ApplicationException($"発注情報が見つかりません：{OrderNo}");
                }
                Update(found);
            }
        }
        #endregion // IReloadable

        private void Update(OrdersToSupplier order)
        {
            SupplierCode = order.Supplier;
            SupplierName = MemorieDeFleursUIModel.Instance.FindSupplier(SupplierCode).Name;
            OrderDate = order.OrderDate;
            InspectionDate = order.DeliveryDate;
            IsInspected = order.Status == OrderToSupplierStatus.ARRIVED;

            Parts.Clear();
            foreach(var detail in order.Details)
            {
                Parts.Add(new InspectionPartsListItemViewModel(detail) { IsInspected = IsInspected });
            }
            RaisePropertyChanged(nameof(Parts));
        }
    }
}
