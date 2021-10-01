using MemorieDeFleurs.Models;

using System;

namespace MemorieDeFleursTest.ModelTest
{
    public class MemorieDeFleursModelTestBase : MemorieDeFleursTestBase
    {
        protected MemorieDeFleursModel Model { get; private set; }

        public MemorieDeFleursModelTestBase() : base()
        {
            AfterTestBaseInitializing += PrepareModel;
            BeforeTestBaseCleaningUp += CleanupModel;
        }

        private void PrepareModel(object sender, EventArgs unused)
        {
            Model = new MemorieDeFleursModel(TestDB);
        }

        private void CleanupModel(object sender, EventArgs unused)
        {
            ClearAll();
        }
    }
}
