namespace NPS.AKRO.ThemeManager.UI.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                _activeSearchBoxFont.Dispose();
                _inactiveSearchBoxFont.Dispose();
                //m_BackgroundColorForNewThemes.Dispose();
                //m_TextColorForNewThemes.Dispose();

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.listsTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.themesTreeView = new NPS.AKRO.ThemeManager.UI.TmTreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newThemeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToFavoritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.favoritesTreeView = new NPS.AKRO.ThemeManager.UI.TmTreeView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.searchTreeView = new NPS.AKRO.ThemeManager.UI.TmTreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.newThemeListToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.newCategoryToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.newThemeToolStripButton = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.printToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.deleteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addToFavoritesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sortToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.preferencesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.viewHelpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.infoTabControl = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.previewLabel = new System.Windows.Forms.Button();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.propertiesLabel = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newThemeListToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.newCategoryToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.newThemeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.searchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToFavoritesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sortToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.launchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.administrativeToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickStartTutorialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHelpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.styleSheetComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.ageComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.searchTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.searchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listsTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.infoTabControl.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listsTabControl
            // 
            this.listsTabControl.Controls.Add(this.tabPage1);
            this.listsTabControl.Controls.Add(this.tabPage2);
            this.listsTabControl.Controls.Add(this.tabPage3);
            this.listsTabControl.DataBindings.Add(new System.Windows.Forms.Binding("SelectedIndex", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "mainFormTabpage1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.listsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listsTabControl.Location = new System.Drawing.Point(0, 0);
            this.listsTabControl.Name = "listsTabControl";
            this.listsTabControl.SelectedIndex = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.mainFormTabpage1;
            this.listsTabControl.Size = new System.Drawing.Size(232, 486);
            this.listsTabControl.TabIndex = 11;
            this.toolTip1.SetToolTip(this.listsTabControl, "A list of all themes in your databases");
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.themesTreeView);
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(224, 448);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Themes";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // themesTreeView
            // 
            this.themesTreeView.AllowDrop = true;
            this.themesTreeView.ContextMenuStrip = this.contextMenuStrip1;
            this.themesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.themesTreeView.HideSelection = false;
            this.themesTreeView.ImageIndex = 0;
            this.themesTreeView.ImageList = this.treeViewImageList;
            this.themesTreeView.LabelEdit = true;
            this.themesTreeView.Location = new System.Drawing.Point(3, 3);
            this.themesTreeView.Name = "themesTreeView";
            this.themesTreeView.SelectedImageIndex = 0;
            this.themesTreeView.Size = new System.Drawing.Size(218, 442);
            this.themesTreeView.Sorted = true;
            this.themesTreeView.TabIndex = 8;
            this.themesTreeView.Tag = "Themes";
            this.themesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCategoryToolStripMenuItem,
            this.newThemeToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator5,
            this.searchToolStripMenuItem,
            this.launchToolStripMenuItem,
            this.addToFavoritesToolStripMenuItem,
            this.preferencesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(224, 346);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // newCategoryToolStripMenuItem
            // 
            this.newCategoryToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newCategory;
            this.newCategoryToolStripMenuItem.Name = "newCategoryToolStripMenuItem";
            this.newCategoryToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.newCategoryToolStripMenuItem.Text = "New Category";
            this.newCategoryToolStripMenuItem.Click += new System.EventHandler(this.newCategoryToolStripMenuItem_Click);
            // 
            // newThemeToolStripMenuItem
            // 
            this.newThemeToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newTheme;
            this.newThemeToolStripMenuItem.Name = "newThemeToolStripMenuItem";
            this.newThemeToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.newThemeToolStripMenuItem.Text = "New Theme";
            this.newThemeToolStripMenuItem.Click += new System.EventHandler(this.newThemeToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.save;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.ToolTipText = "Saves selected item as a new theme list";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(220, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.cut;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.paste;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(220, 6);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.search;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.searchToolStripMenuItem.Text = "Search";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.advancedSearchToolStripMenuItem_Click);
            // 
            // launchToolStripMenuItem
            // 
            this.launchToolStripMenuItem.Name = "launchToolStripMenuItem";
            this.launchToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.launchToolStripMenuItem.Text = "Open Theme(s)";
            this.launchToolStripMenuItem.Click += new System.EventHandler(this.launchToolStripMenuItem_Click);
            // 
            // addToFavoritesToolStripMenuItem
            // 
            this.addToFavoritesToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.Favorites;
            this.addToFavoritesToolStripMenuItem.Name = "addToFavoritesToolStripMenuItem";
            this.addToFavoritesToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.addToFavoritesToolStripMenuItem.Text = "Add to Favorites";
            this.addToFavoritesToolStripMenuItem.Click += new System.EventHandler(this.add2FavToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.tools;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(223, 30);
            this.preferencesToolStripMenuItem.Text = "Options...";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripButton_Click);
            // 
            // treeViewImageList
            // 
            this.treeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewImageList.ImageStream")));
            this.treeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeViewImageList.Images.SetKeyName(0, "Theme");
            this.treeViewImageList.Images.SetKeyName(1, "ThemeList");
            this.treeViewImageList.Images.SetKeyName(2, "ThemeListlock");
            this.treeViewImageList.Images.SetKeyName(3, "Category");
            this.treeViewImageList.Images.SetKeyName(4, "Theme_anno");
            this.treeViewImageList.Images.SetKeyName(5, "Theme_cad");
            this.treeViewImageList.Images.SetKeyName(6, "Theme_group");
            this.treeViewImageList.Images.SetKeyName(7, "Theme_line");
            this.treeViewImageList.Images.SetKeyName(8, "Theme_point");
            this.treeViewImageList.Images.SetKeyName(9, "Theme_poly");
            this.treeViewImageList.Images.SetKeyName(10, "Theme_raster");
            this.treeViewImageList.Images.SetKeyName(11, "Theme_wms");
            this.treeViewImageList.Images.SetKeyName(12, "Theme_tin");
            this.treeViewImageList.Images.SetKeyName(13, "theme_mpatch");
            this.treeViewImageList.Images.SetKeyName(14, "Theme_mxd");
            this.treeViewImageList.Images.SetKeyName(15, "Theme_ge");
            this.treeViewImageList.Images.SetKeyName(16, "Theme_notfound");
            this.treeViewImageList.Images.SetKeyName(17, "Theme_dim");
            this.treeViewImageList.Images.SetKeyName(18, "Theme_network");
            this.treeViewImageList.Images.SetKeyName(19, "Theme_topo");
            this.treeViewImageList.Images.SetKeyName(20, "Theme_terrain");
            this.treeViewImageList.Images.SetKeyName(21, "Theme_cadastral");
            this.treeViewImageList.Images.SetKeyName(22, "Theme_basemap");
            this.treeViewImageList.Images.SetKeyName(23, "theme_dataframe");
            this.treeViewImageList.Images.SetKeyName(24, "Theme_pdf");
            this.treeViewImageList.Images.SetKeyName(25, "Theme_doc");
            this.treeViewImageList.Images.SetKeyName(26, "Theme_xls");
            this.treeViewImageList.Images.SetKeyName(27, "Theme_ppt");
            this.treeViewImageList.Images.SetKeyName(28, "Theme_mosaic");
            this.treeViewImageList.Images.SetKeyName(29, "Theme_las");
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.favoritesTreeView);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(224, 453);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Favorites";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // favoritesTreeView
            // 
            this.favoritesTreeView.AllowDrop = true;
            this.favoritesTreeView.ContextMenuStrip = this.contextMenuStrip1;
            this.favoritesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.favoritesTreeView.HideSelection = false;
            this.favoritesTreeView.ImageIndex = 0;
            this.favoritesTreeView.ImageList = this.treeViewImageList;
            this.favoritesTreeView.LabelEdit = true;
            this.favoritesTreeView.Location = new System.Drawing.Point(3, 3);
            this.favoritesTreeView.Name = "favoritesTreeView";
            this.favoritesTreeView.SelectedImageIndex = 0;
            this.favoritesTreeView.Size = new System.Drawing.Size(218, 447);
            this.favoritesTreeView.Sorted = true;
            this.favoritesTreeView.TabIndex = 0;
            this.favoritesTreeView.Tag = "Favorites";
            this.favoritesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.searchTreeView);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(224, 453);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Search Results";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // searchTreeView
            // 
            this.searchTreeView.AllowDrop = true;
            this.searchTreeView.ContextMenuStrip = this.contextMenuStrip1;
            this.searchTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTreeView.HideSelection = false;
            this.searchTreeView.ImageIndex = 0;
            this.searchTreeView.ImageList = this.treeViewImageList;
            this.searchTreeView.LabelEdit = true;
            this.searchTreeView.Location = new System.Drawing.Point(3, 3);
            this.searchTreeView.Name = "searchTreeView";
            this.searchTreeView.SelectedImageIndex = 0;
            this.searchTreeView.Size = new System.Drawing.Size(218, 447);
            this.searchTreeView.Sorted = true;
            this.searchTreeView.TabIndex = 10;
            this.searchTreeView.Tag = "Search Results";
            this.searchTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.printToolStripButton,
            this.toolStripSeparator,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.deleteToolStripButton,
            this.toolStripSeparator1,
            this.addToFavoritesToolStripButton,
            this.sortToolStripButton,
            this.preferencesToolStripButton,
            this.toolStripSeparator3,
            this.viewHelpToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 33);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(380, 31);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newThemeListToolStripButton,
            this.newCategoryToolStripButton,
            this.newThemeToolStripButton});
            this.newToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newDocument;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(42, 28);
            this.newToolStripButton.Text = "&New";
            // 
            // newThemeListToolStripButton
            // 
            this.newThemeListToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newDB;
            this.newThemeListToolStripButton.Name = "newThemeListToolStripButton";
            this.newThemeListToolStripButton.Size = new System.Drawing.Size(220, 30);
            this.newThemeListToolStripButton.Text = "New Theme List";
            this.newThemeListToolStripButton.Click += new System.EventHandler(this.newThemeListToolStripMenuItem_Click);
            // 
            // newCategoryToolStripButton
            // 
            this.newCategoryToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newCategory;
            this.newCategoryToolStripButton.Name = "newCategoryToolStripButton";
            this.newCategoryToolStripButton.Size = new System.Drawing.Size(220, 30);
            this.newCategoryToolStripButton.Text = "New Category";
            this.newCategoryToolStripButton.Click += new System.EventHandler(this.newCategoryToolStripMenuItem_Click);
            // 
            // newThemeToolStripButton
            // 
            this.newThemeToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newTheme;
            this.newThemeToolStripButton.Name = "newThemeToolStripButton";
            this.newThemeToolStripButton.Size = new System.Drawing.Size(220, 30);
            this.newThemeToolStripButton.Text = "New Theme";
            this.newThemeToolStripButton.Click += new System.EventHandler(this.newThemeToolStripMenuItem_Click);
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.open;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.save;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.saveToolStripButton.Text = "&Save";
            this.saveToolStripButton.ToolTipText = "Saves all modified theme lists";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // printToolStripButton
            // 
            this.printToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.print;
            this.printToolStripButton.Name = "printToolStripButton";
            this.printToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.printToolStripButton.Text = "&Print";
            this.printToolStripButton.Click += new System.EventHandler(this.printToolStripButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 31);
            // 
            // cutToolStripButton
            // 
            this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.cut;
            this.cutToolStripButton.Name = "cutToolStripButton";
            this.cutToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.cutToolStripButton.Text = "C&ut";
            this.cutToolStripButton.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripButton
            // 
            this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.copy;
            this.copyToolStripButton.Name = "copyToolStripButton";
            this.copyToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.copyToolStripButton.Text = "&Copy";
            this.copyToolStripButton.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripButton
            // 
            this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.paste;
            this.pasteToolStripButton.Name = "pasteToolStripButton";
            this.pasteToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.pasteToolStripButton.Text = "&Paste";
            this.pasteToolStripButton.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripButton
            // 
            this.deleteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.delete;
            this.deleteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteToolStripButton.Name = "deleteToolStripButton";
            this.deleteToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.deleteToolStripButton.Text = "Delete";
            this.deleteToolStripButton.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // addToFavoritesToolStripButton
            // 
            this.addToFavoritesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addToFavoritesToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.Favorites;
            this.addToFavoritesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addToFavoritesToolStripButton.Name = "addToFavoritesToolStripButton";
            this.addToFavoritesToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.addToFavoritesToolStripButton.Text = "Add to Favorites";
            this.addToFavoritesToolStripButton.Click += new System.EventHandler(this.add2FavToolStripMenuItem_Click);
            // 
            // sortToolStripButton
            // 
            this.sortToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sortToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.SortAZ;
            this.sortToolStripButton.Name = "sortToolStripButton";
            this.sortToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.sortToolStripButton.Text = "toolStripButton1";
            this.sortToolStripButton.ToolTipText = "Sort";
            this.sortToolStripButton.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
            // 
            // preferencesToolStripButton
            // 
            this.preferencesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.preferencesToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.tools;
            this.preferencesToolStripButton.Name = "preferencesToolStripButton";
            this.preferencesToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.preferencesToolStripButton.Text = "toolStripButton1";
            this.preferencesToolStripButton.ToolTipText = "Options";
            this.preferencesToolStripButton.Click += new System.EventHandler(this.preferencesToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // viewHelpToolStripButton
            // 
            this.viewHelpToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.viewHelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewHelpToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.help;
            this.viewHelpToolStripButton.Name = "viewHelpToolStripButton";
            this.viewHelpToolStripButton.Size = new System.Drawing.Size(28, 28);
            this.viewHelpToolStripButton.Text = "&Help";
            this.viewHelpToolStripButton.Click += new System.EventHandler(this.helpToolStripButton_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(807, 490);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(807, 617);
            this.toolStripContainer1.TabIndex = 15;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.statusBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(807, 30);
            this.statusStrip.TabIndex = 0;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 24);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // statusBar
            // 
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(140, 25);
            this.statusBar.Text = "Theme Manager";
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listsTabControl);
            this.splitContainer.Panel1MinSize = 0;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.infoTabControl);
            this.splitContainer.Panel2MinSize = 0;
            this.splitContainer.Size = new System.Drawing.Size(807, 490);
            this.splitContainer.SplitterDistance = 236;
            this.splitContainer.SplitterWidth = 5;
            this.splitContainer.TabIndex = 12;
            // 
            // infoTabControl
            // 
            this.infoTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoTabControl.Controls.Add(this.tabPage4);
            this.infoTabControl.Controls.Add(this.tabPage5);
            this.infoTabControl.Controls.Add(this.tabPage6);
            this.infoTabControl.DataBindings.Add(new System.Windows.Forms.Binding("SelectedIndex", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "mainFormTabpage2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.infoTabControl.Location = new System.Drawing.Point(0, 0);
            this.infoTabControl.Name = "infoTabControl";
            this.infoTabControl.SelectedIndex = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.mainFormTabpage2;
            this.infoTabControl.Size = new System.Drawing.Size(557, 486);
            this.infoTabControl.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.webBrowser);
            this.tabPage4.Location = new System.Drawing.Point(4, 34);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(549, 448);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Metadata";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(3, 3);
            this.webBrowser.MinimumSize = new System.Drawing.Size(23, 23);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(543, 442);
            this.webBrowser.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage5.Controls.Add(this.previewLabel);
            this.tabPage5.Location = new System.Drawing.Point(4, 29);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(549, 453);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "Preview";
            // 
            // previewLabel
            // 
            this.previewLabel.BackColor = System.Drawing.SystemColors.Window;
            this.previewLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewLabel.Enabled = false;
            this.previewLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.previewLabel.Location = new System.Drawing.Point(3, 3);
            this.previewLabel.Name = "previewLabel";
            this.previewLabel.Size = new System.Drawing.Size(543, 447);
            this.previewLabel.TabIndex = 0;
            this.previewLabel.Text = "Select a theme";
            this.previewLabel.UseVisualStyleBackColor = false;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.propertiesLabel);
            this.tabPage6.Location = new System.Drawing.Point(4, 29);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(549, 453);
            this.tabPage6.TabIndex = 2;
            this.tabPage6.Text = "Properties";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // propertiesLabel
            // 
            this.propertiesLabel.BackColor = System.Drawing.SystemColors.Window;
            this.propertiesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesLabel.Enabled = false;
            this.propertiesLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.propertiesLabel.Location = new System.Drawing.Point(3, 3);
            this.propertiesLabel.Name = "propertiesLabel";
            this.propertiesLabel.Size = new System.Drawing.Size(543, 447);
            this.propertiesLabel.TabIndex = 1;
            this.propertiesLabel.Text = "Select a theme";
            this.propertiesLabel.UseVisualStyleBackColor = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(807, 33);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newThemeListToolStripMenuItem1,
            this.newCategoryToolStripMenuItem1,
            this.newThemeToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.openToolStripMenuItem1,
            this.toolStripMenuItem2,
            this.saveToolStripMenuItem1,
            this.saveAsToolStripMenuItem1,
            this.printToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newThemeListToolStripMenuItem1
            // 
            this.newThemeListToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newDB;
            this.newThemeListToolStripMenuItem1.Name = "newThemeListToolStripMenuItem1";
            this.newThemeListToolStripMenuItem1.Size = new System.Drawing.Size(294, 30);
            this.newThemeListToolStripMenuItem1.Text = "New Theme &List";
            this.newThemeListToolStripMenuItem1.Click += new System.EventHandler(this.newThemeListToolStripMenuItem_Click);
            // 
            // newCategoryToolStripMenuItem1
            // 
            this.newCategoryToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newCategory;
            this.newCategoryToolStripMenuItem1.Name = "newCategoryToolStripMenuItem1";
            this.newCategoryToolStripMenuItem1.Size = new System.Drawing.Size(294, 30);
            this.newCategoryToolStripMenuItem1.Text = "New &Category";
            this.newCategoryToolStripMenuItem1.Click += new System.EventHandler(this.newCategoryToolStripMenuItem_Click);
            // 
            // newThemeToolStripMenuItem1
            // 
            this.newThemeToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.newTheme;
            this.newThemeToolStripMenuItem1.Name = "newThemeToolStripMenuItem1";
            this.newThemeToolStripMenuItem1.Size = new System.Drawing.Size(294, 30);
            this.newThemeToolStripMenuItem1.Text = "New &Theme";
            this.newThemeToolStripMenuItem1.Click += new System.EventHandler(this.newThemeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(291, 6);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.open;
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(294, 30);
            this.openToolStripMenuItem1.Text = "&Open Theme List";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(291, 6);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.save;
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(294, 30);
            this.saveToolStripMenuItem1.Text = "&Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // saveAsToolStripMenuItem1
            // 
            this.saveAsToolStripMenuItem1.Name = "saveAsToolStripMenuItem1";
            this.saveAsToolStripMenuItem1.Size = new System.Drawing.Size(294, 30);
            this.saveAsToolStripMenuItem1.Text = "Save &As...";
            this.saveAsToolStripMenuItem1.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // printToolStripMenuItem1
            // 
            this.printToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.print;
            this.printToolStripMenuItem1.Name = "printToolStripMenuItem1";
            this.printToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printToolStripMenuItem1.Size = new System.Drawing.Size(294, 30);
            this.printToolStripMenuItem1.Text = "&Print";
            this.printToolStripMenuItem1.Click += new System.EventHandler(this.printToolStripButton_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem1,
            this.copyToolStripMenuItem1,
            this.pasteToolStripMenuItem1,
            this.deleteToolStripMenuItem1,
            this.toolStripMenuItem3,
            this.searchToolStripMenuItem1});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // cutToolStripMenuItem1
            // 
            this.cutToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.cut;
            this.cutToolStripMenuItem1.Name = "cutToolStripMenuItem1";
            this.cutToolStripMenuItem1.ShortcutKeyDisplayString = "Ctrl+X";
            this.cutToolStripMenuItem1.Size = new System.Drawing.Size(200, 30);
            this.cutToolStripMenuItem1.Text = "Cu&t";
            this.cutToolStripMenuItem1.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.copy;
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.ShortcutKeyDisplayString = "Ctrl+C";
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(200, 30);
            this.copyToolStripMenuItem1.Text = "&Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem1
            // 
            this.pasteToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.paste;
            this.pasteToolStripMenuItem1.Name = "pasteToolStripMenuItem1";
            this.pasteToolStripMenuItem1.ShortcutKeyDisplayString = "Ctrl+V";
            this.pasteToolStripMenuItem1.Size = new System.Drawing.Size(200, 30);
            this.pasteToolStripMenuItem1.Text = "&Paste";
            this.pasteToolStripMenuItem1.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.delete;
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.ShortcutKeyDisplayString = "Del";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(200, 30);
            this.deleteToolStripMenuItem1.Text = "&Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(197, 6);
            // 
            // searchToolStripMenuItem1
            // 
            this.searchToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.search;
            this.searchToolStripMenuItem1.Name = "searchToolStripMenuItem1";
            this.searchToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchToolStripMenuItem1.Size = new System.Drawing.Size(200, 30);
            this.searchToolStripMenuItem1.Text = "&Find";
            this.searchToolStripMenuItem1.Click += new System.EventHandler(this.fastSearchToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToFavoritesToolStripMenuItem1,
            this.sortToolStripMenuItem1,
            this.launchToolStripMenuItem1,
            this.preferencesToolStripMenuItem1,
            this.administrativeToolsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // addToFavoritesToolStripMenuItem1
            // 
            this.addToFavoritesToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.Favorites;
            this.addToFavoritesToolStripMenuItem1.Name = "addToFavoritesToolStripMenuItem1";
            this.addToFavoritesToolStripMenuItem1.Size = new System.Drawing.Size(288, 30);
            this.addToFavoritesToolStripMenuItem1.Text = "&Add to Favorites";
            this.addToFavoritesToolStripMenuItem1.Click += new System.EventHandler(this.add2FavToolStripMenuItem_Click);
            // 
            // sortToolStripMenuItem1
            // 
            this.sortToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.SortAZ;
            this.sortToolStripMenuItem1.Name = "sortToolStripMenuItem1";
            this.sortToolStripMenuItem1.Size = new System.Drawing.Size(288, 30);
            this.sortToolStripMenuItem1.Text = "&Sort";
            this.sortToolStripMenuItem1.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
            // 
            // launchToolStripMenuItem1
            // 
            this.launchToolStripMenuItem1.Name = "launchToolStripMenuItem1";
            this.launchToolStripMenuItem1.Size = new System.Drawing.Size(288, 30);
            this.launchToolStripMenuItem1.Text = "Open &Theme(s)";
            this.launchToolStripMenuItem1.Click += new System.EventHandler(this.launchToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem1
            // 
            this.preferencesToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.tools;
            this.preferencesToolStripMenuItem1.Name = "preferencesToolStripMenuItem1";
            this.preferencesToolStripMenuItem1.Size = new System.Drawing.Size(288, 30);
            this.preferencesToolStripMenuItem1.Text = "&Options...";
            this.preferencesToolStripMenuItem1.Click += new System.EventHandler(this.preferencesToolStripButton_Click);
            // 
            // administrativeToolsToolStripMenuItem
            // 
            this.administrativeToolsToolStripMenuItem.Name = "administrativeToolsToolStripMenuItem";
            this.administrativeToolsToolStripMenuItem.Size = new System.Drawing.Size(288, 30);
            this.administrativeToolsToolStripMenuItem.Text = "Administrative &Reports...";
            this.administrativeToolsToolStripMenuItem.Click += new System.EventHandler(this.administrativeToolsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quickStartTutorialToolStripMenuItem,
            this.viewHelpToolStripMenuItem1,
            this.aboutToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // quickStartTutorialToolStripMenuItem
            // 
            this.quickStartTutorialToolStripMenuItem.Name = "quickStartTutorialToolStripMenuItem";
            this.quickStartTutorialToolStripMenuItem.Size = new System.Drawing.Size(279, 30);
            this.quickStartTutorialToolStripMenuItem.Text = "&Quick Start Tutorial";
            this.quickStartTutorialToolStripMenuItem.Click += new System.EventHandler(this.quickStartTutorialToolStripMenuItem_Click);
            // 
            // viewHelpToolStripMenuItem1
            // 
            this.viewHelpToolStripMenuItem1.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.help;
            this.viewHelpToolStripMenuItem1.Name = "viewHelpToolStripMenuItem1";
            this.viewHelpToolStripMenuItem1.Size = new System.Drawing.Size(279, 30);
            this.viewHelpToolStripMenuItem1.Text = "View &Help";
            this.viewHelpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripButton_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(279, 30);
            this.aboutToolStripMenuItem1.Text = "&About Theme Manager";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.styleSheetComboBox,
            this.ageComboBox,
            this.toolStripSeparator4,
            this.searchTextBox,
            this.searchToolStripButton});
            this.toolStrip2.Location = new System.Drawing.Point(3, 64);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(440, 33);
            this.toolStrip2.TabIndex = 2;
            // 
            // styleSheetComboBox
            // 
            this.styleSheetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.styleSheetComboBox.DropDownWidth = 180;
            this.styleSheetComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.styleSheetComboBox.Name = "styleSheetComboBox";
            this.styleSheetComboBox.Size = new System.Drawing.Size(163, 33);
            this.styleSheetComboBox.ToolTipText = "Metadata Format";
            // 
            // ageComboBox
            // 
            this.ageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ageComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ageComboBox.Name = "ageComboBox";
            this.ageComboBox.Size = new System.Drawing.Size(75, 33);
            this.ageComboBox.ToolTipText = "Highlight themes newer than this";
            this.ageComboBox.SelectedIndexChanged += new System.EventHandler(this.ageComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 33);
            // 
            // searchTextBox
            // 
            this.searchTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchTextBox.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(150, 33);
            this.searchTextBox.Text = "Search";
            this.searchTextBox.ToolTipText = "Search Text";
            this.searchTextBox.Enter += new System.EventHandler(this.searchToolStripTextBox_Enter);
            this.searchTextBox.Leave += new System.EventHandler(this.searchToolStripTextBox_Leave);
            this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchToolStripTextBox_KeyDown);
            // 
            // searchToolStripButton
            // 
            this.searchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchToolStripButton.Image = global::NPS.AKRO.ThemeManager.Properties.Resources.search;
            this.searchToolStripButton.Name = "searchToolStripButton";
            this.searchToolStripButton.Size = new System.Drawing.Size(28, 30);
            this.searchToolStripButton.Text = "Search";
            this.searchToolStripButton.ToolTipText = "Search";
            this.searchToolStripButton.Click += new System.EventHandler(this.fastSearchToolStripMenuItem_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "tml";
            this.saveFileDialog1.Filter = "Theme Manager Lists (*.tml)| *.tml|All files (*.*)|*.*";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "tml";
            this.openFileDialog1.Filter = "ThemeLists (*.tml, *.xml, *.mdb)|*.tml;*.xml;*.mdb";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 617);
            this.Controls.Add(this.toolStripContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(347, 340);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Theme Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.listsTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.infoTabControl.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TmTreeView themesTreeView;
        private TmTreeView searchTreeView;
        private TmTreeView favoritesTreeView;
        private System.Windows.Forms.TabControl listsTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabControl infoTabControl;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripButton printToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton cutToolStripButton;
        private System.Windows.Forms.ToolStripButton copyToolStripButton;
        private System.Windows.Forms.ToolStripButton pasteToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton viewHelpToolStripButton;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripButton sortToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton preferencesToolStripButton;
        private System.Windows.Forms.ToolStripTextBox searchTextBox;
        private System.Windows.Forms.ToolStripComboBox styleSheetComboBox;
        private System.Windows.Forms.ToolStripButton searchToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToFavoritesToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton newToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem newThemeListToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem newCategoryToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem newThemeToolStripButton;
        private System.Windows.Forms.ToolStripComboBox ageComboBox;
        private System.Windows.Forms.ToolStripStatusLabel statusBar;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.Button previewLabel;
        private System.Windows.Forms.Button propertiesLabel;
        private System.Windows.Forms.ToolStripButton addToFavoritesToolStripButton;
        private System.Windows.Forms.ToolStripButton deleteToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newThemeListToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newCategoryToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newThemeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToFavoritesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem launchToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem newCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newThemeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        internal System.Windows.Forms.ToolStrip toolStrip1;
        internal System.Windows.Forms.ToolStrip toolStrip2;
        internal System.Windows.Forms.MenuStrip menuStrip1;
        internal System.Windows.Forms.StatusStrip statusStrip;
        internal System.Windows.Forms.ToolStripMenuItem administrativeToolsToolStripMenuItem;
        private System.Windows.Forms.ImageList treeViewImageList;
        private System.Windows.Forms.ToolStripMenuItem quickStartTutorialToolStripMenuItem;

    }
}

