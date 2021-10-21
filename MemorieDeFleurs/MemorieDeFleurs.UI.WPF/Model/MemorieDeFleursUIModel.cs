using MemorieDeFleurs.Database.SQLite;
using MemorieDeFleurs.Logging;

using System;
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

        public void OpenSQLiteDatabaseFile(string path)
        {
            LogUtil.DEBUGLOG_BeginMethod(path);
            try
            {
                DbConnection = MemorieDeFleursDatabaseFacade.OpenDatabase(path);
                LogUtil.Info($"Database opened: {path}");
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
    }
}
