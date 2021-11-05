using MemorieDeFleurs.Databese.SQLite;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// 単品在庫アクション検証器
    /// </summary>
    public class PartsInventoryActionValidator
    {
        private IDictionary<Tuple<DateTime, int>, LotInventoryActionValidator> LotValidators { get; }
            = new SortedDictionary<Tuple<DateTime, int>, LotInventoryActionValidator>();

        /// <summary>
        /// この検証器の呼び出し元
        /// </summary>
        private InventoryActionValidator Parent { get; set; }

        private LotInventoryActionValidator CurrentChild { get; set; } = null;

        /// <summary>
        /// 検証器を作成する
        /// </summary>
        /// <param name="p">呼び出し元の検証器</param>
        public PartsInventoryActionValidator(InventoryActionValidator p)
        {
            Parent = p;
        }

        /// <summary>
        /// ロットの在庫アクション検証器を生成する：生成するだけで制御は移さない。
        /// </summary>
        /// <param name="arrivedDate">入荷予定日</param>
        /// <param name="index">ロット番号</param>
        /// <returns>自分自身</returns>
        public PartsInventoryActionValidator Lot(DateTime arrivedDate,  int index = 0)
        {
            LotInventoryActionValidator validator;
            var key = Tuple.Create(arrivedDate, index);
            if (!LotValidators.TryGetValue(key, out validator))
            {
                validator = new LotInventoryActionValidator(this);
                LotValidators.Add(key, validator);
            }

            CurrentChild = validator;
            return this;
        }

        /// <summary>
        /// 現在選択中のロットが在庫アクションを持たないことを明示する。
        /// 
        /// このメソッドを呼び出したときは Begin() を呼び出せない。
        /// Lot() で別のロットを割り当てること。
        /// </summary>
        /// <returns>自分自身</returns>
        public PartsInventoryActionValidator HasNoInventoryActions()
        {
            CurrentChild.HasNoInventoryActions();
            CurrentChild = null;
            return this;
        }

        /// <summary>
        /// ロット検証条件登録開始マーク：
        /// 
        /// ロットの在庫アクション検証器に制御を移す
        /// </summary>
        /// <returns>ロットの在庫アクション検証器</returns>
        public LotInventoryActionValidator BEGIN
        {
            get
            {
                if (CurrentChild == null)
                {
                    throw new InvalidOperationException($"Call {nameof(Lot)}() before calling {nameof(BEGIN)}().");
                }
                return CurrentChild;
            }
        }

        /// <summary>
        /// ロット検証条件登録終了マーク：
        /// 
        /// 呼び出し元の在庫アクション検証器に制御を戻す
        /// </summary>
        /// <returns>在庫アクション検証器</returns>
        public InventoryActionValidator END { get { return Parent; } }

        /// <summary>
        /// データベース上の在庫アクションのうちこの検証器に登録されている各ロットの在庫アクションが、
        /// 期待値通りに登録されているかどうかを検証する
        /// </summary>
        /// <param name="context">検証対象データベース</param>
        /// <param name="partsCode">検証対象の単品</param>
        public void AssertAll(MemorieDeFleursDbContext context, string partsCode)
        {
            LotValidators.All(kv => { kv.Value.AssertAll(context, partsCode, kv.Key.Item1, kv.Key.Item2); return true; });
        }
    }
}
