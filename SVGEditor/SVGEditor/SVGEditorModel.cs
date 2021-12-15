using Svg;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(code);

            var svg = SvgDocument.Open(doc).Draw();
            svg.MakeTransparent();
            return RenderToStream(svg);
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
    }
}
