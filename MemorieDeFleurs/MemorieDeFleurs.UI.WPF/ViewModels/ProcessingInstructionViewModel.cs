using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class ProcessingInstructionViewModel : DetailViewModelBase, IPrintable, IReloadable
    {
        public ProcessingInstructionViewModel() : base("加工指示書") { }

        #region プロパティ
        /// <summary>
        /// 加工日
        /// </summary>
        public DateTime ProcessingDate
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }
        private DateTime _date = DateTime.Today;

        /// <summary>
        /// 商品一覧
        /// </summary>
        public ObservableCollection<BouquetSummaryViewModel> Bouquets { get; } = new ObservableCollection<BouquetSummaryViewModel>();

        /// <summary>
        /// 現在選択中の商品
        /// </summary>
        public BouquetSummaryViewModel SelectedBouquet
        {
            get { return _bouquet; }
            set { SetProperty(ref _bouquet, value); }
        }
        private BouquetSummaryViewModel _bouquet;

        /// <summary>
        /// 現在選択中の商品の花コード
        /// </summary>
        public string SelectedBouquetCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        /// <summary>
        /// 加工数 (花束の数)
        /// </summary>
        public int NumberOfBouquet
        {
            get { return _numProcess; }
            set { SetProperty(ref _numProcess, value); }
        }
        private int _numProcess;

        /// <summary>
        /// 商品構成
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> Parts { get; } = new ObservableCollection<PartsListItemViewModel>();

        /// <summary>
        /// 選択中の商品の加工日当日分の商品をすべて発送したかどうか
        /// </summary>
        public bool IsShippedAll
        {
            get { return _shippedAll; }
            set { SetProperty(ref _shippedAll, value); }
        }
        private bool _shippedAll = false;
        #endregion // プロパティ

        #region コマンド
        public ICommand Print { get; } = new PrintCommand();
        public ICommand ChangeDate { get; } = new ChangeProcessingDateCommand();
        public ICommand ChangeBouquet { get; } = new ChangeProcessingBouquetCommand();
        public ICommand Ship { get; } = new ShippingOrdersCommand();
        #endregion // コマンド

        #region IReloadable
        /// <inheritdoc/>
        public void UpdateProperties()
        {
            // 無条件更新は行わない：画面更新は日付と商品が選択されたタイミングで必要な箇所だけ行う。
        }
        #endregion // IReloadable

        #region IPrintable
        public void ValidateBeforePrinting() { }
        #endregion // IPrintable

        public void ChangeProcessingDate()
        {
            Bouquets.Clear();

            var found = MemorieDeFleursUIModel.Instance.GetShippingBouquetCountAt(ProcessingDate);
            foreach (var bouquet in MemorieDeFleursUIModel.Instance.FindAllBouquets().Where(b => found[b.Code] > 0))
            {
                // 当日出荷予定 (found[花コード] > 0) の花コードだけ追加
                Bouquets.Add(new BouquetSummaryViewModel(bouquet));
            }

            SelectedBouquet = null;
            IsShippedAll = false;
            RaisePropertyChanged(nameof(Bouquets));
        }

        public void ChangeProcessingBouquet()
        {
            Parts.Clear();

            if (SelectedBouquet == null)
            {
                SelectedBouquetCode = string.Empty;
                NumberOfBouquet = 0;
                IsShippedAll = false;
            }
            else
            {
                var bouquet = MemorieDeFleursUIModel.Instance.FindBouquet(SelectedBouquet.BouquetCode);
                if (bouquet == null)
                {
                    throw new ApplicationException($"該当する商品が見つかりません：{SelectedBouquet.BouquetCode}");
                }
                else
                {
                    SelectedBouquetCode = SelectedBouquet.BouquetCode;
                    NumberOfBouquet = MemorieDeFleursUIModel.Instance.GetNumberOfProcessingBouquetsOf(SelectedBouquetCode, ProcessingDate);
                    foreach (var parts in bouquet.PartsList)
                    {
                        Parts.Add(new PartsListItemViewModel(parts) { QuantityPerLot = NumberOfBouquet });
                    }
                    UpdateShippedAll();
                }
            }

            RaisePropertyChanged(nameof(Parts));
        }

        public void ShipBouquets()
        {
            if(!IsShippedAll)
            {
                var orders = MemorieDeFleursUIModel.Instance.FindAllOrdersFromCustomer()
                    .Where(o => o.ShippingDate == ProcessingDate)
                    .Where(o => o.BouquetCode == SelectedBouquetCode)
                    .Where(o => o.Status != OrderFromCustomerStatus.SHIPPED)
                    .Select(o => o.ID)
                    .ToArray();
                MemorieDeFleursUIModel.Instance.ShipBouquetOrders(ProcessingDate, orders);
                UpdateShippedAll();
                LogUtil.Info($"{ProcessingDate:yyyyMMdd}, {SelectedBouquetCode}, Shiped orders: {string.Join(", ", orders)}");
            }
        }

        private void UpdateShippedAll()
        {
            IsShippedAll = MemorieDeFleursUIModel.Instance.FindAllOrdersFromCustomer()
                .Where(o => o.BouquetCode == SelectedBouquetCode)
                .Where(o => o.ShippingDate == ProcessingDate)
                .All(o => o.Status == OrderFromCustomerStatus.SHIPPED);
        }
    }
}
