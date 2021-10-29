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
        #endregion // プロパティ

        public PartsListItemViewModel(BouquetPartsList item)
        {
            _code = item.BouquetCode;
            _name = item.Part.Name;
            _quantity = item.Quantity;
            RaisePropertyChanged(nameof(PartsCode), nameof(PartsName), nameof(Quantity));
        }

        public PartsListItemViewModel(BouquetPart parts)
        {
            _code = parts.Code;
            _name = parts.Name;
            _quantity = 0;
            RaisePropertyChanged(nameof(PartsCode), nameof(PartsName), nameof(Quantity));
        }
    }
}
