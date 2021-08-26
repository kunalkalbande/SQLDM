using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
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
using Infragistics.Win.UltraWinDataSource;
using Idera.SQLdm.Common.Events;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Logs
{
    internal partial class LogsView : ServerBaseView, IShowFilterDialog
    {
        #region constants

        public enum RefreshContext
        {
            User = 0,
            Timer,
            Program
        }

        public enum LogTreeRoot
        {
            SQLServer,
            Agent
        }

        private enum ViewStatus
        {
            Loading,
            NoItems,
            UnableToUpdate,
            Loaded
        }

        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
        ///Ankit Nagpal --Sqldm10.0.0
        private const string SYSADMIN_MESSAGE = @"No data is available for this view.";

        private const string LOGSIZEEXCEEDED =
            "Viewing the current logs is not recommended because the Error and Agent Logs have exceeded the configured maximum values of {0}MB (Error) and {1}MB (Agent). You can fix this by clicking the 'Cycle Log' button above.";
        #endregion

        #region fields
        private string infoLabel = "      Log event times shown in this view are local to the monitored SQL Server instance.";
        private string oneSizeLimitExceeded = "      {0} size limit has been exceeded and the logs are not being displayed by default. Click to configure.";
        private string bothSizeLimitExceeded = "      Error and Agent size limits have been exceeded and the logs are not being displayed by default. Click to configure.";

        private Stopwatch refreshTreeStopwatch = new Stopwatch();
        private Stopwatch actionStopwatch = new Stopwatch();

        private bool initialized = false;
        private bool treeInitialized = false;
        private DateTime? lastEndDate = null;
        private OnDemandConfiguration configurationLogFiles;
        private ErrorLogConfiguration configuration;
        private LogFileList currentSnapshot = null;
        private Control focused = null;
        private UltraGridColumn selectedColumn = null;
        private static readonly object updateLock = new object();
        private SearchTextDialog searchDialog;
        private DateTime? historicalSnapshotDateTime;

        private object argument = null;
        private bool selectAllSqlServer = false;
        private bool selectAllAgent = false;
        private bool treeUpdating = false;
        private bool selectionUpdating = false;
        private bool selectionChanged = false;
        private long errorLogSizeLimitKB = 0;
        private long agentLogSizeLimitKB = 0;
        private bool errorLogLimitExceeded = false;
        private bool agentLogLimitExceeded = false;
        ValueListItem listItem1, listItem2, listItem3, listItem4, listItem5;
        private List<LogFile> selectedLogList
        {
            get
            {
                List<LogFile> logList = new List<LogFile>();

                foreach (TreeNode rootnode in availableLogsTreeView.Nodes)
                {
                    foreach (TreeNode node in rootnode.Nodes)
                    {
                        if (node.Checked && node.Tag is LogFile)
                        {
                            logList.Add((LogFile)node.Tag);
                        }
                    }
                }

                return logList;
            }
        }

        // get the version of the current instance from the snapshot or the app instance cache
        private decimal serverVersion
        {
            get
            {
                if (currentSnapshot != null)
                {
                    return currentSnapshot.ProductVersion.Major;
                }
                else
                {
                    MonitoredSqlServerStatus server = ApplicationModel.Default.GetInstanceStatus(instanceId);
                    if (server != null && server.InstanceVersion != null)
                    {
                        return server.InstanceVersion.Major;
                    }
                    else
                    {
                        // default to assuming it is 2005
                        return 9;
                    }
                }
            }
        }
        private bool showOnly2000Options
        {
            get
            {
                return serverVersion < 9;
            }
        }

        //last Settings values used to determine if changed for saving when leaving
        private int lastLogSplitterDistance = 0;
        private int lastDataSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastLogsVisible = true;
        private bool lastDetailsVisible = true;

        #endregion

        #region constructors

        ThemeSetter themeSetter = new ThemeSetter();
        public LogsView(int instanceId)
            : base(instanceId)
        {
            InitializeComponent();
            AdaptFontSize();
            logGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;

            //clear dummy data from tree if first time error
            availableLogsTreeView.Nodes[(int)LogTreeRoot.SQLServer].Nodes.Clear();
            availableLogsTreeView.Nodes[(int)LogTreeRoot.Agent].Nodes.Clear();

            SetViewStatus(ViewStatus.Loading);

            // load value lists for grid display
            

            logGrid.DisplayLayout.ValueLists["severityValueList"].ValueListItems.Clear();
            listItem1 = new ValueListItem(MonitoredState.Critical, "Critical");
            if(Settings.Default.ColorScheme == "Dark")
                listItem1.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.critical_wht_icon1;
            else
                listItem1.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            logGrid.DisplayLayout.ValueLists["severityValueList"].ValueListItems.Add(listItem1);
            listItem2 = new ValueListItem(MonitoredState.Warning, "Warning");
            if (Settings.Default.ColorScheme == "Dark")
                listItem2.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.warning_wht_icon1;
            else
                listItem2.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            logGrid.DisplayLayout.ValueLists["severityValueList"].ValueListItems.Add(listItem2);
            listItem3 = new ValueListItem(MonitoredState.Informational, "Informational");
            if (Settings.Default.ColorScheme == "Dark")
                listItem3.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.info_wht_icon1;
            else
                listItem3.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.InformationSmall;
            logGrid.DisplayLayout.ValueLists["severityValueList"].ValueListItems.Add(listItem3);
            listItem4 = new ValueListItem(MonitoredState.OK, "OK");
            if (Settings.Default.ColorScheme == "Dark")
                listItem4.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ok_wht_icon1;
            else
                listItem4.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
            logGrid.DisplayLayout.ValueLists["severityValueList"].ValueListItems.Add(listItem4);
            listItem5 = new ValueListItem(MonitoredState.None, "Unknown");
            if (Settings.Default.ColorScheme == "Dark")
                listItem5.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.info_wht_icon1;
            else
                listItem5.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Information16x16;
            logGrid.DisplayLayout.ValueLists["severityValueList"].ValueListItems.Add(listItem5);


            logGrid.DisplayLayout.ValueLists["logTypeValueList"].ValueListItems.Clear();
            ValueListItem listItem6 = new ValueListItem(LogFileType.SQLServer, "SQL Server");
            logGrid.DisplayLayout.ValueLists["logTypeValueList"].ValueListItems.Add(listItem6);
            ValueListItem listItem7 = new ValueListItem(LogFileType.Agent, "SQL Server Agent");
            logGrid.DisplayLayout.ValueLists["logTypeValueList"].ValueListItems.Add(listItem7);

            configurationLogFiles = new OnDemandConfiguration(instanceId);
            configuration = new ErrorLogConfiguration(instanceId);

            SetAboveContentAlertTheme();

            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            if (AutoScaleSizeHelper.isScalingRequired)
            {
                if (AutoScaleSizeHelper.isLargeSize)
                {
                    this.boundLogTypeLabel.Location = new System.Drawing.Point(52, 21);
                    this.boundLogTypeLabel.Size = new System.Drawing.Size(127, 20);
                    this.boundLogFileLabel.Size = new System.Drawing.Size(127, 20);
                    this.boundLogFileLabel.Location = new System.Drawing.Point(50, 63);
                    this.boundLogNameLabel.Size = new System.Drawing.Size(127, 20);
                    this.boundLogNameLabel.Location = new System.Drawing.Point(50, 43);
                    this.informationLabel.Size = new System.Drawing.Size(594, 25);
                    this.infoPanel.Size = new System.Drawing.Size(600, 30);
                }
                if (AutoScaleSizeHelper.isXLargeSize)
                {
                    this.boundLogTypeLabel.Location = new System.Drawing.Point(58, 21);
                    this.boundLogTypeLabel.Size = new System.Drawing.Size(127, 23);
                    this.boundLogFileLabel.Size = new System.Drawing.Size(127, 23);
                    this.boundLogFileLabel.Location = new System.Drawing.Point(50, 71);
                    this.boundLogNameLabel.Size = new System.Drawing.Size(127, 23);
                    this.boundLogNameLabel.Location = new System.Drawing.Point(50, 46);
                    this.informationLabel.Size = new System.Drawing.Size(594, 30);
                    this.infoPanel.Size = new System.Drawing.Size(600, 35);
                }
                if (AutoScaleSizeHelper.isXXLargeSize)
                {
                    this.boundLogTypeLabel.Location = new System.Drawing.Point(65, 21);
                    this.boundLogTypeLabel.Size = new System.Drawing.Size(127, 26);
                    this.boundLogFileLabel.Size = new System.Drawing.Size(127, 26);
                    this.boundLogFileLabel.Location = new System.Drawing.Point(50, 81);
                    this.boundLogNameLabel.Size = new System.Drawing.Size(127, 26);
                    this.boundLogNameLabel.Location = new System.Drawing.Point(50, 51);
                    this.informationLabel.Size = new System.Drawing.Size(594, 35);
                    this.infoPanel.Size = new System.Drawing.Size(600, 40);
                }
            }
            
        }

        #endregion

        #region properties

        public event EventHandler FilterChanged;
        public event EventHandler AvailableLogsPanelVisibleChanged;
        public event EventHandler DetailsPanelVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        public bool AvailableLogsPanelVisible
        {
            get { return !splitContainer1.Panel1Collapsed; }
            set
            {
                splitContainer1.Panel1Collapsed = !value;

                if (AvailableLogsPanelVisibleChanged != null)
                {
                    AvailableLogsPanelVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool DetailsPanelVisible
        {
            get { return !splitContainer2.Panel2Collapsed; }
            set
            {
                splitContainer2.Panel2Collapsed = !value;

                if (DetailsPanelVisibleChanged != null)
                {
                    DetailsPanelVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool GridGroupByBoxVisible
        {
            get { return !logGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                logGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the current configuration settings to manage state for current selections
        /// </summary>
        public ErrorLogConfiguration Configuration
        {
            get { return configuration; }
        }

        #endregion

        #region methods


        /// <summary>
        /// Show the alert configuration dialog
        /// </summary>
        /// <param name="serverid"></param>
        /// <param name="metric"></param>
        private void EditAlertConfiguration(int serverid, Metric metric)
        {
            MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[serverid];
            if (wrapper == null) return;

            var server = wrapper.InstanceName;

            if (!String.IsNullOrEmpty(server))
            {
                try
                {
                    using (AlertConfigurationDialog alertConfigDialog =
                            new AlertConfigurationDialog(wrapper.Id, false))
                    {
                        metric = (Metric)Enum.ToObject(typeof(Metric), ((int)metric));

                        alertConfigDialog.CreateControl();
                        alertConfigDialog.Select((Metric)metric);

                        alertConfigDialog.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                                                    ex);
                }

            }
        }
        /// <summary>
        /// Get the File size limits set in the Advanced Alert configuration
        /// </summary>
        private void GetFileSizeLimits()
        {
            string server = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;

            var alertConfig = RepositoryHelper.GetAlertConfiguration(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                    instanceId, new int[] { (int)Metric.ErrorLog, (int)Metric.AgentLog });

            var advancedErrorLogSettings = (AdvancedAlertConfigurationSettings)alertConfig[Metric.ErrorLog, server].ThresholdEntry.Data;
            var advancedAgentLogSettings = (AdvancedAlertConfigurationSettings)alertConfig[Metric.AgentLog, server].ThresholdEntry.Data;

            if (advancedErrorLogSettings == null) advancedErrorLogSettings = new AdvancedAlertConfigurationSettings();
            if (advancedAgentLogSettings == null) advancedAgentLogSettings = new AdvancedAlertConfigurationSettings();

            errorLogSizeLimitKB = advancedErrorLogSettings.LogSizeLimit;
            agentLogSizeLimitKB = advancedAgentLogSettings.LogSizeLimit;
        }

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.LogsView);
        }

        public override void SetArgument(object argument)
        {

            this.argument = argument;

            if (this.currentSnapshot != null)
            {
                selectionUpdating = true;
                if (argument is LogTreeRoot)
                {
                    // uncheck all the selected logs
                    foreach (TreeNode node in availableLogsTreeView.Nodes[(int)LogTreeRoot.SQLServer].Nodes)
                    {
                        if (node.Checked == true)
                        {
                            node.Checked = false;
                        }
                    }
                    foreach (TreeNode node in availableLogsTreeView.Nodes[(int)LogTreeRoot.Agent].Nodes)
                    {
                        if (node.Checked == true)
                        {
                            node.Checked = false;
                        }
                    }

                    if (((LogTreeRoot)argument) == LogTreeRoot.SQLServer)
                    {
                        var treeNode = availableLogsTreeView.Nodes[(int)LogTreeRoot.SQLServer];

                        errorLogLimitExceeded = TestTreeNodeSizeLimitExceeded(treeNode, LogTreeRoot.SQLServer);

                        if ((treeNode.Nodes.Count > 0) && !errorLogLimitExceeded)
                            treeNode.Nodes[0].Checked = true;

                    }
                    else
                    {
                        var treeNode = availableLogsTreeView.Nodes[(int)LogTreeRoot.Agent];

                        agentLogLimitExceeded = TestTreeNodeSizeLimitExceeded(treeNode, LogTreeRoot.Agent);

                        if (treeNode.Nodes.Count > 0 && !agentLogLimitExceeded)
                            treeNode.Nodes[0].Checked = true;

                    }
                }
                else
                {
                    var treeNode = availableLogsTreeView.Nodes[(int)LogTreeRoot.SQLServer];
                    errorLogLimitExceeded = TestTreeNodeSizeLimitExceeded(treeNode, LogTreeRoot.SQLServer);
                    if (treeNode.Nodes.Count > 0) treeNode.Nodes[0].Checked = !errorLogLimitExceeded;

                    treeNode = availableLogsTreeView.Nodes[(int)LogTreeRoot.Agent];
                    agentLogLimitExceeded = TestTreeNodeSizeLimitExceeded(treeNode, LogTreeRoot.Agent);
                    if (treeNode.Nodes.Count > 0) treeNode.Nodes[0].Checked = !agentLogLimitExceeded;
                }

                selectionUpdating = false;
            }

            RefreshView();
        }

        public override void ApplySettings()
        {
            try
            {
                if (Settings.Default.LogsViewLogsSplitter > splitContainer1.Panel1MinSize 
                    && Settings.Default.LogsViewLogsSplitter < splitContainer1.Width-splitContainer1.Panel2MinSize)
                {
                    lastLogSplitterDistance =
                        splitContainer1.SplitterDistance = Settings.Default.LogsViewLogsSplitter;
                }
            }
            catch(Exception e)
            {
                Log.Debug(e);
            }
            // Fixed panel is second panel, so restore size of second panel
            lastDataSplitterDistance = splitContainer2.Height - Settings.Default.LogsViewDataSplitter;
            if (lastDataSplitterDistance > 0)
            {
                splitContainer2.SplitterDistance = lastDataSplitterDistance;
            }
            else
            {
                lastDataSplitterDistance = splitContainer2.Height - splitContainer2.SplitterDistance;
            }

            lastLogsVisible =
                AvailableLogsPanelVisible = Settings.Default.LogsViewLogsPanelVisible;

            lastDetailsVisible =
                DetailsPanelVisible = Settings.Default.LogsViewDetailsPanelVisible;

            if (Settings.Default.LogsViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.LogsViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, logGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(logGrid);
            // save all settings only if anything has changed
            if (lastLogSplitterDistance != splitContainer1.SplitterDistance
                || lastDataSplitterDistance != splitContainer2.Height - splitContainer2.SplitterDistance
                || !mainGridSettings.Equals(lastMainGridSettings)
                || lastLogsVisible != AvailableLogsPanelVisible
                || lastDetailsVisible != DetailsPanelVisible)
            {
                lastLogSplitterDistance =
                    Settings.Default.LogsViewLogsSplitter = splitContainer1.SplitterDistance;
                // Fixed panel is second panel, so save size of second panel
                lastDataSplitterDistance =
                    Settings.Default.LogsViewDataSplitter = splitContainer2.Height - splitContainer2.SplitterDistance;
                lastMainGridSettings =
                    Settings.Default.LogsViewMainGrid = mainGridSettings;
                lastLogsVisible =
                    Settings.Default.LogsViewLogsPanelVisible = AvailableLogsPanelVisible;
                lastDetailsVisible =
                    Settings.Default.LogsViewDetailsPanelVisible = DetailsPanelVisible;
            }
        }
        
        /// <summary>
        /// Is this an explicit refresh caused directly by the user
        /// If it is then we don't want to show the warning
        /// </summary>
        /// <returns>RefreshContext enum</returns>
        public RefreshContext GetRefreshContext()
        {
            var stackTrace = new StackTrace();

            for(int i = 0;i < stackTrace.FrameCount;i++)
            {
                System.Reflection.MethodBase methodBase = stackTrace.GetFrame(i).GetMethod();

                if (methodBase.Name.Contains("availableLogsTreeView") /*|| methodBase.Name.Contains("TreeView_AfterSelect")*/)
                {
                    return RefreshContext.User;
                }
                if (methodBase.Name.ToLower().Contains("tick"))
                {
                    return RefreshContext.Timer;
                }
            }
            return RefreshContext.Program;
        }

        /// <summary>
        /// Sets up the base refresh which calls the dowork on a new thread
        /// </summary>
        public override void RefreshView()
        {
            GetFileSizeLimits();

            var explicitRefresh = GetRefreshContext();
            
            var treeNode = availableLogsTreeView.Nodes[(int)LogTreeRoot.SQLServer];
            errorLogLimitExceeded = TestTreeNodeSizeLimitExceeded(treeNode, LogTreeRoot.SQLServer);
            treeNode = availableLogsTreeView.Nodes[(int)LogTreeRoot.Agent];
            agentLogLimitExceeded = TestTreeNodeSizeLimitExceeded(treeNode, LogTreeRoot.Agent);


            if (HistoricalSnapshotDateTime == null)
            {
                //We only want to show the large message if neither the agent nor error log are available
                if (errorLogLimitExceeded && agentLogLimitExceeded)
                {
                    LogsView_Fill_Panel.Visible = true;

                    //if this is programatic defaulting
                    if (explicitRefresh == RefreshContext.Program)
                    {
                        lblLogLimitExceeded.BringToFront();
                        lblLogLimitExceeded.Visible = true;
                    }
                    else
                    {
                        SetupInformationLabels(explicitRefresh);
                    }
                }
                else//if both limits have not been exceeded then we can't stick a big message over the whole grid
                    //cos the user will need the grid if they don't heed our warning and click on a node
                {
                    LogsView_Fill_Panel.Visible = true;

                    informationLabel.Text = infoLabel;

                    SetupInformationLabels(explicitRefresh);
                }
                base.RefreshView();
            }
            else
            {
                LogsView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        /// <summary>
        /// Set up the informationlabels that explain why the logs might not get displayed by default
        /// </summary>
        /// <param name="explicitRefresh"></param>
        public void SetupInformationLabels(RefreshContext explicitRefresh)
        {
            if (errorLogLimitExceeded && !agentLogLimitExceeded)
            {
                informationLabel.Text = string.Format(oneSizeLimitExceeded, "Error Log") + ' ' + infoLabel.TrimStart();
            }

            if (agentLogLimitExceeded && !errorLogLimitExceeded)
            {
                informationLabel.Text = string.Format(oneSizeLimitExceeded, "Agent Log") + ' ' + infoLabel.TrimStart();
            }

            if (agentLogLimitExceeded && errorLogLimitExceeded)
            {
                informationLabel.Text = bothSizeLimitExceeded;
            }

            //if this is a user initiated population
            if (explicitRefresh == RefreshContext.User)
            {
                //we dont want to see the warning message
                lblLogLimitExceeded.SendToBack();
            }
        }

        public override void CancelRefresh()
        {
            if (configuration != null && IsRefreshBackgroundWorkerBusy) {
                
                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                try
                {
                    managementService.CancelOnDemandRequest(configuration.ClientSessionId);
                }
                catch (Exception)
                {
                }
            }
            base.CancelRefresh();
        }

        /// <summary>
        /// Called on a background thread to get all of the data required by the view
        /// </summary>
        /// <returns></returns>
        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            LogFileList newfileList = managementService.GetLogList(configurationLogFiles);

            if (LogFilesChanged(newfileList))
            {
                lastEndDate = null;
                return newfileList;
            }
            else
            {
                if (selectionChanged || !lastEndDate.HasValue || selectedLogList.Count == 0)
                {
                    configuration.InternalStartDate = null;
                }
                else
                {
                    configuration.InternalStartDate = lastEndDate.Value;
                }
                configuration.LogFiles = selectedLogList;
                var errorLogObj = (ErrorLog)managementService.GetErrorLog(configuration);
                if(errorLogObj != null && newfileList.ProbeError != null && errorLogObj.ProbeError == null)
                {
                    errorLogObj.ProbeError = newfileList.ProbeError;
                }
                return errorLogObj;
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            SetViewStatus(ViewStatus.UnableToUpdate);
            base.HandleBackgroundWorkerError(e);
        }

        /// <summary>
        /// This c
        /// </summary>
        /// <param name="data">Could be a newfilelist object or an ErrorLog snapshot</param>
        public override void UpdateData(object data)
        {
            Exception e = null;
            int selectedIndex = logGrid.Selected.Rows.Count == 0 ? -1 : logGrid.Selected.Rows[0].Index;

            if (data != null)
            {
                if (data is ErrorLog)
                {
                    ErrorLog snapshot = (ErrorLog)data;
                    lock (updateLock)
                    {
                        if (snapshot.Error == null)
                        {
                            if (snapshot.HasBeenInternallyFiltered)
                            {
                                if (snapshot.Messages.Rows.Count > 0)
                                {
                                    foreach (DataRow row in snapshot.Messages.Rows)
                                    {
                                        ((DataView)logGrid.DataSource).Table.ImportRow(row);
                                        logGrid.Rows[logGrid.Rows.Count - 1].RefreshSortPosition();
                                    }
                                }
                                else
                                {
                                    logGrid.Selected.Rows.Clear();
                                }

                                SetViewStatus(ViewStatus.Loaded);

                                // it will be empty if there were no rows returned
                                // so don't reset it or it will reload all data on next refresh.
                                if (snapshot.LatestDate.HasValue)
                                {
                                    lastEndDate = snapshot.LatestDate;
                                }
                            }
                            else
                            {
                                if (snapshot.Messages.Rows.Count > 0)
                                {
                                    logGrid.SuspendLayout();

                                    snapshot.Messages.DefaultView.Sort = "Local Time";
                                    snapshot.Messages.DefaultView.AllowDelete = true;
                                    snapshot.Messages.DefaultView.AllowNew = true;
                                    snapshot.Messages.DefaultView.AllowEdit = true;

                                    snapshot.Messages.DefaultView.RowFilter = "[Local Time] is not null";
                                    
                                    logGrid.SetDataBinding(snapshot.Messages.DefaultView, string.Empty);

                                    logGrid.ResumeLayout();

                                    SetViewStatus(ViewStatus.Loaded);

                                    lastEndDate = snapshot.LatestDate;
                                }
                                else
                                {
                                    logGrid.Selected.Rows.Clear();
                                    SetViewStatus(ViewStatus.NoItems);
                                }

                                selectionChanged = false;
                            }

                            if (!initialized)
                            {
                                if (lastMainGridSettings != null)
                                {
                                    GridSettings.ApplySettingsToGrid(lastMainGridSettings, logGrid);

                                    initialized = true;
                                }
                            }
                        }
                        else
                        {
                            e = snapshot.Error;
                            SetViewStatus(ViewStatus.UnableToUpdate);
                        }
                    }
                    if(snapshot.ProbeError != null)
                    {
                        e = new Exception(snapshot.ProbeError.ToString());
                        SetViewStatus(ViewStatus.UnableToUpdate);
                        logGridStatusLabel.Text = UNABLE_TO_UPDATE;
                    }
                }
                else if (data is LogFileList)
                {
                    //if the log tree has changed
                    UpdateLogTree(data);
                    RefreshLogs();
                }
                else
                {
                    SetViewStatus(ViewStatus.UnableToUpdate);
                }
            }
            else
            {
                SetViewStatus(ViewStatus.UnableToUpdate);
            }

            //check if there is an active row and if there is then defaut it on refresh
            if (logGrid.ActiveRow != null)
            {
                if (selectedIndex != -1 && selectedIndex == logGrid.ActiveRow.Index)
                {
                    logGrid.Rows[selectedIndex].Selected = true;
                }
            }

            UpdateDetailsPanel();

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
        }

        public void ShowFilter()
        {
            ErrorLogConfiguration selectFilter = new ErrorLogConfiguration(instanceId);
            selectFilter.UpdateValues(configuration);
            GenericFilterDialog dialog = new GenericFilterDialog(selectFilter);

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                selectionChanged = true;
                configuration.UpdateValues(selectFilter);
                RefreshLogs();
            }
            if (FilterChanged != null)
            {
                // This must be called regardless of the result because cancel will change button state
                FilterChanged(this, EventArgs.Empty);
            }
        }

        public void ToggleErrorFilter()
        {
            configuration.ShowErrors = !configuration.ShowErrors;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            selectionChanged = true;
            RefreshLogs();
        }

        public void ToggleWarningFilter()
        {
            configuration.ShowWarnings = !configuration.ShowWarnings;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            selectionChanged = true;
            RefreshLogs();
        }

        public void ToggleInformationalFilter()
        {
            configuration.ShowInformational = !configuration.ShowInformational;
            if (FilterChanged != null)
            {
                FilterChanged(this, EventArgs.Empty);
            }
            selectionChanged = true;
            RefreshLogs();
        }

        public void ShowSearch()
        {
            if (searchDialog == null)
            {
                searchDialog = new SearchTextDialog();
                searchDialog.SearchList.Rows.Add(new object[] { "Message", "Message" });
                searchDialog.SearchList.Rows.Add(new object[] { "Message Number", "Msg #" });
                searchDialog.SearchList.Rows.Add(new object[] { "Source", "Source" });
                searchDialog.FindNext += new EventHandler(SearchGrid);
            }

            if (logGrid.Visible && !searchDialog.Visible)
            {
                searchDialog.Show(this);
            }
        }

        public void CycleServerLog()
        {
            Exception e = null;
            Snapshot snapshot;
            CycleLogsDialog dialog = new CycleLogsDialog(!showOnly2000Options);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                using (Log.DebugCall("CycleServerLog"))
                {
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                    if (dialog.CycleSqlServer)
                    {
                        Log.Info("Cycling SQL Server Log...");
                        actionStopwatch.Reset();
                        actionStopwatch.Start();

                        RecycleLogConfiguration config = new RecycleLogConfiguration(instanceId);
                        snapshot = managementService.SendRecycleLog(config);

                        actionStopwatch.Stop();
                        Log.Info(string.Format("Cycling SQL Server Log completed (Duration = {0}).", actionStopwatch.Elapsed));

                        if (snapshot.Error != null)
                        {
                            e = snapshot.Error;
                            Log.Error("An error occurred while Cycling the SQL Server Log.", e);
                            ApplicationMessageBox.ShowError(this, "Unable to Cycle SQL Server Log.", e);
                        }
                    }

                    if (dialog.CycleSqlAgent)
                    {
                        Log.Info("Cycling SQL Server Agent Log...");
                        actionStopwatch.Reset();
                        actionStopwatch.Start();

                        RecycleAgentLogConfiguration config = new RecycleAgentLogConfiguration(instanceId);
                        snapshot = managementService.SendRecycleAgentLog(config);

                        actionStopwatch.Stop();
                        Log.Info(string.Format("Cycling SQL Server Agent Log completed (Duration = {0}).", actionStopwatch.Elapsed));

                        if (snapshot.Error != null)
                        {
                            e = snapshot.Error;
                            Log.Error("An error occurred while Cycling the SQL Server Agent Log.", e);
                            ApplicationMessageBox.ShowError(this, "Unable to Cycle SQL Server Agent Log.", e);
                        }
                    }

                    if (e == null)
                    {
                        RefreshLogs();
                    }
                }
            }
        }

        public void ConfigureLogs()
        {
            ConfigureLogsDialog dialog;
            if (currentSnapshot != null)
            {
                dialog = new ConfigureLogsDialog(instanceId,
                                                    currentSnapshot.MaximumSqlLogs,
                                                    currentSnapshot.UnlimitedSqlLogs);
            }
            else
            {
                ApplicationMessageBox.ShowWarning(this, "The current number of log files is not known. Do you wish to continue?");
                dialog = new ConfigureLogsDialog(instanceId);
            }
            if (DialogResult.OK == dialog.ShowDialog(this))
            {
                RefreshLogTree();
            }
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetAboveContentAlertTheme();
            SetGridTheme();
            SetImages();
        }

        private void SetImages()
        {
            // load value lists for grid display
            if (Settings.Default.ColorScheme == "Dark")
                listItem1.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.critical_wht_icon1;
            else
                listItem1.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusCriticalSmall;
            if (Settings.Default.ColorScheme == "Dark")
                listItem2.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.warning_wht_icon1;
            else
                listItem2.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusWarningSmall;
            if (Settings.Default.ColorScheme == "Dark")
                listItem3.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.info_wht_icon1;
            else
                listItem3.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.InformationSmall;
            if (Settings.Default.ColorScheme == "Dark")
                listItem4.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ok_wht_icon1;
            else
                listItem4.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StatusOKSmall;
            if (Settings.Default.ColorScheme == "Dark")
                listItem5.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.info_wht_icon1;
            else
                listItem5.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Information16x16;
        }
        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.logGrid);
        }

        private void SetAboveContentAlertTheme()
        {
            //if (Settings.Default.ColorScheme == "Dark")
            //{
                themeSetter.SetAboveContentAlertPanelTheme(this.infoPanel, instanceId);
                themeSetter.SetAboveContentAlertLabelTheme(this.informationLabel, instanceId);
                themeSetter.SetAboveContentAlertPictureBoxTheme(this.pictureBox1, instanceId);
            /*}
            else
            {
                themeSetter.SetPanelTheme(this.infoPanel, System.Drawing.Color.FromArgb(212, 212, 212));
                themeSetter.SetLabelTheme(this.informationLabel, System.Drawing.Color.FromArgb(212, 212, 212), System.Drawing.Color.Black);
                themeSetter.SetPictureBoxTheme(this.pictureBox1, System.Drawing.Color.FromArgb(((int) (((byte) (212)))), ((int) (((byte) (212)))), ((int) (((byte) (212))))), global::Idera.SQLdm.DesktopClient.Properties.Resources.Information16x16);
            }*/
        }

        #endregion

        #region helpers

        private void SetViewStatus(ViewStatus status)
        {
            switch (status)
            {
                case ViewStatus.Loading:
                    if (searchDialog != null)
                    {
                        searchDialog.Hide();
                    }
                    logProgressControl.Active = true;
                    logProgressControl.Show();
                    logGridStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                    logGridStatusLabel.Dock = DockStyle.None;
                    logGrid.Visible = false;
                    ApplicationController.Default.ClearCustomStatus();
                    detailsLayoutPanel.Visible = false;
                    break;
                case ViewStatus.NoItems:
                    if (searchDialog != null)
                    {
                        searchDialog.Hide();
                    }
                    logProgressControl.Active = false;
                    logProgressControl.Hide();
                    logGridStatusLabel.Text = NO_ITEMS;
                    ///Ankit Nagpal --Sqldm10.0.0
                    ///If not a sysadmin display sysadmin message
                    if (!isUserSysAdmin) logGridStatusLabel.Text = SYSADMIN_MESSAGE;
                    logGridStatusLabel.Dock = DockStyle.Fill;
                    logGrid.Visible = false;
                    ApplicationController.Default.SetCustomStatus(
                        configuration.IsFiltered() ? "Filter Applied" : string.Empty,
                        "0 Log Records"
                        );
                    detailsLayoutPanel.Visible = false;
                    break;
                case ViewStatus.UnableToUpdate:
                    if (searchDialog != null)
                    {
                        searchDialog.Hide();
                    }
                    logProgressControl.Active = false;
                    logProgressControl.Hide();
                    logGridStatusLabel.Text = UNABLE_TO_UPDATE;
                    ///Ankit Nagpal --Sqldm10.0.0
                    ///If not a sysadmin display sysadmin message
                    if (!isUserSysAdmin) logGridStatusLabel.Text = SYSADMIN_MESSAGE;
                    logGridStatusLabel.Dock = DockStyle.Fill;
                    logGrid.Visible = false;
                    ApplicationController.Default.ClearCustomStatus();
                    detailsLayoutPanel.Visible = false;
                    break;
                case ViewStatus.Loaded:
                    logGrid.Visible = true;
                    logProgressControl.Active = false;
                    logProgressControl.Hide();
                    int rowCount = logGrid.Rows.GetAllNonGroupByRows().Length;
                    ApplicationController.Default.SetCustomStatus(
                        configuration.IsFiltered() ? "Filter Applied" : string.Empty,
                        String.Format("{0} Log Record{1}",
                                rowCount,
                                rowCount == 1 ? string.Empty : "s")
                        );
                    break;
            }
        }

        /// <summary>
        /// Check if the list of available logs has changed
        /// </summary>
        /// <param name="newfileList"></param>
        /// <returns></returns>
        private bool LogFilesChanged(LogFileList newfileList)
        {
            bool changed = false;
            
            if (currentSnapshot != null
                && newfileList.MaximumSqlLogs == currentSnapshot.MaximumSqlLogs
                && newfileList.UnlimitedSqlLogs == currentSnapshot.UnlimitedSqlLogs
                && newfileList.LogList.Count == currentSnapshot.LogList.Count)
            {
                foreach (LogFile newLog in newfileList.LogList)
                {
                    bool found = false;

                    foreach (LogFile log in currentSnapshot.LogList)
                    {
                        //if nothing about the file has changed. We are ignoring filenumber 0 which will be changing all the time
                        if ((newLog.LogType == log.LogType && newLog.LastModified == log.LastModified && (newLog.LogFileSize.Kilobytes - log.LogFileSize.Kilobytes < 500))
                            /*|| newLog.Number == 0*/)
                        {
                            found = true;
                            break;
                        }
                    }
                    //if the old record is not found then there has been a change
                    if (!found)
                    {
                        changed = true;
                        break;
                    }
                }
            }
            else if (newfileList.LogList.Count > 0)
            {
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// This refreshes the whole view
        /// </summary>
        private void RefreshLogs()
        {
            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
            SetViewStatus(ViewStatus.Loading);
        }

        private void PrintGrid()
        {
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - server logs as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        (lastEndDate.HasValue ? lastEndDate.Value : DateTime.Now).ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            ValueListDisplayStyle severityStyle = ((ValueList)logGrid.DisplayLayout.Bands[0].Columns["MessageType"].ValueList).DisplayStyle;

            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "SQLServerLogs";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ((ValueList)logGrid.DisplayLayout.Bands[0].Columns["MessageType"].ValueList).DisplayStyle = ValueListDisplayStyle.DisplayText;
                    ultraGridExcelExporter.Export(logGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                ((ValueList)logGrid.DisplayLayout.Bands[0].Columns["MessageType"].ValueList).DisplayStyle = severityStyle;
            }
        }

        private void SearchGrid(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            bool ignoreCase = !searchDialog.MatchCase;
            string searchText = ignoreCase ? searchDialog.SearchText.ToLower() : searchDialog.SearchText;
            bool searchUp = searchDialog.SearchUp;
            string searchCol = searchDialog.SearchIn;
            bool found = false;
            int selectedIndex;
            int rowIndex = -1;
            UltraGridRow row;

            UltraGridRow[] rows = logGrid.Rows.GetAllNonGroupByRows();
            if (logGrid.Visible && rows.Length > 0)
            {
                //set the selectedIndex as the current selection or the row we will end on
                if (logGrid.Selected.Rows.Count == 1)
                {
                    selectedIndex = logGrid.Selected.Rows[0].Index;
                    if (rows[selectedIndex] != logGrid.Selected.Rows[0])
                    {
                        // the grid has probably been grouped and the index is invalid, so find it manually
                        // this probably has performance problems, but it worked ok with 30k entries
                        // with 80k, it lagged somewhat
                        for (int i = selectedIndex; i < rows.Length; i++)
                        {
                            if (rows[i] == logGrid.Selected.Rows[0])
                            {
                                selectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    selectedIndex = (searchUp ? 0 : rows.GetLength(0) - 1);
                }

                while (rowIndex != selectedIndex)
                {
                    if (rowIndex < 0)
                    {
                        rowIndex = selectedIndex;
                    }
                    // if we have reached an end, then wrap to the other end and continue
                    if (searchUp && rowIndex == 0)
                    {
                        rowIndex = rows.GetLength(0) - 1;
                    }
                    else if (!searchUp && rowIndex >= rows.GetLength(0) - 1)
                    {
                        rowIndex = 0;
                    }
                    else
                    {
                        rowIndex += (searchUp ? -1 : 1);
                    }
                    row = rows[rowIndex];
                    if (row.IsDataRow)
                    {
                        string rowText = ignoreCase ? row.Cells[searchCol].Text.ToLower() : row.Cells[searchCol].Text;
                        if (rowText.Contains(searchText))
                        {
                            found = true;
                            row.Selected = true;
                            logGrid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                            break;
                        }
                    }
                }
                if (!found)
                {
                    ApplicationMessageBox.ShowMessage(String.Format("The selected text '{0}' was not found", searchDialog.SearchText));
                }

                Cursor = Cursors.Default;
            }
        }

        #region tree loading
        
        private void RefreshLogTree()
        {
            using (Log.DebugCall("RefreshLogTree()"))
            {
                Log.Info("Refreshing Log Tree...");
                refreshTreeStopwatch.Reset();
                refreshTreeStopwatch.Start();

                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                LogFileList data = managementService.GetLogList(configurationLogFiles);

                refreshTreeStopwatch.Stop();

                Log.Info(string.Format("Refresh Log Tree completed (Duration = {0}).", refreshTreeStopwatch.Elapsed));
                
                UpdateLogTree(data);
                RefreshLogs();
            }
        }
        
        /// <summary>
        /// Test the node to see if it exceeds the allowed limit
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="treeRootObject"></param>
        /// <returns></returns>
        private bool TestTreeNodeSizeLimitExceeded(TreeNode treeNode, object treeRootObject)
        {
            bool sizeLimitedExceeded = false;
            var treeRoot = new LogTreeRoot();

            if(treeRootObject is LogTreeRoot) treeRoot = (LogTreeRoot) treeRootObject;

            if (treeRootObject == null && treeNode.Parent != null)
            {
                if(treeNode.Parent.Text.ToLower().Contains("agent"))
                {
                    if (((LogFile)treeNode.Tag).LogFileSize.Kilobytes > agentLogSizeLimitKB)
                    {
                        agentLogLimitExceeded = true;
                        return true;
                    }
                    else
                    {
                        agentLogLimitExceeded = false;
                    }
                }
                else
                {
                    if (((LogFile)treeNode.Tag).LogFileSize.Kilobytes > errorLogSizeLimitKB)
                    {
                        errorLogLimitExceeded = true;
                        return true;
                    }else
                    {
                        errorLogLimitExceeded = false;
                    }
                }
                return false;
            }

            if (treeRoot == LogTreeRoot.SQLServer && treeNode.Nodes.Count > 0)
            {
                if (((LogFile) treeNode.Nodes[0].Tag).LogFileSize.Kilobytes > errorLogSizeLimitKB)
                {
                    var errorLogSizeLimit = new FileSize(errorLogSizeLimitKB);

                    lblLogLimitExceeded.Text = string.Format(LOGSIZEEXCEEDED,
                                                             ((double) (errorLogSizeLimit.Megabytes)).ToString("###,###,##0.#"),
                                                             ((double)(agentLogSizeLimitKB/1024)).ToString("###,###,##0.#"));
                    sizeLimitedExceeded = true;
                }
                else
                {
                    lblLogLimitExceeded.Visible = false;
                    lblLogLimitExceeded.SendToBack();
                }
            }
            else
            {
                //don't bother with this if we are already showing the message
                if (treeNode.Nodes.Count > 0)
                {
                    if (((LogFile)treeNode.Nodes[0].Tag).LogFileSize.Kilobytes > agentLogSizeLimitKB)
                    {
                        var agentLogSizeLimit = new FileSize(agentLogSizeLimitKB);

                        lblLogLimitExceeded.Text = string.Format(LOGSIZEEXCEEDED,
                                                             ((double)(errorLogSizeLimitKB/1024)).ToString("###,###,##0.#"),
                                                             ((double)(agentLogSizeLimit.Megabytes)).ToString("###,###,##0.#"));
                        sizeLimitedExceeded = true;
                    }
                    else
                    {
                        //if we are not showing the warning for error log and agent log is not over the threshold
                        //then don't show the warning
                        lblLogLimitExceeded.Visible = false;
                        lblLogLimitExceeded.SendToBack();
                    }
                }
            }
            return sizeLimitedExceeded;
        }

        private void UpdateLogTree(object data)
        {
            using (Log.DebugCall("UpdateLogTree"))
            {
                Log.Info("Updating Log Tree...");
                refreshTreeStopwatch.Reset();
                refreshTreeStopwatch.Start();

                Exception e = null;

                if (data != null && data is LogFileList)
                {
                    LogFileList snapshot = (LogFileList)data;
                    lock (updateLock)
                    {
                        if (snapshot.Error == null)
                        {
                            availableLogsTreeView.SuspendLayout();
                            treeUpdating = true;

                            // because the position and display name can change on a refresh
                            // build a keyed list of selections to reselect after rebuilding the tree
                            // unless all logs are selected for both SQL Server and Agent in which case
                            // don't bother because they will get selected no matter what
                            Dictionary<Pair<LogFileType?, String>, LogFile> selectionList = new Dictionary<Pair<LogFileType?, String>, LogFile>();
                            if (!selectAllSqlServer || !selectAllAgent)
                            {
                                foreach (TreeNode rootNode in availableLogsTreeView.Nodes)
                                {
                                    foreach (TreeNode node in rootNode.Nodes)
                                    {
                                        int lastselected = -1;
                                        if (node.Tag is LogFile && node.Checked)
                                        {
                                            selectionList.Add(((LogFile)node.Tag).InternalLogFileIdentifier, (LogFile)node.Tag);
                                            if (lastselected == ((LogFile)node.Tag).Number - 1)
                                            {
                                                lastselected += 1;
                                            }
                                        }
                                    }
                                }
                            }

                            availableLogsTreeView.Nodes[(int)LogTreeRoot.SQLServer].Nodes.Clear();
                            availableLogsTreeView.Nodes[(int)LogTreeRoot.Agent].Nodes.Clear();
                            logGrid.DisplayLayout.ValueLists["logNameValueList"].ValueListItems.Clear();

                            if (snapshot.LogList.Count > 0)
                            {
                                foreach (LogFile log in snapshot.LogList)
                                {
                                    bool checkNode = false;
                                    TreeNode parent = null;

                                    if (log.LogType == LogFileType.SQLServer)
                                    {
                                        parent = availableLogsTreeView.Nodes[(int)LogTreeRoot.SQLServer];
                                        if (selectAllSqlServer)
                                        {
                                            checkNode = true;
                                        }
                                    }
                                    else if (log.LogType == LogFileType.Agent)
                                    {
                                        parent = availableLogsTreeView.Nodes[(int)LogTreeRoot.Agent];
                                        if (selectAllAgent)
                                        {
                                            checkNode = true;
                                        }
                                    }

                                    if (parent != null && log.Number.HasValue && log.LogType.HasValue && log.LastModified.HasValue)
                                    {
                                        string logname = String.Format("{0} - {1}",
                                            log.Number == 0 ? "Current" : String.Format("Archive #{0}", log.Number.Value.ToString()),
                                            log.LastModified.ToString());

                                        TreeNode node = new TreeNode(String.Format("{0} ({1})",
                                                            logname,
                                                            log.LogFileSize.AsString(Application.CurrentCulture)));
                                        node.Tag = log;
                                        if (parent.Nodes.Count == 0)
                                        {
                                            parent.Nodes.Add(node);
                                        }
                                        else
                                        {
                                            int position = 0;
                                            foreach (TreeNode existingNode in parent.Nodes)
                                            {
                                                if (((LogFile)existingNode.Tag).Number > log.Number)
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    position++;
                                                }
                                            }
                                            parent.Nodes.Insert(position, node);
                                        }

                                        if (!treeInitialized && log.Number.Value == 0)
                                        {
                                            if (argument is LogTreeRoot)
                                            {
                                                switch ((LogTreeRoot)argument)
                                                {
                                                    case LogTreeRoot.SQLServer:
                                                        if(!TestTreeNodeSizeLimitExceeded(node, LogTreeRoot.SQLServer))
                                                        {
                                                            node.Checked =
                                                                parent.Checked = log.LogType == LogFileType.SQLServer;
                                                            informationLabel.Text = infoLabel;
                                                        }
                                                        else
                                                        {
                                                            informationLabel.Text = oneSizeLimitExceeded;
                                                        }
                                                        break;
                                                    case LogTreeRoot.Agent:
                                                        if (!TestTreeNodeSizeLimitExceeded(node, LogTreeRoot.Agent))
                                                        {
                                                            node.Checked =
                                                                parent.Checked = log.LogType == LogFileType.Agent;
                                                            informationLabel.Text = infoLabel;
                                                        }
                                                        else
                                                        {
                                                            informationLabel.Text = oneSizeLimitExceeded;
                                                        }
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                if (!TestTreeNodeSizeLimitExceeded(node, null))
                                                {
                                                    node.Checked =
                                                        parent.Checked = true;
                                                    informationLabel.Text = infoLabel;
                                                }
                                            }
                                        }
                                        else if (checkNode || selectionList.ContainsKey(((LogFile)node.Tag).InternalLogFileIdentifier))
                                        {
                                            //if this node has does not exceed the limit
                                            if (!TestTreeNodeSizeLimitExceeded(node, null))
                                            {
                                                node.Checked = true;
                                                informationLabel.Text = infoLabel;
                                            }
                                            else
                                            {
                                                //if this node does exceed the size limit but it was explicitly set
                                                node.Checked = selectionList.ContainsKey(((LogFile)node.Tag).InternalLogFileIdentifier);
                                            }
                                        }
                                        ValueListItem listItem = new ValueListItem(log.Name, logname);
                                        logGrid.DisplayLayout.ValueLists["logNameValueList"].ValueListItems.Add(listItem);
                                    }
                                }
                            }
                            else
                            {
                                // clear any last selections because they are no longer valid
                                lastEndDate = null;
                                selectionList.Clear();
                            }

                            treeInitialized = true;
                            argument = null;
                            treeUpdating = false;
                            availableLogsTreeView.ExpandAll();
                            currentSnapshot = snapshot;
                        }
                        else
                        {
                            e = snapshot.Error;
                            Log.Error("An error occurred while updating the Log Tree.", e);
                            currentSnapshot = null;
                        }
                    }
                }

                refreshTreeStopwatch.Stop();
                Log.Info(string.Format("Log Tree update completed (Duration = {0}).", refreshTreeStopwatch.Elapsed));
            }
        }

        private void EditAlertConfiguration()
        {
            if (logGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = logGrid.Selected.Rows[0];
                
                object server = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;

                if (server is string && !String.IsNullOrEmpty((string)server))
                {
                    MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[instanceId];
                    if (wrapper != null)
                    {
                        try
                        {
                            using(AlertConfigurationDialog alertConfigDialog = new AlertConfigurationDialog(wrapper.Id, false))
                            {

                                Metric metric = (Metric)Enum.ToObject(typeof(Metric), ((int)row.Cells[0].Value == 0 ? Metric.ErrorLog : Metric.AgentLog));

                                alertConfigDialog.CreateControl();
                                
                                alertConfigDialog.Select((Metric)metric);

                                alertConfigDialog.ShowDialog(this);
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                                                            ex);
                        }
                    }
                }
            }
        }

        private void UpdateDetailsPanel()
        {
            if (logGrid.Rows.Count > 0
                && logGrid.Selected.Rows.Count == 1
                && logGrid.Selected.Rows[0].IsDataRow)
            {
                UltraGridRow row = logGrid.Selected.Rows[0];
                if (((DataRowView)row.ListObject)["MessageType"] is DBNull)
                {
                    detailsLayoutPanel.Visible = false;
                }
                else
                {
                    ValueListItem severityItem = logGrid.DisplayLayout.ValueLists["severityValueList"].FindByDataValue(row.Cells["MessageType"].Value);
                    boundSeverityPictureBox.Image = (Image)severityItem.Appearance.Image;
                    boundSeverityLabel.Text = severityItem.DisplayText;
                    boundDateLabel.Text = row.Cells["Local Time"].Text;
                    boundSourceLabel.Text = row.Cells["Source"].Text;
                    boundMessageNumberLabel.Text = row.Cells["Message Number"].Text;
                    boundLogNameLabel.Text = row.Cells["Log Name"].Text;
                    boundLogTypeLabel.Text = row.Cells["Log Source"].Text;
                    boundLogFileLabel.Text = row.Cells["Log Name"].Value.ToString();
                    boundMessageTextBox.Text = row.Cells["Message"].Text;

                    detailsLayoutPanel.Visible = true;
                }
            }
            else
            {
                detailsLayoutPanel.Visible = false;
            }
        }

        #endregion

        #region Splitter Focus Handling

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

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void splitContainer2_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer2_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        #endregion

        #region Grid Column Menu Options

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                logGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                logGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                logGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                logGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
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
                    logGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    logGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            logGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            logGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(logGrid);
            dialog.Show(this);
        }

        #endregion

        #endregion

        #region events

        #region tree

        private void availableLogsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            selectionChanged = true;

            // prevent recursion when setting checked values on parents or children
            if (!treeUpdating)
            {
                availableLogsTreeView.SuspendLayout();
                treeUpdating = true;

                // if it is a log type root node, then check or uncheck all child log files to match
                if (e.Node.Level == 0)
                {
                    if (e.Node.Index == (int)LogTreeRoot.SQLServer)
                    {
                        selectAllSqlServer = e.Node.Checked;
                    }
                    else if (e.Node.Index == (int)LogTreeRoot.Agent)
                    {
                        selectAllAgent = e.Node.Checked;
                    }

                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        node.Checked = e.Node.Checked;
                    }
                }
                // if it is a log file node, then check the parent type or uncheck if all are unchecked
                else if (e.Node.Level == 1)
                {
                    if (e.Node.Checked)
                    {
                        e.Node.Parent.Checked = true;
                    }
                    else
                    {
                        bool foundChecked = false;
                        // if this is the last node being unchecked, then uncheck the parent
                        foreach (TreeNode node in e.Node.Parent.Nodes)
                        {
                            if (node.Checked)
                            {
                                foundChecked = true;
                                break;
                            }
                        }
                        e.Node.Parent.Checked = foundChecked;
                    }
                }

                // check to see if this node being checked caused the selected list to change
                // either directly or via recursion for the child log files
                if (selectionChanged && !selectionUpdating)
                {
                    RefreshLogs();
                }
                treeUpdating = false;
                availableLogsTreeView.ResumeLayout();
            }
        }

        private void availableLogsTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            //don't allow selection because it causes problems and is not relevant
            e.Cancel = true;
        }

        private void availableLogsTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //TreeNode selectedNode =
                //    availableLogsTreeView.GetNodeAt(e.X, e.Y);
                //if (selectedNode != null)
                //{
                //    selectedNode.IsSelected = true;
                //    toolbarsManager.SetContextMenuUltra(((TreeView)sender), "treeContextMenu");
                //}
                //else
                {
                    toolbarsManager.SetContextMenuUltra(((TreeView)sender), "treeContextMenu");
                }
            }
        }

        #endregion

        #region grid

        private void logGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            UpdateDetailsPanel();
        }

        private void logGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = logGrid.DisplayLayout.Bands[0];
            EditorWithText textEditor = new EditorWithText();
            band.Columns["MessageType"].Editor = textEditor;
            band.Columns["MessageType"].GroupByEvaluator = new StatusGroupByEvaluator(logGrid.DisplayLayout.ValueLists["severityValueList"]);
        }

        private void logGrid_MouseDown(object sender, MouseEventArgs e)
        {
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

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        #endregion

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
                case "refreshTreeButton":
                    RefreshLogTree();
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
                case "editAlertsConfigurationButton":
                    EditAlertConfiguration();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = logGrid.Rows.Count > 0 && logGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(logGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        #endregion

        #region Status Label
        private void lblLogLimitExceeded_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditAlertConfiguration(instanceId, Metric.ErrorLog);
        }

        private void informationLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!errorLogLimitExceeded && !agentLogLimitExceeded) return;

            if (Settings.Default.ColorScheme == "Light")
            {
                informationLabel.ForeColor = Color.Black;
                informationLabel.BackColor = Color.FromArgb(255, 189, 105);
                informationLabel.BackColor = Color.FromArgb(255, 189, 105);
            }         
        }

        private void informationLabel_MouseLeave(object sender, EventArgs e)
        {
            if (!errorLogLimitExceeded && !agentLogLimitExceeded) return;

            if (Settings.Default.ColorScheme == "Light")
            {
                informationLabel.ForeColor = Color.Black;
                informationLabel.BackColor = Color.FromArgb(211, 211, 211);
                informationLabel.BackColor = Color.FromArgb(211, 211, 211);
            }           
        }

        private void informationLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!errorLogLimitExceeded && !agentLogLimitExceeded) return;

            if (Settings.Default.ColorScheme == "Light")
            {
                informationLabel.ForeColor = Color.White;
                informationLabel.BackColor = Color.FromArgb(251, 140, 60);
                informationLabel.BackColor = Color.FromArgb(251, 140, 60);
            }            
        }

        private void informationLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!errorLogLimitExceeded && !agentLogLimitExceeded) return;

            if (Settings.Default.ColorScheme == "Light")
            {
                informationLabel.ForeColor = Color.Black;
                informationLabel.BackColor = Color.FromArgb(255, 189, 105);
                informationLabel.BackColor = Color.FromArgb(255, 189, 105);
            }

            if (agentLogLimitExceeded && !errorLogLimitExceeded)
            {
                EditAlertConfiguration(instanceId, Metric.AgentLog);
            }
            else
            {
                EditAlertConfiguration(instanceId, Metric.ErrorLog);
            }
        }
        
        #endregion

        private void hideAvailableLogsPanel_Click(object sender, EventArgs e)
        {
            AvailableLogsPanelVisible = false;
        }

        private void hideDetailsPanelButton_Click(object sender, EventArgs e)
        {
            DetailsPanelVisible = false;
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }
        #endregion

        private void LogsView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        public class StatusGroupByEvaluator : Infragistics.Win.UltraWinGrid.IGroupByEvaluatorEx
        {
            private ValueList valueList = null;
            public StatusGroupByEvaluator(ValueList valueList)
            {
                this.valueList = valueList;
            }

            public object GetGroupByValue(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                ValueListItem item = valueList.FindByDataValue(groupbyRow.Value);
                if (item != null)
                {
                    return item.DisplayText;
                }

                return groupbyRow.Value;
            }

            public bool DoesGroupContainRow(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                string groupbyValue = groupbyRow.Value as string;
                ValueListItem item = valueList.FindByDataValue(row.Cells[groupbyRow.Column].Value);
                
                if (item == null || groupbyValue == null)
                {
                    return false;
                }

                return groupbyValue.Equals(item.DisplayText);
            }

            public int Compare(UltraGridCell cell1, UltraGridCell cell2)
            {
                if (cell1.Value.Equals(cell2.Value)) return 0;
                if (cell2.Value.ToString() != "" && cell1.Value.ToString() != "")
                {
                    if ((int)(MonitoredState)cell1.Value < (int)(MonitoredState)cell2.Value) return -1;
                }
                return 1;
            }
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
