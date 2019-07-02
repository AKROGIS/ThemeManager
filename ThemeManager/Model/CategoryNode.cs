using NPS.AKRO.ThemeManager.Extensions;
using System;

namespace NPS.AKRO.ThemeManager.Model
{
    [Serializable]
    class CategoryNode : TmNode
    {
        //The TypeName is persisted in image resources and data files, so it is historic,
        //case sensitive, and needs to be impervious to refactoring
        new public const string TypeString = "category";

        //Loading XML
        public CategoryNode()
            : this(null, null, null, null)
        {
        }

        //File->New, Search Results, Drag Text
        public CategoryNode(string name)
            : this(name, null, null, null)
        {
        }

        //MDB Load, overloads
        public CategoryNode(string name, TmNode parent, Metadata metadata, string desc)
            : base(name, parent, metadata, desc)
        {
        }

        /// <summary>
        /// Creates a new Theme List Node based on the contents of the current Category Node
        /// </summary>
        /// <param name="path">File system location for the new theme list.</param>
        /// <returns>A new theme list node</returns>
        /// <exception cref="System.IO.IOException">Path is not valid, or cannot be written to</exception>
        public ThemeListNode ToThemeList(string path)
        {
            var newNode = new ThemeListNode(this.Name, path, this.Metadata.DeepCopy(), this.Description);
            var author = new ThemeListAuthor();
            author.Name = Environment.UserName;
            newNode.Author = author;
            foreach (var child in Children)
                newNode.Add(child.DeepCopy());
            newNode.SaveAs(path);
            return newNode;
        }

        public override string TypeName { get { return TypeString; } }

    }
}
