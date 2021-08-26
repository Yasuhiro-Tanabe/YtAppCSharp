using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 在庫アクション
    /// </summary>
    public enum StockActionType
    {
        /// <summary>
        /// 入荷予定
        /// </summary>
        SCHEDULED_TO_ARRIVE = 1,

        /// <summary>
        /// 入荷
        /// </summary>
        ARRIVED,

        /// <summary>
        /// 加工予定
        /// </summary>
        SCHEDULED_TO_USE,

        /// <summary>
        /// 加工
        /// </summary>
        USED,

        /// <summary>
        /// 在庫不足
        /// </summary>
        OUT_OF_STOCK,

        /// <summary>
        /// 破棄予定
        /// </summary>
        SCHEDULED_TO_DISCARD,

        /// <summary>
        /// 破棄
        /// </summary>
        DISCARDED,
    }
}
