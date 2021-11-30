using MemorieDeFleurs.Database.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;

using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MemorieDeFleurs.UI.WPF.Model
{
    /// <summary>
    /// ユーザインタフェース内での花束問題モデル；
    /// <see cref="MemorieDeFleursModel"/> および <see cref="MemorieDeFleursModel"/> がプロパティとして持つ得意先/仕入先/商品モデルのファサード
    /// </summary>
    public class MemorieDeFleursUIModel : NotificationObject
    {
        private static MemorieDeFleursUIModel _singleton = new MemorieDeFleursUIModel();

        private MemorieDeFleursUIModel() { }

        /// <summary>
        /// UI用モデルのインスタンス
        /// </summary>
        public static MemorieDeFleursUIModel Instance { get { return _singleton; } }

        /// <summary>
        /// 接続中のデータベース
        /// </summary>
        public DbConnection DbConnection
        {
            get { return _conn; }
            set { SetProperty(ref _conn, value); }
        }
        private DbConnection _conn;

        private MemorieDeFleursModel Model { get; set; }

        #region データベースの操作
        /// <summary>
        /// SQLite データベースファイルをロードし接続する
        /// </summary>
        /// <param name="path">ロードする SQLite データベースファイル</param>
        public void OpenSQLiteDatabaseFile(string path)
        {
            LogUtil.DEBUGLOG_BeginMethod(path);
            try
            {
                if(DbConnection != null) { DbConnection.Dispose(); }
                DbConnection = MemorieDeFleursDatabaseFacade.OpenDatabase(path);
                LogUtil.Info($"Database opened: {path}");
                Model = new MemorieDeFleursModel(DbConnection);
            }
            catch (Exception ex)
            {
                LogUtil.Warn(ex);
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(path);
            }
        }

        /// <summary>
        /// 接続中の SQLite データベースの内容をを指定ファイルに保存する
        /// </summary>
        /// <param name="path">保存するデータベースファイル名</param>
        public void SaveSQLiteConnectinToFile(string path)
        {
            LogUtil.DEBUGLOG_BeginMethod(path);
            try
            {
                if(DbConnection == null) { throw new NotConnectedToDatabaseException(); }
                if(DbConnection is SqliteConnection)
                {
                    var builder = new SqliteConnectionStringBuilder();
                    builder.DataSource = path;
                    builder.Mode = SqliteOpenMode.ReadWriteCreate;
                    builder.ForeignKeys = true;

                    var backupDb = new SqliteConnection(builder.ToString());
                    (DbConnection as SqliteConnection).BackupDatabase(backupDb);
                    LogUtil.Info($"Database saved to: {path}");
                }
                else
                {
                    throw new ApplicationException($"SQLite データベースに接続されていません：現在の接続オブジェクト：{DbConnection.GetType().Name}");
                }
            }
            catch(Exception ex)
            {
                LogUtil.Warn(ex);
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(path);
            }
        }

        #endregion // データベースの操作

        #region 単品の操作
        /// <summary>
        /// 接続中のデータベースに登録されている単品をすべて取得する
        /// </summary>
        /// <returns>登録されている単品のコレクション、単品が登録されていないときは空のコレクション</returns>
        public IEnumerable<BouquetPart> FindAllBouquetParts()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindAllBoueuqtParts();
        }

        /// <summary>
        /// 接続中のデータベースに登録されている単品を取得する
        /// </summary>
        /// <param name="partsCode">花コード</param>
        /// <returns>花コードに該当する単品</returns>
        public BouquetPart FindBouquetParts(string partsCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindBouquetPart(partsCode);
        }

        /// <summary>
        /// 接続中のデータベースに登録されている単品を削除する
        /// </summary>
        /// <param name="partsCode">花コード</param>
        public void RemoveBouquetParts(string partsCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.BouquetModel.RemoveBouquetParts(partsCode);
        }

        /// <summary>
        /// 単品をデータベースに保存する
        /// </summary>
        /// <param name="part">単品エンティティオブジェクト</param>
        public BouquetPart Save(BouquetPart part)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.Save(part);
        }
        #endregion // 単品の操作

        #region 商品と商品構成の操作
        /// <summary>
        /// 接続中のデータベースに登録されている商品をすべて取得する
        /// </summary>
        /// <returns>登録されている商品のコレクション、商品が登録されていないときは空のコレクション</returns>
        public IEnumerable<Bouquet> FindAllBouquets()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindAllBouquets();
        }

        /// <summary>
        /// 接続中のデータベースに登録されている商品を取得する
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        public void RemoveBouquet(string bouquetCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.BouquetModel.RemoveBouquet(bouquetCode);
        }

        /// <summary>
        /// 接続中のデータベースに登録されている商品を削除する
        /// </summary>
        /// <param name="bouquetCode">花束コード</param>
        /// <returns></returns>
        public Bouquet FindBouquet(string bouquetCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindBouquet(bouquetCode);
        }

        /// <summary>
        /// 商品をデータベースに保存する
        /// </summary>
        /// <param name="bouquet">商品エンティティオブジェクト</param>
        public Bouquet Save(Bouquet bouquet)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.Save(bouquet);
        }
        #endregion // 商品と商品構成の操作

        #region 仕入先と単品仕入先の操作
        /// <summary>
        /// 接続中のデータベースに登録されている仕入先をすべて取得する
        /// </summary>
        /// <returns>登録されている仕入先のコレクション、仕入先が登録されていないときは空のコレクション</returns>
        public IEnumerable<Supplier> FindAllSuppliers()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindAllSuppliers();
        }

        /// <summary>
        /// 接続中のデータベースに登録されている仕入先を取得する
        /// </summary>
        /// <param name="supplierCode">仕入先ID</param>
        /// <returns></returns>
        public Supplier FindSupplier(int supplierCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindSupplier(supplierCode);
        }

        /// <summary>
        /// 接続中のデータベースから仕入先を削除する
        /// </summary>
        /// <param name="supplierCode">仕入先ID</param>
        public void RemoveSupplier(int supplierCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.SupplierModel.RemoveSupplier(supplierCode);
        }

        /// <summary>
        /// 得意先をデータベースに保存する
        /// </summary>
        /// <param name="supplier">得意先エンティティオブジェクト</param>
        public Supplier Save(Supplier supplier)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.Save(supplier);
        }
        #endregion // 仕入先と単品仕入先の操作

        #region 得意先の操作
        /// <summary>
        /// 得意先エンティティを取得する
        /// </summary>
        /// <param name="id">得意先ID</param>
        /// <returns>得意先エンティティ</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public Customer FindCustomer(int id)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindCustomer(id);
        }

        /// <summary>
        /// データベースに登録されているすべての得意先エンティティを取得する
        /// </summary>
        /// <returns>得意先エンティティの一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IEnumerable<Customer> FindAllCustomers()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllCustomers();
        }

        /// <summary>
        /// データベースに登録されている得意先エンティティを削除する
        /// </summary>
        /// <param name="id">得意先ID</param>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public void RemoveCustomer(int id)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.CustomerModel.RemoveCustomer(id);
        }

        /// <summary>
        /// 得意先エンティティをデータベースに保存する
        /// </summary>
        /// <param name="customer">保存する得意先エンティティ</param>
        /// <returns>保存後した得意先エンティティ</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public Customer Save(Customer customer)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.Save(customer);
        }
        #endregion // 得意先の操作

        #region 仕入先発注情報の操作
        /// <summary>
        /// 仕入先発注情報を取得する
        /// </summary>
        /// <param name="id">発注番号</param>
        /// <returns>仕入先発注エンティティ</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public OrdersToSupplier FindOrderToSupplier(string id)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindOrder(id);
        }

        /// <summary>
        /// データベースからすべての仕入先発注情報を取得する
        /// </summary>
        /// <returns>仕入先発注エンティティの一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IEnumerable<OrdersToSupplier> FindAllOrdersToSupplier()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindAllOrders();
        }

        /// <summary>
        /// 期間指定でデータベースから仕入先発注情報を取得する
        /// </summary>
        /// <param name="from">期間の開始日</param>
        /// <param name="to">期間の終了日</param>
        /// <returns>仕入先発注エンティティの一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IEnumerable<OrdersToSupplier> FindAllOrdersToSupplier(DateTime from, DateTime to)
        {
            if(DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindAllOrders(from, to);
        }

        /// <summary>
        /// 期間を指定してデータベースから特定得意先の仕入先発注情報を取得する
        /// </summary>
        /// <param name="from">期間の開始日</param>
        /// <param name="to">期間の終了日</param>
        /// <param name="supplier">仕入先ID</param>
        /// <returns>仕入先発注エンティティの一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IEnumerable<OrdersToSupplier> FindAllOrdersToSupplier(DateTime from, DateTime to, int supplier)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindAllOrders(from, to, supplier);
        }

        /// <summary>
        /// 仕入先発注を取り消す
        /// </summary>
        /// <param name="orderNo">発注番号</param>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public void CancelOrderToSupplier(string orderNo)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.SupplierModel.CancelOrder(orderNo);
        }

        /// <summary>
        /// 仕入先に発注する
        /// </summary>
        /// <param name="order">発注情報</param>
        /// <returns>データベースに登録した発注情報</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public string Order(OrdersToSupplier order)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.Order(order.OrderDate, order.Supplier, order.DeliveryDate, order.Details.Select(i => Tuple.Create(i.PartsCode, i.LotCount)).ToArray());
        }

        /// <summary>
        /// 注文済単品の入荷(予定)日を変更する
        /// </summary>
        /// <param name="orderNo">発注番号</param>
        /// <param name="newDate">変更後の入荷(予定)日</param>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public void ChangeArrivalDateOfOrderToSupplier(string orderNo, DateTime newDate)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.SupplierModel.ChangeArrivalDate(orderNo, newDate);
        }

        /// <summary>
        /// 入荷した単品を検品する
        /// </summary>
        /// <param name="orderNo">発注番号</param>
        /// <param name="date">検品実施日</param>
        /// <param name="quantites">検品時に増減のあった単品と実際の入荷数量：検品時に発注数と差異のないものは指定不要</param>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public void InspectArrivedOrder(string orderNo, DateTime date, IEnumerable<Tuple<BouquetPart, int>> quantites)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.SupplierModel.ChangeArrivedQuantities(orderNo, quantites);
            Model.SupplierModel.OrderIsArrived(date, orderNo);

        }
        #endregion // 仕入先発注情報の操作

        #region 得意先受注情報の操作
        /// <summary>
        /// 得意先受注情報を取得する
        /// </summary>
        /// <param name="orderNo">受注番号</param>
        /// <returns>得意先受注情報</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public OrderFromCustomer FindOrdersFromCustomer(string orderNo)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindOrder(orderNo);
        }

        /// <summary>
        /// データベースに登録されているすべての得意先受注情報を取得する
        /// </summary>
        /// <returns>得意先受注情報の一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IEnumerable<OrderFromCustomer> FindAllOrdersFromCustomer()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllOrders();
        }

        /// <summary>
        /// 期間指定で得意先受注情報を取得する
        /// </summary>
        /// <param name="from">期間の開始日</param>
        /// <param name="to">期間の終了日</param>
        /// <returns>得意先受注情報の一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IEnumerable<OrderFromCustomer> FindAllOrdersFromCustomer(DateTime from, DateTime to)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllOrders(from, to);
        }

        /// <summary>
        /// 期間指定で特定得意先からの得意先受注情報を取得する
        /// </summary>
        /// <param name="from">期間の開始日</param>
        /// <param name="to">期間の終了日</param>
        /// <param name="customerID">得意先ID</param>
        /// <returns>得意先受注情報の一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IEnumerable<OrderFromCustomer> FindAllOrdersFromCustomer(DateTime from, DateTime to, int customerID)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllOrders(from, to, customerID);
        }

        /// <summary>
        /// 得意先から受注する
        /// </summary>
        /// <param name="order">受注情報</param>
        /// <param name="arrivalDate">お届け日</param>
        /// <returns>受注番号</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public string OrderFromCustomer(OrderFromCustomer order, DateTime arrivalDate)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.Order(order.OrderDate, order.Bouquet, order.ShippingAddress, arrivalDate);
        }

        /// <summary>
        /// 得意先からの受注を取り消す
        /// </summary>
        /// <param name="orderNo">受注番号</param>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public void CancelOrderFromCustomer(string orderNo)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.CustomerModel.CancelOrder(orderNo);
        }

        /// <summary>
        /// お届け日を変更する
        /// </summary>
        /// <param name="orderNo">受注番号</param>
        /// <param name="arrivalDate">変更後のお届け日</param>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public void ChangeArrivalDateOfOrderFromCustomer(string orderNo, DateTime arrivalDate)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.CustomerModel.ChangeArrivalDate(DateTime.Today, orderNo, arrivalDate);
        }

        /// <summary>
        /// 受注商品を発送する
        /// </summary>
        /// <param name="date">出荷日</param>
        /// <param name="orders">出荷した商品の得意先受注番号、複数指定可能</param>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public void ShipBouquetOrders(DateTime date, params string[] orders)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.CustomerModel.ShipOrders(date, orders);
        }
        #endregion // 得意先受注情報の操作

        #region お届け先の操作
        /// <summary>
        /// 指定得意先が贈ったお届け先の一覧を取得する
        /// </summary>
        /// <param name="customer">得意先ID</param>
        /// <returns>お届け先の一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IEnumerable<ShippingAddress> FindAllShippingAddressOfCustomer(int customer)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllShippingAddressesOfCustomer(customer);
        }

        /// <summary>
        /// お届け先をデータベースに登録する
        /// </summary>
        /// <param name="address">お届け先情報</param>
        /// <returns>データベースに登録したお届け先情報</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public ShippingAddress Save(ShippingAddress address)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.Save(address);
        }
        #endregion // お届け先の操作

        #region 在庫推移表
        /// <summary>
        /// 在庫推移表を作成する
        /// </summary>
        /// <param name="partsCode">花コード</param>
        /// <param name="from">開始日</param>
        /// <param name="to">終了日</param>
        /// <returns>特定単品、指定期間の在庫推移表</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public InventoryTransitionTable CreateInventoryTransitionTable(string partsCode, DateTime from, DateTime to)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CreateInventoryTransitionTable(partsCode, from, (to - from).Days);
        }
        #endregion // 在庫推移表

        #region 加工指示書
        /// <summary>
        /// 指定商品の加工個数を取得する
        /// </summary>
        /// <param name="bouquet">花束コード</param>
        /// <param name="date">加工(予定)日</param>
        /// <returns></returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public int GetNumberOfProcessingBouquetsOf(string bouquet, DateTime date)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.GetNumberOfProcessingBouquetsOf(bouquet, date);

        }

        /// <summary>
        /// 指定日に加工する商品と加工本数を一括で取得する
        /// </summary>
        /// <param name="date">加工(予定)日</param>
        /// <returns>単品毎の加工本数一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IDictionary<string, int> GetShippingBouquetCountAt(DateTime date)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.GetShippingBouquetCountAt(date);
        }
        #endregion // 加工指示書

        #region 在庫の操作、破棄
        /// <summary>
        /// 指定日の単品在庫数を取得する
        /// </summary>
        /// <param name="date">破棄(予定)日</param>
        /// <returns>単品毎の在庫本数一覧</returns>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public IDictionary<string, int> FindInventoriesAt(DateTime date)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindInventoriesAt(date);
        }

        /// <summary>
        /// 在庫を破棄する
        /// </summary>
        /// <param name="date">破棄実施日</param>
        /// <param name="discardParts">破棄する単品と破棄本数、複数指定可能</param>
        /// <exception cref="NotConnectedToDatabaseException">データベース接続前にこのメソッドを呼び出した</exception>
        public void DiscardInventoruies(DateTime date, params Tuple<string, int>[] discardParts)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.BouquetModel.DiscardBouquetParts(date, discardParts);
        }
        #endregion // 在庫の操作、破棄
    }
}
