using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.GISClient;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    class LayerUtilities
    {
        private static LayerFile _layerFile;

        private static async Task<bool> InitLayerFileAsync(string file)
        {
            if (_layerFile == null)
            {
                await EsriLicense.GetLicenseAsync();
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

        internal static async Task<ILayer> GetLayerFromLayerFileAsync(string file)
        {
            if (!await InitLayerFileAsync(file))
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
            // following list.  Types need to be in order of preferred discovery.
            // the following are all of the co-classes in ESRI.ArcGIS.Carto that implement ILayer
            // There are additional types in extension libraries that implement ILayer
            // that are not checked (would require loading extension libraries/licenses)
            if (layer is GroupLayer)
                return "Group Layer";
            if (layer is RasterLayer)
                return "Raster Layer";
            if (layer is FeatureLayer)
                return "Feature Layer";
            // Data Type: TIN is Tin, MapServer, Terrain, Topology, Network, Mosaic, GraphicsSub, CompositeGraphics and Dummy*
            if (layer is TinLayer)
                return "Tin Layer";
            //Data Type: ArcGIS Image Service is ImageServer, MapServer, Raster, Terrain, Topology, Network, Mosaic, GraphicsSub, CompositeGraphics and Dummy*
            if (layer is ImageServerLayer)
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
            if (layer is RasterCatalogLayer)
                return "Raster Catalog Layer";
            if (layer is GdbRasterCatalogLayer)
                return "Gdb Raster Catalog Layer";
            if (layer is LasDatasetLayer)
                return "LAS Dataset Layer";
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
            if (layer is CadastralFabricSubLayer)
                return "Cadastral Fabric SubLayer";
            if (layer is CadAnnotationLayer)
                return "Cad Annotation Layer";
            if (layer is CadFeatureLayer)
                return "Cad Feature Layer";
            if (layer is CoverageAnnotationLayer)
                return "Coverage Annotation Layer";
            if (layer is BasemapLayer)
                return "BaseMap Layer";
            if (layer is RasterBasemapLayer)
                return "Raster Basemap Layer";
            if (layer is FDOGraphicsLayer)
                return "FDO Graphics Layer";
            if (layer is FDOGraphicsSublayer)
                return "FDO Graphics Sublayer";
            if (layer is NITFGraphicsLayer)
                return "NITF Graphics Layer";
            if (layer is BasemapSubLayer)
                return "Basemap SubLayer";
            if (layer is CadLayer)
                return "CAD Layer";
            if (layer is MapServerRESTLayer)
                return "MapServer REST Layer";
            if (layer is MapServerRESTSubLayer)
                return "MapServer REST SubLayer";
            if (layer is SearchResultsLayer)
                return "Search Results Layer";
            if (layer is ProcessLayer)
                return "Process Layer";
            if (layer is MosaicLayer)
                return "Mosaic Layer";
            if (layer is MapServerLayer)
                return "MapServer Layer";
            // The following classes implement ILayer, but they are in extension libraries
            /*
            if (layer is GeoVideoLayer) //ESRI.ArcGIS.GlobeCore
                return "GeoVideo Layer";
            if (layer is GlobeGraphicsLayer) //ESRI.ArcGIS.GlobeCore
                return "Globe Graphics Layer";
            if (layer is GlobeLayer) //ESRI.ArcGIS.GlobeCore
                return "Globe Layer";
            if (layer is GlobeServerLayer) //ESRI.ArcGIS.GlobeCore
                return "GlobeServer Layer";
            if (layer is GraphicsLayer3D) //ESRI.ArcGIS.3DAnalyst
                return "3D Graphics Layer";
            if (layer is JoinedControlPointLayer) //ESRI.ArcGIS.CadastralUI
                return "Joined Control Point Layer";
            if (layer is JoinedLinePointLayer) //ESRI.ArcGIS.CadastralUI
                return "Joined Line Point Layer";
            if (layer is JoinedParcelLayer) //ESRI.ArcGIS.CadastralUI
                return "Joined Parcel Layer";
            if (layer is JoinedParcelLineLayer) //ESRI.ArcGIS.CadastralUI
                return "Joined Parcel Line Layer";
            if (layer is JoinedPointLayer) //ESRI.ArcGIS.CadastralUI
                return "Joined Point Layer";
            if (layer is KmlLayer) //ESRI.ArcGIS.GlobeCore
                return "KML Layer";
            if (layer is NALayer) //ESRI.ArcGIS.NetworkAnalyst
                return "Network Analysis Layer";
            if (layer is PacketJoinedLayer) //ESRI.ArcGIS.CadastralUI
                return "Packet Joined Layer";
            if (layer is SchematicLayer) //ESRI.ArcGIS.Schematic
                return "Schematic Layer";
            if (layer is TemporalFeatureLayer) //ESRI.ArcGIS.TrackingAnalyst
                return "Temporal Feature Layer";
            */
            if (layer is WMTSLayer)
                return "WM Tile Service Layer"; if (layer is GraphicsSubLayer)
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
            if (datasetName is IRasterBandName)
            {
                // Then the datasetName.Name contains the raster container i.e. raster\band#
                // We already got the container (raster) name, so we need to remove it in this case
                var index = datasetName.Name.LastIndexOf('\\');
                if (index > -1 && index < datasetName.Name.Length -1)
                {
                    return datasetName.Name.Substring(index+1);
                }
            }
            return datasetName.Name;
        }

        internal static string GetDataSetNameFromLayer(ILayer layer)
        {
            if (layer == null || !(layer is IDataset))
                return null;
            IDataset dataset = (IDataset)layer;
            return dataset.Name;
        }

        //There may be other container types (however, we are not using them)
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
                return ((IRasterBandName)datasetName).RasterDatasetName.Type.ToString().Replace("esriDT", "");
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

        internal static string GetURLFromWMTSLayer(ILayer layer)
        {
            IWMTSConnectionName wmtsObjectName = GetWMTSConnectionName(layer);
            if (wmtsObjectName == null)
                return "!Error - WMTS Connection Object Name Not Found";
            return wmtsObjectName.ConnectionProperties.GetProperty("URL").ToString();
        }

        internal static string GetAllPropertiesFromWMSLayer(ILayer layer)
        {
            object keys = null, values = null;
            if (HasWMSConnectionName(layer))
            {
                IWMSConnectionName wmsObjectName = GetWMSConnectionName(layer);
                if (wmsObjectName != null)
                    wmsObjectName.ConnectionProperties.GetAllProperties(out keys, out values);
            }

            if (HasWMTSConnectionName(layer))
            {
                IWMTSConnectionName wmtsObjectName = GetWMTSConnectionName(layer);
                if (wmtsObjectName != null)
                    wmtsObjectName.ConnectionProperties.GetAllProperties(out keys, out values);
            }
            if (keys == null || values == null)
                return "!Error - WMS Connection Object Name Not Found";
            string[] names = (string[])keys;
            object[] props = (object[])values;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < names.Length; i++)
            {
                var prop = props[i] == null || string.IsNullOrWhiteSpace(props[i].ToString())
                    ? "<NONE>"
                    : props[i].ToString();
                sb.Append(names[i] + ":" + prop + "; ");
            }

            var res = sb.ToString();
            return res;
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

        internal static IWMTSConnectionName GetWMTSConnectionName(ILayer layer)
        {
            if (layer == null || !(layer is IDataLayer))
                return null;
            return ((IDataLayer)layer).DataSourceName as IWMTSConnectionName;
        }

        internal static bool HasWMSConnectionName(ILayer layer)
        {
            return (GetWMSConnectionName(layer) == null) ? false : true;
        }

        internal static bool HasWMTSConnectionName(ILayer layer)
        {
            return GetWMTSConnectionName(layer) != null;
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
