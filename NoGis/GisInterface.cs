using System;
using System.Threading.Tasks;
using System.Xml.Xsl;

// The following issues result from supporting an expected public API
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    public class GisInterface
    {
        public static XsltArgumentList EsriProcessingArguments()
        {
            return new XsltArgumentList();
        }

        public static string CleanEsriMetadataHtml(string html)
        {
            return html;
        }

        public static async Task<IGisLayer> ParseItemAtPathAsGisLayerAsync(string path)
        {
            return new GisLayer();
        }

        public static async Task<string> GetMetadataAsXmlAsync(string path)
        {
            throw new ApplicationException("A GIS application must be installed to display this metadata.");
        }

        public static bool IsInitialized => true;

        public static string Status => "No GIS Support avaialble.";

        public static async Task<bool> InitializeAsync()
        {
            return true;
        }

        public static string ProgressorMessage => null;

        public static string Uuid => "{AFC0AC76-5982-4DD8-B544-06766CD46634}";

        public static string Name => "No GIS";

    }
}

