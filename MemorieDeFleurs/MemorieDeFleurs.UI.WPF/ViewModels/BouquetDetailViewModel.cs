using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetDetailViewModel : TabItemControlViewModelBase
    {
        public static string Name { get; } = "商品詳細";
        public BouquetDetailViewModel() : base(Name) { }

        #region プロパティ
        /// <summary>
        /// 花束コード
        /// </summary>
        public string BouquetCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        /// <summary>
        /// 花束名称
        /// </summary>
        public string BouquetName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        /// <summary>
        /// イメージファイル名
        /// </summary>
        public string ImageFileName
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }
        private string _image;

        /// <summary>
        /// 最短お届け日
        /// 
        /// 商品を構成する各単品の発注リードタイムの最大値
        /// </summary>
        public int LeadTime { get { return _leadTime; } }
        private int _leadTime;

        /// <summary>
        /// 商品構成(表示用)
        /// </summary>
        public string PartsListString { get { return string.Join(", ", _parts.Select(i => $"{i.Key} x{i.Value}")); } }

        /// <summary>
        /// 商品構成(データベース登録用)
        /// </summary>
        public IDictionary<string, int> PartsList { get { return _parts; } }
        private IDictionary<string, int> _parts = new Dictionary<string, int>();

        /// <summary>
        /// 商品構成編集中に表示するコントロールの可視性
        /// </summary>
        public Visibility EditingModeVisivility { get { return _editing ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// 商品構成編集中でないときに表示するコントロールの可視性
        /// </summary>
        public Visibility ViewModeVisivility { get { return _editing ? Visibility.Collapsed : Visibility.Visible; } }
        private bool _editing = false;

        /// <summary>
        /// 編集中の商品構成
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> SelectedPartListItem { get; } = new ObservableCollection<PartsListItemViewModel>();
        
        /// <summary>
        /// <see cref="SelectedPartListItem"/> 中の、現在選択中のオブジェクト
        /// </summary>
        public PartsListItemViewModel CurrentSelectedInPartsList
        {
            get { return _selectedInPartsList; }
            set { SetProperty(ref _selectedInPartsList, value); }
        }
        private PartsListItemViewModel _selectedInPartsList;

        /// <summary>
        /// 商品構成に追加可能な単品の一覧
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> SelectableParts { get; } = new ObservableCollection<PartsListItemViewModel>();

        public PartsListItemViewModel CurrentSelectedInSelectablePartsList
        {
            get { return _selectedInSelectableParts; }
            set { SetProperty(ref _selectedInSelectableParts, value); }
        }
        private PartsListItemViewModel _selectedInSelectableParts;
        #endregion // プロパティ

        #region コマンド
        public ICommand SelectImage { get; }
        public ICommand Edit { get; } = new EditPartsListCommand();
        public ICommand Fix { get; } = new FixPartsListCommand();
        public ICommand Append { get; } = new AddToListItemCommand();
        public ICommand Remove { get; } = new RemoveFromListItemCommand();
        public ICommand Reload { get; } = new ReloadDetailCommand();
        #endregion // コマンド

        /// <summary>
        /// 表示内容をデータベースから取得したエンティティオブジェクトの値に更新する
        /// </summary>
        /// <param name="bouquet">データベースから取得したエンティティオブジェクト</param>
        public void Update(Bouquet bouquet)
        {
            _code = bouquet.Code;
            _name = bouquet.Name;
            _image = bouquet.Image;
            _leadTime = bouquet.LeadTime;

            _parts.Clear();
            foreach(var p in bouquet.PartsList)
            {
                _parts.Add(p.PartsCode, p.Quantity);
            }
            _editing = false;

            RaisePropertyChanged(nameof(BouquetCode), nameof(BouquetName), nameof(ImageFileName), nameof(LeadTime),
                nameof(PartsListString), nameof(EditingModeVisivility), nameof(ViewModeVisivility), nameof(SelectedPartListItem), nameof(CurrentSelectedInPartsList),
                 nameof(SelectableParts), nameof(CurrentSelectedInSelectablePartsList));
        }

        /// <summary>
        /// 入力されている値を検証する。
        /// 
        /// 検証で何も問題なければ正常終了、何らかの不正値があるときは例外 <see cref="ValidateFailedException"/> がスローされる。
        /// </summary>
        public void Validate()
        {
            var result = new ValidateFailedException();

            if(string.IsNullOrWhiteSpace(_code))
            {
                result.Append("花束コードが指定されていません。");
            }
            if(string.IsNullOrWhiteSpace(_name))
            {
                result.Append("花束名称が指定されていません。");
            }
            if(string.IsNullOrWhiteSpace(_name) && !File.Exists(_image))
            {
                result.Append($"イメージファイルが見つかりません: {_image}");
            }
            if(_parts.Count == 0)
            {
                result.Append("商品構成が指定されていません。");
            }

            if(result.ValidationErrors.Count > 0) { throw result; }
        }

        public void EditPartsList()
        {
            _editing = true;
            var allParts = MemorieDeFleursUIModel.Instance.FindAllBouquetParts().Where(p => !_parts.ContainsKey(p.Code));

            SelectedPartListItem.Clear();
            foreach (var p in _parts)
            {
                SelectedPartListItem.Add(new PartsListItemViewModel() { PartsCode = p.Key, Quantity = p.Value });
            }
            CurrentSelectedInPartsList = null;

            SelectableParts.Clear();
            foreach (var p in allParts)
            {
                SelectableParts.Add(new PartsListItemViewModel() { PartsCode = p.Code, Quantity = 0 });
            }
            CurrentSelectedInSelectablePartsList = null;

            RaisePropertyChanged(nameof(EditingModeVisivility), nameof(ViewModeVisivility), nameof(SelectedPartListItem), nameof(CurrentSelectedInPartsList),
                 nameof(SelectableParts), nameof(CurrentSelectedInSelectablePartsList));
        }

        public void FixPartsList()
        {
            _editing = false;

            _parts.Clear();
            foreach (var i in SelectedPartListItem)
            {
                _parts.Add(i.PartsCode, i.Quantity);
            }

            RaisePropertyChanged(nameof(EditingModeVisivility), nameof(ViewModeVisivility), nameof(PartsListString));
        }

        public void AppendToPartsList()
        {
            var item = CurrentSelectedInSelectablePartsList;

            SelectedPartListItem.Add(item);
            CurrentSelectedInPartsList = item;

            CurrentSelectedInSelectablePartsList = null;
            SelectableParts.Remove(item);

            RaisePropertyChanged(nameof(SelectedPartListItem), nameof(CurrentSelectedInPartsList),
                 nameof(SelectableParts), nameof(CurrentSelectedInSelectablePartsList));
        }

        public void RemoveFromPartsList()
        {
            var item = CurrentSelectedInPartsList;

            SelectableParts.Add(item);
            CurrentSelectedInSelectablePartsList = item;

            CurrentSelectedInPartsList = null;
            SelectedPartListItem.Remove(item);

            RaisePropertyChanged(nameof(SelectedPartListItem), nameof(CurrentSelectedInPartsList),
                 nameof(SelectableParts), nameof(CurrentSelectedInSelectablePartsList));
        }

        public void Update()
        {
            if(string.IsNullOrWhiteSpace(BouquetCode))
            {
                MessageBox.Show("花束コードが指定されていません。");
            }
            else
            {
                var bouquet = MemorieDeFleursUIModel.Instance.FindBouquet(BouquetCode);
                if(bouquet == null)
                {
                    MessageBox.Show($"花束コードに該当する商品がありません：{BouquetCode}");
                }
                else
                {
                    Update(bouquet);
                }
            }
        }
    }
}
