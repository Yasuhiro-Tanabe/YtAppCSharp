﻿using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Logging;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class SupplierModelTest : MemorieDeFleursModelTestBase
    {
        private const string expectedName = "農園1";
        private const string expectedAddress = "住所1";

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

        [TestMethod]
        public void OrdersAreArrived_NotUsedYet()
        {
            var supplier = Model.SupplierModel.GetSupplierBuilder()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();
            var ba001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .QauntityParLotIs(100)
                .LeadTimeIs(1)
                .ExpiryDateIs(3)
                .Create();

            var orderNo = Model.SupplierModel.Order(DateConst.April30th, supplier, DateConst.May2nd, new[] { Tuple.Create(ba001, 2) });

            Model.SupplierModel.OrdersAreArrived(DateConst.May2nd, orderNo);

            // 入荷が確定したので入荷日変更や発注取消は実行できない
            Assert.ThrowsException<ApplicationException>(() => Model.SupplierModel.ChangeArrivalDate(orderNo, DateConst.May3rd), $"入荷日変更できないはず： {orderNo}");
            Assert.ThrowsException<ApplicationException>(() => Model.SupplierModel.CancelOrder(orderNo), $"注文取り消しできないはず： {orderNo}");
        }

        [TestMethod,ExpectedException(typeof(ApplicationException))]
        public void OrdersAreArrived_DoNotCallTwice()
        {
            var supplier = Model.SupplierModel.GetSupplierBuilder()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();
            var ba001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .QauntityParLotIs(100)
                .LeadTimeIs(1)
                .ExpiryDateIs(3)
                .Create();

            var orderNo = Model.SupplierModel.Order(DateConst.April30th, supplier, DateConst.May2nd, new[] { Tuple.Create(ba001, 2) });

            try
            {
                Model.SupplierModel.OrdersAreArrived(DateConst.May2nd, orderNo);
            }
            catch (Exception e)
            {
                // 最初の納品処理は成功するはずなので、例外が出たらテスト失敗
                Assert.Fail($"初回の {nameof(Model.SupplierModel.OrdersAreArrived)}({orderNo}) が失敗した： {e.GetType().Name}, {e.Message}");
            }

            Model.SupplierModel.OrdersAreArrived(DateConst.May2nd, orderNo); 
        }

        [TestMethod]
        public void OrdersAreArrived_TwoOrMoreOrders()
        {
            LogUtil.DEBUGLOG_BeginTest();

            var supplier1 = Model.SupplierModel.GetSupplierBuilder()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();
            var supplier2 = Model.SupplierModel.GetSupplierBuilder()
                .NameIs("木挽町花壇")
                .AddressIs("東京都中央区銀座四丁目121-5")
                .Create();

            var ba001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .QauntityParLotIs(100)
                .LeadTimeIs(1)
                .ExpiryDateIs(3)
                .Create();
            var cn001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN001")
                .PartNameIs("カーネーション(赤)")
                .LeadTimeIs(3)
                .QauntityParLotIs(20)
                .ExpiryDateIs(5)
                .Create();
            var gp001 = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("GP001")
                .PartNameIs("かすみ草")
                .LeadTimeIs(2)
                .QauntityParLotIs(5)
                .ExpiryDateIs(2)
                .Create();

            var orders = new List<string>()
            {
                Model.SupplierModel.Order(DateConst.April30th, supplier1, DateConst.May2nd, new [] { Tuple.Create(ba001, 2), Tuple.Create(gp001, 2) }),
                Model.SupplierModel.Order(DateConst.April30th, supplier1, DateConst.May3rd, new [] { Tuple.Create(ba001, 1), Tuple.Create(gp001, 3) }),
                Model.SupplierModel.Order(DateConst.April30th, supplier2, DateConst.May2nd, new [] { Tuple.Create(gp001, 2), Tuple.Create(cn001, 2) }),
            };

            Model.SupplierModel.OrdersAreArrived(DateConst.May2nd, orders[0], orders[2]);
            Model.SupplierModel.OrdersAreArrived(DateConst.May3rd, orders[1]);

            Assert.ThrowsException<ApplicationException>(() => Model.SupplierModel.CancelOrder(orders[0]));
            Assert.ThrowsException<ApplicationException>(() => Model.SupplierModel.CancelOrder(orders[1]));
            Assert.ThrowsException<ApplicationException>(() => Model.SupplierModel.CancelOrder(orders[2]));

            DEBUGLOG_ShowInventoryActions(TestDB, ba001.Code);
            DEBUGLOG_ShowInventoryActions(TestDB, gp001.Code);
            DEBUGLOG_ShowInventoryActions(TestDB, cn001.Code);

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(ba001).BEGIN
                    .Lot(DateConst.May2nd, 1).BEGIN
                        .At(DateConst.May2nd).Arrived(2 * ba001.QuantitiesPerLot)
                        .END
                    .Lot(DateConst.May3rd, 3).BEGIN
                        .At(DateConst.May3rd).Arrived(1 * ba001.QuantitiesPerLot)
                        .END
                    .END
                .BouquetPartIs(gp001).BEGIN
                    .Lot(DateConst.May2nd, 2).BEGIN
                        .At(DateConst.May2nd).Arrived(2 * gp001.QuantitiesPerLot)
                        .END
                    .Lot(DateConst.May2nd, 5).BEGIN
                        .At(DateConst.May2nd).Arrived(2 * gp001.QuantitiesPerLot)
                        .END
                    .Lot(DateConst.May3rd, 4).BEGIN
                        .At(DateConst.May3rd).Arrived(3 * gp001.QuantitiesPerLot)
                        .END
                    .END
                .BouquetPartIs(cn001).BEGIN
                    .Lot(DateConst.May2nd, 6).BEGIN
                        .At(DateConst.May2nd).Arrived(2 * cn001.QuantitiesPerLot)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();

            LogUtil.DEBUGLOG_EndTest();
        }
    }
}
