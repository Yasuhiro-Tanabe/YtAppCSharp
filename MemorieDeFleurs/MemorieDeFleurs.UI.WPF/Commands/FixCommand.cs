namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class FixCommand : CommandBase
    {
        public FixCommand() : base(typeof(IEditableAndFixable), FixEditing) { }

        private static void FixEditing(object parameter) => (parameter as IEditableAndFixable).FixEditing();
    }
}
