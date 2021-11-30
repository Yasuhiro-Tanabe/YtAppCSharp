using MemorieDeFleurs.Models.Entities;

using Microsoft.EntityFrameworkCore;

using System;

namespace MemorieDeFleurs.Models
{
    /// <summary>
    /// 花束問題で作成する各エンティティオブジェクトの、データ変更有無検証用の拡張メソッド
    /// 
    /// 各メソッドは以下のロジックで呼び出す：
    /// <list type="number">
    /// <item>データベースから newValue のキー情報を使って inDB を取得する</item>
    /// <item>拡張メソッドを呼出し、newValue と inDB の再検出および inDB の内容更新</item>
    /// <item>拡張メソッドの戻り値が真だったら <see cref="DbContext.Update(object)"/> ないし同等のメソッドを呼びだし、データベースにある inDB の内容を更新する</item>
    /// </list>
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// 得意先エンティティオブジェクトの各プロパティが変更されたかをチェックし、差異があるときはそれを反映する
        /// </summary>
        /// <param name="inDB">データベースに登録されている現在のエンティティ</param>
        /// <param name="newValue">画面操作等の反映により差異があるかもしれないエンティティ</param>
        /// <returns>inDB と newValue のプロパティに一つでも差異があれば真、すべて同じだったら偽
        /// 
        /// 各プロパティ毎に差異があるとわかった時点で inDB の内容を newValue の値で変更するので、
        /// メソッドが呼び出し元に復帰後は newValue の値は inDB に反映済</returns>
        public static bool CheckAndModify(this Customer inDB, Customer newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.EmailAddress == newValue.EmailAddress, () => inDB.EmailAddress = newValue.EmailAddress)
                || CheckAndModify(() => inDB.Password == newValue.Password, () => inDB.Password = newValue.Password)
                || CheckAndModify(() => inDB.CardNo == newValue.CardNo, () => inDB.CardNo = newValue.CardNo);
        }

        /// <summary>
        /// お届け先エンティティオブジェクトの各プロパティが変更されたかをチェックし、差異があるときはそれを反映する
        /// </summary>
        /// <param name="inDB">データベースに登録されている現在のエンティティ</param>
        /// <param name="newValue">画面操作等の反映により差異があるかもしれないエンティティ</param>
        /// <returns>inDB と newValue のプロパティに一つでも差異があれば真、すべて同じだったら偽
        /// 
        /// 各プロパティ毎に差異があるとわかった時点で inDB の内容を newValue の値で変更するので、
        /// メソッドが呼び出し元に復帰後は newValue の値は inDB に反映済</returns>
        public static bool CheckAndModify(this ShippingAddress inDB, ShippingAddress newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.Address1 == newValue.Address1, () => inDB.Address1 = newValue.Address1)
                || CheckAndModify(() => inDB.Address2 == newValue.Address2, () => inDB.Address2 = newValue.Address2)
                || CheckAndModify(() => inDB.LatestOrderDate == newValue.LatestOrderDate, () => inDB.LatestOrderDate = newValue.LatestOrderDate);
        }

        /// <summary>
        /// お届け先エンティティオブジェクト間に差異があるかどうかだけをチェックする
        /// </summary>
        /// <param name="inDB">データベースに登録されている現在のエンティティ</param>
        /// <param name="newValue">画面操作等の反映により差異があるかもしれないエンティティ</param>
        /// <returns>inDB と newValue のプロパティに一つでも差異があれば真、すべて同じだったら偽</returns>
        public static bool IsModified(this ShippingAddress inDB, ShippingAddress newValue)
        {
            return inDB.Name != newValue.Name
                || inDB.Address1 != newValue.Address1
                || inDB.Address2 != newValue.Address2
                || inDB.LatestOrderDate != newValue.LatestOrderDate;
        }

        /// <summary>
        /// 仕入先エンティティオブジェクトの各プロパティが変更されたかをチェックし、差異があるときはそれを反映する
        /// </summary>
        /// <param name="inDB">データベースに登録されている現在のエンティティ</param>
        /// <param name="newValue">画面操作等の反映により差異があるかもしれないエンティティ</param>
        /// <returns>inDB と newValue のプロパティに一つでも差異があれば真、すべて同じだったら偽
        /// 
        /// 各プロパティ毎に差異があるとわかった時点で inDB の内容を newValue の値で変更するので、
        /// メソッドが呼び出し元に復帰後は newValue の値は inDB に反映済</returns>
        public static bool CheckAndModify(this Supplier inDB, Supplier newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.Address1 == newValue.Address1, () => inDB.Address1 = newValue.Address1)
                || CheckAndModify(() => inDB.Address2 == newValue.Address2, () => inDB.Address2 = newValue.Address2)
                || CheckAndModify(() => inDB.EmailAddress == newValue.EmailAddress, () => inDB.EmailAddress = newValue.EmailAddress)
                || CheckAndModify(() => inDB.Telephone == newValue.Telephone, () => inDB.Telephone = newValue.Telephone)
                || CheckAndModify(() => inDB.Fax == newValue.Fax, () => inDB.Fax = newValue.Fax);
        }

        /// <summary>
        /// 単品エンティティオブジェクトの各プロパティが変更されたかをチェックし、差異があるときはそれを反映する
        /// </summary>
        /// <param name="inDB">データベースに登録されている現在のエンティティ</param>
        /// <param name="newValue">画面操作等の反映により差異があるかもしれないエンティティ</param>
        /// <returns>inDB と newValue のプロパティに一つでも差異があれば真、すべて同じだったら偽
        /// 
        /// 各プロパティ毎に差異があるとわかった時点で inDB の内容を newValue の値で変更するので、
        /// メソッドが呼び出し元に復帰後は newValue の値は inDB に反映済</returns>
        public static bool CheckAndModify(this BouquetPart inDB, BouquetPart newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.QuantitiesPerLot == newValue.QuantitiesPerLot, () => inDB.QuantitiesPerLot = newValue.QuantitiesPerLot)
                || CheckAndModify(() => inDB.LeadTime == newValue.LeadTime, () => inDB.LeadTime = newValue.LeadTime)
                || CheckAndModify(() => inDB.ExpiryDate == newValue.ExpiryDate, () => inDB.ExpiryDate = newValue.ExpiryDate);
        }

        /// <summary>
        /// 商品エンティティオブジェクトの各プロパティが変更されたかをチェックし、差異があるときはそれを反映する
        /// </summary>
        /// <param name="inDB">データベースに登録されている現在のエンティティ</param>
        /// <param name="newValue">画面操作等の反映により差異があるかもしれないエンティティ</param>
        /// <returns>inDB と newValue のプロパティに一つでも差異があれば真、すべて同じだったら偽
        /// 
        /// 各プロパティ毎に差異があるとわかった時点で inDB の内容を newValue の値で変更するので、
        /// メソッドが呼び出し元に復帰後は newValue の値は inDB に反映済</returns>
        public static bool CheckAndModify(this Bouquet inDB, Bouquet newValue)
        {
            return CheckAndModify(() => inDB.Name == newValue.Name, () => inDB.Name = newValue.Name)
                || CheckAndModify(() => inDB.Image == newValue.Image, () => inDB.Image = newValue.Image)
                || CheckAndModify(() => inDB.LeadTime == newValue.LeadTime, () => inDB.LeadTime = newValue.LeadTime);
        }

        /// <summary>
        /// 商品構成エンティティオブジェクトの各プロパティが変更されたかをチェックし、差異があるときはそれを反映する
        /// </summary>
        /// <param name="inDB">データベースに登録されている現在のエンティティ</param>
        /// <param name="newValue">画面操作等の反映により差異があるかもしれないエンティティ</param>
        /// <returns>inDB と newValue のプロパティに一つでも差異があれば真、すべて同じだったら偽
        /// 
        /// 各プロパティ毎に差異があるとわかった時点で inDB の内容を newValue の値で変更するので、
        /// メソッドが呼び出し元に復帰後は newValue の値は inDB に反映済</returns>
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
