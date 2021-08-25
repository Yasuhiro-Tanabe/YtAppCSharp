using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace GameOfLife.Core.Test
{
    [TestClass]
    public class FieldTest
    {
        [TestMethod]
        public void FieldIsCreatableWithFieldSize()
        {
            var width = 10u;
            var height = 12u;
            var field = new Field(width, height);

            Assert.AreEqual(width, field.Width);
            Assert.AreEqual(height, field.Height);
        }

        [TestMethod,ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenWidthIsZero()
        {
            var width = 0u;
            var height = 10u;
            var field = new Field(width, height);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenHeightIsZero()
        {
            var width = 10u;
            var height = 0u;
            var field = new Field(width, height);
        }
    }
}
