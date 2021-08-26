using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Entities
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
    }

}
