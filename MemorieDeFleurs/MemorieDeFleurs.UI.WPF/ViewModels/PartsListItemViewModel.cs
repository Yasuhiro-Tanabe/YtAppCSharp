namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class PartsListItemViewModel : NotificationObject
    {
        public string PartsCode
        {
            get { return _parts; }
            set { SetProperty(ref _parts, value); }
        }
        private string _parts;

        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }
        private int _quantity;

    }
}
