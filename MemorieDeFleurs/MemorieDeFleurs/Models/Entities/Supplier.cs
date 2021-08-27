using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 仕入先
    /// </summary>
    [Table("SUPPLIERS")]
    public class Supplier
    {
        /// <summary>
        /// 仕入先コード
        /// </summary>
        [Key, Column("CODE")]
        public int Code { get; set; }
        
        /// <summary>
        /// 仕入先名称
        /// </summary>
        [Column("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// 仕入先住所1 
        /// </summary>
        [Column("ADDRESS_1")]
        public string Address1 { get; set; }

        /// <summary>
        /// 仕入先住所2
        /// </summary>
        [Column("ADDRESS_2")]
        public string Address2 { get; set; }

        /// <summary>
        /// 仕入先電話番号
        /// </summary>
        [Column("TEL")]
        public string Telephone { get; set; }

        /// <summary>
        /// 仕入先FAX番号
        /// </summary>
        [Column("FAX")]
        public string Fax { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        [Column("E_MAIL")]
        public string EmailAddress { get; set; }
    }
}
