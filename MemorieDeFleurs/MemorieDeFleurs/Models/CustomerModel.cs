using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using System;
using System.Linq;
using System.Text;

namespace MemorieDeFleurs.Models
{
    public class CustomerModel
    {
        private MemorieDeFleursModel Parent { get; set; }

        private SequenceUtil.SequenceValueManager SEQ_CUSTOMERS
        {
            get { return Parent.Sequences.SEQ_CUSTOMERS; }
        }

        private SequenceUtil.SequenceValueManager SEQ_SHIPPING
        {
            get { return Parent.Sequences.SEQ_SHIPPING; }
        }

        /// <summary>
        /// (パッケージ内限定)コンストラクタ
        /// 
        /// モデルのプロパティとして参照できるので、外部でこのオブジェクトを作成することは想定しない。
        /// </summary>
        /// <param name="parent"></param>
        internal CustomerModel(MemorieDeFleursModel parent)
        {
            Parent = parent;
        }

        #region CustomerBuilder
        /// <summary>
        /// 得意先オブジェクト生成器
        /// 
        /// 各プロパティはフルーエントインタフェースで入力し、最後に Create() で生成とデータベース登録を行う。
        /// </summary>
        public class CustomerBuilder
        {
            private CustomerModel _model;

            string _emailAddress;
            string _name;
            string _password;
            string _cardNo;

            internal static CustomerBuilder GetInstance(CustomerModel parent)
            {
                return new CustomerBuilder(parent);   
            }

            private CustomerBuilder(CustomerModel model)
            {
                _model = model;
            }

            /// <summary>
            /// 得意先名称を登録/変更する
            /// </summary>
            /// <param name="name">得意先名称</param>
            /// <returns>得意先名称変更後の得意先オブジェクト生成器(自分自身)</returns>
            public CustomerBuilder NameIs(string name)
            {
                _name = name;
                return this;
            }

            /// <summary>
            /// e-メールアドレスを登録/変更する
            /// </summary>
            /// <param name="address">e-メールアドレス</param>
            /// <returns>e-メールアドレス変更後の得意先オブジェクト生成器(自分自身)</returns>
            public CustomerBuilder EmailAddressIs(string address)
            {
                _emailAddress = address;
                return this;
            }

            /// <summary>
            /// パスワードを登録/変更する
            /// </summary>
            /// <param name="passwd">パスワード</param>
            /// <returns>パスワード変更後の得意先オブジェクト生成器(自分自身)</returns>
            public CustomerBuilder PasswordIs(string passwd)
            {
                _password = passwd;
                return this;
            }

            /// <summary>
            /// キャッシュカード番号を登録/変更する
            /// </summary>
            /// <param name="no">キャッシュカード番号</param>
            /// <returns>キャッシュカード番号変更後の得意先オブジェクト生成器(自分自身)</returns>
            public CustomerBuilder CardNoIs(string no)
            {
                _cardNo = no;
                return this;
            }

            public Customer Create()
            {
                using (var context = new MemorieDeFleursDbContext(_model.Parent.DbConnection))
                {
                    return Create(context);
                }
            }

            private Customer Create(MemorieDeFleursDbContext context)
            {
                var c = new Customer()
                {
                    ID = _model.SEQ_CUSTOMERS.Next(context),
                    Name = _name,
                    EmailAddress = _emailAddress,
                    Password = _password,
                    CardNo = _cardNo,
                    Status = 0
                };

                context.Customers.Add(c);
                context.SaveChanges();

                return c;
            }
        }

        public CustomerBuilder GetCustomerBuilder()
        {
            return CustomerBuilder.GetInstance(this);
        }
        #endregion // CustomerBuilder

        #region ShippingAddressBuilder
        /// <summary>
        /// お届け先オブジェクト生成器
        /// </summary>
        public class ShippingAddressBuilder
        {
            private CustomerModel _model;

            private string _address1;
            private string _address2;
            private string _name;

            private Customer _sendFrom;
            
            public static ShippingAddressBuilder GetInstance(CustomerModel parent)
            {
                return new ShippingAddressBuilder(parent);
            }

            private ShippingAddressBuilder(CustomerModel model)
            {
                _model = model;
            }

            /// <summary>
            /// 贈り主を登録/変更する
            /// </summary>
            /// <param name="customer">贈り主である得意先</param>
            /// <returns>贈り主を変更したお届け先オブジェクト生成器(自分自身)</returns>
            public ShippingAddressBuilder From(Customer customer)
            {
                _sendFrom = customer;
                return this;
            }

            /// <summary>
            /// お届け先名称を登録/変更する
            /// </summary>
            /// <param name="name">お届け先名称</param>
            /// <returns>お届け先名称を変更したお届け先オブジェクト生成器(自分自身)</returns>
            public ShippingAddressBuilder To(string name)
            {
                _name = name;
                return this;
            }

            /// <summary>
            /// お届け先住所を登録/変更する
            /// </summary>
            /// <param name="address1">お届け先住所1 (入力必須)</param>
            /// <param name="address2">お届け先住所2</param>
            /// <returns>お届け先住所を変更したお届け先オブジェクト生成器(自分自身)</returns>
            public ShippingAddressBuilder AddressIs(string address1, string address2="")
            {
                _address1 = address1;
                _address2 = address2;
                return this;
            }

            /// <summary>
            /// お届け先オブジェクトを生成する
            /// </summary>
            /// <returns></returns>
            public ShippingAddress Create()
            {
                using (var context = new MemorieDeFleursDbContext(_model.Parent.DbConnection))
                {
                    return Create(context);
                }
            }

            private ShippingAddress Create(MemorieDeFleursDbContext context)
            {
                if (_sendFrom == null)
                {
                    throw new ApplicationException($"贈り主は入力必須、登録済みの得意先であること：お届け先名={_name}");
                }

                var shipping = new ShippingAddress()
                {
                    ID = _model.SEQ_SHIPPING.Next(context),
                    CustomerID = _sendFrom.ID,
                    Name = _name,
                    Address1 = _address1,
                    Address2 = _address2,
                    LatestOrderDate = DateTime.Now
                };

                context.ShippingAddresses.Add(shipping);
                context.SaveChanges();

                return shipping;
            }
        }

        public ShippingAddressBuilder GetShippingAddressBuilder()
        {
            return ShippingAddressBuilder.GetInstance(this);
        }
        #endregion // ShippingAddressBuilder

        #region 仕入先の登録改廃
        public Customer Find(int id)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.Customers.Find(id);
            }
        }
        #endregion // 仕入先の登録改廃

        #region 受注履歴の登録改廃
        public OrderFromCustomer FindOrder(string orderID)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return FindOrder(context, orderID);
            }
        }

        private static OrderFromCustomer FindOrder(MemorieDeFleursDbContext context, string orderID)
        {
            var order = context.OrderFromCustomers.Find(orderID);
            if (order.Bouquet == null)
            {
                order.Bouquet = context.Bouquets.Find(order.BouquetCode);
            }
            if (order.Customer == null)
            {
                order.Customer = context.Customers.Find(order.CustomerID);
            }
            if (order.ShippingAddress == null)
            {
                order.ShippingAddress = context.ShippingAddresses.Find(order.ShippingAddressID);
            }
            return order;
        }
        #endregion // 受注履歴の登録改廃

        #region 注文
        /// <summary>
        /// 花束を注文する
        /// </summary>
        /// <param name="orderDate">注文日</param>
        /// <param name="bouquet">注文対象の花束</param>
        /// <param name="sendTo">花束の送り先：贈り主はここから参照して取得する</param>
        /// <param name="arrivalDate">お届け日：花束の作成は前日なので、在庫は arrivalDate - 1 日の分が消費される</param>
        /// <param name="message">（省略可能）お届けメッセージ</param>
        public string Order(DateTime orderDate, Bouquet bouquet, ShippingAddress sendTo, DateTime arrivalDate, string message = "" )
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return Order(context, orderDate, bouquet, sendTo, arrivalDate, message);
            }
        }

        public string Order(MemorieDeFleursDbContext context, DateTime orderDate, Bouquet bouquet, ShippingAddress sendTo, DateTime arrivalDate, string message = "")
        {
            // 在庫アクションの登録改訂に関する検証用、暫定実装
            LogUtil.DEBUGLOG_BeginMethod($"orderDate={orderDate.ToString("yyyyMMdd")}, bouquet={bouquet.Code}" +
                $", shipping={sendTo.CustomerID}-{sendTo.ID}, arrivalDate={arrivalDate.ToString("yyyyMMdd")}");

            var countOfOrdersToday = context.OrderFromCustomers.Count(o => o.OrderDate == orderDate);
            var customer = context.Customers.Find(sendTo.CustomerID);

            if(customer == null)
            {
                throw new InvalidOperationException($"得意先不明 (ID={sendTo.CustomerID})");
            }
            if(countOfOrdersToday > 999999)
            {
                throw new InvalidOperationException($"当日受注数が想定外に多い：受注日={orderDate.ToString("yyyyMMdd")}, 受注数={countOfOrdersToday}");
            }

            var usedDate = arrivalDate.AddDays(-1);
            foreach (var item in bouquet.PartsList)
            {
                var part = context.BouquetParts.Find(item.PartsCode);
                var remain = Parent.BouquetModel.UseBouquetPart(context, part, usedDate, item.Quantity);
                if (remain < 0)
                {
                    throw new NotImplementedException(new StringBuilder()
                        .Append("注文不可：単品在庫なし、")
                        .AppendFormat(" お届け日={0:yyyyMMdd}", arrivalDate)
                        .Append(", 商品=").Append(bouquet.Code)
                        .Append(", 単品=").Append(item.PartsCode)
                        .Append(", 要求数=").Append(item.Quantity)
                        .Append(", 不足数=").Append(-remain) // 符号を＋に変えて表示
                        .ToString());
                }
            }

            var order = new OrderFromCustomer()
            {
                ID = $"{orderDate.ToString("yyyyMMdd")}-{countOfOrdersToday+1:000000}",
                BouquetCode = bouquet.Code,
                CustomerID = context.Customers.Find(sendTo.CustomerID).ID,
                ShippingAddressID = sendTo.ID,
                OrderDate = orderDate,
                ShippingDate = arrivalDate.AddDays(-1),
                HasMessage = string.IsNullOrWhiteSpace(message),
                Message = message,
                Status = 0
            };

            context.OrderFromCustomers.Add(order);
            context.SaveChanges();

            LogUtil.Info($"{bouquet.Code} ordered: {order.ID}");
            
            LogUtil.DEBUGLOG_EndMethod();
            return order.ID;
        }
        #endregion // 注文

        #region 注文取消
        public void CancelOrder(string orderNo)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    CancelOrder(context, orderNo);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void CancelOrder(MemorieDeFleursDbContext context, string orderNo)
        {
            LogUtil.DEBUGLOG_BeginMethod($"order={orderNo}");

            var order = FindOrder(context, orderNo);
            if (order == null)
            {
                throw new NotSupportedException($"該当する受注履歴なし：{orderNo}");
            }

            var partsList = context.PartsList.Where(i => i.BouquetCode == order.BouquetCode);

            LogUtil.Debug($"{LogUtil.Indent}{order.BouquetCode} ({partsList.Count()} part(s)):" +
                $" {string.Join(", ", partsList.Select(p => $"({p.PartsCode} x {p.Quantity})"))}");

            foreach(var item in partsList)
            {
                var part = context.BouquetParts.Find(item.PartsCode);
                // 受注したのと同じ数量を引く
                Parent.BouquetModel.UseBouquetPart(context, part, order.ShippingDate, - item.Quantity);
            }

            LogUtil.Info($"Order canceled: {orderNo}");
            LogUtil.DEBUGLOG_EndMethod($"order={orderNo}");
        }
        #endregion // 注文取消
    }
}
