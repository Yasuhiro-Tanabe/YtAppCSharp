using MemorieDeFleurs.Models.Entities;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class SupplierPartsViewModel : NotificationObject
    {
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

        public SupplierPartsViewModel(BouquetPart parts)
        {
            PartsCode = parts.Code;
            PartsName = parts.Name;
        }
        public SupplierPartsViewModel(PartSupplier parts)
        {
            PartsCode = parts.PartCode;
            PartsName = parts.Part.Name;
        }
    }
}
