using System.Threading.Tasks;
using Esri.FileGDB;

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
                // FIXME: get the dataType from the GDB
                var datatype = "Feature Class";
                return _geodatabase.GetDatasetDocumentation(dataset, datatype);
            });

        }

    }
}
