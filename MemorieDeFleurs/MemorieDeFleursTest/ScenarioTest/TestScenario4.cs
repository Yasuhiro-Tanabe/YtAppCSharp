using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ScenarioTest
{
    /// <summary>
    /// 受発注の両方を行う：納品リードタイムがあるのでテスト期間は4/25～5/8とする
    /// </summary>
    [TestClass]
    public class TestScenario4 : ScenarioTestBase
    {
        private static SqliteConnection TestDB { get; set; }
        private MemorieDeFleursModel Model { get; set; }

        private Customer Customer { get; set; }
        private IDictionary<int, Supplier> Suppliers { get; set; }
        private IDictionary<string, BouquetPart> Parts { get; set; }
        private IDictionary<string, Bouquet> Bouquets { get; set; }

        #region テストの初期化終了
        [ClassInitialize]
        public static void ClassInitialize(TestContext unused)
        {
            TestDB = OpenDatabase();

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CloseDatabase(TestDB);
        }

        [TestInitialize]
        public void CreateModel()
        {
            Model = new MemorieDeFleursModel(TestDB);
            Suppliers = Model.SupplierModel.FindAllSuppliers().ToDictionary(s => s.Code);
            Parts = Model.BouquetModel.FindAllBoueuqtParts().ToDictionary(p => p.Code);
            Bouquets = Model.BouquetModel.FindAllBouquets().ToDictionary(p => p.Code);

            Customer = Model.CustomerModel.GetCustomerBuilder()
                .NameIs("シナリオテストユーザ")
                .EmailAddressIs("testuser@localdomain")
                .CardNoIs("9999876543210000")
                .SendTo("送り先1", "都道府県市町村", "番地-号")
                .Create();
        }

        [TestCleanup]
        public void ReleaseModel()
        {
            Model = null;
        }
        #endregion // テストの初期化終了


        /// <summary>
        /// 初期状態の確認(客先受注履歴)
        /// </summary>
        [TestMethod]
        public void AtApril20_CheckInitialOrdersFromCustomer()
        {
            var expected = new SortedDictionary<DateTime, IDictionary<string, int>>()
            {
                { DateConst.April30th, new SortedDictionary<string,int>() { {"HT001", 5 }, { "HT002", 0 }, { "HT003", 0 }, { "HT004", 3 }, { "HT005", 4 }, { "HT006", 0 }, { "HT007", 0 } } } ,
                { DateConst.May1st, new SortedDictionary<string, int>() { { "HT001", 5 }, { "HT002", 0 }, { "HT003", 3 }, { "HT004", 1 }, { "HT005", 2 }, { "HT006", 5 }, { "HT007", 0 } } },
                { DateConst.May2nd, new SortedDictionary<string, int>() { { "HT001", 4 }, { "HT002", 2 }, { "HT003", 0 }, { "HT004", 4 }, { "HT005", 0 }, { "HT006", 3 }, { "HT007", 1 } } },
                { DateConst.May3rd, new SortedDictionary<string, int>() { { "HT001", 2 }, { "HT002", 4 }, { "HT003", 2 }, { "HT004", 2 }, { "HT005", 4 }, { "HT006", 0 }, { "HT007", 0 } } },
                { DateConst.May4th, new SortedDictionary<string, int>() { { "HT001", 0 }, { "HT002", 0 }, { "HT003", 3 }, { "HT004", 0 }, { "HT005", 0 }, { "HT006", 0 }, { "HT007", 10 } } },
                { DateConst.May5th, new SortedDictionary<string, int>() { { "HT001", 5 }, { "HT002", 0 }, { "HT003", 0 }, { "HT004", 0 }, { "HT005", 0 }, { "HT006", 5 }, { "HT007", 3 } } },
                { DateConst.May6th, new SortedDictionary<string, int>() { { "HT001", 4 }, { "HT002", 0 }, { "HT003", 0 }, { "HT004", 6 }, { "HT005", 0 }, { "HT006", 4 }, { "HT007", 0 } } },
            };

            foreach (var day in Enumerable.Range(0, 7).Select(i => DateConst.April30th.AddDays(i)))
            {
                var actual = Model.CustomerModel.GetShippingBouquetCountAt(day);
                Assert.AreEqual(expected.Count, actual.Count, $"商品数が一致しない： {day:yyyyMMdd}");
                foreach(var item in expected[day])
                {
                    Assert.IsTrue(actual.ContainsKey(item.Key), $"商品 {item.Key} が取得できない： {day:yyyyMMdd}");
                    Assert.AreEqual(expected[day][item.Key], actual[item.Key], $"商品 {item.Key} の数量が一致しない： {day:yyyyMMdd}");
                }
                foreach(var item in actual)
                {
                    Assert.IsTrue(expected[day].ContainsKey(item.Key), $"商品 {item.Key} は存在しないはず： {day:yyyyMMdd}");
                }
            }
        }

        /// <summary>
        /// 初期状態の確認(仕入先発注)
        /// </summary>
        [TestMethod]
        public void AtApril20_CheckInitialOrdersToSupplier()
        {
            var expected = new SortedDictionary<int, IDictionary<DateTime, IDictionary<string, int>>>()
            {
                { 1, new SortedDictionary<DateTime, IDictionary<string, int>>() {
                    { DateConst.April30th, new SortedDictionary<string, int>() { { "BA001", 2 }, { "BA002", 1 }, { "BA003", 1 }, { "GP001", 1} } },
                    { DateConst.May1st, new SortedDictionary<string, int>() { { "BA001", 3 }, { "BA002", 0 }, { "BA003", 0 }, { "GP001", 0} } },
                    { DateConst.May2nd, new SortedDictionary<string, int>() { { "BA001", 1 }, { "BA002", 0 }, { "BA003", 0 }, { "GP001", 0} } },
                    { DateConst.May3rd, new SortedDictionary<string, int>() { { "BA001", 2 }, { "BA002", 2 }, { "BA003", 2 }, { "GP001", 0} } },
                    { DateConst.May4th, new SortedDictionary<string, int>() { { "BA001", 0 }, { "BA002", 0 }, { "BA003", 0 }, { "GP001", 0} } },
                    { DateConst.May5th, new SortedDictionary<string, int>() { { "BA001", 0 }, { "BA002", 0 }, { "BA003", 0 }, { "GP001", 0} } },
                    { DateConst.May6th, new SortedDictionary<string, int>() { { "BA001", 1 }, { "BA002", 0 }, { "BA003", 0 }, { "GP001", 0} } },
                } },
                { 2, new SortedDictionary<DateTime, IDictionary<string, int>>() {
                    { DateConst.April30th, new SortedDictionary<string, int>() { { "CN001", 1 }, { "CN002", 1 }, { "GP001", 0 } } },
                    { DateConst.May1st, new SortedDictionary<string, int>() { { "CN001", 0 }, { "CN002", 1 }, { "GP001", 0 } } },
                    { DateConst.May2nd, new SortedDictionary<string, int>() { { "CN001", 1 }, { "CN002", 0 }, { "GP001", 1 } } },
                    { DateConst.May3rd, new SortedDictionary<string, int>() { { "CN001", 0 }, { "CN002", 1 }, { "GP001", 0 } } },
                    { DateConst.May4th, new SortedDictionary<string, int>() { { "CN001", 0 }, { "CN002", 0 }, { "GP001", 0 } } },
                    { DateConst.May5th, new SortedDictionary<string, int>() { { "CN001", 0 }, { "CN002", 1 }, { "GP001", 1 } } },
                    { DateConst.May6th, new SortedDictionary<string, int>() { { "CN001", 0 }, { "CN002", 0 }, { "GP001", 0 } } },
                } },
            };

            foreach (var supplier in expected)
            {
                foreach (var day in supplier.Value)
                {
                    var actual = Model.SupplierModel.GetArrivalBouquetPartsCountAt(supplier.Key, day.Key);
                    foreach (var bouquet in day.Value)
                    {
                        Assert.IsTrue(actual.ContainsKey(bouquet.Key), $"単品が登録されていない： Supplier{supplier.Key}, {day.Key:yyyyMMdd}, {bouquet.Key}");
                        Assert.AreEqual(expected[supplier.Key][day.Key][bouquet.Key], bouquet.Value, $"ロット数が一致しない： Supplier{supplier.Key}, {day.Key:yyyyMMdd}, {bouquet.Key}");
                    }
                    foreach (var bouquet in actual)
                    {
                        Assert.IsTrue(expected[supplier.Key][day.Key].ContainsKey(bouquet.Key), $"想定外の単品が登録されている： Supplier{supplier.Key}, {day.Key:yyyyMMdd}, {bouquet.Key}");
                    }
                }
            }
        }
    }
}
