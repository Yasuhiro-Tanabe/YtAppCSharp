using System;
using System.Collections.Generic;

namespace GameOfLife.Core
{
    /// <summary>
    /// ライフゲームのフィールド
    /// </summary>
    public class Field
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="width">幅(セル数)</param>
        /// <param name="height">高さ(セル数)</param>
        public Field(UInt32 width, UInt32 height)
        {
            if(width == 0) { throw new ArgumentOutOfRangeException(nameof(width), $"Invalid value: {width}"); }
            if(height == 0) { throw new ArgumentOutOfRangeException(nameof(height), $"Invalid value: {height}"); }
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 幅(セル数)
        /// </summary>
        public UInt32 Width { get; private set; }

        /// <summary>
        /// 高さ(セル数)
        /// </summary>
        public UInt32 Height { get; private set; }

        private ISet<Tuple<UInt32, UInt32>> alliveCells = new HashSet<Tuple<UInt32, UInt32>>();

        /// <summary>
        /// インデクサ
        /// </summary>
        /// <param name="x">幅方向のセル番号</param>
        /// <param name="y">高さ方向のセル番号</param>
        /// <returns>セルの状態：生きているとき真、死んでいるとき偽</returns>
        public bool this[UInt32 x, UInt32 y] 
        { 
            get
            {
                ValidateRangeOfCoordinate(x, y);
                return alliveCells.Contains(Tuple.Create(x,y));
            }
            set 
            {
                ValidateRangeOfCoordinate(x, y);
                var pos = Tuple.Create(x, y);
                if(value == true)
                {
                    alliveCells.Add(pos);
                }
                else if(alliveCells.Contains(pos))
                {
                    alliveCells.Remove(pos);
                }
            }
        }

        private void ValidateRangeOfCoordinate(UInt32 x, UInt32 y)
        {
            if(x >= Width) 
            {
                throw new IndexOutOfRangeException($"Invalid x: {x}. It shall be from 0 to {Width - 1}.");
            }
            if(y >= Height)
            {
                throw new IndexOutOfRangeException($"Invalid y: {y}. It shall be from 0 to {Height - 1}.");
            }
        }
    }
}
