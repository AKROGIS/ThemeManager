using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace NPS.AKRO.ThemeManager.Model
{
    [Serializable]
    class ThemeListAuthor : INotifyPropertyChanged
    {

        #region Properties

        // Properties need to be public and support INotifyPropertyChanged,
        // because they are bound to a WinForm text box.
        private string _name;
        private string _title;
        private string _organization;
        private string _address1;
        private string _address2;
        private string _email;
        private string _phone;

        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }
        public string Title
        {
            get { return _title; }
            set { SetField(ref _title, value); }
        }
        public string Organization
        {
            get { return _organization; }
            set { SetField(ref _organization, value); }
        }
        public string Address1
        {
            get { return _address1; }
            set { SetField(ref _address1, value); }
        }
        public string Address2
        {
            get { return _address2; }
            set { SetField(ref _address2, value); }
        }
        public string Email
        {
            get { return _email; }
            set { SetField(ref _email, value); }
        }
        public string Phone
        {
            get { return _phone; }
            set { SetField(ref _phone, value); }
        }

        #endregion

        #region Serialize to/from XML
        // This XML structure is different from other objects to support a variable set of attributes in the future

        public XElement AsXElement
        {
            get
            {
                return new XElement("author",
                    new XElement("info", new XAttribute("type", "Name"), Name ?? ""),
                    new XElement("info", new XAttribute("type", "Title"), Title ?? ""),
                    new XElement("info", new XAttribute("type", "Organization"), Organization ?? ""),
                    new XElement("info", new XAttribute("type", "Address1"), Address1 ?? ""),
                    new XElement("info", new XAttribute("type", "Address2"), Address2 ?? ""),
                    new XElement("info", new XAttribute("type", "Email"), Email ?? ""),
                    new XElement("info", new XAttribute("type", "Phone"), Phone ?? "")
                );
            }
        }

        public static ThemeListAuthor Load(XElement xEle)
        {
            if (xEle == null)
                throw new ArgumentNullException(nameof(xEle));
            if (xEle.Name != "author")
                throw new ArgumentException("Invalid XElement");
            var author = new ThemeListAuthor();
            foreach (XElement entry in xEle.Elements("info"))
            {
                string attributeType = (string)entry.Attribute("type");
                string value = string.IsNullOrWhiteSpace(entry.Value) ? null : entry.Value;
                switch (attributeType)
                {
                    case "Name":
                        author.Name = value;
                        break;
                    case "Title":
                        author.Title = value;
                        break;
                    case "Organization":
                        author.Organization = value;
                        break;
                    case "Address1":
                        author.Address1 = value;
                        break;
                    case "Address2":
                        author.Address2 = value;
                        break;
                    case "Email":
                        author.Email = value;
                        break;
                    case "Phone":
                        author.Phone = value;
                        break;
                }
            }
            return author;
        }

        #endregion

        #region INotifyPropertyChanged

        // From code provided by Marc Gravell (https://stackoverflow.com/a/1316417)

        [field: NonSerializedAttribute]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
