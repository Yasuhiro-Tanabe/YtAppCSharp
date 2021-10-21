using MemorieDeFleurs.Database.SQLite;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ConnectToSQLiteDatabaseCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is MainWindowViiewModel)
            {
                var vm = parameter as MainWindowViiewModel;

                var dialog = new OpenFileDialog();
                dialog.Title = "データベースファイル選択";
                dialog.Filter = "データベースファイル (*.db)|*.db";
                dialog.DefaultExt = ".db";

                var result = dialog.ShowDialog();
                if(result.HasValue && result.Value)
                {
                    MemorieDeFleursUIModel.Instance.OpenSQLiteDatabaseFile(dialog.FileName);
                    vm.Message = $"データベースファイルを開きました。： {Path.GetFileName(dialog.FileName)}";
                }
            }
        }
    }
}
