﻿using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsSummaryViewModel : ListItemViewModelBase
    {
        #region プロパティ
        public BouquetPartsSummaryViewModel(BouquetPart part) : base(new OpenPartsDetailViewCommand())
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
        #endregion // プロパティ
    }
}
