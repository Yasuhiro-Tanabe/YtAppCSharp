using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// 在庫推移表
    /// </summary>
    public class InventoryTransitionTable
    {
        private BouquetPart BouquetPart { get; set; }

        /// <summary>
        /// 花コード
        /// </summary>
        public string PartsCode { get { return BouquetPart.Code; } }

        /// <summary>
        /// 品質維持可能日数
        /// </summary>
        public int ExpiryDate { get { return BouquetPart.ExpiryDate; } }

        /// <summary>
        /// 在庫推移表の1日分のデータ
        /// </summary>
        public class InventoryTransitionOfTheDay
        {
            /// <summary>
            /// 当日入荷(予定)数
            /// </summary>
            public int Arrived { get; private set; }

            /// <summary>
            /// 当日使用(予定)数
            /// </summary>
            public int Used { get; private set; }

            /// <summary>
            /// ○日前の残数
            /// </summary>
            public class InventoryRemainsList
            {
                private int ExpiryDays { get; set; }
                private int[] _remains;

                /// <summary>
                /// 入荷日別の在庫残数
                /// </summary>
                /// <param name="days">基準日の何日前か： -<see cref="ExpiryDays"/> ≦ days ≦ 0 で指定する</param>
                /// <returns></returns>
                public int this[int days]
                {
                    get
                    {
                        if(days < -ExpiryDays || 0 < days)
                        {
                            throw new IndexOutOfRangeException($"日数指定が正しくない({days})：日数は {-ExpiryDays} ～ 0 の範囲内でなければならない.");
                        }
                        return _remains[-days];
                    }
                }

                public void Fill(IList<InventoryAction> actions, DateTime date, int expiry)
                {
                    ExpiryDays = expiry;
                    _remains = new int[ExpiryDays];

                    var days = Enumerable.Range(0, expiry).Select(i => Tuple.Create(i, date.AddDays(-i))).ToList();
                    foreach(var d in days)
                    {
                        _remains[d.Item1] = actions
                            .Where(act => act.ActionDate == date)
                            .Where(act => act.ArrivalDate == d.Item2)
                            .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE || act.Action == InventoryActionType.USED || act.Action == InventoryActionType.SHORTAGE)
                            .Sum(act => act.Remain);
                    }
                }
            }

            /// <summary>
            /// ○日前残：<see cref="ExpiryDate"/> 日前残 (<see cref="Remains"/>[-<see cref="ExpiryDate"/>]) ～ 当日残(<see cref="Remains"/>[0]) 
            /// </summary>
            public InventoryRemainsList Remains { get; } = new InventoryRemainsList();

            /// <summary>
            /// 当日廃棄(予定)数
            /// </summary>
            public int Discarded { get; private set; }


            /// <summary>
            /// 特定日付の在庫推移情報を作成する
            /// </summary>
            /// <param name="actions">在庫推移情報の元データ(在庫アクション)</param>
            /// <param name="part">対象単品</param>
            /// <param name="date">基準日</param>
            public void Fill(IList<InventoryAction> actions, BouquetPart part, DateTime date)
            {
                Arrived = actions
                    .Where(act => act.ActionDate == date)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE || act.Action == InventoryActionType.ARRIVED)
                    .Sum(act => act.Quantity);

                Discarded = actions
                    .Where(act => act.ActionDate == date)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_DISCARD || act.Action == InventoryActionType.DISCARDED)
                    .Sum(act => act.Quantity);
                
                Used = actions
                    .Where(act => act.ActionDate == date)
                    .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_USE || act.Action == InventoryActionType.USED || act.Action == InventoryActionType.SHORTAGE)
                    .Sum(act => act.Quantity);

                Remains.Fill(actions, date, part.ExpiryDate);
            }
        }

        private IDictionary<DateTime, InventoryTransitionOfTheDay> Inventories { get; } = new SortedDictionary<DateTime, InventoryTransitionOfTheDay>();

        /// <summary>
        /// 生成された在庫推移表のうち最も古い日付
        /// </summary>
        public DateTime FirstDate { get { return Inventories.Min(item => item.Key); } }

        /// <summary>
        /// 生成された在庫推移表のうち最も新しい日付
        /// </summary>
        public DateTime LastDate { get { return Inventories.Max(item => item.Key); } }

        /// <summary>
        /// 指定日の在庫推移を取得する
        /// </summary>
        /// <param name="date">日付：<see cref="FirstDate"/> ～ <see cref="LastDate"/> の範囲内でなければならない。
        /// (<see cref="Fill(DbConnection, string, DateTime, int)"/> で指定した日付からの生成期間日数)
        /// </param>
        /// <returns>指定日付の在庫推移</returns>
        public InventoryTransitionOfTheDay this[DateTime date] { 
            get
            {
                if(date < FirstDate || LastDate < date)
                {
                    throw new IndexOutOfRangeException($"日付指定が正しくない({date:yyyyMMdd})：日付は {FirstDate:yyyyMMdd} ～ {LastDate:yyyyMMdd} でなければならない ");
                }

                return Inventories[date];
            }
        }

        /// <summary>
        /// 指定日の n 日前残を取得する
        /// </summary>
        /// <param name="theDay"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public int this[DateTime theDay, int days] { get { return Inventories[theDay].Remains[days]; } }

        /// <summary>
        /// 指定単品指定期間の在庫推移表を生成する
        /// </summary>
        /// <param name="connection">接続先DB</param>
        /// <param name="partsCode">対象単品の花コード</param>
        /// <param name="date">開始日</param>
        /// <param name="numDays">生成期間</param>
        public void Fill(DbConnection connection, string partsCode, DateTime date, int numDays)
        {
            Inventories.Clear();

            using (var context = new MemorieDeFleursDbContext(connection))
            {
                BouquetPart = context.BouquetParts.Find(partsCode);
                if(BouquetPart == null)
                {
                    throw new NotImplementedException($"該当単品なし：{partsCode}");
                }

                var begin = date.AddDays(-numDays); // begin当日が破棄(予定)日である単品の入荷(予定)日
                var end = date.AddDays(numDays);    // begin 当日が入荷(予定)日である単品の破棄(予定)日

                // 集計対象データをDBから取得：コピーを作成し再度DB接続しなくても良いようにする
                var actions = context.InventoryActions
                    .Where(act => act.PartsCode == partsCode)
                    .Where(act => begin <= act.ActionDate && act.ActionDate <= end)
                    .ToList();

                foreach(var d in Enumerable.Range(0, numDays).Select(i => date.AddDays(i)))
                {
                    var inventryTransfar = new InventoryTransitionOfTheDay();
                    inventryTransfar.Fill(actions, BouquetPart, d);

                    Inventories.Add(d, inventryTransfar);
                }
            }
        }
    }
}
