using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NPS.AKRO.ThemeManager.Model
{
    [Serializable]
    class ThemeListAuthor : Dictionary<string, string>, ICloneable
    {
        public static string[] DefaultKeys = {
                                        "Name", 
                                        "Title", 
                                        "Organization",
                                        "Address1", 
                                        "Address2", 
                                        "Email", 
                                        "Phone"
                                      };

        public XElement AsXElement
        {
            get
            {
                return new XElement("author",
                    from entry in this
                    //select new XElement(entry.Key, entry.Value)
                    select new XElement("info", 
                        new XAttribute("type", entry.Key), 
                        entry.Value)
                    );
            }
        }

        public static ThemeListAuthor Load(XElement xele)
        {
            if (xele == null)
                throw new ArgumentNullException("xele");
            if (xele.Name != "author")
                throw new ArgumentException("Invalid Xelement");
            ThemeListAuthor author = new ThemeListAuthor();
            foreach (XElement entry in xele.Elements("info"))
            {
                string attributeType = entry.Attributes("type").ToString();
                if (!string.IsNullOrEmpty(attributeType))
                    author[attributeType] = entry.Value;
            }
            return author;
        }

        #region ICloneable Members

        public object Clone()
        {
            ThemeListAuthor author = (ThemeListAuthor)MemberwiseClone();
            return author;
        }

        #endregion
    }
}
