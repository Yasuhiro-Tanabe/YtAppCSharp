using MemorieDeFleurs.Models.Entities;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 検品対象単品のビューモデル
    /// </summary>
    public class InspectionPartsListItemViewModel : NotificationObject
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
        /// 発注時の本数 (ロット数×発注単位)
        /// </summary>
        public int OrderedQuantity
        {
            get { return _ordered; }
            set { SetProperty(ref _ordered, value); }
        }
        private int _ordered;

        /// <summary>
        /// 検品合格本数
        /// </summary>
        public int ActualQuantity
        {
            get { return _actual; }
            set { SetProperty(ref _actual, value); }
        }
        private int _actual;

        /// <summary>
        /// 単品エンティティオブジェクト
        /// </summary>
        public BouquetPart BouquetPart
        {
            get { return _part; }
            set
            {
                SetProperty(ref _part, value);
                PartsCode = _part.Code;
                PartsName = _part.Name;
            }
        }
        private BouquetPart _part;

        /// <summary>
        /// 発注時と検品時で本数に差異があったかどうかを判定する
        /// </summary>
        public bool IsQuantityChanged
        {
            get { return OrderedQuantity != ActualQuantity; }
        }

        /// <summary>
        /// この単品を含む発注履歴が検品済かどうか
        /// </summary>
        public bool IsInspected
        {
            get { return _inspected; }
            set { SetProperty(ref _inspected, value); }
        }
        private bool _inspected;
        #endregion // プロパティ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="order">検品対象として表示する仕入先発注明細エンティティ</param>
        public InspectionPartsListItemViewModel(OrderDetailsToSupplier order)
        {
            BouquetPart = order.BouquetPart;
            OrderedQuantity = order.LotCount * order.BouquetPart.QuantitiesPerLot;
            ActualQuantity = OrderedQuantity;
        }
    }
}
