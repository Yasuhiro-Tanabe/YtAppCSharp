using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 仕入先一覧画面内の各仕入先を表示するビューモデル
    /// </summary>
    public class SupplierSummaryViewModel : ListItemViewModelBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SupplierSummaryViewModel() : base(new OpenDetailViewCommand<SupplierDetailViewModel>()) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="supplier">仕入先情報</param>
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


        private void Update(Supplier supplier)
        {
            Update(supplier.Code.ToString());
            SupplierCode = supplier.Code;
            SupplierName = supplier.Name;
            RaisePropertyChanged(nameof(SupplierCode), nameof(SupplierName));
        }
    }
}
