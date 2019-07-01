using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using NPS.AKRO.ThemeManager.ArcGIS;

namespace NPS.AKRO.ThemeManager.Model
{
    [Serializable]
    class ThemeNode : TmNode
    {
        //The TypeName is persisted in image resources and data files, so it is historic,
        //case sensitive, and needs to be impervious to refactoring
        new public const string TypeString = "theme";

        static readonly private DateTime DefaultPubDate = new DateTime(1900, 01, 01); //FIXME - make default date a setting

        //Load From XML
        public ThemeNode()
            : this(null, null, null, null, null, null)
        {
        }

        //File->New
        public ThemeNode(string name)
            : this(name, null, null, null, null, null)
        {
        }

        //File->New
        public ThemeNode(string name, string path)
            : this(name, null, new ThemeData(path), null, null, null)
        {
        }

        //Load From MDB, SubTheme, overloads
        public ThemeNode(string name, TmNode parent, ThemeData data, Metadata metadata, string desc, DateTime? pubDate)
            : base(name, parent, metadata, desc)
        {
            IsInitialized = false;
            Data = data;
            PubDate = pubDate.HasValue ? pubDate.Value : DefaultPubDate;
            //Age is not a saved property, so calling the setter will not flag the node as edited
            Age = CalculateAge();
            IsInitialized = true;
        }

        public override string TypeName { get { return TypeString; } }

        public override bool IsLaunchable
        {
            get
            {
                return (HasData && File.Exists(Data.Path));
            }
        }

        public override bool HasData
        {
            get
            {
                 return (!string.IsNullOrEmpty(Data.Path));
            }
        }

        public override bool HasDataToPreview
        {
            get
            {
                if (HasData && File.Exists(Data.Path) &&
                        (
                        Data.Path.EndsWith("lyr", StringComparison.CurrentCultureIgnoreCase) ||
                        Data.Path.EndsWith("mxd", StringComparison.CurrentCultureIgnoreCase) ||
                        Data.Path.EndsWith("mxt", StringComparison.CurrentCultureIgnoreCase)
                        )
                    )
                    return true;
                return false;
            }
        }

        public override void Reload()
        {
            SyncThemeDataToPath();
            Metadata.Repair(Data);
            //Don't bother syncing ;  User can ask for this later.
        }

        public ThemeData Data
        {
            get
            {
                if (_data == null)
                    Data = new ThemeData();
                return _data;
            }
            set
            {
                if (_data != value)
                {
                    if (_data != null)
                        _data.PropertyChanged -= Data_PropertyChanged;
                    _data = value;
                    if (_data != null) 
                        _data.PropertyChanged += Data_PropertyChanged;
                    //FIXME: Replacing the Data object implicitly changes the data properties.
                    //Should I call Data_PropertyChanged to ensure I am up to date, or is this
                    //too expensive?  For now I calc the image name and set ThemeList.IsDirty 
                    ImageName = CalculateImageName();
                    MarkAsChanged();
                }
            }
        }
        private ThemeData _data;

        public DateTime PubDate
        {
            get { return _pubDate; }
            set
            {
                if (value != _pubDate)
                {
                    _pubDate = value;
                    OnPropertyChanged("PubDate");
                    Age = CalculateAge();
                    MarkAsChanged();
                }
            }
        }
        private DateTime _pubDate;


        protected override int CalculateAge()
        {
            int baseAge = base.CalculateAge();
            int myAge = (DateTime.Now - _pubDate).Days;
            return Math.Min(myAge,baseAge);
        }

        //FIXME - Data.Path changes may load arcObjects, which could cause exceptions
        // (i.e. No license) Need to wrap this with handler code in the UI, however,
        // this is bound to a form text box
        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MarkAsChanged();

            if (e.PropertyName == "Path")
            {
                if (this is ThemeNode)
                {
                    SyncThemeDataToPath();
                    // Get new metadata for new data path/type
                    //replacing the metadaata object, breaks the databinding on the prperties form
                    //Metadata = Metadata.Find(Data)
                    //FIXME - subtheme metadata is not reloaded
                    Metadata.Repair(Data);
                    //resync all metadata
                    SyncWithMetadata(true);
                }
            }
            if (e.PropertyName == "Type")
                ImageName = CalculateImageName();
            //Do nothing else for Datasource property
        }

        internal void SyncThemeDataToPath()
        {
            //Debug.Assert(this is ThemeListNode || this is ThemeNode, "Must be a themelist or theme to change data path");
            //if (!(this is ThemeNode))
            //    return;

            if (string.IsNullOrEmpty(Data.Path) || !File.Exists(Data.Path))
            {
                Data.Type = "Not Found";
                return;
            }

            // Use ToList() to make a copy since we will be modifying Children
            foreach (var subTheme in Children.ToList())
                subTheme.Delete();

            if (Name == "New Theme")
                Name = Path.GetFileNameWithoutExtension(Data.Path);

            var extension = Path.GetExtension(Data.Path);
            if (extension != null)
            {
                string ext = extension.ToLower();
                if (ext == ".kml" || ext == ".kmz")
                {
                    Data.Type = "Google Earth Document";
                    return;
                }

                if (ext == ".pdf")
                {
                    Data.Type = "PDF";
                    return;
                }

                if (ext == ".doc" || ext == ".docx")
                {
                    Data.Type = "MS Word";
                    return;
                }

                if (ext == ".mxd" || ext == ".mxt")
                {
                    Data.Type = "ArcMap Document";
                    Trace.TraceInformation("{0}: Start of ThemeBuilder.BuildSubThemesForMapDocument({1}:{2})", DateTime.Now, this, Data.Path); Stopwatch time = Stopwatch.StartNew();
                    ThemeBuilder.BuildSubThemesForMapDocument(this);
                    time.Stop(); Trace.TraceInformation("{0}: End   of ThemeBuilder.BuildSubThemesForMapDocument() - Elapsed Time: {1}", DateTime.Now, time.Elapsed);
                }
                if (ext == ".lyr")
                {
                    Trace.TraceInformation("{0}: Start of ThemeBuilder.BuildThemesForLayerFile({1}:{2})", DateTime.Now, this, Data.Path); Stopwatch time = Stopwatch.StartNew();
                    ThemeBuilder.BuildThemesForLayerFile(this);
                    time.Stop(); Trace.TraceInformation("{0}: End   of ThemeBuilder.BuildThemesForLayerFile() - Elapsed Time: {1}", DateTime.Now, time.Elapsed);
                }
            }
        }

        internal string GetDataTypeCode()
        {
            //This is based on the logic in Theme Manager 2.2,
            //so it should capture all valid values stored in a historic database
            //numerical image indices returned are shown as comments

            // datatype was one of the following (used only for icon selection in treeview)
            // typically it applied to the primary data source inside a layer file
            //  ImageIndex =  2: "*ANNO*"
            //  ImageIndex =  3: "*CAD*"
            //  ImageIndex =  4: "*GROUP*"
            //  ImageIndex =  5: "ARC", "LINE", "*ROUTE*"
            //  ImageIndex =  6: "NODE", "POINT"
            //  ImageIndex =  7: "*POLY*","*REGION*"
            //  ImageIndex =  8: "IMAGE", "GRID", "MRSID", "RASTER"
            //  ImageIndex =  9: default ("NRECOGNIZED LAYER TYPE")
            //  ImageIndex = 10: "IMAGECAT", "RASTER CATALOG", "RASTERCATALOG"
            //  ImageIndex = 11: "*WMS*", "*IMS*"
            //  ImageIndex = 12: "*TIN*"
            //  ImageIndex = 13: "*AGS*"
            //  ImageIndex = ??: "*SHAPE*" -- ?? = GetShapeImageIndex(sLayerName) = {5, 6, 7, 9, 10}
            //                      esriGeoDatabase.IFeatureClass.ShapeType = 
            //                          esriGeometryPoint | esriGeometryMultipoint => 6:"Point"
            //                          esriGeometryLine | esriGeometryPolyline | esriGeometryPath => 5:"Line"
            //                          esriGeometryPolygon => 6:"Point" or 10:"RasterCatalog" if esriGeoDatabase.IFeatureClass.FeatureType = esriFTRasterCatalogItem
            //                          9:"Unknown" for any thing else.
            //
            // This may not be the complete logic - try to find cls_AKSO_DataSourceName which may tell more.
            // I think it appends the datatype on the end of the data source names i.e. "../layer.lyr::POINT"
            // These values are separated when stored in the database.

            string datatype;
            if (Data == null || Data.Type == null)
                datatype = "";
            else
                datatype = Data.Type.ToUpper();
            
            if (Data != null && Data.DataSource != null && Data.DataSource.StartsWith("http"))
                return "_wms"; //look for non local data sources first

            if (datatype.Contains("ANNO"))
                return "_anno";  //2
            if (datatype.Contains("CAD"))
                return "_cad"; //3
            if (datatype.Contains("GROUP") && datatype != "WMS GROUP LAYER")
                return "_group"; //4
            if (datatype == "ARC" || datatype == "LINE" || datatype.Contains("ROUTE"))
                return "_line"; //5
            if (datatype == "NODE" || datatype == "POINT")
                return "_point";  //6

            if (datatype.Contains("WCS"))
                return "_wms"; //must be done before raster, else theme will get the raster icon

            // Need to insert this New check before the *Poly* check else raster catalogs will be tagged as _poly
            if ((datatype.Contains("RASTER") || datatype.Contains("MOSAIC")) && !datatype.Contains("IMAGESERVER"))
                return "_raster"; //8

            //if (datatype.Contains("POLY") || datatype.Contains("REGION")) //problem polyline matches *poly*
            if ((datatype.Contains("POLY") && !datatype.Contains("POLYLINE")) || datatype.Contains("REGION"))
                return "_poly"; //7
            if (datatype == "MRSID" || datatype == "GRID" || datatype == "RASTER" || datatype == "IMAGE")
                return "_raster"; //8
            if (datatype == "" || datatype == "NRECOGNIZED LAYER TYPE")
                return ""; //9
            if (datatype == "IMAGECAT" || datatype == "RASTER CATALOG" || datatype == "RASTERCATALOG")
                return "_raster"; //10 (ArcGIS v10 uses image 8)
            if (datatype.Contains("WMS") || datatype.Contains("IMS"))
                return "_wms"; //11
            if (datatype.Contains("TIN"))
                return "_tin"; //12
            if (datatype.Contains("AGS"))
                return "_wms"; //13  (ArcGIS v10 uses image 11)
            // Matches extended info now returned (i.e. shapefile)
            //if (datatype.Contains("SHAPE"))
            //    return ""; //returned 5,6,7,9,10 based on additional logic (might not have been used)

            // New data types in version 3.0
            if (datatype.Contains("MULTIPATCH"))
                return "_mpatch"; //new
            if (datatype == "MXD" || datatype == "ARCMAP DOCUMENT")
                return "_mxd"; //new
            if (datatype == "PDF")
                return "_pdf"; //new
            if (datatype == "MS WORD")
                return "_doc"; //new
            if (datatype == "MAP DATA FRAME")
                return "_dataframe"; //new
            if (datatype == "KML" || datatype == "KMZ" || datatype == "GOOGLE EARTH DOCUMENT")
                return "_ge"; //new
            if (datatype == "NOT FOUND")
                return "_notfound"; //new

            //Added to match esriGeometryType and esriDataSourceType Enums
            if (datatype.Contains("POINT"))
                return "_point";  //6
            if (datatype.Contains("LINE") || datatype.Contains("ARC"))
                return "_line"; //5
            if (datatype.Contains("RING") || datatype.Contains("POINT"))
                return "_poly";  //6
            if (datatype.Contains("MAPSERVER") || datatype.Contains("IMAGESERVER") || datatype.Contains("WCS"))
                return "_wms"; //11
            if (datatype.Contains("RASTER") || datatype.Contains("MOSAIC"))
                return "_raster"; //8
            if (datatype.Contains("DIMENSION"))
                return "_dim"; //new
            if (datatype.Contains("NETWORK"))
                return "_network"; //new
            if (datatype.Contains("TOPOLOGY"))
                return "_topo"; //new
            if (datatype.Contains("TERRAIN"))
                return "_terrain"; //new
            if (datatype.Contains("CADASTRAL"))
                return "_cadastral"; //new
            if (datatype.Contains("BASEMAP"))
                return "_basemap"; //new

            return "";
        }

        public override void Launch()
        {
            if (IsLaunchable)
                Process.Start(Data.Path);
        }

        public string SyncPubDate(bool preferMetadata)
        {
            if (preferMetadata)
                if (Metadata.HasPubDate)
                    return SyncPubDateWithMetaData();
                else
                {
                    string ret = SyncPubDateWithFileProps();
                    if (ret == null)
                        return null;
                    return "Metadata has no Publication Date. " + ret;
                }
            return SyncPubDateWithFileProps();
        }

        public string SyncPubDateWithMetaData()
        {
            if (!Metadata.HasPubDate)
                return "Metadata has no Publication Date.";
            PubDate = Metadata.PubDate;
            return null;
        }

        /// <summary>
        /// Reset the PubData based on the file modification time
        /// </summary>
        /// <remarks>
        /// This does not do anything is the data objects is stored in a geodatabase.
        /// </remarks>
        public string SyncPubDateWithFileProps()
        {
            if (!File.Exists(Data.Path))
            {
                PubDate = new DateTime(1900, 1, 1);
                return "Cannot determine file modification date of the data.";
            }
            PubDate = File.GetLastWriteTime(Data.Path);
            return null;
            //TODO - See if ESRI supports an API to query the modification date of a data object
        }

    }
}
