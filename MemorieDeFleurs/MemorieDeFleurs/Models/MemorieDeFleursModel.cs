using MemorieDeFleurs.Databese.SQLite;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
    }
}
