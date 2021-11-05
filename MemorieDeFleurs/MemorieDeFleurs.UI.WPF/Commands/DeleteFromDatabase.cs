using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DeleteFromDatabase : CommandBase
    {
        public DeleteFromDatabase() : base(typeof(ListItemViewModelBase), RemoveMe) { }

        public static void RemoveMe(object parameter) => (parameter as ListItemViewModelBase).RemoveMe();
    }
}
