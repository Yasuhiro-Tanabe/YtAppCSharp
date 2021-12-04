using MemorieDeFleurs.Logging;
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
    /// 単品破棄画面 (タブ要素画面) のビューモデル
    /// </summary>
    public class InventoryViewModel : ListViewModelBase, IReloadable
    {
        /// <summary>
        /// ビューモデルの名称：<see cref="TabItemControlViewModelBase.Header"/> や <see cref="MainWindowViiewModel.FindTabItem(string)"/> に渡すクラス定数として使用する。
        /// </summary>
        public static string Name { get { return TextResourceFinder.FindText("DiscardParts"); } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InventoryViewModel() : base(Name) { }

        #region プロパティ
        /// <summary>
        /// 破棄対象日
        /// </summary>
        public DateTime ActionDate
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }
        private DateTime _date = DateTime.Today;

        /// <summary>
        /// 単品の在庫一覧
        /// </summary>
        public ObservableCollection<InventorySummaryViewModel> Inventories { get; } = new ObservableCollection<InventorySummaryViewModel>();
        #endregion // プロパティ

        #region コマンド
        /// <summary>
        /// 破棄実行ボタン押下時に実行されるコマンド
        /// </summary>
        public ICommand Discard { get; } = new DiscardBouquetPartsCommand();
        #endregion // コマンド

        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            Inventories.Clear();
            foreach(var i in MemorieDeFleursUIModel.Instance.FindInventoriesAt(ActionDate))
            {
                Inventories.Add(new InventorySummaryViewModel(i.Key, i.Value));
            }
            RaisePropertyChanged(nameof(Inventories));
        }
        #endregion IReloadable

        /// <summary>
        /// <see cref="Discard"/> コマンドが実行する処理：指定された単品の破棄を行う
        /// </summary>
        public void DiscardBouquetParts()
        {
            var discardParts = Inventories.Where(p => p.IsChaned).Select(p => Tuple.Create(p.PartsCode, p.DiscardQuantity)).ToArray();
            if(discardParts.Length > 0)
            {
                MemorieDeFleursUIModel.Instance.DiscardInventoruies(ActionDate, discardParts);
                UpdateProperties();
                LogUtil.Info($"{ActionDate:yyyyMMdd}, Parts discarded: {string.Join(", ", discardParts.Select(p => $"{p.Item1} x {p.Item2}"))}");
            }
            else
            {
                LogUtil.Info($"No parts discarded.");
            }
        }
    }
}
