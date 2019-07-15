using NPS.AKRO.ThemeManager.ArcGIS;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Model.ThemeList;
using NPS.AKRO.ThemeManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace NPS.AKRO.ThemeManager.Model
{
    enum TmNodeType
    {
        ThemeList,
        Category,
        Theme,
        //SubTheme,//,
        //Unknown
    }

    /// <summary>
    /// Status of the ThemeList in relation to its persistant storage (backing store)
    /// </summary>
    enum ThemeListStatus
    {
        /// <summary>ThemeList has been created, but does not have a backing store</summary>
        Created,
        /// <summary>ThemeList has a valid backing store but has not been loaded</summary>
        Initialized,
        /// <summary>ThemeList is building tree from backing store</summary>
        Loading,
        /// <summary>ThemeList has a fully loaded tree which matches the backing store</summary>
        Loaded,
        /// <summary>ThemeList has a fully loaded tree, but the memory copy differs from the backing store</summary>
        Dirty,
        /// <summary>ThemeList is writing the tree in memory to the backing store</summary>
        Saving
    }

    /// <summary>
    /// Represents a node in the Theme Manager Tree
    /// </summary>
    [Serializable]
    class TmNode : ICloneable, INotifyPropertyChanged
    {
        static readonly int CountOfNodeTypes = Enum.GetValues(typeof(TmNodeType)).Length;
        private static readonly DateTime DefaultPubDate = new DateTime(1900, 01, 01); //FIXME - make default date a setting
        private ThemeData _data;
        private Store _dataStore;
        private DateTime _pubDate;
        private bool _readonly;
        private ThemeListStatus _status;
        private TmNodeType _type;
        private bool _issueUpdates = true; //Should this object issue update events or not, inherited from parent.

        public TmNode(TmNodeType nodeType)
            : this(nodeType, null, null, null, null, null, null)
        { }

        public TmNode(TmNodeType nodeType, string name)
            : this(nodeType, name, null, null, null, null, null)
        { }

        public TmNode(TmNodeType nodeType, string name, TmNode parent, ThemeData data, Metadata metadata, string desc, DateTime? pubDate)
        {
            _type = nodeType;
            _name = name;
            Parent = parent;

            _pubDate = pubDate.HasValue ? pubDate.Value : DefaultPubDate;
            //_readonly = (Parent == null) ? false : Parent._readonly;

            _description = desc;

            // Always create a data and Metadata object, else data binding in properties form won't work.
            _data = data ?? new ThemeData();
            _data.PropertyChanged += Data_PropertyChanged;
            Metadata = metadata ?? new Metadata();
            Metadata.PropertyChanged += Metadata_PropertyChanged;

            if (_type == TmNodeType.ThemeList)
            {
                Author = new ThemeListAuthor();
                Author.Name = Environment.UserName;
                _status = ThemeListStatus.Created;
                _dataStore = TryToGetDataStore();
                // TryToGetDataStore() will set _readonly for theme
                ThemeList = this;
            }
        }


         /// <summary>
        /// Visibility is used by the UI control to determine which node to display
        /// It is a combination of the Hidden flag, and the current search/filter settings
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    //OnPropertyChanged();
                }
            }
        }
        private bool _isVisible;

        /// <summary>
        /// Hidden is a user controlled setting on each item (usually categories) to
        /// remove unwanted categories from view.
        /// </summary>
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (value != _isHidden)
                {
                    _isHidden = value;
                    OnPropertyChanged("IsHidden");
                }
            }
        }
        private bool _isHidden;

        /// <summary>
        /// True if this node is selected for an action command in the UI
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }
        //This is no serialized, because we do not what copied objects to be selected
        [NonSerialized]
        private bool _isSelected;

        /// <summary>
        /// True if this themelist has items in it that have changed.
        /// </summary>
        public bool IsDirty
            { get { return _status == ThemeListStatus.Dirty; }
                private set
                {
                    if (value == true) // && _status == ThemeListStatus.Loaded)
                        _status = ThemeListStatus.Dirty;
                    if (value == false && _status == ThemeListStatus.Dirty)
                        _status = ThemeListStatus.Loaded;
                }
            }

        public bool IsThemeList { get { return Type == TmNodeType.ThemeList; } }
        public bool IsValidThemeList { get { return IsThemeList && _dataStore != null; } }

        public bool IsCategory { get { return Type == TmNodeType.Category; } }

        public bool IsTheme
        {
            get { return Type == TmNodeType.Theme && (Parent == null || Parent.Type != TmNodeType.Theme); }
        }

        public bool IsSubTheme
        {
            //get { return Type == TmNodeType.SubTheme; }
            get { return Type == TmNodeType.Theme && Parent != null && Parent.Type == TmNodeType.Theme; }
        }

        public bool IsReadOnly
        {
            get
            {
                if (ThemeList == null)
                    return false;
                else
                    if (Type == TmNodeType.ThemeList)
                        return _readonly;
                    else
                        return ThemeList.IsReadOnly;
            }
            private set
            {
                if (Type == TmNodeType.ThemeList)
                {
                    if (_readonly != value)
                    {
                        _readonly = value;
                        UpdateImageIndex(true);
                    }
                }
                else
                    if (ThemeList != null)
                        ThemeList.IsReadOnly = value;
            }
        }

        public bool IsLaunchable
        {
            get
            {
                return IsTheme && HasData && File.Exists(Data.Path);
            }
        }

        private bool HasAuthor
        {
            get { return Author != null; }
        }

        public bool HasMetadata
        {
            get
            {
                return (Metadata != null && !string.IsNullOrEmpty(Metadata.Path));
            }
        }

        public bool HasData
        {
            get
            {
                return (Data != null && !string.IsNullOrEmpty(Data.Path));
            }
        }

        public bool HasDataToPreview
        {
            get
            {
                if (IsThemeList || IsCategory)
                    return false;
                if (HasData)
                    if (Data.Path.EndsWith("lyr", StringComparison.CurrentCultureIgnoreCase) ||
                        Data.Path.EndsWith("mxd", StringComparison.CurrentCultureIgnoreCase))
                        return true;
                return false;
            }
        }

        public bool HasChildren
        {
            get { return _children.Count > 0; }
        }





        public TmNodeType Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                //FIXME - Check that name does not have a path separator character
                if (value != _name)
                {
                    _name = value;
                    if (ThemeList != null)
                        ThemeList.IsDirty = true;
                    OnPropertyChanged("Name");
                }
            }
        }
        private string _name;

        //FIXME - changing Data.Path does not call the setter for Data, hence the new datastore is not loaded.
        public ThemeData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                _data.PropertyChanged += Data_PropertyChanged;
                if (IsThemeList)
                {
                    //We just loaded a new theme list
                    _dataStore = TryToGetDataStore();
                    //if (_dataStore == null)
                    // FIXME - set file not found flag and give a visual representation.
                    //    throw new ApplicationException("Path is not a valid themelist");
                    UpdateImageIndex(true);
                }
            }
        }

        public Metadata Metadata
        {
            get { return _metadata; }
            set
            {
                if (value != _metadata)
                {
                    _metadata = value;
                    _metadata.PropertyChanged += Metadata_PropertyChanged;
                }
            }
        }
        private Metadata _metadata;

        public DateTime PubDate
        {
            get { return _pubDate; }
            set
            {
                if (value != _pubDate)
                {
                    _pubDate = value;
                    DaysSinceNewestPublication = CalcDaysSinceMyPublication();
                    if (ThemeList != null)
                        ThemeList.IsDirty = true;
                    OnPropertyChanged("PubDate");
                }
            }
        }

        public ThemeListAuthor Author
            { get; set; }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    if (ThemeList != null)
                        ThemeList.IsDirty = true;
                    OnPropertyChanged("Description");
                }
            }
        }
        private string _description;

        public string Summary
        {
            get { return _summary; }
            set
            {
                if (value != _summary)
                {
                    _summary = value;
                    if (ThemeList != null)
                        ThemeList.IsDirty = true;
                    OnPropertyChanged("Summary");
                }
            }
        }
        private string _summary;

        public string Tags
        {
            get { return _tags; }
            set
            {
                if (value != _tags)
                {
                    _tags = value;
                    if (ThemeList != null)
                        ThemeList.IsDirty = true;
                    OnPropertyChanged("Tags");
                }
            }
        }
        private string _tags;

        public string ImageKey
        {
            get
            {
                if (_imageKey == null)
                    _imageKey = GetImageKey();
                return _imageKey;
            }
            private set
            {
                if (value != _imageKey)
                {
                    _imageKey = value;
                    OnPropertyChanged("ImageKey");
                }
            }
        }
        private string _imageKey;

        public IEnumerable<TmNode> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<TmNode>();
                return _children;
            }
        }
        private IList<TmNode> _children;

        public TmNode Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (_parent != null)
                {
                    //Dependency properties would be cool here!
                    _issueUpdates = _parent._issueUpdates;
                    //_readonly = _parent._readonly;
                    if (_parent.ThemeList != null)
                    ThemeList = _parent.ThemeList;
                    //Recursively reset the parent for all descendents.
                    // This is done to ensure copyied/dragged nodes get the right ancestory on reserialization.
                    //FIXME - this could be done more elegently during re-serialization
                    foreach (var child in Children)
                        child.Parent = this;
                }
            }
        }

        //serializing the parent will serialize the grandparent and all it's children ...
        //since we don't want to serialize the whole tree for each node, we exclude the parent
        //we need to reinitialize the parent after re-serialization
        //until that happens the re-serialized node is an orphan.
        //This also applies to all the decendents below this node
        
        //The solution (Hack) is to use recursion in the Parent set accessor.
        //the parent for the root of the re-serialized node will be set when it
        //is added into the tree somewhere.

        [NonSerialized]
        private TmNode _parent;

        private TmNode ThemeList
        {
            get { return _themeList; }
            set {
                //Debug.Assert(value.Type == TmNodeType.ThemeList, "Setting themelist to a non-themelist node");
                //themelist will be null for favorites and saved searches. 
                if (value != null && value.Type == TmNodeType.ThemeList)
                    _themeList = value;
                else
                    //Throw exception ???
                    _themeList = null;
            }
        }

        //by virtue of the children property, serializing themelist will serialize the whole tree.
        //_themelist will be reinitialized when the parent property is reinitialized (via Parent set accessor).
        //until then a re-serialized node is an orphan.
        [NonSerialized]
        private TmNode _themeList;


        internal bool NeedsThemeListLoaded
        {
            get
            {
                return (IsThemeList && _status == ThemeListStatus.Initialized);
            }
        }

        private void Remove(TmNode child)
        {
            if (_children != null && child != null)
            {
                _children.Remove(child);
                //FIXME - Remove side effects, This should be a call to update Age
                _ageInDays = -1;
                UpdateImageIndex(false);
                if (ThemeList != null)
                    ThemeList.IsDirty = true;
                OnChildRemoved(child);
            }
        }

        internal void Delete()
        {
            Debug.Assert(Parent != null, "Trying to delete a node with no parent");
            if (Parent != null)
                Parent.Remove(this);
        }

        internal void Add(TmNode child)
        {
            if (_children == null)
                _children = new List<TmNode>();
            _children.Add(child);
            child.Parent = this;

            //FIXME - Remove side effects, This should be a call to update Age
            _ageInDays = -1;
            UpdateImageIndex(false);

            if (ThemeList != null)
                ThemeList.IsDirty = true;
            OnChildAdded(child, _children.Count - 1);
        }

        internal void UpdateImageIndex(bool recurse)
        {
            ImageKey = GetImageKey();
            if (recurse)
                foreach (var child in Children)
                    child.UpdateImageIndex(true);
        }

        private void Metadata_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ThemeList != null)
                ThemeList.IsDirty = true;
            // Do not sync the theme properties with the changed metadata path
            // User may have manually set those properties.
            // After Node/metadata is created, a sync should only be done by user.
        }

        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ThemeList != null)
                ThemeList.IsDirty = true; //so we know to save

            if (e.PropertyName == "Path")
            {
                if (IsTheme)
                {
                    // Changes to Data.Path may trigger exceptions
                    try
                    {
                        // Update Theme properties (icons, etc) for new data path
                        SyncThemeDataToPath();
                        // Update the metadata object (can't replace because it breaks property binding in forms)
                        // FIXME: May need to repair metadata paths on sub themes as well.
                        Metadata.UpdateWithDataSource(Data);
                        // Don't automatically sync metadata attributes.  It may overwrite manually edited Theme data.
                        // User can explicitly ask for a metadata sync if they want it.
                    }
                    catch (Exception)
                    {
                        // FIXME: Exception checking should be done in the view not model
                        // TODO: Figure out how to catch exceptions for form bound properties
                    }
                }
            }
            if (e.PropertyName == "Type")
                UpdateImageIndex(false);
            //Do nothing else for Datasource property
        }

        private void SyncThemeDataToPath()
        {
            Debug.Assert(IsThemeList || IsTheme, "Must be a themelist or theme to change data path");
            if (!IsTheme)
                return;

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

            string ext = Path.GetExtension(Data.Path).ToLower();
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

        private string GetImageKey()
        {
            string key = this._type.ToString();
            if (IsTheme || IsSubTheme)
                //key = key + GetDataTypeCode();
                //both themes and subthemes use the same icons
                key = "Theme" + GetDataTypeCode();
            if (IsReadOnly && IsThemeList)
                //only ThemeListlock is available, add images if this changes
                //key = key + "lock";
                key = "ThemeListlock";
            if (DaysSinceNewestPublication < Settings.Default.AgeInDays)
                key = key + "new";
            return key;
        }

        private string GetDataTypeCode()
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
                return "_wms"; //11

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

        internal TmNode Copy()
        {
            return this.Clone() as TmNode;;
        }

        /// <summary>
        /// Creates a new Theme List Node based on the contents of the current Category Node
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal TmNode CopyAsThemeList(string path)
        {
            Debug.Assert(this.IsCategory, "Should only be called on a Category Node");
            TmNode newNode = new TmNode(TmNodeType.ThemeList, this.Name, null, new ThemeData(path), this.Metadata.Clone() as Metadata, this.Description, null);
            ThemeListAuthor author = new ThemeListAuthor();
            author.Name = Environment.UserName;
            newNode.Author = author;
            foreach (var child in Children)
                newNode.Add(child.Copy());
            newNode.SaveAs(path);
            return newNode;
        }


        //FIXME - this is very confusing.  Clean up the code and the logic. It probably shouldn't be a property.
        // This is the age in days of the newest pub date for me and all my descendents
        // will get called by self when pub date changes, or by a child when it changes.
        private int DaysSinceNewestPublication
        {
            get
            {
                if (_ageInDays == -1)
                    _ageInDays = CalcDaysSinceNewestPublication();
                return _ageInDays;
            }
            set
            {
                // Call the accessor here since member variable may not be set.
                if (value <= DaysSinceNewestPublication) // I got younger, no checking required
                {
                    _ageInDays = value;
                    if (Parent != null)
                        Parent.DaysSinceNewestPublication = _ageInDays;
                }
                else
                {
                    if (_ageInDays < value && _ageInDays < CalcDaysSinceMyPublication())
                    {
                        // I may get older, but I need to check with all childern to see how much
                        int saveAge = _ageInDays;
                        _ageInDays = value < CalcDaysSinceMyPublication() ? value : CalcDaysSinceMyPublication();
                        int ageOfYoungestChild = CalcDaysSincePublicationOfNewestChild();
                        if (ageOfYoungestChild < _ageInDays)
                            _ageInDays = ageOfYoungestChild;
                        if (saveAge != _ageInDays && Parent != null)
                            Parent.DaysSinceNewestPublication = _ageInDays;
                    }
                }
                //FIXME this side effect should not be hidden here
                //FIXME - I should only call it if I got younger or older than the trigger point
                UpdateImageIndex(false);
            }
        }
        private int _ageInDays = -1;

        private int CalcDaysSinceMyPublication()
        {
            return (DateTime.Now - _pubDate).Days;
        }

        private int CalcDaysSincePublicationOfNewestChild()
        {
            if(_children == null)
                return CalcDaysSinceMyPublication();
            if (_children.Count == 0)
                return CalcDaysSinceMyPublication();
            return
                (from tmnode in Children
                 select tmnode.DaysSinceNewestPublication).Min();
        }

        private int CalcDaysSinceNewestPublication()
        {
            return Math.Min(CalcDaysSinceMyPublication(), CalcDaysSincePublicationOfNewestChild());
        }

        public XElement ToXElementWithoutChildren()
        {
            XElement xele = new XElement(
                 Enum.GetName(typeof(TmNodeType), Type).ToLower(),
                 IsValidThemeList ? new XAttribute("version", _dataStore.Version ?? "") : null,
                 new XAttribute("name", Name ?? ""),
                 (Tags != null) ? new XElement("tags", Tags) : null,
                 (Summary != null) ? new XElement("summary", Summary) : null,
                 (Description != null) ? new XElement("description", Description) : null,
                 (Type == TmNodeType.Theme) ? new XElement("pubdate", PubDate) : null,
                 HasAuthor ? Author.AsXElement : null,
                 IsCategory ? null : Data.ToXElement(),
                 HasMetadata ? Metadata.ToXElement() : null
                 );
            return xele;
        }

        public XElement ToXElement()
        {
            XElement xele = ToXElementWithoutChildren();
            xele.Add(
                  from tmnode in Children
                  select tmnode.ToXElement()
                 );
            return xele;
        }

        private TmNode ParentTheme(TmNode node)
        {
            if (Parent == null)
                return null;
            if (Parent.IsTheme)
                return Parent;
            if (Parent.IsSubTheme)
                return ParentTheme(this);
            return null;
        }

        public bool Matches(SearchParameters searchParameters)
        {
            if (searchParameters == null || searchParameters.Count < 1)
                return false;
            if (IsSubTheme)
                return false;
            if ((IsCategory || IsThemeList) && !searchParameters.SearchCategories)
                return false;
            if (IsTheme && !searchParameters.SearchThemes)
                return false;

            if (IsTheme)
            {
                bool subThemeMatch = Children.SelectMany(n => n.Recurse(node => node.Children)
                                  .Where(x => x.Matches2(searchParameters))).Count() > 0;
                return Matches2(searchParameters) || subThemeMatch;
            }
            else
                return Matches2(searchParameters);
        }

        private bool Matches2(SearchParameters searchParameters)
        {
            foreach (SearchOptions search in searchParameters)
            {
                bool lastSearch = (!searchParameters.MatchAll) ? true : searchParameters.IsLast(search);
                bool found;
                switch (search.SearchType)
                {
                    case SearchType.ThemeName:
                        found = MatchName(search); break;
                    case SearchType.ThemeSummary:
                        found = MatchSummary(search); break;
                    case SearchType.ThemeDescription:
                        found = MatchDescription(search); break;
                    case SearchType.ThemeTags:
                        found = MatchTags(search); break;
                    case SearchType.Theme:
                        found = MatchName(search) || MatchSummary(search) || MatchDescription(search) || MatchTags(search); break;
                    case SearchType.PubDate:
                        found = MatchDate(search); break;
                    case SearchType.Metadata:
                        found = MatchMeta(search); break; //FIXME - since this is in a loop, we may load metadata multiple times
                    default:
                        Debug.WriteLine("Invalid Search Condition");
                        found = false; break;
                }
                if (lastSearch && found)
                    return true;
                if (searchParameters.MatchAll && !found)
                    return false;
            }
            return false;
        }

        private bool MatchName(SearchOptions search)
        {
            return MatchText(search, Name);
        }

        private bool MatchDescription(SearchOptions search)
        {
            return MatchText(search, Description);
        }

        private bool MatchSummary(SearchOptions search)
        {
            return MatchText(search, Summary);
        }

        private bool MatchTags(SearchOptions search)
        {
            return MatchText(search, Tags);
        }

        private bool MatchText(SearchOptions search, string text)
        {
            bool found;
            if (search.FindAll)
            {
                found = true;
                foreach (string searchString in search.SearchWords)
                    if (!text.Contains(searchString, search.ComparisonMethod))
                        return false;
            }
            else //FindAny
            {
                found = false;
                foreach (string searchString in search.SearchWords)
                    if (text.Contains(searchString, search.ComparisonMethod))
                        return true;
            }
            return found;
        }

        private bool MatchDate(SearchOptions search)
        {
            int ageInDays = DaysSinceNewestPublication;
            if (search.HasMinDays && search.HasMaxDays)
                if (search.MinDaysSinceEdit < search.MaxDaysSinceEdit)
                    return ((search.MinDaysSinceEdit <= ageInDays)
                             && (ageInDays <= search.MaxDaysSinceEdit));
                else
                    return ((search.MinDaysSinceEdit <= ageInDays)
                             || (ageInDays <= search.MaxDaysSinceEdit));
            if (search.HasMinDays && search.MinDaysSinceEdit <= ageInDays)
                return true;
            if (search.HasMaxDays && ageInDays <= search.MaxDaysSinceEdit)
                return true;
            return false;
        }

        private bool MatchMeta(SearchOptions search)
        {
            if (!HasMetadata)
                return false;
            return Metadata.SearchContent(search);
        }

        public void Launch()
        {
            if (IsLaunchable)
                Process.Start(Data.Path);
        }

        public void Save()
        {
            if (IsThemeList && _status == ThemeListStatus.Dirty)
            {
                _status = ThemeListStatus.Saving;
                _dataStore.Save(this);
                _status = ThemeListStatus.Loaded;
            }
        }

        public void SaveAs(string path)
        {
            SaveAs(path, null);
        }

        // FIXME - work needs to be done on the UI to put this new node in the Themelist treeview
        private void SaveAs(string path, string version)
        {
            Debug.Assert(IsThemeList, "SaveAs... is only valid for a themelist");
            //Debug.Assert(HasData, "Theme List has no data object");
            // create backup of current settings in case save doesn't work.
            ThemeData oldData = (ThemeData)Data.Clone();
            TmNodeType oldKind = Type;
            Store oldDatastore = _dataStore;
            ThemeListStatus oldStatus = _status;

            string ext = Path.GetExtension(path);
            if (ext != null)
                ext = ext.ToLower();
            switch (ext)
            {
                case ".tmz":
                    throw new NotImplementedException();
                case ".tml":
                case ".xml":
                    _dataStore = XmlStore.CreateNew(path, version); break;
                case ".mdb":
                    _dataStore = MdbStore.CreateNew(path, version); break;
                default:
                    throw new ArgumentException(path + " is not a supported theme list file type.");
            }
            if (_dataStore == null)
            {
                _dataStore = oldDatastore;
                throw new ArgumentException("Unable to create a Theme List at " + path);
            }
            Data.Path = path;
            Data.Type = ext.Substring(1, 3);
            Data.Version = version;
            _type = TmNodeType.ThemeList;
            _status = ThemeListStatus.Dirty;

            try
            {
                Save();
            }
            catch (Exception)
            {
                Data = oldData;
                _type = oldKind;
                _dataStore = oldDatastore;
                _status = oldStatus;
                throw;
            }
            IsReadOnly = false;
        }

        // Build is not thread safe.  In a multi-threaded app, it must be called in side a lock.
        // Locking is the callers responsibility.
        public void Build()
        {
            if (!IsValidThemeList)
                throw new Exception("File is not a valid ThemeList");

            if (_status == ThemeListStatus.Initialized)
            {
                _status = ThemeListStatus.Loading;

                _dataStore.Build(this);
                //FIXME - Remove side effects, This should be a call to update Age
                _ageInDays = -1; // Needs to be reset based on children
                // call UpdateImageIndex on all descendents (they may not have been set correctly when loaded).
                UpdateImageIndex(true);

                _status = ThemeListStatus.Loaded;
            }
        }


        public TmNode BuildNodeFromId(int themeId)
        {
            if (!IsValidThemeList)
                throw new Exception("This is not a valid ThemeList");
            if (_dataStore is MdbStore)
                return ((MdbStore)_dataStore).BuildThemeFromId(themeId);
            else
                throw new Exception("This is not a MDB ThemeList");
        }

        public void Load(XElement xele)
        {
                Load(xele, true);
        }

        public void Load(XElement xele, bool recurse)
        {
            if (xele == null)
                throw new ArgumentNullException("xele");

            //Kind was already set when node was created.  Set all other properties
            Name = (string)xele.Attribute("name");
            XElement data;
            data = xele.Element("tags");
            Tags = data == null ? null : data.Value;
            data = xele.Element("summary");
            Summary = data == null ? null : data.Value;
            data = xele.Element("description");
            Description = data == null ? null : data.Value;
            data = xele.Element("pubdate");
            if (data != null)
            {
                DateTime temp;
                if (DateTime.TryParse(data.Value, out temp))
                    PubDate = temp;
            }

            data = xele.Element("data");
            if (data != null)
            {
                // Ignore the path in the data element for themelists.
                if (IsThemeList && !string.IsNullOrEmpty(Data.Path))
                    data.Value = Data.Path;
                Data = ThemeData.Load(data);
            }

            data = xele.Element("metadata");
            if (data != null)
                Metadata = Metadata.FromXElement(data);

            data = xele.Element("author");
            if (data != null)
                Author = ThemeListAuthor.Load(data);

            if (recurse)
            {
                TmNode childNode;

                string category = Enum.GetName(typeof(TmNodeType), TmNodeType.Category).ToLower();
                foreach (XElement xnode in xele.Elements(category))
                {
                    childNode = new TmNode(TmNodeType.Category);
                    Add(childNode);
                    childNode.Load(xnode);
                }

                string theme = Enum.GetName(typeof(TmNodeType), TmNodeType.Theme).ToLower();
                foreach (XElement xnode in xele.Elements(theme))
                {
                    childNode = new TmNode(TmNodeType.Theme);
                    Add(childNode);
                    childNode.Load(xnode);
                }
            }
        }

        private Store TryToGetDataStore()
        {
            if (_type != TmNodeType.ThemeList)
                return null;
            
            // If we return from this routine prematurely, then we do not have a valid datastore
            _status = ThemeListStatus.Created;

            if (!HasData)
                return null;

            //if (string.IsNullOrEmpty(Data.Path))
            //    return null;

            Store datastore;
            string ext = Path.GetExtension(Data.Path);
            if (ext != null)
                ext = ext.ToLower();
            switch (ext)
            {
                case ".tmz":
                    throw new NotImplementedException();
                case ".tml":
                case ".xml":
                    datastore = new XmlStore(Data.Path); break;
                case ".mdb":
                    datastore = new MdbStore(Data.Path); break;
                default:
                    return null;
            }
            if (datastore == null)
                return null;
            if (!datastore.IsThemeList)
                return null;

            if (datastore is MdbStore)
                IsReadOnly = true;
            else
                IsReadOnly = datastore.ReadOnly;

            _status = ThemeListStatus.Initialized;

            return datastore;
        }

        public override string ToString()
        {
            return FullCategoryPath();
        }

        //FIXME - Use a Path Separator variable.
        private string FullCategoryPath()
        {
            if (Parent == null)
                return Name;
            else
                return Parent.FullCategoryPath() + "\\" + Name;
        }

        public string CategoryPath()
        {
            string[] path = FullCategoryPath().Split('\\');
            string results = "";
            for (int i = 1; i < path.Length - 1; i++)
                if (results == "")
                    results = path[i];
                else
                    results = results + "\\" + path[i];
            return results;
        }

        public void ReloadTheme()
        {
            ReloadTheme(false);
        }

        public void ReloadTheme(bool recurse)
        {
            if (IsTheme)
            {
                SyncThemeDataToPath();
                Metadata.UpdateWithDataSource(Data);
                // Don't automatically sync metadata attributes.  It may overwrite manually edited Theme data.
                // User can explicitly ask for a metadata sync if they want it.
            }
            if (recurse)
                foreach (TmNode child in Children)
                    child.ReloadTheme(true);
        }

        internal void UpdateTree()
        {
            OnReloadNode();
        }

        public void SyncWithMetadata(bool recurse)
        {
            var info = Metadata.GetGeneralInfo();
            Tags = info.Tags ?? "";
            Summary = info.Summary ?? "";
            Description = info.Description ?? "";
            if (info.PublicationDate.HasValue)
                PubDate = info.PublicationDate.Value;
            else
            {
                if (!File.Exists(Data.Path))
                {
                    PubDate = new DateTime(1900, 1, 1);
                }
                else
                    PubDate = File.GetLastWriteTime(Data.Path);
            }
            if (recurse)
                foreach (TmNode child in Children)
                    child.SyncWithMetadata(true);
        }

        #region ICloneable Members

        public object Clone()
        {
            // reference fields which need to be considered when cloning.
            //_children, _data, _datastore, _metadata, _parent, _themeList
            //it will be simpler to serialize and re-serialize (as is being done to
            //support the clipboard
            //Cloned items are orphans until they are put into a tree.
            var mem = new MemoryStream();
            var bin = new BinaryFormatter();
            try
            {
                bin.Serialize(mem, this);
                mem.Position = 0;
                return bin.Deserialize(mem);
            }
            catch (Exception ex)
            {
                Debug.Print("Your object cannot be (re)serialized. The reason is: " + ex);
                return null;
            }
        }

        #endregion

        #region events
        public void SuspendUpdates()
        {
            _issueUpdates = false;
            foreach (var child in Children)
                child.SuspendUpdates();
        }

        public void ResumeUpdates()
        {
            _issueUpdates = true;
            foreach (var child in Children)
                child.ResumeUpdates();
        }


        #region INotifyPropertyChanged
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handle = PropertyChanged;
            if (handle != null && _issueUpdates)
                handle(this, new PropertyChangedEventArgs(property));
        }
        
        #endregion

        [field: NonSerializedAttribute()]
        public event TmNodeEventHandler ChildAdded;

        [field: NonSerializedAttribute()]
        public event TmNodeEventHandler ChildRemoved;

        [field: NonSerializedAttribute()]
        public event EventHandler ReloadNode;

        protected void OnChildAdded(TmNode node, int index)
        {
            TmNodeEventHandler handle = ChildAdded;
            if (handle != null && _issueUpdates)
                handle(this, new TmNodeEventArgs { Index = index, Node = node });
        }

        protected void OnChildRemoved(TmNode node)
        {
            TmNodeEventHandler handle = ChildRemoved;
            if (handle != null && _issueUpdates)
                handle(this, new TmNodeEventArgs { Index = -1, Node = node });
        }

        protected void OnReloadNode()
        {
            EventHandler handle = ReloadNode;
            if (handle != null && _issueUpdates)
                handle(this, new EventArgs());
        }
        #endregion

    }

    internal delegate void TmNodeEventHandler(object sender, TmNodeEventArgs e);

    [Serializable]
    class TmNodeEventArgs : EventArgs
    {
        public int Index { get; set; }
        public TmNode Node { get; set; }
    }


}
