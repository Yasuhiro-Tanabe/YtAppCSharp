using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class MainWindowOpenDetailViewCommand<VM> : CommandBase<MainWindowViiewModel> where VM : DetailViewModelBase, new()
    {
        protected override void Execute(MainWindowViiewModel parameter) => parameter.OpenTabItem(new VM());
    }
}
