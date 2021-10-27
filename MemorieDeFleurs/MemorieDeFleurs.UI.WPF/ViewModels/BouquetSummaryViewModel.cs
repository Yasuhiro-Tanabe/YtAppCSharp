using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetSummaryViewModel : NotificationObject
    {
        public event EventHandler DetailViewOpening;

        public BouquetSummaryViewModel(Bouquet bouquet)
        {
            Update(bouquet);
        }
        #region プロパティ
        public string BouquetCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        public string BouquetName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        public Visibility ActionVisivility { get { return _isVisible ? Visibility.Visible : Visibility.Collapsed; } }
        private bool _isVisible = false;

        public string ImageFileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }
        private string _fileName;

        public string PartsList
        {
            get { return _parts; }
            set { SetProperty(ref _parts, value); }
        }
        private string _parts;
        #endregion // プロパティ

        #region コマンド
        public ICommand Remove { get; } = new RemoveListItemCommand();
        public ICommand Detail { get; } = new OpenBouquetDetailViewCommand();
        #endregion // コマンド

        public void ShowCommandButtons()
        {
            _isVisible = true;
            RaisePropertyChanged(nameof(ActionVisivility));
        }

        public void HideCommandButtons()
        {
            _isVisible = false;
            RaisePropertyChanged(nameof(ActionVisivility));
        }

        public void Update(Bouquet bouquet)
        {
            BouquetCode = bouquet.Code;
            BouquetName = bouquet.Name;
            ImageFileName = bouquet.Image;

            if(bouquet.PartsList.Count > 0)
            {
                PartsList = string.Join(", ", bouquet.PartsList.Select(p => $"{p.PartsCode} x{p.Quantity}"));
            }

            RaisePropertyChanged(nameof(BouquetCode), nameof(BouquetName), nameof(ImageFileName), nameof(PartsList));
        }

        public void RemoveMe()
        {
            RaisePropertyChanged(nameof(RemoveMe));
        }

        public void OpenDetailView()
        {
            LogUtil.DEBULOG_MethodCalled();
            DetailViewOpening?.Invoke(this, null);
        }
    }
}
