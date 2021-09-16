using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;

using System.Linq;

namespace MemorieDeFleurs
{
    /// <summary>
    /// シーケンス操作ユーティリティ
    /// </summary>
    public class SequenceUtil
    {
        private SqliteConnection Connection { get; set; }

        public class SequenceValueManager
        {
            private string Name { get; set; }

            private SequenceUtil Util { get; set; }

            public int Next()
            {
                return Util.Next(Name);
            }

            public int Next(MemorieDeFleursDbContext context)
            {
                return Util.Next(context, Name);
            }

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

        public SequenceValueManager SEQ_STOCK_LOT_NUMBER { get; private set; }

        private MemorieDeFleursModel Parent { get; set; }
        public object SingleOrDefault { get; private set; }

        public SequenceUtil(MemorieDeFleursModel model)
        {
            Parent = model;
            Connection = model.DbConnection;

            SEQ_CUSTOMERS = new SequenceValueManager("SEQ_CUSTOMERS", this);
            SEQ_SHIPPING = new SequenceValueManager("SEQ_SHIPPING", this);
            SEQ_SUPPLIERS = new SequenceValueManager("SEQ_SUPPLIERS", this);
            SEQ_SESSION = new SequenceValueManager("SEQ_SESSION", this);
            SEQ_STOCK_LOT_NUMBER = new SequenceValueManager("SEQ_STOCK_LOT_NUMBER", this);
        }

        private int Next(string sequenceName)
        {
            return Next(Parent.DbContext, sequenceName);
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
