using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Models.Entities
{
    [Table("BOUQUET_PARTS")]
    public class BouquetPart
    {
        /// <summary>
        /// 花コード
        /// </summary>
        [Key]
        public string CODE { get; set; }

        /// <summary>
        /// 花名称
        /// </summary>
        public string NAME { get; set; }


        /// <summary>
        /// リードタイム
        /// </summary>
        public int LEAD_TIME { get; set; }

        /// <summary>
        /// 購入単位数
        /// </summary>
        public int NUM_PAR_LOT { get; set; }

        /// <summary>
        /// 品質維持可能日数
        /// </summary>
        public int EXPIRY_DATE { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        public int STATUS { get; set; }
    }
}
