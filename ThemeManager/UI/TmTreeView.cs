using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using NPS.AKRO.ThemeManager.Properties;
using NPS.AKRO.ThemeManager.Model;

namespace NPS.AKRO.ThemeManager.UI
{
    class TmTreeView : TreeView
    {
        public TmTreeView()
        {
            UpdateDelegate = new UpdateCallback(UpdateNode);
        }

        /// <summary>
        /// The method to UpdadateNode() may be called from a background thread, so we
        /// need to set up a delegate that it can access for the callback.
        /// </summary>
        internal delegate void UpdateCallback(TmTreeNode node, bool recurse);

        internal UpdateCallback UpdateDelegate { get; private set; }

        internal IEnumerable<TmTreeNode> SelectedNodes
        {
            get { return _selectedNodes; }
        }
        private readonly List<TmTreeNode> _selectedNodes = new List<TmTreeNode>();

        /// <summary>
        /// RootNodes is the list of top level TmNodes that are in this tree.
        /// Nodes is the list of TreeViewNodes that are visible in the UI
        /// since filtering may remove some top level nodes from the UI, we need
        /// a way to get them back, hence the RootNodes.
        /// </summary>
        internal readonly List<TmNode> RootNodes = new List<TmNode>();


        #region Event Overrides

        // Order of Events:
        //  MouseDown
        //  NodeMouseClick
        //  MouseClick
        //  BeforeSelect
        //  AfterSelect

        // Default Behaviors:
        //   Setting SelectedNode to null does NOT fire the BeforeSelect or AfterSelect
        //   Selected Node is not highlighted unless the treeview has focus
        //   NodeMouseClick and MouseClick are only fired if the click occurs within the line of a node (regardless of FullRowSelect)
        //   if SelectedNode == null then left click will give the treeview focus but will not select a node.
        //   if SelectedNode == null then middle or right click will focus the treeview and select/highlight the first node (focus ring is missing).
        //   TreeView focus selects (Action == Unknown) the first node in the tree if SelectedNode == null
        //   Right click within a node briefly highlights it but does not select it.
        //   Clicking in the selected node does not fire a Select event (this is a problem with unselect/multi-select)

        // TMTreeview Behaviour changes:
        //   bool OkToChangeSelection which allows us to selectively accept/reject Select events with Action.Unknown
        //   ClearSelectedNode() will fire an onAfterSelect so we can respond to SelectedNode set to null
        //   SelectNode() will set and restore OkToChangeSelection around a programatic change to SelectedNode
        //     just setting SelectedNode without this method, will cause the BeforeSelect event to reject it.


        /// <summary>
        /// Provides an option to ignore the automatic selection of the first node
        /// if there is no node selected when the treeview control gets focus.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            
            if (Settings.Default.DontSelectFirstNode)
            {
                if (e.Action == TreeViewAction.Unknown && !OkToChangeSelection)
                    e.Cancel = true;
                else
                    base.OnBeforeSelect(e);
            }
            else
            {
                base.OnBeforeSelect(e);
            }
        }

        /// <summary>
        /// Implements multi-selection behavior
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            TmTreeNode node = e.Node as TmTreeNode;
            if (node != null)
            {
                if (ModifierKeys == Keys.Control) // false if other modifiers are pressed
                {
                    ControlSelect(node);
                }
                else if (ModifierKeys == Keys.Shift)
                {
                    ShiftSelect(node);
                }
                else
                {
                    InternalClearSelectedNodes();
                    InternalSelectNode(node);                    
                }
            }
            base.OnAfterSelect(e);
        }

        private void ControlSelect(TmTreeNode node)
        {
            if (_selectedNodes.Contains(node))
                // FIXME - find a way to unselect the SelectedNode
                // (code works fine for removing a node from _selectedNodes)
                // clicking on the SelectedNode does not fire an AfterSelect event
                // setting node to null in the MouseDown event, reselects node before AfterSelect event
                // setting SelectedNode to another node fires this event recursively
                InternalUnselectNode(node);
            else
                InternalSelectNode(node);
        }

        private void ShiftSelect(TmTreeNode node)
        {
            if (_selectedNodes.Contains(node))
            {
                //Do not change selection set, by default new node will be the selected node 
            }
            else
            {
                InternalSelectNode(node);
                TmTreeNode closestSibling = FindClosestSelectedSibling(node);
                if (closestSibling != null)
                {
                    int myIndex = node.Index;
                    int siblingIndex = closestSibling.Index;
                    int min = (myIndex < siblingIndex) ? myIndex : siblingIndex;
                    int max = (myIndex > siblingIndex) ? myIndex : siblingIndex;
                    TreeNodeCollection nc = (node.Parent == null) ? Nodes : node.Parent.Nodes;
                    for (int i = min + 1; i < max; i++)
                        InternalSelectNode(nc[i] as TmTreeNode);
                }
            }
        }

        /// <summary>
        /// Provides an option to clear all selected nodes if the user clicks
        /// in the treeview, but not on a node.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Settings.Default.ClickToClearNodeSelection)
            {
                if (GetNodeAt(e.Location) == null)
                    if (ModifierKeys == Keys.None)
                        ClearSelectedNodes();
                    // else modifier key is pressed so do nothing
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Provides an option to allow a right click on a node to select the node
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && Settings.Default.RightClickSelectsNode)
            {
                SelectNode(e.Node as TmTreeNode);
            }
            base.OnNodeMouseClick(e);
        }

        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            SelectNode(e.Node as TmTreeNode);
            Launch();
            base.OnNodeMouseDoubleClick(e);
        }

        /// <summary>
        /// Cancel the event if the tag data is readonly
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            // FIXME - annoying to start editing on first click
            // can't check TreeView.SelectedNode, since this node is selected right before editing
            // probably need to set up a click counter on the TreeNode, and only start editing on
            // the second click.  any selection method could set the click counter to 1.
            // unselecting the node would set the click counter back to zero.

            TmTreeNode node = e.Node as TmTreeNode;
            if (node == null || node.TmNode == null)
                return;
            //Cancel the event if data is not editable
            if (node.TmNode.IsReadOnly)
                e.CancelEdit = true;
            else
                base.OnBeforeLabelEdit(e);
        }

        /// <summary>
        /// Cancel the event if the tag data is readonly
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            TmTreeNode node = e.Node as TmTreeNode;
            if (node == null || node.TmNode == null)
                return;
            base.OnAfterLabelEdit(e);
            node.TmNode.Name = e.Label;
        }

        #endregion

        #region Cut/Copy/Paste/Delete

        public bool CanCut()
        {
            return CanCopy() && CanDelete();
        }

        public bool CanCopy()
        {
            if (_selectedNodes.Count == 0)
                return false;
            if (_selectedNodes.All(n => n.TmNode.IsSubTheme))
                return false;
            return true;
        }

        public bool CanPaste()
        {
            //FIXME - allow some cases when selectednode == null
            if (SelectedTmNode == null ||
                SelectedTmNode.IsReadOnly ||
                SelectedTmNode.IsTheme ||
                SelectedTmNode.IsSubTheme)
                return false;
            return (Clipboard.ContainsData("TmNodeList") ||
                   Clipboard.ContainsData("TmNode") ||
                   Clipboard.ContainsFileDropList());
        }

        public bool CanDelete()
        {
            if (_selectedNodes.Count == 0)
                return false;
            if (_selectedNodes.Any(n => (n.TmNode.IsSubTheme || (n.TmNode.IsReadOnly && !n.TmNode.IsThemeList))))
                return false;
            return true;
        }

        public void Cut()
        {
            if (CanCut())
            {
                Copy();
                Delete();
            }
        }

        public void Copy()
        {
            if (CanCopy()) //assures for the following that _selectedNodes.Count > 0
            {
                Clipboard.Clear();
                DataObject data = new DataObject();
                CopyAsTmNodeList(data);
                CopyAsTmNode(data);
                CopyAsFiles(data);
                CopyAsText(data);
                Clipboard.SetDataObject(data);
            }
        }

        private void CopyAsTmNodeList(DataObject clipboardData)
        {
            IList<TmNode> nodeList = _selectedNodes.Select(x => x.TmNode).ToList<TmNode>();
            if (nodeList.Count == 1)
                return;
            Debug.Assert(IsSerializable(nodeList), "Cannot serialize the list of selected nodes");
            clipboardData.SetData("TmNodeList", false, nodeList);
        }

        private void CopyAsTmNode(DataObject clipboardData)
        {
            Trace.TraceInformation("Serializing Node " + _selectedNodes[0].TmNode.Name);
            Debug.Assert(IsSerializable(_selectedNodes[0].TmNode), "Cannot serialize the selected node");
            clipboardData.SetData("TmNode", false, _selectedNodes[0].TmNode);
        }

        private void CopyAsFiles(DataObject clipboardData)
        {
            StringCollection files = new StringCollection();
            foreach (TmTreeNode node in _selectedNodes.Where(node => node.TmNode.IsLaunchable))
            {
                files.Add(node.TmNode.Data.Path);
            }
            if (files.Count > 0)
                clipboardData.SetFileDropList(files);
        }

        private void CopyAsText(DataObject clipboardData)
        {
            StringBuilder text = new StringBuilder();
            text.AppendFormat("{0} Theme Manager Object: {1}\n",_selectedNodes.Count, (_selectedNodes.Count == 1) ? "" : "s");
            foreach (TmTreeNode node in _selectedNodes)
                text.AppendLine(node.TmNode.ToString());
            clipboardData.SetText(text.ToString());
        }

        //Would like to use [Conditional("DEBUG")] to avoid creating IL for production version
        //unfortunately methods with [Conditional("DEBUG")] attribute must return void
        //[Conditional("DEBUG")]
        private static bool IsSerializable(object obj)
        {
            var mem = new MemoryStream();
            var bin = new BinaryFormatter();
            try
            {
                bin.Serialize(mem, obj);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("Your object cannot be serialized. The reason is: " + ex);
                return false;
            }
        }


        public void Paste()
        {
            if (Clipboard.ContainsData("TmNodeList"))
            {
                PasteAsTmNodeList();
                return;
            }
            if (Clipboard.ContainsData("TmNode"))
            {
                PasteAsTmNode();
                return;
            }
            if (Clipboard.ContainsFileDropList())
                PasteAsFiles();
        }

        //FIXME -  Dont paste a node onto itself
        private void PasteAsTmNodeList()
        {
            TmNode currentNode = GetCurrentTmNode();
            PasteAsTmNodeListAtTmNode(currentNode);
        }

        private void PasteAsTmNodeListAtTmNode(TmNode currentNode)
        {
            IEnumerable<TmNode> nodes = Clipboard.GetData("TmNodeList") as IEnumerable<TmNode>;
            if (nodes != null && nodes.Count() > 0)
            {

                if (currentNode == null)
                    foreach (TmNode node in nodes)
                        Add(node);
                else
                    foreach (TmNode node in nodes)
                        currentNode.Add(node);
                // select all the newly pasted nodes.
                //ClearSelectedNodes();
                // FIXME - I think each select will unselect the previously selected,
                // we may need a SelectNodes(Enumerable<TmTreeNode>) method
                //foreach (TmNode node in nodes)
                //    SelectNode(node);
            }
        }

        //FIXME -  Dont paste a node onto itself
        private void PasteAsTmNode()
        {
            object o = Clipboard.GetData("TmNode");
            TmNode n = o as TmNode;
            if (n == null)
                return;

            TmNode currentNode = GetCurrentTmNode();
            if (currentNode == null)
                Add(n); //Select??
            else
                currentNode.Add(n); // Select???
        }

        private TmNode GetCurrentTmNode()
        {
            if (SelectedNode == null)
                return null;
            TmTreeNode node = SelectedNode as TmTreeNode;
            if (node == null)
                return null;
            return node.TmNode;
        }

        private void PasteAsFiles()
        {
            TmNode currentNode = GetCurrentTmNode();
            StringCollection files = Clipboard.GetFileDropList();
            foreach (string file in files)
            {
                string basename = Path.GetFileNameWithoutExtension(file);
                //assume this file is a valid theme
                //FIXME - determine valid theme or themelist; reject otherwise.
                TmNode newNode = new TmNode(TmNodeType.Theme, basename, null, new ThemeData(file), null, null, null);
                try
                {
                    // May need to load to query arc objects which could throw any number of exceptions.
                    newNode.ReloadTheme(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load the theme properties" + ex);
                }

                if (currentNode == null)
                    Add(newNode);
                else
                    currentNode.Add(newNode);
            }
        }

        public void Delete()
        {
            if (CanDelete())
            {
                int index = _selectedNodes.Count - 1;
                TmTreeNode nextNode = _selectedNodes[index--].NextVisibleNode as TmTreeNode;
                while (_selectedNodes.Contains(nextNode) && index >= 0)
                    nextNode = _selectedNodes[index--].NextVisibleNode as TmTreeNode;

                foreach (TmTreeNode node in _selectedNodes)
                    if (node.TmNode.Parent == null)
                    {
                        RootNodes.Remove(node.TmNode);
                        Nodes.Remove(node);
                    }
                    else
                        node.TmNode.Delete();
                _selectedNodes.Clear();
                if (nextNode != null)
                    SelectNode(nextNode);
            }
        }

        #endregion

        #region Drag and Drop

        //Initiate a drag/drop operation if I start dragging a treeview item
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);
            DataObject dataObject = new DataObject();
            TmTreeNode node = e.Item as TmTreeNode;

            Debug.Assert(node != null, "Drag initiated without a node");
            if (node == null)
                return;
            //make sure node being dragged is selected
            SelectNode(node);

            //I may have unselected (ctrl-select) the only node when starting the drag,
            //so check to make sure _selectedNodes has nodes
            if (_selectedNodes.Count == 0)
                return;

            //SourceNode is also used to determine the drag/drop effects  see SetDndEffect();
            sourceNodes = _selectedNodes.Select(x => x.TmNode).Where(x => (x.IsTheme || x.IsCategory)).ToList<TmNode>();
            if (sourceNodes.Count == 0)
                return;

            if (sourceNodes.Count == 1)
            {
                Debug.Assert(IsSerializable(sourceNodes[0]), "Cannot serialize the selected node");
                dataObject.SetData("TmNode", sourceNodes[0]);
            }
            else
            {
                Debug.Assert(IsSerializable(sourceNodes), "Cannot serialize the list of selected nodes");
                dataObject.SetData("TmNodeList", sourceNodes);
            }

            // Also drag as a file list (for ArcMap) 
            StringCollection paths = new StringCollection();
            foreach (var item in sourceNodes)
                if (item.IsTheme && item.HasData && File.Exists(item.Data.Path))
                {
                    paths.Add(item.Data.Path);
                }
            if (paths.Count > 0)
                dataObject.SetFileDropList(paths);

            //FIXME - Add support for ESRI Object types
            //FIXME - do I want to add the metadata to the drag/drop??

            DoDragDrop(dataObject, DragDropEffects.Copy | DragDropEffects.Move);
        }

        //respond to an item dropped in my treeview
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            TmTreeNode destinationTreeNode = NodeAtEvent(e);
            TmNode destinationNode = null;

            if (destinationTreeNode != null)
                destinationNode = destinationTreeNode.TmNode;

            // TmNodeList drop
            if (e.Data.GetDataPresent("TmNodeList", false))
            {
                List<TmNode> nodes = e.Data.GetData("TmNodeList") as List<TmNode>;
                if (nodes != null && nodes.Count() > 0)
                {
                    foreach (TmNode node in nodes)
                    {
                        if (e.Effect == DragDropEffects.Move)
                            MoveNode(destinationNode, node);
                        if (e.Effect == DragDropEffects.Copy)
                            CopyNode(destinationNode, node);
                    }

                    if (e.Effect != DragDropEffects.None)
                    {
                        //SelectNode(destinationTreeNode);
                        destinationTreeNode.Expand();
                        return;
                    }
                }
            }

            // TmNode drop
            if (e.Data.GetDataPresent("TmNode", false))
            {
                // e.Effect == none for illeagal drops. 
                TmNode oldNode = e.Data.GetData("TmNode") as TmNode;
                Debug.Assert(oldNode != null, "Could not reserialize node from clipboard");
                if (oldNode != null)
                {
                    if (e.Effect == DragDropEffects.Move)
                        MoveNode(destinationNode, oldNode);
                    if (e.Effect == DragDropEffects.Copy)
                        CopyNode(destinationNode, oldNode);
                    if (e.Effect != DragDropEffects.None)
                    {
                        //SelectNode(destinationTreeNode);
                        destinationTreeNode.Expand();
                        return;
                    }
                }
            }

            // FileDrop
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                TmTreeNode newTreeNode = null;
                foreach (string file in files)
                {
                    //FIXME - determine if this is a Theme or a ThemeList
                    //currently assuming it is a theme.
                    TmNode newNode = new TmNode(TmNodeType.Theme, Path.GetFileNameWithoutExtension(file), null, new ThemeData(file), null, null, null);
                    try
                    {
                        // May need to load to query arc objects which could throw any number of exceptions.
                        newNode.ReloadTheme(false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to load the theme properties" + ex);
                    }

                    if (destinationNode == null)
                        newTreeNode = Add(newNode);
                    else
                        destinationNode.Add(newNode);
                }
                if (newTreeNode != null)
                    SelectNode(newTreeNode);
                else
                {
                    SelectNode(destinationTreeNode);
                    destinationTreeNode.Expand();
                }
                return;
            }

            // Text Drop
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string txt = (string)e.Data.GetData(DataFormats.Text);

                TmNode newNode = new TmNode(TmNodeType.Category, txt);
                if (destinationNode == null)
                    SelectNode(Add(newNode));
                else
                {
                    destinationNode.Add(newNode);
                    SelectNode(destinationTreeNode);
                    destinationTreeNode.Expand();
                }
            }
        }

        private void CopyNode(TmNode destinationNode, TmNode oldNode)
        {
            //TmNode newNode = oldNode.Clone() as TmNode;
            TmNode newNode = oldNode.Copy();
            Debug.Assert(newNode != null, "Could not clone node from clipboard");
            if (newNode != null)
            {
                if (destinationNode == null)
                    Add(newNode);
                else
                    destinationNode.Add(newNode);
            }
        }

        private void MoveNode(TmNode destinationNode, TmNode oldNode)
        {
            if (oldNode != destinationNode)
            {
                if (oldNode.Parent == null)
                {
                    RootNodes.Remove(oldNode);
                    var node = Nodes.Cast<TmTreeNode>().First(n => n.TmNode == oldNode);
                    Nodes.Remove(node);
                }
                else
                    oldNode.Delete();
                if (destinationNode == null)
                    Add(oldNode);
                else
                    destinationNode.Add(oldNode);
            }
        }

        //Called when something is dragged into (enters) the treeview control
        //or when dragging begins inside the treeview control
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            GetSourceNodes(e);
            e.Effect = SetDndEffect(e);
        }

        //Called repeatedly while something is being dragged within our treeview
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Effect = SetDndEffect(e);  
            CheckForHover(e, 1.0);       //Expand a node if we are hovering for x seconds
        }

        //dnd helper functions
        /// <summary>
        /// Change effect based on state of ctrl key and target node type
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private DragDropEffects SetDndEffect(DragEventArgs e)
        {
            // FIXME - allow drop on root for non-themelist trees ??
            TmTreeNode destinationNode = NodeAtEvent(e);

            if (destinationNode == null)
                return DragDropEffects.None;
            
            TmNode node = destinationNode.TmNode;
            if (node.IsReadOnly || node.IsTheme || node.IsSubTheme)
                return DragDropEffects.None;

            if (sourceNodes != null)
            {
                if (sourceNodes.Any(n => (n.IsThemeList || n.IsSubTheme)))
                    return DragDropEffects.None;
                if ((e.KeyState & 8) == 8 && (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) // Ctrl 
                    return DragDropEffects.Copy;
                if (sourceNodes.Any(n => (n.IsReadOnly || n == node)))
                    return DragDropEffects.None;
                else
                    return DragDropEffects.Move;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                return DragDropEffects.Copy;

            return DragDropEffects.None;
        }

        private void CheckForHover(DragEventArgs e, double seconds)
        {
            if (e.Effect != DragDropEffects.None)
            {
                TmTreeNode destinationNode = NodeAtEvent(e);
                if (destinationNode == null)
                    return;

                //if we are on a new object, reset our timer
                //otherwise check to see if enough time has passed and expand the destination node
                if (destinationNode != lastDragDestination)
                {
                    lastDragDestination = destinationNode;
                    lastDragDestinationTime = DateTime.Now;
                }
                else
                {
                    TimeSpan hoverTime = DateTime.Now.Subtract(lastDragDestinationTime);
                    if (hoverTime.TotalSeconds > seconds)
                        destinationNode.Expand();
                }
            }
        }

        private TmTreeNode NodeAtEvent(DragEventArgs e)
        {
            return GetNodeAt(PointToClient(new Point(e.X, e.Y))) as TmTreeNode;
        }

        private void GetSourceNodes(DragEventArgs e)
        {
            //FIXME - clearing sourceNodes to null is necessary if the nodes
            //being dragged are not of this treeview, but it is a waste if this
            //treeview started the drag (sourceNodes is set in OnItemDrag()).
            //Can I ever get drop nodes dragged from another tree?
            sourceNodes = null;
            if (e.Data.GetDataPresent("TmNode"))
            {
                sourceNodes = new List<TmNode>();
                sourceNodes.Add(e.Data.GetData("TmNode") as TmNode);
            }

            if (e.Data.GetDataPresent("TmNodeList"))
            {
                sourceNodes = e.Data.GetData("TmNodeList") as List<TmNode>;
            }

        }


        // Fields to allow expansion of a node if hovering during drag and drop
        TmTreeNode lastDragDestination;
        DateTime lastDragDestinationTime;
        List<TmNode> sourceNodes;

        #endregion

        #region treeview selection changes

        //Multi-select behaviour for TmTreeView:
        // Selected Node:
        //     Still valid, this is the most recently selected node.
        // Keyboard selection
        //     have single select behaviour
        // Programatic selection:
        //     _selectedNodes list is private, so no access is provided
        //     setting SelectedNode will have single select behaviour.
        // Click on a node:
        //     Clears all selected nodes, adds node to selected list
        // Click not on a node
        //     Clears all selected nodes (or no-op) - user pref.
        // Ctrl-click on a node
        //     Add or remove node from selected list
        // Ctrl-click not on a node
        //     no-op (adds or removes nothing from the selected list)
        // Shift-click on a node
        //     if sibling node is selected, select all to closest sibling
        //     if no sibling is selected Add or remove from list
        // Shift-click not on a node
        //     no-op (adds or removes nothing from the selected list)
        // Ctrl-Shift(or any other unspecified modifiers)-Click on a node 
        //     same as click on a node
        // Ctrl-Shift(or any other unspecified modifiers)-Click not on a node
        //     same as click not on a node
        // Right click on a node (with any modifier keys)
        //     Has behaviour of single click (with modifiers) or no-op (user pref),
        //     then opens the context menu.
        // Right click not on a node (with any modifier keys)
        //     open context menu for treeview.
        // Double click on a node (with any modifier keys)
        //     Has behaviour of single click (with modifiers),
        //     then launches the selected set.
        // Double click not on a node (with any modifier keys)
        //     no-op - do not change selection, do not launch any

        // Some Rules:
        //   Can't select sub-themes (no-op)
        //   Can't have ancestor and descendents in the list
        //      Selecting a node removes all descendents and ancestors from the list

        /// <summary>
        /// Used to selectively (dis)allow programatic node selection
        /// </summary>
        /// <remarks>
        /// Motivation is to deny system actions that change node selection.
        /// </remarks>
        private bool OkToChangeSelection { get; set; }

        private void ClearSelectedNode()
        {
            SelectedNode = null; // does not trigger the Selection events
            OnAfterSelect(new TreeViewEventArgs(null));
        }

        internal void ClearSelectedNodes()
        {
            InternalClearSelectedNodes();
            ClearSelectedNode();
        }

        //Does not cause any SelectionEvents
        private void InternalClearSelectedNodes()
        {
            foreach (TmTreeNode node in _selectedNodes)
            {
                node.TmNode.IsSelected = false;
            }
            _selectedNodes.Clear();
        }

        //called by me or a client, so I will honor it.
        //I need to distinguish from calls by framework
        // particularly the one that selects the first node if selection is null

        internal void SelectNode(TmTreeNode node)
        {
            OkToChangeSelection = true;
            SelectedNode = node; //will trigger OnAfterSelect method to do rest of work.
            OkToChangeSelection = false;
        }
        
        //Does not cause any SelectionEvents
        private void InternalSelectNode(TmTreeNode node)
        {
            //If I am not going to include subThemes in selection set,
            //then I need to ensure that they are not highlighted
            //if I can't highlight them, they should not be select,
            //then how can I see thier metadata
            //better to add them to the set, and then filter later.
            if (node != null && !_selectedNodes.Contains(node)) // && !node.TmNode.IsSubTheme)
            {
                if (_selectedNodes.Count > 0)
                    RemoveSelectedAncestors(node);
                if (_selectedNodes.Count > 0)
                    RemoveSelectedDescendent(node);
                _selectedNodes.Add(node);
                node.TmNode.IsSelected = true;
            }
        }

        internal void UnselectNode(TmTreeNode node)
        {
            if (_selectedNodes.Contains(node))
            {
                InternalUnselectNode(node);
                // the following test always fails now, see InternalUnselectNode(node)
                // The body of the loop seems like a good idea (select the previously selected)
                // however since it triggers an AfterSelect Event, it cascadingly unselects all.

                //if (SelectedNode == node)
                //{
                //    if (_selectedNodes.Count == 0)
                //        ClearSelectedNode();
                //    else
                //    {
                //        OkToChangeSelection = true;
                //        SelectedNode = _selectedNodes.Last();
                //        OkToChangeSelection = false;
                //    }
                //}
            }
        }

        /// <summary>
        /// Changes state of node and selected node list.
        /// Does not fire events or have any side effects.
        /// </summary>
        /// <param name="node"></param>
        private void InternalUnselectNode(TmTreeNode node)
        {
            _selectedNodes.Remove(node);
            node.TmNode.IsSelected = false;
            if (node == SelectedNode)
                SelectedNode = null; //will not fire any events.
        }

        private TmTreeNode FindClosestSelectedSibling(TmTreeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            int myIndex = node.Index;
            TreeNodeCollection nodes = node.Parent == null ? Nodes : node.Parent.Nodes;
            TmTreeNode closestSibling = null;
            int smallestDelta = nodes.Count;
            for (int index = 0; index < nodes.Count; index++ )
            {
                TmTreeNode sibling = nodes[index] as TmTreeNode;
                if (sibling != null && sibling.TmNode.IsSelected)
                {
                    int delta = (index < myIndex) ? myIndex - index : index - myIndex;
                    if (index != myIndex && delta < smallestDelta )
                        closestSibling = sibling;
                }
            }
            return closestSibling;
        }

        /// <summary>
        /// Remove any ancestors in the list of selected nodes.
        /// </summary>
        /// <remarks>
        /// The maximum is one ancestor in the list, because
        /// each insert removes its ancestors.
        /// The list of ancestors is presumed to be shorter than the list of selected nodes
        /// </remarks>
        /// <param name="node"></param>
        private void RemoveSelectedAncestors(TmTreeNode node)
        {
            TmTreeNode parent = node.Parent as TmTreeNode;
            if (parent != null)
            {
                if (parent.TmNode.IsSelected)
                    InternalUnselectNode(parent);
                else
                    RemoveSelectedAncestors(parent);
            }
        }

        /// <summary>
        /// Removes any descendents from the list of selected nodes.
        /// </summary>
        /// <remarks>
        /// There can be any number of descendents in the lists, but
        /// any descendents in the list will not have deeper
        /// descendents in the list.
        /// The list of selected nodes is presumed to be shorter than the set of descendents.
        /// Therefore recusively searching for selected descendents would be inefficient.
        /// </remarks>
        /// <param name="node"></param>
        private void RemoveSelectedDescendent(TmTreeNode node)
        {
            // I make a copy, since I cannot modify a collection while iterating.
            foreach (TmTreeNode selectedNode in _selectedNodes.ToList())
                //add the path separator, so we do not remove siblings that have the same prefix
                if (selectedNode != node && selectedNode.FullPath.StartsWith(node.FullPath + node.TreeView.PathSeparator))
                    InternalUnselectNode(selectedNode);
        }

        public TmNode SelectedTmNode
        {
            get
            {
                TmTreeNode node = SelectedNode as TmTreeNode;
                return (node == null) ? null : node.TmNode;
            }
        }

        #endregion

        #region Add/Update TMNode to TreeNode

        // When you add a node to the tree, it goes in to the root collection.
        // if you want to add a node at a lower point in the tree, then add it to a TmNode\
        public TmTreeNode Add(TmNode newNode)
        {
            Debug.Assert(newNode != null, "Null argument exception for node");

            if (newNode == null)
                return null;

            // Do not allow the same list loaded twice.
            if (newNode.IsThemeList && 
                  //(string.IsNullOrEmpty(newNode.Data.Path) ||
                   FindThemeListNode(newNode.Data.Path) != null)
                return null;

            RootNodes.Add(newNode);
            return Add(newNode, Nodes);
        }

        private TmTreeNode Add(TmNode node, TreeNodeCollection nodes)
        {
            if (node.IsHidden && !Settings.Default.ShowHiddenThemes)
                return null;
            
            TmTreeNode newNode = new TmTreeNode(node);
            nodes.Add(newNode);
            return newNode;
        }

        public TmTreeNode Insert(int index, TmNode inNode, TreeNode atNode)
        {
            if (inNode == null)
                throw new ArgumentNullException("inNode");

            TreeNode newParentNode = atNode ?? SelectedNode;
            TreeNodeCollection nodes = newParentNode == null ? Nodes : newParentNode.Nodes;
            if (nodes == Nodes)
                RootNodes.Add(inNode);
            if (index < 0 || index > nodes.Count)
                index = nodes.Count;
            //FIXME - I think this is broken.
            // I am using index to place this TM node in a treeview, with no
            // check on the structure of the tree that the TM node lives in.
            TmTreeNode newNode = new TmTreeNode(inNode);
            nodes.Insert(index, newNode);
            return newNode;
        }

        public TmTreeNode FindThemeListNode(string path)
        {
            // we can have as many un-"pathed" (newly created) themelists as we want
            if (string.IsNullOrEmpty(path))
                return null;

            foreach (TmTreeNode tNode in Nodes)
                if (tNode.TmNode.Data.Path == path)
                    return tNode;
            return null;
        }

        public void RemoveThemeListNode(TmTreeNode nodeToRemove)
        {
            Debug.Assert(nodeToRemove != null, "The themelist node is not null");
            if (nodeToRemove == null)
                return;
            if (RootNodes.Contains(nodeToRemove.TmNode))
                RootNodes.Remove(nodeToRemove.TmNode);
            if (Nodes.Contains(nodeToRemove))
                Nodes.Remove(nodeToRemove);
        }

        public void UpdateNode(TmTreeNode node, bool recurse)
        {
            Debug.Assert(node != null, "Null argument exception for node");
            if (node == null)
                return;
            BeginUpdate();
            node.UpdateProperties();
            if (recurse)
            {
                //Throw away and restart option
                node.Nodes.Clear();
                foreach (TmNode child in node.TmNode.Children)
                {
                    node.Nodes.Add(new TmTreeNode(child));
                }
                // sync node.Nodes to node.TmNode.Children adding/removing nodes then update all
                //node.SyncNodes(node.TmNode.Children);
                //foreach (TreeNode child in node.Nodes)
                //{
                //    UpdateNode(child as TmTreeNode, true);
                //}
            }
            //Sort();
            EndUpdate();
        }

        #endregion

        #region Sorting

        public NodeSortOrder TextSortOrder
        {
            get
            {
            var sorter = TreeViewNodeSorter as SortTreeNodesByText;
            if (sorter == null)
                TextSortInit();
            sorter = TreeViewNodeSorter as SortTreeNodesByText;
            return sorter.NodeSortOrder;
            }
        }

        /// <summary>
        /// This may get called repeatedly at startup, so we want no side effects
        /// </summary>
        public void TextSortInit()
        {
            TextSortInit(Settings.Default.DefaultSortOrder);
        }

        public void TextSortInit(int sortOrder)
        {
            NodeSortOrder initialTextSortOrder =
                    Enum.IsDefined(typeof(NodeSortOrder), sortOrder) ?
                    (NodeSortOrder)sortOrder :
                    NodeSortOrder.Ascending;
            TextSortInit(initialTextSortOrder);
        }

        public void TextSortInit(string sortOrder)
        {
            if (sortOrder == null)
                TextSortInit();
            else
                try
                {
                    NodeSortOrder initialTextSortOrder = (NodeSortOrder)Enum.Parse(typeof(NodeSortOrder), sortOrder);
                    TextSortInit(initialTextSortOrder);
                }
                catch (Exception ex)
                {
                    Debug.Print("Got an unrecognized sort order ("+sortOrder+")\n"+ex.ToString());
                    TextSortInit();
                }
        }

        public void TextSortInit(NodeSortOrder initialTextSortOrder)
        {
            if (initialTextSortOrder == NodeSortOrder.Unsorted &&
                !Settings.Default.AllowUnsortedOrder)
                initialTextSortOrder = NodeSortOrder.Ascending;

            switch (initialTextSortOrder)
            {
                case NodeSortOrder.Descending:
                    TextSortZA(); break;
                case NodeSortOrder.Unsorted:
                    TextSortNoSort(); break;
                default:
                case NodeSortOrder.Ascending:
                    TextSortAZ(); break;
            }
        }

        /// <summary>
        /// On first call this method sorts the node display text in ascending order
        /// with a CurrentCultureIgnoreCase string comparison. 
        /// Repeated calls toggle between Ascending and Descending
        /// </summary>
        public void TextSortToggle()
        {
            var sorter = TreeViewNodeSorter as SortTreeNodesByText;
            if (sorter == null)
                TextSortInit();
            else
            {
                sorter.ToggleSortOrder();
                if (sorter.NodeSortOrder == NodeSortOrder.Unsorted)
                    UnSort();
                else
                    Sort();
            }
        }

        public void TextSortIncrement()
        {
            var sorter = TreeViewNodeSorter as SortTreeNodesByText;
            if (sorter == null)
                TextSortInit();
            else
            {
                sorter.IncrementSortOrder();
                if (sorter.NodeSortOrder == NodeSortOrder.Unsorted)
                    UnSort();
                else
                    Sort();
            }
        }

        public void TextSortAZ()
        {
            var sorter = TreeViewNodeSorter as SortTreeNodesByText;
            if (sorter == null)
            {
                sorter = new SortTreeNodesByText
                {
                    NodeSortOrder = NodeSortOrder.Ascending,
                    TextComparer = StringComparison.CurrentCultureIgnoreCase
                };
                TreeViewNodeSorter = sorter;  // triggers a sort
            }
            else
            {
                if (sorter.NodeSortOrder != NodeSortOrder.Ascending)
                {
                    sorter.NodeSortOrder = NodeSortOrder.Ascending;
                    Sort();
                }
            }
        }

        public void TextSortZA()
        {
            var sorter = TreeViewNodeSorter as SortTreeNodesByText;
            if (sorter == null)
            {
                sorter = new SortTreeNodesByText
                {
                    NodeSortOrder = NodeSortOrder.Descending,
                    TextComparer = StringComparison.CurrentCultureIgnoreCase
                };
                TreeViewNodeSorter = sorter;  // triggers a sort
            }
            else
            {
                if (sorter.NodeSortOrder != NodeSortOrder.Descending)
                {
                    sorter.NodeSortOrder = NodeSortOrder.Descending;
                    Sort();
                }
            }
        }

        public void TextSortNoSort()
        {
            var sorter = TreeViewNodeSorter as SortTreeNodesByText;
            if (sorter == null)
            {
                sorter = new SortTreeNodesByText
                {
                    NodeSortOrder = NodeSortOrder.Unsorted,
                    TextComparer = StringComparison.CurrentCultureIgnoreCase
                };
                TreeViewNodeSorter = sorter;  // triggers an (unsorted) sort 
                //UnSort();
            }
            else
            {
                if (sorter.NodeSortOrder != NodeSortOrder.Unsorted)
                {
                    sorter.NodeSortOrder = NodeSortOrder.Unsorted;
                    UnSort();
                }
            }
        }

        public void UnSort()
        {
            // optionally, have the mainform clone the unfiltered tree and then replace it.
            //List<TmNode> rootCopy = RootNodes.ToList<TmNode>();
            BeginUpdate();
            //Nodes.Clear();
            //RootNodes.Clear();
            foreach (TmTreeNode node in Nodes)
            {
                //rootCopy.Remove(node.TmNode);
                UpdateNode(node, true);
            }
            //foreach (TmNode node in rootCopy)
               // Add(node);
            EndUpdate();
        }


        #endregion

        #region Searching/Filtering

        internal IEnumerable<TmNode> SearchNodes()
        {
            //if (SelectedNode == null)
            //    return Nodes.Cast<TreeNode>().Select(n => n.Tag).Cast<TmNode>().ToList();
            //else
            //    return new List<TmNode>(new[] { SelectedNode.Tag as TmNode });
            if (_selectedNodes == null || _selectedNodes.Count < 1)
                return Nodes.OfType<TmTreeNode>().Select(n => n.TmNode);
            else
                return _selectedNodes.Select(n => n.TmNode);
        }

        /// <summary>
        /// Prunes all nodes which are not visible from the tree.
        /// Visibility is determined by the data in the node tag.
        /// From the TreeView perspective this is a non-reversable operation.
        /// The original treeview should be cloned if it may needed. 
        /// </summary>
        public void Filter()
        {
            TmTreeNode[] copyNodes = new TmTreeNode[Nodes.Count];
            Nodes.CopyTo(copyNodes, 0);
            foreach (TmTreeNode node in copyNodes)
                node.Filter();
        }

        public void UnFilter()
        {
            // optionally, have the mainform clone the unfiltered tree and then replace it.
            List<TmNode> rootCopy = RootNodes.ToList<TmNode>();
            foreach (TmTreeNode node in Nodes)
            {
                rootCopy.Remove(node.TmNode);
                UpdateNode(node, true);
            }
            foreach (TmNode node in rootCopy)
                Add(node);
        }

        public void ShowHidden()
        {
            // Cloning will not work, since the user may edit the tree in the hidden state
            List<TmNode> rootCopy = RootNodes.ToList<TmNode>();
            foreach (TmTreeNode node in Nodes)
            {
                rootCopy.Remove(node.TmNode);
                UpdateNode(node, true);
            }
            foreach (TmNode node in rootCopy)
                Add(node);
        }

        public void HideHidden()
        {
            TmTreeNode[] copyNodes = new TmTreeNode[Nodes.Count];
            Nodes.CopyTo(copyNodes, 0);
            foreach (TmTreeNode node in copyNodes)
                node.Hide();
        }

        #endregion

        #region Commands

        public bool CanLaunch()
        {
            return _selectedNodes.Select(n => n.TmNode).Where(d => d.IsLaunchable).Any();
        }

        public void Launch()
        {
            var launchableNodes = _selectedNodes.Select(n => n.TmNode).Where(d => d.IsLaunchable);
            foreach (var launchableNode in launchableNodes)
            {
                launchableNode.Launch();
            }
        }

        #endregion
    }
}
