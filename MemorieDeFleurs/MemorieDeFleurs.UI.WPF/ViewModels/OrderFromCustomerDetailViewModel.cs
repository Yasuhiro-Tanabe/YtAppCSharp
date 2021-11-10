using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class OrderFromCustomerDetailViewModel : DetailViewModelBase, IEditableAndFixable
    {
        public static string Name { get; } = "得意先受注詳細";

        public OrderFromCustomerDetailViewModel() : base(Name)
        {
            LoadCustomers();
            LoadBouquets();
        }

        #region プロパティ
        /// <summary>
        /// 受注番号
        /// </summary>
        public string OrderNo
        {
            get { return _orderNo; }
            set { SetProperty(ref _orderNo, value); }
        }
        private string _orderNo;

        /// <summary>
        /// 得意先一覧
        /// </summary>
        public ObservableCollection<CustomerSummaryViewModel> Customers { get; } = new ObservableCollection<CustomerSummaryViewModel>();

        /// <summary>
        /// 現在選択中の得意先
        /// </summary>
        public CustomerSummaryViewModel SelectedCustomer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }
        private CustomerSummaryViewModel _customer;

        /// <summary>
        /// 注文日(表示用)
        /// </summary>
        public string OrderDateText { get { return OrderDate.ToString("yyyy/MM/dd"); } }

        /// <summary>
        /// 注文日
        /// </summary>
        public DateTime OrderDate
        {
            get { return _orderDate; }
            set { SetProperty(ref _orderDate, value); }
        }
        private DateTime _orderDate = DateTime.Today;

        /// <summary>
        /// お届け日
        /// </summary>
        public DateTime ArrivalDate
        {
            get { return _arrival; }
            set { SetProperty(ref _arrival, value); }
        }
        private DateTime _arrival = DateTime.Today.AddDays(1);

        /// <summary>
        /// お届けメッセージ
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        private string _message;

        /// <summary>
        /// 選択可能なお届け先一覧
        /// </summary>
        public ObservableCollection<ShippingAddressViewModel> ShippingAddresses { get; } = new ObservableCollection<ShippingAddressViewModel>();

        /// <summary>
        /// 現在選択中のお届け先：「新規登録」を含む
        /// </summary>
        public ShippingAddressViewModel SelectedShippingAddress { get; set; }

        public ObservableCollection<BouquetSummaryViewModel> Bouquets { get; } = new ObservableCollection<BouquetSummaryViewModel>();
        public BouquetSummaryViewModel SelectedBouquet { get; set; }

        /// <summary>
        /// お届け先名称
        /// </summary>
        public string ShippingName
        {
            get { return _shippingName; }
            set { SetProperty(ref _shippingName, value); }
        }
        private string _shippingName;

        /// <summary>
        /// お届け先住所1
        /// </summary>
        public string Address1
        {
            get { return _shippingAddress1; }
            set { SetProperty(ref _shippingAddress1, value); }
        }
        private string _shippingAddress1;

        /// <summary>
        /// お届け先住所2
        /// </summary>
        public string Address2
        {
            get { return _shippingAddress2; }
            set { SetProperty(ref _shippingAddress2, value); }
        }
        private string _shippingAddress2;

        /// <summary>
        /// お届け先選択モード
        /// </summary>
        public Visibility ShippingAddressListVisivility { get { return _listVisible ? Visibility.Visible : Visibility.Collapsed; } }
        private bool _listVisible;
        #endregion // プロパティ

        #region コマンド
        public ICommand Edit { get; } = new EditCommand();
        public ICommand Fix { get; } = new FixCommand();
        public ICommand Order { get; } = new OrderCommand();
        public ICommand Cancel { get; } = new CancelOrderCommand();
        public ICommand ChangeArrivalDate { get; } = new ChangeDateCommand();
        public ICommand ChangeShippingAddress { get; } = new ChangeShippingAddressCommand();
        #endregion // コマンド

        public void Update(OrderFromCustomer order)
        {
            _orderNo = order.ID;
            _message = order.Message;
            _orderDate = order.OrderDate;
            _arrival = order.ShippingDate.AddDays(1);

            LoadCustomers();
            SelectedCustomer = Customers.SingleOrDefault(c => c.CustomerID == order.CustomerID);

            LoadBouquets();
            SelectedBouquet = Bouquets.SingleOrDefault(b => b.BouquetCode == order.BouquetCode);

            LoadShippingAddresses(order.CustomerID);
            SelectedShippingAddress = ShippingAddresses.SingleOrDefault(a => a.ID == order.ShippingAddressID);

            if(SelectedShippingAddress != null)
            {
                _shippingName = order.ShippingAddress.Name;
                _shippingAddress1 = order.ShippingAddress.Address1;
                _shippingAddress2 = order.ShippingAddress.Address2;
                RaisePropertyChanged(nameof(ShippingName), nameof(Address1), nameof(Address2));
            }

            _listVisible = false;
            RaisePropertyChanged(nameof(OrderNo), nameof(Message), nameof(OrderDateText), nameof(ArrivalDate),
                nameof(Customers), nameof(SelectedCustomer), nameof(Bouquets), nameof(SelectedBouquet),
                nameof(ShippingAddresses), nameof(SelectedShippingAddress), nameof(ShippingAddressListVisivility));

            IsDirty = false;
        }

        public void LoadShippingAddresses(int  customerID)
        {
            ShippingAddresses.Clear();
            foreach (var addr in MemorieDeFleursUIModel.Instance.FindAllShippingAddressOfCustomer(customerID))
            {
                ShippingAddresses.Add(new ShippingAddressViewModel(addr));
            }
        }

        private void LoadCustomers()
        {
            Customers.Clear();
            foreach (var customer in MemorieDeFleursUIModel.Instance.FindAllCustomers())
            {
                Customers.Add(new CustomerSummaryViewModel(customer));
            }
        }

        private void LoadBouquets()
        {
            Bouquets.Clear();
            foreach(var bouquet in MemorieDeFleursUIModel.Instance.FindAllBouquets())
            {
                Bouquets.Add(new BouquetSummaryViewModel(bouquet));
            }
        }

        public override void Update()
        {
            if(string.IsNullOrWhiteSpace(_orderNo))
            {
                throw new ApplicationException("受注番号が指定されていません。");
            }
            else
            {
                var found = MemorieDeFleursUIModel.Instance.FindOrdersFromCustomer(_orderNo);
                if(found == null)
                {
                    throw new ApplicationException($"該当する受注履歴がありません：{_orderNo}");
                }
                else
                {
                    Update(found);
                }
            }
        }

        public override void ClearProperties()
        {
            _orderNo = string.Empty;
            _message = string.Empty;
            _shippingName = string.Empty;
            _shippingAddress1 = string.Empty;
            _shippingAddress2 = string.Empty;
            _orderDate = DateTime.Today;
            _arrival = DateTime.Today.AddDays(1);

            SelectedCustomer = null;
            LoadCustomers();

            SelectedBouquet = null;
            LoadBouquets();

            SelectedShippingAddress = null;
            ShippingAddresses.Clear();

            _listVisible = false;

            RaisePropertyChanged(nameof(OrderNo), nameof(OrderDate), nameof(OrderDateText), nameof(ArrivalDate), nameof(Customers), nameof(SelectedCustomer),
                nameof(Message), nameof(ShippingAddresses), nameof(SelectedShippingAddress), nameof(ShippingName), nameof(Address1), nameof(Address2),
                nameof(ShippingAddressListVisivility));

            IsDirty = false;
        }

        public override void Validate()
        {
            var ex = new ValidateFailedException();
            if (SelectedBouquet == null)
            {
                ex.Append("商品が選択されていません。");
            }
            if (SelectedCustomer == null)
            {
                ex.Append("得意先が選択されていません。");
            }
            if (SelectedShippingAddress == null)
            {
                if(string.IsNullOrWhiteSpace(ShippingName))
                {
                    ex.Append("お届け先名称が指定されていません。");
                }
                if(string.IsNullOrWhiteSpace(Address1))
                {
                    ex.Append("お届け先住所1が指定されていません。");
                    if(!string.IsNullOrWhiteSpace(Address2))
                    {
                        ex.Append("お届け先住所1が空です。お届け先住所1を先に入力してください");
                    }
                }
            }

            var nearest = DateTime.Today.AddDays(SelectedBouquet.LeadTime);
            if (ArrivalDate < nearest)
            {
                ex.Append($"商品 {SelectedBouquet.BouquetCode} は {nearest:yyyy/MM/dd} 以降でなければお届けできません。");
            }

            if (ex.ValidationErrors.Count > 0)
            {
                throw ex;
            }
        }

        #region IEditableFixable
        public void OpenEditView()
        {
            if(SelectedCustomer == null)
            {
                throw new ApplicationException("得意先を先に指定してください。");
            }

            // 既存お届け先を選択中だったら、そのお届け先を選択状態にしてリストを開く。
            // お届け先IDをキーに検索しないのは、前回選択後にユーザが入力内容を変更しているかもしれないため。
            // 入力内容が異なる場合は、新規の登録先として扱いたい。
            LoadShippingAddresses(SelectedCustomer.CustomerID);
            SelectedShippingAddress = ShippingAddresses
                .Where(s => s.Name == _shippingName)
                .Where(s => s.Address1 == _shippingAddress1)
                .Where(s => s.Address2 == _shippingAddress2)
                .SingleOrDefault();

            _listVisible = true;
            RaisePropertyChanged(nameof(ShippingAddresses), nameof(SelectedShippingAddress), nameof(ShippingAddressListVisivility));
        }

        public void FixEditing()
        {
            if(SelectedShippingAddress != null)
            {
                _shippingName = SelectedShippingAddress.Name;
                _shippingAddress1 = SelectedShippingAddress.Address1;
                _shippingAddress2 = SelectedShippingAddress.Address2;

                RaisePropertyChanged(nameof(ShippingName), nameof(Address1), nameof(Address2));
            }

            _listVisible = false;
            RaisePropertyChanged(nameof(ShippingAddressListVisivility));
        }
        #endregion // IEditableFixable

        public void OrderMe()
        {
            if(string.IsNullOrWhiteSpace(_orderNo))
            {
                Validate();

                var address = new ShippingAddress()
                {
                    ID = SelectedShippingAddress == null ? 0 : SelectedShippingAddress.ID,
                    CustomerID = SelectedCustomer.CustomerID,
                    Name = ShippingName,
                    Address1 = Address1,
                    Address2 = Address2,
                    LatestOrderDate = DateTime.Today
                };

                var order = new OrderFromCustomer()
                {
                    OrderDate = OrderDate,
                    Bouquet = MemorieDeFleursUIModel.Instance.FindBouquet(SelectedBouquet.BouquetCode),
                    ShippingAddress = MemorieDeFleursUIModel.Instance.Save(address),
                };

                OrderNo = MemorieDeFleursUIModel.Instance.OrderFromCustomer(order, ArrivalDate);
                Update();
            }
            else
            {
                throw new ApplicationException("すでに発注済です。");
            }
        }

        public void CancelMe()
        {
            if(string.IsNullOrWhiteSpace(_orderNo))
            {
                throw new ApplicationException("発注されていません。");
            }
            else
            {
                MemorieDeFleursUIModel.Instance.CancelOrderFromCustomer(_orderNo);
                Cancel.Execute(this);
            }
        }

        public void ChangeMyArrivalDate()
        {
            if (string.IsNullOrWhiteSpace(_orderNo))
            {
                throw new ApplicationException("発注されていません。");
            }
            else
            {
                MemorieDeFleursUIModel.Instance.ChangeArrivalDateOfOrderFromCustomer(_orderNo, ArrivalDate);
            }
        }
    }
}
