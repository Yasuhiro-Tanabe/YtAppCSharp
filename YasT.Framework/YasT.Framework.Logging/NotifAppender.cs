using log4net.Appender;
using log4net.Core;

using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace YasT.Framework.Logging
{
    /// <summary>
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントハンドラを通じて購読しているクラスにログ出力の変更を通知するログアペンダ
    /// </summary>
    public class NotifyAppender : AppenderSkeleton, INotifyPropertyChanged

    {
        /// <summary>
        /// ログ出力が変更されたことを通知する
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        private static string _notification = "";
        /// <summary>
        /// 通知された文字列＝ログ出力内容。
        /// これまでに通知されたログ文言を含む最新ログを出力する。
        /// </summary>
        public string Notification
        {
            get { return _notification; }
            private set
            {
                if (_notification != value)
                {
                    _notification = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));
                }
            }
        }

        /// <summary>
        /// ログ出力を追加する。Log4Net内部処理で使用する。
        /// </summary>
        /// <param name="theEvent"></param>
        protected override void Append(LoggingEvent theEvent)
        {
            var writer = new StringWriter(CultureInfo.CurrentCulture);
            Layout.Format(writer, theEvent);
            Notification += writer.ToString();
        }

        /// <summary>
        /// 通知されたログをクリアする
        /// </summary>
        public void Clear()
        {
            Notification = "";
        }
    }
}
