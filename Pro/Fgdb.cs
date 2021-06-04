using System;
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
            foreach (var datatype in _geodatabase.DataSetTypes)
            {
                Console.WriteLine("  {0}", datatype);
            }
        }

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

        /// <summary>
        /// Very slow
        /// </summary>
        public void PrintRootDataSetsByType()
        {
            if (_geodatabase == null) { return; }
            foreach (var datatype in _geodatabase.DataSetTypes)
            {
                Console.WriteLine(datatype);
                try
                {
                    foreach (var dataset in _geodatabase.GetChildDatasets(@"\", datatype))
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
