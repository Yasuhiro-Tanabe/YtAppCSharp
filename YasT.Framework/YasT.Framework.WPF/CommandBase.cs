using System;
using System.Windows.Input;

using YasT.Framework.Logging;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// <see cref="ICommand"/> インタフェースを実装する、コマンドクラスの共通ベースクラス。
    /// </summary>
    public abstract class CommandBase<T> : ICommand
    {
        /// <summary>
        /// コマンド実行可否変更イベント。コマンド実行可否が変更されたときに発行される。
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// コマンド実行イベント。コマンド実行後に発行される。
        /// </summary>
        public event EventHandler<T>? CommandExecuted;

        /// <summary>
        /// コマンド実行イベント。コマンド実行前に発行される。
        /// <para>コマンド実行を中断できる。中断するときは、イベントハンドラ内で sender 引数 (このクラスのオブジェクト) に対し
        /// <see cref="SetExecutability(bool)"/> または <see cref="ToUnexecutable()"/> を呼び出すこと。</para>
        /// </summary>
        public event EventHandler<T>? CommandExecuting;

        /// <summary>
        /// コマンド実行失敗イベント。コマンド実行中に例外がスローされたときに発行される。
        /// </summary>
        public event EventHandler<CommandFailedEventArgs<T>>? CommandFailed;

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
        /// ユーザ定義のコマンド処理：<see cref="Execute(T)"/>を実行する。
        /// 
        /// ユーザー定義の処理実行前に <see cref="CommandExecuting"/> イベントが、
        /// ユーザー定義の処理実行後に <see cref="CommandExecuted"/> イベントが発行される。
        /// またコマンド実行エラー(例外)発生時は、
        /// <b>警告ダイアログを表示した後で</b> <see cref="CommandFailed"/> イベントを発行する。
        /// </summary>
        /// <param name="parameter">コマンドパラメータ</param>
        public void Execute(object? parameter)
        {
            try
            {
                if (parameter == null) { throw new ArgumentNullException(nameof(parameter), "Command parameter should not be null in this framework."); }
                CommandExecuting?.Invoke(this, (T)parameter);
                if (_canExecute && parameter is T)
                {
                    Execute((T)parameter);
                    CommandExecuted?.Invoke(this, (T)parameter);
                }
                else if(parameter is not T)
                {
                    // parameter が T 型かそのサブクラスでなかったときは、コマンド実行可否に寄らず例外をスローする
                    throw new ArgumentException($"Command parameter type mismatch: expected {typeof(T).FullName}, but {parameter?.GetType().FullName}");
                }
            }
            catch (Exception ex)
            {
                LogUtil.Warn(ex);
                this.PopupWarning(ex.Message); // MessageBoxExtension 参照
                CommandFailed?.Invoke(this, new CommandFailedEventArgs<T>((T?)parameter, ex));
            }
        }

        /// <summary>
        /// コマンドを実行する。各サブクラスは、このメソッドを実装しなければならない。
        /// </summary>
        /// <param name="parameter">コマンドパラメータ</param>
        protected abstract void Execute(T parameter);

        /// <summary>
        /// コマンド実行可否状態を変更し <see cref="CanExecuteChanged"/> イベントを発行する。
        /// </summary>
        /// <param name="canExecute">コマンド実行可否</param>
        public void SetExecutability(bool canExecute)
        {
            _canExecute = canExecute;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// コマンドを実行可能状態に変更し <see cref="CanExecuteChanged"/> イベントを発行する。
        /// 
        /// <seealso cref="SetExecutability(bool)"/>
        /// </summary>
        public void ToExecutable()
        {
            SetExecutability(true);
        }

        /// <summary>
        /// コマンドを実行不可能状態に変更し <see cref="CanExecuteChanged"/> イベントを発行する。
        /// 
        /// <seealso cref="SetExecutability(bool)"/>
        /// </summary>
        public void ToUnexecutable()
        {
            SetExecutability(false);
        }
    }
}
