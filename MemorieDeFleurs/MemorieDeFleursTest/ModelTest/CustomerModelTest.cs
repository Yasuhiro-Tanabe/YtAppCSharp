using MemorieDeFleurs.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class CustomerModelTest : MemorieDeFleursTestBase
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
            Model = new MemorieDeFleursModel(TestDB);
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
            var model = new MemorieDeFleursModel(TestDB);

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

        public void CanAddShippingAddressFromCustomerViaBuilder()
        {
            var model = new MemorieDeFleursModel(TestDB);

            var customer = model.CustomerModel.GetCustomerBuilder()
                .NameIs("蘇我幸恵")
                .PasswordIs("sogayukie12345")
                .EmailAddressIs("ysoga@localdomain")
                .CardNoIs("9876543210123210")
                .Create();

            var expected = Model.CustomerModel.GetShippingAddressBuilder()
                .From(customer)
                .To("ピアノ生徒1")
                .AddressIs("東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                .Create();

            // 正しく登録されていると確認できればよい
            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var actual = context.ShippingAddresses.Single();

                Assert.AreEqual(expected.Name, actual.Name);
                Assert.IsNotNull(actual.From);
                Assert.AreEqual(customer.ID, actual.From.ID);
            }
        }
    }
}
