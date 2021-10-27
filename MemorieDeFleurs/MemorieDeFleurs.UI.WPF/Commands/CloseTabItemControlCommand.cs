using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CloseTabItemControlCommand :CommandBase
    {
        public CloseTabItemControlCommand() : base(typeof(TabItemControlViewModelBase), CloseControl) { }

        private static void CloseControl(object parameter) => (parameter as TabItemControlViewModelBase).CloseControl();
    }
}
