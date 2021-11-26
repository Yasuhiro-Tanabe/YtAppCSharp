namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class InventorySummaryViewModel : NotificationObject
    {
        public InventorySummaryViewModel(string code, int quantity)
        {
            PartsCode = code;
            InitialQuantity = quantity;
            DiscardQuantity = 0;
        }

        #region プロパティ
        /// <summary>
        /// 花コード
        /// </summary>
        public string PartsCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        /// <summary>
        /// 在庫数量の初期値
        /// </summary>
        public int InitialQuantity
        {
            get { return _initial; }
            set { SetProperty(ref _initial, value); }
        }
        private int _initial;

        /// <summary>
        /// 値変更後の在庫数量
        /// </summary>
        public int DiscardQuantity
        {
            get { return _discard; }
            set {
                SetProperty(ref _discard, value);
                IsChaned = _discard > 0;
            }
        }
        private int _discard;

        /// <summary>
        /// この単品のビューが現在選択中かどうか
        /// </summary>
        public bool IsSelected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }
        private bool _selected;

        /// <summary>
        /// 在庫数量の変更があったかどうか
        /// </summary>
        public bool IsChaned
        {
            get { return _changed; }
            set { SetProperty(ref _changed, value); }
        }
        private bool _changed;
        #endregion // プロパティ


    }
}
