using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

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
                }
            }
        }
    }
}
