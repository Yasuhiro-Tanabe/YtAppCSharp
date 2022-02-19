using MemorieDeFleurs.UI.WPF.Model;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;

using YasT.Framework.Logging;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    /// <summary>
    /// 外部化したテキストリソース管理クラス
    /// 
    /// Visual Studio の XAML コードエディタで使用する (インテリセンス、ビルド警告等) ため、
    /// アプリケーション起動時のリソースディクショナリは App.xaml に記載する。
    /// テキストリソースを XAML 内で利用するときは、 StaticResource または DyanmicResource で指定する。
    /// ソースコード中でリソースにアクセスする場合は <see cref="TextResourceFinder"/> を呼び出す。
    /// </summary>
    public class TextResourceManager
    {

        private class TextResource
        {
            public string CultureName { get; private set; }
            public TextResource(string culture)
            {
                CultureName = culture;
            }

            public ResourceDictionary Resource
            {
                get { return new ResourceDictionary() { Source = new Uri($"Resources/{CultureName}/Text.{CultureName}.xaml", UriKind.Relative) }; }
            }
        }

        private TextResourceManager() { }

        private static IDictionary<string, TextResource> _resources = new SortedDictionary<string, TextResource>()
        {
            { "ja-JP", new TextResource("ja-JP") },
            { "en-US", new TextResource("en-US") }
        };

        /// <summary>
        /// このクラスのインスタンス
        /// </summary>
        public static TextResourceManager Instance
        {
            get
            {
                lock(_resources)
                {
                    if(_instance == null)
                    {
                        _instance = new TextResourceManager();
                        _instance.UpdateCultureInfo(Thread.CurrentThread.CurrentUICulture);
                    }
                }
                return _instance;
            }
        }
        private static TextResourceManager _instance;


        /// <summary>
        /// リソースを切り替える
        /// </summary>
        /// <param name="culture">切替後の言語を示すカルチャー情報</param>
        public void UpdateCultureInfo(CultureInfo culture)
        {
            LogUtil.DEBUGLOG_MethodCalled(culture.Name, $"CurrentUICulture={Thread.CurrentThread.CurrentUICulture.Name}");

            if(culture == null) { return; /* _resource を検索できないので何もしない */ }
            if(culture == CultureInfo.InvariantCulture) { return; /* _resource に登録していないので何もしない */ }
            if(culture == Thread.CurrentThread.CurrentUICulture) { return; /* リソース変更不要なので何もしない */ }

            TextResource maker;
            if(_resources.TryGetValue(culture.Name, out maker))
            {
                // リソースを切り替える
                Thread.CurrentThread.CurrentUICulture = culture;
                App.Current.Resources.MergedDictionaries[0] = maker.Resource;
            }
            else
            {
                // ResourceDictionary がないので対応できない
                throw new ApplicationException($"Unsupported culture:{culture.Name}");
            }
        }
    }
}
