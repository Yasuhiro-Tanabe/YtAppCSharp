using MemorieDeFleurs.Models.Entities;

using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsDetailViewModel : ViewModelBase, ITabItemControlViewModel
    {
        private BouquetPart Part { get; set; } = new BouquetPart();

        #region プロパティ
        public string Header { get; } = "単品詳細";

        public string PartsCode { get { return Part.Code; } }


        public string PartsName
        {
            get { return Part.Name; }
            set { SetProperty(() => Part.Name = value); }
        }

        public int QuantitiesParLot
        {
            get { return Part.QuantitiesPerLot; }
            set { SetProperty(() => Part.QuantitiesPerLot = value); }
        }

        public int LeadTime
        {
            get { return Part.LeadTime; }
            set { SetProperty(() => Part.LeadTime = value); }
        }

        public int ExpiryDate
        {
            get { return Part.ExpiryDate; }
            set { SetProperty(() => Part.ExpiryDate = value); }
        }


        #endregion // プロパティ

    }
}
