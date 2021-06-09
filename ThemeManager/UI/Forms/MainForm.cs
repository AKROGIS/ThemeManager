using Microsoft.Win32;
using NPS.AKRO.ThemeManager.ArcGIS;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Model;
using NPS.AKRO.ThemeManager.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace NPS.AKRO.ThemeManager.UI.Forms
{

    public partial class MainForm : Form
    {
        private readonly Font _activeSearchBoxFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        private readonly Font _inactiveSearchBoxFont = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
        private readonly List<TreeView> _treeViews;
        //= new List<TreeView>(3) { themesTreeView, favoritesTreeView, searchTreeView };

        public MainForm()
        {
            InitializeComponent();
            _treeViews = new List<TreeView>(3) { themesTreeView, favoritesTreeView, searchTreeView };
        }

        #region Single Instance Application functions.

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                ShowMe();
            }
            base.WndProc(ref m);
        }

        private void ShowMe()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            // get our current "TopMost" value (ours will always be false though)
            bool top = TopMost;
            // make our form jump to the top of everything
            TopMost = true;
            // set it back to whatever it was
            TopMost = top;
        }

	    #endregion

        #region Public Form Properties

        internal string SearchText
        {
            get
            {
                if (searchTextBox.Text == "Search")
                    return "";
                else
                    return searchTextBox.Text;
            }
        }

        #endregion

        #region Form Events
        private void MainForm_Load(object sender, EventArgs e)
        {
            RestoreForm();
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            if (!Settings.Default.DontShowBetaWarning)
            {
                WarningForm warning = new WarningForm().CommonInit();
                warning.ShowDialog(this);
            }
            await RestoreStateAsync();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (EnableSaveCommand())
            {
                DialogResult ret = MessageBox.Show(
                    "There are unsaved changes." + Environment.NewLine +
                    "Do you want to save your modified theme lists?",
                    "Save Changes?", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (ret == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (ret == DialogResult.Yes)
                    saveToolStripButton_Click(this, null);
            }
            SaveState();
        }

        #endregion

        #region Treeview Events

        private async void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //e.Node maybe null since I have modified behavior of the TMTreeview
            //Debug.Print("TreeView AfterSelect Event - Sender: " + ((TreeView)sender).Name + "\n\tAction: " + e.Action + "\n\tNode: " + e.Node);
            // If we are not loading themelists at startup, then we need to call this
            //LoadIfNodeIsUnloadedThemeList(newNode);
            EnableButtonStrip();
            EnableMenuStrip();
            await UpdateInfoDisplayAsync();
        }


        #endregion

        #region Misc. Events

        private readonly int _minVisibleSplitterWidth = Settings.Default.MinimumVisibleSplitterWidth;
        private bool _infoPagesAreVisible = true;

        //Give the active text box a chance to respond to typical keyboard
        //mnemonics before the menu hijacks them for the treeview.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            TextBox t = ActiveControl as TextBox;
            if (t != null)
            {
                if (keyData == (Keys.Control | Keys.C))
                {
                    t.Copy();
                    return true;
                }

                if (keyData == (Keys.Control | Keys.V))
                {
                    t.Paste();
                    return true;
                }

                if (keyData == (Keys.Control | Keys.X))
                {
                    t.Cut();
                    return true;
                }

                if (keyData == Keys.Delete)
                {
                    t.Clear();
                    return true;
                }

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool InfoPanelBecameExposedSinceLastCheck
        {
            get
            {
                bool visiblePreviously = _infoPagesAreVisible;
                _infoPagesAreVisible = splitContainer.Panel2.Width > _minVisibleSplitterWidth;
                return (_infoPagesAreVisible && !visiblePreviously);
            }
        }

        private void searchToolStripTextBox_Enter(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "Search")
            {
                searchTextBox.Font = _activeSearchBoxFont;
                searchTextBox.ForeColor = SystemColors.WindowText;
                searchTextBox.Text = "";
            }
        }

        private void searchToolStripTextBox_Leave(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "")
            {
                searchTextBox.Font = _inactiveSearchBoxFont;
                searchTextBox.ForeColor = SystemColors.InactiveCaptionText;
                searchTextBox.Text = "Search";
            }
        }

        private void searchToolStripTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(SearchText))
                DoSimpleSearch(SearchText);
        }

        private void ageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //FIXME - this is being called before it is set up.
            if (!(((ComboBox)ageComboBox.Control).SelectedValue is Int32))
                return;
            Settings.Default.AgeInDays = (int)((ComboBox)ageComboBox.Control).SelectedValue;
            foreach (TreeView tv in new TreeView[] { themesTreeView, favoritesTreeView, searchTreeView })
            {
                tv.BeginUpdate();
                foreach (TmTreeNode node in tv.Nodes.OfType<TmTreeNode>())
                    node.TmNode.UpdateImageIndex(true);
                tv.EndUpdate();
            }
        }

        private async void styleSheetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            await UpdateInfoDisplayAsync();
        }

        private async void listsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentTV = CurrentTreeViewFromIndex(listsTabControl.SelectedIndex);
            if (Settings.Default.FocusTreeviewOnTabChange)
            {
                _currentTV.Focus();
            }
            await UpdateInfoDisplayAsync();
            EnableButtonStrip();
            EnableMenuStrip();
        }

        private async void infoTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            await UpdateInfoDisplayAsync();
        }

        private async void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (InfoPanelBecameExposedSinceLastCheck)
                await UpdateInfoDisplayAsync();
        }

        //State of _areInfoPagesVisible should only be modified by splitter moved events (and initialization)

        #endregion

        #region Menu Click Actions

        private void newThemeListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTreeView = themesTreeView;
            TmTreeNode node = CurrentTreeView.Add(new TmNode(TmNodeType.ThemeList, "New Theme List"));
            node.EnsureVisible();
            CurrentTreeView.SelectNode(node);
        }

        private void newCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TmTreeNode node = CurrentTreeView.SelectedNode as TmTreeNode;
            if (node == null)
                CurrentTreeView.Add(new TmNode(TmNodeType.Category, "New Category"));
            else
            {
                node.TmNode.Add(new TmNode(TmNodeType.Category, "New Category"));
                node.EnsureVisible();
                node.Expand();
            }
        }

        private void newThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TmTreeNode node = CurrentTreeView.SelectedNode as TmTreeNode;
            if (node == null)
                return;
            node.TmNode.Add(new TmNode(TmNodeType.Theme, "New Theme"));
            node.EnsureVisible();
            node.Expand();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                //TmTreeNode prevNode = themesTreeView.SelectedNode as TmTreeNode;
                //themesTreeView.ClearSelectedNodes();
                TmTreeNode node = themesTreeView.Add(new TmNode(TmNodeType.ThemeList,
                                  Path.GetFileNameWithoutExtension(openFileDialog1.FileName),
                                  null, new ThemeData(openFileDialog1.FileName), null, null, null));
                if (node == null)
                {
                    MessageBox.Show("Theme list could not be added.  Probably because it is already loaded.");
                    return;
                }
                if (LoadIfNodeIsUnloadedThemeList(node))
                {
                    CurrentTreeView = themesTreeView;
                    themesTreeView.SelectNode(node);
                    themesTreeView.SelectedNode.Expand();
                    node.TmNode.PropertyChanged += nodePropertyChanged;
                }
                else
                {
                    MessageBox.Show("File could not be loaded as a theme list.");
                    themesTreeView.Nodes.Remove(node);
                    //themesTreeView.SelectNode(prevNode);
                }
            }
        }

        private void nodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                saveToolStripButton.Enabled = EnableSaveCommand();
            }
        }

        // saves all modified themelists
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            foreach (TmTreeNode node in themesTreeView.Nodes)
                if (node.TmNode.IsThemeList && !node.TmNode.IsReadOnly && node.TmNode.IsDirty)
                    if (string.IsNullOrEmpty(node.TmNode.Data.Path))
                        SaveAs(node.TmNode);
                    else
                        Save(node.TmNode);
            saveToolStripButton.Enabled = EnableSaveCommand();
        }

        // saves currently selectd node as a new themelist
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TmTreeNode tmTreeNode = CurrentTreeView.SelectedNode as TmTreeNode;
            if (tmTreeNode != null)
            {
                // This may involve a large change to the treeview, so bracket the update
                CurrentTreeView.BeginUpdate();
                SaveAs(tmTreeNode.TmNode);
                CurrentTreeView.EndUpdate();
            }

         }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTreeView.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTreeView.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTreeView.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTreeView.Delete();
        }

        private void fastSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTreeView = themesTreeView;
            CurrentTreeView.ClearSelectedNodes();
            ShowSearchForm();
        }

        private void advancedSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchForm();
        }

        private void sortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Default.AllowUnsortedOrder)
                CurrentTreeView.TextSortIncrement();
            else
                CurrentTreeView.TextSortToggle();
            UpdateSortButton();
        }

        private void UpdateSortButton()
        {
            if (Settings.Default.AllowUnsortedOrder)
                IncrementSortButton();
            else
                ToggleSortButton();
        }

        private void IncrementSortButton()
        {
            switch (CurrentTreeView.TextSortOrder)
            {
                case NodeSortOrder.Descending:
                    sortToolStripButton.Image = Resources.SortNone;
                    sortToolStripMenuItem1.Image = Resources.SortNone;
                    break;
                case NodeSortOrder.Unsorted:
                    sortToolStripButton.Image = Resources.SortAZ;
                    sortToolStripMenuItem1.Image = Resources.SortAZ;
                    break;
                default:
                case NodeSortOrder.Ascending:
                    sortToolStripButton.Image = Resources.SortZA;
                    sortToolStripMenuItem1.Image = Resources.SortZA;
                    break;
            }
        }

        private void ToggleSortButton()
        {
            switch (CurrentTreeView.TextSortOrder)
            {
                case NodeSortOrder.Descending:
                    sortToolStripButton.Image = Resources.SortAZ;
                    sortToolStripMenuItem1.Image = Resources.SortAZ;
                    break;
                default:
                case NodeSortOrder.Ascending:
                    sortToolStripButton.Image = Resources.SortZA;
                    sortToolStripMenuItem1.Image = Resources.SortZA;
                    break;
            }
        }

        private void add2FavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Do NOT switch to Favorites Tree or select the newly added node(s)
            //This is similar to other browser/explorer behavior
            favoritesTreeView.ClearSelectedNodes();
            foreach (TmTreeNode node in CurrentTreeView.SelectedNodes)
                favoritesTreeView.Add(node.TmNode.Copy());
        }

        private void launchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentTreeView.Launch();
        }

        private void preferencesToolStripButton_Click(object sender, EventArgs e)
        {
            PreferencesForm prefs = new PreferencesForm().CommonInit();
            prefs.Show(this);
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            webBrowser.ShowPrintDialog();
        }

        private void administrativeToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminReports tools = new AdminReports().CommonInit();
            tools.Show(this);
        }

        private void quickStartTutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            infoTabControl.SelectedIndex = 0;
            //FIXME - validate setting, and allow real url
            Uri help = new Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Settings.Default.DefaultHtml));
            webBrowser.Url = help;
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            infoTabControl.SelectedIndex = 0;
            //FIXME - validate setting, and allow real url
            Uri help = new Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Settings.Default.HelpUrl));
            webBrowser.Url = help;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }


        #endregion

        #region Menu Enable/Disable

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            newCategoryToolStripMenuItem.Visible = EnableNewCategoryCommand();
            newThemeToolStripMenuItem.Visible = EnableNewThemeCommand();
            saveAsToolStripMenuItem.Visible = EnableSaveAsCommand();

            cutToolStripMenuItem.Enabled = EnableCutCommand();
            copyToolStripMenuItem.Enabled = EnableCopyCommand();
            pasteToolStripMenuItem.Enabled = EnablePasteCommand();
            deleteToolStripMenuItem.Enabled = EnableDeleteCommand();

            searchToolStripMenuItem.Visible = EnableSearchCommand();
            launchToolStripMenuItem.Visible = EnableLaunchCommand();
            addToFavoritesToolStripMenuItem.Visible = EnableAdd2FavCommand();
            preferencesToolStripMenuItem.Visible = EnablePreferencesOnContextMenu();
        }

        private void EnableMenuStrip()
        {
            newThemeListToolStripMenuItem1.Enabled = EnableNewThemeListCommand();
            newCategoryToolStripMenuItem1.Enabled = EnableNewCategoryCommand();
            newThemeToolStripMenuItem1.Enabled = EnableNewThemeCommand();
            openToolStripMenuItem1.Enabled = EnableOpenCommand();
            saveToolStripMenuItem1.Enabled = EnableSaveAsCommand();
            saveAsToolStripMenuItem1.Enabled = EnableSaveAsCommand();
            printToolStripMenuItem1.Enabled = EnablePrintCommand();

            cutToolStripMenuItem1.Enabled = EnableCutCommand();
            copyToolStripMenuItem1.Enabled = EnableCopyCommand();
            pasteToolStripMenuItem1.Enabled = EnablePasteCommand();
            deleteToolStripMenuItem1.Enabled = EnableDeleteCommand();
            searchToolStripMenuItem1.Enabled = EnableSearchCommand();

            sortToolStripMenuItem1.Enabled = EnableSortCommand();
            launchToolStripMenuItem1.Enabled = EnableLaunchCommand();
            addToFavoritesToolStripMenuItem1.Enabled = EnableAdd2FavCommand();
            preferencesToolStripMenuItem1.Enabled = EnablePreferencesCommand();
            administrativeToolsToolStripMenuItem.Visible = Settings.Default.ShowAdministrativeTools;
        }

        private void EnableButtonStrip()
        {
            newToolStripButton.Enabled = newThemeListToolStripButton.Enabled || newCategoryToolStripButton.Enabled || newThemeToolStripButton.Enabled;
            newThemeListToolStripButton.Enabled = EnableNewThemeListCommand();
            newCategoryToolStripButton.Enabled = EnableNewCategoryCommand();
            newThemeToolStripButton.Enabled = EnableNewThemeCommand();
            openToolStripButton.Enabled = EnableOpenCommand();
            saveToolStripButton.Enabled = EnableSaveCommand();
            printToolStripButton.Enabled = EnablePrintCommand();
            cutToolStripButton.Enabled = EnableCutCommand();
            copyToolStripButton.Enabled = EnableCopyCommand();
            pasteToolStripButton.Enabled = EnablePasteCommand();
            deleteToolStripButton.Enabled = EnableDeleteCommand();
            addToFavoritesToolStripButton.Enabled = EnableAdd2FavCommand();
            sortToolStripButton.Enabled = EnableSortCommand();
            preferencesToolStripButton.Enabled = EnablePreferencesCommand();
            viewHelpToolStripButton.Enabled = EnableHelpCommand();
            styleSheetComboBox.Enabled = EnablePrintCommand();
            //searchToolStripButton.Enabled = EnableSearchCommand();
            //searchToolStripTextBox.Enabled = EnableSearchCommand();
            //launchToolStripButton.Enabled = EnableLaunchCommand();
        }

        private bool EnableNewThemeListCommand()
        {
            return true;
        }

        private bool EnableNewCategoryCommand()
        {
            TmTreeView tv = CurrentTreeView;
            if (tv == themesTreeView && tv.SelectedNode == null)
                return false;
            if (tv.SelectedNode == null)
                return true;
            TmTreeNode node = tv.SelectedNode as TmTreeNode;
            if (node == null)
                return false;
            return (node.TmNode.IsThemeList || node.TmNode.IsCategory) && !node.TmNode.IsReadOnly;
        }

        private bool EnableNewThemeCommand()
        {
            TmTreeView tv = CurrentTreeView;
            if (tv != themesTreeView || tv.SelectedNode == null)
                return false;
            TmTreeNode node = tv.SelectedNode as TmTreeNode;
            if (node == null)
                return false;
            return (node.TmNode.IsThemeList || node.TmNode.IsCategory) && !node.TmNode.IsReadOnly;
        }

        private bool EnableOpenCommand()
        {
            return true;
        }

        // Saves all modified themelists
        private bool EnableSaveCommand()
        {
            //return true if there are any modified themelists.
            foreach (TmTreeNode node in themesTreeView.Nodes)
                if (!node.TmNode.IsReadOnly && node.TmNode.IsDirty)
                    return true;
            return false;
        }

        // saves currently selected node as a new themelist
        private bool EnableSaveAsCommand()
        {
            TmTreeView tv = CurrentTreeView;
            //if (tv != themesTreeView || tv.SelectedNode == null)
            if (tv == null || tv.SelectedNode == null)
                    return false;
            TmTreeNode node = tv.SelectedNode as TmTreeNode;
            if (node == null)
                return false;
            return node.TmNode.IsThemeList || node.TmNode.IsCategory;
        }

        private bool EnablePrintCommand()
        {
            TmTreeView tv = CurrentTreeView;
            if (tv == null || tv.SelectedNode == null)
                return false;
            TmTreeNode node = tv.SelectedNode as TmTreeNode;
            if (node == null)
                return false;
            return node.TmNode.HasMetadata;
        }

        private bool EnableCutCommand()
        {
            return CurrentTreeView.CanCut();
        }

        private bool EnableCopyCommand()
        {
            return CurrentTreeView.CanCopy();
        }

        private bool EnablePasteCommand()
        {
            return CurrentTreeView.CanPaste();
        }

        private bool EnableDeleteCommand()
        {
            return CurrentTreeView.CanDelete();
        }

        private bool EnableSortCommand()
        {
            // Side Effect - first set the correct button type
            if (Settings.Default.AllowUnsortedOrder)
                IncrementSortButton();
            else
                ToggleSortButton();

            return (CurrentTreeView.Nodes.Count != 0) ? true : false;
        }

        private bool EnableSearchCommand()
        {
            TmTreeView tv = CurrentTreeView;
            return (tv != null && tv.Nodes.Count != 0) ? true : false;
        }

        private bool EnableLaunchCommand()
        {
            return CurrentTreeView.CanLaunch();
        }

        private bool EnableAdd2FavCommand()
        {
            TmTreeView tv = CurrentTreeView;
            if (tv == null || tv == favoritesTreeView)
                return false;

            if (!tv.SelectedNodes.Any() || tv.SelectedNodes.Any(n => n.TmNode.IsSubTheme || n.TmNode.IsThemeList))
                return false;
            return true;
        }

        private bool EnablePreferencesCommand()
        {
            return true;
        }

        private bool EnablePreferencesOnContextMenu()
        {
            return !Settings.Default.ShowMainMenu && !Settings.Default.ShowToolbar1;
        }

        private bool EnableHelpCommand()
        {
            return true;
        }

        #endregion

        #region Current TreeView, TreeNode, TMNode

        private TmTreeView _currentTV;

        //accessed by the search form.
        internal TmTreeView CurrentTreeView
        {
            get
            {
                if (_currentTV == null)
                    _currentTV = CurrentTreeViewFromIndex(listsTabControl.SelectedIndex);
                return _currentTV;
            }
            private set
            {
                if (value == themesTreeView)
                    listsTabControl.SelectedIndex = 0;
                if (value == favoritesTreeView)
                    listsTabControl.SelectedIndex = 1;
                if (value == searchTreeView)
                    listsTabControl.SelectedIndex = 2;
            }
        }

        private TmTreeView CurrentTreeViewFromIndex(int index)
        {
            switch (index)
            {
                default:
                case 0:
                    return themesTreeView;
                case 1:
                    return favoritesTreeView;
                case 2:
                    return searchTreeView;
            }
        }

        private TmNode CurrentTMNode
        {
            get
            {
                return CurrentTreeNode == null ? null : CurrentTreeNode.TmNode;
            }
        }

        private TmTreeNode CurrentTreeNode
        {
            get
            {
                return (CurrentTreeView == null)? null : CurrentTreeView.SelectedNode as TmTreeNode;
            }
        }

        internal IEnumerable<TmNode> ThemeLists
        {
            get
            {
                return themesTreeView.RootNodes;
            }
        }

        #endregion

        #region Helper methods

        private TmNode _previousMetadataNode;
        private TmNode _previousPreviewNode;
        private TmNode _previousPropertiesNode;
        private int _previousStyleSheetIndex = -1;

        internal bool InPlaceNodeEditing
        {
            get
            {
                return _currentTV.LabelEdit;
            }
            set
            {
                foreach (TreeView tv in _treeViews)
                    tv.LabelEdit = value;
            }
        }

        internal string SearchLocation
        {
            get
            {
                string searchLocation = CurrentTreeView.Tag.ToString();
                if (CurrentTreeView.SelectedNode == null)
                    searchLocation = "All " + searchLocation;
                else
                    searchLocation += ":" + CurrentTreeView.SelectedNode.FullPath;
                return searchLocation;
            }
        }

        private static bool LoadIfNodeIsUnloadedThemeList(TmTreeNode newNode)
        {
            if (newNode == null)
                return false;
            TmNode tmNode = newNode.TmNode;
            if (tmNode == null || !tmNode.IsValidThemeList)
                return false;
            if (!tmNode.NeedsThemeListLoaded)
                return true;
            tmNode.SuspendUpdates();
            if (TryToLoadThemeList(tmNode))
            {
                // This method may be called on a background thread, if so we need to us Invoke
                // to access the UI on the primary thread.
                Trace.TraceInformation("{0}: Start updating tree for {1}", DateTime.Now, tmNode.Name); Stopwatch time = Stopwatch.StartNew();
                TmTreeView tv = newNode.TreeView as TmTreeView;
                if (tv != null)
                    if (tv.InvokeRequired)
                        tv.Invoke(tv.UpdateDelegate, new object[] { newNode, true });
                    else
                        tv.UpdateNode(newNode, true);
                time.Stop(); Trace.TraceInformation("{0}: End updating tree for {1} - Elapsed Time: {2}", DateTime.Now, tmNode.Name, time.Elapsed);
                tmNode.ResumeUpdates();
                return true;
            }
            tmNode.ResumeUpdates();
            return false;
        }

        private static bool TryToLoadThemeList(TmNode node)
        {
            //FIXME - need to load as much as possible on exception.
            if (node == null)
                return false;
            Trace.TraceInformation("{0}: Start building node {1}", DateTime.Now, node.Name); Stopwatch time = Stopwatch.StartNew();
            try
            {
                //FIXME: do I need to do any locking on this node?
                node.Build();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error Loading Theme\nMessage: {0}\nMethod: {1}", ex.Message, ex.TargetSite);
                MessageBox.Show(msg, "oh, no!");
                return false;
            }
            time.Stop(); Trace.TraceInformation("{0}: Done building node {1} - Elapsed Time: {2}", DateTime.Now, node.Name, time.Elapsed);
            return true;
        }


        private async Task UpdateInfoDisplayAsync()
        {
            TmNode node = CurrentTMNode;
            int newStyleSheetIndex = styleSheetComboBox.SelectedIndex;

            if (_infoPagesAreVisible)
            {
                switch (infoTabControl.SelectedIndex)
                {
                    case 0:
                        if (node == null || node != _previousMetadataNode || newStyleSheetIndex != _previousStyleSheetIndex)
                        {
                            //ShowMetadataSpinner();
                            await DisplayMetadataAsync(node);
                            //HideMetadataSpinner();
                            _previousMetadataNode = node;
                            _previousStyleSheetIndex = newStyleSheetIndex;
                        }
                        break;
                    case 1:
                        if (node == null || node != _previousPreviewNode)
                        {
                            //ShowPreviewSpinner();
                            DisplayPreview(node);
                            //HidePreviewSpinner();
                            _previousPreviewNode = node;
                        }
                        break;
                    case 2:
                        if (node == null || node != _previousPropertiesNode)
                        {
                            DisplayProperties(node);
                            _previousPropertiesNode = node;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /*
         * Showing/Hiding a spinner in the tab title box (ala a web browser) is trickier than I hoped.
         * I can set an imageList for the tabControl and add an animated gif to an imageList
         * and then dynamically set the imageIndex in the tab page to -1 for no image, or 0 (first
         * image in the image list) to display or hide the image.  However  the image does not animate
         * because the tapPage control doesn't know how to do this.
         * I can add a pictureBox to the form, and that will correctly display the animated gif,
         * but I cannot put the pictureBox where I would like.
         * I could swap the pictureBox in/out of the content area of the tab control.
         * It also works to add the gif to a label in the statusBar control.  However the status bar
         * is hidden by default, so not as obvious as putting it in the tab title.
         * I did find a solution that uses owner drawing to do exactly what I want, but it is more
         * complicated than I would like at this point. https://stackoverflow.com/a/30307224/542911
         */

        private void ShowSearchForm()
        {
            SearchForm frm = new SearchForm().CommonInit();
            frm.themeNameTextBox.Text = SearchText;
            frm.Show(this);
        }

        private void DoSimpleSearch(string searchText)
        {
            SearchParameters searchParams = new SearchParameters();
            searchParams.Add(new SearchOptions(SearchType.Theme, searchText) { FindAll = true });
            searchParams.Location = "All Themes";

            Stopwatch time = Stopwatch.StartNew();
            IEnumerable<TmNode> nodes =
                themesTreeView.RootNodes.SelectMany(n => n.Recurse(node => node.Children)
                                  .Where(x => x.Matches(searchParams))
                                  );
            DisplaySearchResults(searchParams, nodes);
            time.Stop(); Trace.TraceInformation("Time for simple search: {0}ms", time.Elapsed.Milliseconds);
        }

        //Also called from the search form.
        internal void DisplaySearchResults(SearchParameters searchParams, IEnumerable<TmNode> results)
        {
            //I need to cache the enumeration for two reasons:
            // 1: I access the enumeration to get the count, and to access the nodes.
            //    A new search is done each time the enumeration is iterated (even for counting).
            // 2: The enumeration searches the treeviews _selected nodes.
            //    I clear the searchtrees selected node when I create the results,
            //    therefore no results are returned if the search was done on the search tree.
            IList<TmNode> nodes = results.ToList();
            if (nodes == null || nodes.Count == 0)
                MessageBox.Show(string.Format("Sorry, your search for '{0}' in {1} has no results.",
                                              searchParams.Description, searchParams.Location));
            else
            {
                string resultsNodeLabel = string.Format("{0} result{1} for '{2}' in {3}",
                    nodes.Count, nodes.Count == 1 ? "" : "s", searchParams.Label, searchParams.Location);
                searchTreeView.BeginUpdate();
                searchTreeView.ClearSelectedNodes();
                TmNode catNode = new TmNode(TmNodeType.Category, resultsNodeLabel);
                TmTreeNode newNode = searchTreeView.Add(catNode);
                if (newNode == null)
                {
                    MessageBox.Show("Unable to add results to display");
                    return;
                }
                foreach (TmNode node in nodes)
                    catNode.Add(node.Copy());
                searchTreeView.SelectNode(newNode);
                newNode.Expand();
                searchTreeView.EndUpdate();
                CurrentTreeView = searchTreeView;
            }
        }

        private void SaveAs(TmNode node)
        {
            saveFileDialog1.FileName = node.Name;
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    if (node.IsThemeList)
                        node.SaveAs(saveFileDialog1.FileName);
                    else if (node.IsCategory)
                    {
                        themesTreeView.Add(node.CopyAsThemeList(saveFileDialog1.FileName));
                    }
                    else
                        MessageBox.Show("Save As... is only valid on a Theme List or a Category");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Unable to save Theme List '{0}'\n in file '{1}'\n{2}", node.Name, saveFileDialog1.FileName, ex.Message),
                                    "Oh no!");
                }
            }
        }

        private void Save(TmNode node)
        {
            try
            {
                node.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to save themelist '{0}'\n in file '{1}'\n{2}", node.Name, node.Data.Path, ex.Message),
                                "Oh no!");
            }
        }

        #endregion

        #region Metadata Display Page

        private string _cachedSafeNoMetadataTemplate;

        //relative hrefs in xml or html are not honored, since the web browser has no location info.

        private Uri _cachedStartupUri;
        private string _invalidUriText;

        private async Task DisplayMetadataAsync(TmNode node)
        {
            Trace.TraceInformation("Display metadata for node: " + (node == null ? "null" : node.ToString()));

            // This is valid, but usually only at startup when the user has not selected an active theme.
            if (node == null)
            {
                LoadDefaultBrowser(); //Loads a default start page
                return;
            }

            // This should never happen, and would indicate a programming error.  We will spare the user the details.
            Debug.Assert(node.Metadata != null, "Node has no metadata object to display");
            if (node.Metadata == null)
            {
                LoadDefaultBrowser(); //Loads a default start page
                return;
            }

            if (_cachedSafeNoMetadataTemplate == null)
                _cachedSafeNoMetadataTemplate = GetSafeNoMetadataTemplate();

            var nodeName = $"{node.Name} ({node.Type})";
            if (string.IsNullOrEmpty(node.Metadata.Path))

                webBrowser.DocumentText = string.Format(_cachedSafeNoMetadataTemplate, nodeName, node.Parent, "&lt;empty&gt;", "No Metadata",
                    "This is typical for items that are not a datasource, i.e. Categories/Folders, Group Layers, MXDs, PDFs, etc.");
            else
            {
                try
                {
                    await node.Metadata.DisplayAsync(webBrowser, styleSheetComboBox.SelectedItem as StyleSheet);
                }
                catch (MetadataDisplayException ex)
                {
                    webBrowser.DocumentText = string.Format(_cachedSafeNoMetadataTemplate, nodeName, node.Parent, node.Metadata.Path, ex.Message, ex);
                }
            }
        }

        //See the html file at Properties.Settings.Default.HTMLNoMetadata for the set of substitutions that may transform the html.
        //This should return an html string for formatting with 5 substitutions
        //This html is used whenever there are problems displaying the metadata for a theme
        private string GetSafeNoMetadataTemplate()
        {
            string result;
            string safetyNet = "<HTML><BODY><H2>Metadata Problems</H2>Theme = <B>{0}</B><BR/>Metadata = <I>{2}</I><BR/>Error = {3}<BR/><I>{4}</I></BODY></HTML>";
            string noMetadataFile = Settings.Default.HTMLNoMetadata;
            if (!File.Exists(noMetadataFile)) {
                return safetyNet;
            }
            try
            {
                result = File.ReadAllText(noMetadataFile);
                // No substitutions are made here, this is just to test that the data I got off the disk is in an acceptable format
                string.Format(result, "Name", "Node", "Path", "Message", "Exception");
            }
            catch (Exception)
            {
                result = safetyNet;
            }
            return result;
        }

        private void LoadDefaultBrowser()
        {
            if (!Settings.Default.DisplayDefaultHtml)
                webBrowser.DocumentText = "";
            else
            {
                if (_cachedStartupUri == null && _invalidUriText == null)
                {
                    if (string.IsNullOrEmpty(Settings.Default.DefaultHtml))
                        _invalidUriText = "";
                    else
                    {
                        _cachedStartupUri = GetUri(Settings.Default.DefaultHtml);
                        if (_cachedStartupUri == null)
                            _invalidUriText = "URL/Path (" + Settings.Default.DefaultHtml + ") is not valid.";
                    }
                }

                if (_invalidUriText != null)
                    webBrowser.DocumentText = _invalidUriText;
                else
                    webBrowser.Url = _cachedStartupUri;
            }
        }

        private Uri GetUri(string Url)
        {
            Debug.Assert(!string.IsNullOrEmpty(Url), "Url is null or empty");

            Uri result;
            if (File.Exists(Url))
                result = new Uri(Path.GetFullPath(Url));
            else
                try
                {
                    result = new Uri(Url);
                }
                catch (UriFormatException)
                {
                    result = null;
                }
            return result;
        }

        private void LoadBrowser(string url)
        {
            if (string.IsNullOrEmpty(url))
                webBrowser.DocumentText = "";
            else
            {
                Uri uri = GetUri(url);
                if (uri == null)
                    webBrowser.DocumentText = "URL/Path (" + url + ") is not valid.";
                else
                    webBrowser.Url = uri;
            }
        }

        #endregion

        #region ArcGIS Preview Page

        private PreviewPage _previewPage;

        private void DisplayPreview(TmNode node)
        {
            Trace.TraceInformation("Display map preview for node: " + (node == null ? "null" : node.ToString()));

            if (_previewPage == null)
            {
                _previewPage = new PreviewPage(tabPage5, this);
            }

            if (node == null || node.IsCategory || node.IsThemeList)
            {
                _previewPage.ShowText("Select a theme");
            }
            else
            {
                if (node.HasDataToPreview)
                {
                    _previewPage.ShowMap(node.Data.Path);
                }
                else
                {
                    _previewPage.ShowText("Theme cannot be previewed, please select another theme");
                }
            }
        }

        #endregion

        #region Properties Page

        private PropertiesForm propertiesForm;

        private void DisplayProperties(TmNode node)
        {
            Trace.TraceInformation("Display properties for node: " + (node == null ? "null" : node.ToString()));

            if (node == null)
            {
                ShowTextInPropertiesPage("Select a Theme List, Category, or Theme");
            }
            else
            {
                DisplayPropertyPanel(tabPage6.Controls, node);
            }
        }

        private void ShowTextInPropertiesPage(string text)
        {
            if (tabPage6.Controls[0] != propertiesLabel)
            {
                tabPage6.Controls.Clear();
                tabPage6.Controls.Add(propertiesLabel);
            }
            propertiesLabel.Text = text;
        }

        private void DisplayPropertyPanel(Control.ControlCollection controls, TmNode node)
        {
            controls.Clear();
            if (node == null)
                return;
            if (propertiesForm == null)
                propertiesForm = new PropertiesForm().CommonInit();
            if (node.Type == TmNodeType.ThemeList)
            {
                AddPropertyPanelToForm(controls, propertiesForm.themelistPanel, node.IsReadOnly);
                PopulateThemeListPropertyPanel(propertiesForm, node);
            }
            if (node.Type == TmNodeType.Category)
            {
                AddPropertyPanelToForm(controls, propertiesForm.categoryPanel, node.IsReadOnly);
                PopulateCategoryPropertyPanel(propertiesForm, node);
            }
            if (node.Type == TmNodeType.Theme)
            {
                AddPropertyPanelToForm(controls, propertiesForm.themePanel, node.IsReadOnly);
                PopulateThemePropertyPanel(propertiesForm, node);
            }
        }

        private void AddPropertyPanelToForm(Control.ControlCollection controls, Panel panel, bool readOnly)
        {
            controls.Add(panel);
            panel.Dock = DockStyle.Fill;

            foreach (Control item in panel.Controls)
                if (item is TextBox)
                {
                    ((TextBox)item).ReadOnly = readOnly;
                    //((TextBox)item).ForeColor = SystemColors.GrayText;
                }
                else
                    item.Enabled = !readOnly;
        }

        private void PopulateThemeListPropertyPanel(PropertiesForm form, TmNode node)
        {
            Debug.Assert(node != null, "PropertyPanel has no Themelist node");
            Debug.Assert(node.Author != null, "Themelist node has no author object");
            Debug.Assert(node.Data != null, "TMNode node has no no data object");
            Debug.Assert(node.Metadata != null, "TMNode node has no metadata object");

            form.themelistName.DataBindings.Clear();
            form.themelistFile.DataBindings.Clear();
            form.themelistMetadata.DataBindings.Clear();
            form.themelistDescription.DataBindings.Clear();
            form.themelistAuthorName.DataBindings.Clear();
            form.themelistAuthorTitle.DataBindings.Clear();
            form.themelistAuthorOrg.DataBindings.Clear();
            form.themelistAuthorAddress1.DataBindings.Clear();
            form.themelistAuthorAddress2.DataBindings.Clear();
            form.themelistAuthorPhone.DataBindings.Clear();
            form.themelistAuthorEmail.DataBindings.Clear();

            form.themelistName.DataBindings.Add(new Binding("Text", node, "Name"));
            form.themelistFile.DataBindings.Add(new Binding("Text", node.Data, "Path"));
            form.themelistMetadata.DataBindings.Add(new Binding("Text", node.Metadata, "Path"));
            form.themelistDescription.DataBindings.Add(new Binding("Text", node, "Description"));
            form.themelistAuthorName.DataBindings.Add(new Binding("Text", node.Author, "Name"));
            form.themelistAuthorTitle.DataBindings.Add(new Binding("Text", node.Author, "Title"));
            form.themelistAuthorOrg.DataBindings.Add(new Binding("Text", node.Author, "Organization"));
            form.themelistAuthorAddress1.DataBindings.Add(new Binding("Text", node.Author, "Address1"));
            form.themelistAuthorAddress2.DataBindings.Add(new Binding("Text", node.Author, "Address2"));
            form.themelistAuthorPhone.DataBindings.Add(new Binding("Text", node.Author, "Phone"));
            form.themelistAuthorEmail.DataBindings.Add(new Binding("Text", node.Author, "Email"));
        }

        private void PopulateCategoryPropertyPanel(PropertiesForm form, TmNode node)
        {
            Debug.Assert(node.Data != null, "TMNode node has no no data object");
            Debug.Assert(node.Metadata != null, "TMNode node has no metadata object");

            form.categoryName.DataBindings.Clear();
            form.categoryMetadata.DataBindings.Clear();
            form.categoryDescription.DataBindings.Clear();
            //form.categoryAge.DataBindings.Clear();
            //form.categoryHide.DataBindings.Clear();

            form.categoryName.DataBindings.Add(new Binding("Text", node, "Name"));
            form.categoryMetadata.DataBindings.Add(new Binding("Text", node.Metadata, "Path"));
            form.categoryDescription.DataBindings.Add(new Binding("Text", node, "Description"));
            //form.categoryAge.DataBindings.Add(new Binding("Text", node, "DaysSinceNewestPublication"));
            //form.categoryHide.DataBindings.Add(new Binding("Checked", node, "IsHidden"));
        }

        private void PopulateThemePropertyPanel(PropertiesForm form, TmNode node)
        {
            Debug.Assert(node.Data != null, "TMNode node has no no data object");
            Debug.Assert(node.Metadata != null, "TMNode node has no metadata object");

            form.themeName.DataBindings.Clear();
            form.themeFile.DataBindings.Clear();
            form.themeMetadata.DataBindings.Clear();
            form.themeTags.DataBindings.Clear();
            form.themeSummary.DataBindings.Clear();
            form.themeDescription.DataBindings.Clear();
            form.themePubDate.DataBindings.Clear();
            form.themeDataType.DataBindings.Clear();
            form.themeDataSource.DataBindings.Clear();
            //form.themeAge.DataBindings.Clear();
            //form.themeHide.DataBindings.Clear();

            form.themeName.DataBindings.Add(new Binding("Text", node, "Name"));
            form.themeFile.DataBindings.Add(new Binding("Text", node.Data, "Path"));
            form.themeDataType.DataBindings.Add(new Binding("Text", node.Data, "Type"));
            form.themeDataSource.DataBindings.Add(new Binding("Text", node.Data, "DataSource"));
            form.themeMetadata.DataBindings.Add(new Binding("Text", node.Metadata, "Path"));
            form.themeTags.DataBindings.Add(new Binding("Text", node, "Tags"));
            form.themeSummary.DataBindings.Add(new Binding("Text", node, "Summary"));
            form.themeDescription.DataBindings.Add(new Binding("Text", node, "Description"));
            form.themePubDate.DataBindings.Add(new Binding("Value", node, "PubDate"));
            //form.themeAge.DataBindings.Add(new Binding("Text", node, "DaysSinceNewestPublication"));
            //form.themeHide.DataBindings.Add(new Binding("Checked", node, "IsHidden"));

            if (node.IsSubTheme)
            {
                form.themeName.Enabled = false;
                form.themeFile.Enabled = false;
                form.themeDataType.Enabled = false;
                form.reloadThemeButton.Visible = false;
                form.themeFileButton.Visible = false;
            }
            else
            {
                form.themeName.Enabled = true;
                form.themeFile.Enabled = true;
                form.themeDataType.Enabled = true;
                form.reloadThemeButton.Visible = true;
                form.themeFileButton.Visible = true;
            }
            form.syncThemeButton.Enabled = node.HasMetadata || node.HasChildren;
        }


        #endregion

        #region Save and Restore State/Settings

        private void SaveState()
        {
            if (themesTreeView.Nodes.Count > 0)
                //count == 0 may be valid, but most likely it is due to a loading error.
                //I want to avoid overwrite a possibly good session with an empty session
                SaveSession(); //attached theme lists, favorites, and search results
            SaveSettings();  //form size, picklist index, etc.
            //Return the ESRI license
            try //Protect against ESRI assembly not found errors
            {
                if (EsriLicenseManager.Running)
                    EsriLicenseManager.Stop();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not load EsriLicenseManager. " + ex.Message);
            }
        }

        private void RestoreForm()
        {
            LoadImageList();
            LoadStyleSheetPickList();
            LoadAgePickList();
            RestoreSettings();
            EnableButtonStrip();
            EnableMenuStrip();
            SetTreeViewToolTips();
        }

        private async Task RestoreStateAsync()
        {
            // These go quickly, so we don't need a status bar update.
            // status bar updates would require Application.DoEvents() to trigger a form redraw
            //progressBar.Visible = true;
            //statusBar.Text = "Loading previous session...";
            //Application.DoEvents();
            RestoreSession();
            SanitizeThemeLists();
            UpdateSortButton();
            //RestoreThemeLists();
            RestoreThemeListsInBackground();
            await DisplayMetadataAsync(null);
            progressBar.Visible = false;
            statusBar.Text = string.Empty;
        }

        private void RestoreThemeLists()
        {
            foreach (TmTreeNode treeNode in themesTreeView.Nodes.OfType<TmTreeNode>())
            {
                LoadIfNodeIsUnloadedThemeList(treeNode);
                treeNode.TmNode.PropertyChanged += nodePropertyChanged;
            }
        }

        private void RestoreThemeListsInBackground()
        {
            foreach (TmTreeNode treeNode in themesTreeView.Nodes.OfType<TmTreeNode>())
            {
                RestoreThemeListsWorker worker = new RestoreThemeListsWorker(treeNode, (node) => node.TmNode.PropertyChanged += nodePropertyChanged);
                Thread t = new Thread(worker.DoWork);
                t.Start();
            }
        }

        internal delegate void WorkerCallback(TmTreeNode node);

        class RestoreThemeListsWorker
        {
            public RestoreThemeListsWorker(TmTreeNode treeNode, WorkerCallback func)
            {
                _node = treeNode;
                _func = func;
            }

            private readonly TmTreeNode _node;
            private readonly WorkerCallback _func;

            public void DoWork()
            {
                LoadIfNodeIsUnloadedThemeList(_node);
                if (_func != null)
                    _func(_node);
            }
        }

        private void SaveSession()
        {
            XElement xml = new XElement(
                "thememanager",
                new XElement(
                    "themelists",
                    new XAttribute("sortorder", themesTreeView.TextSortOrder.ToString()),
                    from treenode in themesTreeView.Nodes.OfType<TmTreeNode>()
                    select treenode.TmNode.ToXElementWithoutChildren()
                    ),
                new XElement(
                    "favorites",
                    new XAttribute("sortorder", favoritesTreeView.TextSortOrder.ToString()),
                    from treenode in favoritesTreeView.Nodes.OfType<TmTreeNode>()
                    select treenode.TmNode.ToXElement()
                    ),
                new XElement(
                    "searches",
                    new XAttribute("sortorder", searchTreeView.TextSortOrder.ToString()),
                    from treenode in searchTreeView.Nodes.OfType<TmTreeNode>()
                    select treenode.TmNode.ToXElement()
                    )
                );
            string path = Path.Combine(Application.UserAppDataPath, Settings.Default.SavedSessionFile);
            xml.Save(path);
        }

        private void SaveSettings()
        {
            Settings.Default.StyleSheetIndex = styleSheetComboBox.SelectedIndex;
            Settings.Default.AgeComboIndex = ageComboBox.SelectedIndex;
            Settings.Default.mainFormSplitter = splitContainer.SplitterDistance;
              //Properties.Settings.Default.mainFormTabpage1 = this.listsTabControl.SelectedIndex;
            Settings.Default.mainFormState = WindowState;

            if (WindowState == FormWindowState.Normal)
            {
                Settings.Default.mainFormSize = Size;
                Settings.Default.mainFormLocation = Location;
            }
            else
            {
                Settings.Default.mainFormSize = RestoreBounds.Size;
                Settings.Default.mainFormLocation = RestoreBounds.Location;
            }
            SaveToolbars();
            Settings.Default.Save();
        }


        private void LoadImageList()
        {
            //try
            //{
            //    treeViewImageList.Images.Add("ThemeList", Image.FromFile(@"Images\DB.png"));
            //    treeViewImageList.Images.Add("Category", Image.FromFile(@"Images\category.png"));
            //    treeViewImageList.Images.Add("Theme", Image.FromFile(@"Images\LayerGeneric16.png"));
            //    treeViewImageList.Images.Add("Theme_anno", Image.FromFile(@"Images\LayerAnnotation16.png"));
            //    treeViewImageList.Images.Add("Theme_cad", Image.FromFile(@"Images\LayerCAD16.png"));
            //    treeViewImageList.Images.Add("Theme_group", Image.FromFile(@"Images\LayerGroup16.png"));
            //    treeViewImageList.Images.Add("Theme_line", Image.FromFile(@"Images\LayerLine16.png"));
            //    treeViewImageList.Images.Add("Theme_point", Image.FromFile(@"Images\LayerPoint16.png"));
            //    treeViewImageList.Images.Add("Theme_poly", Image.FromFile(@"Images\LayerPolygon16.png"));
            //    treeViewImageList.Images.Add("Theme_raster", Image.FromFile(@"Images\LayerRaster16.png"));
            //    treeViewImageList.Images.Add("Theme_wms", Image.FromFile(@"Images\LayerServiceMap16.png"));
            //    treeViewImageList.Images.Add("Theme_tin", Image.FromFile(@"Images\LayerTIN16.png"));
            //    treeViewImageList.Images.Add("Theme_mpatch", Image.FromFile(@"Images\LayerMultiPatch16.png"));
            //    treeViewImageList.Images.Add("Theme_mxd", Image.FromFile(@"Images\ArcMap_MXD_File16.png"));
            //    treeViewImageList.Images.Add("Theme_ge", Image.FromFile(@"Images\KmlIcon.png"));
            //    treeViewImageList.Images.Add("Theme_notfound", Image.FromFile(@"Images\LayerBroken16.png"));
            //    treeViewImageList.Images.Add("Theme_dim", Image.FromFile(@"Images\LayerDimension16.png"));
            //    treeViewImageList.Images.Add("Theme_network", Image.FromFile(@"Images\LayerNetwork16.png"));
            //    treeViewImageList.Images.Add("Theme_topo", Image.FromFile(@"Images\LayerTopology16.png"));
            //    treeViewImageList.Images.Add("Theme_terrain", Image.FromFile(@"Images\LayerTerrain16.png"));
            //    treeViewImageList.Images.Add("Theme_cadastral", Image.FromFile(@"Images\LayerCadastral16.png"));
            //    treeViewImageList.Images.Add("Theme_basemap", Image.FromFile(@"Images\LayerBasemap16.png"));
            //}
            //catch
            //{
            //    Debug.WriteLine("Unable to load images");
            //}

            //Debug.Assert(treeViewImageList.Images.Count == Enum.GetNames(typeof(TmNodeType)).Count(), "Count in image list does not match count in enum.tmnodetype");

            Image overlay = Resources.new_overlay;
            string name = "new";

            //foreach (string name in new[] { "lock", "new" })
            //{
                //string imageName = @"Images\" + name + "_overlay.png";
                //Image overlay;
                //try
                //{
                //    overlay = Image.FromFile(imageName);
                //}
                //catch
                //{
                //    Debug.WriteLine("Unable to load image named: " + imageName);
                //    overlay = new Bitmap(16,16);
                //}
                int imageCount = treeViewImageList.Images.Count;
                for (int i = 0; i < imageCount; i++)
                {
                    Image combo = (Image)treeViewImageList.Images[i].Clone();
                    Graphics gra = Graphics.FromImage(combo);
                    gra.DrawImage(overlay, 0, 0, 16, 16);
                    treeViewImageList.Images.Add(combo);
                    treeViewImageList.Images.SetKeyName(i + imageCount, treeViewImageList.Images.Keys[i] + name);
                }
            //}
        }

        private void LoadStyleSheetPickList()
        {
            try
            {
                styleSheetComboBox.Items.AddRange(new StyleSheetList().ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finding stylesheets. {ex.Message}.", "Oh No!");
            }
        }

        private void LoadAgePickList()
        {
            ageComboBox.BeginUpdate();
            ArrayList durations = new ArrayList();
            durations.AddRange(new[] {
                                         new Duration(1, "1 Day"),
                                         new Duration(7, "1 Week"),
                                         new Duration(14, "2 Weeks"),
                                         new Duration(21, "3 Weeks"),
                                         new Duration(30, "1 Month"),
                                         new Duration(60, "2 Months"),
                                         new Duration(90, "3 Months"),
                                         new Duration(180, "6 Months"),
                                         new Duration(365, "1 Year"),
                                         new Duration(730, "2 Years"),
                                         new Duration(1826, "5 Years")
                                     });
            ComboBox cb = (ComboBox)ageComboBox.Control;
            cb.DataSource = durations;
            cb.DisplayMember = "Description";
            cb.ValueMember = "Days";
            ageComboBox.EndUpdate();
        }

        private void RestoreSettings()
        {
            Text = Settings.Default.AppName;
            Size = Settings.Default.mainFormSize;
            Location = Settings.Default.mainFormLocation;
            SanitizeSizeAndLocation();
            if (Settings.Default.mainFormState != FormWindowState.Minimized)
                WindowState = Settings.Default.mainFormState;

            //            Debug.Print("Default Splitter distance = {0} pixels, SC Width = {1}, Form Width = {2}", splitContainer.SplitterDistance, splitContainer.Width, Size.Width);
            //            Debug.Print("Saved Splitter distance = {0} pixels", Settings.Default.mainFormSplitter);
            //splitContainer.SuspendLayout(); //suspends firing of resize events (which may trigger info reload)
            splitContainer.SplitterDistance = Settings.Default.mainFormSplitter;
            //splitContainer.ResumeLayout(false);
            //            Debug.Print("New Splitter distance = {0} pixels", splitContainer.SplitterDistance);

            int lastIndex = Settings.Default.AgeComboIndex;
            if (lastIndex >= 0 && lastIndex < ageComboBox.Items.Count)
                ageComboBox.SelectedIndex = lastIndex;
            else
                ageComboBox.SelectedIndex = 0;

            lastIndex = Settings.Default.StyleSheetIndex;
            if (lastIndex >= 0 && lastIndex < styleSheetComboBox.Items.Count)
                styleSheetComboBox.SelectedIndex = lastIndex;
            else
                styleSheetComboBox.SelectedIndex = 0;
            RestoreToolbars();
            InPlaceNodeEditing = Settings.Default.InPlaceNodeEditing;

            menuStrip1.Visible = Settings.Default.ShowMainMenu;
            toolStrip1.Visible = Settings.Default.ShowToolbar1;
            toolStrip2.Visible = Settings.Default.ShowToolbar2;
            statusStrip.Visible = Settings.Default.ShowStatusBar;

            //if (Settings.Default.ShowHiddenThemes)
            //    hideShowButton.Text = "Hide";
            //else
            //    hideShowButton.Text = "Show All";

            //wire up event handlers that we defered until after settings were loaded
            listsTabControl.SelectedIndexChanged += listsTabControl_SelectedIndexChanged;
            infoTabControl.SelectedIndexChanged += infoTabControl_SelectedIndexChanged;
            styleSheetComboBox.SelectedIndexChanged += styleSheetComboBox_SelectedIndexChanged;
            splitContainer.SplitterMoved += splitContainer_SplitterMoved;

        }

        internal void SetTreeViewToolTips()
        {
            foreach (var treeview in _treeViews)
                treeview.ShowNodeToolTips = Settings.Default.ShowThemeDescriptionToolTip;
        }

        /// <summary>
        /// Puts the form on the virtual screen
        /// </summary>
        /// <remarks>
        /// Work Area accounts for the taskbar, but not multiple monitors.
        /// Virtual screen accounts for multiple monitors, but not the taskbar
        /// The following may leave the form hidden behind the taksbar.
        /// </remarks>
        private void SanitizeSizeAndLocation()
        {
            int minX = SystemInformation.VirtualScreen.X + SystemInformation.MinimumWindowSize.Width - Size.Width;
            if (Location.X < minX)
                Location = new Point(minX, Location.Y);
            int minY = SystemInformation.VirtualScreen.Y;
            if (Location.Y < minY)
                Location = new Point(Location.X, minY);

            int maxX = SystemInformation.VirtualScreen.Width - SystemInformation.MinimumWindowSize.Width;
            if (Location.X > maxX)
                Location = new Point(maxX, Location.Y);
            int maxY = SystemInformation.VirtualScreen.Height - SystemInformation.MinimumWindowSize.Height;
            if (Location.Y > maxY)
                Location = new Point(Location.X, maxY);
        }

        private void RestoreSession()
        {
            string path = Path.Combine(Application.UserAppDataPath, Settings.Default.SavedSessionFile);
            if (File.Exists(path))
            {
                try
                {
                    LoadSessionFromXML(XElement.Load(path));
                    return;
                }
                catch (Exception ex) //and ignore
                {
                    Debug.WriteLine("Caught and ignored exception\n" + ex);
                }
            }
            if (FindAndLoadOldSession() == true)
                return;
            if (LoadSessionFromRegistry() == true)
                return;
            LoadDefaultThemes();
        }

        private void LoadSessionFromXML(XElement xele)
        {
            if (xele == null)
                throw new ArgumentNullException("xele");
            if (xele.Name != "thememanager")
                throw new ArgumentException("Invalid Xelement");
            XElement xnode = xele.Element("themelists");
            if (xnode != null)
                LoadTreeFromXML(xnode, themesTreeView);
            xnode = xele.Element("favorites");
            if (xnode != null)
                LoadTreeFromXML(xnode, favoritesTreeView);
            xnode = xele.Element("searches");
            if (xnode != null)
                LoadTreeFromXML(xnode, searchTreeView);
        }

        private void LoadTreeFromXML(XElement xele, TmTreeView tv)
        {
            TmNode node;
            string themelist = Enum.GetName(typeof(TmNodeType), TmNodeType.ThemeList).ToLower();
            foreach (XElement xnode in xele.Elements(themelist))
            {
                node = new TmNode(TmNodeType.ThemeList);
                node.Load(xnode, false); // Add only the first node
                tv.Add(node);
            }

            string category = Enum.GetName(typeof(TmNodeType), TmNodeType.Category).ToLower();
            foreach (XElement xnode in xele.Elements(category))
            {
                node = new TmNode(TmNodeType.Category);
                node.Load(xnode, true); // Add entire branch
                node.UpdateImageIndex(true);
                tv.Add(node);
            }

            string theme = Enum.GetName(typeof(TmNodeType), TmNodeType.Theme).ToLower();
            foreach (XElement xnode in xele.Elements(theme))
            {
                node = new TmNode(TmNodeType.Theme);
                node.Load(xnode, true); // Add entire branch
                node.UpdateImageIndex(true);
                tv.Add(node);
            }
            tv.TextSortInit((string)xele.Attribute("sortorder"));
        }

        private bool FindAndLoadOldSession()
        {
            //UserAppDataPath is user_settings_dir/Company/Application/Version
            string applicationDirectory = Path.GetDirectoryName(Application.UserAppDataPath);
            Trace.TraceInformation("Searching {0}", applicationDirectory);

            var versionDirectories =
                from dir in Directory.GetDirectories(applicationDirectory)
                orderby Directory.GetCreationTime(dir) descending
                select dir;

            foreach (string dir in versionDirectories)
            {
                Trace.TraceInformation("Searching {0}", dir);
                string path = Path.Combine(dir, Settings.Default.SavedSessionFile);
                Trace.TraceInformation("Looking for {0}", path);
                if (File.Exists(path))
                {
                    try
                    {
                        Trace.TraceInformation("Trying to load {0}", path);
                        LoadSessionFromXML(XElement.Load(path));
                        Trace.TraceInformation("Successfully loaded {0}", path);
                        return true;
                    }
                    catch (Exception ex) //and ignore
                    {
                        Debug.WriteLine("Caught and ignored exception\n"+ex);
                    }
                }
            }
            return false;
        }

        private bool LoadSessionFromRegistry()
        {
            // Newer versions used HKEY_CURRENT_USER, older versions used HKEY_LOCAL_MACHINE

            string[] favorites = null;
            string[] databases = LoadListFromRegistry(Settings.Default.RegistryUserDatabases);
            if (databases.Length > 0)
            {
                favorites = LoadListFromRegistry(Settings.Default.RegistryUserFavorites);
            }
            else
            {
                databases = LoadListFromRegistry(Settings.Default.RegistryMachineDatabases);
                if (databases.Length > 0)
                {
                    favorites = LoadListFromRegistry(Settings.Default.RegistryMachineFavorites);
                }
            }
            if (databases.Length > 0)
            {
                CreateThemeLists(databases);
                if (favorites != null && favorites.Length > 0)
                    LoadFavoritesFromRegistryList(favorites, databases);
                foreach (TmTreeView tv in _treeViews)
                    tv.TextSortInit();
                return true;
            }
            return false;
        }

        private static string[] LoadListFromRegistry(string registryKeyName)
        {
            // Databases in Registry: keys = 001, 002, ... values = full path to mdb (string)
            // Favorites in Registry: keys = 001, 002, ... values = node code LxxCxxTxx (string)
            List<string> values = new List<string>();
            int index = 0;
            string registryValue;
            do
            {
                index++;
                string registryValueName = string.Format("{0:D3}", index);
                registryValue = Registry.GetValue(registryKeyName, registryValueName, null) as string;
                if (registryValue != null)
                    values.Add(registryValue);
            } while (registryValue != null);
            return values.ToArray();
        }

        private void CreateThemeLists(IEnumerable<string> databases)
        {
            foreach (string database in databases)
            {
                if (!File.Exists(database))
                    MessageBox.Show("Error adding theme list.\nFile not found: " + database);
                else
                    if (themesTreeView.FindThemeListNode(database) == null)
                        themesTreeView.Add(new TmNode(TmNodeType.ThemeList, database, null, new ThemeData(database), null, null, null));
            }
        }

        private void RemoveThemeLists(IEnumerable<string> databases)
        {
            foreach (string database in databases)
            {
                TmTreeNode node = themesTreeView.FindThemeListNode(database);
                if (node != null)
                    themesTreeView.RemoveThemeListNode(node);
            }
        }

        private void LoadFavoritesFromRegistryList(IEnumerable<string> favorites, string[] databases)
        {
            TmNode themeList;
            foreach (string favorite in favorites)
            {
                string database = DatabaseOfFavorite(favorite, databases);
                themeList = ThemeListFromDatabasePath(database);
                //if (themeList == null)
                //    MessageBox.Show("Favorite not loaded because database could not be found.");

                //If I am loading from the registry, then I need to build the themeslist,
                //since the favorites in the registry are pointers into the themelist
                if (themeList != null)
                    foreach (TmTreeNode treeNode in themesTreeView.Nodes.OfType<TmTreeNode>())
                        if (treeNode.TmNode == themeList)
                            if (LoadIfNodeIsUnloadedThemeList(treeNode))
                            {
                                try
                                {
                                    TmNode node = themeList.BuildNodeFromId(ThemeIdOfFavorite(favorite));
                                    favoritesTreeView.Add(node);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error loading favorites from registry.\n" + ex);
                                }
                            }
            }
        }

        private static string DatabaseOfFavorite(string favorite, string[] databases)
        {
            //favorite is a string code for the node that looks like: LxxCxxTxx or LxxSxxTxx
            //where xx is an integer, L = list, S = subcategory, C = category, T = theme
            //The databases are in order in the array, so L1 = string[0], etc.
            string themeListId = favorite.Split(new[] { 'L', 'S', 'C', 'T' },StringSplitOptions.RemoveEmptyEntries)[0];
            //foreach (string t in themeListId)
            //    Trace.TraceInformation("List IDs '{0}'", t);
            Trace.TraceInformation("DatabaseOfFavorite: favorite string: {0}, List ID {1}", favorite, themeListId);
            int index = Convert.ToInt32(themeListId);
            Debug.Assert(index > 0, "DatabaseOfFavorite: Index is not greater than zero");
            Debug.Assert(index <= databases.Length, "DatabaseOfFavorite: Index is greater than list");
            return databases[index - 1];
        }

        private TmNode ThemeListFromDatabasePath(string database)
        {
            if (string.IsNullOrEmpty(database))
                return null;
            foreach (TmTreeNode node in themesTreeView.Nodes.OfType<TmTreeNode>())
                // Assert node.TmNode and node.TmNode.Data are not null
                if (node.TmNode.Data.Path == database)
                    return node.TmNode;
            return null;
        }

        private static int ThemeIdOfFavorite(string favorite)
        {
            //favorite is a string code for the node that looks like: LxxCxxTxx or LxxSxxTxx
            //where xx is an integer, L = list, S = subcategory, C = category, T = theme
            string themeId = favorite.Split(new[] {'T'})[1];
            return Convert.ToInt32(themeId);
        }

        private void LoadDefaultThemes()
        {
            string[] databases = Settings.Default.DefaultDatabases
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (databases.Length > 0)
                CreateThemeLists(databases);
            else
                LoadBrowser(Settings.Default.HTMLNoInitialData);
            foreach (TmTreeView tv in _treeViews)
                tv.TextSortInit();
        }

        /// <summary>
        /// Adds required Themelists, and removes Obsolete ThemeLists
        /// </summary>
        private void SanitizeThemeLists()
        {
            if (Settings.Default.LoadRequiredThemeLists)
            {
                string[] themeListsToAdd = Settings.Default.RequiredThemeLists
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (themeListsToAdd.Length > 0)
                    CreateThemeLists(themeListsToAdd.Select(d => d.Trim()));
            }
            if (Settings.Default.RemoveObsoleteThemeLists)
            {
                string[] themeListsToRemove = Settings.Default.ObsoleteThemeLists
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (themeListsToRemove.Length > 0)
                    RemoveThemeLists(themeListsToRemove.Select(d => d.Trim()));
            }
        }


        private void SaveToolbars()
        {
            Settings.Default.Toolstrip1Location = toolStrip1.Location;
            Settings.Default.Toolstrip1ParentName = GetToolStripParentName(toolStrip1);
            Settings.Default.Toolstrip2Location = toolStrip2.Location;
            Settings.Default.Toolstrip2ParentName = GetToolStripParentName(toolStrip2);
        }

        private void RestoreToolbars()
        {
            toolStripContainer1.SuspendLayout();
            toolStrip1.Parent = GetToolStripParentByName(toolStripContainer1, Settings.Default.Toolstrip1ParentName) ?? toolStrip1.Parent;
            toolStrip1.Location = Settings.Default.Toolstrip1Location;
            toolStrip2.Parent = GetToolStripParentByName(toolStripContainer1, Settings.Default.Toolstrip2ParentName) ?? toolStrip2.Parent;
            toolStrip2.Location = Settings.Default.Toolstrip2Location;
            toolStripContainer1.ResumeLayout(true);
        }

        private string GetToolStripParentName(ToolStrip toolStrip)
        {
            ToolStripPanel panel = toolStrip.Parent as ToolStripPanel;
            string defaultName = String.Empty;

            if (panel == null)
            {
                return defaultName;
            }

            ToolStripContainer container = panel.Parent as ToolStripContainer;

            if (container == null)
            {
                return defaultName;
            }

            if (panel == container.LeftToolStripPanel)
            {
                return "LeftToolStripPanel";
            }

            if (panel == container.RightToolStripPanel)
            {
                return "RightToolStripPanel";
            }

            if (panel == container.TopToolStripPanel)
            {
                return "TopToolStripPanel";
            }

            if (panel == container.BottomToolStripPanel)
            {
                return "BottomToolStripPanel";
            }

            return defaultName;
        }

        private ToolStripPanel GetToolStripParentByName(ToolStripContainer container, string parentName)
        {
            if (parentName == "LeftToolStripPanel")
            {
                return container.LeftToolStripPanel;
            }

            if (parentName == "RightToolStripPanel")
            {
                return container.RightToolStripPanel;
            }

            if (parentName == "TopToolStripPanel")
            {
                return container.TopToolStripPanel;
            }

            if (parentName == "BottomToolStripPanel")
            {
                return container.BottomToolStripPanel;
            }

            return null;
        }

        #endregion

        //private void hideShowButton_Click(object sender, EventArgs e)
        //{
        //    if (Settings.Default.ShowHiddenThemes)
        //    {
        //        Settings.Default.ShowHiddenThemes = false;
        //        CurrentTreeView.HideHidden();
        //        hideShowButton.Text = "Show All";
        //    }
        //    else
        //    {
        //        Settings.Default.ShowHiddenThemes = true;
        //        CurrentTreeView.ShowHidden();
        //        hideShowButton.Text = "Hide";
        //    }
        //}

    }


}
