using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 単品仕入先
    /// </summary>
    [Table("BOUQUET_SUPPLIERS")]
    public class PartSupplier
    {
        [Column("SUPPLIER_CODE")]
        public int SupplierCode { get; set; }
        [Column("BOUQUET_PARTS_CODE")]
        public string PartCode { get; set; }

        [ForeignKey("PartCode")]
        public BouquetPart Part { get; set; }
    }
}
