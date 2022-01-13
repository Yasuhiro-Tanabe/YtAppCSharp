using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class InspectionTest : MemorieDeFleursModelTestBase
    {
        public InspectionTest()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;
        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            using(var context = new MemorieDeFleursDbContext(TestDB))
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    PrepareBouquetParts(context);
                    PrepareBouquets(context);
                    PrepareSuppliers(context);
                    PrepareOrdersToSupplier(context);
                    transaction.Commit();
                }
                catch(Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                LoadItems(context);
            }
        }
        private void PrepareBouquetParts(MemorieDeFleursDbContext context)
        {
            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA002")
                .PartNameIs("薔薇(白)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA003")
                .PartNameIs("薔薇(ピンク)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create(context);

            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("GP001")
                .PartNameIs("かすみ草")
                .LeadTimeIs(2)
                .QauntityParLotIs(50)
                .ExpiryDateIs(2)
                .Create(context);

            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN001")
                .PartNameIs("カーネーション(赤)")
                .LeadTimeIs(3)
                .QauntityParLotIs(50)
                .ExpiryDateIs(5)
                .Create(context);

            Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("CN002")
                .PartNameIs("カーネーション(ピンク)")
                .LeadTimeIs(3)
                .QauntityParLotIs(50)
                .ExpiryDateIs(5)
                .Create(context);
        }
        private void PrepareBouquets(MemorieDeFleursDbContext context)
        {
            Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses("BA001", 4)
                .Create(context);

            Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT002")
                .NameIs("花束-Bセット")
                .Uses("BA001", 3)
                .Uses("BA003", 3)
                .Uses("GP001", 6)
                .Create(context);

            Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT003")
                .NameIs("一輪挿し-A")
                .Uses("BA003", 1)
                .Create(context);

            Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT004")
                .NameIs("結婚式用ブーケ")
                .Uses("BA002", 3)
                .Uses("BA003", 5)
                .Uses("GP001", 3)
                .Uses("CN002", 3)
                .Create(context);

            Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT005")
                .NameIs("母の日感謝セット")
                .Uses("CN001", 6)
                .Uses("CN002", 6)
                .Create(context);

            Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT006")
                .NameIs("花束-Cセット")
                .Uses("BA001", 6)
                .Uses("BA002", 4)
                .Uses("BA003", 3)
                .Uses("GP001", 3)
                .Uses("CN001", 2)
                .Uses("CN002", 5)
                .Create(context);

            Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT007")
                .NameIs("還暦祝い60本セット")
                .Uses("BA001", 40)
                .Uses("BA002", 10)
                .Uses("BA003", 10)
                .Create(context);
        }
        private void PrepareSuppliers(MemorieDeFleursDbContext context)
        {
            Model.SupplierModel.GetSupplierBuilder()
                .NameIs("新橋園芸")
                .AddressIs("東京都中央区銀座", "銀座六丁目園芸団地21-8")
                .EmailIs("shinbashi@localdomain")
                .PhoneNumberIs("00012345678")
                .FaxNumberIs("00012345677")
                .SupplyParts("BA001", "BA002", "BA003", "GP001")
                .Create(context);

            Model.SupplierModel.GetSupplierBuilder()
                .NameIs("木挽町花壇")
                .AddressIs("東京都中央区銀座四丁目121-5")
                .EmailIs("kobiki@localdomain")
                .PhoneNumberIs("09098765432")
                .FaxNumberIs("0120000000")
                .SupplyParts("GP001", "CN001", "CN002")
                .Create(context);
        }
        private void PrepareOrdersToSupplier(MemorieDeFleursDbContext context)
        {
            var orderDate = new DateTime(DateConst.Year, 3, 10);
            var supplier1 = Model.SupplierModel.FindSupplier(1);
            var supplier2 = Model.SupplierModel.FindSupplier(2);
            var ba001 = Model.BouquetModel.FindBouquetPart("BA001");
            var ba002 = Model.BouquetModel.FindBouquetPart("BA002");
            var ba003 = Model.BouquetModel.FindBouquetPart("BA003");
            var gp001 = Model.BouquetModel.FindBouquetPart("GP001");
            var cn001 = Model.BouquetModel.FindBouquetPart("CN001");
            var cn002 = Model.BouquetModel.FindBouquetPart("CN002");

            // 仕入先1
            Model.SupplierModel.Order(context, orderDate, supplier1, DateConst.April30th,
                new[] {
                    Tuple.Create(ba001, 2),
                    Tuple.Create(ba002, 1),
                    Tuple.Create(ba003, 1),
                    Tuple.Create(gp001, 1),
                });
            
            Model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May1st,
                new[] {
                    Tuple.Create(ba001, 3),
                });
            
            Model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May2nd,
                new[] {
                    Tuple.Create(ba001, 2),
                });

            Model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May3rd,
                new[] {
                    Tuple.Create(ba001, 2),
                });

            Model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May3rd,
                new[] {
                    /* 同一日の発注が単品毎に分かれているパターン */
                    Tuple.Create(ba002, 1),
                    Tuple.Create(ba003, 2),
                });

            Model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May3rd,
                new[] {
                    /* 同一商品を同日複数回発注 : 在庫ロットが1日2ロットある状態を作る */
                    Tuple.Create(ba002, 1),
                });

            Model.SupplierModel.Order(context, orderDate, supplier1, DateConst.May6th,
                new[] {
                    Tuple.Create(ba001, 1),
                });

            // 仕入先2
            Model.SupplierModel.Order(context, orderDate, supplier2, DateConst.April30th,
                new[]
                {
                    Tuple.Create(cn001, 1),
                    Tuple.Create(cn002, 1),
                });

            Model.SupplierModel.Order(context, orderDate, supplier2, DateConst.May1st,
                new[]
                {
                    Tuple.Create(cn002, 1),
                });


            Model.SupplierModel.Order(context, orderDate, supplier2, DateConst.May2nd,
                new[]
                {
                    Tuple.Create(gp001, 1),
                    Tuple.Create(cn001, 1),
                });

            Model.SupplierModel.Order(context, orderDate, supplier2, DateConst.May3rd,
                new[]
                {
                    Tuple.Create(cn002, 1),
                });

            Model.SupplierModel.Order(context, orderDate, supplier2, DateConst.May5th,
                new[]
                {
                    Tuple.Create(gp001, 1),
                    Tuple.Create(cn002, 1),
                });
        }
        private void LoadItems(MemorieDeFleursDbContext context)
        {
            Parts = context.BouquetParts.AsEnumerable().ToDictionary(p => p.Code);
            Bouquets = context.Bouquets.AsEnumerable().ToDictionary(b => b.Code);
            Suppliers = context.Suppliers.AsEnumerable().ToDictionary(s => s.Code);

            Orders = context.OrdersToSuppliers.AsEnumerable().GroupBy(o => o.DeliveryDate).ToDictionary(g => g.Key, g => g.OrderBy(o => o.ID).Select(o => o.ID).ToArray());
        }
        #endregion // TestInitialize

        #region TestCleanup
        private void CleanupModel(object sender, EventArgs unused)
        {
            ClearAll();
        }
        #endregion // TestCleanup

        private IDictionary<DateTime, string[]> Orders { get; set; }
        private IDictionary<int, Supplier> Suppliers { get; set; }
        private IDictionary<string, Bouquet> Bouquets { get; set; }
        private IDictionary<string, BouquetPart> Parts { get; set; }

        [TestMethod]
        public void InspectOneOrder()
        {
            Model.SupplierModel.OrderIsArrived(DateConst.April30th, Orders[DateConst.April30th][0]);

            var actual = Model.SupplierModel.FindOrder(Orders[DateConst.April30th][0]);
            Assert.IsNotNull(actual);
            Assert.AreEqual(OrderToSupplierStatus.ARRIVED, actual.Status);

            var validator = InventoryActionValidator.NewInstance();

            foreach(var p in actual.Details)
            {
                validator = validator.BouquetPartIs(p.PartsCode).BEGIN
                        .Lot(DateConst.April30th).BEGIN
                            .LotNumberIs(p.InventoryLotNo)
                                .At(DateConst.April30th)
                                    .ContainsActionType(InventoryActionType.ARRIVED)
                                    .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .END
                    .END;
            }
            validator.TargetDBIs(TestDB).AssertAll();
        }

        [TestMethod]
        public void InspectAllOrdersInTheDay()
        {
            Model.SupplierModel.OrdersAreArrived(DateConst.April30th, Orders[DateConst.April30th]);

            var validator = InventoryActionValidator.NewInstance();
            foreach(var o in  Orders[DateConst.April30th])
            {
                var actual = Model.SupplierModel.FindOrder(o);
                Assert.IsNotNull(actual, $"Order:{o}");
                Assert.AreEqual(OrderToSupplierStatus.ARRIVED, actual.Status);
                foreach(var p in actual.Details)
                {
                    validator = validator.BouquetPartIs(p.PartsCode).BEGIN
                        .Lot(DateConst.April30th, 0).BEGIN // 4/30 分は単品毎に1在庫ロットしかないはずなので、indexは0番決め打ち
                            .LotNumberIs(p.InventoryLotNo)
                                .At(DateConst.April30th)
                                    .ContainsActionType(InventoryActionType.ARRIVED)
                                    .NotContainsActionType(InventoryActionType.SCHEDULED_TO_ARRIVE)
                            .END
                        .END;
                }
            }
            validator.TargetDBIs(TestDB).AssertAll();
        }
    }
}
