namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// DialogOpenCommand をプロパティに持つビューモデルが実装しなければならないインタフェース
    /// </summary>
    public interface IDialogCaller
    {
        public NotificationObject DialogViewModel { get; }
    }
}
