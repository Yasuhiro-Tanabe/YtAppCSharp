using MemorieDeFleurs.Models.Entities;

using System;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 各お届け先情報を表示するビューモデル
    /// </summary>
    public class ShippingAddressViewModel : NotificationObject
    {
        #region プロパティ
        /// <summary>
        /// お届け先ID (参照のみ)
        /// </summary>
        public int ShippingID
        {
            get { return _shippingID; }
            private set { SetProperty(ref _shippingID, value); }
        }
        private int _shippingID;

        /// <summary>
        /// 得意先ID (参照のみ)
        /// </summary>
        public int CustomerID
        {
            get { return _customerID; }
            private set { SetProperty(ref _customerID, value); }
        }
        private int _customerID;

        /// <summary>
        /// 得意先名称 (参照のみ)
        /// </summary>
        public string CustomerName
        {
            get { return _customerName; }
            private set { SetProperty(ref _customerName, value); }
        }
        private string _customerName;

        /// <summary>
        /// お届け先住所 (表示用)
        /// </summary>
        public string AddressText { get { return $"{_address1} {_address2}"; } }

        /// <summary>
        /// お届け先住所1 (入力用)
        /// </summary>
        public string Address1
        {
            get { return _address1; }
            set
            {
                SetProperty(ref _address1, value);
                RaisePropertyChanged(nameof(AddressText));
            }
        }
        private string _address1;

        /// <summary>
        /// お届け先住所2 (入力用)
        /// </summary>
        public string Address2
        {
            get { return _address2; }
            set
            {
                SetProperty(ref _address2, value);
                RaisePropertyChanged(nameof(AddressText));
            }
        }
        private string _address2;

        /// <summary>
        /// お届け先氏名
        /// </summary>
        public string NameOfShipping
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        /// <summary>
        /// 最新注文日 (表示のみ)
        /// </summary>
        public string LatestOrderDateText
        {
            get { return _date.ToString("yyyy/mm/dd"); }
        }

        /// <summary>
        /// 最新注文日
        /// </summary>
        public DateTime LatestOrderDate
        {
            get { return _date; }
            set
            {
                SetProperty(ref _date, value);
                RaisePropertyChanged(nameof(LatestOrderDateText));
            }
        }
        private DateTime _date;
        #endregion // プロパティ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sa">表示するお届け先情報</param>
        public ShippingAddressViewModel(ShippingAddress sa)
        {
            Update(sa);
        }

        private void Update(ShippingAddress sa)
        {
            ShippingID = sa.ID;
            CustomerID = sa.CustomerID;
            CustomerName = sa.Customer.Name;
            NameOfShipping = sa.Name;
            Address1 = sa.Address1;
            Address2 = sa.Address2;
            LatestOrderDate = sa.LatestOrderDate;
        }
    }
}
