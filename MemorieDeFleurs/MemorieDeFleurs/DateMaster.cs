using System;
using System.Data.SQLite;

namespace MemorieDeFleurs
{
    public class DateMaster
    {
        public SQLiteConnection Connection { get; private set; }

        public DateMaster(SQLiteConnection conn)
        {
            Connection = conn;
        }

        public int FirstDate
        {
            get
            {
                using (var query = Connection.CreateCommand())
                {
                    query.CommandText = $"select min(DATE_INDEX) from DATE_MASTER";
                    using (var result = query.ExecuteReader())
                    {
                        if (result.HasRows && result.Read() && !result.IsDBNull(0))
                        {
                            return result.GetInt32(0);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
        }

        public int LastDate
        {
            get
            {
                using (var query = Connection.CreateCommand())
                {
                    query.CommandText = $"select max(DATE) from DATE_MASTER";
                    using(var result = query.ExecuteReader())
                    {
                        if (result.HasRows && result.Read() && !result.IsDBNull(0))
                        {
                            return result.GetInt32(0);
                        }
                        else
                        {
                            return -1;
                        }
                    }

                }
            }
        }
    }
}
