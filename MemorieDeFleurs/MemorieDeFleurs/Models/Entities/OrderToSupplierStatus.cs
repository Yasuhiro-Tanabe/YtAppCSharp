namespace MemorieDeFleurs.Models.Entities
{
    /// <summary>
    /// 仕入先への発注ステータス
    /// </summary>
    public enum OrderToSupplierStatus
    {
        /// <summary>
        /// 発注予定
        /// </summary>
        SCHEDULED = 0,

        /// <summary>
        /// 発注済み入荷待ち
        /// </summary>
        ORDERED,

        /// <summary>
        /// 入荷済み
        /// </summary>
        ARRIVED,
    }
}
