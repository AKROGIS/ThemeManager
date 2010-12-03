using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NPS.AKRO.ThemeManager.Model
{
    public enum SearchType
    {
        ThemeName,
        ThemeSummary,
        ThemeDescription,
        ThemeTags,
        Theme, //  == ThemeName + ThemeSummary + ThemeDescription + ThemeTags,
        PubDate,
        Metadata
    }

    public class SearchOptions
    {

        public SearchOptions(SearchType searchType, string searchRequest)
        {
            // FIXME: Do error checking
            // search request can be null but only for some search types
            // if searchtype is null then assume themename, but search request must not be null
            SearchType = searchType;
            SearchRequest = searchRequest;
            Init();
        }

        private void Init()
        {
            MaxDaysSinceEdit = -1;
            MinDaysSinceEdit = -1;
            ComparisonMethod = StringComparison.CurrentCultureIgnoreCase;
        }

        public IEnumerable<string> SearchWords
        {
            get 
            {
                if (_searchWords == null && SearchRequest != null)
                    _searchWords = Regex.Split(SearchRequest, @"\W+");
                return _searchWords;
            }
        }
        private string[] _searchWords;

        public string SearchRequest { get; private set;}

        public string XmlElement { get; set; }

        public bool FindAll { get; set; }

        public bool FindWholeWords { get; set; }

        public StringComparison ComparisonMethod { get; set; }

        public SearchType SearchType { get; private set; }

        public int MinDaysSinceEdit { get; set; }

        public int MaxDaysSinceEdit { get; set; }

        public bool HasMaxDays
        {
            get { return MaxDaysSinceEdit >= 0; }
        }

        public bool HasMinDays
        {
            get { return MinDaysSinceEdit >= 0; }
        }

        public string Label
        {
            get
            {
                if (Description.Length > 20)
                    return Description.Substring(0, 20) + "...";
                return Description;
            }
        }

        //FIXME - make more detailed
        public string Description
        {
            get
            {
                if (SearchType == SearchType.Theme)
                    return (SearchRequest == null) ? "Invalid Name Search" : SearchRequest + " in All";
                if (SearchType == SearchType.ThemeName)
                    return (SearchRequest == null) ? "Invalid Name Search" : SearchRequest + " in Name";
                if (SearchType == SearchType.ThemeDescription)
                    return (SearchRequest == null) ? "Invalid Name Search" : SearchRequest + " in Description";
                if (SearchType == SearchType.ThemeSummary)
                    return (SearchRequest == null) ? "Invalid Name Search" : SearchRequest + " in Summary";
                if (SearchType == SearchType.ThemeTags)
                    return (SearchRequest == null) ? "Invalid Name Search" : SearchRequest + " in Tags";

                if (SearchType == SearchType.Metadata)
                    return (SearchRequest == null) ? "Invalid Metadata Search" : SearchRequest + " in Metadata";

                if (SearchType == SearchType.PubDate)
                {
                    string after = (HasMaxDays) ? "After " + (DateTime.Now - TimeSpan.FromDays(MaxDaysSinceEdit)).ToShortDateString() : "";
                    string before = (HasMinDays) ? "Before " + (DateTime.Now - TimeSpan.FromDays(MinDaysSinceEdit)).ToShortDateString() : "";
                    if (HasMinDays && HasMaxDays)
                        if (MinDaysSinceEdit < MaxDaysSinceEdit)
                            return after + " And " + before;
                        else
                            return before + " Or " + after;
                    if (HasMinDays)
                        return before;
                    if (HasMaxDays)
                        return after;
                    return "Invalid Date Search";
                }
                return "Invalid Search Type";
            }
        }

        public override string ToString()
        {
            return Description;
        }
    }


    public class SearchParameters: IEnumerable<SearchOptions>
    {
        private readonly List<SearchOptions> _searchOptions = new List<SearchOptions>();
        private bool _searchThemes = true;

        public bool MatchAll { get; set; }

        public string Location { get; set; }

        public bool SearchThemes
        {
            get { return _searchThemes; }
            set { _searchThemes = value; }
        }

        public bool SearchCategories { get; set; }

        public int Count
        {
            get { return _searchOptions.Count; }
        }

        public SearchOptions this[int index]
        {
            get { return _searchOptions[index]; }
            set { _searchOptions[index] = value; }
        }

        public SearchOptions NameSearch
        {
            get
            { return _searchOptions.FirstOrDefault(so => so.SearchType == SearchType.ThemeName); }
        }

        public SearchOptions DateSearch
        {
            get
            { return _searchOptions.FirstOrDefault(so => so.SearchType == SearchType.PubDate); }
        }

        //This is displayed in the search results tree
        public string Label
        {
            get
            {
                if (Description.Length > 20)
                    return Description.Substring(0, 20) + "...";
                return Description;
            }
        }

        // FIXME - This should be more meaningful.  It is presented to the user.
        public string Description
        {
            get
            {
                return (_searchOptions.Count < 1) ? "Nothing" : _searchOptions[0].Description;
            }
        }

        #region IEnumerable<SearchOptions> Members

        public IEnumerator<SearchOptions> GetEnumerator()
        {
            return _searchOptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void Add(SearchOptions searchOption)
        {
            _searchOptions.Add(searchOption);
        }

        public void AddRange(IEnumerable<SearchOptions> searchOption)
        {
            _searchOptions.AddRange(searchOption);
        }

        public void Remove(SearchOptions searchOption)
        {
            _searchOptions.Remove(searchOption);
        }

        public void Clear()
        {
            _searchOptions.Clear();
        }

        public bool IsLast(SearchOptions search)
        {
            return (_searchOptions.IndexOf(search) == _searchOptions.Count - 1);
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
