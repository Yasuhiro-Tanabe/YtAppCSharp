using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Collections.ObjectModel;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class SupplierListViewModel : ListViewModelBase, IReloadable
    {
        public SupplierListViewModel() : base("仕入先一覧") { }

        #region プロパティ
        /// <summary>
        /// 全仕入先の一覧
        /// </summary>
        public ObservableCollection<SupplierSummaryViewModel> Suppliers { get; } = new ObservableCollection<SupplierSummaryViewModel>();

        /// <summary>
        /// 現在選択中の仕入先
        /// </summary>
        public SupplierSummaryViewModel CurrentSupplier
        {
            get { return _supplier; }
            set { SetProperty(ref _supplier, value); }
        }
        private SupplierSummaryViewModel _supplier;
        #endregion // プロパティ

        #region IReloadable
        /// <inheritdoc/>
        public void UpdateProperties()
        {
            Suppliers.Clear();
            foreach (var s in MemorieDeFleursUIModel.Instance.FindAllSuppliers())
            {
                var vm = new SupplierSummaryViewModel(s);
                Subscribe(vm);
                Suppliers.Add(vm);
            }
            CurrentSupplier = null;
            RaisePropertyChanged(nameof(Suppliers));
        }
        #endregion // IReloadable

        protected override void RemoveSelectedItem(object sender)
        {
            var vm = sender as SupplierSummaryViewModel;
            MemorieDeFleursUIModel.Instance.RemoveSupplier(vm.SupplierCode);
            LogUtil.Debug($"{vm.SupplierCode} deleted.");
            UpdateProperties();
        }

        public override DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM)
        {
            LogUtil.DEBUGLOG_BeginMethod(mainVM.GetType().Name);
            var detail = mainVM.FindTabItem(SupplierDetailViewModel.Name) as SupplierDetailViewModel;
            if(detail == null)
            {
                detail = new SupplierDetailViewModel();
                mainVM.OpenTabItem(detail);
            }
            detail.SupplierCode = CurrentSupplier.SupplierCode;
            detail.UpdateProperties();
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name, $"{detail.GetType().Name} opened.");
            return detail;
        }
    }
}
