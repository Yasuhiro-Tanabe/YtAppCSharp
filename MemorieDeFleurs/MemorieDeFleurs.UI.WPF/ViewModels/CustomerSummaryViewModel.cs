using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using YasT.Framework.Logging;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 得意先詳細画面のビューモデル
    /// </summary>
    public class CustomerSummaryViewModel : ListItemViewModelBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomerSummaryViewModel() : base(new OpenDetailViewCommand<CustomerDetailViewModel>()) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="customer">表示する得意先エンティティ</param>
        public CustomerSummaryViewModel(Customer customer) : this()
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

        private void Update(Customer customer)
        {
            Update(customer.ID.ToString());
            CustomerID = customer.ID;
            CustomerName = customer.Name;
            LogUtil.DEBUGLOG_MethodCalled($"#{CustomerID}: {CustomerName}");
        }
    }
}
