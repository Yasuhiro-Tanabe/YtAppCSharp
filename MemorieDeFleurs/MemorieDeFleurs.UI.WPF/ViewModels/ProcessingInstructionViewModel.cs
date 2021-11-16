using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;
using MemorieDeFleurs.UI.WPF.Views;
using MemorieDeFleurs.UI.WPF.Views.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class ProcessingInstructionViewModel : DetailViewModelBase, IPrintable
    {
        public ProcessingInstructionViewModel() : base("加工指示書") { }

        #region プロパティ
        /// <summary>
        /// 加工日
        /// </summary>
        public DateTime ProcessingDate
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }
        private DateTime _date = DateTime.Today;

        /// <summary>
        /// 商品一覧
        /// </summary>
        public ObservableCollection<BouquetSummaryViewModel> Bouquets { get; } = new ObservableCollection<BouquetSummaryViewModel>();

        /// <summary>
        /// 現在選択中の商品
        /// </summary>
        public BouquetSummaryViewModel SelectedBouquet
        {
            get { return _bouquet; }
            set { SetProperty(ref _bouquet, value); }
        }
        private BouquetSummaryViewModel _bouquet;

        /// <summary>
        /// 現在選択中の商品の花コード
        /// </summary>
        public string SelectedBouquetCode
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        /// <summary>
        /// 加工数 (花束の数)
        /// </summary>
        public int NumberOfBouquet
        {
            get { return _numProcess; }
            set { SetProperty(ref _numProcess, value); }
        }
        private int _numProcess;

        /// <summary>
        /// 商品構成
        /// </summary>
        public ObservableCollection<PartsListItemViewModel> Parts { get; } = new ObservableCollection<PartsListItemViewModel>();
        #endregion // プロパティ

        #region コマンド
        public ICommand Print { get; } = new PrintCommand();
        #endregion // コマンド

        public override void Update()
        {
            if(SelectedBouquet == null)
            {
                Cleanup();
            }
            else
            {
                var found = MemorieDeFleursUIModel.Instance.FindBouquet(SelectedBouquet.BouquetCode);
                if(found == null)
                {
                    throw new ApplicationException($"該当する商品が見つかりません：{SelectedBouquet.BouquetCode}");
                }
                else
                {
                    SelectedBouquetCode = SelectedBouquet.BouquetCode;
                    NumberOfBouquet = MemorieDeFleursUIModel.Instance.GetNumberOfProcessingBouquetsOf(SelectedBouquetCode, ProcessingDate);
                    LoadParts(found);
                }
            }
        }
        private void Cleanup()
        {
            Parts.Clear();
            NumberOfBouquet = 0;

            LoadBouquets();
        }
        private void LoadBouquets()
        {
            Bouquets.Clear();
            foreach(var bouquet in MemorieDeFleursUIModel.Instance.FindAllBouquets())
            {
                Bouquets.Add(new BouquetSummaryViewModel(bouquet));
            }
            SelectedBouquet = null;
            RaisePropertyChanged(nameof(Bouquets));
        }
        private void LoadParts(Bouquet bouquet)
        {
            Parts.Clear();
            foreach (var parts in bouquet.PartsList)
            {
                Parts.Add(new PartsListItemViewModel(parts) { QuantityPerLot = NumberOfBouquet });
            }
            RaisePropertyChanged(nameof(Parts));
        }

        #region IPrintable
        public void PrintDocument()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();

                if(UserControlPrinter.PrintDocument<ProcessingInstructionControl>(this))
                {
                    LogUtil.Info($"Processing instructionsheet ({ProcessingDate:yyyyMMdd}, {SelectedBouquetCode}) printed.");
                }
                else
                {
                    LogUtil.Info($"Printing canceled.");
                }
            }
            catch(Exception ex)
            {
                LogUtil.Warn(ex);
            }
            finally
            {
                LogUtil.DEBUGLOG_EndMethod();
            }
        }
        #endregion // IPrintable
    }
}
