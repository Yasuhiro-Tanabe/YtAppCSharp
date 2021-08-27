using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public SupplierModel SupplierModel { get; private set; }

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
            SupplierModel = new SupplierModel(this);

        }
    }
}
