using NPS.AKRO.ThemeManager.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace NPS.AKRO.ThemeManager.UI.Forms
{
    public partial class PropertiesForm : Form
    {
        public PropertiesForm()
        {
            InitializeComponent();
        }

        private void dataBrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "lyr";
            openFileDialog1.Filter = "ArcMap Layerfile|*.lyr|ArcMap Documents|*.mxd;*.mxt;*.pmf|Google Earth|*.kml;*.kmz|All Files|*.*";
            TextBox tb = ((Button)sender).Tag as TextBox;
            Debug.Assert(tb != null, "browse Button has no text box in it's tag field");
            if (tb == null)
                return;
            openFileDialog1.Multiselect = false;
            DialogResult res = openFileDialog1.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                tb.Text = openFileDialog1.FileName;
                Debug.Assert(tb.DataBindings.Count == 1, "more or less than 1 data binding on text field");
                if (tb.DataBindings.Count > 0)
                    tb.DataBindings[0].WriteValue();
                tb.Focus();
            }
        }

        private void metadataBrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "xml";
            openFileDialog1.Filter = "Metadata|*.xml";
            TextBox tb = ((Button)sender).Tag as TextBox;
            Debug.Assert(tb != null, "browse Button has no text box in it's tag field");
            if (tb == null)
                return;
            openFileDialog1.Multiselect = false;
            DialogResult res = openFileDialog1.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                tb.Text = openFileDialog1.FileName;
                Debug.Assert(tb.DataBindings.Count == 1, "more or less than 1 data binding on text field");
                if (tb.DataBindings.Count > 0)
                    tb.DataBindings[0].WriteValue();
                tb.Focus();
            }
        }

        private void reloadThemeButton_Click(object sender, EventArgs e)
        {
            ReloadTheme(themeDescription.DataBindings["Text"].DataSource as TmNode);
        }

        private void reloadThemesbutton_Click(object sender, EventArgs e)
        {
            ReloadTheme(categoryDescription.DataBindings["Text"].DataSource as TmNode);
        }

        private void reloadAllbutton_Click(object sender, EventArgs e)
        {
            ReloadTheme(themelistDescription.DataBindings["Text"].DataSource as TmNode);
        }

        private static void ReloadTheme(TmNode node)
        {
            Debug.Assert(node != null, "There is no bound TMNode for this property panel");
            if (node == null)
            {
                MessageBox.Show("Internal Error:  Unable to find the node to reload.");
                return;
            }
            LoadingForm form = new LoadingForm();
            if (node.IsTheme)
                form.Message = "Reloading " + node.Name + "...";
            else
                form.Message = "Reloading all themes in " + node.Name + "...";
            form.AllowCancel = true;
            form.Node = node;
            form.Command = form.ReloadNode;
            form.ShowDialog();
            //Treeview may need updating.
            //Data type (icon) may have changed, and sub-themes may have been added/removed.
            node.UpdateTree();
        }

        private void syncThemeButton_Click(object sender, EventArgs e)
        {
            SyncTheme(themeDescription.DataBindings["Text"].DataSource as TmNode);
        }

        private void syncThemesButton_Click(object sender, EventArgs e)
        {
            SyncThemes(categoryDescription.DataBindings["Text"].DataSource as TmNode);
        }

        private void syncAllButton_Click(object sender, EventArgs e)
        {
            SyncThemes(themelistDescription.DataBindings["Text"].DataSource as TmNode);
        }

        private void SyncTheme(TmNode node)
        {
            Debug.Assert(node != null, "There is no bound TMNode for this property panel");
            if (node == null)
            {
                MessageBox.Show("Internal Error:  Unable to find the node to sync.");
                return;
            }
            if (string.IsNullOrEmpty(node.Metadata.Path))
            {
                MessageBox.Show("Theme has no metadata");
                return;
            }
            if (node.HasChildren)
                SyncThemes(node);
            else
            {
                try
                {
                    // May need to load/verify metadata which could throw.
                    // No need to recurse, since we just checked that we have no children.
                    node.SyncWithMetadata(false); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Metadata Error: {ex.Message}.");
                 }
            }
        }

        private void SyncThemes(TmNode node)
        {
            Debug.Assert(node != null, "There is no bound TMNode for this property panel");
            if (node == null)
            {
                MessageBox.Show("Internal Error:  Unable to find the node to sync.");
                return;
            }
            LoadingForm form = new LoadingForm();
            form.Message = "Syncing all themes in " + node.Name + "...";
            form.AllowCancel = true;
            form.Node = node;
            form.Command = form.SyncNode;
            form.ShowDialog();
            //Treeview may need updating.
            //Description is used for tool tips, and PubDate is used for highlighting the icon.
            node.UpdateTree();            
        }

        private void textbox_DragDrop(object sender, DragEventArgs e)
        {
            TextBox tb = sender as TextBox;
            Debug.Assert(tb != null, "Drag and Drop not in a Textbox");
            if (tb == null)
                return;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                tb.Text = files[0];
                tb.Focus(); //needed to trigger the binding action.
                return;
            }

            // Text Drop - Can only get here if it is not a filedrop, so it must be a URI 
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                tb.Text = (string)e.Data.GetData(DataFormats.Text);
                tb.Focus(); //needed to trigger the binding action.
            }

        }

        //Called when something is dragged into (enters) the control
        private void file_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = SetDndEffect(e);
        }

        //Called when something is dragged into (enters) the control
        private void metadata_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = SetDndMetaEffect(e);
        }

        //dnd helper functions
        private static DragDropEffects SetDndEffect(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] exts = FileDropExtensions(e);
                if (exts.Length == 1 && exts[0].ToLower() == ".lyr")
                    return DragDropEffects.Copy;
            }
            return DragDropEffects.None;
        }

        private static DragDropEffects SetDndMetaEffect(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                if (Uri.IsWellFormedUriString((string)e.Data.GetData(DataFormats.Text), UriKind.Absolute))
                    return DragDropEffects.Copy;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] exts = FileDropExtensions(e);
                if (exts.Length == 1 && exts[0].ToLower() == ".xml")
                    return DragDropEffects.Copy;
            }
            return DragDropEffects.None;
        }

        private static string[] FileDropExtensions(DragEventArgs e)
        {
            Debug.Assert(e.Data.GetDataPresent(DataFormats.FileDrop),"Not a File Drop");
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i=0; i< files.Length; i++)
                files[i] = Path.GetExtension(files[i]);
            return files;
        }

    }
}
