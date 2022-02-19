using MemorieDeFleurs.UI.WPF.ViewModels;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenProcessingInstructionsPreviewCommand : CommandBase<MainWindowViiewModel>
    {
        protected override void Execute(MainWindowViiewModel parameter) => parameter.OpenTabItem(new ProcessingInstructionViewModel());
    }
}
