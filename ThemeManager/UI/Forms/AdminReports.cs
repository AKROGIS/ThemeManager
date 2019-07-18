using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NPS.AKRO.ThemeManager.UI.Forms
{
    public partial class AdminReports : Form
    {
        public AdminReports()
        {
            InitializeComponent();
            LoadToolList();
        }

        private void AdminTools_Load(object sender, EventArgs e)
        {
            if (!(Owner is MainForm))
            {
                MessageBox.Show("Admin Tools Form_can't find the Main Form.");
                Close();
            }

            LoadThemeLists();
        }

        private void LoadToolList()
        {
            reportComboBox.DataSource = new BindingList<Tool>()
                {
                    new Tool {
                        Name ="All themes", 
                        Description = "List all themes and their layer (or source) file.",
                        Command = ListThemes
                    },
                    new Tool {
                        Name ="Themes without a file",
                        Description = "List themes that do not have (or have a missing) layer (or other) file (slow).",
                        Command = ListThemesNotFound
                    },
                    new Tool {
                        Name ="Themes with unknown type",
                        Description = "List themes for which the data type could not be translated to an icon.",
                        Command = ListThemesWithUnknownIcon
                    },
                    new Tool {
                        Name ="All data sources",
                        Description = "List data sources (and other properties) for all themes and sub themes.",
                        Command = ListDataSources
                    },
                    new Tool {
                        Name ="All metadata",
                        Description = "List the metadata path and type for all themes and sub-themes.",
                        Command = ListMetadata
                    },
                    new Tool {
                        Name ="Themes with no metadata path",
                        Description = "List themes and sub-themes with no path to metadata.",
                        Command = ListThemesWithNoMetadataPath
                    },
                    new Tool {
                        Name ="Themes with no metadata info",
                        Description = "List themes and sub-themes that have a metadata path, but no info (tags, summary..).",
                        Command = ListThemesWithNoMetadataInfo
                    },
                    new Tool {
                        Name ="Check all metadata",
                        Description = "Verifies that all metadata files can be read (slow).",
                        Command = ListMetadataProblems
                    },
                    /*
                    new Tool {Name="List themes with missing data",
                              Description = "List themes with missing data",
                              Command = null},
                    new Tool {Name="List data types used in themes",
                              Description = "List data types used in themes",
                              Command = null},
                    new Tool {Name="List spatial references used in themes",
                              Description = "List spatial references used in themes",
                              Command = null},
                    new Tool {Name="List themes using relative paths",
                              Description = "List themes using relative paths",
                              Command = null}
                    */
                };

            reportComboBox.DisplayMember = "Name";
        }

        private void LoadThemeLists()
        {
            foreach (TmNode themeList in ((MainForm)Owner).ThemeLists)
                themeListComboBox.Items.Add(themeList);
            themeListComboBox.DisplayMember = "Name";
            if (themeListComboBox.Items.Count > 0)
                themeListComboBox.SelectedIndex = 0;
        }

        private void toolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tool tool = reportComboBox.SelectedItem as Tool;
            if (tool != null)
                statusLabel.Text = tool.Description;
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (goButton.Text == "Go")
            {
                Tool tool = reportComboBox.SelectedItem as Tool;
                if (tool == null)
                {
                    MessageBox.Show("Unexpected error accessing the report Combo Box");
                    return;
                }
                TmNode themeList = themeListComboBox.SelectedItem as TmNode;
                if (themeList == null)
                {
                    MessageBox.Show("Unexpected error accessing the theme list Combo Box");
                    return;
                }
                if (tool.Command == null)
                    MessageBox.Show("This function is not yet implemented");
                else
                {
                    tool.Node = themeList;
                    resultsDataGridView.DataSource = null;
                    statusLabel.Text = "Processing Report... ";
                    progressBar.Visible = true;
                    goButton.Text = "Stop";
                    backgroundWorker.RunWorkerAsync(tool);
                }
            }
            else
            {
                statusLabel.Text = "Canceling Report... ";
                goButton.Enabled = false;
                backgroundWorker.CancelAsync();
            }
        }

        private DataTable ListThemes(TmNode themeList, BackgroundWorker bw)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Category", typeof(string)));
            data.Columns.Add(new DataColumn("Theme", typeof(string)));
            data.Columns.Add(new DataColumn("File", typeof(string)));
            data.Columns.Add(new DataColumn("File Type", typeof(string)));
            foreach (var theme in themeList.Recurse(x => x.Children).Where(n => n.IsTheme))
            {
                DataRow row = data.NewRow();
                row["Category"] = theme.CategoryPath();
                row["Theme"] = theme.Name;
                row["File"] = theme.Data.Path;
                if (!string.IsNullOrEmpty(theme.Data.Path))
                    row["File Type"] = Path.GetExtension(theme.Data.Path);
                data.Rows.Add(row);
                if (bw.CancellationPending)
                    return data;
            }
            return data;
        }

        internal DataTable ListThemesNotFound(TmNode themeList, BackgroundWorker bw)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Category", typeof(string)));
            data.Columns.Add(new DataColumn("Theme", typeof(string)));
            data.Columns.Add(new DataColumn("File", typeof(string)));
            data.Columns.Add(new DataColumn("Error", typeof(string)));
            List<TmNode> nodes = themeList.Recurse(x => x.Children).Where(n => n.IsTheme).ToList();
            int count = nodes.Count;
            int index = 0;
            foreach (var theme in nodes)
            {
                if (string.IsNullOrEmpty(theme.Data.Path) || !File.Exists(theme.Data.Path))
                {
                    DataRow row = data.NewRow();
                    row["Category"] = theme.CategoryPath();
                    row["Theme"] = theme.Name;
                    row["File"] = theme.Data.Path;
                    if (string.IsNullOrEmpty(theme.Data.Path))
                        row["Error"] = "Theme has no file";
                    else
                        row["Error"] = "File not found";
                    data.Rows.Add(row);
                }
                index++;
                if (bw.CancellationPending)
                    return data;
                bw.ReportProgress((int)(100 * (float)index / count));
            }
            return data;
        }

        internal DataTable ListMetadata(TmNode themeList, BackgroundWorker bw)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Category", typeof(string)));
            data.Columns.Add(new DataColumn("Theme", typeof(string)));
            data.Columns.Add(new DataColumn("Type", typeof(string)));
            data.Columns.Add(new DataColumn("Metadata Path", typeof(string)));
            data.Columns.Add(new DataColumn("Metadata Type", typeof(string)));
            data.Columns.Add(new DataColumn("Metadata Format", typeof(string)));
            foreach (var theme in themeList.Recurse(x => x.Children)
                                           .Where(n => (n.IsTheme || n.IsSubTheme) && 
                                                       !string.IsNullOrWhiteSpace(n.Metadata.Path)))
            {
                DataRow row = data.NewRow();
                row["Category"] = theme.CategoryPath();
                row["Theme"] = theme.Name;
                row["Type"] = (theme.IsTheme) ? "Theme" : "SubTheme";
                row["Metadata Path"] = theme.Metadata.Path;
                row["Metadata Type"] = theme.Metadata.Type;
                row["Metadata Format"] = theme.Metadata.Format;
                data.Rows.Add(row);
                if (bw.CancellationPending)
                    return data;
            }
            return data;
        }

        internal DataTable ListThemesWithNoMetadataPath(TmNode themeList, BackgroundWorker bw)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Category", typeof(string)));
            data.Columns.Add(new DataColumn("Theme", typeof(string)));
            data.Columns.Add(new DataColumn("Type", typeof(string)));
            data.Columns.Add(new DataColumn("Theme Type", typeof(string)));
            data.Columns.Add(new DataColumn("Data Source Path", typeof(string)));
            foreach (var theme in themeList.Recurse(x => x.Children)
                                           .Where(n => (n.IsTheme || n.IsSubTheme) && 
                                                       string.IsNullOrWhiteSpace(n.Metadata.Path)))
            {
                DataRow row = data.NewRow();
                row["Category"] = theme.CategoryPath();
                row["Theme"] = theme.Name;
                row["Type"] = (theme.IsTheme) ? "Theme" : "SubTheme";
                row["Theme Type"] = theme.Data.Type;
                row["Data Source Path"] = theme.Data.DataSource;
                data.Rows.Add(row);
                if (bw.CancellationPending)
                    return data;
            }
            return data;
        }

        internal DataTable ListThemesWithNoMetadataInfo(TmNode themeList, BackgroundWorker bw)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Category", typeof(string)));
            data.Columns.Add(new DataColumn("Theme", typeof(string)));
            data.Columns.Add(new DataColumn("Type", typeof(string)));
            data.Columns.Add(new DataColumn("Metadata Path", typeof(string)));
            foreach (var theme in themeList.Recurse(x => x.Children)
                                           .Where(n => (n.IsTheme || n.IsSubTheme) && 
                                                       !string.IsNullOrWhiteSpace(n.Metadata.Path) &&
                                                       string.IsNullOrWhiteSpace(n.Description) &&
                                                       string.IsNullOrWhiteSpace(n.Summary) &&
                                                       string.IsNullOrWhiteSpace(n.Tags)))
            {
                DataRow row = data.NewRow();
                row["Category"] = theme.CategoryPath();
                row["Theme"] = theme.Name;
                row["Type"] = (theme.IsTheme) ? "Theme" : "SubTheme";
                row["Metadata Path"] = theme.Metadata.Path;
                data.Rows.Add(row);
                if (bw.CancellationPending)
                    return data;
            }
            return data;
        }

        internal DataTable ListMetadataProblems(TmNode themeList, BackgroundWorker bw)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Category", typeof(string)));
            data.Columns.Add(new DataColumn("Theme", typeof(string)));
            data.Columns.Add(new DataColumn("Metadata", typeof(string)));
            data.Columns.Add(new DataColumn("Error", typeof(string)));
            List<TmNode> nodes = themeList.Recurse(x => x.Children)
                                          .Where(n => n.IsTheme || n.IsSubTheme && 
                                                 !string.IsNullOrEmpty(n.Metadata.Path))
                                          .ToList();
            int count = nodes.Count;
            int index = 0;
            foreach (var theme in nodes)
            {
                string error;
                try {
                    theme.Metadata.GetGeneralInfo();
                    error = theme.Metadata.ErrorMessage;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                if (error != null)
                {
                    DataRow row = data.NewRow();
                    row["Category"] = theme.CategoryPath();
                    row["Theme"] = theme.Name;
                    row["Metadata"] = theme.Metadata.Path;
                    row["Error"] = error;
                    data.Rows.Add(row);
                }
                index++;
                if (bw.CancellationPending)
                    return data;
                bw.ReportProgress((int)(100 * (float)index / count));
            }
            return data;
        }

        internal DataTable ListThemesWithUnknownIcon(TmNode themeList, BackgroundWorker bw)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Category", typeof(string)));
            data.Columns.Add(new DataColumn("Theme", typeof(string)));
            data.Columns.Add(new DataColumn("Type", typeof(string)));
            data.Columns.Add(new DataColumn("Theme Path", typeof(string)));
            data.Columns.Add(new DataColumn("Theme Type", typeof(string)));
            foreach (var theme in themeList.Recurse(x => x.Children)
                                           .Where(n => (n.IsTheme || n.IsSubTheme) && 
                                                       (n.ImageKey == "Theme" || n.ImageKey == "Themelock" || n.ImageKey == "Themenew" ||
                                                        n.Data.Type.Contains("Error", StringComparison.OrdinalIgnoreCase))))
            {
                DataRow row = data.NewRow();
                row["Category"] = theme.CategoryPath();
                row["Theme"] = theme.Name;
                row["Type"] = (theme.IsTheme) ? "Theme" : "SubTheme";
                row["Theme Path"] = theme.Data.Path;
                row["Theme Type"] = theme.Data.Type;
                data.Rows.Add(row);
                if (bw.CancellationPending)
                    return data;
            }
            return data;
        }

        internal DataTable ListDataSources(TmNode themeList, BackgroundWorker bw)
        {
            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Category", typeof(string)));
            data.Columns.Add(new DataColumn("Theme", typeof(string)));
            data.Columns.Add(new DataColumn("Type", typeof(string)));
            data.Columns.Add(new DataColumn("Theme Path", typeof(string)));
            data.Columns.Add(new DataColumn("Layer Name", typeof(string)));
            data.Columns.Add(new DataColumn("Theme Type", typeof(string)));
            data.Columns.Add(new DataColumn("Data Source Path", typeof(string)));
            data.Columns.Add(new DataColumn("Workspace Path", typeof(string)));
            data.Columns.Add(new DataColumn("Workspace Type", typeof(string)));
            data.Columns.Add(new DataColumn("Workspace ProgID", typeof(string)));
            data.Columns.Add(new DataColumn("Container", typeof(string)));
            data.Columns.Add(new DataColumn("Container Type", typeof(string)));
            data.Columns.Add(new DataColumn("Data Source", typeof(string)));
            data.Columns.Add(new DataColumn("Data Source Type", typeof(string)));
            foreach (var theme in themeList.Recurse(x => x.Children)
                                            .Where(n => (n.IsTheme || n.IsSubTheme)))
            {
                DataRow row = data.NewRow();
                //reload cannot be done in the background, because treeview 
                // on UI thread gets property updates
                //theme.ReloadTheme();
                row["Category"] = theme.CategoryPath();
                row["Theme"] = theme.Name;
                row["Type"] = (theme.IsTheme) ? "Theme" : "SubTheme";
                row["Theme Path"] = theme.Data.Path;
                row["Layer Name"] = theme.Data.DataSetName;
                row["Theme Type"] = theme.Data.Type;
                row["Data Source Path"] = theme.Data.DataSource;
                row["Workspace Path"] = theme.Data.WorkspacePath;
                row["Workspace Type"] = theme.Data.WorkspaceType;
                row["Workspace ProgID"] = theme.Data.WorkspaceProgId;
                row["Container"] = theme.Data.Container;
                row["Container Type"] = theme.Data.ContainerType;
                row["Data Source"] = theme.Data.DataSourceName;
                row["Data Source Type"] = theme.Data.DataSetType;
                data.Rows.Add(row);
                if (bw.CancellationPending)
                    return data;
            }
            return data;
        }


        private class Tool
        {
            public string Name { get; set; }
            internal string Description { get; set; }
            internal Func<TmNode,BackgroundWorker,DataTable> Command { get; set; }
            internal TmNode Node { get; set; }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            Tool arg = (Tool)e.Argument;
            e.Result = arg.Command(arg.Node,bw);
            if (bw.CancellationPending)
            {
                //setting e.Cancel to true, renders e.Results invalid.
                //I want to except the results even if incompete.
                //e.Cancel = true;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // There was an error during the operation.
                string msg = String.Format("An error occurred: {0}", e.Error.Message);
                statusLabel.Text = msg;
            }
            else
            {
                // if e.Cancelled = true, then e.Results throws and exception.
                DataTable data = e.Result as DataTable;
                if (data == null)// The operation completed normally.
                    statusLabel.Text = "Error: No results were returned";
                else
                {
                    // e.Cancelled is never true.
                    //if (e.Cancelled)
                    //    statusLabel.Text = "Operation was canceled. Results are incomplete";
                    //else
                    //    statusLabel.Text = "Report Completed Successfully";
                    statusLabel.Text = "Done.";
                    resultsDataGridView.DataSource = data;
                }
            }
            progressBar.Visible = false;
            goButton.Text = "Go";
            goButton.Enabled = true;
        }
    }
}
