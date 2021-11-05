using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    public class TestOrder : SortedDictionary<DateTime, IList<OrderInfo>>
    {

        public void Append(DateTime date, int lotNo, int initial)
        {
            IList<OrderInfo> list;
            if (!TryGetValue(date, out list))
            {
                list = new List<OrderInfo>();
                Add(date, list);
            }

            var item = OrderInfo.Create(lotNo, initial);
            list.Add(item);
        }

        public void Remove(int lotNo)
        {
            foreach (var i in Values)
            {
                var found = i.SingleOrDefault(j => j.LotNo == lotNo);
                if (found != null)
                {
                    i.Remove(found);
                    return;
                }
            }
        }

        public override string ToString()
        {
            return string.Join(", ", this.Select(kv => $"[{kv.Key:yyyyMMdd}, ({string.Join(", ", kv.Value)})]"));
        }
    }
}
