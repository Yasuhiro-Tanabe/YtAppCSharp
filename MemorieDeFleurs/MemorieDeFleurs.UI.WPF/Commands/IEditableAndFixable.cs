namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// EditCommand と FixCommand を持つビューモデルが実装すべきインタフェース
    /// </summary>
    internal interface IEditableAndFixable
    {
        public void OpenEditView();
        public void FixEditing();
    }
}
