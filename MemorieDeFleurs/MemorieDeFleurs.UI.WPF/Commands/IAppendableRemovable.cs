using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// リスト間でリストアイテムを移動させる処理のインタフェース。
    /// 
    /// 実装するにあたり、以下の①～③を別途定義すること。
    /// ①[ViewModel]リスト要素を保持する2つの <see cref="ObservableCollection{T}"/>、
    /// 
    /// ②[View]①の内容を表示する2つの <see cref="ItemsControl"/> (<see cref="ListBox"/>, <see cref="ListView"/> etc.)、
    /// 
    /// ③[View]②の間で要素を移動する <see cref="Button"/>：
    /// それぞれこのインタフェースで実装する <see cref="Append"/> と <see cref="Remove"/> にバインドする
    /// 
    /// ②の <see cref="ItemsControl"/> A, B どちらかを基準として、
    /// 一方から他方 (A → B)へ Item を移動する際は Append コマンドを、
    /// 逆方向 (B → A) へ Item を移動する際は Remove コマンドを実行する。
    /// </summary>
    public interface IAppendableRemovable
    {
        /// <summary>
        /// <see cref="ObservableCollection{T}"/> の要素を一方の他方へ移動する
        /// </summary>
        public AppendToListCommand Append { get; }

        /// <summary>
        /// <see cref="ObservableCollection{T}"/> の要素を <see cref="Append"/> とは逆方向へ移動する
        /// </summary>
        public RemoveFromListCommand Remove { get; }

        /// <summary>
        /// <see cref="Append"/> が実行されたときに呼び出される処理を記述する
        /// </summary>
        public void AppendToList();

        /// <summary>
        /// <see cref="Remove"/> が呼び出されたときの処理を記述する
        /// </summary>
        public void RemoveFromList();
    }
}
