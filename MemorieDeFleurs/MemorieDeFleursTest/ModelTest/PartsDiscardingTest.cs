using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using YasT.Framework.Logging;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class PartsDiscardingTest :MemorieDeFleursModelTestBase
    {
        #region テスト用データ
        private IList<string> SQLs { get; set; }
        private class PartsInfo
        {
            public BouquetPart BA001 { get; set; }
            public BouquetPart BA002 { get; set; }
            public BouquetPart BA003 { get; set; }

            public BouquetPart GP001 { get; set; }
            public BouquetPart CN001 { get; set; }
            public BouquetPart CN002 { get; set; }
        }
        private PartsInfo BouquetParts { get; } = new PartsInfo();

        private class ProductsInfo
        {
            public Bouquet HT001 { get; set; }
            public Bouquet HT002 { get; set; }
            public Bouquet HT003 { get; set; }
            public Bouquet HT004 { get; set; }

            public Bouquet HT005 { get; set; }
            public Bouquet HT006 { get; set; }
            public Bouquet HT007 { get; set; }

        }
        private ProductsInfo Bouquets { get; } = new ProductsInfo();

        private class CustomerInfo
        {
            public Customer Customer1 { get; set; }
            public Customer Customer2 { get; set; }
            public Customer Customer3 { get; set; }
        }
        private CustomerInfo Customers { get; } = new CustomerInfo();

        private class SupplierInfo
        {
            public Supplier Supplier1 { get; set; }
            public Supplier Supplier2 { get; set; }
        }
        private SupplierInfo Suppliers { get; } = new SupplierInfo();

        private IDictionary<DateTime, IList<string>> InitialOrdersToSupplyer { get; } = new SortedDictionary<DateTime, IList<string>>();

        private IDictionary<string, TestOrder> InitialLots { get; } = new SortedDictionary<string, TestOrder>();

        private IDictionary<DateTime, IList<string>> InitialOrdersFromCustomers { get; } = new SortedDictionary<DateTime, IList<string>>();
        #endregion // テスト用データ

        public PartsDiscardingTest()
        {
            AfterTestBaseInitializing += PrepareModel;
        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            LoadSqlData();

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                PrepareParts(context);
                PrepareBouquets(context);
                PrepareCustomers(context);
                PrepareSuppliers(context);
                GetInitialLotNumbers(context);
                GetInitialOrdersToSupplier(context);
                GetInitialOrdersFromCustomer(context);
            }
        }

        private void LoadSqlData()
        {
            var read = new StreamReader("./testdata/TestData.sql").ReadToEnd().Split("\n");
            string currentSQL = string.Empty;
            string previousSQL = string.Empty;

            using (var transaction = TestDB.BeginTransaction())
            {
                try
                {
                    using (var cmd = TestDB.CreateCommand())
                    {
                        foreach (var sql in read.Where(line => line.StartsWith("delete") || line.StartsWith("insert")))
                        {
                            previousSQL = currentSQL;
                            currentSQL = sql;
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    LogUtil.Error($"{ex.GetType().Name}: {ex.Message},\n\tCurrent SQL={currentSQL}\n\tPrevious SQL={previousSQL}");
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private void PrepareParts(MemorieDeFleursDbContext context)
        {
            BouquetParts.BA001 = Model.BouquetModel.FindBouquetPart("BA001");
            BouquetParts.BA002 = Model.BouquetModel.FindBouquetPart("BA002");
            BouquetParts.BA003 = Model.BouquetModel.FindBouquetPart("BA003");
            BouquetParts.GP001 = Model.BouquetModel.FindBouquetPart("GP001");
            BouquetParts.CN001 = Model.BouquetModel.FindBouquetPart("CN001");
            BouquetParts.CN002 = Model.BouquetModel.FindBouquetPart("CN002");
        }

        private void PrepareBouquets(MemorieDeFleursDbContext context)
        {
            Bouquets.HT001 = Model.BouquetModel.FindBouquet("HT001");
            Bouquets.HT002 = Model.BouquetModel.FindBouquet("HT002");
            Bouquets.HT003 = Model.BouquetModel.FindBouquet("HT003");
            Bouquets.HT004 = Model.BouquetModel.FindBouquet("HT004");
            Bouquets.HT005 = Model.BouquetModel.FindBouquet("HT005");
            Bouquets.HT006 = Model.BouquetModel.FindBouquet("HT006");
            Bouquets.HT007 = Model.BouquetModel.FindBouquet("HT007");
        }

        private void PrepareCustomers(MemorieDeFleursDbContext context)
        {
            Customers.Customer1 = Model.CustomerModel.FindCustomer(1);
            Customers.Customer2 = Model.CustomerModel.FindCustomer(2);
            Customers.Customer3 = Model.CustomerModel.FindCustomer(3);
        }

        private void PrepareSuppliers(MemorieDeFleursDbContext context)
        {
            Suppliers.Supplier1 = Model.SupplierModel.FindSupplier(1);
            Suppliers.Supplier2 = Model.SupplierModel.FindSupplier(1);
        }
        private void GetInitialLotNumbers(MemorieDeFleursDbContext context)
        {
            LogUtil.DEBUGLOG_BeginMethod();
            var arrivedActions = context.InventoryActions
                .Where(act => act.Action == InventoryActionType.SCHEDULED_TO_ARRIVE)
                .OrderBy(act => act.PartsCode)
                .ThenBy(act => act.ArrivalDate)
                .ToList();

            foreach (var grp in arrivedActions
                .GroupBy(act => act.PartsCode))
            {
                var order = new TestOrder();
                foreach (var act in grp)
                {
                    order.Append(act.ArrivalDate, act.InventoryLotNo, act.Quantity);
                }
                InitialLots.Add(grp.Key, order);
                LogUtil.DebugWithoutLineNumber($"{grp.Key}=[{order.ToString()}]");
            }
            LogUtil.DEBUGLOG_EndMethod();
        }

        private void GetInitialOrdersToSupplier(MemorieDeFleursDbContext context)
        {
            LogUtil.DEBUGLOG_BeginMethod();
            try
            {
                foreach (var grp in context.OrdersToSuppliers.ToList()
                    .OrderBy(order => order.DeliveryDate)
                    .GroupBy(o => o.DeliveryDate))
                {
                    var list = new List<string>();

                    foreach (var order in grp)
                    {
                        list.Add(order.ID);
                    }

                    InitialOrdersToSupplyer.Add(grp.Key, list.ToList());
                    LogUtil.DebugWithoutLineNumber($"{grp.Key:yyyyMMdd}: {string.Join(", ", list)}");
                }
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

        private void GetInitialOrdersFromCustomer(MemorieDeFleursDbContext context)
        {
            foreach (var grp in context.OrderFromCustomers.AsEnumerable().GroupBy(order => order.ShippingDate))
            {
                LogUtil.Debug($"grp=[Key={grp.Key}, Values=[ {string.Join(", ", grp.Select(order => $"{order.ID}, {order.Bouquet}, {order.OrderDate}"))} ]");
                InitialOrdersFromCustomers.Add(grp.Key, grp.Select(order => order.ID).ToList());
            }
        }
        #endregion // TestInitialize

        [TestMethod]
        public void DiscardOneParts()
        {
            LogUtil.DEBUGLOG_BeginTest();

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            Model.BouquetModel.DiscardBouquetParts(DateConst.May9th, Tuple.Create("BA001", 100));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.May6th).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100)
                        .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .ContainsActionType(InventoryActionType.DISCARDED)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void DiscardBeforeExpiryDate()
        {
            Model.BouquetModel.DiscardBouquetParts(DateConst.May7th, Tuple.Create("BA001", 20));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.May6th).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 80).Discarded(20)
                        .ContainsActionType(InventoryActionType.DISCARDED)
                    .At(DateConst.May8th).Used(0, 80)
                    .At(DateConst.May9th).Used(0, 80).Discarded(80)
                        .ContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .NotContainsActionType(InventoryActionType.DISCARDED)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void DiscardMoreThanExpectedQuantity()
        {
            Model.BouquetModel.DiscardBouquetParts(DateConst.May9th, Tuple.Create("BA001", 120));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.May6th).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(0, 100)
                    .At(DateConst.May7th).Used(0, 100)
                    .At(DateConst.May8th).Used(0, 100)
                    .At(DateConst.May9th).Used(0, 100).Discarded(100).Shortage(20)
                        .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .ContainsActionType(InventoryActionType.DISCARDED)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }

        [TestMethod]
        public void DiscardFromTwoLot()
        {
            Model.BouquetModel.DiscardBouquetParts(DateConst.May6th, Tuple.Create("BA001", 150));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.May3rd).BEGIN
                    .At(DateConst.May6th).Used(40, 90).Discarded(90)
                    .END
                .Lot(DateConst.May6th).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(0, 40).Discarded(60)
                    .At(DateConst.May9th).Used(0, 40).Discarded(40)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        /// <summary>
        /// 4/30 に BA001を100本破棄→ 5/5 に在庫不足発生
        /// </summary>
        [TestMethod]
        public void InentoryShortageCausedToDiscard()
        {
            Model.BouquetModel.DiscardBouquetParts(DateConst.April30th, Tuple.Create("BA001", 150));
            Model.BouquetModel.DiscardBouquetParts(DateConst.May1st, Tuple.Create("BA001", 50));

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            var lot0430 = InitialLots["BA001"][DateConst.April30th][0].LotNo;
            var lot0501 = InitialLots["BA001"][DateConst.May1st][0].LotNo;
            var lot0502 = InitialLots["BA001"][DateConst.May2nd][0].LotNo;
            var lot0503 = InitialLots["BA001"][DateConst.May3rd][0].LotNo;
            var lot0506 = InitialLots["BA001"][DateConst.May6th][0].LotNo;
            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.April30th).BEGIN
                    .At(DateConst.April30th).Arrived(200).Used(20, 30).Discarded(150)
                    .At(DateConst.May1st).Used(30, 0)
                    .At(DateConst.May2nd).Used(0, 0)
                    .At(DateConst.May3rd).Used(0, 0).Discarded(0)
                    .END
                .Lot(DateConst.May1st).BEGIN
                    .At(DateConst.May1st).Arrived(300).Used(20, 230).Discarded(50)
                    .At(DateConst.May2nd).Used(80, 150)
                    .At(DateConst.May3rd).Used(20, 130)
                    .At(DateConst.May4th).Used(130, 0).Discarded(0)
                    .END
                .Lot(DateConst.May2nd).BEGIN
                    .At(DateConst.May2nd).Arrived(200).Used(0, 200)
                    .At(DateConst.May3rd).Used(0, 200)
                    .At(DateConst.May4th).Used(200, 0)
                    .At(DateConst.May5th).Used(0, 0).Discarded(0)
                    .END
                .Lot(DateConst.May3rd).BEGIN
                    .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                    .At(DateConst.May4th).Used(70, 130)
                    .At(DateConst.May5th).Used(130, 0).Shortage(40)
                    .At(DateConst.May6th).Used(0, 0)
                    .END
                .Lot(DateConst.May6th).BEGIN
                    .At(DateConst.May6th).Arrived(100).Used(40, 60)
                    .At(DateConst.May9th).Used(0, 60).Discarded(60)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }
    }
}
