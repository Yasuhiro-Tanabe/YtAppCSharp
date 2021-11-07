using Svg;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace SVGEditor
{
    internal class SVGEditorModel : NotificationObject
    {
        /// <summary>
        /// 表示するイメージが無いときに表示する画像
        /// </summary>
        private static string NO_IMAGE = "./no_images.png";


        #region プロパティ
        /// <summary>
        /// ソース：画面で編集中のSVGコード
        /// </summary>
        public string SvgCode
        {
            get { return _source; }
            private set { SetProperty(ref _source, value); }
        }
        private string _source;

        /// <summary>
        /// <see cref="SvgCode"/> をレンダリングしたイメージソース
        /// </summary>
        public MemoryStream SvgImage
        {
            get { return _image; }
            private set { SetProperty(ref _image, value); }
        }
        private MemoryStream _image;
        #endregion // プロパティ

        private static SVGEditorModel _singleton = new SVGEditorModel();

        private SVGEditorModel()
        {
            SvgImage = Render(Bitmap.FromFile(NO_IMAGE));
        }

        public static SVGEditorModel Instance { get; } = _singleton;


        public void Save(string svgFileName)
        {
            using (var writer = new StreamWriter(svgFileName))
            {
                writer.Write(SvgCode);
                writer.Flush();
            }
        }

        public void Load(string svgFileName)
        {
            var doc = new XmlDocument() { PreserveWhitespace = true, XmlResolver = null };
            doc.Load(svgFileName);
            SvgCode = doc.InnerXml;
            SvgImage = Render(SvgDocument.Open(doc).Draw());
        }


        private MemoryStream Render(Bitmap bmp)
        {
            var stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Bmp);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private MemoryStream Render(Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Bmp);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public void RenderCode(string code)
        {
            SvgCode = code;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(code);
            
            SvgImage = Render(SvgDocument.Open(doc).Draw());
        }

    }
}
