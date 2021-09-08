using MemorieDeFleurs.Entities;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class BoouquetModelTest : MemorieDeFleursDbContextTestBase
    {
        private MemorieDeFleursModel Model { get; set; }

        private Bouquet ExpectedBouquet { get; set; }
        private Customer ExpectedCustomr { get; set; }

        public BoouquetModelTest()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;
        }

        #region TestInitialize
        public  void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDBContext);
        }
        #endregion // TestInitialize

        #region TestCleanup
        public void CleanupModel(object sender, EventArgs unused)
        {
            ClearAll();
        }
        #endregion // TestCleanup

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

            Model.BouquetModel.CreatePartsList(expectedBouquet.Code, expectedPart.Code, 4);

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
    }
}
