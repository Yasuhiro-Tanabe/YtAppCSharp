﻿using MemorieDeFleurs.Models.Entities;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class ShippingAddressViewModel : NotificationObject
    {
        #region プロパティ
        /// <summary>
        /// お届け先ID (参照のみ)
        /// </summary>
        public int ID
        {
            get { return _shippingID; }
        }
        private int _shippingID;

        /// <summary>
        /// 得意先ID (参照のみ)
        /// </summary>
        public int CustomerID
        {
            get { return _customerID; }
        }
        private int _customerID;

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
            set { SetProperty(ref _address1, value); }
        }
        private string _address1;

        /// <summary>
        /// お届け先住所2 (入力用)
        /// </summary>
        public string Address2
        {
            get { return _address2; }
            set { SetProperty(ref _address2, value); }
        }
        private string _address2;

        /// <summary>
        /// お届け先氏名
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        /// <summary>
        /// 最新注文日 (表示のみ)
        /// </summary>
        public string LatestOrderDate
        {
            get { return _date; }
        }
        private string _date;
        #endregion // プロパティ

        public ShippingAddressViewModel(ShippingAddress sa)
        {
            Update(sa);
        }

        public void Update(ShippingAddress sa)
        {
            _shippingID = sa.ID;
            _customerID = sa.CustomerID;
            _name = sa.Name;
            _address1 = sa.Address1;
            _address2 = sa.Address2;
            _date = sa.LatestOrderDate.ToString("yyyy/MM/dd");
            RaisePropertyChanged(nameof(ID), nameof(CustomerID), nameof(Name), nameof(Address1), nameof(Address2), nameof(AddressText), nameof(LatestOrderDate));
        }
    }
}
