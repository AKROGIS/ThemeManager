using NPS.AKRO.ThemeManager.ArcGIS;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ThemeManager
{
    /// <summary>
    /// Creates a CSV list of all the properties Theme Manager extracts from the *.lyrx files in a directory tree.
    /// The name of the output file is set in the debug command line option in the project properties
    /// </summary>
    class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            //await InspectLayerFileAsync(@"C:\tmp\plants.lyrx");
            await InspectLayerFolderAsync(@"C:\tmp\ThemeMgrPro");
        }

        static async Task InspectLayerFolderAsync(string folderPath)
        {
            // See IGisLayer for a description of each of these properties
            Console.WriteLine("Layer File Path, Container, ContainerType, DataSetName, DataSetType, DataSource, " +
                "DataSourceName, DataType, IsGroup, SubLayers.Count(), Name, WorkspacePath, WorkspaceProgId,WorkspaceType");
            try
            {
                var layerFiles = Directory.EnumerateFiles(folderPath, "*.lyrx", SearchOption.AllDirectories);
                foreach (var layerFile in layerFiles)
                {
                    await InspectLayerFileAsync(layerFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Enumeration of files may be incomplete", ex.Message);
            }
        }

        static async Task InspectLayerFileAsync(string layerPath)
        {
            var node = await GisInterface.ParseItemAtPathAsGisLayerAsync(layerPath);
            Print(layerPath, node);
        }

        static void Print(string layerPath, IGisLayer node)
        {
            var line = string.Join(",",
                Quote(layerPath), Quote(node.Container), node.ContainerType, Quote(node.DataSetName),
                node.DataSetType, Quote(node.DataSource), Quote(node.DataSourceName), Quote(node.DataType),
                node.IsGroup, node.SubLayers.Count(), Quote(node.Name), Quote(node.WorkspacePath),
                node.WorkspaceProgId, node.WorkspaceType);
            Console.WriteLine(line);
            foreach (var layer in node.SubLayers)
            {
                Print(layerPath, layer);
            }
        }

        static string Quote(string t)
        {
            return "\"" + t + "\"";
        }
    }
}
