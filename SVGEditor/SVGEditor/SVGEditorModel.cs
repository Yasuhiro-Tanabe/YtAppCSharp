using Svg;

using System.Drawing;
using System.IO;
using System.Xml;

namespace SVGEditor
{
    internal class SVGEditorModel
    {
        public object ViewModel { get; private set; }

        public void SaveToFile(string fileName, string code)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.Write(code);
                writer.Flush();
            }
        }

        public string LoadXmlFile(string fileName)
        {
            var src = new XmlDocument() { PreserveWhitespace = true, XmlResolver = null };
            src.Load(fileName);
            return src.InnerXml;
        }

        public Bitmap Render(string code)
        {
            var xml = new XmlDocument();
            xml.LoadXml(code);
            var svg = SvgDocument.Open(xml);

            return svg.Draw();
        }
    }
}
