using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected CommandBase() { }

        protected CommandBase(Type type, Action<object> action)
        {
            AddAction(type, action);
        }

        protected CommandBase(Type type, Action<object> action, Func<object, bool> cheker)
        {
            AddAction(type, action);
            AddChecker(type, cheker);
        }

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

            return FindChecker(type.BaseType);
        }

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
                PopupWarningDialog(ex.Message, $"DB未接続");
            }
            catch(Exception ex)
            {
                LogUtil.Warn(ex);
                PopupErrorDialog(ex.InnerException == null ? ex.Message : ex.InnerException.Message, "システムエラー");
            }
        }

        private Action<object> FindAction(Type type)
        {
            if(type == null) { return null; }

            Action<object> action;
            if(Actions.TryGetValue(type, out action)) { return action; }

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

        protected void RaiseStatusChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

    }
}