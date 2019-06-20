using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NPS.AKRO.ThemeManager.Model
{
    // This was a first try at having a variable set of attributes for the Author information
    // Any Key/Value pair could be part of the Author's attributes
    // On the UI, the dictionary would be bound to a DataGridView With the attribute column
    // using a combobox picklist for a set of set of default property names.
    // However this doesn't work because Dictionary does not support binding in WinForms
    // See https://stackoverflow.com/q/854953 for way to create a IBindableDictionary
    // Another philisophical problem is that ThemeListAuthor is not a Dictionary, rather it
    // uses a dictionary for storing its attributes.  A cleaner interface may be have a
    // property BindingList<KeyValuePair<TKey,TValue>> AttributeList that bound to the form.
    // See https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.bindinglist-1?view=netframework-4.5

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

        public static ThemeListAuthor Load(XElement xEle)
        {
            if (xEle == null)
                throw new ArgumentNullException(nameof(xEle));
            if (xEle.Name != "author")
                throw new ArgumentException("Invalid XElement");
            ThemeListAuthor author = new ThemeListAuthor();
            foreach (XElement entry in xEle.Elements("info"))
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
