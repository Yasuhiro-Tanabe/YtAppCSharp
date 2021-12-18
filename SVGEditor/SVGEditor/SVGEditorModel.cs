using Svg;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Xml;

namespace SVGEditor
{
    internal class SVGEditorModel : NotificationObject
    {
        /// <summary>
        /// 表示するイメージが無いときに表示する画像 (SVGファイル)
        /// </summary>
        private static string NO_IMAGE_SVG = "./no_images.svg";

        private static SVGEditorModel _singleton = new SVGEditorModel();

        private SVGEditorModel() { }

        public static SVGEditorModel Instance { get; } = _singleton;

        private MemoryStream RenderToStream(Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private MemoryStream RenderCode(string code)
        {
            return RenderToStream(RenderToBitmap(code));
        }

        private SvgDocument ConvertToSvgDocument(string code)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(code);
            return SvgDocument.Open(doc);
        }
        
        private Bitmap RenderToBitmap(string code)
        {
            var svg = ConvertToSvgDocument(code).Draw();
            svg.MakeTransparent();
            return svg;
        }
        public Bitmap RenderToBitmap(string svgCode, int width, int height)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var svg = ConvertToSvgDocument(svgCode);
            svg.Width = width;
            svg.Height = height;
            svg.Draw(Graphics.FromImage(bmp), new Size(width, height));
            bmp.MakeTransparent();
            return bmp;
        }

        public BitmapImage RenderToImage(string svgCode)
        {
            var im = new BitmapImage();
            im.BeginInit();
            im.CacheOption = BitmapCacheOption.OnLoad;

            if (string.IsNullOrWhiteSpace(svgCode))
            {
                im.StreamSource =  RenderCode(File.ReadAllText(NO_IMAGE_SVG));
            }
            else
            {
                im.StreamSource = RenderCode(svgCode);
            }

            im.EndInit();

            return im;
        }

        /// <summary>
        /// ICONDIR構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct T_ICONDIR
        {
            public ushort idReserved;
            public ushort idType;
            public ushort idCount;
        }

        /// <summary>
        /// ICONDIRENTRY構造体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct T_ICONDIRENTRY
        {
            public byte bWidth;
            public byte bHeight;
            public byte bColorCount;
            public byte bReserved;
            public ushort wPlanes;
            public ushort wBitCount;
            public uint dwBytesInRes;
            public uint dwImageOffset;
        }

        public void WriteToIconFile(string path, string svgCode)
        {
            var converters = new List<PictureFormatConvertionInfo>()
            {
                new PictureFormatConvertionInfo(PictureFormatConverter.PNG, 256),
                new PictureFormatConvertionInfo(PictureFormatConverter.BMP, 128),
                new PictureFormatConvertionInfo(PictureFormatConverter.BMP, 48),
                new PictureFormatConvertionInfo(PictureFormatConverter.BMP, 32),
                new PictureFormatConvertionInfo(PictureFormatConverter.BMP, 16)
            };
            foreach (var info in converters)
            {
                // 毎回指定サイズでベース画像を作り直す
                var bmp = SVGEditorModel.Instance.RenderToBitmap(svgCode, info.Width, info.Height);
                info.PictureFormat.CreateImageIcon(bmp, info);
            }
            using (var ofs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                WriteIconDir(ofs, converters.Count);
                WriteIconDirEntry(ofs, converters);
                WriteIconImages(ofs, converters);
            }
        }

        /// <summary>
        /// アイコンイメージを書き出す
        /// </summary>
        /// <param name="stream">書き出し先のアイコンファイルストリーム</param>
        private void WriteIconImages(Stream stream, IEnumerable<PictureFormatConvertionInfo> converters)
        {
            foreach (var info in converters)
            {
                info.IconImage.WriteTo(stream);
            }
        }

        /// <summary>
        /// 各アイコンに関する ICONDIRENTRY を生成し書き出す
        /// </summary>
        /// <param name="stream">書き出し先のアイコンファイルストリーム</param>
        private void WriteIconDirEntry(Stream stream, IEnumerable<PictureFormatConvertionInfo> converters)
        {
            int ICONDIR_SIZE = 6;
            int ICONDIRENTRY_SIZE = 16;
            long offset = ICONDIR_SIZE + (ICONDIRENTRY_SIZE * converters.Count());
            T_ICONDIRENTRY iconDirEntry = new T_ICONDIRENTRY() { bReserved = 0x00, wPlanes = 0x0001 };
            foreach (var info in converters)
            {
                iconDirEntry.bWidth = (0 == info.Width || 256 < info.Width) ? (byte)0 : (byte)info.Width;
                iconDirEntry.bHeight = (0 == info.Height || 256 < info.Height) ? (byte)0 : (byte)info.Height;
                iconDirEntry.bColorCount = (byte)0;
                iconDirEntry.wBitCount = (ushort)32;
                iconDirEntry.dwBytesInRes = (uint)info.IconImage.Length;
                iconDirEntry.dwImageOffset = (uint)offset;

                this.WriteStructData(iconDirEntry, stream);

                // 次に書き込む「ICONDIRENTRY」の「dwImageOffset」を設定
                offset += iconDirEntry.dwBytesInRes;
            }
        }

        /// <summary>
        /// ICONDIR を書き出す
        /// </summary>
        /// <param name="stream">書き出し先のアイコンファイルストリーム</param>
        private void WriteIconDir(Stream stream, int numIcons)
        {
            T_ICONDIR iconDir = new T_ICONDIR() { idReserved = 0x0000, idType = 0x0001, idCount = (ushort)numIcons };
            this.WriteStructData(iconDir, stream);
        }

        /// <summary>
        /// 構造体データ書き込み
        /// </summary>
        /// <param name="data">構造体データ</param>
        /// <param name="stream">出力先ストリーム</param>
        /// <returns>書き込みサイズを返す</returns>
        private int WriteStructData<S>(S data, Stream stream) where S : struct
        {
            int size = Marshal.SizeOf(data);
            IntPtr buffer = Marshal.AllocCoTaskMem(size);
            Marshal.StructureToPtr(data, buffer, true);
            byte[] buf = new byte[size];
            Marshal.Copy(buffer, buf, 0, size);
            stream.Write(buf, 0, size);
            return size;
        }
    }
}
