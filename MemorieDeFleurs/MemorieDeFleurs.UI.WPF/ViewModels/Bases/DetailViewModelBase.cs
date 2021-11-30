using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases

{
    /// <summary>
    /// 詳細画面(タブ要素画面)ビューモデルの共通ベースクラス
    /// </summary>
    public class DetailViewModelBase : TabItemControlViewModelBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="header"></param>
        protected DetailViewModelBase(string header) : base(header) { }

        #region コマンド
        /// <summary>
        /// 内部で保持している値を初期化する
        /// </summary>
        public ICommand Clear { get; } = new ClearDetailCommand();
        #endregion // コマンド

        /// <summary>
        /// ビューモデルで保持している値がDB登録可能かどうか検証する
        /// </summary>
        /// <exception cref="NotImplementedException">検証処理が実装されていない：実装忘れ防止用</exception>
        public virtual void Validate() { throw new NotImplementedException($"{GetType().Name}.{nameof(Validate)}()"); }

        /// <summary>
        /// ビューモデルで保持している値をDB登録する
        /// </summary>
        /// <exception cref="NotImplementedException">DB登録処理が実装されていない：実装忘れ防止用</exception>
        public virtual void SaveToDatabase() { throw new NotImplementedException($"{GetType().Name}.{nameof(SaveToDatabase)}()"); }

        /// <summary>
        /// ビューモデルで現在保持している値を破棄し初期値に戻す
        /// </summary>
        /// <exception cref="NotImplementedException">初期化処理が実装されていない：実装忘れ防止用</exception>
        public virtual void ClearProperties() { throw new NotImplementedException($"{GetType().Name}.{nameof(ClearProperties)}()"); }
    }
}
