using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class SupplierModelTest : MemorieDeFleursDbContextTestBase
    {
        private const string expectedName = "農園1";
        private const string expectedAddress = "住所1";

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
        public void CanRegisterNewSupplier()
        {
            var created = Model.SupplierModel.Entity<Supplier>()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();

            var found = Model.SupplierModel.Find(created.Code);
            Assert.IsNotNull(found);
            Assert.AreEqual(expectedName, found.Name);
            Assert.AreEqual(expectedAddress, found.Address1);
        }

        [TestMethod]
        public void CannotAbortWhenNoSuppliersFoundFromDb()
        {
            // キーに負数は入れない仕様なので、負数のキーは常に見つからないはず。
            Assert.IsNull(Model.SupplierModel.Find(-1));
        }

        [TestMethod]
        public void SupplierIsChangable()
        {
            var expectedEmailAddress = "foo@localdomain";

            var created = Model.SupplierModel.Entity<Supplier>()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();

            created.EmailAddress = expectedEmailAddress;
            Model.SupplierModel.Replace(created);

            var found = Model.SupplierModel.Find(created.Code);
            Assert.AreEqual(expectedEmailAddress, found.EmailAddress);
        }

        [TestMethod]
        public void UpdatingNullObjectHasNoEffectsForSuppliers()
        {
            var created = Model.SupplierModel.Entity<Supplier>()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();

            Model.SupplierModel.Replace(null);

            Assert.IsNotNull(Model.SupplierModel.Find(created.Code));
        }

        [TestMethod]

        public void CanRemoveExistingSupplier()
        {
            var created = Model.SupplierModel.Entity<Supplier>()
                .NameIs(expectedName)
                .AddressIs(expectedAddress)
                .Create();

            Model.SupplierModel.Remove(created);
            Assert.IsNull(Model.SupplierModel.Find(created.Code));
        }
    }
}
