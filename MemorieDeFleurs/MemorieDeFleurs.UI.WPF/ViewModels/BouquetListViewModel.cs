using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetListViewModel : ListViewModelBase
    {

        public BouquetListViewModel() : base("商品一覧") { }

        #region プロパティ
        public ObservableCollection<BouquetSummaryViewModel> Bouquets { get; } = new ObservableCollection<BouquetSummaryViewModel>();

        public BouquetSummaryViewModel CurrentBouquet { get; set; }
        #endregion // プロパティ

        public override void LoadItems()
        {
            Bouquets.Clear();
            foreach(var b in MemorieDeFleursUIModel.Instance.FindAllBouquets())
            {
                var vm = new BouquetSummaryViewModel(b);
                Subscribe(vm);
                Bouquets.Add(vm);
            }
            CurrentBouquet = null;
            RaisePropertyChanged(nameof(Bouquets), nameof(CurrentBouquet));
        }

        protected override void RemoveSelectedItem(object sender)
        {
            var vm = sender as BouquetSummaryViewModel;
            MemorieDeFleursUIModel.Instance.RemoveBouquet(vm.BouquetCode);
            LogUtil.Debug($"{vm.BouquetCode} deleted.");
            LoadItems();
        }

        public override DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM)
        {
            LogUtil.DEBUGLOG_BeginMethod(mainVM.GetType().Name);
            var detail = mainVM.FindTabItem(BouquetDetailViewModel.Name) as BouquetDetailViewModel;

            if (detail == null)
            {
                detail = new BouquetDetailViewModel();
                mainVM.OpenTabItem(detail);
            }

            detail.BouquetCode = CurrentBouquet.BouquetCode;
            detail.Update();
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name);
            return detail;
        }
    }
}
