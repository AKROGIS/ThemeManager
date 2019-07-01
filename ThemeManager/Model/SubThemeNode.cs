using System;

namespace NPS.AKRO.ThemeManager.Model
{
    [Serializable]
    class SubThemeNode : ThemeNode
    {
        //The TypeName is persisted in image resources and data files, so it is historic,
        //case sensitive, and needs to be impervious to refactoring
        //SubThemes have been historically persisted as "theme".  Be careful if changing this.
        new public const string TypeString = "theme";

        //Load XML
        public SubThemeNode()
            : this(null, null, null, null, null, null)
        {
        }

        //Load from MDB, ThemeBuilder, overloads
        public SubThemeNode(string name, TmNode parent, ThemeData data, Metadata metadata, string desc, DateTime? pubDate)
            : base(name, parent, data, metadata, desc, pubDate)
        {
        }

        public override string TypeName { get { return TypeString; } }

        public override bool HasDataToPreview { get { return false; } }

        public override bool IsLaunchable { get { return false; } }

        public override void Launch()
        {
            //Do nothing.  I cannot launch a subtheme.  Changes base class behavior
        }

        public override void Reload()
        {
            //Do nothing.  SubThemes are created/loaded by the theme
        }
    }
}
