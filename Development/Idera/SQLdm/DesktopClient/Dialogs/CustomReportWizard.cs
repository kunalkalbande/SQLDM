using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Controls.NavigationPane;
using Idera.SQLdm.DesktopClient.Views.Reports;
using Infragistics.Win.UltraWinGrid;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Windows.Forms;
    using Helpers;
    using Common.Configuration;
    using Common.Data;
    using Common.Services;
    using Common.UI.Dialogs;
    using Properties;
    using Wintellect.PowerCollections;
    using Infragistics.Win;
    using System.Globalization;
    using Idera.SQLdm.DesktopClient.Controls;
    using Infragistics.Windows.Themes;

    public partial class CustomReportWizard : Form
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CustomReportWizard");

        #region constants

        private const string SELECTEDMETRICCOUNTEXCEEDED =
            "You are limited to {0} counters per custom report. Please remove counters from the selected list, or select fewer items from available list and click \"Add\"";

        private const string INVALIDFILENAME =
            "The report name cannot:\r\n" +
            "- be empty strings\r\n" +
            "- contain any of the following characters: /?:&&\\*\"<>|#%,;@\"$+=\r\n" +
            "- contain Unicode control characters\r\n" +
            "- contain surrogate characters\r\n" +
            "- be system reserved names including 'CON', 'AUX', 'PRN', 'COM1' or 'LPT2'\r\n" +
            "- be '.' or '..'\r\n\r\n" +
            "Please enter a valid name";

        private const int MAXNUMBEROFMETRICS = 10;
       

        #endregion
        #region variables
        private Divelements.WizardFramework.WizardPageBase startPage;
        private DataTable _availableCountersDataTable = null;
        private DataTable _selectedCountersDataTable = null;
        //Dictionary<string, string> _selectedCounters = new Dictionary<string, string>();
        private DataTable AddedCounters = null;
        readonly Image MoveDown = Properties.Resources.MoveDown;
        readonly Image MoveUp = Properties.Resources.MoveUp;

        private SqlConnectionInfo connectionInfo;
        private bool addMode = true;
        private IManagementService managementService;

        private CustomReport _CurrentCustomReport = null;
        //private IList<int> previousSelection;
        #endregion
        #region Constructors
        /// <summary>
        /// Used to default to creating a new custom report
        /// </summary>
        public CustomReportWizard()
            : this(null)
        {
            addMode = true;
        }

        /// <summary>
        /// Used for editing an existing custom report
        /// </summary>
        /// <param name="node"></param>
        public CustomReportWizard(TreeNode node)
        {
            InitializeComponent();
            string selectedReport = null;

            if (node != null)
            {
                if (node.Tag is Pair<ReportsNavigationPane.ReportCategory, ReportTypes>)
                {
                    Pair<ReportsNavigationPane.ReportCategory, ReportTypes> selectedNodeTag
                        = (Pair<ReportsNavigationPane.ReportCategory, ReportTypes>)node.Tag;

                    if (selectedNodeTag.First.ToString() != "Custom")
                    {
                        this.Dispose();
                        return;
                    }
                    else
                    {
                        addMode = false;
                        selectedReport = node.Name;
                    }
                }
            }

            // get repository and management service interface
            connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);

            if (selectedReport != null)
            {
                _CurrentCustomReport = RepositoryHelper.GetCustomReport(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                            selectedReport);    
            
                
                _CurrentCustomReport.ReportRDL = GetCustomReportXML().Replace("{Custom Report Title}", selectedReport);


                //includes aggregation that may have been set up previously
                _selectedCountersDataTable = RepositoryHelper.GetSelectedCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, _CurrentCustomReport.Name);

                //populate the counters that have already been selected
                PopulateCustomReportMetrics(_selectedCountersDataTable, _CurrentCustomReport);

                PopulateSelectedCounters(_CurrentCustomReport);

                startPage = selectCounterPage;

                PrepareSelectCountersPage(_CurrentCustomReport);
            }
            else
            {
                startPage = introPage;
            }
            wizard.SelectedPage = startPage;

            // Autoscale font size.
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        #endregion
        #region Form and control Events
        private void CustomCounterWizard_Load(object sender, EventArgs e)
        {
            hideWelcomePageCheckBox.Checked = Settings.Default.HideCustomReportsWizardWelcomePage;

            if (addMode && Settings.Default.HideCustomReportsWizardWelcomePage)
            {
                wizard.GoNext();
            }
        }

        private void CustomReportsWizard_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(GetHelpTopic());
        }

        private void CustomReportsWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            ApplicationHelper.ShowHelpTopic(GetHelpTopic());
        }

        private void hideWelcomePageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HideCustomReportsWizardWelcomePage = hideWelcomePageCheckBox.Checked;
        }

        /// <summary>
        /// Add a new report with the specified name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddReport_Click(object sender, EventArgs e)
        {
            AddNewReport(txtReportName.Text);
        }

        private void lstAddedCounters_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateSelectedCountersPage();
        }

        private void btnAddCounters_Click(object sender, EventArgs e)
        {
            AddSelectedCounters(_CurrentCustomReport);
        }

        private void btnRemoveCounters_Click(object sender, EventArgs e)
        {
            RemoveSelectedCounters(_CurrentCustomReport);
        }

        private void lstReports_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateSelectReportPage();
        }

        private void lstReports_Click(object sender, EventArgs e)
        {
            UpdateSelectReportPage();
        }

        private void grdSelectedCounters_KeyPress(object sender, KeyPressEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            UltraGridCell activeCell = grid == null ? null : grid.ActiveCell;

            // if there is an active cell, its not in edit mode and can enter edit mode
            if (null != activeCell && false == activeCell.IsInEditMode && activeCell.CanEnterEditMode)
            {
                // if the character is not a control character
                if (char.IsControl(e.KeyChar) == false)
                {
                    // try to put cell into edit mode
                    grid.PerformAction(UltraGridAction.EnterEditMode);

                    // if this cell is still active and it is in edit mode...
                    if (grid.ActiveCell == activeCell && activeCell.IsInEditMode)
                    {
                        // get its editor
                        EmbeddableEditorBase editor = this.grdSelectedCounters.ActiveCell.EditorResolved;

                        // if the editor supports selectable text
                        if (editor.SupportsSelectableText)
                        {
                            // select all the text so it can be replaced
                            editor.SelectionStart = 0;
                            editor.SelectionLength = editor.TextLength;

                            if (editor is EditorWithMask)
                            {
                                // just clear the selected text and let the grid
                                // forward the keypress to the editor
                                editor.SelectedText = string.Empty;
                            }
                            else
                            {
                                // then replace the selected text with the character
                                editor.SelectedText = new string(e.KeyChar, 1);

                                // mark the event as handled so the grid doesn't process it
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }

        private void chkOSCounters_CheckedChanged(object sender, EventArgs e)
        {
            PopulateAvailableCounters(_availableCountersDataTable, _CurrentCustomReport);
        }

        private void chkSQLCounters_CheckedChanged(object sender, EventArgs e)
        {
            PopulateAvailableCounters(_availableCountersDataTable, _CurrentCustomReport);
        }

        //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - Event Handler added
        private void chkVirtCounters_CheckedChanged(object sender, EventArgs e)
        {
            PopulateAvailableCounters(_availableCountersDataTable, _CurrentCustomReport);
        }

        private void chkCustomCounters_CheckedChanged(object sender, EventArgs e)
        {
            PopulateAvailableCounters(_availableCountersDataTable, _CurrentCustomReport);
        }

        private void lstAvailableCounters_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateSelectedCountersPage();
        }

        private void grdSelectedCounters_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;
                    }
                    else
                    {
                        //toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
            else
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(UltraGridCell));

                if (contextObject is UltraGridCell)
                {
                    UltraGridCell cell = contextObject as UltraGridCell;

                    if (cell.Column.Key == "MoveUp")
                    {
                        //is there a row below?
                        if (cell.Row.Index > 0)
                        {
                            int thisRowIndex = cell.Row.Index;
                            int prevRowIndex = thisRowIndex - 1;

                            object[] prevRowObject = new object[7];

                            //save the next row
                            prevRowObject[0] = thisRowIndex;
                            prevRowObject[1] = grdSelectedCounters.Rows[prevRowIndex].Cells["Graph Title"].Value.ToString();
                            prevRowObject[2] = grdSelectedCounters.Rows[prevRowIndex].Cells["Counter"].Value.ToString();
                            prevRowObject[3] = grdSelectedCounters.Rows[prevRowIndex].Cells["Aggregation"].Value.ToString();
                            prevRowObject[4] = int.Parse(grdSelectedCounters.Rows[prevRowIndex].Cells["Source"].Value.ToString());
                            prevRowObject[5] = MoveUp;
                            prevRowObject[6] = MoveDown;

                            object[] thisRowObject = new object[7];

                            //save the next row
                            thisRowObject[0] = prevRowIndex;
                            thisRowObject[1] = grdSelectedCounters.Rows[thisRowIndex].Cells["Graph Title"].Value.ToString();
                            thisRowObject[2] = grdSelectedCounters.Rows[thisRowIndex].Cells["Counter"].Value.ToString();
                            thisRowObject[3] = grdSelectedCounters.Rows[thisRowIndex].Cells["Aggregation"].Value.ToString();
                            thisRowObject[4] = int.Parse(grdSelectedCounters.Rows[thisRowIndex].Cells["Source"].Value.ToString());
                            thisRowObject[5] = MoveUp;
                            thisRowObject[6] = MoveDown;


                            AddedCounters.BeginLoadData();

                            //insert the row with this rows old graph number
                            AddedCounters.LoadDataRow(thisRowObject, true);
                            AddedCounters.LoadDataRow(prevRowObject, true);


                            AddedCounters.EndLoadData();

                            AddedCounters.DefaultView.Sort = "GraphNumber asc";

                            grdSelectedCounters.Refresh();
                            setTooltipsOnArrows(grdSelectedCounters);

                        }
                    }

                    if (cell.Column.Key == "MoveDown")
                    {
                        //is there a row below?
                        if (cell.Row.Index < grdSelectedCounters.Rows.Count - 1)
                        {
                            int thisRowIndex = cell.Row.Index;
                            int nextRowIndex = thisRowIndex + 1;

                            object[] nextRowObject = new object[7];

                            //save the next row
                            nextRowObject[0] = thisRowIndex;
                            nextRowObject[1] = grdSelectedCounters.Rows[nextRowIndex].Cells["Graph Title"].Value.ToString();
                            nextRowObject[2] = grdSelectedCounters.Rows[nextRowIndex].Cells["Counter"].Value.ToString();
                            nextRowObject[3] = grdSelectedCounters.Rows[nextRowIndex].Cells["Aggregation"].Value.ToString();
                            nextRowObject[4] = int.Parse(grdSelectedCounters.Rows[nextRowIndex].Cells["Source"].Value.ToString());
                            nextRowObject[5] = MoveUp;
                            nextRowObject[6] = MoveDown;

                            object[] thisRowObject = new object[7];

                            //save the next row
                            thisRowObject[0] = nextRowIndex;
                            thisRowObject[1] = grdSelectedCounters.Rows[thisRowIndex].Cells["Graph Title"].Value.ToString();
                            thisRowObject[2] = grdSelectedCounters.Rows[thisRowIndex].Cells["Counter"].Value.ToString();
                            thisRowObject[3] = grdSelectedCounters.Rows[thisRowIndex].Cells["Aggregation"].Value.ToString();
                            thisRowObject[4] = int.Parse(grdSelectedCounters.Rows[thisRowIndex].Cells["Source"].Value.ToString());
                            thisRowObject[5] = MoveUp;
                            thisRowObject[6] = MoveDown;


                            AddedCounters.BeginLoadData();

                            //insert the row with this rows old graph number
                            AddedCounters.LoadDataRow(thisRowObject, true);
                            AddedCounters.LoadDataRow(nextRowObject, true);
                            AddedCounters.EndLoadData();

                            AddedCounters.DefaultView.Sort = "GraphNumber asc";

                            grdSelectedCounters.Refresh();
                            setTooltipsOnArrows(grdSelectedCounters);
                        }
                    }
                }
            }
        }

        #endregion
        #region wizard events

        private void newReportPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {

            bool blnFoundReport = false;
            int index = -1;
            string reportName = txtReportName.Text.TrimEnd();

            if (lstReports.SelectedItem == null || reportName.Length > 0)
            {
                //if (txtReportName.Text.TrimEnd().Length > 0) return;

                foreach (object item in lstReports.Items)
                {
                    if (((string)item).Equals(reportName))
                    {
                        blnFoundReport = true;
                        continue;
                    }
                }
                if (!blnFoundReport)
                {
                    if (!CustomReportHelper.ReportNameIsValid(reportName))
                    {
                        ApplicationMessageBox.ShowError(Parent, INVALIDFILENAME);
                        e.Cancel = true;
                        return;
                    }
                    lstReports.Items.Add(txtReportName.Text.TrimEnd());
                    index = lstReports.Items.IndexOf(txtReportName.Text.TrimEnd());
                    lstReports.SelectedIndex = index;
                }
                else
                {
                    index = lstReports.Items.IndexOf(txtReportName.Text.TrimEnd());
                    lstReports.SelectedIndex = index;
                }
            }

            //if no report selected then exit
            if (lstReports.SelectedItem != null)
            {
                reportName = lstReports.SelectedItem.ToString();

                if (!AddNewReport(reportName))
                {
                    newReportPage.AllowMoveNext = false;
                    e.Cancel = true;
                    return;
                }
            }

            PrepareSelectCountersPage(_CurrentCustomReport);
        }

        /// <summary>
        /// Populate the initial report selection page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newReportPage_BeforeDisplay(object sender, EventArgs e)
        {
            int selectedIdx;

            ICollection<CustomReport> reports = RepositoryHelper.GetCustomReportsList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            lstReports.Items.Clear();

            if (_CurrentCustomReport != null)
            {
                foreach (CustomReport report in reports)
                {
                    lstReports.Items.Add(report.Name);
                    if (report.Name == _CurrentCustomReport.Name)
                    {
                        txtReportName.Text = report.Name;
                    }
                }
                selectedIdx = lstReports.Items.IndexOf(_CurrentCustomReport.Name);
                if (selectedIdx == -1)
                {
                    lstReports.Items.Add(_CurrentCustomReport.Name);
                    selectedIdx = lstReports.Items.IndexOf(_CurrentCustomReport.Name);
                }
                lstReports.SelectedIndex = selectedIdx;
            }
            else
            {
                foreach (CustomReport report in reports)
                {
                    lstReports.Items.Add(report.Name);
                }
            }
            UpdateSelectReportPage();

        }

        private void selectCounterPage_BeforeDisplay(object sender, EventArgs e)
        {
            PrepareSelectCountersPage(_CurrentCustomReport);
        }

        private void selectCounterPage_AfterDisplay(object sender, EventArgs e)
        {
            _CurrentCustomReport.ShowTable = chkShowTabularData.Checked;
            _CurrentCustomReport.ShowTopServers = chkShowTopServersData.Checked;

            UpdateSelectedCountersPage();
        }

        /// <summary>
        /// Set up the counter aggregation page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void counterAggregationPage_BeforeDisplay(object sender, EventArgs e)
        {
            _CurrentCustomReport.ShowTable = chkShowTabularData.Checked;
            _CurrentCustomReport.ShowTopServers = chkShowTopServersData.Checked;

            AddedCounters = new DataTable("AddedCounters");

            AddedCounters.Columns.Add("GraphNumber", typeof(int));
            AddedCounters.Columns.Add("Graph Title", typeof(string));
            AddedCounters.Columns.Add("Counter", typeof(string));
            AddedCounters.Columns.Add("Aggregation", typeof(string));
            AddedCounters.Columns.Add("Source", typeof(int));
            AddedCounters.Columns.Add("MoveUp", typeof(Image));
            AddedCounters.Columns.Add("MoveDown", typeof(Image));

            grdSelectedCounters.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
            grdSelectedCounters.DataSource = AddedCounters;

            grdSelectedCounters.DisplayLayout.Bands[0].Columns["GraphNumber"].Hidden = true;
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Counter"].Hidden = true;

            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Graph Title"].CellActivation = Activation.NoEdit;
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Source"].CellActivation = Activation.NoEdit;
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["MoveUp"].CellActivation = Activation.ActivateOnly;
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["MoveDown"].CellActivation = Activation.ActivateOnly;

            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Graph Title"].SortIndicator = SortIndicator.Disabled;
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Source"].SortIndicator = SortIndicator.Disabled;
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Aggregation"].SortIndicator = SortIndicator.Disabled;
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["MoveUp"].SortIndicator = SortIndicator.Disabled;
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["MoveDown"].SortIndicator = SortIndicator.Disabled;

            AddedCounters.Clear();

            AddedCounters.PrimaryKey = new DataColumn[] { AddedCounters.Columns[0] };

            // Assign the "Aggregation" value list to "Aggregation" column.
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Aggregation"].ValueList =
                grdSelectedCounters.DisplayLayout.ValueLists["Aggregation"];
            // DropDownValidate style ensures that the user enters only the values found 
            // in the value list. 
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Aggregation"].Style =
                Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            // Assign the "Source" value list to "Source" column.
            grdSelectedCounters.DisplayLayout.Bands[0].Columns["Source"].ValueList =
                grdSelectedCounters.DisplayLayout.ValueLists["Source"];

            int graphNo = 0;

            foreach (CustomReportMetric metric in _CurrentCustomReport.Metrics.Values)
            {
                object[] rowobject = new object[7];

                rowobject[0] = graphNo++;
                rowobject[1] = metric.MetricDescription;
                //if this is a custom counter the spaces in the name have been replaced by underscores so use the description which is the name(sans underscores) for custom
                //rowobject[2] = intCounterType == 2 ? previouslySelected[i]["CounterShortDescription"] : previouslySelected[i]["CounterName"];
                rowobject[2] = metric.MetricName;                
                //START: SQLdm 10.0 (Tarun Sapra)- For displaying the correct tag for the aggregation type
                switch ((int)metric.Aggregation)
                {
                    case 0: rowobject[3] = "Max"; break;
                    case 1: rowobject[3] = "Weighted Avg"; break;
                    case 2: rowobject[3] = "PerMinute Avg"; break;
                }
                //rowobject[3] = CustomReport.Aggregation.GetName(typeof(CustomReport.Aggregation),(int)metric.Aggregation);
                //END: SQLdm 10.0 (Tarun Sapra)- For displaying the correct tag for the aggregation type
                rowobject[4] = (int)metric.Source;
                rowobject[5] = MoveUp;
                rowobject[6] = MoveDown;

                AddedCounters.LoadDataRow(rowobject, true);
            }

            /*List<string> newlyAddedCounters = new List<string>();

            for (int i = 0; i < lstAddedCounters.Items.Count; i++)
            {
                newlyAddedCounters.Add(lstAddedCounters.Items[i].ToString());
            }

            //Go through the list of added counters
            if (lstAddedCounters.Items.Count > 0)
            {
                AddedCounters.BeginLoadData();

                int graphNo = 0;
                string previouslyAddedCounterString = "";
                string newlyAddedCounterString = "";

                //for every added counter
                foreach (object addedCounter in lstAddedCounters.Items)
                {
                    previouslyAddedCounterString += "'" + addedCounter + "', ";
                }

                if (previouslyAddedCounterString.TrimEnd().Length == 0)
                {
                    return;
                }

                previouslyAddedCounterString = previouslyAddedCounterString.TrimEnd(new char[] { ',', ' ' });

                DataRow[] previouslySelected =
                    _selectedCountersDataTable.Select("CounterShortDescription in (" + previouslyAddedCounterString + ")");

                //Load all the previously selected counters
                for (int i = 0; i < previouslySelected.Length; i++)
                {
                    object[] rowobject = new object[7];
                    int intCounterType = 0;
                    int.TryParse(previouslySelected[i]["CounterType"].ToString(), out intCounterType);

                    rowobject[0] = graphNo++;
                    rowobject[1] = previouslySelected[i]["CounterShortDescription"];
                    //if this is a custom counter the spaces in the name have been replaced by underscores so use the description which is the name(sans underscores) for custom
                    //rowobject[2] = intCounterType == 2 ? previouslySelected[i]["CounterShortDescription"] : previouslySelected[i]["CounterName"];
                    rowobject[2] = previouslySelected[i]["CounterName"];
                    rowobject[3] = int.Parse(previouslySelected[i]["Aggregation"].ToString());
                    rowobject[4] = intCounterType;
                    rowobject[5] = MoveUp;
                    rowobject[6] = MoveDown;

                    AddedCounters.LoadDataRow(rowobject, true);

                    //remove from the newly added list because this was here already
                    newlyAddedCounters.Remove(previouslySelected[i]["CounterShortDescription"].ToString());
                }

                //for every added counter
                for (int i = 0; i < newlyAddedCounters.Count; i++)
                {
                    newlyAddedCounterString += "'" + newlyAddedCounters[i] + "', ";
                }

                newlyAddedCounterString = newlyAddedCounterString.TrimEnd(new char[] { ',', ' ' });

                if (newlyAddedCounterString.TrimEnd().Length > 0)
                {

                    DataRow[] newlyAdded =
                        _availableCountersDataTable.Select("CounterFriendlyName in (" + newlyAddedCounterString + ")");

                    //Load all the previously selected counters
                    for (int i = 0; i < newlyAdded.Length; i++)
                    {
                        object[] rowobject = new object[7];

                        rowobject[0] = graphNo++;
                        rowobject[1] = newlyAdded[i]["CounterFriendlyName"];
                        rowobject[2] = newlyAdded[i]["CounterName"];
                        rowobject[3] = (int)CustomReport.Aggregation.AverageSinceLastCollection;
                        rowobject[4] = newlyAdded[i]["Source"];
                        rowobject[5] = MoveUp;
                        rowobject[6] = MoveDown;

                        AddedCounters.LoadDataRow(rowobject, true);
                    }
                }*/
            AddedCounters.EndLoadData();
            AddedCounters.DefaultView.Sort = "GraphNumber asc";
            //}

            if (AddedCounters.Rows.Count > 0) counterAggregationPage.AllowMoveNext = true;
            setTooltipsOnArrows(grdSelectedCounters);
        }


        private void counterAggregationPage_BeforeMoveNext(object sender, CancelEventArgs e)
        {
            UpdateCustomReportCounters(_CurrentCustomReport);
        }

        private void counterAggregationPage_BeforeMoveBack(object sender, CancelEventArgs e)
        {
            UpdateCustomReportCounters(_CurrentCustomReport);
        }

        /// <summary>
        /// Prepare the final summary page for display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void finishPage1_BeforeDisplay(object sender, EventArgs e)
        {
            _CurrentCustomReport.ShowTable = chkShowTabularData.Checked;
            _CurrentCustomReport.ShowTopServers = chkShowTopServersData.Checked;

            // Get the selected counters from the repository.
            string completedMessage = "\n\n";
            string aggregationExplanation = "";
            foreach (KeyValuePair<int, CustomReportMetric> metric in _CurrentCustomReport.Metrics)
            {
                switch (metric.Value.Aggregation)
                {
                    case CustomReport.Aggregation.AverageSinceLastCollection:
                        aggregationExplanation = "(Average in the selected interval)";
                        break;
                    case CustomReport.Aggregation.MaxSinceLastCollection:
                        aggregationExplanation = "(Maximum in the selected interval)";
                        break;
                    case CustomReport.Aggregation.PerMinuteSinceLastCollection:
                        aggregationExplanation = "(Average per minute in the selected interval)";
                        break;
                }
                completedMessage = completedMessage + "• " + metric.Value.MetricDescription + " " + aggregationExplanation + "\n\n";
            }

            lblReportWizardSummary.Text = completedMessage;
        }

        private void wizard_Finish(object sender, EventArgs args)
        {
            if (_CurrentCustomReport.ShowTopServers)
            {
                FinalizeCustomReportTopServer();
            }
            else
            {
                FinalizeCustomReport();
            }
        }

        #endregion
        #region Methods
        /// <summary>
        /// Save the counters to the dictionary of report metrics
        /// </summary>
        /// <param name="report"></param>
        private void UpdateCustomReportCounters(CustomReport report)
        {
            //_selectedCountersDataTable.Clear();
            report.Metrics.Clear();

            //add only the selected counters
            for (int i = 0; i < grdSelectedCounters.Rows.Count; i++)
            {
                string CounterName = grdSelectedCounters.Rows[i].Cells["Counter"].Value.ToString();
                string CounterFriendlyName = grdSelectedCounters.Rows[i].Cells["Graph Title"].Value.ToString();

                int Aggregation=1;
                int Source;
                //START: SQLdm 10.0 (Tarun Sapra)- Displaying a representative string for aggregation type on the UI, and we are parsing it here
                string aggStringFromUI = grdSelectedCounters.Rows[i].Cells["Aggregation"].Value.ToString();
                switch (aggStringFromUI)
                {
                    case "Max": Aggregation = (int)CustomReport.Aggregation.MaxSinceLastCollection; break;
                    case "Weighted Avg": Aggregation = (int)CustomReport.Aggregation.AverageSinceLastCollection; break;
                    case "PerMinute Avg": Aggregation = (int)CustomReport.Aggregation.PerMinuteSinceLastCollection; break;
                }                
                //int.TryParse(grdSelectedCounters.Rows[i].Cells["Aggregation"].Value.ToString(), out Aggregation);
                //END: SQLdm 10.0 (Tarun Sapra)- Displaying a representative string for aggregation type on the UI, and we are parsing it here
                int.TryParse(grdSelectedCounters.Rows[i].Cells["Source"].Value.ToString(), out Source);

                CustomReportMetric metric = new CustomReportMetric(CounterName, CounterFriendlyName, (CustomReport.CounterType)Source, (CustomReport.Aggregation)Aggregation);
                report.Metrics.Add(i, metric);
            }
        }
        /// <summary>
        ///  Create new report. Or find the matching existing report
        /// </summary>
        /// <param name="reportName"></param>
        /// <returns></returns>
        private bool AddNewReport(string reportName)
        {
            try
            {
                if (reportName.Length == 0)
                {
                    ApplicationMessageBox.ShowInfo(Parent, "Please enter a name for this report");
                    return false;
                }

                if (!CustomReportHelper.ReportNameIsValid(reportName))
                {
                    ApplicationMessageBox.ShowError(Parent, INVALIDFILENAME);

                    newReportPage.AllowMoveNext = false;
                    return false;
                }

                if (ApplicationModel.Default.CustomReports.ContainsKey(reportName))
                {
                    //if this report has already got a populated collection of metrics
                    if (_CurrentCustomReport == null || !(_CurrentCustomReport.Name == reportName))
                    {
                        _CurrentCustomReport = RepositoryHelper.GetCustomReport(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                    reportName);
                       
                        _CurrentCustomReport.ReportRDL = GetCustomReportXML().Replace("{Custom Report Title}", reportName);
                        

                        //includes aggregation that may have been set up previously
                        _selectedCountersDataTable = RepositoryHelper.GetSelectedCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, _CurrentCustomReport.Name);

                        //populate the counters that have already been selected
                        PopulateCustomReportMetrics(_selectedCountersDataTable, _CurrentCustomReport);
                    }

                    wizard.SelectedPage = selectCounterPage;
                    return true;
                }

                //if this report has already got a populated collection of metrics
                if (_CurrentCustomReport == null || !(_CurrentCustomReport.Metrics.Count > 0 && _CurrentCustomReport.Name == reportName))
                {
                    _CurrentCustomReport = new CustomReport(reportName);
                    //Fetch the template report information from CustomReport.RDL
                    _CurrentCustomReport.ReportRDL = GetCustomReportXML().Replace("{Custom Report Title}", reportName);

                    lstReports.Items.Add(reportName);
                }

                newReportPage.AllowMoveNext = true;
                lstReports.SelectedItem = lstReports.Items[lstReports.Items.IndexOf(reportName)];
                return true;
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(Parent, ex.Message);
            }
            return false;
        }

       

        private static void setTooltipsOnArrows(UltraGridBase grid)
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Cells == null) continue;

                row.Cells["MoveUp"].ToolTipText = row.Index == 0 ? "" : "Move this row up";
                row.Cells["MoveDown"].ToolTipText = row.Index == grid.Rows.Count - 1 ? "" : "Move this row down";

            }
        }

        /// <summary>
        /// Prepare the selection page with all counters that are available and that have 
        /// already been selected for this report
        /// </summary>
        private void PrepareSelectCountersPage(CustomReport report)
        {
            chkShowTabularData.Checked = report.ShowTable;
            chkShowGraphicalData.Checked = !report.ShowTopServers;
            chkShowTopServersData.Checked = report.ShowTopServers;
            chkShowTabularData.Enabled = chkShowGraphicalData.Checked;
            //get all counters that we could report on
            //only returns name, friendly name and source
            _availableCountersDataTable = RepositoryHelper.GetAvailableCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            ////includes aggregation that may have been set up previously
            //_selectedCountersDataTable = RepositoryHelper.GetSelectedCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, _CurrentCustomReport.Name);

            ////populate the counters that have already been selected
            PopulateSelectedCounters(report);

            //Populate the counters that are availble to be selected but have not been selected
            PopulateAvailableCounters(_availableCountersDataTable, report);

            UpdateSelectedCountersPage();

            fmeReport.Text = "Report Name: " + report.Name;
        }

        /// <summary>
        /// Take the selected counters and add them to the custom report object
        /// </summary>
        private void FinalizeCustomReport()
        {
            //int? previousServerID = null;

            //build the dataset block of the report
            string dataset = _CurrentCustomReport.GetCounterSummaryDataSet();

            string tableRDL = _CurrentCustomReport.GetCustomReportsTable();
            string chartsRDL = _CurrentCustomReport.GetCustomReportCharts();
            string showTableRDL = _CurrentCustomReport.GetShowTablesParameter(_CurrentCustomReport.ShowTable);

            string reportText = _CurrentCustomReport.ReportRDL;
            StringBuilder sb = new StringBuilder(reportText);

            //insert dataset block
            int startReplace = reportText.IndexOf("<DataSet Name=\"CounterSummary\">");
            if (startReplace <= 0) return;
            int endReplace = reportText.IndexOf("</DataSet>", startReplace) + 10;
            if (endReplace <= 0) return;
            sb.Remove(startReplace, endReplace - startReplace);
            sb.Insert(startReplace, dataset);
            reportText = sb.ToString();

            //Insert table block in the body of the report
            startReplace = reportText.IndexOf("<Table Name=\"table1\">");
            if (startReplace <= 0) return;

            endReplace = reportText.IndexOf("</Table>", startReplace) + 8;
            if (endReplace <= 0) return;

            sb.Remove(startReplace, endReplace - startReplace);
            sb.Insert(startReplace, tableRDL);
            reportText = sb.ToString();

            //Insert chart block
            startReplace = reportText.IndexOf("<Table Name=\"table2\">");
            if (startReplace <= 0) return;

            endReplace = reportText.IndexOf("</Table>", startReplace) + 8;
            if (endReplace <= 0) return;

            startReplace = endReplace + 1;
            endReplace = reportText.IndexOf("<Table Name=\"table1\">", startReplace) - 1;
            sb.Remove(startReplace, endReplace - startReplace);
            sb.Insert(startReplace, chartsRDL);
            reportText = sb.ToString();

            //Insert new DisplayTables parameter
            startReplace = reportText.IndexOf("<ReportParameter Name=\"DisplayTables\">");
            if (startReplace <= 0) return;

            endReplace = reportText.IndexOf("</ReportParameter>", startReplace) + 18;
            if (endReplace <= 0) return;

            sb.Remove(startReplace, endReplace - startReplace);
            sb.Insert(startReplace, showTableRDL);
            reportText = sb.ToString();

            Serialized<string> serialReportText = new Serialized<string>(reportText, true);

            _CurrentCustomReport.ReportRDL = serialReportText;

            ApplicationModel.Default.AddOrUpdateCustomReport(_CurrentCustomReport);

            //delete all counters for this report
            managementService.DeleteCustomReportCounters(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                _CurrentCustomReport.Name);

            UpdateExistingReport(_CurrentCustomReport);

            //if (ApplicationController.Default.ActiveView is ReportsView)
            //{
            //    ReportsView previousReportView = ApplicationController.Default.ActiveView as ReportsView;
            //    if (previousReportView.ActiveReport != null)
            //    {
            //        if (previousReportView.ActiveReport.currentServer != null)
            //        {
            //            previousServerID = previousReportView.ActiveReport.currentServer.Id;
            //        }
            //    }
            //}
            ApplicationController.Default.ShowReportsView(ReportTypes.Custom, null, _CurrentCustomReport.Name, null);
            //ApplicationController.Default.ShowReportsView(ReportTypes.Custom);
        }

        //START: SQLdm 10.0 (Tarun Sapra)- Dynamically changing the rdl as per the selected counters
        /// <summary>
        /// Take the selected counters and add them to the top servers custom report object
        /// </summary>
        private void FinalizeCustomReportTopServer()
        {   
            string tablesRDL = _CurrentCustomReport.GetTopSeversCustomReportTables();
            string reportText = GetTopServersReportXML();
            reportText = System.Text.RegularExpressions.Regex.Replace(reportText, "{Custom Report Title}", _CurrentCustomReport.Name, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();            
            reportText = System.Text.RegularExpressions.Regex.Replace(reportText, "{Tables For Custom Counters}", tablesRDL, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
            Serialized<string> serialReportText = new Serialized<string>(reportText, true);

#if(DEBUG)
            UTF8Encoding encoding = new UTF8Encoding();

            FileStream fs = new FileStream(string.Format("c:\\LastTopServersCustomReportGenerated{0}.rdl", DateTime.Now.ToString("hhmmss")), FileMode.Create);
            fs.Write(encoding.GetBytes(reportText), 0, encoding.GetBytes(reportText).Length);
            fs.Flush();
            fs.Close();
#endif
            _CurrentCustomReport.ReportRDL = serialReportText;

            ApplicationModel.Default.AddOrUpdateCustomReport(_CurrentCustomReport);

            //delete all counters for this report
            managementService.DeleteCustomReportCounters(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                _CurrentCustomReport.Name);

            UpdateExistingReport(_CurrentCustomReport);
            
            ApplicationController.Default.ShowReportsView(ReportTypes.Custom, null, _CurrentCustomReport.Name, null);
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Dynamically changing the rdl as per the selected counters

        private string GetHelpTopic()
        {
            string topic = HelpTopics.CustomReportsWizard;

            if (wizard.SelectedPage == newReportPage)
            {
                topic = HelpTopics.CustomReportsWizardSelectReport;
            }
            else if (wizard.SelectedPage == selectCounterPage)
            {
                topic = HelpTopics.CustomReportsWizardSelectCounters;
            }
            else if (wizard.SelectedPage == counterAggregationPage)
            {
                topic = HelpTopics.CustomReportsWizardAggregateCounters;
            }

            return topic;
        }


        /// <summary>
        /// Delete the report with the specified name
        /// </summary>
        /// <param name="ReportName"></param>
        /// <returns></returns>
        public int DeleteReport(string ReportName)
        {
            try
            {
                //delete report
                return managementService.UpdateCustomReport(CustomReport.Operation.Delete,
                    null,
                    ReportName,
                    null,
                    null, false);
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this.Parent, ex.Message.ToString());
            }
            return -1;
        }

        /// <summary>
        /// Write all of the report metrics to the database
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        private bool UpdateExistingReport(CustomReport report)
        {
            try
            {
                int rowNumber = 0;
                foreach (CustomReportMetric metric in report.Metrics.Values)
                {
                    managementService.InsertCounterToGraph(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
                        report.Name,
                        rowNumber,
                        metric.MetricName,
                        metric.MetricDescription,
                        (int)metric.Aggregation,
                        (int)metric.Source);
                    rowNumber++;
                }


                return true;
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(Parent, ex.Message);
            }
            return false;
        }

        ///// <summary>
        ///// Commit the selected data to the repository to form the foundation of the custom report
        ///// </summary>
        ///// <param name="rowNumber"></param>
        ///// <param name="row"></param>
        ///// <param name="UpdateRepository"></param>
        //private bool UpdateExistingReport(int rowNumber, UltraGridRow row, bool UpdateRepository)
        //{
        //    try
        //    {
        //        string CounterName = row.Cells["Counter"].Value.ToString();
        //        string CounterFriendlyName = row.Cells["Graph Title"].Value.ToString();

        //        int Aggregation;
        //        int.TryParse(row.Cells["Aggregation"].Value.ToString(), out Aggregation);
        //        int Source;
        //        int.TryParse(row.Cells["Source"].Value.ToString(), out Source);

        //        if (UpdateRepository)
        //        {
        //            managementService.InsertCounterToGraph(
        //                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString,
        //                _CurrentCustomReport.Name,
        //                rowNumber,
        //                CounterName,
        //                CounterFriendlyName,
        //                Aggregation,
        //                Source);
        //        }
        //        else
        //        {
        //            CustomReportMetric metric = new CustomReportMetric(CounterName,CounterFriendlyName,(CustomReport.CounterType)Source, (CustomReport.Aggregation)Aggregation);
        //            _CurrentCustomReport.Metrics.Add(rowNumber, metric);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationMessageBox.ShowError(this.Parent, ex.Message.ToString());
        //    }
        //    return false;
        //}

        /// <summary>
        /// Fetch the rdl of the base custom report
        /// </summary>
        /// <returns>a string containing the full</returns>
        private string GetTopServersReportXML()
        {
            String rdlString = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.CustomReportTopServers.rdl";
            
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(rdlString))
            {
                if (stream == null) return null;
                using (StreamReader reader = new StreamReader(stream))
                {
                    try
                    {
                        return reader.ReadToEnd();
                    }
                    catch(Exception ex)
                    {
                        ApplicationMessageBox.ShowError(Parent, ex.Message);
                    }
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Fetch the rdl of the base custom report
        /// </summary>
        /// <returns>a string containing the full</returns>
        private string GetCustomReportXML()
        {
            String rdlString = "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.CustomReport.rdl";
            
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(rdlString))
            {
                if (stream == null) return null;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Enable movenext if a report has been selected
        /// </summary>
        private void UpdateSelectReportPage()
        {
            if (lstReports.Items.Count <= 0)
                newReportPage.AllowMoveNext = txtReportName.Text.Length > 0 ? true : false;
            else
                newReportPage.AllowMoveNext = lstReports.SelectedItems.Count > 0 ? true : false;
            //for (int i = 0; i < lstReports.SelectedItems.Count; i++)
            //{
            //    txtReportName.Text = lstReports.SelectedItems[i].ToString();
            //    return;
            //}
            // based on the commented out code above, the lines below do the same thing... 
            if (lstReports.SelectedItems.Count > 0)
                txtReportName.Text = lstReports.SelectedItems[0].ToString();
            return;

        }

        /// <summary>
        /// Add the metrics to the custom report object from the repository
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="report"></param>
        private void PopulateCustomReportMetrics(DataTable selected, CustomReport report)
        {
            //_selectedCounters.Clear();
            if (report.Metrics == null) report.Metrics = new SortedDictionary<int, CustomReportMetric>();

            report.Metrics.Clear();
            #region header

            ////selectedCounterDataSet.Columns.Add("ID", typeof(string)).AllowDBNull = false;
            ////selectedCounterDataSet.Columns.Add("GraphNumber", typeof(string)).AllowDBNull = false;
            ////selectedCounterDataSet.Columns.Add("CounterShortDescription", typeof(string)).AllowDBNull = false;
            ////selectedCounterDataSet.Columns.Add("CounterName", typeof(string)).AllowDBNull = false;
            ////selectedCounterDataSet.Columns.Add("Aggregation", typeof(string)).AllowDBNull = false;
            ////selectedCounterDataSet.Columns.Add("reportName", typeof(string)).AllowDBNull = false;
            ////selectedCounterDataSet.Columns.Add("CounterType", typeof(string)).AllowDBNull = false;
            ////selectedCounterDataSet.Columns.Add("reportShortDescription", typeof(string)).AllowDBNull = true;

            #endregion
            foreach (DataRow row in selected.Rows)
            {
                string metricName = row["CounterName"].ToString();
                string metricDescription = row["CounterShortDescription"].ToString();

                //_selectedCounters.Add(metricName, metricDescription);

                CustomReport.CounterType type = (CustomReport.CounterType)int.Parse(row["CounterType"].ToString());
                CustomReport.Aggregation aggregation = (CustomReport.Aggregation)int.Parse(row["Aggregation"].ToString());

                report.Metrics.Add(int.Parse(row["GraphNumber"].ToString()),
                            new CustomReportMetric(metricName, metricDescription, type, aggregation));

            }
        }

        /// <summary>
        /// Add the friendly names of selected counters into the selected list
        /// </summary>
        /// <param name="report"></param>
        private void PopulateSelectedCounters(CustomReport report)
        {
            //_selectedCounters.Clear();
            lstAddedCounters.Items.Clear();

            foreach (CustomReportMetric metric in report.Metrics.Values)
            {
                //_selectedCounters.Add(row["CounterName"].ToString(), row["CounterShortDescription"].ToString());
                lstAddedCounters.Items.Add(metric.MetricDescription);
            }
        }

        /// <summary>
        /// Populate the available counters grid with metric objects
        /// </summary>
        /// <param name="available"></param>
        /// <param name="report"></param>
        public void PopulateAvailableCounters(DataTable available, CustomReport report)
        {
            string selectedString = String.Empty;

            if (report.Metrics.Count > 0)
            {
                selectedString = report.Metrics.Aggregate(selectedString,
                    (current, kvp) => current + ("'" + kvp.Value.MetricName + "',"));

                selectedString = selectedString.TrimEnd(',');
            }

            // True if at least one counter has checked.
            bool leastOneCounterChecked = chkSQLCounters.Checked || chkOSCounters.Checked || chkVirtCounters.Checked || chkCustomCounters.Checked; ////SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - Added chkVirtCounters

            if (leastOneCounterChecked)
            {
                lstAvailableCounters.Items.Clear();

                string strGroup = "(";

                if (chkSQLCounters.Checked) strGroup = strGroup + ((int)CustomReport.CounterType.Server);

                if (chkOSCounters.Checked) strGroup = strGroup + (strGroup.Length > 1 ? "," : String.Empty) + ((int)CustomReport.CounterType.OS);
                //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
                if (chkVirtCounters.Checked) strGroup = strGroup + (strGroup.Length > 1 ? "," : String.Empty) + ((int)CustomReport.CounterType.Virtualization);
                //SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here
                if (chkCustomCounters.Checked) strGroup = strGroup + (strGroup.Length > 1 ? "," : String.Empty) + ((int)CustomReport.CounterType.Custom);

                if ("(".Equals(strGroup) || _availableCountersDataTable == null)
                {
                    // Displays the empty counters notification panel.
                    ShowEmptyCountersNotificationPanel();
                }
                else
                {
                    // Get available counters.
                    DataRow[] found = available.Select("Source in " + strGroup + ")" + ((selectedString.Length > 0) ? " and CounterName not in (" + selectedString + ")" : ""));

                    // Populates the available counters.
                    HideEmptyCountersNotificationPanel();

                    foreach (DataRow row in found)
                    {
                        String metricName = row["CounterName"].ToString();
                        String metricDescription = row["CounterFriendlyName"].ToString();
                        var customReport = (CustomReport.CounterType)int.Parse(row["Source"].ToString());
                        var metric = new CustomReportMetric(metricName, metricDescription, customReport,
                            CustomReport.Aggregation.AverageSinceLastCollection);

                        lstAvailableCounters.Items.Add(metric);
                    }
                }
            }
            else
            {
                ShowEmptyCountersNotificationPanel();
            }
        }

        /// <summary>
        /// Displays a panel to notify that not have available counters.
        /// </summary>
        private void ShowEmptyCountersNotificationPanel()
        {
            // Bring to front the 'empty items' notification panel.
            lblAvailableCounters.BringToFront();
            lblAvailableCounters.Visible = true;

            // Hide list available counters.
            lstAvailableCounters.SendToBack();
            lstAvailableCounters.Visible = false;
        }

        /// <summary>
        /// Hide the not available counters panel, and display the list of available counters.
        /// </summary>
        private void HideEmptyCountersNotificationPanel()
        {
            // Hide 'empty items' notification panel.
            lblAvailableCounters.SendToBack();
            lblAvailableCounters.Visible = false;

            // Enable list available counters.
            lstAvailableCounters.BringToFront();
            lstAvailableCounters.Visible = true;
        }

        /// <summary>
        /// Enable the buttons depending on the number of selected items
        /// </summary>
        private void UpdateSelectedCountersPage()
        {
            btnAddCounters.Enabled = false;
            btnRemoveCounters.Enabled = false;

            selectCounterPage.AllowMoveNext = lstAddedCounters.Items.Count > 0 ? true : false;

            if (lstAvailableCounters.SelectedItems.Count > 0) btnAddCounters.Enabled = true;
            if (lstAddedCounters.SelectedItems.Count > 0) btnRemoveCounters.Enabled = true;
        }

        /// <summary>
        /// Take all selected counters from the available list and add them to the selected list
        /// </summary>
        private void AddSelectedCounters(CustomReport report)
        {
            if ((lstAvailableCounters.SelectedItems.Count + lstAddedCounters.Items.Count) > MAXNUMBEROFMETRICS)
            {
                ApplicationMessageBox.ShowMessage(Parent, string.Format(SELECTEDMETRICCOUNTEXCEEDED, MAXNUMBEROFMETRICS));

                return;
            }

            List<int> addedIndices = new List<int>();

            foreach (int selectedIndex in lstAvailableCounters.SelectedIndices)
            {
                string selectedInstance = lstAvailableCounters.Items[selectedIndex].ToString();

                if (selectedInstance == null) continue;

                addedIndices.Insert(0, selectedIndex);
                CustomReportMetric metric = (CustomReportMetric)lstAvailableCounters.Items[selectedIndex];

                ReindexMetrics(report);

                report.Metrics.Add(report.Metrics.Count, metric);

                lstAddedCounters.Items.Add(selectedInstance);
            }

            //remove selected items from the available list
            foreach (int addedIndex in addedIndices)
            {
                lstAvailableCounters.Items.RemoveAt(addedIndex);
            }
        }

        /// <summary>
        /// Reindex the metrics in prepearation for ordering on the aggregation screen
        /// </summary>
        /// <param name="report"></param>
        private static void ReindexMetrics(CustomReport report)
        {
            int index = 0;
            List<CustomReportMetric> metrics = new List<CustomReportMetric>();

            foreach (KeyValuePair<int, CustomReportMetric> kvp in report.Metrics)
                metrics.Add(kvp.Value);

            report.Metrics.Clear();

            for (index = 0; index < metrics.Count; index++)
            {
                report.Metrics.Add(index, metrics[index]);
            }
        }

        /// <summary>
        /// Remove deselected counters from the selected list and put them back into the available list
        /// </summary>
        private void RemoveSelectedCounters(CustomReport report)
        {
            List<int> removedIndices = new List<int>();

            foreach (int selectedIndex in lstAddedCounters.SelectedIndices)
            {
                string selectedCounter = lstAddedCounters.Items[selectedIndex].ToString();

                if (selectedCounter == null) continue;

                removedIndices.Insert(0, selectedIndex);

                foreach (KeyValuePair<int, CustomReportMetric> kvp in report.Metrics)
                {
                    if (kvp.Value.MetricDescription.Equals(selectedCounter))
                    {
                        report.Metrics.Remove(kvp.Key);
                        break;
                    }
                }
            }

            PopulateAvailableCounters(_availableCountersDataTable, report);

            foreach (int removedIndex in removedIndices)
            {
                lstAddedCounters.Items.RemoveAt(removedIndex);
            }

            if (lstAddedCounters.Items.Count <= MAXNUMBEROFMETRICS)
            {
                lblMessage.Text = "";
            }
        }

        #endregion

        private void selectCounterPage_BeforeMoveBack(object sender, CancelEventArgs e)
        {
            int selectedIdx = -1;

            lstReports.Items.Clear();

            ICollection<CustomReport> reports = RepositoryHelper.GetCustomReportsList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            foreach (CustomReport report in reports)
            {
                lstReports.Items.Add(report.Name);
                if (report.Name == _CurrentCustomReport.Name)
                {
                    txtReportName.Text = report.Name;
                }
            }

            selectedIdx = lstReports.Items.IndexOf(_CurrentCustomReport.Name);

            if (selectedIdx == -1)
            {
                lstReports.Items.Add(_CurrentCustomReport.Name);
                selectedIdx = lstReports.Items.IndexOf(_CurrentCustomReport.Name);
            }
            lstReports.SelectedIndex = selectedIdx;

            UpdateSelectReportPage();
        }

        private void txtReportName_KeyUp(object sender, KeyEventArgs e)
        {
            newReportPage.AllowMoveNext = txtReportName.Text.TrimEnd().Length > 0 ? true : false;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.grdSelectedCounters);
        }


        private void chkShowGraphicalData_CheckedChanged(object sender, EventArgs e)
        {
            chkShowTabularData.Enabled = chkShowGraphicalData.Checked;
            if (!chkShowTabularData.Enabled)
            {
                chkShowTabularData.Checked = false;
            }
            
            this.selectCounterPage.NextPage = this.counterAggregationPage;           
        }


        #region Prototype RDL Building code
        //    public void GenerateRdl()
        //    {
        //        string CONSTRG = "Data Source=bsearlel;Initial Catalog=Northwind; Integrated Security=True";
        //        string CmdText = "";

        //    // Open a new RDL file stream for writing
        //    System.IO.FileStream stream;

        //    stream = System.IO.File.OpenWrite("C:\\RDLGen2.rdl");

        //    System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, Encoding.UTF8);

        //        writer.Flush();
        //    // Causes child elements to be indented
        //    writer.Formatting = System.Xml.Formatting.Indented;
        //    // Report element
        //    writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
        //    writer.WriteStartElement("Report");
        //    writer.WriteAttributeString("xmlns", null, "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition");
        //    writer.WriteElementString("Width", "6in");
        //    // DataSource element
        //    writer.WriteStartElement("DataSources");

        //    writer.WriteStartElement("DataSource");
        //    writer.WriteAttributeString("Name", null, "DataSource1")
        //    writer.WriteStartElement("ConnectionProperties");
        //    writer.WriteElementString("DataProvider", "SQL");
        //    writer.WriteElementString("ConnectString", CONSTRG);
        //    writer.WriteElementString("IntegratedSecurity", "true");
        //    writer.WriteEndElement(); // ConnectionProperties
        //    writer.WriteEndElement(); // DataSource

        //    writer.WriteStartElement("DataSource");
        //    writer.WriteAttributeString("Name", null, "SQL Diagnostic manager Data Source");
        //    writer.WriteElementString("rd:DataSourceID", "503bd9b5-ca63-49d6-88bc-603692b62b3b");
        //    writer.WriteElementString("DataSourceReference", "SQL Diagnostic manager Data Source");
        //    writer.WriteEndElement(); // DataSource
        //    writer.WriteEndElement(); // DataSources

        //    //DataSet element
        //    writer.WriteStartElement("DataSets");
        //    writer.WriteStartElement("DataSet");
        //    writer.WriteAttributeString("Name", null, "DataSet1");
        //    // Query element
        //    writer.WriteStartElement("Query");
        //    writer.WriteElementString("DataSourceName", "DataSource1");
        //    writer.WriteElementString("CommandType", "Text");

        //        writer.WriteElementString("CommandText", CmdText);
        //    writer.WriteElementString("Timeout", "30");
        //    writer.WriteEndElement(); // Query
        //    // Fields elements
        //    writer.WriteStartElement("Fields");
        //    string fieldName = null;

        //    For Each fieldName In Flds
        //        writer.WriteStartElement("Field");
        //        writer.WriteAttributeString("Name", Nothing, fieldName);
        //        writer.WriteElementString("DataField", Nothing, fieldName);
        //        writer.WriteEndElement(); // Field
        //    Next fieldName
        //    // End previous elements
        //    writer.WriteEndElement(); // Fields
        //    writer.WriteEndElement(); // DataSet
        //    writer.WriteEndElement(); // DataSets

        //    // Body element
        //    writer.WriteStartElement("Body");
        //    writer.WriteElementString("Height", "5in");
        //    // ReportItems element
        //    writer.WriteStartElement("ReportItems");
        //    // Table element
        //    writer.WriteStartElement("Table");
        //    writer.WriteAttributeString("Name", null, "Table1");
        //    //start border width
        //    writer.WriteStartElement("Style");
        //    writer.WriteElementString("BorderWidth", "2pt");
        //    writer.WriteEndElement(); // Style  
        //    //end Border width
        //    writer.WriteElementString("DataSetName", "DataSet1");
        //    writer.WriteElementString("Top", ".5in");
        //    writer.WriteElementString("Left", ".5in");
        //    writer.WriteElementString("Height", ".5in");

        //    writer.WriteElementString("Width", (Flds.Count * 1.5).ToString() + "in");
        //    // Table Columns
        //    writer.WriteStartElement("TableColumns");
        //    For Each fieldName In Flds
        //        writer.WriteStartElement("TableColumn");
        //        writer.WriteElementString("Width", "1.5in");
        //        writer.WriteEndElement(); // TableColumn
        //    Next fieldName
        //    writer.WriteEndElement(); // TableColumns
        //    // Header RowChapter 10
        //    writer.WriteStartElement("Header");
        //    writer.WriteStartElement("TableRows");
        //    writer.WriteStartElement("TableRow");
        //    writer.WriteElementString("Height", ".25in");
        //    writer.WriteStartElement("TableCells");
        //    For Each fieldName In Flds
        //        writer.WriteStartElement("TableCell");
        //        writer.WriteStartElement("ReportItems");
        //        // Textbox
        //        writer.WriteStartElement("Textbox");
        //        writer.WriteAttributeString("Name", null, "Header" + fieldName);
        //        writer.WriteStartElement("Style");
        //        //add the next two lines to add style to header field
        //        writer.WriteElementString("TextDecoration", "Underline");
        //        writer.WriteElementString("FontWeight", "Bold");
        //        writer.WriteEndElement(); // Style
        //        writer.WriteElementString("Top", "0in");
        //        writer.WriteElementString("Left", "0in");
        //        writer.WriteElementString("Height", ".5in");
        //        writer.WriteElementString("Width", "1.5in");
        //        writer.WriteElementString("Value", fieldName);

        //        writer.WriteEndElement(); // Textbox
        //        writer.WriteEndElement(); // ReportItems
        //        writer.WriteEndElement(); // TableCell
        //    Next fieldName
        //    writer.WriteEndElement(); // TableCells
        //    writer.WriteEndElement(); // TableRow
        //    writer.WriteEndElement(); // TableRows
        //    writer.WriteEndElement(); // Header
        //    // Details Row
        //    writer.WriteStartElement("Details");
        //    writer.WriteStartElement("TableRows");
        //    writer.WriteStartElement("TableRow");
        //    writer.WriteElementString("Height", ".25in");
        //    writer.WriteStartElement("TableCells");
        //    For Each fieldName In Flds
        //        writer.WriteStartElement("TableCell");
        //        writer.WriteStartElement("ReportItems"); //On Programmatically Creating a SSRS Report
        //        // Textbox
        //        writer.WriteStartElement("Textbox");
        //        writer.WriteAttributeString("Name", Nothing, fieldName);
        //        writer.WriteStartElement("Style");
        //        writer.WriteEndElement(); // Style
        //        writer.WriteElementString("Top", "0in");
        //        writer.WriteElementString("Left", "0in");
        //        writer.WriteElementString("Height", ".5in");
        //        writer.WriteElementString("Width", "1.5in");
        //        writer.WriteElementString("Value", "=Fields!" + fieldName + ".Value");
        //        //writer.WriteElementString("HideDuplicates", "DataSet1");
        //        writer.WriteEndElement(); // Textbox
        //        writer.WriteEndElement();// ReportItems
        //        writer.WriteEndElement(); // TableCell
        //    Next fieldName
        //    // End Details element and children   
        //    writer.WriteEndElement(); // TableCells
        //    writer.WriteEndElement(); // TableRow
        //    writer.WriteEndElement(); // TableRows
        //    writer.WriteEndElement(); // Details
        //    // End table element and end report definition file
        //    writer.WriteEndElement(); // Table
        //    writer.WriteEndElement(); // ReportItems
        //    writer.WriteEndElement(); // Body
        //    writer.WriteEndElement(); // Report
        //    // Flush the writer and close the stream
        //    writer.Flush();
        //    stream.Close();
        //} //GenerateRdl
        #endregion
    }
}