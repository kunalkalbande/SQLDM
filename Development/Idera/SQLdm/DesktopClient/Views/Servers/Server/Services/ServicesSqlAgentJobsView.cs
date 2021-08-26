using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Services
{
    using Infragistics.Win.UltraWinDataSource;
    using Idera.SQLdm.Common.Events;

    internal partial class ServicesSqlAgentJobsView : ServerBaseView, IShowFilterDialog
    {
        #region constants

        
        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
		///Ankit Nagpal --Sqldm10.0.0
        private const string SYSADMIN_MESSAGE = @"No data is available for this view.";
        private const string SELECT_JOBS = @"Select one or more jobs.";

        #endregion

        #region fields

        private DateTime? historicalSnapshotDateTime;
        private bool initialized = false;
        private bool initializedHistory = false;
        private DataTable currentDataTable;
        private DataSet historyDataSet;
        private AgentJobSummaryConfiguration configuration;
        private AgentJobHistoryConfiguration historyConfiguration;
        private AgentJobFilter agentJobFilter;
        private AgentJobSummary currentSnapshot = null;
        SortedList<Guid, Guid> selectedJobs = new SortedList<Guid, Guid>();
        private bool selectionChanged = false;
        private ServiceState serviceState = ServiceState.UnableToMonitor;

        private static readonly object updateLock = new object();
        private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private UltraGrid selectedGrid = null;

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private GridSettings lastHistoryGridSettings = null;
        private bool lastHistoryVisible = true;
        #endregion

        #region constructors

        public ServicesSqlAgentJobsView(int instanceId) : base(instanceId)
        {
            InitializeComponent();
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;
            HideFocusRectangleDrawFilter hideFocusRectangleDrawFilter = new HideFocusRectangleDrawFilter();
            jobSummaryGrid.DrawFilter = hideFocusRectangleDrawFilter;
            jobHistoryGrid.DrawFilter = hideFocusRectangleDrawFilter;

            jobSummaryGrid.Tag = "SQL Agent Job Summary";
            jobHistoryGrid.Tag = jobHistoryHeaderStripDropDownButton;

            agentJobFilter = new AgentJobFilter();
            configuration = new AgentJobSummaryConfiguration(instanceId, agentJobFilter.JobSummaryFilter, agentJobFilter.FilterTimeSpan);
            historyConfiguration = new AgentJobHistoryConfiguration(instanceId, agentJobFilter.ShowFailedOnly);

            // load value lists for grid display
            ValueListItem listItem;

            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(JobRunStatus.Executing, "Executing");
            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobRunStatus.WaitingForThread, "Waiting For Thread");
            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobRunStatus.BetweenRetries, "Between Retries");
            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobRunStatus.NotRunning, "Not Running");
            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobRunStatus.Suspended, "Suspended");
            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobRunStatus.WaitingForStepCompletion, "Waiting For Step Completion");
            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobRunStatus.CleanUp, "Clean Up");
            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobRunStatus.Unknown, "Unknown");
            jobSummaryGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);

            jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(JobStepRunStatus.Failed, "Failed");
            jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobStepRunStatus.Succeeded, "Succeeded");
            jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobStepRunStatus.Cancelled, "Cancelled");
            jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobStepRunStatus.Retry, "Retry");
            jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobStepRunStatus.InProgress, "In Progress");
            jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobStepRunStatus.NotYetRun, "Not Yet Run");
            jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(JobStepRunStatus.Unknown, "Unknown");
            jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"].ValueListItems.Add(listItem);

            jobSummaryGrid.DisplayLayout.ValueLists["yesNoValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(true, "Yes");
            jobSummaryGrid.DisplayLayout.ValueLists["yesNoValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(false, "No");
            jobSummaryGrid.DisplayLayout.ValueLists["yesNoValueList"].ValueListItems.Add(listItem);

            ValueListHelpers.CopyValueList(jobSummaryGrid.DisplayLayout.ValueLists["outcomeValueList"],
                jobHistoryGrid.DisplayLayout.ValueLists["outcomeValueList"]);

            jobSummaryGridStatusLabel.Text =
                jobHistoryGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            jobSummaryGrid.Visible =
                jobHistoryGrid.Visible = false;

            InitializeCurrentDataTable();
            InitializeHistoryDataSet();
            AdaptFontSize();
        }


        #endregion

        #region Properties

        public event EventHandler FilterChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler HistoryPanelVisibleChanged;
        public event EventHandler ServiceActionAllowedChanged;
        public event EventHandler JobSelectionChanged;
        public event EventHandler UpdateDataCompleted;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        /// <summary>
        /// Get or Set the Agent Service state and trigger state update event if changed
        /// </summary>
        public ServiceState AgentServiceState
        {
            get
            {
                return serviceState;
            }
            private set
            {
                serviceState = value;

                if (ServiceActionAllowedChanged != null)
                {
                    ServiceActionAllowedChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the availability of starting the service determined by its current state
        /// </summary>
        public bool StartAllowed
        {
            get
            {
                bool clustered = ApplicationModel.Default.ActiveInstances[instanceId].Instance.ActiveClusterNode != null;
                return !clustered && serviceState == ServiceState.Stopped;
            }
        }

        /// <summary>
        /// Get the availability of stopping the service determined by its current state
        /// </summary>
        public bool StopAllowed
        {
            get
            {
                bool clustered = ApplicationModel.Default.ActiveInstances[instanceId].Instance.ActiveClusterNode != null;
                return !clustered && serviceState == ServiceState.Running;
            }
        }

        /// <summary>
        /// Get or Set the Databases Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool GridGroupByBoxVisible
        {
            get { return !jobSummaryGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                jobSummaryGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current database configuration settings to manage state for current selections
        /// </summary>
        public AgentJobFilter Configuration
        {
            get { return agentJobFilter; }
        }

        public bool HistoryPanelVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;

                if (HistoryPanelVisibleChanged != null)
                {
                    HistoryPanelVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public IList<Guid> SelectedJobs
        {
            get { return selectedJobs.Keys; }
        }

        #endregion

        #region methods


        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServicesSqlAgentJobsView);
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.ServicesAgentJobsViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            lastHistoryVisible =
                HistoryPanelVisible = Settings.Default.ServicesAgentJobsViewHistoryVisible;


            if (Settings.Default.ServicesAgentJobsViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.ServicesAgentJobsViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, jobSummaryGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }

            if (Settings.Default.ServicesAgentJobsViewHistoryGrid is GridSettings)
            {
                lastHistoryGridSettings = Settings.Default.ServicesAgentJobsViewHistoryGrid;
                GridSettings.ApplySettingsToGrid(lastHistoryGridSettings, jobHistoryGrid);
            }
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(jobSummaryGrid);
            GridSettings historyGridSettings = GridSettings.GetSettings(jobHistoryGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || lastHistoryVisible != HistoryPanelVisible
                || !mainGridSettings.Equals(lastMainGridSettings)
                || !historyGridSettings.Equals(lastHistoryGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.ServicesAgentJobsViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastHistoryVisible =
                    Settings.Default.ServicesAgentJobsViewHistoryVisible = HistoryPanelVisible;
                lastMainGridSettings =
                    Settings.Default.ServicesAgentJobsViewMainGrid = mainGridSettings;
                lastHistoryGridSettings =
                    Settings.Default.ServicesAgentJobsViewHistoryGrid = historyGridSettings;
            }
        }

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                ServicesSqlAgentJobsView_Fill_Panel.Visible = true;
                base.RefreshView();
            }
            else
            {
                ServicesSqlAgentJobsView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            Pair<AgentJobSummary, AgentJobHistorySnapshot> configs = new Pair<AgentJobSummary, AgentJobHistorySnapshot>();

            if (!selectionChanged)
            {
                configs.First = managementService.GetAgentJobSummary(configuration);

            }

            List<Guid> configJobs = new List<Guid>();
            List<Guid> deleteItems = new List<Guid>();
            if (selectedJobs.Count > 0)
            {
                foreach (Guid jobId in selectedJobs.Values)
                {
                    if (configs.First == null || configs.First.Jobs.ContainsKey(jobId))
                    {
                        configJobs.Add(jobId);
                    }
                    else
                    {
                        deleteItems.Add(jobId);
                    }
                }
            }
            foreach (Guid item in deleteItems)
            {
                selectedJobs.Remove(item);
            }
            if (configJobs.Count == 0)
            {
                configJobs.Add(new Guid());
            }
            historyConfiguration.JobIdList = configJobs;
            configs.Second = managementService.GetAgentJobHistory(historyConfiguration);

            return configs;
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            jobSummaryGridStatusLabel.Text =
                jobHistoryGridStatusLabel.Text = UNABLE_TO_UPDATE;
			///Ankit Nagpal --Sqldm10.0.0
			///If not a sysadmin display sysadmin message
            //if (!isUserSysAdmin) jobSummaryGridStatusLabel.Text =
            //      jobHistoryGridStatusLabel.Text = SYSADMIN_MESSAGE;
            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            Pair<AgentJobSummary, AgentJobHistorySnapshot> snapshotPair;
            Exception e = null;

            if (data != null && data is Pair<AgentJobSummary, AgentJobHistorySnapshot>)
            {
                snapshotPair = (Pair<AgentJobSummary, AgentJobHistorySnapshot>)data;
                if (snapshotPair.First != null)
                {
                    AgentJobSummary snapshot = snapshotPair.First as AgentJobSummary;
                    lock (updateLock)
                    {
                        if (snapshot.Error == null)
                        {
                            // set the current agent service state
                            AgentServiceState = snapshot.AgentServiceState;

                            if (snapshot.Jobs != null && snapshot.Jobs.Count > 0)
                            {
                                currentDataTable.BeginLoadData();

                                // remove any databases that have been deleted
                                List<DataRow> deleteRows = new List<DataRow>();
                                foreach (DataRow row in currentDataTable.Rows)
                                {
                                    if (!snapshot.Jobs.ContainsKey((Guid)row["JobId"]))
                                    {
                                        deleteRows.Add(row);
                                    }
                                }
                                foreach (DataRow row in deleteRows)
                                {
                                    currentDataTable.Rows.Remove(row);
                                }

                                //now update any matching databases or add new ones
                                foreach (AgentJob job in snapshot.Jobs.Values)
                                {
                                    currentDataTable.LoadDataRow(
                                            new object[]
                                    {
                                        job.JobId,
                                        job.JobName,
                                        job.Category,
                                        job.Enabled,
                                        job.Scheduled,
                                        job.Status,
                                        job.LastRunStatus,
                                        job.LastRunEndTime.HasValue ? (object)((DateTime)job.LastRunEndTime).ToLocalTime() : null,
                                        (job.RunDuration.TotalSeconds > 0 || job.LastRunEndTime.HasValue) ? (object)job.RunDuration : null,
                                        job.NextRunDate.HasValue ? (object)((DateTime)job.NextRunDate).ToLocalTime() : null,
                                        job.Owner,
                                        job.JobDescription
                                    }, true);
                                }
                                currentDataTable.EndLoadData();

                                if (!initialized)
                                {
                                    if (lastMainGridSettings != null)
                                    {
                                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, jobSummaryGrid);

                                        initialized = true;
                                    }
                                    else if (snapshot.Jobs.Count > 0)
                                    {
                                        foreach (UltraGridColumn column in jobSummaryGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                            column.Width = Math.Min(column.Width, jobSummaryGrid.Width / 2);
                                        }

                                        initialized = true;
                                    }

                                    if (jobSummaryGrid.Rows.Count > 0 && jobSummaryGrid.Selected.Rows.Count == 0)
                                    {
                                        jobSummaryGrid.Rows[0].Selected = true;
                                    }
                                }
                                currentSnapshot = snapshot;

                                jobSummaryGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(false);
                                jobSummaryGrid.Visible = true;

                                ApplicationController.Default.SetCustomStatus(
                                    agentJobFilter.IsFiltered() ? "Filter Applied" : string.Empty,
                                    String.Format("Jobs: {0} Item{1}",
                                            currentDataTable.Rows.Count,
                                            currentDataTable.Rows.Count == 1 ? string.Empty : "s")
                                    );
                            }
                            else
                            {
                                currentDataTable.Clear();
                                jobSummaryGridStatusLabel.Text = NO_ITEMS;
								///Ankit Nagpal --Sqldm10.0.0
								///If not a sysadmin display sysadmin message
                                //if (!isUserSysAdmin) jobSummaryGridStatusLabel.Text = SYSADMIN_MESSAGE;
                                jobSummaryGrid.Visible = false;
                                jobHistoryGridStatusLabel.Text = SELECT_JOBS;
                                jobHistoryGrid.Visible = false;
                                ApplicationController.Default.ClearCustomStatus();
                            }

                            jobSummaryGrid.ResumeLayout();
                        }
                        else
                        {
                            e = snapshot.Error;
                            AgentServiceState = ServiceState.UnableToMonitor;
                            jobSummaryGrid.Visible = false;
                            jobSummaryGridStatusLabel.Text = UNABLE_TO_UPDATE;
							///Ankit Nagpal --Sqldm10.0.0
							///If not a sysadmin display sysadmin message
                            //if (!isUserSysAdmin) jobSummaryGridStatusLabel.Text = SYSADMIN_MESSAGE;
                            ApplicationController.Default.ClearCustomStatus();
                        }
                    }
                }

                if (snapshotPair.Second != null)
                {
                    lock (updateLock)
                    {
                        AgentJobHistorySnapshot snapshot = snapshotPair.Second as AgentJobHistorySnapshot;

                        if (snapshot.Error == null)
                        {
                            DataTable historyDataTable = historyDataSet.Tables["Job Executions"];
                            DataTable stepDataTable = historyDataSet.Tables["Job Steps"];
                            // Load Files grid data
                            historyDataTable.BeginLoadData();
                            stepDataTable.BeginLoadData();

                            // remove any files that are no longer in view
                            List<DataRow> deleteRows = new List<DataRow>();
                            foreach (DataRow row in historyDataTable.Rows)
                            {
                                if (!snapshot.JobHistories.ContainsKey((Guid)row["JobId"]))
                                {
                                    deleteRows.Add(row);
                                }
                                else
                                {
                                    bool found = false;
                                    foreach (AgentJobExecution jobRun in snapshot.JobHistories[(Guid)row["JobId"]].Executions)
                                    {
                                        if (jobRun.Outcome.InstanceId == (int)row["InstanceId"])
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        deleteRows.Add(row);
                                    }
                                }
                            }
                            foreach (DataRow row in deleteRows)
                            {
                                historyDataTable.Rows.Remove(row);
                            }

                            if (snapshot.JobHistories.Count > 0)
                            {
                                foreach (AgentJobHistory job in snapshot.JobHistories.Values)
                                {
                                    foreach (AgentJobExecution jobRun in job.Executions)
                                    {
                                        DataRow jobRow = historyDataTable.LoadDataRow(
                                            new object[]
                                            {
                                                jobRun.Outcome.InstanceId,
                                                job.JobId,
                                                job.Name,
                                                jobRun.Outcome.EndTime.HasValue ? (object)((DateTime)jobRun.Outcome.EndTime).ToLocalTime() : null,
                                                jobRun.Outcome.RunStatus,
                                                jobRun.Outcome.RunDuration,
                                                jobRun.Outcome.Retries,
                                                jobRun.Outcome.Message
                                            }, true);
                                        foreach (AgentJobStep step in jobRun.Steps)
                                        {
                                            stepDataTable.LoadDataRow(
                                                new object[]
                                            {
                                                jobRun.Outcome.InstanceId,
                                                step.InstanceId,
                                                step.StepId,
                                                step.Name,
                                                step.EndTime.HasValue ? (object)((DateTime)step.EndTime).ToLocalTime() : null,
                                                step.RunStatus,
                                                step.RunDuration,
                                                step.Retries,
                                                step.Message
                                            }, true);
                                        }
                                    }
                                }

                                historyDataTable.EndLoadData();
                                stepDataTable.EndLoadData();

                                if (!initializedHistory)
                                {
                                    if (lastHistoryGridSettings != null)
                                    {
                                        GridSettings.ApplySettingsToGrid(lastHistoryGridSettings, jobHistoryGrid);

                                        initializedHistory = true;
                                    }
                                    else if (historyDataTable.Rows.Count > 0)
                                    {
                                        foreach (UltraGridColumn column in jobHistoryGrid.DisplayLayout.Bands[0].Columns)
                                        {
                                            if (column.Key == "Job Name")
                                            {
                                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                                int colSize = column.Width;
                                                jobHistoryGrid.DisplayLayout.Bands[1].Columns["Step Name"].PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                                column.Width = Math.Min(Math.Max(column.Width, colSize), jobHistoryGrid.Width / 2);
                                            }
                                            else if (column.Key != "Outcome")
                                            {
                                                column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                                column.Width = Math.Min(column.Width, jobHistoryGrid.Width / 2);
                                            }
                                        }
                                        initializedHistory = true;
                                    }
                                }

                                jobHistoryGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                                jobHistoryGrid.DisplayLayout.Bands[1].SortedColumns.RefreshSort(true);
                                jobHistoryGrid.Visible = true;

                                ApplicationController.Default.SetCustomStatus(
                                    String.Format("{0}  Jobs: {1} Item{2}",
                                            agentJobFilter.IsFiltered() ? "Filter Applied" : string.Empty,
                                            currentDataTable.Rows.Count,
                                            currentDataTable.Rows.Count == 1 ? string.Empty : "s"),
                                    String.Format("History: {0} Item{1}",
                                            historyDataTable.Rows.Count,
                                            historyDataTable.Rows.Count == 1 ? string.Empty : "s"
                                            )
                                    );
                            }
                            else
                            {
                                historyDataTable.Clear();
                                historyDataTable.EndLoadData();
                                stepDataTable.Clear();
                                stepDataTable.EndLoadData();
                                jobHistoryGrid.Visible = false;
                                jobHistoryGridStatusLabel.Text = NO_ITEMS;
								///Ankit Nagpal --Sqldm10.0.0
								///If not a sysadmin display sysadmin message
                                //if (!isUserSysAdmin) jobHistoryGridStatusLabel.Text = SYSADMIN_MESSAGE;

                                ApplicationController.Default.SetCustomStatus(
                                    agentJobFilter.IsFiltered() ? "Filter Applied" : string.Empty,
                                    String.Format("Jobs: {0} Item{1}",
                                            currentDataTable.Rows.Count,
                                            currentDataTable.Rows.Count == 1 ? string.Empty : "s")
                                    );
                            }

                            // reset selectionChanged to enable full job refreshes again
                            selectionChanged = false;
                        }
                        else
                        {
                            e = snapshot.Error;
                            jobHistoryGrid.Visible = false;
                            jobHistoryGridStatusLabel.Text = UNABLE_TO_UPDATE;
							///Ankit Nagpal --Sqldm10.0.0
							///If not a sysadmin display sysadmin message
                            //if (!isUserSysAdmin) jobHistoryGridStatusLabel.Text = SYSADMIN_MESSAGE;
                        }
                    }
                }
            }
            else
            {
                AgentServiceState = ServiceState.UnableToMonitor;
                jobSummaryGrid.Visible = false;
                jobSummaryGridStatusLabel.Text = UNABLE_TO_UPDATE;
                jobHistoryGrid.Visible = false;
                jobHistoryGridStatusLabel.Text = UNABLE_TO_UPDATE;
				///Ankit Nagpal --Sqldm10.0.0
				///If not a sysadmin display sysadmin message
                //if (!isUserSysAdmin) jobSummaryGridStatusLabel.Text = jobHistoryGridStatusLabel.Text = SYSADMIN_MESSAGE;
                ApplicationController.Default.ClearCustomStatus();
            }

            AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);
            if (alertConfig != null)
            {
                jobSummaryGrid.SuspendLayout();
                foreach (UltraGridRow gridRow in jobSummaryGrid.Rows.GetAllNonGroupByRows())
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;

                    UpdateCellColor(Metric.JobCompletion, alertConfig, gridRow, "Last Run Outcome", 1);
                }
                jobSummaryGrid.ResumeLayout();
                jobHistoryGrid.SuspendLayout();
                foreach (UltraGridRow gridRow in jobHistoryGrid.Rows.GetAllNonGroupByRows())
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;

                    UpdateCellColor(Metric.JobCompletion, alertConfig, gridRow, "Outcome", 1);

                    if (gridRow.HasChild())
                    {
                        foreach (UltraGridRow childRow in gridRow.ChildBands[0].Rows.GetAllNonGroupByRows())
                        {
                            dataRowView = (DataRowView)childRow.ListObject;
                            dataRow = dataRowView.Row;

                            UpdateCellColor(Metric.JobCompletion, alertConfig, childRow, "Outcome", 1);
                        }
                    }
                }
                jobHistoryGrid.ResumeLayout();
            }

            if (e == null)
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
            else
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now, e));
            }

            if (UpdateDataCompleted != null)
            {
                UpdateDataCompleted(this, EventArgs.Empty);
            }
        }

        #endregion

        public void ShowFilter()
        {
            AgentJobFilter selectFilter = new AgentJobFilter();
            selectFilter.UpdateValues(agentJobFilter);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                agentJobFilter.UpdateValues(selectFilter);
                //Summary
                configuration.JobSummaryFilter = agentJobFilter.JobSummaryFilter;
                configuration.FilterTimeSpan = agentJobFilter.FilterTimeSpan;
                //History
                historyConfiguration.ShowFailedOnly = agentJobFilter.ShowFailedOnly;
                if (agentJobFilter.ShowFailedOnly)
                {
                    jobHistoryHeaderStripDropDownButton.Text = "Job History: Failed";
                }
                else
                {
                    jobHistoryHeaderStripDropDownButton.Text = "Job History: All";
                }

                jobHistoryGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                jobHistoryGrid.Visible = false;

                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            if (FilterChanged != null)
            {
                // This must be called regardless of the result because cancel will change button state
                FilterChanged(this, EventArgs.Empty);
            }
        }

        public void ToggleStatusFilter(AgentJobSummaryConfiguration.JobSummaryFilterType filterType)
        {
            agentJobFilter.JobSummaryFilter =
                configuration.JobSummaryFilter = filterType;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        public void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        public void StartService()
        {
            PerformServiceAction(ServiceControlConfiguration.ServiceControlAction.Start);
        }

        public void StopService()
        {
            if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                        String.Format("You are about to stop the {0} service on {1}. Do you want to continue?",
                                        ApplicationHelper.GetEnumDescription(ServiceName.Agent),
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName),
                                        null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                PerformServiceAction(ServiceControlConfiguration.ServiceControlAction.Stop);
            }
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

        private void PerformServiceAction(ServiceControlConfiguration.ServiceControlAction action)
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            AgentServiceState = ServiceState.UnableToMonitor;
            ServiceControlConfiguration config = new ServiceControlConfiguration(instanceId, ServiceName.Agent, action);

            AuditingEngine.SetContextData(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

            Snapshot snapshot = managementService.SendServiceControl(config);

            if (snapshot.Error == null)
            {
                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
            else
            {
                ApplicationMessageBox.ShowError(this, String.Format("Unable to {0} the {1} service.",
                                                                    ApplicationHelper.GetEnumDescription(action),
                                                                    ApplicationHelper.GetEnumDescription(ServiceName.Agent)),
                                                snapshot.Error);
            }
        }

        #region DataTables

        private void InitializeCurrentDataTable()
        {
            currentDataTable = new DataTable();
            currentDataTable.Columns.Add("JobId", typeof(object));
            currentDataTable.Columns.Add("Name", typeof(string));
            currentDataTable.Columns.Add("Category", typeof(string));
            currentDataTable.Columns.Add("Enabled", typeof(string));
            currentDataTable.Columns.Add("Scheduled", typeof(string));
            currentDataTable.Columns.Add("Status", typeof(int));
            currentDataTable.Columns.Add("Last Run Outcome", typeof(int));
            currentDataTable.Columns.Add("Last Run Time", typeof(DateTime));
            currentDataTable.Columns.Add("Last Run Duration", typeof(TimeSpan));
            currentDataTable.Columns.Add("Next Run Time", typeof(DateTime));
            currentDataTable.Columns.Add("Owner", typeof(string));
            currentDataTable.Columns.Add("Description", typeof(string));

            currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns["JobId"] };
            currentDataTable.CaseSensitive = true;

            currentDataTable.DefaultView.Sort = "[Name]";

            jobSummaryGrid.DataSource = currentDataTable;
        }

        private void InitializeHistoryDataSet()
        {

            historyDataSet = new DataSet("History");

            DataTable historyDataTable = historyDataSet.Tables.Add("Job Executions");
            historyDataTable.Columns.Add("InstanceId", typeof(int));
            historyDataTable.Columns.Add("JobId", typeof(object));
            historyDataTable.Columns.Add("Job Name", typeof(string));
            historyDataTable.Columns.Add("Date", typeof(DateTime));
            historyDataTable.Columns.Add("Outcome", typeof(int));
            historyDataTable.Columns.Add("Duration", typeof(TimeSpan));
            historyDataTable.Columns.Add("Retries", typeof(int));
            historyDataTable.Columns.Add("Message", typeof(string));

            historyDataTable.PrimaryKey = new DataColumn[] { historyDataTable.Columns["InstanceId"] };
            historyDataTable.CaseSensitive = true;

            historyDataTable.DefaultView.Sort = "[Job Name]";

            DataTable stepsDataTable = historyDataSet.Tables.Add("Job Steps");
            stepsDataTable.Columns.Add("ExecutionInstanceId", typeof(int));
            stepsDataTable.Columns.Add("InstanceId", typeof(int));
            stepsDataTable.Columns.Add("Step ID", typeof(int));
            stepsDataTable.Columns.Add("Step Name", typeof(string));
            stepsDataTable.Columns.Add("Date", typeof(DateTime));
            stepsDataTable.Columns.Add("Outcome", typeof(int));
            stepsDataTable.Columns.Add("Duration", typeof(TimeSpan));
            stepsDataTable.Columns.Add("Retries", typeof(int));
            stepsDataTable.Columns.Add("Message", typeof(string));

            stepsDataTable.PrimaryKey = new DataColumn[] { stepsDataTable.Columns["InstanceId"] };
            stepsDataTable.CaseSensitive = true;

            historyDataSet.Relations.Add("Execution Steps", historyDataSet.Tables["Job Executions"].Columns["InstanceId"],
                                              historyDataSet.Tables["Job Steps"].Columns["ExecutionInstanceId"], false);

            jobHistoryGrid.SetDataBinding(historyDataSet, "Job Executions");
        }

        #endregion

        #region grid

        private void ConfigureJobHistory(bool showFailedOnly)
        {
            agentJobFilter.ShowFailedOnly =
                historyConfiguration.ShowFailedOnly = showFailedOnly;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            selectionChanged = true;
            jobHistoryGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            jobHistoryGrid.Visible = false;

            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Band.SortedColumns.Clear();
                selectedColumn.Band.SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Band.SortedColumns.Clear();
                selectedColumn.Band.SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    selectedColumn.Band.SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    selectedColumn.Band.SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void ToggleGroupByBox(UltraGrid grid)
        {
            if (grid == jobSummaryGrid)
            {
                ToggleGroupByBox();
            }
            else
            {
                grid.DisplayLayout.GroupByBox.Hidden = !grid.DisplayLayout.GroupByBox.Hidden;
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups(UltraGrid grid)
        {
            grid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups(UltraGrid grid)
        {
            grid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser(UltraGrid grid)
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(grid);
            dialog.Show(this);
        }

        private void PrintGrid(UltraGrid grid)
        {
            ultraGridPrintDocument.Grid = grid;
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            string title;
            if (grid.Tag is ToolStripItem)
            {
                title = ((ToolStripItem)grid.Tag).Text;
            }
            else if (grid.Tag is string)
            {
                title = (string)grid.Tag;
            }
            else
            {
                title = "SQL Agent Jobs";
            }
            ultraGridPrintDocument.DocumentName = title;
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("{0} - {1} as of {2}",
                              ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                              title,
                              DateTime.Now.ToString("G")
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid(UltraGrid grid)
        {
            saveFileDialog.DefaultExt = "xls";
            string title;
            if (grid.Tag is ToolStripItem)
            {
                title = ((ToolStripItem)grid.Tag).Text;
            }
            else if (grid.Tag is string)
            {
                title = (string)grid.Tag;
            }
            else
            {
                title = "SQLAgentJobs";
            }
            title = ExportHelper.GetValidFileName(title, true);

            saveFileDialog.FileName = title;
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(grid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        private void UpdateCellColor(Metric metric, AlertConfiguration alertConfig, UltraGridRow gridRow, string columnName, int adjustmentMultiplier)
        {
            AlertConfigurationItem alertConfigItem = alertConfig[metric, String.Empty]; // Will need to update this if this metric ever supports multi-thresholds
            if (alertConfigItem != null)
            {
                UltraGridCell cell = gridRow.Cells[columnName];
                if (cell != null)
                {
                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    if (dataRow.IsNull(columnName))
                    {
                        cell.Appearance.ResetBackColor();
                    }
                    else
                    {

                        IComparable value;

                        // If the Job Completion Metric is enabled, then it will color the column according to the configuration
                        //   of the Job Completion thresholds.  Otherwise, it just colors the cells the way the were originally, 
                        //   which was Red for Failed and Canceled, Green for succeded and no color for any other status.
                        if (alertConfigItem.Enabled)
                        {
                            JobStepRunStatus status = (JobStepRunStatus)dataRow[columnName];

                            value = (IComparable)convertRunStatusToCompletionStatus(status);

                            if ((status == JobStepRunStatus.NotYetRun) || (status == JobStepRunStatus.InProgress))
                                value = MonitoredState.None;
                            else if (alertConfigItem.ThresholdEntry.CriticalThreshold.IsInViolation(value))
                                value = MonitoredState.Critical;
                            else if (alertConfigItem.ThresholdEntry.WarningThreshold.IsInViolation(value))
                                value = MonitoredState.Warning;
                            else if (alertConfigItem.ThresholdEntry.InfoThreshold.IsInViolation(value))
                                value = MonitoredState.Informational;
                            else
                                value = MonitoredState.OK;
                        }
                        else
                        {
                            value = (IComparable)dataRow[columnName];
                            // this forces the displayed indicator instead of calling the alert function
                            if (value.Equals((int)JobStepRunStatus.Failed)
                                || value.Equals((int)JobStepRunStatus.Cancelled))
                            {
                                value = MonitoredState.Critical;
                            }
                            else if (value.Equals((int)JobStepRunStatus.Succeeded))
                            {
                                value = MonitoredState.OK;
                            }
                            else
                            {
                                value = MonitoredState.None;
                            }
                        }

                        if (value != null && adjustmentMultiplier != 1)
                        {
                            double dbl = (double)Convert.ChangeType(value, typeof(double));
                            value = dbl * adjustmentMultiplier;
                        }

                        switch (Convert.ToInt16(value))
                        {
                            case (int)MonitoredState.Informational:
                                cell.Appearance.BackColor = Color.Blue;
                                cell.Appearance.ForeColor = Color.White;
                                break;
                            case (int)MonitoredState.Warning:
                                cell.Appearance.BackColor = Color.Gold;
                                cell.Appearance.ResetForeColor();
                                break;
                            case (int)MonitoredState.Critical:
                                cell.Appearance.BackColor = Color.Red;
                                cell.Appearance.ForeColor = Color.White;
                                break;
                            case (int)MonitoredState.OK:
                                cell.Appearance.BackColor = Color.Green;
                                cell.Appearance.ForeColor = Color.White;
                                break;
                            default:
                                cell.Appearance.ResetBackColor();
                                cell.Appearance.ResetForeColor();
                                break;
                        }
                    }
                }
            }
        }

        private JobStepCompletionStatus convertRunStatusToCompletionStatus(JobStepRunStatus value)
        {
            JobStepCompletionStatus result;

            switch (value)
            {
                case JobStepRunStatus.Failed:
                    result = JobStepCompletionStatus.Failed;
                    break;
                case JobStepRunStatus.Succeeded:
                    result = JobStepCompletionStatus.Succeeded;
                    break;
                case JobStepRunStatus.Cancelled:
                    result = JobStepCompletionStatus.Cancelled;
                    break;
                case JobStepRunStatus.Retry:
                    result = JobStepCompletionStatus.Retry;
                    break;
                default:
                    result = JobStepCompletionStatus.Unknown;
                    break;
            }
            return result;
        }

        private void ShowJobText()
        {
            if (jobHistoryGrid.Selected.Rows.Count != 1) 
                return;

            UltraGridRow selectedRow = jobHistoryGrid.Selected.Rows[0];
            
            string message = (string)selectedRow.Cells["Message"].Value;

            string auxText;
            if(selectedRow.ParentRow == null)
                auxText = "Job: " + (string)selectedRow.Cells["Job Name"].Value;
            else
                auxText = "Step: " + (string)selectedRow.Cells["Step Name"].Value;

            auxText = string.Format("SQL Agent {0} - Message Text", auxText);

            DisplayTextDialog n = new DisplayTextDialog(auxText, message, true);
            n.ShowDialog();
        }
        #endregion

        #endregion

        #region Event Handlers

        #region Splitter Focus

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

        private void hideJobHistoryButton_Click(object sender, EventArgs e)
        {
            HistoryPanelVisible = false;
        }

        #region toolbars

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    if (selectedGrid != null)
                    {
                        ShowColumnChooser(selectedGrid);
                    }
                    break;
                case "toggleGroupByBoxButton":
                    if (selectedGrid != null)
                    {
                        ToggleGroupByBox(selectedGrid);
                    }
                    break;
                case "printGridButton":
                    if (selectedGrid != null)
                    {
                        PrintGrid(selectedGrid);
                    }
                    break;
                case "exportGridButton":
                    if (selectedGrid != null)
                    {
                        SaveGrid(selectedGrid);
                    }
                    break;
                case "collapseAllGroupsButton":
                    if (selectedGrid != null)
                    {
                        CollapseAllGroups(selectedGrid);
                    }
                    break;
                case "expandAllGroupsButton":
                    if (selectedGrid != null)
                    {
                        ExpandAllGroups(selectedGrid);
                    }
                    break;
                case "startJobButton":
                    PerformJobAction(JobControlAction.Start);
                    break;
                case "stopJobButton":
                    PerformJobAction(JobControlAction.Stop);
                    break;
                case "viewMessageButton":
                    ShowJobText();                    
                    break;                    
            }
        }

        public void PerformJobAction(JobControlAction action)
        {
            if (jobSummaryGrid.Selected.Rows.Count == 1)
            {
                string selectedJobName = jobSummaryGrid.Selected.Rows[0].Cells["Name"].Value as string;

                if (selectedJobName != null)
                {
                    SqlAgentJobActionDialog jobActionDialog =
                        new SqlAgentJobActionDialog(instanceId, selectedJobName, action);

                    if (jobActionDialog.ShowDialog(ParentForm) != DialogResult.Cancel)
                    {
                        string successMessage = String.Empty;
                        string failedMessage = String.Empty;

                        switch (action)
                        {
                            case JobControlAction.Start:
                                successMessage = "The job was started successfully.";
                                failedMessage = "An error occurred while starting the job.";
                                break;
                            case JobControlAction.Stop:
                                successMessage = "The job was stopped successfully.";
                                failedMessage = "An error occurred while stopping the job.";
                                break;
                        }

                        if (jobActionDialog.Error == null)
                        {
                            if (jobActionDialog.Result != null)
                            {
                                if (jobActionDialog.Result.Error == null)
                                {
                                    ApplicationMessageBox.ShowInfo(ParentForm, successMessage);
                                    ApplicationController.Default.RefreshActiveView();
                                }
                                else
                                {
                                    ApplicationMessageBox.ShowError(ParentForm,
                                                            failedMessage,
                                                            jobActionDialog.Result.Error);
                                }
                            }
                            else
                            {
                                ApplicationMessageBox.ShowError(ParentForm, failedMessage,
                                                                new DesktopClientException(
                                                                    "An invalid result object was returned."));
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowError(ParentForm,
                                                            failedMessage,
                                                            jobActionDialog.Error);
                        }
                    }
                }
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = selectedGrid.Rows.Count > 0 && selectedGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }

            if (e.Tool.Key == "gridDataContextMenu")
            {
                ((PopupMenuTool)e.Tool).Tools["startJobButton"].SharedProps.Visible = 
                    ((PopupMenuTool)e.Tool).Tools["stopJobButton"].SharedProps.Visible = 
                        ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;

                ((PopupMenuTool) e.Tool).Tools["startJobButton"].SharedProps.Enabled = IsStartJobAllowed();
                ((PopupMenuTool) e.Tool).Tools["stopJobButton"].SharedProps.Enabled = IsStopJobAllowed();
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(selectedGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        public bool IsStartJobAllowed()
        {
            bool isStartJobAllowed = false;

            if (jobSummaryGrid.Selected.Rows.Count == 1)
            {
                //JobRunStatus jobRunStatus = (JobRunStatus)jobSummaryGrid.Selected.Rows[0].Cells["Status"].Value;

                var temp = jobSummaryGrid.Selected.Rows[0].Cells["Status"].Value.ToString();

                if(!string.IsNullOrEmpty(temp))
                {
                    JobRunStatus jobRunStatus = (JobRunStatus)jobSummaryGrid.Selected.Rows[0].Cells["Status"].Value;

                    if (jobRunStatus == JobRunStatus.NotRunning)
                    {
                        isStartJobAllowed = true;
                    }
                }
                else
                {
                    isStartJobAllowed = true;
                }
               

            }

            return isStartJobAllowed;
        }

        public bool IsStopJobAllowed()
        {
            bool isStopJobAllowed = false;

            if (jobSummaryGrid.Selected.Rows.Count == 1)
            {
                if (!string.IsNullOrEmpty(jobSummaryGrid.Selected.Rows[0].Cells["Status"].Value.ToString()))
                {
                    JobRunStatus jobRunStatus = (JobRunStatus)jobSummaryGrid.Selected.Rows[0].Cells["Status"].Value;

                    if (jobRunStatus != JobRunStatus.NotRunning)
                    {
                        isStopJobAllowed = true;
                    }
                }
                else
                {
                    isStopJobAllowed = true;
                }
                
            }

            return isStopJobAllowed;
        }

        #endregion

        #region grid

        private void jobSummaryGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            selectedJobs.Clear();
            if (jobSummaryGrid.Rows.Count > 0 && jobSummaryGrid.Selected.Rows.Count > 0)
            {
                foreach (UltraGridRow row in jobSummaryGrid.Selected.Rows)
                {
                    if (row.IsDataRow)
                    {
                        selectedJobs.Add((Guid)row.Cells["JobId"].Value,
                            (Guid)row.Cells["JobId"].Value);
                    }
                }
            }
            selectionChanged = true;

            if (JobSelectionChanged != null)
            {
                JobSelectionChanged(this, EventArgs.Empty);
            }

            jobHistoryGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            jobHistoryGrid.Visible = false;

            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        private void jobSummaryGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = jobSummaryGrid;
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                        selectedColumn.IsGroupByColumn;
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
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void jobHistoryGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                selectedGrid = jobHistoryGrid;
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                        selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));
                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "jobHistoryContextMenu");
                    }
                    else
                    {
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        #endregion

        private void jobHistoryAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jobHistoryHeaderStripDropDownButton.Text = "Job History: All";
            ConfigureJobHistory(false);
        }

        private void jobHistoryFailedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jobHistoryHeaderStripDropDownButton.Text = "Job History: Failed";
            ConfigureJobHistory(true);
        }

        #endregion

        private void ServicesSqlAgentJobsView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {            
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
