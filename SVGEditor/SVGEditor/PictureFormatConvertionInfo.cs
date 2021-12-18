using System.IO;

namespace SVGEditor
{
    /// <summary>
    /// アイコン変換情報クラス
    /// </summary>
    public class PictureFormatConvertionInfo
    {
        /// <summary>
        ///  アイコン画像形式
        /// </summary>
        public PictureFormatConverter PictureFormat { get; }
        /// <summary>
        /// 幅
        /// </summary>
        public ushort Width { get; }
        /// <summary>
        /// 高さ
        /// </summary>
        public ushort Height { get; }

        /// <summary>
        /// アイコンイメージ画像データ
        /// </summary>
        public MemoryStream IconImage { get; } = new MemoryStream();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="format">アイコン画像形式</param>
        /// <param name="squareSize">アイコンのサイズ[px]：アイコンは正方形なので、画像のサイズは squareSize x squareSize になる。</param>
        /// <param name="aWidth">幅</param>
        /// <param name="aHeight">高さ</param>
        public PictureFormatConvertionInfo(PictureFormatConverter format, ushort squareSize)
        {
            this.PictureFormat = format;
            this.Width = squareSize;
            this.Height = squareSize;
        }
    }

}
