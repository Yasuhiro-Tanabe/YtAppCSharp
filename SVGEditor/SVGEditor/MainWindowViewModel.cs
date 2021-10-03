using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SVGEditor
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            RaisePropertyChanged(propertyName);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// ウィンドウタイトル共通部
        /// </summary>
        private static string WINDOW_TITLE = "SVGEditor";
        private static string NoImageFile = "./no_images.png";

        private SVGEditorModel Model { get; } = new SVGEditorModel();

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

        public ImageSource ImageSource { get { return RenderImage(); } }

        private BitmapImage RenderImage()
        {
            var bmp = string.IsNullOrWhiteSpace(SourceCode)
                ? Bitmap.FromFile(NoImageFile)
                : Model.Render(SourceCode);

            var stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Bmp);
            stream.Seek(0, SeekOrigin.Begin);

            var im = new BitmapImage();
            im.BeginInit();
            im.CacheOption = BitmapCacheOption.OnLoad;
            im.StreamSource = stream;
            im.EndInit();

            return im;
        }

        public void RefreshImage()
        {
            RaisePropertyChanged(nameof(ImageSource));
        }
    }
}
