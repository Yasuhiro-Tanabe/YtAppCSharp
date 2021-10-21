using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CloseTabItemControlCommand :CommandBase
    {
        public override void Execute(object parameter)
        {
            var vm = parameter as TabItemControlViewModelBase;
            if(vm != null)
            {
                vm.CloseControl();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
