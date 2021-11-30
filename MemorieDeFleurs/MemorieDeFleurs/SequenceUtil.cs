using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using System.Data.Common;

namespace MemorieDeFleurs
{
    /// <summary>
    /// シーケンス操作ユーティリティ
    /// </summary>
    public class SequenceUtil
    {
        private DbConnection Connection { get; set; }

        /// <summary>
        /// シーケンスの値管理クラス
        /// </summary>
        public class SequenceValueManager
        {
            private string Name { get; set; }

            private SequenceUtil Util { get; set; }

            /// <summary>
            /// 次のシーケンス値を返す
            /// </summary>
            /// <returns>次の値</returns>
            public int Next()
            {
                return Util.Next(Name);
            }

            /// <summary>
            /// 次のシーケンス値を返す
            /// 
            /// トランザクション内での呼出用
            /// </summary>
            /// <param name="context">トランザクション中のDBコンテキスト</param>
            /// <returns>次の値</returns>
            public int Next(MemorieDeFleursDbContext context)
            {
                return Util.Next(context, Name);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name">シーケンスの名前</param>
            /// <param name="parent">この管理オブジェクトを管理しているシーケンスユーティリティ</param>
            public SequenceValueManager(string name, SequenceUtil parent)
            {
                Name = name;
                Util = parent;
            }
        }

        /// <summary>
        /// シーケンス SEQ_CUSTOMERS
        /// 
        /// SEQ_CUSTOMERS.Next で次の連番を自動採番・取得できる。
        /// </summary>
        public SequenceValueManager SEQ_CUSTOMERS { get; private set; }

        /// <summary>
        /// シーケンス SEQ_SHIPPING
        /// 
        /// SEQ_SHIPPING.Next で次の連番を自動採番・取得できる。
        /// </summary>
        public SequenceValueManager SEQ_SHIPPING { get; private set; }

        /// <summary>
        /// シーケンス SEQ_SUPPLIERS
        /// 
        /// SEQ_SUPPLIERS.Next で次の連番を自動採番・取得できる。
        /// </summary>
        public SequenceValueManager SEQ_SUPPLIERS { get; private set; }

        /// <summary>
        /// シーケンス SEQ_SESSION
        /// 
        /// SEQ_SESSION.Next で次の連番を自動採番・取得できる。
        /// </summary>
        public SequenceValueManager SEQ_SESSION { get; private set; }

        /// <summary>
        /// シーケンス SEQ_INVENTORY_LOT_NUMER
        /// 
        /// SEQ_SESSION.Next で次の連番を自動採番・取得できる。
        /// </summary>
        public SequenceValueManager SEQ_INVENTORY_LOT_NUMBER { get; private set; }

        /// <summary>
        /// このシーケンスユーティリティを管理しているモデルオブジェクト
        /// </summary>
        private MemorieDeFleursModel Parent { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model"></param>
        public SequenceUtil(MemorieDeFleursModel model)
        {
            Parent = model;
            Connection = model.DbConnection;

            SEQ_CUSTOMERS = new SequenceValueManager("SEQ_CUSTOMERS", this);
            SEQ_SHIPPING = new SequenceValueManager("SEQ_SHIPPING", this);
            SEQ_SUPPLIERS = new SequenceValueManager("SEQ_SUPPLIERS", this);
            SEQ_SESSION = new SequenceValueManager("SEQ_SESSION", this);
            SEQ_INVENTORY_LOT_NUMBER = new SequenceValueManager("SEQ_STOCK_LOT_NUMBER", this);
        }

        private int Next(string sequenceName)
        {
            using(var context = new MemorieDeFleursDbContext(Parent.DbConnection))
            {
                return Next(context, sequenceName);
            }
        }

        private int Next(MemorieDeFleursDbContext context, string sequenceName)
        {
            var sequenceValue = context.Sequences.Find(sequenceName);
            if (sequenceValue == null)
            {
                sequenceValue = new SequenceValue() { Name = sequenceName, Value = 1 };
                context.Sequences.Add(sequenceValue);
                context.SaveChanges();
            }
            else
            {
                sequenceValue.Value++;
                context.Sequences.Update(sequenceValue);
                context.SaveChanges();
            }
            return sequenceValue.Value;
        }

        /// <summary>
        /// データベースに登録されている全シーケンスの値を初期化する
        /// </summary>
        public void Clear()
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = "delete from SEQUENCES";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
