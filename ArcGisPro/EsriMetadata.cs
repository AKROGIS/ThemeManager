using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NPS.AKRO.ThemeManager.ArcGIS
{
    class EsriMetadata
    {
        private readonly static Dictionary<string, string> _cache = new Dictionary<string, string>();

        internal async static Task<string> GetContentsAsXmlAsync(string datapath)
        {
            if (datapath == null)
                throw new ArgumentNullException("datapath");
            if (datapath == string.Empty)
                throw new ArgumentException("datapath is empty string");

            if (!_cache.ContainsKey(datapath))
            {
                await EsriLicense.GetLicenseAsync();
                await LoadAsync(datapath);
            }
            if (_cache.ContainsKey(datapath))
                return _cache[datapath];
            return null;
        }

        private static async Task LoadAsync(string path)
        {
            if (path.ToLower().Contains(@".gdb\"))
            {
                var text = await GetMetaDataFromFgdbAsync(path);
                _cache[path] = text ?? throw new ApplicationException($"Unable to load FGDB metadata from {path}");
                return;
            }

            // TODO: Support *.mdb, *.sde, Pro style connection strings, and Others?
            throw new ApplicationException("Pro cannot get metadata from this kind of data source");
        }

        private static async Task<string> GetMetaDataFromFgdbAsync(string path)
        {
            var index = path.ToLower().IndexOf(@".gdb\") + 4; 
            var gdbPath = path.Substring(0, index);
            var datasetPath = path.Substring(index, path.Length - index);
            var gdb = new Fgdb(gdbPath);
            await gdb.OpenAsync();
            string xml = await gdb.GetMetadataAsync(datasetPath);
            gdb.Close();
            return xml;
        }

    }
}
