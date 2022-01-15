using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 単品一覧画面内の <see cref="ListView"/> に表示する単品のビューモデル
    /// </summary>
    public class BouquetPartsSummaryViewModel : ListItemViewModelBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="part"></param>
        public BouquetPartsSummaryViewModel(BouquetPart part) : base(new OpenDetailViewCommand())
        {
            Update(part);
        }

        /// <summary>
        /// 指定された単品の内容にこのビューモデルのプロパティを更新する
        /// </summary>
        /// <param name="part">更新元の単品エンティティ</param>
        public void Update(BouquetPart part)
        {
            PartsCode = part.Code;
            PartsName = part.Name;
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
        /// 単品名称
        /// </summary>
        public string PartsName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;
        #endregion // プロパティ
    }
}
