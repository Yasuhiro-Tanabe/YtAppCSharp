﻿using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsListViewModel : TabItemControlViewModelBase
    {
        public event EventHandler DetailViewOpening;

        public BouquetPartsListViewModel() : base("単品一覧") { }

        #region プロパティ
        public ObservableCollection<BouquetPartsSummaryViewModel> BouquetParts { get; } = new ObservableCollection<BouquetPartsSummaryViewModel>();

        public BouquetPartsSummaryViewModel CurrentParts { get; set; }
        #endregion // プロパティ

        #region Command
        public ICommand Reload { get; } = new ReloadListCommand();
        public ICommand Selected { get; } = new SelectionChangedEventCommand();
        #endregion // Command

        public void LoadBouquetParts()
        {
            BouquetParts.Clear();
            foreach(var p in MemorieDeFleursUIModel.Instance.FindAllBouquetParts())
            {
                var vm = new BouquetPartsSummaryViewModel(p);
                vm.PropertyChanged += SummaryViewModelChanged;
                vm.DetailViewOpening += OpenDetailView;
                BouquetParts.Add(vm);
            }
            CurrentParts = null;
            RaisePropertyChanged(nameof(BouquetParts), nameof(CurrentParts));
        }

        private void SummaryViewModelChanged(object sender, PropertyChangedEventArgs args)
        {
            var vm = sender as BouquetPartsSummaryViewModel;
            if(args.PropertyName == nameof(BouquetPartsSummaryViewModel.RemoveMe))
            {
                MemorieDeFleursUIModel.Instance.RemoveBouquetParts(vm.PartsCode);
                LogUtil.Debug($"{vm.PartsCode} deleted.");
                LoadBouquetParts();
            }
        }

        private void OpenDetailView(object sender, EventArgs unused)
        {
            LogUtil.DEBULOG_MethodCalled(sender.GetType().Name);
            DetailViewOpening?.Invoke(this, null);
        }
    }
}
