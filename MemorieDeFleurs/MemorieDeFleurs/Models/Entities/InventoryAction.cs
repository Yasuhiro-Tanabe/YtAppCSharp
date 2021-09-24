using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 在庫アクション
    /// </summary>
    [Table("INVENTORY_ACTIONS")]
    public class InventoryAction
    {
        /// <summary>
        /// 基準日
        /// </summary>
        [Column("ACTION_DATE")]
        public DateTime ActionDate { get; set; }

        /// <summary>
        /// アクション
        /// </summary>
        [Column("ACTION")]
        public InventoryActionType Action { get; set; }

        /// <summary>
        /// 花コード
        /// </summary>
        [Column("BOUQUET_PARTS_CODE")]
        public string PartsCode { get; set; }

        /// <summary>
        /// 入荷日
        /// </summary>
        [Column("ARRIVAL_DATE")]
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// 在庫ロット番号
        /// </summary>
        [Column("LOT_NO")]
        public int InventoryLotNo { get; set; }

        /// <summary>
        /// 数量：入荷(予定)数、加工(予定)数、破棄(予定)数
        /// </summary>
        [Column("QUANTITY")]
        public int Quantity { get; set; }

        /// <summary>
        /// 残数
        /// </summary>
        [Column("REMAIN")]
        public int Remain { get; set; }

        /// <summary>
        /// 花コードに対応する単品情報
        /// </summary>
        [ForeignKey("PartsCode")]
        public BouquetPart BouquetPart { get; set; }
    }
}
