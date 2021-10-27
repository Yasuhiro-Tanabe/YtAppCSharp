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
            if(parameter != null)
            {
                var type = parameter.GetType();
                Func<object, bool> checker;
                if (Checkers.TryGetValue(type, out checker))
                {
                    return checker(parameter);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            try
            {
                if (parameter == null) { throw new NullReferenceException("parameter is null"); }

                var type = parameter.GetType();
                Action<object> action;
                if(Actions.TryGetValue(type, out action))
                {
                    action(parameter);
                }
                else if(Actions.TryGetValue(type.BaseType, out action))
                {
                    action(parameter);
                }
                else
                {
                    throw new NotImplementedException(GetType().Name);
                }
            }
            catch (ValidateFailedException ex)
            {
                PopupWarningDialog($"{string.Join("\n", ex.ValidationErrors)}", ex.Message);
            }
            catch (NotConnectedToDatabaseException ex)
            {
                PopupWarningDialog(ex.Message, $"DB未接続");
            }
            catch(Exception ex)
            {
                var message = string.Empty;
                if (ex.InnerException == null)
                {
                    message = $"{ex.GetType().Name}, {ex.Message}";
                    LogUtil.Warn($"Exception thrown: {message}\n{ex.StackTrace}");
                }
                else
                {
                    message = $"{ex.GetType().Name}, {ex.Message} => {ex.InnerException.GetType()}, {ex.InnerException.Message}";
                    LogUtil.Warn($"Exception thrown: {message}\n{ex.StackTrace}");
                }

                PopupErrorDialog(message, "システムエラー");
            }
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
                LogUtil.WarnFormat("Action overwritten: [{0}]={1}({2}) -> {3}({4})",
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
                LogUtil.WarnFormat("Validator overwritten: [{0}]={1}({2}) -> {3}({4})",
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