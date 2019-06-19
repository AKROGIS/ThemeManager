using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Properties;
using System;
using System.Windows.Forms;

namespace NPS.AKRO.ThemeManager.UI.Forms
{
    public partial class PreferencesForm : Form
    {
        public PreferencesForm()
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
        }

        private void OnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            (Owner).TopMost = onTopCheckBox.Checked;
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
        }

        private void AdvancedOptionsButton_Click(object sender, EventArgs e)
        {
            AdvancedPreferencesForm form = new AdvancedPreferencesForm().CommonInit();
            form.Show(this.Owner);
            this.Close();
        }
    }
}
