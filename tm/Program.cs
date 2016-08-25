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
            themeList.SuspendUpdates();
            themeList.Build();
            Reload(themeList);
            themeList.SaveAs(path);
        }

        static TmNode Load(String path)
        {
            return new TmNode(TmNodeType.ThemeList,
                Path.GetFileNameWithoutExtension(path),
                null, new ThemeData(path), null, null, null);
        }

        static void Reload(TmNode root)
        {
            if (root == null)
            {
                Console.WriteLine("No node provided for reload");
                return;
            }

            List<TmNode> nodes = root.Recurse(x => x.Children)
                                        .Where(n => n.IsTheme)
                                        .ToList();

            Console.WriteLine($"Updating {nodes.Count} Themes");
            foreach (var node in nodes)
            {
                try
                {
                    node.ReloadTheme();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError reloading node {node.Name}: {ex.Message}");
                }
                try
                {
                    node.SyncWithMetadata();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError syncing metadata for node {node.Name}: {ex.Message}");
                }
                Console.Write(".");
            }
            Console.WriteLine("\nDone.");
        }

    }
}
