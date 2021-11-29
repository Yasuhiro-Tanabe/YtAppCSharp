using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsSummaryViewModel : ListItemViewModelBase
    {
        public BouquetPartsSummaryViewModel(BouquetPart part) : base(new OpenDetailViewCommand<BouquetPartsDetailViewModel>())
        {
            Update(part);
        }

        public void Update(BouquetPart part)
        {
            PartsCode = part.Code;
            PartsName = part.Name;
        }

        #region プロパティ
        /// <summary>
        /// 花コード
        /// </summary>
        public string PartsCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        /// <summary>
        /// 単品名称
        /// </summary>
        public string PartsName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;
        #endregion // プロパティ
    }
}
