using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class AppendToListCommand : CommandBase
    {
        public AppendToListCommand() : base(typeof(IAppendableRemovable), AddToList) { }

        private static void AddToList(object parameter) => (parameter as IAppendableRemovable).AppendToList();
    }
}
