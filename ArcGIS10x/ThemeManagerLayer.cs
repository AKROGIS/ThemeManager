using ESRI.ArcGIS.Carto;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NPS.AKRO.ThemeManager.ArcGIS
{

    //TODO: make TmLayer generic enough to convert Map and Map frame into TmLayer
    //TODO make TMLayer support Pro layers
    //Hmm.. Maybe publish an interface, and hide the actual implemntation

    public class TmMap
    {
        private IMapDocument _mapDoc;
        public TmMap(string path)
        {
            _mapDoc = MapUtilities.GetMapDocumentFromFileName(path);
        }

        public IEnumerable<TmMapFrame> MapFrames
        {
            get
            {
                int count = _mapDoc.MapCount;
                for (int i = 0; i < count; i++)
                    yield return new TmMapFrame(_mapDoc.Map[i]);
            }
        }

        public void Close()
        {
            //TODO: This is lame: GetMapDocumentFromFileName() should close file before returning
            _mapDoc.Close();
        }
    }

    public class TmMapFrame
    {

        IMap _map;
        public TmMapFrame(IMap map)
        {
            _map = map;
        }

        public string Name {
            get
            {
                return _map.Name;
            }
        }
    
        public IEnumerable<TmLayer> Layers
        {
            get
            {
                int count = _map.LayerCount;
                for (int i = 0; i < count; i++)
                    yield return new TmLayer(_map.Layer[i]);
            }
        }

    }

    public class TmLayer
    {
        private ILayer _layer;

        public TmLayer(string path)
        {
            var _layer = LayerUtilities.GetLayerFromLayerFile(path);
            Initialize(_layer);

        }

        public TmLayer(ILayer layer)
        {
            _layer = layer;
            Initialize(_layer);
        }

        public string DataSource;
        public string WorkspacePath;
        public string WorkspaceProgId;
        public string WorkspaceType;
        public string Container;
        public string ContainerType;
        public string DataSourceName;
        public string DataSetName;
        public string DataSetType;

        private void Initialize(ILayer layer)
        {
            // Can be called on any layer type, but it will not get sub layer information
            // and a lot of info will be lacking if this is not a data layer
            //LayerUtilities.GetLayerDescriptionFromLayer(layer);
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

        public void Close()
        {
            //TODO: This is lame: GetLayerFromLayerFile() should close file before returning
            LayerUtilities.CloseOpenLayerFile();
        }

        public string Name
        {
            get
            {
                return _layer.Name;
            }
        }
        public string DataType
        {
            get
            {
                return LayerUtilities.GetLayerDescriptionFromLayer(_layer);
            }
        }
        public bool IsGroup
        {
            get
            {
                return DataType == "Group Layer";
            }
        }

        public IEnumerable<TmLayer> SubLayers
        {
            get
            {
                // GroupLayer implements IGroupLayer and ICompositeLayer
                if (!(_layer is ICompositeLayer))
                {
                    Debug.Print("layer is not an ICompositeLayer; BuildSubThemes must be called with a Group Layer.");
                    yield break;
                }
                ICompositeLayer gl = (ICompositeLayer)_layer;
                int count = gl.Count;
                for (int i = 0; i < count; i++)
                    yield return new TmLayer(gl.Layer[i]);
            }
        }

        //FIXME - get this directly from the layer object, or move this to the themeData object 
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
