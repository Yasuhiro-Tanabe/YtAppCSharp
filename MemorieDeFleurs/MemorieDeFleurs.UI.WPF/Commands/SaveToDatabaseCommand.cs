using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.ComponentModel;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SaveToDatabaseCommand : CommandBase<DetailViewModelBase>
    {
        public SaveToDatabaseCommand(NotificationObject vm)
        {
            vm.PropertyChanged += CheckDirtyFlag;
        }

        private void CheckDirtyFlag(object sender, PropertyChangedEventArgs args)
        {
            if (sender is TabItemControlViewModelBase && args.PropertyName == nameof(TabItemControlViewModelBase.IsDirty))
            {
                SetExecutability((sender as TabItemControlViewModelBase).IsDirty);
            }
        }

        protected override void Execute(DetailViewModelBase parameter) => parameter.SaveToDatabase();
    }
}
