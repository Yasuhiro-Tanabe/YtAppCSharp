using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest;
using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ScenarioTest
{
    /// <summary>
    /// 入荷時の検品不良
    /// </summary>
    [TestClass]
    public class TestScenario2 : ScenarioTestBase
    {
        private static SqliteConnection TestDB { get; set; }
        private MemorieDeFleursModel Model { get; set; }

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
        }

        [TestCleanup]
        public void ReleaseModel()
        {
            Model = null;
        }
        #endregion // テストの初期化終了

        [TestMethod]
        public void AtApril30_InspectionDefectFound_BA001()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.April30th).ToArray();
            var ba001 = Model.BouquetModel.FindBouquetPart("BA001");

            Model.SupplierModel.ChangeArrivedQuantities("20200310-000001", new List<Tuple<BouquetPart, int>>() { Tuple.Create(ba001, 195) });
            Model.SupplierModel.OrdersAreArrived(DateConst.April30th, orders);
            Model.CustomerModel.ShipAllBouquets(DateConst.April30th);

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(195).Used(20, 175)
                        .At(DateConst.May1st).Used(50, 125)
                        .At(DateConst.May2nd).Used(80, 45)
                        .At(DateConst.May3rd).Used(20, 25).Discarded(25)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay1st_InspectionDefectFound_BA001()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May1st).ToArray();

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            var ba001 = Model.BouquetModel.FindBouquetPart("BA001");
            Model.SupplierModel.ChangeArrivedQuantities("20200310-000002", new List<Tuple<BouquetPart, int>>() { Tuple.Create(ba001, 120) });
            Model.SupplierModel.OrdersAreArrived(DateConst.May1st, orders);
            Model.CustomerModel.ShipAllBouquets(DateConst.May1st);

            DEBUGLOG_ShowInventoryActions(TestDB, "BA001");

            // 追加発注前：BA001 の 5/5 加工分が不足する
            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May1st).Arrived(120)
                        .At(DateConst.May4th).Used(120, 0)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(200)
                        .At(DateConst.May4th).Used(200, 0)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May3rd).Arrived(200)
                        .At(DateConst.May4th).Used(180, 20)
                        .At(DateConst.May5th).Used(20, 0).Shortage(150)
                            .ContainsActionType(InventoryActionType.SHORTAGE)
                        .At(DateConst.May6th).Used(0, 0)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();

            // 追加発注
            var supplier = Model.SupplierModel.FindSupplier(1);
            var orderNo = Model.SupplierModel.Order(DateConst.May1st, supplier, DateConst.May3rd, new List<Tuple<BouquetPart, int>>() { Tuple.Create(ba001, 2) });
            Assert.AreEqual("20200501-000001", orderNo);

            // 追加発注の結果在庫不足が解消する
            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May1st).Arrived(120)
                        .At(DateConst.May4th).Used(120, 0)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(200)
                        .At(DateConst.May4th).Used(200, 0)
                        .END
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May3rd).Arrived(200)
                        .At(DateConst.May4th).Used(180, 20)
                        .At(DateConst.May5th).Used(20, 0)
                            .NotContainsActionType(InventoryActionType.SHORTAGE)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May3rd).Arrived(200)
                        .At(DateConst.May5th).Used(150, 50)
                        .At(DateConst.May6th).Used(40, 10).Discarded(10)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay2nd()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May2nd);

            Model.SupplierModel.OrdersAreArrived(DateConst.May2nd, orders.ToArray());
            Model.CustomerModel.ShipAllBouquets(DateConst.May2nd);
            Model.BouquetModel.DiscardBouquetParts(DateConst.May2nd, Tuple.Create("GP001", 0));
            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May2nd).Used(0, 120)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(200).Used(0, 200)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay3rd()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May3rd);

            Model.SupplierModel.OrdersAreArrived(DateConst.May3rd, orders.ToArray());
            Model.CustomerModel.ShipAllBouquets(DateConst.May3rd);
            Model.BouquetModel.DiscardBouquetParts(DateConst.May3rd, Tuple.Create("BA001", 25), Tuple.Create("BA002", 28), Tuple.Create("BA003", 0));

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May3rd).Discarded(25)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May3rd).Used(0, 120)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May3rd).Used(0, 200)
                        .END
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay4th()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May4th);

            Model.SupplierModel.OrdersAreArrived(DateConst.May4th, orders.ToArray());
            Model.CustomerModel.ShipAllBouquets(DateConst.May4th);
            Model.BouquetModel.DiscardBouquetParts(DateConst.May4th, Tuple.Create("GP001", 10));

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May4th).Used(120, 0).Discarded(0)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May4th).Used(200, 0)
                        .END
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May4th).Used(180, 20)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May4th).Used(0, 200)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay5th()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May5th);

            Model.SupplierModel.OrdersAreArrived(DateConst.May5th, orders.ToArray());
            Model.CustomerModel.ShipAllBouquets(DateConst.May5th);
            Model.BouquetModel.DiscardBouquetParts(DateConst.May5th, Tuple.Create("BA001", 0), Tuple.Create("CN001", 0), Tuple.Create("CN002", 0));

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May5th).Used(0, 0).Discarded(0)
                        .END
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May5th).Used(20, 0)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May5th).Used(150, 50)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay6th()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May6th);

            Model.SupplierModel.OrdersAreArrived(DateConst.May6th, orders.ToArray());
            Model.CustomerModel.ShipAllBouquets(DateConst.May6th);
            Model.BouquetModel.DiscardBouquetParts(DateConst.May6th, Tuple.Create("BA001", 10), Tuple.Create("CN002", 0));

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May6th).Used(0, 0).Discarded(0)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May6th).Used(40, 10).Discarded(10)
                        .END
                    .Lot(DateConst.May6th).BEGIN
                        .At(DateConst.May6th).Arrived(100).Used(0, 100)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }
    }
}
