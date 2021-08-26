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
        public int ACTION_DATE { get; set; }

        /// <summary>
        /// アクション
        /// </summary>
        public StockActionType ACTION { get; set; }

        /// <summary>
        /// 花コード
        /// </summary>
        public string BOUQUET_PARTS_CODE { get; set; }

        /// <summary>
        /// 入荷日
        /// </summary>
        public int ARRIVAL_DATE { get; set; }

        /// <summary>
        /// 在庫ロット番号
        /// </summary>
        public int LOT_NO { get; set; }

        /// <summary>
        /// 数量：入荷(予定)数、加工(予定)数、破棄(予定)数
        /// </summary>
        public int QUANTITY { get; set; }

        /// <summary>
        /// 残数
        /// </summary>
        public int REMAIN { get; set; }

        /// <summary>
        /// 花コードに対応する単品情報
        /// </summary>
        [ForeignKey("BOUQUET_PARTS_CODE")]
        public BouquetPart BouquetPart { get; set; }
    }
}
