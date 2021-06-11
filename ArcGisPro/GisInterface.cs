using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Xsl;
// Pro 2.5
//using EsriArcGIS = ArcGIS.Desktop.Metadata.Editor;
// Pro 2.7+
using EsriArcGIS = ArcGIS.Desktop.Internal.Metadata;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    public class GisInterface
    {
        public static XsltArgumentList EsriProcessingArguments()
        {
            // Custom Esri extensions are needed to process the Esri Stylesheets
            // This is based on examination of the Stylesheets and the Esri Libraries
            // I could not find this documented by Esri, so it may be subject to change without notice
            XsltArgumentList xslArgList = new XsltArgumentList();
            var esri = new EsriArcGIS.XsltExtFunctions();
            xslArgList.AddExtensionObject("http://www.esri.com/metadata/", esri);
            xslArgList.AddExtensionObject("http://www.esri.com/metadata/res/", esri);
            return xslArgList;
        }
        private static string EsriLocalize(Match match)
        {
            return EsriArcGIS.XsltExtFunctions.GetResString(match.Groups[1].Value);
        }

        public static string CleanEsriMetadataHtml(string html)
        {
            // Use Regex to replace the localizable elements <res:xxx /> with the localized text
            var pattern = @"<res:(\w+)(?:\s?|\s\S*\s)/>";
            html = Regex.Replace(html, pattern, EsriLocalize, RegexOptions.None, TimeSpan.FromSeconds(0.25));
            // The following 2 fixes should be done in the stylesheets, and are only required(?) in the Esri Stylesheets
            //Add DOCTYPE
            html = html.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
            //Fix Thumbnail centering
            html = html.Replace(".noThumbnail {", ".noThumbnail {display:inline-block;");
            return html;
        }

        public static async Task<IGisLayer> ParseItemAtPathAsGisLayerAsync(string path)
        {
            return await Task<IGisLayer>.Run(() =>
            {
                string ext = System.IO.Path.GetExtension(path).ToLower();
                if (ext == ".lyrx")
                {
                    return new ProLayerDoc(path) as IGisLayer;
                }
                throw new ApplicationException("Path is not a ArcGIS Pro layer file");
            });
        }

        public static async Task<string> GetMetadataAsXmlAsync(string path)
        {
            return await EsriMetadata.GetContentsAsXmlAsync(path);
        }

        public static bool IsInitialized => EsriLicense.IsRunning;

        public static string Status => EsriLicense.Message;

        public static async Task<bool> InitializeAsync()
        {
            return await EsriLicense.StartAsync();
        }

        //TODO: Make this an observable property, and have the main form monitor it
        // display a progressing dialog or message on main form if message is not null
        public static string ProgressorMessage { get; internal set; }

    }
}

