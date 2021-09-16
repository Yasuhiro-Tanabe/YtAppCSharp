using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DDLGenerator.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティを更新しイベントを発行する
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="variable">プロパティの値を格納する変数名</param>
        /// <param name="value">プロパティの値</param>
        /// <param name="caller">プロパティ名、省略可。省略した場合は呼び出し元 setter をもつプロパティ名。</param>
        protected void SetProperty<T>(ref T variable, T value, [CallerMemberName] string caller = "")
        {
            variable = value;
            RaisePropertyChanged(caller);
        }

        /// <summary>
        /// <see cref="PropertyChanged"/> イベントを発行する
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
