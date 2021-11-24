using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// DialogWindow 内に表示するビューモデルが実装すべきインタフェース
    /// </summary>
    internal interface IDialogViewModel
    {
        public void FillDialogParameters(DialogParameter param);
        public void DialogOK();
        public void DialogCancel();
    }
}
