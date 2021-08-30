﻿using DDLGenerator.Models.Logging;
using DDLGenerator.ViewModels;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DDLGenerator.Commands
{
    class SelectOutputFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// コマンドが割り当てられたビューモデル
        /// </summary>
        private MainWindowViewModel _vm;

        public SelectOutputFileCommand(MainWindowViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#CanExecute() called. parameter={parameter?.GetType().Name}");
            return true;
        }

        public void Execute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");

            var dialog = new SaveFileDialog();
            dialog.Title = "テーブル定義スクリプトファイルの選択";
            dialog.Filter = "テーブル定義スクリプトファイル (*.sql)|*.sql";
            dialog.DefaultExt = ".sql";
            dialog.CheckPathExists = true;
            dialog.OverwritePrompt = true;
            dialog.FileName = "TableDefinitions.sql";

            var result = dialog.ShowDialog();
            if(result.HasValue && result.Value)
            {
                _vm.OutputDdlFilePath = dialog.FileName;
                LogUtil.Info("出力ファイル:"+dialog.FileName);
            }

        }
    }
}