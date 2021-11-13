﻿using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Collections.ObjectModel;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsListViewModel : ListViewModelBase
    {
        public BouquetPartsListViewModel() : base("単品一覧") { }

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

        public override void LoadItems()
        {
            BouquetParts.Clear();
            foreach(var p in MemorieDeFleursUIModel.Instance.FindAllBouquetParts())
            {
                var vm = new BouquetPartsSummaryViewModel(p);
                Subscribe(vm);
                BouquetParts.Add(vm);
            }
            SelectedParts = null;
            RaisePropertyChanged(nameof(BouquetParts));
        }

        protected override void RemoveSelectedItem(object sender)
        {
            var vm = sender as BouquetPartsSummaryViewModel;
            MemorieDeFleursUIModel.Instance.RemoveBouquetParts(vm.PartsCode);
            LogUtil.Debug($"{vm.PartsCode} deleted.");
            LoadItems();
        }

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
            detail.Update();
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name);
            return detail;
        }
    }
}
