using ClosedXML.Excel;

using ResourceGenerator.Resources;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using YasT.Framework.Logging;

namespace ResourceGenerator.Models
{
    /// <summary>
    /// リソース管理ファイルに登録されているプロパティの一覧。
    /// </summary>
    public class ResourceProperties
    {
        /// <summary>
        /// カルチャー名プロパティ。
        /// </summary>
        public class CultureProperty
        {
            private string fmt;
            private ResourceProperties _properties;
            /// <summary>
            /// コンストラクタ。
            /// </summary>
            public CultureProperty(ResourceProperties properties)
            {
                fmt = ResourceFinder.FindText("CulturePropertyName");
                _properties = properties;
            }
            /// <summary>
            /// operator[]
            /// </summary>
            /// <param name="idx">インデックス番号</param>
            /// <returns>プロパティ Culture.{idx} に登録されているカルチャー情報</returns>
            public CultureInfo this[int idx] { get { return new CultureInfo(_properties[string.Format(fmt, idx)]); } }
        }
        #region プロパティ
        /// <summary>
        /// 登録されているカルチャー(言語)の数。
        /// </summary>
        public int NumberOfCultures { get { return int.Parse(_properties[NumCulturesKey]); } }
        /// <summary>
        /// カルチャー名プロパティ。
        /// </summary>
        public CultureProperty Cultures { get; }
        /// <summary>
        /// プロパティの一覧。
        /// </summary>
        private IDictionary<string, string> _properties = new Dictionary<string, string>();
        /// <summary>
        /// プロパティが記載されているシートの名前。
        /// </summary>
        public string PropertySheetName { get; }
        /// <summary>
        /// カルチャ数プロパティのキー。
        /// </summary>
        private string NumCulturesKey { get; }
        #endregion プロパティ

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ResourceProperties()
        {
            PropertySheetName = ResourceFinder.FindText("PropertySheetName");
            NumCulturesKey = ResourceFinder.FindText("NumCulturesPropertyName");
            Cultures = new CultureProperty(this);
        }

        #region オペレータ
        /// <summary>
        /// operator []
        /// </summary>
        /// <param name="key">プロパティのキー。</param>
        /// <returns>プロパティの値</returns>
        public string this[string key]
        {
            get { return _properties[key] ?? throw new KeyNotFoundException($"Key '{key}' is not found."); }
            set
            {
                if(_properties.ContainsKey(key))
                {
                    _properties[key] = value;
                }
                else
                {
                    _properties.Add(key, value);
                }
            }
        }
        #endregion オペレータ

        /// <summary>
        /// 内部で保持しているプロパティをクリアする。
        /// </summary>
        public void Clear()
        {
            _properties.Clear();
        }
        /// <summary>
        /// Excel ワークブックからプロパティを読み取る。
        /// </summary>
        /// <param name="book">読み取る Excel ワークブック。</param>
        public void LoadFrom(XLWorkbook book)
        {
            if(book == null)
            {
                throw new NullReferenceException(nameof(book));
            }

            IXLWorksheet sheet;
            if(!book.TryGetWorksheet(PropertySheetName, out sheet))
            {
                throw new ArgumentException($"Sheet '{PropertySheetName}' is not found in the book.");
            }

            // セルの情報は 1-origin なので、それを考慮してループを回す。
            for (int i = 1; ; i++)
            {
                var key = sheet.Cell($"A{i}").Value?.ToString();
                if (string.IsNullOrWhiteSpace(key))
                {
                    break;
                }

                var value = sheet.Cell($"B{i}").Value?.ToString();

                _properties.Add(key, value ?? string.Empty);

                LogUtil.Debug($"{PropertySheetName}[{i}] ={{ Key={key}, Value={value} }} ");
            }
        }
        /// <summary>
        /// Excel ワークブックにプロパティを書き込む。
        /// </summary>
        /// <param name="book">読み取る Excel ワークブック。</param>
        public void SaveTo(XLWorkbook book)
        {
            if (book == null)
            {
                throw new NullReferenceException(nameof(book));
            }

            IXLWorksheet sheet;
            if (!book.TryGetWorksheet(PropertySheetName, out sheet))
            {
                throw new ArgumentException($"Sheet '{PropertySheetName}' is not found in the book.");
            }

            var idxs = Indexes(sheet);

            foreach(var kv in _properties.Where(kv => idxs.ContainsKey(kv.Key)))
            {
                var i = idxs[kv.Key];
                sheet.Cell($"B{i}").Value = kv.Value;
                LogUtil.Debug($"{PropertySheetName}[{i}] ={{ Key={kv.Key}, Value={kv.Value} }} ");
            }

            var ii = idxs.Max(kv => kv.Value);
            foreach(var kv in _properties.Where(kv => !idxs.ContainsKey(kv.Key)))
            {
                ii++;
                sheet.Cell($"A{ii}").Value = kv.Key;
                sheet.Cell($"B{ii}").Value = kv.Value;
                LogUtil.Debug($"{PropertySheetName}[{ii}] ={{ Key={kv.Key}, Value={kv.Value} }} ");
            }
        }
        private IDictionary<string, int> Indexes(IXLWorksheet sheet)
        {
            var idx = new Dictionary<string, int>();

            for(int i = 1; ; i++)
            {
                var key = sheet.Cell($"A{i}").Value?.ToString();
                if (string.IsNullOrWhiteSpace(key))
                {
                    break;
                }

                idx.Add(key, i);
            }

            return idx;
        }
    }
}
