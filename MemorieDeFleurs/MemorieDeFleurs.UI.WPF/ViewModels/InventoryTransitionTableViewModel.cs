﻿using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
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
            get { return _parts; }
            set
            {
                SetProperty(ref _parts, value);
                if(value != null)
                {
                    BouquetPartsCode = value.PartsCode;
                    ExpiryDate = value.ExpiryDate;
                }
            }
        }
        private BouquetPartsDetailViewModel _parts;

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
        }

        public override void LoadItems()
        {
            Update();
        }

        #region IPrintable
        public void PrintDocument()
        {
            if (string.IsNullOrWhiteSpace(BouquetPartsCode)) { throw new ApplicationException("印刷対象の花コードが指定されていません。"); }
            if (ExpiryDate == 0) { throw new ApplicationException($"品質維持可能日数が0日です。{BouquetPartsCode} ({SelectedParts.PartsName}) の品質維持可能日数が設定されているか確認してください。"); }
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();

                var w = (5.0 /* [mm] */).MmToPixcel();
                var h = (10.0 /* [mm] */).MmToPixcel();

                var control = new InventoryTransitionTableControl();
                control.DataContext = this;
                control.Margin = new Thickness(w, h, w, h);

                // control 内の在庫推移表 DataGrid を作り直すため PropertyChanged イベントを強制発行する。
                RaisePropertyChanged(nameof(InventoryTransitions));

                var doc = new FixedDocument();
                var page = new FixedPage();

                page.Children.Add(control);
                doc.Pages.Add(new PageContent() { Child = page });

                PrintDocumentImageableArea area = null;
                var writer = PrintQueue.CreateXpsDocumentWriter(ref area);
                if(writer != null)
                {
                    writer.Write(doc);
                    LogUtil.Info($"Inventory transaction table of parts {BouquetPartsCode} was printed.");
                }
                else
                {
                    // 何もしない： プリンタ選択ダイアログをキャンセルで閉じると
                    // CreateXpsDocumentWriter() の戻り値が null になる。このときは印刷を行わずに処理正常終了する。
                    LogUtil.Debug("Canceled printing.");
                }
            }
            catch(Exception ex)
            {
                LogUtil.Warn(ex);
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }
        #endregion // IPrintable
    }
}
