using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class InventoryTransitionTableTest : MemorieDeFleursModelTestBase
    {
        /// <summary>
        /// テストで使用する仕入先
        /// </summary>
        private Supplier ExpectedSupplier { get; set; }

        /// <summary>
        /// テストで使用する単品
        /// </summary>
        private BouquetPart ExpectedPart { get; set; }

        public InventoryTransitionTableTest() : base()
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
                    PrepareSuppliers(context);
                    PrepareBouquetParts(context);
                    PrepareInitialOrders(context);
                    PrepareInitialUsed(context);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private void PrepareSuppliers(MemorieDeFleursDbContext context)
        {
            ExpectedSupplier = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .Create(context);
        }

        private void PrepareBouquetParts(MemorieDeFleursDbContext context)
        {
            ExpectedPart = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);
        }

        private void PrepareInitialUsed(MemorieDeFleursDbContext context)
        {
            Model.BouquetModel.UseFromInventory(context, ExpectedPart, DateConst.April30th, 20);
            Model.BouquetModel.UseFromInventory(context, ExpectedPart, DateConst.May1st, 50);
            Model.BouquetModel.UseFromInventory(context, ExpectedPart, DateConst.May2nd, 80);
            Model.BouquetModel.UseFromInventory(context, ExpectedPart, DateConst.May3rd, 20);
            Model.BouquetModel.UseFromInventory(context, ExpectedPart, DateConst.May4th, 400);
            Model.BouquetModel.UseFromInventory(context, ExpectedPart, DateConst.May5th, 170);
            Model.BouquetModel.UseFromInventory(context, ExpectedPart, DateConst.May6th, 40);
        }

        IDictionary<DateTime, string> InitialOrders { get; } = new SortedDictionary<DateTime, string>();

        private void PrepareInitialOrders(MemorieDeFleursDbContext context)
        {
            var OrderDate = new DateTime(DateConst.Year, 4, 25);
            InitialOrders.Add(DateConst.April30th,
                Model.SupplierModel.Order(context, OrderDate, ExpectedSupplier, DateConst.April30th, new[] { Tuple.Create(ExpectedPart, 2) }));
            InitialOrders.Add(DateConst.May1st,
                Model.SupplierModel.Order(context, OrderDate, ExpectedSupplier, DateConst.May1st, new[] { Tuple.Create(ExpectedPart, 3) }));
            InitialOrders.Add(DateConst.May2nd,
                Model.SupplierModel.Order(context, OrderDate, ExpectedSupplier, DateConst.May2nd, new[] { Tuple.Create(ExpectedPart, 2) }));
            InitialOrders.Add(DateConst.May3rd,
                Model.SupplierModel.Order(context, OrderDate, ExpectedSupplier, DateConst.May3rd, new[] { Tuple.Create(ExpectedPart, 2) }));
            InitialOrders.Add(DateConst.May6th,
                Model.SupplierModel.Order(context, OrderDate, ExpectedSupplier, DateConst.May6th, new[] { Tuple.Create(ExpectedPart, 1) }));
        }
        #endregion // TestInitialize

        #region TableValidator
        private class TableValidator
        {
            private InventoryTransitionTable Table { get; set; }
            private DateTime CurrentDate { get; set; } = DateTime.MinValue;

            private class ItemValidator
            {
                internal int Arrived { get; set; }
                internal int Used { get; set; }
                internal int Discarded { get; set; }
                internal IList<int> Remains { get; set; }
            }

            private IDictionary<DateTime, ItemValidator> Items { get; } = new SortedDictionary<DateTime, ItemValidator>();
            private ItemValidator CurrentItem { get; set; }


            public static TableValidator TableIs(InventoryTransitionTable table)
            {
                return new TableValidator(table);
            }

            private TableValidator(InventoryTransitionTable table)
            {
                Table = table;
            }

            public TableValidator At(DateTime date)
            {
                ItemValidator item;
                if(!Items.TryGetValue(date, out item))
                {
                    CurrentDate = date;
                    CurrentItem = new ItemValidator();
                }
                else
                {
                    CurrentDate = date;
                    CurrentItem = item;
                }
                return this;
            }

            public TableValidator Arrived(int n)
            {
                CurrentItem.Arrived = n;
                return this;
            }
            public TableValidator Used(int n)
            {
                CurrentItem.Used = n;
                return this;
            }
            public TableValidator Discarded(int n)
            {
                CurrentItem.Discarded = n;
                return this;
            }

            public TableValidator Remains(params int[] remains)
            {
                if(Items.Count() > 0)
                {
                    if(Items.First().Value.Remains.Count() != remains.Length)
                    {
                        throw new ArgumentException($"残数リストサイズ不正, {remains.Length} : " +
                            $"すでに登録されている残数は要素数は { Items.First().Value.Remains.Count()} であり一致しない");
                    }
                }

                CurrentItem.Remains = remains.Reverse().ToList();

                return this;
            }

            public TableValidator END
            {
                get
                {
                    if(!Items.ContainsKey(CurrentDate))
                    {
                        Items.Add(CurrentDate, CurrentItem);
                    }

                    CurrentItem = null;
                    CurrentDate = DateTime.MinValue;

                    return this;
                }
            }

            public void AssertAll()
            {
                foreach(var item in Items)
                {
                    Assert.AreEqual(item.Value.Arrived, Table[item.Key].Arrived, $"{item.Key:yyyyMMdd}, Arrived");
                    Assert.AreEqual(item.Value.Used, Table[item.Key].Used, $"{item.Key:yyyyMMdd}, Used");
                    Assert.AreEqual(item.Value.Discarded, Table[item.Key].Discarded, $"{item.Key:yyyyMMdd}, Discarded");
                    
                    foreach(var d in Enumerable.Range(0, item.Value.Remains.Count))
                    {
                        Assert.AreEqual(item.Value.Remains[d], Table[item.Key, -d], $"{item.Key:yyyyMMdd}, Remains[{-d}={item.Key.AddDays(-d):yyyyMMdd}]");
                    }
                }
            }

        }
        #endregion // TableValidator

        [TestMethod]
        public void EmptyTable()
        {
            var date = new DateTime(DateConst.Year, 10, 1); // 当日前後数日の範囲に在庫登録していない日付
            var table = Model.CreateInventoryTransitionTable("BA001", date, 1);

            TableValidator.TableIs(table)
                .At(date).Arrived(0).Used(0).Remains(0, 0, 0).Discarded(0).END
                .AssertAll();
        }

        [TestMethod]
        public void BA001_FromApril30thToMay6th()
        {
            var table = Model.CreateInventoryTransitionTable("BA001", DateConst.April30th, 7);

            TableValidator.TableIs(table)
                .At(DateConst.April30th).Arrived(200).Used(20).Remains(0, 0, 180).END
                .At(DateConst.May1st).Arrived(300).Used(50).Remains(0, 130, 300).END
                .At(DateConst.May2nd).Arrived(200).Used(80).Remains(50, 300, 200).END
                .At(DateConst.May3rd).Arrived(200).Used(20).Remains(300, 200, 200).Discarded(30).END
                .At(DateConst.May4th).Used(400).Remains(100, 200, 0).END
                .At(DateConst.May5th).Used(170).Remains(130, 0, 0).END
                .At(DateConst.May6th).Arrived(100).Used(40).Remains(0, 0, 100).Discarded(90).END
                .AssertAll();
        }

        [TestMethod]
        public void BA001_InventoryShortage()
        {
            Model.SupplierModel.CancelOrder(InitialOrders[DateConst.May2nd]);
            var table = Model.CreateInventoryTransitionTable("BA001", DateConst.April30th, 7);

            TableValidator.TableIs(table)
                .At(DateConst.April30th).Arrived(200).Used(20).Remains(0, 0, 180).END
                .At(DateConst.May1st).Arrived(300).Used(50).Remains(0, 130, 300).END
                .At(DateConst.May2nd).Used(80).Remains(50, 300, 0).END
                .At(DateConst.May3rd).Arrived(200).Used(20).Remains(300, 0, 200).Discarded(30).END
                .At(DateConst.May4th).Used(400).Remains(0, 100, 0).END
                .At(DateConst.May5th).Used(170).Remains(-70, 0, 0).END
                .At(DateConst.May6th).Arrived(100).Used(40).Remains(0, 0, 60).Discarded(0).END
                .AssertAll();
        }
    }
}
