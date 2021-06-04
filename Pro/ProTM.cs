using ArcGIS.Core.CIM;
using ArcGIS.Core.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ThemeManager
{
    class Program
    {

        //[STAThread] must be present on the Application entry point
        [STAThread]
        static void Main(string[] args)
        {
            //Call Host.Initialize before constructing any objects from ArcGIS.Core
            Host.Initialize();

            //InspectLayerFile(@"C:\tmp\plants.lyrx");
            //InspectLayerFolder(@"C:\tmp\ThemeMgrPro");
            TestFGDB();
        }

        static void TestFGDB()
        {
            var gdb = new Fgdb(@"C:\tmp\pro.gdb");
            //gdb.PrintDataSetTypes();
            //gdb.PrintAllRootDataSets();
            gdb.PrintAllDataSets();
            //gdb.PrintDataSetsByType();
            //gdb.PrintDataSetsByType(@"\test");
            //string xml = gdb.GetMetadata(@"\Roads_ln", "Feature Class");
            //Console.WriteLine(xml);
            gdb.Close();
        }
        static void InspectLayerFolder(string folderPath)
        {
            try
            {
                var layerFiles = Directory.EnumerateFiles(folderPath, "*.lyrx", SearchOption.AllDirectories);
                foreach (var layerFile in layerFiles)
                {
                    InspectLayerFile(layerFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Enumeration of files may be incomplete", ex.Message);
            }
        }

        static void InspectLayerFile(string layerPath)
        {
            Console.WriteLine(layerPath);
            var text = System.IO.File.ReadAllText(layerPath, Encoding.UTF8);
            var doc = CIMLayerDocument.FromJson(text);
            foreach (string layer in doc.Layers)
            {
                PrintLayer("  ", doc, layer);
            }
        }
        /*
         Layer types found in the Layer Document.
         Theme Manager only cares about layers with a data source (Data or Service Connection in Pro speak)
         and the layer heirarchy. The type of the data source augmented by the layer type if needed is used
         to set the iconography in the browse panel (for example we use the annotation icon for a feature class
         in a annotation layer. Besides iconography, the data connection is used to find the metadata for the data source.
         TM does not care about the heirarcy below a datasource (like ArcGIS.Core.CIM.CIMSubLayerBase and sub types),
         
         CIMGroupLayer, CIMParcelFabricLayer, CIMBuildingDisciplineLayer, and CIMBuildingDisciplineSceneLayer
         are the only layer type that have sub layers with data connections. (CIMNALayer CIMTopologyLayer, and
         CIMMosaicLayer, CIMTraceNetworkLayer, CIMUtilityNetworkLayer do as well, but they also have a data connection for the whole that is preferable)
         Here are all the published CIM Layer types (as of Pro 2.8 circa 2021-06-01)
         Annotated with the property to obtain a sub class of ArcGIS.Core.CIM.CIMDataConnection

         ArcGIS.Core.CIM.CIMBaseLayer
            ArcGIS.Core.CIM.CIMBasicFeatureLayer - CIMDataConnection FeatureTable.DataConnection
               ArcGIS.Core.CIM.CIMAnnotationLayer - See CIMBasicFeatureLayer
               ArcGIS.Core.CIM.CIMDimensionLayer - See CIMBasicFeatureLayer
               ArcGIS.Core.CIM.CIMGeoFeatureLayerBase - See CIMBasicFeatureLayer
                  ArcGIS.Core.CIM.CIMFeatureLayer - See CIMBasicFeatureLayer
                     ArcGIS.Core.CIM.CIMFeatureMosaicSubLayer - See CIMBasicFeatureLayer
                     ArcGIS.Core.CIM.CIMNitfFeatureSubLayer - See CIMBasicFeatureLayer
               ArcGIS.Core.CIM.CIMSubtypeGroupLayer - See CIMBasicFeatureLayer
               ArcGIS.Core.CIM.CIMSubtypeGroupLayerBase - See CIMBasicFeatureLayer
            ArcGIS.Core.CIM.CIMBuildingDisciplineLayer - No data connection; usestring[] CategoryLayers
            ArcGIS.Core.CIM.CIMBuildingDisciplineSceneLayer - No data connection; use string[] SubLayers
            ArcGIS.Core.CIM.CIMBuildingLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMBuildingSceneLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMGALayer - CIMDataConnection Method.DataConnection (dynamic Geostatistical Analyst layer)
            ArcGIS.Core.CIM.CIMGeodatabaseErrorLayer - CIMWorkspaceConnection WorkspaceConnection and strings for point, line, poly and object layers (unclear if they are CIM paths or geodatabase paths)
            ArcGIS.Core.CIM.CIMGraphicsLayer - No data connection
            ArcGIS.Core.CIM.CIMGroupLayer - No data connection; use string[] Layers to get the URI of sub layers
            ArcGIS.Core.CIM.CIMKMLLayer - CIMKMLDataConnection DataConnection
            ArcGIS.Core.CIM.CIMKnowledgeGraphLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMLASDatasetLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMMosaicLayer - CIMDataConnection MosaicDatasetConnection; Note this is a group layer can also get the DataConnection of ImageLayer (the URI of a CIMRasterLayer)
            ArcGIS.Core.CIM.CIMNALayer - CIMDataConnection NetworkDataset and/or CIMDataConnection NAWorkspace, or render as group with URIs to sub layers in Layers
            ArcGIS.Core.CIM.CIMNetworkDatasetLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMNitfLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMParcelFabricLayer - a group layer use string[] AllLayers
            ArcGIS.Core.CIM.CIMParcelLayer - CIMDataConnection ParcelConnection
            ArcGIS.Core.CIM.CIMPointCloudLayer - CIMSceneDataConnection DataConnection
            ArcGIS.Core.CIM.CIMRasterLayer - CIMDataConnection DataConnection
               ArcGIS.Core.CIM.CIMImageServiceLayer - see CIMRasterLayer
               ArcGIS.Core.CIM.CIMNitfImageSubLayer - see CIMRasterLayer
            ArcGIS.Core.CIM.CIMSceneServiceLayer - CIMSceneDataConnection DataConnection
            ArcGIS.Core.CIM.CIMServiceLayer - CIMServiceConnection ServiceConnection; ignore the service sub layers
               ArcGIS.Core.CIM.CIMDynamicServiceLayer - See CIMServiceLayer
               ArcGIS.Core.CIM.CIMGlobeServiceLayer - See CIMServiceLayer
               ArcGIS.Core.CIM.CIMTiledServiceLayer - See CIMServiceLayer
            ArcGIS.Core.CIM.CIMTerrainLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMTinLayer - CIMDataConnection DataConnection
            ArcGIS.Core.CIM.CIMTopologyLayer - CIMDataConnection TopologyConnection; also a group layer string[] AllLayers
            ArcGIS.Core.CIM.CIMTraceNetworkLayer - CIMDataConnection DataConnection; Also a group layer with DirtyAreaLayer, LineErrorLayer, PointErrorLayer, and SystemJunctionsLayer
            ArcGIS.Core.CIM.CIMUtilityNetworkLayer - CIMDataConnection DataConnection; Also a group layer with DirtyAreaLayer, LineErrorLayer, PointErrorLayer, and PolygonErrorLayer 
            ArcGIS.Core.CIM.CIMVectorTileLayer - CIMVectorTileDataConnection DataConnection
            ArcGIS.Core.CIM.CIMVoxelLayer - CIMDataConnection DataConnection
         */
        static void PrintLayer(string indent, CIMLayerDocument doc, string layer)
        {
            if (indent == null) indent = "";
            var layerDef = doc.LayerDefinitions.FirstOrDefault(l => l.URI == layer);
            if (layerDef == null)
            {
                Console.WriteLine("No layer definition found for {0}", layer);
            }
            else
            {
                if (layerDef is CIMFeatureLayer) PrintFeatureLayer(indent, layerDef as CIMFeatureLayer);
                // TODO a CIMImageServiceLayer is also a CIMRasterLayer
                else if (layerDef is CIMRasterLayer) PrintRasterLayer(indent, layerDef as CIMRasterLayer);
                else if (layerDef is CIMLASDatasetLayer) PrintLASDatasetLayer(indent, layerDef as CIMLASDatasetLayer);
                else if (layerDef is CIMTiledServiceLayer) PrintTiledServiceLayer(indent, layerDef as CIMTiledServiceLayer);
                else if (layerDef is CIMTerrainLayer) PrintTerrainLayer(indent, layerDef as CIMTerrainLayer);
                else if (layerDef is CIMTinLayer) PrintTinLayer(indent, layerDef as CIMTinLayer);
                else if (layerDef is CIMAnnotationLayer) PrintAnnoLayer(indent, layerDef as CIMAnnotationLayer);
                else if (layerDef is CIMDynamicServiceLayer) PrintDynamicServiceLayer(indent, layerDef as CIMDynamicServiceLayer);
                else if (layerDef is CIMMosaicLayer mosaicLayer)
                {
                    Console.WriteLine(indent + "Mosaic Group layer: {0}", mosaicLayer.Name);
                    var subLayers = new List<string>() {
                        mosaicLayer.BoundaryLayer,
                        mosaicLayer.SeamlineLayer,
                        mosaicLayer.FootprintLayer,
                        mosaicLayer.ImageLayer
                    };
                    foreach (var sublayer in subLayers)
                    {
                        if (sublayer != null) PrintLayer(indent + "  ", doc, sublayer);
                    }
                }
                else if (layerDef is CIMGroupLayer groupLayer)
                {
                    Console.WriteLine(indent + "Group layer: {0}", groupLayer.Name);
                    foreach (var sublayer in groupLayer.Layers)
                    {
                        PrintLayer(indent + "  ", doc, sublayer);
                    }
                }
                else
                {
                    Console.WriteLine("** Layer: {0}; Type: {1} not supported.", layerDef.Name, layerDef.GetType().ToString());
                }
            }
        }

        static void PrintFeatureLayer(string indent, CIMFeatureLayer layer)
        {
            Console.WriteLine(indent + "Feature Layer: {0}", layer.Name);
            PrintDataConnection(indent, layer.FeatureTable.DataConnection);
        }
        static void PrintRasterLayer(string indent, CIMRasterLayer layer)
        {
            Console.WriteLine(indent + "Raster Layer: {0}", layer.Name);
            PrintDataConnection(indent, layer.DataConnection);
        }

        static void PrintLASDatasetLayer(string indent, CIMLASDatasetLayer layer)
        {
            Console.WriteLine(indent + "LAS Dataset Layer: {0}", layer.Name);
            PrintDataConnection(indent, layer.DataConnection);
        }

        static void PrintTiledServiceLayer(string indent, CIMTiledServiceLayer layer)
        {
            Console.WriteLine(indent + "Tiled Service Layer: {0}", layer.Name);
            PrintDataConnection(indent, layer.ServiceConnection);
        }

        static void PrintTerrainLayer(string indent, CIMTerrainLayer layer)
        {
            Console.WriteLine(indent + "Terrain Layer: {0}", layer.Name);
            PrintDataConnection(indent, layer.DataConnection);
        }
        static void PrintTinLayer(string indent, CIMTinLayer layer)
        {
            Console.WriteLine(indent + "Tin Layer: {0}", layer.Name);
            PrintDataConnection(indent, layer.DataConnection);
        }

        static void PrintAnnoLayer(string indent, CIMAnnotationLayer layer)
        {
            Console.WriteLine(indent + "Annotation Layer: {0}", layer.Name);
            PrintDataConnection(indent, layer.FeatureTable.DataConnection);
        }

        static void PrintDynamicServiceLayer(string indent, CIMDynamicServiceLayer layer)
        {
            Console.WriteLine(indent + "Dynamic Service Layer: {0}", layer.Name);
            PrintDataConnection(indent, layer.ServiceConnection);
        }


        static void PrintDataConnection(string indent, CIMDataConnection connection)
        {
            if (connection == null)
            {
                Console.WriteLine("** No Data Connection");
            }
            else if (connection is CIMStandardDataConnection)
            {
                PrintStandardDataConnection(indent, connection as CIMStandardDataConnection);
            }
            else if (connection is CIMFeatureDatasetDataConnection)
            {
                PrintFeatureDataConnection(indent, connection as CIMFeatureDatasetDataConnection);
            }
            else if (connection is CIMXYEventDataConnection)
            {
                PrintXYEventDataConnection(indent, connection as CIMXYEventDataConnection);
            }
            else if (connection is CIMRasterBandDataConnection)
            {
                PrintRasterBandDataConnection(indent, connection as CIMRasterBandDataConnection);
            }
            else if (connection is CIMRelQueryTableDataConnection)
            {
                PrintRelQueryTableDataConnection(indent, connection as CIMRelQueryTableDataConnection);
            }
            else if (connection is CIMAGSServiceConnection)
            {
                PrintAGSServiceConnection(indent, connection as CIMAGSServiceConnection);
            }
            else if (connection is CIMWMSServiceConnection)
            {
                PrintWMSServiceConnection(indent, connection as CIMWMSServiceConnection);
            }
            else if (connection is CIMWMTSServiceConnection)
            {
                PrintWMTSServiceConnection(indent, connection as CIMWMTSServiceConnection);
            }
            else
            {
                Console.WriteLine("** Data Connection: {0} not supported", connection.GetType());
            }
        }

        /*
         Sub Classes of CIMDataConnection (an abstract base class with no useful members)
         Those marked with a * have these standard properties:
            string CustomWorkspaceFactoryCLSID,
            WorkspaceFactory WorkspaceFactory, string WorkspaceConnectionString,
            esriDatasetType DatasetType and string Dataset
         ArcGIS.Core.CIM.CIMFeatureDatasetDataConnection - * + string FeatureDataset
         ArcGIS.Core.CIM.CIMGADataConnection - CIMDataConnection[] DataConnections
         ArcGIS.Core.CIM.CIMInMemoryDatasetDataConnection - N/A
         ArcGIS.Core.CIM.CIMInMemoryWorkspaceDataConnection - N/A
         ArcGIS.Core.CIM.CIMKMLDataConnection - string KMLURI
         ArcGIS.Core.CIM.CIMKnowledgeGraphDataConnection - * + string DefinitionQuery
         ArcGIS.Core.CIM.CIMKnowledgeGraphTableDataConnection - * + string DefinitionQuery, string ExclusionSetURI, string InclusionSetURI
         ArcGIS.Core.CIM.CIMNetCDFRasterDataConnection - * + lots of other special properties
         ArcGIS.Core.CIM.CIMNetCDFStandardDataConnection - * + lots of other special properties
         ArcGIS.Core.CIM.CIMNITFDataConnection - string URI
         ArcGIS.Core.CIM.CIMRasterBandDataConnection - * + string RasterBandName
         ArcGIS.Core.CIM.CIMRelQueryTableDataConnection - CIMDataConnection SourceTable, DestinationTable, string PrimaryKey, ForeignKey and others
         ArcGIS.Core.CIM.CIMRouteEventDataConnection - CIMDataConnection EventTable, CIMDataConnection RouteFeatureClass
         ArcGIS.Core.CIM.CIMSceneDataConnection - string URI
         ArcGIS.Core.CIM.CIMServiceConnection (abstract base class) - string Description
            ArcGIS.Core.CIM.CIMAGSServiceConnection - string URL + CIMServerConnection ServerConnection
            ArcGIS.Core.CIM.CIMOGCAPIServiceConnection  - CIMInternetServerConnectionBase ServerConnection + string ServiceName
            ArcGIS.Core.CIM.CIMStandardServiceConnection - string URL
            ArcGIS.Core.CIM.CIMWCSServiceConnection - CIMInternetServerConnectionBase ServerConnection + string CoverageName
            ArcGIS.Core.CIM.CIMWFSServiceConnection - CIMInternetServerConnectionBase ServerConnection + string LayerName
            ArcGIS.Core.CIM.CIMWMSServiceConnection - CIMInternetServerConnectionBase ServerConnection + string LayerName
            ArcGIS.Core.CIM.CIMWMTSServiceConnection - CIMInternetServerConnectionBase ServerConnection + string LayerName
         ArcGIS.Core.CIM.CIMSqlQueryDataConnection - * + string SqlQuery,  string OIDFields, and others
         ArcGIS.Core.CIM.CIMStandardDataConnection - *
         ArcGIS.Core.CIM.CIMStreamServiceDataConnection - * + age and expiration members
         ArcGIS.Core.CIM.CIMTableQueryNameDataConnection - * + string WhereClause and others
         ArcGIS.Core.CIM.CIMTemporalDataConnection - * + temporal members
         ArcGIS.Core.CIM.CIMTrackingServerDataConnection - * + purge members
         ArcGIS.Core.CIM.CIMVectorTileDataConnection - string URI, string ResourcesURI
         ArcGIS.Core.CIM.CIMVideoDataConnection - string URI
         ArcGIS.Core.CIM.CIMVoxelDataConnection - string URI
         ArcGIS.Core.CIM.CIMWorkspaceConnection - * less DatasetType and Dataset
         ArcGIS.Core.CIM.CIMXYEventDataConnection - CIMDataConnection XYEventTableDataConnection

         ArcGIS.Core.CIM.CIMInternetServerConnectionBase - string URL, bool Anonymous, string User, string Password
            ArcGIS.Core.CIM.CIMServerConnection - no additional properties

         */
        static void PrintStandardDataConnection(string indent, CIMStandardDataConnection connection)
        {
            Console.WriteLine(indent + "  CIMStandardDataConnection");
            Console.WriteLine(indent + "    Workspace Type: {0}, Name: {1}", connection.WorkspaceFactory, connection.WorkspaceConnectionString);
            Console.WriteLine(indent + "    Dataset Type: {0}, Name: {1}", connection.DatasetType, connection.Dataset);
        }
        static void PrintXYEventDataConnection(string indent, CIMXYEventDataConnection connection)
        {
            Console.WriteLine(indent + "  CIMXYEventDataConnection");

            Console.WriteLine(indent + "    X: {0}, Y: {1}, Z: {2}", connection.XFieldName, connection.YFieldName, connection.ZFieldName);
            Console.WriteLine(indent + "    XY Event Table Data Connection:");
            PrintDataConnection(indent + "    ", connection.XYEventTableDataConnection);
        }
        static void PrintRasterBandDataConnection(string indent, CIMRasterBandDataConnection connection)
        {
            Console.WriteLine(indent + "  CIMRasterBandDataConnection");
            Console.WriteLine(indent + "    Workspace Type: {0}, Name: {1}", connection.WorkspaceFactory, connection.WorkspaceConnectionString);
            Console.WriteLine(indent + "    Dataset Type: {0}, Name: {1}", connection.DatasetType, connection.Dataset);
            Console.WriteLine(indent + "    Raster Band Name: {0}", connection.RasterBandName);
        }
        static void PrintFeatureDataConnection(string indent, CIMFeatureDatasetDataConnection connection)
        {
            Console.WriteLine(indent + "  CIMFeatureDatasetDataConnection");
            Console.WriteLine(indent + "    Workspace Type: {0}, Name: {1}", connection.WorkspaceFactory, connection.WorkspaceConnectionString);
            Console.WriteLine(indent + "    Feature Dataset Name: {0}", connection.FeatureDataset);
            Console.WriteLine(indent + "    Dataset Type: {0}, Name: {1}", connection.DatasetType, connection.Dataset);
        }
        static void PrintAGSServiceConnection(string indent, CIMAGSServiceConnection connection)
        {
            Console.WriteLine(indent + "  CIMAGSServiceConnection");
            Console.WriteLine(indent + "    URL: {0}", connection.URL);
            Console.WriteLine(indent + "    Capabilities: {0}, Description: {1}", connection.Capabilities, connection.Description);
            if (connection.ServerConnection is CIMInternetServerConnectionBase server)
            {
                Console.WriteLine(indent + "    Server: {0}", server.URL);
            }
            else
            {
                Console.WriteLine(indent + "    Unknown Server Connection: {0}", connection.ServerConnection.GetType());
            }
        }
        static void PrintWMTSServiceConnection(string indent, CIMWMTSServiceConnection connection)
        {
            Console.WriteLine(indent + "  CIMWMTSServiceConnection");
            Console.WriteLine(indent + "    Layer Name: {0}", connection.LayerName);
            Console.WriteLine(indent + "    Capabilities: {0}, Description: {1}", connection.CapabilitiesParameters, connection.Description);
            if (connection.ServerConnection is CIMInternetServerConnectionBase server)
            {
                Console.WriteLine(indent + "    Server: {0}", server.URL);
            }
            else
            {
                Console.WriteLine(indent + "    Unknown Server Connection: {0}", connection.ServerConnection.GetType());
            }
        }
        static void PrintWMSServiceConnection(string indent, CIMWMSServiceConnection connection)
        {
            Console.WriteLine(indent + "  CIMWMSServiceConnection");
            Console.WriteLine(indent + "    Layer Name: {0}", connection.LayerName);
            Console.WriteLine(indent + "    Capabilities: {0}, Description: {1}", connection.CapabilitiesParameters, connection.Description);
            if (connection.ServerConnection is CIMInternetServerConnectionBase server)
            {
                Console.WriteLine(indent + "    Server: {0}", server.URL);
            }
            else
            {
                Console.WriteLine(indent + "    Unknown Server Connection: {0}", connection.ServerConnection.GetType());
            }
        }
        static void PrintRelQueryTableDataConnection(string indent, CIMRelQueryTableDataConnection connection)
        {
            indent += "  ";
            Console.WriteLine(indent + "CIMRelQueryTableDataConnection");
            Console.WriteLine(indent + "  Primary Key: {0}, Foreign Key: {0}", connection.PrimaryKey, connection.ForeignKey);
            Console.WriteLine(indent + "  Source Table:");
            PrintDataConnection(indent + "  ", connection.SourceTable);
            Console.WriteLine(indent + "  Destination Table:");
            PrintDataConnection(indent + "  ", connection.DestinationTable);
        }
    }

    /*
    Data Connection Types
    5443 CIMStandardDataConnection
     835 CIMFeatureDatasetDataConnection
     192 CIMRasterBandDataConnection
         Subtotal Data Connections = 6470 ( Each CIMXYEventDataConnection has one and CIMRelQueryTableDataConnection has two of the above)
     145 CIMXYEventDataConnection
     117 CIMRelQueryTableDataConnection
     125 CIMAGSServiceConnection
      93 CIMWMTSServiceConnection
       5 CIMWMSServiceConnection
    6955 Total (6732 Data + 223 Service)

    Connection Info Workspace Types
    4909 FileGDB        (all but 3 workspace names are in the form DATABASE=X:\...); 3 as DATABASE=\\inpakro....
    1007 Raster         (all but 31 workspace names are in the form DATABASE=X:\...); 31 raster functions: 1 as DATABASE=C:\, 30 as <DataConnections.... 
     152 Shapefile      (all workspace names are in the form DATABASE=X:\...)
     148 Cad            (all workspace names are in the form DATABASE=X:\...)
     144 ArcInfo        (all workspace names are in the form DATABASE=X:\...)
      50 FeatureService (all workspace names are in the form URL=https://...)
      42 Access         (all workspace names are in the form DATABASE=X:\...; 7 of the workspace names are in the form DATABASE=X:\...mdb;PROVIDERCLSID={..})
      12 SDE            (all workspace names are connection property strings: key=value;...)
       5 LASDataset     (all workspace names are in the form DATABASE=X:\...)
       1 Tin            (all workspace names are just a naked X drive path)
    6470 Total

    Checking Workspace Names
    6377 DATABASE= (6473 = X:\...; 1 = C:\..., 3 = \\inpakro...)
      12 connection string (Workspace Type = SDE) 
      50 URL= (Workspace Type = FeatureService) 
      30 <DataConnections array...  (XML string for Raster function)
       1 x drive path without "DATABASE=" (Workspace Type = Tin)
    6470 Total (searching for "Workspace Type: *, Name: *")
     */
}
