using MemorieDeFleurs.Databese.SQLite;
using MemorieDeFleurs.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    public class CustomerModelTest : MemorieDeFleursModelTestBase
    {
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

        [TestMethod]
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

            using (var context = new MemorieDeFleursDbContext(TestDB))
            {
                var actualShippingAddress = context.ShippingAddresses.Single();
                var actualCustomer = context.Customers.Find(customer.ID);

                Assert.AreEqual(expected.Name, actualShippingAddress.Name);
                Assert.IsNotNull(actualShippingAddress.Customer);
                Assert.AreEqual(customer.ID, actualShippingAddress.Customer.ID);

                Assert.IsNotNull(actualCustomer.ShippingAddresses);
                Assert.AreEqual(1, actualCustomer.ShippingAddresses.Count());
                Assert.AreEqual(actualShippingAddress.ID, actualCustomer.ShippingAddresses[0].ID);
            }
        }
    }
}
