using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace GameOfLife.Core.Test
{
    /// <summary>
    /// ���C�t�Q�[���̃t�B�[���h�̃e�X�g
    /// </summary>
    [TestClass]
    public class FieldTest
    {
        /// <summary>
        /// �w�肳�ꂽ��/�����Ńt�B�[���h���쐬���邱�Ƃ��ł���
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
        /// ����0 �̃t�B�[���h�͍��Ȃ�
        /// </summary>
        [TestMethod,ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenWidthIsZero()
        {
            var width = 0u;
            var height = 10u;
            var field = new Field(width, height);
        }

        /// <summary>
        /// ������0�̃t�B�[���h�A�͍��Ȃ�
        /// </summary>
        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FieldCannotCreateWhenHeightIsZero()
        {
            var width = 10u;
            var height = 0u;
            var field = new Field(width, height);
        }

        /// <summary>
        /// ������Ԃ̃t�B�[���h�́A�S�Z�������ŏ��
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
        /// �t�B�[���h�̃Z���P�ʂŐ������ł��w��ł���A
        /// 1�̃Z���Ő����w�肵���ꍇ�A����ȊO�̃Z���͏�Ԃ����ł̂܂ܕς��Ȃ�
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
        /// �t�B�[���h�������̕����傫��X���W�͎w��ł��Ȃ� (�Z���̏�ԕύX)
        /// </summary>
        [TestMethod,ExpectedException(typeof(IndexOutOfRangeException))]
        public void SetterThrowExceptionWhenXisMoreThanOrEqualsWith()
        {
            var width = 10u;
            var height = 10u;
            var y = 0u;
            var field = new Field(width, height);

            field[width, y] = true;
            Assert.Fail("Setter����O��f���͂�");
        }

        /// <summary>
        /// �t�B�[���h�������̕����傫��X���W�͎w��ł��Ȃ� (�Z���̏�Ԏ擾)
        /// </summary>
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetterThrowExceptionWhenXisMoreThanOrEqualsWith()
        {
            var width = 10u;
            var height = 10u;
            var y = 0u;
            var field = new Field(width, height);

            if(field[width, y]) { Assert.Fail("Getter����O��f���͂�"); }
        }

        /// <summary>
        /// �t�B�[���h�������̍������傫��Y���W�͎w��ł��Ȃ�(�Z���̏�ԕύX)
        /// </summary>
        [TestMethod,ExpectedException(typeof(IndexOutOfRangeException))]
        public void SetterThrowExceptionWhenYisMoreThanOrEqualsHeight()
        {
            var width = 10u;
            var height = 10u;
            var x = 0u;
            var field = new Field(width, height);

            field[x, height] = true;
            Assert.Fail("Setter����O��f���͂�");
        }

        /// <summary>
        /// �t�B�[���h�������̍������傫��Y���W�͎w��ł��Ȃ�(�Z���̏�Ԏ擾)
        /// </summary>
        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetterThrowExceptionWhenYisMoreThanOrEqualsHeight()
        {
            var width = 10u;
            var height = 10u;
            var x = 0u;
            var field = new Field(width, height);

            if(field[x,height]) { Assert.Fail("Getter����O��f���͂�"); }
        }
    }
}
