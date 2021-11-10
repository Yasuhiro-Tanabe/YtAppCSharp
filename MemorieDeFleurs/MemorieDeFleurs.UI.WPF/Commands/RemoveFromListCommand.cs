using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class RemoveFromListCommand : CommandBase
    {
        public RemoveFromListCommand() : base(typeof(IAppendableRemovable), RemoveFromList) { }

        private static void RemoveFromList(object parameter) => (parameter as IAppendableRemovable).RemoveFromList();
    }
}
