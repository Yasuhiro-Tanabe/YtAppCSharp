using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SVGEditor
{
    internal class MainWindowViewModel : NotificationObject
    {
        #region 定数
        /// <summary>
        /// ウィンドウタイトル共通部
        /// </summary>
        private static string WINDOW_TITLE = "SVGEditor";

        #endregion // 定数

        #region プロパティ
        /// <summary>
        /// ウィンドウタイトル
        /// </summary>
        public string WindowTitle
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
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
        public string SvgCode
        {
            get { return _sourceCode; }
            set { SetProperty(ref _sourceCode, value); }
        }
        private string _sourceCode;

        /// <summary>
        /// 現在表示中のSVGイメージ
        /// </summary>
        public BitmapImage SvgImage
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }
        private BitmapImage _image;
        #endregion // プロパティ

        public MainWindowViewModel() : base()
        {
            SVGEditorModel.Instance.PropertyChanged += OnModelChanged;
        }

        #region コマンド
        public OpenFileCommand Open { get; } = new OpenFileCommand();
        public RenderCommand Render { get; } = new RenderCommand();
        public SaveToFileCommand Save { get; } = new SaveToFileCommand();
        public SaveAsNewFileCommand SaveAs { get; } = new SaveAsNewFileCommand();
        #endregion // コマンド

        private void OnModelChanged(object sender, PropertyChangedEventArgs args)
        {
            var model = sender as SVGEditorModel;
            
            if(args.PropertyName == nameof(model.SvgCode))
            {
                if(SvgCode != model.SvgCode) { SvgCode = model.SvgCode; }
            }
            else if(args.PropertyName == nameof(model.SvgImage))
            {
                SvgImage = Convert(model.SvgImage);
            }
        }

        private BitmapImage Convert(MemoryStream stream)
        {
            var im = new BitmapImage();
            im.BeginInit();
            im.CacheOption = BitmapCacheOption.OnLoad;
            im.StreamSource = stream;
            im.EndInit();

            return im;
        }
    }
}
