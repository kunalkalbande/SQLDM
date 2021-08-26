//------------------------------------------------------------------------------
// <copyright file="AgentJobSummaryProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.Collections.Generic;
    using Common;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;
    using Objects;
    using Wintellect.PowerCollections;
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Auditing;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Configuration.ServerActions;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Idera.SQLdm.DesktopClient.Controls;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Idera.SQLdm.DesktopClient.Properties;
    using Infragistics.Win.UltraWinToolbars;
    using Microsoft.SqlServer.MessageBox;
    using System.Reflection;
    using System.Linq;
    using TracerX;
    using Infragistics.Win;

    public enum MonitoredSqlServerInstancePropertiesDialogPropertyPages
    {
        Popular = 0,
        Baseline = 1,
        QueryMonitor = 2,
        ActivityMonitor = 3,
        Replication = 4,
        TableStatistics = 5,
        CustomCounters = 6,
        MaintenanceMode = 7,
        OleAutomation = 8,
        Disk = 9,
        ClusterSettings = 10,
        WaitStatistics = 11,
        Virtualization = 12,
        AnalysisConfiguration = 13
    }

    public enum MonitoredAzureInstancePropertiesDialogPropertyPages
    {
        Popular = 0,
        Baseline = 1,
        QueryMonitor = 2,
        ActivityMonitor = 3,
        CustomCounters = 4,
        MaintenanceMode = 5,
        WaitStatistics = 6,
        AnalysisConfiguration = 7

    }

    public enum MonitoredAWSInstancePropertiesDialogPropertyPages
    {
        Popular = 0,
        Baseline = 1,
        QueryMonitor = 2,
        ActivityMonitor = 3,
        TableStatistics = 4,
        CustomCounters = 5,
        MaintenanceMode = 6,
        WaitStatistics = 7
    }

    public partial class MonitoredSqlServerInstancePropertiesDialog : Form
    {
        private const string PasswordDecoyText = "XXXXXXXXXXXXXXX";
        private const string NewTagToolKey = "New tag...";
        private const string AdhocDiskInstructionText = "< Type semicolon separated names >";
        private const string ChooseTagText = "< Click here to choose tags >";
        // SQLdm Minimum Privileges - Varun Chopra - Wait Monitoring Permissions Modifications
        private const string WaitMonitoringWarningQs = "Warning: The SQL Diagnostic Manager monitoring account " +
                                                       "does not have the necessary permissions to collect this data using Query Store";
        private const string WaitMonitoringWarningEx = "Warning: The SQL Diagnostic Manager monitoring account " +
                                                       "does not have the necessary permissions to collect this data using ExtendedEvents";
        private const string WaitMonitoringWarningTrc = "Warning: The SQL Diagnostic Manager monitoring account " +
                                                       "does not have the necessary permissions to collect this data using Trace";
        // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store
        private const string QueryMonitoringWarningQs = "Warning: The SQL Diagnostic Manager monitoring account does not have the necessary permissions to collect this data using Query Store";

        // SQLdm Minimum Privileges - Activity Monitor and Query monitoring warning labels
        private const string QueryMonitoringWarningEx = "Warning: The SQL Diagnostic Manager monitoring account does not have the necessary permissions to collect this data using ExtendedEvents";
        private const string QueryMonitoringWarningTrc = "Warning: The SQL Diagnostic Manager monitoring account does not have the necessary permissions to collect this data using Trace";

        private const string ActivityMonitorWarningEx = "Warning: The SQL Diagnostic Manager monitoring account " +
                                                       "does not have the necessary permissions to collect this data using ExtendedEvents";
        private const string ActivityMonitorWarningTrc = "Warning: The SQL Diagnostic Manager monitoring account " +
                                                       "does not have the necessary permissions to collect this data using Trace";
        private MonitoredSqlServerConfiguration configuration;
        private MonitoredSqlServer savedServer = null;
        private readonly int id;
        private readonly int? cloudProviderId;

        MetricDefinitions metrics = null;
        private MultiDictionary<int, int> customCounterTagsLookupTable;
        private readonly SortedDictionary<string, int> availableTags = new SortedDictionary<string, int>();
        private bool tagsChanged = false;
        private bool customCountersInitialized = false;
        private BackgroundWorker availableDisksBackgroundWorker;
        private DateTime serverLocalDateTime = DateTime.MinValue;
        private DateTime collectionServiceLocalDateTime = DateTime.MinValue;
        private ServerVersion serverVersion = null;

        // SQLdm Minimum Privileges - Varun Chopra - Server Permissions
        private MinimumPermissions minimumPermissions;
        private MetadataPermissions metadataPermissions;
        private CollectionPermissions collectionPermissions;

        private bool alertRefreshInMinutes;

        private bool viewClosing = false;
        private int blockedProcessThreshold;

        private string oldTag = "";

        private VirtualizationConfiguration oldVmConfiguration;

        private string CLOUDPROVIDER = "Cloud Provider";
        //To log errors
        Logger Log = Logger.GetLogger("MonitoredSqlServerInstancePropertiesDialog");

        public MonitoredSqlServerInstancePropertiesDialog(int id)
        {
            this.id = id;

            MonitoredSqlServer instance =
                RepositoryHelper.GetMonitoredSqlServer(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, id);

            this.alertRefreshInMinutes = instance.AlertRefresInMinutes;

            ApplicationModel.Default.RefreshLocalTags();
            //SQLDM 10.1 (srishti purohit)
            //removing global tags from list 
            foreach (Tag tag in ApplicationModel.Default.LocalTags)
            {
                availableTags.Add(tag.Name, tag.Id);
            }

            if (instance == null)
            {
                string instanceName = id.ToString();

                if (ApplicationModel.Default.ActiveInstances.Contains(id))
                {
                    instanceName = ApplicationModel.Default.ActiveInstances[id].InstanceName;
                }

                throw new ArgumentException(
                    string.Format("{0} is not registered in the repository.", instanceName));
            }

            configuration = instance.GetConfiguration();
            cloudProviderId = configuration.CloudProviderId;
            oldVmConfiguration = configuration.VirtualizationConfiguration;

            AuditingEngine.SetAuxiliarData("VmConfigurationOldValue", oldVmConfiguration);

            InitializeComponent();
            InitializeForm();

            this.AdaptFontSize();

        }

        void customCountersSelectionList_SelectionChanged(object sender, EventArgs e)
        {
            testCustomCountersButton.Enabled = customCountersSelectionList.Selected.Items.Count > 0;
        }

        public MonitoredSqlServerInstancePropertiesDialog(MonitoredSqlServerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.configuration = configuration;
            cloudProviderId = configuration.CloudProviderId;
            InitializeComponent();
            InitializeForm();

            this.AdaptFontSize();
        }

        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private int GetActivePropertyPageIndex(int? cloudProviderId, MonitoredSqlServerInstancePropertiesDialogPropertyPages propertyPage)
        {
            Type pageType;

            if (cloudProviderId.HasValue == false)
            {
                return (int)propertyPage;
            }

            if (cloudProviderId == Common.Constants.MicrosoftAzureId || cloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                pageType = typeof(MonitoredAzureInstancePropertiesDialogPropertyPages);
            }
            else if (cloudProviderId == Common.Constants.AmazonRDSId)
            {
                pageType = typeof(MonitoredAWSInstancePropertiesDialogPropertyPages);
            }
            else
            {
                pageType = typeof(MonitoredSqlServerInstancePropertiesDialogPropertyPages);
            }

            return (int)Enum.Parse(pageType, propertyPage.ToString());
        }

        public MonitoredSqlServerConfiguration Configuration
        {
            get { return configuration; }
        }

        public MonitoredSqlServerInstancePropertiesDialogPropertyPages SelectedPropertyPage
        {
            get
            {
                return (MonitoredSqlServerInstancePropertiesDialogPropertyPages)propertiesControl.SelectedPropertyPageIndex;
            }
            set
            {
                propertiesControl.SelectedPropertyPageIndex = GetActivePropertyPageIndex(Configuration.CloudProviderId, value);
            }
        }

        public MonitoredSqlServer SavedServer
        {
            get { return savedServer; }
        }

        private void InitializeForm()
        {
            //SQLDM 10.3 Fix for ticket SQLDM-28718 & SQLDM-28784
            if (id > 0)
            {
                popularPropertiesContentPage.Enabled =
                    queryMonitorPropertyPage.Enabled =
                    activeMonitorPropertyPage.Enabled =
                    replicationPropertyPage.Enabled =
                    tableStatisticsPropertyPage.Enabled =
                    osPropertyPage.Enabled =
                    customCountersPropertyPage.Enabled =
                    clusterPropertyPage.Enabled =
                    waitPropertyPage.Enabled =
                    baselineConfiguration1.Enabled =
                    analysisConfigurationPropertyPage.Enabled =
                    maintenancePropertyPage.Enabled =
                        ApplicationModel.Default.UserToken.GetServerPermission(id) >= PermissionType.Modify;

                //SQLDM 10.3 Fix for ticket SQLDM-28718 & SQLDM-28784
                diskPropertyPage.Enabled = configuration.CloudProviderId.HasValue && configuration.CloudProviderId == SQLdm.Common.Constants.LinuxId ? false : true && ApplicationModel.Default.UserToken.GetServerPermission(id) >= PermissionType.Modify;

                GeneralPageSecurityPermissions(ApplicationModel.Default.UserToken.GetServerPermission(id));
            }


            propertiesControl.SelectedPropertyPageIndex = 0;

            Text = string.Format(Text, configuration.InstanceName);

            UpdateTags();

            //Saving the current list of tags selected or a < Click here to choose tags > text
            oldTag = tagsDropDownButton.Text;

            if (id > 0)
            {
                serverDateTimeVersionBackgroundWorker.RunWorkerAsync(id);
                // SQLdm Minimum Privileges - Varun Chopra - Fetch Server Permissions
                serverPermissionsBackgroundWorker.RunWorkerAsync(id);
            }
            else
            {
                mmServerDateTime.Text = "Not yet monitored.";
            }

            //Get the blocked process threshold
            if (id > 0)
            {
                blockedProcessThresholdBackgroundWorker.RunWorkerAsync(id);
            }

            InitializeTimespanSpinnerButtons();

            collectionIntervalTimeSpanEditor.MaxValue = TimeSpan.FromSeconds(Properties.Constants.MaxScheduledRefreshIntervalSeconds);
            collectionIntervalTimeSpanEditor.MinValue = TimeSpan.FromSeconds(Properties.Constants.MinScheduledRefreshIntervalSeconds);

            collectionIntervalTimeSpanEditor.TimeSpan = configuration.ScheduledCollectionInterval;
            useWindowsAuthenticationRadioButton.Checked = configuration.ConnectionInfo.UseIntegratedSecurity;
            useSqlServerAuthenticationRadioButton.Checked = !configuration.ConnectionInfo.UseIntegratedSecurity;

            //databaseStatisticsTimeSpanEditor.TimeSpan = configuration.DatabaseStatisticsInterval;
            if (!configuration.ConnectionInfo.UseIntegratedSecurity)
            {
                loginNameTextbox.Text = configuration.ConnectionInfo.UserName;
                passwordTextbox.Text = PasswordDecoyText;
            }

            if (configuration.FriendlyServerName != null)
            {
                friendlyNameTextbox.Text = configuration.FriendlyServerName;
            }

            // Handle time-limited query monitor configuration
            if (!configuration.QueryMonitorConfiguration.Enabled
                || configuration.QueryMonitorConfiguration.StopTimeUTC == null
                || !configuration.QueryMonitorConfiguration.StopTimeUTC.HasValue)
            {
                enableQueryMonitorTraceCheckBox.Checked = configuration.QueryMonitorConfiguration.Enabled;
                queryMonitorRunningMessage.Visible = false;
                queryMonitorWarningImage.Visible = false;
            }
            else
            {
                if (configuration.QueryMonitorConfiguration.StopTimeUTC.Value > DateTime.Now.ToUniversalTime())
                {
                    queryMonitorRunningMessage.Text =
                        String.Format(queryMonitorRunningMessage.Text,
                                      configuration.QueryMonitorConfiguration.StopTimeUTC.Value.ToLocalTime().
                                          ToLongTimeString());

                    enableQueryMonitorTraceCheckBox.Checked = configuration.QueryMonitorConfiguration.Enabled;
                    queryMonitorRunningMessage.Visible = true;
                    queryMonitorWarningImage.Visible = true;
                }
                else  // It has already stopped
                {
                    enableQueryMonitorTraceCheckBox.Checked = configuration.QueryMonitorConfiguration.Enabled;
                    queryMonitorRunningMessage.Visible = false;
                    queryMonitorWarningImage.Visible = false;
                }
            }
            //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- Configuring newly added section on load
            ConfigureHowToCollectDataSection();

            //SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- Configuring newly added section on load
            ConfigureHowToCollectActivityDataSection();

            captureSqlBatchesCheckBox.Checked = configuration.QueryMonitorConfiguration.SqlBatchEventsEnabled;
            captureSqlStatementsCheckBox.Checked = configuration.QueryMonitorConfiguration.SqlStatementEventsEnabled;
            captureStoredProceduresCheckBox.Checked =
                configuration.QueryMonitorConfiguration.StoredProcedureEventsEnabled;

            chkActivityMonitorEnable.Checked = configuration.ActivityMonitorConfiguration.Enabled;

            if (serverVersion != null)
            {
                if (serverVersion.Major <= 8)
                {
                    captureDeadlockCheckBox.Enabled = false;
                    captureDeadlockCheckBox.Checked = false;
                    chkCaptureBlocking.Enabled = false;
                    chkCaptureBlocking.Checked = false;
                    if (cloudProviderId == 2) //Disabling of Autogrow event for azure
                    {
                        chkCaptureAutogrow.Enabled = false;
                        chkCaptureAutogrow.Checked = false;
                    }
                    else
                    {
                        chkCaptureAutogrow.Enabled = true;
                        chkCaptureAutogrow.Checked = true;
                    }

                }
                else
                {
                    captureDeadlockCheckBox.Checked = configuration.ActivityMonitorConfiguration.DeadlockEventsEnabled;
                    chkCaptureBlocking.Checked = configuration.ActivityMonitorConfiguration.BlockingEventsEnabled;
                    if (cloudProviderId == 2) //Disabling of Autogrow event for azure
                    {
                        chkCaptureAutogrow.Checked = false;
                    }
                    else
                    {
                        chkCaptureAutogrow.Checked = configuration.ActivityMonitorConfiguration.AutoGrowEventsEnabled;
                    }
                  
                }
            }
            else
            {
                captureDeadlockCheckBox.Checked = configuration.ActivityMonitorConfiguration.DeadlockEventsEnabled;
                chkCaptureBlocking.Checked = configuration.ActivityMonitorConfiguration.BlockingEventsEnabled;
                if (cloudProviderId == 2) //Disabling of Autogrow event for azure
                {
                    chkCaptureAutogrow.Checked = false;
                }
                else
                {
                    chkCaptureAutogrow.Checked = configuration.ActivityMonitorConfiguration.AutoGrowEventsEnabled;
                }
            }

            spnBlockedProcessThreshold.Enabled = chkCaptureBlocking.Checked && chkCaptureBlocking.Enabled;
            lblBlockedProcessSpinner.Enabled = spnBlockedProcessThreshold.Enabled;

            durationThresholdSpinner.Value = Convert.ToInt32(configuration.QueryMonitorConfiguration.DurationFilter.TotalMilliseconds);
            this.topPlanSpinner.Value = Convert.ToInt32(configuration.QueryMonitorConfiguration.TopPlanCountFilter);
            this.topPlanComboBox.Value = configuration.QueryMonitorConfiguration.TopPlanCategoryFilter;
            cpuThresholdSpinner.Value = Convert.ToInt32(configuration.QueryMonitorConfiguration.CpuUsageFilter.TotalMilliseconds);
            logicalReadsThresholdSpinner.Value = configuration.QueryMonitorConfiguration.LogicalDiskReads;
            physicalWritesThresholdSpinner.Value = configuration.QueryMonitorConfiguration.PhysicalDiskWrites;

            DateTime? quietTime = configuration.ReorgStatisticsStartTime;
            quietTimeStartEditor.DateTime = quietTime == null ? new DateTime(1900, 1, 1, 3, 0, 0) : quietTime.Value;
            setEndTime(quietTimeStartEditor.Time);

            if (configuration.LastGrowthStatisticsRunTime.HasValue)
            {
                lastTableGrowthCollectionTimeLabel.Text = configuration.LastGrowthStatisticsRunTime.Value.ToString("G");
            }
            else
            {
                lastTableGrowthCollectionTimeLabel.Text = "Never";
            }

            if (configuration.LastReorgStatisticsRunTime.HasValue)
            {
                lastTableFragmentationCollectionTimeLabel.Text = configuration.LastReorgStatisticsRunTime.Value.ToString("G");
            }
            else
            {
                lastTableFragmentationCollectionTimeLabel.Text = "Never";
            }

            short? growthStatisticsDays = configuration.GrowthStatisticsDays;
            if (growthStatisticsDays != null)
            {
                collectTableStatsSundayCheckBox.Checked =
                    MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, growthStatisticsDays);
                collectTableStatsMondayCheckBox.Checked =
                    MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, growthStatisticsDays);
                collectTableStatsTuesdayCheckBox.Checked =
                    MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, growthStatisticsDays);
                collectTableStatsWednesdayCheckBox.Checked =
                    MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, growthStatisticsDays);
                collectTableStatsThursdayCheckBox.Checked =
                    MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, growthStatisticsDays);
                collectTableStatsFridayCheckBox.Checked =
                    MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, growthStatisticsDays);
                collectTableStatsSaturdayCheckBox.Checked =
                    MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, growthStatisticsDays);
            }

            encryptDataCheckbox.Checked = configuration.ConnectionInfo.EncryptData;
            trustServerCertificateCheckbox.Checked = configuration.ConnectionInfo.TrustServerCertificate;
            trustServerCertificateCheckbox.Enabled = encryptDataCheckbox.Checked;
            collectExtendedSessionDataCheckBox.Checked = !configuration.ExtendedHistoryCollectionDisabled;

            limitInputBuffer.Checked = configuration.InputBufferLimited;

            inputBufferLimiter.Enabled = limitInputBuffer.Checked;
            inputBufferLimiter.Value = configuration.InputBufferLimiter > 0 ? configuration.InputBufferLimiter : 1;

            minimumTableSize.Value = configuration.ReorganizationMinimumTableSize.Kilobytes.Value;

            disableReplicationStuff.Checked = configuration.ReplicationMonitoringDisabled;

            //SQLDM-30197.
            if (configuration.PreferredClusterNode == null || configuration.PreferredClusterNode.Equals(""))
            {
                String currentNode = null;
                IManagementService defaultManagementService =
               ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                try
                {
                    currentNode = defaultManagementService.GetCurrentClusterNode(id);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message == "Monitored SQL Server is not clustered")
                    {
                        clusterWarningLabel.Visible = true;
                    }
                }
                if(currentNode != null)
                {
                    preferredNode.Text = defaultManagementService.GetPreferredClusterNode(id);
                }
            }
            else
            {
                preferredNode.Text = configuration.PreferredClusterNode;
            }

            serverPingTimeSpanEditor.MinValue = TimeSpan.FromSeconds(Properties.Constants.MinServerPingIntervalSeconds);
            serverPingTimeSpanEditor.MaxValue = TimeSpan.FromSeconds(Properties.Constants.MaxServerPingIntervalSeconds);
            serverPingTimeSpanEditor.TimeSpan = configuration.ServerPingInterval;

            databaseStatisticsTimeSpanEditor.MinValue = TimeSpan.FromSeconds(Properties.Constants.MinDatabaseStatisticsIntervalSeconds);
            databaseStatisticsTimeSpanEditor.MaxValue = TimeSpan.FromSeconds(Properties.Constants.MaxDatabaseStatisticsIntervalSeconds);
            databaseStatisticsTimeSpanEditor.TimeSpan = configuration.DatabaseStatisticsInterval;

            // SQLdm 10.3 - SQLDM - 28784
            if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId == SQLdm.Common.Constants.LinuxId)
            {
                if (!optionWmiNone.Checked)
                    optionWmiNone.Checked = true;
                if (optionWmiDirect.Enabled)
                    optionWmiDirect.Enabled = false;
                if (optionWmiOleAutomation.Enabled)
                    optionWmiOleAutomation.Enabled = false;
                if (optionWmiCSCreds.Enabled)
                    optionWmiCSCreds.Enabled = false;
            }
            if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != null && (configuration.CloudProviderId.HasValue && configuration.CloudProviderId == Common.Constants.AmazonRDSId || configuration.CloudProviderId.HasValue && configuration.CloudProviderId == Common.Constants.MicrosoftAzureId))
            {
                if (optionWmiDirect.Enabled)
                    optionWmiNone.Checked = true;
                if (!optionWmiDirect.Enabled)
                    optionWmiNone.Checked = true;

            }
            else
            {
                if (!optionWmiDirect.Enabled)
                    optionWmiDirect.Enabled = true;
                if (!optionWmiOleAutomation.Enabled)
                    optionWmiOleAutomation.Enabled = true;
                if (!optionWmiCSCreds.Enabled)
                    optionWmiCSCreds.Enabled = true;

                var wmi = configuration.WmiConfig;
                optionWmiNone.Checked = !wmi.DirectWmiEnabled && wmi.OleAutomationDisabled;
                optionWmiOleAutomation.Checked = !(optionWmiNone.Checked | optionWmiDirect.Checked);
                optionWmiDirect.Checked = wmi.DirectWmiEnabled;
                optionWmiCSCreds.Checked = wmi.DirectWmiConnectAsCollectionService;
                directWmiUserName.Text = wmi.DirectWmiUserName;
                directWmiPassword.Text = wmi.DirectWmiPassword;
                optionWmiChanged(null, null);
            }

            EnableAnalysisConfiguration();

            InitializeComboBox();
            InitializeCustomCounterPage();
            InitializeMaintenanceMode();
            InitializeDiskPage();
            InitializeWaitsPage();
            InitializeVMPage();
            InitializeBaselineConfigurationPage();
            InitializeAnalysisConfigurationPage();
            ShowHidePageTabByInstanceId();
            InitializeServerType();

        }
        //Bind the Server Type
        private void InitializeServerType()
        {
            Dictionary<string, int> cloudProvidersDetail = RepositoryHelper.GetBinding(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            if (cloudProvidersDetail != null)
            {
                cmbServerType.DataSource = new BindingSource(cloudProvidersDetail, null);
                cmbServerType.DisplayMember = "Key";
                cmbServerType.ValueMember = "Value";
                if (configuration.CloudProviderId == null)
                    cmbServerType.SelectedIndex = 0;
                else
                    cmbServerType.SelectedValue = cloudProvidersDetail.Where(x => x.Value == configuration.CloudProviderId.Value).SingleOrDefault().Value;
            }
        }

        /// <summary>
        /// Enables/Disables the UI components of analysis config page according to 
        /// server version.
        /// 10.0 SQLdm srishti purohit
        /// </summary>
        private void EnableAnalysisConfiguration()
        {
            try
            {
                Log.Info("Checking for server version to enable analysis configurations");
                //[START] SQLdm 10.0.2 always enable analysis configuration Property
                //int sqlVersionMajor = 0;
                //MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[id];
                //if (instance != null)
                //{
                //    if (instance.MostRecentSQLVersion == null)
                //    {
                //        if (instance.ConnectionInfo != null)
                //        {
                //            //SQLdm 10.0.2 (Barkha Khatri) get Product version using management service
                //            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
                //            var productVersion = managementService.GetProductVersion(instance.ConnectionInfo.ConnectionString, instance.Id);

                //            //var productVersion = RepositoryHelper.GetProductVersion(instance.ConnectionInfo.ConnectionString);

                //            sqlVersionMajor = productVersion != null ? productVersion.Major : 0;
                //        }
                //        else
                //            sqlVersionMajor = 0;
                //    }
                //    else
                //        sqlVersionMajor = instance.MostRecentSQLVersion.Major;
                //}
                //if (sqlVersionMajor < 9)
                //{
                //    this.analysisConfigurationPropertyPage.Enabled = false;
                //    this.analysisConfigurationPropertyPage.Visible = false;
                //    this.propertiesControl.PropertyPages.Remove(this.analysisConfigurationPropertyPage);
                //    Log.Info("Removing/Hidding analysis configurations for server, as major version is : " + sqlVersionMajor);
                //}
                //else
                //{
                Log.Info("Adding/Showing analysis configurations for server ");
                this.analysisConfigurationPropertyPage.Enabled = true;
                this.analysisConfigurationPropertyPage.Visible = true;
                if (!this.propertiesControl.PropertyPages.Contains(this.analysisConfigurationPropertyPage))
                    this.propertiesControl.PropertyPages.Add(this.analysisConfigurationPropertyPage);
                //}
            }
            catch (Exception ex)
            {
                this.analysisConfigurationPropertyPage.Enabled = true;
                Log.Error("Error in enable set of Analyze configuration in tree view. " + ex);
            }
        }

        /// <summary>
        /// Enables/Disables the UI components of General page according to 
        /// current account with security permissions (View, Modify or Administrator).
        /// </summary>
        /// <param name="getServerPermission">contains the server permission type.</param>
        private void GeneralPageSecurityPermissions(PermissionType getServerPermission)
        {
            tagsDropDownButton.Enabled = getServerPermission == PermissionType.Administrator;
            tableLayoutPanel8.Enabled =
                tableLayoutPanel6.Enabled = getServerPermission >= PermissionType.Modify;
        }

        private void InitializeTimespanSpinnerButtons()
        {
            // Do not use the default spinner for these controls as it increments in hours

            PermissionType permission = ApplicationModel.Default.UserToken.GetServerPermission(id);

            UltraTimeSpanEditorHelper helper = new UltraTimeSpanEditorHelper(Properties.Constants.MinScheduledRefreshIntervalSeconds,
                                                                             Properties.Constants.MaxScheduledRefreshIntervalSeconds,
                                                                             Properties.Constants.DefaultScheduledRefreshIntervalSeconds);

            helper.SetDefaults(collectionIntervalTimeSpanEditor);
            if (permission >= PermissionType.Modify) collectionIntervalTimeSpanEditor.Appearance.BackColor = Color.White;
            else collectionIntervalTimeSpanEditor.Enabled = false;


            helper = new UltraTimeSpanEditorHelper(Properties.Constants.MinServerPingIntervalSeconds,
                                                    Properties.Constants.MaxServerPingIntervalSeconds,
                                                    Properties.Constants.DefaultServerPingIntervalSeconds);

            helper.SetDefaults(serverPingTimeSpanEditor);

            if (permission >= PermissionType.Modify) serverPingTimeSpanEditor.Appearance.BackColor = Color.White;
            else serverPingTimeSpanEditor.Enabled = false;

            helper = new UltraTimeSpanEditorHelper(Properties.Constants.MinDatabaseStatisticsIntervalSeconds,
                                                    Properties.Constants.MaxDatabaseStatisticsIntervalSeconds,
                                                    Properties.Constants.DefaultDatabaseStatisticsIntervalSeconds);

            helper.SetDefaults(databaseStatisticsTimeSpanEditor);

            if (permission >= PermissionType.Modify) databaseStatisticsTimeSpanEditor.Appearance.BackColor = Color.White;
            else databaseStatisticsTimeSpanEditor.Enabled = false;
        }

        private void InitializeDiskPage()
        {
            adhocDisksTextBox.Text = AdhocDiskInstructionText;
            DiskCollectionSettings diskSettings = configuration.DiskCollectionSettings;
            autoDiscoverDisksCheckBox.Checked = diskSettings.AutoDiscover;
            foreach (string drive in diskSettings.Drives)
            {
                AddToListBox(selectedDisksListBox, drive);
            }
            UpdateDiskControls(true);
        }

        private void InitializeCustomCounterPage()
        {
            // enable multi-select on the selected counter listbox
            customCountersSelectionList.Selected.SelectionMode = SelectionMode.MultiExtended;

            metrics = new MetricDefinitions(true, false, true, false, configuration.CloudProviderId);
            metrics.Load(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString, true);

            List<int> currentCounters = configuration.CustomCounters;

            if (currentCounters == null)
            {
                // load current counter list from the repository
                Dictionary<int, List<int>> counterMap = RepositoryHelper
                    .GetMonitoredServerCustomCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, id, false);
                counterMap.TryGetValue(id, out currentCounters);
                // update the configuration with the list of counters
                configuration.CustomCounters = currentCounters;
            }

            UpdateCustomCounters();
        }

        private void InitializeWaitsPage()
        {

            if (String.IsNullOrEmpty(waitStatisticsStartTime.MaskInput))
            {
                waitStatisticsStartTime.MaskInput = "{time}";
            }
            if (configuration.ActiveWaitsConfiguration != null)
            {
                // Only make available to SQL 2005+
                if (serverVersion != null)
                {
                    if (serverVersion.Major <= 8)
                    {
                        chkCollectActiveWaits.Enabled = false;
                        chkCollectActiveWaits.Checked = false;
                        chkUseXE.Checked = false;
                        chkUseXE.Enabled = false;
                        // SQLdm 10.4(Varun Chopra) query waits using Query Store
                        chkUseQs.Checked = false;
                        chkUseQs.Enabled = false;
                    }
                    else
                    {
                        chkCollectActiveWaits.Checked = configuration.ActiveWaitsConfiguration.Enabled;
                        // SQLdm 10.4(Varun Chopra) query waits using Query Store
                        this.chkUseQs.Checked = (this.serverVersion.Major >= 14 || configuration.CloudProviderId ==
                                                 Common.Constants.MicrosoftAzureManagedInstanceId ||
                                                 configuration.CloudProviderId ==
                                                 Common.Constants.MicrosoftAzureId) &&
                                                this.configuration.ActiveWaitsConfiguration.EnabledQs;

                        chkUseXE.Checked = !this.chkUseQs.Checked && serverVersion.Major >= 11 && configuration.ActiveWaitsConfiguration.EnabledXe;
                        if (ApplicationModel.Default.AllInstances[id].CloudProviderId == Common.Constants.AmazonRDSId)
                        {
                            chkUseXE.Checked = false;
                            chkUseXE.Enabled = false;
                        }
                    }
                }
                else
                {
                    chkCollectActiveWaits.Checked = configuration.ActiveWaitsConfiguration.Enabled;
                    // SQLdm 10.4(Varun Chopra) query waits using Query Store
                    this.chkUseQs.Checked = this.configuration.ActiveWaitsConfiguration.EnabledQs;
                    if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
                    {
                        chkUseXE.Checked = !this.chkUseQs.Checked && configuration.ActiveWaitsConfiguration.EnabledXe;
                    }
                    else
                    {
                        chkUseXE.Checked = false;
                        chkUseXE.Enabled = false;
                    }
                }

                if (serverVersion != null)
                {
                    // SQLdm 10.4(Varun Chopra) query waits using Query Store
                    this.chkUseQs.Enabled = this.serverVersion.Major >= 14 || configuration.CloudProviderId ==
                                            Common.Constants.MicrosoftAzureManagedInstanceId ||
                                            configuration.CloudProviderId ==
                                            Common.Constants.MicrosoftAzureId;
                    chkUseXE.Enabled = serverVersion.Major >= 11;
                    if (configuration.CloudProviderId == Common.Constants.AmazonRDSId)
                    {
                        chkUseXE.Checked = false;
                        chkUseXE.Enabled = false;
                    }
                }

                if (configuration.ActiveWaitsConfiguration.Enabled)
                {
                    chkCollectActiveWaits.Checked = true;
                    ScheduledQueryWaits.Enabled = true;
                    PerpetualQueryWaits.Enabled = true;
                }
                else
                {
                    chkCollectActiveWaits.Checked = false;
                    ScheduledQueryWaits.Enabled = false;
                    PerpetualQueryWaits.Enabled = false;
                }

                // Run continuously
                if (!configuration.ActiveWaitsConfiguration.RunTime.HasValue)
                {

                    ScheduledQueryWaits.Checked = false;
                    PerpetualQueryWaits.Checked = true;
                }
                else
                {
                    ScheduledQueryWaits.Checked = true;
                    PerpetualQueryWaits.Checked = false;
                }

                if (configuration.ActiveWaitsConfiguration.RunTime.HasValue
                    && configuration.ActiveWaitsConfiguration.StartTimeRelative > DateTime.MinValue)
                {
                    if (serverLocalDateTime > DateTime.MinValue)
                    {
                        //Wait collection enabled
                        if (configuration.ActiveWaitsConfiguration.Enabled)
                        {
                            if (//if we are in the collection interval
                                configuration.ActiveWaitsConfiguration.StartTimeRelative.Add(
                                    configuration.ActiveWaitsConfiguration.RunTime.Value) > serverLocalDateTime)
                            {
                                waitStatisticsStartDate.DateTime =
                                    configuration.ActiveWaitsConfiguration.StartTimeRelative;
                                waitStatisticsStartTime.DateTime =
                                    configuration.ActiveWaitsConfiguration.StartTimeRelative;
                                waitStatisticsRunTime.Time = configuration.ActiveWaitsConfiguration.RunTime.Value;
                            }
                            else
                            {
                                chkCollectActiveWaits.Checked = false;//disable collection of activewaits
                                waitStatisticsStartDate.DateTime = serverLocalDateTime.Date;
                                waitStatisticsStartTime.DateTime = serverLocalDateTime;
                                waitStatisticsRunTime.Time = TimeSpan.FromMinutes(30);
                            }
                        }
                    }
                    else
                    {
                        if (
                            configuration.ActiveWaitsConfiguration.StartTimeRelative.Add(
                                configuration.ActiveWaitsConfiguration.RunTime.Value) > DateTime.MinValue)
                        {
                            waitStatisticsStartDate.DateTime =
                                configuration.ActiveWaitsConfiguration.StartTimeRelative;
                            waitStatisticsStartTime.DateTime =
                                configuration.ActiveWaitsConfiguration.StartTimeRelative;
                            waitStatisticsRunTime.Time = configuration.ActiveWaitsConfiguration.RunTime.Value;
                        }
                        else
                        {
                            chkCollectActiveWaits.Checked = false;
                            waitStatisticsStartDate.DateTime = DateTime.Now.Date;
                            waitStatisticsStartTime.DateTime = DateTime.Now;
                            waitStatisticsRunTime.Time = TimeSpan.FromMinutes(30);
                        }
                    }
                }
                else
                {
                    if (serverLocalDateTime > DateTime.MinValue)
                    {
                        waitStatisticsStartDate.DateTime = serverLocalDateTime.Date;
                        waitStatisticsStartTime.DateTime = serverLocalDateTime;
                        waitStatisticsRunTime.Time = TimeSpan.FromMinutes(30);
                    }
                    else
                    {
                        waitStatisticsStartDate.DateTime = DateTime.Now.Date;
                        waitStatisticsStartTime.DateTime = DateTime.Now;
                        waitStatisticsRunTime.Time = TimeSpan.FromMinutes(30);
                    }
                }
            }
            // No data available, use defaults
            else
            {
                chkCollectActiveWaits.Checked = false;
                waitStatisticsStartDate.Enabled = false;
                waitStatisticsStartTime.Enabled = false;
                waitStatisticsRunTime.Enabled = false;
                PerpetualQueryWaits.Enabled = false;
                ScheduledQueryWaits.Enabled = false;
                ScheduledQueryWaits.Checked = true;
                // SQLdm 10.4(Varun Chopra) query waits using Query Store
                this.chkUseQs.Checked = false;
                chkUseXE.Checked = false;
                if (serverLocalDateTime > DateTime.MinValue)
                {
                    waitStatisticsStartDate.DateTime = serverLocalDateTime.Date;
                    waitStatisticsStartTime.DateTime = serverLocalDateTime;
                    waitStatisticsRunTime.Time = TimeSpan.FromMinutes(30);
                }
                else
                {
                    waitStatisticsStartDate.DateTime = DateTime.Now.Date;
                    waitStatisticsStartTime.DateTime = DateTime.Now;
                    waitStatisticsRunTime.Time = TimeSpan.FromMinutes(30);
                }
            }

            ShowHideWaitSchedulePanel();

            if (!waitCollectorStatusBackgroundWorker.IsBusy && id > 0)
                waitCollectorStatusBackgroundWorker.RunWorkerAsync(id);

            if (id < 1)
            {
                waitStatisticsServerDateTime.Text = "Not yet monitored.";
                waitStatisticsStatus.Text = "Not yet monitored.";
                refreshWaitCollectorStatus.Enabled = false;
            }
        }

        private void InitializeVMPage()
        {
            btnVMConfig.Visible =
                (ApplicationModel.Default.UserToken.GetServerPermission(this.id) >= PermissionType.Modify);

            vCenterNameLabel.Text = configuration.VirtualizationConfiguration != null
                                    ? configuration.VirtualizationConfiguration.VCName
                                    : "Not Connected";

            vmNameLabel.Text = configuration.VirtualizationConfiguration != null
                                ? configuration.VirtualizationConfiguration.VMName
                                : string.Empty;
        }

        private void InitializeBaselineConfigurationPage()
        {
            // do it.  Now!
            baselineConfiguration1.BaselineConfig = ObjectHelper.Clone(configuration.BaselineConfiguration);
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            baselineConfiguration1.BaselineConfigList = ObjectHelper.Clone(configuration.BaselineConfigurationList);
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            baselineConfiguration1.InstanceId = id;
        }

        //Update analysi page accroding to fetch configration from DB
        private void InitializeAnalysisConfigurationPage()
        {
            try
            {
                analysisConfigurationPage.InstanceId = id;
                analysisConfigurationPage.updateAnalysisControlsUsingAvailalbleData(configuration.AnalysisConfiguration);
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Not able to update analysis configuration page.", ex);
            }
        }

        private void UpdateCustomCounters()
        {
            int[] metricKeys = metrics.GetMetricDescriptionKeys();

            if (metricKeys.Length == 0)
            {
                customCounterStackLayoutPanel.ActiveControl = customCounterMessageLabel;
            }
            else
            {
                IList<int> selectedTags = GetSelectedTags();
                customCounterTagsLookupTable =
                    RepositoryHelper.GetCustomCountersWithTagIds(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo, selectedTags);

                ListBox.ObjectCollection available = customCountersSelectionList.Available.Items;
                ListBox.ObjectCollection selected = customCountersSelectionList.Selected.Items;

                if (customCountersInitialized)
                {
                    List<ExtendedListItem> itemsToRemove = new List<ExtendedListItem>();

                    foreach (ExtendedListItem listItem in selected)
                    {
                        if (listItem.Tag is Pair<int, int>)
                        {
                            itemsToRemove.Add(listItem);
                        }
                    }

                    foreach (ExtendedListItem itemToRemove in itemsToRemove)
                    {
                        selected.Remove(itemToRemove);
                    }
                }

                // Add custom counters linked through tags
                foreach (KeyValuePair<int, ICollection<int>> customCounter in customCounterTagsLookupTable)
                {
                    MetricDescription? definition = metrics.GetMetricDescription(customCounter.Key);

                    if (definition.HasValue)
                    {
                        foreach (int tagId in customCounter.Value)
                        {
                            if (selectedTags.Contains(tagId) && ApplicationModel.Default.Tags.Contains(tagId))
                            {
                                StringBuilder itemName = new StringBuilder(definition.Value.Name);
                                itemName.Append(" - [");
                                itemName.Append(ApplicationModel.Default.Tags[tagId].Name);
                                itemName.Append("]");

                                ExtendedListItem listItem = new ExtendedListItem(itemName.ToString());
                                listItem.Tag = new Pair<int, int>(customCounter.Key, tagId);
                                listItem.TextColor = SystemColors.GrayText;
                                listItem.CanRemove = false;
                                selected.Add(listItem);
                            }
                        }
                    }
                }

                // Add custom counters explicitely linked to this server
                if (!customCountersInitialized)
                {
                    foreach (int metricID in metricKeys)
                    {
                        MetricDescription? definition = metrics.GetMetricDescription(metricID);

                        if (definition.HasValue)
                        {
                            if (configuration.CustomCounters != null && configuration.CustomCounters.Contains(metricID))
                            {
                                selected.Add(new ExtendedListItem(definition.Value.Name, metricID));
                            }
                            else
                            {
                                available.Add(new ExtendedListItem(definition.Value.Name, metricID));
                            }
                        }
                    }
                }

                testCustomCountersButton.Enabled = customCountersSelectionList.Selected.Items.Count > 0;
                customCounterStackLayoutPanel.ActiveControl = customCounterContentPanel;
            }

            customCountersInitialized = true;
            tagsChanged = false;
        }

        private void InitializeMaintenanceMode()
        {
            switch (configuration.MaintenanceMode.MaintenanceModeType)
            {
                case MaintenanceModeType.Never:
                    {
                        mmNeverRadio.Checked = true;
                        break;
                    }
                case MaintenanceModeType.Always:
                    {
                        mmAlwaysRadio.Checked = true;
                        break;
                    }
                case MaintenanceModeType.Once:
                    {
                        mmOnceRadio.Checked = true;
                        break;
                    }
                case MaintenanceModeType.Recurring:
                    {
                        mmRecurringRadio.Checked = true;
                        break;
                    }
                case MaintenanceModeType.Monthly:
                    {
                        mmMonthlyRecurringRadio.Checked = true;
                        mmMonthlyDayRadio.Checked = true;
                        break;
                    }
            }
            short? mmDays = configuration.MaintenanceMode.MaintenanceModeDays;
            mmBeginSunCheckbox.Checked =
             MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, mmDays);
            mmBeginMonCheckbox.Checked =
             MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, mmDays);
            mmBeginTueCheckbox.Checked =
             MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, mmDays);
            mmBeginWedCheckbox.Checked =
             MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, mmDays);
            mmBeginThurCheckbox.Checked =
             MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, mmDays);
            mmBeginFriCheckbox.Checked =
             MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, mmDays);
            mmBeginSatCheckbox.Checked =
             MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, mmDays);

            if (configuration.MaintenanceMode.MaintenanceModeRecurringStart.HasValue)
            {
                mmRecurringBegin.DateTime = (DateTime)configuration.MaintenanceMode.MaintenanceModeRecurringStart;
            }
            if (configuration.MaintenanceMode.MaintenanceModeDuration.HasValue)
            {
                mmRecurringDuration.Time = (TimeSpan)configuration.MaintenanceMode.MaintenanceModeDuration;
            }
            if (configuration.MaintenanceMode.MaintenanceModeMonthRecurringStart.HasValue)
            {
                mmMonthRecurringBegin.DateTime = (DateTime)configuration.MaintenanceMode.MaintenanceModeMonthRecurringStart;
            }
            if (configuration.MaintenanceMode.MaintenanceModeMonthDuration.HasValue)
            {
                mmMonthRecurringDuration.Time = (TimeSpan)configuration.MaintenanceMode.MaintenanceModeMonthDuration;
            }
            if (configuration.MaintenanceMode.MaintenanceModeStart.HasValue)
            {
                mmOnceBeginDate.DateTime = (DateTime)configuration.MaintenanceMode.MaintenanceModeStart;
                mmOnceBeginTime.DateTime = (DateTime)configuration.MaintenanceMode.MaintenanceModeStart;
            }
            else
            {
                if (serverLocalDateTime > DateTime.MinValue)
                {
                    mmOnceBeginDate.DateTime = serverLocalDateTime.Date;
                    mmOnceBeginTime.DateTime = serverLocalDateTime;
                }
                else
                {
                    mmOnceBeginDate.DateTime = DateTime.Now.Date;
                    mmOnceBeginTime.DateTime = DateTime.Now;
                }
            }

            if (configuration.MaintenanceMode.MaintenanceModeStop.HasValue)
            {
                mmOnceStopDate.DateTime = (DateTime)configuration.MaintenanceMode.MaintenanceModeStop;
                mmOnceStopTime.DateTime = (DateTime)configuration.MaintenanceMode.MaintenanceModeStop;
            }
            else
            {
                DateTime stopTime;

                if (serverLocalDateTime > DateTime.MinValue)
                {
                    stopTime = serverLocalDateTime.Add(TimeSpan.FromHours(1));
                }
                else
                {
                    stopTime = DateTime.Now.Add(TimeSpan.FromHours(1));
                }

                mmOnceStopDate.DateTime = stopTime.Date;
                mmOnceStopTime.DateTime = stopTime;

            }

            // added for monthly part
            if (mmMonthlyRecurringRadio.Checked)
            {
                short mmMonth = (short)configuration.MaintenanceMode.MaintenanceModeMonth;
                short maintenanceModeSpecificDay = (short)configuration.MaintenanceMode.MaintenanceModeSpecificDay;
                short mmWeekOrdinal = (short)configuration.MaintenanceMode.MaintenanceModeWeekOrdinal;
                short mmWeekDay = (short)configuration.MaintenanceMode.MaintenanceModeWeekDay;

                if (maintenanceModeSpecificDay > 0)
                {
                    try
                    {
                        mmMonthlyDayRadio.Checked = true;
                        inputDayLimiter.Value = maintenanceModeSpecificDay;
                        inputOfEveryMonthLimiter.Value = mmMonth;
                    }
                    catch (Exception ex)
                    {
                    }

                }
                else if (mmWeekOrdinal > 0 && mmWeekDay > 0)
                {
                    try
                    {
                        mmMonthlyTheRadio.Checked = true;
                        WeekcomboBox.SelectedValue = mmWeekOrdinal;
                        DaycomboBox.SelectedValue = mmWeekDay;
                        inputOfEveryTheMonthLimiter.Value = mmMonth;
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
        }

        private void useWindowsAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            loginNameTextbox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
            passwordTextbox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
        }

        private void useSqlServerAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            loginNameTextbox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
            passwordTextbox.Enabled = useSqlServerAuthenticationRadioButton.Checked;
        }

        private void enableQueryMonitorTraceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = enableQueryMonitorTraceCheckBox.Checked;

            fmePoorlyPerformingThresholds.Enabled = enabled;

            captureSqlBatchesCheckBox.Enabled = enabled;
            captureSqlStatementsCheckBox.Enabled = enabled;
            captureStoredProceduresCheckBox.Enabled = enabled;
            durationThresholdLabel.Enabled = enabled;
            durationThresholdSpinner.Enabled = enabled;
            cpuThresholdLabel.Enabled = enabled;
            cpuThresholdSpinner.Enabled = enabled;
            logicalReadsThresholdLabel.Enabled = enabled;
            logicalReadsThresholdSpinner.Enabled = enabled;
            physicalWritesThresholdLabel.Enabled = enabled;
            physicalWritesThresholdSpinner.Enabled = enabled;

            queryMonitorAdvancedOptionsButton.Enabled = enabled;

            chkCollectEstimatedQueryPlans.Enabled = enabled;
            chkCollectQueryPlans.Enabled = enabled;

            //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- Configuring newly added section on enable disable checkbox
            ConfigureHowToCollectDataSection();

            // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store
            // To Enable the Top Plan Filter only of the Extended Events Radio Button is checked and enabled
            enabled = enabled && ((this.rButtonUseQueryStore.Checked && this.rButtonUseQueryStore.Enabled) || (rButtonUseExtendedEvents.Checked && rButtonUseExtendedEvents.Enabled));

            // Top Plan
            this.topPlanLabel.Enabled = enabled;
            this.topPlanSpinner.Enabled = enabled;
            this.topPlanSuffixLabel.Enabled = enabled;
            this.topPlanComboBox.Enabled = enabled;
            this.topPlanTableLayoutPanel.Enabled = enabled;

            UpdateQueryMonitoringLabel();

            //chkCaptureAutogrow.Enabled = enabled;

            //if (serverVersion != null)
            //{
            //    if (serverVersion.Major <= 8)
            //    {
            //        captureDeadlockCheckBox.Enabled = false;
            //        captureDeadlockCheckBox.Checked = false;
            //        chkCaptureBlocking.Enabled = false;
            //        chkCaptureBlocking.Checked = false;
            //    }
            //    else
            //    {
            //        captureDeadlockCheckBox.Enabled = enabled;
            //        chkCaptureBlocking.Enabled = enabled;
            //    }
            //}
            //else
            //{
            //    captureDeadlockCheckBox.Enabled = enabled;
            //    chkCaptureBlocking.Enabled = enabled;
            //}
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (id == 0 || ApplicationModel.Default.UserToken.GetServerPermission(id) >= PermissionType.Modify)
            {
                if (useSqlServerAuthenticationRadioButton.Checked && loginNameTextbox.Text.Trim().Length == 0)
                {
                    ApplicationMessageBox.ShowInfo(this,
                                                   "Please specify the SQL Server credentials to use when connecting to this instance.");
                    DialogResult = DialogResult.None;
                    propertiesControl.SelectedPropertyPageIndex =
                        (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.Popular;
                    return;
                }

                if (baselineConfiguration1.CheckForErrors())
                {
                    DialogResult = DialogResult.None;
                    ApplicationMessageBox.ShowInfo(this, baselineConfiguration1.ErrorMessage);
                    propertiesControl.SelectedPropertyPageIndex = (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.Baseline;
                    return;
                }
                if (analysisConfigurationPage.CheckForErrors())
                {
                    DialogResult = DialogResult.None;
                    ApplicationMessageBox.ShowInfo(this, analysisConfigurationPage.ErrorMessage);
                    propertiesControl.SelectedPropertyPageIndex = (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.AnalysisConfiguration;
                    return;
                }

                if (enableQueryMonitorTraceCheckBox.Checked)
                {
                    if (ValidateQueryMonitorSettings() == false)
                    {
                        DialogResult = DialogResult.None;
                        propertiesControl.SelectedPropertyPageIndex =
                            (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.QueryMonitor;
                        return;
                    }
                }

                if (ValidateMaintenanceModeSettings() == false)
                {
                    DialogResult = DialogResult.None;
                    propertiesControl.SelectedPropertyPageIndex =
                        (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.MaintenanceMode;
                    return;
                }

                if (ValidateWmiSettings() == false)
                {
                    DialogResult = DialogResult.None;
                    propertiesControl.SelectedPropertyPageIndex = (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.OleAutomation;
                    return;
                }

                if (ValidateWaitStatisticsSettings() == false)
                {
                    DialogResult = DialogResult.None;
                    propertiesControl.SelectedPropertyPageIndex =
                        (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.WaitStatistics;
                    return;
                }

                //configuration = BuildConfiguration();
                MonitoredSqlServerConfiguration newConfiguration = BuildConfiguration();

                if (MModeTimeEqualsQuietTime(newConfiguration))
                {
                    if (ApplicationMessageBox.ShowWarning(this,
                            "Maintenance Mode and Quiet time data collection are scheduled at the same time.  Quiet time data collection will not occur when the server is in maintenance mode. Do you wish to continue?",
                            ExceptionMessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        DialogResult = DialogResult.None;

                        if ((propertiesControl.SelectedPropertyPageIndex != (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.MaintenanceMode) &&
                            (propertiesControl.SelectedPropertyPageIndex != (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.TableStatistics))
                        {
                            propertiesControl.SelectedPropertyPageIndex = (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.MaintenanceMode;
                        }
                        return;
                    }
                }
                if (id >= 1)
                {
                    SaveConfiguration(newConfiguration);
                }
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- method for Configuring newly added section on load
        private void ConfigureHowToCollectDataSection()
        {
            if (enableQueryMonitorTraceCheckBox.Checked)
            {
                if (configuration.CloudProviderId == Common.Constants.MicrosoftAzureId ||
                    configuration.CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
                    rButtonUseTrace.Enabled = false;
                else
                    rButtonUseTrace.Enabled = true; // this is sql trace option
                var productVersion = serverVersion ?? configuration.MostRecentSQLVersion;
                if (productVersion != null && productVersion.Major <= 9 && configuration.CloudProviderId !=
                    Common.Constants.MicrosoftAzureManagedInstanceId && configuration.CloudProviderId !=
                    Common.Constants.MicrosoftAzureId)//SQLdm 9.0 (Ankit Srivastava) -Fixed defect DE43931 and DE43929 -- change the version major check from 8 to 9
                {
                    chkCollectQueryPlans.Enabled = false;
                    chkCollectQueryPlans.Checked = false;

                    chkCollectEstimatedQueryPlans.Enabled = false;//SQLdm 10.0 (Tarun Sapra)- Display Estimated Query Plan
                    chkCollectEstimatedQueryPlans.Checked = false;//SQLdm 10.0 (Tarun Sapra)- Display Estimated Query Plan

                    // SQLdm 10.4 (Varun Chopra) - Disable Query Store and related UI
                    if (this.rButtonUseQueryStore.Enabled)
                    {
                        this.rButtonUseQueryStore.Enabled = false;
                    }
                    if (this.rButtonUseQueryStore.Checked)
                    {
                        this.rButtonUseQueryStore.Checked = false;
                    }
                    // Batch collection not allowed with query store
                    if (!this.captureSqlBatchesCheckBox.Enabled)
                    {
                        this.captureSqlBatchesCheckBox.Enabled = true;
                    }

                    rButtonUseExtendedEvents.Enabled = false;
                    rButtonUseTrace.Checked = true;
                }
                else if (productVersion != null && productVersion.Major <= 12 && configuration.CloudProviderId !=
                         Common.Constants.MicrosoftAzureManagedInstanceId && configuration.CloudProviderId !=
                         Common.Constants.MicrosoftAzureId)
                {
                    if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
                    {
                        rButtonUseExtendedEvents.Enabled = true;
                    }
                    else
                    {
                        rButtonUseExtendedEvents.Enabled = false;
                    }
                    rButtonUseTrace.Checked = configuration.QueryMonitorConfiguration.TraceMonitoringEnabled;
                    // SQLdm 10.4 (Varun Chopra) - Disable Query Store
                    if (this.rButtonUseQueryStore.Enabled)
                    {
                        this.rButtonUseQueryStore.Enabled = false;
                    }
                    if (this.rButtonUseQueryStore.Checked)
                    {
                        if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
                        {
                            rButtonUseExtendedEvents.Enabled = true;
                        }
                        else
                        {
                            rButtonUseExtendedEvents.Enabled = false;
                        }
                    }
                    // Batch collection not allowed with query store
                    if (!this.captureSqlBatchesCheckBox.Enabled)
                    {
                        this.captureSqlBatchesCheckBox.Enabled = true;
                    }
                }
                else
                {
                    // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store
                    if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
                    {
                        this.rButtonUseExtendedEvents.Enabled = true;
                    }
                    else
                    {
                        this.rButtonUseExtendedEvents.Enabled = false;
                    }
                    this.rButtonUseTrace.Checked = this.configuration.QueryMonitorConfiguration.TraceMonitoringEnabled;

                    if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId == Common.Constants.AmazonRDSId)
                    {
                        this.rButtonUseTrace.Checked = true;
                    }
                    else
                    {
                        this.rButtonUseTrace.Checked = false;
                    }

                    // SQLdm 10.4 (Varun Chopra) - Disable Query Store
                    if (!this.rButtonUseQueryStore.Enabled)
                    {
                        this.rButtonUseQueryStore.Enabled = true;
                    }
                    if (this.rButtonUseQueryStore.Checked
                        != this.configuration.QueryMonitorConfiguration.QueryStoreMonitoringEnabled)
                    {
                        this.rButtonUseQueryStore.Checked = this.configuration.QueryMonitorConfiguration
                            .QueryStoreMonitoringEnabled;
                    }
                    // Batch collection not allowed with query store
                    if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
                    {
                        if (this.captureSqlBatchesCheckBox.Enabled == this.rButtonUseQueryStore.Checked)
                        {
                            this.captureSqlBatchesCheckBox.Enabled = !this.rButtonUseQueryStore.Checked;
                        }
                    }
                }
                // Either Query Store is not enabled or Query Store is not checked then allow update to extended events checkbox
                if (rButtonUseExtendedEvents.Enabled && (!this.rButtonUseQueryStore.Enabled || !this.rButtonUseQueryStore.Checked))
                    rButtonUseExtendedEvents.Checked = !rButtonUseTrace.Checked;

                if (productVersion != null && productVersion.Major >= 13 || configuration.CloudProviderId ==
                    Common.Constants.MicrosoftAzureManagedInstanceId || configuration.CloudProviderId ==
                    Common.Constants.MicrosoftAzureId)
                {

                    if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
                    {
                        rButtonUseExtendedEvents.Enabled = true;
                    }
                    else
                    {
                        rButtonUseExtendedEvents.Enabled = false;
                    }
                    rButtonUseTrace.Checked = configuration.QueryMonitorConfiguration.TraceMonitoringEnabled;
                    // SQLdm 10.4 (Varun Chopra) - Disable Query Store
                    if (this.rButtonUseTrace.Checked)
                    {
                        this.rButtonUseTrace.Checked = true;
                    }
                    if (this.rButtonUseQueryStore.Checked)
                    {
                        if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
                        {
                            rButtonUseExtendedEvents.Enabled = true;
                        }
                        else
                        {
                            rButtonUseExtendedEvents.Enabled = false;
                        }
                    }
                    // Batch collection not allowed with query store
                    //if (!this.captureSqlBatchesCheckBox.Enabled)
                    //{
                    //    this.captureSqlBatchesCheckBox.Enabled = true;
                    //}

                }
            }

            else
            {
                chkCollectQueryPlans.Enabled = false;
                chkCollectEstimatedQueryPlans.Enabled = false;//SQLdm 10.0 (Tarun Sapra) : Display Estimated Query Plan

                rButtonUseExtendedEvents.Enabled = false;
                rButtonUseTrace.Enabled = false;
                // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store
                this.rButtonUseQueryStore.Enabled = false;

                //UI bug fix to update controls state even if query monitor disable
                //SQLdm10.1 (srishti purohit)
                // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store
                if (this.rButtonUseQueryStore.Checked != this.configuration.QueryMonitorConfiguration.QueryStoreMonitoringEnabled)
                {
                    this.rButtonUseQueryStore.Checked = this.configuration.QueryMonitorConfiguration.QueryStoreMonitoringEnabled;
                }
                if (!this.rButtonUseQueryStore.Checked)
                {
                    rButtonUseTrace.Checked = configuration.QueryMonitorConfiguration.TraceMonitoringEnabled;
                    rButtonUseExtendedEvents.Checked = !rButtonUseTrace.Checked;
                }
            }
            //SQLdm 9.1 (Ankit Srivastava) -- Fixed Open functional isssue - disabling query plans option in case of trace monitoring 
            // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store
            chkCollectQueryPlans.Enabled = enableQueryMonitorTraceCheckBox.Checked && (rButtonUseExtendedEvents.Checked || this.rButtonUseQueryStore.Checked);
            chkCollectQueryPlans.Checked = configuration.QueryMonitorConfiguration.CollectQueryPlan;

            chkCollectEstimatedQueryPlans.Enabled = enableQueryMonitorTraceCheckBox.Checked && (rButtonUseExtendedEvents.Checked || this.rButtonUseQueryStore.Checked);
            chkCollectEstimatedQueryPlans.Checked = configuration.QueryMonitorConfiguration.CollectEstimatedQueryPlan;//SQLdm 10.0 (Tarun Sapra)- Estimated Query Plan

            if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId == Common.Constants.AmazonRDSId)
            {
                chkCollectQueryPlans.Enabled = false;
                chkCollectEstimatedQueryPlans.Enabled = false;
            }
        }

        //SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- method for Configuring newly added section on load
        private void ConfigureHowToCollectActivityDataSection()
        {
            var productVersion = serverVersion ?? configuration.MostRecentSQLVersion;
            if (chkActivityMonitorEnable.Checked)
            {
                if (configuration.CloudProviderId == Common.Constants.MicrosoftAzureId ||
                    configuration.CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
                    rButtonAMUseTrace.Enabled = false;
                else
                    rButtonAMUseTrace.Enabled = true;
                if (productVersion != null && productVersion.Major <= 10) //SQLdm 9.1 (Ankit Srivastava) -- Resolved Rally Defect DE44439
                    rButtonAMUseExtendedEvents.Enabled = false;
                else
                {
                    if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
                    {
                        rButtonAMUseExtendedEvents.Enabled = true;
                    }
                    else
                    {
                        rButtonAMUseExtendedEvents.Enabled = false; ;
                    }
                }
            }
            else
            {
                rButtonAMUseExtendedEvents.Enabled = false;
                rButtonAMUseTrace.Enabled = false;
            }

            //START -SQLdm 9.1 (Ankit Srivastava) Fixed Rally Defect DE44669 - taking the values directly from the Repository - no default value
            rButtonAMUseTrace.Checked = configuration.ActivityMonitorConfiguration.TraceMonitoringEnabled;
            rButtonAMUseExtendedEvents.Checked = !rButtonAMUseTrace.Checked;
            //END -SQLdm 9.1 (Ankit Srivastava) Fixed Rally Defect DE44669 - taking the values directly from the Repository - no default value

            if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId == Common.Constants.AmazonRDSId)
            {
                rButtonAMUseExtendedEvents.Checked = false;
                rButtonAMUseTrace.Checked = true;
            }
        }

        private bool ValidateWmiSettings()
        {
            if (optionWmiDirect.Checked)
            {
                if (!optionWmiCSCreds.Checked)
                {
                    if (String.IsNullOrEmpty(directWmiUserName.Text) || String.IsNullOrEmpty(directWmiPassword.Text))
                    {
                        ApplicationMessageBox.ShowError(this, "A user and password are required when electing to not use the SQLdm Collection Service account when establishing a direct WMI connection.");
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ValidateQueryMonitorSettings()
        {
            // SQLdm 10.4 (Varun Chopra) - Query Store supports statement and stored procedures only
            // Note: Support SQL Statements and Procedures for Query Store
            if ((!captureSqlBatchesCheckBox.Checked &&
                !captureSqlStatementsCheckBox.Checked &&
                !captureStoredProceduresCheckBox.Checked)
                ||
                (this.rButtonUseQueryStore.Checked &&
                !captureSqlStatementsCheckBox.Checked &&
                !captureStoredProceduresCheckBox.Checked))
            {
                ApplicationMessageBox.ShowInfo(this,
                                               "Please specify at least one event type for the Query Monitor Trace to capture.");
                return false;
            }
            else if (durationThresholdSpinner.Value < 500)
            {
                if (ApplicationMessageBox.ShowWarning(this,
                                                      "A low duration threshold has been specified for the Query Monitor Trace.  A duration threshold greater than 500 ms is recommended to reduce the performance impact of the trace on the monitored SQL Server.  This is especially important on SQL Server 2000 instances, where CPU performance is significantly impacted.  Do you wish to continue with a low duration threshold?",
                                                      ExceptionMessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return false;
                }
            } // This code will never be reached.
            else if (durationThresholdSpinner.Value == 0 && cpuThresholdSpinner.Value == 0 &&
                     logicalReadsThresholdSpinner.Value == 0 && physicalWritesThresholdSpinner.Value == 0)
            {
                if (ApplicationMessageBox.ShowWarning(this,
                                                      "No threshold filters have been specified for the Query Monitor Trace. Thresholds are recommended to reduce the performance impact of the trace on the monitored SQL Server. Do you wish to continue without specifying thresholds?",
                                                      ExceptionMessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateMaintenanceModeSettings()
        {
            if (mmOnceRadio.Checked)
            {
                DateTime start = mmOnceBeginDate.DateTime.Date + mmOnceBeginTime.Time;
                DateTime stop = mmOnceStopDate.DateTime.Date + mmOnceStopTime.Time;

                if (start >= stop)
                {
                    ApplicationMessageBox.ShowInfo(this, "The maintanence mode start time and date must be less than the maintenance mode end time and date.");
                    return false;
                }

                if (collectionServiceLocalDateTime > DateTime.MinValue)
                {
                    if (stop < collectionServiceLocalDateTime)
                    {
                        ApplicationMessageBox.ShowInfo(this,
                                                       "The Maintenance Mode end time must be greater than the collection service current date and time.");
                        return false;
                    }
                }
                else
                {
                    if (stop < DateTime.Now)
                    {
                        ApplicationMessageBox.ShowInfo(this,
                                                       "The Maintenance Mode end time must be greater than the current date and time.");
                        return false;
                    }
                }
            }

            if (mmRecurringRadio.Checked)
            {
                bool dayChecked = false;

                if ((mmBeginSunCheckbox.Checked) ||
                    (mmBeginMonCheckbox.Checked) ||
                    (mmBeginTueCheckbox.Checked) ||
                    (mmBeginWedCheckbox.Checked) ||
                    (mmBeginThurCheckbox.Checked) ||
                    (mmBeginFriCheckbox.Checked) ||
                    (mmBeginSatCheckbox.Checked))
                {
                    dayChecked = true;
                }

                if (dayChecked == false)
                {
                    ApplicationMessageBox.ShowInfo(this, "You must select at least one day for recurring maintenance mode.");
                    return false;
                }

                if ((mmRecurringDuration.Time.Hours == 0) && (mmRecurringDuration.Time.Minutes == 0))
                {
                    ApplicationMessageBox.ShowInfo(this, "The duration for recurring maintenance mode must be greater than zero minutes.");
                    return false;
                }
            }

            if (mmMonthlyRecurringRadio.Checked)
            {
                if ((mmMonthRecurringDuration.Time.Hours == 0) && (mmMonthRecurringDuration.Time.Minutes == 0))
                {
                    ApplicationMessageBox.ShowInfo(this, "The duration for recurring maintenance mode must be greater than zero minutes.");
                    return false;
                }
            }
            return true;
        }

        private bool ValidateWaitStatisticsSettings()
        {
            if (chkCollectActiveWaits.Checked && ScheduledQueryWaits.Checked)
            {
                if (serverLocalDateTime > DateTime.MinValue)
                {
                    if ((waitStatisticsStartDate.DateTime.Date + waitStatisticsStartTime.Time).Add(waitStatisticsRunTime.Time) < serverLocalDateTime)
                    {
                        ApplicationMessageBox.ShowInfo(this,
                                                        "The wait statistics collection window has already elapsed.");
                        return false;
                    }
                }

                if (waitStatisticsRunTime.Time.TotalMinutes == 0)
                {
                    ApplicationMessageBox.ShowInfo(this, "The duration for wait statistics collection must be greater than zero minutes.");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Save the configuration and send the blocked process report changes if required
        /// </summary>
        /// <param name="newConfiguration">contains the blocking threshold, maintenance mode changes etc</param>
        private void SaveConfiguration(MonitoredSqlServerConfiguration newConfiguration)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                bool blockingEnableChanged =
                    configuration.ActivityMonitorConfiguration.BlockingEventsEnabled !=
                    newConfiguration.ActivityMonitorConfiguration.BlockingEventsEnabled;

                AuditingEngine.SetContextData(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                bool blockingValueChanged = configuration.ActivityMonitorConfiguration.BlockedProcessThreshold !=
                                    spnBlockedProcessThreshold.Value;

                bool blockedWasAlreadyDisabled = blockedProcessThreshold < spnBlockedProcessThreshold.Minimum;
                bool captureIsDisabled = !chkCaptureBlocking.Checked;
                Serialized<Snapshot> blockedProcessThresholdSnapshot = null;

                if (!(blockedWasAlreadyDisabled && captureIsDisabled) && (blockingValueChanged || blockingEnableChanged))
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    blockedProcessThresholdSnapshot = defaultManagementService.SendBlockedProcessThresholdChange(
                        new BlockedProcessThresholdConfiguration(id, (int)spnBlockedProcessThreshold.Value));
                }

                // We need to know if we succeded changing the BlockedProcessThreshold value on the monitored server
                if (blockedProcessThresholdSnapshot != null && blockedProcessThresholdSnapshot.Deserialize().Error != null)
                {
                    newConfiguration.ActivityMonitorConfiguration.BlockedProcessThreshold =
                        configuration.ActivityMonitorConfiguration.BlockedProcessThreshold;

                    spnBlockedProcessThreshold.Value =
                        configuration.ActivityMonitorConfiguration.BlockedProcessThreshold;

                    ApplicationMessageBox.ShowWarning(this,
                                                      "A problem occurred while trying to set the Blocked Process Threshold value on the monitored server. Make sure to verify connectivity to this server.",
                                                      ExceptionMessageBoxButtons.OK);
                }

                //The VmConfigurationFlag indicate to management service this action should be auditing in sql server properties change.
                //true is for audit and the false is for ignore
                AuditingEngine.SetAuxiliarData("VmConfigurationFlag", new AuditAuxiliar<bool>(true));
                AuditingEngine.SetAuxiliarData("OldBlockedProcessThreshold",
                                               new AuditAuxiliar<int>(blockedProcessThreshold));
                AuditingEngine.SetAuxiliarData("NewBlockedProcessThreshold",
                                               new AuditAuxiliar<int>(chkCaptureBlocking.Checked
                                                                          ? (int)
                                                                            spnBlockedProcessThreshold
                                                                                .Value
                                                                          : 0));
                //RepositoryHelper.TestSri(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString, id, newConfiguration);
                savedServer = ApplicationModel.Default.UpdateMonitoredSqlServer(id, newConfiguration);
                configuration = newConfiguration;

                // SQLDM-28912: Commented the RefreshActiveInstances due we do not to refresh the instances when we change a property.
                //ApplicationModel.Default.RefreshActiveInstances();  

            }
            catch (Exception exception)
            {
                ApplicationMessageBox.ShowError(this, "A connection to the management service may not be available.",
                                                exception);
                DialogResult = DialogResult.None;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private static bool IsOnlyDefaultText(string[] oldtags)
        {
            return oldtags.Length == 1 && oldtags[0].Contains(ChooseTagText);
        }

        public static string GetEnumDescription(object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        private bool MModeTimeEqualsQuietTime(MonitoredSqlServerConfiguration configuration)
        {
            bool mmodeTimeQuietTimeEqual = false;
            DateTime? mModeStart = null;
            DateTime? mModeStop = configuration.MaintenanceMode.MaintenanceModeStop;
            DateTime? quietTimeStart = configuration.GrowthStatisticsStartTime;
            DateTime? quietTimeStop = quietTimeStart + TimeSpan.FromHours(2);

            switch (configuration.MaintenanceMode.MaintenanceModeType)
            {
                case MaintenanceModeType.Always:
                    {
                        mmodeTimeQuietTimeEqual = true;
                        break;
                    }
                case MaintenanceModeType.Once:
                    {
                        mModeStart = configuration.MaintenanceMode.MaintenanceModeStart;

                        if ((mModeStop - mModeStart).Value.TotalDays <= 1)
                        {
                            if (((mModeStart.Value.TimeOfDay >= quietTimeStart.Value.TimeOfDay) && (mModeStart.Value.TimeOfDay < quietTimeStop.Value.TimeOfDay)) ||
                                ((mModeStop.Value.TimeOfDay <= quietTimeStop.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay > quietTimeStart.Value.TimeOfDay)) ||
                                ((mModeStart.Value.TimeOfDay <= quietTimeStart.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay >= quietTimeStop.Value.TimeOfDay)))
                            {
                                mmodeTimeQuietTimeEqual = true;
                            }
                        }
                        else
                        {
                            // the maintenance mode length is multiple days.
                            mmodeTimeQuietTimeEqual = true;
                        }
                        break;
                    }
                case MaintenanceModeType.Recurring:
                    {
                        mModeStart = configuration.MaintenanceMode.MaintenanceModeRecurringStart;
                        mModeStop = mModeStart + configuration.MaintenanceMode.MaintenanceModeDuration;

                        if ((configuration.MaintenanceMode.MaintenanceModeDays & configuration.GrowthStatisticsDays) != 0)
                        {
                            if (((mModeStart.Value.TimeOfDay >= quietTimeStart.Value.TimeOfDay) && (mModeStart.Value.TimeOfDay < quietTimeStop.Value.TimeOfDay)) ||
                                ((mModeStop.Value.TimeOfDay <= quietTimeStop.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay > quietTimeStart.Value.TimeOfDay)) ||
                                ((mModeStart.Value.TimeOfDay <= quietTimeStart.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay >= quietTimeStop.Value.TimeOfDay)))
                            {
                                mmodeTimeQuietTimeEqual = true;
                            }
                        }
                        break;
                    }
                case MaintenanceModeType.Monthly:
                    {
                        mModeStart = configuration.MaintenanceMode.MaintenanceModeMonthRecurringStart;
                        mModeStop = mModeStart + configuration.MaintenanceMode.MaintenanceModeMonthDuration;

                        if (((mModeStart.Value.TimeOfDay >= quietTimeStart.Value.TimeOfDay) && (mModeStart.Value.TimeOfDay < quietTimeStop.Value.TimeOfDay)) ||
                                ((mModeStop.Value.TimeOfDay <= quietTimeStop.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay > quietTimeStart.Value.TimeOfDay)) ||
                                ((mModeStart.Value.TimeOfDay <= quietTimeStart.Value.TimeOfDay) && (mModeStop.Value.TimeOfDay >= quietTimeStop.Value.TimeOfDay)))
                        {
                            mmodeTimeQuietTimeEqual = true;
                        }
                        break;
                    }
                default:
                    {
                        mmodeTimeQuietTimeEqual = false;
                        break;
                    }
            }
            return mmodeTimeQuietTimeEqual;
        }

        private MonitoredSqlServerConfiguration BuildConfiguration()
        {
            MonitoredSqlServerConfiguration newConfiguration =
                new MonitoredSqlServerConfiguration(configuration.InstanceName);
            newConfiguration.CollectionServiceId = configuration.CollectionServiceId;
            newConfiguration.ConnectionInfo.UseIntegratedSecurity = useWindowsAuthenticationRadioButton.Checked;

            if (!newConfiguration.ConnectionInfo.UseIntegratedSecurity)
            {
                newConfiguration.ConnectionInfo.UserName = loginNameTextbox.Text;

                if (string.CompareOrdinal(passwordTextbox.Text, PasswordDecoyText) == 0)
                {
                    newConfiguration.ConnectionInfo.Password = configuration.ConnectionInfo.Password;
                }
                else
                {
                    newConfiguration.ConnectionInfo.Password = passwordTextbox.Text;
                }
            }

            newConfiguration.ConnectionInfo.EncryptData = encryptDataCheckbox.Checked;
            if (newConfiguration.ConnectionInfo.EncryptData)
                newConfiguration.ConnectionInfo.TrustServerCertificate = trustServerCertificateCheckbox.Checked;

            if (friendlyNameTextbox.Text != null && !friendlyNameTextbox.Text.Equals(""))
            {
                newConfiguration.FriendlyServerName = friendlyNameTextbox.Text;
            }
            newConfiguration.AlertRefreshInMinutes = this.alertRefreshInMinutes;

            newConfiguration.ScheduledCollectionInterval = collectionIntervalTimeSpanEditor.TimeSpan;
            newConfiguration.CloudProviderId = ((KeyValuePair<string, int>)cmbServerType.SelectedItem).Value;//(int)cmbServerType.SelectedValue;

            newConfiguration.Tags = new List<int>(GetSelectedTags());

            bool tempQueryMonitorEnabled = enableQueryMonitorTraceCheckBox.Checked;

            // Save enabled as true if the trace is going to stop automatically
            if (!tempQueryMonitorEnabled
                && configuration.QueryMonitorConfiguration.StopTimeUTC != null
                && configuration.QueryMonitorConfiguration.StopTimeUTC.HasValue
                && configuration.QueryMonitorConfiguration.StopTimeUTC.Value >= DateTime.Now.ToUniversalTime())
            {
                tempQueryMonitorEnabled = true;
            }

            newConfiguration.QueryMonitorConfiguration = new QueryMonitorConfiguration(
                tempQueryMonitorEnabled,
                captureSqlBatchesCheckBox.Checked,
                captureSqlStatementsCheckBox.Checked,
                captureStoredProceduresCheckBox.Checked,
                TimeSpan.FromMilliseconds(Convert.ToInt32(durationThresholdSpinner.Value)),
                TimeSpan.FromMilliseconds(Convert.ToInt32(cpuThresholdSpinner.Value)),
                Convert.ToInt32(logicalReadsThresholdSpinner.Value),
                Convert.ToInt32(physicalWritesThresholdSpinner.Value),
                configuration.QueryMonitorConfiguration.TraceFileSize,
                configuration.QueryMonitorConfiguration.TraceFileRollovers,
                configuration.QueryMonitorConfiguration.RecordsPerRefresh,
                configuration.QueryMonitorConfiguration.AdvancedConfiguration,
                !enableQueryMonitorTraceCheckBox.Checked
                    && configuration.QueryMonitorConfiguration.StopTimeUTC != null
                    && configuration.QueryMonitorConfiguration.StopTimeUTC.HasValue
                    && configuration.QueryMonitorConfiguration.StopTimeUTC.Value >= DateTime.Now.ToUniversalTime()
                    ? configuration.QueryMonitorConfiguration.StopTimeUTC
                    : null,
                rButtonUseTrace.Enabled ? rButtonUseTrace.Checked : false,
                chkCollectQueryPlans.Checked, //SQLdm 9.1 (Ankit Srivastava) -- Fixed Open functional isssue - disabling query plans option in case of trace monitoring 
                                              //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- added the traceenabled,collectQueryPlans Values to the constructor
                chkCollectEstimatedQueryPlans.Checked,
                Convert.ToInt32(this.topPlanSpinner.Value),
                Convert.ToInt32(this.topPlanComboBox.Value),
                this.rButtonUseQueryStore.Checked);

            newConfiguration.ActivityMonitorConfiguration = new ActivityMonitorConfiguration(
                chkActivityMonitorEnable.Checked,
                captureDeadlockCheckBox.Checked,
                chkCaptureBlocking.Checked,
                cloudProviderId==2?false:chkCaptureAutogrow.Checked,
                (int)spnBlockedProcessThreshold.Value,
                configuration.QueryMonitorConfiguration.TraceFileSize,
                configuration.QueryMonitorConfiguration.TraceFileRollovers,
                configuration.QueryMonitorConfiguration.RecordsPerRefresh,
                configuration.QueryMonitorConfiguration.AdvancedConfiguration,
                !enableQueryMonitorTraceCheckBox.Checked
                    && configuration.QueryMonitorConfiguration.StopTimeUTC != null
                    && configuration.QueryMonitorConfiguration.StopTimeUTC.HasValue
                    && configuration.QueryMonitorConfiguration.StopTimeUTC.Value >= DateTime.Now.ToUniversalTime()
                    ? configuration.QueryMonitorConfiguration.StopTimeUTC
                    : null,
                    rButtonAMUseTrace.Enabled ? rButtonAMUseTrace.Checked : false); //SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- added the traceEnabled

            newConfiguration.GrowthStatisticsStartTime = quietTimeStartEditor.DateTime;
            newConfiguration.ReorgStatisticsStartTime = quietTimeStartEditor.DateTime;

            short statDays = 0;
            if (collectTableStatsSundayCheckBox.Checked)
                statDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
            if (collectTableStatsMondayCheckBox.Checked)
                statDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            if (collectTableStatsTuesdayCheckBox.Checked)
                statDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            if (collectTableStatsWednesdayCheckBox.Checked)
                statDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            if (collectTableStatsThursdayCheckBox.Checked)
                statDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            if (collectTableStatsFridayCheckBox.Checked)
                statDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            if (collectTableStatsSaturdayCheckBox.Checked)
                statDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);

            newConfiguration.GrowthStatisticsDays = statDays;
            newConfiguration.ReorgStatisticsDays = statDays;
            newConfiguration.TableStatisticsExcludedDatabases = configuration.TableStatisticsExcludedDatabases;
            newConfiguration.ReorganizationMinimumTableSize.Kilobytes = minimumTableSize.Value;
            newConfiguration.ReplicationMonitoringDisabled = disableReplicationStuff.Checked;
            newConfiguration.ExtendedHistoryCollectionDisabled = !collectExtendedSessionDataCheckBox.Checked;

            newConfiguration.InputBufferLimited = limitInputBuffer.Checked;

            newConfiguration.PreferredClusterNode = preferredNode.Text;

            newConfiguration.InputBufferLimiter = (int)Math.Floor(inputBufferLimiter.Value);

            List<int> counters = new List<int>();
            foreach (ExtendedListItem counterItem in customCountersSelectionList.Selected.Items)
            {
                if (counterItem.Tag is int)
                {
                    counters.Add((int)counterItem.Tag);
                }
            }
            newConfiguration.CustomCounters = counters;

            //add maintenance mode info
            newConfiguration.MaintenanceMode.MaintenanceModeStart = mmOnceBeginDate.DateTime.Date + mmOnceBeginTime.Time;
            newConfiguration.MaintenanceMode.MaintenanceModeStop = mmOnceStopDate.DateTime.Date + mmOnceStopTime.Time;

            short mmDays = 0;
            if (mmBeginSunCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
            if (mmBeginMonCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            if (mmBeginTueCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            if (mmBeginWedCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            if (mmBeginThurCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            if (mmBeginFriCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            if (mmBeginSatCheckbox.Checked)
                mmDays |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);

            int mmMonth = 0;
            if (mmMonthlyDayRadio.Checked)
                mmMonth = (int)inputOfEveryMonthLimiter.Value;
            else
                mmMonth = (int)inputOfEveryTheMonthLimiter.Value;

            int maintenanceModeSpecificDay = 0;
            if (mmMonthlyRecurringRadio.Checked)
                if (mmMonthlyDayRadio.Checked)
                    maintenanceModeSpecificDay = (int)inputDayLimiter.Value;
                else
                    maintenanceModeSpecificDay = 0;
            else
                maintenanceModeSpecificDay = 0;

            int maintenanceModeWeekOrdinal = 0;
            if (mmMonthlyRecurringRadio.Checked)
                if (mmMonthlyTheRadio.Checked)
                    maintenanceModeWeekOrdinal = (int)WeekcomboBox.SelectedValue;
                else
                    maintenanceModeWeekOrdinal = 0;
            else
                maintenanceModeWeekOrdinal = 0;

            int maintenanceModeWeekDay = 0;
            if (mmMonthlyRecurringRadio.Checked)
                if (mmMonthlyTheRadio.Checked)
                    maintenanceModeWeekDay = (int)DaycomboBox.SelectedValue;
                else
                    maintenanceModeWeekDay = 0;
            else
                maintenanceModeWeekDay = 0;

            newConfiguration.MaintenanceMode.MaintenanceModeDays = mmDays;
            newConfiguration.MaintenanceMode.MaintenanceModeRecurringStart = mmRecurringBegin.DateTime;
            newConfiguration.MaintenanceMode.MaintenanceModeDuration = mmRecurringDuration.Time;

            newConfiguration.MaintenanceMode.MaintenanceModeMonth = mmMonth;
            newConfiguration.MaintenanceMode.MaintenanceModeSpecificDay = maintenanceModeSpecificDay;
            newConfiguration.MaintenanceMode.MaintenanceModeWeekOrdinal = maintenanceModeWeekOrdinal;
            newConfiguration.MaintenanceMode.MaintenanceModeWeekDay = maintenanceModeWeekDay;

            DateTime mmModeStartTime = new DateTime(1900, 1, 1,
                                                                                 mmMonthRecurringBegin.DateTime.Hour,
                                                                                 mmMonthRecurringBegin.DateTime.Minute,
                                                                                mmMonthRecurringBegin.DateTime.Second);
            newConfiguration.MaintenanceMode.MaintenanceModeMonthRecurringStart = mmModeStartTime;

            newConfiguration.MaintenanceMode.MaintenanceModeMonthDuration = mmMonthRecurringDuration.Time;


            if (mmNeverRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Never;
                newConfiguration.MaintenanceModeEnabled = false;
            }
            else if (mmOnceRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Once;

                //do not set the maintenance mode flag if it is not time for maintenance mode yet.
                if (DateTime.Now < newConfiguration.MaintenanceMode.MaintenanceModeStart)
                {
                    newConfiguration.MaintenanceModeEnabled = false;
                }
                else
                {
                    newConfiguration.MaintenanceModeEnabled = true;
                }

            }
            else if (mmRecurringRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Recurring;

                bool mmModeForToday = false;


                //See if today is of of the day for recurring maintenance mode.
                foreach (int val in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (MonitoredSqlServer.MatchDayOfWeek((DayOfWeek)val, newConfiguration.MaintenanceMode.MaintenanceModeDays))
                    {
                        if (val == (int)DateTime.Now.DayOfWeek)
                        {
                            mmModeForToday = true;
                            break;
                        }
                    }
                }

                //do not set the maintenance mode flag if it is not time for maintenance mode yet.
                if (mmModeForToday &&
                    (DateTime.Now.TimeOfDay >= newConfiguration.MaintenanceMode.MaintenanceModeRecurringStart.Value.TimeOfDay) &&
                    (DateTime.Now.TimeOfDay < (newConfiguration.MaintenanceMode.MaintenanceModeRecurringStart.Value.TimeOfDay + newConfiguration.MaintenanceMode.MaintenanceModeDuration)))
                {
                    newConfiguration.MaintenanceModeEnabled = true;
                }
                else
                {
                    newConfiguration.MaintenanceModeEnabled = false;
                }
            }
            else if (mmMonthlyRecurringRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Monthly;
                bool mmModeForToday = false;

                DateTime dt = DateTime.Now;
                int currentMonth = dt.Month;
                int currentDay = dt.Day;
                int selectedWeek = currentDay / 7;
                int remForselectedWeek = currentDay % 7;
                if (remForselectedWeek > 0)
                    selectedWeek = selectedWeek + 1;

                // if the current month is devisible by mmMonth, then we can say that Maintenance is scheduled for current month.
                if (currentMonth % mmMonth == 0)
                {
                    if (mmMonthlyDayRadio.Checked)
                    {
                        if (currentDay.ToString().Equals(inputDayLimiter.Value.ToString()))
                        {
                            mmModeForToday = true;
                        }
                    }
                    else
                    {
                        if ((selectedWeek == (int)WeekcomboBox.SelectedValue) && ((int)dt.DayOfWeek == (int)WeekcomboBox.SelectedValue))
                        {
                            mmModeForToday = true;
                        }
                    }
                }

                //do not set the maintenance mode flag if it is not time for maintenance mode yet.
                if (mmModeForToday &&
                    (DateTime.Now.TimeOfDay >= newConfiguration.MaintenanceMode.MaintenanceModeMonthRecurringStart.Value.TimeOfDay) &&
                                                (DateTime.Now.TimeOfDay <
                                                 (newConfiguration.MaintenanceMode.MaintenanceModeMonthRecurringStart.Value.TimeOfDay +
                                                  newConfiguration.MaintenanceMode.MaintenanceModeMonthDuration)))
                {
                    newConfiguration.MaintenanceModeEnabled = true;
                }
                else
                {
                    newConfiguration.MaintenanceModeEnabled = false;
                }
            }
            else if (mmAlwaysRadio.Checked)
            {
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Always;
                newConfiguration.MaintenanceModeEnabled = true;
            }
            else
            {
                //We should never get here but it is included to be through
                newConfiguration.MaintenanceMode.MaintenanceModeType = MaintenanceModeType.Never;
                newConfiguration.MaintenanceModeEnabled = false;
            }

            DiskCollectionSettings diskSettings = new DiskCollectionSettings();
            diskSettings.AutoDiscover = autoDiscoverDisksCheckBox.Checked;
            if (configuration != null && configuration.DiskCollectionSettings != null)
                diskSettings.CookedDiskDriveWaitTimeInSeconds = configuration.DiskCollectionSettings.CookedDiskDriveWaitTimeInSeconds;
            Set<string> drives = GetListBoxItems(selectedDisksListBox, false);
            diskSettings.Drives = drives.ToArray();

            newConfiguration.DiskCollectionSettings = diskSettings;

            // if anything in the qm config changed then clear the stop time utc value
            if (!newConfiguration.QueryMonitorConfiguration.Equals(configuration.QueryMonitorConfiguration))
            {
                newConfiguration.QueryMonitorConfiguration.StopTimeUTC = null;
                newConfiguration.QueryMonitorConfiguration.IsAlertResponseQueryTrace = false;
            }

            newConfiguration.ActiveWaitsConfiguration = new ActiveWaitsConfiguration(id);
            newConfiguration.ActiveWaitsConfiguration.Enabled = chkCollectActiveWaits.Checked;
            newConfiguration.ActiveWaitsConfiguration.StartTimeRelative = waitStatisticsStartDate.DateTime.Date + waitStatisticsStartTime.Time;
            if (PerpetualQueryWaits.Checked)
                newConfiguration.ActiveWaitsConfiguration.RunTime = null;
            else
                newConfiguration.ActiveWaitsConfiguration.RunTime = TimeSpan.FromMilliseconds(waitStatisticsRunTime.Time.TotalMilliseconds);
            newConfiguration.ActiveWaitsConfiguration.AdvancedConfiguration =
                configuration.ActiveWaitsConfiguration.AdvancedConfiguration;

            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            newConfiguration.ActiveWaitsConfiguration.EnabledQs = this.chkUseQs.Checked;
            newConfiguration.ActiveWaitsConfiguration.EnabledXe = chkUseXE.Checked;

            newConfiguration.ClusterCollectionSetting = configuration.ClusterCollectionSetting;

            newConfiguration.ServerPingInterval = serverPingTimeSpanEditor.TimeSpan;

            newConfiguration.DatabaseStatisticsInterval = databaseStatisticsTimeSpanEditor.TimeSpan;

            // Carrying over the old virtualization configuration because this is currently just set in the 
            //  virtualization configuration dialog... eventually this will change
            newConfiguration.VirtualizationConfiguration = Configuration.VirtualizationConfiguration;

            newConfiguration.BaselineConfiguration = baselineConfiguration1.BaselineConfig;
            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            newConfiguration.BaselineConfigurationList = baselineConfiguration1.BaselineConfigList;
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            //10.0 SQLdm srishti purohit -- doctors UI configuration object
            newConfiguration.AnalysisConfiguration = analysisConfigurationPage.AnalysisConfig;
            var wmi = newConfiguration.WmiConfig;
            wmi.OleAutomationDisabled = !optionWmiOleAutomation.Checked;
            wmi.DirectWmiEnabled = optionWmiDirect.Checked;
            wmi.DirectWmiConnectAsCollectionService = optionWmiCSCreds.Checked;
            wmi.DirectWmiUserName = directWmiUserName.Text;
            wmi.DirectWmiPassword = directWmiPassword.Text;

            return newConfiguration;
        }

        private IList<int> GetSelectedTags()
        {
            List<int> selectedTags = new List<int>();

            foreach (StateButtonTool tag in ((PopupMenuTool)(toolbarManager.Tools["tagsPopupMenu"])).Tools)
            {
                if (tag.Key != NewTagToolKey && tag.Checked)
                {
                    selectedTags.Add(Convert.ToInt32(tag.Key));
                }
            }

            return selectedTags;
        }

        private void testConnectionCredentialsButton_Click(object sender, EventArgs e)
        {
            if (useSqlServerAuthenticationRadioButton.Checked && loginNameTextbox.Text.Trim().Length == 0)
            {
                ApplicationMessageBox.ShowInfo(this,
                                               "Please specify the SQL Server credentials before testing this connection.");
            }
            else
            {
                Cursor = Cursors.WaitCursor;
                testConnectionCredentialsButton.Enabled = false;
                testConnectionBackgroundWorker.RunWorkerAsync(BuildConfiguration());
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {

            viewClosing = true;


            if (testConnectionBackgroundWorker != null)
            {
                testConnectionBackgroundWorker.CancelAsync();
            }

            if (waitCollectorStatusBackgroundWorker != null)
            {
                waitCollectorStatusBackgroundWorker.CancelAsync();
            }

            if (serverDateTimeVersionBackgroundWorker != null)
            {
                serverDateTimeVersionBackgroundWorker.CancelAsync();
            }

            // SQLdm Minimum Privileges - Varun Chopra - Cancel Server Permissions
            if (serverPermissionsBackgroundWorker != null)
            {
                serverPermissionsBackgroundWorker.CancelAsync();
            }

            if (stopWaitCollectorBackgroundWorker != null)
            {
                stopWaitCollectorBackgroundWorker.CancelAsync();
            }

            if (availableDisksBackgroundWorker != null)
            {
                availableDisksBackgroundWorker.CancelAsync();
            }

            if (wmiTestBackgroundWorker != null)
            {
                wmiTestBackgroundWorker.CancelAsync();
            }

            if (blockedProcessThresholdBackgroundWorker != null)
            {
                blockedProcessThresholdBackgroundWorker.CancelAsync();
            }

            base.OnClosing(e);
        }

        private void testConnectionBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                MonitoredSqlServerConfiguration newConfiguration = e.Argument as MonitoredSqlServerConfiguration;

                if (newConfiguration != null)
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    e.Result = defaultManagementService.TestSqlConnection(newConfiguration.ConnectionInfo);
                }
                else
                {
                    throw new DesktopClientException("The configuration object for test connection is invalid.");
                }
            }
            finally
            {
                BackgroundWorker backgroundWorker = sender as BackgroundWorker;

                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        private void testConnectionBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (viewClosing)
                return;
            Cursor = Cursors.Default;
            testConnectionCredentialsButton.Enabled = true;

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    TestSqlConnectionResult result = e.Result as TestSqlConnectionResult;

                    if (result != null)
                    {
                        if (result.Succeeded)
                        {
                            string[] versionSplit = result.ServerVersion.Split(new char[] { '.' });

                            if (Convert.ToInt32(versionSplit[0]) >= 8)
                            {
                                StringBuilder message = new StringBuilder("The connection test succeeded.");
                                message.Append("\r\n\r\n");
                                message.Append("Version: ");
                                message.Append(result.ServerVersion);
                                ApplicationMessageBox.ShowInfo(this, message.ToString());
                            }
                            else
                            {
                                StringBuilder message =
                                    new StringBuilder(
                                        "The specified SQL Server instance is an unsupported version. SQL Diagnostic Manager monitors SQL Server 2000 and higher.");
                                message.Append("\r\n\r\n");
                                message.Append("Version: ");
                                message.Append(result.ServerVersion);
                                ApplicationMessageBox.ShowWarning(this, message.ToString());
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowError(this, "The connection test failed.", result.Error);
                        }
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this, "The connection test result is invalid.");
                    }
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "The connection test failed.", e.Error);
                }
            }
        }

        private void selectTableStatisticsTablesButton_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                ApplicationMessageBox.ShowInfo(this,
                                               "This instance is not currently monitored. The instance must be monitored before databases can be excluded from table statistics collection.");
                return;
            }

            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            using (DatabaseBrowserDialog dbd = new DatabaseBrowserDialog(defaultManagementService, id, configuration.InstanceName, true, false, "Select databases to exclude from table growth statistics collection"))
            {
                dbd.CheckedDatabases = configuration.TableStatisticsExcludedDatabases;
                dbd.HelpTopic = HelpTopics.TableStatisticsDatabaseExclude;
                if (dbd.ShowDialog(this) == DialogResult.OK)
                {
                    configuration.TableStatisticsExcludedDatabases = dbd.CheckedDatabases;
                }
            }
        }

        private void quietTimeStartEditor_ValueChanged(object sender, EventArgs e)
        {
            setEndTime(quietTimeStartEditor.Time);
        }

        private void encryptDataCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            trustServerCertificateCheckbox.Enabled = encryptDataCheckbox.Checked;
        }

        private void setEndTime(TimeSpan start)
        {
            TimeSpan end = start + TimeSpan.FromHours(3);
            DateTime temp = DateTime.Now;
            DateTime endDt = new DateTime(temp.Year, temp.Month, temp.Day, end.Hours, end.Minutes, 0);
            endTimeLabel.Text = "and " + endDt.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Replace(":ss", string.Empty));
        }

        private void testCustomCountersButton_Click(object sender, EventArgs e)
        {
            List<int> counters = new List<int>();

            foreach (ExtendedListItem counterItem in customCountersSelectionList.Selected.Items)
            {
                int counterId = -1;

                if (counterItem.Tag is int)
                {
                    counterId = (int)counterItem.Tag;
                }
                else if (counterItem.Tag is Pair<int, int>)
                {
                    counterId = ((Pair<int, int>)counterItem.Tag).First;
                }

                if (counterId != -1 && !counters.Contains(counterId))
                {
                    counters.Add(counterId);
                }
            }

            MonitoredSqlServerWrapper wrappedServer = ApplicationModel.Default.ActiveInstances[id];
            using (TestInstanceCustomCountersDialog ticcd = new TestInstanceCustomCountersDialog(wrappedServer.Instance, metrics, counters.ToArray()))
            {
                ticcd.ShowDialog(this);
            }
        }

        private void MonitoredSqlServerInstancePropertiesDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            ServerDetailsView activeView = ApplicationController.Default.ActiveView as ServerDetailsView;

            if (activeView != null)
            {
                activeView.RefreshView();
            }
        }

        private string GetHelpTopic()
        {
            string topic;

            switch (SelectedPropertyPage)
            {
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.QueryMonitor:
                    topic = HelpTopics.ServerPropertiesQueryMonitor;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.Replication:
                    topic = HelpTopics.ServerPropertiesReplication;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.TableStatistics:
                    topic = HelpTopics.ServerPropertiesTableStatistics;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.CustomCounters:
                    topic = HelpTopics.TagsServerPropertiesCustomCounters;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.MaintenanceMode:
                    topic = HelpTopics.MaintenanceMode;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.OleAutomation:
                    topic = HelpTopics.ServerPropertiesOleAutomation;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.Disk:
                    topic = HelpTopics.ServerPropertiesDiskStatistics;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.ClusterSettings:
                    topic = HelpTopics.ServerPropertiesClusterSettings;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.WaitStatistics:
                    topic = HelpTopics.ServerPropertiesWaitMonitoring;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.Virtualization:
                    topic = HelpTopics.ServerPropertiesVirtualization;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.Baseline:
                    topic = HelpTopics.ServerPropertiesBaseline;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.ActivityMonitor:
                    topic = HelpTopics.ServerPropertiesActivityMonitor;
                    break;
                case MonitoredSqlServerInstancePropertiesDialogPropertyPages.AnalysisConfiguration:
                    topic = HelpTopics.ServerPropertiesAnalysisConfiguration;
                    break;
                default:
                    topic = HelpTopics.ServerPropertiesGeneral;
                    break;
            }

            return topic;
        }

        private void MonitoredSqlServerInstancePropertiesDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(GetHelpTopic());
        }

        private void MonitoredSqlServerInstancePropertiesDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(GetHelpTopic());
        }

        private void mmNeverRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmNeverRadio.Checked)
            {
                mmMonthlyRecurringPanel.Visible = false;
                mmRecurringPanel.Visible = false;
                mmOncePanel.Visible = false;
            }
        }

        private void mmAlwaysRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmAlwaysRadio.Checked)
            {
                mmMonthlyRecurringPanel.Visible = false;
                mmRecurringPanel.Visible = false;
                mmOncePanel.Visible = false;
            }
        }

        private void mmWeeklyRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmRecurringRadio.Checked)
            {
                mmRecurringPanel.Visible = true;
                mmOncePanel.Visible = false;
                mmMonthlyRecurringPanel.Visible = false;
            }
        }

        private void mmOnceRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmOnceRadio.Checked)
            {
                mmRecurringPanel.Visible = false;
                mmOncePanel.Visible = true;
                mmMonthlyRecurringPanel.Visible = false;
            }
        }

        private void mmMonthlyRecurringRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmMonthlyRecurringRadio.Checked)
            {
                mmMonthlyRecurringPanel.Visible = true;
                mmMonthlyDayRadio.Checked = true;
                mmRecurringPanel.Visible = false;
                mmOncePanel.Visible = false;
            }
        }

        private void mmMonthlyDayRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (mmMonthlyDayRadio.Checked)
            {
                WeekcomboBox.Enabled = false;
                DaycomboBox.Enabled = false;
                inputOfEveryTheMonthLimiter.Enabled = false;
                inputDayLimiter.Enabled = true;
                inputOfEveryMonthLimiter.Enabled = true;
            }
        }
        void mmMonthlyTheRadio_CheckedChanged(object sender, System.EventArgs e)
        {
            if (mmMonthlyTheRadio.Checked)
            {
                inputDayLimiter.Enabled = false;
                inputOfEveryMonthLimiter.Enabled = false;
                WeekcomboBox.Enabled = true;
                DaycomboBox.Enabled = true;
                inputOfEveryTheMonthLimiter.Enabled = true;

            }
        }
        private void queryMonitorAdvancedOptionsButton_Click(object sender, EventArgs e)
        {
            AdvancedQueryFilterConfigurationDialog dialog =
                new AdvancedQueryFilterConfigurationDialog(
                    configuration.QueryMonitorConfiguration.AdvancedConfiguration, "Query Monitor");
            dialog.ShowDialog(this);
        }

        private void toolbarManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool.OwningMenu == toolbarManager.Tools["tagsPopupMenu"])
            {
                if (e.Tool.Key == NewTagToolKey)
                {
                    AddTagDialog dialog = new AddTagDialog();

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        availableTags.Add(dialog.TagName, dialog.TagId);
                        configuration.Tags.Add(dialog.TagId);
                        UpdateTags();
                    }
                }
                else
                {
                    UpdateSelectedTags();
                }

                tagsChanged = true;
            }
        }

        private void UpdateTags()
        {
            ((PopupMenuTool)(toolbarManager.Tools["tagsPopupMenu"])).Tools.Clear();

            if (availableTags.Count > 0)
            {
                foreach (KeyValuePair<string, int> tag in availableTags)
                {
                    if (!toolbarManager.Tools.Exists(tag.Value.ToString()))
                    {
                        StateButtonTool tagTool = new StateButtonTool(tag.Value.ToString());
                        tagTool.MenuDisplayStyle = StateButtonMenuDisplayStyle.DisplayCheckmark;
                        tagTool.SharedProps.Caption = tag.Key;
                        tagTool.Checked = configuration.Tags.Contains(tag.Value);
                        toolbarManager.Tools.Add(tagTool);
                        ((PopupMenuTool)(toolbarManager.Tools["tagsPopupMenu"])).Tools.Add(tagTool);
                    }
                    else
                    {
                        ((PopupMenuTool)(toolbarManager.Tools["tagsPopupMenu"])).Tools.Add(
                            toolbarManager.Tools[tag.Value.ToString()]);
                    }
                }
            }

            ToolBase newTagTool;
            if (!toolbarManager.Tools.Exists(NewTagToolKey))
            {
                newTagTool = new StateButtonTool(NewTagToolKey);
                newTagTool.SharedProps.Caption = NewTagToolKey;
                toolbarManager.Tools.Add(newTagTool);
            }
            else
            {
                newTagTool = toolbarManager.Tools[NewTagToolKey];
            }

            ((PopupMenuTool)(toolbarManager.Tools["tagsPopupMenu"])).Tools.Add(newTagTool);
            ((PopupMenuTool)(toolbarManager.Tools["tagsPopupMenu"])).Tools[NewTagToolKey].InstanceProps.IsFirstInGroup = true;

            UpdateSelectedTags();
        }

        private void UpdateSelectedTags()
        {
            StringBuilder newText = new StringBuilder();

            foreach (StateButtonTool tag in ((PopupMenuTool)(toolbarManager.Tools["tagsPopupMenu"])).Tools)
            {
                if (tag.Key != NewTagToolKey && tag.Checked)
                {
                    if (newText.Length > 0)
                    {
                        newText.Append(", ");
                    }

                    newText.Append(tag.SharedProps.Caption);
                }
            }

            if (newText.Length != 0)
            {
                tagsDropDownButton.Text = newText.ToString();
            }
            else
            {
                tagsDropDownButton.Text = ChooseTagText;
            }
        }

        private void propertiesControl_PropertyPageChanged(object sender, EventArgs e)
        {
            switch (propertiesControl.SelectedPropertyPageIndex)
            {
                case (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.CustomCounters:
                    if (tagsChanged)
                        UpdateCustomCounters();
                    break;
                case (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.Disk:
                    if (availableDisksListBox.Items.Count == 0)
                        StartAvailableDisksBackgroundWorker();
                    break;
                case (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.QueryMonitor:
                    UpdateQueryMonitoringLabel();
                    break;
                case (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.ActivityMonitor:
                    UpdateActivityMonitorLabel();
                    break;
                case (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.WaitStatistics:
                    UpdateWaitMonitoringLabel();
                    break;
                case (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.OleAutomation:
                    UpdateOLEAutomationLabel();
                    break;
            }
        }

        private void MonitoredSqlServerInstancePropertiesDialog_Load(object sender, EventArgs e)
        {

        }

        private void availableTablesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void availableTablesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void databaseComboBox_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void addButton_Click(object sender, EventArgs e)
        {
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
        }


        private void selectedTablesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void selectedTablesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void availableTablesTextBox_TextChanged(object sender, EventArgs e)
        {
        }


        private void availableTablesTextBox_Leave(object sender, EventArgs e)
        {

        }

        private void availableTablesTextBox_Enter(object sender, EventArgs e)
        {

        }

        private void autoDiscoverDisksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            availableDisksLayoutPanel.Enabled = !autoDiscoverDisksCheckBox.Checked;
        }

        private void StartAvailableDisksBackgroundWorker()
        {
            KillAvailableDisksBackgroundWorker();

            availableDisksStackPanel.ActiveControl = availableDisksMessageLabel;
            availableDisksMessageLabel.Text = "Discovering Disk Drives...";

            var wmiConfiguration = new WmiConfiguration();
            wmiConfiguration.OleAutomationDisabled = !optionWmiOleAutomation.Checked;
            wmiConfiguration.DirectWmiEnabled = optionWmiDirect.Checked;
            wmiConfiguration.DirectWmiConnectAsCollectionService = optionWmiCSCreds.Checked;
            wmiConfiguration.DirectWmiUserName = directWmiUserName.Text;
            wmiConfiguration.DirectWmiPassword = directWmiPassword.Text;

            availableDisksBackgroundWorker = new BackgroundWorker();
            availableDisksBackgroundWorker.WorkerSupportsCancellation = true;
            availableDisksBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(availableDisksBackgroundWorker_RunWorkerCompleted);
            availableDisksBackgroundWorker.DoWork += new DoWorkEventHandler(availableDisksBackgroundWorker_DoWork);
            availableDisksBackgroundWorker.RunWorkerAsync(wmiConfiguration);
        }

        void CheckWmiCredentials(ref bool retry, WmiConfiguration wmiConfiguration, Exception exception)
        {
            var answer = ApplicationMessageBox.ShowMessage(this,
                "Failed to connect to the WMI service on the monitored server to gather disk drive list.  Would you like to provide a different set of credentials and try again?",
                exception,
                ExceptionMessageBoxSymbol.Question,
                ExceptionMessageBoxButtons.YesNo,
                false);

            if (answer == DialogResult.No)
            {
                retry = false;
                return;
            }

            using (var dialog = new WmiCredentialsDialog(wmiConfiguration))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    retry = false;
                }
            }
        }

        void availableDisksBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var backgroundWorker = (BackgroundWorker)sender;

            try
            {
                SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                IManagementService defaultManagementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
                //SQLDM 10.3 SQLDM-28718
                DataTable result = null;
                var wmiConfiguration = (WmiConfiguration)e.Argument;
                bool retry = true;

                while (retry && !(configuration.CloudProviderId.HasValue && configuration.CloudProviderId == Common.Constants.LinuxId || configuration.CloudProviderId == Common.Constants.MicrosoftAzureId || configuration.CloudProviderId == Common.Constants.AmazonRDSId)) //SQLDM 10.3 SQLDM-28718 - Allow the process if its not Linux
                {
                    Exception lastException = null;
                    if (backgroundWorker.CancellationPending) break;
                    try
                    {
                        result = defaultManagementService.GetDriveConfiguration(configuration.ConnectionInfo, wmiConfiguration);
                        break;
                    }
                    catch (Exception excp)
                    {
                        lastException = excp;
                    }
                    var action = new System.Action(() => CheckWmiCredentials(ref retry, wmiConfiguration, lastException));
                    this.Invoke(action);
                }

                if (result != null && !backgroundWorker.CancellationPending)
                {
                    List<string> theList = new List<string>();
                    foreach (DataRow row in result.Rows)
                    {
                        // only include local and removable drives
                        if (row.IsNull("DriveType"))
                            continue;
                        uint driveType = (uint)row["DriveType"];
                        if (driveType != 2 && driveType != 3)
                            continue;

                        string drive = row["DriveLetter"] as string;
                        if (String.IsNullOrEmpty(drive))
                        {
                            drive = row["Name"] as string;
                            if (String.IsNullOrEmpty(drive))
                                continue;
                        }

                        drive = drive.TrimEnd('\\', '/');

                        theList.Add(drive);
                    }
                    e.Result = theList;
                }
            }
            finally
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }


        void availableDisksBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (viewClosing)
                return;

            BackgroundWorker worker = (BackgroundWorker)sender;

            if (e.Cancelled || this.IsDisposed)
                return;

            try
            {
                if (e.Error != null)
                {
                    ApplicationMessageBox.ShowError(this, e.Error);
                    return;
                }

                Set<string> selected = GetListBoxItems(selectedDisksListBox, true);
                Set<string> available = GetListBoxItems(availableDisksListBox, true);

                List<string> thelist = e.Result as List<string>;
                if (thelist != null)
                {
                    thelist.Sort();
                    foreach (string disk in thelist)
                    {
                        string ldisk = disk.ToLower();
                        if (!selected.Contains(ldisk) && !available.Contains(ldisk))
                        {
                            availableDisksListBox.Items.Add(disk);
                            available.Add(ldisk);
                        }
                    }
                }


            }
            finally
            {
                if (!viewClosing)
                {
                    availableDisksStackPanel.ActiveControl = availableDisksListBox;
                    UpdateDiskControls(true);
                }
            }
        }

        private Set<string> GetListBoxItems(ListBox listBox, bool lowerCase)
        {
            Set<string> result = new Set<string>();
            foreach (String item in listBox.Items)
            {
                result.Add(lowerCase ? item.ToLower() : item);
            }
            return result;
        }

        private void KillAvailableDisksBackgroundWorker()
        {
            if (availableDisksBackgroundWorker != null)
            {
                if (availableDisksBackgroundWorker.IsBusy)
                    availableDisksBackgroundWorker.CancelAsync();
                availableDisksBackgroundWorker = null;
            }
        }

        private void addDiskButton_Click(object sender, EventArgs e)
        {
            AddSelectedDisks();
            AddAdhocDisks();
            UpdateDiskControls();
            adhocDisksTextBox.Focus();
            adhocDisksTextBox_Enter(adhocDisksTextBox, e);
        }

        private void removeDiskButton_Click(object sender, EventArgs e)
        {
            while (selectedDisksListBox.SelectedItems.Count > 0)
            {
                string firstItem = (string)selectedDisksListBox.SelectedItems[0];
                AddToListBox(availableDisksListBox, firstItem);
                selectedDisksListBox.Items.Remove(firstItem);
            }
            UpdateDiskControls();
        }

        private void UpdateDiskControls()
        {
            UpdateDiskControls(false);
        }

        private void UpdateDiskControls(bool ignoreAvailableTextBox)
        {
            availableDisksLayoutPanel.Enabled = !autoDiscoverDisksCheckBox.Checked;
            removeDiskButton.Enabled = selectedDisksListBox.SelectedItems.Count > 0;
            addDiskButton.Enabled = availableDisksListBox.SelectedItems.Count > 0 ||
                                         (adhocDisksTextBox.Text.Trim().Length > 0 &&
                                         string.CompareOrdinal(AdhocDiskInstructionText, adhocDisksTextBox.Text.Trim()) != 0);


            if (addDiskButton.Enabled)
            {
                this.AcceptButton = addDiskButton;
            }
            else if (removeDiskButton.Enabled)
            {
                this.AcceptButton = removeDiskButton;
            }

            if (!ignoreAvailableTextBox && adhocDisksTextBox.Text.Trim().Length == 0)
            {
                adhocDisksTextBox.ForeColor = SystemColors.GrayText;
                adhocDisksTextBox.Text = AdhocDiskInstructionText;
            }
        }

        private void RemoveFromListBox(ListBox listBox, string value)
        {
            string lvalue = value.ToLower();
            foreach (string item in listBox.Items)
            {
                if (item.ToLower().Equals(lvalue))
                {
                    listBox.Items.Remove(item);
                    return;
                }
            }
        }

        private void AddToListBox(ListBox listBox, string value)
        {
            string lvalue = value.ToLower();
            foreach (string item in listBox.Items)
            {
                if (item.ToLower().Equals(lvalue))
                    return;
            }
            listBox.Items.Add(value);
        }

        private void AddAdhocDisks()
        {
            string adhocDisksText = adhocDisksTextBox.Text.Trim();

            if (adhocDisksText.Length != 0 &&
                string.CompareOrdinal(adhocDisksText, AdhocDiskInstructionText) != 0)
            {
                string[] adhocDisks = adhocDisksText.Split(new char[] { ';' });

                foreach (string adhocDisk in adhocDisks)
                {
                    string trimmedDiskName = adhocDisk.Trim();
                    trimmedDiskName = trimmedDiskName.TrimEnd('\\', '/');
                    switch (trimmedDiskName.Length)
                    {
                        case 0:
                            continue;
                        case 1:
                            if (!char.IsLetter(trimmedDiskName[0]))
                                continue;
                            trimmedDiskName = trimmedDiskName.ToUpper() + ":";
                            break;
                        default:
                            if (!char.IsLetter(trimmedDiskName[0]) || trimmedDiskName[1] != ':')
                                continue;
                            break;
                    }

                    AddToListBox(selectedDisksListBox, trimmedDiskName);
                    RemoveFromListBox(availableDisksListBox, trimmedDiskName);
                }
            }

            adhocDisksTextBox.Clear();
        }

        private void AddSelectedDisks()
        {
            while (availableDisksListBox.SelectedItems.Count > 0)
            {
                string firstItem = (string)availableDisksListBox.SelectedItems[0];
                AddToListBox(selectedDisksListBox, firstItem);
                availableDisksListBox.Items.Remove(firstItem);
            }
        }

        private void adhocDisksTextBox_Enter(object sender, EventArgs e)
        {
            if (string.CompareOrdinal(AdhocDiskInstructionText, adhocDisksTextBox.Text) == 0)
            {
                adhocDisksTextBox.Clear();
            }

            adhocDisksTextBox.ForeColor = SystemColors.WindowText;
        }

        private void adhocDisksTextBox_Leave(object sender, EventArgs e)
        {
            UpdateDiskControls();
        }

        private void adhocDisksTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateDiskControls(true);
        }

        private void selectedDisksListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeDiskButton.Enabled = (selectedDisksListBox.SelectedIndex != -1);
        }

        private void selectedDisksListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedDisksListBox.SelectedItems.Count > 0)
            {
                string qualifiedName = selectedDisksListBox.SelectedItem.ToString();
                AddToListBox(availableDisksListBox, qualifiedName);
                selectedDisksListBox.Items.Remove(qualifiedName);
            }
        }

        private void availableDisksListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addDiskButton.Enabled = (availableDisksListBox.SelectedIndex != -1);
        }

        private void availableDisksListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (availableDisksListBox.SelectedItems.Count > 0)
            {
                String qualifiedName = availableDisksListBox.SelectedItem.ToString();
                AddToListBox(selectedDisksListBox, qualifiedName);
                availableDisksListBox.Items.Remove(qualifiedName);
            }
        }

        private void limitInputBuffer_CheckedChanged(object sender, EventArgs e)
        {
            inputBufferLimiter.Enabled = limitInputBuffer.Checked;
        }

        private void setCurrentNode_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                ApplicationMessageBox.ShowInfo(this,
                                               "This instance is not currently monitored. The instance must be monitored before the current cluster node can be retrieved.");
                return;
            }

            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            try
            {
                preferredNode.Text = defaultManagementService.GetCurrentClusterNode(id);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message == "Monitored SQL Server is not clustered")
                {
                    ApplicationMessageBox.ShowWarning(this,
                                                      "The monitored server is not clustered.  The current node may only be retrieved for clustered servers.",
                                                      ex);
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "The current node could not be retrieved.", ex);
                }
            }

        }

        private void chkCollectActiveWaits_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCollectActiveWaits.Checked)
            {
                ScheduledQueryWaits.Enabled = true;
                PerpetualQueryWaits.Enabled = true;
            }
            else
            {
                ScheduledQueryWaits.Enabled = false;
                PerpetualQueryWaits.Enabled = false;
            }
            ShowHideWaitSchedulePanel();
            if (!waitCollectorStatusBackgroundWorker.IsBusy)
                waitCollectorStatusBackgroundWorker.RunWorkerAsync(id);
        }

        private void ShowHideWaitSchedulePanel()
        {
            if (chkCollectActiveWaits.Checked && ScheduledQueryWaits.Checked)
            {
                waitStatisticsStartDate.Enabled = true;
                waitStatisticsStartTime.Enabled = true;
                waitStatisticsRunTime.Enabled = true;
            }
            else
            {
                waitStatisticsStartDate.Enabled = false;
                waitStatisticsStartTime.Enabled = false;
                waitStatisticsRunTime.Enabled = false;
            }
        }

        private void waitCollectorStatusBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int MonitoredSQLServerID = (int)e.Argument;


                if (MonitoredSQLServerID > 0)
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    e.Result = defaultManagementService.GetActiveWaitCollectorStatus(MonitoredSQLServerID);
                }


            }
            catch (Exception)
            {
                e.Result = new Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions, CollectionPermissions>(ContinuousCollectorRunStatus.Unknown, MinimumPermissions.None, MetadataPermissions.None, CollectionPermissions.None);
            }
            finally
            {
                BackgroundWorker backgroundWorker = sender as BackgroundWorker;

                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        private void UpdateQueryMonitoringLabel()
        {
            try
            {
                // Permissions have been read
                if (minimumPermissions == MinimumPermissions.None && metadataPermissions == MetadataPermissions.None && collectionPermissions == CollectionPermissions.None)
                {
                    HideAndClearLabel(lblQueryMonitoringWarningLabel);
                    return;
                }
                string queryMonitoringWarningText = this.rButtonUseQueryStore.Checked ? QueryMonitoringWarningQs : rButtonUseExtendedEvents.Checked ? QueryMonitoringWarningEx : QueryMonitoringWarningTrc;
                //var newConfiguration= BuildConfiguration();
                if (savedServer == null)
                {
                    savedServer = ApplicationModel.Default.UpdateMonitoredSqlServer(id, configuration);
                }
                var timeStamp = new ScheduledRefresh(savedServer).TimeStamp;

                if (enableQueryMonitorTraceCheckBox.Checked)
                {
                    var queryCollector = "Query Monitor";
                    var probeError = new ProbePermissionHelpers.ProbeError() { Name = queryCollector };
                    // Included Query Store for permissions
                    var isPermissionValid = ProbePermissionHelpers.ValidateProbePermissions(
                        minimumPermissions,
                        metadataPermissions,
                        collectionPermissions,
                        serverVersion ?? configuration.MostRecentSQLVersion,
                        queryCollector,
                        configuration.CloudProviderId,
                        enableQueryMonitorTraceCheckBox.Checked,
                        captureSqlBatchesCheckBox.Checked,
                        captureSqlStatementsCheckBox.Checked,
                        captureStoredProceduresCheckBox.Checked,
                        configuration.QueryMonitorConfiguration.IsAlertResponseQueryTrace,
                        configuration.QueryMonitorConfiguration.StopTimeUTC,
                        timeStamp,
                        this.rButtonUseQueryStore.Checked,
                        probeError);

                    if (!isPermissionValid)
                    {
                        lblQueryMonitoringWarningLabel.Visible = true;
                        lblQueryMonitoringWarningLabel.Text = queryMonitoringWarningText;
                    }
                    else
                    {
                        HideAndClearLabel(lblQueryMonitoringWarningLabel);
                    }
                }
                else
                {
                    HideAndClearLabel(lblQueryMonitoringWarningLabel);
                }
            }
            catch (Exception exception)
            {
                HideAndClearLabel(lblQueryMonitoringWarningLabel);
                Log.Error("UpdateQueryMonitoringLabel: Exception occured " + exception);
            }
        }

        /// <summary>
        /// Hide and Clear the <paramref name="label"/> control
        /// </summary>
        /// <param name="label">Label to set visible property to false and clear the text property</param>
        private void HideAndClearLabel(Label label)
        {
            HideLabel(label);

            if (!string.IsNullOrEmpty(label.Text))
            {
                label.Text = string.Empty;
            }
        }

        /// <summary>
        /// Hides the <paramref name="label"/> control
        /// </summary>
        /// <param name="label">Label to set visible property to false</param>
        private void HideLabel(Label label)
        {
            if (label.Visible)
            {
                label.Visible = false;
            }
        }

        private void waitCollectorStatusBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (viewClosing)
                return;

            if (!e.Cancelled)
            {
                refreshWaitCollectorStatus.Enabled = true;
                ContinuousCollectorRunStatus runStatus = ContinuousCollectorRunStatus.Unknown;
                if (e.Error == null && e.Result != null)
                {
                    // SQLdm Minimum Privileges - Varun Chopra - Allow update Server Permissions when wait monitoring status is updated
                    var returnedResult =
                    (Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions,
                        CollectionPermissions>)e.Result;
                    runStatus = (ContinuousCollectorRunStatus)returnedResult.Item1;

                    if (returnedResult.Item2 != MinimumPermissions.None &&
                        returnedResult.Item3 != MetadataPermissions.None &&
                        returnedResult.Item4 != CollectionPermissions.None)
                    {
                        minimumPermissions = returnedResult.Item2;
                        metadataPermissions = returnedResult.Item3;
                        collectionPermissions = returnedResult.Item4;
                    }
                }
                switch (runStatus)
                {
                    case ContinuousCollectorRunStatus.Running:
                        waitStatisticsStatus.Text = "Running";
                        stopWaitCollector.Visible = true;
                        stopWaitCollector.Enabled = true;
                        break;
                    case ContinuousCollectorRunStatus.Unknown:
                        waitStatisticsStatus.Text = "Unknown";
                        stopWaitCollector.Visible = false;
                        break;
                    default:
                        waitStatisticsStatus.Text = "Not Running";
                        stopWaitCollector.Visible = false;
                        break;
                }
                // Update Wait Monitoring Warning Label
                UpdateWaitMonitoringLabel();
                UpdateOLEAutomationLabel();
                UpdateActivityMonitorLabel();
                UpdateQueryMonitoringLabel();
            }
        }

        /// <summary>
        /// Update Wait Monitoring Label based on the permissions
        /// </summary>
        private void UpdateWaitMonitoringLabel()
        {
            try
            {
                // Permissions have been read
                if (minimumPermissions == MinimumPermissions.None &&
                    metadataPermissions == MetadataPermissions.None &&
                    collectionPermissions == CollectionPermissions.None)
                {
                    HideLabel(lblWaitMonitoringWarningLabel);
                    return;
                }

                var activeWaits = "Active Waits";
                var probeError = new ProbePermissionHelpers.ProbeError() { Name = activeWaits };
                // Wait Monitoring Warning Label after validating probe permissions
                if (refreshWaitCollectorStatus.Enabled && (chkCollectActiveWaits.Checked || chkUseXE.Checked || this.chkUseQs.Checked) &&
                    !ProbePermissionHelpers.ValidateProbePermissions(minimumPermissions, metadataPermissions,
                        collectionPermissions, serverVersion ?? configuration.MostRecentSQLVersion, activeWaits,
                        configuration.CloudProviderId, chkUseXE.Checked, this.chkUseQs.Checked, probeError))
                {
                    // Update Label
                    var warningText = this.chkUseQs.Checked
                                          ? WaitMonitoringWarningQs
                                          : chkUseXE.Checked
                                              ? WaitMonitoringWarningEx
                                              : WaitMonitoringWarningTrc;
                    if (!warningText.Equals(lblWaitMonitoringWarningLabel.Text))
                    {
                        lblWaitMonitoringWarningLabel.Text = warningText;
                    }

                    // Show lblWaitMonitoringWarningLabel if not visible
                    if (!lblWaitMonitoringWarningLabel.Visible)
                    {
                        lblWaitMonitoringWarningLabel.Visible = true;
                    }
                }
                else
                {
                    // Hide lblWaitMonitoringWarningLabel if visible
                    HideLabel(lblWaitMonitoringWarningLabel);
                }
            }
            catch (Exception exception)
            {
                HideLabel(lblWaitMonitoringWarningLabel);
                Log.Error("UpdateWaitMonitoringLabel: Exception occured " + exception);
            }
        }

        /// <summary>
        /// Update Activity Monitor Label based on the permissions
        /// </summary>
        private void UpdateOLEAutomationLabel()
        {
            try
            {

                // Permissions have been read
                if (minimumPermissions == MinimumPermissions.None &&
                    metadataPermissions == MetadataPermissions.None &&
                    collectionPermissions == CollectionPermissions.None)
                {
                    HideLabel(lblOSMetricOLEWarningLabel);
                    return;
                }

                var OSMetricsCollector = "OS Metrics";
                var probeError = new ProbePermissionHelpers.ProbeError() { Name = OSMetricsCollector };

                // Activity Monitor Warning Label after validating probe permissions
                if (optionWmiOleAutomation.Checked && !ProbePermissionHelpers.ValidateProbePermissions(minimumPermissions, metadataPermissions,
                        collectionPermissions, serverVersion ?? configuration.MostRecentSQLVersion, OSMetricsCollector,
                        configuration.CloudProviderId, optionWmiOleAutomation.Checked, probeError))
                {
                    // Show lblOSMetricOLEWarningLabel if not visible
                    if (!lblOSMetricOLEWarningLabel.Visible)
                    {
                        lblOSMetricOLEWarningLabel.Visible = true;
                    }
                }
                else
                {
                    // Hide lblActivityMonitorWarningLabel if visible
                    HideLabel(lblOSMetricOLEWarningLabel);
                }
            }
            catch (Exception exception)
            {
                HideLabel(lblOSMetricOLEWarningLabel);
                Log.Error("UpdateOLEAutomationLabel: Exception occured " + exception);
            }
        }

        private void UpdateActivityMonitorLabel()
        {
            try
            {
                // Permissions have been read
                if (minimumPermissions == MinimumPermissions.None &&
                    metadataPermissions == MetadataPermissions.None &&
                    collectionPermissions == CollectionPermissions.None)
                {
                    HideLabel(lblActivityMonitorWarningLabel);
                    return;
                }

                var activityCollector = "Activity Monitor";
                var probeError = new ProbePermissionHelpers.ProbeError() { Name = activityCollector };

                // Activity Monitor Warning Label after validating probe permissions
                if ((chkActivityMonitorEnable.Checked) && !ProbePermissionHelpers.ValidateProbePermissions(minimumPermissions, metadataPermissions,
                        collectionPermissions, serverVersion ?? configuration.MostRecentSQLVersion, activityCollector,
                        configuration.CloudProviderId, rButtonAMUseTrace.Checked, probeError))
                {
                    // Update Label
                    var warningText = rButtonAMUseExtendedEvents.Checked
                        ? ActivityMonitorWarningEx
                        : ActivityMonitorWarningTrc;
                    if (!warningText.Equals(lblActivityMonitorWarningLabel.Text))
                    {
                        lblActivityMonitorWarningLabel.Text = warningText;
                    }

                    // Show lblActivityMonitorWarningLabel if not visible
                    if (!lblActivityMonitorWarningLabel.Visible)
                    {
                        lblActivityMonitorWarningLabel.Visible = true;
                    }
                }
                else
                {
                    // Hide lblActivityMonitorWarningLabel if visible
                    HideLabel(lblActivityMonitorWarningLabel);
                }
            }
            catch (Exception exception)
            {
                HideLabel(lblActivityMonitorWarningLabel);
                Log.Error("UpdateActivityMonitorLabel: Exception occured " + exception);
            }
        }


        private void stopWaitCollector_Click(object sender, EventArgs e)
        {
            stopWaitCollector.Enabled = false;
            if (!stopWaitCollectorBackgroundWorker.IsBusy)
                stopWaitCollectorBackgroundWorker.RunWorkerAsync(id);
        }

        private void refreshWaitCollectorStatus_Click(object sender, EventArgs e)
        {
            refreshWaitCollectorStatus.Enabled = false;
            if (!waitCollectorStatusBackgroundWorker.IsBusy)
                waitCollectorStatusBackgroundWorker.RunWorkerAsync(id);
        }

        private void chkUseXE_CheckChanged(object sender, EventArgs e)
        {
            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            if (this.chkUseXE.Checked && this.chkUseQs.Checked)
            {
                this.chkUseQs.Checked = false;
            }
            if (!waitCollectorStatusBackgroundWorker.IsBusy)
            {
                waitCollectorStatusBackgroundWorker.RunWorkerAsync(id);
            }
        }

        private void chkUseQs_CheckChanged(object sender, EventArgs e)
        {
            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            if (this.chkUseQs.Checked && this.chkUseXE.Checked)
            {
                this.chkUseXE.Checked = false;
            }
            if (!waitCollectorStatusBackgroundWorker.IsBusy)
            {
                waitCollectorStatusBackgroundWorker.RunWorkerAsync(id);
            }
        }

        private void rButtonAMStatus_Changed(object sender, EventArgs e)
        {
            UpdateActivityMonitorLabel();
        }

        private void stopWaitCollectorBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                int MonitoredSQLServerID = (int)e.Argument;


                if (MonitoredSQLServerID > 0)
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    defaultManagementService.StopActiveWaitCollector(MonitoredSQLServerID);
                }

            }
            finally
            {
                BackgroundWorker backgroundWorker = sender as BackgroundWorker;

                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }

        }

        private void stopWaitCollectorBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (viewClosing)
                return;

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    stopWaitCollector.Enabled = true;
                    refreshWaitCollectorStatus.Enabled = false;
                    chkCollectActiveWaits.Checked = false;
                    // Added condition to check if Background worker is busy
                    if (!waitCollectorStatusBackgroundWorker.IsBusy)
                        waitCollectorStatusBackgroundWorker.RunWorkerAsync(id);
                }
                else
                {
                    ApplicationMessageBox.ShowError(this, "There was an error when stopping the collector.", e.Error);
                }
            }
        }

        /// <summary>
        /// Gets Server Permissions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverPermissionsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int MonitoredSQLServerID = (int)e.Argument;

                if (MonitoredSQLServerID > 0)
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    // To Skip Permissions Check for Cloud Providers
                    if (configuration.CloudProviderId == Common.Constants.MicrosoftAzureId ||
                        configuration.CloudProviderId == Common.Constants.AmazonRDSId)
                    {
                        e.Result = new Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions>(
                            MinimumPermissions.None, MetadataPermissions.None, CollectionPermissions.None);
                    }
                    else
                    {
                        // Read Server Permissions
                        e.Result = defaultManagementService.GetServerPermissions(MonitoredSQLServerID);
                    }
                }
                else
                {
                    e.Result = new Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions>(
                        MinimumPermissions.None, MetadataPermissions.None, CollectionPermissions.None);
                }
            }
            catch (Exception)
            {
                e.Result = null;
            }
            finally
            {
                BackgroundWorker backgroundWorker = sender as BackgroundWorker;

                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        private void serverDateTimeVersionBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int MonitoredSQLServerID = (int)e.Argument;

                if (MonitoredSQLServerID > 0)
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    e.Result = defaultManagementService.GetServerTimeAndVersion(MonitoredSQLServerID);
                }


            }
            catch (Exception)
            {
                e.Result = null;
            }
            finally
            {
                BackgroundWorker backgroundWorker = sender as BackgroundWorker;

                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Updates server permissions - <seealso cref="minimumPermissions"/>, <seealso cref="metadataPermissions"/> and <seealso cref="collectionPermissions"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverPermissionsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (viewClosing)
                return;

            if (!e.Cancelled)
            {
                if (e.Error == null && e.Result != null)
                {
                    Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions> result = (Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions>)e.Result;

                    if (result != null)
                    {
                        minimumPermissions = result.Item1;
                        metadataPermissions = result.Item2;
                        collectionPermissions = result.Item3;

                        // Update Wait Monitoring Warning Label
                        UpdateWaitMonitoringLabel();
                        UpdateQueryMonitoringLabel();
                        UpdateActivityMonitorLabel();
                        UpdateOLEAutomationLabel();
                    }
                }
            }
        }

        private void serverDateTimeVersionBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (viewClosing)
                return;

            if (!e.Cancelled)
            {
                if (e.Error == null && e.Result != null)
                {
                    Triple<ServerVersion, DateTime, DateTime> result = (Triple<ServerVersion, DateTime, DateTime>)e.Result;

                    if (result != null)
                    {
                        serverVersion = result.First;
                        serverLocalDateTime = result.Second;
                        collectionServiceLocalDateTime = result.Third;
                    }
                }

                if (serverVersion != null)
                {
                    if (serverVersion.Major <= 8)
                    {
                        chkCollectActiveWaits.Enabled = false;
                        chkCollectActiveWaits.Checked = false;
                        captureDeadlockCheckBox.Enabled = false;
                        captureDeadlockCheckBox.Checked = false;
                        chkCaptureBlocking.Enabled = false;
                        chkCaptureBlocking.Checked = false;
                        if (cloudProviderId == 2)
                            chkCaptureAutogrow.Enabled = false;
                        else
                        chkCaptureAutogrow.Enabled = true;
                    }
                    else
                    {
                        //server is 2005 or higher
                        chkCollectActiveWaits.Checked = configuration.ActiveWaitsConfiguration.Enabled;
                        captureDeadlockCheckBox.Checked = configuration.ActivityMonitorConfiguration.DeadlockEventsEnabled;
                        chkCaptureBlocking.Checked = configuration.ActivityMonitorConfiguration.BlockingEventsEnabled;

                        // SQLdm 10.4(Varun Chopra) query waits using Query Store
                        this.chkUseQs.Enabled = this.serverVersion.Major >= 14 || configuration.CloudProviderId ==
                                                Common.Constants.MicrosoftAzureManagedInstanceId ||
                                                configuration.CloudProviderId ==
                                                Common.Constants.MicrosoftAzureId;
                        this.chkUseQs.Checked = this.configuration.ActiveWaitsConfiguration.EnabledQs;

                        chkUseXE.Enabled = serverVersion.Major >= 11;
                        chkUseXE.Checked = !this.chkUseQs.Checked && configuration.ActiveWaitsConfiguration.EnabledXe;


                    }
                }
                else
                {
                    chkCollectActiveWaits.Checked = configuration.ActiveWaitsConfiguration.Enabled;
                    captureDeadlockCheckBox.Checked = configuration.ActivityMonitorConfiguration.DeadlockEventsEnabled;
                    chkCaptureBlocking.Checked = configuration.ActivityMonitorConfiguration.BlockingEventsEnabled;
                    chkCaptureAutogrow.Checked = configuration.ActivityMonitorConfiguration.AutoGrowEventsEnabled;
                    //let the checkbox reflect the configged value
                    // SQLdm 10.4(Varun Chopra) query waits using Query Store
                    this.chkUseQs.Checked = this.configuration.ActiveWaitsConfiguration.EnabledQs;
                    chkUseXE.Checked = this.chkUseQs.Checked && configuration.ActiveWaitsConfiguration.EnabledXe;
                }

                if (serverLocalDateTime > DateTime.MinValue)
                {
                    waitStatisticsServerDateTime.Text = serverLocalDateTime.ToShortDateString() + ' ' + serverLocalDateTime.ToShortTimeString();
                }
                else
                {
                    waitStatisticsServerDateTime.Text = "Unknown";
                }

                if (collectionServiceLocalDateTime > DateTime.MinValue)
                {
                    mmServerDateTime.Text = collectionServiceLocalDateTime.ToShortDateString() + ' ' + collectionServiceLocalDateTime.ToShortTimeString();
                }
                else
                {
                    mmServerDateTime.Text = "Unknown";
                }

                InitializeWaitsPage();
            }
        }


        private void waitStatisticsFilterButton_Click(object sender, EventArgs e)
        {
            if (configuration.ActiveWaitsConfiguration == null)
            {
                configuration.ActiveWaitsConfiguration = new ActiveWaitsConfiguration(id);
            }

            AdvancedQueryFilterConfigurationDialog dialog =
                new AdvancedQueryFilterConfigurationDialog(
                    configuration.ActiveWaitsConfiguration.AdvancedConfiguration, "Query Wait Statistics");
            dialog.ShowDialog(this);
        }



        private void ScheduledQueryWaits_CheckedChanged(object sender, EventArgs e)
        {
            if (ScheduledQueryWaits.Checked)
            {
                PerpetualQueryWaits.Checked = false;
                ShowHideWaitSchedulePanel();
            }
        }

        private void PerpetualQueryWaits_CheckedChanged(object sender, EventArgs e)
        {
            if (PerpetualQueryWaits.Checked)
            {
                ScheduledQueryWaits.Checked = false;
                ShowHideWaitSchedulePanel();
            }
        }



        private void btnVMConfig_Click(object sender, EventArgs e)
        {
            try
            {
                using (VirtualizationConfig virtualizationConfig = new VirtualizationConfig(true))
                {
                    if (configuration.VirtualizationConfiguration != null)
                    {
                        virtualizationConfig.AddLinkedServer(this.id,
                                                            configuration.InstanceName,
                                                            configuration.VirtualizationConfiguration.VCHostID,
                                                            configuration.VirtualizationConfiguration.VCAddress,
                                                            configuration.VirtualizationConfiguration.VCName,
                                                            configuration.VirtualizationConfiguration.InstanceUUID,
                                                            configuration.VirtualizationConfiguration.VMName,
                                                            configuration.VirtualizationConfiguration.VMDomainName);
                    }
                    else
                    {
                        virtualizationConfig.AddServer(this.id, configuration.InstanceName);
                    }

                    if (DialogResult.OK == virtualizationConfig.ShowDialog(this))
                    {
                        configuration.VirtualizationConfiguration =
                            virtualizationConfig.GetVirtualizationConfig(configuration.InstanceName);

                        InitializeVMPage();
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Error in the Virtualization Configuration Dialog", ex);
            }
        }

        private void optionWmiChanged(object sender, EventArgs e)
        {
            var directEnabled = optionWmiDirect.Checked;
            optionWmiCSCreds.Enabled = directEnabled;

            if (directEnabled)
                directEnabled = !optionWmiCSCreds.Checked;

            directWmiPassword.Enabled = directWmiUserName.Enabled = directEnabled;

            wmiTestButton.Visible = id > 0;

            if (wmiTestButton.Text.Equals("Connecting")) return;

            wmiTestButton.Enabled = !optionWmiNone.Checked;
            if (directEnabled && !optionWmiCSCreds.Checked)
            {
                if (String.IsNullOrEmpty(directWmiPassword.Text) || String.IsNullOrEmpty(directWmiPassword.Text))
                    wmiTestButton.Enabled = false;
            }
            UpdateOLEAutomationLabel();
        }

        private BackgroundWorker wmiTestBackgroundWorker;
        private void wmiTestButton_Click(object sender, EventArgs e)
        {
            wmiTestButton.Enabled = false;
            wmiTestButton.Text = "Connecting";

            var wmi = new WmiConfiguration();
            wmi.OleAutomationDisabled = !optionWmiOleAutomation.Checked;
            wmi.DirectWmiEnabled = optionWmiDirect.Checked;
            wmi.DirectWmiConnectAsCollectionService = optionWmiCSCreds.Checked;
            wmi.DirectWmiUserName = directWmiUserName.Text;
            wmi.DirectWmiPassword = directWmiPassword.Text;

            wmiTestBackgroundWorker = new BackgroundWorker();
            wmiTestBackgroundWorker.WorkerSupportsCancellation = true;
            wmiTestBackgroundWorker.DoWork += wmiTestBackgroundWorker_DoWork;
            wmiTestBackgroundWorker.RunWorkerCompleted += wmiTestBackgroundWorker_RunWorkerCompleted;
            wmiTestBackgroundWorker.RunWorkerAsync(wmi);
        }

        void wmiTestBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var wmi = e.Argument as WmiConfiguration;
                if (wmi == null) throw new ArgumentException("Wmi test worker requires a WmiConfiguration object for the argument");

                var managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                var result = managementService.SendWmiConfigurationTest(new TestWmiConfiguration(this.id, wmi));
                e.Result = (WmiConfigurationTestSnapshot)result;
            }
            finally
            {
                BackgroundWorker backgroundWorker = sender as BackgroundWorker;

                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        void wmiTestBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (viewClosing) return;

                if (e.Cancelled) return;
                if (e.Error != null)
                {
                    ApplicationMessageBox.ShowError(this, "The connection test failed.", e.Error);
                    return;
                }
                var result = (WmiConfigurationTestSnapshot)e.Result;
                if (result.CollectionFailed)
                {
                    ApplicationMessageBox.ShowError(this, "The connection test failed.", result.Error);
                    return;
                }

                var warn = result.CollectedValue == null;
                var passfail = String.Format("The connection test passed{0}",
                    !warn ? "." : " but failed to retrieve a value.  This could be due to an internal problem in the WMI service or an authorization problem with the user used to connect to the WMI service.");

                var message = result.DirectWmiStatus ?? result.OleAutomationStatus;
                if (message != null)
                {
                    warn = true;
                    passfail = passfail + "\n\n" + message;
                }
                if (warn)
                    ApplicationMessageBox.ShowWarning(this, passfail);
                else
                    ApplicationMessageBox.ShowInfo(this, passfail);
            }
            finally
            {
                if (!viewClosing)
                {
                    // put the test button back 
                    wmiTestButton.Text = "Test";
                    optionWmiChanged(this, EventArgs.Empty);
                }
            }
        }

        private void blockedProcessThresholdBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                var MonitoredSQLServerID = (int)e.Argument;


                if (MonitoredSQLServerID > 0)
                {
                    var configuration = new OnDemandConfiguration(MonitoredSQLServerID);

                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    e.Result = (Snapshot)defaultManagementService.GetConfiguration(configuration);
                }
            }
            finally
            {
                BackgroundWorker backgroundWorker = sender as BackgroundWorker;

                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }

        }

        private void blockedProcessThresholdBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e == null || e.Cancelled) return;

            var data = e.Result;

            if (data != null && data is ConfigurationSnapshot)
            {
                ConfigurationSnapshot snapshot = data as ConfigurationSnapshot;

                if (snapshot.Error == null)
                {
                    string settingName =
                        "blocked process threshold" +
                        (snapshot.ProductVersion != null && snapshot.ProductVersion.Major >= 10 ? " (s)" : "");
                    DataRow[] blockedDataTable =
                        snapshot.ConfigurationSettings.Select(
                            string.Format("Name = '{0}'", settingName));

                    if (blockedDataTable.Length > 0)
                    {
                        captureDeadlockCheckBox.Enabled =
                            chkCaptureBlocking.Enabled = chkActivityMonitorEnable.Checked;
                        chkCaptureAutogrow.Enabled = (cloudProviderId == 2 ? false: chkActivityMonitorEnable.Checked);
                        lblBlockedProcessSpinner.Enabled =
                            spnBlockedProcessThreshold.Enabled =
                            chkCaptureBlocking.Checked && chkCaptureBlocking.Enabled;

                        int.TryParse(blockedDataTable[0]["Run Value"].ToString(),
                                        out blockedProcessThreshold
                                    );

                        //if the servers blocked process threshold is less than our minimum
                        spnBlockedProcessThreshold.Value =
                            blockedProcessThreshold <= spnBlockedProcessThreshold.Minimum ?
                                spnBlockedProcessThreshold.Minimum : blockedProcessThreshold;

                        //Assign the value fetched from the Server to the local configuration object also to keep it in sync with the
                        //spin control value.
                        configuration.ActivityMonitorConfiguration.BlockedProcessThreshold = (int)spnBlockedProcessThreshold.Value;
                    }
                    else
                    {
                        spnBlockedProcessThreshold.Enabled = false;
                        chkCaptureBlocking.Enabled = false;
                        chkCaptureBlocking.Checked = false;
                        lblBlockedProcessSpinner.Enabled = false;
                    }
                }
                else
                {
                    blockedProcessThreshold = ApplicationModel.Default.ActiveInstances[id].
                        Instance.ActivityMonitorConfiguration.BlockedProcessThreshold;

                    spnBlockedProcessThreshold.Value = blockedProcessThreshold;
                }
            }
        }

        private void chkCaptureBlocking_CheckedChanged(object sender, EventArgs e)
        {
            spnBlockedProcessThreshold.Enabled = chkCaptureBlocking.Checked;
            lblBlockedProcessSpinner.Enabled = spnBlockedProcessThreshold.Enabled;

            //Setting the value for "Blocked Process Threshold" for default value
            if (chkCaptureBlocking.Checked && spnBlockedProcessThreshold.Value == 0)
            {
                spnBlockedProcessThreshold.Value = 30;
            }
        }

        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- Configuring evnts for new controls
        private void chkCollectQueryPlans_CheckedChanged(object sender, EventArgs e)
        {
            //SQLdm 10.0 (Tarun Sapra): Display Estimated Query Plan- If the check for the actual query plan is checked, then uncheck the estimated one automatically
            if (chkCollectQueryPlans.Checked)
            {
                chkCollectEstimatedQueryPlans.Checked = false;
            }
            UpdateQueryMonitoringLabel();
        }

        //SQLdm 10.0 (Tarun Sapra): Display Estimated Query Plan- If the check for the estimated query plan is checked, then uncheck the actual one automatically
        private void chkCollectEstimatedQueryPlans_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCollectEstimatedQueryPlans.Checked)
            {
                chkCollectQueryPlans.Checked = false;
            }
            UpdateQueryMonitoringLabel();
        }

        private void rButtonClicked(object sender, System.EventArgs e)
        {

            if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId != Common.Constants.AmazonRDSId)
            {
                //SQLdm 9.1 (Ankit Srivastava) -- Fixed Open functional isssue - disabling query plans option in case of trace monitoring 
                // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
                chkCollectQueryPlans.Enabled = (rButtonUseExtendedEvents.Checked || this.rButtonUseQueryStore.Checked);
                //SQLdm 10.0 (Tarun Sapra)- If the trace monitoring is enabled, then this checkbox would remain disabled 
                chkCollectEstimatedQueryPlans.Enabled = (rButtonUseExtendedEvents.Checked || this.rButtonUseQueryStore.Checked);
                this.captureSqlBatchesCheckBox.Enabled = !this.rButtonUseQueryStore.Checked;
            }
            UpdateQueryMonitoringLabel();

            //START - SQLdm 10.4 (Nikhil Bansal) - Top X Query Plan Filter
            // To Enable the Top Plan Filter only of the Extended Events Radio Button is checked and enabled
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            bool enabled = ((this.rButtonUseQueryStore.Checked && this.rButtonUseQueryStore.Enabled) || (rButtonUseExtendedEvents.Checked && rButtonUseExtendedEvents.Enabled));

            // Top Plan
            this.topPlanLabel.Enabled = enabled;
            this.topPlanSpinner.Enabled = enabled;
            this.topPlanSuffixLabel.Enabled = enabled;
            this.topPlanComboBox.Enabled = enabled;
            this.topPlanTableLayoutPanel.Enabled = enabled;
            //END - SQLdm 10.4 (Nikhil Bansal) - Top X Query Plan Filter

        }

        private void chkActivityMonitorEnable_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkActivityMonitorEnable.Checked;
            queryMonitorAdvancedOptionsButton.Enabled = enableQueryMonitorTraceCheckBox.Checked;
            if(cloudProviderId==2)
                chkCaptureAutogrow.Enabled = false;
            else
                chkCaptureAutogrow.Enabled = enabled;

            if (serverVersion != null)
            {
                if (serverVersion.Major <= 8)
                {
                    captureDeadlockCheckBox.Enabled = false;
                    captureDeadlockCheckBox.Checked = false;
                    chkCaptureBlocking.Enabled = false;
                    chkCaptureBlocking.Checked = false;
                    spnBlockedProcessThreshold.Enabled = false;
                    lblBlockedProcessSpinner.Enabled = false;
                }
                else
                {
                    captureDeadlockCheckBox.Enabled = enabled;
                    chkCaptureBlocking.Enabled = enabled;
                    spnBlockedProcessThreshold.Enabled = enabled
                        && chkCaptureBlocking.Checked;
                    lblBlockedProcessSpinner.Enabled = spnBlockedProcessThreshold.Enabled;
                }
            }
            else
            {
                captureDeadlockCheckBox.Enabled = enabled;
                chkCaptureBlocking.Enabled = enabled;
                spnBlockedProcessThreshold.Enabled = enabled;
                lblBlockedProcessSpinner.Enabled = enabled;
            }

            //SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- Configuring newly added section on load
            ConfigureHowToCollectActivityDataSection();
            // Update Activity Monitor Label
            UpdateActivityMonitorLabel();
        }

        private void tableLayoutPanel15_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// This event is when ever the user change the value for "Bloqued Process Threshold". 
        /// If  the user set this to 0, unchecked the "Capture Blocking" checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spnBlockedProcessThreshold_TextChanged(object sender, System.EventArgs e)
        {
            if (spnBlockedProcessThreshold.Value == 0)
            {
                chkCaptureBlocking.Checked = false;
            }
        }

        private void InitializeComboBox()
        {
            DataTable dt1 = new DataTable();
            DataColumn displayColumn1 = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn valueColumn1 = new DataColumn("Value", Type.GetType("System.Int32"));
            dt1.Columns.Add(displayColumn1);
            dt1.Columns.Add(valueColumn1);

            for (int i = 1; i <= 4; i++)
            {
                DataRow row = dt1.NewRow();

                if (i == 1)
                    row["Name"] = "First";
                if (i == 2)
                    row["Name"] = "Second";
                if (i == 3)
                    row["Name"] = "Third";
                if (i == 4)
                    row["Name"] = "Fourth";

                row["Value"] = i;
                dt1.Rows.Add(row);
            }
            WeekcomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            WeekcomboBox.BindingContext = new BindingContext();
            WeekcomboBox.DataSource = dt1;
            WeekcomboBox.DisplayMember = "Name";
            WeekcomboBox.ValueMember = "Value";
            WeekcomboBox.SelectedValue = 1;

            DataTable dt2 = new DataTable();
            DataColumn displayColumn2 = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn valueColumn2 = new DataColumn("Value", Type.GetType("System.Int32"));
            dt2.Columns.Add(displayColumn2);
            dt2.Columns.Add(valueColumn2);

            for (int i = 1; i <= 7; i++)
            {
                DataRow row = dt2.NewRow();

                if (i == 1)
                    row["Name"] = "Sunday";
                if (i == 2)
                    row["Name"] = "Monday";
                if (i == 3)
                    row["Name"] = "Tuesday";
                if (i == 4)
                    row["Name"] = "Wednesday";
                if (i == 5)
                    row["Name"] = "Thursday";
                if (i == 6)
                    row["Name"] = "Friday";
                if (i == 7)
                    row["Name"] = "Saturday";

                row["Value"] = i;
                dt2.Rows.Add(row);
            }
            DaycomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DaycomboBox.BindingContext = new BindingContext();
            DaycomboBox.DataSource = dt2;
            DaycomboBox.DisplayMember = "Name";
            DaycomboBox.ValueMember = "Value";
            DaycomboBox.SelectedValue = 1;

        }
        private void ShowHidePageTabByInstanceId()
        {
            if (configuration.CloudProviderId.HasValue && configuration.CloudProviderId == Common.Constants.AmazonRDSId)
            {
                //Remove tab in specific instance type.
                // propertiesControl.PropertyPages.Remove(activeMonitorPropertyPage);
                //propertiesControl.PropertyPages.Remove(replicationPropertyPage);
                // propertiesControl.PropertyPages.Remove(tableStatisticsPropertyPage);
                propertiesControl.PropertyPages.Remove(osPropertyPage);
                propertiesControl.PropertyPages.Remove(diskPropertyPage);
                propertiesControl.PropertyPages.Remove(clusterPropertyPage);
                //propertiesControl.PropertyPages.Remove(waitPropertyPage);
                propertiesControl.PropertyPages.Remove(virtualizationPropertyPage);
                propertiesControl.PropertyPages.Remove(analysisConfigurationPropertyPage);
                propertiesControl.PropertyPages.Remove(replicationPropertyPage);
                //Select first tab as default after removing the tabs.
                propertiesControl.SelectedPropertyPageIndex = (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.Popular;
            }
            else if (configuration.CloudProviderId.HasValue &&
                     (configuration.CloudProviderId == Common.Constants.MicrosoftAzureId ||
                      configuration.CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
            {
                //Remove tab in specific instance type.
                // propertiesControl.PropertyPages.Remove(queryMonitorPropertyPage); //SQLdm 11.0 Azure Query Monitor
               // propertiesControl.PropertyPages.Remove(activeMonitorPropertyPage);
                propertiesControl.PropertyPages.Remove(replicationPropertyPage);
                propertiesControl.PropertyPages.Remove(tableStatisticsPropertyPage);
                propertiesControl.PropertyPages.Remove(osPropertyPage);
                propertiesControl.PropertyPages.Remove(diskPropertyPage);
                propertiesControl.PropertyPages.Remove(clusterPropertyPage);
                //propertiesControl.PropertyPages.Remove(waitPropertyPage);
                propertiesControl.PropertyPages.Remove(virtualizationPropertyPage);
                //Select first tab as default after removing the tabs.
                propertiesControl.SelectedPropertyPageIndex = (int)MonitoredSqlServerInstancePropertiesDialogPropertyPages.Popular;
            }
        }
        private void cmbServerType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cmbServerType.SelectedIndex != 0)
            {
                configuration.CloudProviderId = ((KeyValuePair<string, int>)cmbServerType.SelectedItem).Value;
            }
        }
    }
}
