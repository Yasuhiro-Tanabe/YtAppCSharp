using System.ComponentModel;

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
        protected void SetProperty<T>(ref T? variable, T value)
        {
            variable = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(variable)));
        }
    }
}
