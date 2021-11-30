
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 受注履歴
    /// </summary>
    [Table("ORDER_FROM_CUSTOMER")]
    public class OrderFromCustomer
    {
        /// <summary>
        /// 受注番号
        /// </summary>
        [Key,Column("ID")]
        public string ID { get; set; }

        /// <summary>
        /// 受付日
        /// </summary>
        [Column("DATE")]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 得意先ID
        /// </summary>
        [Column("CUSTOMER")]
        public int CustomerID { get; set; }


        /// <summary>
        /// 花束コード
        /// </summary>
        [Column("BOUQUET")]
        public string BouquetCode { get; set; }

        /// <summary>
        /// お届け先ID
        /// </summary>
        [Column("SHIPPING_ADDRESS")]
        public int ShippingAddressID { get; set; }

        /// <summary>
        /// お届け日
        /// </summary>
        [Column("SHIPPING_DATE")]
        public DateTime ShippingDate { get; set; }

        /// <summary>
        /// メッセージ要否
        /// </summary>
        [Column("HAS_MESSAGE")]
        public bool HasMessage { get; set; }

        /// <summary>
        /// お届けメッセージ
        /// </summary>
        [Column("MESSAGE")]
        public string Message { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        [Column("STATUS")]
        public OrderFromCustomerStatus Status { get; set; }

        /// <summary>
        /// 注文した商品：花コードで関連付けられたエンティティオブジェクト
        /// </summary>
        [ForeignKey(nameof(BouquetCode))]
        public Bouquet Bouquet { set; get; }

        /// <summary>
        /// 注文主の得意先：得意先IDで関連付けられたエンティティオブジェクト
        /// </summary>
        [ForeignKey(nameof(CustomerID))]
        public Customer Customer { get; set; }

        /// <summary>
        /// お届け先：お届け先番号で関連付けられたエンティティオブジェクト
        /// </summary>
        [ForeignKey(nameof(ShippingAddressID))]
        public ShippingAddress ShippingAddress { get; set; }
    }
}

