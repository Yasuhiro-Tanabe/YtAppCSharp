using MemorieDeFleurs;
using MemorieDeFleurs.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace MemorieDeFleursTest
{
    [TestClass]
    public class MemorieDeFleursDbContextTestBase : MemorieDeFleursTestBase
    {
        protected MemorieDeFleursDbContext TestDBContext { get; set; }

        public MemorieDeFleursDbContextTestBase() : base()
        {
            AfterTestBaseInitializing += SetupDBContext;
            BeforeTestBaseCleaningUp += DisposeDBContext;
        }

        private void SetupDBContext(object sender, EventArgs unused)
        {
            LogUtil.Debug($"MemorieDeFleursDbContextTestBase#SetupDBContext() is called.");
            // DbContext を継承したクラスのコンストラクタを適宜用意することで、
            // 接続文字列や DbConnection を DbContext に渡すことができる。
            TestDBContext = new MemorieDeFleursDbContext(TestDB);
        }

        private void DisposeDBContext(object sender, EventArgs unused)
        {
            LogUtil.Debug($"MemorieDeFleursDbContextTestBase#DisposeDBContext() is called.");
            // テーブル全削除はORマッピングフレームワークが持つ「DBを隠蔽する」意図にそぐわないため
            // DbContext.Customers.Clear() のような操作は用意されていない。
            // DbConnection 経由かDbContext.Database.ExecuteSqlRaw() を使い、DELETEまたはTRUNCATE文を発行すること。
            TestDBContext.Dispose();
        }
    }
}
