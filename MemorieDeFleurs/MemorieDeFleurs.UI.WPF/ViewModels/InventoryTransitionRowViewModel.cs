using System;
using System.Windows;

using static MemorieDeFleurs.Models.InventoryTransitionTable;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class InventoryTransitionRowViewModel : NotificationObject
    {
        public class InventoryRemains
        {
            internal InventoryRemains(int size) 
            {
                _remains = new int[size];
            }
            private int[] _remains;

            public int this[int i] 
            {
                get { return _remains[i]; }
                set { _remains[i] = value; }
            }
        }

        #region プロパティ
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

        public string DateText
        {
            get { return _date.ToString("yyyy.MM.dd"); }
        }

        public int Arrived
        {
            get { return _arrived; }
            set { SetProperty(ref _arrived, value); }
        }
        private int _arrived;

        public int Used
        {
            get { return _used; }
            set { SetProperty(ref _used, value); }
        }
        private int _used;

        public int Discarded
        {
            get { return _discarded; }
            set { SetProperty(ref _discarded, value); }
        }
        private int _discarded;

        public InventoryRemains Remains { get; }
        #endregion // プロパティ

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
