using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CloseTabItemControlCommand :CommandBase
    {
        public override void Execute(object parameter)
        {
            var vm = parameter as ITabItemControlViewModel;
            if(vm != null)
            {
                vm.ParentViewModel.CloseTabItem(vm);
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
