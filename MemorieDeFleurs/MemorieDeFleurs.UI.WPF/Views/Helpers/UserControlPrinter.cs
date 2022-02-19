using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;

using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    /// <summary>
    /// 印刷実行クラス
    /// </summary>
    public class UserControlPrinter
    {
        /// <summary>
        /// データを指定された書式で印刷する
        /// </summary>
        /// <typeparam name="ViewModel">viewModel で渡す印刷対象データのクラス名を指定する</typeparam>
        /// <typeparam name="View">印刷書式：XAMLで記載したユーザコントロールクラスのクラス名を指定する</typeparam>
        /// <param name="viewModel">印刷対象データ</param>
        public static void PrintDocument<ViewModel, View>(ViewModel viewModel) where ViewModel:NotificationObject, IPrintable, IReloadable where View : UserControl, new()
        {
            try
            {
                var control = new View();
                LogUtil.DEBUGLOG_BeginMethod(msg: $"VM: {viewModel.GetType().Name}, V: {control.GetType().Name}");

                var w = (5.0 /* [mm] */).MmToPixcel();
                var h = (10.0 /* [mm] */).MmToPixcel();

                control.DataContext = viewModel;
                control.Margin = new Thickness(w, h, w, h);

                viewModel.ValidateBeforePrinting();
                viewModel.UpdateProperties();

                var doc = new FixedDocument();
                var page = new FixedPage();

                page.Children.Add(control);
                doc.Pages.Add(new PageContent() { Child = page });

                PrintDocumentImageableArea area = null;
                var writer = PrintQueue.CreateXpsDocumentWriter(ref area);
                if (writer == null)
                {
                    // プリンタ選択ダイアログをキャンセルで閉じると
                    // CreateXpsDocumentWriter() の戻り値が null になる。
                    // このときは印刷を行わずに終了する。
                    LogUtil.Info("No documents printed.");
                }
                else
                {
                    writer.Write(doc);
                    LogUtil.Info($"Printed: {viewModel}");
                }
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }

        }
    }
}
