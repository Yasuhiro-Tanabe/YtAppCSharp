﻿using MemorieDeFleurs.Logging;
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
            detail.Update();
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name, $"{detail.GetType().Name} opened.");
            return detail;
        }
    }
}
