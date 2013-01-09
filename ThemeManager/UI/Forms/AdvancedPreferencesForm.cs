using System;
using System.Windows.Forms;
using NPS.AKRO.ThemeManager.Properties;

namespace NPS.AKRO.ThemeManager.UI.Forms
{
    public partial class AdvancedPreferencesForm : Form
    {
        public AdvancedPreferencesForm()
        {
            InitializeComponent();
        }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            if (!(Owner is MainForm))
            {
                MessageBox.Show(Resources.PreferencesForm_Load_Cannot_Find_Owner);
                Close();
            }
            int so = Settings.Default.DefaultSortOrder;
            if (so == -1 || so == 0 || so == 1)
                // add 1 to get zero based index to match -1 based enum UI.NodeSortOrder
                sortOrderComboBox.SelectedIndex = so + 1;
        }

        private void EditCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MainForm)Owner).InPlaceNodeEditing = editCheckBox.Checked;
        }

        private void OnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            (Owner).TopMost = onTopCheckBox.Checked;
        }

        private void MemoryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (memoryCheckBox.Checked)
                ((MainForm)Owner).StartMetadataLoad();
            else
                ((MainForm)Owner).StopMetadataLoad();
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
        }

        private void mainMenuCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MainForm)Owner).menuStrip1.Visible = mainMenuCheckBox.Checked;
        }

        private void mainToolbarCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MainForm)Owner).toolStrip1.Visible = mainToolbarCheckBox.Checked;
        }

        private void secondToolbarCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MainForm)Owner).toolStrip2.Visible = secondToolbarCheckBox.Checked;
        }

        private void statusBarCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MainForm)Owner).statusStrip.Visible = statusBarCheckBox.Checked;
        }

        private void sortOrderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // subtract 1 to get zero based index to match -1 based enum UI.NodeSortOrder
            Settings.Default.DefaultSortOrder = sortOrderComboBox.SelectedIndex - 1;
        }

        private void allowUnsortedOrderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (allowUnsortedOrderCheckBox.Checked)
                sortOrderComboBox.Items[1] = "Unsorted";
            else
                sortOrderComboBox.Items[1] = "";
        }

        private void showAdministrativeToolsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MainForm)Owner).administrativeToolsToolStripMenuItem.Visible = showAdministrativeToolsCheckBox.Checked;
        }

        private void themeDescriptionToolTipCheckBox_Click(object sender, EventArgs e)
        {
            ((MainForm)Owner).SetTreeViewToolTips();
        }

    }
}
