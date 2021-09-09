using MemorieDeFleurs.Models;
using MemorieDeFleurs.Models.Entities;

using MemorieDeFleursTest.ModelTest.Fluent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleursTest.ModelTest
{
    [TestClass]
    class AddAndRemoveOrdersFromCustomerTest: MemorieDeFleursDbContextTestBase
    {
        /// <summary>
        /// テストで使用する商品
        /// </summary>
        private Bouquet ExpectedBouquet { get; set; }

        /// <summary>
        /// テストで使用する得意先
        /// </summary>
        private Customer ExpectedCustomer { get; set; }

        /// <summary>
        /// テストで使用するお届け先
        /// </summary>
        private ShippingAddress ExpectedShippingAddress { get; set; }

        /// <summary>
        /// 検証対象モデル
        /// </summary>
        private MemorieDeFleursModel Model { get; set; }

        /// <summary>
        /// 在庫一覧：在庫ロット毎の、ロット番号と入荷(予定)数量。在庫アクションの検証に使用
        /// </summary>
        private TestOrder InitialOrders { get; } = new TestOrder();

        /// <summary>
        /// 在庫一覧：日々の在庫数に関する、テスト前の初期値。受注による在庫増減の期待値を計算するために使用
        /// </summary>
        private IDictionary<DateTime, int> InitialStocks { get; } = new Dictionary<DateTime, int>();

        public AddAndRemoveOrdersFromCustomerTest()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;
        }

        #region TestInitialize
        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDBContext);

            var partBuilder = Model.BouquetModel.GetBouquetPartBuilder();
            ExpectedBouquet = Model.BouquetModel.GetBouquetBuilder()
                .CodeIs("HT001")
                .NameIs("花束-Aセット")
                .Uses(partBuilder
                    .PartCodeIs("BA001")
                    .PartNameIs("薔薇(赤)")
                    .LeadTimeIs(1)
                    .QauntityParLotIs(100)
                    .ExpiryDateIs(3)
                    .Create(),
                    4)
                .Create();

            ExpectedCustomer = Model.CustomerModel.GetCustomerBuilder()
                .EmailAddressIs("ysoga@localdomain")
                .NameIs("蘇我幸恵")
                .PasswordIs("sogayukie12345")
                .CardNoIs("9876543210123210")
                .Create();

            ExpectedShippingAddress = Model.CustomerModel.GetShippingAddressBuilder()
                .From(ExpectedCustomer)
                .To("ピアノ生徒1")
                .AddressIs("東京都中央区京橋1-10-7", "KPP八重洲ビル10階")
                .Create();

        }
        #endregion

        #region TestCleanup
        private void CleanupModel(object sender, EventArgs unused)
        {
            ClearAll();
        }
    #endregion

    }
}
