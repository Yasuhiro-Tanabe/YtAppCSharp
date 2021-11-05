using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

using System;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SQLiteOpenDbFileCommand : CommandBase
    {
        public SQLiteOpenDbFileCommand() : base(typeof(MainWindowViiewModel), OpenDatabase) { }

        private static void OpenDatabase(object parameter)
        {
            if(parameter is MainWindowViiewModel)
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
            else
            {
                throw new NotImplementedException($"{typeof(SQLiteOpenDbFileCommand).Name}.{nameof(OpenDatabase)}({parameter.GetType().Name})");
            }
        }
    }
}
