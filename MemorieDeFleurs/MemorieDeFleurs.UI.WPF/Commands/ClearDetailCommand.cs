using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ClearDetailCommand : CommandBase<DetailViewModelBase>
    {
        protected override void Execute(DetailViewModelBase parameter) => parameter.ClearProperties();
    }
}
