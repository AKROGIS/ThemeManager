using ESRI.ArcGIS.Carto;
using NPS.AKRO.ThemeManager.Model;
using System;
using System.Diagnostics;
using System.IO;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    static class ThemeBuilder
    {
        internal static void BuildThemesForLayerFile(TmNode tmNode)
        {
            ILayer layer;
            try
            {
                layer = LayerUtilities.GetLayerFromLayerFile(tmNode.Data.Path);
            }
            catch (Exception ex)
            {
                Debug.Print("Unable to load layer file: " + tmNode.Data.Path + " " + ex.Message);
                tmNode.Data.Type = "Unable to load layer file (" + ex.Message + ")";
                return;
            }

            tmNode.Data.Type = LayerUtilities.GetLayerDescriptionFromLayer(layer);
            if (tmNode.Data.Type == "Group Layer")
            {
                BuildSubThemesForGroupLayer(tmNode, layer);
            }
            else
                BuildThemeDataForLayer(tmNode.Data, layer);
            LayerUtilities.CloseOpenLayerFile();
        }

        private static void BuildSubThemesForGroupLayer(TmNode node, ILayer layer)
        {
            // GroupLayer implements IGroupLayer and ICompositeLayer
            if (!(layer is ICompositeLayer))
            {
                Debug.Print("layer is not an ICompositeLayer; BuildSubThemes must be called with a Group Layer.");
                return;
            }
            ICompositeLayer gl = (ICompositeLayer)layer;
            int count = gl.Count;
            for (int i = 0; i < count; i++)
                BuildSubThemeForLayer(node, gl.Layer[i]);
        }

        internal static void BuildSubThemesForMapDocument(TmNode node)
        {
            IMapDocument mapDoc = MapUtilities.GetMapDocumentFromFileName(node.Data.Path);
            int count = mapDoc.MapCount;
            for (int i = 0; i < count; i++)
                BuildSubThemeForMap(node, mapDoc.Map[i]);
            mapDoc.Close();
        }

        private static void BuildSubThemeForMap(TmNode node, IMap map)
        {
            TmNode newNode = new TmNode(TmNodeType.Theme, map.Name, node, new ThemeData(null, "Map Data Frame"), null, null, null);
            node.Add(newNode);
            int count = map.LayerCount;
            for (int i = 0; i < count; i++)
                BuildSubThemeForLayer(newNode, map.Layer[i]);
        }

        private static void BuildSubThemeForLayer(TmNode node, ILayer subLayer)
        {
            if (subLayer is GroupLayer)
            {
                TmNode newNode = new TmNode(TmNodeType.Theme, subLayer.Name, node, new ThemeData(null, "Group Layer"), null, null, null);
                node.Add(newNode);
                BuildSubThemesForGroupLayer(newNode, subLayer);
            }
            else
            {
                string dataType = LayerUtilities.GetLayerDescriptionFromLayer(subLayer);
                ThemeData data = new ThemeData(null, dataType);

                BuildThemeDataForLayer(data, subLayer);

                Metadata md = Metadata.FromDataSource(data);
                TmNode newNode = new TmNode(TmNodeType.Theme, subLayer.Name, node, data, md, null, null);
                node.Add(newNode);
            }
        }

        private static void BuildThemeDataForLayer(ThemeData data, ILayer layer)
        {
            // Can be called on any layer type, but it will not get sub layer information
            // and a lot of info will be lacking if this is not a data layer
            //LayerUtilities.GetLayerDescriptionFromLayer(layer);
            if (LayerUtilities.HasDataSetName(layer))
            {
                data.DataSource = GetDataSourceFullNameFromLayer(layer);
                data.WorkspacePath = LayerUtilities.GetWorkspacePathFromLayer(layer);
                data.WorkspaceProgId = LayerUtilities.GetWorkspaceProgIDFromLayer(layer);
                data.WorkspaceType = LayerUtilities.GetWorkspaceTypeFromLayer(layer);
                data.Container = LayerUtilities.GetDataSourceContainerFromLayer(layer);
                data.ContainerType = LayerUtilities.GetDataSourceContainerTypeFromLayer(layer);
                data.DataSourceName = LayerUtilities.GetDataSourceNameFromLayer(layer);
                data.DataSetName = LayerUtilities.GetDataSetNameFromLayer(layer);
                data.DataSetType = LayerUtilities.GetDataSetTypeFromLayer(layer);
            }
            else if (LayerUtilities.HasAGSServerObjectName(layer))
            {
                data.DataSource = LayerUtilities.GetURLFromAGSLayer(layer);
                data.DataSourceName = LayerUtilities.GetNameFromAGSLayer(layer);
                data.DataSetName = data.DataSourceName;
                data.DataSetType = LayerUtilities.GetTypeFromAGSLayer(layer);
                data.WorkspacePath = null;
                data.WorkspaceProgId = null;
                data.WorkspaceType = null;
                data.Container = null;
                data.ContainerType = null;
            }
            else if (LayerUtilities.HasIMSServiceDescription(layer))
            {
                data.DataSource = LayerUtilities.GetURLFromIMSLayer(layer);
                data.DataSourceName = LayerUtilities.GetNameFromIMSLayer(layer);
                data.DataSetName = data.DataSourceName;
                data.DataSetType = LayerUtilities.GetTypeFromIMSLayer(layer);
                data.WorkspacePath = null;
                data.WorkspaceProgId = null;
                data.WorkspaceType = null;
                data.Container = null;
                data.ContainerType = null;
            }
            else if (LayerUtilities.HasWMSConnectionName(layer))
            {
                data.DataSource = LayerUtilities.GetURLFromWMSLayer(layer);
                data.DataSourceName = data.DataSource;
                data.DataSetName = data.DataSource;
                data.DataSetType = LayerUtilities.GetAllPropertiesFromWMSLayer(layer);
                data.WorkspacePath = null;
                data.WorkspaceProgId = null;
                data.WorkspaceType = null;
                data.Container = null;
                data.ContainerType = null;
            }
            else if (LayerUtilities.HasDataSourceName(layer))
            {
                data.DataSource = LayerUtilities.GetDataSourceName(layer);
                data.DataSourceName = data.DataSource;
                data.DataSetName = data.DataSource;
                data.DataSetType = data.DataSource;
                data.WorkspacePath = null;
                data.WorkspaceProgId = null;
                data.WorkspaceType = null;
                data.Container = null;
                data.ContainerType = null;
            }
            else
            {
                data.DataSource = "!Error, Unable to determine data source type";
                data.WorkspacePath = null;
                data.WorkspaceProgId = null;
                data.WorkspaceType = null;
                data.Container = null;
                data.ContainerType = null;
                data.DataSourceName = null;
                data.DataSetName = null;
                data.DataSetType = null;
            }
            if (string.IsNullOrEmpty(data.DataSource))
                data.DataSource = "!Error - Data source not found";
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
