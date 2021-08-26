using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Entities
{
    /// <summary>
    /// 得意先
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// 得意先ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// e-メールアドレス
        /// </summary>
        public string E_MAIL { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        public string PASSWORD { get; set; }

        /// <summary>
        /// カード番号
        /// </summary>
        public string CARD_NO { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        public int STATUS { get; set; }
    }

}
