using MemorieDeFleurs.Models.Entities;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class PartsListItemViewModel : NotificationObject
    {
        #region プロパティ
        public string PartsCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        public string PartsName
        {
            get { return _name; }
            set { SetProperty(ref _code, value); }
        }
        private string _name;

        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }
        private int _quantity;

        public int LeadTime
        {
            get { return _leadtime; }
            set { SetProperty(ref _leadtime, value); }
        }
        private int _leadtime;
        #endregion // プロパティ

        public PartsListItemViewModel(BouquetPartsList item)
        {
            _code = item.PartsCode;
            _name = item.Part.Name;
            _quantity = item.Quantity;
            _leadtime = item.Part.LeadTime;
            RaiseAllPropertiesChanged();
        }

        public PartsListItemViewModel(BouquetPart parts)
        {
            _code = parts.Code;
            _name = parts.Name;
            _quantity = 0;
            _leadtime = parts.LeadTime;
            RaiseAllPropertiesChanged();
        }

        public PartsListItemViewModel(OrderDetailsToSupplier detail)
        {
            _code = detail.PartsCode;
            _name = detail.BouquetPart.Name;
            _quantity = detail.LotCount;
            _leadtime = detail.BouquetPart.LeadTime;
            RaiseAllPropertiesChanged();
        }

        public void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged(nameof(PartsCode), nameof(PartsName), nameof(Quantity), nameof(LeadTime));
        }
    }
}
