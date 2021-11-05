using System.Linq;

namespace MemorieDeFleurs.Logging
{
    public class IndentString
    {
        private int _depth = 0;

        private string _str = _defaultIndentString;

        private static string _defaultIndentString = "    ";

        public IndentString(int depth=0, string ws = "    ")
        {
        }

        public static IndentString operator ++(IndentString indent)
        {
            indent._depth++;
            return indent;
        }

        public static IndentString operator--(IndentString indent)
        {
            indent._depth--;
            return indent;
        }

        public override string ToString()
        {
            return _depth == 0 ? "" : string.Join("", Enumerable.Range(0, _depth).Select(i => _str));
        }
    }
}
