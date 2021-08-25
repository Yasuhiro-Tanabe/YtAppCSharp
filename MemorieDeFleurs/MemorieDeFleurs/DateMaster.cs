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
                    query.CommandText = $"select min(DATE) from DATE_MASTER";
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

        private int DateTimeToDate(DateTime datetime)
        {
            return datetime.Year * 10000 + datetime.Month * 100 + datetime.Day;
        }

        private DateTime DateToDateTime(int date)
        {
            return DateTime.Parse($"{date / 10000:0000}-{(date % 10000) / 100}-{date % 100}");
        }

        public void Fill(int start, int end)
        {
            if(start <= FirstDate)
            {
                throw new IndexOutOfRangeException($"開始日 {start} 以前の日付は指定できません。");
            }
            if(end <= LastDate)
            {
                throw new IndexOutOfRangeException($"終了日 {end} までの日付は登録済みです。");
            }
            if(start > end)
            {
                var tmp = end;
                end = start;
                start = tmp;
            }

            var startDate = DateToDateTime(start);
            var endDate = DateToDateTime(end);
            if(start <= LastDate)
            {
                var last = DateToDateTime(LastDate);
                startDate = last.AddDays(1);
            }

            if(startDate == endDate)
            {
                // 何もしない
                return;
            }


            var transaction = Connection.BeginTransaction();

            try
            {
                using(var cmd = Connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "insert into DATE_MASTER values ( @date, @index )";

                    var index = LastDate < 0 ? 0 : DateToIndex(LastDate) + 1;
                    for (var d = startDate; d <= endDate; d = d.AddDays(1), index++)
                    {
                        cmd.Parameters.AddWithValue("@date", DateTimeToDate(d));
                        cmd.Parameters.AddWithValue("@index", index);
                        cmd.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }

        public bool IsValidDate(int date)
        {
            DateTime dt;
            return DateTime.TryParse($"{date / 10000:0000}-{(date % 10000) / 100}-{date % 100}", out dt);
        }

        private int DateToIndex(int date)
        {
            if(date < FirstDate || LastDate < date)
            {
                throw new IndexOutOfRangeException($"未登録日付です：{date}");
            }
            if(!IsValidDate(date))
            {
                throw new IndexOutOfRangeException($"不正な日付です：{date}");
            }

            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = $"select DATE_INDEX from DATE_MASTER where DATE={date}";
                using (var result = cmd.ExecuteReader())
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

        public void Clear()
        {
            using(var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = "delete from DATE_MASTER";
                cmd.ExecuteNonQuery();
            }

        }
    }
}
