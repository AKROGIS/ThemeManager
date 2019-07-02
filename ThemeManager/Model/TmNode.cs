using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Properties;

namespace NPS.AKRO.ThemeManager.Model
{
    /// <summary>
    /// Represents a node in the Theme Manager Tree
    /// </summary>
    [Serializable]
    class TmNode : INotifyPropertyChanged
    {
        public TmNode(string name, TmNode parent, Metadata metadata, string desc)
        {
            IsInitialized = false;
            Name = name;
            Description = desc;
            Metadata = metadata;
            Parent = parent;
            if (parent != null)
                IsUpdating = parent.IsUpdating;
            IsInitialized = true;
            //FIXME: Check that I don't send messages or updates while Initializing.
        }

        #region Simple Properties
        //The TypeName is persisted in image resources and data files, so it is historic,
        //case sensitive, and needs to be impervious to refactoring
        public const string TypeString = "unknown";

        /// <summary>
        /// A string constant used to identify this type of TMNode in icons and xml
        /// </summary>
        public virtual string TypeName { get { return TypeString;} }

        /// <summary>
        /// The display name of this node.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                //FIXME - Check that name does not have a path separator character
                if (value != _name)
                {
                    _name = value;
                    MarkAsChanged();
                    OnPropertyChanged("Name");
                }
            }
        }
        private string _name;

        /// <summary>
        /// A short description of the contents of this nodel.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    MarkAsChanged();
                    OnPropertyChanged("Description");
                }
            }
        }
        private string _description;

        /// <summary>
        /// Returns true if this node is updating (No events are fired during updates), false otherwise
        /// </summary>
        /// <remarks>
        /// Inherited from the parent at creation.  Defaults to false if there is no parent.
        /// If this node is re-parented, it does NOT change it's IsUpdating state to match.
        /// </remarks>
        private bool IsUpdating { get; set; }

        /// <summary>
        /// Visibility is used by the UI control to determine which node to display
        /// It is a combination of the Hidden flag, and the current search/filter settings
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                //if (value != _isVisible)
                //{
                    _isVisible = value;
                    //OnPropertyChanged();
                //}
            }
        }
        private bool _isVisible;

        /// <summary>
        /// Hidden is a user controlled setting on each item (usually categories) to
        /// remove unwanted categories from view.
        /// </summary>
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (value != _isHidden)
                {
                    _isHidden = value;
                    OnPropertyChanged("IsHidden");
                }
            }
        }
        private bool _isHidden;

        /// <summary>
        /// True if this node is selected for an action command in the UI
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }
        //This is not serialized, because we do not want copied objects to be selected
        [NonSerialized]
        private bool _isSelected;

        /// <summary>
        /// Changes to this object while IsInitialied is false, do not need to be saved.
        /// </summary>
        protected bool IsInitialized { get; set; }

        /// <summary>
        /// Returns true if this node can be edited, false otherwise
        /// </summary>
        /// <remarks>
        /// This is implemented for all nodes to quickly let the UI, know if it can be edited
        /// </remarks>
        public virtual bool IsEditable { get {return (ThemeList == null || ThemeList.IsEditable);}}

        /// <summary>
        /// Returns true if this node is readonly, false otherwise
        /// </summary>
        public bool IsReadOnly { get { return !IsEditable; } }

        public virtual bool HasData  { get { return false;} }

        public virtual bool HasDataToPreview { get { return false; } }

        public virtual bool IsLaunchable { get { return false; } }

        public virtual void Launch() { }
#endregion

        #region Familial relations

        //If ThemeList is serialized with a given TmNode, then the whole tree will be serialized whenever
        //a single node is serialized.  Unfortunately an automatic property cannot be marke NonSerialized.
        //ThemeList will be reinitialized when the parent property is reinitialized (via Parent set accessor).
        //until then a re-serialized node is an orphan.
        [NonSerialized]
        private ThemeListNode _themelist;
        protected ThemeListNode ThemeList { get { return _themelist; } set { _themelist = value; } }

        //serializing the parent will serialize the grandparent and all it's children ...
        //since we don't want to serialize the whole tree for each node, we exclude the parent
        //we need to reinitialize the parent after re-serialization
        //until that happens the re-serialized node is an orphan.
        //This also applies to all the decendents below this node

        //The solution (Hack) is to use recursion in the Parent set accessor.
        //the parent for the root of the re-serialized node will be set when it
        //is added into the tree somewhere.
        [NonSerialized]
        private TmNode _parent;

        public TmNode Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    if (_parent != null)
                    {
                        if (_parent.ThemeList != null)
                            ThemeList = _parent.ThemeList;
                        //Recursively reset the parent for all descendents.
                        // This is done to ensure copyied/dragged nodes get the right ancestory on reserialization.
                        //FIXME - this could be done more elegently during re-serialization
                        if (HasChildren)
                            foreach (var child in Children)
                                child.Parent = this;
                    }
                }
            }
        }

        public bool HasChildren
        {
            get { return _children != null && _children.Count > 0; }
        }



        public IEnumerable<TmNode> Children
        {
            get { return _children ?? (_children = new List<TmNode>()); }
        }
        private IList<TmNode> _children;

        internal void Add(TmNode child)
        {
            if (_children == null)
                _children = new List<TmNode>();
            _children.Add(child);
            child.Parent = this;

            //My age is based on age of my youngest child
            Age = CalculateAge();

            MarkAsChanged();
            OnChildAdded(child, _children.Count - 1);
        }

        private void Remove(TmNode child)
        {
            if (_children != null && child != null)
            {
                _children.Remove(child);

                //My age is based on age of my youngest child
                Age = CalculateAge();

                MarkAsChanged();
                OnChildRemoved(child);
            }
        }

        internal void Delete()
        {
            Debug.Assert(Parent != null, "Can't delete a node with no parent");
            if (Parent != null)
                Parent.Remove(this);
        }

        #endregion

        #region Metadata
        //TODO: decide if Metadata belongs as a property of all TMNodes or just ThemeNodes
        // issue - metadata provides for a richer description of a category or themelist
        //         however metadata properties pubdate, summary, tags, are not exposed to
        //         to the UI except on themes
        //TODO: Decide if node should copy or reference metadata properties
        // issue - node may have a description or pubdate but not metadata
        public bool HasMetadata
        {
            get
            {
                return (Metadata != null && !string.IsNullOrEmpty(Metadata.Path));
            }
        }

        public Metadata Metadata
        {
            get
            {
                if (_metadata == null)
                    Metadata = new Metadata();
                return _metadata;
            }
            set
            {
                if (_metadata != value)
                {
                    if (_metadata != null)
                        _metadata.PropertyChanged -= Metadata_PropertyChanged;
                    _metadata = value;
                    if (_metadata != null)
                        _metadata.PropertyChanged += Metadata_PropertyChanged;
                    //FIXME: Replacing the metadata object implicitly changes the metadata properties.
                    //Should I call Metadata_PropertyChanged to ensure I am up to date, or is this
                    //too expensive?    For now I just set ThemeList.IsDirty
                    MarkAsChanged();
                }
            }
        }
        private Metadata _metadata;

        public string Summary
        {
            get { return _summary; }
            set
            {
                if (value != _summary)
                {
                    _summary = value;
                    MarkAsChanged();
                    OnPropertyChanged("Summary");
                }
            }
        }

        private string _summary;

        public string Tags
        {
            get { return _tags; }
            set
            {
                if (value != _tags)
                {
                    _tags = value;
                    MarkAsChanged();
                    OnPropertyChanged("Tags");
                }
            }
        }
        private string _tags;

        public void SyncWithMetadata(bool recurse)
        {
            var info = Metadata.GetGeneralInfo();
            Tags = info.Tags ?? "";
            Summary = info.Summary ?? "";
            Description = info.Description ?? "";

            // PubDate is a property of ThemeNode
            //FIXME: Remove this subclass dependency
            var themeNode = this as ThemeNode;
            if (themeNode != null) {
                themeNode.SetPubDate(info.PublicationDate);
            }

            if (recurse)
                foreach (TmNode child in Children)
                    child.SyncWithMetadata(true);
        }

        private void Metadata_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MarkAsChanged();
            // Do not sync the theme properties with the changed metadata path
            // User may have manually set those properties.
            // After Node/metadata is created, a sync should only be done by user.
        }

        #endregion

        #region Age

        public int Age
        {
            get { return _age; }
            protected set
            {
                if (value != _age)
                {
                    //Debug.Print("Setting age for {0} to {1}", this, value);
                    _age = value;
                    ImageName = CalculateImageName();
                    if (IsInitialized && Parent != null && !IsUpdating)
                        Parent.Age = Parent.CalculateAge();
                }
            }
        }
        private int _age = int.MaxValue;

        /// <summary>
        /// Calculates the age of the this node, assumes all dependencies are up to date
        /// </summary>
        /// <returns>The age in days.</returns>
        virtual protected int CalculateAge()
        {
            //Debug.Print("Calculating Age for {0}", this);
            if (HasChildren)
            {
                return Children.Select(c => c.Age).Min();
            }
            return int.MaxValue;
        }

        /// <summary>
        /// Updates the age of this node, by first updating all nodes which might influence the age
        /// </summary>
        /// <remarks>
        /// Checks every node only once, so it does not rely on events (_issueUpdates == false),
        /// Therefore it can (and should) be done inside a Begin/EndUpdate pair
        /// </remarks>
        virtual internal void UpdateAge()
        {
            if (HasChildren)
                foreach (var child in Children)
                    child.UpdateAge();
            Age = CalculateAge();
        }
        #endregion

        #region Icon

        public string ImageName
        {
            get { return _imageName ?? (_imageName = CalculateImageName()); }
            protected set
            {
                if (value != _imageName)
                {
                    _imageName = value;
                    OnPropertyChanged("ImageName");
                }
            }
        }
        private string _imageName;

        /// <summary>
        /// Determines the correct image name for this node
        /// </summary>
        /// <remarks>
        /// This is dependent on Age, Readonly and TypeCode being up to date.
        /// When those properties change, the setter must update ImageName with this method.
        /// </remarks>
        /// <returns></returns>
        protected string CalculateImageName()
        {
            //FIXME: make this an override, so we do not have subclass dependencies

            // This is dependent on Age, Readonly and TypeCode being up to date.
            // When those properties change, they update ImageName with this method.
            // If other dependencies are introduced into this method, ensure the same happens.
            string key = TypeName;
            if (this is ThemeNode || this is SubThemeNode)
                key = "Theme" + ((ThemeNode)this).GetDataTypeCode();
            if (IsReadOnly && this is ThemeListNode)
                key = "ThemeListlock";
            if (Age < Settings.Default.AgeInDays)
                key = key + "new";
            return key;
        }

        /// <summary>
        /// Recursively updates all the ImageNames at this node and below.
        /// </summary>
        /// <remarks>
        /// This will only be necessary when conditions change, i.e. Settings.Default.AgeInDays
        /// FIXME: Should settings be a static variable that triggers this behavior.
        /// </remarks>
        internal void UpdateImageNames()
        {
            if (HasChildren)
                foreach (var child in Children)
                    child.UpdateImageNames();
            ImageName = CalculateImageName();
        }

        #endregion

        #region Load/Store

        /// <summary>
        /// Refresh this node from permanent storage
        /// </summary>
        public virtual void Reload()
        {
            if (HasChildren)
                foreach (TmNode child in Children)
                    child.Reload();
        }

        public void Load(XElement xele)
        {
            Load(xele, true);
        }

        public void Load(XElement xele, bool recurse)
        {
            if (xele == null)
                throw new ArgumentNullException("xele");

            //Kind was already set when node was created.  Set all other properties
            Name = (string)xele.Attribute("name");
            XElement data = xele.Element("tags");
            Tags = data == null ? null : data.Value;
            data = xele.Element("summary");
            Summary = data == null ? null : data.Value;
            data = xele.Element("description");
            Description = data == null ? null : data.Value;
            data = xele.Element("pubdate");
            if (data != null)
            {
                DateTime temp;
                if (DateTime.TryParse(data.Value, out temp))
                    if (this is ThemeNode)
                        ((ThemeNode)this).PubDate = temp;
            }

            data = xele.Element("data");
            if (data != null)
            {
                // Ignore data elements for themelists.
                var themeNode = this as ThemeNode;
                if (themeNode != null)
                    (themeNode).Data = ThemeData.Load(data);
                var themeListNode = this as ThemeListNode;
                if (themeListNode != null && themeListNode.FilePath == null && data.Value != null)
                    // If the path of a themelist is already set (by the open dialog), do not restore the old path
                    themeListNode.FilePath = data.Value;
            }

            data = xele.Element("metadata");
            if (data != null)
                Metadata = Metadata.FromXElement(data);

            data = xele.Element("author");
            if (data != null)
                if (this is ThemeListNode)
                        ((ThemeListNode)this).Author = ThemeListAuthor.Load(data);

            if (recurse)
            {
                TmNode childNode;

                foreach (XElement xnode in xele.Elements(CategoryNode.TypeString))
                {
                    childNode = new CategoryNode();
                    Add(childNode);
                    childNode.Load(xnode);
                }

                foreach (XElement xnode in xele.Elements(ThemeNode.TypeString))
                {
                    if (this is SubThemeNode || this is ThemeNode)
                        childNode = new SubThemeNode();
                    else
                        childNode = new ThemeNode();
                    Add(childNode);
                    childNode.Load(xnode);
                }
            }
        }

        public XElement ToXElementWithoutChildren()
        {
            var xele = new XElement(
                 TypeName,
                 this is ThemeListNode ? new XAttribute("version", ((ThemeListNode)this).Version) : null,
                 new XAttribute("name", Name ?? ""),
                 (Tags != null) ? new XElement("tags", Tags) : null,
                 (Summary != null) ? new XElement("summary", Summary) : null,
                 (Description != null) ? new XElement("description", Description) : null,
                 this is ThemeNode ? new XElement("pubdate", ((ThemeNode)this).PubDate) : null,
                 this is ThemeListNode ? ((ThemeListNode)this).Author.AsXElement : null,
                 this is ThemeNode ? ((ThemeNode)this).Data.ToXElement() : null,
                 this is ThemeListNode ? new XElement("data", ((ThemeListNode)this).FilePath) : null,
                 HasMetadata ? Metadata.ToXElement() : null
                 );
            return xele;
        }

        public XElement ToXElement()
        {
            XElement xele = ToXElementWithoutChildren();
            xele.Add(
                  from tmnode in Children
                  select tmnode.ToXElement()
                 );
            return xele;
        }
        #endregion

        #region Searching

        public bool Matches(SearchParameters searchParameters)
        {
            if (searchParameters == null || searchParameters.Count < 1)
                return false;
            if (this is SubThemeNode)
                return false;
            if ((this is CategoryNode || this is ThemeListNode) && !searchParameters.SearchCategories)
                return false;
            if (this is ThemeNode && !searchParameters.SearchThemes)
                return false;

            if (this is ThemeNode)
            {
                bool subThemeMatch = Children.SelectMany(n => n.Recurse(node => node.Children)
                                                                  .Where(x => x.Matches2(searchParameters))).Any();
                return Matches2(searchParameters) || subThemeMatch;
            }
            return Matches2(searchParameters);
        }

        private bool Matches2(SearchParameters searchParameters)
        {
            foreach (SearchOptions search in searchParameters)
            {
                bool lastSearch = (!searchParameters.MatchAll) || searchParameters.IsLast(search);
                bool found;
                switch (search.SearchType)
                {
                    case SearchType.ThemeName:
                        found = MatchName(search); break;
                    case SearchType.ThemeSummary:
                        found = MatchSummary(search); break;
                    case SearchType.ThemeDescription:
                        found = MatchDescription(search); break;
                    case SearchType.ThemeTags:
                        found = MatchTags(search); break;
                    case SearchType.Theme:
                        found = MatchName(search) || MatchSummary(search) || MatchDescription(search) || MatchTags(search); break;
                    case SearchType.PubDate:
                        found = MatchDate(search); break;
                    case SearchType.Metadata:
                        found = MatchMeta(search); break; //FIXME - since this is in a loop, we may load metadata multiple times
                    default:
                        Debug.WriteLine("Invalid Search Condition");
                        found = false; break;
                }
                if (lastSearch && found)
                    return true;
                if (searchParameters.MatchAll && !found)
                    return false;
            }
            return false;
        }

        private bool MatchName(SearchOptions search)
        {
            return MatchText(search, Name);
        }

        private bool MatchDescription(SearchOptions search)
        {
            return MatchText(search, Description);
        }

        private bool MatchSummary(SearchOptions search)
        {
            return MatchText(search, Summary);
        }

        private bool MatchTags(SearchOptions search)
        {
            return MatchText(search, Tags);
        }

        private static bool MatchText(SearchOptions search, string text)
        {
            bool found;
            if (search.FindAll)
            {
                found = true;
                if (search.SearchWords.Any(searchString => !text.Contains(searchString, search.ComparisonMethod)))
                    return false;
            }
            else //FindAny
            {
                found = false;
                if (search.SearchWords.Any(searchString => text.Contains(searchString, search.ComparisonMethod)))
                    return true;
            }
            return found;
        }

        private bool MatchDate(SearchOptions search)
        {
            int ageInDays = Age;
            if (search.HasMinDays && search.HasMaxDays)
                if (search.MinDaysSinceEdit < search.MaxDaysSinceEdit)
                    return ((search.MinDaysSinceEdit <= ageInDays)
                             && (ageInDays <= search.MaxDaysSinceEdit));
                else
                    return ((search.MinDaysSinceEdit <= ageInDays)
                             || (ageInDays <= search.MaxDaysSinceEdit));
            if (search.HasMinDays && search.MinDaysSinceEdit <= ageInDays)
                return true;
            if (search.HasMaxDays && ageInDays <= search.MaxDaysSinceEdit)
                return true;
            return false;
        }

        private bool MatchMeta(SearchOptions search)
        {
            if (!HasMetadata)
                return false;
            return Metadata.SearchContent(search);
        }

        #endregion

        #region Path Names
        //FIXME - Use a Path Separator variable.
        private string FullCategoryPath()
        {
            if (Parent == null)
                return Name;
            return Parent.FullCategoryPath() + "\\" + Name;
        }

        public string CategoryPath()
        {
            string[] path = FullCategoryPath().Split('\\');
            string results = "";
            for (int i = 1; i < path.Length - 1; i++)
                if (results == "")
                    results = path[i];
                else
                    results = results + "\\" + path[i];
            return results;
        }

        public override string ToString()
        {
            return FullCategoryPath();
        }

        #endregion

        #region Helper functions
        protected void MarkAsChanged()
        {
            if (IsInitialized && ThemeList != null)
                ThemeList.IsDirty = true;
        }
        #endregion

        #region events

        /// <summary>
        /// Fires the NodeHasBeenUpdated event
        /// </summary>
        /// <remarks>
        /// Primarily intended to let foreground threads know the node was changed in the background.
        /// Also Major updates are done inside an Begin/EndUpdate pair, and no
        /// events are fired.  This can be called to let all event subscribers
        /// know that this node (and likely all sub nodes) have changed.
        /// </remarks>
        internal void BroadcastNodeHasBeenUpdatedEvent()
        {
            OnNodeHasBeenUpdated();
        }

        public void BeginUpdate()
        {
            IsUpdating = true;
            if (HasChildren)
                foreach (var child in Children)
                    child.BeginUpdate();
        }

        public void EndUpdate()
        {
            IsUpdating = false;
            if (HasChildren)
                foreach (var child in Children)
                    child.EndUpdate();
        }


        #region INotifyPropertyChanged
        [field: NonSerializedAttribute]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handle = PropertyChanged;
            if (handle != null && !IsUpdating && IsInitialized)
                handle(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        [field: NonSerializedAttribute]
        public event TmNodeEventHandler ChildAdded;

        [field: NonSerializedAttribute]
        public event TmNodeEventHandler ChildRemoved;

        [field: NonSerializedAttribute]
        public event EventHandler NodeHasBeenUpdated;

        protected void OnChildAdded(TmNode node, int index)
        {
            TmNodeEventHandler handle = ChildAdded;
            if (handle != null && !IsUpdating && IsInitialized)
                handle(this, new TmNodeEventArgs { Index = index, Node = node });
        }

        protected void OnChildRemoved(TmNode node)
        {
            TmNodeEventHandler handle = ChildRemoved;
            if (handle != null && !IsUpdating && IsInitialized)
                handle(this, new TmNodeEventArgs { Index = -1, Node = node });
        }

        protected void OnNodeHasBeenUpdated()
        {
            EventHandler handle = NodeHasBeenUpdated;
            if (handle != null && !IsUpdating && IsInitialized)
                handle(this, new EventArgs());
        }
        #endregion

    }

    internal delegate void TmNodeEventHandler(object sender, TmNodeEventArgs e);

    [Serializable]
    class TmNodeEventArgs : EventArgs
    {
        public int Index { get; set; }
        public TmNode Node { get; set; }
    }


}
