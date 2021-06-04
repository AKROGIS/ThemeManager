using System;
using System.Collections.Generic;
using System.Linq;
using Esri.FileGDB;

namespace ThemeManager
{
    /// <summary>
    /// A Class for experimenting with the esri File Geodatabase API
    /// https://github.com/Esri/file-geodatabase-api
    /// download and unzip the latest version for windows.
    /// Application must be built for 64 bit, 32 bit, or AnyCPU but it MUST be deployed with
    /// the correct set of binary files for the OS it is executing on.
    /// Your application must target .Net 4.5.1 or higher.
    /// Add `using Esri.FileGDB;` to the top of your source file.
    /// for example, copy the files in "C:\Users\RESarwas\Downloads\FileGDB_API_1_5_1-VS2017\bin64
    /// to some convenient spot. This sample project has added them to the project folder (but not
    /// commited to the source code repository).
    /// Add Esri.FileGDBAPI.dll to the list of references and set it to "Copy Local". 
    /// The files FileGDBAPI.dll and FileGDBAPID.dll must also
    /// be copied to the executable folder.  They are not required for compiling, but must be
    /// in the load path when the app is executed.
    /// See the readme file and samples delivered with the download for more information.
    /// </summary>
    class Fgdb
    {
        Geodatabase _geodatabase = null;

        /// <summary>
        /// Load a geodatabase object.  In a real application, the exceptions should be
        /// sent to the caller to deal with.
        /// Example:
        /// var gdb = new Fgdb(@"C:\tmp\akr_facility.gdb");
        /// gdb.PrintDataSetTypes();
        /// gdb.Close();
        /// </summary>
        /// <param name="path"></param>
        public Fgdb(string path)
        {
            Path = path;
            try
            {
                _geodatabase = Geodatabase.Open(path);
                Console.WriteLine("The geodatabase {0} has been opened.", path);
            }
            catch (FileGDBException ex)
            {
                Console.WriteLine("{0} - {1}", ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General exception. " + ex.Message);
            }
        }

        public string Path { get; }

        public void Close()
        {
            if (_geodatabase == null) { return; }
            _geodatabase.Close();
        }

        public void PrintDataSetTypes()
        {
            if (_geodatabase == null) { return; }
            Console.WriteLine("Data Set Types in {0}:", Path);
            foreach (var datatype in _geodatabase.DataSetTypes.OrderBy(i => i))
            {
                Console.WriteLine("  {0}", datatype);
            }
        }

        /*
         List of Dataset types.  This is the list from a new empty geodatabase created with 10.8
         The list remained the same after adding every possible new datset type from ArcCatalog right click new...
            AbstractTable
            Catalog Dataset
            Coded Value Domain
            Dataset
            Domain
            Extension Dataset
            Feature Class
            Feature Dataset
            Folder
            Geometric Network
            Historical Marker
            Item
            Mosaic Dataset
            Network Dataset
            Parcel Fabric
            Range Domain
            Raster Catalog
            Raster Dataset
            Relationship Class
            Replica
            Replica Dataset
            Representation Class
            Resource
            Schematic Dataset
            Survey Dataset
            Sync Dataset
            Sync Replica
            Table
            Terrain
            Tin
            Toolbox
            Topology
            Workspace
            Workspace Extension

        The akr_facility.gdb (created with 10.6) also included the following four
        additional dataset types, even though it has none of these data types:
            Location Referencing Dataset
            Parcel Dataset
            Trace Network
            Utility Network

        I was able to create the following data types from ArcCatalog right click new...
            Feature Class
            Feature Dataset
            Mosaic Dataset
            Raster Catalog
            Raster Dataset
            Relationship Class
            Table
            Geometric Network
            Parcel Fabric
            Toolbox
            Topology
        */
        public void PrintAllRootDataSets()
        {
            PrintAllChildDataSets(@"\");
        }

        public void PrintAllChildDataSets(string parent)
        {
            PrintChildDataSetsWithType(parent, "");
        }
        public void PrintChildDataSetsWithType(string parent, string type)
        {
            if (_geodatabase == null) { return; }
            Console.WriteLine("Data Sets in {0}{1} with Type = \"{2}\":", Path, parent, type);
            foreach (var dataset in _geodatabase.GetChildDatasets(parent, type))
            {
                Console.WriteLine("  {0}", dataset);
            }
        }

        public void PrintAllDataSets()
        {
            Console.WriteLine("All Data Sets in {0}:", Path);
            foreach (var dataset in GetAllDataSets().OrderBy(i => i))
            {
                Console.WriteLine("  {0}", dataset);
            }
        }

        public IEnumerable<string> GetAllDataSets()
        {
            if (_geodatabase == null) { return new List<string>(); }
            var items = new List<string>(_geodatabase.GetChildDatasets(@"\", ""));
            var folders = _geodatabase.GetChildDatasets(@"\", "Feature Dataset");
            foreach (var folder in folders)
            {
                items.AddRange(_geodatabase.GetChildDatasets(folder, ""));
            }
            return items;
        }

        /// <summary>
        /// Very slow
        /// </summary>
        public void PrintDataSetsByType(string parent = @"\")
        {
            if (_geodatabase == null) { return; }
            Console.WriteLine("Data Sets in {0}{1} by type:", Path, parent);
            foreach (var datatype in _geodatabase.DataSetTypes.OrderBy(i => i))
            {
                Console.WriteLine(datatype);
                try
                {
                    foreach (var dataset in _geodatabase.GetChildDatasets(parent, datatype))
                    {
                        Console.WriteLine(@"  {0}", dataset);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("  Datatype {0} not in GDB", datatype);
                }
            }
        }

        /// <summary>
        /// Gets the metadata as an XML string for the data set and datatype.  The correct datatype
        /// must be provided or it will return nothing.
        /// 
        /// Example:
        /// var gdb = new Fgdb(@"C:\tmp\akr_facility.gdb");
        /// string xml = gdb.GetMetadata(@"\Roads_ln", "Feature Class");
        /// </summary>
        /// <param name="dataset">Must be a full path name starting with "\"; case insensitive</param>
        /// <param name="datatype">Must be a well known value.  Case Sensitive.  Examples:
        /// "Raster Dataset", "Mosaic Dataset", "Feature Class", "Table", "Relationship Class", "Table"</param>
        /// <returns></returns>
        public string GetMetadata(string dataset, string datatype)
        {
            if (_geodatabase == null) { return null; }
            return _geodatabase.GetDatasetDocumentation(@"\Roads_ln", datatype);
        }

    }
}
