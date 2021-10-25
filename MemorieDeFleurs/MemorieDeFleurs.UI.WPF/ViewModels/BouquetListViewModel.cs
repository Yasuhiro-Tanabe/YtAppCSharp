using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetListViewModel : TabItemControlViewModelBase
    {
        public BouquetListViewModel() : base("商品一覧") { }

        #region プロパティ
        public ObservableCollection<BouquetSummaryViewModel> Bouquets { get; } = new ObservableCollection<BouquetSummaryViewModel>();

        public BouquetSummaryViewModel CurrentBouquet { get; set; }
        #endregion // プロパティ

        #region コマンド
        public ICommand Reload { get; } = new ReloadListCommand();
        public ICommand Selected { get; } = new SelectedListItemCommand();
        #endregion // コマンド

        public void LoadBouquets()
        {
            Bouquets.Clear();
            foreach(var b in MemorieDeFleursUIModel.Instance.FindAllBouquets())
            {
                var vm = new BouquetSummaryViewModel(b);
                vm.PropertyChanged += SummaryViewModelChanged;
                Bouquets.Add(vm);
            }
            CurrentBouquet = null;
            RaisePropertyChanged(nameof(Bouquets), nameof(CurrentBouquet));
        }

        private void SummaryViewModelChanged(object sender, PropertyChangedEventArgs args)
        {
            var vm = sender as BouquetSummaryViewModel;
            if (args.PropertyName == nameof(BouquetSummaryViewModel.RemoveMe))
            {
                MemorieDeFleursUIModel.Instance.RemoveBouquet(vm.BouquetCode);
                LogUtil.Debug($"{vm.BouquetCode} removed.");
                LoadBouquets();
            }
        }
    }
}
