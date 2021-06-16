using ArcGIS.Core.CIM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.ArcGIS
{

    public class ProLayerDoc : GisLayer, IGisLayer
    {
        private readonly string _path;
        private CIMLayerDocument _layerDoc;
        private ProLayer _proxy;

        public ProLayerDoc(string path)
        {
            _path = path;
        }
        public async Task OpenAsync()
        {
            await Task.Run(() => { Open(); });
        }
        private void Open()
        {
            var text = System.IO.File.ReadAllText(_path, Encoding.UTF8);
            _layerDoc = CIMLayerDocument.FromJson(text);
            Initialize();
        }

        private void Initialize()
        {
            // If a layer doc has only one layer, pose as that layer, otherwise pose as a group layer
            if (_layerDoc.Layers.Length == 1)
            {
                _proxy = new ProLayer(_path, _layerDoc, _layerDoc.Layers[0]);

                Container = _proxy.Container;
                ContainerType = _proxy.ContainerType;
                DataSetName = _proxy.DataSetName;
                DataSetType = _proxy.DataSetType;
                DataSource = _proxy.DataSource;
                DataSourceName = _proxy.DataSourceName;
                DataType = _proxy.DataType;
                IsGroup = _proxy.IsGroup;
                Name = _proxy.Name;
                WorkspacePath = _proxy.WorkspacePath;
                WorkspaceProgId = _proxy.WorkspaceProgId;
                WorkspaceType = _proxy.WorkspaceType;

            }
            else if (_layerDoc.Layers.Length > 1)
            {
                IsGroup = true;
                DataType = "Group Layer";
                SubLayers = GetSubLayers();
            }
            else
            {
                throw new ApplicationException("The layer file to have less than 1 layer");
            }
        }

        private IEnumerable<IGisLayer> GetSubLayers()
        {
            if (_proxy == null)
            {
                return _layerDoc.Layers.Select(l => new ProLayer(_path, _layerDoc, l));
            }
            else
            {
                return _proxy.SubLayers;
            }
        }
    }
    public class ProLayer : GisLayer, IGisLayer
    {
        private readonly CIMDefinition _layer;
        private string _layerClassName;
        private readonly CIMLayerDocument _layerDoc;
        private readonly string _path;
        private IEnumerable<string> _subLayers;

        public ProLayer(string path, CIMLayerDocument doc, string uri)
        {
            _path = path;
            _layerDoc = doc;
            _layer = doc.LayerDefinitions.FirstOrDefault(l => l.URI == uri);
            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                DataType = "Error: " + ex.Message;
            }
            if (DataType == null) { DataType = LayerDescription; }
            SubLayers = GetSubLayers();
        }

        private IEnumerable<IGisLayer> GetSubLayers()
        {
            if (_subLayers == null)
            {
                return new List<IGisLayer>();
            }
            else
            {
                return _subLayers.Select(l => new ProLayer(_path, _layerDoc, l));
            }
        }

        #region Layer Classes
        /*
         Useful properties on the sub classes of CIMBaseLayer (sub class of CIMDefinition):
         All we need is the data connection OR a list of "interesting" sub layers

         Theme Manager only cares about composite layers or layers with data source (Data or Service Connection in Pro speak)
         If a layer has both, a choice must be made if the layer is going to be a group or a single theme.  If the individual
         sub layers can/do have different metadata (i.e. Network Layer), then use a group, if they do not (i.e. Mosaic dataset),
         then do not. If a composite layer is shown as a data source, then the sub layers will be hidden.
         
         The type of the Layer and data source the geometry of the selection symbol is used
         to set the iconography in the browse panel (for example we use the annotation icon for a feature class
         in a annotation layer. Besides iconography, the data connection is used to find the metadata for the data source.
         
         Here are all the published CIM Layer types (as of Pro 2.8 circa 2021-06-01)
         Annotated with the property to obtain a sub class of ArcGIS.Core.CIM.CIMDataConnection

         ArcGIS.Core.CIM.CIMBaseLayer - an abstract base class with no useful members
            ArcGIS.Core.CIM.CIMBasicFeatureLayer - CIMDataConnection FeatureTable.DataConnection
                ArcGIS.Core.CIM.CIMAnnotationLayer - See CIMBasicFeatureLayer
                ArcGIS.Core.CIM.CIMDimensionLayer - See CIMBasicFeatureLayer
                ArcGIS.Core.CIM.CIMGeoFeatureLayerBase - See CIMBasicFeatureLayer
                    ArcGIS.Core.CIM.CIMFeatureLayer - See CIMBasicFeatureLayer
                        ArcGIS.Core.CIM.CIMFeatureMosaicSubLayer - See CIMBasicFeatureLayer
                        ArcGIS.Core.CIM.CIMNitfFeatureSubLayer - See CIMBasicFeatureLayer
                ArcGIS.Core.CIM.CIMSubtypeGroupLayer - See CIMBasicFeatureLayer
                ArcGIS.Core.CIM.CIMSubtypeGroupLayerBase - See CIMBasicFeatureLayer
            ArcGIS.Core.CIM.CIMBuildingDisciplineLayer - No data connection; use string[] CategoryLayers
            ArcGIS.Core.CIM.CIMBuildingDisciplineSceneLayer - No data connection; use string[] SubLayers
            ArcGIS.Core.CIM.CIMBuildingLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMBuildingSceneLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMGALayer - CIMDataConnection Method.DataConnection (dynamic Geostatistical Analyst layer)
            ArcGIS.Core.CIM.CIMGeodatabaseErrorLayer - CIMWorkspaceConnection WorkspaceConnection and strings for point, line, poly and object layers (unclear if they are CIM paths or geodatabase paths)
            ArcGIS.Core.CIM.CIMGraphicsLayer - No data connection
            ArcGIS.Core.CIM.CIMGroupLayer - No data connection; use string[] Layers to get the URI of sub layers
            ArcGIS.Core.CIM.CIMKMLLayer - CIMKMLDataConnection DataConnection
            ArcGIS.Core.CIM.CIMKnowledgeGraphLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMLASDatasetLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMMosaicLayer - CIMDataConnection MosaicDatasetConnection; Note this is a group layer can also get the DataConnection of ImageLayer (the URI of a CIMRasterLayer)
            ArcGIS.Core.CIM.CIMNALayer - CIMDataConnection NetworkDataset and/or CIMDataConnection NAWorkspace, or render as group with URIs to sub layers in Layers
            ArcGIS.Core.CIM.CIMNetworkDatasetLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMNitfLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMParcelFabricLayer - a group layer use string[] AllLayers
            ArcGIS.Core.CIM.CIMParcelLayer - CIMDataConnection ParcelConnection
            ArcGIS.Core.CIM.CIMPointCloudLayer - CIMSceneDataConnection DataConnection
            ArcGIS.Core.CIM.CIMRasterLayer - CIMDataConnection DataConnection
                ArcGIS.Core.CIM.CIMImageServiceLayer - see CIMRasterLayer
                ArcGIS.Core.CIM.CIMNitfImageSubLayer - see CIMRasterLayer
            ArcGIS.Core.CIM.CIMSceneServiceLayer - CIMSceneDataConnection DataConnection
            ArcGIS.Core.CIM.CIMServiceLayer - CIMServiceConnection ServiceConnection; ignore the service sub layers
                ArcGIS.Core.CIM.CIMDynamicServiceLayer - See CIMServiceLayer
                ArcGIS.Core.CIM.CIMGlobeServiceLayer - See CIMServiceLayer
                ArcGIS.Core.CIM.CIMTiledServiceLayer - See CIMServiceLayer
            ArcGIS.Core.CIM.CIMTerrainLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMTinLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMTopologyLayer - CIMDataConnection TopologyConnection; also a group layer string[] AllLayers
            ArcGIS.Core.CIM.CIMTraceNetworkLayer - CIMDataConnection DataConnection; Also a group layer with DirtyAreaLayer, LineErrorLayer, PointErrorLayer, and SystemJunctionsLayer
            ArcGIS.Core.CIM.CIMUtilityNetworkLayer - CIMDataConnection DataConnection; Also a group layer with DirtyAreaLayer, LineErrorLayer, PointErrorLayer, and PolygonErrorLayer 
            ArcGIS.Core.CIM.CIMVectorTileLayer - CIMVectorTileDataConnection DataConnection
            ArcGIS.Core.CIM.CIMVoxelLayer - CIMDataConnection DataConnection
        */

        private void Initialize()
        {
            Name = _layer.Name;
            if (_layer is CIMBasicFeatureLayer layer1) { InitBasicFeature(layer1); return; };
            // CIMBasicFeatureLayer covers: CIMAnnotationLayer, CIMDimensionLayer, CIMGeoFeatureLayerBase, CIMSubtypeGroupLayer, CIMSubtypeGroupLayerBase
            // CIMGeoFeatureLayerBase covers: CIMFeatureLayer
            // CIMFeatureLayer covers: CIMFeatureMosaicSubLayer, CIMNitfFeatureSubLayer
            if (_layer is CIMBuildingDisciplineLayer layer2) { InitBuildingDiscipline(layer2); return; };
            if (_layer is CIMBuildingDisciplineSceneLayer layer3) { InitBuildingDisciplineScene(layer3); return; };
            if (_layer is CIMBuildingLayer layer4) { InitBuilding(layer4); return; };
            if (_layer is CIMBuildingSceneLayer layer5) { InitBuildingScene(layer5); return; };
            if (_layer is CIMGALayer layer6) { InitGA(layer6); return; };
            if (_layer is CIMGeodatabaseErrorLayer layer7) { InitGeodatabaseError(layer7); return; };
            if (_layer is CIMGroupLayer layer9) { InitGroup(layer9); return; };
            if (_layer is CIMKMLLayer layer10) { InitKML(layer10); return; };
            if (_layer is CIMLASDatasetLayer layer12) { InitLASDataset(layer12); return; };
            if (_layer is CIMMosaicLayer layer13) { InitMosaic(layer13); return; };
            if (_layer is CIMNALayer layer14) { InitNA(layer14); return; };
            if (_layer is CIMNetworkDatasetLayer layer15) { InitNetworkDataset(layer15); return; };
            if (_layer is CIMParcelFabricLayer layer17) { InitParcelFabric(layer17); return; };
            if (_layer is CIMParcelLayer layer18) { InitParcel(layer18); return; };
            if (_layer is CIMPointCloudLayer layer19) { InitPointCloud(layer19); return; };
            if (_layer is CIMRasterLayer layer20) { InitRaster(layer20); return; };
            // CIMRasterLayer covers: CIMImageServiceLayer, CIMNitfImageSubLayer
            if (_layer is CIMSceneServiceLayer layer21) { InitSceneService(layer21); return; };
            if (_layer is CIMServiceLayer layer22) { InitService(layer22); return; };
            // CIMServiceLayer covers: CIMDynamicServiceLayer, CIMGlobeServiceLayer, CIMTiledServiceLayer
            if (_layer is CIMTerrainLayer layer23) { InitTerrain(layer23); return; };
            if (_layer is CIMTinLayer layer24) { InitTin(layer24); return; };
            if (_layer is CIMTopologyLayer layer25) { InitTopology(layer25); return; };
            if (_layer is CIMTraceNetworkLayer layer26) { InitTraceNetwork(layer26); return; };
            if (_layer is CIMUtilityNetworkLayer layer27) { InitUtilityNetwork(layer27); return; };
            if (_layer is CIMVectorTileLayer layer28) { InitVectorTile(layer28); return; };
#if !Pro25
            // Added in 2.6
            if (_layer is CIMGraphicsLayer layer8) { InitGraphics(layer8); return; };
            if (_layer is CIMNitfLayer layer16) { InitNitf(layer16); return; };
#if !Pro26
            // Added in 2.7
            if (_layer is CIMKnowledgeGraphLayer layer11) { InitKnowledgeGraph(layer11); return; };
            if (_layer is CIMVoxelLayer layer29) { InitVoxel(layer29); return; };
#endif
#endif
        }

            // All layers type that yield have subLayes (IsGroup = true) should have no
            // connection properties and have "Group" in the data type to get the group icon.
            private void InitBasicFeature(CIMBasicFeatureLayer layer)
        {
            InitDataConnection(layer.FeatureTable.DataConnection);
            GeometryType = BuildGeometryType(layer);
        }
        private void InitBuildingDiscipline(CIMBuildingDisciplineLayer layer)
        {
            IsGroup = true;
            _subLayers = layer.CategoryLayers;
            DataType = "Building Discipline Group Layer";
        }
        private void InitBuildingDisciplineScene(CIMBuildingDisciplineSceneLayer layer)
        {
            IsGroup = true;
            _subLayers = layer.SubLayers;
            DataType = "Building Discipline Scene Group Layer";
        }
        private void InitBuilding(CIMBuildingLayer layer)
        {
            IsGroup = true;
            _subLayers = layer.BuildingDisciplineLayers;
            DataType = "Building Group Layer";
        }
        private void InitBuildingScene(CIMBuildingSceneLayer layer)
        {
            IsGroup = true;
            _subLayers = layer.SubLayers;
            DataType = "Building Scene Group Layer";
        }
        private void InitGA(CIMGALayer layer)
        {
            _layerClassName = "Geostatistical Analysis Layer";
            InitDataConnection(layer.Method.DataConnection);
        }
        private void InitGeodatabaseError(CIMGeodatabaseErrorLayer layer)
        {
            IsGroup = true;
            _subLayers = new string[] { layer.PointLayer, layer.LineLayer, layer.PolygonLayer, layer.ObjectTable };
            DataType = "Geodatabase Error Group Layer";
        }
        private void InitGroup(CIMGroupLayer layer)
        {
            IsGroup = true;
            _subLayers = layer.Layers;
            DataType = "Group Layer";
        }
        private void InitKML(CIMKMLLayer layer)
        {
            _layerClassName = "KML Layer";
            InitDataConnection(layer.DataConnection);
        }
        private void InitLASDataset(CIMLASDatasetLayer layer)
        {
            _layerClassName = "LAS Dataset Layer";
            InitDataConnection(layer.DataConnection);
        }
        private void InitMosaic(CIMMosaicLayer layer)
        {
            InitDataConnection(layer.MosaicDatasetConnection);
            // Note: could also represented as a group layer
            // However, there is only one metadata item, so it makes sense to have only one TM item
            // _subLayers = new string[] { layer.BoundaryLayer, layer.FootprintLayer, layer.SeamlineLayer, layer.ImageLayer };
        }
        private void InitNA(CIMNALayer layer)
        {
            IsGroup = true;
            _subLayers = layer.Layers;
            DataType = "Network Analyst Group Layer";
        }
        private void InitNetworkDataset(CIMNetworkDatasetLayer layer)
        {
            _layerClassName = "Network Dataset Layer";
            InitDataConnection(layer.DataConnection);
        }
        private void InitParcelFabric(CIMParcelFabricLayer layer)
        {
            IsGroup = true;
            _subLayers = layer.AllLayers;
            DataType = "Parcel Fabric Layer";
        }
        private void InitParcel(CIMParcelLayer layer)
        {
            InitDataConnection(layer.ParcelConnection);
        }
        private void InitPointCloud(CIMPointCloudLayer layer)
        {
            _layerClassName = "Point Cloud Layer";
            InitDataConnection(layer.DataConnection);
        }
        private void InitRaster(CIMRasterLayer layer)
        {
            InitDataConnection(layer.DataConnection);
        }
        private void InitSceneService(CIMSceneServiceLayer layer)
        {
            _layerClassName = "Scene Service Layer";
            InitDataConnection(layer.DataConnection);
        }
        private void InitService(CIMServiceLayer layer)
        {
            InitDataConnection(layer.ServiceConnection);
        }
        private void InitTerrain(CIMTerrainLayer layer)
        {
            InitDataConnection(layer.DataConnection);
        }
        private void InitTin(CIMTinLayer layer)
        {
            InitDataConnection(layer.DataConnection);
        }
        private void InitTopology(CIMTopologyLayer layer)
        {
            InitDataConnection(layer.TopologyConnection);
            // Or it could be a group layer
            // However the sub layers may vary depending on the current topology issues
            //IsGroup = true;
            //_subLayers = layer.AllLayers;
            //DataType = "Topology Group Layer";

        }
        private void InitTraceNetwork(CIMTraceNetworkLayer layer)
        {
            _layerClassName = "Trace Network Layer";
            InitDataConnection(layer.DataConnection);
            // Or it could be a group layer
            //IsGroup = true;
            //_subLayers = new string[] { layer.PointErrorLayer, layer.LineErrorLayer, layer.DirtyAreaLayer };
            //DataType = "Trace Network Group Layer";
        }
        private void InitUtilityNetwork(CIMUtilityNetworkLayer layer)
        {
            _layerClassName = "Utility Network Layer";
            InitDataConnection(layer.DataConnection);
            // Or it could be a group layer
            //IsGroup = true;
            //_subLayers = new string[] { layer.PointErrorLayer, layer.LineErrorLayer, layer.PolygonErrorLayer, layer.DirtyAreaLayer };
            //DataType = "Utility Network Group Layer";
        }
        private void InitVectorTile(CIMVectorTileLayer layer)
        {
            _layerClassName = "Vector Tile Layer";
            InitDataConnection(layer.DataConnection);
        }
#if !Pro25
        // Added in 2.6
        private void InitGraphics(CIMGraphicsLayer layer)
        {
            // No sub layers and no data connection, just show a node with a name and a type
        }
        private void InitNitf(CIMNitfLayer layer)
        {
            _layerClassName = "NITF Layer";
            InitDataConnection(layer.DataConnection);
        }
#if !Pro26
        // Added in 2.7
        private void InitKnowledgeGraph(CIMKnowledgeGraphLayer layer)
        {
            _layerClassName = "Knowledge Graph Layer";
            InitDataConnection(layer.DataConnection);
        }
        private void InitVoxel(CIMVoxelLayer layer)
        {
            InitDataConnection(layer.DataConnection);
        }
#endif
#endif

#endregion

        #region Data Connections

        /*
         Useful properties on sub classes of CIMDataConnection (an abstract base class with no useful members)
         Those marked with a * have these standard properties:
            string CustomWorkspaceFactoryCLSID,
            WorkspaceFactory WorkspaceFactory, string WorkspaceConnectionString,
            esriDatasetType DatasetType, string Dataset

         ArcGIS.Core.CIM.CIMFeatureDatasetDataConnection - * + string FeatureDataset
         ArcGIS.Core.CIM.CIMGADataConnection - CIMDataConnection[] DataConnections
         ArcGIS.Core.CIM.CIMInMemoryDatasetDataConnection - N/A
         ArcGIS.Core.CIM.CIMInMemoryWorkspaceDataConnection - N/A
         ArcGIS.Core.CIM.CIMKMLDataConnection - string KMLURI
         ArcGIS.Core.CIM.CIMKnowledgeGraphDataConnection - * + string DefinitionQuery
         ArcGIS.Core.CIM.CIMKnowledgeGraphTableDataConnection - * + string DefinitionQuery, string ExclusionSetURI, string InclusionSetURI
         ArcGIS.Core.CIM.CIMNetCDFRasterDataConnection - * + lots of other special properties
         ArcGIS.Core.CIM.CIMNetCDFStandardDataConnection - * + lots of other special properties
         ArcGIS.Core.CIM.CIMNITFDataConnection - string URI
         ArcGIS.Core.CIM.CIMRasterBandDataConnection - * + string RasterBandName
         ArcGIS.Core.CIM.CIMRelQueryTableDataConnection - CIMDataConnection SourceTable, DestinationTable, string PrimaryKey, ForeignKey and others
         ArcGIS.Core.CIM.CIMRouteEventDataConnection - CIMDataConnection EventTable, CIMDataConnection RouteFeatureClass
         ArcGIS.Core.CIM.CIMSceneDataConnection - string URI
         ArcGIS.Core.CIM.CIMServiceConnection (abstract base class) - string Description
            ArcGIS.Core.CIM.CIMAGSServiceConnection - string URL + CIMServerConnection ServerConnection
            ArcGIS.Core.CIM.CIMOGCAPIServiceConnection  - CIMInternetServerConnectionBase ServerConnection + string ServiceName
            ArcGIS.Core.CIM.CIMStandardServiceConnection - string URL
            ArcGIS.Core.CIM.CIMWCSServiceConnection - CIMInternetServerConnectionBase ServerConnection + string CoverageName
            ArcGIS.Core.CIM.CIMWFSServiceConnection - CIMInternetServerConnectionBase ServerConnection + string LayerName
            ArcGIS.Core.CIM.CIMWMSServiceConnection - CIMInternetServerConnectionBase ServerConnection + string LayerName
            ArcGIS.Core.CIM.CIMWMTSServiceConnection - CIMInternetServerConnectionBase ServerConnection + string LayerName
         ArcGIS.Core.CIM.CIMSqlQueryDataConnection - * + string SqlQuery,  string OIDFields, and others
         ArcGIS.Core.CIM.CIMStandardDataConnection - *
         ArcGIS.Core.CIM.CIMStreamServiceDataConnection - * + age and expiration members
         ArcGIS.Core.CIM.CIMTableQueryNameDataConnection - * + string WhereClause and others
         ArcGIS.Core.CIM.CIMTemporalDataConnection - * + temporal members
         ArcGIS.Core.CIM.CIMTrackingServerDataConnection - * + purge members
         ArcGIS.Core.CIM.CIMVectorTileDataConnection - string URI, string ResourcesURI
         ArcGIS.Core.CIM.CIMVideoDataConnection - string URI
         ArcGIS.Core.CIM.CIMVoxelDataConnection - string URI
         ArcGIS.Core.CIM.CIMWorkspaceConnection - * less DatasetType and Dataset
         ArcGIS.Core.CIM.CIMXYEventDataConnection - CIMDataConnection XYEventTableDataConnection

         ArcGIS.Core.CIM.CIMInternetServerConnectionBase - string URL, bool Anonymous, string User, string Password
            ArcGIS.Core.CIM.CIMServerConnection - no additional properties
         */

        private void InitDataConnection(CIMDataConnection connection)
        {
            ConnectionClassName = BuildConnectionClassName(connection);
            if (connection is CIMFeatureDatasetDataConnection conn1) { InitDataConnection(conn1); }
            if (connection is CIMGADataConnection conn2) { InitDataConnection(conn2); }
            if (connection is CIMInMemoryDatasetDataConnection conn3) { InitDataConnection(conn3); }
            if (connection is CIMInMemoryWorkspaceDataConnection conn4) { InitDataConnection(conn4); }
            if (connection is CIMKMLDataConnection conn5) { InitDataConnection(conn5); }
            if (connection is CIMNetCDFRasterDataConnection conn8) { InitDataConnection(conn8); }
            if (connection is CIMNetCDFStandardDataConnection conn9) { InitDataConnection(conn9); }
            if (connection is CIMRasterBandDataConnection conn11) { InitDataConnection(conn11); }
            if (connection is CIMRelQueryTableDataConnection conn12) { InitDataConnection(conn12); }
            if (connection is CIMRouteEventDataConnection conn13) { InitDataConnection(conn13); }
            if (connection is CIMSceneDataConnection conn32) { InitDataConnection(conn32); }
            if (connection is CIMAGSServiceConnection conn14) { InitDataConnection(conn14); }
            if (connection is CIMStandardServiceConnection conn16) { InitDataConnection(conn16); }
            if (connection is CIMWCSServiceConnection conn17) { InitDataConnection(conn17); }
            if (connection is CIMWFSServiceConnection conn18) { InitDataConnection(conn18); }
            if (connection is CIMWMSServiceConnection conn19) { InitDataConnection(conn19); }
            if (connection is CIMWMTSServiceConnection conn20) { InitDataConnection(conn20); }
            if (connection is CIMSqlQueryDataConnection conn21) { InitDataConnection(conn21); }
            if (connection is CIMStandardDataConnection conn22) { InitDataConnection(conn22); }
            if (connection is CIMStreamServiceDataConnection conn23) { InitDataConnection(conn23); }
            if (connection is CIMTableQueryNameDataConnection conn24) { InitDataConnection(conn24); }
            if (connection is CIMTemporalDataConnection conn25) { InitDataConnection(conn25); }
            if (connection is CIMTrackingServerDataConnection conn26) { InitDataConnection(conn26); }
            if (connection is CIMVectorTileDataConnection conn27) { InitDataConnection(conn27); }
            if (connection is CIMVideoDataConnection conn28) { InitDataConnection(conn28); }
            if (connection is CIMWorkspaceConnection conn30) { InitDataConnection(conn30); }
            if (connection is CIMXYEventDataConnection conn31) { InitDataConnection(conn31); }
            // Versions before 2.5 are not supported
#if !Pro25
            // Added in 2.6
            if (connection is CIMNITFDataConnection conn10) { InitDataConnection(conn10); }
#if !Pro26
            // Added in 2.7            
            if (connection is CIMKnowledgeGraphDataConnection conn6) { InitDataConnection(conn6); }
            if (connection is CIMKnowledgeGraphTableDataConnection conn7) { InitDataConnection(conn7); }
            if (connection is CIMOGCAPIServiceConnection conn15) { InitDataConnection(conn15); }
            if (connection is CIMVoxelDataConnection conn29) { InitDataConnection(conn29); }
#endif
#endif
        }

        //TODO: Check the URI format for KML, NITF, Scene, VectorTile, Video, Voxel,
        private void InitDataConnection(CIMFeatureDatasetDataConnection connection)
        {
            Container = connection.FeatureDataset;
            ContainerType = "FeatureDataset";
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMGADataConnection connection)
        {
            // TODO: Make this a group theme so we can see all the data sources
            InitDataConnection(connection.DataConnections[0]);
            ConnectionClassName = "Geostatistical Analysis";
        }
        private void InitDataConnection(CIMInMemoryDatasetDataConnection connection) { }
        private void InitDataConnection(CIMInMemoryWorkspaceDataConnection connection) { }
        private void InitDataConnection(CIMKMLDataConnection connection)
        {
            WorkspacePath = connection.KMLURI;
            DataSourceName = Path.GetFileName(WorkspacePath);
            DataSetName = DataSourceName;
            DataSetType = "KML File/Service";
            DataSource = connection.KMLURI;
        }
        private void InitDataConnection(CIMNetCDFRasterDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
            ConnectionClassName = "Net CDF Raster";
        }
        private void InitDataConnection(CIMNetCDFStandardDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
            ConnectionClassName = "Net CDF";
        }
        private void InitDataConnection(CIMRasterBandDataConnection connection)
        {
            Container = connection.Dataset;
            ContainerType = "RasterDataset";
            DataSetName = connection.RasterBandName;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.RasterBandName;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMRelQueryTableDataConnection connection)
        {
            // TODO: Make this a group theme so we can also see all the destination table
            InitDataConnection(connection.SourceTable);
            //InitDataConnection(connection.destinationTable);
        }
        private void InitDataConnection(CIMRouteEventDataConnection connection)
        {
            // TODO: Make this a group theme so we can see all the event table
            InitDataConnection(connection.RouteFeatureClass);
            //InitDataConnection(connection.EventTable);
        }
        private void InitDataConnection(CIMSceneDataConnection connection) { }

        // The next 7 are sub classes of CIMServiceConnection
        private void InitDataConnection(CIMAGSServiceConnection connection)
        {
            WorkspacePath = (connection.ServerConnection as CIMInternetServerConnectionBase)?.URL;
            DataSourceName = connection.ObjectName;
            DataSetName = DataSourceName;
            DataSetType = connection.ObjectType;
            DataSource = connection.URL;
        }
        private void InitDataConnection(CIMStandardServiceConnection connection)
        {
            WorkspacePath = connection.URL;
            DataSourceName = connection.ServiceProvider;
            DataSetName = DataSourceName;
            DataSetType = connection.ServiceType;
            DataSource = connection.URL;
        }
        private void InitDataConnection(CIMWCSServiceConnection connection)
        {
            WorkspacePath = connection.ServerConnection.URL;
            DataSourceName = connection.CoverageName;
            DataSetName = DataSourceName;
            DataSetType = "Web Coverage Service";
            DataSource = BuildFullUrl();
        }
        private void InitDataConnection(CIMWFSServiceConnection connection)
        {
            WorkspacePath = connection.ServerConnection.URL;
            DataSourceName = connection.LayerName;
            DataSetName = DataSourceName;
            DataSetType = "Web Feature Service";
            DataSource = BuildFullUrl();
        }
        private void InitDataConnection(CIMWMSServiceConnection connection)
        {
            WorkspacePath = connection.ServerConnection.URL;
            DataSourceName = connection.LayerName;
            DataSetName = DataSourceName;
            DataSetType = "Web Mapping Service";
            DataSource = BuildFullUrl();
        }
        private void InitDataConnection(CIMWMTSServiceConnection connection)
        {
            WorkspacePath = connection.ServerConnection.URL;
            DataSourceName = connection.LayerName;
            DataSetName = DataSourceName;
            DataSetType = "Web Tile Service";
            DataSource = BuildFullUrl();
        }

        private void InitDataConnection(CIMSqlQueryDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMStandardDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
            ConnectionClassName = null;
        }
        private void InitDataConnection(CIMStreamServiceDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMTableQueryNameDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMTemporalDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMTrackingServerDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMVectorTileDataConnection connection) { }
        private void InitDataConnection(CIMVideoDataConnection connection) { }
        private void InitDataConnection(CIMWorkspaceConnection connection)
        {
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = WorkspacePath;
        }
        private void InitDataConnection(CIMXYEventDataConnection connection)
        {
            InitDataConnection(connection.XYEventTableDataConnection);
            ConnectionClassName = "XY Event";
        }

#if !Pro25
        // Added in 2.6
        private void InitDataConnection(CIMNITFDataConnection connection)
        {
            WorkspacePath = connection.URI;
            DataSourceName = Path.GetFileName(WorkspacePath);
            DataSetName = DataSourceName;
            DataSetType = "NITF files";
            DataSource = connection.URI;
        }            
#if !Pro26
        // Added in 2.7
        private void InitDataConnection(CIMKnowledgeGraphDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMKnowledgeGraphTableDataConnection connection)
        {
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMOGCAPIServiceConnection connection)
        {
            WorkspacePath = connection.ServerConnection.URL;
            DataSourceName = connection.ServiceName;
            DataSetName = DataSourceName;
            DataSetType = "OGC Service";
            DataSource = BuildFullUrl();
            ConnectionClassName = "OGC API Service";
        }
        private void InitDataConnection(CIMVoxelDataConnection connection) { }
#endif
#endif

        #endregion

        private string BuildFullDataSourceName()
        {
            if (DataSourceName == null)
                return null;
            if (WorkspacePath == null)
                return DataSourceName;
            if (Container == null)
                return Path.Combine(WorkspacePath, DataSourceName);
            return Path.Combine(WorkspacePath, Container, DataSourceName);
        }

        private string BuildFullUrl()
        {
            if (DataSourceName == null)
                return WorkspacePath;
            if (WorkspacePath == null)
                return DataSourceName;
            return WorkspacePath + "/" + DataSourceName;
        }

        private string BuildGeometryType(CIMBasicFeatureLayer layer)
        {
            if (layer is CIMAnnotationLayer) return "Annotation";
            if (layer is CIMDimensionLayer) return "Dimension";
            if (layer is CIMFeatureLayer)
            {
                var symbolName = layer.SelectionSymbol.Symbol.GetType().Name;
                switch (symbolName)
                {
                    case "CIMLineSymbol": return "Polyline";
                    case "CIMMeshSymbol": return "Mesh";
                    case "CIMPointSymbol": return "Point";
                    case "CIMPolygonSymbol": return "Polygon";
                    case "CIMTextSymbol": return "Annotation";
                    default: return null;
                }
            }
            return null;
        }

        private string GeometryType;

        private string ConnectionClassName;
        private string LayerClassName
        {
            get
            {
                if (_layerClassName == null)
                {
                    _layerClassName = AddSpacesToClassName(_layer.GetType().Name.Replace("CIM", ""));
                }
                return _layerClassName;
            }
        }
        private string BuildConnectionClassName(CIMDataConnection connection)
        {
            return AddSpacesToClassName(connection.GetType().Name.Replace("CIM", "").Replace("DataConnection","").Replace("ServiceConnection", " Service"));
        }

        private string LayerDescription
        {
            get {
                var descriptors = new string[] { LayerClassName, ConnectionClassName, WorkspaceProgId, ContainerType, DataSetType, GeometryType };
                return string.Join(", ", descriptors.Where(d => !string.IsNullOrEmpty(d)));
            }
        }

        private string FixWorkspacePath(string conn)
        {
            if (conn.StartsWith("DATABASE="))
            {
                var path = conn.Replace("DATABASE=", "");
                // path may be a relative path
                if (path.StartsWith("."))
                {
                    var savedCWD = Environment.CurrentDirectory;
                    Environment.CurrentDirectory = Path.GetDirectoryName(_path);
                    path = Path.GetFullPath(path);
                    Environment.CurrentDirectory = savedCWD;
                }
                return path;
            }
            if (conn.StartsWith("URL="))
            {
                //when workspaceType is FeatureService, the workspacePath is URL=...;URL=... with identical URLs.
                return conn.Replace("URL=", "").Split(';').FirstOrDefault();
            }
            // return everything else as is
            // Based on Theme Manager circa June 2021, this could be a naked path when workspace = "Tin"
            // or connection parameters (";" separated KEY=Value) when workspace = "SDE"
            // or "<DataConnections ... " XML for raster function definition when workspace == "Raster" and DataSetType == "Any"
            return conn;
        }

        private string FixDatasetType(esriDatasetType dt)
        {
            // Do not add spaces - to maintain compatibility with ArcObjects 10.x
            return dt.ToString().Replace("esriDT","");
        }

        /// <summary>
        /// Return a new string with a space before each Upper case letter after a lower case letter.
        /// e.g. "TableQueryNameData" => "Table Query Name Data"
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string AddSpacesToClassName(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            for (int i = 0; i < text.Length-1; i++)
            {
                newText.Append(text[i]);
                // maximize short circuiting the and operation
                if (char.IsUpper(text[i + 1]) && char.IsLower(text[i]))
                    newText.Append(' ');
            }
            newText.Append(text[text.Length - 1]);
            return newText.ToString();
        }

    }
}
