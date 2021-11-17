using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class CustomerDetailViewModel : DetailViewModelBase, IReloadable
    {
        public static string Name { get; } = "得意先詳細";
        public CustomerDetailViewModel() : base(Name) { }


        #region プロパティ
        /// <summary>
        /// 得意先ID
        /// </summary>
        public int CustomerID
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
        public string CardNumber
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
            CustomerID = c.ID;
            EmailAddress = c.EmailAddress;
            CustomerName = c.Name;
            Password = c.Password;
            CardNumber = c.CardNo;

            ShippingAddresses.Clear();
            foreach(var sa in c.ShippingAddresses)
            {
                ShippingAddresses.Add(new ShippingAddressViewModel(sa));
            }

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

        #region IReloadable
        /// <inheritdoc/>
        public void UpdateProperties()
        {
            if (CustomerID == 0)
            {
                throw new ApplicationException($"得意先IDが入力されていません。");
            }
            else
            {
                var customer = MemorieDeFleursUIModel.Instance.FindCustomer(CustomerID);
                if (customer == null)
                {
                    throw new ApplicationException($"該当する得意先がありません: ID={CustomerID}");
                }
                else
                {
                    Update(customer);
                }
            }
        }
        #endregion // IReloadable

        public override void SaveToDatabase()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();

                Validate();

                var customer = new Customer()
                {
                    ID = CustomerID,
                    Name = CustomerName,
                    EmailAddress = EmailAddress,
                    Password = Password,
                    CardNo = CardNumber
                };

                if (ShippingAddresses.Count > 0)
                {
                    foreach (var vm in ShippingAddresses)
                    {
                        var sa = new ShippingAddress()
                        {
                            CustomerID = CustomerID,
                            ID = vm.ShippingID,
                            Name = vm.NameOfShipping,
                            Address1 = vm.Address1,
                            Address2 = vm.Address2,
                            LatestOrderDate = vm.LatestOrderDate
                        };
                        customer.ShippingAddresses.Add(sa);
                    }
                }

                var saved = MemorieDeFleursUIModel.Instance.Save(customer);
                Update(saved);

                LogUtil.Info($"Customer {CustomerID} saved.");
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
            CustomerID = 0;
            CustomerName = string.Empty;
            EmailAddress = string.Empty;
            CardNumber = string.Empty;
            Password = string.Empty;
            ShippingAddresses.Clear();
        }
    }
}
