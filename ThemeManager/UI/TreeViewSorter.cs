using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NPS.AKRO.ThemeManager.UI
{
    public enum NodeSortOrder
    {
        Descending = -1,
        Unsorted = 0,
        Ascending = 1
    }

    public class SortTreeNodesByText : IComparer, IComparer<TreeNode>
    {
        public NodeSortOrder NodeSortOrder { get; set; }
        public StringComparison TextComparer { get; set; }

        public void IncrementSortOrder()
        {
            if (NodeSortOrder == NodeSortOrder.Ascending)
                NodeSortOrder = NodeSortOrder.Descending;
            else
                NodeSortOrder = (NodeSortOrder)((int)NodeSortOrder + 1);

        }

        public void ToggleSortOrder()
        {
            if (NodeSortOrder != NodeSortOrder.Descending)
                NodeSortOrder = NodeSortOrder.Descending;
            else
                NodeSortOrder = NodeSortOrder.Ascending;
        }

        #region Interface Members

        int IComparer.Compare(object x, object y)
        {
            var tx = x as TreeNode;
            var ty = y as TreeNode;
            if (tx == null && ty == null)
                return 0;
            if (tx == null)
                return 1;
            if (ty == null)
                return -1;
            return Compare(tx, ty);
        }

        public int Compare(TreeNode x, TreeNode y)
        {
            //if (SortOrder == SortOrder.Unsorted)
            // Applying no sort order after sorting leave list sorted.
            // I need a mechanism to restore the themelist to it's native order.
            //else
            // sorts on Node Text (label) with alphabetic (cultural aware) sort
            return (int)NodeSortOrder * string.Compare(x.Text, y.Text, TextComparer);
        }

        #endregion
    }
}
