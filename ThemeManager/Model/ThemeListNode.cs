using System;
using NPS.AKRO.ThemeManager.Model.ThemeList;
using System.IO;

namespace NPS.AKRO.ThemeManager.Model
{
    [Serializable]
    class ThemeListNode : TmNode
    {
        //The TypeName is persisted in image resources and data files, so it is historic,
        //case sensitive, and needs to be impervious to refactoring
        new public const string TypeString = "themelist";

        private ThemeListStatus _status;
        private Store _dataStore;

        // Load from XML
        public ThemeListNode()
            : this(null, null, null, null)
        {
        }

        //File->New, File->Open, Load from Registry
        public ThemeListNode(string name, string path)
            : this(name, path, null, null)
        {
        }

        // Category.ToThemeList, overloads
        public ThemeListNode(string name, string path, Metadata metadata, string desc)
            : base(name, null, metadata, desc)
        {
            IsInitialized = false;
            FilePath = path;
            Author = new ThemeListAuthor();
            _status = ThemeListStatus.Created;
            GetDataSource();
            // TryToGetDataStore() will set _readonly for theme
            ThemeList = this;
            IsInitialized = true;
        }

        // Reloading a theme list has two potential meanings.
        // 1) reset the theme list to the previously saved state.
        // 2) reload all themes in the theme list. (inherited behavior)
        // If we want the 1st behavior we will write a new method instead of
        // overriding the existing reload method.
        //public override void Reload()

        public override string TypeName { get { return TypeString; } }

        //FIXME: make set private (requires fix in TmNode.Load)
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (value != _filePath)
                {
                    _filePath = value;
                    GetDataSource();
                }
            }
        }
        private string _filePath;

        public string FileType { set; get; }

        public bool IsValid { get { return _dataStore != null; } }

        public bool NeedsLoading { get { return _status == ThemeListStatus.Initialized; } }

        public override bool HasData { get { return File.Exists(FilePath); } }

        /// <summary>
        /// True if this themelist has items in it that have changed.
        /// </summary>
        public bool IsDirty
        {
            get { return _status == ThemeListStatus.Dirty; }
            set
            {
                if (value) // && _status == ThemeListStatus.Loaded)
                    _status = ThemeListStatus.Dirty;
                if (value == false && _status == ThemeListStatus.Dirty)
                    _status = ThemeListStatus.Loaded;
            }
        }

        public override bool IsEditable
        {
            get
            {
                return (_dataStore == null || _dataStore.IsEditable);
            }
        }

        public ThemeListAuthor Author
        { get; set; }

        public string Version
        {
            get
            {
                if (IsValid)
                    return _dataStore.Version;
                return "";
            }
        }

        public void GetDataSource()
        {
            _dataStore = TryToGetDataStore();
        }

        private Store TryToGetDataStore()
        {
            // If we return from this routine prematurely, then we do not have a valid datastore
            _status = ThemeListStatus.Created;

            if (!HasData)
                return null;

            //if (string.IsNullOrEmpty(Data.Path))
            //    return null;

            Store datastore;
            string ext = Path.GetExtension(FilePath);
            if (ext != null)
                ext = ext.ToLower();
            switch (ext)
            {
                case ".tmz":
                    throw new NotImplementedException();
                case ".tml":
                case ".xml":
                    datastore = new XmlStore(FilePath); break;
                case ".mdb":
                    datastore = new MdbStore(FilePath); break;
                default:
                    return null;
            }
            if (!datastore.IsThemeList)
                return null;

            _status = ThemeListStatus.Initialized;

            return datastore;
        }


        // Build is not thread safe.  In a multi-threaded app, it must be called in side a lock.
        // Locking is the callers responsibility.
        public void Build()
        {
            if (!IsValid)
                throw new Exception("File is not a valid ThemeList");

            if (_status == ThemeListStatus.Initialized)
            {
                _status = ThemeListStatus.Loading;

                _dataStore.Build(this);
                //UpdateImageIndex(true);

                _status = ThemeListStatus.Loaded;
            }
        }

        public TmNode BuildNodeFromId(int themeId)
        {
            if (!IsValid)
                throw new Exception("This is not a valid ThemeList");
            var dataStore = _dataStore as MdbStore;
            if (dataStore != null)
                return (dataStore).BuildThemeFromId(themeId);
            throw new Exception("This is not a MDB ThemeList");
        }

        public void Save()
        {
            if (this is ThemeListNode && _status == ThemeListStatus.Dirty)
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
            // create backup of current settings in case save doesn't work.
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
            FilePath = path;
            FileType = ext.Substring(1, 3);
            _status = ThemeListStatus.Dirty;

            try
            {
                Save();
            }
            catch (Exception)
            {
                _dataStore = oldDatastore;
                _status = oldStatus;
                throw;
            }
        }
    }
}
