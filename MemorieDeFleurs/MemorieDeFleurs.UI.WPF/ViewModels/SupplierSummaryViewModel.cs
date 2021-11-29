using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class SupplierSummaryViewModel : ListItemViewModelBase
    {
        public SupplierSummaryViewModel() : base(new OpenDetailViewCommand<SupplierDetailViewModel>()) { }
        public SupplierSummaryViewModel(Supplier supplier) : this()
        {
            Update(supplier);
        }

        #region プロパティ
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public int SupplierCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private int _code;

        /// <summary>
        /// 仕入先名称
        /// </summary>
        public string SupplierName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        #endregion // プロパティ


        public void Update(Supplier supplier)
        {
            Update(supplier.Code.ToString());
            SupplierCode = supplier.Code;
            SupplierName = supplier.Name;
            RaisePropertyChanged(nameof(SupplierCode), nameof(SupplierName));
        }
    }
}
