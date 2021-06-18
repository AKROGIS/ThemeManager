using NPS.AKRO.ThemeManager.Model;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NPS.AKRO.ThemeManager.UI
{
    [Serializable]
    class TmTreeNode :TreeNode
    {
        public TmTreeNode(TmNode tmNode)
        {
            TmNode = tmNode ?? throw new ArgumentNullException("tmNode");
            UpdateProperties();

            tmNode.PropertyChanged += Data_PropertyChanged;
            tmNode.ReloadNode += Data_ReloadNode;
            tmNode.ChildRemoved += Data_ChildRemoved;
            tmNode.ChildAdded += Data_ChildAdded;

            foreach (TmNode child in tmNode.Children)
                Nodes.Add(new TmTreeNode(child));
        }

        internal void UpdateProperties()
        {
            Text = TmNode.Name;
            //ImageIndex = TmNode.ImageIndex;
            ImageKey = TmNode.ImageKey;
            SelectedImageKey = TmNode.ImageKey;
            //SelectedImageIndex = TmNode.ImageIndex;
            if (!string.IsNullOrEmpty(TmNode.Description))
                ToolTipText = TmNode.Description;
            BackColor = TmNode.IsSelected ? Color.DeepSkyBlue : Color.Empty;
            //if (TmNode.IsHidden && !Settings.Default.ShowHiddenThemes)
            //    Remove();
            ForeColor = (TmNode.IsHidden || TmNode.IsSubTheme) ? Color.Gray : Color.Black;
        }

        public TmNode TmNode { get; private set; }

        //private void Data_SelectionChanged(object sender, TmNodeEventArgs e)
        //{
        //    BackColor = TmNode.IsSelected ? Color.DeepSkyBlue : Color.Empty;
        //}

        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
                Text = TmNode.Name;
            if (e.PropertyName == "ImageKey")
            {
                ImageKey = TmNode.ImageKey;
                SelectedImageKey = TmNode.ImageKey;
            }
            if (e.PropertyName == "Description")
                if (string.IsNullOrEmpty(TmNode.Description))
                    ToolTipText = null;
                else
                    ToolTipText = TmNode.Description;
            if (e.PropertyName == "IsSelected")
                BackColor = TmNode.IsSelected ? Color.DeepSkyBlue : Color.Empty;
            if (e.PropertyName == "IsHidden" || e.PropertyName == "IsSubTheme")
                ForeColor = (TmNode.IsHidden || TmNode.IsSubTheme) ? Color.Gray : Color.Black;
        }

        //private void Data_ImageChanged(object sender, TmNodeEventArgs e)
        //{
        //    ImageIndex = ((TmNode)Tag).ImageIndex;
        //    SelectedImageIndex = ((TmNode)Tag).ImageIndex;
        //}

        private void Data_ChildRemoved(object sender, TmNodeEventArgs e)
        {
            //If I am filtering/sorting, then the indices of the
            //TmTreeNode.Nodes and TMNode.Children collections will not align.
            //Nodes.RemoveAt(e.Index);
            foreach (TmTreeNode node in Nodes)
                if (node.TmNode == e.Node)
                {
                    Nodes.Remove(node);
                    break;
                }
        }

        private void Data_ChildAdded(object sender, TmNodeEventArgs e)
        {
            Nodes.Insert(e.Index, new TmTreeNode(e.Node));
            // this node 
        }

        private void Data_ReloadNode(object sender, EventArgs e)
        {
            ((TmTreeView)TreeView).UpdateNode(this, true);
        }


        /// <summary>
        /// Removes a node (and it's sub-tree) if it is not visible or any children which are not visible.
        /// </summary>
        /// <remarks>
        /// Visibility is determined by the state of the TmNode data.
        /// </remarks>
        internal void Filter()
        {
            if (!TmNode.IsVisible)
                Remove();
            else
            {
                TmTreeNode[] copyNodes = new TmTreeNode[Nodes.Count];
                Nodes.CopyTo(copyNodes, 0);
                foreach (TmTreeNode node in copyNodes)
                {
                    node.Filter();
                }
            }
        }

        /// <summary>
        /// Removes a node (and it's sub-tree) if it is hidden or any children which are hidden.
        /// </summary>
        /// <remarks>
        /// Hidden(ness) is determined by the state of the TmNode data.
        /// </remarks>
        internal void Hide()
        {
            if (TmNode.IsHidden)
                Remove();
            else
            {
                TmTreeNode[] copyNodes = new TmTreeNode[Nodes.Count];
                Nodes.CopyTo(copyNodes,0);
                foreach (TmTreeNode node in copyNodes)
                {
                    node.Hide();
                }
            }
        }

        /// <summary>
        /// Syncs the node's children with the list of TmNodes
        /// </summary>
        /// <param name="tmNodes"></param>
        internal void SyncNodes(System.Collections.Generic.IEnumerable<TmNode> tmNodes)
        {
            //foreach TmNode in tmNodes, ensure there is a corresponding node in this.Nodes
            //remove any nodes in this.Nodes that do not have a corresponding TmNode in tmNodes
            throw new NotImplementedException();
        }
    }
}
