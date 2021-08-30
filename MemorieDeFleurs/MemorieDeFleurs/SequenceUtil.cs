using Microsoft.Data.Sqlite;

using System;

namespace MemorieDeFleurs
{
    /// <summary>
    /// シーケンス操作ユーティリティ
    /// </summary>
    public class SequenceUtil
    {
        private const string GetSQL = "select VALUE from SEQUENCES where NAME=@name";
        private const string SetSQL = "update SEQUENCES set VALUE = @value where NAME=@name";
        private const string CreateSQL = "insert into SEQUENCES values (@name, @value)";
        private const string Value = "@value";
        private const string Name = "@name";

        private SqliteCommand GetStatement;
        private SqliteCommand SetStatement;
        private SqliteCommand CreateStatement;

        private SqliteConnection Connection { get; set; }

        public class SequenceValue
        {
            private string Name { get; set; }

            private SequenceUtil Util { get; set; }

            public int Next
            {
                get { return Util.Next(Name); }
            }

            public SequenceValue(string name, SequenceUtil parent)
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
        public SequenceValue SEQ_CUSTOMERS { get; private set; }

        /// <summary>
        /// シーケンス SEQ_SHIPPING
        /// 
        /// SEQ_SHIPPING.Next で次の連番を自動採番・取得できる。
        /// </summary>
        public SequenceValue SEQ_SHIPPING { get; private set; }

        /// <summary>
        /// シーケンス SEQ_SUPPLIERS
        /// 
        /// SEQ_SUPPLIERS.Next で次の連番を自動採番・取得できる。
        /// </summary>
        public SequenceValue SEQ_SUPPLIERS { get; private set; }

        /// <summary>
        /// シーケンス SEQ_SESSION
        /// 
        /// SEQ_SESSION.Next で次の連番を自動採番・取得できる。
        /// </summary>
        public SequenceValue SEQ_SESSION { get; private set; }

        public SequenceValue SEQ_STOCK_LOT_NUMBER { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// 
        /// テストでの使い勝手をよくするため、DBとの接続は外部から注入させる。
        /// </summary>
        /// <param name="conn">接続先DB</param>
        public SequenceUtil(SqliteConnection conn)
        {
            Connection = conn;

            GetStatement = conn.CreateCommand();
            GetStatement.CommandText = GetSQL;
            GetStatement.Parameters.Add(Name, SqliteType.Text);

            SetStatement = conn.CreateCommand();
            SetStatement.CommandText = SetSQL;
            SetStatement.Parameters.Add(Name, SqliteType.Text);
            SetStatement.Parameters.Add(Value, SqliteType.Integer);

            CreateStatement = conn.CreateCommand();
            CreateStatement.CommandText = CreateSQL;
            CreateStatement.Parameters.Add(Name, SqliteType.Text);
            CreateStatement.Parameters.Add(Value, SqliteType.Integer);

            SEQ_CUSTOMERS = new SequenceValue("SEQ_CUSTOMERS", this);
            SEQ_SHIPPING = new SequenceValue("SEQ_SHIPPING", this);
            SEQ_SUPPLIERS = new SequenceValue("SEQ_SUPPLIERS", this);
            SEQ_SESSION = new SequenceValue("SEQ_SESSION", this);
            SEQ_STOCK_LOT_NUMBER = new SequenceValue("SEQ_STOCK_LOT_NUMBER", this);
        }

        private int Next(string sequenceName)
        {
            var transaction = Connection.BeginTransaction();

            try
            {
                GetStatement.Transaction = transaction;
                SetStatement.Transaction = transaction;
                CreateStatement.Transaction = transaction;

                GetStatement.Parameters[Name].Value = sequenceName;

                var next = 0;
                using (var reader = GetStatement.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read() && !reader.IsDBNull(0))
                    {
                        next = reader.GetInt32(0) + 1;

                        SetStatement.Parameters[Name].Value = sequenceName;
                        SetStatement.Parameters[Value].Value = next;
                        SetStatement.ExecuteNonQuery();
                    }
                    else
                    {
                        next = 1;

                        CreateStatement.Parameters[Name].Value = sequenceName;
                        CreateStatement.Parameters[Value].Value = next;
                        CreateStatement.ExecuteNonQuery();
                    }
                }

                transaction.Commit();

                return next;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
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
