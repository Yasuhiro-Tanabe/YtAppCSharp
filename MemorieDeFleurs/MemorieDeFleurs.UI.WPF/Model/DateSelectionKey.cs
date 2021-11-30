using MemorieDeFleurs.UI.WPF.Views;

namespace MemorieDeFleurs.UI.WPF.Model
{
    /// <summary>
    /// 入荷検品一覧画面 <see cref="OrderToSupplierInspectionListControl"/> 内で指定する日付の区分
    /// </summary>
    public enum DateSelectionKey
    {
        /// <summary>
        /// 発注日または受注日
        /// </summary>
        ORDERED = 1,

        /// <summary>
        /// 入荷(予定)日
        /// </summary>
        ARRIVED,

        /// <summary>
        /// 出荷(予定)日
        /// </summary>
        SHIPPED
    }
}
