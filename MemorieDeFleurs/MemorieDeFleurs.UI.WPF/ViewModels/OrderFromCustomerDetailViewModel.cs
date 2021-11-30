using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 得意先受注詳細画面のビューモデル
    /// </summary>
    public class OrderFromCustomerDetailViewModel : DetailViewModelBase, IEditableAndFixable, IOrderable, IReloadable
    {
        /// <summary>
        /// ビューモデルの名称：<see cref="TabItemControlViewModelBase.Header"/> や <see cref="MainWindowViiewModel.FindTabItem(string)"/> に渡すクラス定数として使用する。
        /// </summary>
        public static string Name { get; } = "得意先受注詳細";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderFromCustomerDetailViewModel() : base(Name)
        {
            LoadCustomers();
            LoadBouquets();
        }

        #region プロパティ
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
        /// 注文日
        /// </summary>
        public DateTime OrderDate
        {
            get { return _ordered; }
            set { SetProperty(ref _ordered, value); }
        }
        private DateTime _ordered = DateTime.Today;

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
        public ShippingAddressViewModel SelectedShippingAddress
        {
            get { return _shipping; }
            set { SetProperty(ref _shipping, value); }
        }
        private ShippingAddressViewModel _shipping;

        /// <summary>
        /// 選択可能な商品一覧
        /// </summary>
        public ObservableCollection<BouquetSummaryViewModel> Bouquets { get; } = new ObservableCollection<BouquetSummaryViewModel>();

        /// <summary>
        /// 現在選択中の商品
        /// </summary>
        public BouquetSummaryViewModel SelectedBouquet
        {
            get { return _bouquet; }
            set { SetProperty(ref _bouquet, value); }
        }
        private BouquetSummaryViewModel _bouquet;

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
        #endregion // プロパティ

        #region コマンド
        /// <summary>
        /// お届け先変更時に実行するコマンド
        /// </summary>
        public ICommand ChangeShippingAddress { get; } = new ChangeShippingAddressCommand();
        #endregion // コマンド

        /// <summary>
        /// 画面の表示内容を指定された受注情報のそれに更新する
        /// </summary>
        /// <param name="order">受注情報</param>
        public void Update(OrderFromCustomer order)
        {
            OrderNo = order.ID;
            Message = order.Message;
            OrderDate = order.OrderDate;
            ArrivalDate = order.ShippingDate.AddDays(1);

            LoadCustomers();
            SelectedCustomer = Customers.SingleOrDefault(c => c.CustomerID == order.CustomerID);

            LoadBouquets();
            SelectedBouquet = Bouquets.SingleOrDefault(b => b.BouquetCode == order.BouquetCode);

            LoadShippingAddresses(order.CustomerID);
            SelectedShippingAddress = ShippingAddresses.SingleOrDefault(a => a.ShippingID == order.ShippingAddressID);

            if(SelectedShippingAddress != null)
            {
                ShippingName = order.ShippingAddress.Name;
                Address1 = order.ShippingAddress.Address1;
                Address2 = order.ShippingAddress.Address2;
            }

            IsEditing = false;

            IsDirty = false;
        }

        /// <summary>
        /// お届け先を変更する
        /// 
        /// <see cref="ChangeShippingAddress"/> コマンド実行時に呼び出される
        /// </summary>
        public void LoadShippingAddresses()
        {
            if(SelectedCustomer != null)
            {
                LoadShippingAddresses(SelectedCustomer.CustomerID);
            }
            SelectedShippingAddress = null;
        }

        private  void LoadShippingAddresses(int  customerID)
        {
            ShippingAddresses.Clear();
            foreach (var addr in MemorieDeFleursUIModel.Instance.FindAllShippingAddressOfCustomer(customerID))
            {
                ShippingAddresses.Add(new ShippingAddressViewModel(addr));
            }
            RaisePropertyChanged(nameof(ShippingAddress));
        }

        private void LoadCustomers()
        {
            Customers.Clear();
            foreach (var customer in MemorieDeFleursUIModel.Instance.FindAllCustomers())
            {
                Customers.Add(new CustomerSummaryViewModel(customer));
            }
            RaisePropertyChanged(nameof(Customers));
        }

        private void LoadBouquets()
        {
            Bouquets.Clear();
            foreach(var bouquet in MemorieDeFleursUIModel.Instance.FindAllBouquets())
            {
                Bouquets.Add(new BouquetSummaryViewModel(bouquet));
            }
            RaisePropertyChanged(nameof(Bouquets));
        }

        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            if (string.IsNullOrWhiteSpace(OrderNo))
            {
                throw new ApplicationException("受注番号が指定されていません。");
            }
            else
            {
                var found = MemorieDeFleursUIModel.Instance.FindOrdersFromCustomer(OrderNo);
                if (found == null)
                {
                    throw new ApplicationException($"該当する受注履歴がありません：{OrderNo}");
                }
                else
                {
                    Update(found);
                }
            }
        }
        #endregion // IReloadable

        /// <inheritdoc/>
        public override void ClearProperties()
        {
            OrderNo = string.Empty;
            Message = string.Empty;
            ShippingName = string.Empty;
            Address1 = string.Empty;
            Address2 = string.Empty;
            OrderDate = DateTime.Today;
            ArrivalDate = DateTime.Today.AddDays(1);

            SelectedCustomer = null;
            LoadCustomers();

            SelectedBouquet = null;
            LoadBouquets();

            SelectedShippingAddress = null;
            ShippingAddresses.Clear();

            IsEditing = false;

            IsDirty = false;
        }

        /// <inheritdoc/>
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

            var nearest = OrderDate.AddDays(SelectedBouquet.LeadTime);
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
        /// <inheritdoc/>
        public EditCommand Edit { get; } = new EditCommand();

        /// <inheritdoc/>
        public FixCommand Fix { get; } = new FixCommand();

        /// <inheritdoc/>
        public bool IsEditing
        {
            get { return _editing; }
            set { SetProperty(ref _editing, value); }
        }
        private bool _editing;

        /// <inheritdoc/>
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
                .Where(s => s.NameOfShipping == ShippingName)
                .Where(s => s.Address1 == Address1)
                .Where(s => s.Address2 == Address2)
                .SingleOrDefault();

            IsEditing = true;
        }

        /// <inheritdoc/>
        public void FixEditing()
        {
            if(SelectedShippingAddress != null)
            {
                ShippingName = SelectedShippingAddress.NameOfShipping;
                Address1 = SelectedShippingAddress.Address1;
                Address2 = SelectedShippingAddress.Address2;
            }

            IsEditing = false;
        }
        #endregion // IEditableFixable

        #region IOrderable
        /// <inheritdoc/>
        public OrderCommand Order { get; } = new OrderCommand();

        /// <inheritdoc/>
        public CancelOrderCommand Cancel { get; } = new CancelOrderCommand();

        /// <inheritdoc/>
        public ChangeArrivalDateCommand ChangeArrivalDate { get; } = new ChangeArrivalDateCommand();

        /// <inheritdoc/>
        public string OrderNo
        {
            get { return _orderNo; }
            set { SetProperty(ref _orderNo, value); }
        }
        private string _orderNo;


        /// <inheritdoc/>
        public void OrderMe()
        {
            if(string.IsNullOrWhiteSpace(OrderNo))
            {
                Validate();

                var address = new ShippingAddress()
                {
                    ID = SelectedShippingAddress == null ? 0 : SelectedShippingAddress.ShippingID,
                    CustomerID = SelectedCustomer.CustomerID,
                    Name = ShippingName,
                    Address1 = Address1,
                    Address2 = Address2,
                    LatestOrderDate = OrderDate
                };

                var order = new OrderFromCustomer()
                {
                    OrderDate = OrderDate,
                    Bouquet = MemorieDeFleursUIModel.Instance.FindBouquet(SelectedBouquet.BouquetCode),
                    ShippingAddress = MemorieDeFleursUIModel.Instance.Save(address),
                };

                OrderNo = MemorieDeFleursUIModel.Instance.OrderFromCustomer(order, ArrivalDate);
                UpdateProperties();
            }
            else
            {
                throw new ApplicationException("すでに発注済です。");
            }
        }

        /// <inheritdoc/>
        public void CancelMe()
        {
            if(string.IsNullOrWhiteSpace(OrderNo))
            {
                throw new ApplicationException("発注されていません。");
            }
            else
            {
                MemorieDeFleursUIModel.Instance.CancelOrderFromCustomer(OrderNo);
                Clear.Execute(this);
            }
        }

        /// <inheritdoc/>
        public void ChangeMyArrivalDate()
        {
            if (string.IsNullOrWhiteSpace(OrderNo))
            {
                throw new ApplicationException("発注されていません。");
            }
            else
            {
                MemorieDeFleursUIModel.Instance.ChangeArrivalDateOfOrderFromCustomer(OrderNo, ArrivalDate);
            }
        }
        #endregion // IOrderable
    }
}
