namespace NPS.AKRO.ThemeManager.UI.Forms
{
    partial class SearchForm
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
            this.searchButton = new System.Windows.Forms.Button();
            this.themeNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.themeSummaryCheckBox = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.themeKeywordsCheckBox = new System.Windows.Forms.CheckBox();
            this.themeDescriptionCheckBox = new System.Windows.Forms.CheckBox();
            this.themeNameCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.anyThemeNamesRadioButton = new System.Windows.Forms.RadioButton();
            this.allThemeNamesRadioButton = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.parkUnitCodeTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.metadataPlaceKeywordsCheckBox = new System.Windows.Forms.CheckBox();
            this.metadataThemeKeywordsCheckBox = new System.Windows.Forms.CheckBox();
            this.metadataAbstactsCheckBox = new System.Windows.Forms.CheckBox();
            this.MetadataAllSectionsCheckBox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.anyMetadataRadioButton = new System.Windows.Forms.RadioButton();
            this.allMetadataRadioButton = new System.Windows.Forms.RadioButton();
            this.metadataTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dateAndOrText = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.pubDateAfter = new System.Windows.Forms.DateTimePicker();
            this.pubDateBefore = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.helpButton = new System.Windows.Forms.Button();
            this.searchCategoriesCheckBox = new System.Windows.Forms.CheckBox();
            this.andOrButton1 = new System.Windows.Forms.Button();
            this.andOrButton2 = new System.Windows.Forms.Button();
            this.andOrButton3 = new System.Windows.Forms.Button();
            this.searchThemesCheckBox = new System.Windows.Forms.CheckBox();
            this.backgroundSearcher = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressText = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Enabled = false;
            this.searchButton.Location = new System.Drawing.Point(253, 583);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(87, 27);
            this.searchButton.TabIndex = 1;
            this.searchButton.Text = "&Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // themeNameTextBox
            // 
            this.themeNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.themeNameTextBox.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.themeNameTextBox.Location = new System.Drawing.Point(7, 48);
            this.themeNameTextBox.Name = "themeNameTextBox";
            this.themeNameTextBox.Size = new System.Drawing.Size(310, 21);
            this.themeNameTextBox.TabIndex = 2;
            this.themeNameTextBox.TextChanged += new System.EventHandler(this.ThemeNameTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label1.Location = new System.Drawing.Point(3, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Theme contains";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.themeSummaryCheckBox);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.themeKeywordsCheckBox);
            this.panel1.Controls.Add(this.themeDescriptionCheckBox);
            this.panel1.Controls.Add(this.themeNameCheckBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.anyThemeNamesRadioButton);
            this.panel1.Controls.Add(this.allThemeNamesRadioButton);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.themeNameTextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(14, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(327, 128);
            this.panel1.TabIndex = 8;
            // 
            // themeSummaryCheckBox
            // 
            this.themeSummaryCheckBox.AutoSize = true;
            this.themeSummaryCheckBox.Checked = true;
            this.themeSummaryCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.themeSummaryCheckBox.Location = new System.Drawing.Point(100, 101);
            this.themeSummaryCheckBox.Name = "themeSummaryCheckBox";
            this.themeSummaryCheckBox.Size = new System.Drawing.Size(77, 19);
            this.themeSummaryCheckBox.TabIndex = 29;
            this.themeSummaryCheckBox.Text = "Summary";
            this.themeSummaryCheckBox.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 15);
            this.label11.TabIndex = 28;
            this.label11.Text = "Search in:";
            // 
            // themeKeywordsCheckBox
            // 
            this.themeKeywordsCheckBox.AutoSize = true;
            this.themeKeywordsCheckBox.Checked = true;
            this.themeKeywordsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.themeKeywordsCheckBox.Location = new System.Drawing.Point(183, 101);
            this.themeKeywordsCheckBox.Name = "themeKeywordsCheckBox";
            this.themeKeywordsCheckBox.Size = new System.Drawing.Size(77, 19);
            this.themeKeywordsCheckBox.TabIndex = 27;
            this.themeKeywordsCheckBox.Text = "Keywords";
            this.themeKeywordsCheckBox.UseVisualStyleBackColor = true;
            // 
            // themeDescriptionCheckBox
            // 
            this.themeDescriptionCheckBox.AutoSize = true;
            this.themeDescriptionCheckBox.Checked = true;
            this.themeDescriptionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.themeDescriptionCheckBox.Location = new System.Drawing.Point(183, 75);
            this.themeDescriptionCheckBox.Name = "themeDescriptionCheckBox";
            this.themeDescriptionCheckBox.Size = new System.Drawing.Size(86, 19);
            this.themeDescriptionCheckBox.TabIndex = 26;
            this.themeDescriptionCheckBox.Text = "Description";
            this.themeDescriptionCheckBox.UseVisualStyleBackColor = true;
            // 
            // themeNameCheckBox
            // 
            this.themeNameCheckBox.AutoSize = true;
            this.themeNameCheckBox.Checked = true;
            this.themeNameCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.themeNameCheckBox.Location = new System.Drawing.Point(100, 75);
            this.themeNameCheckBox.Name = "themeNameCheckBox";
            this.themeNameCheckBox.Size = new System.Drawing.Size(58, 19);
            this.themeNameCheckBox.TabIndex = 25;
            this.themeNameCheckBox.Text = "Name";
            this.themeNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label2.Location = new System.Drawing.Point(184, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "of the words:";
            // 
            // anyThemeNamesRadioButton
            // 
            this.anyThemeNamesRadioButton.AutoSize = true;
            this.anyThemeNamesRadioButton.Checked = true;
            this.anyThemeNamesRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.anyThemeNamesRadioButton.Location = new System.Drawing.Point(138, 22);
            this.anyThemeNamesRadioButton.Name = "anyThemeNamesRadioButton";
            this.anyThemeNamesRadioButton.Size = new System.Drawing.Size(44, 17);
            this.anyThemeNamesRadioButton.TabIndex = 12;
            this.anyThemeNamesRadioButton.TabStop = true;
            this.anyThemeNamesRadioButton.Text = "Any";
            this.anyThemeNamesRadioButton.UseVisualStyleBackColor = true;
            // 
            // allThemeNamesRadioButton
            // 
            this.allThemeNamesRadioButton.AutoSize = true;
            this.allThemeNamesRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.allThemeNamesRadioButton.Location = new System.Drawing.Point(95, 22);
            this.allThemeNamesRadioButton.Name = "allThemeNamesRadioButton";
            this.allThemeNamesRadioButton.Size = new System.Drawing.Size(36, 17);
            this.allThemeNamesRadioButton.TabIndex = 11;
            this.allThemeNamesRadioButton.Text = "All";
            this.allThemeNamesRadioButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Desktop;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label3.Size = new System.Drawing.Size(325, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "  Theme:";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.parkUnitCodeTextBox);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(13, 349);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(327, 63);
            this.panel2.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Park Unit Code:";
            // 
            // parkUnitCodeTextBox
            // 
            this.parkUnitCodeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.parkUnitCodeTextBox.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.parkUnitCodeTextBox.Location = new System.Drawing.Point(103, 29);
            this.parkUnitCodeTextBox.Name = "parkUnitCodeTextBox";
            this.parkUnitCodeTextBox.Size = new System.Drawing.Size(214, 21);
            this.parkUnitCodeTextBox.TabIndex = 1;
            this.parkUnitCodeTextBox.TextChanged += new System.EventHandler(this.ParkUnitCodeTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.Desktop;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label4.Size = new System.Drawing.Size(325, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "  NPS tag options:";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.metadataPlaceKeywordsCheckBox);
            this.panel3.Controls.Add(this.metadataThemeKeywordsCheckBox);
            this.panel3.Controls.Add(this.metadataAbstactsCheckBox);
            this.panel3.Controls.Add(this.MetadataAllSectionsCheckBox);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.anyMetadataRadioButton);
            this.panel3.Controls.Add(this.allMetadataRadioButton);
            this.panel3.Controls.Add(this.metadataTextBox);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Location = new System.Drawing.Point(13, 180);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(327, 134);
            this.panel3.TabIndex = 10;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 15);
            this.label10.TabIndex = 23;
            this.label10.Text = "Search in:";
            // 
            // metadataPlaceKeywordsCheckBox
            // 
            this.metadataPlaceKeywordsCheckBox.AutoSize = true;
            this.metadataPlaceKeywordsCheckBox.Location = new System.Drawing.Point(180, 106);
            this.metadataPlaceKeywordsCheckBox.Name = "metadataPlaceKeywordsCheckBox";
            this.metadataPlaceKeywordsCheckBox.Size = new System.Drawing.Size(108, 19);
            this.metadataPlaceKeywordsCheckBox.TabIndex = 22;
            this.metadataPlaceKeywordsCheckBox.Text = "Place Keywords";
            this.metadataPlaceKeywordsCheckBox.UseVisualStyleBackColor = true;
            this.metadataPlaceKeywordsCheckBox.CheckedChanged += new System.EventHandler(this.PlaceKeywordsCheckBox_CheckedChanged);
            // 
            // metadataThemeKeywordsCheckBox
            // 
            this.metadataThemeKeywordsCheckBox.AutoSize = true;
            this.metadataThemeKeywordsCheckBox.Location = new System.Drawing.Point(180, 80);
            this.metadataThemeKeywordsCheckBox.Name = "metadataThemeKeywordsCheckBox";
            this.metadataThemeKeywordsCheckBox.Size = new System.Drawing.Size(117, 19);
            this.metadataThemeKeywordsCheckBox.TabIndex = 21;
            this.metadataThemeKeywordsCheckBox.Text = "Theme Keywords";
            this.metadataThemeKeywordsCheckBox.UseVisualStyleBackColor = true;
            this.metadataThemeKeywordsCheckBox.CheckedChanged += new System.EventHandler(this.ThemeKeywordsCheckBox_CheckedChanged);
            // 
            // metadataAbstactsCheckBox
            // 
            this.metadataAbstactsCheckBox.AutoSize = true;
            this.metadataAbstactsCheckBox.Checked = true;
            this.metadataAbstactsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.metadataAbstactsCheckBox.Location = new System.Drawing.Point(75, 106);
            this.metadataAbstactsCheckBox.Name = "metadataAbstactsCheckBox";
            this.metadataAbstactsCheckBox.Size = new System.Drawing.Size(75, 19);
            this.metadataAbstactsCheckBox.TabIndex = 20;
            this.metadataAbstactsCheckBox.Text = "Abstracts";
            this.metadataAbstactsCheckBox.UseVisualStyleBackColor = true;
            this.metadataAbstactsCheckBox.CheckedChanged += new System.EventHandler(this.AbstactsCheckBox_CheckedChanged);
            // 
            // MetadataAllSectionsCheckBox
            // 
            this.MetadataAllSectionsCheckBox.AutoSize = true;
            this.MetadataAllSectionsCheckBox.Location = new System.Drawing.Point(75, 80);
            this.MetadataAllSectionsCheckBox.Name = "MetadataAllSectionsCheckBox";
            this.MetadataAllSectionsCheckBox.Size = new System.Drawing.Size(87, 19);
            this.MetadataAllSectionsCheckBox.TabIndex = 19;
            this.MetadataAllSectionsCheckBox.Text = "All Sections";
            this.MetadataAllSectionsCheckBox.UseVisualStyleBackColor = true;
            this.MetadataAllSectionsCheckBox.CheckedChanged += new System.EventHandler(this.AllSectionsCheckBox_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(210, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 15);
            this.label8.TabIndex = 18;
            this.label8.Text = "of the words:";
            // 
            // anyMetadataRadioButton
            // 
            this.anyMetadataRadioButton.AutoSize = true;
            this.anyMetadataRadioButton.Checked = true;
            this.anyMetadataRadioButton.Location = new System.Drawing.Point(159, 24);
            this.anyMetadataRadioButton.Name = "anyMetadataRadioButton";
            this.anyMetadataRadioButton.Size = new System.Drawing.Size(46, 19);
            this.anyMetadataRadioButton.TabIndex = 17;
            this.anyMetadataRadioButton.TabStop = true;
            this.anyMetadataRadioButton.Text = "Any";
            this.anyMetadataRadioButton.UseVisualStyleBackColor = true;
            // 
            // allMetadataRadioButton
            // 
            this.allMetadataRadioButton.AutoSize = true;
            this.allMetadataRadioButton.Location = new System.Drawing.Point(115, 24);
            this.allMetadataRadioButton.Name = "allMetadataRadioButton";
            this.allMetadataRadioButton.Size = new System.Drawing.Size(39, 19);
            this.allMetadataRadioButton.TabIndex = 16;
            this.allMetadataRadioButton.Text = "All";
            this.allMetadataRadioButton.UseVisualStyleBackColor = true;
            // 
            // metadataTextBox
            // 
            this.metadataTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.metadataTextBox.Location = new System.Drawing.Point(7, 51);
            this.metadataTextBox.Name = "metadataTextBox";
            this.metadataTextBox.Size = new System.Drawing.Size(310, 23);
            this.metadataTextBox.TabIndex = 14;
            this.metadataTextBox.TextChanged += new System.EventHandler(this.MetadataTextBox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(105, 15);
            this.label9.TabIndex = 15;
            this.label9.Text = "Metadata contains";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Desktop;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label5.Size = new System.Drawing.Size(325, 18);
            this.label5.TabIndex = 0;
            this.label5.Text = "  Metadata options:";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.dateAndOrText);
            this.panel4.Controls.Add(this.label16);
            this.panel4.Controls.Add(this.label15);
            this.panel4.Controls.Add(this.pubDateAfter);
            this.panel4.Controls.Add(this.pubDateBefore);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Location = new System.Drawing.Point(12, 447);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(327, 72);
            this.panel4.TabIndex = 10;
            // 
            // dateAndOrText
            // 
            this.dateAndOrText.AutoSize = true;
            this.dateAndOrText.Location = new System.Drawing.Point(143, 47);
            this.dateAndOrText.Name = "dateAndOrText";
            this.dateAndOrText.Size = new System.Drawing.Size(29, 15);
            this.dateAndOrText.TabIndex = 6;
            this.dateAndOrText.Text = "And";
            this.dateAndOrText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dateAndOrText.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 24);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 15);
            this.label16.TabIndex = 5;
            this.label16.Text = "After";
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(180, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 15);
            this.label15.TabIndex = 4;
            this.label15.Text = "Before";
            // 
            // pubDateAfter
            // 
            this.pubDateAfter.Checked = false;
            this.pubDateAfter.CustomFormat = "";
            this.pubDateAfter.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.pubDateAfter.Location = new System.Drawing.Point(5, 42);
            this.pubDateAfter.Name = "pubDateAfter";
            this.pubDateAfter.ShowCheckBox = true;
            this.pubDateAfter.Size = new System.Drawing.Size(138, 23);
            this.pubDateAfter.TabIndex = 2;
            this.pubDateAfter.ValueChanged += new System.EventHandler(this.PubDate_AfterValueChanged);
            // 
            // pubDateBefore
            // 
            this.pubDateBefore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pubDateBefore.Checked = false;
            this.pubDateBefore.CustomFormat = "";
            this.pubDateBefore.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.pubDateBefore.Location = new System.Drawing.Point(182, 42);
            this.pubDateBefore.Name = "pubDateBefore";
            this.pubDateBefore.ShowCheckBox = true;
            this.pubDateBefore.Size = new System.Drawing.Size(138, 23);
            this.pubDateBefore.TabIndex = 1;
            this.pubDateBefore.ValueChanged += new System.EventHandler(this.PubDate_BeforeValueChanged);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Desktop;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label6.Size = new System.Drawing.Size(325, 18);
            this.label6.TabIndex = 0;
            this.label6.Text = "  Publication Date:";
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(159, 583);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(87, 27);
            this.closeButton.TabIndex = 11;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.helpButton.Location = new System.Drawing.Point(12, 583);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(87, 27);
            this.helpButton.TabIndex = 12;
            this.helpButton.Text = "&Help";
            this.helpButton.UseVisualStyleBackColor = true;
            // 
            // searchCategoriesCheckBox
            // 
            this.searchCategoriesCheckBox.AutoSize = true;
            this.searchCategoriesCheckBox.Location = new System.Drawing.Point(129, 525);
            this.searchCategoriesCheckBox.Name = "searchCategoriesCheckBox";
            this.searchCategoriesCheckBox.Size = new System.Drawing.Size(120, 19);
            this.searchCategoriesCheckBox.TabIndex = 13;
            this.searchCategoriesCheckBox.Text = "Search Categories";
            this.searchCategoriesCheckBox.UseVisualStyleBackColor = true;
            this.searchCategoriesCheckBox.CheckedChanged += new System.EventHandler(this.SearchCategories_CheckedChanged);
            // 
            // andOrButton1
            // 
            this.andOrButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.andOrButton1.Location = new System.Drawing.Point(146, 150);
            this.andOrButton1.Name = "andOrButton1";
            this.andOrButton1.Size = new System.Drawing.Size(49, 23);
            this.andOrButton1.TabIndex = 14;
            this.andOrButton1.Text = "And";
            this.andOrButton1.UseVisualStyleBackColor = true;
            this.andOrButton1.Click += new System.EventHandler(this.AndOrButton_Click);
            // 
            // andOrButton2
            // 
            this.andOrButton2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.andOrButton2.Location = new System.Drawing.Point(146, 320);
            this.andOrButton2.Name = "andOrButton2";
            this.andOrButton2.Size = new System.Drawing.Size(49, 23);
            this.andOrButton2.TabIndex = 15;
            this.andOrButton2.Text = "And";
            this.andOrButton2.UseVisualStyleBackColor = true;
            this.andOrButton2.Click += new System.EventHandler(this.AndOrButton_Click);
            // 
            // andOrButton3
            // 
            this.andOrButton3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.andOrButton3.Location = new System.Drawing.Point(146, 418);
            this.andOrButton3.Name = "andOrButton3";
            this.andOrButton3.Size = new System.Drawing.Size(50, 25);
            this.andOrButton3.TabIndex = 16;
            this.andOrButton3.Text = "And";
            this.andOrButton3.UseVisualStyleBackColor = true;
            this.andOrButton3.Click += new System.EventHandler(this.AndOrButton_Click);
            // 
            // searchThemesCheckBox
            // 
            this.searchThemesCheckBox.AutoSize = true;
            this.searchThemesCheckBox.Checked = true;
            this.searchThemesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.searchThemesCheckBox.Location = new System.Drawing.Point(12, 525);
            this.searchThemesCheckBox.Name = "searchThemesCheckBox";
            this.searchThemesCheckBox.Size = new System.Drawing.Size(103, 19);
            this.searchThemesCheckBox.TabIndex = 17;
            this.searchThemesCheckBox.Text = "SearchThemes";
            this.searchThemesCheckBox.UseVisualStyleBackColor = true;
            this.searchThemesCheckBox.CheckedChanged += new System.EventHandler(this.SearchThemes_CheckedChanged);
            // 
            // backgroundSearcher
            // 
            this.backgroundSearcher.WorkerReportsProgress = true;
            this.backgroundSearcher.WorkerSupportsCancellation = true;
            this.backgroundSearcher.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundSearcher_DoWork);
            this.backgroundSearcher.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundSearcher_ProgressChanged);
            this.backgroundSearcher.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundSearcher_RunWorkerCompleted);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 551);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(183, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 18;
            // 
            // progressText
            // 
            this.progressText.AutoSize = true;
            this.progressText.Location = new System.Drawing.Point(197, 555);
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(100, 15);
            this.progressText.TabIndex = 19;
            this.progressText.Text = "No themes found";
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 621);
            this.Controls.Add(this.progressText);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.searchThemesCheckBox);
            this.Controls.Add(this.andOrButton3);
            this.Controls.Add(this.andOrButton2);
            this.Controls.Add(this.andOrButton1);
            this.Controls.Add(this.searchCategoriesCheckBox);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.searchButton);
            this.DataBindings.Add(new System.Windows.Forms.Binding("TopMost", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "StayOnTop", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(360, 580);
            this.Name = "SearchForm";
            this.Text = "Search Themes";
            this.TopMost = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.StayOnTop;
            this.Load += new System.EventHandler(this.SearchForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton anyThemeNamesRadioButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton allThemeNamesRadioButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton anyMetadataRadioButton;
        private System.Windows.Forms.RadioButton allMetadataRadioButton;
        private System.Windows.Forms.TextBox metadataTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox metadataPlaceKeywordsCheckBox;
        private System.Windows.Forms.CheckBox metadataThemeKeywordsCheckBox;
        private System.Windows.Forms.CheckBox metadataAbstactsCheckBox;
        private System.Windows.Forms.CheckBox MetadataAllSectionsCheckBox;
        private System.Windows.Forms.TextBox parkUnitCodeTextBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DateTimePicker pubDateAfter;
        private System.Windows.Forms.DateTimePicker pubDateBefore;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox searchCategoriesCheckBox;
        private System.Windows.Forms.Button andOrButton1;
        private System.Windows.Forms.Button andOrButton2;
        private System.Windows.Forms.Button andOrButton3;
        private System.Windows.Forms.Label dateAndOrText;
        private System.Windows.Forms.CheckBox searchThemesCheckBox;
        private System.ComponentModel.BackgroundWorker backgroundSearcher;
        internal System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressText;
        internal System.Windows.Forms.TextBox themeNameTextBox;
        private System.Windows.Forms.CheckBox themeSummaryCheckBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox themeKeywordsCheckBox;
        private System.Windows.Forms.CheckBox themeDescriptionCheckBox;
        private System.Windows.Forms.CheckBox themeNameCheckBox;

    }
}