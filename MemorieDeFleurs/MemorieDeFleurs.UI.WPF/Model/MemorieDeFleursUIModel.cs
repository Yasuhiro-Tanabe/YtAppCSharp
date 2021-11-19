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
        public Customer FindCustomer(int id)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindCustomer(id);
        }

        public IEnumerable<Customer> FindAllCustomers()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllCustomers();
        }

        public void RemoveCustomer(int id)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.CustomerModel.RemoveCustomer(id);
        }

        public Customer Save(Customer customer)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.Save(customer);
        }
        #endregion // 得意先の操作

        #region 仕入先発注情報の操作
        public OrdersToSupplier FindOrderToSupplier(string id)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindOrder(id);
        }

        public IEnumerable<OrdersToSupplier> FindAllOrdersToSupplier()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindAllOrders();
        }

        public IEnumerable<OrdersToSupplier> FindAllOrdersToSupplier(DateTime from, DateTime to)
        {
            if(DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindAllOrders(from, to);
        }

        public IEnumerable<OrdersToSupplier> FindAllOrdersToSupplier(DateTime from, DateTime to, int supplier)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindAllOrders(from, to, supplier);
        }

        public void CancelOrderToSupplier(string orderNo)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.SupplierModel.CancelOrder(orderNo);
        }

        public string Order(OrdersToSupplier order)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.Order(order.OrderDate, order.Supplier, order.DeliveryDate, order.Details.Select(i => Tuple.Create(i.PartsCode, i.LotCount)).ToArray());
        }

        public void ChangeArrivalDateOfOrderToSupplier(string orderNo, DateTime newDate)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.SupplierModel.ChangeArrivalDate(orderNo, newDate);
        }
        #endregion // 仕入先発注情報の操作

        #region 得意先受注情報の操作
        public OrderFromCustomer FindOrdersFromCustomer(string orderNo)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindOrder(orderNo);
        }
        public IEnumerable<OrderFromCustomer> FindAllOrdersFromCustomer()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllOrders();
        }

        public IEnumerable<OrderFromCustomer> FindAllOrdersFromCustomer(DateTime from, DateTime to)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllOrders(from, to);
        }

        public IEnumerable<OrderFromCustomer> FindAllOrdersFromCustomer(DateTime from, DateTime to, int customerID)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllOrders(from, to, customerID);
        }

        public string OrderFromCustomer(OrderFromCustomer order, DateTime arrivalDate)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.Order(order.OrderDate, order.Bouquet, order.ShippingAddress, arrivalDate);
        }

        public void CancelOrderFromCustomer(string orderNo)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.CustomerModel.CancelOrder(orderNo);
        }

        public void ChangeArrivalDateOfOrderFromCustomer(string orderNo, DateTime arrivalDate)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.CustomerModel.ChangeArrivalDate(DateTime.Today, orderNo, arrivalDate);
        }
        #endregion // 得意先受注情報の操作

        #region お届け先の操作
        public IEnumerable<ShippingAddress> FindAllShippingAddressOfCustomer(int customer)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.FindAllShippingAddressesOfCustomer(customer);
        }

        public ShippingAddress Save(ShippingAddress address)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.Save(address);
        }
        #endregion // お届け先の操作

        #region 在庫推移表
        public InventoryTransitionTable CreateInventoryTransitionTable(string partsCode, DateTime from, DateTime to)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CreateInventoryTransitionTable(partsCode, from, (to - from).Days);
        }
        #endregion // 在庫推移表

        #region 加工指示書
        public int GetNumberOfProcessingBouquetsOf(string bouquet, DateTime date)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.GetNumberOfProcessingBouquetsOf(bouquet, date);

        }

        public IDictionary<string, int> GetShippingBouquetCountAt(DateTime date)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.CustomerModel.GetShippingBouquetCountAt(date);
        }
        #endregion // 加工指示書
    }
}
