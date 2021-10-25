using System;

namespace MemorieDeFleurs.UI.WPF.Model.Exceptions
{
    public class NotConnectedToDatabaseException : ApplicationException
    {
        public NotConnectedToDatabaseException() : base("データベースに接続されていません。") { }
    }
}
