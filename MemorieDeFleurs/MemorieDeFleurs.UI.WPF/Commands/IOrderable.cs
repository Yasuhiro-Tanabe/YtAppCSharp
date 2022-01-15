namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 受発注処理を行うビューモデルが実装すべきインタフェース。
    /// 
    /// 受発注処理を同じインタフェースで処理する必然性はないが、機能がほぼ同じ
    /// ①発注(受注)、②発注(受注)取消、および③到着日(入荷日またはお届け日)変更、の3つについて同一インタフェースで取り扱う。
    /// </summary>
    public interface IOrderable
    {
        /// <summary>
        /// 受注または発注を行う
        /// </summary>
        public OrderCommand Order { get; }

        /// <summary>
        /// 受発注を取り消す
        /// </summary>
        public CancelOrderCommand Cancel { get; }

        /// <summary>
        /// 入荷日またはお届け日を変更する
        /// </summary>
        public ChangeArrivalDateCommand ChangeArrivalDate { get; }

        /// <summary>
        /// 受注番号または発注番号
        /// </summary>
        public string OrderNo { get; }

        /// <summary>
        /// このインタフェースを実装したビューモデルが現在保持している情報で受発注を行う。
        /// 受発注成功時、その受発注番号を <see cref="Order"/> に登録する。
        /// </summary>
        public void OrderMe();

        /// <summary>
        /// <see cref="OrderNo"/> で指定された受発注を取り消す
        /// </summary>
        public void CancelMe();

        /// <summary>
        /// <see cref="OrderNo"/> で指定された受発注の到着日 (受注の場合商品のお届け日、発注の場合単品の入荷予定日) を変更する
        /// </summary>
        public void ChangeMyArrivalDate();
    }
}
