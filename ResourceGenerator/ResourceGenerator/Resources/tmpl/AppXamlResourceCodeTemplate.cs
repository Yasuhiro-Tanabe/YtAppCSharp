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
    
    #line 1 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class AppXamlResourceCodeTemplate : AppXamlResourceCodeTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("<Application x:Class=\"");
            
            #line 7 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write(".App\"\r\n             xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentat" +
                    "ion\"\r\n             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n     " +
                    "        xmlns:local=\"clr-namespace:");
            
            #line 10 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\"\r\n             StartupUri=\"MainWindow.xaml\">\r\n    <Application.Resources>\r\n     " +
                    "   <!-- デフォルトロケールは ");
            
            #line 13 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DefaultCulture.Name));
            
            #line default
            #line hidden
            this.Write(" -->\r\n        <ResourceDictionary>\r\n            <ResourceDictionary.MergedDiction" +
                    "aries>\r\n");
            
            #line 16 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
 foreach(var f in ResourceFiles) { 
            
            #line default
            #line hidden
            this.Write("                <ResourceDictionary Source=\"");
            
            #line 17 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ResourceRootPath));
            
            #line default
            #line hidden
            this.Write("/");
            
            #line 17 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DefaultCulture.Name));
            
            #line default
            #line hidden
            this.Write("/");
            
            #line 17 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(f));
            
            #line default
            #line hidden
            this.Write(".xaml\"/>\r\n");
            
            #line 18 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
 } 
            
            #line default
            #line hidden
            
            #line 19 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
 foreach(var f in CultureIndependentResourceFiles) { 
            
            #line default
            #line hidden
            this.Write("                <ResourceDictionary Source=\"");
            
            #line 20 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ResourceRootPath));
            
            #line default
            #line hidden
            this.Write("/");
            
            #line 20 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DefaultCulture.Name));
            
            #line default
            #line hidden
            this.Write("/");
            
            #line 20 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(f));
            
            #line default
            #line hidden
            this.Write(".xaml\"/>\r\n");
            
            #line 21 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("            </ResourceDictionary.MergedDictionaries>\r\n        </ResourceDictionar" +
                    "y>\r\n    </Application.Resources>\r\n</Application>\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 26 "D:\C#Dev\GitHub\YtAppCSharp\ResourceGenerator\ResourceGenerator\Resources\tmpl\AppXamlResourceCodeTemplate.tt"

/// <summary>
/// アプリケーションの名前空間：設定画面で設定した名前空間の、"."で区切られた最初の名前空間を使う。
/// </summary>
public string Namespace { get; set; }
/// <summary>
/// リソースファイルを配置するフォルダの名前。
/// </summary>
public string ResourceRootPath { get; set; }
/// <summary>
/// カルチャーに依存するリソースファイルのファイル名 (拡張子を除く)：リソース管理ファイルにあるシートのシート名。
/// </summary>
public ICollection<string> ResourceFiles { get; } = new List<string>();
/// <summary>
/// カルチャーに依存しないリソースファイルのファイル名 (拡張子を除く)。
/// </summary>
public ICollection<string> CultureIndependentResourceFiles { get; } = new List<string>();
/// <summary>
/// 出力するデフォルトカルチャー。
/// </summary>
public CultureInfo DefaultCulture { get; set; }

        
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
    public class AppXamlResourceCodeTemplateBase
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