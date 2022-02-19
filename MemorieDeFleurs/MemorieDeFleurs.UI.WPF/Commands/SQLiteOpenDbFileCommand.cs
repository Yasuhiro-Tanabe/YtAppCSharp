using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SQLiteOpenDbFileCommand : CommandBase<MainWindowViiewModel>
    {
        protected override void Execute(MainWindowViiewModel parameter)
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
