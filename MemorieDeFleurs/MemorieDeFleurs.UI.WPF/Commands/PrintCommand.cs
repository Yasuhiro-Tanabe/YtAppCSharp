namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class PrintCommand : CommandBase
    {
        public PrintCommand() : base(typeof(IPrintable), Print) { }

        private static void Print(object parameter) => (parameter as IPrintable).PrintDocument();
    }
}
