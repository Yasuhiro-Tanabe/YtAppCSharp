using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeArrivalDateCommand : CommandBase
    {
        public ChangeArrivalDateCommand() : base(typeof(IOrderable), ChangeArrivalDate) { }

        private static void ChangeArrivalDate(object parameter) => (parameter as IOrderable).ChangeMyArrivalDate();
    }
}
