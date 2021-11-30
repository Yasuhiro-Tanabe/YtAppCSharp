using MemorieDeFleurs.Models.Entities;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 単品仕入先一覧の各要素を表示するビューモデル
    /// </summary>
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="parts">単品</param>
        public SupplierPartsViewModel(BouquetPart parts)
        {
            PartsCode = parts.Code;
            PartsName = parts.Name;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="parts">仕入先に関連付けられた単品</param>
        public SupplierPartsViewModel(PartSupplier parts)
        {
            PartsCode = parts.PartCode;
            PartsName = parts.Part.Name;
        }
    }
}
