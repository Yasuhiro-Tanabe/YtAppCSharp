using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;

using YasT.Framework.Logging;

namespace ResourceGenerator.Resources
{
    /// <summary>
    /// リソース管理クラス。
    /// <para>XAMLファイル内では、App.xamlに登録し StaticResource/DynamicResource でリソースを参照する。</para>
    /// <para>C#ソースコード中では、<see cref="ResourceFinder"/>を使ってリソースを参照する。</para>
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        /// 管理情報クラス。
        /// </summary>
        private class CultureResource
        {
            public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

            public CultureResource(string culture)
            {
                if (string.IsNullOrEmpty(culture)) { throw new ArgumentNullException(nameof(culture)); }
                Culture = new CultureInfo(culture);
                CreateResourceFiles();
                CreateCultureIndependentResourceFiles();
            }

            public Collection<ResourceDictionary> ResourceFiles { get; private set; } = new Collection<ResourceDictionary>();
            public Collection<ResourceDictionary> CultureIndependentResourceFiles { get; private set; } = new Collection<ResourceDictionary>();

            private void CreateResourceFiles()
            {
                foreach (var f in _resourceFiles)
                {
                    var res = new ResourceDictionary() { Source = new Uri($"Resources/{Culture}/{f}.xaml", UriKind.Relative) };
                    ResourceFiles.Add(res);
                }
            }

            private void CreateCultureIndependentResourceFiles()
            {
                foreach (var f in _cultureIndependent)
                {
                    var res = new ResourceDictionary() { Source = new Uri($"Resources/{f}.xaml", UriKind.Relative) };
                    CultureIndependentResourceFiles.Add(res);
                }
            }

            private static readonly string[] _resourceFiles = new string[]
            {
                "ViewItems",
                "Messages",
            };

            private static string[] _cultureIndependent = new string[]
            {
                "CultureIndependentResources",
            };
        }

        /// <summary>
        /// カルチャ(≒言語)単位のリソース管理情報。
        /// </summary>
        private readonly IReadOnlyDictionary<string, CultureResource> _cultures = new SortedDictionary<string, CultureResource>()
        {
            { "en-US", new CultureResource("en-US") },
            { "ja-JP", new CultureResource("ja-JP") },
        };

        /// <summary>
        /// このクラスのシングルトンオブジェクト。
        /// </summary>
        public static ResourceManager Instance { get { return _instance; } }
        private static readonly ResourceManager _instance = new ResourceManager();

        /// <summary>
        /// 画面の
        /// </summary>
        /// <param name="culture"></param>
        public void UpdateCulture(CultureInfo culture)
        {
            if(culture == null) { return; /* リソース管理情報を検索できないので何もしない。 */ }
            if(culture == CultureInfo.InvariantCulture) { return; /* リソース管理情報として登録していないので何もしない。 */ }

            var th = Thread.CurrentThread;
            LogUtil.DEBUGLOG_MethodCalled(culture.Name, $"current:{th.CurrentCulture.Name}/{th.CurrentUICulture.Name}");

            if(culture == th.CurrentUICulture)
            {
                // 変更不要。
                return;
            }
            else
            {
                if (_cultures.TryGetValue(culture.Name, out CultureResource? res))
                {
                    th.CurrentUICulture = res.Culture;
                    th.CurrentCulture = res.Culture;

                    var appResDic = Application.Current.Resources.MergedDictionaries;
                    appResDic.Clear();
                    foreach (var f in res.ResourceFiles)
                    {
                        appResDic.Add(f);
                    }
                    foreach (var f in res.CultureIndependentResourceFiles)
                    {
                        appResDic.Add(f);
                    }
                }
                else
                {
                    // 該当するリソースなし。
                    return;
                }
            }
        }
    }
}
