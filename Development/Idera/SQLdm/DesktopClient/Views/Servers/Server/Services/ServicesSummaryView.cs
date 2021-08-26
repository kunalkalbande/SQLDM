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
using ChartFX.WinForms;
using ChartFX.WinForms.Galleries;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Microsoft.SqlServer.MessageBox;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using System.Globalization;
using Constants = Idera.SQLdm.Common.Constants;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Services
{
    internal partial class ServicesSummaryView : ServerBaseView
    {
        #region constants

        
        private const string NO_ITEMS = @"There are no items to show in this view.";
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";
		///Ankit Nagpal --Sqldm10.0.0
        private const string SYSADMIN_MESSAGE = @"No data is available for this view.";
        private const string SELECT_SERVICES = @"Select one or more services.";
        private const string OperationalStatusMessage = "       {0}";

        // Help Popup Members
        private const string OsMetricsUnavailableHelp = HelpTopics.EnablingOsMetricsMonitoring;
        private const string LightweightPoolingHelp = HelpTopics.EnablingOsMetricsMonitoring;

        #region Services

        private const string SVC_SQLSERVER = "SQL Server";
        private const string SVC_SQLAGENT = "SQL Server Agent";
        private const string SVC_DTC = "Distributed Transaction Coordinator";
        private const string SVC_FULLTEXT = "Full-Text Search";

        private const string COL_INCLUDE = "Include In Chart";
        private const string COL_SERVICE = "Service";
        private const string COL_STATUS = "Status";
        private const string COL_RUNNING_SINCE = "Running Since";

        private enum chartServiceState
        {
            Running,
            Stopped,
            Unavailable,
            [Description("Not Installed")]
            NotInstalled,
            [Description("Unable To Monitor")]
            UnableToMonitor
        }

        #endregion

        #endregion

        #region fields

        private DateTime? historicalSnapshotDateTime;

        private ServicesSnapshot lastServicesSnapshot;
        private DataTable realTimeSnapshotsDataTable;


        private bool initialized = false;
        private bool enableOleAutomation = false;
        private bool lightweightPoolingEnabled = false;
        private DataTable currentDataTable;
        private ServiceName? _selectedService = null;
        private ServiceName? selectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                if (ServiceActionAllowedChanged != null)
                {
                    ServiceActionAllowedChanged(this, EventArgs.Empty);
                }
            }
        }
        private ServiceState selectedServiceState;

        private static readonly object updateLock = new object();
        private UltraGridColumn selectedColumn = null;
        private Control focused = null;
        private PointAttributes[] chartPointAttributes = new PointAttributes[5];

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastChartPanelVisible = true;
        #endregion

        #region constructors

        public ServicesSummaryView(int instanceId, ServerSummaryHistoryData historyData)
            : base(instanceId)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(serviceAvailabilityChart, toolbarsManager);

            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;
            HideFocusRectangleDrawFilter hideFocusRectangleDrawFilter = new HideFocusRectangleDrawFilter();
            servicesGrid.DrawFilter = hideFocusRectangleDrawFilter;

            // load value lists for grid display
            ValueListItem listItem;

            //fix the value that is not an actual windows acct
            servicesGrid.DisplayLayout.ValueLists["logOnAsValueList"].ValueListItems.Clear();
            listItem = new ValueListItem("LocalSystem", "Local System");
            servicesGrid.DisplayLayout.ValueLists["logOnAsValueList"].ValueListItems.Add(listItem);

            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Clear();
            listItem = new ValueListItem(ServiceState.Running, "Running");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.Paused, "Paused");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.Stopped, "Stopped");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.NotInstalled, "Not Installed");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.StartPending, "Start Pending");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.StopPending, "Stop Pending");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.ContinuePending, "Continue Pending");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.PausePending, "Pause Pending");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);
            listItem = new ValueListItem(ServiceState.UnableToMonitor, "Unable To Monitor");
            servicesGrid.DisplayLayout.ValueLists["statusValueList"].ValueListItems.Add(listItem);

            servicesGrid.DisplayLayout.ValueLists["serviceValueList"].ValueListItems.Clear();
            foreach (ServiceName service in Enum.GetValues(typeof(ServiceName)))
            {
                listItem = new ValueListItem(service, ApplicationHelper.GetEnumDescription(service));
                servicesGrid.DisplayLayout.ValueLists["serviceValueList"].ValueListItems.Add(listItem);
            }

            servicesGridStatusLabel.Text =
                serviceAvailabilityChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            servicesGrid.Visible =
                serviceAvailabilityChart.Visible = false;

            InitializeCurrentDataTable();
            InitializeRealTimeDataTable();
            InitializeChart();
            AdaptFontSize();
        }

        #endregion

        #region Properties

        public event EventHandler ChartPanelVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler ServiceActionAllowedChanged;

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        /// <summary>
        /// Get the availability of starting a service determined by whether a service is currently selected
        /// </summary>
        public bool StartAllowed
        {
            get
            {
                if (!isUserSysAdmin)
                {
                    return false;
                }
                bool clustered = ApplicationModel.Default.ActiveInstances[instanceId].Instance.ActiveClusterNode != null;
                return !clustered && selectedService != null && selectedService.Value != ServiceName.SqlServer
                            && selectedServiceState == ServiceState.Stopped; }
        }

        /// <summary>
        /// Get the availability of pausing a service determined by whether a service is currently selected
        /// </summary>
        public bool PauseAllowed
        {
            get { return false; } // selectedService != null && selectedService == ServiceName.SqlServer && selectedServiceState == ServiceState.Running; }
        }

        /// <summary>
        /// Get the availability of resuming a service determined by whether a service is currently paused
        /// </summary>
        public bool ResumeAvailable
        {
            get { return false; } // selectedService != null && selectedServiceState == ServiceState.Paused; }
        }

        /// <summary>
        /// Get the availability of stopping a service determined by whether a service is currently selected
        /// </summary>
        public bool StopAllowed
        {
            get
            {
                if (!isUserSysAdmin)
                {
                    return false;
                }
                bool clustered = ApplicationModel.Default.ActiveInstances[instanceId].Instance.ActiveClusterNode != null;
                return !clustered && selectedService != null && selectedService.Value != ServiceName.SqlServer
                           && selectedServiceState == ServiceState.Running;
            }
        }

        /// <summary>
        /// Get or Set the Chart visibility and trigger state update event if changed
        /// </summary>
        public bool ChartPanelVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;
                RestoreChart();

                if (ChartPanelVisibleChanged != null)
                {
                    ChartPanelVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get or Set the Databases Grid GroupByBox visibility and trigger state update event if changed
        /// </summary>
        public bool GridGroupByBoxVisible
        {
            get { return !servicesGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                servicesGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region methods


        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServicesSummaryView);
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.ServicesSummaryViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            lastChartPanelVisible =
                ChartPanelVisible = Settings.Default.ServicesSummaryViewChartVisible;

            if (Settings.Default.ServicesSummaryViewMainGrid != null)
            {
                lastMainGridSettings = Settings.Default.ServicesSummaryViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, servicesGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            GridSettings gridSettings = GridSettings.GetSettings(servicesGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || lastChartPanelVisible != ChartPanelVisible
                || !gridSettings.Equals(lastMainGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.ServicesSummaryViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastChartPanelVisible =
                    Settings.Default.ServicesSummaryViewChartVisible = ChartPanelVisible;
                lastMainGridSettings =
                    Settings.Default.ServicesSummaryViewMainGrid = gridSettings;
            }
        }

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                ServicesSummaryView_Fill_Panel.Visible = true;
                base.RefreshView();
            }
            else
            {
                ServicesSummaryView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            if (enableOleAutomation)
            {
                managementService.SendReconfiguration(
                    new ReconfigurationConfiguration(instanceId, "Ole Automation Procedures", 1));
                enableOleAutomation = false;
            }


            OnDemandConfiguration servicesSnapshotConfiguration = new OnDemandConfiguration(instanceId);

            return (Snapshot)managementService.GetServices(servicesSnapshotConfiguration);
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (enableOleAutomation)
            {
                operationalStatusImage.Image = Properties.Resources.StatusCriticalSmall;
                operationalStatusLabel.Text = string.Format(OperationalStatusMessage, "Failed to enable OLE Automation.");
                enableOleAutomation = false;
            }
            else
            {
                servicesGridStatusLabel.Text = UNABLE_TO_UPDATE;
				///Ankit Nagpal --Sqldm10.0.0
				///If not a sysadmin display sysadmin message
                if (!isUserSysAdmin) servicesGridStatusLabel.Text = SYSADMIN_MESSAGE;
            }

            base.HandleBackgroundWorkerError(e);
        }

        public override void UpdateData(object data)
        {
            Exception e = null;

            if (data != null && data is ServicesSnapshot)
            {
                lock (updateLock)
                {
                    ServicesSnapshot servicesSnapshot = data as ServicesSnapshot;

                    AddRealTimeSnapshots(servicesSnapshot);

                    if (servicesSnapshot != null)
                    {
                        if (servicesSnapshot.Error == null)
                        {
                            UpdateOperationalStatus(servicesSnapshot);

                            currentDataTable.BeginLoadData();

                            foreach (KeyValuePair<ServiceName, Service> service in servicesSnapshot.ServiceDetails)
                            {
                                bool rowSelected = true;
                                if (currentDataTable.Rows.Contains(service.Key))
                                {
                                    rowSelected = (bool) currentDataTable.Rows.Find(service.Key)[COL_INCLUDE];
                                }

                                currentDataTable.LoadDataRow(
                                    new object[]
                                        {
                                            rowSelected,
                                            service.Key,
                                            service.Value.RunningState,
                                            service.Value.RunningSince.HasValue
                                                ? (object) service.Value.RunningSince.Value.ToLocalTime()
                                                : null,
                                            service.Value.ServiceName,
                                            service.Value.Description,
                                            service.Value.StartupType,
                                            service.Value.LogOnAs
                                        }, true);
                            }

                            currentDataTable.EndLoadData();

                            if (!initialized)
                            {
                                if (lastMainGridSettings != null)
                                {
                                    GridSettings.ApplySettingsToGrid(lastMainGridSettings, servicesGrid);

                                    initialized = true;
                                }
                                else if (currentDataTable.Rows.Count > 0)
                                {
                                    foreach (UltraGridColumn column in servicesGrid.DisplayLayout.Bands[0].Columns)
                                    {
                                        if (column.Key != "Description")
                                        {
                                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                        }
                                    }

                                    initialized = true;
                                }

                                if (servicesGrid.Rows.Count > 0 && servicesGrid.Selected.Rows.Count == 0)
                                {
                                    servicesGrid.Rows[0].Selected = true;
                                }
                            }

                            servicesGrid_AfterSelectChange(servicesGrid,
                                                           new AfterSelectChangeEventArgs(typeof (UltraGridRow)));

                            servicesGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);
                            servicesGrid.Visible = true;
                            ConfigureChart();
                            UpdateChartData();

                            ApplicationController.Default.SetCustomStatus(
                                String.Format("{0} Service{1}",
                                              currentDataTable.Rows.Count,
                                              currentDataTable.Rows.Count == 1 ? string.Empty : "s")
                                );
                        }
                        else
                        {
                            e = servicesSnapshot.Error;
                            servicesGrid.Visible = false;
                            servicesGridStatusLabel.Text = UNABLE_TO_UPDATE;
							///Ankit Nagpal --Sqldm10.0.0
							///If not a sysadmin display sysadmin message
                            if (!isUserSysAdmin) servicesGridStatusLabel.Text = SYSADMIN_MESSAGE;
                            ApplicationController.Default.ClearCustomStatus();
                        }
                    }
                }
            }
            else
            {
                servicesGrid.Visible = false;
                servicesGrid.Text = UNABLE_TO_UPDATE;
				///Ankit Nagpal --Sqldm10.0.0
				///If not a sysadmin display sysadmin message
                if (!isUserSysAdmin) servicesGrid.Text = SYSADMIN_MESSAGE;
                ApplicationController.Default.ClearCustomStatus();
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
        }

        public void AddRealTimeSnapshots(ServicesSnapshot snapshot)
        {
            if (snapshot != null)
            {
                lock (updateLock)
                {
                    if (snapshot.Error == null)
                    {   // add columns to table
                        foreach (ServiceName service in snapshot.ServiceDetails.Keys)
                        {
                            if (!realTimeSnapshotsDataTable.Columns.Contains(ApplicationHelper.GetEnumDescription(service)))
                            {
                                realTimeSnapshotsDataTable.Columns.Add(ApplicationHelper.GetEnumDescription(service), typeof(int));
                            }
                        }
                    }

                    var dataRow = realTimeSnapshotsDataTable.NewRow();
                    dataRow["Date"] = DateTime.Now;
                    try
                    {
                        if (snapshot.Error == null && dataRow != null)
                        {   // add data to columns
                            foreach (KeyValuePair<ServiceName, Service> service in snapshot.ServiceDetails)
                            {
                                dataRow[ApplicationHelper.GetEnumDescription(service.Key)] = service.Value.RunningState;
                            }

                            lastServicesSnapshot = snapshot;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("An error occurred while adding the services snapshot.", e);
                    }

                    // add row to table
                    realTimeSnapshotsDataTable.Rows.Add(dataRow);

                    // groom data table
                    var groomThreshold = DateTime.Now.Subtract(TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes));
                    var groomedRows = realTimeSnapshotsDataTable.Select(string.Format("Date < #{0}#", groomThreshold.ToString(CultureInfo.InvariantCulture))); // SQLDM-19237, Tolga K
                    foreach (var row in groomedRows)
                    {
                        row.Delete();
                    }
                    realTimeSnapshotsDataTable.AcceptChanges();
                }
            }
        }

        #endregion

        public void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        public void StartService()
        {
            PerformServiceAction(ServiceControlConfiguration.ServiceControlAction.Start);
        }

        public void PauseService()
        {
            if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                        String.Format("You are about to pause the {0} service on {1}. Do you want to continue?",
                                        ApplicationHelper.GetEnumDescription(selectedService),
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName),
                                        null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                PerformServiceAction(ServiceControlConfiguration.ServiceControlAction.Pause);
            }
        }

        public void ResumeService()
        {
            PerformServiceAction(ServiceControlConfiguration.ServiceControlAction.Continue);
        }

        public void StopService()
        {
            if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                        String.Format("You are about to stop the {0} service on {1}. Do you want to continue?",
                                        ApplicationHelper.GetEnumDescription(selectedService),
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

            AuditingEngine.SetContextData(
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

            ServiceControlConfiguration config = new ServiceControlConfiguration(instanceId, selectedService.Value, action);
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
                                                                    ApplicationHelper.GetEnumDescription(selectedService.Value)),
                                                snapshot.Error);
            }
        }

        private void UpdateOperationalStatus(ServicesSnapshot snapshot)
        {
            if (snapshot != null)
            {
                // Hide message for Services > Summary (Managed Instance View)
                var isAzureManagedInstance = ApplicationModel.Default.AllInstances.ContainsKey(instanceId) &&
                                     ApplicationModel.Default.AllInstances[instanceId].CloudProviderId ==
                                     Constants.MicrosoftAzureManagedInstanceId;
                var isAwsInstance = ApplicationModel.Default.AllInstances.ContainsKey(instanceId) &&
                                     ApplicationModel.Default.AllInstances[instanceId].CloudProviderId ==
                                     Constants.AmazonRDSId;
                if (snapshot.OsMetricsUnavailable && !isAzureManagedInstance && !isAwsInstance)
                {
                    ApplicationMessageBox box = new ApplicationMessageBox();
                    box.Symbol = ExceptionMessageBoxSymbol.Information;
                    box.Buttons = ExceptionMessageBoxButtons.OK;
                    box.ShowCheckBox = true;
                    box.CheckBoxText = "Don't show this message again.";
                    box.Caption = "Operating System metrics collection is disabled.";
                    
                    if (snapshot.LightweightPoolingEnabled)
                    {
                        operationalStatusLabel.Text = string.Format(OperationalStatusMessage,
                                          "OLE Automation is needed to collect service details, but is unavailable because Lightweight Pooling is enabled. Click here for more information.");
                        box.Text = "OLE Automation is needed to collect service details, but is unavailable because Lightweight Pooling is enabled.";
                        lightweightPoolingEnabled = true;

                        if ((operationalStatusPanel.Visible == false) && (Properties.Settings.Default.ShowMessage_LightweightPooling))
                        {
                            box.Show(this);
                            Properties.Settings.Default.ShowMessage_LightweightPooling = !box.IsCheckBoxChecked;
                        }
                    }
                    else
                    {
                        operationalStatusLabel.Text = string.Format(OperationalStatusMessage,
                                          "Operating System metrics collection must be enabled to collect service details on this server. Click here for more information.");
                        box.Text = "Operating System metrics collection must be enabled to collect service details on this server.";
                        lightweightPoolingEnabled = false;

                        if ((operationalStatusPanel.Visible == false) && (Properties.Settings.Default.ShowMessage_OLEAutomationUnavailable))
                        {
                            box.Show(this);
                            Properties.Settings.Default.ShowMessage_OLEAutomationUnavailable = !box.IsCheckBoxChecked;
                        }
                    }
                    operationalStatusImage.Image = Properties.Resources.StatusCriticalSmall;
                    operationalStatusPanel.Visible = true;
                }
                else
                {
                    operationalStatusPanel.Visible = false;
                }
            }
        }

        #region DataTables

        private void InitializeCurrentDataTable()
        {
            currentDataTable = new DataTable();
            currentDataTable.Columns.Add(COL_INCLUDE, typeof(bool));
            currentDataTable.Columns.Add(COL_SERVICE, typeof(ServiceName));
            currentDataTable.Columns.Add(COL_STATUS, typeof(int));
            currentDataTable.Columns.Add(COL_RUNNING_SINCE, typeof(DateTime));
            currentDataTable.Columns.Add("Service Name", typeof(string));
            currentDataTable.Columns.Add("Description", typeof(string));
            currentDataTable.Columns.Add("Startup Type", typeof(string));
            currentDataTable.Columns.Add("Log On As", typeof(string));

            currentDataTable.PrimaryKey = new DataColumn[] { currentDataTable.Columns[COL_SERVICE] };
            currentDataTable.CaseSensitive = true;

            currentDataTable.DefaultView.Sort = COL_SERVICE;

            servicesGrid.DataSource = currentDataTable;
        }

        private void InitializeRealTimeDataTable()
        {
            realTimeSnapshotsDataTable = new DataTable();
            realTimeSnapshotsDataTable.Columns.Add("Date", typeof(DateTime));
        }

        private void UpdateChartData()
        {
            if (realTimeSnapshotsDataTable != null)
            {
                // clear all data points and rebuild the data
                serviceAvailabilityChart.Data.Clear();
                serviceAvailabilityChart.Points.Clear();
                serviceAvailabilityChart.AxisX.Labels.Clear();

                // create new datapoints for each selected series
                int dataPoint = -1;
                SortedList<string, int> serviceList = new SortedList<string, int>();
                foreach (DataRow row in currentDataTable.Rows)
                {
                    if ((bool) row[COL_INCLUDE])
                    {
                        dataPoint++;
                        string service = ApplicationHelper.GetEnumDescription((ServiceName) row[COL_SERVICE]);

                        serviceList.Add(service, 0);
                    }
                }

                // apply the latest filter to the history data
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                realTimeSnapshotsDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));

                // add data to the points
                if (serviceList.Count > 0)
                {
                    int series = 0;
                    for (int svcIndex = serviceList.Keys.Count - 1; svcIndex >= 0; svcIndex--)
                    {
                        string service = serviceList.Keys[svcIndex];
                        serviceAvailabilityChart.AxisX.Labels[series] = service;
                        if (realTimeSnapshotsDataTable.DefaultView.Count > 1)
                        {
                            DateTime lowDate = (DateTime) realTimeSnapshotsDataTable.Rows[0]["Date"];
                            DateTime currDate = (DateTime) realTimeSnapshotsDataTable.Rows[0]["Date"];
                            DateTime? prevDate = null;
                            dataPoint = 0;
                            foreach (DataRowView row in realTimeSnapshotsDataTable.DefaultView)
                            {
                                currDate = (DateTime) row["Date"];
                                if (prevDate.HasValue)
                                {
                                    if (row[service] != DBNull.Value)
                                    {
                                        int state;

                                        serviceAvailabilityChart.Data.YFrom[dataPoint, series] = prevDate.Value.ToOADate();
                                        serviceAvailabilityChart.Data.Y[dataPoint, series] = currDate.ToOADate();

                                        switch ((ServiceState) row[service])
                                        {
                                            case ServiceState.Running:
                                                state = (int) chartServiceState.Running;
                                                break;
                                            case ServiceState.Stopped:
                                                state = (int) chartServiceState.Stopped;
                                                break;
                                            case ServiceState.NotInstalled:
                                                state = (int) chartServiceState.NotInstalled;
                                                break;
                                            case ServiceState.ContinuePending:
                                            case ServiceState.Paused:
                                            case ServiceState.PausePending:
                                            case ServiceState.StartPending:
                                            case ServiceState.StopPending:
                                                state = (int) chartServiceState.Unavailable;
                                                break;
                                            default:
                                                state = (int) chartServiceState.UnableToMonitor;
                                                break;
                                        }

                                        serviceAvailabilityChart.Points[dataPoint, series] = chartPointAttributes[state];
                                    }
                                    
                                    dataPoint++;
                                }
                                else
                                {
                                    lowDate = currDate;
                                }

                                prevDate = currDate;
                            }
                            serviceAvailabilityChart.AllSeries.AxisY.Min = lowDate.ToOADate();
                            serviceAvailabilityChart.AllSeries.AxisY.Max = currDate.ToOADate();
                        }
                        else
                        {
                            DateTime currDate = DateTime.Now;

                            if (lastServicesSnapshot != null &&
                                lastServicesSnapshot.TimeStamp.HasValue)
                            {
                                currDate = lastServicesSnapshot.TimeStamp.Value;
                            }

                            serviceAvailabilityChart.Data.YFrom[0, series] = currDate.ToOADate();
                            serviceAvailabilityChart.Data.Y[0, series] = currDate.ToOADate();
                        }
                        series++;
                    }
                }
                serviceAvailabilityChart.Data.RecalculateScale();
            }
        }

        #endregion

        #region grid

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - services as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            servicesGrid.SuspendLayout();
            bool selectionHidden = servicesGrid.DisplayLayout.Bands[0].Columns[COL_INCLUDE].Hidden;
            servicesGrid.DisplayLayout.Bands[0].Columns[COL_INCLUDE].Hidden = true;

            ultraPrintPreviewDialog.ShowDialog();
            
            servicesGrid.DisplayLayout.Bands[0].Columns[COL_INCLUDE].Hidden = selectionHidden;
            servicesGrid.ResumeLayout();
        }

        private void SaveGrid()
        {
            bool selectionHidden = servicesGrid.DisplayLayout.Bands[0].Columns[COL_INCLUDE].Hidden;

            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "ServicesSummary";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    servicesGrid.SuspendLayout();
                    servicesGrid.DisplayLayout.Bands[0].Columns[COL_INCLUDE].Hidden = true;
                    ultraGridExcelExporter.Export(servicesGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }

            servicesGrid.DisplayLayout.Bands[0].Columns[COL_INCLUDE].Hidden = selectionHidden;
            servicesGrid.ResumeLayout();
        }

        private void updateCellAppearance(UltraGridCell cell, bool hilight)
        {
            if (hilight)
            {
                cell.Appearance.BackColor = Color.Red;
                cell.Appearance.ForeColor = Color.White;
                cell.Appearance.FontData.Bold = DefaultableBoolean.True;
            }
            else
            {
                cell.Appearance.BackColor = Color.White;
                cell.Appearance.ForeColor = Color.Black;
                cell.Appearance.FontData.Bold = DefaultableBoolean.False;
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                servicesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                servicesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                servicesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                servicesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    servicesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    servicesGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            servicesGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            servicesGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(servicesGrid);
            dialog.Show(this);
        }

        #endregion

        #region chart

        private void InitializeChart()
        {
            ((Bar)serviceAvailabilityChart.GalleryAttributes).Overlap = true;
            serviceAvailabilityChart.ToolTipFormat = "%l\n%L\nFrom: %i To: %v";
            serviceAvailabilityChart.AxisY.LabelsFormat.Format = AxisFormat.Time;
            serviceAvailabilityChart.LegendBox.PlotAreaOnly = false;
            serviceAvailabilityChart.Printer.Orientation = PageOrientation.Landscape;
            serviceAvailabilityChart.Printer.Compress = true;
            serviceAvailabilityChart.Printer.ForceColors = true;
            serviceAvailabilityChart.Printer.Document.DocumentName = "Service Availability Chart";
            serviceAvailabilityChart.ToolBar.RemoveAt(0);

            // Hide standard legend items
            LegendItemAttributes legendItems = new LegendItemAttributes();
            legendItems.Visible = false;
            serviceAvailabilityChart.LegendBox.ItemAttributes[serviceAvailabilityChart.Series] = legendItems;

            // Configure point attributes for various states
            chartPointAttributes[(int)chartServiceState.Running] = new PointAttributes();
            chartPointAttributes[(int)chartServiceState.Running].Color = Color.Green;
            //chartPointAttributes[(int)chartServiceState.Paused] = new PointAttributes();
            //chartPointAttributes[(int)chartServiceState.Paused].Color = Color.Goldenrod;
            chartPointAttributes[(int)chartServiceState.Stopped] = new PointAttributes();
            chartPointAttributes[(int)chartServiceState.Stopped].Color = Color.Red;
            chartPointAttributes[(int)chartServiceState.Unavailable] = new PointAttributes();
            chartPointAttributes[(int)chartServiceState.Unavailable].Color = Color.Yellow;
            chartPointAttributes[(int)chartServiceState.NotInstalled] = new PointAttributes();
            chartPointAttributes[(int)chartServiceState.NotInstalled].Color = Color.Gray;
            chartPointAttributes[(int)chartServiceState.UnableToMonitor] = new PointAttributes();
            chartPointAttributes[(int)chartServiceState.UnableToMonitor].Color = Color.CornflowerBlue;

            // Configure Custom Legend
            CustomLegendItem legendItem = new CustomLegendItem();
            legendItem.Attributes = chartPointAttributes[(int)chartServiceState.Running];
            legendItem.Text = ApplicationHelper.GetEnumDescription(ServiceState.Running);
            legendItem.MarkerShape = MarkerShape.Rect;
            serviceAvailabilityChart.LegendBox.CustomItems.Add(legendItem);

            //legendItem = new CustomLegendItem();
            //legendItem.Attributes = chartPointAttributes[(int)chartServiceState.Paused];
            //legendItem.Text = ApplicationHelper.GetEnumDescription(ServiceState.Paused);
            //legendItem.MarkerShape = MarkerShape.Rect;
            //serviceAvailabilityChart.LegendBox.CustomItems.Add(legendItem);

            legendItem = new CustomLegendItem();
            legendItem.Attributes = chartPointAttributes[(int)chartServiceState.Stopped];
            legendItem.Text = ApplicationHelper.GetEnumDescription(chartServiceState.Stopped);
            legendItem.MarkerShape = MarkerShape.Rect;
            serviceAvailabilityChart.LegendBox.CustomItems.Add(legendItem);

            legendItem = new CustomLegendItem();
            legendItem.Attributes = chartPointAttributes[(int)chartServiceState.Unavailable];
            legendItem.Text = ApplicationHelper.GetEnumDescription(chartServiceState.Unavailable);
            legendItem.MarkerShape = MarkerShape.Rect;
            serviceAvailabilityChart.LegendBox.CustomItems.Add(legendItem);

            legendItem = new CustomLegendItem();
            legendItem.Attributes = chartPointAttributes[(int)chartServiceState.NotInstalled];
            legendItem.Text = ApplicationHelper.GetEnumDescription(chartServiceState.NotInstalled);
            legendItem.MarkerShape = MarkerShape.Rect;
            serviceAvailabilityChart.LegendBox.CustomItems.Add(legendItem);

            legendItem = new CustomLegendItem();
            legendItem.Attributes = chartPointAttributes[(int)chartServiceState.UnableToMonitor];
            legendItem.Text = ApplicationHelper.GetEnumDescription(chartServiceState.UnableToMonitor);
            legendItem.MarkerShape = MarkerShape.Rect;
            serviceAvailabilityChart.LegendBox.CustomItems.Add(legendItem);

            ConfigureChart();

            serviceAvailabilityChart.Visible = false;
        }

        private void ConfigureChart()
        {
            serviceAvailabilityChart.SuspendLayout();
            serviceAvailabilityChart.DataSourceSettings.Fields.Clear();

            bool showService = false;
            foreach (DataRow row in currentDataTable.Rows)
            {
                if ((bool)row[COL_INCLUDE])
                {
                    showService = true;
                    break;
                }
            }

            if (showService)
            {
                serviceAvailabilityChart.Visible = true;
            }
            else
            {
                if (initialized)
                {
                    serviceAvailabilityChartStatusLabel.Text = SELECT_SERVICES;
                }
                serviceAvailabilityChart.Visible = false;
            }
        }

        private void AddSelectedServiceToChart()
        {
            if (servicesGrid.Selected.Rows.Count == 1)
            {
                if (servicesGrid.Selected.Rows[0].ListObject is DataRowView)
                {
                    DataRowView dataRow = servicesGrid.Selected.Rows[0].ListObject as DataRowView;
                    string serviceType = ApplicationHelper.GetEnumDescription((ServiceName)dataRow[COL_SERVICE]);
                    AddServiceToChart((int)dataRow[COL_SERVICE], serviceType);
                    dataRow[COL_INCLUDE] = !(bool)dataRow[COL_INCLUDE];
                    UpdateChartData();
                    servicesGrid.Invalidate();
                }
            }
        }

        private void AddServiceToChart(int serviceName, string serviceType)
        {
            serviceAvailabilityChart.AxisX.Labels[serviceName] = serviceType;

            serviceAvailabilityChart.Visible = true;
            ChartPanelVisible = true;
        }

        private void RemoveSelectedServiceFromChart()
        {
            if (servicesGrid.Selected.Rows.Count == 1)
            {
                if (servicesGrid.Selected.Rows[0].ListObject is DataRowView)
                {
                    DataRowView dataRow = servicesGrid.Selected.Rows[0].ListObject as DataRowView;
                    string serviceType = ApplicationHelper.GetEnumDescription((ServiceName)dataRow[COL_SERVICE]);
                    RemoveServiceFromChart(serviceType, false);
                    dataRow[COL_INCLUDE] = !(bool)dataRow[COL_INCLUDE];
                    UpdateChartData();
                    servicesGrid.Invalidate();
                }
            }
        }

        private void RemoveServiceFromChart(string serviceType, bool supressFieldMapLookupTableModifications)
        {
            FieldMap existingFieldMap = new FieldMap();

            bool found = false;
            foreach (FieldMap fieldMap in serviceAvailabilityChart.DataSourceSettings.Fields)
            {
                if (fieldMap.Name == serviceType)
                {
                    found = true;
                    existingFieldMap = fieldMap;
                    break;
                }
            }
            if (found)
            {
                serviceAvailabilityChart.DataSourceSettings.Fields.Remove(existingFieldMap);
            }

            if (serviceAvailabilityChart.DataSourceSettings.Fields.Count == 1)
            {
                serviceAvailabilityChart.Visible = false;
            }
            else
            {
                serviceAvailabilityChart.DataSourceSettings.ReloadData();
                serviceAvailabilityChart.Invalidate();
            }
        }

        private void ToggleChartToolbar(bool Visible)
        {
            serviceAvailabilityChart.ToolBar.Visible = Visible;
        }

        private void PrintChart()
        {
            ExportHelper.ChartHelper.PrintChartWithTitle(this, serviceAvailabilityChart, "Service Availability", ultraPrintPreviewDialog);
        }

        private void SaveChartData()
        {
            ExportHelper.ChartHelper.ExportToCsv(this, serviceAvailabilityChart, "ServiceAvailability");
        }

        private void SaveChartImage()
        {
            ExportHelper.ChartHelper.ExportImageWithTitle(this, serviceAvailabilityChart, "Service Availability", "ServiceAvailability");
        }

        private void MaximizeChart()
        {
            splitContainer.Visible = false;
            splitContainer.Panel2.Controls.Remove(chartContentPanel);
            maximizeChartButton.Visible = false;
            restoreChartButton.Visible = true;
            ServicesSummaryView_Fill_Panel.Controls.Add(chartContentPanel);
        }

        private void RestoreChart()
        {
            ServicesSummaryView_Fill_Panel.Controls.Remove(chartContentPanel);
            maximizeChartButton.Visible = true;
            restoreChartButton.Visible = false;
            splitContainer.Panel2.Controls.Add(chartContentPanel);
            splitContainer.Visible = true;
        }

        #endregion

        #endregion

        #region events

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

        #region toolbars

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridDataContextMenu")
            {
                ((PopupMenuTool) e.Tool).Tools["startServiceButton"].SharedProps.Visible =
                    ((PopupMenuTool)e.Tool).Tools["stopServiceButton"].SharedProps.Visible =
                        ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;

                ((PopupMenuTool)e.Tool).Tools["startServiceButton"].SharedProps.Enabled = StartAllowed;
                ((PopupMenuTool)e.Tool).Tools["stopServiceButton"].SharedProps.Enabled = StopAllowed;

                ((PopupMenuTool)e.Tool).Tools["pauseServiceButton"].SharedProps.Visible =
                    ((PopupMenuTool)e.Tool).Tools["resumeServiceButton"].SharedProps.Visible = PauseAllowed &&
                        ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
                
                if (PauseAllowed)
                {
                    ((PopupMenuTool)e.Tool).Tools["pauseServiceButton"].SharedProps.Enabled = PauseAllowed;
                    ((PopupMenuTool)e.Tool).Tools["resumeServiceButton"].SharedProps.Enabled = !PauseAllowed;
                }
            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = servicesGrid.Rows.Count > 0 && servicesGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(servicesGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
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
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "toggleChartToolbarButton":
                    ToggleChartToolbar(((StateButtonTool)e.Tool).Checked);
                    break;
                case "printChartButton":
                    PrintChart();
                    break;
                case "exportChartDataButton":
                    SaveChartData();
                    break;
                case "exportChartImageButton":
                    SaveChartImage();
                    break;
                case "startServiceButton":
                    StartService();
                    break;
                case "pauseServiceButton":
                    PauseService();
                    break;
                case "resumeServiceButton":
                    ResumeService();
                    break;
                case "stopServiceButton":
                    StopService();
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
            }
        }

        #endregion

        #region grid

        private void servicesGrid_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                ChartPanelVisible = true;
                bool isIncludedInChart = (bool)e.Row.Cells[COL_INCLUDE].Value;
                if (isIncludedInChart)
                {
                    RemoveSelectedServiceFromChart();
                }
                else
                {
                    AddSelectedServiceToChart();
                }
            }
        }

        private void servicesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            UIElement selectedElement =
                ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

            if (selectedElement != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (selectedElement is CheckIndicatorUIElement)
                    {
                        RowUIElement selectedRowUIElement = selectedElement.GetAncestor(typeof(RowUIElement)) as RowUIElement;

                        if (selectedRowUIElement != null && selectedRowUIElement.Row.ListObject is DataRowView)
                        {
                            DataRowView dataRow = selectedRowUIElement.Row.ListObject as DataRowView;
                            bool isIncludedInChart = (bool)dataRow[COL_INCLUDE];

                            if (isIncludedInChart)
                            {
                                RemoveSelectedServiceFromChart();
                            }
                            else
                            {
                                AddSelectedServiceToChart();
                            }
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                    if (contextObject is ColumnHeader)
                    {
                        ColumnHeader columnHeader =
                            contextObject as ColumnHeader;
                        selectedColumn = columnHeader.Column;
                        ((Infragistics.Win.UltraWinToolbars.StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
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
        }

        private void servicesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (servicesGrid.Rows.Count > 0
                && servicesGrid.Selected.Rows.Count == 1
                && servicesGrid.Selected.Rows[0].IsDataRow)
            {
                // be sure to set the state first so the service update will work
                selectedServiceState = (ServiceState)servicesGrid.Selected.Rows[0].Cells["Status"].Value;
                selectedService = (ServiceName)servicesGrid.Selected.Rows[0].Cells["Service"].Value;
            }
            else
            {
                selectedService = null;
            }
        }

        #endregion

        private void restoreChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart();
        }

        private void maximizeChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart();
        }

        private void closeChartButton_Click(object sender, EventArgs e)
        {
            ChartPanelVisible = false;
            RestoreChart();
        }

        #region operationalStatusLabel

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

            if (lightweightPoolingEnabled)
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(LightweightPoolingHelp);
            }
            else
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(OsMetricsUnavailableHelp);
            }
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        #endregion

        private void ServicesSummaryView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        #endregion

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
