using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest;
using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace MemorieDeFleursTest.ScenarioTest
{
    /// <summary>
    /// テストシナリオ1：納品時の欠品も予定本数以上の破棄も追加発注受注もなく淡々と入荷/加工/出荷が行われた場合
    /// </summary>
    [TestClass]
    public class TestScenario1 : ScenarioTestBase
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext unused)
        {
            OpenDatabase();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CloseDatabase();
        }

        [TestMethod]
        public void AtApril30th()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.April30th);

            Model.SupplierModel.OrdersAreArrived(DateConst.April30th, orders.ToArray());
            Model.CustomerModel.ShipAllBouquets(DateConst.April30th);

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(200).Used(20, 180)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(100).Used(9, 91)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(100).Used(15, 85)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(50).Used(9, 41)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(50).Used(24, 26)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(50).Used(33, 17)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay1st()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May1st);

            Model.SupplierModel.OrdersAreArrived(DateConst.May1st, orders.ToArray());
            Model.CustomerModel.ShipAllBouquets(DateConst.May1st);

            DEBUGLOG_ShowInventoryActions(TestDB, "CN002");

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(50, 130)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May1st).Arrived(300).Used(0, 300)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(23, 68)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(23, 62)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(18, 23)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(22, 4)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(17, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May1st).Arrived(50).Used(23, 27)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
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

            DEBUGLOG_ShowInventoryActions(TestDB, "CN002");

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May2nd).Used(80, 50)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May2nd).Used(0, 300)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(200).Used(0, 200)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May2nd).Used(34, 34)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May2nd).Used(45, 17)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May2nd).Used(23, 0).Discarded(0)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(50).Used(10, 40)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May2nd).Used(4, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(50).Used(2, 48)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May2nd).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May2nd).Used(27, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
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
            Model.BouquetModel.DiscardBouquetParts(DateConst.May3rd, Tuple.Create("BA001", 30), Tuple.Create("BA002", 28), Tuple.Create("BA003", 0));

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May3rd).Used(20, 30).Discarded(30)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May3rd).Used(0, 300)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May3rd).Used(0, 200)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May3rd).Arrived(200).Used(0, 200)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May3rd).Used(6, 28).Discarded(28)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May3rd).Arrived(100).Used(0, 100)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May3rd).Arrived(100).Used(0, 100)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May3rd).Used(17, 0).Discarded(0)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May3rd).Arrived(200).Used(7, 193)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May3rd).Used(30, 10)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May3rd).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May3rd).Used(24, 24)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May3rd).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May3rd).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May3rd).Arrived(50).Used(30, 20)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
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

            DEBUGLOG_ShowInventoryActions(TestDB, "GP001");

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May4th).Used(300, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May4th).Used(100, 100)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May4th).Used(0, 200)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May4th).Used(100, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May4th).Used(0, 100)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May4th).Used(103, 90)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May4th).Used(0, 10).Discarded(10)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May4th).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May4th).Used(0, 24)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May4th).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May4th).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May4th).Used(0, 20)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
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
            Model.BouquetModel.DiscardBouquetParts(DateConst.May5th, Tuple.Create("BA001",0), Tuple.Create("CN001", 0), Tuple.Create("CN002", 0));

            DEBUGLOG_ShowInventoryActions(TestDB, "CN002");

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May5th).Used(100, 0).Discarded(0)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May5th).Used(70, 130)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May5th).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May5th).Used(50, 50)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May5th).Used(45, 45)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.May5th).BEGIN
                        .At(DateConst.May5th).Arrived(50).Used(15,35)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May5th).Used(0, 0).Discarded(0)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May5th).Used(10, 14)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May5th).Used(0, 0).Discarded(0)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May5th).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May5th).Used(20, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May5th).BEGIN
                        .At(DateConst.May5th).Arrived(50).Used(5, 45)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
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
            Model.BouquetModel.DiscardBouquetParts(DateConst.May6th, Tuple.Create("BA001", 90), Tuple.Create("CN002", 0));

            DEBUGLOG_ShowInventoryActions(TestDB, "CN002");

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May6th).Used(40, 90).Discarded(90)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May6th).BEGIN
                        .At(DateConst.May6th).Arrived(100).Used(0, 100)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.May3rd, 0).BEGIN
                        .At(DateConst.May6th).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May3rd, 1).BEGIN
                        .At(DateConst.May6th).Used(34, 16)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May6th).Used(42, 3)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.May5th).BEGIN
                        .At(DateConst.May6th).Used(30, 5)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May6th).Used(8, 6)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.May1st).BEGIN
                        .At(DateConst.May6th).Used(0, 0).Discarded(0)
                            .ContainsActionType(InventoryActionType.USED)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May6th).Used(0, 0)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .Lot(DateConst.May5th).BEGIN
                        .At(DateConst.May6th).Used(38, 7)
                            .ContainsActionType(InventoryActionType.USED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_USE)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }
    }
}
