using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// フレール・メモワール向け花卉仕入販売支援システムのデータモデル
    /// </summary>
    public class MemorieDeFleursModel
    {
        /// <summary>
        /// (パッケージ内限定) Entity Framework Data Context
        /// </summary>
        internal MemorieDeFleursDbContext DbContext { get; private set; }
        internal SqliteConnection DbConnection { get; private set; }
        
        public SupplierModel SupplierModel { get; private set; }

        public BouquetModel BouquetModel { get; private set; }

        public CustomerModel CustomerModel { get; private set; }

        public DateUtil DateMaster { get; private set; }
        public SequenceUtil Sequences { get; set; }

        /// <summary>
        /// データモデルを生成する。
        /// 
        /// テスト・デバッグの利便性を考慮し、操作するデータベース(Entity Framework Data Context) を
        /// 外部から指定できるようにしている。
        /// </summary>
        /// <param name="db"></param>
        public MemorieDeFleursModel(MemorieDeFleursDbContext db)
        {
            DbContext = db;
            DbConnection = db.Database.GetDbConnection() as SqliteConnection;

            SupplierModel = new SupplierModel(this);
            BouquetModel = new BouquetModel(this);
            CustomerModel = new CustomerModel(this);
            
            DateMaster = new DateUtil(DbConnection);
            Sequences = new SequenceUtil(DbConnection);

        }
    }
}
