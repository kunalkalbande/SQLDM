using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using System.Globalization;

using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using System.Drawing.Drawing2D;
using Idera.SQLdm.Common;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.Common.UI.Dialogs;
using System.Text;
using System.Collections.Generic;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Sessions
{
    internal partial class SessionsSummaryView : ServerBaseView
    {
        #region constants

        private const string FORMAT_CHARTTYPE = "Lock Statistics: {0}";

        public enum BlockingType
        {
            Locks,
            Sessions
        }

        #endregion

        #region fields
        
        private readonly ServerSummaryHistoryData historyData;
        private static readonly object updateLock = new object();
        private Chart contextMenuSelectedChart = null;
        private Exception historyModeLoadError = null;

        //last Settings values used to determine if changed for saving when leaving
        private string lastLocksChartType = string.Empty;

        #endregion

        #region constructors
        private DataTable top5DataTable;
        private List<string> selectedDatabases = new List<string>();
        private HashSet<string> uniqueDatabases = new HashSet<String>();
        private Dictionary<string, string> dbValMap = new Dictionary<string, string>();

        public SessionsSummaryView(int instanceId, ServerSummaryHistoryData historyData)
            : base(instanceId)
        {
            RefreshReportsProgress = true;
            this.historyData = historyData;

            InitializeComponent();

            // Remove max sessions panel
            // this.tableLayoutPanel.Controls.Remove(this.maxSessionPanel);
            this.tableLayoutPanel.Controls.Remove(this.maxSessionPanel);
            /* Dropdown to select pool
            ToolStripDropDownButton poolSelect = new ToolStripDropDownButton("PoolSelection");
            poolSelect.Alignment = System.Windows.Forms.ToolStripItemAlignment.Left;
            poolSelect.Name = "SelectPool";
            poolSelect.ToolTipText = "Select Pool";
            poolSelect.DropDownItems.Add(new ToolStripMenuItem("pool1"));
            poolSelect.DropDownItems.Add(new ToolStripMenuItem("pool2"));
            poolSelect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            poolSelect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            headerStrip1.Items.Add(poolSelect);
            */

            //Setting Panel at AZureDB instance
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
            {
                this.tableLayoutPanel.Controls.Clear();
                this.tableLayoutPanel.Controls.Add(this.responseTimePanel, 0, 0);
                this.tableLayoutPanel.Controls.Add(this.sessionsPanel, 0, 1);
                this.tableLayoutPanel.Controls.Add(this.blockedSessionsPanel, 1, 1);
                this.tableLayoutPanel.Controls.Add(this.maxSessionPanel, 0, 2);
                this.tableLayoutPanel.Controls.Add(this.lockStatisticsPanel, 1, 2);
                this.tableLayoutPanel.SetColumnSpan(this.responseTimePanel, 2);
                setBackgroundColor();
            }
            else if(ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                this.tableLayoutPanel.Controls.Clear();
                this.tableLayoutPanel.Controls.Add(this.responseTimePanel, 0, 0);
                this.tableLayoutPanel.Controls.Add(this.sessionsPanel, 0, 1);
                this.tableLayoutPanel.Controls.Add(this.blockedSessionsPanel, 1, 1);
                this.tableLayoutPanel.Controls.Add(this.maxSessionPanel, 0, 2);
                this.tableLayoutPanel.Controls.Add(this.lockStatisticsPanel, 1, 2);
                this.tableLayoutPanel.SetColumnSpan(this.responseTimePanel, 2);
                setBackgroundColor();
            }
            else
            {
                this.tableLayoutPanel.RowCount = 2;
                this.tableLayoutPanel.RowStyles.Clear();
                this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
                this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            }
            blockedSessionsChart.SetContextMenu(toolbarsManager);
            lockStatisticsChart.SetContextMenu(toolbarsManager);
            responseTimeChart.SetContextMenu(toolbarsManager);
            sessionsChart.SetContextMenu(toolbarsManager);
            maxSessionChart.SetContextMenu(toolbarsManager);

            InitializeDatabasesGrid();
            CreateChartDataSource();
            InitializeCharts();
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            AdaptFontSize();

            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        private void ScaleControlsAsPerResolution()
        {
            this.lockStatisticsChart.LegendBox.AutoSize = true;
        }
        #endregion

        #region properties

        /// <summary>
        /// Start datetime for custom range, otherwise null.
        /// </summary>
        public override DateTime? HistoricalStartDateTime
        {
            get { return historyData.HistoricalStartDateTime; }
            set
            {
                if (historyData.HistoricalStartDateTime != value)
                {
                    historyData.HistoricalStartDateTime = value;
                    currentHistoricalStartDateTime = null;
                }
            }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historyData.HistoricalSnapshotDateTime; }
            set
            {
                historyData.HistoricalSnapshotDateTime = value;
                currentHistoricalSnapshotDateTime = null;
            }
        }

        #endregion

        #region methods

        public override void ShowHelp() 
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.SessionsSummaryView);
        }

        public override void ApplySettings()
        {
            string chartType = Settings.Default.SessionsSummaryViewLocksChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in lockStatisticsOptionsButton.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureLockStatisticsChart(chartType);
                        lastLocksChartType = chartType;
                        break;
                    }
                }
            }
        }

        public override void SaveSettings()
        {
            // save all settings only if anything has changed
            if (string.Format(FORMAT_CHARTTYPE, lastLocksChartType) != lockStatisticsOptionsButton.Text)
            {
                lastLocksChartType =
                    Settings.Default.SessionsSummaryViewLocksChartType = lockStatisticsOptionsButton.Text.Substring(FORMAT_CHARTTYPE.Length - 3);
            }
        }

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime || HistoricalStartDateTime != currentHistoricalStartDateTime)
            {
                historyModeLoadError = null;
                base.RefreshView();

                //SQLdm 10.1 (pulkit puri) -- SQLDM-25894 fix
                //if (!isUserSysAdmin)
                //    ShowChartStatusMessage("No data is available for this view.", false);
                //else
                    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING, false);
                label2.Text = Idera.SQLdm.Common.Constants.LOADING;
            }
        }

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
            currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;
            if (HistoricalSnapshotDateTime == null)
            {
                if (!historyData.RealTimeSnapshotsPrePopulated)
                    historyData.PrePopulateRealTimeSnapshots(instanceId, this);
                else
                {
                    historyData.BackfillScheduledRefreshData(instanceId, this);

                    // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                    if (previousVisibleLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                        historyData.ForwardFillHistoricalData(instanceId, this);

                    // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                    if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                        historyData.BackFillScheduledHistoricalData(instanceId, this); historyData.BackFillScheduledHistoricalData(instanceId, this);
                }

                if (backgroundWorker.CancellationPending) return null;

                // this will refresh the current realtime data on entry and switching from history to realtime
                backgroundWorker.ReportProgress((int)Progress.Backfill, historyData);

                if (backgroundWorker.CancellationPending) return null;

                // go fetch the realtime snapshot
                return GetRealTimeSnapshots();
            }
            else
            {
                // If historical start/end did not change global, the snapshot is already populated.
                if (!historyData.HistoricalSnapshotsPopulated)
                    return historyData.PopulateHistoricalSnapshots(instanceId, this, backgroundWorker);
                else
                    return historyData.HistoricalSnapshots;
            }
        }

        protected override void UpdateProgress(object state, int progress)
        {
            using (Log.VerboseCall("UpdateProgress"))
            {
                switch (progress)
                {
                    case (int)Progress.Backfill:
                        UpdateChartDataFilter();
                        if (historyData.RealTimeSnapshotsDataTable.DefaultView.Count == 0) return;
                        operationalStatusPanel.Visible = false;
                        fillPanel.Visible = true;
                        ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                        ConfigureDataSourceForMaxSession(historyData.RealTimeSnapshotsDataTable);
                        ChartsVisible(true);
                        break;
                }
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                fillPanel.Visible = false;
            }

            base.HandleBackgroundWorkerError(e);
        }

        private ServerSummarySnapshots GetRealTimeSnapshots()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            var now = DateTime.Now;
            var timeDiff = historyData.LastServerOverviewSnapshot == null ? TimeSpan.MinValue : now - historyData.LastServerOverviewSnapshot.TimeStampLocal;

            ServerOverviewConfiguration serverOverviewSnapshotConfiguration;

            if (timeDiff > TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds))
            {
                Log.DebugFormat("Last ServerOverviewSnapshot point is too old with a time difference {0}", timeDiff);
                serverOverviewSnapshotConfiguration = new ServerOverviewConfiguration(instanceId);
            }
            else
            {
                serverOverviewSnapshotConfiguration = new ServerOverviewConfiguration(instanceId, historyData.LastServerOverviewSnapshot);
            }

            if (serverOverviewSnapshotConfiguration.LastRefresh == null)
            {
                ServerSummarySnapshots summarySnapshots = managementService.GetServerSummary(serverOverviewSnapshotConfiguration);
                if (summarySnapshots != null)
                {
                    serverOverviewSnapshotConfiguration = new ServerOverviewConfiguration(instanceId, summarySnapshots.ServerOverviewSnapshot);
                }
            }

            return managementService.GetServerSummary(serverOverviewSnapshotConfiguration);

        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (historyData.HistoricalSnapshotDateTime == null)
                {
                    UpdateWithRealTimeSnapshots(data as ServerSummarySnapshots);
                }
                else
                {
                    UpdateWithHistoricalSnapshots(data as ServerSummarySnapshots);
                }
            }
        }

        private void UpdateWithRealTimeSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            operationalStatusPanel.Visible = false;
            fillPanel.Visible = true;

            if (summarySnapshots != null)
            {
                historyData.AddRealTimeSnapshots(summarySnapshots);

                if (summarySnapshots.ServerOverviewSnapshot != null)
                {
                    if (summarySnapshots.ServerOverviewSnapshot.Error == null)
                    {
                        ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                        ConfigureDataSourceForMaxSession(historyData.RealTimeSnapshotsDataTable);
                        UpdateChartDataFilter();
                        ChartsVisible(true);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    else
                    {
                        ShowChartStatusMessage("No data available.", false);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, summarySnapshots.ServerOverviewSnapshot.Error));
                    }
                }
            }
        }

        private void UpdateWithHistoricalSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            if (summarySnapshots != null)
            {
                fillPanel.Visible = true;
                ConfigureChartDataSource(historyData.HistoricalSnapshotsDataTable);
                ConfigureDataSourceForMaxSession(historyData.HistoricalSnapshotsDataTable);
                ChartsVisible(true);
                ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                      Properties.Resources.StatusWarningSmall);
                currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                currentHistoricalStartDateTime = HistoricalStartDateTime;
            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                fillPanel.Visible = false;
            }

            ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }

        #endregion

        #region helpers

        #region charts

        private void InitializeCharts()
        {
            responseTimeChart.Tag = responseTimeHeaderStripLabel;
            responseTimeChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            responseTimeChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            responseTimeChart.Printer.Orientation = PageOrientation.Landscape;
            responseTimeChart.Printer.Compress = true;
            responseTimeChart.Printer.ForceColors = true;
            responseTimeChart.Printer.Document.DocumentName = "Response Time Chart";
            responseTimeChart.ToolBar.RemoveAt(0);
            responseTimeChart.ToolTipFormat = "%v ms\n%x";
            responseTimeChart.DataSource = historyData.RealTimeSnapshotsDataTable;
                     
            sessionsChart.Tag = sessionsHeaderStripLabel;
            sessionsChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            sessionsChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            sessionsChart.Printer.Orientation = PageOrientation.Landscape;
            sessionsChart.Printer.Compress = true;
            sessionsChart.Printer.ForceColors = true;
            sessionsChart.Printer.Document.DocumentName = "Sessions Chart";
            sessionsChart.ToolBar.RemoveAt(0);
            sessionsChart.ToolTipFormat = "%v %s sessions\n%x";
            sessionsChart.DataSource = historyData.RealTimeSnapshotsDataTable;

            lockStatisticsChart.Tag = lockStatisticsOptionsButton;
            lockStatisticsChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            lockStatisticsChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            lockStatisticsChart.LegendBox.PlotAreaOnly = false;
            lockStatisticsChart.LegendBox.Width = 100;
            lockStatisticsChart.Printer.Orientation = PageOrientation.Landscape;
            lockStatisticsChart.Printer.Compress = true;
            lockStatisticsChart.Printer.ForceColors = true;
            lockStatisticsChart.Printer.Document.DocumentName = "Session Locks Chart";
            lockStatisticsChart.ToolBar.RemoveAt(0);
            lockStatisticsChart.ToolTipFormat = "%v %s\n%x";
            lockStatisticsChart.DataSource = historyData.RealTimeSnapshotsDataTable;

            blockedSessionsChart.Tag = blockedSessionsHeaderStripLabel;
            blockedSessionsChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            blockedSessionsChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            blockedSessionsChart.Printer.Orientation = PageOrientation.Landscape;
            blockedSessionsChart.Printer.Compress = true;
            blockedSessionsChart.Printer.ForceColors = true;
            blockedSessionsChart.Printer.Document.DocumentName = "Sessions Blocking Chart";
            blockedSessionsChart.ToolBar.RemoveAt(0);
            blockedSessionsChart.ToolTipFormat = "%v %s\n%x";
            blockedSessionsChart.DataSource = historyData.RealTimeSnapshotsDataTable;

            maxSessionChart.Printer.Orientation = PageOrientation.Landscape;
            maxSessionChart.Printer.Compress = true;
            maxSessionChart.Printer.ForceColors = true;
            maxSessionChart.Printer.Document.DocumentName = "Max Sessions Chart";
            maxSessionChart.ToolTipFormat = "%v %s\n%x";
            // maxSessionChart.ToolTipFormat = string.Format("%s\n%v {0}\n%x", headerSelectTypeDropDownButton.Text);

            ConfigureCharts();
            ForceChartColors();
            drilldownProperties.ChartCursor = Cursors.Hand;
            InitalizeDrilldown(responseTimeChart, sessionsChart, lockStatisticsChart, blockedSessionsChart, maxSessionChart);  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void ForceChartColors()
        {
            sessionsChart.SetAreaSeriesAlphaChannel(175, 0);
        }
        private void setBackgroundColor()
        {
            this.maxSessionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.maxSessionPanel.Padding = new System.Windows.Forms.Padding(2, 0, 3, 1);
        }
        private void ConfigureCharts()
        {
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";

            responseTimeChart.DataSourceSettings.Fields.Clear();

            FieldMap responseTimeFieldMap = new FieldMap("ResponseTime", FieldUsage.Value);
            responseTimeFieldMap.DisplayName = "Response Time";
            responseTimeChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        dateFieldMap,
                        responseTimeFieldMap});

            responseTimeChart.DataSourceSettings.ReloadData();

            sessionsChart.DataSourceSettings.Fields.Clear();
            FieldMap systemSessionsFieldMap = new FieldMap("SystemSessions", FieldUsage.Value);
            systemSessionsFieldMap.DisplayName = "System";
            FieldMap idleSessionsFieldMap = new FieldMap("IdleSessions", FieldUsage.Value);
            idleSessionsFieldMap.DisplayName = "Idle";
            FieldMap activeSessionsFieldMap = new FieldMap("ActiveSessions", FieldUsage.Value);
            activeSessionsFieldMap.DisplayName = "Active";
            sessionsChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        dateFieldMap,
                        systemSessionsFieldMap,
                        idleSessionsFieldMap,
                        activeSessionsFieldMap});

            ChartFxExtensions.SetAreaSeriesAlphaChannel(sessionsChart, 175);

            sessionsChart.DataSourceSettings.ReloadData();

            blockedSessionsChart.DataSourceSettings.Fields.Clear();

            FieldMap blockedSessionsFieldMap = new FieldMap("BlockedProcesses", FieldUsage.Value);
            blockedSessionsFieldMap.DisplayName = "Blocked Sessions";
            FieldMap leadBlockersFieldMap = new FieldMap("LeadBlockers", FieldUsage.Value);
            leadBlockersFieldMap.DisplayName = "Lead Blockers";
            FieldMap deadlockedFieldMap = new FieldMap("Total Deadlocks", FieldUsage.Value);
            deadlockedFieldMap.DisplayName = "Total Deadlocks";

            blockedSessionsChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
                        dateFieldMap,
                        blockedSessionsFieldMap,
                        leadBlockersFieldMap,
                        deadlockedFieldMap});

            blockedSessionsChart.DataSourceSettings.ReloadData();
            ConfigureDatabasesChart();
            ConfigureLockStatisticsChart("Requests");
        }

        private void ConfigureDatabasesGrid(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDatabasesGrid"))
            {
                if (selectedDatabases.Count == 0)
                    return;
                topDatabasesGrid.SuspendLayout();

                if (dataSource == null || dataSource.DefaultView == null)
                    return;


                DataView sourceView = new DataView(dataSource);
                DataTable lastSnapshot = sourceView.Table.Clone();

                // go backwards through the rows
                if (sourceView.Count > 0)
                {
                    lastSnapshot.BeginLoadData();
                    DataRowView lastRow = sourceView[sourceView.Count - 1];
                    DateTime match = (DateTime)lastRow["Date"];

                    for (int i = sourceView.Count - 1; i >= 0; i--)
                    {
                        DataRowView row = sourceView[i];
                        if (match.Equals(row["Date"]))
                            lastSnapshot.ImportRow(row.Row);
                        else
                            break;
                    }
                    lastSnapshot.EndLoadData();
                }

                DataTable dataTable = new DataTable("AzureCloudMetricsMaxSession");
                
                DataColumn databaseColumn = new DataColumn();
                databaseColumn.DataType = System.Type.GetType("System.String");
                databaseColumn.ColumnName = "Database";
                databaseColumn.Caption = "Database";
                databaseColumn.ReadOnly = true;
                databaseColumn.Unique = true;
                dataTable.Columns.Add(databaseColumn);

                DataColumn metricValueColumn = new DataColumn();
                metricValueColumn.DataType = System.Type.GetType("System.Decimal");
                metricValueColumn.ColumnName = "MaxSessionPct";
                metricValueColumn.Caption = "MaxSessionPct";
                metricValueColumn.ReadOnly = false;
                metricValueColumn.Unique = false;
                dataTable.Columns.Add(metricValueColumn);

                DataView dataView = new DataView(dataSource);
                foreach (DataRowView row in dataView)
                {
                    if (row["AzureCloudMetrics"] != DBNull.Value && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Count > 0)
                    {
                        Dictionary<string, Dictionary<string, object>>.KeyCollection keyColl =
                                        ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Keys;
                        foreach (string dbName in keyColl)
                        {
                            bool exists = false;
                            foreach (DataRow dr in dataTable.Rows) // search whole table
                            {
                                if (dr["Database"].Equals(dbName))
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("MaxSessionPercent"))
                                    {
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["MaxSessionPercent"] != DBNull.Value)
                                        {
                                            dr["MaxSessionPct"] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["MaxSessionPercent"]);
                                            exists = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!exists)
                            {
                                DataRow theDataRow = dataTable.NewRow();

                                // Add data
                                if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(dbName) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName].ContainsKey("MaxSessionPercent"))
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["MaxSessionPercent"] != DBNull.Value)
                                    {
                                        theDataRow[0] = dbName;
                                        theDataRow[1] = Convert.ToDecimal(((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[dbName]["MaxSessionPercent"]);
                                        dataTable.Rows.Add(theDataRow);
                                        dataTable.AcceptChanges();
                                    }
                                    
                                }
                            }

                        }
                    }
                }
                DataSet gridDataSet = new DataSet();
                DataView dv = dataTable.DefaultView;
                dv.Sort = "MaxSessionPct desc";
                gridDataSet.Tables.Add(dv.ToTable());
                topDatabasesGrid.DataSource = gridDataSet;
                foreach (UltraGridColumn col in topDatabasesGrid.DisplayLayout.Bands[0].Columns)
                {
                    col.Hidden = false;
                }
                                
                topDatabasesGrid.ResumeLayout(); 
            }
        }


        internal void ConfigureDataSourceForMaxSession(DataTable dataSource)
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {


                try
                {
                    top5DataTable.BeginLoadData();
                    top5DataTable.Rows.Clear();

                    // SQLDM 10.2 (Anshul Aggarwal) - Added null checks
                    // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
                    DataTable dataTableMetrics = dataSource;

                    if (dataTableMetrics != null)
                    {
                        DataView currentData = new DataView(dataTableMetrics);

                        DateTime top5RowDate = DateTime.MinValue;
                        DataRow top5Row = null;

                        foreach (DataRowView row in currentData)
                        {
                            row.Row.Table.Columns.Contains("AzureCloudMetrics");
                            if (row["AzureCloudMetrics"] != DBNull.Value && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Count > 0)
                            {
                                DateTime date = (DateTime)row["Date"];
                                if (top5Row == null || date != top5RowDate)
                                {
                                    top5Row = top5DataTable.NewRow();
                                    top5Row["Date"] = date;
                                    if (top5DataTable.Rows.Contains(date))
                                    {
                                        top5DataTable.Rows.Remove(top5DataTable.Rows.Find(date));
                                    }
                                    top5DataTable.Rows.Add(top5Row);
                                    top5RowDate = date;
                                }

                                Dictionary<string, Dictionary<string, object>>.KeyCollection keyColl =
                                        ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).Keys;
                                foreach (string s in keyColl)
                                {
                                    if (!uniqueDatabases.Contains(s))
                                    {
                                        selectedDatabases.Add(s);
                                        uniqueDatabases.Add(s);
                                    }
                                }
                                
                                for (int i = 0; i < (selectedDatabases.Count > 5 ? 5 : selectedDatabases.Count); i++)
                                {
                                    if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"]).ContainsKey(selectedDatabases[i]) && ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]].ContainsKey("MaxSessionPercent"))
                                    {
                                        if (((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]]["MaxSessionPercent"] != DBNull.Value)
                                        {
                                            top5Row["MaxSession" + (i + 1)] = ((Dictionary<string, Dictionary<string, object>>)row["AzureCloudMetrics"])[selectedDatabases[i]]["MaxSessionPercent"];
                                            dbValMap["MaxSession" + (i + 1)] = selectedDatabases[i];
                                        }
                                    }
                                }
                            }
                            
                        }
                    }

                    // update the grid
                    ConfigureDatabasesGrid(dataSource);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurrred determining the top 5 databases", ex);
                }
                finally
                {
                    top5DataTable.EndLoadData();
                }
                maxSessionChart.DataSource = top5DataTable;
                maxSessionChart.DataSourceSettings.ReloadData();
                //ChartFxExtensions.SetAxisXTimeScale(maxSessionChart, 2);
                ConfigureDatabasesChart();

                Invalidate();
            }
        }
        private void maxSessionChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 3;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
                chart.Invalidate();
            }
        }
        private void ConfigureDatabasesChart()
        {
            if (selectedDatabases.Count == 0)
            {
                maxSessionChart.Visible = false;
                label2.Text = Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE;
                return;
            }
            else
            {
                maxSessionChart.Visible = true;
            }
            maxSessionChart.SuspendLayout();
            maxSessionChart.DataSourceSettings.Fields.Clear();
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            maxSessionChart.DataSourceSettings.Fields.Add(dateFieldMap);

            if (selectedDatabases.Count > 0)
            {
                for (int idx = 1; idx <= (selectedDatabases.Count <= 5 ? selectedDatabases.Count : 5); idx++)
                {
                    if (dbValMap.ContainsKey("MaxSession" + idx))
                    {
                        string database = selectedDatabases[idx - 1];
                        FieldMap fieldMap = new FieldMap("MaxSession" + idx, FieldUsage.Value);
                        fieldMap.DisplayName = dbValMap["MaxSession" + idx];
                        maxSessionChart.DataSourceSettings.Fields.Add(fieldMap);
                    }
                }

                maxSessionChart.DataSourceSettings.ReloadData();
                maxSessionChart_Resize(maxSessionChart, new EventArgs());
                int showDecimals = 4;
                maxSessionChart.AxisY.DataFormat.Decimals = showDecimals;
                maxSessionChart.Invalidate();
            }
            maxSessionChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            maxSessionChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            maxSessionChart.ResumeLayout();
        }

        private void ConfigureLockStatisticsChart(string lockType)
        {
            lockStatisticsOptionsButton.Text = string.Format(FORMAT_CHARTTYPE, lockType);

            if (lockType.IndexOf("Wait Time") > -1)
            {
                lockStatisticsChart.ToolTipFormat = string.Format("%s \n%v ms\n%x", lockType);
            }
            else
            {
                lockStatisticsChart.ToolTipFormat = string.Format("%v %s {0}\n%x", lockType);
            }
            
            lockStatisticsChart.DataSourceSettings.Fields.Clear();
            lockStatisticsPanel.Visible = true;
           
            FieldMap allocUnitFieldMap = new FieldMap(string.Format("{0} - AllocUnit", lockType), FieldUsage.Value);
            allocUnitFieldMap.DisplayName = "AllocUnit";
            FieldMap applicationFieldMap = new FieldMap(string.Format("{0} - Application", lockType), FieldUsage.Value);
            applicationFieldMap.DisplayName = "Application";
            FieldMap databaseFieldMap = new FieldMap(string.Format("{0} - Database", lockType), FieldUsage.Value);
            databaseFieldMap.DisplayName = "Database";
            FieldMap extentFieldMap = new FieldMap(string.Format("{0} - Extent", lockType), FieldUsage.Value);
            extentFieldMap.DisplayName = "Extent";
            FieldMap fileFieldMap = new FieldMap(string.Format("{0} - File", lockType), FieldUsage.Value);
            fileFieldMap.DisplayName = "File";
            FieldMap hobtFieldMap = new FieldMap(string.Format("{0} - HoBT", lockType), FieldUsage.Value);
            hobtFieldMap.DisplayName = "HoBT";
            FieldMap keyFieldMap = new FieldMap(string.Format("{0} - Key", lockType), FieldUsage.Value);
            keyFieldMap.DisplayName = "Key";
            FieldMap latchFieldMap = new FieldMap(string.Format("{0} - Latch", lockType), FieldUsage.Value);
            latchFieldMap.DisplayName = "Latch";
            FieldMap metadataFieldMap = new FieldMap(string.Format("{0} - Metadata", lockType), FieldUsage.Value);
            metadataFieldMap.DisplayName = "Metadata";
            FieldMap objectFieldMap = new FieldMap(string.Format("{0} - Object", lockType), FieldUsage.Value);
            objectFieldMap.DisplayName = "Object";
            FieldMap pageFieldMap = new FieldMap(string.Format("{0} - Page", lockType), FieldUsage.Value);
            pageFieldMap.DisplayName = "Page";
            FieldMap ridFieldMap = new FieldMap(string.Format("{0} - RID", lockType), FieldUsage.Value);
            ridFieldMap.DisplayName = "RID";
            FieldMap tableFieldMap = new FieldMap(string.Format("{0} - Table", lockType), FieldUsage.Value);
            tableFieldMap.DisplayName = "Table";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";

            lockStatisticsChart.DataSourceSettings.Fields.AddRange(new FieldMap[] {
            allocUnitFieldMap,
            applicationFieldMap,
            databaseFieldMap,
            extentFieldMap,
            fileFieldMap,
            hobtFieldMap,
            keyFieldMap,
            latchFieldMap,
            metadataFieldMap,
            objectFieldMap,
            pageFieldMap,
            ridFieldMap,
            tableFieldMap,
            dateFieldMap});

            lockStatisticsChart.DataSourceSettings.ReloadData();
        }

        private void ToggleChartToolbar(Chart chart, bool Visible)
        {
            chart.ToolBar.Visible = Visible;
        }

        private void PrintChart(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
        }

        private void SaveChartData(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
        }

        private void SaveChartImage(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
        }

        private void UpdateChartDataFilter()
        {
            if (historyData.RealTimeSnapshotsDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                historyData.RealTimeSnapshotsDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                historyData.RealTimeSnapshotsDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
        }

        private void ConfigureChartDataSource(DataTable dataSource)
        {
            if (responseTimeChart.DataSource == dataSource) return;

            responseTimeChart.DataSource =
                sessionsChart.DataSource =
                lockStatisticsChart.DataSource =
                blockedSessionsChart.DataSource =  dataSource;
                 
            ForceChartColors();
        }

        #endregion

        #endregion

        #region events

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChartDataFilter();
                    break;
            }
        }

        private void averageWaitTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Average Wait Time");
        }

        private void deadlocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Deadlocks");
        }

        private void requestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Requests");
        }

        private void timeoutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Timeouts");
        }

        private void waitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Waits");
        }

        private void waitTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureLockStatisticsChart("Wait Time");
        }

        private void MaximizeChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            tableLayoutPanel.Visible = false;
            tableLayoutPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = false;
            restoreButton.Visible = true;
            chartContainerPanel.Controls.Add(chartPanel);
        }

        private void RestoreChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton,
                                  int column, int row)
        {
            chartContainerPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = true;
            restoreButton.Visible = false;
            tableLayoutPanel.Controls.Add(chartPanel);
            tableLayoutPanel.SetCellPosition(chartPanel, new TableLayoutPanelCellPosition(column, row));
            tableLayoutPanel.Visible = true;
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
            {
                this.tableLayoutPanel.Controls.Clear();
                this.tableLayoutPanel.Controls.Add(this.responseTimePanel, 0, 0);
                this.tableLayoutPanel.Controls.Add(this.sessionsPanel, 0, 1);
                this.tableLayoutPanel.Controls.Add(this.blockedSessionsPanel, 1, 1);
                this.tableLayoutPanel.Controls.Add(this.maxSessionPanel, 0, 2);
                this.tableLayoutPanel.Controls.Add(this.lockStatisticsPanel, 1, 2);
                this.tableLayoutPanel.SetColumnSpan(this.responseTimePanel, 2);
                setBackgroundColor();
            }
            else if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != null && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                this.tableLayoutPanel.Controls.Clear();
                this.tableLayoutPanel.Controls.Add(this.responseTimePanel, 0, 0);
                this.tableLayoutPanel.Controls.Add(this.sessionsPanel, 0, 1);
                this.tableLayoutPanel.Controls.Add(this.blockedSessionsPanel, 1, 1);
                this.tableLayoutPanel.Controls.Add(this.maxSessionPanel, 0, 2);
                this.tableLayoutPanel.Controls.Add(this.lockStatisticsPanel, 1, 2);
                this.tableLayoutPanel.SetColumnSpan(this.responseTimePanel, 2);
                setBackgroundColor();
            }
            else
            {
                this.tableLayoutPanel.RowCount = 2;
                this.tableLayoutPanel.RowStyles.Clear();
                this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
                this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));

            }
        }

        private void maximizeResponseTimeChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(responseTimePanel, maximizeResponseTimeChartButton, restoreResponseTimeChartButton);
        }

        private void restoreResponseTimeChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(responseTimePanel, maximizeResponseTimeChartButton, restoreResponseTimeChartButton, 0, 0);
        }

        private void maximizeMaxSessionChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(maxSessionPanel, maximizeMaxSessionChartButton, restoreMaxSessionChartButton);
        }

        private void restoreMaxSessionChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(maxSessionPanel, maximizeMaxSessionChartButton, restoreMaxSessionChartButton, 0,2);
        }

        private void maximizeSessionsChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(sessionsPanel, maximizeSessionsChartButton, restoreSessionsChartButton);
        }

        private void restoreSessionsChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(sessionsPanel, maximizeSessionsChartButton, restoreSessionsChartButton, 1, 0);
        }

        private void maximizeLockStatisticsChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(lockStatisticsPanel, maximizeLockStatisticsChartButton, restoreLockStatisticsChartButton);
        }

        private void restoreLockStatisticsChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(lockStatisticsPanel, maximizeLockStatisticsChartButton, restoreLockStatisticsChartButton, 0, 1);
        }

        private void maximizeBlockedSessionsChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(blockedSessionsPanel, maximizeBlockedSessionsChartButton, restoreBlockedSessionsChartButton);
        }

        private void restoreBlockedSessionsChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(blockedSessionsPanel, maximizeBlockedSessionsChartButton, restoreBlockedSessionsChartButton, 1, 1);
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            if (contextMenuSelectedChart != null)
            {
                switch (e.Tool.Key)
                {
                    case "toggleChartToolbarButton":
                        ToggleChartToolbar(contextMenuSelectedChart, ((StateButtonTool)e.Tool).Checked);
                        break;
                    case "printChartButton":
                        PrintChart(contextMenuSelectedChart);
                        break;
                    case "exportChartDataButton":
                        SaveChartData(contextMenuSelectedChart);
                        break;
                    case "exportChartImageButton":
                        SaveChartImage(contextMenuSelectedChart);
                        break;
                }
            }
            contextMenuSelectedChart = null;
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
                    contextMenuSelectedChart = chart;
                    break;
            }
        }

        #region Chart Click Events

        private void OnChartMouseClick(object sender, HitTestEventArgs e)
        {
            if (e.Button != MouseButtons.Right && e.HitType != HitType.Other
                && !ChartHelper.IsMouseInsideChartArea(e))//SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            {
                // hit type of other means they clicked on the chart toolbar
                ServerViews targetView;
                if (sender == responseTimeChart)
                    targetView = ServerViews.SessionsDetails;
                else if (sender == sessionsChart)
                    targetView = ServerViews.SessionsDetails;
                else if (sender == lockStatisticsChart)
                    targetView = ServerViews.SessionsLocks;
                else if (sender == blockedSessionsChart)
                    targetView = ServerViews.SessionsBlocking;
                else
                    return;

                ApplicationController.Default.ShowServerView(instanceId, targetView);
            }
        }

        #endregion

        private void SessionsSummaryView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        #endregion

        #region Operational Status

        private void ShowOperationalStatus(string message, Image icon)
        {
            operationalStatusImage.Image = icon;
            operationalStatusLabel.Text = string.Format(message, message);
            operationalStatusPanel.Visible = true;
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
            operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.White;
            operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
            operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);

            // Switching to real-time mode is the only supported opertional action at this time
            SwitchToRealTimeMode();
        }

        private void SwitchToRealTimeMode()
        {
            operationalStatusPanel.Visible = false;
            //SQLdm 10.1 (pulkit puri) -- SQLDM-25894 fix
            //if (!isUserSysAdmin)
            //    ShowChartStatusMessage("No data is available for this view.", true );
            //else
                ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING, true);
            label2.Text = Idera.SQLdm.Common.Constants.LOADING;
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void ChartsVisible(bool value)
        {
            responseTimeChart.Visible =
                sessionsChart.Visible =
                lockStatisticsChart.Visible =
                blockedSessionsChart.Visible = value;
        }

        private void ShowChartStatusMessage(string message, bool forceHide)
        {
            if (forceHide)
            {
                ChartsVisible(false);
            }

            responseTimeChartStatusLabel.Text =
                sessionsChartStatusLabel.Text = 
                    label2.Text = 
                lockStatisticsChartStatusLabel.Text =
                blockedProcessesChartStatusLabel.Text = message;
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (historyModeLoadError == null)
            {
                SwitchToRealTimeMode();
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while loading a historical snapshot.",
                                                historyModeLoadError, false);
            }
        }

        #endregion

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
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
            themeManager.updateGridTheme(this.topDatabasesGrid);
        }

        private void tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void InitializeDatabasesGrid()
        {
            topDatabasesGrid.DrawFilter = new PercentageBackgroundDrawFilter();
        }

        private void CreateChartDataSource()
        {

            top5DataTable = new DataTable();
            top5DataTable.Columns.Add("Date", typeof(DateTime));

            top5DataTable.Columns.Add("MaxSession1", typeof(decimal));
            top5DataTable.Columns.Add("MaxSession2", typeof(decimal));
            top5DataTable.Columns.Add("MaxSession3", typeof(decimal));
            top5DataTable.Columns.Add("MaxSession4", typeof(decimal));
            top5DataTable.Columns.Add("MaxSession5", typeof(decimal));
            
            top5DataTable.PrimaryKey = new DataColumn[] { top5DataTable.Columns["Date"] };

            top5DataTable.DefaultView.Sort = "Date";
        }
    }
    internal sealed class PercentageBackgroundDrawFilter : IUIElementDrawFilter
    {
        private readonly IUIElementDrawFilter previousFilter;

        internal PercentageBackgroundDrawFilter()
        {
            this.previousFilter = null;
        }

        internal PercentageBackgroundDrawFilter(IUIElementDrawFilter previousFilter)
        {
            this.previousFilter = previousFilter;
        }

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {
            if (drawPhase == DrawPhase.BeforeDrawFocus ||
                drawPhase == DrawPhase.BeforeDrawForeground)
            {
                return true;
            }

            if (drawPhase == DrawPhase.BeforeDrawBackColor)
            {
                if (drawParams.Element is EditorWithTextDisplayTextUIElement && drawParams.Element.Parent.Parent is CellUIElement)
                {
                    CellUIElement cell = (CellUIElement)drawParams.Element.Parent.Parent;
                    if (cell.Column.Key == "ValueBar")
                    {
                        Rectangle rect = (Rectangle)drawParams.Element.RectInsideBorders;
                        using (Brush brush = new SolidBrush(Color.White))
                        {
                            drawParams.Graphics.FillRectangle(brush, rect);
                        }

                        rect.Width = Convert.ToInt32(rect.Width * Convert.ToDouble(cell.Cell.Value));
                        if (rect.Width > 0)
                        {
                            //drawParams.Graphics.FillRectangle(drawParams.BackBrush, rect);
                            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, cell.Column.CellAppearance.BackColor, cell.Column.CellAppearance.BackColor2, LinearGradientMode.Vertical))
                            {
                                drawParams.Graphics.FillRectangle(brush, rect);
                            }
                        }
                    }
                }
                return true;
            }

            if (previousFilter != null)
                return previousFilter.DrawElement(drawPhase, ref drawParams);

            return false;
        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {
            DrawPhase result = DrawPhase.BeforeDrawFocus;

            if (drawParams.Element is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (drawParams.Element is EditorWithTextUIElement && drawParams.Element.Parent is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element.Parent;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (drawParams.Element is EditorWithTextDisplayTextUIElement && drawParams.Element.Parent.Parent is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element.Parent.Parent;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (previousFilter != null)
                result |= previousFilter.GetPhasesToFilter(ref drawParams);

            return result;
        }
    }
}