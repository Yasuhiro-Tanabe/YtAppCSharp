using log4net.Appender;
using log4net.Core;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDLGenerator.Models.Logging
{
    /// <summary>
    /// Log4Net 用、UIコントロール用の簡易アペンダ。
    /// 
    /// どのコントロールにでも出力できるよう、プロパティ変更イベントの通知を使う
    /// </summary>
    public class NotifyAppender : AppenderSkeleton, INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static string _notification = "";
        /// <summary>
        /// 通知された文字列＝ログ出力内容。
        /// これまでに通知されたログ文言を含む最新ログを出力する。
        /// 文言は無限に増え続けるので、適宜クリアが必要。
        /// </summary>
        public string Notification
        {
            get { return _notification; }
            private set
            {
                if(_notification != value )
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
