namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// OrderCommand, CancelOrderCommand, ChangeArrivalDateCommand を持つビューモデルが実装すべきインタフェース
    /// </summary>
    internal interface IOrderable
    {
        public void OrderMe();
        public void CancelMe();
        public void ChangeMyArrivalDate();
    }
}
