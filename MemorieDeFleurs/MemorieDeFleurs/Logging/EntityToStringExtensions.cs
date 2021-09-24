using MemorieDeFleurs.Models.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.Logging
{
    public static class EntityToStringExtensions
    {
        public static string ToString(this InventoryAction action, string format = "")
        {
            if(string.IsNullOrWhiteSpace(format))
            {
                return action.ToString();
            }
            else if(format == "s")
            {
                return new StringBuilder()
                    .Append(action.PartsCode)
                    .AppendFormat(".Lot{0}[{1:yyyyMMdd}]", action.InventoryLotNo, action.ActionDate)
                    .AppendFormat("(Q={0},R={1})", action.Quantity, action.Remain)
                    .ToString();
            }
            else if(format == "L")
            {
                return  new StringBuilder()
                    .Append(action.Action).Append("[")
                    .AppendFormat("Parts={0}", action.PartsCode)
                    .AppendFormat(", ArrivalDate={0:yyyyMMdd}", action.ArrivalDate)
                    .AppendFormat(", Lot={0}", action.InventoryLotNo)
                    .AppendFormat(", ActionDate={0:yyyyMMdd}", action.ActionDate)
                    .AppendFormat(", Quality={0}", action.Quantity)
                    .AppendFormat(", Remain={0}", action.Remain)
                    .ToString();
            }
            else if(format == "h")
            {
                return $"{action.PartsCode}.Lot{action.InventoryLotNo}[{action.ActionDate.ToString("yyyyMMdd")}]";
            }
            else
            {
                return action.ToString();
            }
        }
    }
}
