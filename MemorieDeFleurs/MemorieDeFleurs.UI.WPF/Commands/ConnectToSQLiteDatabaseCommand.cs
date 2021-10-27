using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ConnectToSQLiteDatabaseCommand : CommandBase
    {
        public ConnectToSQLiteDatabaseCommand() : base(typeof(MainWindowViiewModel), OpenDatabase) { }

        private static void OpenDatabase(object unused)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "データベースファイル選択";
            dialog.Filter = "データベースファイル (*.db)|*.db";
            dialog.DefaultExt = ".db";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                MemorieDeFleursUIModel.Instance.OpenSQLiteDatabaseFile(dialog.FileName);
            }
        }
    }
}
