using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetListViewModel : TabItemControlViewModelBase
    {
        public event EventHandler DetailViewOpening;

        public BouquetListViewModel() : base("商品一覧") { }

        #region プロパティ
        public ObservableCollection<BouquetSummaryViewModel> Bouquets { get; } = new ObservableCollection<BouquetSummaryViewModel>();

        public BouquetSummaryViewModel CurrentBouquet { get; set; }
        #endregion // プロパティ

        #region コマンド
        public ICommand Reload { get; } = new ReloadListCommand();
        public ICommand Selected { get; } = new SelectionChangedEventCommand();
        #endregion // コマンド

        public void LoadBouquets()
        {
            Bouquets.Clear();
            foreach(var b in MemorieDeFleursUIModel.Instance.FindAllBouquets())
            {
                var vm = new BouquetSummaryViewModel(b);
                vm.PropertyChanged += SummaryViewModelChanged;
                vm.DetailViewOpening += OpenDetailView;
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
                LogUtil.Debug($"{vm.BouquetCode} deleted.");
                LoadBouquets();
            }
        }

        private void OpenDetailView(object sender, EventArgs unused)
        {
            LogUtil.DEBULOG_MethodCalled(sender.GetType().Name);
            DetailViewOpening?.Invoke(this, null);
        }
    }
}
