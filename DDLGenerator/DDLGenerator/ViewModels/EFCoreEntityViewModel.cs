using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DDLGenerator.ViewModels
{
    public class EFCoreEntityViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティを更新しイベントを発行する
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="variable">プロパティの値を格納する変数名</param>
        /// <param name="value">プロパティの値</param>
        /// <param name="caller">プロパティ名、省略可。省略した場合は呼び出し元 setter をもつプロパティ名。</param>
        private void SetProperty<T>(ref T variable, T value, [CallerMemberName] string caller = "")
        {
            variable = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

    }
}
