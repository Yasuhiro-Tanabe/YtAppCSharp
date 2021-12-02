using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 入荷検品一覧画面のビューモデル
    /// </summary>
    public class OrderToSupplierInspectionListViewModel : ListViewModelBase, IReloadable
    {
        /// <summary>
        /// ビューモデルの名称：<see cref="TabItemControlViewModelBase.Header"/> や <see cref="MainWindowViiewModel.FindTabItem(string)"/> に渡すクラス定数として使用する。
        /// </summary>
        public static string Name { get { return TextResourceFinder.FindText("PartsInspection_List"); } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderToSupplierInspectionListViewModel() : base(Name)
        {
            SelectedKey = DateKeies[0];
        }

        #region プロパティ
        /// <summary>
        /// 発注情報の一覧
        /// </summary>
        public ObservableCollection<OrderToSupplierInspectionSummaryViewModel> Orders { get; } = new ObservableCollection<OrderToSupplierInspectionSummaryViewModel>();

        /// <summary>
        /// 選択中の発注情報
        /// </summary>
        public OrderToSupplierInspectionSummaryViewModel SelectedOrder
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }
        private OrderToSupplierInspectionSummaryViewModel _order;

        /// <summary>
        /// 日付選択肢の一覧：<see cref="From"/> と <see cref="To"/> が発注日か入荷予定日かの選択肢
        /// </summary>
        public IList<DateSelectionKeyViewModel> DateKeies { get; } = new List<DateSelectionKeyViewModel>() {
            new DateSelectionKeyViewModel() { Key=DateSelectionKey.ORDERED, ContentText = TextResourceFinder.FindText("OrderDateToSupplier_Collon") },
            new DateSelectionKeyViewModel() { Key =DateSelectionKey.ARRIVED, ContentText = TextResourceFinder.FindText("ArrivalDateFromSupplier_Collon")}
        };

        /// <summary>
        /// 現在選択中の日付選択肢
        /// </summary>
        public DateSelectionKeyViewModel SelectedKey
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }
        private DateSelectionKeyViewModel _key;

        /// <summary>
        /// 発注情報一覧に表示する要素の開始日：発注日か入荷予定日かは <see cref="SelectedKey"/> で指定する
        /// </summary>
        public DateTime From
        {
            get { return _from; }
            set { SetProperty(ref _from, value); }
        }
        private DateTime _from = DateTime.Today;

        /// <summary>
        /// 発注情報一覧に表示する要素の終了日：発注日か入荷予定日かは <see cref="SelectedKey"/> で指定する
        /// </summary>
        public DateTime To
        {
            get { return _to; }
            set { SetProperty(ref _to, value); }
        }
        private DateTime _to = DateTime.Today;

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
        #endregion // プロパティ

        #region コマンド
        /// <summary>
        /// 日付区分および期間の開始終了日が変更された際に実行するコマンド
        /// </summary>
        public ICommand DateChanged { get; } = new ChangeInspectionDateCommand();
        /// <summary>
        /// 仕入先変更時に実行するコマンド
        /// </summary>
        public ICommand SupplierChanged { get; } = new ChangeInspectionSupplierCommand();
        #endregion // コマンド

        /// <summary>
        /// 発注情報を読み込む
        /// </summary>
        public void ReloadOrders()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod(msg: $"{SelectedSupplier?.SupplierName}, {SelectedKey.Key}, {From:yyyyMMdd} - {To:yyyyMMdd}");

                if(To < From)
                {
                    LogUtil.Debug($"Skip reloading... To: {To:yyyyMMdd}->{From:yyyyMMdd}");
                    To = From;
                    return;
                }

                if (SelectedKey.Key == DateSelectionKey.ORDERED)
                {
                    if (SelectedSupplier == null || SelectedSupplier.SupplierCode < 1)
                    {
                        ReloadOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersToSupplier()
                            .Where(o => From <= o.OrderDate && o.OrderDate <= To));
                    }
                    else
                    {
                        ReloadOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersToSupplier()
                            .Where(o => o.Supplier == SelectedSupplier.SupplierCode)
                            .Where(o => From <= o.OrderDate && o.OrderDate <= To));
                    }
                }
                else
                {
                    if (SelectedSupplier == null || SelectedSupplier.SupplierCode < 1)
                    {
                        ReloadOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersToSupplier()
                            .Where(o => From <= o.DeliveryDate && o.DeliveryDate <= To)
                            .OrderBy(o => o.DeliveryDate));
                    }
                    else
                    {
                        ReloadOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersToSupplier()
                            .Where(o => o.Supplier == SelectedSupplier.SupplierCode)
                            .Where(o => From <= o.DeliveryDate && o.DeliveryDate <= To)
                            .OrderBy(o => o.DeliveryDate));
                    }
                }
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }


        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            LoadSuppliers();

            ReloadOrders(MemorieDeFleursUIModel.Instance.FindAllOrdersToSupplier());

            if(Orders.Count > 0)
            {
                _from = Orders.FirstOrDefault().OrderDate;
                _to = Orders.LastOrDefault().OrderDate;
                RaisePropertyChanged(nameof(From), nameof(To));
            }

            SelectedSupplier = Suppliers[0];
        }
        private void LoadSuppliers()
        {
            Suppliers.Clear();
            Suppliers.Add(new SupplierSummaryViewModel() { SupplierCode = -1, SupplierName = TextResourceFinder.FindText("AllSuppliers") });
            foreach (var s in MemorieDeFleursUIModel.Instance.FindAllSuppliers())
            {
                Suppliers.Add(new SupplierSummaryViewModel(s));
            }
            RaisePropertyChanged(nameof(Suppliers));
        }
        #endregion // IReloadable

        private void ReloadOrders(IEnumerable<OrdersToSupplier> orders)
        {
            foreach(var o in Orders)
            {
                o.Inspected -= HandleReloadOrders;
            }
            Orders.Clear();

            foreach (var o in orders)
            {
                var summary = new OrderToSupplierInspectionSummaryViewModel(o);
                Subscribe(summary);
                summary.Inspected += HandleReloadOrders;
                Orders.Add(summary);
            }
            RaisePropertyChanged(nameof(Orders));
        }

        private void HandleReloadOrders(object sender, EventArgs unused)
        {
            ReloadOrders();
        }
    }
}
