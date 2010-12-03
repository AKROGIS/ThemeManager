using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using NPS.AKRO.ThemeManager.UI.Forms;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    class EsriMetadata
    {
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static GxCatalog _catalog;

        public static string GetContentsAsXml(string datapath)
        {
            if (datapath == null)
                throw new ArgumentNullException("datapath");
            if (datapath == string.Empty)
                throw new ArgumentException("datapath is empty string");

            if (!_cache.ContainsKey(datapath))
            {
                Trace.TraceInformation("{0}:   Begin Load ESRI Metadata for dataset path: {1}", DateTime.Now, datapath);
                Load(datapath);
                Trace.TraceInformation("{0}:   End   Load ESRI Metadata for dataset path: {1}", DateTime.Now, datapath);
            }
            if (_cache.ContainsKey(datapath))
                return _cache[datapath];
            return null;
        }

        //public static string ReloadContentsAsXml(string datapath)
        //{
        //    if (datapath == null)
        //        throw new ArgumentNullException("datapath");
        //    if (datapath == string.Empty)
        //        throw new ArgumentException("datapath is empty string");

        //    if (_cache.ContainsKey(datapath))
        //        _cache.Remove(datapath);

        //    return GetContentsAsXml(datapath);
        //}

        private static void Load(string path)
        {
            if (!EsriLicenseManager.Running)
                EsriLicenseManager.Start(true);
            if (!EsriLicenseManager.Running)
                throw new Exception("Could not initialize an ArcGIS license. \n" + EsriLicenseManager.Message);

            //FIXME - test with catalog.  Can ".gdb" be anywhere in path?, can a FGDB extension be changed?
            string text = null;
            if (path.ToLower().Contains(".gdb\\"))
            {
                text = GetMetaDataFromFGDB(path);
                if (text == null)
                    throw new ApplicationException("Unable to load FGDB metadata (" + path + ")");
                _cache[path] = text;
                return;
            }

            if (path.ToLower().Contains(".mdb\\"))
            {
                text = GetMetaDataFromPGDB(path);
                if (text == null)
                    throw new ApplicationException("Unable to load PGDB metadata (" + path + ")");
                _cache[path] = text;
                return;
            }
            // This is reliable, but can be very slow (don't know why)
            //throw new ApplicationException("About to call catalog on (" + path + ")");
            LoadWithCatalog(path);
            //LoadWithCatalogShowDialog(path);
        }

        private static string GetMetaDataFromFGDB(string path)
        {
            string[] parts = path.ToLower().Split(new string [] {".gdb\\"},StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return null;
            if (!Directory.Exists(parts[0] + ".gdb"))
                return null;
            return GetMetaData("esriDataSourcesGDB.FileGDBWorkspaceFactory", parts[0] + ".gdb", null, parts[1], esriDatasetType.esriDTAny);
        }

        private static string GetMetaDataFromPGDB(string path)
        {
            string[] parts = path.ToLower().Split(new string[] { ".mdb\\" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return null;
            if (!File.Exists(parts[0] + ".mdb"))
                return null;
            return GetMetaData("esriDataSourcesGDB.AccessWorkspaceFactory", parts[0] + ".mdb", null, parts[1], esriDatasetType.esriDTAny);
        }

        private static string GetMetaData(string workspaceType, string workspacePath, string container, string data, esriDatasetType type)
        {
            IDatasetName results = null;
            IWorkspace workspace = GetWorkspace(workspaceType, workspacePath, null);
            if (string.IsNullOrEmpty(container))
                results = GetDatasetName(workspace, data, type);
            else if (container == "*")
                results = GetDatasetNameInWorkspaceSearchContainers(workspace, data, type);
            else
                results = GetDatasetNameFromContainer(workspace, container, data, type);
            if (results == null)
                return null;
             return ((IXmlPropertySet2)((IMetadata)results).Metadata).GetXml("");
        }

        private static IWorkspace GetWorkspace(string progId, string path, string connection)
        {
            // Create a name object for the dataset.
            IWorkspaceName2 workspaceName = new WorkspaceNameClass
            {
                WorkspaceFactoryProgID = progId,
                PathName = path,
                //ConnectionString = connection
            };
            IName workspaceIName = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)workspaceIName.Open();
            return workspace;
        }

        private static IDatasetName GetDatasetName(IWorkspace workspace, string data, esriDatasetType type)
        {
            IDatasetName results = null;

            // look for workspace\container\dataset
            if (data.Contains("\\"))
            {
                string[] parts = data.Split('\\');
                if (parts.Length == 2)
                    results = GetDatasetNameFromContainer(workspace, parts[0], parts[1], type);
            }
            if (results != null)
                return results;

            //look for workspace\data
            results = GetDatasetNameInWorkspaceSearchTop(workspace, data, type);
            if (results != null)
                return results;

            //look for workspace\*\data
            return GetDatasetNameInWorkspaceSearchContainers(workspace, data, type);
        }

        private static IDatasetName GetDatasetNameInWorkspaceSearchTop(IWorkspace workspace, string dataname, esriDatasetType type)
        {
            //look for data in top level names
            IEnumDatasetName names = workspace.DatasetNames[type];
            IDatasetName name = names.Next();
            while (name != null)
            {
                if (name.Name.ToLower() == dataname.ToLower())
                    return name;
                name = names.Next();
            }
            return null;
        }

        private static IDatasetName GetDatasetNameInWorkspaceSearchContainers(IWorkspace workspace, string dataname, esriDatasetType type)
        {
            //look for data in all FeatureDatasets
            IEnumDatasetName containers = workspace.DatasetNames[esriDatasetType.esriDTFeatureDataset];
            IDatasetName name1 = containers.Next();
            while (name1 != null)
            {
                IEnumDatasetName names = name1.SubsetNames;
                IDatasetName name2 = names.Next();
                while (name2 != null)
                {
                    if (name2.Name.ToLower() == dataname.ToLower())
                        return name2;
                    name2 = names.Next();
                }
                name1 = containers.Next();
            }
            
            //look for data in all containers (feature datasets are not containers, why I don't know)
            //containers are rasters (contain bands), coverage feature classes (contain arc, lines, ...), etc.
            containers = workspace.DatasetNames[esriDatasetType.esriDTContainer];
            name1 = containers.Next();
            while (name1 != null)
            {
                IEnumDatasetName names = name1.SubsetNames;
                IDatasetName name2 = names.Next();
                while (name2 != null)
                {
                    if (name2.Name.ToLower() == dataname.ToLower())
                        return name2;
                    name2 = names.Next();
                }
                name1 = names.Next();
            }
            return null;
        }

        private static IDatasetName GetDatasetNameFromContainer(IWorkspace workspace, string container, string dataset, esriDatasetType type)
        {
            //look for data in all FeatureDatasets
            IEnumDatasetName containers = workspace.DatasetNames[esriDatasetType.esriDTFeatureDataset];
            IDatasetName name1 = containers.Next();
            while (name1 != null)
            {
                if (name1.Name.ToLower() == container.ToLower())
                    break;
                name1 = containers.Next();
            }

            //look for data in all containers (feature datasets are not containers, why I don't know)
            //containers are rasters (contain bands), coverage feature classes (contain arc, lines, ...), etc.
            //find the container
            if (name1 == null)
            {
                containers = workspace.DatasetNames[esriDatasetType.esriDTContainer];
                name1 = containers.Next();
                while (name1 != null)
                {
                    if (name1.Name.ToLower() == container.ToLower())
                        break;
                    name1 = containers.Next();
                }
                if (name1 == null)
                    return null;
            }

            // look for data in the feature dataset
            IEnumDatasetName names = name1.SubsetNames;
            IDatasetName name2 = names.Next();
            while (name2 != null)
            {
                if (name2.Name.ToLower() == dataset.ToLower())
                    return name2;
                name2 = names.Next();
            }
            return null;
        }

        private static void LoadWithCatalogShowDialog(string path)
        {
            LoadingForm _dialog = new LoadingForm();
            _dialog.Message = "Loading ArcCatalog to try and find metadata.";
            _dialog.Path = path;
            _dialog.Command = _dialog.LoadMetadataWithCatalog;
            _dialog.ShowDialog();
        }

        //may be called by a background thread with the "Loading" dialog
        internal static void LoadWithCatalog(string path)
        {
            Trace.TraceInformation("{0}:   Begin get ESRI Metadata With Catalog for {1}", DateTime.Now, path); Stopwatch time = new Stopwatch(); time.Start();

            if (_catalog == null)
                _catalog = new GxCatalogClass(); // Fairly quick to return. May throw an exception.
            if (_catalog == null)
                throw new Exception("Unable to communicate with ArcCatalog");

            int numberFound;
            //This takes a long time on the first call (loads up the ESRI libraries)
            //FIXME - If this is the first time, and we are on the UI thread, then provide feedback
            Trace.TraceInformation("{0}:     Begin _catalog.GetObjectFromFullName({1}), elapsed ms = {2}", DateTime.Now, path, time.Elapsed.TotalMilliseconds); time.Reset(); time.Start();
            object obj = _catalog.GetObjectFromFullName(path, out numberFound);
            Trace.TraceInformation("{0}:     End   _catalog.GetObjectFromFullName({1}), elapsed ms = {2}", DateTime.Now, path, time.Elapsed.TotalMilliseconds); time.Reset(); time.Start();

            if (numberFound == 0)
            {
                // If the user does not have a folder connection to some point above this dataset,
                // then ArcGIS will not find the dataset.  Therefore:
                // try adding the root of path as a new folder connection, and trying again.
                _catalog.ConnectFolder(Path.GetPathRoot(path));
                obj = _catalog.GetObjectFromFullName(path, out numberFound);
                if (numberFound == 0)
                    // TM2.2 stored the data path without the dataset, so
                    // database/dataset/featureclass was saved as just database/featureclass
                    // Therefore if we didn't find database/featureclass, try looking for database/*/featureclass
                    // FIXME - store the corrected path in the xml file, store uncorrected path in mdb
                    obj = FindInDataset(_catalog, path, out numberFound);
                if (numberFound == 0)
                    throw new ArgumentException("Path (" + path + ") not found");
            }

            IGxObject geoObject;
            if (numberFound == 1)
                geoObject = obj as IGxObject;
            else
                geoObject = ((IEnumGxObject)obj).Next(); //get the first and ignore the rest.

            IPropertySet propertySet;

            if (geoObject is IMetadata)
                propertySet = ((IMetadata)geoObject).Metadata;
            else
                throw new ArgumentException("Path (" + path + ") has no metadata");

            if (!(propertySet is IXmlPropertySet2))
                throw new ArgumentException("Path (" + path + ") metadata not available as XML");
            string text = ((IXmlPropertySet2)propertySet).GetXml("");

            Trace.TraceInformation("{0}:   End   get ESRI Metadata With Catalog for {1}, elapsed ms = {2}", DateTime.Now, path, time.Elapsed.TotalMilliseconds);
            _cache[path] = text; 
            return;
        }

        private static object FindInDataset(GxCatalog _catalog, string path, out int numberFound)
        {
            numberFound = 0;
            int lastBackslashIndex = path.LastIndexOf('\\');
            int lastBackslashIndexPlusOne = lastBackslashIndex + 1;
            if (lastBackslashIndex == -1 || lastBackslashIndexPlusOne == path.Length)
                return null;
            string dbName = path.Substring(0,lastBackslashIndex);
            string fcName = path.Substring(lastBackslashIndexPlusOne, path.Length - lastBackslashIndexPlusOne);
            object obj = _catalog.GetObjectFromFullName(dbName, out numberFound);
            if (numberFound == 0)
                return null;
            IGxObject geoObject;
            if (numberFound == 1)
                geoObject = obj as IGxObject;
            else
                geoObject = ((IEnumGxObject)obj).Next(); //get the first and ignore the rest.
            numberFound = 0;
            if (geoObject == null)
                return null;
            IGxDatabase db = geoObject as IGxDatabase;
            if (db == null || db.Workspace == null)
                return null;
            foreach (string dsName in GetFeatureDataSetNames(db.Workspace))
            {
                string newPath = dbName + "\\" + dsName + "\\" + fcName;
                obj = _catalog.GetObjectFromFullName(newPath, out numberFound);
                    if (numberFound != 0)
                        return obj;
            }
            return null;
        }

        private static IEnumerable<string> GetFeatureDataSetNames(IWorkspace workspace)
        {
            IEnumDatasetName datasetNames = workspace.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
            IDatasetName name = datasetNames.Next();
            while (name != null)
            {
                yield return name.Name;
                name = datasetNames.Next();
            }
        }

    }
}
