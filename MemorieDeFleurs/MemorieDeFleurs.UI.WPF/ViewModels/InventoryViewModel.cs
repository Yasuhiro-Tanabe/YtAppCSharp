using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class InventoryViewModel : ListViewModelBase, IReloadable
    {
        public InventoryViewModel() : base("単品破棄") { }

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
        public ICommand Discard { get; } = new DiscardBouquetPartsCommand();
        #endregion // コマンド

        #region IReloadable
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
