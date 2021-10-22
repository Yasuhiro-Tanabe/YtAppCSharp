
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
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

            foreach( var kv in expectedUsedParts)
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
    }
}
