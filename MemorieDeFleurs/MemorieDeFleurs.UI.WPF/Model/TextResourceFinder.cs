using System.Windows;

namespace MemorieDeFleurs.UI.WPF.Model
{
    /// <summary>
    /// アプリケーションに登録したテキストリソースを取得する
    /// </summary>
    public static class TextResourceFinder
    {
        /// <summary>
        /// 文字列リソースを取得する
        /// </summary>
        /// <param name="key">リソースのキー</param>
        /// <returns>文字列リソース</returns>
        public static string FindText(string key)
        {
            try
            {
                return App.Current.FindResource(key) as string;
            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                return $"???{key}???";
            }
        }

        /// <summary>
        /// 文字列リソースを取得する
        /// </summary>
        /// <param name="key">リソースのキー</param>
        /// <param name="args">フォーマット引数</param>
        /// <returns>整形した文字列リソース</returns>
        public static string FindText(string key, params object[] args)
        {
            try
            {
                var format = App.Current.FindResource(key) as string;
                return string.Format(format, args);
            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                return $"???{key}({string.Join(", ", args)}???";
            }
        }
    }
}
