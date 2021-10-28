using MemorieDeFleurs.Models.Entities;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class SupplierPartsViewModel : NotificationObject
    {
        public string PartsCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        public string PartsName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        public SupplierPartsViewModel(BouquetPart parts)
        {
            _code = parts.Code;
            _name = parts.Name;
            RaisePropertyChanged(nameof(PartsCode), nameof(PartsName));
        }
        public SupplierPartsViewModel(PartSupplier parts)
        {
            _code = parts.PartCode;
            _name = parts.Part.Name;
            RaisePropertyChanged(nameof(PartsCode), nameof(PartsName));
        }
    }
}
