using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace GameOfLife.Core.Test
{
    /// <summary>
    /// 世代交代で死滅状態のセルが生存状態に変わるパターンのテスト
    /// </summary>
    [TestClass]
    public class BirthTest
    {
        /// <summary>
        /// フィールド内側
        /// </summary>
        /// <remarks>
        /// ×の位置が死滅→生存に変わる
        /// <code>
        ///      | 0| 1| 2| 3| 4| 5| 6
        ///     -+--+--+--+--+--+--+--
        ///     0|  |  |  |  |  |  |
        ///     -+--+--+--+--+--+--+--
        ///     1|  |  |○|  |  |  |
        ///     -+--+--+--+--+--+--+--
        ///     2|  |○|×|  |  |  |
        ///     -+--+--+--+--+--+--+--
        ///     3|  |  |  |○|  |  |
        ///     -+--+--+--+--+--+--+--
        ///     4|  |  |  |  |  |  |
        ///     -+--+--+--+--+--+--+--
        ///     5|  |  |  |  |  |  |
        /// </code>
        /// </remarks>
        [TestMethod]
        public void InsideField()
        {
            var width = 10u;
            var height = 10u;
            var alliveCells = new List<Tuple<UInt32, UInt32>>()
            {
                Tuple.Create(2u, 1u),
                Tuple.Create(1u, 2u),
                Tuple.Create(3u, 3u)
            };
            var x = 2u;
            var y = 2u;

            var field = new Field(width, height);
            foreach(var c in alliveCells)
            {
                field[c.Item1, c.Item2] = true;
            }

            Assert.IsFalse(field[x, y]);
            field.AdvanceGeneration();
            Assert.IsTrue(field[x, y]);
        }
    }
}
