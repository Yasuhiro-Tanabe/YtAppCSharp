using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class SupplierModelTest : MemorieDeFleursTestBase
    {
        private const string expectedName = "農園1";
        private const string expectedAddress = "住所1";

        private MemorieDeFleursModel Model { get; set; }

        public SupplierModelTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;
        }

        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDB);
        }

        private void CleanupModel(object sender ,EventArgs unused)
        {
            ClearAll();
        }

        [TestMethod]
        public void CanRegisterNewSupplier()
        {
            var created = Model.SupplierModel.GetSupplierBuilder()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();

            var found = Model.SupplierModel.Find(created.Code);
            Assert.IsNotNull(found);
            Assert.AreEqual(expectedName, found.Name);
            Assert.AreEqual(expectedAddress, found.Address1);
        }

        [TestMethod]
        public void CannotAbortWhenNoSuppliersFoundFromDb()
        {
            // キーに負数は入れない仕様なので、負数のキーは常に見つからないはず。
            Assert.IsNull(Model.SupplierModel.Find(-1));
        }

        [TestMethod]
        public void OrderToSupplier_RelationShipToOrderAndDetails()
        {
            var expectedSupplier = Model.SupplierModel.GetSupplierBuilder()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();
            var expectedPart = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .QauntityParLotIs(100)
                .LeadTimeIs(1)
                .ExpiryDateIs(3)
                .Create();
            var expectedOrder = Model.SupplierModel.GetOrderToSupplierBuilder()
                .SupplierTo(expectedSupplier)
                .OrderAt(DateConst.April30th)
                .DerivalyAt(DateConst.May2nd)
                .Order(expectedPart, 2)
                .Create();

            var expectedOrderID = $"{DateConst.April30th.ToString("yyyyMMdd")}-000001";
            Assert.AreEqual(expectedOrderID, expectedOrder.ID);

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var actualOrder = context.OrdersToSuppliers.Find(expectedOrder.ID);
                var actualDetails = context.OrderDetailsToSuppliers.ToList();// Where(o => o.OrderToSupplierID == expectedOrder.ID);

                Assert.AreEqual(expectedOrder.OrderDate, actualOrder.OrderDate);
                Assert.IsNotNull(actualOrder.Details);
                Assert.AreEqual(1, actualOrder.Details.Count());
                Assert.AreEqual(expectedOrder.ID, actualOrder.Details[0].OrderToSupplierID);
                Assert.AreEqual(expectedPart.Code, actualOrder.Details[0].PartsCode);
                Assert.AreEqual(2, actualOrder.Details[0].LotCount);
                Assert.AreEqual(1, actualOrder.Details[0].InventoryLotNo);
            }
        }
    }
}
