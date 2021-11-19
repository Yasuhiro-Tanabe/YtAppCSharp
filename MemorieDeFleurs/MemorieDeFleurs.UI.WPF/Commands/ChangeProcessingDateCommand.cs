using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeProcessingDateCommand : CommandBase
    {
        public ChangeProcessingDateCommand() : base(typeof(ProcessingInstructionViewModel), ChangeProcessingDate) { }

        private static void ChangeProcessingDate(object parameter) => (parameter as ProcessingInstructionViewModel).ChangeProcessingDate();
    }
}
