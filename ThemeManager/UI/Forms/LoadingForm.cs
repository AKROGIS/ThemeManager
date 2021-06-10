using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        internal string Message { get; set; }
        internal TmNode Node { get; set; }
        internal string Path { get; set; }
        internal Func<BackgroundWorker, TmNode, string, string> Command { get; set; }
        private string Error { get; set; }

        private void LoadingForm_Shown(object sender, EventArgs e)
        {
            if (Command == null)
                Close();
            else
                Run();
        }

        private void Run()
        {
            Error = null;
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
            //Application.DoEvents();  //Paint the form in case background finishes quickly
            backgroundWorker.RunWorkerAsync();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (cancelButton.Text == "Close")
                Close();
            else
            {
                errorLabel.Text = "Cancelling request...";
                cancelButton.Enabled = false;
                backgroundWorker.CancelAsync();
            }
        }

        //This runs on the background thread
        //Do not access any UI controls from this thead.
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            //background worker was created on the main (UI) thread.
            //Since it is now available on multiple threads, because of modifying it's state.
            //It is best to not modify it's state, and just use the event args for communication
            //with the main thread.

            
            //I'm not sure I should be accessing these form properties on the background thread,
            //but it seems to be working.
            e.Result = Command(bw, Node, Path);

            //Command does not have access to the DoWorkEventArgs, and cannot set the Cancel property
            //In order to respond to a cancel event, Command will need to periodically
            //check the bw.CancellationPending, and return prematurely if true.
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        //This runs on the UI thread, when the background thread is done.
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // There was an error during the operation.
                Error = e.Error.Message;
                string msg = String.Format("An error occurred: {0}", Error);
                errorLabel.Text = msg;
            }
            else if (e.Cancelled)
            {
                // The user canceled the operation.
                errorLabel.Text = "Operation was canceled.";
            }
            else
            {
                if (e.Result == null) // The operation completed normally.
                    errorLabel.Text = "Work completed successfully.";
                else
                {
                    Error = (string)e.Result;
                    errorLabel.Text = Error;
                }
            }
            if (Error == null)
                Close();

            errorLabel.Visible = true;
            cancelButton.Visible = true;
            cancelButton.Text = "Close";
            cancelButton.Enabled = true;
        }

        //this runs on the UI thread whenever the background thread calls ReportProgress(int)
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            progressLabel.Text = Message;
        }

        internal string SyncNode(BackgroundWorker bw, TmNode root, string path)
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
                Message = string.Format("Syncing {1} of {2} ({0})", node.Name, index + 1, count);
                bw.ReportProgress((int)(100 * (float)index / count));
                try
                {
                    // May need to load/verify metadata which could throw.
                    // No need to recurse because we already have a list of all nodes in this tree
                    // This is in a cancellable background thread, so we can just wait for the task to finish
                    node.SyncWithMetadataAsync(false).RunSynchronously();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                if (bw.CancellationPending)
                    return null;
                index++;
            }
            root.ResumeUpdates();
            return null;
        }

        internal string ReloadNode(BackgroundWorker bw, TmNode root, string path)
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
                Message = string.Format("Loading {1} of {2} ({0})", node.Name, index + 1, count);
                bw.ReportProgress((int)(100 * (float)index / count));
                try
                {
                    // This is on a cancellable background thread, so we do the async work synchronously
                    // node.ReloadTheme() may need to load to query ArcObjects
                    // which could throw any number of exceptions.
                    node.ReloadThemeAsync().RunSynchronously();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                if (bw.CancellationPending)
                    return null;
                index++;
            }
            root.ResumeUpdates();
            return null;
        }

    }
}
