using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelTest.Fluent
{
    public class OrderInfo
    {
        public OrderInfo(int lotNo, int initial)
        {
            LotNo = lotNo;
            InitialQuantity = initial;
        }

        public int LotNo { get; private set; }
        public int InitialQuantity { get; private set; }

        public static OrderInfo Create(int lotNo, int initial)
        {
            return new OrderInfo(lotNo, initial);
        }

    }
}
