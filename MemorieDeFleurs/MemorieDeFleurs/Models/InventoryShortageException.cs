using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;

using System;

namespace MemorieDeFleurs.Models
{
    public class InventoryShortageException : ApplicationException
    {
        public InventoryAction InventoryShortageAction { get; private set; }

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
