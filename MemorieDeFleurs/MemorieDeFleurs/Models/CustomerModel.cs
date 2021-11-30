using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// 得意先および得意先受注情報の操作モデル
    /// </summary>
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

            IDictionary<string, Tuple<string, string>> _shipping = new SortedDictionary<string, Tuple<string, string>>();

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
            /// お届け先を登録する
            /// </summary>
            /// <param name="name">お届け先名称</param>
            /// <param name="address1">お届け先住所1</param>
            /// <param name="address2">(省略可)お届け先住所2</param>
            /// <returns></returns>
            public CustomerBuilder SendTo(string name, string address1, string address2 = "")
            {
                if (_shipping.ContainsKey(name))
                {
                    throw new ArgumentException($"すでに登録済み： {name}");
                }
                _shipping.Add(name, Tuple.Create(address1, address2));
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

            /// <summary>
            /// 現在の内容で仕入先オブジェクトを生成、データベースに登録する
            /// </summary>
            /// <returns>データベースに登録された仕入先オブジェクト</returns>
            public Customer Create()
            {
                using (var context = new MemorieDeFleursDbContext(_model.Parent.DbConnection))
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var customer = Create(context);
                        transaction.Commit();
                        return customer;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            /// <summary>
            /// 現在の内容で仕入先オブジェクトを生成、データベースに登録する
            /// 
            /// トランザクション内での呼出用
            /// </summary>
            /// <param name="context">トランザクション中のDBコンテキスト</param>
            /// <returns>データベースに登録された仕入先オブジェクト</returns>
            public Customer Create(MemorieDeFleursDbContext context)
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

                if (_shipping.Count > 0)
                {
                    var builder = _model.GetShippingAddressBuilder();
                    foreach (var addr in _shipping)
                    {
                        builder.From(c)
                            .To(addr.Key)
                            .AddressIs(addr.Value.Item1, addr.Value.Item2)
                            .Create(context);
                    }
                }
                context.SaveChanges();

                return c;
            }
        }

        /// <summary>
        /// 得意先オブジェクト生成器を取得する
        /// </summary>
        /// <returns>得意先オブジェクト生成器</returns>
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

            /// <summary>
            /// お届け先オブジェクト生成器を生成して返す
            /// </summary>
            /// <param name="parent">内部で参照する得意先・受注情報操作モデル</param>
            /// <returns></returns>
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
            public ShippingAddressBuilder AddressIs(string address1, string address2 = "")
            {
                _address1 = address1;
                _address2 = address2;
                return this;
            }

            /// <summary>
            /// 現在の内容でお届け先オブジェクトを生成しデータベースに登録する
            /// </summary>
            /// <returns>データベースに登録されたお届け先オブジェクト</returns>
            public ShippingAddress Create()
            {
                using (var context = new MemorieDeFleursDbContext(_model.Parent.DbConnection))
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var address = Create(context);
                        transaction.Commit();
                        return address;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            /// <summary>
            /// 現在の内容でお届け先オブジェクトを生成しデータベースに登録する
            /// 
            /// トランザクション内での呼出用
            /// </summary>
            /// <param name="context">トランザクション中のDBコンテキスト</param>
            /// <returns>データベースに登録されたお届け先オブジェクト</returns>
            public ShippingAddress Create(MemorieDeFleursDbContext context)
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

        /// <summary>
        /// お届け先オブジェクト生成器を取得する
        /// </summary>
        /// <returns>お届け先オブジェクト生成器</returns>
        public ShippingAddressBuilder GetShippingAddressBuilder()
        {
            return ShippingAddressBuilder.GetInstance(this);
        }
        #endregion // ShippingAddressBuilder

        #region 得意先の登録改廃
        /// <summary>
        /// 指定の得意先IDを持つ得意先エンティティオブジェクトを取得する
        /// </summary>
        /// <param name="id">得意先ID</param>
        /// <returns></returns>
        public Customer FindCustomer(int id)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return FindCustomer(context, id);
            }
        }
        private Customer FindCustomer(MemorieDeFleursDbContext context, int id)
        {
            return context.Customers
                .Include(c => c.ShippingAddresses)
                .SingleOrDefault(c => c.ID == id);
        }

        /// <summary>
        /// 登録されている全得意先の一覧を取得する
        /// </summary>
        /// <returns>得意先の一覧</returns>
        public IEnumerable<Customer> FindAllCustomers()
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.Customers
                    .Include(c => c.ShippingAddresses)
                    .ToList().AsEnumerable();
            }
        }

        /// <summary>
        /// 指定IDの得意先情報をデータベースから削除する
        /// </summary>
        /// <param name="customerID">得意先ID</param>
        public void RemoveCustomer(int customerID)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using(var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(customerID.ToString());
                    RemoveCustomer(context, customerID);
                    LogUtil.Info($"Customer {customerID} removed.");
                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn(ex);
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod(customerID.ToString());
                }
            }
        }
        private void RemoveCustomer(MemorieDeFleursDbContext context, int customerID)
        {
            var customer = context.Customers.Find(customerID);
            if(customer == null)
            {
                throw new ApplicationException($"IDに該当する得意先なし：得意先ID={customerID}");
            }
            else
            {
                if(context.OrderFromCustomers.Count(o => o.CustomerID == customerID) > 0)
                {
                    throw new ApplicationException($"受注実績のある得意先は削除できない：得意先ID={customerID}");
                }

                var shipping = context.ShippingAddresses.Where(a => a.CustomerID == customerID);
                if (shipping.Count() > 0) { context.ShippingAddresses.RemoveRange(shipping); }
                context.Customers.Remove(customer);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// 得意先をデータベースに保存する
        /// </summary>
        /// <param name="customer">保存する得意先情報</param>
        /// <returns>データベースから再取得した</returns>
        public Customer Save(Customer customer)
        {
            using(var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(customer.ID.ToString());
                    var saved = Save(context, customer);
                    transaction.Commit();
                    LogUtil.Info($"Customer {saved.ID} saved.");
                    return saved;
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn(ex);
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod(customer.ID.ToString());
                }
            }
        }
        private Customer Save(MemorieDeFleursDbContext context, Customer customer)
        {
            var found = FindCustomer(context, customer.ID);
            if(found == null)
            {
                if(customer.ID == 0)
                {
                    customer.ID = Parent.Sequences.SEQ_CUSTOMERS.Next(context);
                }
                found = context.Customers.Add(customer).Entity;
            }
            else
            {
                if(found.CheckAndModify(customer))
                {
                    context.Customers.Update(found);
                }


                foreach(var addr in found.ShippingAddresses)
                {
                    var foundShipping = customer.ShippingAddresses.SingleOrDefault(a => a.ID == addr.ID);
                    if (foundShipping == null)
                    {
                        // found にあって customer にないお届け先を削除
                        context.ShippingAddresses.Remove(addr);
                    }
                    else if (addr.CheckAndModify(foundShipping))
                    {
                        // 両方に存在するお届け先は、名称等変更があったらそれを反映する
                        context.ShippingAddresses.Update(addr);
                    }
                }

                foreach (var addr in customer.ShippingAddresses)
                {
                    if(found.ShippingAddresses.SingleOrDefault(a => a.ID == addr.ID) == null)
                    {
                        // customer にあって found にないお届け先を追加
                        context.ShippingAddresses.Add(addr);
                    }
                }
            }
            context.SaveChanges();

            return FindCustomer(found.ID);
        }
        #endregion // 得意先の登録改廃

        #region お届け先の登録改廃
        /// <summary>
        /// お届け先をデータベースに保存する
        /// </summary>
        /// <param name="address">お届け先情報</param>
        /// <returns>データベースに登録した、関連する参照先が適切に設定されているお届け先情報</returns>
        public ShippingAddress Save(ShippingAddress address)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    LogUtil.DEBUGLOG_BeginMethod(address.ID.ToString());
                    var saved = Save(context, address);
                    transaction.Commit();
                    return saved;
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn(ex);
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod(address.ID.ToString());
                }
            }
        }
        private ShippingAddress Save(MemorieDeFleursDbContext context, ShippingAddress address)
        {
            var found = FindShippingAddress(context, address.ID);
            if(found == null)
            {
                if(address.ID == 0)
                {
                    address.ID = Parent.Sequences.SEQ_SHIPPING.Next(context);
                }

                if (address.Customer != null)
                {
                    address.CustomerID = address.Customer.ID;
                    address.Customer = null;
                }

                found = context.ShippingAddresses.Add(address).Entity;
            }
            else if(found.IsModified(address))
            {
                // 変更があったら(更新ではなく)新規のお届け先として登録する
                address.ID = Parent.Sequences.SEQ_SHIPPING.Next(context);
                if(address.Customer != null)
                {
                    address.CustomerID = address.Customer.ID;
                    address.Customer = null;
                }
                found = context.ShippingAddresses.Add(address).Entity;
            }

            context.SaveChanges();

            return FindShippingAddress(context, found.ID);
        }

        /// <summary>
        /// 得意先が過去に商品を贈ったお届け先を取得する
        /// </summary>
        /// <param name="customer">得意先ID</param>
        /// <returns>お届け先一覧</returns>
        public IEnumerable<ShippingAddress> FindAllShippingAddressesOfCustomer(int customer)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.ShippingAddresses
                    .Where(s => s.CustomerID == customer)
                    .Include(s => s.Customer)
                    .OrderBy(s => s.LatestOrderDate)
                    .ToList()
                    .AsEnumerable();
            }
        }

        private ShippingAddress FindShippingAddress(MemorieDeFleursDbContext context, int id)
        {
            return context.ShippingAddresses
                .Where(s => s.ID == id)
                .Include(s => s.Customer)
                .SingleOrDefault();
        }
        #endregion // お届け先の登録改廃

        #region 受注履歴の登録改廃
        /// <summary>
        /// 登録されている全得意先受注履歴を取得する
        /// </summary>
        /// <returns>得意先受注履歴の一覧</returns>
        public IEnumerable<OrderFromCustomer> FindAllOrders()
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrderFromCustomers
                    .Include(o => o.Bouquet)
                    .Include(o => o.Customer)
                    .Include(o => o.ShippingAddress)
                    .OrderBy(o => o.OrderDate)
                    .ThenBy(o => o.ShippingDate)
                    .ThenBy(o => o.CustomerID)
                    .ThenBy(o => o.ShippingAddressID)
                    .ToList()
                    .AsEnumerable();
            }
        }

        /// <summary>
        /// 期間指定で得意先受注履歴を取得する
        /// </summary>
        /// <param name="from">期間の開始日</param>
        /// <param name="to">期間の終了日</param>
        /// <returns>得意先受注履歴の一覧</returns>
        public IEnumerable<OrderFromCustomer> FindAllOrders(DateTime from, DateTime to)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrderFromCustomers
                    .Include(o => o.Bouquet)
                    .Include(o => o.Customer)
                    .Include(o => o.ShippingAddress)
                    .Where(o => from <= o.OrderDate && o.OrderDate <= to)
                    .OrderBy(o => o.OrderDate)
                    .ThenBy(o => o.ShippingDate)
                    .ThenBy(o => o.CustomerID)
                    .ThenBy(o => o.ShippingAddressID)
                    .ToList()
                    .AsEnumerable();
            }
        }

        /// <summary>
        /// 期間と得意先を指定して受注履歴を取得する
        /// </summary>
        /// <param name="from">期間の開始日</param>
        /// <param name="to">期間の終了日</param>
        /// <param name="customerID">得意先ID</param>
        /// <returns>得意先受注履歴の一覧</returns>
        public IEnumerable<OrderFromCustomer> FindAllOrders(DateTime from, DateTime to, int customerID)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrderFromCustomers
                    .Include(o => o.Bouquet)
                    .Include(o => o.Customer)
                    .Include(o => o.ShippingAddress)
                    .Where(o => from <= o.OrderDate && o.OrderDate <= to)
                    .Where(o => o.CustomerID == customerID)
                    .OrderBy(o => o.OrderDate)
                    .ThenBy(o => o.ShippingDate)
                    .ThenBy(o => o.ShippingAddressID)
                    .ToList()
                    .AsEnumerable();
            }
        }

        /// <summary>
        /// 得意先を指定して受注履歴を取得する
        /// </summary>
        /// <param name="orderID">得意先</param>
        /// <returns>得意先受注履歴の一覧</returns>
        public OrderFromCustomer FindOrder(string orderID)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return FindOrder(context, orderID);
            }
        }

        private static OrderFromCustomer FindOrder(MemorieDeFleursDbContext context, string orderID)
        {
            return context.OrderFromCustomers
                .Include(o => o.Bouquet)
                .Include(o => o.Customer)
                .Include(o => o.ShippingAddress)
                .SingleOrDefault(o => o.ID == orderID);
        }

        /// <summary>
        /// 指定日に発送した／発送予定の受注番号を取得する
        /// </summary>
        /// <param name="date">発送(予定)日</param>
        /// <returns>受注番号一覧</returns>
        public IEnumerable<string> FindAllOrdersShippingAt(DateTime date)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return context.OrderFromCustomers
                    .Where(order => order.ShippingDate == date)
                    .OrderBy(order => order.ID)
                    .Select(order => order.ID)
                    .ToList();
            }
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
        public string Order(DateTime orderDate, Bouquet bouquet, ShippingAddress sendTo, DateTime arrivalDate, string message = "")
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var orderNo = Order(context, orderDate, bouquet, sendTo, arrivalDate, message);
                    transaction.Commit();

                    var partsList = context.PartsList.Where(i => i.BouquetCode == bouquet.Code).Select(item => $"{item.PartsCode} x {item.Quantity}");
                    LogUtil.Info($"{arrivalDate:yyyyMMdd}, {bouquet.Code} ordered: {orderNo}, Using {string.Join(", ", partsList)}");

                    return orderNo;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 得意先からの商品受注を処理する
        /// 
        /// トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderDate">受注日</param>
        /// <param name="bouquet">受注商品</param>
        /// <param name="sendTo">お届け先</param>
        /// <param name="arrivalDate">お届け日</param>
        /// <param name="message">(省略可) お届けメッセージ</param>
        /// <returns>受注番号</returns>
        public string Order(MemorieDeFleursDbContext context, DateTime orderDate, Bouquet bouquet, ShippingAddress sendTo, DateTime arrivalDate, string message = "")
        {
            // 在庫アクションの登録改訂に関する検証用、暫定実装
            LogUtil.DEBUGLOG_BeginMethod($"orderDate={orderDate.ToString("yyyyMMdd")}, bouquet={bouquet.Code}" +
                $", shipping={sendTo.CustomerID}-{sendTo.ID}, arrivalDate={arrivalDate.ToString("yyyyMMdd")}");

            try
            {
                var countOfOrdersToday = context.OrderFromCustomers.Count(o => o.OrderDate == orderDate);
                var customer = context.Customers.Find(sendTo.CustomerID);

                if (customer == null)
                {
                    throw new InvalidOperationException($"得意先不明 (ID={sendTo.CustomerID})");
                }
                if (countOfOrdersToday > 999999)
                {
                    throw new InvalidOperationException($"当日受注数が想定外に多い：受注日={orderDate.ToString("yyyyMMdd")}, 受注数={countOfOrdersToday}");
                }

                var minmalArrivalDate = orderDate.AddDays(bouquet.LeadTime);
                if (arrivalDate < minmalArrivalDate)
                {
                    throw new ApplicationException($"注文不可：お届け日 {arrivalDate:yyyyMMdd} が最短お届け可能日 {minmalArrivalDate:yyyyMMdd} より近い");
                }

                var usedDate = arrivalDate.AddDays(-1);
                foreach (var item in bouquet.PartsList)
                {
                    var part = context.BouquetParts.Find(item.PartsCode);
                    Parent.BouquetModel.UseFromInventory(context, part, usedDate, item.Quantity);

                    if (Parent.BouquetModel.ShortageInventories.Count() > 0)
                    {
                        throw new InventoryShortageException(Parent.BouquetModel.ShortageInventories.First(), item.Quantity);
                    }
                }

                var order = new OrderFromCustomer()
                {
                    ID = $"{orderDate.ToString("yyyyMMdd")}-{countOfOrdersToday + 1:000000}",
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
                return order.ID;
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
        #endregion // 注文

        #region 注文取消
        /// <summary>
        /// 受注を取り消す
        /// </summary>
        /// <param name="orderNo">受注番号</param>
        public void CancelOrder(string orderNo)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    CancelOrder(context, orderNo);
                    transaction.Commit();
                    LogUtil.Info($"Order canceled: {orderNo}");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 受注を取り消す
        /// 
        /// トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderNo">受注番号</param>
        public void CancelOrder(MemorieDeFleursDbContext context, string orderNo)
        {
            LogUtil.DEBUGLOG_BeginMethod($"order={orderNo}");

            try
            {
                var order = FindOrder(context, orderNo);
                if (order == null)
                {
                    throw new NotSupportedException($"該当する受注履歴なし：{orderNo}");
                }

                if (order.Status == OrderFromCustomerStatus.SHIPPED)
                {
                    // 出荷済み注文はキャンセル不可
                    throw new ApplicationException($"出荷済みキャンセル不可：{orderNo}");
                }

                var partsList = context.PartsList.Where(i => i.BouquetCode == order.BouquetCode);

                LogUtil.Debug($"{order.BouquetCode} ({partsList.Count()} part(s)):" +
                    $" {string.Join(", ", partsList.Select(p => $"({p.PartsCode} x {p.Quantity})"))}");

                foreach (var item in partsList)
                {
                    var part = context.BouquetParts.Find(item.PartsCode);
                    Parent.BouquetModel.ReturnToInventory(context, part, order.ShippingDate, item.Quantity);
                }

                context.OrderFromCustomers.Remove(order);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod($"order={orderNo}");
            }
        }
        #endregion // 注文取消

        #region お届け日変更
        /// <summary>
        /// 得意先から受注した商品のお届け日を変更する
        /// </summary>
        /// <param name="orderChangeDate">お届け日変更の発生日=この処理を呼び出した日</param>
        /// <param name="orderNo">受注番号</param>
        /// <param name="newArrivalDate">変更後のお届け日</param>
        public void ChangeArrivalDate(DateTime orderChangeDate, string orderNo, DateTime newArrivalDate)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var oldShippingDate = context.OrderFromCustomers.Find(orderNo).ShippingDate;

                    LogUtil.DEBUGLOG_BeginMethod($"{orderNo}, {newArrivalDate:yyyyMMdd}");
                    ChangeArrivalDate(context, orderChangeDate, orderNo, newArrivalDate);
                    transaction.Commit();
                    LogUtil.DEBUGLOG_EndMethod($"{orderNo}", "succeeded.");
                    LogUtil.Info($"Shipping Date changed; order={orderNo}, from {oldShippingDate:yyyyMMdd} to {newArrivalDate.AddDays(-1):yyyyMMdd}");
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    LogUtil.Warn($"ChangeArrivalDate({orderNo},{newArrivalDate.ToString("yyyyMMdd")}) failed, cause={e.GetType().Name}: {e.Message}");
                    LogUtil.DEBUGLOG_EndMethod($"{orderNo}", "failed.");
                    throw;
                }
            }
        }

        /// <summary>
        /// 得意先から受注した商品のお届け日を変更する
        /// 
        /// トランザクション内での呼出用
        /// </summary>
        /// <param name="context">トランザクション中のDBコンテキスト</param>
        /// <param name="orderChangeDate">お届け日変更の発生日=この処理を呼び出した日</param>
        /// <param name="orderNo">受注番号</param>
        /// <param name="newArrivalDate">変更後のお届け日</param>
        /// <exception cref="NotImplementedException">処理未実装：機能考慮されていないパターンがあったときのガードとして、</exception>
        /// <exception cref="ApplicationException">お届け日を変更できない：変更後のお届け日が当日以前の場合など。</exception>
        public void ChangeArrivalDate(MemorieDeFleursDbContext context, DateTime orderChangeDate, string orderNo, DateTime newArrivalDate)
        {

            if (string.IsNullOrWhiteSpace(orderNo))
            {
                throw new NotImplementedException("エラー処理未実装：orderNo が null");
            }

            var order = FindOrder(context, orderNo);
            if (order == null)
            {
                throw new NotImplementedException($"エラー処理未実装：{orderNo} に該当するオーダーがない");
            }

            if (order.Status == OrderFromCustomerStatus.SHIPPED)
            {
                // 出荷済み注文はキャンセル不可
                throw new ApplicationException($"出荷済み出荷日変更不可：{orderNo}");
            }

            var bouquet = Parent.BouquetModel.FindBouquet(order.BouquetCode);
            var minimalArrivalDate = orderChangeDate.AddDays(bouquet.LeadTime);
            if (newArrivalDate <= minimalArrivalDate)
            {
                // リードタイムより近い日付への移動はできない
                // 等号を含む：当日受け付けた注文変更に伴う在庫不足があると、仕入先への単品発注が間に合わないため。
                throw new ApplicationException($"注文変更不可：お届け日 {newArrivalDate:yyyyMMdd} が最短お届け可能日 {minimalArrivalDate:yyyyMMdd} より近い");
            }

            var newShippingDate = newArrivalDate.AddDays(-1);
            var partsList = bouquet.PartsList.Select(i => $"{i.PartsCode} x{i.Quantity}");
            LogUtil.Debug($"Order={orderNo}, Shipping={order.ShippingDate.ToString("yyyyMMdd")}->{newShippingDate.ToString("yyyyMMdd")}" +
                $", Bouquet({bouquet.PartsList.Count()} part(s))=[{string.Join(", ", partsList).Trim()}]");

            foreach (var item in bouquet.PartsList)
            {
                Parent.BouquetModel.ReturnToInventory(context, item.Part, order.ShippingDate, item.Quantity);
                Parent.BouquetModel.UseFromInventory(context, item.Part, newShippingDate, item.Quantity);
            }
            context.SaveChanges();
        }
        #endregion // お届け日変更

        #region 出荷
        /// <summary>
        /// 日付指定で当日出荷予定の全受注履歴を出荷済に変更する
        /// </summary>
        /// <param name="date"></param>
        public void ShipAllBouquets(DateTime date)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                LogUtil.DEBUGLOG_BeginMethod(date.ToString("yyyyMMdd"));
                try
                {
                    ShipAllOrdersFromCustomerInTheDay(context, date);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod();
                }

            }
        }

        private void ShipAllOrdersFromCustomerInTheDay(MemorieDeFleursDbContext context, DateTime date)
        {
            foreach (var order in context.OrderFromCustomers.Where(order => order.ShippingDate == date).ToList())
            {
                if (order.Status == OrderFromCustomerStatus.SHIPPED)
                {
                    // 二重出荷はできない
                    throw new ApplicationException($"すでに出荷済み：{order.ID}");
                }

                order.Status = OrderFromCustomerStatus.SHIPPED;
                context.OrderFromCustomers.Update(order);
            }
            context.SaveChanges();

            Parent.BouquetModel.ChangeAllScheduledPartsOfTheDayUsed(context, date);
        }

        /// <summary>
        /// 得意先からの受注番号指定で出荷処理を行う。
        /// </summary>
        /// <param name="date">出荷日</param>
        /// <param name="orderNumbers">出荷した受注番号</param>
        public void ShipOrders(DateTime date, params string[] orderNumbers)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            using (var transaction = context.Database.BeginTransaction())
            {
                LogUtil.DEBUGLOG_BeginMethod($"{date:yyyyMMdd}, [ {string.Join(", ", orderNumbers)} ]");
                try
                {
                    ShipOrders(context, date, orderNumbers);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogUtil.Warn($"Order shipping failed: {date:yyyyMMdd}, {ex.GetType().Name} : {ex.Message}");
                    throw;
                }
                finally
                {
                    LogUtil.DEBUGLOG_EndMethod();
                }
            }
        }

        private void ShipOrders(MemorieDeFleursDbContext context, DateTime date, params string[] orderNumbers)
        {
            var orderList = context.OrderFromCustomers.Where(o => orderNumbers.Contains(o.ID)).ToList();
            foreach (var order in orderList)
            {
                order.ShippingDate = date;
                order.Status = OrderFromCustomerStatus.SHIPPED;
                context.OrderFromCustomers.Update(order);
            }
            context.SaveChanges();
            LogUtil.Debug("OrderFromCustomers changed.");


            var partsList = orderList
                .SelectMany(o => context.PartsList.Where(i => i.BouquetCode == o.BouquetCode))
                .GroupBy(i => i.PartsCode)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));
            foreach (var item in partsList)
            {
                Parent.BouquetModel.UpdatePartsUsedQuantity(context, date, item);
            }
            context.SaveChanges();
        }
        #endregion // 出荷

        #region 加工指示書
        /// <summary>
        /// 指定日付の出荷商品数の一覧を取得する
        /// </summary>
        /// <param name="date">出荷(予定)日=お届け(予定)日の1日前</param>
        /// <returns>出荷予定商品数一覧：出荷予定のない商品も、商品数=0で登録されている。</returns>
        public IDictionary<string, int> GetShippingBouquetCountAt(DateTime date)
        {
            using (var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                var found = context.OrderFromCustomers
                    .Where(o => o.ShippingDate == date)
                    .AsEnumerable()
                    .GroupBy(o => o.BouquetCode)
                    .ToDictionary(g => g.Key, g => g.Count());

                var all = context.Bouquets.Select(b => b.Code);
                
                foreach(var code in all)
                {
                    if(!found.ContainsKey(code))
                    {
                        found.Add(code, 0);
                    }
                }
                return found;
            }
        }
        #endregion // 加工指示書
    }
}
