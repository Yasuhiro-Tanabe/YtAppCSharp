using YasT.Framework.Logging;

using System;
using System.Windows;

namespace SVGEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        internal void LoadFile(object sender, FileNameSelectedEventArgs args)
        {
            if(string.IsNullOrWhiteSpace(args.FileName))
            {
                PopupError("ファイル名が指定されていません。");
            }
            else
            {
                Editor.Document.FileName = args.FileName;
                Editor.Load(args.FileName);
                (DataContext as MainWindowViewModel).UpdateImage(Editor.Text);
                LogUtil.InfoFormat("File {0} loaded.", args.FileName);

            }
        }

        internal void SaveFile(object sender, FileNameSelectedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.FileName))
            {
                if (string.IsNullOrWhiteSpace(Editor.Document.FileName))
                {
                    PopupError("ファイル名が指定されていません。");
                }
                else
                {
                    Editor.Save(Editor.Document.FileName);
                    LogUtil.InfoFormat("Document saved to file: {0}", Editor.Document.FileName);
                }
            }
            else
            {
                Editor.Save(args.FileName);
                LogUtil.InfoFormat("Document saved to file: {0}", args.FileName);
            }
        }

        internal void UpdateImage(object sender, EventArgs unused)
        {
            (DataContext as MainWindowViewModel).UpdateImage(Editor.Text);
        }

        private void PopupError(string msg)
        {
            MessageBox.Show(msg, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
