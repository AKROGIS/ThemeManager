using Esri.FileGDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    /// <summary>
    /// A class for customizing the esri File Geodatabase API
    /// https://github.com/Esri/file-geodatabase-api
    /// download and unzip the latest version for windows.
    /// Application can be built for 64 bit, 32 bit, or AnyCPU but it MUST be deployed with
    /// the correct set of binary files for the bit depth the application is running as.
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
        /// A geodatabase object at path.
        /// This does not open or validate the path.
        /// </summary>
        /// <param name="path"></param>
        public Fgdb(string path)
        {
            Path = path;
        }
        public void Open()
        {
            _geodatabase = Geodatabase.Open(Path);
        }

        public async Task OpenAsync()
        {
            await Task.Run(() => Open());
        }

        public string Path { get; }

        public void Close()
        {
            if (_geodatabase == null) { return; }
            _geodatabase.Close();
        }

        /// <summary>
        /// Gets the metadata as an XML string for the dataset.
        /// </summary>
        /// <param name="dataset">Must be a full path name starting with "\"; case insensitive</param>
        /// <returns>null if the dataset is not in the GDB.</returns>
        public async Task<string> GetMetadataAsync(string dataset)
        {
            if (_geodatabase == null) { return null; }
            return await Task.Run(() =>
            {
                // Note: _geodatabase.GetDatasetDocumentation(dataset, datatype) throws if there
                // is no dataset with that datatype.  Since excpetions are expensive in C#, do not
                // use this method to test multiple datatypes in try/catch logic
                var datatype = GetDataType(dataset);
                if (datatype == null) return null;
                return _geodatabase.GetDatasetDocumentation(dataset, datatype);
            });
        }

        /// <summary>
        /// Finds all the Feature datasets in the FGDB
        /// </summary>
        /// <remarks>
        /// A feature dataset has the type "Feature Dataset" and acts as a container for other
        /// datasets. All feature datasets are in the root of the FGDB (can only be one level deep).
        /// If an item is in a feature dataset the dataset name must be included in the path to
        /// the item.
        /// </remarks>
        /// <returns>An enumerable collection of strings, may be empty.</returns>
        public IEnumerable<string> AllFeatureDataSets => _geodatabase.GetChildDatasets(@"\", "Feature Dataset");

        /// <summary>
        /// Lists all the items in the FGDB, including Feature Datasets
        /// </summary>
        /// <remarks>
        /// An item may be in the root of the FGDB or in a Feature Dataset.
        /// </remarks>
        /// <returns>An enumerable collection of strings, may be empty.</returns>
        public IEnumerable<string> AllDataSets
        {
            get
            {
                if (_geodatabase == null) { return new List<string>(); }
                var items = new List<string>(_geodatabase.GetChildDatasets(@"\", ""));
                foreach (var folder in AllFeatureDataSets)
                {
                    items.AddRange(_geodatabase.GetChildDatasets(folder, ""));
                }
                return items;
            }
        }

        public bool Exists(string itemPath)
        {
            return AllDataSets.Any(i => string.Equals(i, itemPath, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determine the DataType of a geodatabase item
        /// </summary>
        /// <remarks>
        /// This is done by searching the database for all items of each type in turn, until a match is found.
        /// Many of the DataTypes are very uncommon, so we search the likely suspects first.
        /// </remarks>
        /// <param name="itemPath">The full name of the item</param>
        /// <returns>One of the well known data types</returns>
        public string GetDataType(string itemPath)
        {
            // To avoid searching each type for an item does not exist
            if (!Exists(itemPath)) return null;
            // Searching is done by folder (feature dataset); bu the item names returned are the full path.
            // All paths start with a single backslash and will have an additional backslash if in a feature dataset.
            // the folder name includes the first backslash, but not the second.
            var index = itemPath.LastIndexOf('\\');
            if (index < 0) return null;
            var folder = index == 0 ? @"\" : itemPath.Remove(index, itemPath.Length - index);
            foreach (var datatype in CommonDataSetTypes)
            {
                foreach (var item in _geodatabase.GetChildDatasets(folder, datatype))
                {
                    if (string.Equals(item, itemPath, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return datatype;
                    }
                }
            }
            foreach (var datatype in OtherDataSetTypes)
            {
                foreach (var item in _geodatabase.GetChildDatasets(folder, datatype))
                {
                    if (string.Equals(item, itemPath, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return datatype;
                    }
                }
            }
            return null;
        }

        public IEnumerable<string> DataSetTypes => _geodatabase.DataSetTypes;

        // Data set types
        // ==============
        // The data set type must be provided when asking for metadata.
        // Unfortunately, this is not obvious if all you have is the name of the gdb item.
        // The DataSetTypes property returns the possible type in the FGDB.
        //    _geodatabase.DataSetTypes => string[]
        // The following is the list from a new empty geodatabase created with 10.8
        // The list remained the same after adding every possible new datset type from
        // ArcCatalog (using right click new..)
        /*
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
        */
        // The akr_facility.gdb (likely created with 10.6) also included the following four
        // additional dataset types, even though it has none of these data types:
        //   Location Referencing Dataset
        //   Parcel Dataset
        //   Trace Network
        //   Utility Network

        // I was able to create the following data types from ArcCatalog (using right click new...)
        /*
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

        /// <summary>
        /// A priority search list of the most common DataSetTypes
        /// </summary>
        /// <remarks>
        /// Hopefully this will allow the search for an items type to complete sooner, as we will not check
        /// the unlikely types unless we have to.
        /// Since this is used to find an items metadata, It is the most common data types 
        /// in the Alaska NPS data that may have metadata
        /// </remarks>
        public IEnumerable<string> CommonDataSetTypes => new string[] {
            "Feature Class",
            "Mosaic Dataset",
            "Raster Dataset",
            "Table",
            "Raster Catalog",
            "Tin",
            "Terrain",
            "Topology",
            "Geometric Network",
            "Parcel Fabric",
            "Feature Dataset",
            "Relationship Class",
            "Toolbox",
            };

        public IEnumerable<string> OtherDataSetTypes
        {
            get
            {
                var others = new HashSet<string>(DataSetTypes);
                others.ExceptWith(new HashSet<string>(CommonDataSetTypes));
                return others;
            }
        }
    }
}
