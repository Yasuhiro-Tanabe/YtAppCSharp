using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class CustomerSummaryViewModel : ListItemViewModelBase
    {
        public CustomerSummaryViewModel() : base(new OpenCustomerDetailViewCommand()) { }

        public CustomerSummaryViewModel(Customer customer) : base(new OpenCustomerDetailViewCommand())
        {
            Update(customer);
        }

        #region プロパティ
        /// <summary>
        /// 得意先ID
        /// </summary>
        public int CustomerID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        private int _id;

        /// <summary>
        /// 得意先名称
        /// </summary>
        public string CustomerName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;
        #endregion // プロパティ

        public void Update(Customer customer)
        {
            Update(customer.ID.ToString());
            CustomerID = customer.ID;
            CustomerName = customer.Name;
            LogUtil.DEBUGLOG_MethodCalled($"#{CustomerID}: {CustomerName}");
        }
    }
}
