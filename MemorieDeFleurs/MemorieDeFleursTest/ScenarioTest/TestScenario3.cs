using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest;
using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ScenarioTest
{
    /// <summary>
    /// 毎日少しずつ破棄が発生するパターン
    /// </summary>
    [TestClass]
    public class TestScenario3 : ScenarioTestBase
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
        public void AtApril30_BA001AndBA002Discarded()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.April30th);

            Model.SupplierModel.OrdersAreArrived(DateConst.April30th, orders.ToArray());
            Model.BouquetModel.DiscardBouquetParts(DateConst.April30th, Tuple.Create("BA001", 6), Tuple.Create("BA002", 8));
            Model.CustomerModel.ShipAllBouquets(DateConst.April30th);

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(200).Used(20, 174).Discarded(6)
                        .At(DateConst.May3rd).Discarded(24)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(100).Used(9, 83).Discarded(8)
                        .At(DateConst.May3rd).Discarded(20)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        /// <summary>
        /// CN002を除く全品に多少の在庫破棄が発生
        /// 
        /// CN002 は破棄すると5/2に在庫不足発生、発注が間に合わないので本テストでは破棄しない。
        /// 発注リードタイムが長い(3日)ので、本来なら少し多めに在庫を確保するはず。
        /// </summary>
        [TestMethod]
        public void AtMay1st_ManyPartsDiscarded()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May1st);

            Model.SupplierModel.OrdersAreArrived(DateConst.May1st, orders.ToArray());
            Model.BouquetModel.DiscardBouquetParts(DateConst.May1st, Tuple.Create("BA001", 10), Tuple.Create("BA002", 5), Tuple.Create("BA003", 7),
                Tuple.Create("GP001", 1), Tuple.Create("CN001", 3));
            Model.CustomerModel.ShipAllBouquets(DateConst.May1st);

            // BA003 が 5/6に在庫不足(4本)、追加発注する
            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(50, 114).Discarded(10)
                        .At(DateConst.May3rd).Discarded(14)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA002")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(100).Used(9, 83).Discarded(8)
                        .At(DateConst.May1st).Used(23, 55).Discarded(5)
                        .At(DateConst.May3rd).Discarded(15)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(100).Used(15, 85)
                        .At(DateConst.May1st).Used(23,55).Discarded(7)
                        .At(DateConst.May2nd).Used(45,10)
                        .At(DateConst.May3rd).Used(10,0).Discarded(0)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May3rd).Arrived(200).Used(14, 186)
                        .At(DateConst.May4th).Used(103, 83)
                        .At(DateConst.May5th).Used(45,38)
                        .At(DateConst.May6th).Used(38,0).Discarded(0).Shortage(4)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(18,22).Discarded(1)
                        .At(DateConst.May2nd).Used(22, 0).Discarded(0)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(50).Used(11, 39)
                        .At(DateConst.May4th).Discarded(9)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May1st).Used(22, 1).Discarded(3)
                        .At(DateConst.May2nd).Used(1, 0)
                        .At(DateConst.May5th).Discarded(0)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(50).Used(5,45)
                        .At(DateConst.May3rd).Used(24, 21)
                        .At(DateConst.May5th).Used(10, 11)
                        .At(DateConst.May6th).Used(8,3)
                        .At(DateConst.May7th).Used(0,3).Discarded(3)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();

            var supplier1 = Model.SupplierModel.FindSupplier(1);
            var ba003 = Model.BouquetModel.FindBouquetPart("BA003");
            Model.SupplierModel.Order(DateConst.May1st, supplier1, DateConst.May5th, new List<Tuple<BouquetPart, int>>() { Tuple.Create(ba003, 1) });

            // 追加発注により在庫不足が解消される
            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.April30th).Arrived(100).Used(15, 85)
                        .At(DateConst.May1st).Used(23, 55).Discarded(7)
                        .At(DateConst.May2nd).Used(45, 10)
                        .At(DateConst.May3rd).Used(10, 0).Discarded(0)
                        .END
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May3rd).Arrived(200).Used(14, 186)
                        .At(DateConst.May4th).Used(103, 83)
                        .At(DateConst.May5th).Used(45, 38)
                        .At(DateConst.May6th).Used(38, 0).Discarded(0)
                            .NotContainsActionType(InventoryActionType.SHORTAGE)
                        .END
                    .Lot(DateConst.May5th).BEGIN
                        .At(DateConst.May5th).Arrived(100)
                        .At(DateConst.May6th).Used(4, 96)
                        .At(DateConst.May8th).Discarded(96)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();

        }

        [TestMethod]
        public void AtMay2nd_GP001Discarded()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May2nd);

            Model.SupplierModel.OrdersAreArrived(DateConst.May2nd, orders.ToArray());
            Model.BouquetModel.DiscardBouquetParts(DateConst.May2nd, Tuple.Create("GP001", 2));
            Model.CustomerModel.ShipAllBouquets(DateConst.May2nd);

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("GP001")).BEGIN
                    .Lot(DateConst.April30th).BEGIN
                        .At(DateConst.May2nd).Used(22, 0).Discarded(0)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                            .NotContainsActionType(InventoryActionType.SCHEDULED_TO_DISCARD)
                        .END
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May2nd).Arrived(50).Used(11, 37).Discarded(2)
                            .ContainsActionType(InventoryActionType.DISCARDED)
                        .At(DateConst.May4th).Discarded(7)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay3rd_CN002Discarded()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May3rd);

            Model.SupplierModel.OrdersAreArrived(DateConst.May3rd, orders.ToArray());
            Model.BouquetModel.DiscardBouquetParts(DateConst.May3rd, Tuple.Create("CN001", 3), Tuple.Create("CN002", 7));
            Model.CustomerModel.ShipAllBouquets(DateConst.May3rd);

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN001")).BEGIN
                    .Lot(DateConst.May2nd).BEGIN
                        .At(DateConst.May3rd).Used(24, 18).Discarded(3)
                        .At(DateConst.May5th).Used(10, 8)
                        .At(DateConst.May6th).Used(8, 0)
                        .At(DateConst.May7th).Discarded(0)
                        .END
                    .END
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("CN002")).BEGIN
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May3rd).Arrived(50).Used(30, 13).Discarded(7)
                        .At(DateConst.May5th).Used(13, 0)
                        .At(DateConst.May8th).Discarded(0)
                        .END
                    .Lot(DateConst.May5th).BEGIN
                        .At(DateConst.May5th).Arrived(50).Used(12, 38)
                        .At(DateConst.May6th).Used(38, 0)
                        .At(DateConst.May5th.AddDays(5)).Discarded(0)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay4th_BA003Discarded()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May4th);

            Model.SupplierModel.OrdersAreArrived(DateConst.May4th, orders.ToArray());
            Model.BouquetModel.DiscardBouquetParts(DateConst.May4th, Tuple.Create("BA003", 15));
            Model.CustomerModel.ShipAllBouquets(DateConst.May4th);

            InventoryActionValidator.NewInstance()
                .BouquetPartIs(Model.BouquetModel.FindBouquetPart("BA003")).BEGIN
                    .Lot(DateConst.May3rd).BEGIN
                        .At(DateConst.May4th).Used(103, 68).Discarded(15)
                        .At(DateConst.May5th).Used(45, 23)
                        .At(DateConst.May6th).Used(23, 0).Discarded(0)
                        .END
                    .Lot(DateConst.May5th).BEGIN
                        .At(DateConst.May5th).Arrived(100)
                        .At(DateConst.May6th).Used(19, 81)
                        .At(DateConst.May8th).Discarded(81)
                        .END
                    .END
                .TargetDBIs(TestDB)
                .AssertAll();
        }

        [TestMethod]
        public void AtMay5th_NoPartsDiscarded()
        {
            var orders = Model.SupplierModel.FindAllOrdersAt(DateConst.May5th);

            Model.SupplierModel.OrdersAreArrived(DateConst.May5th, orders.ToArray());
            Model.CustomerModel.ShipAllBouquets(DateConst.May5th);
        }
    }
}
