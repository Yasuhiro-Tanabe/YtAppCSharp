using System;

using YasT.Framework.WPF;

using static MemorieDeFleurs.Models.InventoryTransitionTable;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 在庫推移一日分のビューモデル
    /// </summary>
    public class InventoryTransitionRowViewModel : NotificationObject
    {
        /// <summary>
        /// 残数表示：○日前残～当日残を保持する
        /// </summary>
        public class InventoryRemains
        {
            internal InventoryRemains(int size) 
            {
                _remains = new int[size];
            }
            private int[] _remains;

            /// <summary>
            /// i 日前残を取得する
            /// </summary>
            /// <param name="i">何日前か：0=当日残、1=前日残、...</param>
            /// <returns>残数</returns>
            public int this[int i] 
            {
                get { return _remains[i]; }
                set { _remains[i] = value; }
            }
        }

        #region プロパティ
        /// <summary>
        /// 日付
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set
            {
                SetProperty(ref _date, value);
                RaisePropertyChanged(nameof(DateText));
            }
        }
        private DateTime _date;

        /// <summary>
        /// 日付 (表示用)
        /// </summary>
        public string DateText
        {
            get { return _date.ToString("yyyy.MM.dd"); }
        }

        /// <summary>
        /// 入荷予定数
        /// </summary>
        public int Arrived
        {
            get { return _arrived; }
            set { SetProperty(ref _arrived, value); }
        }
        private int _arrived;

        /// <summary>
        /// 加工予定数
        /// </summary>
        public int Used
        {
            get { return _used; }
            set { SetProperty(ref _used, value); }
        }
        private int _used;

        /// <summary>
        /// 破棄数
        /// </summary>
        public int Discarded
        {
            get { return _discarded; }
            set { SetProperty(ref _discarded, value); }
        }
        private int _discarded;

        /// <summary>
        /// 納品日別残数 (当日残～N日前残、N=品質維持可能日数)
        /// </summary>
        public InventoryRemains Remains { get; }
        #endregion // プロパティ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="date">日付</param>
        /// <param name="row">表示する在庫推移一日分の情報</param>
        /// <param name="size">残数表示の幅：品質維持可能日数</param>
        public InventoryTransitionRowViewModel(DateTime date, InventoryTransitionOfTheDay row, int size)
        {
            Date = date;
            Arrived = row.Arrived;
            Used = row.Used;
            Discarded = row.Discarded;

            Remains = new InventoryRemains(size);
            for (var i = 0; i < size; i++)
            {
                Remains[i] = row.Remains[-i];
            }

        }
    }
}
