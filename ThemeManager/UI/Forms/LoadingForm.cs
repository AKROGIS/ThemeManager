using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NPS.AKRO.ThemeManager.UI.Forms
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
        }

        internal bool AllowCancel { get; set; }
        private bool CancelRequested { get; set; }
        internal string Message { get; set; }
        internal TmNode Node { get; set; }
        internal Func<TmNode, Task<string>> Command { get; set; }

        private async void LoadingForm_Shown(object sender, EventArgs e)
        {
            if (Command == null)
                Close();
            else
                await RunAsync();
        }

        private async Task RunAsync()
        {
            taskLabel.Text = Message;
            if (AllowCancel)
                progressBar.Style = ProgressBarStyle.Continuous;
            else
                progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Visible = true;
            if (AllowCancel)
                cancelButton.Visible = true;
            else
                cancelButton.Visible = false;
            cancelButton.Text = "Stop";
            var result = await Command(Node);
            if (result == null)
                Close();
            errorLabel.Text = result;
            errorLabel.Visible = true;
            cancelButton.Visible = true;
            cancelButton.Text = "Close";
            cancelButton.Enabled = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (cancelButton.Text == "Close")
                Close();
            else
            {
                errorLabel.Text = "Cancelling request...";
                cancelButton.Enabled = false;
                CancelRequested = true;
            }
        }

        internal async Task<string> SyncNodeAsync(TmNode root)
        {
            if (root == null)
                return "No node provided for sync";

            root.SuspendUpdates();
            List<TmNode> nodes = root.Recurse(x => x.Children)
                                        .Where(n => !string.IsNullOrEmpty(n.Metadata.Path))
                                        .ToList();
            int count = nodes.Count;
            int index = 0;
            foreach (var node in nodes)
            {
                progressLabel.Text = string.Format("Syncing {1} of {2} ({0})", node.Name, index + 1, count);
                progressBar.Value = (int)(100 * (float)index / count);
                try
                {
                    await node.SyncWithMetadataAsync(false);
                }
                catch (Exception ex)
                {
                    root.ResumeUpdates(); 
                    return ex.Message;
                }
                if (CancelRequested)
                {
                    root.ResumeUpdates();
                    return null;
                }
                index++;
            }
            root.ResumeUpdates();
            return null;
        }

        internal async Task<string> ReloadNodeAsync(TmNode root)
        {
            if (root == null)
                return "No node provided for reload";

            root.SuspendUpdates();
            List<TmNode> nodes = root.Recurse(x => x.Children)
                                        .Where(n => n.IsTheme)
                                        .ToList();
            int count = nodes.Count;
            int index = 0;
            foreach (var node in nodes)
            {
                progressLabel.Text = string.Format("Loading {1} of {2} ({0})", node.Name, index + 1, count);
                progressBar.Value = (int)(100 * (float)index / count);
                try
                {
                    await node.ReloadThemeAsync();
                }
                catch (Exception ex)
                {
                    root.ResumeUpdates(); 
                }
                if (CancelRequested)
                {
                    Console.WriteLine("Canceling...");
                    root.ResumeUpdates();
                    return null;
                }
                index++;
            }
            root.ResumeUpdates();
            return null;
        }

    }
}
