using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest;
using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
