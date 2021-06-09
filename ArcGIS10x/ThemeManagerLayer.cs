using ESRI.ArcGIS.Carto;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    public class TmMap : GisLayer, IGisLayer
    {
        private readonly IMapDocument _mapDoc;
        public TmMap(string path)
        {
            _mapDoc = MapUtilities.GetMapDocumentFromFileName(path);
            DataType = "ArcMap Document";
            IsGroup = true;
        }

        public override IEnumerable<IGisLayer> SubLayers
        {
            get
            {
                int count = _mapDoc.MapCount;
                for (int i = 0; i < count; i++)
                    yield return new TmMapFrame(_mapDoc.Map[i]);
            }
        }

        public override void Close()
        {
            //TODO: This is lame: GetMapDocumentFromFileName() should close file before returning
            _mapDoc.Close();
        }
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
        }
    
        public override IEnumerable<IGisLayer> SubLayers
        {
            get
            {
                int count = _map.LayerCount;
                for (int i = 0; i < count; i++)
                    yield return new TmLayer(_map.Layer[i]);
            }
        }

    }

    public class TmLayer : GisLayer, IGisLayer
    {
        private readonly ILayer _layer;

        public TmLayer(string path)
        {
            var _layer = LayerUtilities.GetLayerFromLayerFile(path);
            Initialize(_layer);

        }

        internal TmLayer(ILayer layer)
        {
            _layer = layer;
            Initialize(_layer);
        }

        private void Initialize(ILayer layer)
        {
            // Layer properties
            Name = layer.Name;
            DataType =  LayerUtilities.GetLayerDescriptionFromLayer(layer);
            IsGroup = DataType == "Group Layer";

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

        public override void Close()
        {
            //TODO: This is lame: GetLayerFromLayerFile() should close file before returning
            LayerUtilities.CloseOpenLayerFile();
        }

        public override IEnumerable<IGisLayer> SubLayers
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
