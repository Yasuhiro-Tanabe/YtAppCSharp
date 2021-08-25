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
            var width = 10;
            var height = 12;
            var field = new Field(width, height);

            Assert.AreEqual(width, field.Width);
            Assert.AreEqual(height, field.Height);
        }

        [TestMethod,ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenWidthIsZero()
        {
            var width = 0;
            var height = 10;
            var field = new Field(width, height);
        }

        [TestMethod,ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenWidthIsMinusValue()
        {
            var width = -10;
            var height = 10;
            var field = new Field(width, height);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenHeightIsZero()
        {
            var width = 10;
            var height = 0;
            var field = new Field(width, height);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenHeightIsMinusValue()
        {
            var width = 10;
            var height = -12;
            var field = new Field(width, height);
        }
    }
}
