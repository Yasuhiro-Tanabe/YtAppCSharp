using MemorieDeFleurs.Models.Entities;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
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
        #endregion // プロパティ

        public PartsListItemViewModel(BouquetPartsList item)
        {
            PartsCode = item.PartsCode;
            PartsName = item.Part.Name;
            Quantity = item.Quantity;
            LeadTime = item.Part.LeadTime;
        }

        public PartsListItemViewModel(BouquetPart parts)
        {
            PartsCode = parts.Code;
            PartsName = parts.Name;
            Quantity = 0;
            LeadTime = parts.LeadTime;
        }

        public PartsListItemViewModel(OrderDetailsToSupplier detail)
        {
            PartsCode = detail.PartsCode;
            PartsName = detail.BouquetPart.Name;
            Quantity = detail.LotCount;
            LeadTime = detail.BouquetPart.LeadTime;
        }
    }
}
