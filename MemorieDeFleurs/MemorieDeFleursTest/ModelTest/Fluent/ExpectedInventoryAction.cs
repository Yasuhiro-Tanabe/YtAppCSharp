using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// 在庫アクションの期待値
    /// </summary>
    public class ExpectedInventoryAction
    {
        /// <summary>
        /// アクションタイプ
        /// </summary>
        public InventoryActionType Type { get; private set; }

        /// <summary>
        /// 数量(入荷数・使用数・破棄数)の期待値
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// アクション後の残数の期待値
        /// </summary>
        public int Remain { get; private set; }

        /// <summary>
        /// 在庫アクションの期待値を生成する
        /// </summary>
        /// <param name="t">在庫アクションタイプ</param>
        /// <param name="q">数量</param>
        /// <param name="r">残数</param>
        private ExpectedInventoryAction(InventoryActionType t, int q, int r)
        {
            Type = t;
            Quantity = q;
            Remain = r;
        }

        /// <summary>
        /// 入荷予定在庫アクションの期待値を生成する
        /// </summary>
        /// <param name="arrived">入荷数量</param>
        /// <returns>入荷予定在庫アクションの期待値</returns>
        public static ExpectedInventoryAction CreateArrivedAction(int arrived)
        {
            return new ExpectedInventoryAction(InventoryActionType.SCHEDULED_TO_ARRIVE, arrived, arrived);
        }

        /// <summary>
        /// 加工予定在庫アクションの期待値を生成する
        /// </summary>
        /// <param name="used">加工数量</param>
        /// <param name="remain">当日の残数</param>
        /// <returns>加工予定在庫アクションの期待値</returns>
        public static ExpectedInventoryAction CreateUsedAction(int used, int remain)
        {
            return new ExpectedInventoryAction(InventoryActionType.SCHEDULED_TO_USE, used, remain);
        }

        /// <summary>
        /// 破棄予定在庫アクションの期待値を生成する
        /// </summary>
        /// <param name="discarded">破棄数量</param>
        /// <returns>破棄予定在庫アクションの期待値</returns>
        public static ExpectedInventoryAction CreateDiscardAction(int discarded)
        {
            return new ExpectedInventoryAction(InventoryActionType.SCHEDULED_TO_DISCARD, discarded, 0);
        }

        public static ExpectedInventoryAction CreateInventoryShortageAction(int lacked)
        {
            return new ExpectedInventoryAction(InventoryActionType.SHORTAGE, lacked, -lacked);
        }

        IDictionary<InventoryActionType, InventoryActionType> AnotherType = new SortedDictionary<InventoryActionType, InventoryActionType>()
            {
                { InventoryActionType.SCHEDULED_TO_ARRIVE, InventoryActionType.ARRIVED },
                { InventoryActionType.SCHEDULED_TO_DISCARD, InventoryActionType.DISCARDED },
                { InventoryActionType.SCHEDULED_TO_USE, InventoryActionType.USED },
                { InventoryActionType.SHORTAGE, InventoryActionType.SHORTAGE }
            };

        /// <summary>
        /// 特定の１在庫アクションが、数量や残数も含めすべて意図通り登録されているかどうかを検証する
        /// </summary>
        /// <param name="context">データベースコンテキスト</param>
        /// <param name="actionDate">基準日</param>
        /// <param name="part">花コード</param>
        /// <param name="lot">在庫ロット番号</param>
        /// <param name="arrivedDate">入荷(予定)日</param>
        public void AssertExists(MemorieDeFleursDbContext context, DateTime actionDate, string part, int lot, DateTime arrivedDate)
        {
            var key = new StringBuilder()
                .AppendFormat("基準日={0:yyyyMMdd}", actionDate)
                .Append(", アクション=").Append(Type)
                .Append(", 花コード=").Append(part)
                .Append(", 在庫ロット番号=").Append(lot)
                .AppendFormat(", 入荷日={0:yyyyMMdd}", arrivedDate)
                .ToString();

            var candidate = context.InventoryActions
                .Where(a => a.Action == Type || a.Action == AnotherType[Type])
                .Where(a => a.ActionDate == actionDate)
                .Where(a => a.PartsCode == part)
                .Where(a => a.InventoryLotNo == lot)
                .Where(a => a.ArrivalDate == arrivedDate);

            Assert.IsNotNull(candidate, "抽出結果が null：" + key);
            Assert.AreNotEqual(0, candidate.Count(), "該当するアクションが0件：" + key);
            AssertActionCount(candidate, Type, key);
            AssertActionCount(candidate, AnotherType[Type], key);
            AssertActionQuantity(Quantity, candidate, key);
            AssertActionRemain(Remain, candidate, key);
        }

        private void AssertActionCount(IQueryable<InventoryAction> candidate, InventoryActionType type, string key)
        {
            var count = candidate.Count(act => act.Action == type);
            if (count > 1)
            {
                Assert.Fail($"{type} アクションが {count} 個ある：{key}");
            }
        }

        private void AssertActionQuantity(int expected, IQueryable<InventoryAction> candidate, string key)
        {
            var actual = candidate.Sum(act => act.Quantity);
            Assert.AreEqual(expected, actual, $"数量不一致：{key}");
        }

        private void AssertActionRemain(int expected, IQueryable<InventoryAction> candidate, string key)
        {
            var action = candidate.SingleOrDefault(act => act.Action == Type);
            if(action == null)
            {
                action = candidate.SingleOrDefault(act => act.Action == AnotherType[Type]);
            }

            Assert.AreEqual(expected, action.Remain, $"残数不一致；{key}");
        }
    }
}
