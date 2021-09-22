using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 得意先
    /// </summary>
    [Table("CUSTOMERS")]
    public class Customer
    {
        /// <summary>
        /// 得意先ID
        /// </summary>
        [Column("ID")]
        public int ID { get; set; }

        /// <summary>
        /// e-メールアドレス
        /// </summary>
        [Column("E_MAIL")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        [Column("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        [Column("PASSWORD")]
        public string Password { get; set; }

        /// <summary>
        /// カード番号
        /// </summary>
        [Column("CARD_NO")]
        public string CardNo { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        [Column("STATUS")]
        public int Status { get; set; }


        /// <summary>
        /// この得意先が発注した商品のお届け先一覧
        /// </summary>
        public IList<ShippingAddress> ShippingAddresses { get; } = new List<ShippingAddress>();
    }

}
