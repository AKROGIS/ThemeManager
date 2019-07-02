using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPS.AKRO.ThemeManager.ArcGIS;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Model;

namespace tm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting ESRI License");
            EsriLicenseManager.Start(false);
            if (!EsriLicenseManager.Running)
            {
                Console.WriteLine($"Could not initialize an ArcGIS license. {EsriLicenseManager.Message}");
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
            themeList.BeginUpdate();
            themeList.Build();
            Reload(themeList);
            Sync(themeList);
            Console.WriteLine("Saving Updated Theme List");
            themeList.SaveAs(path);
            Console.WriteLine("Done.");
        }

        static ThemeListNode Load(String path)
        {
            return new ThemeListNode(
                Path.GetFileNameWithoutExtension(path), path, null, null);
        }

        static void Reload(TmNode root)
        {
            List<TmNode> nodes = root.Recurse(x => x.Children)
                                        .Where(n => n is ThemeNode)
                                        .ToList();
            Console.WriteLine($"Reloading {nodes.Count} Themes");
            foreach (var node in nodes)
            {
                try
                {
                    node.Reload();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError reloading node {node.Name}: {ex.Message}");
                }
                Console.Write(".");
            }
            Console.WriteLine("");
        }
        static void Sync(TmNode root)
        {
            List<TmNode> nodes = root.Recurse(x => x.Children)
                            .Where(n => !string.IsNullOrEmpty(n.Metadata.Path))
                            .ToList();

            Console.WriteLine($"Syncing metadata for {nodes.Count} Themes");
            foreach (var node in nodes)
            {
                try
                {
                    node.SyncWithMetadata(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError syncing metadata for node {node.Name}: {ex.Message}");
                }
                Console.Write(".");
            }
            Console.WriteLine("");
        }
    }
}
