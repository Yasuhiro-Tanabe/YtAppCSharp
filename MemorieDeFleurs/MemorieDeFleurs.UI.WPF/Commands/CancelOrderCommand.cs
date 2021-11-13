namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CancelOrderCommand : CommandBase
    {
        public CancelOrderCommand() : base(typeof(IOrderable), CancelOrder) { }

        private static void CancelOrder(object parameter) => (parameter as IOrderable).CancelMe();
    }
}
