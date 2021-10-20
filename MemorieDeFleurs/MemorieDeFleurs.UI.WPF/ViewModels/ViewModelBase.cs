using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string name = "")
        {
            property = value;
            RaisePropertyChanged(name);
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach(var name in propertyNames)
            {
                if(!string.IsNullOrWhiteSpace(name))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        protected void SetProperty(Action action, [CallerMemberName] string name="")
        {
            action();
            RaisePropertyChanged(name);
        }
    }
}
