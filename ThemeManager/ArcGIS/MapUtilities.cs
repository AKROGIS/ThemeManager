using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    class MapUtilities
    {
        private static MapDocument _mapDocument;

        static void GetLicense()
        {
            if (!EsriLicenseManager.Running)
                EsriLicenseManager.Start(true);
            if (!EsriLicenseManager.Running)
                throw new Exception("Could not initialize an ArcGIS license. \n" + EsriLicenseManager.Message);
        }

        private static bool InitLayerFile(string file)
        {
            if (_mapDocument == null)
            {
                GetLicense();
                _mapDocument = new MapDocumentClass();
            }
            _mapDocument.Open(file);
            return _mapDocument.IsMapDocument[file];
        }

        static public IMapDocument GetMapDocumentFromFileName(string file)
        {
            if (!InitLayerFile(file))
                throw new Exception("Map file is not valid.");
            return _mapDocument;
        }

    }
}
