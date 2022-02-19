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
    public class OrderShippingTest : MemorieDeFleursModelTestBase
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

        public OrderShippingTest()
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
            Suppliers.Supplier2 = Model.SupplierModel.FindSupplier(2);
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
            foreach(var grp in context.OrderFromCustomers.AsEnumerable().GroupBy(order => order.ShippingDate))
            {
                InitialOrdersFromCustomers.Add(grp.Key, grp.Select(order => order.ID).ToList());
            }
        }
        #endregion // TestInitialize

        [TestMethod]
        public void ShippingOrdersAtApril30th()
        {
            Model.CustomerModel.ShipAllBouquets(DateConst.April30th);

            var order = InitialOrdersFromCustomers[DateConst.April30th][0];
            Assert.ThrowsException<ApplicationException>(() => Model.CustomerModel.ChangeArrivalDate(DateConst.April30th.AddDays(-5), order, DateConst.May1st));
            Assert.ThrowsException<ApplicationException>(() => Model.CustomerModel.CancelOrder(order));
        }

        [TestMethod]
        public void ShippingOneOrderAtApril30th()
        {
            var orderNo = InitialOrdersFromCustomers[DateConst.April30th][0];

            Model.CustomerModel.ShipOrders(DateConst.April30th, orderNo);

            Assert.ThrowsException<ApplicationException>(() => Model.CustomerModel.ChangeArrivalDate(DateConst.April30th, orderNo, DateConst.May5th));
            Assert.ThrowsException<ApplicationException>(() => Model.CustomerModel.CancelOrder(orderNo));

            var lotNo = InitialLots["BA001"][DateConst.April30th][0].LotNo;
            InventoryActionValidator.NewInstance().BouquetPartIs(BouquetParts.BA001).BEGIN
                .Lot(DateConst.April30th).BEGIN
                    .At(DateConst.April30th).Arrived(200).Used(20, 180)
                    .At(DateConst.May1st).Used(50, 130)
                    .At(DateConst.May2nd).Used(80, 50)
                    .At(DateConst.May3rd).Used(20, 30).Discarded(30)
                    .END
                .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }
    }
}
