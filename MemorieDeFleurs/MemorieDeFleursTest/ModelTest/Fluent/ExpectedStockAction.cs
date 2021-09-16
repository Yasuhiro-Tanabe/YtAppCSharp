using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Text;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// 在庫アクションの期待値
    /// </summary>
    public class ExpectedStockAction
    {
        /// <summary>
        /// アクションタイプ
        /// </summary>
        public StockActionType Type { get; private set; }

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
        private ExpectedStockAction(StockActionType t, int q, int r)
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
        public static ExpectedStockAction CreateArrivedAction(int arrived)
        {
            return new ExpectedStockAction(StockActionType.SCHEDULED_TO_ARRIVE, arrived, arrived);
        }

        /// <summary>
        /// 加工予定在庫アクションの期待値を生成する
        /// </summary>
        /// <param name="used">加工数量</param>
        /// <param name="remain">当日の残数</param>
        /// <returns>加工予定在庫アクションの期待値</returns>
        public static ExpectedStockAction CreateUsedAction(int used, int remain)
        {
            return new ExpectedStockAction(StockActionType.SCHEDULED_TO_USE, used, remain);
        }

        /// <summary>
        /// 破棄予定在庫アクションの期待値を生成する
        /// </summary>
        /// <param name="discarded">破棄数量</param>
        /// <returns>破棄予定在庫アクションの期待値</returns>
        public static ExpectedStockAction CreateDiscardAction(int discarded)
        {
            return new ExpectedStockAction(StockActionType.SCHEDULED_TO_DISCARD, discarded, 0);
        }

        public static ExpectedStockAction CreateOutOfStockAction(int lacked)
        {
            return new ExpectedStockAction(StockActionType.OUT_OF_STOCK, lacked, -lacked);
        }

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

            var candidate = context.StockActions
                .Where(a => a.Action == Type)
                .Where(a => a.ActionDate == actionDate)
                .Where(a => a.PartsCode == part)
                .Where(a => a.StockLotNo == lot)
                .Where(a => a.ArrivalDate == arrivedDate);

            Assert.IsNotNull(candidate, "抽出結果が null：" + key);
            Assert.AreNotEqual(0, candidate.Count(), "該当するアクションが0件：" + key);
            Assert.AreEqual(1, candidate.Count(), $"該当するアクションが {candidate.Count()} 個ある：" + key);

            var action = candidate.SingleOrDefault();

            Assert.AreEqual(Quantity, action.Quantity, "数量不一致：" + key);
            Assert.AreEqual(Remain, action.Remain, "残数不一致：" + key);
        }
    }
}
