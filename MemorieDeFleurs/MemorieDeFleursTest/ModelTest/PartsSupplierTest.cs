using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class PartsSupplierTest : MemorieDeFleursModelTestBase
    {
        public PartsSupplierTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            PrepareParts();
            PrepareSuppliers();
            PreparePartsSuppliers();
        }

        private void PrepareParts()
        {
            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();
            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA002")
                .PartNameIs("薔薇(白)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();
            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA003")
                .PartNameIs("薔薇(ピンク)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();
            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("GP001")
                .PartNameIs("かすみ草")
                .LeadTimeIs(2)
                .QauntityParLotIs(50)
                .ExpiryDateIs(2)
                .Create();
            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN001")
                .PartNameIs("カーネーション(赤)")
                .LeadTimeIs(3)
                .QauntityParLotIs(100)
                .ExpiryDateIs(5)
                .Create();
            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN002")
                .PartNameIs("カーネーション(ピンク)")
                .LeadTimeIs(3)
                .QauntityParLotIs(100)
                .ExpiryDateIs(5)
                .Create();
        }

        private Supplier SupplierProvidesNoParts { get; set; }
        private Supplier SupplierProvidesManyParts { get; set; }

        private void PrepareSuppliers()
        {
            SupplierProvidesNoParts = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .PhoneNumberIs("03012345678")
                .FaxNumberIs("03012345677")
                .EmailIs("shinbashi@localdomain")
                .Create();

            SupplierProvidesManyParts = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("木挽町花壇")
                .AddressIs("東京都中央区銀座四丁目121-5")
                .PhoneNumberIs("09098765432")
                .FaxNumberIs("0120000000")
                .EmailIs("kobiki@localdomain")
                .Create();
        }

        private void PreparePartsSuppliers()
        {
            ExecuteSqlRaw("insert into BOUQUET_SUPPLIERS values (2, 'GP001' )");
            ExecuteSqlRaw("insert into BOUQUET_SUPPLIERS values (2, 'CN001' )");
            ExecuteSqlRaw("insert into BOUQUET_SUPPLIERS values (2, 'CN002' )");

        }

        private void ExecuteSqlRaw(string sql)
        {
            using (var cmd = TestDB.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
        #endregion // TestInitialize

        #region SupplierPartsValidator
        private class SupplierPartsValidator
        {
            private Supplier _supplier;
            private ISet<string> _provides = new SortedSet<string>();
            private ISet<string> _notProvides = new SortedSet<string>();
            private SqliteConnection _dbConnection;

            private SupplierPartsValidator() { }

            public static SupplierPartsValidator NewInstance()
            {
                return new SupplierPartsValidator();
            }

            public SupplierPartsValidator SupplierIs(Supplier supplier)
            {
                _supplier = supplier;
                return this;
            }
            public SupplierPartsValidator TargetDbIs(SqliteConnection dbConnection)
            {
                _dbConnection = dbConnection;
                return this;
            }
            public SupplierPartsValidator Provides(params string[] partsCode)
            {
                foreach (var parts in partsCode)
                {
                    if (!_provides.Contains(parts))
                    {
                        _provides.Add(parts);
                    }
                }
                return this;
            }
            public SupplierPartsValidator NotProvides(params string[] partsCode)
            {
                foreach(var parts in partsCode)
                {
                    if (!_notProvides.Contains(parts))
                    {
                        _notProvides.Add(parts);
                    }
                }
                return this;
            }

            public void AssertAll()
            {
                Assert.IsNotNull(_dbConnection, "DB未指定");

                using (var context = new MemorieDeFleursDbContext(_dbConnection))
                {
                    Assert.IsNotNull(_supplier, "仕入先未定義");

                    foreach(var parts in _provides)
                    {
                        Assert.IsNotNull(context.PartsSuppliers.Find(_supplier.Code, parts), $"単品が登録されているはず： {parts}");
                    }
                    foreach(var parts in _notProvides)
                    {
                        Assert.IsNull(context.PartsSuppliers.Find(_supplier.Code, parts), $"単品は登録されていないはず： {parts}");
                    }
                }
            }
        }
        #endregion // SupplierPartsValidator

        #region 単品仕入先の登録
        [TestMethod]
        public void StartProvidingParts()
        {
            Model.SupplierModel.StartPrividingParts(SupplierProvidesNoParts.Code, "BA001");

            SupplierPartsValidator.NewInstance().SupplierIs(SupplierProvidesNoParts)
                .Provides("BA001")
                .NotProvides("BA002", "BA003", "GP001", "CN001", "CN002")
                .TargetDbIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void StartProvidingParts_SamePartsToSupplier()
        {
            Model.SupplierModel.StartPrividingParts(SupplierProvidesManyParts.Code, "GP001");

            SupplierPartsValidator.NewInstance().SupplierIs(SupplierProvidesManyParts)
                .Provides("GP001", "CN001", "CN002")
                .NotProvides("BA001", "BA002", "BA003")
                .TargetDbIs(TestDB)
                .AssertAll();
        }
        #endregion // 単品仕入先の登録

        #region 単品仕入先の登録解除
        [TestMethod]
        public void StopProvidingParts()
        {
            Model.SupplierModel.StopProvidingParts(SupplierProvidesManyParts.Code, "GP001");

            SupplierPartsValidator.NewInstance().SupplierIs(SupplierProvidesManyParts)
                .Provides("CN001", "CN002")
                .NotProvides("BA001", "BA002", "BA003", "GP001")
                .TargetDbIs(TestDB)
                .AssertAll();
        }

        [TestMethod,ExpectedException(typeof(ArgumentException))]
        public void StopUnprovidingParts()
        {
            Model.SupplierModel.StopProvidingParts(SupplierProvidesManyParts.Code, "BA001");
        }
        #endregion // 単品仕入先の登録解除
    }
}
