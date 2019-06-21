using NPS.AKRO.ThemeManager.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace NPS.AKRO.ThemeManager.Model
{
    /*
     * Theme Manager no longer supports the ArcGIS pre 10.x Stylesheet format (Microsoft's
     * pre standard XSL dialect).  Theme Manager now uses .Net's XslCompiledTransform,
     * which uses the final XSLT 1.0 Recommendation published in 1999.  The Stylesheets
     * provided by ESRI since 10.0 use this format.
     */

    internal class StyleSheet
    {
        // Assume we can find the Esri Metadata Library, until proved otherwise.  Then we won't check anymore
        private static bool EsriLibraryAvailable = true;

        private XslCompiledTransform _xlst;

        /// <summary>
        /// Creates a new object for transforming XML
        /// </summary>
        /// <param name="path">Must be a local filesystem path</param>
        internal StyleSheet(string path, bool esriStyleSheet = false)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            EsriStyleSheet = esriStyleSheet;
            //Do not pre-load the style sheets.  1) it takes significant time to compile, 2) it might not be used
            //_xlst = GetXslt(Path);
            //This means that a bad (invalid) stylesheet might be in the list
        }

        internal string ErrorMessage {get; private set;}
        private string Name { get; }
        private string Path { get; }
        // Esri Style Sheets are not installed by default, and they require extra processing which might not be possible.
        private bool EsriStyleSheet { get; set; }

        // Stylesheet selector in UI calls ToString() on the stylesheet object to get text for the pick list.
        public override string ToString() {return Name;}

        internal string TransformText(string text)
        {
            string htmlText;
            using (StringReader sr = new StringReader(text))
            {
                var metadataXml = new XPathDocument(sr);

                XsltArgumentList xslArgList;
                try
                {
                    xslArgList = EsriLibraryAvailable ? EsriProcessingArguments() : new XsltArgumentList();
                }
                catch (FileNotFoundException)
                {
                    EsriLibraryAvailable = false;
                    xslArgList = new XsltArgumentList();
                }

                if (EsriStyleSheet && !EsriLibraryAvailable) {
                    throw new FileNotFoundException("Esri Metadata library not found.  See installation instructions");
                }

                XslCompiledTransform xlst = GetCachedXslt();

                // HTML output
                TextWriter textWriter = new Utf8StringWriter();
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter))
                {
                    xlst.Transform(metadataXml, xslArgList, xmlWriter);
                }
                htmlText = textWriter.ToString();

                if (EsriStyleSheet) {
                    // Use Regex to replace the localizable elements <res:xxx /> with the localized text
                    var pattern = @"<res:(\w+)(?:\s?|\s\S*\s)/>";
                    htmlText = Regex.Replace(htmlText, pattern, EsriLocalize, RegexOptions.None, TimeSpan.FromSeconds(0.25));
                    // The following 2 fixes should be done in the stylesheets, and are only required(?) in the Esri Stylesheets
                    //Add DOCTYPE
                    htmlText = htmlText.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?><!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
                    //Fix Thumbnail centering
                    htmlText = htmlText.Replace(".noThumbnail {", ".noThumbnail {display:inline-block;");
                }
            }
            return htmlText;
        }

        private static XsltArgumentList EsriProcessingArguments()
        {
            // Custom Esri extensions are needed to process the Esri Stylesheets
            // This is based on examination of the Stylesheets and the Esri Libraries
            // I could not find this documented by Esri, so it may be subject to change without notice
            XsltArgumentList xslArgList = new XsltArgumentList();
            var esri = new ESRI.ArcGIS.Metadata.Editor.XsltExtensionFunctions();
            xslArgList.AddExtensionObject("http://www.esri.com/metadata/", esri);
            xslArgList.AddExtensionObject("http://www.esri.com/metadata/res/", esri);
            return xslArgList;
        }
        private static string EsriLocalize(Match match)
        {
            return ESRI.ArcGIS.Metadata.Editor.XsltExtensionFunctions.GetResString(match.Groups[1].Value);
        }

        private XslCompiledTransform GetCachedXslt()
        {
            if (_xlst == null && string.IsNullOrEmpty(ErrorMessage))
                _xlst = GetXslt(Path);
            if (_xlst == null)
                throw new Exception("No stylesheet was loaded\n" + ErrorMessage);
            return _xlst;
        }

        private XslCompiledTransform GetXslt(string path)
        {
            XslCompiledTransform xlst;
            try
            {
                xlst = new XslCompiledTransform();
                xlst.Load(path, null, new XmlUrlResolver());
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                xlst = null;
                ErrorMessage = "Unable to load " + path + "\n" + ex;
            }
            return xlst;
        }
    }

    // The default string writer encodes UTF16 which is not compatible with the UTF8 generated XML
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

    class StyleSheetList: List<StyleSheet>
    {
        public StyleSheetList()
        {
            string dir = GetEmbeddedStyleSheetDir();
            // Directory.Exists() returns false if it encounters any exceptions
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir)) {
                return;
            }
            // Get files could fail if dir gets deleted/renamed/etc by another process after the check above
            try
            {
                // Theme Manager provided Style Sheets
                foreach (string file in Directory.GetFiles(dir, "*.xslt", SearchOption.TopDirectoryOnly))
                {
                    Add(new StyleSheet(file, false));
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Directory.GetFiles() failed on directory = {0}. Exception: = {1}", dir, ex.Message);
                throw new DirectoryNotFoundException($"Style Sheet directory ({dir}) disappeared. {ex.Message}", ex);
            }

            // TODO: try and load the esri metadata library, and skip the Esri Stylesheets if not found.
            // Unfortunately, there would be no way to notify the user at this point.

            // Esri provided Style Sheets
            dir = Path.Combine(dir, "Esri");
            if (Directory.Exists(dir)) {
                try
                {
                    foreach (string file in Directory.GetFiles(dir, "*.xslt", SearchOption.TopDirectoryOnly))
                    {
                        Add(new StyleSheet(file, true));
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print("Directory.GetFiles() failed on directory = {0}. Exception: = {1}", dir, ex.Message);
                    throw new DirectoryNotFoundException($"Style Sheet directory ({dir}) disappeared. {ex.Message}", ex);
                }
            }
        }

        // Does not verify that path exists the caller should do that.  It may throw an exception if
        // The user modified Settings.Default.StyleSheetDirectory is invalid
        private string GetEmbeddedStyleSheetDir()
        {
            // need a full path for the stylesheet transformer
            var styleSheetDir = Path.Combine(
                Path.GetDirectoryName(Application.ExecutablePath),
                Settings.Default.StyleSheetDirectory ?? ""
            );
            Debug.Print($"Stylesheet directory = {styleSheetDir}, Exists = {Directory.Exists(styleSheetDir)}");
            if (!Directory.Exists(styleSheetDir)) {
                Debug.Print($"Style Sheet directory ({styleSheetDir}) not found.");
                throw new DirectoryNotFoundException($"Style Sheet directory ({styleSheetDir}) not found: Check settings in ThemeManager.config");
            }
            return styleSheetDir;
        }
    }
}
