using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DeleteFromDatabase : CommandBase<ListItemViewModelBase>
    {
        protected override void Execute(ListItemViewModelBase parameter) => parameter.RemoveMe();
    }
}
