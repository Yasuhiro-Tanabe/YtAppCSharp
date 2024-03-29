﻿using YasT.Framework.Logging;

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YasT.Framework.WPF;

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

        /// <summary>
        /// ログ出力内容
        /// </summary>
        public string LogMessage { get { return LogUtil.Appender?.Notification; } }
        #endregion // プロパティ

        public MainWindowViewModel() : base()
        {
            LogUtil.Appender.PropertyChanged += LogNotified;
            Loaded = new LoadedEventHandler(this);
            SvgImage = SVGEditorModel.Instance.RenderToImage(string.Empty);
        }
        private void LogNotified(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == nameof(LogUtil.Appender.Notification))
            {
                RaisePropertyChanged(nameof(LogMessage));
            }
        }


        #region コマンド
        public ICommand Loaded { get; }
        public OpenFileCommand Open { get; } = new OpenFileCommand();
        public RenderCommand Render { get; } = new RenderCommand();
        public SaveToFileCommand Save { get; } = new SaveToFileCommand();
        public SaveAsNewFileCommand SaveAs { get; } = new SaveAsNewFileCommand();
        public ICommand Exit { get; } = new ExitCommand();
        public IconiseCommand Iconise { get; } = new IconiseCommand();
        #endregion // コマンド



        public void LoadFile(string fileName)
        {
            SvgFileName = fileName;
            LoadFileNameSelected?.Invoke(this, new FileNameSelectedEventArgs() { FileName = fileName });
        }

        public void SaveToCurrentFile()
        {
            SaveFileNameSelected?.Invoke(this, new FileNameSelectedEventArgs() { FileName = string.Empty });
        }

        public void SaveToFile(string fileName)
        {
            SvgFileName = fileName;
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
