using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OrderCommand : CommandBase
    {
        public OrderCommand() : base(typeof(IOrderable), Order) { }

        private static void Order(object parameter) => (parameter as IOrderable).OrderMe();
    }
}
