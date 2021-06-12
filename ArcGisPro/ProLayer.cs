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
            }
            else
            {
                throw new ApplicationException("The layer file to have less than 1 layer");
            }
        }

        public override IEnumerable<IGisLayer> SubLayers
        {
            get
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
    }
    public class ProLayer : GisLayer, IGisLayer
    {
        private readonly string _path;
        private readonly CIMLayerDocument _layerDoc;
        private readonly CIMDefinition _layer;
        private IEnumerable<string> _subLayers;
        public ProLayer(string path, CIMLayerDocument doc, string uri)
        {
            _path = path;
            _layerDoc = doc;
            _layer = doc.LayerDefinitions.FirstOrDefault(l => l.URI == uri);
            Initialize();
            if (DataType == null) { DataType = LayerDescription; }
        }

        public override IEnumerable<IGisLayer> SubLayers
        {
            get
            {
                if (_subLayers == null)
                {
                    return base.SubLayers;
                }
                else
                {
                    return _subLayers.Select(l => new ProLayer(_path, _layerDoc, l));
                }
            }
        }

        private void Initialize()
        {
            Name = _layer.Name;
            if (_layer is CIMBasicFeatureLayer layer1) { InitBasicFeature(layer1); return; };
            if (_layer is CIMBuildingDisciplineLayer layer2) { InitBuildingDiscipline(layer2); return; };
            if (_layer is CIMBuildingDisciplineSceneLayer layer3) { InitBuildingDisciplineScene(layer3); return; };
            if (_layer is CIMBuildingLayer layer4) { InitBuilding(layer4); return; };
            if (_layer is CIMBuildingSceneLayer layer5) { InitBuildingScene(layer5); return; };
            if (_layer is CIMGALayer layer6) { InitGA(layer6); return; };
            if (_layer is CIMGeodatabaseErrorLayer layer7) { InitGeodatabaseError(layer7); return; };
            if (_layer is CIMGraphicsLayer layer8) { InitGraphics(layer8); return; };
            if (_layer is CIMGroupLayer layer9) { InitGroup(layer9); return; };
            if (_layer is CIMGALayer layer10) { InitGA(layer10); return; };
            //TODO: Finish list
        }

        private void InitBasicFeature(CIMBasicFeatureLayer layer)
        {
            InitDataConnection(layer.FeatureTable.DataConnection);
        }
        private void InitBuildingDiscipline(CIMBuildingDisciplineLayer layer) { }
        private void InitBuildingDisciplineScene(CIMBuildingDisciplineSceneLayer layer) { }
        private void InitBuilding(CIMBuildingLayer layer) { }
        private void InitBuildingScene(CIMBuildingSceneLayer layer) { }
        private void InitGA(CIMGALayer layer) { }
        private void InitGeodatabaseError(CIMGeodatabaseErrorLayer layer) { }
        private void InitGraphics(CIMGraphicsLayer layer) { }
        private void InitGroup(CIMGroupLayer layer)
        {
            DataType = "Group Layer";
            IsGroup = true;
            _subLayers = layer.Layers;
        }

        /*
            ArcGIS.Core.CIM.CIMBaseLayer
            ArcGIS.Core.CIM.CIMBasicFeatureLayer - CIMDataConnection FeatureTable.DataConnection
                ArcGIS.Core.CIM.CIMAnnotationLayer - See CIMBasicFeatureLayer
                ArcGIS.Core.CIM.CIMDimensionLayer - See CIMBasicFeatureLayer
                ArcGIS.Core.CIM.CIMGeoFeatureLayerBase - See CIMBasicFeatureLayer
                    ArcGIS.Core.CIM.CIMFeatureLayer - See CIMBasicFeatureLayer
                        ArcGIS.Core.CIM.CIMFeatureMosaicSubLayer - See CIMBasicFeatureLayer
                        ArcGIS.Core.CIM.CIMNitfFeatureSubLayer - See CIMBasicFeatureLayer
                ArcGIS.Core.CIM.CIMSubtypeGroupLayer - See CIMBasicFeatureLayer
                ArcGIS.Core.CIM.CIMSubtypeGroupLayerBase - See CIMBasicFeatureLayer
            ArcGIS.Core.CIM.CIMBuildingDisciplineLayer - No data connection; usestring[] CategoryLayers
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


        private void InitDataConnection(CIMDataConnection connection)
        {
            if (connection is CIMStandardDataConnection conn1) { InitDataConnection(conn1); }
            if (connection is CIMGADataConnection conn2) { InitDataConnection(conn2); }
            if (connection is CIMKMLDataConnection conn3) { InitDataConnection(conn3); }
            if (connection is CIMNITFDataConnection conn4) { InitDataConnection(conn4); }
            if (connection is CIMFeatureDatasetDataConnection conn5) { InitDataConnection(conn5); }
            if (connection is CIMStandardDataConnection conn6) { InitDataConnection(conn6); }
            //TODO: finish list
        }

        private void InitDataConnection(CIMStandardDataConnection connection)
        {
            //TODO difference between DataSetName and DataSourceName
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
        private void InitDataConnection(CIMGADataConnection connection) { }
        private void InitDataConnection(CIMKMLDataConnection connection) { }
        private void InitDataConnection(CIMNITFDataConnection connection) { }
        private void InitDataConnection(CIMFeatureDatasetDataConnection connection) {
            Container = connection.FeatureDataset;
            ContainerType = "Feature Dataset";
            DataSetName = connection.Dataset;
            DataSetType = FixDatasetType(connection.DatasetType);
            DataSourceName = connection.Dataset;
            WorkspacePath = FixWorkspacePath(connection.WorkspaceConnectionString);
            WorkspaceProgId = connection.WorkspaceFactory.ToString();
            WorkspaceType = connection.WorkspaceFactory.ToString();
            DataSource = BuildFullDataSourceName();
        }
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

        private string LayerClass =>
            _layer.GetType().Name.Replace("CIM", "").Replace("Layer", " Layer");

        private string LayerDescription =>
            //FIXME: Add other descriptors.  See ArcGis10.x.LayerUtilities.GetLayerDescriptionFromLayer
            // Need Coverage and geometry and other items for iconography
            string.Join(", ", LayerClass, WorkspaceProgId, ContainerType, DataSetType);

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
                    return path;
                }
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
            return dt.ToString().Replace("esriDT","");
        }
    }
}
