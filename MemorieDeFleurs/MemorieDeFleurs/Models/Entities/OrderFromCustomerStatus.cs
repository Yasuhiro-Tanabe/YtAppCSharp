namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 得意先からの受注情報の現在の状態
    /// </summary>
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
