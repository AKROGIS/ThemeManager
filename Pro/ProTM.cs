using ArcGIS.Core.CIM;
using ArcGIS.Core.Hosting;
using System;
using System.Linq;
using System.Text;

namespace ThemeManager
{
    class Program
    {
        static private readonly string layerFile = @"C:\tmp\plants.lyrx";

        //[STAThread] must be present on the Application entry point
        [STAThread]
        static void Main(string[] args)
        {
            //Call Host.Initialize before constructing any objects from ArcGIS.Core
            Host.Initialize();
            //TODO: Add your business logic here.
            //
            var text = System.IO.File.ReadAllText(layerFile, Encoding.UTF8);
            var doc = CIMLayerDocument.FromJson(text);
            foreach (string layer in doc.Layers)
            {
                PrintLayer(doc, layer);
            }
        }

        static void PrintLayer(CIMLayerDocument doc, string layer)
        {
            var layerDef = doc.LayerDefinitions.FirstOrDefault(l => l.URI == layer);

            if (layerDef == null)
            {
                Console.WriteLine("No layer definition found for {0}", layer);
            }
            else
            {
                if (layerDef is CIMFeatureLayer) PrintFeatureLayer(layerDef as CIMFeatureLayer);
                else if (layerDef is CIMGroupLayer groupLayer)
                {
                    Console.WriteLine("Group layer: {0}", groupLayer.Name);
                    foreach (var sublayer in groupLayer.Layers)
                    {
                        PrintLayer(doc, sublayer);
                    }
                }
                else PrintLayerDef(layerDef);
            }
        }

        static void PrintLayerDef(CIMDefinition layerDef)
        {
            Console.WriteLine("Layer: {0}; Type: {1} not supported.", layerDef.Name, layerDef.GetType().ToString());
        }

        static void PrintFeatureLayer(CIMFeatureLayer layer)
        {
            Console.WriteLine("Feature Layer: {0}", layer.Name);
            if (layer.FeatureTable.DataConnection is CIMStandardDataConnection connection1)
            {
                Console.WriteLine("  CIMStandardDataConnection");
                Console.WriteLine("  Workspace Type: {0}, Name: {1}", connection1.WorkspaceFactory, connection1.WorkspaceConnectionString);
                Console.WriteLine("  Dataset Type: {0}, Name: {1}", connection1.DatasetType, connection1.Dataset);
            }
            else if (layer.FeatureTable.DataConnection is CIMFeatureDatasetDataConnection connection2)
            {
                Console.WriteLine("  CIMFeatureDatasetDataConnection");
                Console.WriteLine("  Workspace Type: {0}, Name: {1}", connection2.WorkspaceFactory, connection2.WorkspaceConnectionString);
                Console.WriteLine("  Dataset Type: {0}, Name: {1}", connection2.DatasetType, connection2.Dataset);
            }
            else
            {
                Console.WriteLine("  Data Connection: {0} not supported", layer.FeatureTable.DataConnection.GetType().ToString());
            }
        }
    }
}
