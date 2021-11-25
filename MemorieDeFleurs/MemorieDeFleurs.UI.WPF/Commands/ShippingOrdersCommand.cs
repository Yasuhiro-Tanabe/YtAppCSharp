using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ShippingOrdersCommand : CommandBase
    {
        public ShippingOrdersCommand() : base(typeof(ProcessingInstructionViewModel), Ship) { }

        private static void Ship(object parameter) => (parameter as ProcessingInstructionViewModel).ShipBouquets();
    }
}
