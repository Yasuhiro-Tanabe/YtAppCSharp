using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using YasT.Framework.Logging;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// ボタン押下時やイベント発行時に実行するコマンドのベースクラス
    /// </summary>
    public class CommandBase : ICommand
    {
        /// <summary>
        /// コマンドの実行可否が変わったことを通知する
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected CommandBase() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type">このコマンドが扱うコマンドパラメータのデータ型</param>
        /// <param name="action">指定データ型のコマンドパラメータを受け取ったときに実行する処理</param>
        protected CommandBase(Type type, Action<object> action)
        {
            AddAction(type, action);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="type">このコマンドが扱うコマンドパラメータのデータ型</param>
        /// <param name="action">指定データ型のコマンドパラメータを受け取ったときに実行する処理</param>
        /// <param name="checker">指定データ型のコマンドパラメータを受け取ったときに実行するコマンド実行可否判定処理</param>
        protected CommandBase(Type type, Action<object> action, Func<object, bool> checker)
        {
            AddAction(type, action);
            AddChecker(type, checker);
        }

        /// <inheritdoc/>
        public bool CanExecute(object parameter)
        {
            if(parameter == null) { return true; }

            var checker = FindChecker(parameter.GetType());
            if (checker == null)
            {
                return true;
            }
            else
            {
                return checker(parameter);
            }
        }
        private Func<object, bool> FindChecker(Type type)
        {
            if(type == null) { return null; }

            Func<object, bool> checker;
            if (Checkers.TryGetValue(type, out checker)) { return checker; }

            foreach (var i in type.GetInterfaces())
            {
                if(Checkers.TryGetValue(i, out checker)) { return checker; }
            }

            return FindChecker(type.BaseType);
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            try
            {
                if (parameter == null) { throw new NullReferenceException("parameter is null"); }

                var action = FindAction(parameter.GetType());
                if(action == null)
                {
                    throw new NotImplementedException($"{GetType().Name}.Execute({parameter.GetType().Name})");
                }
                else
                {
                    action(parameter);
                }
            }
            catch (ValidateFailedException ex)
            {
                LogUtil.Warn(ex);
                PopupWarningDialog($"{string.Join("\n", ex.ValidationErrors)}", ex.Message);
            }
            catch (NotConnectedToDatabaseException ex)
            {
                LogUtil.Warn(ex);
                PopupWarningDialog(ex.Message, TextResourceFinder.FindText("Title_DbConnectionError"));
            }
            catch(Exception ex)
            {
                LogUtil.Warn(ex);
                PopupErrorDialog(ex.InnerException == null ? ex.Message : ex.InnerException.Message, TextResourceFinder.FindText("Title_SystemError"));
            }
        }

        private Action<object> FindAction(Type type)
        {
            if(type == null) { return null; }

            Action<object> action;
            if(Actions.TryGetValue(type, out action)) { return action; }

            foreach(var i in type.GetInterfaces())
            {
                if(Actions.TryGetValue(i, out action)) { return action; }
            }

            return FindAction(type.BaseType);
        }

        private void PopupWarningDialog(string message, string header = "")
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        private void PopupErrorDialog(string message, string header = "")
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);

        }

        /// <summary>
        /// このコマンドが受け付けるコマンドパラメータのデータ型とそのデータ型のコマンドパラメータを受け取ったときに実行する処理を登録する
        /// </summary>
        /// <param name="type">このコマンドが扱うコマンドパラメータのデータ型</param>
        /// <param name="action">指定データ型のコマンドパラメータを受け取ったときに実行する処理</param>
        protected void AddAction(Type type, Action<object> action)
        {
            if(Actions.ContainsKey(type))
            {
                LogUtil.WarnFormat("[{0}] Action overwritten: view={1}, {2}({3}) -> {4}({5})",
                    GetType().Name,
                    type.Name,
                    Actions[type].Method.Name,
                    string.Join(", ", Actions[type].Method.GetParameters().Select(p => p.ParameterType.Name)),
                    action.Method.Name,
                    string.Join(", ", action.Method.GetParameters().Select(p => p.ParameterType.Name)));

                Actions[type] = action;
            }
            else
            {
                Actions.Add(type, action);
            }
        }

        /// <summary>
        /// このコマンドが受け付けるコマンドパラメータのデータ型とそのデータ型のコマンドパラメータを受け取ったときに実行するコマンド実行可否判定処理を登録する
        /// </summary>
        /// <param name="type">このコマンドが扱うコマンドパラメータのデータ型</param>
        /// <param name="checker">指定データ型のコマンドパラメータを受け取ったときに実行するコマンド実行可否判定処理</param>
        protected void AddChecker(Type type, Func<object, bool> checker)
        {
            if (Checkers.ContainsKey(type))
            {
                LogUtil.WarnFormat("[{0}] Validator overwritten: view={1}, {2}({3}) -> {4}({5})",
                    GetType().Name,
                    type.Name,
                    Checkers[type].Method.Name,
                    string.Join(", ", Checkers[type].Method.GetParameters().Select(p => p.ParameterType.Name)),
                    checker.Method.Name,
                    string.Join(", ", checker.Method.GetParameters().Select(p => p.ParameterType.Name)));
                Checkers[type] = checker;
            }
            else
            {
                Checkers.Add(type, checker);
            }
        }

        private IDictionary<Type, Action<object>> Actions { get; } = new Dictionary<Type, Action<object>>();
        private IDictionary<Type, Func<object, bool>> Checkers { get; } = new Dictionary<Type, Func<object, bool>>();

        /// <summary>
        /// コマンド実行可否変更イベントを通知する
        /// </summary>
        protected void RaiseStatusChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

    }
}