using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 単品仕入先
    /// </summary>
    [Table("BOUQUET_SUPPLIERS")]
    public class PartSupplier
    {
        /// <summary>
        /// 仕入先コード
        /// </summary>
        [Column("SUPPLIER_CODE")]

        public int SupplierCode { get; set; }
        /// <summary>
        /// 花コード
        /// </summary>
        [Column("BOUQUET_PARTS_CODE")]
        public string PartCode { get; set; }

        /// <summary>
        /// 花コードで関連付けられた単品情報
        /// </summary>
        [ForeignKey("PartCode")]
        public BouquetPart Part { get; set; }

        /// <summary>
        /// 仕入先コードで関連付けられた仕入先情報
        /// </summary>
        [ForeignKey("SupplierCode")]
        public Supplier Supplier { get; set; }
    }
}
