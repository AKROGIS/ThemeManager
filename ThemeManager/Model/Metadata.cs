using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Properties;
using NPS.AKRO.ThemeManager.ArcGIS;
using System.ComponentModel;

namespace NPS.AKRO.ThemeManager.Model
{
    struct GeneralInfo
    {
        string Description;
        DateTime? PublicationDate;
        string Summary;
        string Tags;
    }

    class MetadataDisplayException : Exception
    {
        internal MetadataDisplayException() { }
        internal MetadataDisplayException(string message) : base(message) { }
        internal MetadataDisplayException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Defines the meaning of the string in Metadata.Path and
    /// how it is used to find the metadata content.
    /// See MetadataFormat for how to interpret the content.
    /// </summary>
    enum MetadataType
    {
        /// <summary>
        /// The meaning of the text in Path is unknown and undefined
        /// </summary>
        Undefined,
        /// <summary>
        /// The metadata content is stored directly in the Path property (unusual)
        /// </summary>
        Inline,
        /// <summary>
        /// A file system path to a file containing metadata (usually in XML format)
        /// </summary>
        FilePath,
        /// <summary>
        /// A data source path (ArcGIS is used to get the metadata content from the data source)
        /// </summary>
        EsriDataPath,
        /// <summary>
        /// Uniform Resource Locator to a metadata resource (assumed to return Html)
        /// </summary>
        Url
    }

    /// <summary>
    /// Defines how to interpret the metadata content.
    /// See MetadataType for how to obtain the content from Path.
    /// </summary>
    enum MetadataFormat
    {
        /// <summary>
        /// The format of the metadata content is unknown and undefined
        /// </summary>
        Undefined,
        /// <summary>
        /// The metadata content is XML and can be styled for display (typical)
        /// Note XML may come in many different styles/schemas (FGDC, ISO, ArcGIS)
        /// and a schema may come in various different versions.
        /// It is up to the stylesheets to differentiate by schema/version as necessary
        /// </summary>
        Xml,
        /// <summary>
        /// The metadata content is HTML that is already formatted for display
        /// </summary>
        Html,
        /// <summary>
        /// The metadata content is plain text and will be displayed as is
        /// </summary>
        Text
    }

    enum MetadataState
    {
        Initialized,
        Loading,
        Loaded,
        Invalid
    }

    [Serializable]
    class Metadata : ICloneable, INotifyPropertyChanged
    {
        #region  Class Fields (Private)

        private static Dictionary<string, string> _cache = new Dictionary<string, string>();

        #endregion


        #region  Class Methods (Public)

        // Called by ThemeBuilder.cs line 83 (data added to theme list)
        internal static Metadata Find(ThemeData data)
        {
            if (data == null)
                return null;

            meta myProps = ExpectedMetadataProperties(data);
            Metadata metadata = new Metadata(myProps.path, myProps.type, myProps.format);
            if (metadata != null && metadata.Validate())
                return metadata;
            return null;
        }

        //Called by TmNode.cs line 1133 (building object from themelist XML)
        internal static Metadata Load(XElement xele)
        {
            if (xele == null)
                throw new ArgumentNullException("xele");
            if (xele.Name != "metadata")
                throw new ArgumentException("Invalid Xelement");
            var data = new Metadata(
                xele.Value,
                (MetadataType)Enum.Parse(typeof(MetadataType), (string)xele.Attribute("type")),
                (MetadataFormat)Enum.Parse(typeof(MetadataFormat), (string)xele.Attribute("format")),
                (string)xele.Attribute("version"),
                (string)xele.Attribute("schema")
                );
            return data;
        }

        #endregion


        #region  Class Methods (Private)

        private static meta ExpectedMetadataProperties(ThemeData data)
        {
            meta newMeta = new meta()
            {
                path=null,
                type = MetadataType.Undefined,
                format= MetadataFormat.Undefined
            };

            if (data == null)
                return newMeta;

            newMeta.type = MetadataType.FilePath;
            newMeta.format = MetadataFormat.Xml;

            //general file based metadata
            if (data.Path != null && File.Exists(data.Path + ".xml"))
            {
                newMeta.path = data.Path + ".xml";
                return newMeta;
            }

            if (data.DataSource != null && File.Exists(data.DataSource + ".xml"))
            {
                newMeta.path = data.DataSource + ".xml";
                return newMeta;
            }

            //grids & tins
            if (data.DataSource != null
                //&& data.WorkspacePath != null
                && System.IO.Directory.Exists(data.DataSource))
            {
                string metapath = System.IO.Path.Combine(data.DataSource, "metadata.xml");
                if (File.Exists(metapath))
                {
                    newMeta.path = metapath;
                    return newMeta;
                }
            }

            //shapefile
            if (data.IsShapefile)
            {
                string metapath = data.DataSource + ".shp.xml";
                if (File.Exists(metapath))
                {
                    newMeta.path = metapath;
                    return newMeta;
                }
            }

            //coverages
            if (data.IsCoverage && data.WorkspacePath != null && data.Container != null)
            {
                string coverageDir = System.IO.Path.Combine(data.WorkspacePath, data.Container);
                if (System.IO.Directory.Exists(coverageDir))
                {
                    string metapath = System.IO.Path.Combine(coverageDir, "metadata.xml");
                    if (File.Exists(metapath))
                    {
                        newMeta.path = metapath;
                        return newMeta;
                    }
                }
            }

            //CAD
            if (data.IsCad && data.WorkspacePath != null && data.Container != null)
            {
                string cadFile = System.IO.Path.Combine(data.WorkspacePath, data.Container);
                if (System.IO.File.Exists(cadFile))
                {
                    string metapath = cadFile + ".xml";
                    if (File.Exists(metapath))
                    {
                        newMeta.path = metapath;
                        return newMeta;
                    }
                }
            }

            newMeta.type = MetadataType.EsriDataPath;

            if (data.IsInGeodatabase && !data.IsLayerFile)
            {
                newMeta.path = data.DataSource;
                return newMeta;
            }
            if (data.IsLayerFile && !data.IsGroupLayerFile)
            {
                newMeta.path = data.DataSource;
                return newMeta;
            }

            //FIXME - does not work for web services ???
            newMeta.type = MetadataType.Undefined;
            newMeta.format = MetadataFormat.Undefined;
            return newMeta;
        }

        private static bool Match(string haystack, IEnumerable<string> needles, bool findAll, StringComparison comparisonMethod)
        {
            bool found;
            if (findAll)
            {
                found = true;
                if (needles.Any(needle => !haystack.Contains(needle, comparisonMethod)))
                {
                    return false;
                }
            }
            else //FindAny
            {
                found = false;
                if (needles.Any(needle => haystack.Contains(needle, comparisonMethod)))
                {
                    return true;
                }
            }
            return found;
        }

        #endregion


        #region  Constructors

        // Called by TmNode.cs line 82 (TmNode default constructor)
        internal Metadata()
            : this(null, MetadataType.Undefined, MetadataFormat.Undefined, null, null)
        {
        }

        private Metadata(string path)
            : this(path, MetadataType.Undefined, MetadataFormat.Undefined, null, null)
        {
        }

        // Called by MdbStore.cs lines 104-414 (building object form old DB format)
        internal Metadata(string path, MetadataType type, MetadataFormat format)
            : this(path, type, format, null, null)
        {
        }

        private Metadata(string path, MetadataType type, MetadataFormat format, string version, string schema)
        {
            _path = path;
            Type = type;
            Format = format;
            Version = version;
            Schema = schema;
            State = MetadataState.Initialized;
        }

        #endregion


        #region  Instance Fields (All Private; Should only be accessed by Contructors or Properties)

        private string _description;
        private bool _metadataHasBeenScanned;
        private string _path;
        private string _pubdate;
        private string _summary;
        private string _tags;

        #endregion


        #region  Public Properties

        // Called by TmNode.cs line 1294 (setting node properties) AdminReports.cs line 217 (ListMetadataProblems)
        // Call from AdminReports should be replaced with a call to load/validate
        // Call from TMNode should be replaced with a call to GetInfo() -> struct(pubdate?, tags, summary, description)
        internal string Description
        {
            get
            {
                if (!_metadataHasBeenScanned)
                    ScanMetadata();
                return _description;
            }
        }

        // Called by AdminReports.cs line 218 (ListMetadataProblems)
        internal string ErrorMessage { get; private set; }

        // Called by TmNode.cs line 1301 and 1317 (setting node properties)
        // Calls from TMNode should be replaced with a call to GetInfo() -> struct(pubdate?, tags, summary, description)
        internal bool HasPubDate
        {
            get
            {
                if (!_metadataHasBeenScanned)
                    ScanMetadata();
                return (_pubdate != null);
            }
        }

        // Called by TmNode.cs line 527 (Metadata_PropertyChanged, tiggerd when Path changes)
        // Remove this call from the caller.  They should not call this (it is pointless) until the user requests a sync (GetInfo)
        internal bool IsValid { get; set; }

        // Called in lots of places
        // Verify that form binding is not a form of setting the path (how else does the user change the metadata path?)
        internal string Path
        {
            get
            {
                return _path;
            }
            private set
            {
                if (_path != value)
                {
                    Reset();
                    _path = value;
                    Type = MetadataType.Undefined;
                    OnPropertyChanged("Path");
                    //FIXME - this is broken.
                    // changing the path, or creating a new metadata object with the same path
                    // does not revalidate - Need to cache whole metadata objects.
                }
            }
        }

        /// <summary>
        /// Will throw an exception if there is no pubdate
        /// Client should use HasPubDate first.
        /// </summary>
        // Called by TmNode.cs line 1319 (setting node properties)
        // Call from TMNode should be replaced with a call to GetInfo() -> struct(pubdate?, tags, summary, description)
        internal DateTime PubDate
        {
            get
            {
                return DateTime.Parse(_pubdate);
            }
        }

        // Called by TmNode.cs line 1286 (setting node properties)
        // Call from TMNode should be replaced with a call to GetInfo() -> struct(pubdate?, tags, summary, description)
        internal string Summary
        {
            get
            {
                if (!_metadataHasBeenScanned)
                    ScanMetadata();
                return _summary;
            }
        }

        // Called by TmNode.cs line 1278 (setting node properties)
        // Call from TMNode should be replaced with a call to GetInfo() -> struct(pubdate?, tags, summary, description)
        internal string Tags
        {
            get
            {
                if (!_metadataHasBeenScanned)
                    ScanMetadata();
                return _tags;
            }
        }

        #endregion


        #region  Interface Implementations
        #region IClonable Interface

        // Called by MainForm, TmNode, and others to copy a Metadata object
        public object Clone()
        {
            var obj = (Metadata)MemberwiseClone();
            return obj;
        }

        #endregion

        #region INotifyPropertyChanged Interface

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handle = PropertyChanged;
            if (handle != null)
                handle(this, new PropertyChangedEventArgs(property));
        }

        #endregion
        #endregion


        #region  Public Methods

        // Add Async internal MetaInfo GetInfo() -> struct MetaInfo(pubdate?, tags, summary, description)
        //   Called on theme's "Sync with Metadata" to get select XML Tags for storing in the theme's properties
        // Add Async internal void LoadContentAndValidate()
        //  Called by user who wants to check the ErrorMessage without calling Display(), GetInfo() or Match()
        //  called internally by Display(), GetInfo(), and Match()
        // Make Display() and Match() Async

        //FIXME - return more meaningful exception messages or create web pages on the fly

        // Called by MainForm.cs line 1077 (display in metadata tab (web browser))
        // Caller can call Load/Validate first, and check ErrorMessage Property before calling display
        internal void Display(WebBrowser webBrowser, StyleSheet styleSheet)
        {
            Debug.Assert(webBrowser != null, "The WebBrowser control is null");
            if (webBrowser == null)
                throw new ArgumentNullException("webBrowser");

            // TODO: replace with LoadContentAndValidate() throw if ErrorMessage is non NULL
            string xmlString = LoadAsText();
            if (!IsValid)
                throw new MetadataDisplayException("Metadata is not valid"); //FIXME: return ErrorMessage

            // Exception message for Type == Undefined
            //  Unable to obtain the metadata content because Theme Manager doesn't know the meaning of {Path}
            // Exception message for Format == Undefined
            //  Theme Manager is doesn't know the format of the metadata content, so it is presented as plain text below



            try
            {

                if (Type == MetadataType.Url)
                    webBrowser.Url = new Uri(Path);
                else if (Format == MetadataFormat.Xml && styleSheet != null)
                {
                    // Do this in two steps to avoid sending partial processing to the webBrowser
                    string html = styleSheet.TransformText(xmlString);
                    webBrowser.DocumentText = html;
                }
                //TODO: Ensure that all permutations of Format and Type have been considered
                else
                    webBrowser.DocumentText = xmlString;
            }
            catch (Exception ex)
            {
                throw new MetadataDisplayException(ex.Message, ex);
            }
        }

        internal GeneralInfo GetGeneralInfo()
        {
            string description;
            DateTime? publicationDate = null;
            string summary;
            string tags;

            return new GeneralInfo {
                description,
                publicationDate,
                summary,
                tags
            };
        }

        // Called by TmNode.cs line 987 (advanced search option)
        internal bool Match(SearchOptions search)
        {
            if (search == null)
                return false;

            Debug.Assert(!string.IsNullOrEmpty(Path), "This Metadata object does not have a path");
            Debug.Assert(search.SearchType == SearchType.Metadata, "The search parameters are not appropriate for metadata");

            if (string.IsNullOrEmpty(search.XmlElement))
            {
                //to search the whole document, we don't need to parse it as XML
                string content = LoadAsText();
                return content == null ? false:  Match(content, search.SearchWords, search.FindAll, search.ComparisonMethod);
            }
            XDocument xDoc = LoadAsXDoc();
            if (xDoc == null)
                return false;
            return xDoc.Descendants(search.XmlElement)
                .Select(element => element.Value)
                .Where(value => !string.IsNullOrEmpty(value))
                .Any(value => Match(value, search.SearchWords, search.FindAll, search.ComparisonMethod));
        }

        // Called by TmNode.cs line 1159 .. -> MainForm.designer.cs line 1058 (preload all metadata in background)
        // Removing this "feature"; Metadata will only loaded on demand.  Advanced search may require loading all metadata (user can be warned)
        internal void PreloadAsText()
        {
            if (Settings.Default.KeepMetaDataInMemory)
                if (!string.IsNullOrEmpty(Path))
                    _cache[Path] = LoadAsText();
        }

        // Called from TmNode.cs line 548 (data path changed) and line 1245 (reload theme)
        // Ensure this is a low cost synchronous method (the data path may or may not change); This is not a user request to Sync
        internal void Repair(ThemeData data)
        {
            //FIXME - this is a redundant load/validate/scan going on.
            meta myNewProps = Metadata.ExpectedMetadataProperties(data);
            if (myNewProps.path == null)
                return;  //exception or error ???
            Type = myNewProps.type;
            Format = myNewProps.format;
            Path = myNewProps.path;  //will revalidation only if path changes
            Validate();
        }

        // Called by TmNode.cs line 845 (write object to themelist XML)
        internal XElement ToXElement()
        {
            return new XElement("metadata",
                                new XAttribute("type", Enum.GetName(typeof(MetadataType), Type)),
                                new XAttribute("format", Enum.GetName(typeof(MetadataFormat), Format)),
                                new XAttribute("version", Version ?? ""),
                                new XAttribute("schema", Schema ?? ""),
                                Path
                );
        }

        #endregion


        #region  Private Enums/Structs/Classes

        private struct meta
        {
            internal string path;
            internal MetadataType type;
            internal MetadataFormat format;
        }

        #endregion


        #region  Private Properties

        private MetadataFormat Format { get; set; }
        private bool HasBeenValidated {get; set;}
        private string Schema { get; set; } //FIXME - use or toss
        private MetadataState State { get; set; }
        private MetadataType Type { get; set; }
        private string Version { get; set; } //FIXME - use or toss

        #endregion


        #region  Private Methods

        //TODO: Replace LoadAsText() LoadAsXDoc() and Validate(), etc with: LoadContentAndValidate()
        // cache results on class, to speed up multiple requests, avoid duplication, and avoid cloning large blocks of text
        // Validation is in two parts: 1) Validate Type (requires loading even URLs) and 2) Validate Format (may require parsing)
        // Validation can short circuit if Content is non null.  If content is null reload will be retried
        // Errors in Loading/Validation will be stored in ErrorMessages for display to the user.
        //These changes will improve display robustness, as well as simplify the code.  It may result in more
        //time spent loading or retrying, but always (and only) when the user requests it.

        /// <summary>
        /// Converts a date in YYYY, YYYYMM, or YYYYMMDD format to YYYY-MM-DD
        /// </summary>
        private string NormalizeFgdcDateString(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;
            if (dateString.Length == 4)
                return dateString + "-01-01";
            if (dateString.Length == 6)
                return dateString.Substring(0, 4) + "-" + dateString.Substring(4, 2) + "-01";
            if (dateString.Length == 8)
                return dateString.Substring(0, 4) + "-" + dateString.Substring(4, 2) + "-" + dateString.Substring(6, 2);
            return null;
        }

        //!! side effect of LoadAsText() is that Path, Type and IsValid are validated (for Display()).
        //!! side effect of LoadAsText() is that Format, IsValid are validated (for LoadAsXDoc()).
        // This will not throw any exceptions, rather it returns null, and sets ErrorMessage
        private string LoadAsText()
        {
            if (string.IsNullOrEmpty(Path))
                return null;
            if (_cache.ContainsKey(Path))
                return _cache[Path];

            string contents = null;

            Trace.TraceInformation("{0}: Start of Metadata.LoadASText({1})", DateTime.Now, Path); Stopwatch time = Stopwatch.StartNew();
            // If another thread is loading the text, wait for it to finish.
            // because of side effects, including changing state of CachedContentsAsText,
            // we lock the whole routine.
            lock (this)
            {
                //if (!HasBeenValidated)
                //Validate is now fast enough that we will always do it.
                //eliminates false positives
                    IsValid = Validate();
                if (IsValid)
                {
                    try
                    {
                        if (Type == MetadataType.Inline)
                            contents = Path;
                        if (Type == MetadataType.FilePath)
                            contents = File.ReadAllText(Path);
                        if (Type == MetadataType.EsriDataPath)
                            contents = EsriMetadata.GetContentsAsXml(Path);
                        if (Type == MetadataType.Url)
                            contents = Path;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = ex.Message;
                        Debug.Print("Exception thrown trying to load Metadata.\n" + ex);
                        contents = null;
                    }
                }
                if (string.IsNullOrEmpty(contents))
                {
                    IsValid = false;
                    contents = null;
                    Format = MetadataFormat.Undefined;
                }
                else
                    if (Settings.Default.KeepMetaDataInMemory)
                        if (!string.IsNullOrEmpty(Path))
                            _cache[Path] = LoadAsText();
            }
            time.Stop(); Trace.TraceInformation("{0}: End of Metadata.LoadASText() - Elapsed Time: {1}", DateTime.Now, time.Elapsed);

            return contents;
        }

        // !!! side effect is that IsValid and Format may be changed if found to be incorrect.
        private XDocument LoadAsXDoc()
        {
            // LoadAsText() will have the side effect of validating the Format, IsValid, ... so we do it first.
            string xmlString = LoadAsText();

            if (Format != MetadataFormat.Xml && Format != MetadataFormat.Undefined)
                return null;

            XDocument contents = null;
            if (IsValid)
            {
                try
                {
                    contents = XDocument.Parse(xmlString);
                    Format = MetadataFormat.Xml;
                }
                catch (XmlException ex)
                {
                    ErrorMessage = ex.Message;
                    IsValid = false;
                    contents = null;
                    Format = MetadataFormat.Undefined;
                }
            }
            return contents;
        }

        private void Reset()
        {
            HasBeenValidated = false;
            _metadataHasBeenScanned = false;
            if (!string.IsNullOrEmpty(Path) && _cache.ContainsKey(Path))
                _cache.Remove(Path);
        }

        private void ScanMetadata()
        {
            XDocument xDoc = LoadAsXDoc();
            if (xDoc == null)
            {
                _tags = null;
                _summary = null;
                _description = null;
                _pubdate = null;
            }
            else
            {
                // ArcGIS 9.3 XML metadata will have only FGDC elements
                // ArcGIS 10 XML metadata may have both ArcGIS and FGDC elements

                // Description(Abstract) - FGDC: /metadata/idinfo/descript/abstract
                // Description(Abstract) - ArcGIS 10: /metadata/dataIdInfo/idAbs
                _description = xDoc.Descendants("abstract")
                    .Concat(xDoc.Descendants("idAbs"))
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) &&
                                    !value.StartsWith("REQUIRED:"))
                    .FirstOrDefault();
                // Summary (Purpose) - FGDC: /metadata/idinfo/descript/purpose
                // Summary (Purpose) - ArcGIS 10: /metadata/dataIdInfo/idPurp
                _summary = xDoc.Descendants("purpose")
                    .Concat(xDoc.Descendants("idPurp"))
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) &&
                                    !value.StartsWith("REQUIRED:"))
                    .FirstOrDefault();
                // Tags (keywords) - FGDC: metadata/idinfo/keywords/*/*key (where * = theme, place, strat, temp); each keyword in a separate element
                // Tags (keywords) - ArcGIS 10: metadata/dataIdInfo/*Keys/keyword (where * = desc, other, place, temp, disc, strat, search, theme) *Keys and keyword may appear multiple times.
                _tags = xDoc.Descendants("keyword")
                    .Concat(xDoc.Descendants("themekey"))
                    .Concat(xDoc.Descendants("placekey"))
                    .Concat(xDoc.Descendants("stratkey"))
                    .Concat(xDoc.Descendants("tempkey"))
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) &&
                                    !value.StartsWith("00") &&           // keyword in otherKeys for all new ArcGIS metadata
                                    !value.StartsWith("REQUIRED:"))
                    .Distinct().Concat(", ");
                // PubDate - FGDC: /metadata/idinfo/citation/citeinfo/pubdate
                // PubDate - ArcGIS 10: /metadata/dataIdInfo/idCitation/date/pubDate
                _pubdate = xDoc.Descendants("pubdate")
                    .Concat(xDoc.Descendants("pubDate"))
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) &&
                                    !value.StartsWith("REQUIRED:"))
                    .FirstOrDefault();
                _pubdate = NormalizeFgdcDateString(_pubdate);
                DateTime date;
                if (!DateTime.TryParse(_pubdate, out date))
                    _pubdate = null;
            }
            _metadataHasBeenScanned = true;
        }

        /// <summary>
        /// Returns true if this is a Valid metadata object
        /// </summary>
        /// <remarks>
        /// If the type is unknown, all valid types will be checked to determine the type
        /// This is generally very fast, and can be called multiple times.
        /// The exception is ESRI metadata which will do a license load, once per process
        /// and will do some slow arcObjects calls if the metadata path has not been cached
        /// </remarks>
        /// <returns>True if this is a Valid metadata object, False otherwise</returns>
        private bool Validate()
        {
            HasBeenValidated = true;
            if (string.IsNullOrEmpty(Path))
            {
                Type = MetadataType.Undefined;
                return false;
            }

            // Validate the Type
            // TODO: URL, and FilePath should also be loaded to do proper validation
            try
            {
                switch (Type)
                {
                    case MetadataType.FilePath:
                        return File.Exists(Path);
                    case MetadataType.Url:
                        //if Path is not a valid URI, an exception will be thrown
                        //FIXME - Validate that that the URI can be found ??
                        //   (careful - URI may be valid, but temporarily not available)
                        var uri = new Uri(Path);
                        if (uri.IsFile)
                        {
                            Type = MetadataType.FilePath;
                            return File.Exists(uri.LocalPath);
                        }
                        else
                            return true;
                    case MetadataType.EsriDataPath:
                        // See if ArcObjects can load the metadata or throw an exception trying
                        EsriMetadata.GetContentsAsXml(Path);
                        return true;
                    case MetadataType.Inline:
                        //FIXME - Validate the contents against MetadataFormat ???
                        return !string.IsNullOrEmpty(Path);
                    case MetadataType.Undefined:
                    default:
                        // MetadataType.Undefined (or some other anomaly
                        // Try all each type - first valid type wins, so order matters.
                        Type = MetadataType.FilePath;
                        if (Validate())
                            return true;
                        Type = MetadataType.Url;
                        if (Validate())
                            return true;
                        Type = MetadataType.EsriDataPath;
                        if (Validate())
                            return true;
                        Type = MetadataType.Inline;
                        if (Validate())
                            return true;
                        Type = MetadataType.Undefined;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Debug.Print("Exception validating metadata: " + ex);
            }
            return false;

            //FIXME: Validate the Format; will require loading
        }

        #endregion
    }
}
