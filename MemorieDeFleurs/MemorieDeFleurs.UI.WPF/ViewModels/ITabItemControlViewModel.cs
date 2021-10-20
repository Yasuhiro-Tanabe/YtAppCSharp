using MemorieDeFleurs.UI.WPF.Commands;

using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public interface ITabItemControlViewModel
    {
        public string Header { get; }

        public MainWindowViiewModel ParentViewModel { get; set; }

        public ICommand Close { get; }
    }
}
