using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NPS.AKRO.ThemeManager.ArcGIS;

namespace NPS.AKRO.ThemeManager.Model
{
    static class ThemeBuilder
    {
        internal static async Task BuildThemesForLayerFileAsync(TmNode node)
        {
            await BuildThemesForNodeAsync(node);
        }

        internal static async Task BuildSubThemesForMapDocumentAsync(TmNode node)
        {
            await BuildThemesForNodeAsync(node);
        }

        private static async Task BuildThemesForNodeAsync(TmNode tmNode)
        {
            IGisLayer layer;
            try
            {
                layer = await GisInterface.ParseItemAtPathAsGisLayerAsync(tmNode.Data.Path);
            }
            catch (Exception ex)
            {
                Debug.Print("Unable to load GIS data layers at " + tmNode.Data.Path + " " + ex.Message);
                tmNode.Data.Type = "Unable to load  GIS data layers (" + ex.Message + ")";
                return;
            }
            tmNode.Data.Type = layer.DataType;
            if (layer.IsGroup)
            {
                await BuildSubThemesForGroupLayerAsync(tmNode, layer);
            }
            else
                BuildThemeDataForLayer(tmNode.Data, layer);
            layer.Close();
        }

        private static async Task BuildSubThemesForGroupLayerAsync(TmNode node, IGisLayer layer)
        {
            foreach (var sublayer in layer.SubLayers)
            {
                //TODO: Build in parallel
                await BuildSubThemeForLayerAsync(node, sublayer);
            }
        }

        private static async Task BuildSubThemeForLayerAsync(TmNode node, IGisLayer subLayer)
        {
            ThemeData data = new ThemeData(null, subLayer.DataType);
            if (subLayer.IsGroup)
            {
                TmNode newNode = new TmNode(TmNodeType.Theme, subLayer.Name, node, data, null, null, null);
                node.Add(newNode);
                await BuildSubThemesForGroupLayerAsync(newNode, subLayer);
            }
            else
            {
                BuildThemeDataForLayer(data, subLayer);
                Metadata md = await Metadata.FromDataSourceAsync(data);
                TmNode newNode = new TmNode(TmNodeType.Theme, subLayer.Name, node, data, md, null, null);
                node.Add(newNode);
            }
        }

        private static void BuildThemeDataForLayer(ThemeData data, IGisLayer layer)
        {
            data.DataSource = layer.DataSource;
            data.WorkspacePath = layer.WorkspacePath;
            data.WorkspaceProgId = layer.WorkspaceProgId;
            data.WorkspaceType = layer.WorkspaceType;
            data.Container = layer.Container;
            data.ContainerType = layer.ContainerType;
            data.DataSourceName = layer.DataSourceName;
            data.DataSetName = layer.DataSetName;
            data.DataSetType = layer.DataSetType;
        }

    }
}
