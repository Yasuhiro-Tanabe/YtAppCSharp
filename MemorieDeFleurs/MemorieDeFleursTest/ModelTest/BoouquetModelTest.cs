
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class BoouquetModelTest : MemorieDeFleursModelTestBase
    {
        [TestMethod]
        public void CanAddBouquetViaModel()
        {
            var expected = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Create();

            var actual = Model.BouquetModel.FindBouquet(expected.Code);

            Assert.AreEqual(expected.Code, actual.Code);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod]
        public void CanAddBouquetPartsToBouquetViaModel()
        {
            var expectedBouquet = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Create();

            var expectedPart = Model.BouquetModel.GetBouquetPartBuilder()
                .PartCodeIs("BA001")
                .PartNameIs("薔薇(赤)")
                .LeadTimeIs(1)
                .QauntityParLotIs(100)
                .ExpiryDateIs(3)
                .Create();

            Model.BouquetModel.AppendPartsTo(expectedBouquet.Code, expectedPart.Code, 4);

            var actualBouquet = Model.BouquetModel.FindBouquet(expectedBouquet.Code);

            Assert.AreEqual(1, actualBouquet.PartsList.Count());
            Assert.AreEqual(expectedPart.Code, actualBouquet.PartsList[0].Part.Code);
            Assert.AreEqual(expectedBouquet.Code, actualBouquet.PartsList[0].Bouquet.Code);
            Assert.AreEqual(4, actualBouquet.PartsList[0].Quantity);
        }

        [TestMethod]
        public void CanAddBouquetPartsToBouquetViaBuilder()
        {
            var partsBuilder = Model.BouquetModel.GetBouquetPartBuilder();

            var expectedBouquet = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT002")
                .NameIs("花束-Bセット")
                .Uses(partsBuilder
                        .PartCodeIs("BA001")
                        .PartNameIs("薔薇(赤)")
                        .LeadTimeIs(1)
                        .QauntityParLotIs(100)
                        .ExpiryDateIs(3)
                        .Create()
                    , 3)
                .Uses(partsBuilder
                        .PartCodeIs("BA002")
                        .PartNameIs("薔薇(白)")
                        .LeadTimeIs(1)
                        .QauntityParLotIs(100)
                        .ExpiryDateIs(3)
                        .Create()
                    , 5)
                .Uses(partsBuilder
                        .PartCodeIs("BA003")
                        .PartNameIs("薔薇(ピンク)")
                        .LeadTimeIs(1)
                        .QauntityParLotIs(100)
                        .ExpiryDateIs(3)
                        .Create()
                    , 3)
                .Uses(partsBuilder
                        .PartCodeIs("GP001")
                        .PartNameIs("かすみ草")
                        .LeadTimeIs(2)
                        .QauntityParLotIs(50)
                        .ExpiryDateIs(2)
                        .Create()
                    , 6)
                .Create();
            var expectedUsedParts = new Dictionary<string, int>()
            {
                { "BA001", 3 }, { "BA002", 5 }, { "BA003", 3 }, { "GP001", 6 }
            };


            var actualBouquet = Model.BouquetModel.FindBouquet(expectedBouquet.Code);

            Assert.AreEqual(4, actualBouquet.PartsList.Count());

            foreach (var kv in expectedUsedParts)
            {
                // モデル経由で登録したわけではないが、念のためすべて登録されていることを確認
                Assert.IsNotNull(Model.BouquetModel.FindBouquetPart(kv.Key), $"単品未登録：{kv.Key}");

                var actual = actualBouquet.PartsList.SingleOrDefault(p => p.PartsCode == kv.Key);
                Assert.IsNotNull(actual, $"商品構成に登録されていない：{kv.Key}");
                Assert.AreEqual(kv.Value, actual.Quantity, $"商品構成の数が合わない：{kv.Key}");
            }

        }

        [TestMethod]
        public void NoBouquetPartsFound()
        {
            Assert.AreEqual(0, Model.BouquetModel.FindAllBoueuqtParts().Count());
        }

        [TestMethod]
        public void NoBouquetsFound()
        {
            Assert.AreEqual(0, Model.BouquetModel.FindAllBouquets().Count());
        }

        [TestMethod]
        public void FindAllBouquetsAndParts()
        {
            var expectedParts = new Dictionary<string, BouquetPart>()
            {
                {"BA001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA001").PartNameIs("薔薇(赤)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"BA002", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA002").PartNameIs("薔薇(白)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"BA003", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA003").PartNameIs("薔薇(ピンク)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"GP001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("GP001").PartNameIs("かすみ草").LeadTimeIs(2).QauntityParLotIs(50).ExpiryDateIs(2).Create() },
            };
            var expectedBouquets = new Dictionary<string, Bouquet>()
            {
                {"HT001", Model.BouquetModel.GetBouquetBuilder().CodeIs("HT001").NameIs("花束-Aセット").Uses("BA001", 4).Create() },
                {"HT002", Model.BouquetModel.GetBouquetBuilder().CodeIs("HT002").NameIs("花束-Aセット").Uses("BA001", 3).Uses("BA002", 5).Uses("BA003", 3).Uses("GP001", 6).Create() }
            };

            var actualParts = Model.BouquetModel.FindAllBoueuqtParts();
            Assert.AreEqual(expectedParts.Count(), actualParts.Count(), "登録した単品数量が一致しない");
            foreach (var parts in expectedParts) { Assert.IsTrue(actualParts.Any(p => p.Code == parts.Key), $"単品が登録されていない：{parts.Value.Name}"); }
            foreach (var parts in actualParts) { Assert.IsTrue(expectedParts.ContainsKey(parts.Code), $"登録されていないはずの単品がある：{parts.Name}"); }

            var actualBouquets = Model.BouquetModel.FindAllBouquets();
            Assert.AreEqual(expectedBouquets.Count(), actualBouquets.Count(), "登録した商品数量が一致しない");
            foreach (var bouquet in expectedBouquets) { Assert.IsTrue(actualBouquets.Any(b => b.Code == bouquet.Key), $"商品が登録されていない：{bouquet.Value.Name}"); }
            foreach (var bouquet in actualBouquets) { Assert.IsTrue(expectedBouquets.ContainsKey(bouquet.Code), $"登録されていないはずの商品がある：{bouquet.Name}"); }
        }

        [TestMethod]
        public void RemoveBouquetParts_UnusedPartsAreRemovable()
        {
            var expectedParts = new Dictionary<string, BouquetPart>()
            {
                {"BA001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA001").PartNameIs("薔薇(赤)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"BA002", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA002").PartNameIs("薔薇(白)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"BA003", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA003").PartNameIs("薔薇(ピンク)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"GP001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("GP001").PartNameIs("かすみ草").LeadTimeIs(2).QauntityParLotIs(50).ExpiryDateIs(2).Create() },
            };
            var expectedBouquets = new Dictionary<string, Bouquet>()
            {
                {"HT001", Model.BouquetModel.GetBouquetBuilder().CodeIs("HT001").NameIs("花束-Aセット").Uses("BA001", 4).Create() },
            };

            Model.BouquetModel.RemoveBouquetParts("BA002");
            var actualParts = Model.BouquetModel.FindAllBoueuqtParts();

            Assert.AreEqual(expectedParts.Count - 1, actualParts.Count());
            Assert.AreEqual(0, actualParts.Count(p => p.Code == "BA002"));
        }

        [TestMethod]
        public void RemoveBouquetParts_UsedPartsAreNotRemovable()
        {
            var expectedParts = new Dictionary<string, BouquetPart>()
            {
                {"BA001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA001").PartNameIs("薔薇(赤)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"BA002", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA002").PartNameIs("薔薇(白)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"BA003", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA003").PartNameIs("薔薇(ピンク)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"GP001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("GP001").PartNameIs("かすみ草").LeadTimeIs(2).QauntityParLotIs(50).ExpiryDateIs(2).Create() },
            };
            var expectedBouquets = new Dictionary<string, Bouquet>()
            {
                {"HT001", Model.BouquetModel.GetBouquetBuilder().CodeIs("HT001").NameIs("花束-Aセット").Uses("BA001", 4).Create() },
            };

            Assert.ThrowsException<ApplicationException>(() => Model.BouquetModel.RemoveBouquetParts("BA001"));

            // 削除処理が行われていないことを確認する
            var actualParts = Model.BouquetModel.FindAllBoueuqtParts();
            Assert.AreEqual(expectedParts.Count, actualParts.Count());
            Assert.AreEqual(1, actualParts.Count(p => p.Code == "BA001"));
        }

        [TestMethod]
        public void RemoveBouquets_NoOrdersFromCustomer()
        {
            var parts = new Dictionary<string, BouquetPart>()
            {
                {"BA001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA001").PartNameIs("薔薇(赤)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"BA002", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA002").PartNameIs("薔薇(白)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"BA003", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("BA003").PartNameIs("薔薇(ピンク)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create() },
                {"GP001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("GP001").PartNameIs("かすみ草").LeadTimeIs(2).QauntityParLotIs(50).ExpiryDateIs(2).Create() },
                {"CN001", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("CN001").PartNameIs("カーネーション(赤)").LeadTimeIs(3).QauntityParLotIs(20).ExpiryDateIs(5).Create() },
                {"CN002", Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs("CN002").PartNameIs("カーネーション(ピンク)").LeadTimeIs(3).QauntityParLotIs(20).ExpiryDateIs(5).Create() },
            };
            var expectedBouquets = new Dictionary<string, Bouquet>()
            {
                {"HT001", Model.BouquetModel.GetBouquetBuilder().CodeIs("HT001").NameIs("花束-Aセット").Uses("BA001", 4).Create() },
                {"HT002", Model.BouquetModel.GetBouquetBuilder().CodeIs("HT002").NameIs("花束-Aセット").Uses("BA001", 3).Uses("BA002", 5).Uses("BA003", 3).Uses("GP001", 6).Create() },
                {"HT003", Model.BouquetModel.GetBouquetBuilder().CodeIs("HT004").NameIs("結婚式用ブーケ").Uses("BA002", 3).Uses("BA003", 5).Uses("GP001", 3).Uses("CN002", 3).Create() },
            };

            Model.BouquetModel.RemoveBouquet("HT001");
            var actualBouquets = Model.BouquetModel.FindAllBouquets();

            Assert.AreEqual(expectedBouquets.Count - 1, actualBouquets.Count());
            Assert.AreEqual(0, actualBouquets.Count(b => b.Code == "HT001"));
        }

        [TestMethod]
        public void Save_NewBouquetPart()
        {
            var code = "BA001";
            var name = "薔薇(赤)";
            var parts = new BouquetPart()
            {
                Code = code,
                Name = name,
                LeadTime = 1,
                QuantitiesPerLot = 100,
                ExpiryDate = 3
            };

            Model.BouquetModel.Save(parts);

            var actual = Model.BouquetModel.FindBouquetPart(code);

            Assert.AreEqual(name, actual.Name);
        }

        [TestMethod]
        public void Save_BouquetPartNameChanged()
        {
            var code = "BA001";
            var MODIFIED_NAME = "薔薇(バラ色)";

            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(code).PartNameIs("薔薇(赤)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create();

            var parts = new BouquetPart()
            {
                Code = code,
                Name = MODIFIED_NAME,
                LeadTime = 1,
                QuantitiesPerLot = 100,
                ExpiryDate = 3
            };

            Model.BouquetModel.Save(parts);

            var actual = Model.BouquetModel.FindBouquetPart(code);

            Assert.AreEqual(MODIFIED_NAME, actual.Name);
        }

        private class PartsListComparer : IEqualityComparer<BouquetPartsList>
        {
            public bool Equals(BouquetPartsList x, BouquetPartsList y)
            {
                return x.BouquetCode == y.BouquetCode ? x.PartsCode == y.PartsCode : true;
            }

            public int GetHashCode([DisallowNull] BouquetPartsList obj)
            {
                return (obj.BouquetCode + obj.PartsCode).GetHashCode();
            }
        }

        [TestMethod]
        public void Save_NewBouquet()
        {
            var ba001 = "BA001";

            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(ba001).PartNameIs("薔薇(赤)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create();

            var ht001 = "HT001";
            var quantity = 4;
            var bouquet = new Bouquet()
            {
                Code = ht001,
                Name = "花束-Aセット",
                LeadTime = 1
            };
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht001, PartsCode = ba001, Quantity = quantity });

            Model.BouquetModel.Save(bouquet);

            var actual = Model.BouquetModel.FindBouquet(ht001);
            Assert.AreEqual(bouquet.Name, actual.Name, "花束名称が一致しない");
            Assert.AreEqual(bouquet.PartsList.Count, actual.PartsList.Count, "花束を構成する単品の種類数が一致しない");
            Assert.AreEqual(ba001, actual.PartsList[0].PartsCode, $"花束を構成する単品が {ba001} ではない");
            Assert.AreEqual(quantity, actual.PartsList[0].Quantity, $"単品 {ba001} の使用数が一致しない");

            var otherParts = actual.PartsList.Except(bouquet.PartsList, new PartsListComparer());
            Assert.AreEqual(0, otherParts.Count(), $"余計な単品が花束構成要素として登録されている：{string.Join(",", otherParts.Select(p => p.PartsCode))}");
        }

        [TestMethod]
        public void Save_PartsListRemoved()
        {
            var ba002 = "BA002";
            var ba003 = "BA003";
            var gp001 = "GP001";
            var cn002 = "CN002";
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(ba002).PartNameIs("薔薇(白)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create();
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(ba003).PartNameIs("薔薇(ピンク)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create();
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(gp001).PartNameIs("かすみ草").LeadTimeIs(2).QauntityParLotIs(50).ExpiryDateIs(2).Create();
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(cn002).PartNameIs("カーネーション(ピンク)").LeadTimeIs(3).QauntityParLotIs(20).ExpiryDateIs(5).Create();

            var ht004 = "HT004";
            var name = "結婚式用ブーケ";
            Model.BouquetModel.GetBouquetBuilder().CodeIs(ht004).NameIs(name).Uses(ba002, 3).Uses(ba003, 5).Uses(gp001, 3).Uses(cn002, 3).Create();

            var bouquet = new Bouquet()
            {
                Code = ht004,
                Name = name,
                LeadTime = 5,
            };
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht004, PartsCode = ba002, Quantity = 3 });
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht004, PartsCode = gp001, Quantity = 3 });
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht004, PartsCode = cn002, Quantity = 3 });

            Model.BouquetModel.Save(bouquet);

            var actual = Model.BouquetModel.FindBouquet(ht004);

            Assert.AreEqual(bouquet.PartsList.Count, actual.PartsList.Count, "商品を構成する単品の種類数が一致しない");
            foreach(var parts in bouquet.PartsList)
            {
                Assert.IsNotNull(actual.PartsList.SingleOrDefault(p => p.PartsCode == parts.PartsCode), $"単品 {parts.PartsCode} が登録されていない");
            }
            foreach(var parts in actual.PartsList)
            {
                Assert.IsNotNull(bouquet.PartsList.SingleOrDefault(p => p.PartsCode == parts.PartsCode), $"想定外の単品 {parts.PartsCode} が登録されている");
            }
        }

        [TestMethod]
        public void Save_PartsListAdded()
        {
            var ba001 = "BA001";
            var ba002 = "BA002";
            var ba003 = "BA003";
            var gp001 = "GP001";
            var cn002 = "CN002";
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(ba001).PartNameIs("薔薇(赤)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create();
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(ba002).PartNameIs("薔薇(白)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create();
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(ba003).PartNameIs("薔薇(ピンク)").LeadTimeIs(1).QauntityParLotIs(100).ExpiryDateIs(3).Create();
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(gp001).PartNameIs("かすみ草").LeadTimeIs(2).QauntityParLotIs(50).ExpiryDateIs(2).Create();
            Model.BouquetModel.GetBouquetPartBuilder().PartCodeIs(cn002).PartNameIs("カーネーション(ピンク)").LeadTimeIs(3).QauntityParLotIs(20).ExpiryDateIs(5).Create();

            var ht004 = "HT004";
            var name = "結婚式用ブーケ";
            Model.BouquetModel.GetBouquetBuilder().CodeIs(ht004).NameIs(name).Uses(ba002, 3).Uses(ba003, 5).Uses(gp001, 3).Uses(cn002, 3).Create();

            var bouquet = new Bouquet()
            {
                Code = ht004,
                Name = name,
                LeadTime = 5,
            };
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht004, PartsCode = ba001, Quantity = 5 });
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht004, PartsCode = ba002, Quantity = 5 });
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht004, PartsCode = ba003, Quantity = 5 });
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht004, PartsCode = gp001, Quantity = 5 });
            bouquet.PartsList.Add(new BouquetPartsList() { BouquetCode = ht004, PartsCode = cn002, Quantity = 5 });

            Model.BouquetModel.Save(bouquet);

            var actual = Model.BouquetModel.FindBouquet(ht004);

            Assert.AreEqual(bouquet.PartsList.Count, actual.PartsList.Count, "商品を構成する単品の種類数が一致しない");
            foreach (var parts in bouquet.PartsList)
            {
                Assert.IsNotNull(actual.PartsList.SingleOrDefault(p => p.PartsCode == parts.PartsCode), $"単品 {parts.PartsCode} が登録されていない");
            }
            foreach (var parts in actual.PartsList)
            {
                Assert.IsNotNull(bouquet.PartsList.SingleOrDefault(p => p.PartsCode == parts.PartsCode), $"想定外の単品 {parts.PartsCode} が登録されている");
            }
        }
    }
}
