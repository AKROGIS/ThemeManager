using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace NPS.AKRO.ThemeManager.Model.ThemeList
{
    [Serializable]
    class MdbStore : Store
    {
        private const string DbProvider = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
        private const string DefaultVersion = "2";
        private readonly Dictionary<int, List<Category>> _categoriesInCategories = new Dictionary<int,List<Category>>();
        private readonly string _oleConnection;

        //lists of themes with the same parent ID, key is the themeID of the parent Theme

        //lists of themes with the same category ID, key is the catID of the parent Category
        private readonly Dictionary<int, List<Theme>> _themesInCategories = new Dictionary<int,List<Theme>>();
        private readonly Dictionary<int, List<Theme>> _themesInThemes = new Dictionary<int,List<Theme>>();

        //lists of categories with the same category ID, key is the catID of the parent Category

        private DatabaseInfo _databaseInfo;

        public MdbStore(string path)
            : base(path)
        {
            _oleConnection = DbProvider + path + ";";
        }

        /// <summary>
        /// MDB are readonly (never editable) until we provide write support
        /// </summary>
        public override bool IsReadOnly { get {return true;} }

        public override bool IsThemeList
        {
            get
            {
                if (_valid == null)
                    _valid = IsValid();
                return (bool)_valid;
            }
        }


        public override string Version
        {
            get
            {
                if (_version == null)
                    _version = IsThemeList ? GetVersion() : DefaultVersion;

                return _version;
            }
        }

        private string GetVersion()
        {
            if (!IsVersion2Plus())
                return "1";
            if (_databaseInfo.Version == null)
                LoadDatabaseInfo();
            return _databaseInfo.Version;
        }

        public static MdbStore CreateNew(string path)
        {
            return CreateNew(path, DefaultVersion);
        }

        public static MdbStore CreateNew(string path, string version)
        {
            if (string.IsNullOrEmpty(version))
                version = DefaultVersion;
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Save As: Path not provided");
            if (File.Exists(path))
                throw new ArgumentException("Save As: File already Exists");
            if (Path.GetExtension(path).ToLower() != ".mdb")
                throw new ArgumentException("Save As: Path must be a MS Access file");

            //FIXME:  Need to create the file on disk

            if (!File.Exists(path))
                return null;
            return new MdbStore(path) {_version = version, _isReadOnly = false, _valid = true, _loaded = true};
        }

        public override void Build(ThemeListNode node)
        {
            if (node == null)
                throw new ArgumentNullException("TMNode is null");

            if (!_loaded)
                Load();
            if (Version[0] == '1')
            {
                // version 1 had no tbl_list, and hence no databaseInfo
                node.Name = _path;
            }
            if (Version[0] == '2')
            {
                node.Name = _databaseInfo.Name;
                if (!string.IsNullOrEmpty(_databaseInfo.Link))
                    node.Metadata = new Metadata(_databaseInfo.Link, MetadataType.Url, MetadataFormat.Html);
                node.Description = _databaseInfo.Description;
                if (_databaseInfo.HasAuthor)
                {
                    node.Author = _databaseInfo.Author;
                }
            }
            FillCategory(node, -1);
        }

        internal TmNode BuildThemeFromId(int themeId)
        {
            if (!_loaded)
                Load();

            Theme theme = FindThemebyId(themeId);
            TmNode newNode = new ThemeNode(
                theme.Name,
                null,
                MakeThemeData(theme.Name, theme.DataSource, theme.Type),
                MakeMetadataForTheme(theme.Metadata),
                null,
                theme.PubDate
                );
            FillTheme(newNode, themeId);
            return newNode;
        }

        private Theme FindThemebyId(int themeId)
        {
            foreach (List<Theme> list in _themesInCategories.Values)
                foreach (Theme theme in list)
                    if (themeId == theme.Id)
                        return theme;
            foreach (List<Theme> list in _themesInThemes.Values)
                foreach (Theme theme in list)
                    if (themeId == theme.Id)
                        return theme;
            throw new ArgumentException("No Theme for theme ID = " + themeId);
        }

        public override void Save(ThemeListNode node)
        {
            //FIXME - Implement Save;
            // check version and save appropriate to that version
            throw new NotImplementedException();
        }

        private bool IsValid()
        {
            using (OleDbConnection conn = new OleDbConnection(_oleConnection))
            {
                conn.Open();
                DataTable tbl = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                int found = (from DataRow row in tbl.Rows
                                select ((string) row["TABLE_NAME"]).ToLower()
                            ).Count(tblName => tblName == "tbl_theme" || tblName == "tbl_category");
                return found == 2 ? true : false;
            }
        }

        private bool IsVersion2Plus()
        {
            using (OleDbConnection conn = new OleDbConnection(_oleConnection))
            {
                conn.Open();
                DataTable tbl = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                return (from DataRow row in tbl.Rows
                        select ((string) row["TABLE_NAME"]).ToLower()).Any(tblName => tblName == "tbl_list");
            }
        }

        private void Load()
        {
            if (IsThemeList)
                LoadCategories();
                LoadThemes();
            if (_databaseInfo.Version == null)
                LoadDatabaseInfo();
            _loaded = true;
        }

        private void LoadCategories()
        {
            using (OleDbConnection conn = new OleDbConnection(_oleConnection))
            {
                bool version1 = (Version[0] == '1');
                bool version2 = (Version[0] == '2');
                conn.Open();
                const string sql = "SELECT * FROM tbl_Category;";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                using (OleDbDataReader rdr = cmd.ExecuteReader())
                {
                    Category cat;
                    while (rdr.Read())
                    {
                        cat = new Category
                                  {
                                      Id = (int) rdr["Category_ID"],
                                      Name = rdr["Category_Name"] as string,
                                      ParentId = (rdr["Parent_Category_ID"] is DBNull) ? -1 : (int) rdr["Parent_Category_ID"],
                                      //Level = (rdr["Category_Level"] is DBNull) ? -1 : (int) rdr["Category_Level"]
                                  };
                        if (version1)
                        {
                            //cat.IsSubCategory = false;
                            cat.Link = null;
                            cat.Description = null;
                        }
                        if (version2)
                        {
                            //cat.IsSubCategory = (rdr["IsSubCategory"] is DBNull) ? false : (bool)rdr["IsSubCategory"];
                            cat.Link = rdr["CategoryLink"] as string;
                            cat.Description = rdr["Description"] as string;
                        }
                        // make lists of categories with the same category ID
                        if (!_categoriesInCategories.ContainsKey(cat.ParentId))
                            _categoriesInCategories.Add(cat.ParentId, new List<Category>());
                        _categoriesInCategories[cat.ParentId].Add(cat);
                    }
                }
            }
        }

        private void LoadThemes()
        {
            using (OleDbConnection conn = new OleDbConnection(_oleConnection))
            {
                bool version1 = (Version[0] == '1');
                //bool version2 = (Version[0] == '2');
                conn.Open();
                const string sql = "SELECT * FROM tbl_Theme;";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                using (OleDbDataReader rdr = cmd.ExecuteReader())
                {
                    Theme theme;
                    while (rdr.Read())
                    {
                        theme = new Theme
                                    {
                                        Id = (int) rdr["Theme_ID"],
                                        Name = rdr["Theme_Name"] as string,
                                        DataSource = rdr["Data_Source_Name"] as string,
                                        Type = rdr["Feature_Class_Type"] as string,
                                        Metadata = rdr["Metadata_Source_Name"] as string,
                                        CategoryId = (int) rdr["Category_ID"],
                                        ParentId = (rdr["Parent_Theme_ID"] is DBNull) ? -1 : (int) rdr["Parent_Theme_ID"]
                                    };
                        // ignore these fields from v1 and v2 (never used in any previous code):
                        // Data_Source_Type (string),  Definition Query (string), Legend_Source_Name (string), Default_Symbol_Color (string)
                        // Special_Code (string), Special_Code_Parms (string), Link_Document_Name (string), Link_Field (string)
                        if (version1)
                        {
                            theme.PubDate = new DateTime(1900, 01, 01);
                        }
                        else
                        {
                            theme.PubDate = (rdr["Date_Last_Updated"] is DBNull) ? new DateTime(1900, 01, 01)
                                                                              : (DateTime)rdr["Date_Last_Updated"];
                        }

                        // if parent ID = -1 and category ID = -1 then theme parent is the theme List (category ID = -1).
                        // if parent ID = -1 and category ID <> -1 then parent is a category (look for it in the category list)
                        // if parent ID <> -1 and category ID = -1 then parent is a theme (look for it in the theme list)
                        // if parent ID <> -1 and category <> -1 then parent is a theme (category is a grand parent).
                        // In a well formed database, the category should never be -1, so check parent ID first.

                        // make lists of themes with the same parent ID, if it is valid
                        if (theme.ParentId != -1)
                        {
                            if (!_themesInThemes.ContainsKey(theme.ParentId))
                                _themesInThemes.Add(theme.ParentId, new List<Theme>());
                            _themesInThemes[theme.ParentId].Add(theme);
                        }
                        // make lists of themes with the same catagory ID
                        // skip themes with a parent theme, they are in the other lists
                        else
                        {
                            if (!_themesInCategories.ContainsKey(theme.CategoryId))
                                _themesInCategories.Add(theme.CategoryId, new List<Theme>());
                            _themesInCategories[theme.CategoryId].Add(theme);
                        }
                    }
                }
            }
        }

        private void LoadDatabaseInfo()
        {
            using (OleDbConnection conn = new OleDbConnection(_oleConnection))
            {
                conn.Open();
                const string sql = "SELECT * FROM tbl_List;";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                using (OleDbDataReader rdr = cmd.ExecuteReader())
                {
                    _databaseInfo = new DatabaseInfo();
                    if (rdr.Read()) //only read the first record
                    {
                        _databaseInfo.Name = rdr["Name"] as string;
                        _databaseInfo.Description = rdr["Description"] as string;
                        _databaseInfo.Version = rdr["Version"] as string;
                        _databaseInfo.AuthorName = rdr["AuthorName"] as string;
                        _databaseInfo.AuthorTitle = rdr["AuthorTitle"] as string;
                        _databaseInfo.AuthorOrganization = rdr["AuthorOrganization"] as string;
                        _databaseInfo.AuthorAddress1 = rdr["AuthorAddress1"] as string;
                        _databaseInfo.AuthorAddress2 = rdr["AuthorAddress2"] as string;
                        _databaseInfo.AuthorPhone = rdr["AuthorPhone"] as string;
                        _databaseInfo.AuthorEmail = rdr["AuthorEmail"] as string;
                        _databaseInfo.Link = rdr["Link"] as string;
                    }
                    else
                    {
                        // If the first record is missing, fill in some default values
                        _databaseInfo.Name = "No Name";
                        _databaseInfo.Version = "2";
                    }
                }
            }
        }


        private void FillCategory(TmNode node, int catId)
        {
            //  A category can have categories and/or themes for children.
            List<Category> cats;
            _categoriesInCategories.TryGetValue(catId, out cats);
            if (cats != null)
                foreach (Category cat in cats)
                {
                    TmNode newNode = new CategoryNode(
                        cat.Name,
                        node,
                        MakeMetadataForCategory(cat.Link),
                        cat.Description
                        );
                    node.Add(newNode);
                    FillCategory(newNode, cat.Id);
                }

            List<Theme> themes;
            _themesInCategories.TryGetValue(catId, out themes);
            if (themes != null)
                foreach (Theme theme in themes)
                {
                    TmNode newNode = new ThemeNode(
                        theme.Name,
                        node,
                        MakeThemeData(theme.Name, theme.DataSource, theme.Type),
                        MakeMetadataForTheme(theme.Metadata),
                        null,
                        theme.PubDate
                        );
                    node.Add(newNode);
                    FillTheme(newNode, theme.Id);
                }
        }

        private void FillTheme(TmNode node, int themeId)
        {
            //  A theme can have only themes for children, we create new subtheme nodes..
            List<Theme> themes;
            _themesInThemes.TryGetValue(themeId, out themes);
            if (themes != null)
                foreach (Theme theme in themes)
                {
                    TmNode newNode = new SubThemeNode(
                        theme.Name,
                        node,
                        MakeThemeData(theme.Name, theme.DataSource, theme.Type),
                        MakeMetadataForTheme(theme.Metadata),
                        null,
                        theme.PubDate
                        );
                    node.Add(newNode);
                    FillTheme(newNode, theme.Id);
                }
        }

        // In versions < 3.0:
        // metadata for a category must have been a URL
        // metadata for a theme must have been an xml file,
        //       or a FGDB feature class with '::FGDB' appended
        //       or a PGDB feature class with '::PGDB' appended

        // datasource was typically a layer file, but it could also be an ArcCatalog datapath (i.e. ../file.gdb/feat_class

        private static Metadata MakeMetadataForTheme(string path)
        {
            Metadata newMetadata;
            //Apply ThemeManager < 3.0 coding strategy for metadata in Geodatabases
            if (!string.IsNullOrEmpty(path) && (path.EndsWith("::FGDB") || path.EndsWith("::PGDB")))
            {
                newMetadata = new Metadata(
                    path.Substring(0, path.Length - 6),
                    MetadataType.EsriDataPath,
                    MetadataFormat.Xml
                    );
            }
            else
                newMetadata = new Metadata(path,MetadataType.FilePath,MetadataFormat.Xml);
            return newMetadata;
        }


        private Metadata MakeMetadataForCategory(string link)
        {
            // All the category links in ThemeManager < 3.0 were URLs
            return new Metadata(link, MetadataType.Url, MetadataFormat.Html);
        }


        private static ThemeData MakeThemeData(string name, string path, string datatype)
        {
            if (string.IsNullOrEmpty(datatype))
            {
                //HACK - these are cleanup on the Alaska region themelist.
                //I should use arcCatalog to determine the type
                //or flag them for administrative action.
                if (name.Contains("MultiPatch"))
                    datatype = "MultiPatch";
                if (name.Contains("Image Service"))
                    datatype = "AGS";
                if (name.Contains("Microsoft Virtual Earth"))
                    datatype = "WMS";
            }
            return new ThemeData(path, datatype, null);
        }

        #region Nested type: Category

        private struct Category
        {
            internal string Description { get; set; }
            internal int Id { get; set; }
            //internal bool IsSubCategory { get; set;}
            //internal int Level { get; set;}
            internal string Link { get; set; }
            internal string Name { get; set; }
            internal int ParentId { get; set; }
        }

        #endregion

        #region Nested type: DatabaseInfo

        private struct DatabaseInfo
        {
            internal string AuthorAddress1 { private get; set;}
            internal string AuthorAddress2 { private get; set; }
            internal string AuthorEmail { private get; set; }
            internal string AuthorName { private get; set; }
            internal string AuthorOrganization { private get; set; }
            internal string AuthorPhone { private get; set; }
            internal string AuthorTitle { private get; set; }
            internal string Description { get; set; }
            internal string Link { get; set; }
            internal string Name { get; set; }
            internal string Version { get; set; }

            internal bool HasAuthor
            {
                get
                {
// ReSharper disable ConditionIsAlwaysTrueOrFalse
                    if ( string.IsNullOrEmpty(AuthorName) &&
                         string.IsNullOrEmpty(AuthorTitle) &&
                         string.IsNullOrEmpty(AuthorOrganization) &&
                         string.IsNullOrEmpty(AuthorAddress1) &&
                         string.IsNullOrEmpty(AuthorAddress2) &&
                         string.IsNullOrEmpty(AuthorPhone) &&
                         string.IsNullOrEmpty(AuthorEmail)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
                        )
                        return false;
                    return true;
                }
            }

            internal ThemeListAuthor Author
            {
                get
                {
                    if (!HasAuthor)
                        return null;
                    var author = new ThemeListAuthor {
                        Name = string.IsNullOrWhiteSpace(AuthorName) ? null : AuthorName,
                        Title = string.IsNullOrWhiteSpace(AuthorTitle) ? null : AuthorTitle,
                        Organization = string.IsNullOrWhiteSpace(AuthorOrganization) ? null : AuthorOrganization,
                        Address1 = string.IsNullOrWhiteSpace(AuthorAddress1) ? null : AuthorAddress1,
                        Address2 = string.IsNullOrWhiteSpace(AuthorAddress2) ? null : AuthorAddress2,
                        Phone = string.IsNullOrWhiteSpace(AuthorPhone) ? null : AuthorPhone,
                        Email = string.IsNullOrWhiteSpace(AuthorEmail) ? null : AuthorEmail,
                    };
                    return author;
                }
            }

        }

        #endregion

        #region Nested type: Theme

        private struct Theme
        {
            internal int CategoryId { get; set; }
            internal string DataSource { get; set; }
            internal DateTime PubDate { get; set; }
            internal int Id { get; set; }
            internal string Metadata { get; set; }
            internal string Name { get; set; }
            internal int ParentId { get; set; }
            internal string Type { get; set; }
        }

        #endregion

        //When writing to MDB, make sure to
        //Categories:
        //   add ID, parentID, Category Level and isSubCategory
        //   links must be URLs
        //Themes:
        //   add ThemeID, Category_ID, and Parent_theme,
        //   feature class type was a well know list of choices
        //   add '::FGDB' and or '::PGDB' to those types of metadata, do not support SDE or other FC
        //   if geodatabase metadata path was corrected to include feature dataset, remove from path
        //   all other metadata should be an xml file.
        //Theme List:
        //   only support certain keys from author properties

        /*
                        public int GetMaxThemeI()
                        {
                            string sql = "SELECT Max(theme_ID) FROM tbl_Theme";
                            using (OleDbConnection conn = new OleDbConnection(oleConnection))
                            {
                                conn.Open();
                                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                                {
                                    object maxid = cmd.ExecuteScalar();
                                    //return (maxid.GetType() == typeof(System.DBNull)) ? 1 : (int)maxid + 1;
                                    return (maxid is System.DBNull) ? 1 : (int)maxid + 1;
                                }
                            }
                        }


                        private long AddThemeToDB(List<Theme> themes, long targetCatID)
                        {
                            long mainTheme = GetMaxThemeI();
                            long currentTheme = mainTheme;

                            string sql = "INSERT INTO tbl_theme " +
                                          "(Theme_id, Theme_Name, Data_source_name, feature_class_type," +
                                          " category_id, Metadata_source_name, Date_Last_Updated, Parent_theme_ID) " +
                                         "VALUES " +
                                           "(@Theme_id, @Theme_Name, @Data_source_name, @feature_class_type," +
                                           " @category_id, @Metadata_source_name, @Date_Last_Updated, @Parent_theme_ID);";
                            using (OleDbConnection conn = new OleDbConnection(oleConnection))
                            {
                                conn.Open();
                                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                                {
                                    foreach (Theme theme in themes)
                                    {
                                        cmd.Parameters["Theme_id"].Value = currentTheme;
                                        cmd.Parameters["Theme_Name"].Value = theme.name;
                                        cmd.Parameters["Data_source_name"].Value = theme.datasource;
                                        cmd.Parameters["feature_class_type"].Value = theme.type;
                                        cmd.Parameters["category_id"].Value = targetCatID;
                                        cmd.Parameters["Metadata_source_name"].Value = theme.metadata;
                                        cmd.Parameters["Date_Last_Updated"].Value = theme.date;
                                        if (currentTheme == mainTheme)
                                            cmd.Parameters["Parent_theme_ID"].Value = null;
                                        else
                                            cmd.Parameters["Parent_theme_ID"].Value = mainTheme;
                                        cmd.ExecuteNonQuery();
                                        currentTheme++;
                                    }
                                }
                            }
                            return mainTheme;
                        }

                */
    }
}
