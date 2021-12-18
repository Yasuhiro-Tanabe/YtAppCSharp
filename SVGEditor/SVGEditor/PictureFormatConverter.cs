using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SVGEditor
{
    public class PictureFormatConverter
    {
        public static PictureFormatConverter BMP { get; } = new BmpConverter();
        public static PictureFormatConverter PNG { get; } = new PngConverter();

        public class BmpConverter : PictureFormatConverter
        {
            public override void CreateImageIcon(Bitmap src, PictureFormatConvertionInfo info)
            {
                int BITMAPFILEHEADER_SIZE = 14;
                int BITMAP_HEIGHT_OFFSET = 8;

                base.CreateImageIcon(src, info);

                // 作成したアイコンイメージデータを一時ストリームへ書き込み
                var stream = new MemoryStream();
                IconBitmap.Save(stream, ImageFormat.Bmp);

                // マスクビットマップデータを読み込み
                Rectangle bitmapRectangle = new Rectangle(0, 0, MaskBitmap.Width, MaskBitmap.Height);
                BitmapData bitmapData = MaskBitmap.LockBits(bitmapRectangle, ImageLockMode.ReadOnly, MaskBitmap.PixelFormat);
                byte[] maskBuf = new byte[bitmapData.Height * bitmapData.Stride];
                Marshal.Copy(bitmapData.Scan0, maskBuf, 0, bitmapData.Height * bitmapData.Stride);
                MaskBitmap.UnlockBits(bitmapData);

                // マスクデータの書き込み
                stream.Write(maskBuf, 0, maskBuf.Length);

                // Bitmap破棄
                IconBitmap.Dispose();
                MaskBitmap.Dispose();

                // 「BMPINFOHEADER」の「biHeight」箇所へ移動
                stream.Seek(BITMAPFILEHEADER_SIZE + BITMAP_HEIGHT_OFFSET, SeekOrigin.Begin);
                // 「biHeight」を元の2倍の値に変更
                stream.Write(BitConverter.GetBytes(info.Height * 2), 0, sizeof(int));

                // 作成したアイコンイメージデータをストリームへ書き込み
                info.IconImage.Write(stream.ToArray(), BITMAPFILEHEADER_SIZE, (int)(stream.Length - BITMAPFILEHEADER_SIZE));
            }
        }
        public class PngConverter : PictureFormatConverter
        {
            public override void CreateImageIcon(Bitmap src, PictureFormatConvertionInfo info)
            {
                base.CreateImageIcon(src, info);

                // 作成したアイコンイメージデータをストリームへ書き込み
                IconBitmap.Save(info.IconImage, ImageFormat.Png);
                // Bitmap破棄
                IconBitmap.Dispose();
            }
        }

        protected Bitmap IconBitmap { get; set; }
        protected Bitmap MaskBitmap { get; set; }

        public virtual void CreateImageIcon(Bitmap src, PictureFormatConvertionInfo info)
        {
            int width = info.Width;
            int height = info.Height;

            // 幅が「0」指定又は「256」より大きい場合、「256」に設定
            if (0 == info.Width || 256 < info.Width)
            {
                width = 256;
            }
            // 高さが「0」指定又は「256」より大きい場合、「256」に設定
            if (0 == info.Height || 256 < info.Height)
            {
                height = 256;
            }

            // Bitmapオブジェクト作成(変換後のサイズで作成)
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // アイコンイメージデータ作成
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.Default;
            graphics.DrawImage(src, 0, 0, width, height);

            IconBitmap = (Bitmap)bitmap.Clone();
            MaskBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format1bppIndexed);

            ToAlphaMask(bitmap, MaskBitmap);
        }

        /// <summary>
        /// アルファマスク画像に変換
        /// </summary>
        /// <param name="src">入力画像Bitmap</param>
        /// <param name="mask">出力画像Bitmap</param>
        public static void ToAlphaMask(Bitmap src, Bitmap mask)
        {
            Rectangle rect = new Rectangle(0, 0, src.Width, src.Height);
            BitmapData srcData = src.LockBits(rect, ImageLockMode.ReadOnly, src.PixelFormat);
            BitmapData dstData = mask.LockBits(rect, ImageLockMode.ReadWrite, mask.PixelFormat);

            byte[] srcBytes = BitmapDataToByteArray(srcData);
            byte[] dstBytes = BitmapDataToByteArray(dstData);
            var points = Enumerable.Range(0, src.Height)
                .SelectMany(y => Enumerable.Range(0, src.Width).Select(x => new Point(x, y)));

            ToAlphaMask(srcBytes, dstBytes, AlphaMaskParameter.Create(points, src.Height, srcData.Stride, dstData.Stride));

            CopyBiteArrayToBitmapData(dstData, dstBytes);

            mask.UnlockBits(dstData);
            src.UnlockBits(srcData);
        }

        private static void ToAlphaMask(byte[] src, byte[] dst, IEnumerable<AlphaMaskParameter> points)
        {
            int transparentThreshold = 128;
            byte alpha;
            byte bitBuffer = 0x00;
            int shift;

            foreach (var p in points)
            {
                alpha = src[p.X * 4 + p.SrcStride * p.Y + 3];
                shift = 7 - (p.X % 8);

                if (7 == shift) { bitBuffer = 0; }

                if (alpha < transparentThreshold)
                {
                    bitBuffer |= (byte)(0x01 << shift);
                }
                dst[(p.X / 8) + p.DstStride * (p.Height - (p.Y + 1))] = bitBuffer;
            }
        }

        public static byte[] BitmapDataToByteArray(BitmapData data)
        {
            var array = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, array, 0, data.Stride * data.Height);
            return array;
        }
        public static void CopyBiteArrayToBitmapData(BitmapData bmp, byte[] array)
        {
            Marshal.Copy(array, 0, bmp.Scan0, array.Length);
        }
    }
}
