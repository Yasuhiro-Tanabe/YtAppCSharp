using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SVGEditor
{
    public class AlphaMaskParameter
    {
        public int X { get; }
        public int Y { get; }
        public int Height { get; }
        public int SrcStride { get; }
        public int DstStride { get; }

        private AlphaMaskParameter(Point p, int height, int srcStride, int dstStrind)
        {
            X = p.X;
            Y = p.Y;
            Height = height;
            SrcStride = srcStride;
            DstStride = dstStrind;
        }

        public static IEnumerable<AlphaMaskParameter> Create(IEnumerable<Point> points, int height, int srcStride, int dstStride)
        {
            return points.Select(p => new AlphaMaskParameter(p, height, srcStride, dstStride));
        }
    }
}
