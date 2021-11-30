using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenProcessingInstructionsPreviewCommand : CommandBase
    {
        public OpenProcessingInstructionsPreviewCommand() : base(typeof(MainWindowViiewModel), OpenTubItem) { }

        private static void OpenTubItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new ProcessingInstructionViewModel());
    }
}
