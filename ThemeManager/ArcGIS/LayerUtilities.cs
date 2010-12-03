using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Diagnostics;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.esriSystem;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    class LayerUtilities
    {
        private static LayerFile _layerFile;

        private static void GetLicense()
        {
            if (!EsriLicenseManager.Running)
                EsriLicenseManager.Start(true);
            if (!EsriLicenseManager.Running)
                throw new Exception("Could not initialize an ArcGIS license. \n" + EsriLicenseManager.Message);
        }

        private static bool InitLayerFile(string file)
        {
            if (_layerFile == null)
            {
                GetLicense();
                _layerFile = new LayerFileClass();
            }
            //doesn't hurt, and ensures that any layer file left open is closed.
            //all properties on _layerfile throw an exception if _layerfile is not opened
            _layerFile.Close();
            _layerFile.Open(file);
            //return _layerFile.IsLayerFile[file] && _layerFile.Layer != null;
            //The previous check failed on "X:\\GIS\\ThemeMgr\\Albers\\WRST\\IKONOS OrthoNED NearIR- WRST.lyr"
            //It might have been due to a memory problem
            return _layerFile.Layer != null;
        }

        internal static ILayer GetLayerFromLayerFile(string file)
        {
            if (!InitLayerFile(file))
                throw new Exception("!Error - Layer file (" + file + ") is not valid.");
            return _layerFile.Layer;
        }

        internal static void CloseOpenLayerFile()
        {
            if (_layerFile != null)
                _layerFile.Close();
        }

        internal static string GetLayerDescriptionFromLayer(ILayer layer)
        {
            string layerType = GetLayerTypeFromLayer(layer);
            string dataSetType = null;
            if (layer is IDataset)
                dataSetType = ", " + GetDataSetTypeFromLayer(layer);
            string featureInfo = null;
            if (layer is IFeatureLayer)
                featureInfo = ", " + GetFormattedFeatureInfoFromLayer(layer);
            return layerType + (dataSetType ?? "") + (featureInfo ?? "");
        }


        internal static string GetLayerTypeFromLayer(ILayer layer)
        {
            // ESRI's COM objects are multi-typed, so a layer will match multiple types in the
            // following list.  Types need to be in order of prefered discovery.
            // the following are all of the coclasses in ESRI.ArcGIS.Carto that implement ILayer
            // There are additional types in extension libraries that implement ILayer
            // that are not checked (would require loading extension libraries/licenses)
            if (layer is GroupLayer)
                return "Group Layer";
            if (layer is FeatureLayer)
                return "Feature Layer";
            // Data Type: 
            if (layer is TinLayer)  // TIN is Tin, MapServer, Terrain, Topology, Network, Mosaic, GraphicsSub, CompositeGraphics and Dummy*
                return "Tin Layer";
            //Data Type: ArcGIS Image Service is ImageServer, MapServer, Raster, Terrain, Topology, Network, Mosaic, GraphicsSub, CompositeGraphics and Dummy*
            if (layer is ImageServerLayer) // 
                return "ImageServer Layer";
            if (layer is IMSMapLayer)
                return "IMS Map Layer";
            if (layer is IMSSubFeatureLayer)
                return "IMS SubFeature Layer";
            if (layer is IMSSubLayer)
                return "IMS SubLayer";
            if (layer is WCSLayer)
                return "WCS Layer";
            if (layer is WMSGroupLayer)
                return "WMS Group Layer";
            if (layer is WMSMapLayer)
                return "WMS Map Layer";
            if (layer is WMSLayer)
                return "WMS Layer";
            if (layer is MapServerBasicSublayer)
                return "MapServer Basic Sublayer";
            if (layer is MapServerFindSublayer)
                return "MapServer Find Sublayer";
            if (layer is MapServerIdentifySublayer)
                return "MapServer Identify Sublayer";
            if (layer is MapServerQuerySublayer)
                return "MapServer Query Sublayer";
            if (layer is MapServerLayer)
                return "MapServer Layer";
            if (layer is RasterLayer)
                return "Raster Layer";
            if (layer is RasterCatalogLayer)
                return "Raster Catalog Layer";
            if (layer is GdbRasterCatalogLayer)
                return "Gdb Raster Catalog Layer";
            if (layer is TerrainLayer)
                return "Terrain Layer";
            if (layer is TopologyLayer)
                return "Topology Layer";
            if (layer is NetworkLayer)
                return "Network Layer";
            if (layer is DimensionLayer)
                return "Dimension Layer";
            if (layer is CadastralFabricLayer)
                return "Cadastral Fabric Layer";
            //Version 10 only
            if (layer is CadastralFabricSubLayer)
                return "Cadastral Fabric SubLayer";
            if (layer is CadAnnotationLayer)
                return "Cad Annotation Layer";
            if (layer is CadFeatureLayer)
                return "Cad Feature Layer";
            if (layer is CoverageAnnotationLayer)
                return "Coverage Annotation Layer";
            //Version 10 only
            if (layer is BasemapLayer)
                return "BaseMap Layer";
            //Version 10 only
            if (layer is RasterBasemapLayer)
                return "Raster Basemap Layer";
            //Version 10 only
            if (layer is MosaicLayer)
                return "Mosaic Layer";
            if (layer is FDOGraphicsLayer)
                return "FDO Graphics Layer";
            if (layer is FDOGraphicsSublayer)
                return "FDO Graphics Sublayer";
            if (layer is NITFGraphicsLayer)
                return "NITF Graphics Layer";
            if (layer is GraphicsSubLayer)
                return "Graphics SubLayer";
            if (layer is CompositeGraphicsLayer)
                return "Composite Graphics Layer";
            if (layer is DummyGraduatedMarkerLayer)
                return "Dummy Graduated Marker Layer";
            if (layer is DummyLayer)
                return "Dummy Layer";

            return "Unknown Layer Type";
        }

        //esriDatasetType Enum
        internal static string GetDataSetTypeFromLayer(ILayer layer)
        {
            if (layer == null || !(layer is IDataset))
                return null;
            return ((IDataset)layer).Type.ToString().Replace("esriDT", "");
        }

        private static string GetFormattedFeatureInfoFromLayer(ILayer layer)
        {
            if (layer == null || !(layer is IFeatureLayer))
                return "!Error - Layer is not a Feature Layer";

            string dataType = GetDataSourceTypeFromLayer(layer);
            string geomType = GetGeometryTypeFromLayer(layer);
            string featureType = GetFeatureTypeFromLayer(layer);
            geomType = (dataType.ToLower().Contains(geomType.ToLower())) ?
                "" : ", " + geomType;
            featureType = (featureType == "Simple") ?
                "" : ", " + featureType;

            return dataType + geomType + featureType;
        }

        //esriGeometryType Enum
        internal static string GetGeometryTypeFromLayer(ILayer layer)
        {
            if (layer == null || !(layer is IFeatureLayer))
                return "!Error - Layer is not a Feature Layer";
            if (!layer.Valid)
                return "!Error - Layer's feature data is not valid";
            return ((IFeatureLayer)layer).FeatureClass.ShapeType.ToString().Replace("esriGeometry", "");
        }

        //esriFeatureType Enum
        internal static string GetFeatureTypeFromLayer(ILayer layer)
        {
            if (layer == null || !(layer is IFeatureLayer))
                return "!Error - Layer is not a Feature Layer";
            if (!layer.Valid)
                return "!Error - Layer's feature data is not valid";
            return ((IFeatureLayer)layer).FeatureClass.FeatureType.ToString().Replace("esriFT", "");
        }

        // see IFeatureLayer.DataSourceType:  Personal Geodatabase Feature Class, SDE Feature Class, Shapefile Feature Class, ...
        internal static string GetDataSourceTypeFromLayer(ILayer layer)
        {
            if (layer == null || !(layer is IFeatureLayer))
                return "!Error - Layer is not a Feature Layer";
            string result = ((IFeatureLayer)layer).DataSourceType.Replace(" Feature Class", "");
            if (result == "Annotation" || result == "Point" || result == "Arc" || result == "Polygon")
                result = result + " Coverage ";
            return result;
        }

        internal static string GetWorkspacePathFromLayer(ILayer layer)
        {
            IDatasetName datasetName = GetDataSetName(layer);
            if (datasetName == null)
                return "!Error - Data Set Name Not Found";
            if (datasetName.WorkspaceName == null)
                return "!Error - No workspace";
            return datasetName.WorkspaceName.PathName;
        }

        internal static string GetWorkspaceProgIDFromLayer(ILayer layer)
        {
            IDatasetName datasetName = GetDataSetName(layer);
            if (datasetName == null)
                return "!Error - Data Set Name Not Found";
            if (datasetName.WorkspaceName == null)
                return "!Error - No workspace";
            return datasetName.WorkspaceName.WorkspaceFactoryProgID;
        }

        internal static string GetWorkspaceTypeFromLayer(ILayer layer)
        {
            IDatasetName datasetName = GetDataSetName(layer);
            if (datasetName == null)
                return "!Error - Data Set Name Not Found";
            if (datasetName.WorkspaceName == null)
                return "!Error - No workspace";
            return datasetName.WorkspaceName.Type.ToString();
        }

        internal static string GetDataSourceNameFromLayer(ILayer layer)
        {
            IDatasetName datasetName = GetDataSetName(layer);
            if (datasetName == null)
                return "!Error - Data Set Name Not Found";
            return datasetName.Name;
        }

        internal static string GetDataSetNameFromLayer(ILayer layer)
        {
            if (layer == null || !(layer is IDataset))
                return null;
            IDataset dataset = (IDataset)layer;
            return dataset.Name;
        }

        //FIXME - does this have the complete list of container types
        internal static string GetDataSourceContainerFromLayer(ILayer layer)
        {
            IDatasetName datasetName = GetDataSetName(layer);
            if (datasetName == null)
                return "!Error - Data Set Name Not Found";
            if (datasetName is IFeatureClassName && ((IFeatureClassName)datasetName).FeatureDatasetName != null)
                return ((IFeatureClassName)datasetName).FeatureDatasetName.Name;
            if (datasetName is IRasterBandName && ((IRasterBandName)datasetName).RasterDatasetName != null)
                return ((IRasterBandName)datasetName).RasterDatasetName.Name;
            return null;
        }

        internal static string GetDataSourceContainerTypeFromLayer(ILayer layer)
        {
            IDatasetName datasetName = GetDataSetName(layer);
            if (datasetName == null)
                return "!Error - Data Set Name Not Found";
            if (datasetName is IFeatureClassName && ((IFeatureClassName)datasetName).FeatureDatasetName != null)
                return ((IFeatureClassName)datasetName).FeatureDatasetName.Type.ToString().Replace("esriDT", "");
            if (datasetName is IRasterBandName && ((IRasterBandName)datasetName).RasterDatasetName != null)
                return ((IRasterBandName)datasetName).RasterDatasetName.Name.ToString().Replace("esriDT", "");
            return null;
        }

        internal static string GetURLFromAGSLayer(ILayer layer)
        {
            IAGSServerObjectName serverObjectName = GetAGSServerObjectName(layer);
            if (serverObjectName == null)
                return "!Error - AGS Server Object Name Not Found";
            return serverObjectName.URL;
        }

        internal static string GetTypeFromAGSLayer(ILayer layer)
        {
            IAGSServerObjectName serverObjectName = GetAGSServerObjectName(layer);
            if (serverObjectName == null)
                return "!Error - AGS Server Object Name Not Found";
            return serverObjectName.Type;
        }

        internal static string GetNameFromAGSLayer(ILayer layer)
        {
            IAGSServerObjectName serverObjectName = GetAGSServerObjectName(layer);
            if (serverObjectName == null)
                return "!Error - AGS Server Object Name Not Found";
            return serverObjectName.Name;
        }

        internal static string GetURLFromIMSLayer(ILayer layer)
        {
            IIMSServiceDescription serverObjectName = GetIMSServiceDescription(layer);
            if (serverObjectName == null)
                return "!Error - IMS Service Description Not Found";
            return serverObjectName.URL;
        }

        internal static string GetTypeFromIMSLayer(ILayer layer)
        {
            IIMSServiceDescription serverObjectName = GetIMSServiceDescription(layer);
            if (serverObjectName == null)
                return "!Error - IMS Service Description Not Found";
            return serverObjectName.ServiceType.ToString();
        }

        internal static string GetNameFromIMSLayer(ILayer layer)
        {
            IIMSServiceDescription serverObjectName = GetIMSServiceDescription(layer);
            if (serverObjectName == null)
                return "!Error - IMS Service Description Not Found";
            return serverObjectName.Name;
        }

        internal static string GetURLFromWMSLayer(ILayer layer)
        {
            IWMSConnectionName wmsObjectName = GetWMSConnectionName(layer);
            if (wmsObjectName == null)
                return "!Error - WMS Connection Object Name Not Found";
            return wmsObjectName.ConnectionProperties.GetProperty("URL").ToString();
        }

        internal static string GetAllPropertiesFromWMSLayer(ILayer layer)
        {
            IWMSConnectionName wmsObjectName = GetWMSConnectionName(layer);
            if (wmsObjectName == null)
                return "!Error - WMS Connection Object Name Not Found";
            object keys, values;
            wmsObjectName.ConnectionProperties.GetAllProperties(out keys, out values);
            string[] names = (string[])keys;
            object[] props = (object[])values;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < names.Length; i++)
            {
                sb.Append(names[i] + ":" + props[i].ToString() + "; ");
            }
            return sb.ToString();
        }

        internal static IDatasetName GetDataSetName(ILayer layer)
        {
            if (layer == null || !(layer is IDataLayer))
                return null;
            return ((IDataLayer)layer).DataSourceName as IDatasetName;
        }

        internal static bool HasDataSetName(ILayer layer)
        {
            return (GetDataSetName(layer) == null) ? false : true;
        }

        internal static IAGSServerObjectName GetAGSServerObjectName(ILayer layer)
        {
            if (layer == null || !(layer is IDataLayer))
                return null;
            return ((IDataLayer)layer).DataSourceName as IAGSServerObjectName;
        }

        internal static bool HasAGSServerObjectName(ILayer layer)
        {
            return (GetAGSServerObjectName(layer) == null) ? false : true;
        }

        internal static IWMSConnectionName GetWMSConnectionName(ILayer layer)
        {
            if (layer == null || !(layer is IDataLayer))
                return null;
            return ((IDataLayer)layer).DataSourceName as IWMSConnectionName;
        }

        internal static bool HasWMSConnectionName(ILayer layer)
        {
            return (GetWMSConnectionName(layer) == null) ? false : true;
        }

        internal static IIMSServiceDescription GetIMSServiceDescription(ILayer layer)
        {
            if (layer == null || !(layer is IDataLayer))
                return null;
            return ((IDataLayer)layer).DataSourceName as IIMSServiceDescription;
        }

        internal static bool HasIMSServiceDescription(ILayer layer)
        {
            return (GetIMSServiceDescription(layer) == null) ? false : true;
        }

        internal static string GetDataSourceName(ILayer layer)
        {
            if (layer == null || !(layer is IDataLayer))
                return null;
            IName name = ((IDataLayer)layer).DataSourceName as IName;
            if (name == null || string.IsNullOrEmpty(name.NameString))
                return null;
            return name.NameString;
        }

        internal static bool HasDataSourceName(ILayer layer)
        {
            return (GetDataSourceName(layer) == null) ? false : true;
        }

    }
}
