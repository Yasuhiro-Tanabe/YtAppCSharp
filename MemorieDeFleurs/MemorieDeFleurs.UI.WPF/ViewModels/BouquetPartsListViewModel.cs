using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsListViewModel : TabItemControlViewModelBase
    {
        public BouquetPartsListViewModel() : base("単品一覧") { }

        #region プロパティ
        public ObservableCollection<BouquetPartsSummaryViewModel> BouquetParts
        {
            get { return _parts; }
            set { SetProperty(ref _parts, value); }
        }
        private ObservableCollection<BouquetPartsSummaryViewModel> _parts; 

        public BouquetPartsSummaryViewModel CurrentParts { get; set; }
        #endregion // プロパティ

        #region Command
        public ICommand Reload { get; } = new ReloadListCommand();
        #endregion // Command

        public void LoadBouquetParts()
        {
            _parts = new ObservableCollection<BouquetPartsSummaryViewModel>(MemorieDeFleursUIModel.Instance.FindAllBouquetParts().Select(p => new BouquetPartsSummaryViewModel(p)));
            CurrentParts = null;
            RaisePropertyChanged(nameof(BouquetParts), nameof(CurrentParts));
        }
    }
}
