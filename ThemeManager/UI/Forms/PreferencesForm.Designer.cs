namespace NPS.AKRO.ThemeManager.UI.Forms
{
    partial class PreferencesForm
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
            this.restoreButton = new System.Windows.Forms.Button();
            this.onTopCheckBox = new System.Windows.Forms.CheckBox();
            this.AdvancedOptionsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // restoreButton
            // 
            this.restoreButton.Location = new System.Drawing.Point(124, 35);
            this.restoreButton.Name = "restoreButton";
            this.restoreButton.Size = new System.Drawing.Size(136, 23);
            this.restoreButton.TabIndex = 9;
            this.restoreButton.Text = "Restore Default Settings";
            this.restoreButton.UseVisualStyleBackColor = true;
            this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
            // 
            // onTopCheckBox
            // 
            this.onTopCheckBox.AutoSize = true;
            this.onTopCheckBox.Checked = global::NPS.AKRO.ThemeManager.Properties.Settings.Default.StayOnTop;
            this.onTopCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::NPS.AKRO.ThemeManager.Properties.Settings.Default, "StayOnTop", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.onTopCheckBox.Location = new System.Drawing.Point(12, 12);
            this.onTopCheckBox.Name = "onTopCheckBox";
            this.onTopCheckBox.Size = new System.Drawing.Size(248, 17);
            this.onTopCheckBox.TabIndex = 8;
            this.onTopCheckBox.Text = "Keep Theme Manager on top of other windows";
            this.onTopCheckBox.UseVisualStyleBackColor = true;
            this.onTopCheckBox.Click += new System.EventHandler(this.OnTopCheckBox_CheckedChanged);
            // 
            // AdvancedOptionsButton
            // 
            this.AdvancedOptionsButton.Location = new System.Drawing.Point(12, 35);
            this.AdvancedOptionsButton.Name = "AdvancedOptionsButton";
            this.AdvancedOptionsButton.Size = new System.Drawing.Size(106, 23);
            this.AdvancedOptionsButton.TabIndex = 10;
            this.AdvancedOptionsButton.Text = "Advanced Options";
            this.AdvancedOptionsButton.UseVisualStyleBackColor = true;
            this.AdvancedOptionsButton.Click += new System.EventHandler(this.AdvancedOptionsButton_Click);
            // 
            // PreferencesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 66);
            this.Controls.Add(this.AdvancedOptionsButton);
            this.Controls.Add(this.restoreButton);
            this.Controls.Add(this.onTopCheckBox);
            this.Name = "PreferencesForm";
            this.Text = "Preferences";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button restoreButton;
        private System.Windows.Forms.CheckBox onTopCheckBox;
        private System.Windows.Forms.Button AdvancedOptionsButton;
    }
}