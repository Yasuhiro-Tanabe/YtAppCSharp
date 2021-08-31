using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace GameOfLife.Core.Test
{
    /// <summary>
    /// ライフゲームのフィールドのテスト
    /// </summary>
    [TestClass]
    public class FieldTest
    {
        /// <summary>
        /// 指定された幅/高さでフィールドを作成することができる
        /// </summary>
        [TestMethod]
        public void FieldIsCreatableWithFieldSize()
        {
            var width = 10u;
            var height = 12u;
            var field = new Field(width, height);

            Assert.AreEqual(width, field.Width);
            Assert.AreEqual(height, field.Height);
        }

        /// <summary>
        /// 幅＝0 のフィールドは作れない
        /// </summary>
        [TestMethod,ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenWidthIsZero()
        {
            var width = 0u;
            var height = 10u;
            var field = new Field(width, height);
        }

        /// <summary>
        /// 高さ＝0のフィールドアは作れない
        /// </summary>
        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenHeightIsZero()
        {
            var width = 10u;
            var height = 0u;
            var field = new Field(width, height);
        }

        /// <summary>
        /// 初期状態のフィールドは、全セルが死滅状態
        /// </summary>
        [TestMethod]
        public void AllCellsAreDeadInInitialField()
        {
            var width = 10u;
            var height = 10u;
            var field = new Field(width, height);

            var allX = Enumerable.Range(0, (int)width).Select(x => (uint)x);
            var allY = Enumerable.Range(0, (int)height).Select(x => (uint)x);

            Assert.IsTrue(allX.All(x => allY.All(y => !field[x,y])));
        }

        /// <summary>
        /// フィールドのセル単位で生存死滅を指定できる、
        /// 1つのセルで生存指定した場合、それ以外のセルは状態が死滅のまま変わらない
        /// </summary>
        [TestMethod]
        public void OneCellChangesAliveButOthersAreNotAlive()
        {
            var width = 10u;
            var height = 10u;
            var x1 = 0u;
            var y1 = 0u;
            var field = new Field(width, height);

            field[x1, y1] = true;

            foreach (var x in Enumerable.Range(0, (int)width).Select(x => (UInt32)x))
            {
                foreach (var y in Enumerable.Range(0, (int)height).Select(y => (UInt32)y))
                {
                    if (x == x1 && y == y1)
                    {
                        Assert.IsTrue(field[x, y], $"field[{x},{y}] expected true but false.");
                    }
                    else
                    {
                        Assert.IsFalse(field[x, y], $"field[{x},{y}] expected false but true.");
                    }
                }

            }
        }

        /// <summary>
        /// フィールド生成時の幅より大きいX座標は指定できない (セルの状態変更)
        /// </summary>
        [TestMethod,ExpectedException(typeof(IndexOutOfRangeException))]
        public void SetterThrowExceptionWhenXisMoreThanOrEqualsWith()
        {
            var width = 10u;
            var height = 10u;
            var y = 0u;
            var field = new Field(width, height);

            field[width, y] = true;
            Assert.Fail("Setterが例外を吐くはず");
        }

        /// <summary>
        /// フィールド生成時の幅より大きいX座標は指定できない (セルの状態取得)
        /// </summary>
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetterThrowExceptionWhenXisMoreThanOrEqualsWith()
        {
            var width = 10u;
            var height = 10u;
            var y = 0u;
            var field = new Field(width, height);

            if(field[width, y]) { Assert.Fail("Getterが例外を吐くはず"); }
        }

        /// <summary>
        /// フィールド生成時の高さより大きいY座標は指定できない(セルの状態変更)
        /// </summary>
        [TestMethod,ExpectedException(typeof(IndexOutOfRangeException))]
        public void SetterThrowExceptionWhenYisMoreThanOrEqualsHeight()
        {
            var width = 10u;
            var height = 10u;
            var x = 0u;
            var field = new Field(width, height);

            field[x, height] = true;
            Assert.Fail("Setterが例外を吐くはず");
        }

        /// <summary>
        /// フィールド生成時の高さより大きいY座標は指定できない(セルの状態取得)
        /// </summary>
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetterThrowExceptionWhenYisMoreThanOrEqualsHeight()
        {
            var width = 10u;
            var height = 10u;
            var x = 0u;
            var field = new Field(width, height);

            if(field[x,height]) { Assert.Fail("Getterが例外を吐くはず"); }
        }
    }
}
