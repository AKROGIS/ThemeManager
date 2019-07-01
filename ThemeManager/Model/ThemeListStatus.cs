namespace NPS.AKRO.ThemeManager.Model
{
    /// <summary>
    /// Status of the ThemeList in relation to its persistant storage (backing store)
    /// </summary>
    enum ThemeListStatus
    {
        /// <summary>ThemeList has been created, but does not have a backing store</summary>
        Created,
        /// <summary>ThemeList has a valid backing store but has not been loaded</summary>
        Initialized,
        /// <summary>ThemeList is building tree from backing store</summary>
        Loading,
        /// <summary>ThemeList has a fully loaded tree which matches the backing store</summary>
        Loaded,
        /// <summary>ThemeList has a fully loaded tree, but the memory copy differs from the backing store</summary>
        Dirty,
        /// <summary>ThemeList is writing the tree in memory to the backing store</summary>
        Saving
    }


}
