using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameOfLife.Core.Test
{
    [TestClass]
    public class FieldTest
    {
        [TestMethod]
        public void FieldIsCreatableWithFieldSize()
        {
            var field = new Field(10, 10);
        }
    }
}
