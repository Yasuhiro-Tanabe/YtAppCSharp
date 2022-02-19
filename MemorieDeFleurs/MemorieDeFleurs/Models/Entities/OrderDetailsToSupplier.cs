using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 発注明細
    /// </summary>
    [Table("ORDER_DETAILS_TO_SUPPLIER")]
    public class OrderDetailsToSupplier
    {
        /// <summary>
        /// 発注番号
        /// </summary>
        [Column("ORDER_TO_SUPPLIER_ID")]
        public string OrderToSupplierID { get; set; }

        /// <summary>
        /// 明細番号
        /// </summary>
        [Column("ORDER_INDEX")]
        public int OrderIndex { get; set; }

        /// <summary>
        /// 花コード
        /// </summary>
        [Column("PARTS_CODE")]
        public string PartsCode { get; set; }

        /// <summary>
        /// ロット数
        /// </summary>
        [Column("LOT_COUNT")]
        public int LotCount { get; set; }

        /// <summary>
        /// 在庫ロット番号
        /// </summary>
        [Column("INVENTORY_LOT_NO")]
        public int InventoryLotNo { get; set; }

        /// <summary>
        /// 受注オブジェクト：このオブジェクトをリスト要素として持つ、受注番号で関連付けられたエンティティオブジェクト
        /// </summary>
        [ForeignKey(nameof(OrderToSupplierID))]
        public OrdersToSupplier Order { get; set; }

        /// <summary>
        /// 単品オブジェクト：このオブジェクトの花コードで関連付けられたエンティティオブジェクト
        /// </summary>
        [ForeignKey(nameof(PartsCode))]
        public BouquetPart BouquetPart { get; set; }
    }
}


