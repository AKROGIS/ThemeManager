using System;
using System.Xml.Linq;
using System.ComponentModel;
using NPS.AKRO.ThemeManager.ArcGIS;
using System.Diagnostics;

//FIXME - Refactoring change - break into two objects: ThemeListData, ThemeData and SubThemeData
//FIXME - Add Datasource to ThemeListData, and remove from TmNode.
//FIXME - Consider a compatibility breaking change:
//        Themes do not contain data sources (they point to a layer file, or other document)
//        Sub-Themes contain the data source information.
//        Therefore all layer file themes will have one and only one sub-theme
//        Historically a theme contains the properties of this singular sub-theme.  

namespace NPS.AKRO.ThemeManager.Model
{
    [Serializable]
    class ThemeData : ICloneable, INotifyPropertyChanged
    {
        public ThemeData()
            : this(null, null, null, null, null, null, null, null, null, null, null, null, null)
        {
        }

        public ThemeData(string path)
            : this(path, null, null, null, null, null, null, null, null, null, null, null, null)
        {
        }

        public ThemeData(string path, string type, string format)
            : this(path, type, format, null, null, null, null, null, null, null, null, null, null)
        {
        }

        public ThemeData(string path, string type, string format, string version,
            string datasource, string workspacePath, string workspaceType,
            string workspaceProgId, string container, string containerType,
            string dataSourceName, string datasetName, string datasetType)
        {
            Path = path;
            Type = type;
            Format = format;
            Version = version;
            DataSource = datasource;
            WorkspacePath = workspacePath;
            WorkspaceType = workspaceType;
            WorkspaceProgId = workspaceProgId;
            Container = container;
            ContainerType = containerType;
            DataSourceName = dataSourceName;
            DataSetName = datasetName;
            DataSetType = datasetType;
        }

        //Path is a filesystem path to a file, not an ArcObject.
        //typically this is a layer file.
        //Path will be null for sub-themes. Sub-theme data is in other proeprties
        //Path is available to the UI, so it needs to notify the UI if it changes
        public string Path
        {
            get { return _path; }
            set
            {
                if (value != _path)
                {
                    _path = value;
                    OnPropertyChanged("Path");
                }
            }
        }
        private string _path;

        //Type controls the icon displayed.
        //It needs to notify the UI if it changes
        public string Type
        {
            get { return _type; }
            set
            {
                if (value != _type)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        private string _type;

        // Historically, when data.path points to a layer file, data type != layer file
        // oh no, it is either "group Layer" if the layer file contains a group of layers,
        // or it is the type of the data source in the layer. Similarly, Path contains either
        // the pathe to the layer file (for a group layer, or a layer file with a single datasource,
        // however for all items in a group layer, then path has the ArcObjects path to the
        // data source.
        
        // For the future, Path will contain only contain a file system path.
        // DatasourceName will contain the ArcObjects name of the data source.
        // for a layer file with a single data layer, then both Path and DataSourceName are populated
        // for a group layer file, then DataSourceName is null
        // for a datasource in a group layer, then Path is null.

        // Data loaded from mdb or tml can only be guaranteed to meet the historical specifications
        // If a theme has been added with TM3.0, or an old theme has been re-loaded,
        // then the DataSourceName (and other properties) will be populated

        // DataSource is typically:
        // WorkspacePath + "\\" + {Container + "\\" +} DataSourceName
        // However this needs to be verified for all datasources
        // For now, we will track both DataSource and the constituent parts

        //Datasource is available to the UI, so it needs to notify the UI if it changes
        //Datasource is settable by other classes, but not the UI.
        public string DataSource
        {
            get { return _datasource; }
            set
            {
                if (value != _datasource)
                {
                    _datasource = value;
                    OnPropertyChanged("DataSource");
                }
            }
        }
        private string _datasource;

        internal string Format { get; set; }
        internal string Version { get; set; }
        internal string WorkspacePath { get; set; }
        internal string WorkspaceProgId { get; set; }
        internal string WorkspaceType { get; set; }
        internal string Container { get; set; }
        internal string ContainerType { get; set; }
        internal string DataSourceName { get; set; }
        internal string DataSetName { get; set; }
        internal string DataSetType { get; set; }

        internal bool IsLayerFile
        {
            get
            {
                return (Path == null) ? false : (System.IO.Path.GetExtension(Path).ToLower() == ".lyr");
            }
        }

        internal bool IsGroupLayerFile
        {
            get
            {
                return (IsLayerFile && Type == "Group Layer");
            }
        }

        //The following checks are based on Type information only available to 
        //themes loaded with TM3.0, however these checks are only called
        //when trying to find metadata for themes loaded with TM3.0
        internal bool IsCoverage
        {
            get
            {
                return Type != null && (Type.Contains("Coverage") || Type.Contains("Region") || Type.Contains("Route"));
            }
        }

        internal bool IsShapefile
        {
            get
            {
                return Type != null && Type.Contains("Shapefile");
            }
        }

        internal bool IsCad
        {
            get
            {
                return Type != null && Type.Contains(" CAD ");
            }
        }


        internal bool IsInGeodatabase
        {
            get
            {
                return Type != null && Type.Contains("Geodatabase");
            }
        }

        internal bool IsEsriMapService
        {
            get
            {
                return DataSource != null && DataSource.StartsWith("http", StringComparison.OrdinalIgnoreCase) && 
                       DataSource.EndsWith("/MapServer", StringComparison.OrdinalIgnoreCase);
            }
        }

        internal bool IsEsriImageService
        {
            get
            {
                return DataSource != null && DataSource.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                       DataSource.EndsWith("/ImageServer", StringComparison.OrdinalIgnoreCase);
            }
        }

        internal bool IsEsriFeatureService
        {
            get
            {
                return WorkspacePath != null && WorkspacePath.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                       WorkspacePath.EndsWith("/FeatureServer", StringComparison.OrdinalIgnoreCase);
            }
        }

        // FIXME - the following would be nice to have, but I don't have
        // workable code, so they are not used
        //
        //internal bool IsThemeListFile
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        #region XML Serialize

        public static ThemeData Load(XElement xele)
        {
            if (xele == null)
                throw new ArgumentNullException("xele");
            if (xele.Name != "data")
                throw new ArgumentException("Invalid Xelement");
            ThemeData data = new ThemeData(
                xele.Value,
                (string)xele.Attribute("type"),
                (string)xele.Attribute("format"),
                (string)xele.Attribute("version"),
                (string)xele.Attribute("datasource"),
                (string)xele.Attribute("workspace"),
                (string)xele.Attribute("workspacetype"),
                (string)xele.Attribute("workspaceprogid"),
                (string)xele.Attribute("container"),
                (string)xele.Attribute("containertype"),
                (string)xele.Attribute("datasourcename"),
                (string)xele.Attribute("datasetname"),
                (string)xele.Attribute("datasettype")
            );
            return data;
        }

        public XElement ToXElement()
        {
            return new XElement("data",
                new XAttribute("type", Type ?? ""),
                new XAttribute("format", Format ?? ""),
                new XAttribute("version", Version ?? ""),
                new XAttribute("datasource", DataSource ?? ""),
                new XAttribute("workspace", WorkspacePath ?? ""),
                new XAttribute("workspacetype", WorkspaceType ?? ""),
                new XAttribute("workspaceprogid", WorkspaceProgId ?? ""),
                new XAttribute("container", Container ?? ""),
                new XAttribute("containertype", ContainerType ?? ""),
                new XAttribute("datasourcename", DataSourceName ?? ""),
                new XAttribute("datasetname", DataSetName ?? ""),
                new XAttribute("datasettype", DataSetType ?? ""),
                Path
            );
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            ThemeData obj = (ThemeData)MemberwiseClone();
            return obj;
            // return MemberwiseClone();
        }

        #endregion

        #region INotifyPropertyChanged

        //FIXME - do I need this??? are events serialized???
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handle = PropertyChanged;
            if (handle != null)
                handle(this, new PropertyChangedEventArgs(property));
        }

        #endregion

    }
}
