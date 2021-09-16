
using DDLGenerator.Models.Logging;
using DDLGenerator.ViewModels;

using Microsoft.WindowsAPICodePack.Dialogs;

using System;
using System.Windows.Input;


namespace DDLGenerator.Commands
{
    class SelectOutputFolderCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return IsExecutable;
        }

        public void Execute(object parameter)
        {
            if(CommonOpenFileDialog.IsPlatformSupported)
            {
                using (var dialog = new CommonOpenFileDialog() { IsFolderPicker = true, Title = "出力先フォルダの選択" })
                {
                    var result = dialog.ShowDialog();
                    if (result == CommonFileDialogResult.Ok)
                    {
                        LogUtil.Info("出力先フォルダ：" + dialog.FileName);

                        if (parameter is EFCoreEntityViewModel)
                        {
                            LogUtil.Debug($"EFCoreEntityViewModel.OutputFolderPath = '{dialog.FileName}'");
                            (parameter as EFCoreEntityViewModel).OutputFolderPath = dialog.FileName;
                        }
                        else
                        {
                            LogUtil.Warn($"Unexpected View: {parameter.GetType().Name}");
                        }
                    }
                }
            }
        }

        private bool IsExecutable { get; set; } = true;

        public void OnGenerationStarted(object sender, EventArgs unused)
        {
            IsExecutable = false;
            CanExecuteChanged?.Invoke(this, null);
        }

        public void OnGenerationFinished(object sender, EventArgs unused)
        {
            IsExecutable = true;
            CanExecuteChanged?.Invoke(this, null);
        }
    }
}
