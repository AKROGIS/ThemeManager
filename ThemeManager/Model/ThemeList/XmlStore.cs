using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace NPS.AKRO.ThemeManager.Model.ThemeList
{
    [Serializable]
    class XmlStore : Store
    {
        private const string DefaultVersion = "1";
        [NonSerialized]
        private XElement _xml;

        public XmlStore(string path) : base(path)
        {
        }

        public override bool IsThemeList
        {
            get {
                if (_valid == null)
                    _valid = CheckThemeList();
                return (bool)_valid;
            }
        }

        private bool CheckThemeList()
        {
            if (_xml == null)
                Load();
            if (_xml == null)
                return false;
            return _xml.Name == ThemeListNode.TypeString;
        }

        public override string Version
        {
            get
            {
                if (_version == null)
                    _version = IsThemeList ? (string) _xml.Attribute("version") : string.Empty;
                return _version;
            }
        }

        public static XmlStore CreateNew(string path)
        {
            return CreateNew(path, DefaultVersion);
        }

        public static XmlStore CreateNewGZip(string path, string version)
        {
            if (string.IsNullOrEmpty(version))
                version = DefaultVersion;
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Save As: Path not provided");

            //test that we can create/write to path, user will get exception
            FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Close();

            if (!File.Exists(path))
                return null;
            return new XmlStore(path) { _version = version, _isReadOnly = false, _valid = true, _loaded = true };
        }

        public static XmlStore CreateNew(string path, string version)
        {
            if (string.IsNullOrEmpty(version))
                version = DefaultVersion;
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Save As: Path not provided");

            //test that we can create/write to path, user will get exception
            FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Close();

            if (!File.Exists(path))
                return null;
            return new XmlStore(path) { _version = version, _isReadOnly = false, _valid = true, _loaded = true };
        }

        private void Load()
        {
            if (_loaded)
                return;
            try
            {
                _xml = XElement.Load(_path);
                _loaded = true;
            }
            catch (Exception ex)
            {
                Debug.Print("XmlStore.Load(): Unable to load XML themelist\n" + ex);
            }
        }

        public override void Build(ThemeListNode node)
        {
            if (!IsThemeList)
                return;
            node.Load(_xml);
        }

        public override void Save(ThemeListNode node)
        {
            _xml = node.ToXElement();
            _xml.Save(_path);
        }

/*
        private string Fullpath(XElement xele)
        {
            string res = null;
            foreach (var x in xele.AncestorsAndSelf())
            {
                if (string.IsNullOrEmpty(res))
                    res = (string)x.Attribute("name");
                else
                    res = (string)x.Attribute("name") + "/" + res;

            }
            return res;
        }
*/
    }
}
