using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 単品エンティティ
    /// </summary>
    [Table("BOUQUET_PARTS")]
    public class BouquetPart
    {
        /// <summary>
        /// 花コード
        /// </summary>
        [Key, Column("CODE")]
        public string Code { get; set; }

        /// <summary>
        /// 花名称
        /// </summary>
        [Column("NAME")]
        public string Name { get; set; }


        /// <summary>
        /// リードタイム
        /// </summary>
        [Column("LEAD_TIME")]
        public int LeadTime { get; set; }

        /// <summary>
        /// 購入単位数
        /// </summary>
        [Column("NUM_PAR_LOT")]
        public int QuantitiesPerLot { get; set; }

        /// <summary>
        /// 品質維持可能日数
        /// </summary>
        [Column("EXPIRY_DATE")]
        public int ExpiryDate { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        [Column("STATUS")]
        public int Status { get; set; }


        /// <summary>
        /// 個の単品を提供している仕入先一覧
        /// </summary>
        public IList<PartSupplier> Suppliers { get; } = new List<PartSupplier>();
    }
}
