using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NPS.AKRO.ThemeManager.ArcGIS;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Model;

namespace tm
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Getting ESRI License");
            await GisInterface.InitializeAsync();
            if (!GisInterface.IsInitialized)
            {
                Console.WriteLine($"Could not initialize an ArcGIS license. {GisInterface.Status}");
                return;
            }
            var path = args[0];
            Console.WriteLine($"Loading {path}");
            var themeList = Load(path);
            if (themeList == null)
            {
                Console.WriteLine("Could not load the Theme List.");
                return;
            }
            themeList.SuspendUpdates();
            themeList.Build();
            await ReloadAsync(themeList);
            Console.WriteLine("Saving Updated Theme List");
            themeList.SaveAs(path.Replace(".tml", "1.tml"));
            await SyncAsync(themeList);
            Console.WriteLine("Saving Updated Theme List");
            themeList.SaveAs(path.Replace(".tml", "2.tml"));
            Console.WriteLine("Done.");
        }

        static TmNode Load(String path)
        {
            return new TmNode(TmNodeType.ThemeList,
                Path.GetFileNameWithoutExtension(path),
                null, new ThemeData(path), null, null, null);
        }

        static async Task ReloadAsync(TmNode root)
        {
            List<TmNode> nodes = root.Recurse(x => x.Children)
                                        .Where(n => n.IsTheme)
                                        .ToList();
            Console.WriteLine($"Reloading {nodes.Count} Themes");
            //TODO: reload nodes in parallel
            foreach (var node in nodes)
            {
                try
                {
                    await node.ReloadThemeAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError reloading node {node.CategoryPath()}/{node.Name}: {ex.Message}");
                }
                if (!node.HasData)
                {
                    Console.WriteLine($"\nNode {node.CategoryPath()}/{node.Name} has no data");
                }
                if (!node.HasMetadata)
                {
                    Console.WriteLine($"\nNode {node.CategoryPath()}/{node.Name} ({node.Data?.Type}) has no metadata");
                }
                Console.Write(".");
            }
            Console.WriteLine("");
        }
        static async Task SyncAsync(TmNode root)
        {
            List<TmNode> nodes = root.Recurse(x => x.Children)
                            .Where(n => !string.IsNullOrEmpty(n.Metadata.Path))
                            .ToList();

            Console.WriteLine($"Syncing metadata for {nodes.Count} Themes");
            foreach (var node in nodes)
            {
                try
                {
                    await node.SyncWithMetadataAsync(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError syncing metadata for node {node.CategoryPath()}/{node.Name}: {ex.Message}");
                }
                Console.Write(".");
            }
            Console.WriteLine("");
        }
    }
}
