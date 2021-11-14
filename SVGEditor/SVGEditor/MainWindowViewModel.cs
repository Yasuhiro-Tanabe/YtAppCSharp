using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SVGEditor
{
    internal class MainWindowViewModel : NotificationObject
    {
        #region イベント
        public event EventHandler<FileNameSelectedEventArgs> LoadFileNameSelected;
        public event EventHandler<FileNameSelectedEventArgs> SaveFileNameSelected;
        public event EventHandler RenderCalled;
        #endregion // イベント

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
            Loaded = new LoadedEventHandler(this);
            SvgImage = SVGEditorModel.Instance.RenderToImage(string.Empty);
        }

        #region コマンド
        public ICommand Loaded { get; }
        public OpenFileCommand Open { get; } = new OpenFileCommand();
        public RenderCommand Render { get; } = new RenderCommand();
        public SaveToFileCommand Save { get; } = new SaveToFileCommand();
        public SaveAsNewFileCommand SaveAs { get; } = new SaveAsNewFileCommand();
        #endregion // コマンド



        public void LoadFile(string fileName)
        {
            LoadFileNameSelected?.Invoke(this, new FileNameSelectedEventArgs() { FileName = fileName });
        }

        public void SaveToCurrentFile()
        {
            SaveFileNameSelected?.Invoke(this, new FileNameSelectedEventArgs() { FileName = string.Empty });
        }

        public void SaveToFile(string fileName)
        {
            SaveFileNameSelected?.Invoke(this, new FileNameSelectedEventArgs() { FileName = fileName });
        }

        public void Reload()
        {
            RenderCalled?.Invoke(this, null);
        }

        public void UpdateImage(string svgCode)
        {
            SvgImage = SVGEditorModel.Instance.RenderToImage(svgCode);
        }
    }
}
