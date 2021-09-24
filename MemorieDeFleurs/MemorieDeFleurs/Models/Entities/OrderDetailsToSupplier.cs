using System.ComponentModel.DataAnnotations;
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

        
        [ForeignKey(nameof(OrderToSupplierID))]
        public OrdersToSupplier Order { get; set; }

        [ForeignKey(nameof(PartsCode))]
        public BouquetPart BouquetPart { get; set; }
    }
}


