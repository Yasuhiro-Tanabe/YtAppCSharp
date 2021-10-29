using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

using System;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SQLiteSaveToDbFileCommand : CommandBase
    {
        public SQLiteSaveToDbFileCommand() : base(typeof(MainWindowViiewModel), Save) { }

        private static void Save(object parameter)
        {
            if(parameter is MainWindowViiewModel)
            {
                var dialog = new SaveFileDialog();
                dialog.Title = "データベースをファイルに保存";
                dialog.Filter = "データベースファイル (*.db)|*.db";
                dialog.DefaultExt = ".db";
                dialog.CheckPathExists = true;
                dialog.OverwritePrompt = true;
                dialog.FileName = "MemorieDeFleurs.db";

                var result = dialog.ShowDialog();
                if(result.HasValue && result.Value)
                {
                    MemorieDeFleursUIModel.Instance.SaveSQLiteConnectinToFile(dialog.FileName);
                }
            }
            else
            {
                throw new NotImplementedException($"{typeof(SQLiteSaveToDbFileCommand).Name}.{nameof(Save)}({parameter.GetType().Name})");
            }
        }
    }
}
