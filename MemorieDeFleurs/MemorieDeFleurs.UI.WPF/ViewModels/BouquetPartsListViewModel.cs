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
    public class BouquetPartsListViewModel : ListViewModelBase
    {
        public BouquetPartsListViewModel() : base("単品一覧") { }

        #region プロパティ
        public ObservableCollection<BouquetPartsSummaryViewModel> BouquetParts { get; } = new ObservableCollection<BouquetPartsSummaryViewModel>();

        public BouquetPartsSummaryViewModel CurrentParts { get; set; }
        #endregion // プロパティ

        public override void LoadItems()
        {
            BouquetParts.Clear();
            foreach(var p in MemorieDeFleursUIModel.Instance.FindAllBouquetParts())
            {
                var vm = new BouquetPartsSummaryViewModel(p);
                Subscribe(vm);
                BouquetParts.Add(vm);
            }
            CurrentParts = null;
            RaisePropertyChanged(nameof(BouquetParts), nameof(CurrentParts));
        }

        //private void SummaryViewModelChanged(object sender, PropertyChangedEventArgs args)
        //{
        //    var vm = sender as BouquetPartsSummaryViewModel;
        //    if(args.PropertyName == nameof(BouquetPartsSummaryViewModel.RemoveMe))
        //    {
        //        MemorieDeFleursUIModel.Instance.RemoveBouquetParts(vm.PartsCode);
        //        LogUtil.Debug($"{vm.PartsCode} deleted.");
        //        LoadItems();
        //    }
        //}
    }
}
