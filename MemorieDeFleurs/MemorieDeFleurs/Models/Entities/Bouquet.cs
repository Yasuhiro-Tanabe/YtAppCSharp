using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 商品(花束セット)
    /// </summary>
    [Table("BOUQUET_SET")]
    public class Bouquet
    {
        /// <summary>
        /// 花束コード
        /// </summary>
        [Key, Column("CODE")]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// イメージ画像
        /// </summary>
        [Column("IMAGE")]
        public string Image { get; set; }

        /// <summary>
        /// 発注リードタイム：構成する単品在庫がないときの、受注後発送までの最短日数
        /// </summary>
        [Column("LEAD_TIME")]
        public int LeadTime { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        [Column("STATUS")]
        public int Status { get; set; }

        /// <summary>
        /// この商品の商品構成
        /// </summary>
        public IList<BouquetPartsList> PartsList { get; set; } = new List<BouquetPartsList>();
    }
}
