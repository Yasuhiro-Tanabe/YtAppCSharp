using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class SupplierSummaryViewModel : ListItemViewModelBase
    {
        public SupplierSummaryViewModel(Supplier supplier) :base()
        {
            Update(supplier);
        }

        #region プロパティ
        /// <summary>
        /// 仕入先コード
        /// </summary>
        public int SupplierCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private int _code;

        /// <summary>
        /// 仕入先名称
        /// </summary>
        public string SupplierName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        #endregion // プロパティ


        public void Update(Supplier supplier)
        {
            Update(supplier.Code.ToString());
            _code = supplier.Code;
            _name = supplier.Name;
            RaisePropertyChanged(nameof(SupplierCode), nameof(SupplierName));
        }
    }
}
