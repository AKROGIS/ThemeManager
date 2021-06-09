using System;
using System.Diagnostics;
using NPS.AKRO.ThemeManager.ArcGIS;

namespace NPS.AKRO.ThemeManager.Model
{
    static class ThemeBuilder
    {
        internal static void BuildThemesForLayerFile(TmNode node)
        {
            BuildThemesForNode(node);
        }

        internal static void BuildSubThemesForMapDocument(TmNode node)
        {
            BuildThemesForNode(node);
        }

        private static void BuildThemesForNode(TmNode tmNode)
        {
            IGisLayer layer;
            try
            {
                layer = GisInterface.ParseItemAtPathAsGisLayer(tmNode.Data.Path);
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
                BuildSubThemesForGroupLayer(tmNode, layer);
            }
            else
                BuildThemeDataForLayer(tmNode.Data, layer);
            layer.Close();
        }

        private static void BuildSubThemesForGroupLayer(TmNode node, IGisLayer layer)
        {
            foreach (var sublayer in layer.SubLayers)
            {
                BuildSubThemeForLayer(node, sublayer);
            }
        }

        private static void BuildSubThemeForLayer(TmNode node, IGisLayer subLayer)
        {
            ThemeData data = new ThemeData(null, subLayer.DataType);
            if (subLayer.IsGroup)
            {
                TmNode newNode = new TmNode(TmNodeType.Theme, subLayer.Name, node, data, null, null, null);
                node.Add(newNode);
                BuildSubThemesForGroupLayer(newNode, subLayer);
            }
            else
            {
                BuildThemeDataForLayer(data, subLayer);
                Metadata md = Metadata.FromDataSource(data);
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
