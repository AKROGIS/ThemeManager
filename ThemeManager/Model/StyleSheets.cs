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
        private XslCompiledTransform _xlst;

        /// <summary>
        /// Creates a new object for transforming XML
        /// </summary>
        /// <param name="path">Must be a local filesystem path</param>
        internal StyleSheet(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            //Do not pre-load the style sheets.  1) it takes significant time to compile, 2) it might not be used
            //_xlst = GetXslt(Path);
            //This means that a bad (invalid) stylesheet might be in the list
        }

        internal string ErrorMessage {get; private set;}
        private string Name { get; }
        private string Path { get; }

        // Stylesheet selector in UI calls ToString() on the stylesheet object to get text for the pick list.
        public override string ToString() {return Name;}

        internal string TransformText(string text)
        {
            string htmlText;
            using (StringReader sr = new StringReader(text))
            {
                var metadataXml = new XPathDocument(sr);

                XslCompiledTransform xlst = GetCachedXslt();

                // Custom Esri extensions are needed to process the Esri Stylesheets
                // This is based on examination of the Stylesheets and the Esri Libraries
                // I could not find this documented by Esri, so it may be subject to change without notice
                XsltArgumentList xslArgList = new XsltArgumentList();
                var esri = new ESRI.ArcGIS.Metadata.Editor.XsltExtensionFunctions();
                xslArgList.AddExtensionObject("http://www.esri.com/metadata/", esri);
                xslArgList.AddExtensionObject("http://www.esri.com/metadata/res/", esri);

                // HTML output
                TextWriter textWriter = new Utf8StringWriter();
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter))
                {
                    xlst.Transform(metadataXml, xslArgList, xmlWriter);
                }
                // Use Regex to replace the localizable elements <res:xxx /> with the localized text
                var pattern = @"<res:(\w+)(?:\s?|\s\S*\s)/>";
                MatchEvaluator evaluator = EsriLocalize;
                htmlText = Regex.Replace(textWriter.ToString(), pattern, evaluator, RegexOptions.None, TimeSpan.FromSeconds(0.25));
            }

            // The following 2 fixes should be done in the stylesheets
            //Add DOCTYPE
            htmlText = htmlText.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
            //Fix Thumbnail centering
            htmlText = htmlText.Replace(".noThumbnail {", ".noThumbnail {display:inline-block;");
            return htmlText;
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

    class StyleSheetsByEsri: List<StyleSheet>
    {
        private string _esriStyleSheetDir;

        public StyleSheetsByEsri()
        {
            //FIXME - catch and ignore file access related exceptions
            string dir = EmbeddedStyleSheetDir;
            if (string.IsNullOrEmpty(dir))
                return;

            foreach (string file in Directory.GetFiles(dir, "*.xslt", SearchOption.TopDirectoryOnly))
            {
                if (Path.GetExtension(file).ToLower() == ".xslt")
                    Add(new StyleSheet(file));
            }
        }

        // return a valid directory or empty string
        private string EmbeddedStyleSheetDir
        {
            get
            {
                if (_esriStyleSheetDir == null)
                {
                    // need a full path for the stylesheet transformer
                    _esriStyleSheetDir = Path.Combine(
                        Path.GetDirectoryName(Application.ExecutablePath),
                        Settings.Default.StyleSheetDirectory
                    );
                    Debug.Print("Stylesheet directory = {0}, Exists = {1}", _esriStyleSheetDir, Directory.Exists(_esriStyleSheetDir));
                }
                return _esriStyleSheetDir;
            }
        }
    }
}
