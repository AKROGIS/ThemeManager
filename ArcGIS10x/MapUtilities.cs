using ESRI.ArcGIS.Carto;
using System;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    static class MapUtilities
    {
        private static MapDocument _mapDocument;

        private static async Task<bool> InitLayerFileAsync(string file)
        {
            if (_mapDocument == null)
            {
                await EsriLicense.GetLicenseAsync();
                _mapDocument = new MapDocumentClass();
            }
            //TODO: await an async open
            _mapDocument.Open(file);
            return _mapDocument.IsMapDocument[file];
        }

        internal static async Task<IMapDocument> GetMapDocumentFromFileNameAsync(string file)
        {
            if (!await InitLayerFileAsync(file))
                throw new Exception("Map file is not valid.");
            return _mapDocument;
        }

    }
}
