using System;

namespace NPS.AKRO.ThemeManager.Model
{
    [Serializable]
    class ShortcutNode : TmNode
    {
        //The TypeName is persisted in image resources and data files, so it is historic,
        //case sensitive, and needs to be impervious to refactoring
        new public const string TypeString = "shortcut";

        public ShortcutNode()
            : base("New Shortcut", null, null, null)
        {
        }

        public ShortcutNode(TmNode target)
            : this()
        {
            if (target == null)
                throw new ArgumentNullException("target");

            IsInitialized = false;
            Name = "Shortcut to " + target.Name;
            Target = target;
            IsInitialized = true;
        }

        public ShortcutNode(string name, string path, IThemeListLoader loader)
            : this()
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path == null)
                throw new ArgumentNullException("loader");

            IsInitialized = false;
            Name = name;
            PathToTarget = path;
            ThemeListNode themeList = loader.LoadedThemeList(PathToThemeList);
            if (themeList == null)
                WaitForThemeList(loader);
            else
                ValidatePathToTarget(loader, themeList);
            IsInitialized = true;
        }
        public override string TypeName { get { return TypeString; } }

        [NonSerialized]
        private TmNode _target;
        public TmNode Target { get { return _target; } set { _target = value; } }

        private string PathToTarget { get;  set;}
        private string PathToThemeList { get { return PathToTarget;}}
        private string PathToNode { get { return PathToTarget;}}

        private void WaitForThemeList(IThemeListLoader loader)
        {
            State = ShortcutState.WaitingForThemeList;
            loader.ThemeListLoadedEvent += ValidatePathToTarget;
        }

        private void ValidatePathToTarget(IThemeListLoader loader, ThemeListNode themeList)
        {
            IsInitialized = false;
            Target = loader.LoadedNode(themeList, PathToNode);
            IsInitialized = true;
            if (Target == null)
            {
                State = ShortcutState.Broken;
                loader.ShortcutIsBroken(this);
            }
            else
            {
                State = ShortcutState.Verified;
            }
        }



        //A shortcut can be created either by:
        //  1) a UI interaction in which the user provides the target object
        //  2) retrevial from storage (i.e. loaded from XML)
        //  3) deserialized (i.e. copy/paste or drag/drop) directly, or part of a larger branch
        //
        // Shortcuts can exist in a themelist, or in favorites and search results.
        // When a shortcut is created without a target object, the path cannot be verified
        // until the target themelist is done loading.  The themelist may be 1) loading, i.e.
        // the shortcut is in the themelist, 2) yet to be loaded, i.e. the shortcut was 
        // created in favorites, and points to a themelist that will begin loading
        // shortly, 3) Not in the startup session, but loaded manually by the user later,
        // 4) an obsolete or invalid themelist).  Once the path can be verified, it will
        // either find a target or not.  If the target is not found, the user should be allowed
        // to fix the broken link.

        // When a shortcut is created without a target object, _target and
        // _determinedPathIsInvalid have default values of null and false respectively.
        // When and if the theme is ever finished loading, the path can be checked.
        // After the path is checked, either _target will be non-null (it was found),
        // or _determinedPathIsInvalid will be true (target is not found)
        //
        // Truth Table:
        //   _target  | _determinedPathIsInvalid  |  meaning
        //     null   |     false                 |  themelist has not been loaded
        //     null   |     true                  |  themelist is loaded, path is invalid
        //   non-null |     false                 |  path is valid, target found
        //   non-null |     true                  |  Illogical condition
        //
        //
        //

        private ShortcutState State {get; set;}

        public override bool HasData
        {
            get
            {
                return Target.HasData;
            }
        }



        //create a shortcut from a target and name
        //create a shortcut from an XML element
        //output a shortcut as an XML element
        //return target properties in most cases.
        //each of the TMNode properties need to be reviewed to see if applies to the shortcut or the target
        //do not return target properties when saving or serializing.
        //do not serialize the target, do serialize a path to the target
        //Target may be null - i.e. if shortcut is created from xml before target or if target does not exist
        //searching - shortcuts can be searched as if they were the target, unless the target is already on the search path

    }


    /// <summary>
    /// State of the Shortcut
    /// </summary>
    enum ShortcutState {
        /// <summary>
        /// The shortcut cannot be verfied until the themelist is loaded
        /// </summary>
        WaitingForThemeList,
        /// <summary>
        /// The shortcut has verify link is broken
        /// </summary>
        Broken,
        /// <summary>
        /// The shortcut has verified the link exists, and can be used normally
        /// </summary>
        Verified
    }

    delegate void ThemeListLoadedEventHandler(IThemeListLoader loader, ThemeListNode themeList);

    interface IThemeListLoader
    {
        event ThemeListLoadedEventHandler ThemeListLoadedEvent;
        ThemeListNode LoadedThemeList(string pathToThemeList);
        TmNode LoadedNode(ThemeListNode themeList, string pathToNode);
        void ShortcutIsBroken(ShortcutNode shortcut);
    }
}
