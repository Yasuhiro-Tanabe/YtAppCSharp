using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// 日別在庫アクション検証
    /// </summary>
    public class DateInventoryActionValidator
    {
        private static DateTime INVALID_DATE = DateTime.MinValue;
        private DateTime CurrentDate { get; set; } = INVALID_DATE;

        private IList<ExpectedInventoryAction> ExpectedActions { get; } = new List<ExpectedInventoryAction>();
        private IDictionary<DateTime, ISet<InventoryActionType>> ExistActionTypes { get; } = new Dictionary<DateTime, ISet<InventoryActionType>>();
        private IDictionary<DateTime, ISet<InventoryActionType>> NotExistActionTypes { get; } = new Dictionary<DateTime, ISet<InventoryActionType>>();

        /// <summary>
        /// この検証器の呼び出し元
        /// </summary>
        private LotInventoryActionValidator Parent { get; set; }

        /// <summary>
        /// 検証器を作成する
        /// </summary>
        /// <param name="p">呼び出し元の検証器</param>
        public DateInventoryActionValidator(LotInventoryActionValidator p, DateTime d)
        {
            Parent = p;
            CurrentDate = d;
        }

        /// <summary>
        /// 入荷予定の期待値を登録する
        /// </summary>
        /// <param name="arrived">入荷本数</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator Arrived(int arrived)
        {
            ExpectedActions.Add(ExpectedInventoryAction.CreateArrivedAction(arrived));
            return this;
        }

        /// <summary>
        /// 使用予定の期待値を登録する
        /// </summary>
        /// <param name="used">使用量</param>
        /// <param name="remain">残数</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator Used(int used, int remain)
        {
            ExpectedActions.Add(ExpectedInventoryAction.CreateUsedAction(used, remain));
            return this;
        }

        /// <summary>
        /// 在庫不足の期待値を登録する
        /// </summary>
        /// <param name="shortage">在庫不足量</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator Shortage(int shortage)
        {
            ExpectedActions.Add(ExpectedInventoryAction.CreateInventoryShortageAction(shortage));
            return this;
        }

        /// <summary>
        /// 破棄予定の期待値を登録する
        /// </summary>
        /// <param name="discarded">破棄数</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator Discarded(int discarded)
        {
            ExpectedActions.Add(ExpectedInventoryAction.CreateDiscardAction(discarded));
            return this;
        }

        /// <summary>
        /// 日別在庫検証項目登録終了マーク：
        /// 
        /// 呼び出し元のロットの在庫アクション検証器に制御を戻す
        /// </summary>
        /// <returns>ロットの在庫アクション検証器</returns>
        public PartsInventoryActionValidator END {
            get {
                CurrentDate = INVALID_DATE;
                return Parent.END;
            }
        }

        public DateInventoryActionValidator At(DateTime date)
        {
            return Parent.At(date);
        }

        /// <summary>
        /// 特定の在庫アクションタイプが存在することを確認する
        /// 
        /// 同一基準日に同じアクションタイプのエンティティは1つしかない。
        /// </summary>
        /// <param name="type">在庫アクションタイプ</param>
        /// <returns>自分自身</returns>
        public DateInventoryActionValidator ContainsActionType(InventoryActionType type)
        {
            if(NotExistActionTypes.Any(kv => kv.Key == CurrentDate && kv.Value.Contains(type)))
            {
                throw new ArgumentException($"この日存在しないアクションタイプとして登録済み：{type}");
            }

            ISet<InventoryActionType> set;
            if(!ExistActionTypes.TryGetValue(CurrentDate, out set))
            {
                set = new HashSet<InventoryActionType>();
                ExistActionTypes.Add(CurrentDate, set);
            }

            if(!set.Contains(type))
            {
                set.Add(type);
            }

            return this;
        }

        public DateInventoryActionValidator NotContainsActionType(InventoryActionType type)
        {
            if (ExistActionTypes.Any(kv => kv.Key == CurrentDate && kv.Value.Contains(type)))
            {
                throw new ArgumentException($"この日存在するアクションタイプとして登録済み：{type}");
            }

            ISet<InventoryActionType> set;
            if(!NotExistActionTypes.TryGetValue(CurrentDate, out set))
            {
                set = new HashSet<InventoryActionType>();
                NotExistActionTypes.Add(CurrentDate, set);
            }

            if(!set.Contains(type))
            {
                set.Add(type);
            }

            return this;
        }

        /// <summary>
        /// データベース上の在庫アクションのうちこの検証器に登録されている各基準日の在庫アクションが、
        /// 期待値通りに登録されているかどうかを検証する
        /// </summary>
        /// <param name="context">検証対象データベース</param>
        /// <param name="partsCode">対象単品の花コード</param>
        /// <param name="arrivedDate">対象ロットの入荷予定日</param>
        /// <param name="lotNo">対象ロットのロット番号</param>
        public void AssertAll(MemorieDeFleursDbContext context, string partsCode, DateTime arrivedDate, int lotNo, DateTime actionDate)
        {
            ExpectedActions.All(a => { a.AssertExists(context, actionDate, partsCode, lotNo, arrivedDate); return true; });

            ISet<InventoryActionType> set;
            if(ExistActionTypes.TryGetValue(actionDate, out set))
            {
                foreach(var type in set)
                {
                    var actual = context.InventoryActions
                        .Where(act => act.PartsCode == partsCode)
                        .Where(act => act.InventoryLotNo == lotNo)
                        .Where(act => act.ArrivalDate == arrivedDate)
                        .Where(act => act.ActionDate == actionDate)
                        .Count(act => act.Action == type);
                    Assert.AreEqual(1, actual, $"在庫アクションは1件だけ存在するはず：ActionDate={actionDate:yyyyMMdd}, {set}, {partsCode}.Lot{lotNo}, ArrivalDate={arrivedDate:yyyyMMdd}");
                }
            }

            set = null;
            if(NotExistActionTypes.TryGetValue(actionDate, out set))
            {
                foreach(var type in set)
                {
                    var actual = context.InventoryActions
                        .Where(act => act.PartsCode == partsCode)
                        .Where(act => act.InventoryLotNo == lotNo)
                        .Where(act => act.ArrivalDate == arrivedDate)
                        .Where(act => act.ActionDate == actionDate)
                        .Count(act => act.Action == type);
                    Assert.AreEqual(0, actual, $"在庫アクションは存在しないはず：ActionDate={actionDate:yyyyMMdd}, {type}, {partsCode}.Lot{lotNo}, ArrivalDate={arrivedDate:yyyyMMdd}");
                }
            }
        }
    }
}
