using MemorieDeFleurs.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class CustomerModelTest : MemorieDeFleursDbContextTestBase
    {
        private MemorieDeFleursModel Model { get; set; }

        public CustomerModelTest() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;
        }

        #region TestInitialize
        public void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDBContext);
        }
        #endregion // TestInitialize

        #region TestTerminate
        public void CleanupModel(object sender, EventArgs unused)
        {
            ClearAll();
        }
        #endregion // TestTerminate

        [TestMethod]
        public void CanAddCustomerViaModel()
        {
            var model = new MemorieDeFleursModel(TestDBContext);

            var expected = model.CustomerModel.GetCustomerBuilder()
                .NameIs("蘇我幸恵")
                .PasswordIs("sogayukie12345")
                .EmailAddressIs("ysoga@localdomain")
                .CardNoIs("9876543210123210")
                .Create();

            var actual = model.CustomerModel.Find(expected.ID);

            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.Password, actual.Password);
        }
    }
}
