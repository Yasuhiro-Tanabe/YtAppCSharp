using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeProcessingBouquetCommand : CommandBase
    {
        public ChangeProcessingBouquetCommand() : base(typeof(ProcessingInstructionViewModel), ChangeProcessingBouquet) { }

        private static void ChangeProcessingBouquet(object parameter) => (parameter as ProcessingInstructionViewModel).ChangeProcessingBouquet();
    }
}
