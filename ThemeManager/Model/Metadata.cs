using NPS.AKRO.ThemeManager.ArcGIS;
using NPS.AKRO.ThemeManager.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NPS.AKRO.ThemeManager.Model
{
    /// <summary>
    /// Selected key attributes of the metadata content.  These attributes are harvested
    /// from the metadata content so they can be stored with the theme node to speed up searching.
    /// </summary>
    struct GeneralInfo
    {
        internal string Description;
        internal DateTime? PublicationDate;
        internal string Summary;
        internal string Tags;
    }

    /// <summary>
    /// The content at Path cannot be displayed
    /// </summary>
    class MetadataDisplayException : Exception
    {
        internal MetadataDisplayException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Defines the meaning of the string in Metadata.Path and
    /// how it is used to find the metadata content.
    /// See Metadata.Format for how to interpret the content.
    /// </summary>
    enum MetadataType
    {
        /// <summary>
        /// The meaning of the text in Path is unknown and undefined.
        /// </summary>
        Undefined,
        /// <summary>
        /// A file system path to a file containing metadata (XML, HTML or plain text).
        /// </summary>
        FilePath,
        /// <summary>
        /// A data source path (ArcGIS is used to get the metadata content from the data source).
        /// Always in XML format.
        /// </summary>
        EsriDataPath,
        /// <summary>
        /// Uniform Resource Locator to an online metadata resource.
        /// While the file:// schema is a valid URL, those will be treated as a FilePath.  
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
        /// The metadata content is HTML that is already formatted for display.
        /// Attribute info (GetInfo()), and attribute search is not supported.
        /// </summary>
        Html,
        /// <summary>
        /// The metadata content is not XML or HTML, so it is assumed to be plain text and will be displayed as is.
        /// Attribute info (GetInfo()), and attribute search is not supported.
        /// </summary>
        Text
    }

    /// <summary>
    /// Provides a path to a metadata resource and methods to display, search and get attributes
    /// from that resource.
    /// </summary>
    /// <remarks>
    /// This class supports
    /// 1) Serializing (loading from and saving to) to an XML representation.  This only converts
    ///    a few instance properties to/from an XML representation and does not load or validate
    ///    the referenced resource.
    /// 2) Cloning by memberwise copy to support copying Theme list items in WinForms.
    /// 3) INotifyPropertyChanged to support binding the Path property to a WinForm.
    /// 4) Creating a new instance (for a new empty Theme, for a new theme with datasource, or
    ///    while loading an XML or MDB theme list), Updating the Path property either directly, or
    ///    as the result of a change to the node's datasource.  These operations may guess at the
    ///    Type and Format properties, but they do not load the resource or validate the properties.
    ///    These operation should be fast and non-blocking.  They should not throw exceptions.
    /// 5) Displaying, searching or getting attributes from the metadata resource.  These
    ///    operation will block waiting for I/O and may fail.  They will verify/adjust the
    ///    Type and Format properties to match the state of the resource when it is used.
    ///    They may throw an exception or return null and set the ErrorMessage property.
    ///    They may need to load the Esri License Manager to read metadata in a geo-database
    ///
    /// The contents of the metadata resource are not cached since 1) it may change between operations,
    /// 2) these operations are generally pretty fast, and 3) they are only done at the user's request.
    ///
    /// The metadata object is never validated.  If it cannot be used as requested, an exception
    /// will be thrown, or null will be returned and the ErrorMessage property set.
    /// 
    /// The Type and Format Properties are informational and may be wrong. They will be assumed at
    /// object creation and path/datasource changes.  They will be verified when the resource is
    /// used (display, search, get attributes).  Even then the contents of an external file or
    /// online resource may change before the resource is used again.
    /// </remarks>
    [Serializable]
    class Metadata : ICloneable, INotifyPropertyChanged
    {
        #region  Class Methods (Public)

        /// <summary>
        /// Creates a Metadata reference appropriate for the data source.   
        /// </summary>
        /// <remarks>
        /// This method will guess the metadata Path, Type and Format based on Esri conventions
        /// and file existence. It is Async, because Iit will check the file system for a file
        /// and/or directory existance which will block, and could take significant time in unusual
        /// circumstances: missing drive, network connection problems, drive asleep, etc. This may
        /// need to check several different paths to ensure we do not miss existing metadata for a datasource.
        /// This method is only called by ThemeBuilder.cs when the user adds a new data source.
        /// </remarks>
        /// <param name="data">The theme's data source</param>
        /// <returns>a new Metadata object for a theme</returns>
        internal static async Task<Metadata> FromDataSourceAsync(ThemeData data)
        {
            Metadata newMetadata = new Metadata();

            if (data == null)
                return newMetadata;

            //Caution: Setting Path will clear the Type and Format, so set Path first.

            // General file based metadata
            //   Includes layer files.  if *.lyr.xml exists, it trumps the datasource metadata (for single datasource layers)
            if (data.Path != null && await MyFile.ExistsAsync(data.Path + ".xml"))
            {
                newMetadata.Path = data.Path + ".xml";
                newMetadata.Type = MetadataType.FilePath;
                newMetadata.Format = MetadataFormat.Xml;
                return newMetadata;
            }

            // General file based metadata
            //   Includes file based raster data, LAS datasets, and others.
            if (data.DataSource != null && await MyFile.ExistsAsync(data.DataSource + ".xml"))
            {
                newMetadata.Path = data.DataSource + ".xml";
                newMetadata.Type = MetadataType.FilePath;
                newMetadata.Format = MetadataFormat.Xml;
                return newMetadata;
            }

            // Grids, TINs, and other directory based feature classes
            if (data.IsRasterBand && data.DataSource != null && await MyDirectory.ExistsAsync(data.DataSource))
            {
                string metadataPath = System.IO.Path.Combine(data.DataSource, "metadata.xml");
                if (await MyFile.ExistsAsync(metadataPath))
                {
                    newMetadata.Path = metadataPath;
                    newMetadata.Type = MetadataType.FilePath;
                    newMetadata.Format = MetadataFormat.Xml;
                }
                return newMetadata;
            }

            // Shapefile
            if (data.IsShapefile)
            {
                string metadataPath = data.DataSource + ".shp.xml";
                if (await MyFile.ExistsAsync(metadataPath))
                {
                    newMetadata.Path = metadataPath;
                    newMetadata.Type = MetadataType.FilePath;
                    newMetadata.Format = MetadataFormat.Xml;
                }
                return newMetadata;
            }

            // ArcInfo Coverages
            if (data.IsCoverage && data.WorkspacePath != null && data.Container != null)
            {
                string coverageDir = System.IO.Path.Combine(data.WorkspacePath, data.Container);
                if (await MyDirectory.ExistsAsync(coverageDir))
                {
                    string metadataPath = System.IO.Path.Combine(coverageDir, "metadata.xml");
                    if (await MyFile .ExistsAsync(metadataPath))
                    {
                        newMetadata.Path = metadataPath;
                        newMetadata.Type = MetadataType.FilePath;
                        newMetadata.Format = MetadataFormat.Xml;
                    }
                    return newMetadata;
                }
            }

            // CAD
            if (data.IsCad && data.WorkspacePath != null && data.Container != null)
            {
                string cadFile = System.IO.Path.Combine(data.WorkspacePath, data.Container);
                if (await MyFile .ExistsAsync(cadFile))
                {
                    string metadataPath = cadFile + ".xml";
                    if (await MyFile .ExistsAsync(metadataPath))
                    {
                        newMetadata.Path = metadataPath;
                        newMetadata.Type = MetadataType.FilePath;
                        newMetadata.Format = MetadataFormat.Xml;
                    }
                    return newMetadata;
                }
            }

            // SDC - Smart Data Compression for ESRI Street Map and Sample datasets.
            if (data.IsSdc && data.WorkspacePath != null && data.Container != null)
            {
                string sdcFile = System.IO.Path.Combine(data.WorkspacePath, data.Container);
                if (await MyFile .ExistsAsync(sdcFile))
                {
                    string metadataPath = sdcFile + ".xml";
                    if (await MyFile .ExistsAsync(metadataPath))
                    {
                        newMetadata.Path = metadataPath;
                        newMetadata.Type = MetadataType.FilePath;
                        newMetadata.Format = MetadataFormat.Xml;
                    }
                    return newMetadata;
                }
            }

            // GeoDatabases - Metadata is not a separate XML file, it is internal to the database
            //   For SDE datasets to work, the original connection file (*.sde) must be available
            //     to all theme manager users, i.e. not in a local profile (the typical default location)
            if (data.IsInGeodatabase)
            {
                newMetadata.Path = data.DataSource;
                if (data.IsRasterBand)
                {
                    newMetadata.Path = data.DataSource?.Replace("\\" + data.DataSourceName, "");
                }
                newMetadata.Type = MetadataType.EsriDataPath;
                newMetadata.Format = MetadataFormat.Xml;
                return newMetadata;
            }

            // File based Raster band metadata
            //   DataSource will contain the Band Name, while metadata will not
            //   Needs to be done after geodatabase, else we miss rasterbands in geodatabases
            if (data.IsRasterBand && data.WorkspacePath != null && data.Container != null)
            {
                string rasterName = System.IO.Path.Combine(data.WorkspacePath, data.Container);
                string metadataPath = rasterName + ".xml";
                if (await MyFile .ExistsAsync(metadataPath))
                {
                    newMetadata.Path = metadataPath;
                    newMetadata.Type = MetadataType.FilePath;
                    newMetadata.Format = MetadataFormat.Xml;
                }
                return newMetadata;
            }

            // Esri Web services
            if ((data.IsEsriMapService || data.IsEsriImageService) && data.DataSource != null)
            {
                newMetadata.Path = Regex.Replace(data.DataSource, "/arcgis/services/", "/arcgis/rest/services/", RegexOptions.IgnoreCase);
                newMetadata.Path = newMetadata.Path + "/info/metadata";
                newMetadata.Type = MetadataType.Url;
                newMetadata.Format = MetadataFormat.Xml;
                return newMetadata;
            }
            if (data.IsEsriFeatureService && data.WorkspacePath != null)
            {
                newMetadata.Path = Regex.Replace(data.WorkspacePath, "/arcgis/services/", "/arcgis/rest/services/", RegexOptions.IgnoreCase);
                newMetadata.Path = newMetadata.Path + "/info/metadata";
                newMetadata.Type = MetadataType.Url;
                newMetadata.Format = MetadataFormat.Xml;
                return newMetadata;
            }

            // Other web services may have metadata, but we are not ready to support them
            // LiDAR data tiles (.las) are not data sources that ArcGIS manages directly (*.lasD files are handled above)
            // Raster functions have no permanent disk representation except a layer file

            // Do not provide a default, it was usually wrong, and it will be better to return nothing

            Debug.Print($"Metadata not found for Data.Path:{data.Path}, Data.DataSource:{data.DataSource}, Data.DataSetType:{data.DataSetType}");

            return newMetadata;
        }

        /// <summary>
        /// Reconstitutes an archived Metadata object from XML
        /// </summary>
        /// <remarks>
        /// This is called for each node when loading a theme list from a TML file.
        /// This will not load the metadata resource at the path, nor validate
        /// the properties.  This method is non-blocking and very fast.
        /// It may throw the following exceptions:
        ///   ArgumentNullException - XML not provided, or expected attributes in XML are missing
        ///   ArgumentException - XML contains unexpected values
        /// </remarks>
        /// <param name="element">The XML representation of a metadata object</param>
        /// <returns>a new Metadata object for a theme</returns>
        internal static Metadata FromXElement(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (element.Name != "metadata")
                throw new ArgumentException("Invalid XElement");

            // 'Inline' was formerly a valid value for the type.  Do not reject a formerly valid TML file,
            // rather quietly convert the Inline value to Undefined.  Any other unexpected value is an exception.
            var type = MetadataType.Undefined;
            try
            {
                type = (MetadataType)Enum.Parse(typeof(MetadataType), (string)element.Attribute("type"));
            }
            catch (ArgumentException ex)
            {
                if (!ex.Message.Contains("'Inline'", StringComparison.Ordinal))
                {
                    throw;
                }
            }

            var data = new Metadata(
                element.Value,
                type,
                (MetadataFormat)Enum.Parse(typeof(MetadataFormat), (string)element.Attribute("format")),
                (string)element.Attribute("version"),
                (string)element.Attribute("schema")
                );
            return data;
        }

        #endregion


        #region  Class Methods (Private)

        /// <summary>
        /// Search a large text string for a number of small text strings
        /// </summary>
        /// <param name="haystack">The text string to search</param>
        /// <param name="needles">a list of text strings to look for</param>
        /// <param name="findAll">true => only return true if all needles are found, otherwise
        /// return true if any needle is found</param>
        /// <param name="comparisonMethod">How the strings should be compared, i.e. case sensitive, locale aware, ...</param>
        /// <returns>true if all (or any) needles were found in the haystack</returns>
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
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return Regex.Replace(input, @"<[^>]+>|&nbsp;", "").Trim();
        }

        /// <summary>
        /// Return the multiple white space (including new lines) condensed to a single space
        /// </summary>
        private static string MinimizeWhiteSpace(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return Regex.Replace(input, @"\s{2,}", " ");
        }

        /// <summary>
        /// Return the input with all newlines as windows (\r\n) style newlines
        /// </summary>
        private static string NormalizeToWindowsNewline(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            return input.Replace("\r\n","\n").Replace("\n","\r\n");
        }

        #endregion


        #region  Constructors

        /// <summary>
        /// Create a new 'empty' Metadata object to be fleshed out later (maybe).
        /// </summary>
        /// <remarks>
        /// Called internally, and by the user when clicking the menu item: add new theme/category
        /// </remarks>
        internal Metadata()
            : this(null, MetadataType.Undefined, MetadataFormat.Undefined, null, null)
        {
        }

        /// <summary>
        /// Create a new Metadata object with a few common properties.
        /// </summary>
        /// <remarks>
        /// None of the properties are checked for correctness.  This is used externally when
        /// building a theme list stored in a Microsoft Access database (version 2.0 theme list)
        /// </remarks>
        /// <param name="path">String to the metadata resource</param>
        /// <param name="type">See Metadata.Type</param>
        /// <param name="format">See Metadata.Format</param>
        internal Metadata(string path, MetadataType type, MetadataFormat format)
            : this(path, type, format, null, null)
        {
        }

        /// <summary>
        /// Create a new Metadata object with all properties stored in the XML format.
        /// </summary>
        /// <remarks>
        /// None of the properties are checked for correctness.  This is the "master"
        /// constructor that all other constructors call
        /// </remarks>
        /// <param name="path">String to the metadata resource</param>
        /// <param name="type">See Metadata.Type</param>
        /// <param name="format">See Metadata.Format</param>
        /// <param name="version">string documenting the version of the xml schema</param>
        /// <param name="schema">string documenting the xml schema</param>
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

        // backing for Path property
        private string _path;

        #endregion


        #region  Public Properties

        /// <summary>
        /// A text string documenting any problems reading/using the metadata content at Path
        /// </summary>
        /// <remarks>
        /// This is an option to throwing an exception.  If a method GetInfo() returns null, the
        /// caller can check this property for an possible explanation.
        /// This property will usually be null, and is not guaranteed to have a value. 
        /// </remarks>
        internal string ErrorMessage { get; private set; }

        /// <summary>
        /// The format of the content in Metadata.Path.
        /// </summary>
        /// <remarks>
        /// See Metadata.Format for more info.
        /// </remarks>
        public MetadataFormat Format { get; private set; }

        /// <summary>
        /// The path to the metadata resource.
        /// </summary>
        /// <remarks>
        /// See Metadata.Type for the meaning of the value in the string.
        /// The getter and setter needs to be public so that it can be bound to a WinForm control.
        /// That is the only location (except internally) that will change the path.
        /// The property must support INotifyPropertyChanged for WinForm binding.
        /// The setter is a noop if the value isn't different.
        /// </remarks>
        public string Path
        {
            get
            {
                return _path;
            }
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if (_path != value)
                {
                    _path = value;
                    Type = MetadataType.Undefined;
                    Format = MetadataFormat.Undefined;
                    OnPropertyChanged("Path");
                }
            }
        }

        /// <summary>
        /// The meaning of the string in Metadata.Path.
        /// </summary>
        /// <remarks>
        /// See Metadata.Type for the meaning of the value in the string.
        /// </remarks>
        public MetadataType Type { get; private set; }

        #endregion


        #region  Interface Implementations
        #region IClonable Interface

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

        /// <summary>
        /// Displays formatted metadata content in an web browser control
        /// </summary>
        /// <remarks>
        /// If the metadata content is XML, then it the provided stylesheet
        /// will be used to convert it to styled html.  If the content is html or text
        /// then it will be provided to the browser control as is.  In the case of
        /// online html (Type == URL and Format == html), then the URL is given to the
        /// browser so that it can also load any external javascript, css, etc.
        /// This may throw a MetadataDisplayException if it can't obtain or style the
        /// content.  The browser control shouldn't throw any exceptions and will handle
        /// http error and html render problems by altering the browser window display as
        /// appropriate.
        /// This method will block while content is loaded from disk or network.
        /// The first time an XSL stylesheet is used, the content must be loaded from disk,
        /// and then compiled before the transformation can be applied to the XML.
        /// This method may also stall while an Esri license is obtained.
        /// </remarks>
        /// <param name="webBrowser"></param>
        /// <param name="styleSheet"></param>
        internal async Task DisplayAsync(WebBrowser webBrowser, StyleSheet styleSheet)
        {
            Debug.Assert(webBrowser != null, "The WebBrowser control is null");
            if (webBrowser == null)
                throw new ArgumentNullException(nameof(webBrowser));

            // We need XML data as a string for the Stylesheet transformation, even if the XML is at a URL.
            // GetContentAsText will also try to validate the Type and Format properties.
            // This will not throw an exception, but might return null
            // It will return null if Type = URL and Format in (Html, Text), but that is ok, since we only need the Path
            string content = await GetContentAsTextAsync();
            if (content == null && (Type != MetadataType.Url || Format == MetadataFormat.Xml || Format == MetadataFormat.Undefined))
            {
                throw new MetadataDisplayException($"Unable to load metadata content: {ErrorMessage}", null);
            }

            // If GetContentAsText() thinks the content is XML (even if the Type is URL), we will
            // send it to the stylesheet for transformation.  We will also send Undefined data in the
            // hopes that it may actually be XML data.
            if (Format == MetadataFormat.Xml || Format == MetadataFormat.Undefined)
            {
                if (styleSheet == null)
                {
                    // We could present Format.Undefined as plain text, but not having a stylesheet is
                    // an error, especially if the content is actually XML.
                    throw new MetadataDisplayException("Unable to transform metadata for display: There is no stylesheet available", null);
                }
                try
                {
                    // Do this in two steps to avoid sending partial processing to the webBrowser
                    string html = styleSheet.TransformText(content);
                    webBrowser.DocumentText = html;
                }
                catch (Exception ex)
                {
                    if (Format == MetadataFormat.Undefined)
                    {
                        // Apparently it wasn't really XML after all
                        webBrowser.DocumentText = content;
                    }
                    else
                    {
                        throw new MetadataDisplayException($"Unable to stylize the metadata content: {ex.Message}", ex);
                    }
                }
            }
            else
            {
                if (Type == MetadataType.Url)
                {
                    webBrowser.Url = new Uri(Path);
                }
                else
                {
                    webBrowser.DocumentText = content;
                }
            }
        }

        /// <summary>
        /// Get selected attributes from known XML metadata formats
        /// </summary>
        /// <remarks>
        /// If there are any problems reading the content at Path as XML, then all the
        /// attributes will be null and ErrorMessage will be set to an explanation.
        /// If the metadata is in an unknown XML schema, then some or all of the attributes
        /// may be null. This method should be able to parse ArcGIS, CSGDM and ISO 19139 schemas.
        /// This method will not throw any exceptions.
        /// This method will block while content is loaded from disk or network.
        /// This method may stall while an Esri license is obtained.
        /// </remarks>
        /// <returns>A tuple with Summary, Description, Tags and a Publication date</returns>
        [SuppressMessage("ReSharper", "CommentTypo")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        internal async Task<GeneralInfo> GetGeneralInfoAsync()
        {
            string description = null;
            DateTime? publicationDate = null;
            string summary = null;
            string tags = null;

            // We can only extract meaningful data from metadata content is an XML document.
            XDocument xmlMetadata = await ContentAsXDocumentAsync();
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
                description = NormalizeToWindowsNewline(StripSimpleHtmlTags(description)); // The description might be multiline (\r\n is required for winforms)

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
                summary = MinimizeWhiteSpace(StripSimpleHtmlTags(summary));

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
                tags = MinimizeWhiteSpace(StripSimpleHtmlTags(tags));
            }

            return new GeneralInfo {
                Description = description,
                PublicationDate = publicationDate,
                Summary = summary,
                Tags = tags
            };
        }

        /// <summary>
        /// Search the metadata content for text strings. 
        /// </summary>
        /// <remarks>
        /// This method is required to load and possibly parse the metadata content.
        /// It can be slow, and is only used by the advanced search interface.
        /// It will not throw any exceptions, but will instead set the result to false
        /// and set the ErrorMessage property.
        /// This method will block while content is loaded from disk or network.
        /// This method may stall while an Esri license is obtained.
        /// </remarks>
        /// <param name="search">the text and searching constraints to use.</param>
        /// <returns>true if the metadata satisfies the search options</returns>
        internal async Task<bool> SearchContentAsync(SearchOptions search)
        {
            if (search == null)
                return false;

            Debug.Assert(!string.IsNullOrEmpty(Path), "This Metadata object does not have a path");
            Debug.Assert(search.SearchType == SearchType.Metadata, "The search parameters are not appropriate for metadata");

            if (string.IsNullOrEmpty(search.XmlElement))
            {
                //to search the whole document, we don't need to parse it as XML
                string content = await GetContentAsTextAsync();
                return content != null && Match(content, search.SearchWords, search.FindAll, search.ComparisonMethod);
            }
            XDocument xmlMetadata = await ContentAsXDocumentAsync();
            if (xmlMetadata == null)
                return false;
            return xmlMetadata.Descendants(search.XmlElement)
                .Select(element => element.Value)
                .Where(value => !string.IsNullOrEmpty(value))
                .Any(value => Match(value, search.SearchWords, search.FindAll, search.ComparisonMethod));
        }

        /// <summary>
        /// Updates the Path, Type and Format based on a new data source
        /// </summary>
        /// <remarks>
        /// This is called when the user changes the data source of a theme either by editing the path
        /// directly, or by reloading the theme.  It does not load the content or validate the properties.
        /// This method will not throw any exceptions.
        /// This should be a fast non-blocking method, but it isn't quite, see the docs on FromDataSource()
        /// for details on why.
        /// This method is required because WinForm binding is difficult to manage if an object is replaced
        /// by a new one. It is much easier to update the existing bound object.
        /// </remarks>
        /// <param name="data">The theme's data source</param>
        internal async Task UpdateWithDataSourceAsync(ThemeData data)
        {
            Metadata newMetadata = await FromDataSourceAsync(data);
            if (Path != null && newMetadata.Path == null)
                // This item has a non-standard metadata path (set manually), do not delete it.
                return;
            Path = newMetadata.Path;
            Type = newMetadata.Type;
            Format = newMetadata.Format;
        }

        /// <summary>
        /// Serializes the major properties as an XElement.
        /// </summary>
        /// <remarks>
        /// Called by TMNode.cs when the object is written to a TML file.
        /// This method is fast and non-blocking
        /// It will not throw any exceptions.
        /// </remarks>
        /// <returns>an XElement representation of the Metadata object</returns>
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


        #region  Private Properties

        private string Schema { get; }  // Unused but supported as part of the TML file
        private string Version { get; }  // Unused but supported as part of the TML file

        #endregion


        #region  Private Methods

        /// <summary>
        /// Tries to load the metadata resource at Path as a text string
        /// </summary>
        /// <remarks>
        /// This method will not throw an exception.
        /// Side effects:
        /// It will set ErrorMessage to an error message (if any are encountered) 
        /// It may change the Type and Format if they are wrong.
        /// </remarks>
        /// <returns>a text string of the metadata if available or null</returns>
        private async Task<string> GetContentAsTextAsync()
        {
            string contents = null;
            ErrorMessage = null;

            if (string.IsNullOrEmpty(Path))
            {
                ErrorMessage = "Metadata has no Path to the content.";
                return null;
            }

            // using System.Diagnostics.Eventing.Reader;
            // Trace.TraceInformation("{0}: Start of Metadata.LoadASText({1})", DateTime.Now, Path); Stopwatch time = Stopwatch.StartNew();

            // Try to guess an Undefined MetadataType
            if (Type == MetadataType.Undefined)
            {
                if (Path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    Type = MetadataType.Url;
                }
                else if (await MyFile.ExistsAsync(Path))
                {
                    Type = MetadataType.FilePath;
                }
                else if (Path.Contains(".gdb\\", StringComparison.OrdinalIgnoreCase) ||
                         Path.Contains(".sde\\", StringComparison.OrdinalIgnoreCase) ||
                         Path.Contains(".mdb\\", StringComparison.OrdinalIgnoreCase))
                {
                    
                    Type = MetadataType.EsriDataPath;
                }
                else
                {
                    ErrorMessage = "The Type of the metadata Path is Undefined and not obvious.";
                    return null;
                }
            }

            // Correct a URL with the file:// scheme
            if (Type == MetadataType.Url && new Uri(Path).IsFile)
            {
                Path = new Uri(Path).LocalPath;
                Type = MetadataType.FilePath;
            }

            // Try and load the content
            try
            {
                if (Type == MetadataType.FilePath)
                {
                    contents = await MyFile.ReadAllTextAsync(Path);
                }
                if (Type == MetadataType.EsriDataPath)
                {
                    contents = await GisInterface.GetMetadataAsXmlAsync(Path);
                    Format = MetadataFormat.Xml;
                }
                if (Type == MetadataType.Url)
                {
                    // If the format is Undefined, I need to download the contents to check the Format
                    // If the Format is Xml, I will download the contents for getting attributes
                    // For Html and Text Formats, I can't do anything with the contents, so skip this step
                    // The Display method will use the URL to get the contents
                    if (Format == MetadataFormat.Xml || Format == MetadataFormat.Undefined)
                    {
                        var uri = new Uri(Path);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        HttpClient client = new HttpClient();
                        var response = await client.GetAsync(uri);
                        if (response.IsSuccessStatusCode)
                        {
                            contents = await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            ErrorMessage =
                                $"Could not get {Path} from server.  Status code: {response.StatusCode}({response.ReasonPhrase})";
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Debug.Print("Exception thrown trying to load Metadata.\n" + ex);
                return null;
            }

            // If the file, service, or geo-database returned nothing, we are done for.
            if (string.IsNullOrEmpty(contents) && (Type != MetadataType.Url || Format == MetadataFormat.Xml || Format == MetadataFormat.Undefined))
            {
                ErrorMessage = "The content of the resource at Path is empty.";
                Format = MetadataFormat.Undefined;
                return null;
            }

            // Test the format
            if (Format == MetadataFormat.Undefined)
            {
                // ReSharper disable once PossibleNullReferenceException
                if (contents.StartsWith("<?xml version=", StringComparison.OrdinalIgnoreCase) ||
                    contents.StartsWith("<metadata", StringComparison.OrdinalIgnoreCase))
                {
                    Format = MetadataFormat.Xml;
                }
                if (Regex.IsMatch(contents, "<html>|<html .+>"))
                {
                    Format = MetadataFormat.Html;
                }
                if (!Regex.IsMatch(contents, "<.+>|</.+>"))
                {
                    Format = MetadataFormat.Text;
                }
            }

            // time.Stop(); Trace.TraceInformation("{0}: End of Metadata.LoadASText() - Elapsed Time: {1}", DateTime.Now, time.Elapsed);

            return contents;
        }

        /// <summary>
        /// Returns the contents of the metadata at Path as an XDocument
        /// </summary>
        /// <remarks>
        /// This method will not throw an exception.
        /// Side effects:
        /// It will set ErrorMessage to an exception message (if encountered) 
        /// It may change the Type and Format if they are wrong.
        /// </remarks>
        /// <returns>An XDocument or null</returns>
        private async Task<XDocument> ContentAsXDocumentAsync()
        {
            string xmlString = await GetContentAsTextAsync();
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

        #endregion
    }
}
