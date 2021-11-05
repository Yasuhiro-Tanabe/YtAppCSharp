using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Models.Entities
{
    public enum OrderFromCustomerStatus
    {
        /// <summary>
        /// 受注予定
        /// </summary>
        SCHEDULED = 0,

        /// <summary>
        /// 受注確定出荷前
        /// </summary>
        ORDERED,

        /// <summary>
        /// 出荷済
        /// </summary>
        SHIPPED,
    }
}
