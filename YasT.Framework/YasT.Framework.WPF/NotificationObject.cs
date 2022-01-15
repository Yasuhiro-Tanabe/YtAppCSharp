using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// <see cref="INotifyPropertyChanged"/> インタフェースを実装するクラスの共通ベースクラス。
    /// </summary>
    public class NotificationObject : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ変更イベントハンドラの登録先。
        /// 
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティを変更しイベントを発行する。
        /// </summary>
        /// <typeparam name="T">プロパティのデータ型</typeparam>
        /// <param name="variable">プロパティを格納する変数</param>
        /// <param name="value">変更するプロパティの値</param>
        /// <param name="name">(通常は省略)このメソッドを呼び出したプロパティのプロパティ名</param>
        protected void SetProperty<T>(ref T? variable, T value, [CallerMemberName] string name = "")
        {
            variable = value;
            RaisePropertyChanged(name);
        }

        /// <summary>
        /// プロパティ変更イベントを発行する。
        /// </summary>
        /// <param name="name">イベント発行対象のプロパティ名</param>
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
