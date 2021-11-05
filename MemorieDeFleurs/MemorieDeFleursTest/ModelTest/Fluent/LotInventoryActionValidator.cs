using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    /// <summary>
    /// ロットの在庫アクション検証器
    /// </summary>
    public class LotInventoryActionValidator
    {
        private IDictionary<DateTime, DateInventoryActionValidator> DateValidators { get; }
            = new SortedDictionary<DateTime, DateInventoryActionValidator>();

        /// <summary>
        /// この検証器の呼び出し元
        /// </summary>
        private PartsInventoryActionValidator Parent { get; set; } = null;

        private DateInventoryActionValidator CurrentChild { get; set; } = null;

        private bool InventoryActionNotExists { get; set; } = false;

        /// <summary>
        /// このロットが在庫アクション中に存在しないことを指定する
        /// </summary>
        /// <returns>自分自身</returns>
        public LotInventoryActionValidator HasNoInventoryActions()
        {
            InventoryActionNotExists = true;
            return this;
        }


        /// <summary>
        /// 検証器を作成する
        /// </summary>
        /// <param name="p">呼び出し元の検証器</param>
        public LotInventoryActionValidator(PartsInventoryActionValidator p)
        {
            Parent = p;
        }

        /// <summary>
        /// 現在登録しようとしている日別在庫アクションの基準日
        /// </summary>
        private DateTime CurrentActionDate { get; set; }

        /// <summary>
        /// 日別在庫アクション検証器を生成し制御を移す
        /// </summary>
        /// <param name="actionDate">基準日</param>
        /// <returns>日別在庫アクション検証器</returns>
        public DateInventoryActionValidator At(DateTime actionDate)
        {
            DateInventoryActionValidator validator;
            if (!DateValidators.TryGetValue(actionDate, out validator))
            {
                validator = new DateInventoryActionValidator(this, actionDate);
                DateValidators.Add(actionDate, validator);
            }

            CurrentActionDate = actionDate;
            CurrentChild = validator;
            return CurrentChild;
        }

        /// <summary>
        /// 在庫ロット番号の期待値
        /// </summary>
        private int CurrentLotNo { get; set; } = 0;

        /// <summary>
        /// 在庫ロット番号の期待値を指定する
        /// </summary>
        /// <param name="lotNo">ロット番号の期待値</param>
        /// <returns>自分自身</returns>
        public LotInventoryActionValidator LotNumberIs(int lotNo)
        {
            CurrentLotNo = lotNo;
            return this;
        }


        /// <summary>
        /// 日別在庫検証項目登録終了マーク：
        /// 
        /// 呼び出し元の単品在庫アクション検証器に制御を戻す
        /// </summary>
        /// <returns>単品在庫アクション検証器</returns>
        public PartsInventoryActionValidator END { get { return Parent; } }

        /// <summary>
        /// データベース上の在庫アクションのうちこの検証器に登録されている各基準日の在庫アクションが、
        /// 期待値通りに登録されているかどうかを検証する
        /// </summary>
        /// <param name="context">検証対象データベース</param>
        /// <param name="partsCode">対象単品の花コード</param>
        /// <param name="arrivedDate">対象ロットの入荷予定日</param>
        /// <param name="index">入荷予定日の入荷ロットが複数ある場合、その中での順番(0, 1, 2, ...)</param>
        public void AssertAll(MemorieDeFleursDbContext context, string partsCode, DateTime arrivedDate, int index)
        {
            if (InventoryActionNotExists)
            {
                if(CurrentLotNo == 0)
                {
                    Assert.Fail($"ロット番号未指定： 花コード {partsCode}, 入荷日 {arrivedDate:yyyyMMdd}");
                }

                var actual = context.InventoryActions
                    .Where(act => act.PartsCode == partsCode)
                    .Where(act => act.ArrivalDate == arrivedDate)
                    .Count(act => act.InventoryLotNo == CurrentLotNo);
                Assert.AreEqual(0, actual,
                    $"このロットの在庫アクションは存在しないはず： LotNo={CurrentLotNo} (part={partsCode}, arrived={arrivedDate.ToString("yyyyMMdd")})");
            }
            else
            {
                if (CurrentLotNo == 0)
                {
                    var lots = context.InventoryActions
                        .Where(act => act.PartsCode == partsCode)
                        .Where(act => act.ArrivalDate == arrivedDate)
                        .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE || act.Action == InventoryActionType.ARRIVED)
                        .OrderBy(act => act.InventoryLotNo).ToArray();
                    CurrentLotNo = lots[index].InventoryLotNo;
                }

                DateValidators.All(kv => { kv.Value.AssertAll(context, partsCode, arrivedDate, CurrentLotNo, kv.Key); return true; });
            }
        }

    }
}
