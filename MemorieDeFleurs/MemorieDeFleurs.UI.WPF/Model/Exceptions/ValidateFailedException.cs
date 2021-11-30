using System;
using System.Collections.Generic;

namespace MemorieDeFleurs.UI.WPF.Model.Exceptions
{
    /// <summary>
    /// 画面入力等の検証エラーが発生したときにスローされる例外
    /// </summary>
    public class ValidateFailedException : ApplicationException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ValidateFailedException() : base("検証に失敗しました。") { }

        /// <summary>
        /// 検証中に見つかったエラーの一覧：アプリケーション例外の一覧
        /// </summary>
        public IReadOnlyList<ApplicationException> ValidationErrors { get { return _errors.AsReadOnly(); } }

        private List<ApplicationException> _errors = new List<ApplicationException>();

        /// <summary>
        /// 検証エラーメッセージを追加する
        /// </summary>
        /// <param name="msg">エラーメッセージ</param>
        public void Append(string msg)
        {
            _errors.Add(new ApplicationException(msg));
        }
    }
}
