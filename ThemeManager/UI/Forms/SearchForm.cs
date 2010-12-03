using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using NPS.AKRO.ThemeManager.Extensions;
using NPS.AKRO.ThemeManager.Properties;
using NPS.AKRO.ThemeManager.Model;

namespace NPS.AKRO.ThemeManager.UI.Forms
{
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();
        }

        #region UI Events

        private void SearchForm_Load(object sender, EventArgs e)
        {
            if (!(Owner is MainForm))
            {
                MessageBox.Show(Resources.SearchForm_Message_Cant_find_owner);
                Close();
            }
            FormGotFocus();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (searchButton.Text == Resources.SearchForm_SearchButtonText_Search)
                DoSearch();
            else
                if (backgroundSearcher.WorkerSupportsCancellation)
                    backgroundSearcher.CancelAsync();
        }

        private void ThemeNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetSearchButton();
        }

        private void MetadataTextBox_TextChanged(object sender, EventArgs e)
        {
            if (HaveMetaSearchText && !HaveMetaSearchSection)
                MetadataAllSectionsCheckBox.Checked = true;
            SetSearchButton();
        }

        private void AllSectionsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (MetadataAllSectionsCheckBox.Checked)
            {
                metadataAbstactsCheckBox.Checked = false;
                metadataAbstactsCheckBox.Enabled = false;
                metadataThemeKeywordsCheckBox.Checked = false;
                metadataThemeKeywordsCheckBox.Enabled = false;
                metadataPlaceKeywordsCheckBox.Checked = false;
                metadataPlaceKeywordsCheckBox.Enabled = false;
            }
            else
            {
                if (HaveMetaSearchText && !HaveMetaSearchSection)
                    metadataAbstactsCheckBox.Checked = true;
                metadataAbstactsCheckBox.Enabled = true;
                metadataThemeKeywordsCheckBox.Enabled = true;
                metadataPlaceKeywordsCheckBox.Enabled = true;
            }
            SetSearchButton();
        }

        private void ThemeKeywordsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (HaveMetaSearchText && !HaveMetaSearchSection)
                MetadataAllSectionsCheckBox.Checked = true;
            SetSearchButton();
        }

        private void PlaceKeywordsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (HaveMetaSearchText && !HaveMetaSearchSection) MetadataAllSectionsCheckBox.Checked = true;
            SetSearchButton();
        }

        private void AbstactsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (HaveMetaSearchText && !HaveMetaSearchSection) MetadataAllSectionsCheckBox.Checked = true;
            SetSearchButton();
        }

        private void ParkUnitCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            SetSearchButton();
        }

        private void PubDate_AfterValueChanged(object sender, EventArgs e)
        {
            if (pubDateAfter.Checked && pubDateAfter.Value > DateTime.Now)
            {
                MessageBox.Show(Resources.SearchForm_Message_Invalid_PubDates);
                pubDateAfter.Value = DateTime.Now;
            }
            SetDateAndOrText();
            SetSearchButton();
        }

        private void PubDate_BeforeValueChanged(object sender, EventArgs e)
        {
            SetDateAndOrText();
            SetSearchButton();
        }

        private void AndOrButton_Click(object sender, EventArgs e)
        {
            ToggleAndOrButtons(((Control)sender).Text);
        }

        private void SearchThemes_CheckedChanged(object sender, EventArgs e)
        {
            SetSearchButton();
        }

        private void SearchCategories_CheckedChanged(object sender, EventArgs e)
        {
            SetSearchButton();
        }

        #endregion

        #region Helper Methods

        private void FormGotFocus()
        {
            themeNameTextBox.Text = ((MainForm)Owner).SearchText;
            themeNameTextBox.Focus();
            Text = Resources.SearchForm_Title_Prefix + ((MainForm)Owner).SearchLocation;
        }

        private bool HaveNodeType
        {
            get { return (searchThemesCheckBox.Checked || searchCategoriesCheckBox.Checked); }
        }

        private bool HaveThemeSearch
        {
            get { return themeNameTextBox.Text.Length > 0; }
        }

        private bool HaveMetaSearch
        {
            get
            {
                return HaveMetaSearchText && HaveMetaSearchSection;
            }
        }

        private bool HaveMetaSearchSection
        {
            get
            {
                return (metadataAbstactsCheckBox.Checked || MetadataAllSectionsCheckBox.Checked ||
                        metadataPlaceKeywordsCheckBox.Checked || metadataThemeKeywordsCheckBox.Checked);
            }
        }
        
        private bool HaveMetaSearchText
        {
            get
            {
                return metadataTextBox.Text.Length > 0;
            }
        }

        private bool HaveDateSearch
        {
            get
            {
                return (pubDateBefore.Checked || pubDateAfter.Checked);
            }
        }

        private bool HaveParkCode
        {
            get
            {
                return parkUnitCodeTextBox.Text.Length > 0;

            }
        }

        private void SetSearchButton()
        {
            if (HaveNodeType && (HaveThemeSearch || HaveMetaSearch || HaveDateSearch || HaveParkCode))
                searchButton.Enabled = true;
            else
                searchButton.Enabled = false;
        }

        private void ToggleAndOrButtons(string text)
        {
            if (text == Resources.SearchForm_AndOrButtons_Or)
            {
                andOrButton1.Text = Resources.SearchForm_AndOrButtons_And;
                andOrButton2.Text = Resources.SearchForm_AndOrButtons_And;
                andOrButton3.Text = Resources.SearchForm_AndOrButtons_And;
            }
            else
            {
                andOrButton1.Text = Resources.SearchForm_AndOrButtons_Or;
                andOrButton2.Text = Resources.SearchForm_AndOrButtons_Or;
                andOrButton3.Text = Resources.SearchForm_AndOrButtons_Or;
            }
        }

        private void SetDateAndOrText()
        {
            if (pubDateAfter.Checked && pubDateBefore.Checked)
            {
                dateAndOrText.Visible = true;
                dateAndOrText.Text = pubDateAfter.Value < pubDateBefore.Value
                                         ? Resources.SearchForm_AndOrButtons_And
                                         : Resources.SearchForm_AndOrButtons_Or;
            }
            else
                dateAndOrText.Visible = false;
        }

        #endregion

        #region Search Functions

        private void BackgroundSearcher_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            SearchInBackground(worker, e);
        }

        private void BackgroundSearcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            string text = string.Format("Found {0} in {1} of {2}", _found, _searched, _total);
            progressText.Text = text;
            //The follwoing cute idea draws the text on top of the progress bar, but it looks terrible
            //using (Graphics gr = progressBar.CreateGraphics())
            //{
            //    gr.DrawString(text,
            //        this.Font,
            //        Brushes.Black,
            //        new PointF(progressBar.Width / 2.0F - (gr.MeasureString(text,
            //            this.Font).Width / 2.0F),
            //        progressBar.Height / 2.0F - (gr.MeasureString(text,
            //            this.Font).Height / 2.0F))); //SystemFonts.DefaultFont
            //}
        }

        private void BackgroundSearcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                if (e.Cancelled)
                {
                    backgroundSearcher.CancelAsync();
                    if (_results != null && _results.Count() > 0)
                        if (MessageBox.Show("Do you want to display the incomplete results?","Search Canceled",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                            FinishSearch(_results, _searchParams);
                    searchButton.Text = Resources.SearchForm_SearchButtonText_Search;
                    progressBar.Value = 0;
                    progressText.Text = Resources.SearchForm_FinishSearch_No_themes_found;
                }
                else
                {
                    FinishSearch(_results, _searchParams);
                }
            }
        }

        private void DoSearch()
        {
            _searchParams = BuildSearchParameters();
            Debug.Assert(Owner is MainForm, Resources.SearchForm_Message_Cant_find_owner);
            var owner = Owner as MainForm;
            if (owner == null)
                throw new ApplicationException(Resources.SearchForm_Message_Cant_find_owner);
            _searchParams.Location = owner.SearchLocation;
            if (HaveMetaSearch || HaveParkCode)
            {
                if (!backgroundSearcher.IsBusy)
                {
                    _searchNodes = owner.CurrentTreeView.SearchNodes().ToList();
                    searchButton.Text = Resources.SearchForm_SearchButtonText_Cancel;
                    backgroundSearcher.RunWorkerAsync();
                }
            }
            else
            {
                Stopwatch sw = Stopwatch.StartNew();
                IEnumerable<TmNode> nodes = owner.CurrentTreeView.SearchNodes()
                        .SelectMany(n => n.Recurse(node => node.Children)
                                    .Where(x => x.Matches(_searchParams))
                        );
                owner.DisplaySearchResults(_searchParams, nodes);
                sw.Stop();
                Debug.Print("\n***** Foreground search/displaying took {0}ms", sw.ElapsedMilliseconds);
                if (nodes != null && nodes.Count() > 0)
                    if (!Settings.Default.KeepSearchFormOpen)
                        Close();
            }
        }

        // private variables used in both threads to communicate progress and results.
        private List<TmNode> _searchNodes;
        private List<TmNode> _results;
        private SearchParameters _searchParams;
        private int _found;
        private int _searched;
        private int _total;

        private void SearchInBackground(BackgroundWorker worker, DoWorkEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<TmNode> allNodes = _searchNodes.SelectMany(n => n.Recurse(node => node.Children)).ToList();
            _results = new List<TmNode>();
            _found = 0;
            _searched = 0;
            _total = allNodes.Count;
            worker.ReportProgress(0);
            sw.Stop();
            Debug.Print("\n***** Setting up for background searching took {0}ms", sw.ElapsedMilliseconds);
            sw.Reset(); sw.Start();
            foreach (var node in allNodes)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                if (node.Matches(_searchParams))
                {
                    _found++;
                    _results.Add(node);
                } 
                _searched++;
                worker.ReportProgress((100 * _searched) / _total);
            }
            sw.Stop();
            Debug.Print("***** Background searching took {0}ms", sw.ElapsedMilliseconds);
        }

        // Called from the UI thread when done.
        private void FinishSearch(IEnumerable<TmNode> nodes, SearchParameters searchParams)
        {
            Debug.Assert(Owner is MainForm, Resources.SearchForm_Message_Cant_find_owner);
            var owner = Owner as MainForm;
            if (owner == null)
                throw new ApplicationException(Resources.SearchForm_Message_Cant_find_owner);
            searchButton.Text = Resources.SearchForm_SearchButtonText_Search;
            progressBar.Value = 0;
            progressText.Text = Resources.SearchForm_FinishSearch_No_themes_found;
            Debug.Print("\n\n****** Start Treeview Update *****\n\n");
            Stopwatch sw = Stopwatch.StartNew();
            owner.DisplaySearchResults(searchParams, nodes);
            sw.Stop();
            Debug.Print("***** displaying took {2}ms", owner.CurrentTreeView, searchParams, sw.ElapsedMilliseconds);
            if (nodes != null && nodes.Count() > 0)
                if (!Settings.Default.KeepSearchFormOpen)
                    Close();
        }
        
        #endregion

        #region Build Search Paramaters
        
        private SearchParameters BuildSearchParameters()
        {
            var sp = new SearchParameters
                         {
                             MatchAll = (andOrButton1.Text == Resources.SearchForm_AndOrButtons_And),
                             SearchThemes = searchThemesCheckBox.Checked,
                             SearchCategories = searchCategoriesCheckBox.Checked
                         };

            SearchOptions so;
            if (HaveThemeSearch)
            {
                if (themeNameCheckBox.Checked &&
                    themeDescriptionCheckBox.Checked &&
                    themeSummaryCheckBox.Checked &&
                    themeKeywordsCheckBox.Checked)
                {
                    so = new SearchOptions(SearchType.Theme, themeNameTextBox.Text) { FindAll = allThemeNamesRadioButton.Checked };
                    sp.Add(so);
                }
                else
                {
                    if (themeNameCheckBox.Checked)
                    {
                        so = new SearchOptions(SearchType.ThemeName, themeNameTextBox.Text) { FindAll = allThemeNamesRadioButton.Checked };
                        sp.Add(so);
                    }
                    if (themeDescriptionCheckBox.Checked)
                    {
                        so = new SearchOptions(SearchType.ThemeDescription, themeNameTextBox.Text) { FindAll = allThemeNamesRadioButton.Checked };
                        sp.Add(so);
                    }
                    if (themeSummaryCheckBox.Checked)
                    {
                        so = new SearchOptions(SearchType.ThemeSummary, themeNameTextBox.Text) { FindAll = allThemeNamesRadioButton.Checked };
                        sp.Add(so);
                    }
                    if (themeKeywordsCheckBox.Checked)
                    {
                        so = new SearchOptions(SearchType.ThemeTags, themeNameTextBox.Text) { FindAll = allThemeNamesRadioButton.Checked };
                        sp.Add(so);
                    }
                }
            }

            if (HaveMetaSearch)
            {
                if (MetadataAllSectionsCheckBox.Checked)
                    sp.Add(MetaSearch("")); // search the whole document
                else
                {
                    if (metadataAbstactsCheckBox.Checked)
                        sp.Add(MetaSearch("abstract")); // /metadata/idinfo/descript/abstract
                    if (metadataPlaceKeywordsCheckBox.Checked)
                        sp.Add(MetaSearch("placekey"));// /metadata/idinfo/keywords/place/placekey
                    if (metadataThemeKeywordsCheckBox.Checked)
                        sp.Add(MetaSearch("themekey")); // /metadata/idinfo/keywords/theme/themekey
                    
                }
            }
            if (HaveDateSearch)
            {
                so = new SearchOptions(SearchType.PubDate, null);
                if (pubDateAfter.Checked)
                    so.MaxDaysSinceEdit = (DateTime.Now - pubDateAfter.Value).Days;
                if (pubDateBefore.Checked)
                {
                    so.MinDaysSinceEdit = (DateTime.Now - pubDateBefore.Value).Days;
                    if (so.MinDaysSinceEdit < 0)
                        so.MinDaysSinceEdit = 0;
                }
                sp.Add(so);
            }

            if (HaveParkCode)
            {
                // Xpath in FGDC Metadata: /metadata/NPS_Info/NPS_Unit/UnitCode
                so = new SearchOptions(SearchType.Metadata, parkUnitCodeTextBox.Text) {XmlElement = "UnitCode"};
                sp.Add(so);
            }
            return sp;
        }

        private SearchOptions MetaSearch(string xmlTag)
        {
            var so = new SearchOptions(SearchType.Metadata, metadataTextBox.Text)
                                   {
                                       FindAll = allMetadataRadioButton.Checked,
                                       XmlElement = xmlTag
                                   };
            return so;
        }

        #endregion

    }
}
