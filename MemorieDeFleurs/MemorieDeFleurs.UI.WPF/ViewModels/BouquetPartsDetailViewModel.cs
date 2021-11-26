using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models.Entities;
using MemorieDeFleurs.UI.WPF.Commands;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System;
using System.Windows;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class BouquetPartsDetailViewModel : DetailViewModelBase, IReloadable
    {
        public static string Name { get; } = "単品詳細";
        public BouquetPartsDetailViewModel() : base(Name) { }
        public BouquetPartsDetailViewModel(BouquetPart parts) : base(Name) { Update(parts); }

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

        /// <summary>
        /// 購入単位数：1ロットあたりの単品本数
        /// </summary>
        public int QuantitiesParLot
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }
        private int _quantity;

        /// <summary>
        /// 発注リードタイム [単位：日]
        /// </summary>
        public int LeadTime
        {
            get { return _leadTime; }
            set { SetProperty(ref _leadTime, value); }
        }
        private int _leadTime;

        /// <summary>
        /// 品質維持可能日数 [単位：日]
        /// </summary>
        public int ExpiryDate
        {
            get { return _expriy; }
            set { SetProperty(ref _expriy, value); }
        }
        private int _expriy;
        #endregion // プロパティ

        public void Update(BouquetPart part)
        {
            PartsCode = part.Code;
            PartsName = part.Name;
            QuantitiesParLot = part.QuantitiesPerLot;
            LeadTime = part.LeadTime;
            ExpiryDate = part.ExpiryDate;

            IsDirty = false;
        }

        /// <summary>
        /// 入力されている値を検証する。
        /// 
        /// 検証で何も問題なければ正常終了、何らかの不正値があるときは例外 <see cref="ValidateFailedException"/> がスローされる。
        /// </summary>
        public override void Validate()
        {
            var result = new ValidateFailedException();

            if(string.IsNullOrWhiteSpace(PartsCode))
            {
                result.Append("花コードを入力してください。");
            }
            if(string.IsNullOrWhiteSpace(PartsName))
            {
                result.Append("単品名称を入力してください。");
            }
            if(QuantitiesParLot < 1)
            {
                result.Append($"購入単位数が不適切です：{QuantitiesParLot}。1本以上の値を入力してください。");
            }
            if(LeadTime < 1)
            {
                result.Append($"商品リードタイムが不適切です：{LeadTime}。1日以上の値を入力してください。");
            }
            if(ExpiryDate < 1)
            {
                result.Append($"品質維持可能日数が不適切です：{ExpiryDate}。1日以上の値を入力してください。");
            }

            if(result.ValidationErrors.Count > 0) { throw result; }
        }

        #region IReloadable
        /// <inheritdoc/>
        public ICommand Reload { get; } = new ReloadCommand();

        /// <inheritdoc/>
        public void UpdateProperties()
        {
            if (string.IsNullOrWhiteSpace(PartsCode))
            {
                throw new ApplicationException("花コードが指定されていません。");
            }
            else
            {
                var parts = MemorieDeFleursUIModel.Instance.FindBouquetParts(PartsCode);
                if (parts == null)
                {
                    throw new ApplicationException($"花コードに該当する単品が登録されていません：{PartsCode}");
                }
                else
                {
                    Update(parts);
                }
            }
        }
        #endregion // IReloadable

        public override void SaveToDatabase()
        {
            try
            {
                LogUtil.DEBUGLOG_BeginMethod();

                Validate();

                var parts = new BouquetPart()
                {
                    Code = PartsCode,
                    Name = PartsName,
                    QuantitiesPerLot = QuantitiesParLot,
                    LeadTime = LeadTime,
                    ExpiryDate = ExpiryDate
                };

                var saved = MemorieDeFleursUIModel.Instance.Save(parts);

                Update(saved);

                LogUtil.Info($"Bouquet parts {PartsCode} is saved.");
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

        public override void ClearProperties()
        {
            PartsCode = string.Empty;
            PartsName = string.Empty;
            QuantitiesParLot = 0;
            LeadTime = 0;
            ExpiryDate = 0;
        }
    }
}
