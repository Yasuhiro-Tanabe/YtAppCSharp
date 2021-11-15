using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// DialogWindow に表示するビューモデルが実装すべきインタフェース
    /// </summary>
    internal interface IDialogUser
    {
        public void FillDialogParameters(DialogParameter param);
        public void DialogOK();
        public void DialogCancel();

        public void OnDialogOpened();
    }
}
