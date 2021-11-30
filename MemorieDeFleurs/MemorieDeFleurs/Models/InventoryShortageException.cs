using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using System;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// 在庫不足例外
    /// </summary>
    public class InventoryShortageException : ApplicationException
    {
        /// <summary>
        /// 例外派生した在庫ロット・数量の情報を格納し、このままデータベースに登録可能な在庫不足アクション
        /// </summary>
        public InventoryAction InventoryShortageAction { get; private set; }

        /// <summary>
        /// 在庫不足例外を生成する
        /// </summary>
        /// <param name="action">在庫不足が発生したときの USED 在庫アクション</param>
        /// <param name="quantity"></param>
        public InventoryShortageException(InventoryAction action, int quantity)
            : base($"Inventory shortage: {action.ToString("h")}")
        {
            InventoryShortageAction = new InventoryAction()
            {
                Action = InventoryActionType.SHORTAGE,
                ActionDate = action.ActionDate,
                ArrivalDate = action.ArrivalDate,
                InventoryLotNo = action.InventoryLotNo,
                PartsCode = action.PartsCode,
                Quantity = quantity,
                Remain = -quantity
            };
        }
    }
}
