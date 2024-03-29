﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

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
        public ThemeData(string path = null, string type = null, string format = null, string version = null,
            string datasource = null, string workspacePath = null, string workspaceType = null,
            string workspaceProgId = null, string container = null, string containerType = null,
            string dataSourceName = null, string datasetName = null, string datasetType = null)
        {
            Path = string.IsNullOrWhiteSpace(path) ? null : path;
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
        //Path will be null for sub-themes. Sub-theme data is in other properties
        //Path is available to the UI, so it needs to notify the UI if it changes
        public string Path
        {
            get { return _path; }
            set { SetObservableField(ref _path, value); }
        }
        private string _path;

        //Type controls the icon displayed.
        //It needs to notify the UI if it changes
        public string Type
        {
            get { return _type; }
            set { SetObservableField(ref _type, value); }
        }
        private string _type;

        // Historically, when data.path points to a layer file, data type != layer file
        // oh no, it is either "group Layer" if the layer file contains a group of layers,
        // or it is the type of the data source in the layer. Similarly, Path contains either
        // the path to the layer file (for a group layer, or a layer file with a single datasource,
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
        // However this needs to be verified for all data sources
        // For now, we will track both DataSource and the constituent parts

        //Datasource is available to the UI, so it needs to notify the UI if it changes
        //Datasource is settable by other classes, but not the UI.
        public string DataSource
        {
            get { return _datasource; }
            set { SetObservableField(ref _datasource, value); }
        }
        private string _datasource;

        private string Format { get; }
        internal string Version { private get; set; }
        internal string WorkspacePath { get; set; }
        internal string WorkspaceProgId { get; set; }
        internal string WorkspaceType { get; set; }
        internal string Container { get; set; }
        internal string ContainerType { get; set; }
        internal string DataSourceName { get; set; }
        internal string DataSetName { get; set; }
        internal string DataSetType { get; set; }

        //The following checks are based on Type information only available to 
        //themes loaded with TM3.0, however these checks are only called
        //when trying to find metadata for themes loaded with TM3.0
        internal bool IsCoverage =>
            WorkspaceProgId == "ArcInfo" ||
            (WorkspaceProgId != null && WorkspaceProgId.StartsWith("esriDataSourcesFile.ArcInfoWorkspaceFactory")) ||
            (Type != null && (Type.Contains("Coverage") || Type.Contains("Region") || Type.Contains("Route")));

        internal bool IsShapefile => 
            WorkspaceProgId == "Shapefile" ||
            (WorkspaceProgId != null && WorkspaceProgId.StartsWith("esriDataSourcesFile.ShapefileWorkspaceFactory")) ||
            (Type != null && Type.Contains("Shapefile"));

        internal bool IsCad =>
            WorkspaceProgId == "Cad" ||
            (WorkspaceProgId != null && WorkspaceProgId.StartsWith("esriDataSourcesFile.CadWorkspaceFactory")) ||
            (Type != null && Type.Contains(" CAD "));

        // SCD is not supported in Pro
        internal bool IsSdc =>
            (WorkspaceProgId != null && WorkspaceProgId.StartsWith("esriDataSourcesFile.SDCWorkspaceFactory"));

        internal bool IsRasterBand => DataSetType == "RasterBand";

        // This is used to set the metadata path to the DataSource as Esri formatted XML metadata
        // We can include the Pro FileGDB in this category
        internal bool IsInGeodatabase =>
            WorkspaceProgId != null && (
            WorkspaceProgId.StartsWith("esriDataSourcesGDB.FileGDBWorkspaceFactory") ||
            WorkspaceProgId.StartsWith("esriDataSourcesGDB.AccessWorkspaceFactory") ||
            WorkspaceProgId.StartsWith("esriDataSourcesGDB.SdeWorkspaceFactory") ||
            WorkspaceProgId == "FileGDB");

        internal bool IsEsriMapService =>
            DataSource != null && DataSource.StartsWith("http", StringComparison.OrdinalIgnoreCase) && 
            DataSource.EndsWith("/MapServer", StringComparison.OrdinalIgnoreCase);

        internal bool IsEsriImageService =>
            DataSource != null && DataSource.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            DataSource.EndsWith("/ImageServer", StringComparison.OrdinalIgnoreCase);

        internal bool IsEsriFeatureService =>
            WorkspacePath != null && WorkspacePath.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            WorkspacePath.EndsWith("/FeatureServer", StringComparison.OrdinalIgnoreCase);

        #region XML Serialize

        public static ThemeData Load(XElement xEle)
        {
            if (xEle == null)
                throw new ArgumentNullException(nameof(xEle));
            if (xEle.Name != "data")
                throw new ArgumentException("Invalid XElement");
            ThemeData data = new ThemeData(
                xEle.Value, // returns an empty string when there is no value
                (string)xEle.Attribute("type"),
                (string)xEle.Attribute("format"),
                (string)xEle.Attribute("version"),
                (string)xEle.Attribute("datasource"),
                (string)xEle.Attribute("workspace"),
                (string)xEle.Attribute("workspacetype"),
                (string)xEle.Attribute("workspaceprogid"),
                (string)xEle.Attribute("container"),
                (string)xEle.Attribute("containertype"),
                (string)xEle.Attribute("datasourcename"),
                (string)xEle.Attribute("datasetname"),
                (string)xEle.Attribute("datasettype")
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
            return (ThemeData)MemberwiseClone();
        }

        #endregion

        #region INotifyPropertyChanged

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        }

        // From code provided by Marc Gravell (https://stackoverflow.com/a/1316417)
        private bool SetObservableField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

    }
}
