// requires a reference to the COM interop library MSXML2 (Microsoft XML, v3.0)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
//using Microsoft.Win32;  //for registry access
using MSXML2;
using NPS.AKRO.ThemeManager.Properties;

namespace NPS.AKRO.ThemeManager.Model
{
    /*
     * What a nightmare.  ESRI is as of 9.3.1, ESRI's stylesheets are in an obsolete format that
     * can not be read by the .Net Framework.  An obsolete COM library (MSXML) must be used. However,
     * Microsoft does not support the use of any version of MSXML in .Net ( see
     * http://support.microsoft.com/?scid=kb%3Ben-us%3B815112&x=15&y=14)
     *
     * In ArcGIS 10 the stylesheets will use the a modern standard, However, Theme Manager will
     * need to support both formats for a while. So we won't have clean code for a while.
     */

    /* from Michael Kay (mich...@ntlworld.com) Jun 12, 2002 2:46:43 am on net.sourceforge.lists.saxon-help
     *
     * Microsoft produced an implementation of XSLT (it was called XSL at the time) based on an early
     * 1998 working draft of the language. Microsoft call this dialect XSL, most people call it
     * WD-xsl, (working draft XSL), and you will see that the namespace declaration in a stylesheet
     * that uses this dialect ends with "WD-xsl". No-one else supports this dialect of the language,
     * in fact Microsoft themselves have abandoned it.
     *
     * [.Net] can't process this dialect, which is very different from the final XSLT 1.0
     * Recommendation published in 1999. It detects that you are using this dialect by checking
     * the namespace on the xsl:stylesheet element. If you are new to XML and XSLT, don't touch
     * WD-xsl with a bargepole.
     */

    /* from Oleg Tkachenko [XML MVP] 28 May 2008 on http://dotnet.itags.org/dotnet-tech/249517/
     *
     * As a matter of interest, MSXML3 does support WD-XSL language. MSXML4
     * doesn't though. I think the reason why WD-XSL is still supported is that
     * default XML stylesheet (which works when you open styleless XML
     * document in IE) is written in WD-XSL (res://msxml3.dll/defaultss.xsl)
     * and can't be translated into XSLT due to differences in data model (e.g.
     * XSLT never can process XML declaration or CDATA sectin, while WD-XSL could).
     */


    internal class StyleSheet
    {
        private FreeThreadedDOMDocument _xlsOld;
        private XslCompiledTransform _xlstNew;

        /// <summary>
        /// Creates a new object for transforming XML
        /// </summary>
        /// <param name="path">Must be a local filesystem path</param>
        internal StyleSheet(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            //Pre-loading takes time, but ensures that only valid files are in list
            //_xlsOld = GetOldStyleXsl(path);
            //_xlstNew = GetNewStyleXsl(path);
        }

        internal string ErrorMessage {get; private set;}
        internal string Name {get; private set;}
        private string Path { get; set; }

        public override string ToString() {return Name;}

        internal string Xform(XmlDocument doc)
        {
            var stream = new StringWriter();
            XslCompiledTransform xlst = GetCachedNewStyleXsl();
            xlst.Transform(doc, null, stream);
            return stream.ToString();
        }

        internal string Xform(string url)
        {
            var xml = new FreeThreadedDOMDocument();
            xml.load(url);
            return OldStyleTransform(xml);
        }

        internal string TransformText(string text)
        {
            var xml = new FreeThreadedDOMDocument();
            xml.loadXML(text);
            return OldStyleTransform(xml);
        }

        private string OldStyleTransform(FreeThreadedDOMDocument xml)
        {
            return xml.transformNode(GetCachedOldStyleXsl());
        }

        private FreeThreadedDOMDocument GetCachedOldStyleXsl()
        {
            if (_xlsOld == null && string.IsNullOrEmpty(ErrorMessage))
                _xlsOld = GetOldStyleXsl(Path);
            if (_xlsOld == null)
                throw new Exception("No stylesheet was loaded\n" + ErrorMessage);
            return _xlsOld;
        }

        private FreeThreadedDOMDocument GetOldStyleXsl(string path)
        {
            try
            {
                var xls = new FreeThreadedDOMDocument();
                xls.load(path);
                FixAksoImageLinks(xls);
                ErrorMessage = "";
                return xls;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Unable to load " + path + "\n" + ex;
                return null;
            }
        }

        private void FixAksoImageLinks(FreeThreadedDOMDocument xls)
        {
            if (Name.StartsWith("AKSO"))
            {
                IXMLDOMNode node = xls.selectSingleNode("//img");
                if (node != null)
                {
                    IXMLDOMNode att = node.attributes.getNamedItem("src");
                    if (att != null)
                    {
                        string newDir = System.IO.Path.GetDirectoryName(Path);
                        foreach (string image in new[] { "AKSO_header.GIF", "akso_header_message2.jpg" })
                        {
                            if (att.text == image)
                            {
                                att.text = "file://" + System.IO.Path.Combine(newDir, image);
                            }
                        }
                    }
                }
            }
        }

        private XslCompiledTransform GetCachedNewStyleXsl()
        {
            if (_xlstNew == null && string.IsNullOrEmpty(ErrorMessage))
                _xlstNew = GetNewStyleXsl(Path);
            if (_xlstNew == null)
                throw new Exception("No stylesheet was loaded\n" + ErrorMessage);
            return _xlstNew;
        }

        private XslCompiledTransform GetNewStyleXsl(string path)
        {
            XslCompiledTransform xlst;
            try
            {
                xlst = new XslCompiledTransform();
                xlst.Load(path);
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                xlst = null;
                ErrorMessage = "Unable to load " + path + "\n" + ex;
            }
            return xlst;
        }

    }

    class StyleSheetsByEsri: List<StyleSheet>
    {
        private string _esriStyleSheetDir;

        public StyleSheetsByEsri()
        {
            //FIXME - catch and ignore file access related exceptions
            string dir = EmbededStyleSheetDir;
            if (string.IsNullOrEmpty(dir))
                return;

            foreach (string file in Directory.GetFiles(dir, "*.xsl", SearchOption.TopDirectoryOnly))
            {
                // ESRI provides two versions of each stylesheet (*.xls file)
                // We will skip the version that starts with "_"
                if (Path.GetExtension(file).ToLower() == ".xsl" &&
                    Path.GetFileName(file)[0] != '_')
                    Add(new StyleSheet(file));
            }
        }

        // return a valid directory or empty string
        internal string EmbededStyleSheetDir
        {
            get
            {
                if (_esriStyleSheetDir == null)
                {
                    //need a full path for images in stylesheets
                    _esriStyleSheetDir = Path.Combine(
                        //The assembly returns an bad path (prefixed with file:/)
                        //Path.GetDirectoryName(Assembly.GetAssembly(typeof(StyleSheet)).CodeBase),
                        // The Application object is winform specific what if I swithch to WPF
                        Path.GetDirectoryName(Application.ExecutablePath),
                        Settings.Default.StyleSheetDirectory
                    );
                    Debug.Print("Stylesheet directory = {0}, Exists = {1}", _esriStyleSheetDir, Directory.Exists(_esriStyleSheetDir));
                }
                return _esriStyleSheetDir;
            }
        }

        // return a valid directory or an empty string
/*
        private static string OldGetESRIStyleSheetDir()
        {
            string[] dirs = {
                //FIXME - catch Registry access exceptions
                (string)Registry.GetValue(Settings.Default.RegistryArcDir,
                                          Settings.Default.RegistryArcKey, null),
                (string)Registry.GetValue(Settings.Default.RegistryArc8Dir,
                                          Settings.Default.RegistryArcKey, null),
                Settings.Default.ArcDir,
                Settings.Default.Arc64Dir
            };
            foreach (string dir in dirs)
            {
                string path;
                try
                {
                    path = Path.Combine(dir, Settings.Default.StylesheetSubDir);
                }
                catch (ArgumentException ex)
                {
                    path = null;
                    Debug.Print("Unable to build path" + ex);
                }
                if (Directory.Exists(path))
                {
                    //FIXME - check check for *.xls in directory - if none, then skip this folder
                    return path;
                }
            }
            // FIXME - Ask the user to browse to the Install directory and then save in settings.
            return null;
        }
*/
    }
}
