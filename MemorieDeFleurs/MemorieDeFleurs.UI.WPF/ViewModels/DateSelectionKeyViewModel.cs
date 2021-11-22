using MemorieDeFleurs.UI.WPF.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
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
