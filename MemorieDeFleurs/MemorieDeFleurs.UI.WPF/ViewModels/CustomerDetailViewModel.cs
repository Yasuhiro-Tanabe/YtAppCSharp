using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class CustomerDetailViewModel : DetailViewModelBase
    {
        public static string Name { get; } = "得意先詳細";
        public CustomerDetailViewModel() : base(Name) { }


        #region プロパティ
        /// <summary>
        /// 得意先ID
        /// </summary>
        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        private int _id;

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
        /// 名前
        /// </summary>
        public string CustomerName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password
        {
            get { return _passwd; }
            set { SetProperty(ref _passwd, value); }
        }
        private string _passwd;

        /// <summary>
        /// カード番号
        /// </summary>
        public string CardNo
        {
            get { return _cardNo; }
            set { SetProperty(ref _cardNo, value); }
        }
        private string _cardNo;

        /// <summary>
        /// この得意先が発注した商品のお届け先一覧
        /// </summary>
        public ObservableCollection<ShippingAddressViewModel> ShippingAddresses { get; } = new ObservableCollection<ShippingAddressViewModel>();
        #endregion // プロパティ

        public void Update(Customer c)
        {
            _id = c.ID;
            _email = c.EmailAddress;
            _name = c.Name;
            _passwd = c.Password;
            _cardNo = c.CardNo;

            ShippingAddresses.Clear();
            foreach(var sa in c.ShippingAddresses)
            {
                ShippingAddresses.Add(new ShippingAddressViewModel(sa));
            }

            RaisePropertyChanged(nameof(ID), nameof(EmailAddress), nameof(CustomerName), nameof(Password), nameof(CardNo), nameof(ShippingAddresses));

            IsDirty = false;
        }

        public override void Validate()
        {
            var ex = new ValidateFailedException();
            if(string.IsNullOrWhiteSpace(Name))
            {
                ex.Append("得意先氏名が入力されていません。");
            }
            if(string.IsNullOrWhiteSpace(EmailAddress))
            {
                ex.Append("e-メールアドレスが入力されていません。");
            }

            if(ex.ValidationErrors.Count > 0) { throw ex; }
        }

        public override void Update()
        {
            if(_id == 0)
            {
                throw new ApplicationException($"得意先IDが入力されていません。");
            }
            else
            {
                var customer = MemorieDeFleursUIModel.Instance.FindCustomer(_id);
                if(customer == null)
                {
                    throw new ApplicationException($"該当する得意先がありません: ID={_id}");
                }
                else
                {
                    Update(customer);
                }
            }
        }

        public override void SaveToDatabase()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();

                Validate();

                var customer = new Customer()
                {
                    ID = ID,
                    Name = CustomerName,
                    EmailAddress = EmailAddress,
                    Password = Password,
                    CardNo = CardNo
                };

                if (ShippingAddresses.Count > 0)
                {
                    foreach (var vm in ShippingAddresses)
                    {
                        var sa = new ShippingAddress()
                        {
                            CustomerID = ID,
                            ID = vm.ID,
                            Name = vm.Name,
                            Address1 = vm.Address1,
                            Address2 = vm.Address2,
                            LatestOrderDate = vm.LatestOrderDate
                        };
                        customer.ShippingAddresses.Add(sa);
                    }
                }

                var saved = MemorieDeFleursUIModel.Instance.Save(customer);
                Update(saved);

                LogUtil.Info($"Customer {ID} saved.");
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
    }
}
