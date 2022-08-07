using ResourceGenerator.Commands;
using ResourceGenerator.Models;
using ResourceGenerator.Resources;

using System.Collections.Generic;
using System.Windows.Input;

namespace ResourceGenerator.ViewModels
{
    /// <summary>
    /// サンプルコード表示画面のビューモデル。
    /// </summary>
    public class SampleCodeViewModel : TabItemControlViewModel
    {
        #region プロパティ
        /// <summary>
        /// 表示するコード。
        /// </summary>
        public string Code
        {
            get { return _code ?? string.Empty; }
            set { SetProperty(ref _code, value); }
        }
        private string? _code;
        /// <summary>
        /// デフォルトファイル名。
        /// </summary>
        public string DefaultFileName
        {
            get { return _fname ?? string.Empty; }
            set { SetProperty(ref _fname, value); }
        }
        private string? _fname;
        #endregion プロパティ
        #region コマンド
        /// <summary>
        /// サンプルコードをファイル保存する。
        /// </summary>
        public ICommand Save { get; } = new SaveSampleCodeCommand(); 
        #endregion コマンド

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="model">呼び出し元で生成したリソース管理モデル。</param>
        /// <param name="fname">サンプルコードのデフォルトファイル名。</param>
        public SampleCodeViewModel(ResourceGenerationModel model, string fname) : base(model, ResourceFinder.FindText("Sample_Header", fname))
        {
            DefaultFileName = fname;
        }

        /// <summary>
        /// XAML エディタ用コンストラクタ。
        /// </summary>
        internal SampleCodeViewModel() : base(new ResourceGenerationModel(), "") { }

        /// <summary>
        /// 自分が表示しているサンプルコードが更新されたとき、その内容を反映する。
        /// </summary>
        /// <param name="sender">イベント送信元(<see cref="ResourceGenerationModel"/> オブジェクト)。</param>
        /// <param name="code">更新されたコード：key=デフォルトファイル名、value=更新後のコード。</param>
        public void SampleCodeUpdated(object? sender, KeyValuePair<string, string> code)
        {
            if (code.Key == DefaultFileName)
            {
                // 自分が表示しているサンプルコードだったときだけ変更する。
                Code= code.Value;
            }
        }
    }
}
