using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 在庫推移表画面 (タブ要素画面) のビューモデル
    /// </summary>
    public class InventoryTransitionTableViewModel : ListViewModelBase, IPrintable, IReloadable
    {
        /// <summary>
        /// ビューモデルの名称：<see cref="TabItemControlViewModelBase.Header"/> や <see cref="MainWindowViiewModel.FindTabItem(string)"/> に渡すクラス定数として使用する。
        /// </summary>
        public static string Name { get { return TextResourceFinder.FindText("InventoryTransitionTable"); } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InventoryTransitionTableViewModel() : base(Name) { }

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
        /// <summary>
        /// 在庫推移表の表示範囲(開始日終了日、対象単品)が変更されたときに実行されるコマンド
        /// </summary>
        public ICommand UpdateTable { get; } = new InventoryTransitionTableChangedCommand();
        #endregion // コマンド


        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            if (SelectedParts == null)
            {
                Cleanup();
            }
            else
            {
                if (To < From)
                {
                    To = From;
                }

                var table = MemorieDeFleursUIModel.Instance.CreateInventoryTransitionTable(BouquetPartsCode, From, To);

                UpdateInventoryTransitions(table);
            }
        }
        #endregion // IReloadable

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

        #region IPrintable
        /// <inheritdoc/>
        public PrintCommand Print { get; } = new PrintInventoryTransitionTableCommand();

        /// <inheritdoc/>
        public void ValidateBeforePrinting()
        {
            if (string.IsNullOrWhiteSpace(BouquetPartsCode)) { throw new ApplicationException("印刷対象の花コードが指定されていません。"); }
            if (ExpiryDate == 0) { throw new ApplicationException($"品質維持可能日数が0日です。{BouquetPartsCode} ({SelectedParts.PartsName}) の品質維持可能日数が設定されているか確認してください。"); }
        }
        #endregion // IPrintable
    }
}
