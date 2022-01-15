using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

using System;
using System.IO;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SQLiteNewDbCommand : CommandBase<MainWindowViiewModel>
    {
        protected override void Execute(MainWindowViiewModel parameter)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "新規データベースファイル作成";
            dialog.Filter = "データベースファイル (*.db)|*.db";
            dialog.DefaultExt = ".db";
            dialog.OverwritePrompt = true;
            dialog.FileName = "MemorieDeFleurs.db";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                if (File.Exists(dialog.FileName))
                {
                    // 新規作成のため既存DBファイルを削除する
                    File.Delete(dialog.FileName);
                }

                MemorieDeFleursUIModel.Instance.OpenSQLiteDatabaseFile(dialog.FileName);
            }
        }
    }
}
