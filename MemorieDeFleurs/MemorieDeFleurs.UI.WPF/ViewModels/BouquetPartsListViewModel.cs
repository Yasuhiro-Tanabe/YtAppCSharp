using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Collections.ObjectModel;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 単品一覧画面のビューモデル
    /// </summary>
    public class BouquetPartsListViewModel : ListViewModelBase, IReloadable
    {
        /// <summary>
        /// ビューモデルの名称：<see cref="TabItemControlViewModelBase.Header"/> や <see cref="MainWindowViiewModel.FindTabItem(string)"/> に渡すクラス定数として使用する。
        /// </summary>
        public static string Name { get { return TextResourceFinder.FindText("Parts_List"); } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BouquetPartsListViewModel() : base(Name) { }

        #region プロパティ
        /// <summary>
        /// 全単品の一覧
        /// </summary>
        public ObservableCollection<BouquetPartsSummaryViewModel> BouquetParts { get; } = new ObservableCollection<BouquetPartsSummaryViewModel>();

        /// <summary>
        /// 現在選択中の単品
        /// </summary>
        public BouquetPartsSummaryViewModel SelectedParts
        {
            get { return _parts; }
            set { SetProperty(ref _parts, value); }
        }
        private BouquetPartsSummaryViewModel _parts;
        #endregion // プロパティ

        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            BouquetParts.Clear();
            foreach (var p in MemorieDeFleursUIModel.Instance.FindAllBouquetParts())
            {
                var vm = new BouquetPartsSummaryViewModel(p);
                Subscribe(vm);
                BouquetParts.Add(vm);
            }
            SelectedParts = null;
            RaisePropertyChanged(nameof(BouquetParts));
        }
        #endregion // IReloadable

        /// <inheritdoc/>
        protected override void RemoveSelectedItem(object sender)
        {
            var vm = sender as BouquetPartsSummaryViewModel;
            MemorieDeFleursUIModel.Instance.RemoveBouquetParts(vm.PartsCode);
            LogUtil.Debug($"{vm.PartsCode} deleted.");
            UpdateProperties();
        }

        /// <inheritdoc/>
        public override DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM)
        {
            LogUtil.DEBUGLOG_BeginMethod(mainVM.GetType().Name);
            var detail = mainVM.FindTabItem(BouquetPartsDetailViewModel.Name) as BouquetPartsDetailViewModel;
            if (detail == null)
            {
                detail = new BouquetPartsDetailViewModel();
                mainVM.OpenTabItem(detail);
            }
            detail.PartsCode = SelectedParts.PartsCode;
            detail.UpdateProperties();
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name);
            return detail;
        }
    }
}
