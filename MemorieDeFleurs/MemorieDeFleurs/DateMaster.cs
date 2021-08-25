using System;
using System.Data.SQLite;

namespace MemorieDeFleurs
{
    /// <summary>
    /// 日付マスタテーブルの操作
    /// </summary>
    public class DateMaster
    {
        /// <summary>
        /// 不正な日付を表す定数値
        /// </summary>
        public const int InvalidDate = -1;

        /// <summary>
        /// 不正な日付インデックスを表す定数値
        /// </summary>
        public const int InvalidDateIndex = -1; 

        public SQLiteConnection Connection { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// 
        /// テストでの使い勝手をよくするため、DBとの接続は外部から注入させる。
        /// </summary>
        /// <param name="conn">接続先DB</param>
        public DateMaster(SQLiteConnection conn)
        {
            Connection = conn;
        }

        /// <summary>
        /// 日付マスタに登録されている最も古い日付、日付マスタに何も登録されていないときは InvalidDate を返す。
        /// </summary>
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

        /// <summary>
        /// 日付登録されている最も新しい日付、日付マスタに何も登録されていないときは InvalidDate を返す。
        /// </summary>
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

        /// <summary>
        /// 日付マスタに start ～ end の日付を登録する
        /// </summary>
        /// <param name="start">最も古い日付</param>
        /// <param name="end">最も新しい日付</param>
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
            catch(Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 日付として正しいデータかどうかのチェックを行う：日付は整数値なので、20201234や20200230といった
        /// 日付として正しくないデータを扱えてしまう。本メソッドで、日付としての正しさをチェックする。
        /// </summary>
        /// <param name="date">チェック対象日付</param>
        /// <returns>日付として正しい値は真、正しくない値は偽。
        /// 20210229 (閏年でない年の閏日)や 21000229 (閏年だが西暦年%400=100，200，300の時は閏日がない) なども、
        /// 「正しくない日付」として判定する</returns>
        public bool IsValidDate(int date)
        {
            DateTime dt;
            return DateTime.TryParse($"{date / 10000:0000}-{(date % 10000) / 100}-{date % 100}", out dt);
        }

        public int Add(int date, int shift)
        {
            return IndexToDate(DateToIndex(date) + shift);
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

        private int IndexToDate(int index)
        {
            if(index < 0) 
            {
                throw new IndexOutOfRangeException($"不正な日付インデックスです：{index}");
            }

            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = $"select DATE from DATE_MASTER where DATE_INDEX={index}";
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

        /// <summary>
        /// 日付マスタから全データを削除する
        /// </summary>

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
