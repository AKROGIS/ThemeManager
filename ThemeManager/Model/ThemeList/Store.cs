using System;
using System.Diagnostics;
using System.IO;

namespace NPS.AKRO.ThemeManager.Model.ThemeList
{
    [Serializable]
    class Store
    {
        protected bool _loaded;
        protected bool? _readonly;
        protected bool? _valid;
        protected string _version;
        protected string _path;

        public Store(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Verifies that the datastore is actually a themelist
        /// Must be overridden by implementors with actual logic
        /// </summary>
        public virtual bool IsThemeList { get {return false; }}

        public virtual bool ReadOnly
        {
            get
            {
                return _readonly ?? (bool)(_readonly = !CanWriteToExistingPath);
            }
        }

        public virtual string Version { get { return _version; } }

        /// <summary>
        /// Returns true IFF the file path can be opened read/write.
        /// </summary>
        protected bool CanWriteToPath
        {
            get
            {
                return CanOpen(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
        }

        /// <summary>
        /// Returns true IFF the file path in this datastore exists and can be opened read/write.
        /// </summary>
        protected bool CanWriteToExistingPath
        {
            get
            {
                return CanOpen(FileMode.Open, FileAccess.ReadWrite);
            }
        }

        public virtual void Build(TmNode node) { }

        public virtual void Save(TmNode node) { }

        public virtual void Reset()
        {
            _version = null;
            _readonly = null;
            _valid = null;
            _loaded = false;
        }

        /// <summary>
        /// Checks to see if the file path can be opened.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <returns>true iff the file path in this datastore can be opened with the mode and access specified.</returns>
        /// <exception cref="Exception">This property catchs all exceptions and throws none.</exception>
        private bool CanOpen(FileMode mode, FileAccess access)
        {
            try
            {
                FileStream fs = File.Open(_path, mode, access);
                fs.Close();
                return true;
            }
            catch (Exception ex) //and ignore
            {
                Debug.Print("Caught and ignored exception\n"+ex);
            }
            return false;
        }
    }
}
