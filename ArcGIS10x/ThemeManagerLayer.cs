using ESRI.ArcGIS.Carto;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    public class TmMap : GisLayer, IGisLayer
    {
        private readonly string _path;
        public TmMap(string path)
        {
            _path = path;
            DataType = "ArcMap Document";
            IsGroup = true;
            SubLayers = new List<IGisLayer>();
        }

        public async Task LoadAsync()
        {
            IMapDocument mapDoc = await MapUtilities.GetMapDocumentFromFileNameAsync(_path);
            await Task.Run(() =>
            {
                // This will read the open map document, which may do blocking IO
                LoadMaps(mapDoc);
            });
            mapDoc.Close();
        }

        private void LoadMaps(IMapDocument mapDoc)
        {
            int count = mapDoc.MapCount;
            for (int i = 0; i < count; i++)
                ((List<IGisLayer>)SubLayers).Add(new TmMapFrame(mapDoc.Map[i]));
        }

        public override void Close() { }

    }

    public class TmMapFrame : GisLayer, IGisLayer
    {
        private readonly IMap _map;

        public TmMapFrame(IMap map)
        {
            _map = map;
            Name = _map.Name;
            DataType = "Map Frame";
            IsGroup = true;
            SubLayers = GetSubLayers();
        }
    
        private List<IGisLayer> GetSubLayers()
        {
            var subLayers = new List<IGisLayer>();
            int count = _map.LayerCount;
            for (int i = 0; i < count; i++)
                subLayers.Add(new TmLayer(_map.Layer[i]));
            return subLayers;
        }

    }

    public class TmLayer : GisLayer, IGisLayer
    {
        private readonly string _path;
        private ILayer _layer;

        public TmLayer(string path)
        {
            _path = path;
        }

        internal TmLayer(ILayer layer)
        {
            _layer = layer;
            Initialize(_layer);
        }

        public async Task LoadAsync()
        {
            // Create a Layer object from a layer file
            var layer = await LayerUtilities.GetLayerFromLayerFileAsync(_path);
            // The layer object was fully hydrated from the file; Initialize will do no IO.
            Initialize(layer);
        }

        private void Initialize(ILayer layer)
        {

            // Layer properties
            Name = layer.Name;
            DataType =  LayerUtilities.GetLayerDescriptionFromLayer(layer);
            IsGroup = DataType == "Group Layer";
            SubLayers = GetSubLayers(layer);

            // data source properties;
            if (LayerUtilities.HasDataSetName(layer))
            {
                DataSource = GetDataSourceFullNameFromLayer(layer);
                WorkspacePath = LayerUtilities.GetWorkspacePathFromLayer(layer);
                WorkspaceProgId = LayerUtilities.GetWorkspaceProgIDFromLayer(layer);
                WorkspaceType = LayerUtilities.GetWorkspaceTypeFromLayer(layer);
                Container = LayerUtilities.GetDataSourceContainerFromLayer(layer);
                ContainerType = LayerUtilities.GetDataSourceContainerTypeFromLayer(layer);
                DataSourceName = LayerUtilities.GetDataSourceNameFromLayer(layer);
                DataSetName = LayerUtilities.GetDataSetNameFromLayer(layer);
                DataSetType = LayerUtilities.GetDataSetTypeFromLayer(layer);
            }
            else if (LayerUtilities.HasAGSServerObjectName(layer))
            {
                DataSource = LayerUtilities.GetURLFromAGSLayer(layer);
                DataSourceName = LayerUtilities.GetNameFromAGSLayer(layer);
                DataSetName = DataSourceName;
                DataSetType = LayerUtilities.GetTypeFromAGSLayer(layer);
                WorkspacePath = null;
                WorkspaceProgId = null;
                WorkspaceType = null;
                Container = null;
                ContainerType = null;
            }
            else if (LayerUtilities.HasIMSServiceDescription(layer))
            {
                DataSource = LayerUtilities.GetURLFromIMSLayer(layer);
                DataSourceName = LayerUtilities.GetNameFromIMSLayer(layer);
                DataSetName = DataSourceName;
                DataSetType = LayerUtilities.GetTypeFromIMSLayer(layer);
                WorkspacePath = null;
                WorkspaceProgId = null;
                WorkspaceType = null;
                Container = null;
                ContainerType = null;
            }
            else if (LayerUtilities.HasWMSConnectionName(layer))
            {
                DataSource = LayerUtilities.GetURLFromWMSLayer(layer);
                DataSourceName = DataSource;
                DataSetName = DataSource;
                DataSetType = LayerUtilities.GetAllPropertiesFromWMSLayer(layer);
                WorkspacePath = null;
                WorkspaceProgId = null;
                WorkspaceType = null;
                Container = null;
                ContainerType = null;
            }
            else if (LayerUtilities.HasWMTSConnectionName(layer))
            {
                DataSource = LayerUtilities.GetURLFromWMTSLayer(layer);
                DataSourceName = DataSource;
                DataSetName = DataSource;
                DataSetType = LayerUtilities.GetAllPropertiesFromWMSLayer(layer);
                WorkspacePath = null;
                WorkspaceProgId = null;
                WorkspaceType = null;
                Container = null;
                ContainerType = null;
            }
            else if (LayerUtilities.HasDataSourceName(layer))
            {
                DataSource = LayerUtilities.GetDataSourceName(layer);
                DataSourceName = DataSource;
                DataSetName = DataSource;
                DataSetType = DataSource;
                WorkspacePath = null;
                WorkspaceProgId = null;
                WorkspaceType = null;
                Container = null;
                ContainerType = null;
            }
            else
            {
                DataSource = "!Error, Unable to determine data source type";
                WorkspacePath = null;
                WorkspaceProgId = null;
                WorkspaceType = null;
                Container = null;
                ContainerType = null;
                DataSourceName = null;
                DataSetName = null;
                DataSetType = null;
            }
            if (string.IsNullOrEmpty(DataSource))
                DataSource = "!Error - Data source not found";
        }

        public override void Close() { }

        private List<IGisLayer> GetSubLayers(ILayer layer)
        {
            var subLayers = new List<IGisLayer>();
            // GroupLayer implements IGroupLayer and ICompositeLayer
            if (!(layer is ICompositeLayer))
            {
                return subLayers;
            }
            ICompositeLayer gl = (ICompositeLayer)_layer;
            int count = gl.Count;
            for (int i = 0; i < count; i++)
                subLayers.Add(new TmLayer(gl.Layer[i]));
            return subLayers;
        }

        // Potential cleanup; this could be a derived property from the other layer properties
        private static string GetDataSourceFullNameFromLayer(ILayer layer)
        {
            if (!(layer is IDataLayer))
                return null;
            string workspace = LayerUtilities.GetWorkspacePathFromLayer(layer);
            string container = LayerUtilities.GetDataSourceContainerFromLayer(layer);
            string dataset = LayerUtilities.GetDataSourceNameFromLayer(layer);
            if (dataset == null)
                return null;
            if (workspace == null)
                return dataset;
            if (container == null)
                return Path.Combine(workspace, dataset);
            return Path.Combine(Path.Combine(workspace, container), dataset);
        }
    }

}
