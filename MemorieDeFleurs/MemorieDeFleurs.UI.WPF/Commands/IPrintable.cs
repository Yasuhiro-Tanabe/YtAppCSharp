namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 印刷対象のビューモデルが実装すべきインタフェース
    /// </summary>
    public interface IPrintable
    {
        public void ValidateBeforePrinting();
    }
}
