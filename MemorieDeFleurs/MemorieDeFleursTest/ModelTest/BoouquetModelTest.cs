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
    }
}
