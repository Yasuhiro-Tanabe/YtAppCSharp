using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace MemorieDeFleursTest.ModelTest
{
    /// <summary>
    /// 商品構成の登録改廃に関するテスト
    /// </summary>
    [TestClass]
    public class BouquetPartsListCrationTest : MemorieDeFleursModelTestBase
    {
        private BouquetModel BouquetModel { get { return Model.BouquetModel; } }

        private Bouquet HasPartsList { get; set; }

        private Bouquet NoPartsList { get; set; }

        private IDictionary<string, BouquetPart> BouquetParts { get; } = new SortedDictionary<string, BouquetPart>();

        public BouquetPartsListCrationTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            using (var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    RegisterBouquetParts(context);
                    RegisterBouquets(context);
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private void RegisterBouquets(MemorieDeFleursDbContext context)
        {
            HasPartsList = BouquetModel.GetBouquetBuilder()
                .CodeIs("HT002")
                .NameIs("花束-Bセット")
                .Uses("BA001", 3)
                .Uses("BA002", 5)
                .Uses("BA003", 3)
                .Create(context);

            NoPartsList = BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Create(context);
        }

        private void RegisterBouquetParts(MemorieDeFleursDbContext context)
        {
            var builder = BouquetModel.GetBouquetPartBuilder();
            BouquetParts.Add("BA001",
                builder.PartCodeIs("BA001")
                        .PartNameIs("薔薇(赤)")
                        .LeadTimeIs(1)
                        .QauntityParLotIs(100)
                        .ExpiryDateIs(3)
                        .Create(context));
            BouquetParts.Add("BA002",
                builder.PartCodeIs("BA002")
                        .PartNameIs("薔薇(白)")
                        .LeadTimeIs(1)
                        .QauntityParLotIs(100)
                        .ExpiryDateIs(3)
                        .Create(context));
            BouquetParts.Add("BA003",
                builder.PartCodeIs("BA003")
                        .PartNameIs("薔薇(ピンク)")
                        .LeadTimeIs(1)
                        .QauntityParLotIs(100)
                        .ExpiryDateIs(3)
                        .Create(context));
            BouquetParts.Add("GP001",
                builder.PartCodeIs("GP001")
                        .PartNameIs("かすみ草")
                        .LeadTimeIs(2)
                        .QauntityParLotIs(50)
                        .ExpiryDateIs(2)
                        .Create(context));
        }
        #endregion // TestInitialize

        #region PartsListValidator
        private class PartsListValidator
        {
            private string _bouquetCode;
            private IDictionary<string, int> _includes = new Dictionary<string, int>();
            private ISet<string> _excludes = new SortedSet<string>();
            private SqliteConnection _dbConnection;

            private PartsListValidator() { }
            public static PartsListValidator NewInstance()
            {
                return new PartsListValidator();
            }

            public PartsListValidator TargetDBIs(SqliteConnection conn)
            {
                _dbConnection = conn;
                return this;
            }

            public PartsListValidator BouquetIs(string code)
            {
                _bouquetCode = code;
                return this;
            }

            public PartsListValidator Contains(string partsCode, int quantity)
            {
                if(_includes.ContainsKey(partsCode))
                {
                    _includes[partsCode] += quantity;
                }
                else
                {
                    _includes.Add(partsCode, quantity);
                }
                return this;
            }

            public PartsListValidator NotContains(string partsCode)
            {
                if(_excludes.Contains(partsCode))
                {
                    _excludes.Add(partsCode);
                }
                return this;
            }

            public void AssertAll()
            {
                Assert.IsNotNull(_dbConnection, $"TargetDbIs() を呼び出していない");
                Assert.IsFalse(string.IsNullOrWhiteSpace(_bouquetCode), "BouquetIs() を呼び出していない");

                using(var context = new MemorieDeFleursDbContext(_dbConnection))
                {
                    Assert.IsNotNull(context.Bouquets.Find(_bouquetCode), $"該当商品未登録：{_bouquetCode}");
                    foreach (var item in _includes)
                    {
                        Assert.IsNotNull(context.BouquetParts.Find(item.Key), $"該当単品未登録：{item.Key}");
                        var i = context.PartsList.Find(_bouquetCode, item.Key);
                        Assert.IsNotNull(i, $"商品構成に登録されているはず： {item.Key}");
                        Assert.AreEqual(item.Value, i.Quantity, $"登録数が一致しない： {item.Key}");
                    }
                    foreach(var code in _excludes)
                    {
                        Assert.IsNotNull(context.BouquetParts.Find(code), $"該当単品未登録：{code}");
                        var i = context.PartsList.Find(_bouquetCode, code);
                        Assert.IsNull(i, $"商品構成に登録されていないはず：{code}");
                    }
                }
            }

        }
        #endregion // PartsListValidator

        #region 商品構成作成・商品追加のテスト
        [TestMethod]
        public void AppendPartsToHT001()
        {
            BouquetModel.AppendPartsTo("HT001", "BA001", 2);

            PartsListValidator.NewInstance().BouquetIs("HT001")
                .Contains("BA001", 2)
                .NotContains("GP001")
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void CreatePartsListOfHT001()
        {
            BouquetModel.CreatePartsListOf("HT001", new Dictionary<string, int>() { { "BA001", 2 }, { "GP001", 3 } });

            PartsListValidator.NewInstance().BouquetIs("HT001")
                .Contains("BA001", 2)
                .Contains("GP001", 3)
                .NotContains("BA002")
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AppendPartsToHT002()
        {
            BouquetModel.AppendPartsTo("HT002", "GP001", 6);

            PartsListValidator.NewInstance().BouquetIs("HT002")
                .Contains("BA001", 3)
                .Contains("BA002", 5)
                .Contains("BA003", 3)
                .Contains("GP001", 6)
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod,ExpectedException(typeof(ArgumentException))]
        public void AppendPartsToInvalidBouquet()
        {
            BouquetModel.AppendPartsTo("HT003", "BA001", 1);
        }

        [TestMethod,ExpectedException(typeof(ArgumentException))]
        public void AppendInvalidPartsToBouquet()
        {
            BouquetModel.AppendPartsTo("HT001", "XYZ12", 1);
        }

        [TestMethod,ExpectedException(typeof(ArgumentException))]
        public void AppendInvalidQuantityOfParts()
        {
            BouquetModel.AppendPartsTo("HT001", "BA001", 0);
        }

        [TestMethod]
        public void AppendSamePartsToBouquet()
        {
            BouquetModel.AppendPartsTo("HT002", "BA001", 3);

            // HT002 には元々 BA001 が 3 本登録されているので、
            // Append により合計6本になるはず
            PartsListValidator.NewInstance().BouquetIs("HT002")
                .Contains("BA001", 6)
                .Contains("BA002", 5)
                .Contains("BA003", 3)
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void CreateBouquetWithPartsListContainsSameParts()
        {
            BouquetModel.AppendPartsTo("HT001", "BA001", 2);
            BouquetModel.AppendPartsTo("HT001", "BA001", 3);
            BouquetModel.AppendPartsTo("HT001", "BA001", 3);

            // BA001 を 2+3+3 = 8本登録したことになるはず
            PartsListValidator.NewInstance().BouquetIs("HT001")
                .Contains("BA001", 8)
                .TargetDBIs(TestDB)
                .AssertAll();
        }
        #endregion // 商品構成作成・追加

        #region 商品構成削除のテスト
        [TestMethod]
        public void RemovePartsFromHT002()
        {
            BouquetModel.RemovePartsFrom("HT002", "BA001");

            PartsListValidator.NewInstance().BouquetIs("HT002")
                .NotContains("BA001")
                .Contains("BA002", 5)
                .Contains("BA003", 3)
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void RemoveAllPartsFromHT002()
        {
            BouquetModel.RemoveAllPartsFrom("HT002");

            PartsListValidator.NewInstance().BouquetIs("HT002")
                .NotContains("BA001")
                .NotContains("BA002")
                .NotContains("BA003")
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod,ExpectedException(typeof(ArgumentException))]
        public void RemovePartsNotContainsInBouquet()
        {
            BouquetModel.RemovePartsFrom("HT002", "GP001");
        }
        #endregion // 商品構成削除 

        #region 単品数量変更のテスト
        [TestMethod]
        public void UpdatePartsQuantity()
        {
            BouquetModel.UpdateQuantityOf("HT002", "BA002", 8);

            PartsListValidator.NewInstance().BouquetIs("HT002")
                .Contains("BA001", 3)
                .Contains("BA002", 8)
                .Contains("BA003", 3)
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod,ExpectedException(typeof(ArgumentException))]
        public void UpdatePartsQuantity_NotContainsInBouquet()
        {
            BouquetModel.UpdateQuantityOf("HT002", "GP001", 1);
        }
        #endregion // 単品数量変更 
    }
}
