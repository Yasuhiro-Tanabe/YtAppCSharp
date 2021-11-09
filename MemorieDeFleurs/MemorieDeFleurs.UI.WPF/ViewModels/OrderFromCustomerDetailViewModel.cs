using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class OrderFromCustomerDetailViewModel : DetailViewModelBase
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
        private DateTime _orderDate;

        /// <summary>
        /// お届け日
        /// </summary>
        public DateTime ArrivalDate
        {
            get { return _arrival; }
            set { SetProperty(ref _arrival, value); }
        }
        private DateTime _arrival;

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

        #region // 新規お届け先登録時に使用するプロパティ
        /// <summary>
        /// 新規登録するお届け先名称
        /// </summary>
        public string NewShippingName
        {
            get { return _newShippingName; }
            set { SetProperty(ref _newShippingName, value); }
        }
        private string _newShippingName;

        /// <summary>
        /// 新規登録するお届け先住所1
        /// </summary>
        public string NewAddress1
        {
            get { return _newShippingAddress1; }
            set { SetProperty(ref _newShippingAddress1, value); }
        }
        private string _newShippingAddress1;

        /// <summary>
        /// 新規登録するお届け先住所2
        /// </summary>
        public string NewAddress2
        {
            get { return _newShippingAddress2; }
            set { SetProperty(ref _newShippingAddress2, value); }
        }
        private string _newShippingAddress2;
        #endregion // 新規お届け先登録時に使用するプロパティ

        /// <summary>
        /// お届け先新規登録モード
        /// </summary>
        public Visibility EditingModeVisibility { get { return _editing ? Visibility.Visible : Visibility.Collapsed; } }
        private bool _editing;
        #endregion // プロパティ

        #region コマンド
        public ICommand Edit { get; } = new EditPartsListCommand();
        public ICommand Fix { get; } = new FixPartsListCommand();
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

            _editing = false;
            RaisePropertyChanged(nameof(OrderNo), nameof(Message),
                nameof(OrderDateText), nameof(ArrivalDate),
                nameof(ShippingAddresses), nameof(SelectedShippingAddress), nameof(EditingModeVisibility));

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
            _newShippingName = string.Empty;
            _newShippingAddress1 = string.Empty;
            _newShippingAddress2 = string.Empty;
            _orderDate = DateTime.Today;
            _arrival = DateTime.Today;

            SelectedCustomer = null;
            LoadCustomers();

            SelectedBouquet = null;
            LoadBouquets();

            SelectedShippingAddress = null;
            ShippingAddresses.Clear();

            _editing = false;

            RaisePropertyChanged(nameof(OrderNo), nameof(OrderDate), nameof(OrderDateText), nameof(ArrivalDate), nameof(Customers), nameof(SelectedCustomer),
                nameof(Message), nameof(ShippingAddresses), nameof(SelectedShippingAddress), nameof(NewShippingName), nameof(NewAddress1), nameof(NewAddress2),
                nameof(EditingModeVisibility));
        }
    }
}
