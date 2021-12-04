using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 商品詳細画面のビューモデル
    /// </summary>
    public class BouquetDetailViewModel : DetailViewModelBase, IEditableAndFixable, IAppendableRemovable, IReloadable
    {
        /// <summary>
        /// ビューモデルの名称：<see cref="TabItemControlViewModelBase.Header"/> や <see cref="MainWindowViiewModel.FindTabItem(string)"/> に渡すクラス定数として使用する。
        /// </summary>
        public static string Name { get { return TextResourceFinder.FindText("Bouquet_Detail"); } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BouquetDetailViewModel() : base(Name) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="b">詳細表示する商品エンティティオブジェクト</param>
        public BouquetDetailViewModel(Bouquet b) : this() { Update(b); }

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
        public int LeadTime
        {
            get { return _leadTime; }
            private set { SetProperty(ref _leadTime, value); }
        }
        private int _leadTime;

        /// <summary>
        /// 商品構成
        /// </summary>
        public string PartsListText
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        private string _text;

        /// <summary>
        /// 編集中の商品構成
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> SelectedPartsList { get; } = new ObservableCollection<PartsListItemViewModel>();
        
        /// <summary>
        /// <see cref="SelectedPartsList"/> 中の、現在選択中のオブジェクト
        /// </summary>
        public PartsListItemViewModel SelectedParts
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }
        private PartsListItemViewModel _selected;

        /// <summary>
        /// 商品構成に追加可能な単品の一覧
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> CandidatePartsList { get; } = new ObservableCollection<PartsListItemViewModel>();

        /// <summary>
        /// 現在選択中の追加単品
        /// </summary>
        public PartsListItemViewModel CandidateParts
        {
            get { return _candidate; }
            set { SetProperty(ref _candidate, value); }
        }
        private PartsListItemViewModel _candidate;
        #endregion // プロパティ

        #region コマンド
        /// <summary>
        /// 画像ファイル検索ボタン押下時に実行されるコマンド
        /// </summary>
        public ICommand FindImageSource { get; } = new FindImageSourceFileCommand();
        #endregion // コマンド

        /// <summary>
        /// 表示内容をデータベースから取得したエンティティオブジェクトの値に更新する
        /// </summary>
        /// <param name="bouquet">データベースから取得したエンティティオブジェクト</param>
        public void Update(Bouquet bouquet)
        {
            BouquetCode = bouquet.Code;
            BouquetName = bouquet.Name;
            ImageFileName = bouquet.Image;
            LeadTime = bouquet.LeadTime;

            LoadSelectedPartsList(bouquet);
            LoadCandidatePartsList();
            PartsListText = string.Join(", ", SelectedPartsList.Select(i => $"{i.PartsCode} x{i.Quantity}"));
            IsEditing = false;

            IsDirty = false;
        }

        private void LoadSelectedPartsList(Bouquet bouquet)
        {
            SelectedPartsList.Clear();
            foreach (var p in bouquet.PartsList)
            {
                SelectedPartsList.Add(new PartsListItemViewModel(p));
            }
            SelectedParts = null;
            RaisePropertyChanged(nameof(SelectedPartsList));
        }

        /// <summary>
        /// 入力されている値を検証する。
        /// 
        /// 検証で何も問題なければ正常終了、何らかの不正値があるときは例外 <see cref="ValidateFailedException"/> がスローされる。
        /// </summary>
        public override void Validate()
        {
            var result = new ValidateFailedException();

            if(string.IsNullOrWhiteSpace(BouquetCode))
            {
                result.Append("花束コードが指定されていません。");
            }
            if(string.IsNullOrWhiteSpace(BouquetName))
            {
                result.Append("花束名称が指定されていません。");
            }
            if(string.IsNullOrWhiteSpace(ImageFileName) && !File.Exists(ImageFileName))
            {
                result.Append($"イメージファイルが見つかりません: {ImageFileName}");
            }
            if(SelectedPartsList.Count == 0)
            {
                result.Append("商品構成が指定されていません。");
            }

            if(result.ValidationErrors.Count > 0) { throw result; }
        }

        #region IEditableFixable
        /// <inheritdoc/>
        public EditCommand Edit { get; } = new EditCommand();

        /// <inheritdoc/>
        public FixCommand Fix { get; } = new FixCommand();

        /// <inheritdoc/>
        public bool IsEditing
        {
            get { return _editing; }
            set { SetProperty(ref _editing, value); }
        }
        private bool _editing = false;

        /// <inheritdoc/>
        public void OpenEditView()
        {
            IsEditing = true;

            LoadCandidatePartsList();
            SelectedParts = null;
        }

        /// <inheritdoc/>
        private void LoadCandidatePartsList()
        {
            CandidatePartsList.Clear();
            foreach (var p in MemorieDeFleursUIModel.Instance.FindAllBouquetParts())
            {
                if (SelectedPartsList.SingleOrDefault(i => i.PartsCode == p.Code) == null)
                {
                    // 選択中の仕入可能な単品一覧にない単品が対象
                    CandidatePartsList.Add(new PartsListItemViewModel(p));
                }
            }
            CandidateParts = null;
        }

        /// <inheritdoc/>
        public void FixEditing()
        {
            IsEditing = false;
            LeadTime = SelectedPartsList.Count() > 0 ? SelectedPartsList.Max(p => p.LeadTime) : 0;
            PartsListText = string.Join(", ", SelectedPartsList.Select(i => $"{i.PartsCode} x{i.Quantity}"));
        }
        #endregion // IEditableFixable

        #region IAddableRemovable
        /// <inheritdoc/>
        public AppendToListCommand Append { get; } = new AppendToListCommand();
        /// <inheritdoc/>
        public RemoveFromListCommand Remove { get; } = new RemoveFromListCommand();

        /// <inheritdoc/>
        public void AppendToList()
        {
            var item = CandidateParts;

            SelectedPartsList.Add(item);
            SelectedParts = item;

            CandidateParts = null;
            CandidatePartsList.Remove(item);
        }

        /// <inheritdoc/>
        public void RemoveFromList()
        {
            var item = SelectedParts;

            CandidatePartsList.Add(item);
            CandidateParts = item;

            SelectedParts = null;
            SelectedPartsList.Remove(item);
        }
        #endregion // IAddableRemovbable

        #region IReloadable
        /// <inheritdoc/>
        public ReloadCommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            if (string.IsNullOrWhiteSpace(BouquetCode))
            {
                throw new ApplicationException("花束コードが指定されていません。");
            }
            else
            {
                var bouquet = MemorieDeFleursUIModel.Instance.FindBouquet(BouquetCode);
                if (bouquet == null)
                {
                    throw new ApplicationException($"花束コードに該当する商品がありません：{BouquetCode}");
                }
                else
                {
                    Update(bouquet);
                }
            }
        }
        #endregion // IReloadable

        /// <inheritdoc/>
        public override void SaveToDatabase()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();

                Validate();

                var bouquet = new Bouquet()
                {
                    Code = BouquetCode,
                    Name = BouquetName,
                    Image = ImageFileName,
                    LeadTime = LeadTime
                };
                foreach(var parts in SelectedPartsList)
                {
                    bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = bouquet.Code, PartsCode = parts.PartsCode, Quantity = parts.Quantity });
                }

                var saved = MemorieDeFleursUIModel.Instance.Save(bouquet);

                Update(saved);
                LogUtil.Info($"Bouquet {BouquetCode} is saved.");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }

        /// <inheritdoc/>
        public override void ClearProperties()
        {
            BouquetCode = string.Empty;
            BouquetName = string.Empty;
            ImageFileName = string.Empty;
            LeadTime = 0;

            SelectedPartsList.Clear();
            CandidatePartsList.Clear();
            SelectedParts = null;
            CandidateParts = null;

            IsEditing = false;

            IsDirty = false;
        }
    }
}
