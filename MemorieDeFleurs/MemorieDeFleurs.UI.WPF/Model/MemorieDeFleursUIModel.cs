using MemorieDeFleurs.Database.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;

using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.Data.Common;

namespace MemorieDeFleurs.UI.WPF.Model
{
    public class MemorieDeFleursUIModel : NotificationObject
    {
        private static MemorieDeFleursUIModel _singleton = new MemorieDeFleursUIModel();

        private MemorieDeFleursUIModel() { }

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
        public IEnumerable<BouquetPart> FindAllBouquetParts()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindAllBoueuqtParts();
        }

        public BouquetPart FindBouquetParts(string partsCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindBouquetPart(partsCode);
        }

        public void RemoveBouquetParts(string partsCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.BouquetModel.RemoveBouquetParts(partsCode);
        }
        #endregion // 単品の操作

        #region 商品と商品構成の操作
        public IEnumerable<Bouquet> FindAllBouquets()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindAllBouquets();
        }

        public void RemoveBouquet(string bouquetCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.BouquetModel.RemoveBouquet(bouquetCode);
        }

        public Bouquet FindBouquet(string bouquetCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindBouquet(bouquetCode);
        }

        public void UpdatePartsList(string bouquetCode, IEnumerable<KeyValuePair<string, int>> partsList)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.BouquetModel.RemoveAllPartsFrom(bouquetCode);
            foreach(var kv in partsList)
            {
                Model.BouquetModel.UpdateQuantityOf(bouquetCode, kv.Key, kv.Value);
            }
        }
        #endregion // 商品と商品構成の操作

        #region 仕入先と単品仕入先の操作
        public IEnumerable<Supplier> FindAllSuppliers()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindAllSuppliers();
        }

        public Supplier FindSupplier(int supplierCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.SupplierModel.FindSupplier(supplierCode);
        }

        public void RemoveSupplier(int supplierCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.SupplierModel.RemoveSupplier(supplierCode);
        }

        public void UpdateSupplingParts(int supplierCode, IEnumerable<string> parts)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            throw new NotImplementedException();
        }
        #endregion // 仕入先と単品仕入先の操作

    }
}
