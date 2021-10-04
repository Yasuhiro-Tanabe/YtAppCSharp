using MemorieDeFleurs.Databese.SQLite;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// フレール・メモワール向け花卉仕入販売支援システムのデータモデル
    /// </summary>
    public class MemorieDeFleursModel
    {
        internal SqliteConnection DbConnection { get; private set; }
        
        public SupplierModel SupplierModel { get; private set; }

        public BouquetModel BouquetModel { get; private set; }

        public CustomerModel CustomerModel { get; private set; }

        public SequenceUtil Sequences { get; set; }

        /// <summary>
        /// データモデルを生成する。
        /// 
        /// テスト・デバッグの利便性を考慮し、操作するデータベースを
        /// 外部から指定できるようにしている。
        /// </summary>
        /// <param name="db"></param>
        public MemorieDeFleursModel(SqliteConnection connection)
        {
            DbConnection = connection;

            SupplierModel = new SupplierModel(this);
            BouquetModel = new BouquetModel(this);
            CustomerModel = new CustomerModel(this);

            Sequences = new SequenceUtil(this);
        }

        /// <summary>
        /// 在庫推移表を作成する
        /// </summary>
        /// <param name="partsCode">対象単品の花コード</param>
        /// <param name="date">起点となる日付</param>
        /// <param name="numDays">作成期間</param>
        /// <returns>在庫推移表 (期間： date ～ date + numDays)</returns>
        public InventoryTransitionTable CreateInventoryTransitionTable(string partsCode, DateTime date, int numDays)
        {
            var ret = new InventoryTransitionTable();
            ret.Fill(DbConnection, partsCode, date, numDays);
            return ret;
        }
    }
}
