using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// お届け先
    /// </summary>
    [Table("SHIPPING_ADDRESS")]
    public class ShippingAddress
    {
        /// <summary>
        /// お届け先ID
        /// </summary>
        [Key,Column("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 得意先ID
        /// </summary>
        [Column("CUSTOMER_ID")]
        public int CustomerID { get; set; }

        /// <summary>
        /// お届け先住所1
        /// </summary>
        [Column("ADDRESS_1")]
        public string Address1 { get; set; }

        /// <summary>
        /// お届け先住所2
        /// </summary>
        [Column("ADDRESS_2")]
        public string Address2 { get; set; }

        /// <summary>
        /// お届け先氏名
        /// </summary>
        [Column("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// 最新注文日
        /// </summary>
        [Column("LATEST_ORDER")]
        public DateTime LatestOrderDate { get; set; }

        /// <summary>
        /// 贈り主
        /// </summary>
        [ForeignKey("CustomerID")]
        public Customer From { get; set; }
    }
}
