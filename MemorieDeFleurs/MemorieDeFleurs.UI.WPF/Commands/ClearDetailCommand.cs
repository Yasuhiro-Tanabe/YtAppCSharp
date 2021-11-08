using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ClearDetailCommand : CommandBase
    {
        public ClearDetailCommand() : base(typeof(DetailViewModelBase), Clear) { }

        private static void Clear(object parameter) => (parameter as DetailViewModelBase).ClearProperties();
    }
}
