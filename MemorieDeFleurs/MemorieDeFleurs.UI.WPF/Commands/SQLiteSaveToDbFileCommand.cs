using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SQLiteSaveToDbFileCommand : CommandBase<MainWindowViiewModel>
    {
        protected override void Execute(MainWindowViiewModel parameter)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "データベースをファイルに保存";
            dialog.Filter = "データベースファイル (*.db)|*.db";
            dialog.DefaultExt = ".db";
            dialog.CheckPathExists = true;
            dialog.OverwritePrompt = true;
            dialog.FileName = "MemorieDeFleurs.db";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                MemorieDeFleursUIModel.Instance.SaveSQLiteConnectinToFile(dialog.FileName);
            }
        }
    }
}
