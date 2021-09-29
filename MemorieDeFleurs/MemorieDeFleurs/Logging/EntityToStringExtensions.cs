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
        /// <summary>
        /// 在庫アクションを特定の書式で文字列化する
        /// 
        /// 書式毎の出力内容は下記。
        /// <code>
        /// var action = new InventoryAction() {
        ///     Action = InventoryActionType.SCHEDULED_TO_USE,
        ///     ActionDate = new DateTime(2020, 5, 3),
        ///     ArrivalDate = new DateTime(2020, 5, 5),
        ///     InventoryLotNo = 1,
        ///     PartsCode = "BA001",
        ///     Quantity = 0,
        ///     Remain = 100
        /// };
        /// 
        /// action.ToString("s");  // BA001.Lot1[20200505](Q=0,R=100)
        /// action.ToString("L");  // SCHEDULED_TO_USE[Parts=BA001, ArrivalDate=20200503, Lot=1, ActionDate=20200505, Quality=0, Remain=100]
        /// action.ToString("h");  // BA001.Lot1[20200505]
        /// action.ToString("o");  // Lot1, BA001 x 0, arrive at 20200503
        /// action.ToStirng("DB"); // 20200505|SCHEDULED_TO_USE    |BA001|20200503|     0|   100
        /// action.ToString("x");  // s, L, h, o 以外の書式を指定した場合、ArguemtException がスローされる
        /// action.ToString(" ");  // 空白類を指定した場合も、上記同様 ArguemtException がスローされる
        /// action.ToString("");   // ArgumentNullException がスローされる
        /// action.ToString();     // ArgumentNullException がスローされる
        /// action.ToString(null); // ArgumentNullException がスローされる
        /// </code>
        /// 
        /// <list type="">
        ///     <item>
        ///         <term>"s"</term>
        ///         <description>短い書式：花コード、ロット番号、基準日、数量、残数を含む文字列</description>
        ///     </item>
        ///     <item>
        ///         <term>"L"</term>
        ///         <description>長い書式：短い書式の出力内容に加えて、在庫アクションタイプと納品日(納品予定日)を含む文字列</description>
        ///     </item>
        ///     <item>
        ///         <term>"h"</term>
        ///         <description>ヘッダー書式：花コード、ロット番号、基準日のみを含む文字列</description>
        ///     </item>
        ///     <item>
        ///         <term>"o"</term>
        ///         <description>発注パラメータ書式：花コード、ロット番号、数量、納品予定日のみを含む文字列</description>
        ///     </item>
        ///     <item>
        ///         <term>"DB"</term>
        ///         <description>長い書式の内容を、DB風の区切り文字と空白文字で固定長に整形した文字列</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="action">対象在庫アクション</param>
        /// <param name="format">書式：s,L,h,o のいずれか。各書式は説明欄参照。</param>
        /// <returns>書式に従って在庫アクションを出力した文字列</returns>
        public static string ToString(this InventoryAction action, string format = "")
        {
            if(string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException("null or white space is not supported.");
            }
            else if(format == "s")
            {
                return new StringBuilder()
                    .Append(action.ToString("h"))
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
            else if(format == "o")
            {
                return new StringBuilder()
                    .AppendFormat("Lot{0}", action.InventoryLotNo)
                    .AppendFormat(", {0} x {1}", action.PartsCode, action.Quantity)
                    .AppendFormat(", arrive at {0:yyyyMMdd}", action.ArrivalDate)
                    .ToString();
            }
            else if(format == "DB")
            {
                return new StringBuilder()
                    .AppendFormat("{0:yyyyMMdd}", action.ActionDate)
                    .AppendFormat("|{0, -20}", action.Action)
                    .AppendFormat("|{0, 5}", action.PartsCode)
                    .AppendFormat("|{0:yyyyMMdd}", action.ArrivalDate)
                    .AppendFormat("|{0, 2}", action.InventoryLotNo)
                    .AppendFormat("|{0, 5}|{1, 5}", action.Quantity, action.Remain)
                    .ToString();
            }
            else
            {
                throw new ArgumentException($"Invalid argument: {format}");
            }
        }
    }
}
