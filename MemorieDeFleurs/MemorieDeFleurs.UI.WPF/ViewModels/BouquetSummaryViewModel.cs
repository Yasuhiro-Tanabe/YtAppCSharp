using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Linq;
using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// 商品一覧画面内の <see cref="ListView"/> に表示する各商品ビューモデル
    /// </summary>
    public class BouquetSummaryViewModel : ListItemViewModelBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="bouquet">表示する商品エンティティオブジェクト</param>
        public BouquetSummaryViewModel(Bouquet bouquet) : base(new OpenDetailViewCommand())
        {
            Update(bouquet);
        }

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
        /// 商品名称
        /// </summary>
        public string BouquetName
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        /// <summary>
        /// 商品イメージ (画像ファイル名)
        /// </summary>
        public string ImageFileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }
        private string _fileName;

        /// <summary>
        /// 商品構成
        /// </summary>
        public string PartsList
        {
            get { return _parts; }
            set { SetProperty(ref _parts, value); }
        }
        private string _parts;

        /// <summary>
        /// 商品リードタイム：構成各単品の発注リードタイムの最大値
        /// 
        /// 構成する単品が発送日に数量不足であっても受注は行い、受注当日に仕入先発注し数量不足を解消したい。
        /// </summary>
        public int LeadTime
        {
            get { return _lead; }
            set { SetProperty(ref _lead, value); }
        }
        private int _lead;
        #endregion // プロパティ

        /// <summary>
        /// このビューモデルのプロパティを指定された商品エンティティオブジェクトの値で更新する
        /// </summary>
        /// <param name="bouquet">更新元の商品エンティティ</param>
        public void Update(Bouquet bouquet)
        {
            BouquetCode = bouquet.Code;
            BouquetName = bouquet.Name;
            ImageFileName = bouquet.Image;
            LeadTime = bouquet.LeadTime;

            if(bouquet.PartsList.Count == 0)
            {
                PartsList = string.Empty;
            }
            else
            {
                PartsList = string.Join(", ", bouquet.PartsList.Select(p => $"{p.PartsCode} x{p.Quantity}"));
            }
        }
    }
}
