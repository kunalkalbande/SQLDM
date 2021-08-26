using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using ChartFX.WinForms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries
{
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.DesktopClient.Dialogs.Query;
    using System.Data.SqlClient;
    using System.Linq;
    using Infragistics.Windows.Themes;
    using Idera.SQLdm.DesktopClient.Controls.CustomControls;

    internal partial class QueryMonitorView : ServerBaseView, IShowFilterDialog
    {
        public enum QueryMonitorViewMode
        {
            Signature,
            Statement,
            History,
            Waits
        }

        private const string QUERY_MONITOR_DISABLED =
            "The Query Monitor is currently disabled. Click here to configure the Query Monitor now.";
        // SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
       // private const string ADVANCED_QUERY_VIEWS =
          //  "Click here to check out the Advanced Query Views in the SQLDM Web Console.";
       
        private const string NO_ITEMS = @"There are no items to show for the selected timeframe and filters.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        ///Ankit Nagpal --Sqldm10.0.0
        ///If not a sysadmin display sysadmin message
        private const string SYSADMIN_MESSAGE = @"Sysadmin privileges are required to obtain data for this view.";
        private const string FORMAT_QUERY_LABEL = "Signature Name:   {0}";
        private const string NO_VALUE = @"N/A";

        private const string NO_SIGNATURE_TEXT = @"No Sql text was found for the specified query signature.";
        private const string NO_STATEMENT_TEXT = @"No Sql text was found for the specified query signature.";

        private const decimal percentCritical = 20;
        private const decimal percentWarning = 5;

        private DateTime? currentHistoricalSnapshotDateTime = null;
        private DateTime? historicalSnapshotDateTime = null;
        private QueryMonitorData queryMonitorData = null;
        private QueryMonitorFilter filter;
        //keep statement mode filter values that are not available in Signature Mode
        private QueryMonitorFilter lastStatementModeFilter;
		//SQLDM-20966 - Removed the keepFilter flag to ensure date range stay the same when switching between the views
        private bool initialized = false;
        private bool initializedSignatureMode = false;
        private bool initializedStatementMode = false;
        private bool initializedHistoryMode = false;
        private bool initializingCharts = false;
        private bool updatingMenu = false;
        private UltraGridColumn selectedColumn = null;
        private long? selectedSignatureId = null;
        private int? selectedStatementType = null;
        private bool selectedKeepDetail = false;
        private Control focused = null;
        private static readonly object updateLock = new object();
        private bool isFilterInitialized = false;
        //last Settings values used to determine if changed for saving when leaving
        private int lastMainSplitterDistance = 0;
        /// <summary>
        /// This is the status of the grid & chart settings at load time
        /// </summary>
        private GridSettings lastSignatureModeGridSettings = null;
        private GridSettings lastStatementModeGridSettings = null;
        private GridSettings lastHistoryModeGridSettings = null;
        private ChartSettings lastSignatureModeChartSettings1 = null;
        private ChartSettings lastSignatureModeChartSettings2 = null;
        private ChartSettings lastStatementModeChartSettings1 = null;
        private ChartSettings lastStatementModeChartSettings2 = null;
        private ChartSettings lastHistoryModeChartSettings1 = null;
        private ChartSettings lastHistoryModeChartSettings2 = null;

        //Start- SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- declared the new dummy data and grid objects
        private UltraGrid toBeExportedGrid = null;
        private QueryMonitorData toBeExportedData = null;
        //End - SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- declared the new dummy data and grid objects

        /// <summary>
        /// This reflects the settings that the user has currently chosen
        /// </summary>
        private GridSettings signatureModeGridSettings = null;
        private GridSettings statementModeGridSettings = null;
        private GridSettings historyModeGridSettings = null;
        private ChartSettings signatureModeChartSettings1 = null;
        private ChartSettings signatureModeChartSettings2 = null;
        private ChartSettings statementModeChartSettings1 = null;
        private ChartSettings statementModeChartSettings2 = null;
        private ChartSettings historyModeChartSettings1 = null;
        private ChartSettings historyModeChartSettings2 = null;
        private bool lastFiltersVisible = true;
        private bool lastGridVisible = true;
        private QueryMonitorViewMode viewMode = QueryMonitorViewMode.Signature;
        private QueryMonitorViewMode lastViewMode = QueryMonitorViewMode.Signature;

        public event EventHandler FiltersVisibleChanged;
        public event EventHandler GridVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler FilterChanged;

        private List<QueryMonitorPlanParameters> queriesIconsList = new List<QueryMonitorPlanParameters>();
        private EventHandler<ChartClickEventArgs> chartClickedEventHandler;

        // Delegate for the save estimated plan method to save the estimated plan asynchronously..
        private delegate void SaveEstimatedPlan_Delegate(SqlConnectionInfo connectionInfo, int statementID, int queryStatisticsID, string queryPlan);

        ThemeSetter themeSetter = new ThemeSetter();
        public QueryMonitorView(int instanceId) : base (instanceId)
        {
            InitializeComponent();
            AdaptFontSize();

            queryMonitorGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            chartClickedEventHandler = new EventHandler<ChartClickEventArgs>(this.chartPanel_ChartClicked);
            
            Initialize();

            ConfigureDisplayForMode();

            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;

            detailValuesNumericUpDown.NumericUpDownControl.Maximum = 500;
            detailValuesNumericUpDown.NumericUpDownControl.Minimum = 1;
            detailValuesNumericUpDown.NumericUpDownControl.Value = 200;

            // remove designer values from display fields to prevent showing values that switch to N/A on initial refresh
            avgCPUValueLabel.Text =
                avgReadsValueLabel.Text =
                avgWritesValueLabel.Text =
                executionsValueLabel.Text =
                maxCPUValueLabel.Text =
                maxReadsValueLabel.Text =
                maxWritesValueLabel.Text =
                executionsPerDayValueLabel.Text = string.Empty;

            SetAboveContentAlertTheme();
            SetGridTheme();
            SetDividerColor();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }
        
        private void ScaleControlsAsPerResolution()
        {
            //queryMonitorFiltersPanel.BackColor = Color.Red;
            this.queryMonitorFiltersPanel.Size = new System.Drawing.Size(1073, 156);
            this.queryMonitorFiltersPanel.Location = new System.Drawing.Point(0, 81);

            this.beginDateLabel.Size = new System.Drawing.Size(230, 30);
            this.beginDateLabel.Location = new System.Drawing.Point(5, 35);
            this.beginDateDateTimePicker.Location = new System.Drawing.Point(246, 35);
            this.beginDateDateTimePicker.Size = new System.Drawing.Size(147, 20);

            this.endDateLabel.Size = new System.Drawing.Size(80, 25);
            this.endDateLabel.Location = new System.Drawing.Point(410, 35);
            this.endDateDateTimePicker.Size = new System.Drawing.Size(147, 25);
            this.endDateDateTimePicker.Location = new System.Drawing.Point(500, 35);

            this.beginTimeLabel.Size = new System.Drawing.Size(250, 30);
            this.beginTimeLabel.Location = new System.Drawing.Point(700, 35);

            this.beginTimeDateTimePicker.Size = new System.Drawing.Size(150, 25);
            this.beginTimeDateTimePicker.Location = new System.Drawing.Point(960, 35);

            this.EndTimeLabel.Size = new System.Drawing.Size(84, 25);
            this.EndTimeLabel.Location = new System.Drawing.Point(1110, 35);

            this.endTimeDateTimePicker.Location = new System.Drawing.Point(1200, 35);
            this.endTimeDateTimePicker.Size = new System.Drawing.Size(150, 25);

            // second row
            this.appLabel.Location = new System.Drawing.Point(5, 72);
            this.appLabel.Size = new System.Drawing.Size(160, 30);

            this.applicationTextBox.Location = new System.Drawing.Point(172, 72);
            this.applicationTextBox.Size = new System.Drawing.Size(200, 30);

            this.userLabel.Location = new System.Drawing.Point(373, 72);
            this.userLabel.Size = new System.Drawing.Size(80, 30);

            this.userTextBox.Location = new System.Drawing.Point(453, 72);
            this.userTextBox.Size = new System.Drawing.Size(200, 30);

            this.excludeSqlLabel.Location = new System.Drawing.Point(700, 72);
            this.excludeSqlLabel.Size = new System.Drawing.Size(200, 30);

            this.sqlTextExcludeTextBox.Location = new System.Drawing.Point(900, 72);
            this.sqlTextExcludeTextBox.Size = new System.Drawing.Size(450, 30);
            //third row

            this.dbLabel.Location = new System.Drawing.Point(5, 102);
            this.dbLabel.Size = new System.Drawing.Size(160, 30);

            this.databaseTextBox.Location = new System.Drawing.Point(172, 102);
            this.databaseTextBox.Size = new System.Drawing.Size(200, 30);

            this.wsLabel.Location = new System.Drawing.Point(373, 102);
            this.wsLabel.Size = new System.Drawing.Size(80, 30);

            this.hostTextBox.Location = new System.Drawing.Point(453, 102);
            this.hostTextBox.Size = new System.Drawing.Size(200, 30);

            this.includeSqlLabel.Location = new System.Drawing.Point(700, 102);
            this.includeSqlLabel.Size = new System.Drawing.Size(200, 30);

            this.sqlTextIncludeTextBox.Location = new System.Drawing.Point(900, 102);
            this.sqlTextIncludeTextBox.Size = new System.Drawing.Size(450, 30);

            //checkboxes
            this.includeSqlStatementsCheckBox.Location = new System.Drawing.Point(1400, 35);
            this.includeSqlStatementsCheckBox.Size = new System.Drawing.Size(133, 25);

            this.includeStoredProcedureCheckBox.Location = new System.Drawing.Point(1400, 60);
            this.includeStoredProcedureCheckBox.Size = new System.Drawing.Size(133, 25);

            this.includeSqlBatchesCheckBox.Location = new System.Drawing.Point(1400, 85);
            this.includeSqlBatchesCheckBox.Size = new System.Drawing.Size(133, 25);

            this.includeOnlyResourceRowsCheckBox.Location = new System.Drawing.Point(1400, 110);
            this.includeOnlyResourceRowsCheckBox.Size = new System.Drawing.Size(133, 25);
            //queryMonitorHistoryPanel
            this.queryMonitorHistoryPanel.Size = new System.Drawing.Size(1073, 140);

            this.queryNameLabel.Location = new System.Drawing.Point(5, 35);
            this.queryNameLabel.Size = new System.Drawing.Size(129, 20);

            this.avgCPULabel.Location = new System.Drawing.Point(5, 70);
            this.avgCPULabel.Size = new System.Drawing.Size(100, 20);

            this.avgCPUValueLabel.Location = new System.Drawing.Point(150, 70);
            this.avgCPUValueLabel.Size = new System.Drawing.Size(150, 20);

            this.avgReadsLabel.Location = new System.Drawing.Point(350, 70);
            this.avgReadsLabel.Size = new System.Drawing.Size(150, 20);

            this.avgReadsValueLabel.Location = new System.Drawing.Point(500, 70);
            this.avgReadsValueLabel.Size = new System.Drawing.Size(150, 20);

            this.avgWritesLabel.Location = new System.Drawing.Point(700, 70);
            this.avgWritesLabel.Size = new System.Drawing.Size(150, 20);

            this.avgWritesValueLabel.Location = new System.Drawing.Point(900, 70);
            this.avgWritesValueLabel.Size = new System.Drawing.Size(150, 20);

            this.executionsPerDayLabel.Location = new System.Drawing.Point(1200, 70);
            this.executionsPerDayLabel.Size = new System.Drawing.Size(150, 20);

            this.executionsPerDayValueLabel.Location = new System.Drawing.Point(1400, 70);
            this.executionsPerDayValueLabel.Size = new System.Drawing.Size(150, 20);

            //second row
            this.maxCPULabel.Location = new System.Drawing.Point(5, 100);
            this.maxCPULabel.Size = new System.Drawing.Size(100, 20);

            this.maxCPUValueLabel.Location = new System.Drawing.Point(150, 100);
            this.maxCPUValueLabel.Size = new System.Drawing.Size(150, 20);

            this.maxReadsLabel.Location = new System.Drawing.Point(350, 100);
            this.maxReadsLabel.Size = new System.Drawing.Size(150, 20);

            this.maxReadsValueLabel.Location = new System.Drawing.Point(500, 100);
            this.maxReadsValueLabel.Size = new System.Drawing.Size(150, 20);

            this.maxWritesLabel.Location = new System.Drawing.Point(700, 100);
            this.maxWritesLabel.Size = new System.Drawing.Size(150, 20);

            this.maxWritesValueLabel.Location = new System.Drawing.Point(900, 100);
            this.maxWritesValueLabel.Size = new System.Drawing.Size(150, 20);

            this.executionsLabel.Location = new System.Drawing.Point(1200, 100);
            this.executionsLabel.Size = new System.Drawing.Size(150, 20);

            this.executionsValueLabel.Location = new System.Drawing.Point(1400, 100);
            this.executionsValueLabel.Size = new System.Drawing.Size(150, 20);

            this.splitLabel.Location = new Point(this.splitLabel.Location.X, this.splitLabel.Location.Y + 10);
            this.splitLabel.Height = 5;
            this.splitLabel.Width += 1800;
        }



        protected void Initialize()
        {
            if (DesignMode) return;

            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                MonitoredSqlServerWrapper instanceWrapper = ApplicationModel.Default.ActiveInstances[instanceId];
                InitializeFilter(instanceWrapper.Instance);
                InitializeData();   // do after initializing the filter so it can use the filter
                UpdateOperationalStatus(instanceWrapper.Instance);
                instanceWrapper.Changed += new EventHandler<MonitoredSqlServerChangedEventArgs>(MonitoredSqlServerChanged);
                
            }

           // UpdateWebUiStatus();//SQLdm 9.0 (vineet kumar) -- Fixing DE44333
          //  keepDetailedHistoryButton.Visible = (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify);
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set
            {
                if (historicalSnapshotDateTime != value)
                {
                    if (!value.HasValue)
                    {
                        filter.ResetTimeRangeValues();
                    }
                    historicalSnapshotDateTime = value;
                    currentHistoricalSnapshotDateTime = null;
                }
            }
        }

        public bool FiltersVisible
        {
            get { return queryMonitorFiltersPanel.Visible; }
            set
            {
                queryMonitorFiltersPanel.Visible = value;

                if (FiltersVisibleChanged != null)
                {
                    FiltersVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool GridVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;

                if (GridVisibleChanged != null)
                {
                    GridVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool GridGroupByBoxVisible
        {
            get { return !queryMonitorGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                queryMonitorGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public QueryMonitorFilter Filter
        {
            get { return filter; }
        }

        public QueryMonitorViewMode ViewMode
        {
            get { return viewMode; }
        }

        private void OnFilterChanged()
        {
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }

            applicationTextBox.Text = filter.ApplicationNameIncludeFilter;
            databaseTextBox.Text = filter.DatabaseNameIncludeFilter;
            userTextBox.Text = filter.UserNameIncludeFilter;
            hostTextBox.Text = filter.HostNameIncludeFilter;
            sqlTextExcludeTextBox.Text = filter.SqlTextExcludeFilter;
            sqlTextIncludeTextBox.Text = filter.SqlTextIncludeFilter;
            beginDateDateTimePicker.Value = filter.StartDate.Date;
            endDateDateTimePicker.Value = filter.EndDate.Date;
            beginTimeDateTimePicker.Value = beginTimeDateTimePicker.MinDate.Add(filter.StartTime);
            endTimeDateTimePicker.Value = endTimeDateTimePicker.MinDate.Add(filter.EndTime);
            includeSqlStatementsCheckBox.Checked = filter.IncludeSqlStatements;
            includeStoredProcedureCheckBox.Checked = filter.IncludeStoredProcedures;
            includeSqlBatchesCheckBox.Checked = filter.IncludeSqlBatches;
            
            if (viewMode == QueryMonitorViewMode.History)
            {
                filter.IncludeOnlyResourceRows = false;
            }
            else
            {
                includeOnlyResourceRowsCheckBox.Checked = filter.IncludeOnlyResourceRows;
            }
           

            InitializeCharts();
        }

        #region methods

        public override void ShowHelp()
        {
            switch (viewMode)
            {
                case QueryMonitorViewMode.Signature:
                    Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.QueriesViewSigMode);
                    break;
                case QueryMonitorViewMode.Statement:
                    Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.QueriesViewStmtMode);
                    break;
                case QueryMonitorViewMode.History:
                    Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.QueriesViewHistory);
                    break;
                default:
                    Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.QueriesView);
                    break;
            }
            //Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.QueriesView);
        }

        public override void SetArgument(object argument)
        {
            if (argument is long)
            {
                queryMonitorData.SignatureId = (long)argument;
                queryMonitorData.SignatureHash = null;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            else if (argument is string)
            {
                queryMonitorData.SignatureHash = (string)argument;
                queryMonitorData.SignatureId = null;
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
        }

        /// <summary>
        /// Get column size settings from disk and apply them to the controls
        /// </summary>
        public override void ApplySettings()
        {

            if (DesignMode) return;

            // Fixed panel is second panel, so restore size of second panel
            lastMainSplitterDistance =splitContainer.Height-Settings.Default.QueryMonitorViewMainSplitter;
            if (lastMainSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastMainSplitterDistance;
            }
            else
            {
                lastMainSplitterDistance = splitContainer.SplitterDistance;
            }

            if (Settings.Default.QueryMonitorViewSignatureModeChart1 is ChartSettings)
            {
                lastSignatureModeChartSettings1 =
                    signatureModeChartSettings1 = Settings.Default.QueryMonitorViewSignatureModeChart1;
            }

            if (Settings.Default.QueryMonitorViewSignatureModeChart2 is ChartSettings)
            {
                lastSignatureModeChartSettings2 =
                    signatureModeChartSettings2 = Settings.Default.QueryMonitorViewSignatureModeChart2;
            }

            if (Settings.Default.QueryMonitorViewSignatureModeGrid is GridSettings)
            {
                lastSignatureModeGridSettings =
                    signatureModeGridSettings = Settings.Default.QueryMonitorViewSignatureModeGrid;
            }

            if (Settings.Default.QueryMonitorViewStatementModeChart1 is ChartSettings)
            {
                lastStatementModeChartSettings1 =
                    statementModeChartSettings1 = Settings.Default.QueryMonitorViewStatementModeChart1;
            }

            if (Settings.Default.QueryMonitorViewStatementModeChart2 is ChartSettings)
            {
                lastStatementModeChartSettings2 =
                    statementModeChartSettings2 = Settings.Default.QueryMonitorViewStatementModeChart2;
            }

            if (Settings.Default.QueryMonitorViewStatementModeGrid is GridSettings)
            {
                lastStatementModeGridSettings =
                    statementModeGridSettings = Settings.Default.QueryMonitorViewStatementModeGrid;
            }

            if (Settings.Default.QueryMonitorViewHistoryModeChart1 is ChartSettings)
            {
                lastHistoryModeChartSettings1 =
                    historyModeChartSettings1 = Settings.Default.QueryMonitorViewHistoryModeChart1;
            }

            if (Settings.Default.QueryMonitorViewHistoryModeChart2 is ChartSettings)
            {
                lastHistoryModeChartSettings2 =
                    historyModeChartSettings2 = Settings.Default.QueryMonitorViewHistoryModeChart2;
            }

            if (Settings.Default.QueryMonitorViewHistoryModeGrid is GridSettings)
            {
                lastHistoryModeGridSettings =
                    historyModeGridSettings = Settings.Default.QueryMonitorViewHistoryModeGrid;
            }

            ConfigureDisplayForMode();

            if (viewMode == QueryMonitorViewMode.Signature && lastSignatureModeGridSettings != null)
            {
                GridSettings.ApplySettingsToGrid(lastSignatureModeGridSettings, queryMonitorGrid);
            }
            else if (viewMode == QueryMonitorViewMode.Statement && lastStatementModeGridSettings != null)
            {
                GridSettings.ApplySettingsToGrid(lastStatementModeGridSettings, queryMonitorGrid);
            }
            else if (viewMode == QueryMonitorViewMode.History && lastHistoryModeGridSettings != null)
            {
                GridSettings.ApplySettingsToGrid(lastHistoryModeGridSettings, queryMonitorGrid);
            }

            // force a change so ribbon stays in sync on group by
            GridGroupByBoxVisible = GridGroupByBoxVisible;

            lastFiltersVisible =
                FiltersVisible = Settings.Default.QueryMonitorViewFiltersVisible;

            lastGridVisible =
                GridVisible = Settings.Default.QueryMonitorViewGridVisible;

            detailValuesNumericUpDown.NumericUpDownControl.Value =
                queryMonitorData.DetailItemsToReturn = Settings.Default.QueryMonitorViewDetailItems;

            OnFilterChanged();            
        }

        /// <summary>
        /// Save chart and grid settings to disk
        /// </summary>
        public override void SaveSettings()
        {
            if (!initialized)
                return;

            GridSettings gridSettings = GridSettings.GetSettings(queryMonitorGrid);
            ChartSettings chartSettings1 = ChartSettings.GetSettings(chartPanel1);
            ChartSettings chartSettings2 = ChartSettings.GetSettings(chartPanel2);

            // Fixed panel is first panel, so save size of first panel and adjust the splitter to always save for non-history mode
            int splitterDistance = splitContainer.SplitterDistance;

            // save all settings only if anything has changed
            //SQLdm 10.0 (Rajesh Gupta) : Console Configuration Retention - Fixed Query View Retention
            if (lastMainSplitterDistance != splitterDistance
                || (viewMode == QueryMonitorViewMode.Signature && (
                        !gridSettings.Equals(Settings.Default.QueryMonitorViewSignatureModeGrid)
                        || !chartSettings1.Equals(Settings.Default.QueryMonitorViewSignatureModeChart1)
                        || !chartSettings2.Equals(Settings.Default.QueryMonitorViewSignatureModeChart2)))
                || (viewMode == QueryMonitorViewMode.Statement && (
                        !gridSettings.Equals(Settings.Default.QueryMonitorViewStatementModeGrid)
                        || !chartSettings1.Equals(Settings.Default.QueryMonitorViewStatementModeChart1)
                        || !chartSettings2.Equals(Settings.Default.QueryMonitorViewStatementModeChart2)))
                || (viewMode == QueryMonitorViewMode.History && (
                        !gridSettings.Equals(Settings.Default.QueryMonitorViewHistoryModeGrid)
                        || !chartSettings1.Equals(Settings.Default.QueryMonitorViewHistoryModeChart1)
                        || !chartSettings2.Equals(Settings.Default.QueryMonitorViewHistoryModeChart2)))
                || lastFiltersVisible != FiltersVisible
                || lastGridVisible != GridVisible
                || queryMonitorData.DetailItemsToReturn != Settings.Default.QueryMonitorViewDetailItems)
            {
                lastMainSplitterDistance =
                    Settings.Default.QueryMonitorViewMainSplitter = splitterDistance;
                //Save the grid and chart settings to the relevant mode settings object
                if (viewMode == QueryMonitorViewMode.Signature)
                {
                    signatureModeGridSettings = gridSettings;
                    signatureModeChartSettings1 = chartSettings1;
                    signatureModeChartSettings2 = chartSettings2;
                }
                else if (viewMode == QueryMonitorViewMode.Statement)
                {
                    statementModeGridSettings = gridSettings;
                    statementModeChartSettings1 = chartSettings1;
                    statementModeChartSettings2 = chartSettings2;
                }
                else if (viewMode == QueryMonitorViewMode.History)
                {
                    historyModeGridSettings = gridSettings;
                    historyModeChartSettings1 = chartSettings1;
                    historyModeChartSettings2 = chartSettings2;
                }

                if (initializedSignatureMode)
                {
                    if (signatureModeGridSettings != null)
                    {
                        lastSignatureModeGridSettings =
                            Settings.Default.QueryMonitorViewSignatureModeGrid = signatureModeGridSettings;
                    }

                    if (signatureModeChartSettings1 != null)
                    {
                        lastSignatureModeChartSettings1 =
                            Settings.Default.QueryMonitorViewSignatureModeChart1 = signatureModeChartSettings1;
                    }

                    if (signatureModeChartSettings2 != null)
                    {
                        lastSignatureModeChartSettings2 =
                            Settings.Default.QueryMonitorViewSignatureModeChart2 = signatureModeChartSettings2;
                    }
                }

                if (initializedStatementMode)
                {
                    if (statementModeGridSettings != null)
                    {
                        lastStatementModeGridSettings =
                            Settings.Default.QueryMonitorViewStatementModeGrid = statementModeGridSettings;
                    }

                    if (statementModeChartSettings1 != null)
                    {
                        lastStatementModeChartSettings1 =
                            Settings.Default.QueryMonitorViewStatementModeChart1 = statementModeChartSettings1;
                    }

                    if (statementModeChartSettings2 != null)
                    {
                        lastStatementModeChartSettings2 =
                            Settings.Default.QueryMonitorViewStatementModeChart2 = statementModeChartSettings2;
                    }
                }

                if (initializedHistoryMode)
                {
                    if (historyModeGridSettings != null)
                    {
                        lastHistoryModeGridSettings =
                            Settings.Default.QueryMonitorViewHistoryModeGrid = historyModeGridSettings;
                    }

                    if (historyModeChartSettings1 != null)
                    {
                        lastHistoryModeChartSettings1 =
                            Settings.Default.QueryMonitorViewHistoryModeChart1 = historyModeChartSettings1;
                    }

                    if (historyModeChartSettings2 != null)
                    {
                        lastHistoryModeChartSettings2 =
                            Settings.Default.QueryMonitorViewHistoryModeChart2 = historyModeChartSettings2;
                    }
                }

                lastFiltersVisible =
                    Settings.Default.QueryMonitorViewFiltersVisible = FiltersVisible;
                lastGridVisible =
                    Settings.Default.QueryMonitorViewFiltersVisible = GridVisible;

                Settings.Default.QueryMonitorViewDetailItems = queryMonitorData.DetailItemsToReturn;
            }
        }

        public void ShowFilter()
        {
            QueryMonitorFilter selectFilter = new QueryMonitorFilter(instanceId);
            selectFilter.UpdateValues(filter);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                filter.UpdateValues(selectFilter);
                ApplicationController.Default.RefreshActiveView();

                OnFilterChanged();
            }
        }

        public bool ConfigurationPropertiesDialogVisible { get; private set; }
        public void ShowConfigurationProperties()
        {
            try
            {
                MonitoredSqlServerInstancePropertiesDialog dialog =
                    new MonitoredSqlServerInstancePropertiesDialog(instanceId);
                dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.QueryMonitor;
                ConfigurationPropertiesDialogVisible = true;
                dialog.ShowDialog(this);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while loading the instance properties dialog.", e);
            }
            ConfigurationPropertiesDialogVisible = false;
        }

        public void ShowSignatureMode()
        {
            //SQLdm 10.4 Removal of advertisement for Web Console
            /* START : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
            advancedQueryViewLabel.Text = ADVANCED_QUERY_VIEWS;
                advancedQueryViewPanel.Visible = true;
             END : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client*/
            if (viewMode == QueryMonitorViewMode.Statement)
            {
                lastStatementModeFilter.HostNameExcludeFilter = filter.HostNameExcludeFilter;
                lastStatementModeFilter.HostNameIncludeFilter = filter.HostNameIncludeFilter;
                lastStatementModeFilter.UserNameExcludeFilter = filter.UserNameExcludeFilter;
                lastStatementModeFilter.UserNameIncludeFilter = filter.UserNameIncludeFilter;
            }
            if (viewMode != QueryMonitorViewMode.Signature
                || !initialized)
            {
                if (initialized)
                {
                    SaveSettings();
                }
                viewMode = QueryMonitorViewMode.Signature;

                InitializeCharts();
                ConfigureDisplayForMode();
            }
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ShowStatementMode()
        {
            //SQLdm 10.4 Removal of advertisement for Web Console
            /*
            // START : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
            advancedQueryViewLabel.Text = ADVANCED_QUERY_VIEWS;
            advancedQueryViewPanel.Visible = true;
            // END : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client*/
            if (viewMode != QueryMonitorViewMode.Statement
                || !initialized)
            {
                if (initialized)
                {
                    SaveSettings();
                }

                // use the saved statement mode only filter values
                hostTextBox.Text =
                    filter.HostNameIncludeFilter = (lastStatementModeFilter.HostNameIncludeFilter == null ?
                                                        string.Empty :
                                                        lastStatementModeFilter.HostNameIncludeFilter);
                filter.HostNameExcludeFilter = (lastStatementModeFilter.HostNameExcludeFilter == null ?
                                                        string.Empty :
                                                        lastStatementModeFilter.HostNameExcludeFilter);
                userTextBox.Text =
                    filter.UserNameIncludeFilter = (lastStatementModeFilter.UserNameIncludeFilter == null ?
                                                        string.Empty :
                                                        lastStatementModeFilter.UserNameIncludeFilter);
                filter.UserNameExcludeFilter = (lastStatementModeFilter.UserNameExcludeFilter == null ?
                                                        string.Empty :
                                                        lastStatementModeFilter.UserNameExcludeFilter);

                viewMode = QueryMonitorViewMode.Statement;

                InitializeCharts();
                ConfigureDisplayForMode();
            }
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ShowHistoryMode(int SignatureId)
        {
            queryMonitorData.SignatureId = SignatureId;

            ShowHistoryMode();
        }

        public void ShowHistoryMode(string SignatureHash)
        {
            queryMonitorData.SignatureHash = SignatureHash;

            ShowHistoryMode();
        }

        public void ShowHistoryMode()
        {
            //SQLdm 10.4 Removal of advertisement for Web Console
            /*
            // START : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
            advancedQueryViewLabel.Text = ADVANCED_QUERY_VIEWS;
            advancedQueryViewPanel.Visible = true;
            // END : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client*/
            if (viewMode == QueryMonitorViewMode.Statement)
            {
                //save the values in case the user jumps from history to signature back to statement 
                lastStatementModeFilter.HostNameExcludeFilter = filter.HostNameExcludeFilter;
                lastStatementModeFilter.HostNameIncludeFilter = filter.HostNameIncludeFilter;
                lastStatementModeFilter.UserNameExcludeFilter = filter.UserNameExcludeFilter;
                lastStatementModeFilter.UserNameIncludeFilter = filter.UserNameIncludeFilter;
            }
            //SQLDM-20966 - Removed the keepFilter flag to ensure date range stay the same when switching between the views
            if (viewMode != QueryMonitorViewMode.History
                || !initialized)
            {
                if (initialized)
                {
                    SaveSettings();
                }
                viewMode = QueryMonitorViewMode.History;

                InitializeCharts();
                ConfigureDisplayForMode();
            }
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        #endregion

        private void InitializeData()
        {
            queryMonitorData = new QueryMonitorData(viewMode, filter, null, null, null);
        }

        private void InitializeFilter(MonitoredSqlServer instance)
        {
            if (instance != null)
            {
                filter = new QueryMonitorFilter(instanceId);
                long durationFilter =
                    Convert.ToInt64(instance.QueryMonitorConfiguration.DurationFilter.TotalMilliseconds);
                filter.DurationFilter = durationFilter;
                filter.SetDefaultDurationFilter(durationFilter);

                //initialize a blank filter for tracking the statement mode only filters
                lastStatementModeFilter = new QueryMonitorFilter(instanceId);
            }
        }

        private void ConfigureDisplayForMode()
        {
            userTextBox.Enabled =
                hostTextBox.Enabled = (viewMode != QueryMonitorViewMode.Signature);

            includeOnlyResourceRowsCheckBox.Checked =
                includeOnlyResourceRowsCheckBox.Enabled = (viewMode != QueryMonitorViewMode.History);

            if (viewMode == QueryMonitorViewMode.Signature)
            {
                userTextBox.Text =
                    hostTextBox.Text = string.Empty;
            }
            queryMonitorHistoryPanel.Visible =
                historySelectLabel.Visible = (viewMode == QueryMonitorViewMode.History);

            chartPanel1.StatusText =
               chartPanel2.StatusText =
               queryMonitorGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            chartPanel1.ChartVisible =
                chartPanel2.ChartVisible =
                queryMonitorGrid.Visible = false;
        }

        #region Initialize Charts

        private void InitializeCharts()
        {
            initializingCharts = true;
            List<ChartType> selectionTypes = ChartHelper.GetQueryChartTypes(viewMode);
            List<ChartView> selectionViews = ConfigureChartViews();

            if (viewMode == QueryMonitorViewMode.Signature)
            {
                if (signatureModeChartSettings1 == null)
                {
                    signatureModeChartSettings1 = new ChartSettings();
                    signatureModeChartSettings1.ChartType = ChartType.QueryCPU;
                    signatureModeChartSettings1.ChartView = ChartView.QuerySQL;
                    signatureModeChartSettings1.ChartDisplayValues = 5;
                }
                InitializeChart(selectionTypes, selectionViews, chartPanel1, signatureModeChartSettings1);
                if (signatureModeChartSettings2 == null)
                {
                    signatureModeChartSettings2 = new ChartSettings();
                    signatureModeChartSettings2.ChartType = ChartType.QueryReads;
                    signatureModeChartSettings2.ChartView = ChartView.QueryApplication;
                    signatureModeChartSettings2.ChartDisplayValues = 5;
                }
                InitializeChart(selectionTypes, selectionViews, chartPanel2, signatureModeChartSettings2);
            }
            else if (viewMode == QueryMonitorViewMode.Statement)
            {
                if (statementModeChartSettings1 == null)
                {
                    statementModeChartSettings1 = new ChartSettings();
                    statementModeChartSettings1.ChartType = ChartType.QueryCPU;
                    statementModeChartSettings1.ChartView = ChartView.QuerySQL;
                    statementModeChartSettings1.ChartDisplayValues = 5;
                }
                InitializeChart(selectionTypes, selectionViews, chartPanel1, statementModeChartSettings1);
                if (statementModeChartSettings2 == null)
                {
                    statementModeChartSettings2 = new ChartSettings();
                    statementModeChartSettings2.ChartType = ChartType.QueryReads;
                    statementModeChartSettings2.ChartView = ChartView.QueryApplication;
                    statementModeChartSettings2.ChartDisplayValues = 5;
                }
                InitializeChart(selectionTypes, selectionViews, chartPanel2, statementModeChartSettings2);
            }
            else if (viewMode == QueryMonitorViewMode.History)
            {
                if (historyModeChartSettings1 == null)
                {
                    historyModeChartSettings1 = new ChartSettings();
                    historyModeChartSettings1.ChartType = ChartType.QueryCPU;
                    historyModeChartSettings1.ChartView = ChartView.QuerySQL;
                    historyModeChartSettings1.ChartDisplayValues = 5;
                }
                InitializeChart(selectionTypes, selectionViews, chartPanel1, historyModeChartSettings1);

                if (historyModeChartSettings2 == null)
                {
                    historyModeChartSettings2 = new ChartSettings();
                    historyModeChartSettings2.ChartType = ChartType.QueryReads;
                    historyModeChartSettings2.ChartView = ChartView.QueryApplication;
                    historyModeChartSettings2.ChartDisplayValues = 5;
                }
                InitializeChart(selectionTypes, selectionViews, chartPanel2, historyModeChartSettings2);
            }
            
            initializingCharts = false;
        }

        private void InitializeChart(List<ChartType> types, List<ChartView> views, ChartPanel chart, ChartSettings settings)
        {
            // make sure charts initialize in the right mode if it is changing.
            queryMonitorData.ViewMode = viewMode;
            // always remove the event handler first or it gets hooked up multiple times
            chart.ChartClicked -= chartClickedEventHandler;
            if (viewMode == QueryMonitorViewMode.History)
            {
                chart.InitializeChart(types, settings.ChartType);
                chart.SetDataSource(queryMonitorData, queryMonitorData.DetailTable);
            }
            else
            {
                chart.InitializeChart(types, settings.ChartType, views, settings.ChartView, settings.ChartDisplayValues, 1, 10);
                chart.ChartClicked += chartClickedEventHandler;
                chart.SetDataSource(queryMonitorData, queryMonitorData.SummaryTable);
                chart.AxisXTooltipShowsPointLabel = true;
            }

            chart.StatusText = Idera.SQLdm.Common.Constants.LOADING;
            chart.ChartVisible = false;
            CustomChart customchart = new CustomChart();
            customchart.SetCurrentChartTheme(chart.Chart);
        }

        private void ConfigureChartDataSource(QueryMonitorData dataObject, DataTable dataSource)
        {
            // Make sure that chart columns are configured correctly once there is data
            chartPanel1.CheckChartConfigured();
            chartPanel2.CheckChartConfigured();

            chartPanel1.SetDataSource(dataObject, dataSource);
            chartPanel2.SetDataSource(dataObject, dataSource);
        }

        private List<ChartView> ConfigureChartViews()
        {
            List<ChartView> views = ChartHelper.GetQueryChartViews(viewMode);

            //remove views that are already filtered to one value
            if (filter.ApplicationNameIncludeFilter != null && views.Contains(ChartView.QueryApplication))
            {
                views.Remove(ChartView.QueryApplication);
            }
            if (filter.DatabaseNameIncludeFilter != null && views.Contains(ChartView.QueryDatabase))
            {
                views.Remove(ChartView.QueryDatabase);
            }
            if (filter.HostNameIncludeFilter != null && views.Contains(ChartView.QueryHost))
            {
                views.Remove(ChartView.QueryHost);
            }
            if (filter.UserNameIncludeFilter != null && views.Contains(ChartView.QueryUser))
            {
                views.Remove(ChartView.QueryUser);
            }

            return views;
        }

        #endregion

        #region Refresh Data

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale 
            // or the view type changed
            if (viewMode == QueryMonitorViewMode.History ||
                (HistoricalSnapshotDateTime == null  ||
                viewMode != lastViewMode            ||
                (HistoricalSnapshotDateTime != null && (HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime
                || HistoricalStartDateTime != currentHistoricalStartDateTime))))
            {
                if (HistoricalSnapshotDateTime != null)
                {
                    // SqlDM 10.2 (Anshul Aggarwal) - History Browser - Fetch data depending on custom/historical mode.
                    DateTime? start = null, end = null;
                    if (base.ViewMode == ServerViewMode.Historical)
                    {
                        start = HistoricalSnapshotDateTime.Value.Subtract(
                            TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                        end = HistoricalSnapshotDateTime.Value;

                    }
                    else
                    {
                        start = HistoricalStartDateTime.Value;
                        end = HistoricalSnapshotDateTime.Value;
                    }

                    filter.StartDate = start.Value.Date;
                    filter.StartTime = new TimeSpan(start.Value.Hour, start.Value.Minute, start.Value.Second);
                    filter.EndDate = end.Value.Date;
                    filter.EndTime = new TimeSpan(end.Value.Hour, end.Value.Minute, end.Value.Second);

                    currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                    currentHistoricalStartDateTime = HistoricalStartDateTime;
                    beginDateDateTimePicker.Value = filter.StartDate;
                    endDateDateTimePicker.Value = filter.EndDate;
                    beginTimeDateTimePicker.Value = beginTimeDateTimePicker.MinDate.Add(filter.StartTime);
                    endTimeDateTimePicker.Value = endTimeDateTimePicker.MinDate.Add(filter.EndTime);
                }
                base.RefreshView();
            }
        }

        public override object DoRefreshWork()
        {
            queryMonitorData.ViewMode = viewMode;

            return
                RepositoryHelper.GetQueryMonitorStatements(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                           queryMonitorData, true);
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            queryMonitorGridStatusLabel.Text = UNABLE_TO_UPDATE;
            ///Ankit Nagpal --Sqldm10.0.0
            ///If not a sysadmin display sysadmin message

            //if (!isUserSysAdmin)
            //    queryMonitorGridStatusLabel.Text = SYSADMIN_MESSAGE;
            base.HandleBackgroundWorkerError(e);
        }


        public override void UpdateData(object data)
        {
            UpdateOperationalStatus(ApplicationModel.Default.ActiveInstances[instanceId]);
           
            if (data != null && data is QueryMonitorData)
            {
                lock (updateLock)
                {
                    queryMonitorData = (QueryMonitorData)data;

                    queriesIconsList = queryMonitorData.QueriesIcons;

                    //START: SQLdm 9.1 (Abhishek Joshi) -Fixed key customer defect (DE43877)
                    if (this.toolStripLabel1.Text.Contains("("))
                        this.toolStripLabel1.Text = "Event Occurrences";
                    // SQLDM-19787: 10.1.3 SQL DM Performance - Query Monitor View - Cache DetailTable to prevent recreating filtered table
                    var detailTableCache = queryMonitorData.DetailTable;
                    var detailTableCacheRowCount = (detailTableCache != null && detailTableCache.Rows != null) ? detailTableCache.Rows.Count : 0;

                    if (detailTableCache != null && detailTableCacheRowCount != 0)
                        this.toolStripLabel1.Text += " (" + detailTableCacheRowCount + ")";
                    
                    //END: SQLdm 9.1 (Abhishek Joshi) -Fixed key customer defect (DE43877)

                    if (queryMonitorData.ViewMode == viewMode && detailTableCacheRowCount > 0)
                    {
                        DataTable chartDataSource;
                        GridSettings gridSettings = lastSignatureModeGridSettings;
                        if (!initialized)
                        {
                            // SQLDM-19787 10.1.3 Performance - Query Monitor
                            chartDataSource = (queryMonitorData.ViewMode == QueryMonitorViewMode.History
                                                    ? detailTableCache
                                                    : queryMonitorData.SummaryTable);
                            ConfigureChartDataSource(queryMonitorData, chartDataSource);
                            if (lastViewMode == QueryMonitorViewMode.Signature)
                            {
                                if (lastSignatureModeGridSettings == null)
                                {
                                    lastSignatureModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                }
                                gridSettings = lastSignatureModeGridSettings;
                                initialized = true;
                            }
                            else if (lastViewMode == QueryMonitorViewMode.Statement)
                            {
                                if (lastStatementModeGridSettings == null)
                                {
                                    lastStatementModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                }
                                gridSettings = lastStatementModeGridSettings;
                                initialized = true;
                            }
                            else if (lastViewMode == QueryMonitorViewMode.History)
                            {
                                if (lastHistoryModeGridSettings != null)
                                {
                                    lastHistoryModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                }
                                gridSettings = lastHistoryModeGridSettings;
                                initialized = true;
                            }
                        }
                        //SQLdm 10.0 (Rajesh Gupta) : Console Configuration Retention - Fixed Query View Retention
                        else
                        {
                            if (viewMode == QueryMonitorViewMode.Signature)
                            {
                                if (signatureModeGridSettings == null || lastViewMode == QueryMonitorViewMode.Signature)
                                {
                                    signatureModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (signatureModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(signatureModeGridSettings, queryMonitorGrid);
                                    }
                                }
                                else if (lastViewMode == QueryMonitorViewMode.Statement)
                                {
                                    statementModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (statementModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(statementModeGridSettings, queryMonitorGrid);
                                    }
                                }
                                else
                                {
                                    historyModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (historyModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(historyModeGridSettings, queryMonitorGrid);
                                    }
                                }

                                gridSettings = signatureModeGridSettings;
                            }
                            else if (viewMode == QueryMonitorViewMode.Statement)
                            {
                                if (statementModeGridSettings == null || lastViewMode == QueryMonitorViewMode.Statement)
                                {
                                    statementModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (statementModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(statementModeGridSettings, queryMonitorGrid);
                                    }
                                }
                                else if (lastViewMode == QueryMonitorViewMode.Signature)
                                {
                                    signatureModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (signatureModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(signatureModeGridSettings, queryMonitorGrid);
                                    }
                                }
                                else
                                {
                                    historyModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (historyModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(historyModeGridSettings, queryMonitorGrid);
                                    }
                                }
                                gridSettings = statementModeGridSettings;
                            }
                            else if (viewMode == QueryMonitorViewMode.History)
                            {
                                if (historyModeGridSettings == null || lastViewMode == QueryMonitorViewMode.History)
                                {
                                    historyModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (historyModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(historyModeGridSettings, queryMonitorGrid);
                                    }
                                }
                                else if (lastViewMode == QueryMonitorViewMode.Signature)
                                {
                                    signatureModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (signatureModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(signatureModeGridSettings, queryMonitorGrid);
                                    }
                                }
                                else
                                {
                                    statementModeGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                                    if (statementModeGridSettings.Columns.Count > 0)
                                    {
                                        GetColumnSettings(statementModeGridSettings, queryMonitorGrid);
                                    }
                                }

                                gridSettings = historyModeGridSettings;
                            }
                        }

                        // SQLDM-19787 10.1.3 Performance - Query Monitor
                        chartDataSource = (queryMonitorData.ViewMode == QueryMonitorViewMode.History
                                                ? detailTableCache
                                                : queryMonitorData.SummaryTable);
                        ConfigureChartDataSource(queryMonitorData, chartDataSource);

                        if (queryMonitorData.GetSummaryView(chartPanel1.ChartType, chartPanel1.ChartView).Count == 0)
                        {
                            chartPanel1.StatusText = NO_ITEMS;
                            ///Ankit Nagpal --Sqldm10.0.0
                            ///If not a sysadmin display sysadmin message

                            //if(!isUserSysAdmin)
                            //    chartPanel1.StatusText = SYSADMIN_MESSAGE;
                            chartPanel1.ChartVisible = false;
                        }
                        else
                        {
                            chartPanel1.ChartVisible = true;
                        }

                        if (queryMonitorData.GetSummaryView(chartPanel2.ChartType, chartPanel2.ChartView).Count == 0)
                        {
                            chartPanel2.StatusText = NO_ITEMS;
                            ///Ankit Nagpal --Sqldm10.0.0
                            ///If not a sysadmin display sysadmin message

                            //if (!isUserSysAdmin)
                            //    chartPanel2.StatusText = SYSADMIN_MESSAGE;
                            chartPanel2.ChartVisible = false;
                        }
                        else
                        {
                            chartPanel2.ChartVisible = true;
                        }

                        UltraGridHelper.GridState state = UltraGridHelper.GetGridState(queryMonitorGrid, QueryMonitorData.COL_NAME);
                        queryMonitorGrid.SuspendLayout();

                        // SQLDM-19787 10.1.3 Performance - Query Monitor
                        queryMonitorGrid.DataSource = detailTableCache;

                        UltraGridHelper.RestoreGridState(state);
                        queryMonitorGrid.ResumeLayout();

                        queryMonitorGrid.DisplayLayout.Bands[0].Columns[QueryMonitorData.COL_NAME].SortComparer = new srtComparer();

                        //reapply grid settings on every refresh because datasource is changed
                        GridSettings.ApplySettingsToGrid(gridSettings, queryMonitorGrid);
                        UpdateGridColumns();
                        queryMonitorGrid.Visible = true;

                        lastViewMode = viewMode;
                        if (viewMode == QueryMonitorViewMode.Signature)
                        {
                            initializedSignatureMode = true;
                        }
                        if (viewMode == QueryMonitorViewMode.Statement)
                        {
                            initializedStatementMode = true;
                        }
                        if (viewMode == QueryMonitorViewMode.History)
                        {
                            initializedHistoryMode = true;
                        }
                    }
                    else
                    {
                        // if returned data is no longer valid for the view or there is no data, just hide everything.
                        if (queryMonitorData.ViewMode != viewMode || queryMonitorData.ResultsNoData)
                        {
                            chartPanel1.StatusText =
                                chartPanel2.StatusText =
                                queryMonitorGridStatusLabel.Text = NO_ITEMS;
                            ///Ankit Nagpal --Sqldm10.0.0
                            ///If not a sysadmin display sysadmin message

                            //if (!isUserSysAdmin)
                            //    chartPanel1.StatusText = chartPanel2.StatusText = queryMonitorGridStatusLabel.Text = SYSADMIN_MESSAGE;
                            chartPanel1.ChartVisible =
                                chartPanel2.ChartVisible =
                                queryMonitorGrid.Visible = false;
                        }
                        else
                        {
                            if (queryMonitorData.GetSummaryView(chartPanel1.ChartType, chartPanel1.ChartView).Count == 0)
                            {
                                chartPanel1.StatusText = NO_ITEMS;
                                ///Ankit Nagpal --Sqldm10.0.0
                                ///If not a sysadmin display sysadmin message

                                //if (!isUserSysAdmin)
                                //    chartPanel1.StatusText = SYSADMIN_MESSAGE;
                                chartPanel1.ChartVisible = false;
                            }
                            else
                            {
                                chartPanel1.ChartVisible = true;
                            }

                            if (queryMonitorData.GetSummaryView(chartPanel2.ChartType, chartPanel2.ChartView).Count == 0)
                            {
                                chartPanel2.StatusText = NO_ITEMS;
                                ///Ankit Nagpal --Sqldm10.0.0
                                ///If not a sysadmin display sysadmin message

                                //if (!isUserSysAdmin)
                                //    chartPanel2.StatusText = SYSADMIN_MESSAGE;
                                chartPanel2.ChartVisible = false;
                            }
                            else
                            {
                                chartPanel2.ChartVisible = true;
                            }

                            queryMonitorGridStatusLabel.Text = NO_ITEMS;
                            ///Ankit Nagpal --Sqldm10.0.0
                            ///If not a sysadmin display sysadmin message

                            //if (!isUserSysAdmin)
                            //    queryMonitorGridStatusLabel.Text = SYSADMIN_MESSAGE;
                            queryMonitorGrid.Visible = false;
                        }
                    }

                    if (viewMode == QueryMonitorViewMode.History)
                    {
                        DataView historyData = queryMonitorData.GetHistoryTotalView();

                        if (historyData.Count > 0)
                        {
                            DataRowView row = historyData[0];
                            string signatureText = row[QueryMonitorData.COL_SUM_SQLText].ToString();
                            if (signatureText.Length > 60)
                            {
                                signatureText = string.Format("{0}...", signatureText.Substring(0, 60));
                            }
                            querySignatureValueLabel.Text = signatureText;
                            queryNameValueLabel.Text = queryMonitorData.SignatureName;
                            avgCPUValueLabel.Text = (row[QueryMonitorData.COL_SUM_AVG_CPU] == DBNull.Value ?
                                                            NO_VALUE :
                                                            string.Format("{0} ms", ((decimal)row[QueryMonitorData.COL_SUM_AVG_CPU]).ToString("N0")));
                            avgReadsValueLabel.Text = (row[QueryMonitorData.COL_SUM_AVG_Reads] == DBNull.Value ?
                                                            NO_VALUE :
                                                            ((decimal)row[QueryMonitorData.COL_SUM_AVG_Reads]).ToString("N0"));
                            avgWritesValueLabel.Text = (row[QueryMonitorData.COL_SUM_AVG_Writes] == DBNull.Value ?
                                                            NO_VALUE :
                                                            ((decimal)row[QueryMonitorData.COL_SUM_AVG_Writes]).ToString("N0"));
                            maxCPUValueLabel.Text = (row[QueryMonitorData.COL_SUM_MAX_CPU] == DBNull.Value ?
                                                            NO_VALUE :
                                                            string.Format("{0} ms", ((decimal)row[QueryMonitorData.COL_SUM_MAX_CPU]).ToString("N0")));
                            maxReadsValueLabel.Text = (row[QueryMonitorData.COL_SUM_MAX_Reads] == DBNull.Value ?
                                                            NO_VALUE :
                                                            ((decimal)row[QueryMonitorData.COL_SUM_MAX_Reads]).ToString("N0"));
                            maxWritesValueLabel.Text = (row[QueryMonitorData.COL_SUM_MAX_Writes] == DBNull.Value ?
                                                            NO_VALUE :
                                                            ((decimal)row[QueryMonitorData.COL_SUM_MAX_Writes]).ToString("N0"));
                            executionsValueLabel.Text = (row[QueryMonitorData.COL_SUM_Occurrences] == DBNull.Value ?
                                                            NO_VALUE :
                                                            ((long)row[QueryMonitorData.COL_SUM_Occurrences]).ToString("N0"));
                            executionsPerDayValueLabel.Text = (row[QueryMonitorData.COL_SUM_OccurrencesPerDay] == DBNull.Value ?
                                                            NO_VALUE :
                                                            ((decimal)row[QueryMonitorData.COL_SUM_OccurrencesPerDay]).ToString("N2"));
                            bool keepDetail = false;
                            if (row[QueryMonitorData.COL_SUM_DoNotAggregate] != DBNull.Value)
                            {
                                keepDetail = (bool)row[QueryMonitorData.COL_SUM_DoNotAggregate];
                            }
                            if (keepDetail)
                            {
                                keepDetailedHistoryButton.Text = "Aggregate History";
                            }
                            else
                            {
                                keepDetailedHistoryButton.Text = "Keep Detailed History";
                            }
                            keepDetailedHistoryButton.Visible = queryMonitorData.HistorySignatureId.HasValue && (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify);
                            keepDetailedHistoryButton.Tag = keepDetail;
                            keepHistoryValueLabel.Visible = keepDetail;
                            viewHistorySqlTextButton.Visible = true;
                        }
                        else
                        {
                            querySignatureValueLabel.Text = string.Empty;
                            queryNameValueLabel.Text =
                                avgCPUValueLabel.Text =
                                avgReadsValueLabel.Text =
                                avgWritesValueLabel.Text =
                                maxCPUValueLabel.Text =
                                maxReadsValueLabel.Text =
                                maxWritesValueLabel.Text =
                                executionsValueLabel.Text =
                                executionsPerDayValueLabel.Text = NO_VALUE;
                            keepDetailedHistoryButton.Tag = false;
                            keepDetailedHistoryButton.Visible = false;
                            keepHistoryValueLabel.Visible = false;
                            viewHistorySqlTextButton.Visible = false;
                        }
                    }

                    UpdateDataUpdatingStatus(queryMonitorData);

                    // SQLDM-19787 10.1.3 Performance - Query Monitor
                    ApplicationController.Default.SetCustomStatus(
                        filter.IsFiltered() ? "Filter Applied" : string.Empty,
                        String.Format("{0} {1}",
                                      detailTableCacheRowCount,
                                      detailTableCacheRowCount == 1 ? "Query" : "Queries")
                        );

                    ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                }
            }
            else
            {
                chartPanel1.StatusText =
                    chartPanel2.StatusText =
                    queryMonitorGridStatusLabel.Text = NO_ITEMS;
                ///Ankit Nagpal --Sqldm10.0.0
                ///If not a sysadmin display sysadmin message

                //if(!isUserSysAdmin)
                //queryMonitorGridStatusLabel.Text = chartPanel1.StatusText = chartPanel2.StatusText = SYSADMIN_MESSAGE;
                chartPanel1.ChartVisible =
                    chartPanel2.ChartVisible =
                    queryMonitorGrid.Visible = false;

                UpdateDataUpdatingStatus(null);

                ApplicationController.Default.ClearCustomStatus();
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now, new ArgumentNullException("data")));
            }            
        }

        public void SetSqlStatementFilter(bool includeSqlStatements)
        {
            filter.IncludeSqlStatements =
                includeSqlStatementsCheckBox.Checked = includeSqlStatements;
            OnFilterChanged();

            ApplicationController.Default.RefreshActiveView();
        }

        public void SetStoredProceduresFilter(bool includeStoredProcedures)
        {
            filter.IncludeStoredProcedures =
                includeStoredProcedureCheckBox.Checked = includeStoredProcedures;
            OnFilterChanged();

            ApplicationController.Default.RefreshActiveView();
        }

        public void SetSqlBatchesFilter(bool includeSqlBatches)
        {
            filter.IncludeSqlBatches =
                includeSqlBatchesCheckBox.Checked = includeSqlBatches;
            OnFilterChanged();

            ApplicationController.Default.RefreshActiveView();
        }

        //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options -- Added new method below
        public void SetRowsFilter(bool includeOnlyResourceRows)
        {
            filter.IncludeOnlyResourceRows =
                includeOnlyResourceRowsCheckBox.Checked = includeOnlyResourceRows;
            OnFilterChanged();

            ApplicationController.Default.RefreshActiveView();
        }

        #endregion

        #region helpers

        private static Control GetFocusedControl(ControlCollection controls)
        {
            Control focusedControl = null;

            foreach (Control control in controls)
            {
                if (control.Focused)
                {
                    focusedControl = control;
                }
                else if (control.ContainsFocus)
                {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl != null ? focusedControl : controls[0];
        }

        #region grid

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - query data as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "QueryData";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Start- SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- Modified the export functionality
                    toBeExportedData = queryMonitorData; //Cloning the queryMonitorData object to a dummy object
                    toBeExportedData.GetFullQueryText = true; //changing the IsForExport
                    toBeExportedData = RepositoryHelper.GetQueryMonitorStatements(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                                   toBeExportedData); //getting the data from repostiory for export

                    toBeExportedGrid = queryMonitorGrid;//cloning the exisiting grid object to a dummy grid object
                    toBeExportedGrid.DataSource = toBeExportedData.DetailTable;//passing the newly fetched data for export
                    toBeExportedGrid.DataBind(); //binding the data

                    ultraGridExcelExporter.Export(toBeExportedGrid, saveFileDialog.FileName);//exporting from the dummy grid object
                    //End- SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- Modified the export functionality
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
                //Start- SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- Added finally block and disposing the dummy data and grid object
                finally
                {
                    toBeExportedGrid = null; 
                    toBeExportedData = null;
                }
                //End- SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- Added finally block and disposing the dummy data and grid object
            }

        }
        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                queryMonitorGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                queryMonitorGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                queryMonitorGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                queryMonitorGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    queryMonitorGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    queryMonitorGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups()
        {
            queryMonitorGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            queryMonitorGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(queryMonitorGrid);
            dialog.Show(this);
        }

        private void ShowQueryHistory()
        {
            if (queryMonitorGrid.Selected.Rows.Count == 1 && queryMonitorGrid.Selected.Rows[0].Cells != null)
            {
                long signatureId = (long)queryMonitorGrid.Selected.Rows[0].Cells[QueryMonitorData.COL_DB_SignatureID].Value;
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.QueriesHistory, signatureId);
            }
        }

        private void ShowQueryText()
        {
            if (queryMonitorGrid.Selected.Rows.Count == 1 && queryMonitorGrid.Selected.Rows[0].Cells != null)
            {
                bool isAggregated = Convert.ToBoolean((byte)queryMonitorGrid.Selected.Rows[0].Cells[QueryMonitorData.COL_DB_AggregationFlag].Value);
                //SQLdm 9.0 (Ankit Srivastava) removed unnecessary object creation

                if (viewMode == QueryMonitorViewMode.Signature || isAggregated)
                {
                    if (queryMonitorGrid.Selected.Rows[0].Cells[QueryMonitorData.COL_DB_SignatureID].Value != null)
                    {
                        long signatureId = (long)queryMonitorGrid.Selected.Rows[0].Cells[QueryMonitorData.COL_DB_SignatureID].Value;
                        string database = (string)queryMonitorGrid.Selected.Rows[0].Cells[QueryMonitorData.COL_DB_Database].Value;
                        ShowQuerySignatureText(signatureId, database);
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(ParentForm, NO_SIGNATURE_TEXT);
                    }
                }
                else
                {
                    if (queryMonitorGrid.Selected.Rows[0].Cells[QueryMonitorData.COL_DB_SqlTextID].Value != null)
                    {
                        long sqlTextId = (long)queryMonitorGrid.Selected.Rows[0].Cells[QueryMonitorData.COL_DB_SqlTextID].Value;
                        string database = (string)queryMonitorGrid.Selected.Rows[0].Cells[QueryMonitorData.COL_DB_Database].Value;
                        ShowQueryStatementText(sqlTextId, database);
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(ParentForm, NO_STATEMENT_TEXT);
                    }
                }
            }
        }

        private string SqlTextFromService(long signatureId)
        {
            IManagementService managementService =
                           ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            Pair<long, string> returnVal = managementService.GetQuerySignatureText(signatureId);

            if (returnVal == null)
                return null;

            return returnVal.Second;
        }

        private void ShowQuerySignatureText(long signatureId, string database)
        {
            var sql = SqlTextFromService(signatureId);

            if (sql != null)
            {
                SqlTextDialog dialog = new SqlTextDialog(sql, database, instanceId, false);
                dialog.ShowDialog(ParentForm);
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm, NO_SIGNATURE_TEXT);
            }
        }

        private void ShowQueryStatementText(long sqlTextId, string database)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            Pair<long, string> returnVal = managementService.GetQueryStatementText(sqlTextId);
            if (returnVal != null && returnVal.Second != null)
            {
                SqlTextDialog dialog = new SqlTextDialog(returnVal.Second,database, instanceId, true);
                dialog.ShowDialog(ParentForm);
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm, NO_STATEMENT_TEXT);
            }
        }

        private void KeepDetailedHistory(long signatureId, bool keepHistory)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            managementService.SetQueryAggregationFlag(signatureId, keepHistory);
            RefreshView();
        }

        /// <summary>
        /// Update the grid with columns enabled or disabled according to the signature mode setting.
        /// Change the heading to average for cpu, reads and writes
        /// </summary>
        private void UpdateGridColumns()
        {
            try
            {
                //gets settings from the grid and puts them in the gridsettings object
                //GridSettings gridSettings = GridSettings.GetSettings(queryMonitorGrid);
                bool signatureMode = (viewMode == QueryMonitorViewMode.Signature);
                bool statementMode = (viewMode == QueryMonitorViewMode.Statement);
                bool historyMode = (viewMode == QueryMonitorViewMode.History);

                UltraGridColumn col;
                GridSettings currentEffectiveMainGridSettings = signatureMode
                                                                    ? signatureModeGridSettings
                                                                    : statementMode
                                                                        ? statementModeGridSettings
                                                                        : historyModeGridSettings;

                if (currentEffectiveMainGridSettings == null)
                {
                    currentEffectiveMainGridSettings = GridSettings.GetSettings(queryMonitorGrid);
                }

                //iterate through the columns of the effective grid settings and apply signature mode rules
                foreach (ColumnSettings colSettings in currentEffectiveMainGridSettings.Columns.Values)
                {
                    col = queryMonitorGrid.DisplayLayout.Bands[colSettings.Band].Columns[colSettings.Key];

                    //string strCaption = col.Header.Caption;
                    //bool blnHidden = false;
                    //bool blnExcludefromColumnChooser = false;

                    switch (colSettings.Key)
                    {
                        // These columns are always shown
                        case QueryMonitorData.COL_DB_Database:
                        case QueryMonitorData.COL_DB_Application:
                        case QueryMonitorData.COL_DB_CPUPerSecond:
                        case QueryMonitorData.COL_DB_IOPerSecond:
                        case QueryMonitorData.COL_DB_AggregationFlag:
                        case QueryMonitorData.COL_DB_DoNotAggregate:                            
                            break;
                        // These columns are always shown, but the caption varies by mode
                        case QueryMonitorData.COL_NAME:
                            col.Header.Caption = col.ColumnChooserCaption = historyMode ? "Occurrence" : "Query";
                            break;
                        case QueryMonitorData.COL_DB_CompletionTime:
                            col.Header.Caption = col.ColumnChooserCaption = signatureMode ? "Most Recent Completion" : "End Time";
                            break;
                        case QueryMonitorData.COL_DB_Duration:
                            col.Header.Caption = col.ColumnChooserCaption = signatureMode ? "Avg. Duration (ms)" : "Duration (ms)";
                            break;
                        case QueryMonitorData.COL_DB_CPU:
                            col.Hidden = historyMode;
                            break;
                        case QueryMonitorData.COL_DB_Reads:
                            col.Hidden = historyMode;
                            break;
                        case QueryMonitorData.COL_DB_Writes:
                            col.Hidden = historyMode;
                            break;
                        case QueryMonitorData.COL_DB_Waits:
                            col.Header.Caption = col.ColumnChooserCaption = signatureMode ? "Avg. Wait Time (ms)" : "Wait Time (ms)";
                            break;
                        case QueryMonitorData.COL_DB_Deadlocks:
                            col.Header.Caption = col.ColumnChooserCaption = signatureMode ? "Avg. Deadlocks" : "Deadlocks";
                            break;
                        case QueryMonitorData.COL_DB_Blocking:
                            col.Header.Caption = col.ColumnChooserCaption = signatureMode ? "Avg. Blocking Time (ms)" : "Blocking Time (ms)";
                            break;
                        // These columns are always hidden
                        case QueryMonitorData.COL_QueryStatisticsId:     //SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
                        case QueryMonitorData.COL_NAME_SORT:
                        case QueryMonitorData.COL_DB_SignatureID:
                        case QueryMonitorData.COL_DB_SqlTextID:
                            col.Hidden = true;
                            col.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            //blnHidden =
                            //    blnExcludefromColumnChooser = true;
                            break;
                        // These columns are NOT shown in Signature mode
                        case QueryMonitorData.COL_DB_StartTime:
                        case QueryMonitorData.COL_DB_Host:
                        case QueryMonitorData.COL_DB_User:
                        case QueryMonitorData.COL_DB_Spid:
                            if (signatureMode)
                            {
                                col.Hidden = true;
                                col.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            }
                            else
                            {
                                col.Hidden = false;            //SQLdm 10.0 (Rajesh Gupta) : Console Configuration Retention - Fixed Query View Retention
                                col.ExcludeFromColumnChooser = ExcludeFromColumnChooser.False;
                            }
                            //blnHidden =
                            //    blnExcludefromColumnChooser = signatureMode;
                            break;
                        // These columns are always shown in Signature mode
                        case QueryMonitorData.COL_DB_Occurrences:
                            if (signatureMode)
                            {
                                col.Hidden = false;
                                col.ExcludeFromColumnChooser = ExcludeFromColumnChooser.False;
                            }
                            else
                            {
                                col.Hidden = true;
                                col.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            }
                            //blnHidden =
                            //    blnExcludefromColumnChooser = !signatureMode;
                            break;
                        // These columns are are NOT shown in History mode
                        case QueryMonitorData.COL_CPU_Pct:
                        case QueryMonitorData.COL_Reads_Pct:
                        case QueryMonitorData.COL_Writes_Pct:
                            if (historyMode)
                            {
                                col.Hidden = true;
                                col.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            }
                            //blnHidden =
                            //    blnExcludefromColumnChooser = historyMode;
                            break;
                    }

                    //if the column is irrelevant then exclude it from the column chooser
                    //col.ExcludeFromColumnChooser = blnExcludefromColumnChooser ? ExcludeFromColumnChooser.True : ExcludeFromColumnChooser.False;
                    // don't redisplay a column that is now permanently hidden
                    //col.Hidden = blnHidden || colSettings.Hidden || col.ExcludeFromColumnChooser == ExcludeFromColumnChooser.True;
                    //set the new caption
                    //col.Header.Caption = strCaption;
                }
            }
            catch (Exception ex01)
            {
                Log.Error(ex01);
            }
        }

        #endregion

        private void MaximizeGrid()
        {
            splitContainer.Visible = false;
            splitContainer.Panel2.Controls.Remove(queryMonitorGridPanel);
            maximizeQueryMonitorGridButton.Visible = false;
            restoreQueryMonitorGridButton.Visible = true;
            QueryMonitorView_Fill_Panel.Controls.Add(queryMonitorGridPanel);
            queryMonitorGridPanel.BringToFront();
        }

        private void RestoreGrid()
        {
            QueryMonitorView_Fill_Panel.Controls.Remove(queryMonitorGridPanel);
            maximizeQueryMonitorGridButton.Visible = true;
            restoreQueryMonitorGridButton.Visible = false;
            splitContainer.Panel2.Controls.Add(queryMonitorGridPanel);
            splitContainer.Visible = true;
        }

        #endregion

        #region events

        private void MonitoredSqlServerChanged(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            UpdateOperationalStatus(e.Instance);
        }

        #region splitter

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        #endregion

        private void clearQueryMonitorFiltersButton_Click(object sender, EventArgs e)
        {
            filter.ResetValues();

            OnFilterChanged();

            ApplicationController.Default.RefreshActiveView();
        }

        private void closeQueryMonitorFiltersButton_Click(object sender, EventArgs e)
        {
            FiltersVisible = false;
        }
        private void closeQueryMonitorGridButton_Click(object sender, EventArgs e)
        {
            GridVisible = false;
        }

        private void maximizeQueryMonitorGridButton_Click(object sender, EventArgs e)
        {
            MaximizeGrid();
        }

        private void restoreQueryMonitorGridButton_Click(object sender, EventArgs e)
        {
            RestoreGrid();
        }

        #region toolbarsManager

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (updatingMenu)
                return;

            switch (e.Tool.Key)
            {
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "toggleGroupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
                case "queryHistoryButton":
                    ShowQueryHistory();
                    break;
                case "showQueryTextButton":
                    ShowQueryText();
                    break;
                case "showQueryPlanButton":
                    if (selectedSignatureId.HasValue && selectedStatementType != null)
                    {
                        GetShowPlanData(selectedSignatureId.Value, selectedStatementType.Value);
                    }
                    break;
                case "keepDetailedHistoryButton":
                    if (selectedSignatureId.HasValue)
                    {
                        KeepDetailedHistory(selectedSignatureId.Value, !selectedKeepDetail);
                    }
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "gridDataContextMenu":
                    bool isHistoryMode = (viewMode == QueryMonitorViewMode.History);
                    bool isStatementMode = (viewMode == QueryMonitorViewMode.Statement);
                    bool isSignatureMode = (viewMode == QueryMonitorViewMode.Signature);
                    bool isWaitsMode = (viewMode == QueryMonitorViewMode.Waits);
                    ((PopupMenuTool)e.Tool).Tools["queryHistoryButton"].SharedProps.Visible = !isHistoryMode;
                    ((PopupMenuTool)e.Tool).Tools["keepDetailedHistoryButton"].SharedProps.Visible = !isHistoryMode && (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify);
                    ((PopupMenuTool)e.Tool).Tools["showQueryPlanButton"].SharedProps.Visible = !isHistoryMode;
                    ((PopupMenuTool)e.Tool).Tools["showQueryPlanButton"].SharedProps.Visible = !isStatementMode;
                    ((PopupMenuTool)e.Tool).Tools["showQueryPlanButton"].SharedProps.Visible = isSignatureMode;
                    ((PopupMenuTool)e.Tool).Tools["showQueryPlanButton"].SharedProps.Visible = isWaitsMode;
                    ((StateButtonTool)toolbarsManager.Tools["keepDetailedHistoryButton"]).Checked = selectedKeepDetail;
                    updatingMenu = false;
                    break;
            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = queryMonitorGrid.Rows.Count > 0 && queryMonitorGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
        }

        #endregion

        #region Operational Status

        private void UpdateOperationalStatus(MonitoredSqlServer instance)
        {
            SetAboveContentAlertTheme();

            if (HistoricalSnapshotDateTime != null)
            {
                operationalStatusLabel.Text = Properties.Resources.HistoryModeOperationalStatusLabel;
                operationalStatusPanel.Visible = true;
            }
            else if (instance != null && !instance.QueryMonitorConfiguration.Enabled)
            {
                operationalStatusLabel.Text = QUERY_MONITOR_DISABLED;
                operationalStatusPanel.Visible = true;
            }
            else
            {
                operationalStatusPanel.Visible = false;
            }
        }

        private void UpdateDataUpdatingStatus(QueryMonitorData data)
        {
            if (data != null && data.ResultsQueryMonitorUpgrading)
            {
                updatingStatusLabel.Text = data.ResultsQueryMonitorUpgradingText;
                updatingStatusPanel.Visible = true;
            }
            else
            {
                updatingStatusPanel.Visible = false;
            }
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Light")
            {
                operationalStatusLabel.ForeColor = Color.Black;
                operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
                operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
            }           
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            if (Settings.Default.ColorScheme == "Light")
            {
                operationalStatusLabel.ForeColor = Color.Black;
                operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
                operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
            }           
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (Settings.Default.ColorScheme == "Light")
            {
                operationalStatusLabel.ForeColor = Color.White;
                operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
                operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
            }           
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (Settings.Default.ColorScheme == "Light")
            {
                operationalStatusLabel.ForeColor = Color.Black;
                operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
                operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
            }        

            if (HistoricalSnapshotDateTime != null)
            {
                operationalStatusPanel.Visible = false;
                ApplicationController.Default.SetActiveViewToRealTimeMode();
            }
            else
            {
                ShowConfigurationProperties();
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetAboveContentAlertTheme();
            SetGridTheme();
            SetDividerColor();
        }

        private void SetDividerColor()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.splitLabel.BackColor = System.Drawing.Color.White;
            }
            else
            {
                this.splitLabel.BackColor = System.Drawing.Color.Black;
            }
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.queryMonitorGrid);
        }

        private void SetAboveContentAlertTheme()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                themeSetter.SetAboveContentAlertPanelTheme(this.operationalStatusPanel, instanceId);
                themeSetter.SetAboveContentAlertLabelTheme(this.operationalStatusLabel, instanceId);
                themeSetter.SetAboveContentAlertPictureBoxTheme(this.operationalStatusImage, instanceId);
            }
            else
            {
                themeSetter.SetPanelTheme(this.operationalStatusPanel, System.Drawing.Color.FromArgb(212, 212, 212));
                themeSetter.SetLabelTheme(this.operationalStatusLabel, System.Drawing.Color.FromArgb(212, 212, 212), System.Drawing.Color.Black);
                themeSetter.SetPictureBoxTheme(this.operationalStatusImage, System.Drawing.Color.FromArgb(((int) (((byte) (212)))), ((int) (((byte) (212)))), ((int) (((byte) (212))))), global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall);
            }
        }

        #endregion

        #region Grid

        private void queryMonitorGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (queryMonitorGrid.Selected.Rows.Count == 1 && queryMonitorGrid.Selected.Rows[0].Cells != null)
            {
                //sqlTextBox.Text = queryMonitorGrid.Selected.Rows[0].Cells["StatementText"].Value as string;
            }
        }

        private void queryMonitorGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
			//START: SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
            UltraGridBand band = queryMonitorGrid.DisplayLayout.Bands[0];
            object[] gridColumns = band.Columns.All;
            bool flag = false;
            foreach (object column in gridColumns)
            {
                string col = column.ToString();
                if (col.Equals(QueryMonitorData.COL_WEBUI_QueryLink, StringComparison.OrdinalIgnoreCase))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                UltraGridColumn imageColumn = band.Columns.Add(QueryMonitorData.COL_WEBUI_QueryLink);
                imageColumn.Header.VisiblePosition = 2;
                imageColumn.DataType = typeof(Image);
                imageColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                imageColumn.CellActivation = Activation.ActivateOnly;
                imageColumn.CellAppearance.ImageHAlign = HAlign.Center;
                imageColumn.CellAppearance.ImageVAlign = VAlign.Middle;
                imageColumn.CellAppearance.Cursor = Cursors.Hand;
            }
			//END: SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
            queryMonitorGrid.DisplayLayout.Bands[0].SortedColumns.Add(QueryMonitorData.COL_DB_CompletionTime, true);
        }

        private void queryMonitorGrid_MouseDown(object sender, MouseEventArgs e)
        {
            selectedSignatureId = null;
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader =
                        contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridDataContextMenu");

                        //bool keepsDetail = false;
                        if (row.Row.ListObject is DataRowView)
                        {
                            if (((DataRowView)row.Row.ListObject).Row.Table.Columns.Contains(QueryMonitorData.COL_DB_SignatureID))
                            {
                                selectedStatementType = (int)((DataRowView)row.Row.ListObject)[QueryMonitorData.COL_DB_StatementType];
                                selectedSignatureId = (long)((DataRowView)row.Row.ListObject)[QueryMonitorData.COL_DB_SignatureID];
                                if (((DataRowView)row.Row.ListObject).Row.Table.Columns.Contains(QueryMonitorData.COL_DB_DoNotAggregate))
                                {
                                    selectedKeepDetail = (bool)((DataRowView)row.Row.ListObject)[QueryMonitorData.COL_DB_DoNotAggregate];
                                }
                            }
                        }
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
            //START: SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
            else if (e.Button == MouseButtons.Left)
            {
                
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

                if (selectedElement == null) 
                    return;

                CellUIElement objCellElement =
                    (CellUIElement)selectedElement.GetAncestor(typeof(Infragistics.Win.UltraWinGrid.CellUIElement));

                RowUIElement objRowElement =
                    (RowUIElement)selectedElement.GetAncestor(typeof(Infragistics.Win.UltraWinGrid.RowUIElement));

                if (objCellElement == null || objRowElement == null) 
                    return;

                UltraGridCell objCell =
                    (UltraGridCell)objCellElement.GetContext(typeof(UltraGridCell));

                UltraGridRow objRow =
                    (UltraGridRow)objRowElement.GetContext(typeof(UltraGridRow));

                if (objCell == null || objRow == null) 
                    return;
                var isSupported = false;
                    isSupported = HandleUnsupportedSQLServerQP();
                if (objCell.Column.Key == QueryMonitorData.COL_WEBUI_QueryLink)
                {
                    try
                    {

                        bool isActualPlan = false;
                        var sqlScript = string.Empty;
                        string planXML = string.Empty;
                        int signatureId = 0, statementType = 0, queryStatisticsId = 0;
                        if (this.viewMode == QueryMonitorViewMode.Signature)
                        {
                            if (objRow.Cells[QueryMonitorData.COL_DB_StatementType] != null
                                && objRow.Cells[QueryMonitorData.COL_DB_SignatureID] != null)
                            {
                                if (!String.IsNullOrEmpty(
                                        objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString())
                                    && !String.IsNullOrEmpty(
                                        objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString())
                                    && int.TryParse(objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString(), out signatureId)
                                    && int.TryParse(objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString(), out statementType)
                                        )
                                {
                                    planXML = RepositoryHelper.GetQueryPlanInformation(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                        signatureId,
                                        statementType,
                                        null);
                                }
                                
                            }
                        }
                        if (this.viewMode == QueryMonitorViewMode.Statement)
                        {
                            if (objRow.Cells[QueryMonitorData.COL_DB_StatementType] != null
                                && objRow.Cells[QueryMonitorData.COL_DB_SignatureID] != null)
                            {
                                if (!String.IsNullOrEmpty(
                                        objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString())
                                    && !String.IsNullOrEmpty(
                                        objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString())
                                    && int.TryParse(objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString(), out signatureId)
                                    && int.TryParse(objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString(), out statementType)
                                        )
                                {
                                    planXML = RepositoryHelper.GetQueryPlanInformation(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                        signatureId,
                                        statementType,
                                        null);
                                }
                                
                            }
                        }
                        if (this.viewMode == QueryMonitorViewMode.History)
                        {
                            if (objRow.Cells[QueryMonitorData.COL_DB_StatementType] != null
                                && objRow.Cells[QueryMonitorData.COL_DB_SignatureID] != null)
                            {
                                if (!String.IsNullOrEmpty(
                                        objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString())
                                    && !String.IsNullOrEmpty(
                                        objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString())
                                    && int.TryParse(objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString(), out signatureId)
                                    && int.TryParse(objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString(), out statementType)
                                        )
                                {
                                    planXML = RepositoryHelper.GetQueryPlanInformation(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                        signatureId,
                                        statementType,
                                        null);
                                }
                                
                            }
                        }
                        if (this.viewMode == QueryMonitorViewMode.Waits)
                        {
                            if (objRow.Cells[QueryMonitorData.COL_DB_StatementType] != null
                                && objRow.Cells[QueryMonitorData.COL_DB_SignatureID] != null)
                            {
                                if (!String.IsNullOrEmpty(
                                        objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString())
                                    && !String.IsNullOrEmpty(
                                        objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString())
                                    && int.TryParse(objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString(), out signatureId)
                                    && int.TryParse(objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString(), out statementType)
                                        )
                                {
                                    planXML = RepositoryHelper.GetQueryPlanInformation(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                        signatureId,
                                        statementType,
                                        null);
                                }
                                
                            }
                        }
                        else if (objRow.Cells[QueryMonitorData.COL_QueryStatisticsId] != null
                                 && objRow.Cells[QueryMonitorData.COL_DB_SignatureID] != null
                                 && objRow.Cells[QueryMonitorData.COL_DB_StatementType] != null)
                                
                        {
                            if (!String.IsNullOrEmpty(
                                    objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString())
                                && !String.IsNullOrEmpty(
                                    objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString())
                                && !String.IsNullOrEmpty(
                                    objRow.Cells[QueryMonitorData.COL_QueryStatisticsId].Value.ToString())
                                && int.TryParse(
                                    objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString(),
                                    out signatureId)
                                && int.TryParse(
                                    objRow.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString(),
                                    out statementType)
                                && int.TryParse(
                                    objRow.Cells[QueryMonitorData.COL_QueryStatisticsId].Value.ToString(),
                                    out queryStatisticsId))
                            {
                                planXML = RepositoryHelper.GetQueryPlanInformation(
                                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                    signatureId,
                                    statementType,
                                    queryStatisticsId);
                            }

                       }

                        // SQLdm10.4 - Get Estimated Plan if not present (Nikhil Bansal)
                        if (string.IsNullOrEmpty(planXML))
                    {
                        IManagementService managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                        if (managementService == null)
                        {
                            Log.Error("Could not get the interface to the management service");
                            return;
                        }
                        String queryText, databaseName;
                        int textId;
                        Pair<long, String> value;
                        Pair<int, int> planInfo = RepositoryHelper.GetInfoToSavePlan(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, signatureId, statementType);

                        // To save the plan for the query on which the user has clicked not a random statement
                        if (viewMode != QueryMonitorViewMode.Signature)
                            planInfo.Second = queryStatisticsId;

                        //Get the Query Text for the selected statement
                        if (viewMode == QueryMonitorViewMode.Signature && !String.IsNullOrEmpty(
                                objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString())
                            && int.TryParse(
                                objRow.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString(),
                                out signatureId)
                            && (planInfo.First != -1 && planInfo.Second != -1))
                        {
                            value = managementService.GetQueryStatementText(planInfo.First);
                        }
                        else if (!String.IsNullOrEmpty(
                                objRow.Cells[QueryMonitorData.COL_DB_SqlTextID].Value.ToString())
                                && int.TryParse(
                                objRow.Cells[QueryMonitorData.COL_DB_SqlTextID].Value.ToString(),
                                out textId)
                                && (planInfo.First != -1 && planInfo.Second != -1))
                        {
                            value = managementService.GetQueryStatementText(textId);
                        }
                        else
                        {
                            Log.Error("Could not get the sql query of the selected statment");
                            return;
                        }


                        if (value != null && value.Second != null)
                        {
                            queryText = value.Second;
                            databaseName = !String.IsNullOrEmpty(objRow.Cells[QueryMonitorData.COL_DB_Database].Value.ToString())
                                                ? objRow.Cells[QueryMonitorData.COL_DB_Database].Value.ToString() : null;

                            if (databaseName != null)
                            {
                                EstimatedQueryPlanConfiguration configuration = new EstimatedQueryPlanConfiguration(instanceId, queryText, databaseName);
                                planXML = managementService.GetEstimatedQueryPlan(configuration);

                                if (planXML != null)
                                {
                                    //Save the plan Asynchronously
                                    SaveEstimatedPlan_Delegate saveEstimatedPlan = new SaveEstimatedPlan_Delegate(RepositoryHelper.SaveEstimatedQueryPlan);
                                    IAsyncResult result = saveEstimatedPlan.BeginInvoke(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, planInfo.First, planInfo.Second, planXML, null, null);
                                        
                                }
                                else
                                {
                                    Log.Error("Not able to fetch the estimated query Plan");
                                    return;
                                }
                            }
                            else
                            {
                                Log.Error("Not able to fetch the database name");
                                return;
                            }
                        }
                        else
                        {
                            Log.Error("Not able to fetch the sql query statement");
                            return;
                        }
                            
                    }

                        if (String.IsNullOrEmpty(planXML))
                            throw new NullReferenceException("Not able to fetch the sql query plan xml");

                        isActualPlan = GetResultingPlanType(signatureId);
                       
                        sqlScript = SqlTextFromService(signatureId);

                        
                        ShowQueryDiagram(planXML, isSupported, isActualPlan, sqlScript, instanceId, viewMode == QueryMonitorViewMode.Statement);
                    }
                    
                    catch (Exception exception)
                    {
                        Log.Info("Not able to open query plan viewer : => " + exception.InnerException);
                    }
                    
                }
            }
            //END: SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
        }

        /// <summary>
        /// ///////// SQL 10.4 (Charles Schultz) -adding a client side desktop window to handle query plan display
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
         
        private void GetShowPlanData(long? selectedSignatureId, int? selectedStatementType)
        {
            string errorMessage = "Failed to load Query Diagram display, because XML Plan could not be retrieve for this Query:" +
                       Environment.NewLine;
            string xmlPlan = string.Empty;
            
            var isSupported = HandleUnsupportedSQLServerQP();
            if (selectedSignatureId.HasValue && selectedStatementType != null)
            {
                int SignatureId = (int)selectedSignatureId;
                xmlPlan = RepositoryHelper.GetQueryPlanInformation(
                                       Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                       SignatureId,
                                       selectedStatementType,
                                       null);
                var isActualPlan = GetResultingPlanType(SignatureId);
                var sqlScript = RepositoryHelper.GetSQLStatement(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, SignatureId);

                ShowQueryDiagram(xmlPlan, isSupported, isActualPlan, sqlScript, instanceId, viewMode == QueryMonitorViewMode.Statement);
            }
            else
                ApplicationMessageBox.ShowError(ParentForm, errorMessage);

        }

        private bool GetResultingPlanType(int signatureId)
        {
            
            var actualPlan = RepositoryHelper.IsActualQueryPlan(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, signatureId);

            return actualPlan;
        }

        private bool HandleUnsupportedSQLServerQP()
        {
            var version = RepositoryHelper.GetProductVersion(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
            var versionCheckString = version.IsSupported;
            if (versionCheckString)
                return true;
            return false;
        }
        private void ShowQueryDiagram(string xmlPlan, bool isSupported, bool isActualPlan, string sqlStatement, int instanceId, bool canAnalyze)
        {
            if (String.IsNullOrEmpty(xmlPlan))
            {
                string stackTrace = Environment.StackTrace.ToString();
                Log.Error("Unable to retrieve XML Data for Query: ", Environment.NewLine, stackTrace);
            }
            else
            {
                QueryPlanDiagramDialog queryPlanDiagramDialog = new QueryPlanDiagramDialog(xmlPlan,isSupported,isActualPlan, sqlStatement, instanceId, canAnalyze);
                queryPlanDiagramDialog.Show();
            }
            
        }

        private void queryMonitorGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // set the alpha level based on percent
            //int alpha = 255;
            decimal pct = 0;
            int signatureId;
            decimal val;
            Color color = Color.White;
            if (decimal.TryParse(e.Row.Cells[QueryMonitorData.COL_CPU_Pct].Text, out val))
            {
                pct = Math.Floor(val);
                color = Color.FromArgb(125, pct.CompareTo(percentCritical) > 0 ? Color.Red : Color.Yellow);

                if (pct.CompareTo(percentWarning) >= 0)
                {
                    e.Row.Cells[QueryMonitorData.COL_DB_CPU].Appearance.BackColor = color;
                    e.Row.Cells[QueryMonitorData.COL_DB_CPU].Appearance.BackColor2 = Color.White;
                    e.Row.Cells[QueryMonitorData.COL_DB_CPU].Appearance.BackGradientStyle = GradientStyle.Horizontal;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_CPU].Appearance.BackColor = color;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_CPU].Appearance.BackColor2 = Color.White;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_CPU].Appearance.BackGradientStyle = GradientStyle.Horizontal;
                }
            }

            if (decimal.TryParse(e.Row.Cells[QueryMonitorData.COL_Reads_Pct].Text, out val))
            {
                pct = Math.Floor(val);
                color = Color.FromArgb(125, pct.CompareTo(percentCritical) > 0 ? Color.Red : Color.Yellow);

                if (pct.CompareTo(percentWarning) >= 0)
                {
                    e.Row.Cells[QueryMonitorData.COL_DB_Reads].Appearance.BackColor = color;
                    e.Row.Cells[QueryMonitorData.COL_DB_Reads].Appearance.BackColor2 = Color.White;
                    e.Row.Cells[QueryMonitorData.COL_DB_Reads].Appearance.BackGradientStyle = GradientStyle.Horizontal;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_Reads].Appearance.BackColor = color;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_Reads].Appearance.BackColor2 = Color.White;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_Reads].Appearance.BackGradientStyle = GradientStyle.Horizontal;
                }
            }

            if (decimal.TryParse(e.Row.Cells[QueryMonitorData.COL_Writes_Pct].Text, out val))
            {
                pct = Math.Floor(val);
                color = Color.FromArgb(125, pct.CompareTo(percentCritical) > 0 ? Color.Red : Color.Yellow);

                if (pct.CompareTo(percentWarning) >= 0)
                {
                    e.Row.Cells[QueryMonitorData.COL_DB_Writes].Appearance.BackColor = color;
                    e.Row.Cells[QueryMonitorData.COL_DB_Writes].Appearance.BackColor2 = Color.White;
                    e.Row.Cells[QueryMonitorData.COL_DB_Writes].Appearance.BackGradientStyle = GradientStyle.Horizontal;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_Writes].Appearance.BackColor = color;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_Writes].Appearance.BackColor2 = Color.White;
                    e.Row.Cells[QueryMonitorData.COL_SUM_AVG_Writes].Appearance.BackGradientStyle = GradientStyle.Horizontal;
                }
            }

            if (queriesIconsList == null)
                queriesIconsList = new List<QueryMonitorPlanParameters>();
            //SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
            int statementTypeId = 0;
            if (int.TryParse(e.Row.Cells[QueryMonitorData.COL_DB_SignatureID].Value.ToString(), out signatureId)
                && int.TryParse(e.Row.Cells[QueryMonitorData.COL_DB_StatementType].Value.ToString(), out statementTypeId))
            {
                int qmsId = 0;
                int.TryParse(e.Row.Cells[QueryMonitorData.COL_QueryStatisticsId].Value.ToString(), out qmsId);

                try
                {
                    /* We wrote this out to clarify the logic of the lambda expression.  We may want to use this in the future.*/
                    /*  
                    foreach(QueryMonitorPlanParameters x in queriesIconsList)
                    {
                         if(x != null && x.SignatureId.Equals(signatureId) && x.StatementTypeId.Equals(statementTypeId) && (qmsId == 0 || x.QueryMonitorStatisticsId.Equals(qmsId)))
                        {
                            e.Row.Cells[QueryMonitorData.COL_WEBUI_QueryLink].Value = global::Idera.SQLdm.DesktopClient.Properties.Resources.QMWebUI16x16;
                        
                    } else
                        {
                        e.Row.Cells[QueryMonitorData.COL_WEBUI_QueryLink].Value = global::Idera.SQLdm.DesktopClient.Properties.Resources.QMWebUI16x16gray;
                    }
                    

                    }
                    */
                    if (queriesIconsList.Any(x => x != null && x.SignatureId.Equals(signatureId) && x.StatementTypeId.Equals(statementTypeId) && (qmsId == 0 || x.QueryMonitorStatisticsId.Equals(qmsId))))
                    {
                        e.Row.Cells[QueryMonitorData.COL_WEBUI_QueryLink].Value = global::Idera.SQLdm.DesktopClient.Properties.Resources.QMWebUI16x16;
                    }
                    else
                    {
                        e.Row.Cells[QueryMonitorData.COL_WEBUI_QueryLink].Value = global::Idera.SQLdm.DesktopClient.Properties.Resources.QMWebUI16x16gray;
                    }
                } catch (Exception exception)
                {
                    Log.Error(String.Format("queryMonitorGrid_InitializeRow(object sender, InitializeRowEventArgs e):  ") + exception.Message);
                    e.Row.Cells[QueryMonitorData.COL_WEBUI_QueryLink].Value = global::Idera.SQLdm.DesktopClient.Properties.Resources.QMWebUI16x16gray;

                }
            }
        }

        #endregion

        #region filter events

        private void beginDateDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (beginDateDateTimePicker.Value.Date != filter.StartDate.Date)
            {
                filter.StartDate = beginDateDateTimePicker.Value.Date;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void endDateDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (endDateDateTimePicker.Value.Date != filter.EndDate.Date)
            {
                filter.EndDate = endDateDateTimePicker.Value.Date;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void beginTimeDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan time = beginTimeDateTimePicker.Value.Subtract(beginTimeDateTimePicker.MinDate);
            if (time.Ticks != filter.StartTime.Ticks)
            {
                filter.StartTime = time;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void endTimeDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan time = endTimeDateTimePicker.Value.Subtract(endTimeDateTimePicker.MinDate);
            if (time.Ticks != filter.EndTime.Ticks)
            {
                filter.EndTime = time;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void applicationTextBox_Leave(object sender, EventArgs e)
        {
            string text = applicationTextBox.Text == string.Empty ? null : applicationTextBox.Text;
            if (text != filter.ApplicationNameIncludeFilter)
            {
                filter.ApplicationNameIncludeFilter = text;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void applicationTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                applicationTextBox_Leave(sender, e as EventArgs);
            }
        }

        private void databaseTextBox_Leave(object sender, EventArgs e)
        {
            string text = databaseTextBox.Text == string.Empty ? null : databaseTextBox.Text;
            if (text != filter.DatabaseNameIncludeFilter)
            {
                filter.DatabaseNameIncludeFilter = text;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void databaseTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                databaseTextBox_Leave(sender, e as EventArgs);
            }
        }

        private void userTextBox_Leave(object sender, EventArgs e)
        {
            string text = userTextBox.Text == string.Empty ? null : userTextBox.Text;
            if (text != filter.UserNameIncludeFilter)
            {
                filter.UserNameIncludeFilter = text;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void userTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                userTextBox_Leave(sender, e as EventArgs);
            }
        }

        private void hostTextBox_Leave(object sender, EventArgs e)
        {
            string text = hostTextBox.Text == string.Empty ? null : hostTextBox.Text;
            if (text != filter.HostNameIncludeFilter)
            {
                filter.HostNameIncludeFilter = text;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void hostTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                hostTextBox_Leave(sender, e as EventArgs);
            }
        }

        private void sqlTextExcludeTextBox_Leave(object sender, EventArgs e)
        {
            string text = sqlTextExcludeTextBox.Text == string.Empty ? null : sqlTextExcludeTextBox.Text;
            if (text != filter.SqlTextExcludeFilter)
            {
                filter.SqlTextExcludeFilter = text;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void sqlTextExcludeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                sqlTextExcludeTextBox_Leave(sender, e as EventArgs);
            }
        }

        private void sqlTextIncludeTextBox_Leave(object sender, EventArgs e)
        {
            string text = sqlTextIncludeTextBox.Text == string.Empty ? null : sqlTextIncludeTextBox.Text;
            if (text != filter.SqlTextIncludeFilter)
            {
                filter.SqlTextIncludeFilter = text;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void sqlTextIncludeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                sqlTextIncludeTextBox_Leave(sender, e as EventArgs);
            }
        }

        private void includeSqlStatementsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (includeSqlStatementsCheckBox.Checked != filter.IncludeSqlStatements)
            {
                filter.IncludeSqlStatements = includeSqlStatementsCheckBox.Checked;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void includeStoredProcedureCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (includeStoredProcedureCheckBox.Checked != filter.IncludeStoredProcedures)
            {
                filter.IncludeStoredProcedures = includeStoredProcedureCheckBox.Checked;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        private void includeSqlBatchesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (includeSqlBatchesCheckBox.Checked != filter.IncludeSqlBatches)
            {
                filter.IncludeSqlBatches = includeSqlBatchesCheckBox.Checked;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options --Added New event handler for new text app 
        private void ResourceRowsNeededCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (includeOnlyResourceRowsCheckBox.Checked != filter.IncludeOnlyResourceRows)
            {
                filter.IncludeOnlyResourceRows = includeOnlyResourceRowsCheckBox.Checked;
                OnFilterChanged();

                ApplicationController.Default.RefreshActiveView();
            }

            includeOnlyResourceRowsCheckBox.Height = 20;

        }

        #endregion

        #endregion

        private void QueryMonitorView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        /// <summary>
        /// Save current column settings from the passed grid to the passed GridSettings
        /// </summary>
        /// <param name="settings">The GridSettings that will have the settings saved to it</param>
        /// <param name="grid">The UltraGrid from which the settings will be saved</param>
        public static void GetColumnSettings(GridSettings settings, UltraGrid grid)
        {
            SortedList<Pair<int, int>, ColumnSettings> colSorts = new SortedList<Pair<int, int>, ColumnSettings>();

            settings.GroupByBoxVisible = !grid.DisplayLayout.GroupByBox.Hidden;

            //ColumnSettings colSettings;
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
            {
                foreach (UltraGridColumn col in band.Columns)
                {
                    //colSettings = new ColumnSettings(band.Index, col.Key, col.Header.VisiblePosition, col.Header.Fixed, col.Width, settings.Columns[new Pair<int, string>(0, col.Key)].Hidden);
                    //colSettings.SetColumnSort(band.SortedColumns.IndexOf(col), col.SortIndicator, col.IsGroupByColumn);
                    if (!settings.Columns.ContainsKey(new Pair<int, string>(band.Index, col.Key)))
                    {
                        settings.Columns.Add(new Pair<int, string>(band.Index, col.Key),
                                             new ColumnSettings(band.Index,
                                                                col.Key,
                                                                col.Header.VisiblePosition,
                                                                col.Header.Fixed,
                                                                col.Width,
                                                                col.Hidden));
                    }
                    settings.Columns[new Pair<int, string>(band.Index, col.Key)].SetColumnSort(band.SortedColumns.IndexOf(col), col.SortIndicator, col.IsGroupByColumn);
                }
            }
        }

        private void chartPanel_ChartClicked(object sender, ChartClickEventArgs e)
        {
            ChartView chartView = e.ChartInfo.ChartView;
            string hitval = string.Empty;
            string tagval = string.Empty;
            if (e.HitTestEventArgs.HitType == HitType.Point)
            {
                hitval = e.Chart.Data.Labels[e.HitTestEventArgs.Point];
                if (e.Chart.Points[0, e.HitTestEventArgs.Point] != null
                        && e.Chart.Points[0, e.HitTestEventArgs.Point].Tag != null)
                {
                    tagval = e.Chart.Points[0, e.HitTestEventArgs.Point].Tag.ToString();
                }
            }
            else if (e.HitTestEventArgs.HitType == HitType.Axis
                        && e.HitTestEventArgs.Object == e.Chart.AxisX)
            {
                int closestPoint = (int)Math.Floor(e.HitTestEventArgs.Value);
                hitval = e.Chart.Data.Labels[closestPoint];
                if (e.Chart.Points[0, closestPoint] != null
                        && e.Chart.Points[0, closestPoint].Tag != null)
                {
                    tagval = e.Chart.Points[0, closestPoint].Tag.ToString();
                }
            }

            if (hitval.Length > 0)
            {
                if (chartView == ChartView.QueryApplication)
                {
                    applicationTextBox.Text = tagval;
                    applicationTextBox_Leave(applicationTextBox, EventArgs.Empty);
                }
                else if (chartView == ChartView.QueryDatabase)
                {
                    databaseTextBox.Text = tagval;
                    databaseTextBox_Leave(databaseTextBox, EventArgs.Empty);
                }
                else if (chartView == ChartView.QueryHost)
                {
                    hostTextBox.Text = tagval;
                    hostTextBox_Leave(hostTextBox, EventArgs.Empty);
                }
                else if (chartView == ChartView.QueryUser)
                {
                    userTextBox.Text = tagval;
                    userTextBox_Leave(userTextBox, EventArgs.Empty);
                }
                else if (chartView == ChartView.QuerySQL)
                {
                    long signatureId = 0;
                    if (Int64.TryParse(tagval, out signatureId))
                    {
                        ApplicationController.Default.ShowServerView(instanceId, ServerViews.QueriesHistory, signatureId);
                    }
                }
            }
        }

        private void chartPanel1_ChartSelectionChanged(object sender, ChartSelectionEventArgs e)
        {
            if (!initializingCharts)
            {
                if (viewMode == QueryMonitorViewMode.Signature)
                {
                    signatureModeChartSettings1 = e.ChartSettings;
                }
                else if (viewMode == QueryMonitorViewMode.Statement)
                {
                    statementModeChartSettings1 = e.ChartSettings;
                }
                else if (viewMode == QueryMonitorViewMode.History)
                {
                    historyModeChartSettings1 = e.ChartSettings;
                }
            }
        }

        private void chartPanel2_ChartSelectionChanged(object sender, ChartSelectionEventArgs e)
        {
            if (!initializingCharts)
            {
                if (viewMode == QueryMonitorViewMode.Signature)
                {
                    signatureModeChartSettings2 = e.ChartSettings;
                }
                else if (viewMode == QueryMonitorViewMode.Statement)
                {
                    statementModeChartSettings2 = e.ChartSettings;
                }
                else if (viewMode == QueryMonitorViewMode.History)
                {
                    historyModeChartSettings2 = e.ChartSettings;
                }
            }
        }

        private void detailValuesNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            queryMonitorData.DetailItemsToReturn = (int)detailValuesNumericUpDown.NumericUpDownControl.Value;
            RefreshView();
        }

        private void keepDetailedHistoryButton_Click(object sender, EventArgs e)
        {
            if (queryMonitorData.HistorySignatureId.HasValue && keepDetailedHistoryButton.Tag is Boolean)
            {
                KeepDetailedHistory(queryMonitorData.HistorySignatureId.Value, !(bool)keepDetailedHistoryButton.Tag);
            }
        }

        private void viewHistorySqlTextButton_Click(object sender, EventArgs e)
        {
            if (queryMonitorData.HistorySignatureId.HasValue)
            {
                ShowQuerySignatureText(queryMonitorData.HistorySignatureId.Value, string.Empty);
            }
        }

        private void splitContainer_SizeChanged(object sender, EventArgs e)
        {
            if (splitContainer.Height >= (splitContainer.Panel1MinSize + splitContainer.Panel2MinSize))
            {
                if (splitContainer.SplitterDistance > (splitContainer.Height - splitContainer.Panel2MinSize))
                {
                    splitContainer.SplitterDistance = (splitContainer.Height - splitContainer.Panel2MinSize);
                }
            }
        }

        public class srtComparer : IComparer
        {
            public srtComparer()
            {
            }

            public int Compare(object x, object y)
            {

                UltraGridCell xCell = (UltraGridCell)x;
                UltraGridCell yCell = (UltraGridCell)y;

                return ((long)xCell.Row.Cells[QueryMonitorData.COL_NAME_SORT].Value).CompareTo((long)yCell.Row.Cells[QueryMonitorData.COL_NAME_SORT].Value);
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // START : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
        // This section covers the SQL Web UI access for the SQL Plan send to a windows form that has the requested funtionality
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////  
        /// <summary>
        /// SQLdm 10.4 (Charles Schultz) - Remove Web Console Advertisement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        private void advancedQueryViewLabel_MouseEnter(object sender, EventArgs e)
        {
            advancedQueryViewLabel.ForeColor = Color.Black;
            advancedQueryViewLabel.BackColor = Color.FromArgb(255, 189, 105);
            advancedQueryViewImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void advancedQueryViewLabel_MouseLeave(object sender, EventArgs e)
        {
            advancedQueryViewLabel.ForeColor = Color.Black;
            advancedQueryViewLabel.BackColor = Color.FromArgb(135, 206, 250);
            advancedQueryViewImage.BackColor = Color.FromArgb(135, 206, 250);
        }

        private void advancedQueryViewLabel_MouseDown(object sender, MouseEventArgs e)
        {
            advancedQueryViewLabel.ForeColor = Color.White;
            advancedQueryViewLabel.BackColor = Color.FromArgb(251, 140, 60);
            advancedQueryViewImage.BackColor = Color.FromArgb(251, 140, 60);
        }
        
        private void advancedQueryViewLabel_MouseUp(object sender, MouseEventArgs e)
        {
            advancedQueryViewLabel.ForeColor = Color.Black;
            advancedQueryViewLabel.BackColor = Color.FromArgb(255, 189, 105);
            advancedQueryViewImage.BackColor = Color.FromArgb(255, 189, 105);

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//START: SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
            // String identified by this part is actually the url that is associated with the DB Web Interface associated with IDERA
            // Find WebUI Algorithms and convert to Windows form
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            List<string> queryTabInfo = managementService.GetWebUIQueryInfo();

            if (queryTabInfo[0] != null)
            {
                try
                {
                    if (!String.IsNullOrEmpty(queryTabInfo[1]) && !String.IsNullOrEmpty(queryTabInfo[2]) && instanceId > 0)
                    {
                        string queryTabURL = queryTabInfo[0] + queryTabInfo[1] + "/" + queryTabInfo[2] + "/singleInstance/" + instanceId + "?tab=2";
                        Process.Start(queryTabURL);
                    }
                    else
                        Process.Start(queryTabInfo[0]);
                }
                catch (Exception exception)
                {
                    Log.Info("Not able to open URL : " + queryTabInfo[0] + " => " + exception.InnerException);
                }
			//END: SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
            }
            else
            {
                Log.Info("CWF Web URL is null !!");
            }
        }
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // END : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
        //////////////////////////////////////////////////////////////////////////////////////////////////
             

        //START : SQLdm 9.0 (vineet kumar) -- Fixing DE44333
        private void UpdateWebUiStatus()
        {
            IManagementService managementService =
               ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            string cwfWebUrl = managementService.GetCWFWebURL();
            if (string.IsNullOrEmpty(cwfWebUrl))
            {
                advancedQueryViewPanel.Visible = false;
            }
            else
                advancedQueryViewPanel.Visible = true;
        }*/
        //END : SQLdm 9.0 (vineet kumar) -- Fixing DE44333
        
    }
}
