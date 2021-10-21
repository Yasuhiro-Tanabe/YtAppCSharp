﻿using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsDetailViewModel : ViewModelBase, ITabItemControlViewModel
    {
        #region ITabItemControlViewModel
        #region プロパティ
        public string Header { get; } = "単品詳細";
        public MainWindowViiewModel ParentViewModel { get; set; }
        public bool IsDirty
        {
            get { return _dirty; }
            set { SetProperty(ref _dirty, value); }
        }
        private bool _dirty;
        #endregion // プロパティ
        #region コマンド
        public ICommand Close { get; } = new CloseTabItemControlCommand();
        public ICommand Register { get; private set; }
        #endregion // コマンド
        #endregion // ITabItemControlViewModel

        #region ビュー固有のプロパティ
        /// <summary>
        /// 花コード
        /// </summary>
        public string PartsCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        /// <summary>
        /// 単品名称
        /// </summary>
        public string PartsName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        /// <summary>
        /// 購入単位数：1ロットあたりの単品本数
        /// </summary>
        public int QuantitiesParLot
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }
        private int _quantity;

        /// <summary>
        /// 発注リードタイム [単位：日]
        /// </summary>
        public int LeadTime
        {
            get { return _leadTime; }
            set { SetProperty(ref _leadTime, value); }
        }
        private int _leadTime;

        /// <summary>
        /// 品質維持可能日数 [単位：日]
        /// </summary>
        public int ExpiryDate
        {
            get { return _expriy; }
            set { SetProperty(ref _expriy, value); }
        }
        private int _expriy;
        #endregion // ビュー固有のプロパティ

        public BouquetPartsDetailViewModel()
        {
            Register = new RegisterCommand(this);
            PropertyChanged += SetDirtyFlag;
        }

        private void SetDirtyFlag(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName != nameof(IsDirty))
            {
                IsDirty = true;
            }
        }

        public void Update(BouquetPart part)
        {
            _code = part.Code;
            _name = part.Name;
            _quantity = part.QuantitiesPerLot;
            _leadTime = part.LeadTime;
            _expriy = part.ExpiryDate;
            RaisePropertyChanged(nameof(PartsCode), nameof(PartsName), nameof(QuantitiesParLot), nameof(LeadTime), nameof(ExpiryDate));
        }

        /// <summary>
        /// 入力されている値を検証する。
        /// 
        /// 検証で何も問題なければ正常終了、何らかの不正値があるときは例外 <see cref="ValidateFailedException"/> がスローされる。
        /// </summary>
        public void Validate()
        {
            var result = new ValidateFailedException();

            if(string.IsNullOrWhiteSpace(_code))
            {
                result.Append("花コードを入力してください。");
            }
            if(string.IsNullOrWhiteSpace(_name))
            {
                result.Append("単品名称を入力してください。");
            }
            if(_quantity < 1)
            {
                result.Append($"購入単位数が不適切です：{_quantity}。1本以上の値を入力してください。");
            }
            if(_leadTime < 1)
            {
                result.Append($"商品リードタイムが不適切です：{_leadTime}。1日以上の値を入力してください。");
            }
            if(_expriy < 1)
            {
                result.Append($"品質維持可能日数が不適切です：{_expriy}。1日以上の値を入力してください。");
            }

            if(result.ValidationErrors.Count > 0) { throw result; }
        }
    }
}
