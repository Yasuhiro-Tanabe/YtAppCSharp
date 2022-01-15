using MemorieDeFleurs.Models.Entities;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 様々なビューで利用する、単品一覧の各要素である単品の内容を表示するビューモデル
    /// </summary>
    public class PartsListItemViewModel : NotificationObject
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

        /// <summary>
        /// 数量：本数あるいはロット数。
        /// 
        /// どちらを格納しているかは、元データと表示するビューにより異なる。
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }
        private int _quantity;

        /// <summary>
        /// 発注リードタイム [日]
        /// </summary>
        public int LeadTime
        {
            get { return _leadtime; }
            set { SetProperty(ref _leadtime, value); }
        }
        private int _leadtime;

        /// <summary>
        /// 購入単位数
        /// </summary>
        public int QuantityPerLot
        {
            get { return _quantityPerLot; }
            set { SetProperty(ref _quantityPerLot, value); }
        }
        private int _quantityPerLot;
        #endregion // プロパティ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="item">商品を構成する単品</param>
        public PartsListItemViewModel(BouquetPartsList item)
        {
            PartsCode = item.PartsCode;
            PartsName = item.Part.Name;
            Quantity = item.Quantity;
            LeadTime = item.Part.LeadTime;
            QuantityPerLot = item.Part.QuantitiesPerLot;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="parts">単品</param>
        public PartsListItemViewModel(BouquetPart parts)
        {
            PartsCode = parts.Code;
            PartsName = parts.Name;
            Quantity = 0;
            LeadTime = parts.LeadTime;
            QuantityPerLot = parts.QuantitiesPerLot;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="detail">仕入先発注明細としての単品</param>
        public PartsListItemViewModel(OrderDetailsToSupplier detail)
        {
            PartsCode = detail.PartsCode;
            PartsName = detail.BouquetPart.Name;
            Quantity = detail.LotCount;
            LeadTime = detail.BouquetPart.LeadTime;
            QuantityPerLot = detail.BouquetPart.QuantitiesPerLot;
        }
    }
}
