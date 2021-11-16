using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;

using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    public class UserControlPrinter
    {
        /// <summary>
        /// ユーザーコントロールで表示している内容を印刷する
        /// </summary>
        /// <typeparam name="T">印刷に使用するユーザーコントロール：画面に表示しているのと同じものを使用可能</typeparam>
        /// <param name="viewModel">ユーザーコントロールのDataContextとして使用するオブジェクト</param>
        /// <param name="updateViewModel">(省略可能) ユーザーコントロールにDataContext 設定後に行う処理：RaisePropertyChanged() 呼出など、必要に応じて指定する</param>
        /// <returns></returns>
        public static bool PrintDocument<T>(NotificationObject viewModel,Action updateViewModel = null) where T : UserControl, new()
        {
            var w = (5.0 /* [mm] */).MmToPixcel();
            var h = (10.0 /* [mm] */).MmToPixcel();

            var control = new T();
            control.DataContext = viewModel;
            control.Margin = new Thickness(w, h, w, h);

            updateViewModel?.Invoke();

            var doc = new FixedDocument();
            var page = new FixedPage();

            page.Children.Add(control);
            doc.Pages.Add(new PageContent() { Child = page });

            PrintDocumentImageableArea area = null;
            var writer = PrintQueue.CreateXpsDocumentWriter(ref area);
            if(writer == null)
            {
                // プリンタ選択ダイアログをキャンセルで閉じると
                // CreateXpsDocumentWriter() の戻り値が null になる。
                // このときは印刷を行わずに終了する。
                return false;
            }
            else
            {
                writer.Write(doc);
                return true;
            }
        }
    }
}
