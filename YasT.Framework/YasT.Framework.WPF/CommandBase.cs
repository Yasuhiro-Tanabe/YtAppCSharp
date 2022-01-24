using System;
using System.Windows.Input;

using YasT.Framework.Logging;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// <see cref="ICommand"/> インタフェースを実装するクラスの共通ベースクラス。
    /// </summary>
    public abstract class CommandBase<T> : ICommand
    {
        /// <summary>
        /// コマンド実行可否変更イベントハンドラの登録先。
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// コマンド実行イベント。コマンド実行後に発行される。
        /// </summary>
        public event EventHandler<object>? CommandExecuted;

        /// <summary>
        /// コマンド実行イベント。コマンド実行前に発行される。
        /// <para>コマンド実行を中断できる。中断するときは、イベントハンドラ内で sender 引数 (このクラスのオブジェクト) に対し
        /// <see cref="SetExecutability(bool)"/> または <see cref="ToUnexecutable()"/> を呼び出すこと。</para>
        /// </summary>
        public event EventHandler<object>? CommandExecuting;

        private bool _canExecute = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="canExecute">実行可否の初期状態</param>
        public CommandBase(bool canExecute = true)
        {
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンド実行可否を返す。
        /// <para>実行可否を動的に変更したい場合は、このメソッドを継承せず
        /// <see cref="SetExecutability(bool)"/>, <see cref="ToExecutable()"/>, <see cref="ToUnexecutable()"/>
        /// を呼び出して状態変更すること。</para>
        /// </summary>
        /// <param name="notUsed">このメソッドでは使用しない。</param>
        /// <returns>コマンド実行可否</returns>
        public bool CanExecute(object? notUsed)
        {
            return _canExecute;
        }

        /// <summary>
        /// コマンドを実行する。
        /// </summary>
        /// <param name="parameter">コマンドパラメータ</param>
        public void Execute(object? parameter)
        {
            try
            {
                if (parameter == null) { throw new ArgumentNullException(nameof(parameter)); }
                CommandExecuting?.Invoke(this, parameter);
                if (_canExecute && parameter is T)
                {
                    Execute((T)parameter);
                    CommandExecuted?.Invoke(this, parameter);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Warn(ex);
                this.PopupWarning(ex.Message); // MessageBoxExtension 参照
            }
        }

        /// <summary>
        /// コマンドを実行する。各サブクラスは、このメソッドを実装しなければならない。
        /// </summary>
        /// <param name="parameter">コマンドパラメータ</param>
        protected abstract void Execute(T parameter);

        /// <summary>
        /// コマンド実行可否状態を変更しイベントを発行する。
        /// </summary>
        /// <param name="canExecute">コマンド実行可否</param>
        public void SetExecutability(bool canExecute)
        {
            _canExecute = canExecute;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// コマンドを実行可能状態に変更する。
        /// </summary>
        public void ToExecutable()
        {
            SetExecutability(true);
        }

        /// <summary>
        /// コマンドを実行不可能状態に変更する。
        /// </summary>
        public void ToUnexecutable()
        {
            SetExecutability(false);
        }
    }
}
