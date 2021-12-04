using MemorieDeFleurs.UI.WPF.Views.Helpers;

using System;

namespace MemorieDeFleurs.UI.WPF.Model.Exceptions
{
    /// <summary>
    /// データベース未接続例外
    /// </summary>
    public class NotConnectedToDatabaseException : ApplicationException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NotConnectedToDatabaseException() : base(TextResourceFinder.FindText("Message_NotConnectedToDb")) { }
    }
}
