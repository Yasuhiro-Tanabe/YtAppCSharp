
using System;
using System.Data.Common;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// フレール・メモワール向け花卉仕入販売支援システムのデータモデル
    /// </summary>
    public class MemorieDeFleursModel
    {
        /// <summary>
        /// システム内部で使用するデータベース
        /// </summary>
        internal DbConnection DbConnection { get; private set; }
        
        /// <summary>
        /// 仕入先モデル：仕入先の登録改廃、単品仕入先の登録、仕入先への発注、納品処理を行う。
        /// </summary>
        public SupplierModel SupplierModel { get; private set; }

        /// <summary>
        /// 商品モデル：商品と単品の登録改廃を行う。
        /// 他に、<see cref="SupplierModel"/> や <see cref="CustomerModel"/> からの単品在庫数量変更要求を受け付ける。
        /// </summary>
        public BouquetModel BouquetModel { get; private set; }

        /// <summary>
        /// 得意先モデル：得意先やお届け先の登録改廃、得意先からの商品受注、出荷処理を行う。
        /// </summary>
        public CustomerModel CustomerModel { get; private set; }

        /// <summary>
        /// システム内部で使用する連番管理クラス
        /// </summary>
        public SequenceUtil Sequences { get; set; }

        /// <summary>
        /// データモデルを生成する。
        /// 
        /// テスト・デバッグの利便性を考慮し、操作するデータベースを
        /// 外部から指定できるようにしている。
        /// </summary>
        /// <param name="connection">操作するデータベースの接続オブジェクト</param>
        public MemorieDeFleursModel(DbConnection connection)
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
