using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;

using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsDetailViewModel : ViewModelBase, ITabItemControlViewModel
    {
        private BouquetPart Part { get; set; } = new BouquetPart();

        #region プロパティ
        public string Header { get; } = "単品詳細";

        /// <summary>
        /// 花コード
        /// </summary>
        public string PartsCode { get { return Part.Code; } }

        /// <summary>
        /// 単品名称
        /// </summary>
        public string PartsName
        {
            get { return Part.Name; }
            set { SetProperty(() => Part.Name = value); }
        }

        /// <summary>
        /// 購入単位数：1ロットあたりの単品本数
        /// </summary>
        public int QuantitiesParLot
        {
            get { return Part.QuantitiesPerLot; }
            set { SetProperty(() => Part.QuantitiesPerLot = value); }
        }

        /// <summary>
        /// 発注リードタイム [単位：日]
        /// </summary>
        public int LeadTime
        {
            get { return Part.LeadTime; }
            set { SetProperty(() => Part.LeadTime = value); }
        }

        /// <summary>
        /// 品質維持可能日数 [単位：日]
        /// </summary>
        public int ExpiryDate
        {
            get { return Part.ExpiryDate; }
            set { SetProperty(() => Part.ExpiryDate = value); }
        }

        /// <summary>
        /// このビューモデルがバインドされたタブアイテムコントロールが登録されているタブコントロールを持つウィンドウにバインドされたビューモデル
        /// </summary>
        public MainWindowViiewModel ParentViewModel { get; set; }

        #endregion // プロパティ

        #region コマンド
        public ICommand Close { get; } = new CloseTabItemControlCommand();
        #endregion // 
    }
}
