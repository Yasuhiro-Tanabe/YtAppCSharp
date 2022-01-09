using System.Linq;

namespace YasT.Framework.Logging
{
    /// <summary>
    /// デバッグログ出力時に使用する字下げ文字列
    /// </summary>
    public class IndentString
    {
        private int _depth = 0;

        private string _str = _defaultIndentString;

        private static string _defaultIndentString = "    ";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="ws"></param>
        public IndentString(int depth=0, string ws = "    ")
        {
        }

        /// <summary>
        /// インデントを1レベル下げるための演算子
        /// </summary>
        /// <param name="indent">変更対象の <see cref="IndentString"/></param>
        /// <returns>インデントレベル変更後の <see cref="IndentString"/></returns>
        public static IndentString operator ++(IndentString indent)
        {
            indent._depth++;
            return indent;
        }

        /// <summary>
        /// インデントを1レベル上げるための演算子
        /// </summary>
        /// <param name="indent">変更対象の <see cref="IndentString"/></param>
        /// <returns>インデントレベル変更後の <see cref="IndentString"/></returns>
        public static IndentString operator--(IndentString indent)
        {
            if(indent._depth < 1)
            {
                return indent;
            }
            else
            {
                indent._depth--;
                return indent;
            }
        }

        /// <summary>
        /// インデント文字に変換する
        /// </summary>
        /// <returns>現在のインデントレベルに相当するインデント文字列</returns>
        public override string ToString()
        {
            return _depth == 0 ? "" : string.Join("", Enumerable.Range(0, _depth).Select(i => _str));
        }
    }
}
