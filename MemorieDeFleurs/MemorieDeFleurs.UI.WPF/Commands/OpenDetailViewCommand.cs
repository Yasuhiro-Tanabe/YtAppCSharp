using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 一覧画面から詳細画面を開くコマンド
    /// </summary>
    public class OpenDetailViewCommand : CommandBase<ListItemViewModelBase>
    {
        /// <inheritdoc/>
        protected override void Execute(ListItemViewModelBase listVM) => listVM.OpenDetailView();
    }
}
