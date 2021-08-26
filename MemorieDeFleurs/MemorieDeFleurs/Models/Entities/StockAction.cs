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
    /// 在庫アクション
    /// </summary>
    [Table("STOCK_ACTIONS")]
    public class StockAction
    {
        /// <summary>
        /// 基準日
        /// </summary>
        [Column("ACTION_DATE")]
        public int ActionDate { get; set; }

        /// <summary>
        /// アクション
        /// </summary>
        [Column("ACTION")]
        public StockActionType Action { get; set; }

        /// <summary>
        /// 花コード
        /// </summary>
        [Column("BOUQUET_PARTS_CODE")]
        public string PartsCode { get; set; }

        /// <summary>
        /// 入荷日
        /// </summary>
        [Column("ARRIVAL_DATE")]
        public int ArrivalDate { get; set; }

        /// <summary>
        /// 在庫ロット番号
        /// </summary>
        [Column("LOT_NO")]
        public int StockLotNo { get; set; }

        /// <summary>
        /// 数量：入荷(予定)数、加工(予定)数、破棄(予定)数
        /// </summary>
        [Column("QUANTITY")]
        public int Quantity { get; set; }

        /// <summary>
        /// 残数
        /// </summary>
        [Column("REMAIN")]
        public int Remain { get; set; }

        /// <summary>
        /// 花コードに対応する単品情報
        /// </summary>
        [ForeignKey("PartsCode")]
        public BouquetPart BouquetPart { get; set; }
    }
}
