using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class SupplierDetailViewModel : DetailViewModelBase
    {
        public static string Name { get; } = "仕入先詳細";

        public SupplierDetailViewModel() : base(Name) { }

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

        /// <summary>
        /// 仕入先住所1
        /// </summary>
        public string Address1
        {
            get { return _address1; }
            set { SetProperty(ref _address1, value); }
        }
        private string _address1;

        /// <summary>
        /// 仕入先住所2
        /// </summary>
        public string Address2
        {
            get { return _address2; }
            set { SetProperty(ref _address2, value); }
        }
        private string _address2;

        /// <summary>
        /// e-メールアドレス
        /// </summary>
        public string EmailAddress
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }
        private string _email;

        /// <summary>
        /// 連絡先電話番号
        /// </summary>
        public string TelephoneNumber
        {
            get { return _tel; }
            set { SetProperty(ref _tel, value); }
        }
        private string _tel;

        /// <summary>
        /// 連絡先FAX番号
        /// </summary>
        public string FaxNumber
        {
            get { return _fax; }
            set { SetProperty(ref _fax, value); }
        }
        private string _fax;

        /// <summary>
        /// 現在仕入先に登録されている仕入可能な単品一覧 (サマリ表示用)
        /// </summary>
        public string PartsText
        {
            get { return string.Join(", ", SupplingParts.Select(p => p.PartsCode)); }
        }

        /// <summary>
        /// 現在仕入先に登録されている仕入可能な単品一覧 (データベース参照更新用)
        /// </summary>
        private ISet<string> _parts = new SortedSet<string>();

        /// <summary>
        /// 現在仕入先に登録されている仕入可能な単品一覧 (一覧表示用)
        /// </summary>
        public ObservableCollection<SupplierPartsViewModel> SupplingParts { get; } = new ObservableCollection<SupplierPartsViewModel>();

        /// <summary>
        /// 選択中の仕入可能な単品
        /// </summary>
        public SupplierPartsViewModel SelectedSuppling { get; set; }

        /// <summary>
        /// 仕入可能な単品候補一覧
        /// </summary>
        public ObservableCollection<SupplierPartsViewModel> PartsCandidate { get; } = new ObservableCollection<SupplierPartsViewModel>();

        /// <summary>
        /// 選択中の仕入可能な単品候補
        /// </summary>
        public SupplierPartsViewModel SelectedCandidate { get; set; }

        /// <summary>
        /// 商品構成編集中に表示するコントロールの可視性
        /// </summary>
        public Visibility EditingModeVisivility { get { return _editing ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// 商品構成編集中でないときに表示するコントロールの可視性
        /// </summary>
        public Visibility ViewModeVisivility { get { return _editing ? Visibility.Collapsed : Visibility.Visible; } }
        private bool _editing = false;
        #endregion // プロパティ

        #region コマンド
        public ICommand Edit { get; } = new EditPartsListCommand();
        public ICommand Fix { get; } = new FixPartsListCommand();
        public ICommand Append { get; } = new AddToListItemCommand();
        public ICommand Remove { get; } = new RemoveFromListItemCommand();
        public ICommand Reload { get; } = new ReloadDetailCommand();
        #endregion // コマンド

        public void Update(Supplier supplier)
        {
            _code = supplier.Code;
            _name = supplier.Name;
            _address1 = supplier.Address1;
            _address2 = supplier.Address2;
            _email = supplier.EmailAddress;
            _tel = supplier.Telephone;
            _fax = supplier.Fax;

            SupplingParts.Clear();
            foreach(var parts in supplier.SupplyParts)
            {
                SupplingParts.Add(new SupplierPartsViewModel(parts));
            }

            SelectedSuppling = null;
            PartsCandidate.Clear();
            SelectedCandidate = null;

            _editing = false;
            RaisePropertyChanged(nameof(SupplierCode), nameof(SupplierName), nameof(Address1), nameof(Address2),
                nameof(EmailAddress), nameof(TelephoneNumber), nameof(FaxNumber), nameof(PartsText),
                nameof(EditingModeVisivility), nameof(ViewModeVisivility));
        }

        public void Update()
        {
            if(_code == 0)
            {
                throw new ApplicationException($"仕入先コードが指定されていません。");
            }
            else
            {
                var supplier = MemorieDeFleursUIModel.Instance.FindSupplier(_code);
                if(supplier == null)
                {
                    throw new ApplicationException($"該当する仕入先がありません。");
                }
                else
                {
                    Update(supplier);
                }
            }
        }

        public override void Validate()
        {
            var result = new ValidateFailedException();

            if(string.IsNullOrWhiteSpace(_name))
            {
                result.Append("仕入先名称が入力されていません。");
            }
            if(string.IsNullOrWhiteSpace(_address1))
            {
                result.Append("住所1が入力されていません。");
                if(!string.IsNullOrWhiteSpace(_address2))
                {
                    result.Append("住所1を先に入力してください。");
                }
            }

            if (result.ValidationErrors.Count > 0) { throw result; }
        }

        public void EditSupplierParts()
        {
            PartsCandidate.Clear();
            var allParts = MemorieDeFleursUIModel.Instance.FindAllBouquetParts();
            foreach (var parts in allParts)
            {
                if (SupplingParts.SingleOrDefault(p => p.PartsCode == parts.Code) == null)
                {
                    // 単品仕入先に登録されていない単品が対象
                    PartsCandidate.Add(new SupplierPartsViewModel(parts));
                }
            }
            _editing = true;

            RaisePropertyChanged(nameof(SupplingParts), nameof(SelectedSuppling), nameof(PartsCandidate), nameof(SelectedCandidate),
                nameof(EditingModeVisivility), nameof(ViewModeVisivility));
        }

        public void FixSupplierParts()
        {
            var parts = SupplingParts.Select(p => p.PartsCode);
            _editing = false;

            RaisePropertyChanged(nameof(EditingModeVisivility), nameof(ViewModeVisivility), nameof(PartsText));
        }

        public void AppnedToSupplingParts()
        {
            var parts = SelectedCandidate;

            SupplingParts.Add(parts);
            SelectedSuppling = parts;

            SelectedCandidate = null;
            PartsCandidate.Remove(parts);

            RaisePropertyChanged(nameof(SupplingParts), nameof(SelectedSuppling), nameof(PartsCandidate), nameof(SelectedCandidate),
                nameof(EditingModeVisivility), nameof(ViewModeVisivility));
        }

        public void RemoveFromSupplingParts()
        {
            var parts = SelectedSuppling;

            PartsCandidate.Add(parts);
            SelectedCandidate = parts;

            SelectedSuppling = null;
            SupplingParts.Remove(parts);

            RaisePropertyChanged(nameof(SupplingParts), nameof(SelectedSuppling), nameof(PartsCandidate), nameof(SelectedCandidate),
                nameof(EditingModeVisivility), nameof(ViewModeVisivility));
        }
    }
}
