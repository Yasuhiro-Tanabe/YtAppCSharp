using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemorieDeFleurs.UI.WPF
{
    /// <summary>
    /// プロパティ変更通知を発行するオブジェクトの共通ベースクラス
    /// </summary>
    public class NotificationObject : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティの値が変更されたことを通知する
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティを設定しプロパティ変更通知を発行する
        /// </summary>
        /// <typeparam name="T">プロパティのデータ型</typeparam>
        /// <param name="property">プロパティの値格納先オブジェクト</param>
        /// <param name="value">プロパティの値</param>
        /// <param name="name">プロパティ名：通常は省略する</param>
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string name = "")
        {
            property = value;
            RaisePropertyChanged(name);
        }

        /// <summary>
        /// 特定のプロパティに対するプロパティ変更通知を発行する
        /// 
        /// 指定されたプロパティのセッター以外の場所でプロパティーの値変更イベントを通知する場合はこのメソッドを呼び出す。
        /// </summary>
        /// <param name="propertyNames">変更するプロパティ名、複数指定可能</param>
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
    }
}
