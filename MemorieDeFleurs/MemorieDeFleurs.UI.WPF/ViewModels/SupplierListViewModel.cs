using MemorieDeFleurs.UI.WPF.Model;

using System.Collections.ObjectModel;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class SupplierListViewModel : ListViewModelBase
    {
        public SupplierListViewModel() : base("仕入先一覧") { }

        #region プロパティ
        public ObservableCollection<SupplierSummaryViewModel> Suppliers { get; } = new ObservableCollection<SupplierSummaryViewModel>();

        public SupplierSummaryViewModel CurrentSupplier { get; set; }
        #endregion // プロパティ

        public override void LoadItems()
        {
            Suppliers.Clear();
            foreach(var s in MemorieDeFleursUIModel.Instance.FindAllSuppliers())
            {
                var vm = new SupplierSummaryViewModel(s);
                Subscribe(vm);
                Suppliers.Add(vm);
            }
            CurrentSupplier = null;
            RaisePropertyChanged(nameof(Suppliers), nameof(CurrentSupplier));
        }

        protected override void RemoveSelectedItem(object sender)
        {
            var vm = sender as SupplierSummaryViewModel;
            MemorieDeFleursUIModel.Instance.RemoveSupplier(vm.SupplierCode);
            LoadItems();
        }
    }
}
