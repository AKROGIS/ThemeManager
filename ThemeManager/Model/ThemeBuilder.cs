using System;
using System.Diagnostics;
using NPS.AKRO.ThemeManager.ArcGIS;

namespace NPS.AKRO.ThemeManager.Model
{
    static class ThemeBuilder
    {
        internal static void BuildThemesForLayerFile(TmNode tmNode)
        {

            TmLayer layer;
            try
            {
                layer = new TmLayer(tmNode.Data.Path);
            }
            catch (Exception ex)
            {
                Debug.Print("Unable to load layer file: " + tmNode.Data.Path + " " + ex.Message);
                tmNode.Data.Type = "Unable to load layer file (" + ex.Message + ")";
                return;
            }
            if (layer.IsGroup)
            {
                BuildSubThemesForGroupLayer(tmNode, layer);
            }
            else
                BuildThemeDataForLayer(tmNode.Data, layer);
            layer.Close();
        }

        private static void BuildSubThemesForGroupLayer(TmNode node, TmLayer layer)
        {
            foreach (var sublayer in layer.SubLayers)
            {
                BuildSubThemeForLayer(node, sublayer);
            }
        }

        internal static void BuildSubThemesForMapDocument(TmNode node)
        {
            var map = new TmMap(node.Data.Path);
            foreach (var mapFrame in map.MapFrames)
                BuildSubThemeForMap(node, mapFrame);
            map.Close();
        }

        private static void BuildSubThemeForMap(TmNode node, TmMapFrame map)
        {
            TmNode newNode = new TmNode(TmNodeType.Theme, map.Name, node, new ThemeData(null, "Map Data Frame"), null, null, null);
            node.Add(newNode);
            foreach (var layer in map.Layers)
            {
                BuildSubThemeForLayer(newNode, layer);
            }
        }

        private static void BuildSubThemeForLayer(TmNode node, TmLayer subLayer)
        {
            if (subLayer.IsGroup)
            {
                TmNode newNode = new TmNode(TmNodeType.Theme, subLayer.Name, node, new ThemeData(null, "Group Layer"), null, null, null);
                node.Add(newNode);
                BuildSubThemesForGroupLayer(newNode, subLayer);
            }
            else
            {
                string dataType = subLayer.DataType;
                ThemeData data = new ThemeData(null, subLayer.DataType);

                BuildThemeDataForLayer(data, subLayer);

                Metadata md = Metadata.FromDataSource(data);
                TmNode newNode = new TmNode(TmNodeType.Theme, subLayer.Name, node, data, md, null, null);
                node.Add(newNode);
            }
        }

        private static void BuildThemeDataForLayer(ThemeData data, TmLayer layer)
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
