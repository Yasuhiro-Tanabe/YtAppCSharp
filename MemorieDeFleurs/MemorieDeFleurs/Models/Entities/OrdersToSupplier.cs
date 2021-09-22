using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 発注履歴
    /// </summary>
    [Table("ORDERS_TO_SUPPLIER")]
    public class OrdersToSupplier
    {
        /// <summary>
        /// 発注番号
        /// </summary>
        [Key,Column("ID")]
        public string ID { get; set; }

        /// <summary>
        /// 発注日
        /// </summary>
        [Column("ORDER_DATE")]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 仕入れ先ID
        /// </summary>
        [Column("SUPPLIER")]
        public int Supplier { get; set; }

        /// <summary>
        /// 納品希望日
        /// </summary>
        [Column("DELIVERY_DATE")]
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        [Column("STATUS")]
        public int Status { get; set; }


        public IList<OrderDetailsToSupplier> Details { get; } = new List<OrderDetailsToSupplier>();

    }
}


