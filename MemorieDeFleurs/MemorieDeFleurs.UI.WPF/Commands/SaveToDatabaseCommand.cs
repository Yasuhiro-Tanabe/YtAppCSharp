using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.ComponentModel;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SaveToDatabaseCommand : CommandBase
    {
        public SaveToDatabaseCommand(NotificationObject vm) : base(typeof(DetailViewModelBase), SaveToDatabase, IsDirty)
        {
            vm.PropertyChanged += CheckDirtyFlag;
        }

        private static bool IsDirty(object parameter) => (parameter as TabItemControlViewModelBase).IsDirty;
        private static void SaveToDatabase(object parameter) => (parameter as DetailViewModelBase).SaveToDatabase();

        public void CheckDirtyFlag(object sender, PropertyChangedEventArgs args)
        {
            var vm = sender as TabItemControlViewModelBase;
            if (args.PropertyName == nameof(vm.IsDirty))
            {
                RaiseStatusChanged();
            }
        }
    }
}
