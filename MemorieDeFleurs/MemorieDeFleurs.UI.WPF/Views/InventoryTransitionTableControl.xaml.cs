using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views.Helpers;

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views
{
    /// <summary>
    /// InventoryTransactionTableControl.xaml の相互作用ロジック
    /// </summary>
    public partial class InventoryTransitionTableControl : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InventoryTransitionTableControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ビューのプロパティ (DataContext) が変更されたときに呼び出されるイベントハンドラ。
        /// </summary>
        /// <param name="sender">イベント送信元：このビュー</param>
        /// <param name="e">変更されたプロパティの名前("DataContext")や変更前後のプロパティの値</param>
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != null) { (e.OldValue as InventoryTransitionTableViewModel).PropertyChanged -= ChangeDataGridColumns; }
            if(e.NewValue != null) { (e.NewValue as InventoryTransitionTableViewModel).PropertyChanged += ChangeDataGridColumns; }
            LogUtil.DEBUGLOG_MethodCalled($"{sender?.GetType()?.Name}, {e.Property.Name}[old={e.OldValue}, new={e.NewValue}]");
        }

        /// <summary>
        /// ビューモデルの在庫推移表データが更新されたら、更新結果(品質維持可能日数)に合わせて DataGrid の列情報を更新する：
        /// 
        /// ビューの構成を変更するので、この処理はコードビハインド側に実装する。
        /// </summary>
        /// <param name="sender">イベント送信元 (InventoryTransitionTableViewModel)</param>
        /// <param name="args">イベントパラメータ：イベントが発行されたビューモデルのプロパティ名</param>
        private void ChangeDataGridColumns(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(InventoryTransitionTableViewModel.InventoryTransitions))
            {
                var vm = sender as InventoryTransitionTableViewModel;

                InventoryTransitionTable.Columns.Clear();

                var converter = new ZeroToEmptyStringConverter();
                var style = new Style();
                style.Setters.Add(new Setter() { Property = DataGridCell.HorizontalAlignmentProperty, Value = HorizontalAlignment.Right });

                InventoryTransitionTable.Columns.Add(new DataGridTextColumn() { Header = "日付", Binding = new Binding("DateText") });
                InventoryTransitionTable.Columns.Add(new DataGridTextColumn() { Header = "入荷予定数", Binding = new Binding("Arrived"), ElementStyle = style });
                InventoryTransitionTable.Columns.Add(new DataGridTextColumn() { Header = "加工予定数", Binding = new Binding("Used"), ElementStyle = style });
                for (var i = vm.ExpiryDate; i > 3; i--)
                {
                    InventoryTransitionTable.Columns.Add(new DataGridTextColumn() { Header = $"{i - 1}日前残", Binding = new Binding($"Remains[{i - 1}]"), ElementStyle = style });
                }
                if (vm.ExpiryDate > 2) { InventoryTransitionTable.Columns.Add(new DataGridTextColumn() { Header = "前々日残", Binding = new Binding("Remains[2]"), ElementStyle = style }); }
                if (vm.ExpiryDate > 1) { InventoryTransitionTable.Columns.Add(new DataGridTextColumn() { Header = "前日残", Binding = new Binding("Remains[1]"), ElementStyle = style }); }
                InventoryTransitionTable.Columns.Add(new DataGridTextColumn() { Header = "当日残", Binding = new Binding("Remains[0]"), ElementStyle = style });
                InventoryTransitionTable.Columns.Add(new DataGridTextColumn() { Header = "破棄", Binding = new Binding("Discarded") { Converter = converter }, ElementStyle = style });

                LogUtil.DEBUGLOG_MethodCalled($"{sender.GetType().Name}, {args.PropertyName}",
                    $"{vm.BouquetPartsCode}, {vm.ExpiryDate}: {string.Join(" ,", InventoryTransitionTable.Columns.Select(col => col.Header))}");
            }
        }
    }
}
