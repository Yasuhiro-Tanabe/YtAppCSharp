using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SVGEditor
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// ウィンドウタイトル共通部
        /// </summary>
        private static string WINDOW_TITLE = "SVGEditor";

        /// <summary>
        /// ウィンドウタイトル
        /// </summary>
        public string WindowTitle { get { return _title; } set { SetProperty(ref _title, value); } }
        private string _title = WINDOW_TITLE;

        /// <summary>
        /// 現在表示中の SVG ファイル
        /// </summary>
        public string SvgFileName
        {
            get { return _svgFileName; }
            set
            {
                SetProperty(ref _svgFileName, value);
                WindowTitle = $"{WINDOW_TITLE}: {Path.GetFileName(_svgFileName)}";
            }
        }
        private string _svgFileName;

        /// <summary>
        /// 現在表示中の SVG ソースコード
        /// </summary>
        public string SourceCode
        {
            get { return _sourceCode; }
            set { SetProperty(ref _sourceCode, value); }
        }
        private string _sourceCode;

        public OpenFileCommand Open { get; } = new OpenFileCommand();
        public RenderCommand Render { get; } = new RenderCommand();
        public SaveToFileCommand Save { get; } = new SaveToFileCommand();
        public SaveAsNewFileCommand SaveAs { get; } = new SaveAsNewFileCommand();

    }
}
