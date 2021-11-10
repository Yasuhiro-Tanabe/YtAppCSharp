using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class EditCommand : CommandBase
    {
        public EditCommand() : base(typeof(IEditableAndFixable), OpenEditView) { }

        private static void OpenEditView(object parameter) => (parameter as IEditableAndFixable).OpenEditView();
    }
}
