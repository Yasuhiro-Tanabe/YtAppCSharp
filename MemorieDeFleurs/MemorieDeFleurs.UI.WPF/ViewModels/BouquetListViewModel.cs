using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 商品一覧画面のビューモデル
    /// </summary>
    public class BouquetListViewModel : ListViewModelBase, IReloadable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BouquetListViewModel() : base("商品一覧") { }

        #region プロパティ
        /// <summary>
        /// 全商品の一覧
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
        #endregion // プロパティ

        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            Bouquets.Clear();
            foreach (var b in MemorieDeFleursUIModel.Instance.FindAllBouquets())
            {
                var vm = new BouquetSummaryViewModel(b);
                Subscribe(vm);
                Bouquets.Add(vm);
            }
            SelectedBouquet = null;
            RaisePropertyChanged(nameof(Bouquets));
        }
        #endregion // IReloadable

        /// <inheritdoc/>
        protected override void RemoveSelectedItem(object sender)
        {
            var vm = sender as BouquetSummaryViewModel;
            MemorieDeFleursUIModel.Instance.RemoveBouquet(vm.BouquetCode);
            LogUtil.Debug($"{vm.BouquetCode} deleted.");
            UpdateProperties();
        }

        /// <inheritdoc/>
        public override DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM)
        {
            LogUtil.DEBUGLOG_BeginMethod(mainVM.GetType().Name);
            var detail = mainVM.FindTabItem(BouquetDetailViewModel.Name) as BouquetDetailViewModel;

            if (detail == null)
            {
                detail = new BouquetDetailViewModel();
                mainVM.OpenTabItem(detail);
            }

            detail.BouquetCode = SelectedBouquet.BouquetCode;
            detail.UpdateProperties();
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name);
            return detail;
        }
    }
}
