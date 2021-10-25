using MemorieDeFleurs.Database.SQLite;
using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;

using System;
using System.Collections.Generic;
using System.Data.Common;

using static MemorieDeFleurs.Models.BouquetModel;

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
                LogUtil.Warn($"{ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod(path);
            }
        }

        public IEnumerable<BouquetPart> FindAllBouquetParts()
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            return Model.BouquetModel.FindAllBoueuqtParts();
        }

        public void RemoveBouquetParts(string partsCode)
        {
            if (DbConnection == null) { throw new NotConnectedToDatabaseException(); }
            Model.BouquetModel.RemoveBouquetParts(partsCode);
        }

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
    }
}
