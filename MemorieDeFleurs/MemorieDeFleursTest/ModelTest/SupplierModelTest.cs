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
    public class SupplierModelTest : MemorieDeFleursDbContextTestBase
    {
        private MemorieDeFleursModel Model { get; set; }

        public SupplierModelTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
        }

        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDBContext);
        }

        [TestMethod]
        public void CanRegisterNewUser()
        {
            string expectedName = "染井花卉農園";
            string expectedAddress = "千葉県香取郡多古町染井";

            var created = Model.SupplierModel.Entity<Supplier>()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();

            var found = Model.SupplierModel.Find(created.Code);
            Assert.IsNotNull(found);
            Assert.AreEqual(expectedName, found.Name);
            Assert.AreEqual(expectedAddress, found.Address1);
        }
    }
}
