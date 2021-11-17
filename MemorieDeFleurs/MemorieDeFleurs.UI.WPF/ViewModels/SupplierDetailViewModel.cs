using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class SupplierDetailViewModel : DetailViewModelBase, IEditableAndFixable, IAppendableRemovable, IReloadable
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
        public SupplierPartsViewModel SelectedSuppling
        {
            get { return _suppling; }
            set { SetProperty(ref _suppling, value); }
        }
        private SupplierPartsViewModel _suppling;

        /// <summary>
        /// 仕入可能な単品候補一覧
        /// </summary>
        public ObservableCollection<SupplierPartsViewModel> PartsCandidate { get; } = new ObservableCollection<SupplierPartsViewModel>();

        /// <summary>
        /// 選択中の仕入可能な単品候補
        /// </summary>
        public SupplierPartsViewModel SelectedCandidate
        {
            get { return _candidate; }
            set { SetProperty(ref _candidate, value); }
        }
        private SupplierPartsViewModel _candidate;

        /// <summary>
        /// 商品構成編集中かどうか
        /// </summary>
        public bool IsEditing
        {
            get { return _editing; }
            private set { SetProperty(ref _editing, value); }
        }
        private bool _editing = false;
        #endregion // プロパティ

        #region コマンド
        public ICommand Edit { get; } = new EditCommand();
        public ICommand Fix { get; } = new FixCommand();
        public ICommand Append { get; } = new AppendToListCommand();
        public ICommand Remove { get; } = new RemoveFromListCommand();
        #endregion // コマンド

        public void Update(Supplier supplier)
        {
            SupplierCode = supplier.Code;
            SupplierName = supplier.Name;
            Address1 = supplier.Address1;
            Address2 = supplier.Address2;
            EmailAddress = supplier.EmailAddress;
            TelephoneNumber = supplier.Telephone;
            FaxNumber = supplier.Fax;

            SupplingParts.Clear();
            foreach(var parts in supplier.SupplyParts)
            {
                SupplingParts.Add(new SupplierPartsViewModel(parts));
            }

            SelectedSuppling = null;
            PartsCandidate.Clear();
            SelectedCandidate = null;

            IsEditing = false;

            IsDirty = false;
        }

        #region IReloadable
        /// <inheritdoc/>
        public void UpdateProperties()
        {
            if (SupplierCode == 0)
            {
                throw new ApplicationException($"仕入先コードが指定されていません。");
            }
            else
            {
                var supplier = MemorieDeFleursUIModel.Instance.FindSupplier(SupplierCode);
                if (supplier == null)
                {
                    throw new ApplicationException($"該当する仕入先がありません。");
                }
                else
                {
                    Update(supplier);
                }
            }
        }
        #endregion // IReloadable

        public override void Validate()
        {
            var result = new ValidateFailedException();

            if(string.IsNullOrWhiteSpace(SupplierName))
            {
                result.Append("仕入先名称が入力されていません。");
            }
            if(string.IsNullOrWhiteSpace(Address1))
            {
                result.Append("住所1が入力されていません。");
                if(!string.IsNullOrWhiteSpace(Address2))
                {
                    result.Append("住所1を先に入力してください。");
                }
            }

            if (result.ValidationErrors.Count > 0) { throw result; }
        }

        #region IEditableFixable
        public void OpenEditView()
        {
            PartsCandidate.Clear();
            foreach (var parts in MemorieDeFleursUIModel.Instance.FindAllBouquetParts())
            {
                if (SupplingParts.SingleOrDefault(p => p.PartsCode == parts.Code) == null)
                {
                    // 単品仕入先に登録されていない単品が対象
                    PartsCandidate.Add(new SupplierPartsViewModel(parts));
                }
            }
            IsEditing = true;
        }

        public void FixEditing()
        {
            RaisePropertyChanged(nameof(PartsText));
            IsEditing = false;
        }
        #endregion // IEditableFixable

        #region IAddableRemovable
        public void AppendToList()
        {
            var parts = SelectedCandidate;

            SupplingParts.Add(parts);
            SelectedSuppling = parts;

            SelectedCandidate = null;
            PartsCandidate.Remove(parts);
        }

        public void RemoveFromList()
        {
            var parts = SelectedSuppling;

            PartsCandidate.Add(parts);
            SelectedCandidate = parts;

            SelectedSuppling = null;
            SupplingParts.Remove(parts);
        }
        #endregion // IAddableRemovable

        public override void SaveToDatabase()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();

                Validate();

                var supplier = new Supplier()
                {
                    Code = SupplierCode,
                    Name = SupplierName,
                    Address1 = Address1,
                    Address2 = Address2,
                    EmailAddress = EmailAddress,
                    Telephone = TelephoneNumber,
                    Fax = FaxNumber
                };
                foreach(var parts in SupplingParts)
                {
                    supplier.SupplyParts.Add(new PartSupplier() { SupplierCode = supplier.Code, PartCode = parts.PartsCode });
                }

                var saved = MemorieDeFleursUIModel.Instance.Save(supplier);

                Update(saved);

                LogUtil.Info($"Supplier {SupplierCode} is saved.");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }

        public override void ClearProperties()
        {
            SupplierCode = 0;
            SupplierName = string.Empty;
            Address1 = string.Empty;
            Address2 = string.Empty;
            EmailAddress = string.Empty;
            FaxNumber = string.Empty;
            TelephoneNumber = string.Empty;
            IsEditing = false;

            SupplingParts.Clear();
            PartsCandidate.Clear();
            SelectedCandidate = null;
            SelectedSuppling = null;

            IsDirty = false;
        }
    }
}
