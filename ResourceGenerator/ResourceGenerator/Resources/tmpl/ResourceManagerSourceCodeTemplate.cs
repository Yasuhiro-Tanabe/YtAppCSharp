﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン: 17.0.0.0
//  
//     このファイルへの変更は、正しくない動作の原因になる可能性があり、
//     コードが再生成されると失われます。
// </auto-generated>
// ------------------------------------------------------------------------------
namespace ResourceGenerator.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Globalization;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class ResourceManagerSourceCodeTemplate : ResourceManagerSourceCodeTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing System.Collections.Object" +
                    "Model;\r\nusing System.Globalization;\r\nusing System.Threading;\r\nusing System.Windo" +
                    "ws;\r\n\r\nnamespace ");
            
            #line 14 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    /// <summary>\r\n    /// リソース管理クラス。\r\n    /// <para>XAMLファイル内では、App.xamlに登録" +
                    "し StaticResource/DynamicResource でリソースを参照する。</para>\r\n");
            
            #line 19 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 if(IsGenerateResourceFinder) { 
            
            #line default
            #line hidden
            this.Write("    /// <para>C#ソースコード中では、<see cref=\"ResourceFinder\"/>を使ってリソースを参照する。</para>\r\n");
            
            #line 21 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("    /// <para>C#ソースコード中では、<see cref=\"Application.FindResource(object)\"/>を使ってリソースを" +
                    "参照する。</para>\r\n");
            
            #line 23 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("    /// </summary>\r\n    public class ResourceManager\r\n    {\r\n        /// <summary" +
                    ">\r\n        /// 管理情報クラス。\r\n        /// </summary>\r\n        private class CultureRe" +
                    "source\r\n        {\r\n            public CultureInfo Culture { get; set; } = Cultur" +
                    "eInfo.InvariantCulture;\r\n\r\n            public CultureResource(string culture)\r\n " +
                    "           {\r\n                if (string.IsNullOrEmpty(culture)) { throw new Arg" +
                    "umentNullException(nameof(culture)); }\r\n                Culture = new CultureInf" +
                    "o(culture);\r\n                CreateResourceFiles();\r\n                CreateCultu" +
                    "reIndependentResourceFiles();\r\n            }\r\n\r\n            public Collection<Re" +
                    "sourceDictionary> ResourceFiles { get; private set; } = new Collection<ResourceD" +
                    "ictionary>();\r\n            public Collection<ResourceDictionary> CultureIndepend" +
                    "entResourceFiles { get; private set; } = new Collection<ResourceDictionary>();\r\n" +
                    "\r\n            private void CreateResourceFiles()\r\n            {\r\n               " +
                    " foreach (var f in _resourceFiles)\r\n                {\r\n                    var r" +
                    "es = new ResourceDictionary() { Source = new Uri($\"Resources/{Culture}/{f}.xaml\"" +
                    ", UriKind.Relative) };\r\n                    ResourceFiles.Add(res);\r\n           " +
                    "     }\r\n            }\r\n\r\n            private void CreateCultureIndependentResour" +
                    "ceFiles()\r\n            {\r\n                foreach (var f in _cultureIndependent)" +
                    "\r\n                {\r\n                    var res = new ResourceDictionary() { So" +
                    "urce = new Uri($\"Resources/{f}.xaml\", UriKind.Relative) };\r\n                    " +
                    "CultureIndependentResourceFiles.Add(res);\r\n                }\r\n            }\r\n\r\n");
            
            #line 63 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 if(ResourceFiles.Count == 0) { 
            
            #line default
            #line hidden
            this.Write("            private static readonly string[] _resourceFiles = new string[] { };\r\n" +
                    "");
            
            #line 65 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write(" \r\n            private static readonly string[] _resourceFiles = new string[]\r\n  " +
                    "          {\r\n");
            
            #line 68 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
     foreach(var f in ResourceFiles) { 
            
            #line default
            #line hidden
            this.Write("                \"");
            
            #line 69 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(f));
            
            #line default
            #line hidden
            this.Write("\",\r\n");
            
            #line 70 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
     } 
            
            #line default
            #line hidden
            this.Write("            };\r\n");
            
            #line 72 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 74 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 if(CultureIndependentResourceFiles.Count == 0) {
            
            #line default
            #line hidden
            this.Write("            private static string[] _cultureIndependent = new string[] { };\r\n");
            
            #line 76 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("            private static string[] _cultureIndependent = new string[]\r\n         " +
                    "   {\r\n");
            
            #line 79 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
     foreach(var f in CultureIndependentResourceFiles) { 
            
            #line default
            #line hidden
            this.Write("                \"");
            
            #line 80 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(f));
            
            #line default
            #line hidden
            this.Write("\",\r\n");
            
            #line 81 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
     } 
            
            #line default
            #line hidden
            this.Write("            };\r\n");
            
            #line 83 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("        }\r\n\r\n        /// <summary>\r\n        /// カルチャ(≒言語)単位のリソース管理情報。\r\n        //" +
                    "/ </summary>\r\n");
            
            #line 89 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 if(Cultures.Count == 0) { 
            
            #line default
            #line hidden
            this.Write("        private readonly IReadOnlyDictionary<string, CultureResource> _cultures =" +
                    " new SortedDictionary<string, CultureResource>();\r\n");
            
            #line 91 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 } else { 
            
            #line default
            #line hidden
            this.Write("        private readonly IReadOnlyDictionary<string, CultureResource> _cultures =" +
                    " new SortedDictionary<string, CultureResource>()\r\n        {\r\n");
            
            #line 94 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
     foreach(var c in Cultures) { 
            
            #line default
            #line hidden
            this.Write("            { \"");
            
            #line 95 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(c.Name));
            
            #line default
            #line hidden
            this.Write("\", new CultureResource(\"");
            
            #line 95 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(c.Name));
            
            #line default
            #line hidden
            this.Write("\") },\r\n");
            
            #line 96 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
     } 
            
            #line default
            #line hidden
            this.Write("        };\r\n");
            
            #line 98 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n        /// <summary>\r\n        /// このクラスのシングルトンオブジェクト。\r\n        /// </summary>\r" +
                    "\n        public static ResourceManager Instance { get { return _instance; } }\r\n " +
                    "       private static readonly ResourceManager _instance = new ResourceManager()" +
                    ";\r\n\r\n        /// <summary>\r\n        /// カルチャーを変更する。\r\n        /// </summary>\r\n   " +
                    "     /// <param name=\"culture\">変更したいカルチャー</param>\r\n        public void UpdateCul" +
                    "ture(CultureInfo culture)\r\n        {\r\n            if(culture == null) { return; " +
                    "/* リソース管理情報を検索できないので何もしない。 */ }\r\n            if(culture == CultureInfo.Invariant" +
                    "Culture) { return; /* リソース管理情報として登録していないので何もしない。 */ }\r\n\r\n            var th = Th" +
                    "read.CurrentThread;\r\n\r\n            if(culture == th.CurrentUICulture)\r\n         " +
                    "   {\r\n                // 変更不要。\r\n                return;\r\n            }\r\n        " +
                    "    else\r\n            {\r\n                if (_cultures.TryGetValue(culture.Name," +
                    " out CultureResource? res))\r\n                {\r\n                    th.CurrentUI" +
                    "Culture = res.Culture;\r\n                    th.CurrentCulture = res.Culture;\r\n\r\n" +
                    "                    var appResDic = Application.Current.Resources.MergedDictiona" +
                    "ries;\r\n                    appResDic.Clear();\r\n                    foreach (var " +
                    "f in res.ResourceFiles)\r\n                    {\r\n                        appResDi" +
                    "c.Add(f);\r\n                    }\r\n                    foreach (var f in res.Cult" +
                    "ureIndependentResourceFiles)\r\n                    {\r\n                        app" +
                    "ResDic.Add(f);\r\n                    }\r\n                }\r\n                else\r\n" +
                    "                {\r\n                    // 該当するリソースなし。\r\n                    retur" +
                    "n;\r\n                }\r\n            }\r\n        }\r\n    }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 149 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\ResourceManagerSourceCodeTemplate.tt"

/// <summary>
/// クラスの名前空間。
/// </summary>
public string Namespace { get; set; }
/// <summary>
/// ResourceFinder クラスのソースコードを同時生成するかどうか。
/// 同時生成するかどうかでコメントが変わる。
/// </summary>
public bool IsGenerateResourceFinder { get; set; }
/// <summary>
/// カルチャーに依存するリソースファイルのファイル名 (拡張子を除く)：リソース管理ファイルにあるシートのシート名。
/// </summary>
public ICollection<string> ResourceFiles { get; } = new List<string>();
/// <summary>
/// カルチャーに依存しないリソースファイルのファイル名 (拡張子を除く)。
/// </summary>
public ICollection<string> CultureIndependentResourceFiles { get; } = new List<string>();
/// <summary>
/// リソースファイルを生成するカルチャー。
/// </summary>
public ICollection<CultureInfo> Cultures { get; } = new List<CultureInfo>();

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public class ResourceManagerSourceCodeTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}