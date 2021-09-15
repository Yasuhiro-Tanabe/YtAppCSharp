using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 連番管理
    /// </summary>
    [Table("SEQUENCES")]
    public class SequenceValue
    {
        /// <summary>
        /// シーケンス名
        /// </summary>
        [Key,Column("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// 現在値
        /// </summary>
        [Column("VALUE")]
        public int Value { get; set; }
    }
}

