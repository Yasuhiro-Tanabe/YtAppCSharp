using MemorieDeFleurs.Models.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsSummaryViewModel : NotificationObject
    {
        public BouquetPartsSummaryViewModel(BouquetPart part) : base()
        {
            Update(part);
        }

        public void Update(BouquetPart part)
        {
            _code = part.Code;
            _name = part.Name;
            RaisePropertyChanged(nameof(PartsCode), nameof(PartsName));
        }

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
    }
}
