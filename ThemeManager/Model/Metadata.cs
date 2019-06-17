using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.ArcGIS;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace NPS.AKRO.ThemeManager.Model
{
    struct GeneralInfo
    {
        internal string Description;
        internal DateTime? PublicationDate;
        internal string Summary;
        internal string Tags;
    }

    class MetadataDisplayException : Exception
    {
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

    [Serializable]
    class Metadata : ICloneable, INotifyPropertyChanged
    {
        #region  Class Fields (Private)

        private static readonly Dictionary<string, string> ContentCache = new Dictionary<string, string>();

        #endregion


        #region  Class Methods (Public)

        // Called by ThemeBuilder.cs line 83 (data added to theme list)
        internal static Metadata Find(ThemeData data)
        {
            if (data == null)
                return null;

            Meta myProps = ExpectedMetadataProperties(data);
            Metadata metadata = new Metadata(myProps.Path, myProps.Type, myProps.Format);
            if (metadata.Validate())
                return metadata;
            return null;
        }

        //Called by TmNode.cs line 1133 (building object from theme list XML)
        internal static Metadata Load(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (element.Name != "metadata")
                throw new ArgumentException("Invalid XElement");
            var data = new Metadata(
                element.Value,
                (MetadataType)Enum.Parse(typeof(MetadataType), (string)element.Attribute("type")),
                (MetadataFormat)Enum.Parse(typeof(MetadataFormat), (string)element.Attribute("format")),
                (string)element.Attribute("version"),
                (string)element.Attribute("schema")
                );
            return data;
        }

        #endregion


        #region  Class Methods (Private)

        private static Meta ExpectedMetadataProperties(ThemeData data)
        {
            Meta newMeta = new Meta()
            {
                Path=null,
                Type = MetadataType.Undefined,
                Format= MetadataFormat.Undefined
            };

            if (data == null)
                return newMeta;

            newMeta.Type = MetadataType.FilePath;
            newMeta.Format = MetadataFormat.Xml;

            //general file based metadata
            if (data.Path != null && File.Exists(data.Path + ".xml"))
            {
                newMeta.Path = data.Path + ".xml";
                return newMeta;
            }

            if (data.DataSource != null && File.Exists(data.DataSource + ".xml"))
            {
                newMeta.Path = data.DataSource + ".xml";
                return newMeta;
            }

            //grids & tins
            if (data.DataSource != null
                //&& data.WorkspacePath != null
                && Directory.Exists(data.DataSource))
            {
                string metaPath = System.IO.Path.Combine(data.DataSource, "metadata.xml");
                if (File.Exists(metaPath))
                {
                    newMeta.Path = metaPath;
                    return newMeta;
                }
            }

            //Shapefile
            if (data.IsShapefile)
            {
                string metaPath = data.DataSource + ".shp.xml";
                if (File.Exists(metaPath))
                {
                    newMeta.Path = metaPath;
                    return newMeta;
                }
            }

            //coverages
            if (data.IsCoverage && data.WorkspacePath != null && data.Container != null)
            {
                string coverageDir = System.IO.Path.Combine(data.WorkspacePath, data.Container);
                if (Directory.Exists(coverageDir))
                {
                    string metaPath = System.IO.Path.Combine(coverageDir, "metadata.xml");
                    if (File.Exists(metaPath))
                    {
                        newMeta.Path = metaPath;
                        return newMeta;
                    }
                }
            }

            //CAD
            if (data.IsCad && data.WorkspacePath != null && data.Container != null)
            {
                string cadFile = System.IO.Path.Combine(data.WorkspacePath, data.Container);
                if (File.Exists(cadFile))
                {
                    string metaPath = cadFile + ".xml";
                    if (File.Exists(metaPath))
                    {
                        newMeta.Path = metaPath;
                        return newMeta;
                    }
                }
            }

            newMeta.Type = MetadataType.EsriDataPath;

            if (data.IsInGeodatabase && !data.IsLayerFile)
            {
                newMeta.Path = data.DataSource;
                return newMeta;
            }
            if (data.IsLayerFile && !data.IsGroupLayerFile)
            {
                newMeta.Path = data.DataSource;
                return newMeta;
            }

            //FIXME - does not work for web services ???
            newMeta.Type = MetadataType.Undefined;
            newMeta.Format = MetadataFormat.Undefined;
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

        /// <summary>
        /// Converts a date in YYYY, YYYYMM, or YYYYMMDD format to YYYY-MM-DD
        /// </summary>
        private static string NormalizeFgdcDateString(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;
            if (dateString.Length == 4)
                return dateString + "-01-01";
            if (dateString.Length == 6)
                return dateString.Substring(0, 4) + "-" + dateString.Substring(4, 2) + "-01";
            if (dateString.Length == 8)
                return dateString.Substring(0, 4) + "-" + dateString.Substring(4, 2) + "-" + dateString.Substring(6, 2);
            // Return all other formats we do not recognize
            return dateString;
        }

        /// <summary>
        /// Return the input string without HTML Tags
        /// </summary>
        /// <remarks>
        /// I've read https://stackoverflow.com/a/1758162 and know that I cannot REALLY use Regex on HTML.
        /// However, all my input will be coming from snippets in an XML document.
        /// In order for the HTML fragment to be part of an XML document, it has to have been pre sanitized.
        /// This solution was adapted from https://stackoverflow.com/a/19524158
        /// </remarks>
        private static string StripSimpleHtmlTags(string input)
        {
            var noTags = Regex.Replace(input, @"<[^>]+>|&nbsp;", "").Trim();
            var minimalWhiteSpace = Regex.Replace(noTags, @"\s{2,}", " ");
            return minimalWhiteSpace;
        }

        #endregion


        #region  Constructors

        // Called by TmNode.cs line 82 (TmNode default constructor)
        internal Metadata()
            : this(null, MetadataType.Undefined, MetadataFormat.Undefined, null, null)
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
        }

        #endregion


        #region  Instance Fields (All Private; Should only be accessed by Contructors or Properties)

        private string _path;

        #endregion


        #region  Public Properties

        // Called by AdminReports.cs line 218 (ListMetadataProblems)
        internal string ErrorMessage { get; private set; }

        // Called in lots of places
        // Verify that form binding is not a form of setting the path (how else does the user change the metadata path?)
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        #endregion


        #region  Public Methods

        // Add Async internal MetaInfo GetInfo() -> struct MetaInfo(publicationDate?, tags, summary, description)
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
                throw new ArgumentNullException(nameof(webBrowser));

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

        // Called by AdminReports.cs line 217 and TmNode.cs line 1270
        // This method may throw exceptions; it may load content from disk or network; it may stall while loading a license.
        [SuppressMessage("ReSharper", "CommentTypo")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        internal GeneralInfo GetGeneralInfo()
        {
            string description = null;
            DateTime? publicationDate = null;
            string summary = null;
            string tags = null;

            // We can only extract meaningful data from metadata content is an XML document.
            XDocument xmlMetadata = ContentAsXDocument();
            if (xmlMetadata != null)
            {
                // The XML content may be in a number of different schemas and version
                // 1. FGDC (CSGDM)
                // 2. ArcGIS (various flavors)
                // 3. ISO 19115 content standard (19139 XML encoding)

                // Note that since I don't have a copy of the ISO specs,
                // the XPaths are based on reviewing metadata documents, currently with a sample size of 2
                // Note the ISO 19115-2 has a different root element (gmi:MI_Metadata)
                // The root for ISO 19115 and 19115-1 is gmd:MD_Metadata

                // 19139 Namespaces (relevant to searches below)
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("gmd", "http://www.isotc211.org/2005/gmd");
                namespaceManager.AddNamespace("gmi", "http://www.isotc211.org/2005/gmi");
                namespaceManager.AddNamespace("gco", "http://www.isotc211.org/2005/gco");

                // Description (aka Abstract)
                //   FGDC: /metadata/idinfo/descript/abstract
                //   ArcGIS: /metadata/dataIdInfo/idAbs
                //   ISO 19139: {root}/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:abstract/gco:CharacterString
                description = xmlMetadata
                    .Descendants("abstract")
                    .Concat(xmlMetadata.Descendants("idAbs"))
                    .Concat(xmlMetadata.XPathSelectElements("/gmd:MD_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:abstract/gco:CharacterString", namespaceManager))
                    .Concat(xmlMetadata.XPathSelectElements("/gmi:MI_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:abstract/gco:CharacterString", namespaceManager))
                    .Select(element => element.Value)
                    .FirstOrDefault(value => !string.IsNullOrEmpty(value) &&
                                             !value.StartsWith("REQUIRED:"));  // Unpopulated data from FGDC template
                description = StripSimpleHtmlTags(description);

                // PublicationDate
                //   FGDC: /metadata/idinfo/citation/citeinfo/pubdate
                //   ArcGIS: /metadata/dataIdInfo/idCitation/date/pubDate
                //   ISO 19139: {root}/gmd:dateStamp/gco:DateTime (or {root}/gmd:dateStamp/gco:Date if just a date is provided)
                //       that is actually the date of the metadata.  The actual publication date will be in 
                //       publication date: {root}/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:citation/gmd:CI_Citation/gmd:date/gmd:CI_Date/gmd:date/gco:Date
                //       where the ../../gmd:CI_Date/gmd:dateType/gmd:CI_DateTypeCode = "publication; publication" (actual values may vary based on codelist used)
                var pubDateString = xmlMetadata
                    .Descendants("pubdate")
                    .Concat(xmlMetadata.Descendants("pubDate"))
                    .Concat(xmlMetadata.XPathSelectElements("/gmd:MD_Metadata/gmd:dateStamp/gco:DateTime", namespaceManager))
                    .Concat(xmlMetadata.XPathSelectElements("/gmd:MD_Metadata/gmd:dateStamp/gco:Date", namespaceManager))
                    .Concat(xmlMetadata.XPathSelectElements("/gmi:MI_Metadata/gmd:dateStamp/gco:DateTime", namespaceManager))
                    .Concat(xmlMetadata.XPathSelectElements("/gmi:MI_Metadata/gmd:dateStamp/gco:Date", namespaceManager))
                    .Select(element => element.Value)
                    .FirstOrDefault(value => !string.IsNullOrEmpty(value) &&
                                             !value.StartsWith("REQUIRED:"));  // Unpopulated data from FGDC template
                // Normalize date string and convert to optional datetime
                pubDateString = NormalizeFgdcDateString(pubDateString);
                DateTime date;
                if (DateTime.TryParse(pubDateString, out date))
                    publicationDate = date;

                // Summary (aka Purpose)
                //   FGDC: /metadata/idinfo/descript/purpose
                //   ArcGIS: /metadata/dataIdInfo/idPurp
                //   ISO 19139: {root}/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:purpose/gco:CharacterString
                summary = xmlMetadata
                    .Descendants("purpose")
                    .Concat(xmlMetadata.Descendants("idPurp"))
                    .Concat(xmlMetadata.XPathSelectElements("/gmd:MD_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:purpose/gco:CharacterString", namespaceManager))
                    .Concat(xmlMetadata.XPathSelectElements("/gmi:MI_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:purpose/gco:CharacterString", namespaceManager))
                    .Select(element => element.Value)
                    .FirstOrDefault(value => !string.IsNullOrEmpty(value) &&
                                             !value.StartsWith("REQUIRED:"));   // Unpopulated data from FGDC template
                summary = StripSimpleHtmlTags(summary);

                // Tags (aka Keywords)
                //   FGDC: metadata/idinfo/keywords/*/*key
                //     Where * = theme, place, strat, temp; Each keyword is a distinct element
                //   ArcGIS: metadata/dataIdInfo/*Keys/keyword
                //     Where * = desc, other, place, temp, disc, strat, search, theme; *Keys and keyword may appear multiple times.
                //   ISO 19139: /gmd:MD_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:keyword/gco:CharacterString
                tags = xmlMetadata.Descendants("keyword")
                    .Concat(xmlMetadata.Descendants("themekey"))
                    .Concat(xmlMetadata.Descendants("placekey"))
                    .Concat(xmlMetadata.Descendants("stratkey"))
                    .Concat(xmlMetadata.Descendants("tempkey"))
                    .Concat(xmlMetadata.XPathSelectElements("/gmd:MD_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:keyword/gco:CharacterString", namespaceManager))
                    .Concat(xmlMetadata.XPathSelectElements("/gmi:MI_Metadata/gmd:identificationInfo/gmd:MD_DataIdentification/gmd:descriptiveKeywords/gmd:MD_Keywords/gmd:keyword/gco:CharacterString", namespaceManager))
                    .Select(element => element.Value)
                    .Where(value => !string.IsNullOrEmpty(value) &&
                                    !value.StartsWith("00") &&       // keyword in otherKeys for all new ArcGIS metadata
                                    !value.StartsWith("REQUIRED:"))  // Unpopulated data from FGDC template
                    .Distinct().Concat(", ");
                tags = StripSimpleHtmlTags(tags);  // Also condenses whitespace
            }

            return new GeneralInfo {
                Description = description,
                PublicationDate = publicationDate,
                Summary = summary,
                Tags = tags
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
                return content != null && Match(content, search.SearchWords, search.FindAll, search.ComparisonMethod);
            }
            XDocument xmlMetadata = ContentAsXDocument();
            if (xmlMetadata == null)
                return false;
            return xmlMetadata.Descendants(search.XmlElement)
                .Select(element => element.Value)
                .Where(value => !string.IsNullOrEmpty(value))
                .Any(value => Match(value, search.SearchWords, search.FindAll, search.ComparisonMethod));
        }

        // Called from TmNode.cs line 548 (data path changed) and line 1245 (reload theme)
        // Ensure this is a low cost synchronous method (the data path may or may not change); This is not a user request to Sync
        internal void Repair(ThemeData data)
        {
            //FIXME - this is a redundant load/validate/scan going on.
            Meta myNewProps = ExpectedMetadataProperties(data);
            if (myNewProps.Path == null)
                return;  //exception or error ???
            Type = myNewProps.Type;
            Format = myNewProps.Format;
            Path = myNewProps.Path;  //will re validate only if path changes
            Validate();
        }

        // Called by TmNode.cs line 845 (write object to theme list XML)
        internal XElement ToXElement()
        {
            return new XElement("metadata",
                                new XAttribute("type", Enum.GetName(typeof(MetadataType), Type) ?? ""),
                                new XAttribute("format", Enum.GetName(typeof(MetadataFormat), Format) ?? ""),
                                new XAttribute("version", Version ?? ""),
                                new XAttribute("schema", Schema ?? ""),
                                Path
                );
        }

        #endregion


        #region  Private Enums/Structs/Classes

        private struct Meta
        {
            internal string Path;
            internal MetadataType Type;
            internal MetadataFormat Format;
        }

        #endregion


        #region  Private Properties

        private MetadataFormat Format { get; set; }
        private bool IsValid { get; set; }
        private string Schema { get; } // TODO - use or toss
        private MetadataType Type { get; set; }
        private string Version { get; } // TODO - use or toss

        #endregion


        #region  Private Methods

        //TODO: Replace LoadAsText() ContentAsXDocument() and Validate(), etc with: LoadContentAndValidate()
        // cache results on class, to speed up multiple requests, avoid duplication, and avoid cloning large blocks of text
        // Validation is in two parts: 1) Validate Type (requires loading even URLs) and 2) Validate Format (may require parsing)
        // Validation can short circuit if Content is non null.  If content is null reload will be retried
        // Errors in Loading/Validation will be stored in ErrorMessages for display to the user.
        //These changes will improve display robustness, as well as simplify the code.  It may result in more
        //time spent loading or retrying, but always (and only) when the user requests it.

        //!! side effect of LoadAsText() is that Path, Type and IsValid are validated (for Display()).
        //!! side effect of LoadAsText() is that Format, IsValid are validated (for ContentAsXDocument()).
        // This will not throw any exceptions, rather it returns null, and sets ErrorMessage
        private string LoadAsText()
        {
            if (string.IsNullOrEmpty(Path))
                return null;
            if (ContentCache.ContainsKey(Path))
                return ContentCache[Path];

            string contents = null;

            Trace.TraceInformation("{0}: Start of Metadata.LoadASText({1})", DateTime.Now, Path); Stopwatch time = Stopwatch.StartNew();
            // If another thread is loading the text, wait for it to finish.
            // because of side effects, including changing state of CachedContentsAsText,
            // we lock the whole routine.
            lock (this)
            {
                //Validate is now fast enough that we will always do it. Eliminates false positives
                IsValid = Validate();
                if (IsValid)
                {
                    try
                    {
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
                    if (!string.IsNullOrEmpty(Path))
                        ContentCache[Path] = contents;
            }
            time.Stop(); Trace.TraceInformation("{0}: End of Metadata.LoadASText() - Elapsed Time: {1}", DateTime.Now, time.Elapsed);

            return contents;
        }

        // !!! side effect is that IsValid and Format may be changed if found to be incorrect.
        /// <summary>
        /// Returns the contents of the metadata at Path as an XDocument
        /// </summary>
        /// <remarks>
        /// This method will not throw an exception.
        /// It will set ErrorMessage to an exception message (if encountered) 
        /// It may change the Format if it is wrong.
        /// </remarks>
        /// <returns>An XDocument or null</returns>
        private XDocument ContentAsXDocument()
        {
            string xmlString = LoadAsText();
            if (xmlString == null)
            {
                return null;
            }
            XDocument contents = null;
            // Don't trust what we think the format is, just try parsing it.
            try
            {
                contents = XDocument.Parse(xmlString);
                Format = MetadataFormat.Xml;
            }
            catch (XmlException ex)
            {
                ErrorMessage = ex.Message;
                if (Format == MetadataFormat.Xml)
                    Format = MetadataFormat.Undefined;
            }
            return contents;
        }

        private void Reset()
        {
            if (!string.IsNullOrEmpty(Path) && ContentCache.ContainsKey(Path))
                ContentCache.Remove(Path);
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
                    case MetadataType.Undefined:
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
