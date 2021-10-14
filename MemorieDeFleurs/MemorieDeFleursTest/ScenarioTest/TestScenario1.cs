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
                        .At(DateConst.April30th).Arrived(200)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(100)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(100)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(50)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(50)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(50)
                            .ContainsActionType(InventoryActionType.ARRIVED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

    }
}
