﻿using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;
using MemorieDeFleurs.UI.WPF.Views;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class InventoryTransitionTableViewModel : ListViewModelBase, IPrintable
    {
        public InventoryTransitionTableViewModel() : base("在庫推移表") { }

        #region プロパティ
        /// <summary>
        /// 在庫推移表表示期間開始日
        /// </summary>
        public DateTime From
        {
            get { return _from; }
            set { SetProperty(ref _from, value); }
        }
        private DateTime _from = DateTime.Today;

        /// <summary>
        /// 在庫推移期間表示終了日
        /// </summary>
        public DateTime To
        {
            get { return _to; }
            set { SetProperty(ref _to, value); }
        }
        private DateTime _to = DateTime.Today;

        /// <summary>
        /// 在庫推移表示可能な単品の一覧
        /// </summary>
        public ObservableCollection<BouquetPartsDetailViewModel> BouquetParts { get; } = new ObservableCollection<BouquetPartsDetailViewModel>();

        /// <summary>
        /// 現在在庫推移表表示中の単品
        /// </summary>
        public BouquetPartsDetailViewModel SelectedParts
        {
            get { return _selected; }
            set
            {
                SetProperty(ref _selected, value);
                if(value != null)
                {
                    BouquetPartsCode = value.PartsCode;
                    ExpiryDate = value.ExpiryDate;
                }
            }
        }
        private BouquetPartsDetailViewModel _selected;

        /// <summary>
        /// 現在表示中の単品の花コード
        /// </summary>
        public string BouquetPartsCode
        { 
            get { return _partsCode; }
            set { SetProperty(ref _partsCode, value); }
        }
        private string _partsCode;

        /// <summary>
        /// 現在表示中の単品の品質維持可能日数
        /// </summary>
        public int ExpiryDate
        {
            get { return _expirDate; }
            set { SetProperty(ref _expirDate, value); }
        }
        private int _expirDate;

        /// <summary>
        /// 日別在庫推移を表示するためのデータテンプレート
        /// 
        /// 単品によって品質維持可能日数が異なるので、プロパティとして用意し単品を切り替える都度変更する。
        /// </summary>
        public DataTemplate ColumnsTemplate
        {
            get { return _template; }
            set { SetProperty(ref _template, value); }
        }
        private DataTemplate _template;

        /// <summary>
        /// 在庫推移表に表示するデータ
        /// </summary>
        public ObservableCollection<InventoryTransitionRowViewModel> InventoryTransitions { get; } = new ObservableCollection<InventoryTransitionRowViewModel>();
        #endregion

        #region コマンド
        public ICommand Print { get; } = new PrintCommand();
        public ICommand UpdateTable { get; } = new InventoryTransitionTableChangedCommand();
        #endregion // コマンド

        public void Update()
        {
            if(SelectedParts == null)
            {
                Cleanup();
            }
            else
            {
                if(To < From)
                {
                    To = From;
                }

                var table = MemorieDeFleursUIModel.Instance.CreateInventoryTransitionTable(BouquetPartsCode, From, To);

                UpdateInventoryTransitions(table);
            }
        }

        private void UpdateInventoryTransitions(Models.InventoryTransitionTable table)
        {
            InventoryTransitions.Clear();
            foreach (var d in Enumerable.Range(0, (To - From).Days).Select(i => From.AddDays(i)))
            {
                InventoryTransitions.Add(new InventoryTransitionRowViewModel(d, table[d], table.ExpiryDate));
            }
            RaisePropertyChanged(nameof(InventoryTransitions));
        }

        private void Cleanup()
        {
            SelectedParts = null;
            BouquetPartsCode = string.Empty;
            ExpiryDate = 0;

            LoadBouquetParts();
        }

        private void LoadBouquetParts()
        {
            BouquetParts.Clear();
            foreach(var p in MemorieDeFleursUIModel.Instance.FindAllBouquetParts())
            {
                BouquetParts.Add(new BouquetPartsDetailViewModel(p));
            }
            SelectedParts = null;
            RaisePropertyChanged(nameof(BouquetParts));
        }

        public override void LoadItems()
        {
            Update();
        }

        #region IPrintable
        public void PrintDocument()
        {
            var control = new InventoryTransitionTableControl();
            control.DataContext = this;
            control.Margin = new Thickness(MmToPixcel(5.0), MmToPixcel(10.0), MmToPixcel(5.0), MmToPixcel(10.0));

            var doc = new FixedDocument();
            var page = new FixedPage();

            page.Children.Add(control);
            doc.Pages.Add(new PageContent() { Child = page });

            PrintDocumentImageableArea area = null;
            var writer = PrintQueue.CreateXpsDocumentWriter(ref area);

            writer.Write(doc);
        }

        private double InchToPixcell(double inch)
        {
            return inch * 96.0;
        }

        private double MmToPixcel(double mm)
        {
            return InchToPixcell(mm / 24.5);
        }
        #endregion // IPrintable
    }
}