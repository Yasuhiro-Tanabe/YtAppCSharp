using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CloseTabItemControlCommand :CommandBase<TabItemControlViewModelBase>
    {
        protected override void Execute(TabItemControlViewModelBase parameter) => parameter.CloseControl();
    }
}
