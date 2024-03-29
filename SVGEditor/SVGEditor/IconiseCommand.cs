﻿using YasT.Framework.Logging;

using Microsoft.Win32;

using System.IO;
using YasT.Framework.WPF;

namespace SVGEditor
{
    internal class IconiseCommand : CommandBase<MainWindowViewModel>
    {
        protected override void Execute(MainWindowViewModel vm)
        {
            var view = App.Current.MainWindow as MainWindow;

            var dialog = new SaveFileDialog()
            {
                Filter = "ICON File (*.ico)|*.ico",
                FileName = Path.GetFileName(vm.SvgFileName).Replace(".svg", ".ico"),
                OverwritePrompt = true,
            };

            var result = dialog.ShowDialog();
            if(result.HasValue && result.Value)
            {
                SVGEditorModel.Instance.WriteToIconFile(dialog.FileName, view.Editor.Text);
                LogUtil.Info($"Icon file {dialog.FileName} created.");
            }
        }
    }
}
