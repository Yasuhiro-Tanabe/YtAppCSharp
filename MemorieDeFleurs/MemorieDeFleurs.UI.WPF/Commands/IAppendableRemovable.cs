namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// AppendToListCommand と RemoveFromListCommand を持つビューモデルが実装すべきインタフェース
    /// </summary>
    internal interface IAppendableRemovable
    {
        public void AppendToList();
        public void RemoveFromList();
    }
}
