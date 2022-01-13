using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Collections.ObjectModel;

using YasT.Framework.Logging;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 得意先一覧画面のビューモデル
    /// </summary>
    public class CustomerListViewModel : ListViewModelBase, IReloadable
    {
        /// <summary>
        /// ビューモデルの名称：<see cref="TabItemControlViewModelBase.Header"/> や <see cref="MainWindowViiewModel.FindTabItem(string)"/> に渡すクラス定数として使用する。
        /// </summary>
        public static string Name { get { return TextResourceFinder.FindText("Customer_List"); } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomerListViewModel() : base(Name) { }

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

        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();
                Customers.Clear();
                foreach (var c in MemorieDeFleursUIModel.Instance.FindAllCustomers())
                {
                    var vm = new CustomerSummaryViewModel(c);
                    Subscribe(vm);
                    Customers.Add(vm);
                    LogUtil.Debug($"[#{c.ID}:{c.Name}]");
                }
                SelectedCustomer = null;
                RaisePropertyChanged(nameof(Customers));
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }
        #endregion // IReloadable

        /// <inheritdoc/>
        protected override void RemoveSelectedItem(object sender)
        {
            var vm = sender as CustomerSummaryViewModel;
            MemorieDeFleursUIModel.Instance.RemoveCustomer(vm.CustomerID);
            UpdateProperties();
        }

        /// <inheritdoc/>
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
            detail.UpdateProperties();
            LogUtil.DEBUGLOG_EndMethod(mainVM.GetType().Name);
            return detail;
        }

    }
}
