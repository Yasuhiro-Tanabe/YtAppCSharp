using System;

namespace GameOfLife.Core
{
    public class Field
    {
        public Field(int width, int height)
        {
            if(width <= 0) { throw new ArgumentOutOfRangeException(nameof(width), $"Invalid value: {width}"); }
            if(height <= 0) { throw new ArgumentOutOfRangeException(nameof(height), $"Invalid value: {height}"); }
            Width = width;
            Height = height;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}
