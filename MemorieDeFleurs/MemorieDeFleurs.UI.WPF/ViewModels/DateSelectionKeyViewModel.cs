using MemorieDeFleurs.UI.WPF.Model;

using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 日付区分(受発注日、入荷予定日、出荷予定日などの種別)を表示する際のビューモデル
    /// 
    /// コード上区別しやすい列挙型 <see cref="DateSelectionKey"/> の値と
    /// <see cref="ComboBox"/> 等の選択肢として表示する際の表示文字列を持たせることで、
    /// 画面表示時の選択・視認容易性とコード上の使いやすさを両立させる。
    /// </summary>
    public class DateSelectionKeyViewModel : NotificationObject
    {
        #region プロパティ
        /// <summary>
        /// 日付選択方法
        /// </summary>
        public DateSelectionKey Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }
        private DateSelectionKey _key;

        /// <summary>
        /// 表示用文字列
        /// </summary>
        public string ContentText
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        private string _text;
        #endregion // プロパティ
    }
}
