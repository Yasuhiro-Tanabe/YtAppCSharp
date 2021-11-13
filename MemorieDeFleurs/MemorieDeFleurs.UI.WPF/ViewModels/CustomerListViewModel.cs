using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Collections.ObjectModel;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class CustomerListViewModel : ListViewModelBase
    {
        public CustomerListViewModel() : base("得意先一覧") { }

        #region プロパティ
        /// <summary>
        /// 全得意先の一覧
        /// </summary>
        public ObservableCollection<CustomerSummaryViewModel> Customers { get; } = new ObservableCollection<CustomerSummaryViewModel>();

        /// <summary>
        /// 現在選択中の得意先
        /// </summary>
        public CustomerSummaryViewModel SelectedCustomer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }
        private CustomerSummaryViewModel _customer;
        #endregion // プロパティ

        public override void LoadItems()
        {
            LogUtil.DEBUGLOG_BeginMethod();
            Customers.Clear();
            foreach(var c in MemorieDeFleursUIModel.Instance.FindAllCustomers())
            {
                var vm = new CustomerSummaryViewModel(c);
                Subscribe(vm);
                Customers.Add(vm);
                LogUtil.Debug($"[#{c.ID}:{c.Name}]");
            }
            SelectedCustomer = null;
            LogUtil.DEBUGLOG_EndMethod();
        }

        protected override void RemoveSelectedItem(object sender)
        {
            var vm = sender as CustomerSummaryViewModel;
            MemorieDeFleursUIModel.Instance.RemoveCustomer(vm.CustomerID);
            LoadItems();
        }

        public override DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM)
        {
            LogUtil.DEBUGLOG_BeginMethod(mainVM.GetType().Name);
            var detail = mainVM.FindTabItem(CustomerDetailViewModel.Name) as CustomerDetailViewModel;
            if(detail == null)
            {
                detail = new CustomerDetailViewModel();
                mainVM.OpenTabItem(detail);
            }
            detail.CustomerID = SelectedCustomer.CustomerID;
            detail.Update();
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name);
            return detail;
        }

    }
}
