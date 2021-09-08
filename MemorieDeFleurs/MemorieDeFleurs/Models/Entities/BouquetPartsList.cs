using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 商品構成
    /// </summary>
    [Table("BOUQUET_PARTS_LIST")]
    public class BouquetPartsList
    {
        /// <summary>
        /// 花束コード
        /// </summary>
        [Column("BOUQUET_CODE")]
        public string BouquetCode { get; set; }

        /// <summary>
        /// 花コード
        /// </summary>
        [Column("PARTS_CODE")]
        public string PartsCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Column("QUANTITY")]
        public int Quantity { get; set; }
    }
}
