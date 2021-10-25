﻿using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsSummaryViewModel : NotificationObject
    {
        #region プロパティ
        public BouquetPartsSummaryViewModel(BouquetPart part) : base()
        {
            Update(part);
        }

        public void Update(BouquetPart part)
        {
            _code = part.Code;
            _name = part.Name;
            RaisePropertyChanged(nameof(PartsCode), nameof(PartsName));
        }

        public string PartsCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        public string PartsName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        public Visibility ActionVisivility { get { return _isVisible ? Visibility.Visible : Visibility.Collapsed; } }
        private bool _isVisible = false;
        #endregion // プロパティ

        #region コマンド
        public ICommand Remove { get; } = new RemoveListItemCommand();
        public ICommand OpenDetailVew { get; }
        #endregion // コマンド

        public void ShowCommandButtons()
        {
            _isVisible = true;
            RaisePropertyChanged(nameof(ActionVisivility));
        }

        public void HideCommandButtons()
        {
            _isVisible = false;
            RaisePropertyChanged(nameof(ActionVisivility));
        }

        public void RemoveMe()
        {
            RaisePropertyChanged(nameof(RemoveMe));
        }
    }
}