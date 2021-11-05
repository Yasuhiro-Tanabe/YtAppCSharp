using MemorieDeFleurs.Models.Entities;

using System;

namespace MemorieDeFleurs.Models
{
    public static class EntityExtensions
    {
        public static bool CheckAndModify(this Customer inDB, Customer newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.EmailAddress == newValue.EmailAddress, () => inDB.EmailAddress = newValue.EmailAddress)
                || CheckAndModify(() => inDB.Password == newValue.Password, () => inDB.Password = newValue.Password)
                || CheckAndModify(() => inDB.CardNo == newValue.CardNo, () => inDB.CardNo = newValue.CardNo);
        }

        public static bool CheckAndModify(this ShippingAddress inDB, ShippingAddress newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.Address1 == newValue.Address1, () => inDB.Address1 = newValue.Address1)
                || CheckAndModify(() => inDB.Address2 == newValue.Address2, () => inDB.Address2 = newValue.Address2)
                || CheckAndModify(() => inDB.LatestOrderDate == newValue.LatestOrderDate, () => inDB.LatestOrderDate = newValue.LatestOrderDate);
        }

        public static bool CheckAndModify(this Supplier inDB, Supplier newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.Address1 == newValue.Address1, () => inDB.Address1 = newValue.Address1)
                || CheckAndModify(() => inDB.Address2 == newValue.Address2, () => inDB.Address2 = newValue.Address2)
                || CheckAndModify(() => inDB.EmailAddress == newValue.EmailAddress, () => inDB.EmailAddress = newValue.EmailAddress)
                || CheckAndModify(() => inDB.Telephone == newValue.Telephone, () => inDB.Telephone = newValue.Telephone)
                || CheckAndModify(() => inDB.Fax == newValue.Fax, () => inDB.Fax = newValue.Fax);
        }

        public static bool CheckAndModify(this BouquetPart inDB, BouquetPart newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.QuantitiesPerLot == newValue.QuantitiesPerLot, () => inDB.QuantitiesPerLot = newValue.QuantitiesPerLot)
                || CheckAndModify(() => inDB.LeadTime == newValue.LeadTime, () => inDB.LeadTime = newValue.LeadTime)
                || CheckAndModify(() => inDB.ExpiryDate == newValue.ExpiryDate, () => inDB.ExpiryDate = newValue.ExpiryDate);
        }

        public static bool CheckAndModify(this Bouquet inDB, Bouquet newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.Image == newValue.Image, () => inDB.Image = newValue.Image)
                || CheckAndModify(() => inDB.LeadTime == newValue.LeadTime, () => inDB.LeadTime = newValue.LeadTime);
        }

        public static bool CheckAndModify(this BouquetPartsList inDB, BouquetPartsList newValue)
        {
            return CheckAndModify(() => inDB.Quantity == newValue.Quantity, () => inDB.Quantity = newValue.Quantity);
        }

        /// <summary>
        /// 変更チェック：姑息ではあるが、同じロジックが大量発生してコードが縦に長大となるのでif文をメソッド化する
        /// 
        /// プロパティの各型に対して CheckAndModify(ref T property, T value) とか CheckModify(this T variable, T value) が実装できなかったのでその代替策。
        /// </summary>
        /// <param name="isSame">判定文</param>
        /// <param name="modify">代入文</param>
        /// <returns>判定結果：代入が実行されたかどうか</returns>
        private static bool CheckAndModify(Func<bool> isSame, Action modify)
        {
            if (isSame())
            {
                return false;
            }
            else
            {
                modify();
                return true;
            }
        }
    }
}
