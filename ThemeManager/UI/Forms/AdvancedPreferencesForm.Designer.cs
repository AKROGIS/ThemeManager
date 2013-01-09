namespace NPS.AKRO.ThemeManager.UI.Forms
{
    partial class AdvancedPreferencesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedPreferencesForm));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.searchCheckBox = new System.Windows.Forms.CheckBox();
            this.licenseCheckBox = new System.Windows.Forms.CheckBox();
            this.startupCheckBox = new System.Windows.Forms.CheckBox();
            this.onTopCheckBox = new System.Windows.Forms.CheckBox();
            this.memoryCheckBox = new System.Windows.Forms.CheckBox();
            this.editCheckBox = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.themeDescriptionToolTipCheckBox = new System.Windows.Forms.CheckBox();
            this.rightClickCheckBox = new System.Windows.Forms.CheckBox();
            this.firstNodeCheckBox = new System.Windows.Forms.CheckBox();
            this.clearNodeCheckBox = new System.Windows.Forms.CheckBox();
            this.changeFocusCheckBox = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.showAdministrativeToolsCheckBox = new System.Windows.Forms.CheckBox();
            this.statusBarCheckBox = new System.Windows.Forms.CheckBox();
            this.secondToolbarCheckBox = new System.Windows.Forms.CheckBox();
            this.mainToolbarCheckBox = new System.Windows.Forms.CheckBox();
            this.mainMenuCheckBox = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.allowUnsortedOrderCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.sortOrderComboBox = new System.Windows.Forms.ComboBox();
            this.removeObsoleteheckBox = new System.Windows.Forms.CheckBox();
            this.addRequiredCheckBox = new System.Windows.Forms.CheckBox();
            this.restoreButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchCheckBox
            // 
            this.searchCheckBox.AutoSize = true;
            this.searchCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.KeepSearchFormOpen;
            this.searchCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "KeepSearchFormOpen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.searchCheckBox.Location = new System.Drawing.Point(8, 106);
            this.searchCheckBox.Name = "searchCheckBox";
            this.searchCheckBox.Size = new System.Drawing.Size(287, 19);
            this.searchCheckBox.TabIndex = 1;
            this.searchCheckBox.Text = "Keep search form open after results are displayed ";
            this.toolTip1.SetToolTip(this.searchCheckBox, "Uncheck this option is you want the search panel to disappear after a successful " +
                    "search.\r\nCheck this option if you like to do several searches in a row.");
            this.searchCheckBox.UseVisualStyleBackColor = true;
            // 
            // licenseCheckBox
            // 
            this.licenseCheckBox.AutoSize = true;
            this.licenseCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.CheckForArcViewBeforeArcInfo;
            this.licenseCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "CheckForArcViewBeforeArcInfo", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.licenseCheckBox.Location = new System.Drawing.Point(8, 81);
            this.licenseCheckBox.Name = "licenseCheckBox";
            this.licenseCheckBox.Size = new System.Drawing.Size(244, 19);
            this.licenseCheckBox.TabIndex = 6;
            this.licenseCheckBox.Text = "Check for ArcView before ArcInfo License";
            this.toolTip1.SetToolTip(this.licenseCheckBox, "Unchecking this option can speed up license check out\r\nif you have an ArcInfo lic" +
                    "ense and you don\'t mind using it.");
            this.licenseCheckBox.UseVisualStyleBackColor = true;
            // 
            // startupCheckBox
            // 
            this.startupCheckBox.AutoSize = true;
            this.startupCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.DisplayDefaultHtml;
            this.startupCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.startupCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "DisplayDefaultHtml", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.startupCheckBox.Location = new System.Drawing.Point(8, 31);
            this.startupCheckBox.Name = "startupCheckBox";
            this.startupCheckBox.Size = new System.Drawing.Size(276, 19);
            this.startupCheckBox.TabIndex = 5;
            this.startupCheckBox.Text = "Display introduction when no theme is selected";
            this.toolTip1.SetToolTip(this.startupCheckBox, resources.GetString("startupCheckBox.ToolTip"));
            this.startupCheckBox.UseVisualStyleBackColor = true;
            // 
            // onTopCheckBox
            // 
            this.onTopCheckBox.AutoSize = true;
            this.onTopCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.StayOnTop;
            this.onTopCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "StayOnTop", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.onTopCheckBox.Location = new System.Drawing.Point(8, 6);
            this.onTopCheckBox.Name = "onTopCheckBox";
            this.onTopCheckBox.Size = new System.Drawing.Size(275, 19);
            this.onTopCheckBox.TabIndex = 4;
            this.onTopCheckBox.Text = "Keep Theme Manager on top of other windows";
            this.toolTip1.SetToolTip(this.onTopCheckBox, "Tells windows to keep the main theme manager window on top of most other windows." +
                    "");
            this.onTopCheckBox.UseVisualStyleBackColor = true;
            this.onTopCheckBox.CheckedChanged += new System.EventHandler(this.OnTopCheckBox_CheckedChanged);
            // 
            // memoryCheckBox
            // 
            this.memoryCheckBox.AutoSize = true;
            this.memoryCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.KeepMetaDataInMemory;
            this.memoryCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "KeepMetaDataInMemory", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.memoryCheckBox.Location = new System.Drawing.Point(8, 56);
            this.memoryCheckBox.Name = "memoryCheckBox";
            this.memoryCheckBox.Size = new System.Drawing.Size(166, 19);
            this.memoryCheckBox.TabIndex = 3;
            this.memoryCheckBox.Text = "Keep metadata in memory";
            this.toolTip1.SetToolTip(this.memoryCheckBox, "Uncheck this option if you are running out of memory.\r\nChecking this option will " +
                    "improve the performance of searching metadata.");
            this.memoryCheckBox.UseVisualStyleBackColor = true;
            this.memoryCheckBox.CheckedChanged += new System.EventHandler(this.MemoryCheckBox_CheckedChanged);
            // 
            // editCheckBox
            // 
            this.editCheckBox.AutoSize = true;
            this.editCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.InPlaceNodeEditing;
            this.editCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "InPlaceNodeEditing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.editCheckBox.Location = new System.Drawing.Point(8, 6);
            this.editCheckBox.Name = "editCheckBox";
            this.editCheckBox.Size = new System.Drawing.Size(285, 19);
            this.editCheckBox.TabIndex = 2;
            this.editCheckBox.Text = "Enable theme/category name editing in tree view";
            this.toolTip1.SetToolTip(this.editCheckBox, "Uncheck this option if you prefer editing names in the properties panel.");
            this.editCheckBox.UseVisualStyleBackColor = true;
            this.editCheckBox.CheckedChanged += new System.EventHandler(this.EditCheckBox_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(334, 203);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.searchCheckBox);
            this.tabPage1.Controls.Add(this.licenseCheckBox);
            this.tabPage1.Controls.Add(this.startupCheckBox);
            this.tabPage1.Controls.Add(this.onTopCheckBox);
            this.tabPage1.Controls.Add(this.memoryCheckBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(326, 175);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.themeDescriptionToolTipCheckBox);
            this.tabPage2.Controls.Add(this.rightClickCheckBox);
            this.tabPage2.Controls.Add(this.firstNodeCheckBox);
            this.tabPage2.Controls.Add(this.clearNodeCheckBox);
            this.tabPage2.Controls.Add(this.changeFocusCheckBox);
            this.tabPage2.Controls.Add(this.editCheckBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(326, 175);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tree View";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // themeDescriptionToolTipCheckBox
            // 
            this.themeDescriptionToolTipCheckBox.AutoSize = true;
            this.themeDescriptionToolTipCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.ShowThemeDescriptionToolTip;
            this.themeDescriptionToolTipCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "ShowThemeDescriptionToolTip", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.themeDescriptionToolTipCheckBox.Location = new System.Drawing.Point(8, 136);
            this.themeDescriptionToolTipCheckBox.Name = "themeDescriptionToolTipCheckBox";
            this.themeDescriptionToolTipCheckBox.Size = new System.Drawing.Size(261, 19);
            this.themeDescriptionToolTipCheckBox.TabIndex = 7;
            this.themeDescriptionToolTipCheckBox.Text = "Show theme description as a pop-up tool tip";
            this.themeDescriptionToolTipCheckBox.UseVisualStyleBackColor = true;
            this.themeDescriptionToolTipCheckBox.Click += new System.EventHandler(this.themeDescriptionToolTipCheckBox_Click);
            // 
            // rightClickCheckBox
            // 
            this.rightClickCheckBox.AutoSize = true;
            this.rightClickCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.RightClickSelectsNode;
            this.rightClickCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rightClickCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "RightClickSelectsNode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rightClickCheckBox.Location = new System.Drawing.Point(9, 110);
            this.rightClickCheckBox.Name = "rightClickCheckBox";
            this.rightClickCheckBox.Size = new System.Drawing.Size(149, 19);
            this.rightClickCheckBox.TabIndex = 6;
            this.rightClickCheckBox.Text = "Right click selects node";
            this.rightClickCheckBox.UseVisualStyleBackColor = true;
            // 
            // firstNodeCheckBox
            // 
            this.firstNodeCheckBox.AutoSize = true;
            this.firstNodeCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.DontSelectFirstNode;
            this.firstNodeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.firstNodeCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "DontSelectFirstNode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.firstNodeCheckBox.Location = new System.Drawing.Point(9, 84);
            this.firstNodeCheckBox.Name = "firstNodeCheckBox";
            this.firstNodeCheckBox.Size = new System.Drawing.Size(275, 19);
            this.firstNodeCheckBox.TabIndex = 5;
            this.firstNodeCheckBox.Text = "Don\'t select the first node if no node is selected";
            this.firstNodeCheckBox.UseVisualStyleBackColor = true;
            // 
            // clearNodeCheckBox
            // 
            this.clearNodeCheckBox.AutoSize = true;
            this.clearNodeCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.ClickToClearNodeSelection;
            this.clearNodeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clearNodeCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "ClickToClearNodeSelection", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.clearNodeCheckBox.Location = new System.Drawing.Point(9, 58);
            this.clearNodeCheckBox.Name = "clearNodeCheckBox";
            this.clearNodeCheckBox.Size = new System.Drawing.Size(198, 19);
            this.clearNodeCheckBox.TabIndex = 4;
            this.clearNodeCheckBox.Text = "Click in tree view clears selection";
            this.clearNodeCheckBox.UseVisualStyleBackColor = true;
            // 
            // changeFocusCheckBox
            // 
            this.changeFocusCheckBox.AutoSize = true;
            this.changeFocusCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.FocusTreeviewOnTabChange;
            this.changeFocusCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.changeFocusCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "FocusTreeviewOnTabChange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.changeFocusCheckBox.Location = new System.Drawing.Point(9, 32);
            this.changeFocusCheckBox.Name = "changeFocusCheckBox";
            this.changeFocusCheckBox.Size = new System.Drawing.Size(189, 19);
            this.changeFocusCheckBox.TabIndex = 3;
            this.changeFocusCheckBox.Text = "Tab selection focuses tree view";
            this.changeFocusCheckBox.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.showAdministrativeToolsCheckBox);
            this.tabPage4.Controls.Add(this.statusBarCheckBox);
            this.tabPage4.Controls.Add(this.secondToolbarCheckBox);
            this.tabPage4.Controls.Add(this.mainToolbarCheckBox);
            this.tabPage4.Controls.Add(this.mainMenuCheckBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(326, 175);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Menus";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // showAdministrativeToolsCheckBox
            // 
            this.showAdministrativeToolsCheckBox.AutoSize = true;
            this.showAdministrativeToolsCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.ShowAdministrativeTools;
            this.showAdministrativeToolsCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "ShowAdministrativeTools", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.showAdministrativeToolsCheckBox.Location = new System.Drawing.Point(7, 110);
            this.showAdministrativeToolsCheckBox.Name = "showAdministrativeToolsCheckBox";
            this.showAdministrativeToolsCheckBox.Size = new System.Drawing.Size(210, 17);
            this.showAdministrativeToolsCheckBox.TabIndex = 4;
            this.showAdministrativeToolsCheckBox.Text = "Show administrative tools in main menu";
            this.showAdministrativeToolsCheckBox.UseVisualStyleBackColor = true;
            this.showAdministrativeToolsCheckBox.CheckedChanged += new System.EventHandler(this.showAdministrativeToolsCheckBox_CheckedChanged);
            // 
            // statusBarCheckBox
            // 
            this.statusBarCheckBox.AutoSize = true;
            this.statusBarCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.ShowStatusBar;
            this.statusBarCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "ShowStatusBar", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.statusBarCheckBox.Location = new System.Drawing.Point(7, 85);
            this.statusBarCheckBox.Name = "statusBarCheckBox";
            this.statusBarCheckBox.Size = new System.Drawing.Size(102, 17);
            this.statusBarCheckBox.TabIndex = 3;
            this.statusBarCheckBox.Text = "Show status bar";
            this.statusBarCheckBox.UseVisualStyleBackColor = true;
            this.statusBarCheckBox.CheckedChanged += new System.EventHandler(this.statusBarCheckBox_CheckedChanged);
            // 
            // secondToolbarCheckBox
            // 
            this.secondToolbarCheckBox.AutoSize = true;
            this.secondToolbarCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.ShowToolbar2;
            this.secondToolbarCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.secondToolbarCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "ShowToolbar2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.secondToolbarCheckBox.Location = new System.Drawing.Point(7, 59);
            this.secondToolbarCheckBox.Name = "secondToolbarCheckBox";
            this.secondToolbarCheckBox.Size = new System.Drawing.Size(179, 17);
            this.secondToolbarCheckBox.TabIndex = 2;
            this.secondToolbarCheckBox.Text = "Show picklist and search toolbar";
            this.secondToolbarCheckBox.UseVisualStyleBackColor = true;
            this.secondToolbarCheckBox.CheckedChanged += new System.EventHandler(this.secondToolbarCheckBox_CheckedChanged);
            // 
            // mainToolbarCheckBox
            // 
            this.mainToolbarCheckBox.AutoSize = true;
            this.mainToolbarCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.ShowToolbar1;
            this.mainToolbarCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mainToolbarCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "ShowToolbar1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.mainToolbarCheckBox.Location = new System.Drawing.Point(7, 33);
            this.mainToolbarCheckBox.Name = "mainToolbarCheckBox";
            this.mainToolbarCheckBox.Size = new System.Drawing.Size(113, 17);
            this.mainToolbarCheckBox.TabIndex = 1;
            this.mainToolbarCheckBox.Text = "Show main toolbar";
            this.mainToolbarCheckBox.UseVisualStyleBackColor = true;
            this.mainToolbarCheckBox.CheckedChanged += new System.EventHandler(this.mainToolbarCheckBox_CheckedChanged);
            // 
            // mainMenuCheckBox
            // 
            this.mainMenuCheckBox.AutoSize = true;
            this.mainMenuCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.ShowMainMenu;
            this.mainMenuCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mainMenuCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "ShowMainMenu", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.mainMenuCheckBox.Location = new System.Drawing.Point(7, 7);
            this.mainMenuCheckBox.Name = "mainMenuCheckBox";
            this.mainMenuCheckBox.Size = new System.Drawing.Size(107, 17);
            this.mainMenuCheckBox.TabIndex = 0;
            this.mainMenuCheckBox.Text = "Show main menu";
            this.mainMenuCheckBox.UseVisualStyleBackColor = true;
            this.mainMenuCheckBox.Click += new System.EventHandler(this.mainMenuCheckBox_CheckedChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.allowUnsortedOrderCheckBox);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.sortOrderComboBox);
            this.tabPage3.Controls.Add(this.removeObsoleteheckBox);
            this.tabPage3.Controls.Add(this.addRequiredCheckBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(326, 175);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Other";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // allowUnsortedOrderCheckBox
            // 
            this.allowUnsortedOrderCheckBox.AutoSize = true;
            this.allowUnsortedOrderCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.AllowUnsortedOrder;
            this.allowUnsortedOrderCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "AllowUnsortedOrder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.allowUnsortedOrderCheckBox.Location = new System.Drawing.Point(9, 57);
            this.allowUnsortedOrderCheckBox.Name = "allowUnsortedOrderCheckBox";
            this.allowUnsortedOrderCheckBox.Size = new System.Drawing.Size(126, 17);
            this.allowUnsortedOrderCheckBox.TabIndex = 4;
            this.allowUnsortedOrderCheckBox.Text = "Allow Unsorted Order";
            this.allowUnsortedOrderCheckBox.UseVisualStyleBackColor = true;
            this.allowUnsortedOrderCheckBox.CheckedChanged += new System.EventHandler(this.allowUnsortedOrderCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Default Sort Order: ";
            // 
            // sortOrderComboBox
            // 
            this.sortOrderComboBox.FormattingEnabled = true;
            this.sortOrderComboBox.Items.AddRange(new object[] {
            "Descending (Z to A)",
            "",
            "Ascending (A to Z)"});
            this.sortOrderComboBox.Location = new System.Drawing.Point(123, 82);
            this.sortOrderComboBox.Name = "sortOrderComboBox";
            this.sortOrderComboBox.Size = new System.Drawing.Size(195, 23);
            this.sortOrderComboBox.TabIndex = 2;
            this.sortOrderComboBox.SelectedIndexChanged += new System.EventHandler(this.sortOrderComboBox_SelectedIndexChanged);
            // 
            // removeObsoleteheckBox
            // 
            this.removeObsoleteheckBox.AutoSize = true;
            this.removeObsoleteheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.RemoveObsoleteThemeLists;
            this.removeObsoleteheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.removeObsoleteheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "RemoveObsoleteThemeLists", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.removeObsoleteheckBox.Location = new System.Drawing.Point(9, 32);
            this.removeObsoleteheckBox.Name = "removeObsoleteheckBox";
            this.removeObsoleteheckBox.Size = new System.Drawing.Size(161, 17);
            this.removeObsoleteheckBox.TabIndex = 1;
            this.removeObsoleteheckBox.Text = "Remove obsolete theme lists";
            this.removeObsoleteheckBox.UseVisualStyleBackColor = true;
            // 
            // addRequiredCheckBox
            // 
            this.addRequiredCheckBox.AutoSize = true;
            this.addRequiredCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.LoadRequiredThemeLists;
            this.addRequiredCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.addRequiredCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "LoadRequiredThemeLists", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.addRequiredCheckBox.Location = new System.Drawing.Point(9, 7);
            this.addRequiredCheckBox.Name = "addRequiredCheckBox";
            this.addRequiredCheckBox.Size = new System.Drawing.Size(143, 17);
            this.addRequiredCheckBox.TabIndex = 0;
            this.addRequiredCheckBox.Text = "Load required theme lists";
            this.addRequiredCheckBox.UseVisualStyleBackColor = true;
            // 
            // restoreButton
            // 
            this.restoreButton.Location = new System.Drawing.Point(170, 209);
            this.restoreButton.Name = "restoreButton";
            this.restoreButton.Size = new System.Drawing.Size(152, 23);
            this.restoreButton.TabIndex = 7;
            this.restoreButton.Text = "Restore Default Settings";
            this.restoreButton.UseVisualStyleBackColor = true;
            this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
            // 
            // AdvancedPreferencesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 244);
            this.Controls.Add(this.restoreButton);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AdvancedPreferencesForm";
            this.Text = "Advanced Options";
            this.Load += new System.EventHandler(this.PreferencesForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox searchCheckBox;
        private System.Windows.Forms.CheckBox editCheckBox;
        private System.Windows.Forms.CheckBox memoryCheckBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox onTopCheckBox;
        private System.Windows.Forms.CheckBox startupCheckBox;
        private System.Windows.Forms.CheckBox licenseCheckBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox rightClickCheckBox;
        private System.Windows.Forms.CheckBox firstNodeCheckBox;
        private System.Windows.Forms.CheckBox clearNodeCheckBox;
        private System.Windows.Forms.CheckBox changeFocusCheckBox;
        private System.Windows.Forms.Button restoreButton;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox removeObsoleteheckBox;
        private System.Windows.Forms.CheckBox addRequiredCheckBox;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.CheckBox secondToolbarCheckBox;
        private System.Windows.Forms.CheckBox mainToolbarCheckBox;
        private System.Windows.Forms.CheckBox mainMenuCheckBox;
        private System.Windows.Forms.CheckBox statusBarCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox sortOrderComboBox;
        private System.Windows.Forms.CheckBox allowUnsortedOrderCheckBox;
        private System.Windows.Forms.CheckBox showAdministrativeToolsCheckBox;
        private System.Windows.Forms.CheckBox themeDescriptionToolTipCheckBox;
    }
}