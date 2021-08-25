using System;

namespace GameOfLife.Core
{
    public class Field
    {
        public Field(UInt32 width, UInt32 height)
        {
            if(width == 0) { throw new ArgumentOutOfRangeException(nameof(width), $"Invalid value: {width}"); }
            if(height == 0) { throw new ArgumentOutOfRangeException(nameof(height), $"Invalid value: {height}"); }
            Width = width;
            Height = height;
        }

        public UInt32 Width { get; private set; }
        public UInt32 Height { get; private set; }

        public bool this[UInt32 x, UInt32 y] { get { return false; } }
    }
}
