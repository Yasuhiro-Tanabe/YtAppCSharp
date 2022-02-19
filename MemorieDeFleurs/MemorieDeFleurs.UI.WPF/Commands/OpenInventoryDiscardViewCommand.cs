using MemorieDeFleurs.UI.WPF.ViewModels;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenInventoryDiscardViewCommand : CommandBase<MainWindowViiewModel>
    {
        protected override void Execute(MainWindowViiewModel parameter) => parameter.OpenTabItem(new InventoryViewModel());
    }
}
