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
    class MetadataDisplayException : Exception
    {
        internal MetadataDisplayException() { }
        internal MetadataDisplayException(string message) : base(message) { }
        internal MetadataDisplayException(string message, Exception inner) : base(message, inner) { }
    }

    enum MetadataType
    {
        /// <summary>
        /// The type is unknown or undefined
        /// </summary>
        Undefined,
        /// <summary>
        /// Metadata is stored directly in this metadata object
        /// </summary>
        Inline,
        /// <summary>
        /// Files system path to a standalone metadata file (usually in XML format)
        /// </summary>
        FilePath,
        /// <summary>
        /// ArcCatalog path to a data object (ArcCatalog is queried for the metadata) 
        /// </summary>
        EsriDataPath,
        /// <summary>
        /// Uniform Resoure Locator to a metadata file (usually in Html format) 
        /// </summary>
        Url
    }

    enum MetadataFormat
    {
        Undefined,
        Xml,
        Html,
        Text
        //CSDGM
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
        #region  Public Properties

        public string Path
        {
            get
            {
                return _path;
            }
            set
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
        private string _path;

        internal string ErrorMessage { get; private set; }

        #endregion

        #region  Private Properties

        private static Dictionary<string, string> _cache = new Dictionary<string, string>();

        internal bool IsValid { get; set; }
        private bool HasBeenValidated {get; set;}
        private MetadataFormat Format { get; set; }
        private MetadataType Type { get; set; }
        private string Version { get; set; } //FIXME - use or toss
        private string Schema { get; set; } //FIXME - use or toss
        private MetadataState State { get; set; }

        #endregion

        #region Public Methods

        internal Metadata()
            : this(null, MetadataType.Undefined, MetadataFormat.Undefined, null, null)
        {
        }

        internal Metadata(string path)
            : this(path, MetadataType.Undefined, MetadataFormat.Undefined, null, null)
        {
        }

        internal Metadata(string path, MetadataType type, MetadataFormat format)
            : this(path, type, format, null, null)
        {
        }

        internal Metadata(string path, MetadataType type, MetadataFormat format, string version, string schema)
        {
            _path = path;
            Type = type;
            Format = format;
            Version = version;
            Schema = schema;
            State = MetadataState.Initialized;
        }

        public object Clone()
        {
            var obj = (Metadata)MemberwiseClone();
            return obj;
        }

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

        internal struct meta
        {
            internal string path;
            internal MetadataType type;
            internal MetadataFormat format;
        }

        internal static meta ExpectedMetadataProperties(ThemeData data)
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

        internal void PreloadAsText()
        {
            if (Settings.Default.KeepMetaDataInMemory)
                if (!string.IsNullOrEmpty(Path))
                    _cache[Path] = LoadAsText();
        }

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

        //FIXME - return more meaningful exception messages
        //FIXME - do more checking of types and formats
        internal void Display(WebBrowser webBrowser, StyleSheet styleSheet)
        {
            Debug.Assert(webBrowser != null, "The WebBrowser control is null");
            if (webBrowser == null)
                throw new ArgumentNullException("webBrowser");
            string xmlString = LoadAsText();
            //side effect of loading is that Path, Type and IsValid are validated, so we load first.
            if (!IsValid)
                throw new MetadataDisplayException("Metadata is not valid"); //In what way???

            try
            {
                if (Type == MetadataType.Url)
                    webBrowser.Url = new Uri(Path);
                else
                    if (styleSheet != null)
                        webBrowser.DocumentText = styleSheet.XformText(xmlString); //check Format == xml ??
                    else
                        webBrowser.DocumentText = xmlString;
            }
            catch (Exception ex)
            {
                throw new MetadataDisplayException(ex.Message, ex);
            }
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
        internal bool Validate()
        {
            HasBeenValidated = true;
            if (string.IsNullOrEmpty(Path))
                return false;

            // try to load the metadata, if I can it is valid (return true),
            // if I get any exceptions it is invalid
            try
            {
                switch (Type)
                {
                    case MetadataType.FilePath:
                        //FIXME - Validate the contents against MetadataFormat ???
                        return File.Exists(Path);
                    case MetadataType.Url:
                        //if Path is not a valid URI, an exception will be thrown
                        //FIXME - Validate that that the URI can be found ??
                        //   (careful - URI may be valid, but temporarily not available)
                        var uri = new Uri(Path);
                        if (uri.IsFile)
                            return File.Exists(uri.LocalPath);
                        else
                            return true;
                    case MetadataType.EsriDataPath:
                        // See if ArcObjects can load the metadata or throw an exception trying
                        EsriMetadata.GetContentsAsXml(Path);
                        return true;
                    case MetadataType.Inline:
                        //FIXME - Validate the contents against MetadataFormat ???
                        return !string.IsNullOrEmpty(Path);
                    default:
                    case MetadataType.Undefined:
                        //Try all each type - first valid type wins, so order matters.
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
        }

        #endregion

        #region Private Methods

        internal void Reset()
        {
            HasBeenValidated = false;
            _metadataHasBeenScanned = false;
            if (!string.IsNullOrEmpty(Path) && _cache.ContainsKey(Path))
                _cache.Remove(Path);
        }

        //!! side effect of LoadAsText() is that Path, Type and IsValid are validated (for Display()).
        //!! side effect of LoadAsText() is that Format, IsValid are validated (for LoadAsXDoc()).
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

        #region key metadata elements
        internal bool HasPubDate
        {
            get
            {
                if (!_metadataHasBeenScanned)
                    ScanMetadata();
                return (_pubdate != null);
            }
        }
        private string _pubdate;

        /// <summary>
        /// Will throw an exception if there is no pubdate
        /// Client should use HasPubDate first.
        /// </summary>
        internal DateTime PubDate
        {
            get
            {
                return DateTime.Parse(_pubdate);
            }
        }

        internal string Tags
        {
            get
            {
                if (!_metadataHasBeenScanned)
                    ScanMetadata();
                return _tags;
            }
        }
        private bool _metadataHasBeenScanned;
        private string _tags;

        internal string Summary
        {
            get
            {
                if (!_metadataHasBeenScanned)
                    ScanMetadata();
                return _summary;
            }
        }
        private string _summary;

        internal string Description
        {
            get
            {
                if (!_metadataHasBeenScanned)
                    ScanMetadata();
                return _description;
            }
        }
        private string _description;

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

                // Description(Abstract) - FGDC & ArcGIS: /metadata/idinfo/descript/abstract            
                _description = xDoc.Descendants("abstract")
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) &&
                                    !value.StartsWith("REQUIRED:"))
                    .FirstOrDefault();
                // Summary (Purpose) - FGDC & ArcGIS: /metadata/idinfo/descript/purpose            
                _summary = xDoc.Descendants("purpose")
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) &&
                                    !value.StartsWith("REQUIRED:"))
                    .FirstOrDefault();
                // Tags (keywords) - FGDC: metadata/idinfo/keywords/*/*key (where * = theme, place, strat, temp); each keyword in a separate element          
                // Tags (keywords) - ArcGIS: metadata/dataIdInfo/*Keys/keyword (where * = desc, other, place, temp, disc, strat, search, theme) *Keys and keyword may appear multiple times.           
                _tags = xDoc.Descendants("keyword")
                    .Concat(xDoc.Descendants("themekey"))
                    .Concat(xDoc.Descendants("placekey"))
                    .Concat(xDoc.Descendants("stratkey"))
                    .Concat(xDoc.Descendants("tempkey"))
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) &&
                                    value != "009" &&           // keyword in otherKeys for all new ArcGIS metadata 
                                    !value.StartsWith("REQUIRED:"))
                    .Distinct().Concat(", ");
                // PubDate - FGDC: /metadata/idinfo/citation/citeinfo/pubdate          
                // PubDate - ArcGIS: /metadata/dataIdInfo/idCitation/date/pubdate            
                _pubdate = xDoc.Descendants("pubdate")
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) && 
                                    !value.StartsWith("REQUIRED:"))
                    .FirstOrDefault();
                _pubdate = ExpandFgdcDate(_pubdate);
                DateTime date;
                if (!DateTime.TryParse(_pubdate, out date))
                    _pubdate = null;
            }
            _metadataHasBeenScanned = true;
        }

        private string ExpandFgdcDate(string _pubdate)
        {
            if (string.IsNullOrEmpty(_pubdate))
                return null;
            if (_pubdate.Length == 4)
                return _pubdate + "-01-01";
            if (_pubdate.Length == 6)
                return _pubdate.Substring(0, 4) + "-" + _pubdate.Substring(4, 2) + "-01";
            if (_pubdate.Length == 8)
                return _pubdate.Substring(0, 4) + "-" + _pubdate.Substring(4, 2) + "-" + _pubdate.Substring(6, 2);
            return null;
        }
        #endregion

        #region INotifyPropertyChanged

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
