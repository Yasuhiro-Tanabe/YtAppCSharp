using ResourceGenerator.Models;
using ResourceGenerator.Resources;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace ResourceGenerator.ViewModels
{
    /// <summary>
    /// メインウィンドウのビューモデル。
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        #region プロパティ
        /// <summary>
        /// ウィンドウタイトル。
        /// </summary>
        public string Title
        {
            get { return _title ?? ResourceFinder.FindText("ApplicationName"); }
            set { SetProperty(ref _title, value); }
        }
        private string? _title;
        /// <summary>
        /// ログ出力内容。
        /// </summary>
        public string Logs { get { return LogUtil.Appender?.Notification ?? string.Empty; } }
        /// <summary>
        /// 表示可能なタブアイテム一覧
        /// </summary>
        public ObservableCollection<TabItemControlViewModel> TabItems { get; set; } = new ObservableCollection<TabItemControlViewModel>();
        /// <summary>
        /// リソース管理モデル。
        /// </summary>
        public ResourceGenerationModel Model { get; private set; } = new ResourceGenerationModel();
        /// <summary>
        /// 表示中のタブアイテムを示すインデックス番号。
        /// </summary>
        public int SelectedIndex
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        } 
        private int _index;
        #endregion プロパティ
        #region コマンド
        #endregion コマンド

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public MainWindowViewModel()
        {
            TabItems.Add(new SettingControlViewModel(Model));
            if(LogUtil.Appender != null)
            {
                LogUtil.Appender.PropertyChanged += OnLogNotified;
            }
            Model.CodeGenerated += SampleCodeGenerated;
        }

        /// <summary>
        /// ログ通知が来たら画面に反映する。
        /// </summary>
        /// <param name="sender">イベント発行元(<see cref="LogUtil.Appender"/>オブジェクト)。</param>
        /// <param name="args">変更されたプロパティ名</param>
        private void OnLogNotified(object? sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == nameof(LogUtil.Appender.Notification))
            {
                RaisePropertyChanged(nameof(Logs));
            }
        }

        /// <summary>
        /// モデルにサンプルコードが追加されたら、対応する <see cref="SampleCodeViewModel"/> を <see cref="TabItems"/> に追加する。
        /// </summary>
        /// <param name="sender">イベント発行元(<see cref="ResourceGenerationModel"/>オブジェクト)。</param>
        /// <param name="code">追加されたサンプルコード：key=デフォルトファイル名、value=生成されたコード。</param>
        private void SampleCodeGenerated(object? sender, KeyValuePair<string, string> code)
        {
            var vm = new SampleCodeViewModel(Model, code.Key) { Code = code.Value };
            Model.CodeUpdated += vm.SampleCodeUpdated;
            vm.Close.CommandExecuted += TabItemClosed;

            TabItems.Add(vm);
        }

        /// <summary>
        /// サンプルコード表示タブアイテムがクローズされたら <see cref="TabItems"/> からビューモデルを削除する。
        /// </summary>
        /// <param name="sender">イベント発行元(各 <see cref="SampleCodeViewModel"/> オブジェクトの Close コマンド)。</param>
        /// <param name="vm">イベント発行したビューモデル。</param>
        private void TabItemClosed(object? sender, TabItemControlViewModel vm)
        {
            vm.Close.CommandExecuted -= TabItemClosed;
            var i = TabItems.IndexOf(vm);
            if(i > 0)
            {
                TabItems.RemoveAt(i);
            }
        }
    }
}
