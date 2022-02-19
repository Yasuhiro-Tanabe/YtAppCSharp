using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// <see cref="CommandBase{T}.CommandFailed"/> イベントのイベント引数。
    /// <see cref="CommandBase{T}.Execute(T)"/> が例外をスローしたとき、このイベントが発行される。
    /// </summary>
    public class CommandFailedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// キャッチした例外
        /// </summary>
        public Exception Cause { get; private set; }

        /// <summary>
        /// コマンドパラメータ
        /// </summary>
        public T? CommandParameter { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="param">コマンドパラメータ</param>
        /// <param name="cause">キャッチした例外</param>
        public CommandFailedEventArgs(T? param, Exception cause)
        {
            Cause = cause;
            CommandParameter = param;
        }
    }
}
