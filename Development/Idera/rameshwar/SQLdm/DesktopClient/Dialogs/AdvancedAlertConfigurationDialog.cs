using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.Text;
    using Common;
    using Controls;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Infragistics.Win.UltraWinTabControl;

    public partial class AdvancedAlertConfigurationDialog : Form
    {
        private readonly int instanceId;
        private readonly AdvancedAlertConfigurationSettings alertSettings;
        private readonly bool isDefaultAlertConfiguration;
        private readonly bool isDefaultInstanceThreshold;
        private bool settingsChanged;
        private bool readOnly;
        private DataGridView jobRules;
        private Set<string> visibleControls = new Set<string>();
        private const string FILTER_EXISTS = "This filter already exists as an {0} filter";
        private const string EXCLUDE = "exclude";
        private const string INCLUDE = "include";
        private readonly string thresholdInstanceName;   // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --threshold instance name (database name) for per database filegroup filters
        Type checknumerictype;
        public AdvancedAlertConfigurationDialog(
            int instanceId, string alertName, AdvancedAlertConfigurationSettings alertSettings, bool isDefaultAlertConfiguration, bool isDefaultInstanceThreshold, Type checktype)
        {
            InitializeComponent();
            tabControl.DrawFilter = new HideFocusRectangleDrawFilter();
            this.checknumerictype = checktype;
            this.instanceId = instanceId;
            Text = string.Format(Text, alertName);
            this.alertSettings = alertSettings;
            this.isDefaultAlertConfiguration = isDefaultAlertConfiguration;
            this.isDefaultInstanceThreshold = isDefaultInstanceThreshold;

            readOnly = !(ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify);
            okButton.DialogResult = readOnly ? DialogResult.Cancel : DialogResult.OK;

            ConfigureAlertSuppressionTab();
            ConfigureAlertFiltersTab();
            ConfigureCollectionFailuresTab();
            ConfigureAutogrowSettingsTab();
            ConfigureJobFiltersTab();
            AdaptFontSize();
        }

        // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --constructor overloading with an additional parameter -> threshold instance name (database name) for per database filegroup filters
        public AdvancedAlertConfigurationDialog(
            int instanceId, string alertName, AdvancedAlertConfigurationSettings alertSettings, bool isDefaultAlertConfiguration, bool isDefaultInstanceThreshold, string thresholdInstanceName, Type checktype)
            : this(instanceId, alertName, alertSettings, isDefaultAlertConfiguration, isDefaultInstanceThreshold, checktype)
        {
            this.thresholdInstanceName = thresholdInstanceName;
        }

        private void ConfigureAlertSuppressionTab()
        {
            int index = 0;
            smoothingContentPropertyPage.Enabled = !readOnly;
            switch (alertSettings.Metric)
            {
                case Metric.ReorganisationPct:
                case Metric.BlockingAlert:
                case Metric.OldestOpenTransMinutes:
                case Metric.ErrorLog:
                case Metric.AgentLog:
                case Metric.OldestOpenTransactionMinutesBaseline:

                    tabControl.Tabs["alertSuppressionTab"].Visible = false;
                    break;
                case Metric.LongJobs:
                case Metric.LongJobsMinutes:
                    tabControl.Tabs["alertSuppressionTab"].Text = "Alert Duration";
                    timeSuppressionPanel.Visible = false;
                    jobAlertActivePanel.Visible = true;
                    jobAlertActiveTimeHeaderStrip.Text = String.Format(jobAlertActiveTimeHeaderStrip.Text, "long running job");
                    UpdateActiveDurationDayTime();
                    break;
                case Metric.BombedJobs:
                    tabControl.Tabs["alertSuppressionTab"].Text = "Alert Duration";
                    timeSuppressionPanel.Visible = false;
                    jobFailureSuppressionPanel.Visible = true;
                    raiseJobFailureAlertLimitedRadioButton.Checked = !alertSettings.TreatSingleJobFailureAsCritical;
                    jobAlertActivePanel.Visible = true;
                    jobAlertActiveTimeHeaderStrip.Text = String.Format(jobAlertActiveTimeHeaderStrip.Text, "job failure");
                    UpdateActiveDurationDayTime();
                    break;
                case Metric.JobCompletion:
                    tabControl.Tabs["alertSuppressionTab"].Text = "Alert Duration";
                    timeSuppressionPanel.Visible = false;
                    jobFailureSuppressionPanel.Visible = false;
                    raiseJobFailureAlertLimitedRadioButton.Checked = !alertSettings.TreatSingleJobFailureAsCritical;
                    jobAlertActivePanel.Visible = true;
                    jobAlertActiveTimeHeaderStrip.Text = String.Format(jobAlertActiveTimeHeaderStrip.Text, "job/step completion");
                    UpdateActiveDurationDayTime();
                    break;
                case Metric.VmHostServerChange:
                    tabControl.Tabs["alertSuppressionTab"].Text = "Alert Duration";
                    timeSuppressionPanel.Visible =
                        jobFailureSuppressionPanel.Visible =
                        jobAlertActivePanel.Visible = false;
                    agingAlertActivePanel.Visible = true;
                    agingAlertActiveTimeHeaderStrip.Text = String.Format(agingAlertActiveTimeHeaderStrip.Text,
                                                                         " VM host server change");
                    agingAlertActiveDuration.Time = TimeSpan.FromMinutes(alertSettings.NumberMinutesAgingAlertsAreActive);
                    break;
                case Metric.VmConfigChange:
                    tabControl.Tabs["alertSuppressionTab"].Text = "Alert Duration";
                    timeSuppressionPanel.Visible =
                        jobFailureSuppressionPanel.Visible =
                        jobAlertActivePanel.Visible = false;
                    agingAlertActivePanel.Visible = true;
                    agingAlertActiveTimeHeaderStrip.Text = String.Format(agingAlertActiveTimeHeaderStrip.Text,
                                                                         " VM configuration change");
                    agingAlertActiveDuration.Time = TimeSpan.FromMinutes(alertSettings.NumberMinutesAgingAlertsAreActive);
                    break;
                case Metric.SSBackupOperation:
                case Metric.SSRestoreOperation:
                case Metric.SSDefragOperation:
                    tabControl.Tabs["alertSuppressionTab"].Text = "Alert Duration";
                    timeSuppressionPanel.Visible =
                        jobFailureSuppressionPanel.Visible =
                        jobAlertActivePanel.Visible = false;
                    agingAlertActivePanel.Visible = true;
                    agingAlertActiveTimeHeaderStrip.Text = String.Format(agingAlertActiveTimeHeaderStrip.Text,
                                                                         " SQLsafe operation");
                    agingAlertActiveDuration.Time = TimeSpan.FromMinutes(alertSettings.NumberMinutesAgingAlertsAreActive);
                    break;
                case Metric.MirroringSessionRoleChange:
                case Metric.ClusterFailover:
                    timeSuppressionPanel.Visible = false;
                    agingAlertActivePanel.Visible = true;
                    agingAlertActiveTimeHeaderStrip.Text = String.Format(agingAlertActiveTimeHeaderStrip.Text, alertSettings.Metric == Metric.MirroringSessionRoleChange ? "mirroring role change" : "cluster failover");
                    agingAlertActiveDuration.Time = TimeSpan.FromMinutes(alertSettings.NumberMinutesAgingAlertsAreActive);
                    break;
                case Metric.AlwaysOnAvailabilityGroupRoleChange:
                    // SQLDM - 26298 - Code changes to get the AgingAlertActiveDuration
                    tabControl.Tabs["alertSuppressionTab"].Text = "Alert Information";
                    jobFailureSuppressionPanel.Visible = jobAlertActivePanel.Visible = false;
                    timeSuppressionPanel.Visible = agingAlertActivePanel.Visible = true;
                    agingAlertActiveTimeHeaderStrip.Text = String.Format(agingAlertActiveTimeHeaderStrip.Text, "Availability Group Role change");
                    agingAlertActiveDuration.Time = TimeSpan.FromMinutes(alertSettings.NumberMinutesAgingAlertsAreActive);
                    suppressionMinutesSpinner.Value = alertSettings.SuppressionPeriodMinutes;
                    break;
                default:
                    suppressionMinutesSpinner.Value = alertSettings.SuppressionPeriodMinutes;
                    break;
            }
            if (alertSettings.ThresholdTime >= 1)
            {
                FilterOptionNumeric.Value = alertSettings.ThresholdTime;
                
                
            }
            
            if (alertSettings.ThresholdRefreshTime >= 1)
            {
                FilterOptionNumericOutOf.Value = alertSettings.ThresholdRefreshTime;
            }
        }

        private void UpdateActiveDurationDayTime()
        {
            suppressionDaysSpinner.Value = alertSettings.NumberMinutesJobAlertsAreActive / 1440;
            jobAlertActiveDuration.Time = TimeSpan.FromMinutes(alertSettings.NumberMinutesJobAlertsAreActive % 1440);
        }

        private void ConfigureAlertFiltersTab()
        {
            if (!isDefaultInstanceThreshold)
            {
                // START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --making alerts tab filter visible for filegroup alerts 
                if (alertSettings.Metric == Metric.FilegroupSpaceFullPct || alertSettings.Metric == Metric.FilegroupSpaceFullSize)
                {
                    tabControl.Tabs["alertFiltersTab"].Visible = true;
                    filegroupsExcludeFilterTextBox.Text = alertSettings.FilegroupExcludeToString();
                    filegroupsFilterPanel.Visible = true;
                    sessionExludeDMappFilterPanel.Visible = false;
                    databasesFilterPanel.Visible = false;
                    jobsFilterPanel.Visible = false;
                    jobCategoriesFilterPanel.Visible = false;
                    sessionApplicationsFilterPanel.Visible = false;
                    sessionHostServersFilterPanel.Visible = false;
                    sessionUsersFilterPanel.Visible = false;
                    logScanPanel.Visible = false;
                    logScanRegularExpressionPanel.Visible = false;
                    logSizeLimitPanel.Visible = false;
                    versionStorePanel.Visible = false;
                    versionStoreSizePanel.Visible = false;
                }
                else
                {
                    // END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --making alerts tab filter visible for filegroup alerts
                    // hide the Alert Filters tab if it is not the default instance threshold         
                    tabControl.Tabs["alertFiltersTab"].Visible = true;
                    //SQLDM-30443.
                    jobsFilterPanel.Visible = false;
                    jobCategoriesFilterPanel.Visible = false;
                    sessionApplicationsFilterPanel.Visible = false;
                    sessionHostServersFilterPanel.Visible = false;
                    sessionUsersFilterPanel.Visible = false; 
                    logScanPanel.Visible = false;
                    logScanRegularExpressionPanel.Visible = false;
                    logSizeLimitPanel.Visible = false;
                    versionStorePanel.Visible = false;
                    versionStoreSizePanel.Visible = false;

                }
                if (checknumerictype == typeof(double) || checknumerictype == typeof(float) || checknumerictype == typeof(int) || checknumerictype == typeof(Int64))
                {
                    FilterOptionPanel.Visible = true;
                    thresholdHeaderStrip.Visible = true;
                }
            }
            else
            { 
                // disable these browse buttons when editing the default alert configuration
                FilterOptionPanel.Visible = false;
                filterDrivesButton.Visible =
                filterDatabasesButton.Visible =
                filterTablesButton.Visible =
                filterJobsButton.Visible =
                filterJobCategoriesButton.Visible = !isDefaultAlertConfiguration;
                sessionExludeDMappFilterPanel.Visible = false;
                office2007PropertyPage7.Enabled = !readOnly;
                office2007PropertyPage7.Text = "Filter out objects for which you don't wish to receive alerts.";
                switch (alertSettings.Metric)
                {
                    case Metric.ReorganisationPct:
                        databasesExcludeFilterTextBox.Text = alertSettings.DatabaseExcludeToString();
                        databasesFilterPanel.Visible = true;
                        tablesExcludeFilterTextBox.Text = alertSettings.TableExcludeToString();
                        tablesFilterPanel.Visible = true;
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        sessionApplicationsFilterPanel.Visible = false;
                        sessionHostServersFilterPanel.Visible = false;
                        sessionUsersFilterPanel.Visible = false;
                        logScanPanel.Visible = false;
                        logScanRegularExpressionPanel.Visible = false;
                        logSizeLimitPanel.Visible = false;
                        versionStorePanel.Visible = false;
                        versionStoreSizePanel.Visible = false;
                        break;
                    case Metric.OSDiskAverageDiskQueueLengthPerDisk:
                    case Metric.OSDiskFull:
                    case Metric.OSDiskPhysicalDiskTimePctPerDisk:
                    case Metric.AverageDiskMillisecondsPerRead:
                    case Metric.AverageDiskMillisecondsPerTransfer:
                    case Metric.AverageDiskMillisecondsPerWrite:
                    case Metric.DiskReadsPerSecond:
                    case Metric.DiskTransfersPerSecond:
                    case Metric.DiskWritesPerSecond:
                    case Metric.OsDiskFreeMb:
                        drivesExcludeFilterTextBox.Text = alertSettings.DriveExcludeToString();
                        driveFilterPanel.Visible = true;
                        databasesFilterPanel.Visible = false;
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        sessionApplicationsFilterPanel.Visible = false;
                        sessionHostServersFilterPanel.Visible = false;
                        sessionUsersFilterPanel.Visible = false;
                        logScanPanel.Visible = false;
                        logScanRegularExpressionPanel.Visible = false;
                        logSizeLimitPanel.Visible = false;
                        versionStorePanel.Visible = false;
                        versionStoreSizePanel.Visible = false;
                        break;
                    //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --changed tabs settings for Filegroup space full metrices
                    case Metric.FilegroupSpaceFullPct:
                    case Metric.FilegroupSpaceFullSize:
                        filegroupsExcludeFilterTextBox.Text = alertSettings.FilegroupExcludeToString();
                        databasesExcludeFilterTextBox.Text = alertSettings.DatabaseExcludeToString();
                        filegroupsFilterPanel.Visible = true;
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        sessionApplicationsFilterPanel.Visible = false;
                        sessionHostServersFilterPanel.Visible = false;
                        sessionUsersFilterPanel.Visible = false;
                        logScanPanel.Visible = false;
                        logScanRegularExpressionPanel.Visible = false;
                        logSizeLimitPanel.Visible = false;
                        versionStorePanel.Visible = false;
                        versionStoreSizePanel.Visible = false;
                        break;
                    //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --changed tabs settings for Filegroup space full metrices
                    case Metric.DatabaseFileSize:
                    case Metric.DatabaseSizePct:
                    case Metric.DatabaseStatus:
                    case Metric.TransLogSize:
                    case Metric.DataFileAutogrow:
                    case Metric.LogFileAutogrow:
                    case Metric.DatabaseSizeMb:
                    case Metric.TransLogSizeMb:
                        databasesExcludeFilterTextBox.Text = alertSettings.DatabaseExcludeToString();
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        sessionApplicationsFilterPanel.Visible = false;
                        sessionHostServersFilterPanel.Visible = false;
                        sessionUsersFilterPanel.Visible = false;
                        logScanPanel.Visible = false;
                        logScanRegularExpressionPanel.Visible = false;
                        logSizeLimitPanel.Visible = false;
                        versionStorePanel.Visible = false;
                        versionStoreSizePanel.Visible = false;
                        break;
                    //case Metric.BombedJobs:
                    //case Metric.LongJobs:
                    //case Metric.LongJobsMinutes:
                    //    jobsExcludeFilterTextBox.Text = alertSettings.JobsExcludeToString();
                    //    jobCategoriesExcludeFilterTextBox.Text = alertSettings.JobCategoriesExcludeToString();
                    //    databasesFilterPanel.Visible = false;
                    //    sessionApplicationsFilterPanel.Visible = false;
                    //    sessionHostServersFilterPanel.Visible = false;
                    //    sessionUsersFilterPanel.Visible = false;
                    //    logScanPanel.Visible = false;
                    //    logScanRegularExpressionPanel.Visible = false;
                    //    break;
                    case Metric.BlockingAlert:
                    case Metric.OldestOpenTransMinutes:
                    case Metric.ResourceAlert:
                    case Metric.DatabaseLastBackupDate:
                    case Metric.SessionTempdbSpaceUsage:
                        sessionApplicationsExcludeFilterTextBox.Text = alertSettings.ApplicationsExcludeToString();
                        sessionHostServersExcludeFilterTextBox.Text = alertSettings.HostServersExcludeToString();
                        sessionUsersExcludeFilterTextBox.Text = alertSettings.UsersExcludeToString();
                        databasesFilterPanel.Visible = false;
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        logScanPanel.Visible = false;
                        logScanRegularExpressionPanel.Visible = false;
                        logSizeLimitPanel.Visible = false;
                        versionStorePanel.Visible = false;
                        versionStoreSizePanel.Visible = false;
                        if (alertSettings.Metric == Metric.ResourceAlert)
                        {
                            sessionExludeDMappFilterPanel.Visible = true;
                            excludeDMapplicationCheckBox.Checked = alertSettings.ExcludeSqldmApp;
                        }
                        // For DatabaseLastBackupDate to just exclude data bases
                        if (alertSettings.Metric == Metric.DatabaseLastBackupDate)
                        {
                            databasesExcludeFilterTextBox.Text = alertSettings.DatabaseExcludeToString();
                            sessionApplicationsFilterPanel.Visible = false;
                            sessionHostServersFilterPanel.Visible = false;
                            sessionUsersFilterPanel.Visible = false;
                            databasesFilterPanel.Visible = true;
                            thresholdHeaderStrip.Visible = true;
							//SQLDM-29210 -- Add option to include alerts for Read-only databases.
                            readOnlyDatabaseIncludePanel.Visible = true;
                            // SQLDM-29410 -- Add availability group alert options.
                            availabilityGroupBackupAlertPanel.Visible = true;
                            availabilityGroupPrimaryOption.Visible = true;
                            availabilityGroupSecondaryOption.Visible = true;
                            availabilityGroupDefaultOption.Visible = true;
                            availabilityGroupBothOption.Visible = true;

                            replicaCollectionOptionPanel.Visible = true;
                            replicaCollectionMonitoredOnlyOption.Visible = true;
                            replicaCollectionAllOption.Visible = true;

                            switch (alertSettings.AvailabilityGroupOption)
                            {
                                case "Primary": availabilityGroupPrimaryOption.Checked = true;
                                    break;

                                case "Secondary": availabilityGroupSecondaryOption.Checked = true;
                                    break;

                                case "Default": availabilityGroupDefaultOption.Checked = true;
                                    break;

                                case "Both": 
                                default: availabilityGroupBothOption.Checked = true;
                                    break;
                            }

                            switch (alertSettings.ReplicaCollectionOption)
                            {
                                case "MonitoredOnly": replicaCollectionMonitoredOnlyOption.Checked = true;
                                    break;

                                case "All":
                                default:
                                    replicaCollectionAllOption.Checked = true;
                                    break;

                                
                            }

                            includeReadOnlyDatabases.Checked = alertSettings.IncludeReadOnly;
                        }
                        break;
                    case Metric.BlockedSessions:
                        sessionExludeDMappFilterPanel.Visible = true;
                        excludeDMapplicationCheckBox.Checked = alertSettings.ExcludeSqldmApp;
                        driveFilterPanel.Visible = false;
                        sessionApplicationsFilterPanel.Visible = false;
                        sessionHostServersFilterPanel.Visible = false;
                        sessionUsersFilterPanel.Visible = false;
                        logScanPanel.Visible = false;
                        logScanRegularExpressionPanel.Visible = false;
                        logSizeLimitPanel.Visible = false;
                        databasesFilterPanel.Visible = false;
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        versionStorePanel.Visible = false;
                        versionStoreSizePanel.Visible = false;
                        break;
                    case Metric.ErrorLog:
                    case Metric.AgentLog:
                        logScanIncludeTextBoxCritical.Text = alertSettings.LogIncludeCriticalToString();
                        logScanIncludeTextBoxWarning.Text = alertSettings.LogIncludeWarningToString();
                        logScanIncludeTextBoxInfo.Text = alertSettings.LogIncludeInfoToString();
                        FilterOptionPanel.Visible = true;
                        FilterOptionPanel.Size = new System.Drawing.Size(511, 85);
                        thresholdHeaderStrip.Size = new System.Drawing.Size(511, 25);
                        thresholdHeaderStrip.Visible = true;
                        logScanPanel.Visible = true;
                        logScanIncludeTextBoxRegexCritical.Text = alertSettings.LogRegexIncludeCriticalToString();
                        logScanIncludeTextBoxRegexWarning.Text = alertSettings.LogRegexIncludeWarningToString();
                        logScanIncludeTextBoxRegexInfo.Text = alertSettings.LogRegexIncludeInfoToString();
                        logScanRegularExpressionPanel.Visible = true;
                        spnLogSize.Value = alertSettings.LogSizeLimit / 1024;
                        logSizeLimitPanel.Visible = true;
                        databasesFilterPanel.Visible = false;
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        sessionApplicationsFilterPanel.Visible = false;
                        sessionHostServersFilterPanel.Visible = false;
                        sessionUsersFilterPanel.Visible = false;
                        office2007PropertyPage7.Text = "Specify text or size limits for which you wish to receive alerts.";
                        versionStorePanel.Visible = false;
                        versionStoreSizePanel.Visible = false;
                        break;
                    case Metric.VersionStoreGenerationRatio:
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        sessionApplicationsFilterPanel.Visible = false;
                        sessionHostServersFilterPanel.Visible = false;
                        sessionUsersFilterPanel.Visible = false;
                        logScanPanel.Visible = false;
                        logScanRegularExpressionPanel.Visible = false;
                        logSizeLimitPanel.Visible = false;
                        databasesFilterPanel.Visible = false;
                        versionStorePanel.Visible = true;
                        minVersionStoreGenerationRate.Value = alertSettings.MinGenerationRatePerSecond;
                        versionStoreSizePanel.Visible = true;
                        versionStoreSizeUpDown.Value = alertSettings.MinVersionStoreSize.Megabytes.HasValue
                                                           ? alertSettings.MinVersionStoreSize.Megabytes.Value
                                                           : 0;
                        break;
                    case Metric.LongRunningVersionStoreTransaction:
                        jobsFilterPanel.Visible = false;
                        jobCategoriesFilterPanel.Visible = false;
                        sessionApplicationsFilterPanel.Visible = false;
                        sessionHostServersFilterPanel.Visible = false;
                        sessionUsersFilterPanel.Visible = false;
                        logScanPanel.Visible = false;
                        logScanRegularExpressionPanel.Visible = false;
                        logSizeLimitPanel.Visible = false;
                        databasesFilterPanel.Visible = false;
                        versionStorePanel.Visible = false;
                        versionStoreSizePanel.Visible = true;
                        versionStoreSizeUpDown.Value = alertSettings.MinVersionStoreSize.Megabytes.HasValue
                                                           ? alertSettings.MinVersionStoreSize.Megabytes.Value
                                                           : 0;
                        break;
                    default:
                        if (checknumerictype == typeof(double) || checknumerictype == typeof(float) || checknumerictype == typeof(int) || checknumerictype == typeof(Int64))
                        {
                            //need to check with the theshold type is numeric(int+float)
                            sessionApplicationsExcludeFilterTextBox.Text = alertSettings.ApplicationsExcludeToString();
                            sessionHostServersExcludeFilterTextBox.Text = alertSettings.HostServersExcludeToString();
                            sessionUsersExcludeFilterTextBox.Text = alertSettings.UsersExcludeToString();
                            databasesFilterPanel.Visible = false;
                            jobsFilterPanel.Visible = false;
                            jobCategoriesFilterPanel.Visible = false;
                            logScanPanel.Visible = false;
                            logScanRegularExpressionPanel.Visible = false;
                            logSizeLimitPanel.Visible = false;
                            FilterOptionPanel.Visible = true;
                            FilterOptionPanel.Dock = DockStyle.Fill;
                            thresholdHeaderStrip.Visible = true;
                            versionStorePanel.Visible = false;
                            versionStoreSizeUpDown.Visible = false;
                            versionStoreSizeHeaderStrip.Visible = false;
                            versionStoreSizePanel.Visible = false;
                            sessionExludeDMappFilterPanel.Visible = false;
                            excludeDMapplicationCheckBox.Checked = false;
                            databasesExcludeFilterPanel.Visible = false;
                            databasesExcludeFilterTextBox.Text = alertSettings.DatabaseExcludeToString();
                            sessionApplicationsFilterPanel.Visible = false;
                            sessionHostServersFilterPanel.Visible = false;
                            sessionUsersFilterPanel.Visible = false;
                            databasesFilterPanel.Visible = false;
                            tabControl.Tabs["alertFiltersTab"].Visible = this.checkMetricSupportAdvancefilterTab((int)alertSettings.Metric); ;
                        }
                        else
                        {
                            tabControl.Tabs["alertFiltersTab"].Visible = false;
                        }
                        break;
                }
               
            }
        }

        private void ConfigureCollectionFailuresTab()
        {
            office2007PropertyPage5.Enabled = !readOnly;
            switch (alertSettings.Metric)
            {
                case Metric.Custom:
                    generateAlertButton.Checked = alertSettings.TreatCustomCounterFailureAsCritical;
                    skipAlertButton.Checked = !generateAlertButton.Checked;
                    break;
                default:
                    tabControl.Tabs["collectionFailuresTab"].Visible = false;
                    break;
            }
        }

        private void ConfigureAutogrowSettingsTab()
        {
            office2007PropertyPage8.Enabled = !readOnly;
            switch (alertSettings.Metric)
            {
                //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --changed tabs settings for Filegroup space full metrices
                case Metric.FilegroupSpaceFullPct:
                case Metric.DatabaseSizePct:
                case Metric.TransLogSize:
                    useAutogrow.Checked = alertSettings.IncludeAutogrowInDatabaseSizeCalc;
                    ignoreAutogrow.Checked = !alertSettings.IncludeAutogrowInDatabaseSizeCalc;
                    break;
                default:
                    tabControl.Tabs["autogrowSettingsTab"].Visible = false;
                    break;
            }
        }


        private void ConfigureJobFiltersTab()
        {
            office2007PropertyPage9.Enabled = !readOnly;

            switch (alertSettings.Metric)
            {
                case Metric.BombedJobs:
                case Metric.LongJobs:
                case Metric.LongJobsMinutes:
                case Metric.JobCompletion:
                    chkAlertOnJobSteps.Checked = alertSettings.AlertOnJobSteps;
                    gridInclude.Columns["includeStepOpCode"].Visible =
                        gridInclude.Columns["includeStepName"].Visible =
                        gridExclude.Columns["excludeStepOpCode"].Visible =
                        gridExclude.Columns["excludeStepName"].Visible = chkAlertOnJobSteps.Checked;

                    if (alertSettings.JobIncludeFilter != null)
                    {
                        foreach (JobFilter filter in alertSettings.JobIncludeFilter)
                        {
                            gridInclude.Rows.Add(ParseFilterInfo(filter));
                        }
                    }
                    if (alertSettings.JobExcludeFilter != null)
                    {
                        foreach (JobFilter filter in alertSettings.JobExcludeFilter)
                        {
                            gridExclude.Rows.Add(ParseFilterInfo(filter));
                        }
                    }
                    if (alertSettings.Metric == Metric.LongJobs)
                    {
                        this.minimalRuntimeUpDown.Value = alertSettings.MinSecondsRuntimeRunningJobByPercent;
                        minRuntimeJobsPanel.Visible = true;
                    }
                    break;
                default:
                    tabControl.Tabs["jobFiltersTab"].Visible = false;
                    minRuntimeJobsPanel.Visible = false;
                    break;
            }
        }

        private bool GetControlVisible(Control control)
        {
            return control.Visible || visibleControls.Contains(control.Name);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!readOnly)
            {
                ApplyFilterOptionChanges();
                ApplyAlertSuppressionChanges();
                ApplyDatabasesFilterChanges();
                ApplyFilegroupsFilterChanges();
                ApplyDriveFilterChanges();
                ApplyTablesFilterChanges();
                ApplyJobsFiltersChanges();
                ApplyJobCategoriesFilterChanges();
                ApplyJobMinRuntimeByPercent();
                ApplyApplicationsFilterChanges();
                ApplyHostServersFilterChanges();
                ApplyUsersFilterChanges();
                ApplyCollectionFailuresChanges();
                ApplyLogScanIncludeChanges();
                ApplyLogScanSizeLimitChanges();
                ApplyAutogrowthSettingsChanges();
                ApplyVersionStoreSettingsChanges();
                ApplyExcludeDMappChanges();
                //SQLDM-29210 -- Add option to include alerts for Read-only databases.
                ApplyReadOnlyIncludeChanges();
                //SQLDM-29041 -- Add availability group alert options.
                ApplyAvailabilityGroupChanges();
                ApplyReplicaCollectionChanges();

            }
            if (!settingsChanged) 
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private void ApplyFilterOptionChanges() //   this code for filter option changes
        {
            int index = 0;
            if (tabControl.Tabs["alertSuppressionTab"].Visible)
            {
                if (GetControlVisible(FilterOptionPanel))
                {
                    int newValue = (int)FilterOptionNumeric.Value;
                   

                    
                    if ((newValue != index ) && (newValue <= alertSettings.ThresholdRefreshTime))
                    {
                        alertSettings.ThresholdTime = newValue;

                        settingsChanged = true;
                    }
                }
            }


        }

		//SQLDM-29041 -- Add availability group alert options.
        private void ApplyReplicaCollectionChanges()
        {
            String currentOption = alertSettings.ReplicaCollectionOption;
            if (GetControlVisible(replicaCollectionOptionPanel))
            {
                        if (replicaCollectionMonitoredOnlyOption.Checked)
                        {
                            alertSettings.ReplicaCollectionOption = replicaCollectionMonitoredOnlyOption.Text;
                        }
                        else if (replicaCollectionAllOption.Checked)
                        {
                            alertSettings.ReplicaCollectionOption = replicaCollectionAllOption.Text;
                        }


            }
                    if (!currentOption.Equals(alertSettings.ReplicaCollectionOption))
                    {
                        settingsChanged = true;
                    }
            
        }

        //SQLDM-29041 -- Add availability group alert options.
        private void ApplyAvailabilityGroupChanges()
        {
            String currentOption = alertSettings.AvailabilityGroupOption;
            if (GetControlVisible(availabilityGroupBackupAlertPanel))
            {
                if (availabilityGroupPrimaryOption.Checked)
                {
                    alertSettings.AvailabilityGroupOption = availabilityGroupPrimaryOption.Text;
                }
                else if (availabilityGroupSecondaryOption.Checked)
                {
                    alertSettings.AvailabilityGroupOption = availabilityGroupSecondaryOption.Text;
                }
                else if (availabilityGroupBothOption.Checked)
                {
                    alertSettings.AvailabilityGroupOption = availabilityGroupBothOption.Text;
                }
                else if (availabilityGroupDefaultOption.Checked)
                {
                    alertSettings.AvailabilityGroupOption = availabilityGroupDefaultOption.Text;
                }
               
            }
            if (!currentOption.Equals(alertSettings.AvailabilityGroupOption))
            {
                settingsChanged = true;
            }
        }

        //SQLDM-29210 -- Add option to include alerts for Read-only databases.
        private void ApplyReadOnlyIncludeChanges()
        {

            if (GetControlVisible(readOnlyDatabaseIncludePanel))
            {

                if (!alertSettings.IncludeReadOnly.Equals(includeReadOnlyDatabases.Checked))
                {
                    alertSettings.IncludeReadOnly = includeReadOnlyDatabases.Checked;
                    settingsChanged = true;
                }
            }

        }

        private void ApplyAlertSuppressionChanges()
        {
            if (tabControl.Tabs["alertSuppressionTab"].Visible)
            {
                if (GetControlVisible(timeSuppressionPanel))
                {
                    int newValue = (int)suppressionMinutesSpinner.Value;

                    if (newValue != alertSettings.SuppressionPeriodMinutes)
                    {
                        alertSettings.SuppressionPeriodMinutes = newValue;
                        settingsChanged = true;
                    }
                }
                if (GetControlVisible(jobFailureSuppressionPanel))
                {
                    bool newValue = raiseJobFailureAlertAnytimeRadioButton.Checked;

                    if (newValue != alertSettings.TreatSingleJobFailureAsCritical)
                    {
                        alertSettings.TreatSingleJobFailureAsCritical = newValue;
                        settingsChanged = true;
                    }
                }
                if (GetControlVisible(jobAlertActivePanel))
                {
                    TimeSpan time = jobAlertActiveDuration.Time;
                    int newValue = (int)time.TotalMinutes;
                    newValue += ((int)suppressionDaysSpinner.Value) * 1440;//1440 minutes = 1 day
                    if (newValue != alertSettings.NumberMinutesJobAlertsAreActive)
                    {
                        alertSettings.NumberMinutesJobAlertsAreActive = newValue;
                        settingsChanged = true;
                    }
                }
                if (GetControlVisible(agingAlertActivePanel))
                {
                    TimeSpan time = agingAlertActiveDuration.Time;
                    int newValue = (int)time.TotalMinutes;
                    if (newValue != alertSettings.NumberMinutesAgingAlertsAreActive)
                    {
                        alertSettings.NumberMinutesAgingAlertsAreActive = newValue;
                        settingsChanged = true;
                    }
                }
            }
        }

        private void ApplyDatabasesFilterChanges()
        {
            if (GetControlVisible(databasesFilterPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(databasesExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.DatabaseExcludeMatch, matchFilters))
                {
                    alertSettings.DatabaseExcludeMatch = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.DatabaseExcludeLike, likeFilters))
                {
                    alertSettings.DatabaseExcludeLike = likeFilters;
                    settingsChanged = true;
                }
            }
        }

        //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --apply the filegroup filter changes
        private void ApplyFilegroupsFilterChanges()
        {
            if (GetControlVisible(filegroupsFilterPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(filegroupsExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.FilegroupExcludeMatch, matchFilters))
                {
                    alertSettings.FilegroupExcludeMatch = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.FilegroupExcludeLike, likeFilters))
                {
                    alertSettings.FilegroupExcludeLike = likeFilters;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyDriveFilterChanges()
        {
            if (GetControlVisible(driveFilterPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(drivesExcludeFilterTextBox.Text, out matchFilters, out likeFilters);
                Set<string> filters = new Set<string>();
                if (matchFilters != null)
                    filters.AddMany(matchFilters);
                if (likeFilters != null)
                    filters.AddMany(likeFilters);
                filters = fixupDriveFilterStrings(filters);

                matchFilters = filters.ToArray();

                if (SetChanged(alertSettings.DrivesExcludedMatch, matchFilters))
                {
                    alertSettings.DrivesExcludedMatch = matchFilters;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyTablesFilterChanges()
        {
            if (GetControlVisible(tablesFilterPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(tablesExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.TableExcludeMatch, matchFilters))
                {
                    alertSettings.TableExcludeMatch = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.TableExcludeLike, likeFilters))
                {
                    alertSettings.TableExcludeLike = likeFilters;
                    settingsChanged = true;
                }
            }
        }



        //private void ApplyJobsFiltersChanges()
        //{
        //    if (GetControlVisible(jobsFilterPanel))
        //    {
        //        string[] matchFilters, likeFilters;
        //        ParseFilterString(jobsExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

        //        if (SetChanged(alertSettings.JobNameExcludeMatch, matchFilters))
        //        {
        //            alertSettings.JobNameExcludeMatch = matchFilters;
        //            settingsChanged = true;
        //        }

        //        if (SetChanged(alertSettings.JobNameExcludeLike, likeFilters))
        //        {
        //            alertSettings.JobNameExcludeLike = likeFilters;
        //            settingsChanged = true;
        //        }
        //    }
        //}

        private void ApplyJobsFiltersChanges()
        {
            if (tabControl.Tabs["jobFiltersTab"].Visible)
            {
                if (settingsChanged)
                {
                    if (alertSettings.JobIncludeFilter != null)
                        alertSettings.JobIncludeFilter.Clear();
                    if (alertSettings.JobExcludeFilter != null)
                        alertSettings.JobExcludeFilter.Clear();

                    if (alertSettings.AlertOnJobSteps != chkAlertOnJobSteps.Checked)
                    {
                        alertSettings.AlertOnJobSteps = chkAlertOnJobSteps.Checked;
                    }

                    alertSettings.JobIncludeFilter = ParseJobFilters(gridInclude);
                    alertSettings.JobExcludeFilter = ParseJobFilters(gridExclude);
                }
            }
        }

        private void ApplyJobMinRuntimeByPercent()
        {
            if (tabControl.Tabs["jobFiltersTab"].Visible)
            {
                if (alertSettings.MinSecondsRuntimeRunningJobByPercent != this.minimalRuntimeUpDown.Value)
                {
                    alertSettings.MinSecondsRuntimeRunningJobByPercent = (int)this.minimalRuntimeUpDown.Value;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyJobCategoriesFilterChanges()
        {
            if (GetControlVisible(jobCategoriesFilterPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(jobCategoriesExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.JobCategoryExcludeMatch, matchFilters))
                {
                    alertSettings.JobCategoryExcludeMatch = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.JobCategoryExcludeLike, likeFilters))
                {
                    alertSettings.JobCategoryExcludeLike = likeFilters;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyApplicationsFilterChanges()
        {
            if (GetControlVisible(sessionApplicationsFilterPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(sessionApplicationsExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.AppNameExcludeMatch, matchFilters))
                {
                    alertSettings.AppNameExcludeMatch = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.AppNameExcludeLike, likeFilters))
                {
                    alertSettings.AppNameExcludeLike = likeFilters;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyHostServersFilterChanges()
        {
            if (GetControlVisible(sessionHostServersFilterPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(sessionHostServersExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.HostExcludeMatch, matchFilters))
                {
                    alertSettings.HostExcludeMatch = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.HostExcludeLike, likeFilters))
                {
                    alertSettings.HostExcludeLike = likeFilters;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyUsersFilterChanges()
        {
            if (GetControlVisible(sessionUsersFilterPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(sessionUsersExcludeFilterTextBox.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.UserExcludeMatch, matchFilters))
                {
                    alertSettings.UserExcludeMatch = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.UserExcludeLike, likeFilters))
                {
                    alertSettings.UserExcludeLike = likeFilters;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyLogScanSizeLimitChanges()
        {
            if (GetControlVisible(logSizeLimitPanel))
            {
                alertSettings.LogSizeLimit = (long)spnLogSize.Value * 1024;
                settingsChanged = true;
            }
        }

        private void ApplyLogScanIncludeChanges()
        {
            if (GetControlVisible(logScanPanel))
            {
                string[] matchFilters, likeFilters;
                ParseFilterString(logScanIncludeTextBoxCritical.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.LogIncludeMatchCritical, matchFilters))
                {
                    alertSettings.LogIncludeMatchCritical = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.LogIncludeLikeCritical, likeFilters))
                {
                    alertSettings.LogIncludeLikeCritical = likeFilters;
                    settingsChanged = true;
                }

                ParseFilterString(logScanIncludeTextBoxWarning.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.LogIncludeMatchWarning, matchFilters))
                {
                    alertSettings.LogIncludeMatchWarning = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.LogIncludeLikeWarning, likeFilters))
                {
                    alertSettings.LogIncludeLikeWarning = likeFilters;
                    settingsChanged = true;
                }

                // Commenting this out below because it appears identical to the section above, so I don't think its needed
                //ParseFilterString(logScanIncludeTextBoxWarning.Text, out matchFilters, out likeFilters);

                //if (SetChanged(alertSettings.LogIncludeMatchWarning, matchFilters))
                //{
                //    alertSettings.LogIncludeMatchWarning = matchFilters;
                //    settingsChanged = true;
                //}

                //if (SetChanged(alertSettings.LogIncludeLikeWarning, likeFilters))
                //{
                //    alertSettings.LogIncludeLikeWarning = likeFilters;
                //    settingsChanged = true;
                //}

                ParseFilterString(logScanIncludeTextBoxInfo.Text, out matchFilters, out likeFilters);

                if (SetChanged(alertSettings.LogIncludeMatchInfo, matchFilters))
                {
                    alertSettings.LogIncludeMatchInfo = matchFilters;
                    settingsChanged = true;
                }

                if (SetChanged(alertSettings.LogIncludeLikeInfo, likeFilters))
                {
                    alertSettings.LogIncludeLikeInfo = likeFilters;
                    settingsChanged = true;
                }

                matchFilters = ParseFilterStringForRegex(logScanIncludeTextBoxRegexCritical.Text);

                if (SetChanged(alertSettings.LogIncludeRegexCritical, matchFilters))
                {
                    alertSettings.LogIncludeRegexCritical = matchFilters;
                    settingsChanged = true;
                }

                matchFilters = ParseFilterStringForRegex(logScanIncludeTextBoxRegexWarning.Text);

                if (SetChanged(alertSettings.LogIncludeRegexWarning, matchFilters))
                {
                    alertSettings.LogIncludeRegexWarning = matchFilters;
                    settingsChanged = true;
                }

                matchFilters = ParseFilterStringForRegex(logScanIncludeTextBoxRegexInfo.Text);

                if (SetChanged(alertSettings.LogIncludeRegexInfo, matchFilters))
                {
                    alertSettings.LogIncludeRegexInfo = matchFilters;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyCollectionFailuresChanges()
        {
            if (tabControl.Tabs["collectionFailuresTab"].Visible)
            {
                if (alertSettings.TreatCustomCounterFailureAsCritical != generateAlertButton.Checked)
                {
                    alertSettings.TreatCustomCounterFailureAsCritical = generateAlertButton.Checked;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyAutogrowthSettingsChanges()
        {
            if (tabControl.Tabs["autogrowSettingsTab"].Visible)
            {
                if (alertSettings.IncludeAutogrowInDatabaseSizeCalc != useAutogrow.Checked)
                {
                    alertSettings.IncludeAutogrowInDatabaseSizeCalc = useAutogrow.Checked;
                    settingsChanged = true;
                }
            }
        }

        private void ApplyVersionStoreSettingsChanges()
        {

            if (alertSettings.MinGenerationRatePerSecond != minVersionStoreGenerationRate.Value)
            {
                alertSettings.MinGenerationRatePerSecond = (int)Math.Floor(minVersionStoreGenerationRate.Value);
                settingsChanged = true;
            }
            if (alertSettings.MinVersionStoreSize.Megabytes != versionStoreSizeUpDown.Value)
            {
                //alertSettings.MinVersionStoreSize = new FileSize();
                alertSettings.MinVersionStoreSize.Megabytes = versionStoreSizeUpDown.Value;
                settingsChanged = true;
            }
        }

        private void ApplyExcludeDMappChanges()
        {
            if (tabControl.Tabs["alertFiltersTab"].Visible)
            {
                if (alertSettings.ExcludeSqldmApp != this.excludeDMapplicationCheckBox.Checked)
                {
                    alertSettings.ExcludeSqldmApp = this.excludeDMapplicationCheckBox.Checked;
                    settingsChanged = true;
                }
            }
        }


        private static void ParseFilterString(string filterString, out string[] matchFilters, out string[] likeFilters)
        {
            OrderedSet<string> matchSet = new OrderedSet<string>();
            OrderedSet<string> likeSet = new OrderedSet<string>();

            if (!string.IsNullOrEmpty(filterString))
            {
                string[] filters = filterString.Split(';');

                foreach (string untrimmedFilter in filters)
                {
                    string filter = untrimmedFilter.Trim();

                    if (filter.Length > 1 && filter[0] == '[' && filter[filter.Length - 1] == ']')
                    {
                        filter = filter.Remove(0, 1);
                        filter = filter.Remove(filter.Length - 1, 1);
                    }

                    if (filter.Contains("%") && !likeSet.Contains(filter))
                    {
                        likeSet.Add(filter);
                    }
                    else if (!matchSet.Contains(filter))
                    {
                        matchSet.Add(filter);
                    }
                }
            }

            matchFilters = matchSet.ToArray();
            likeFilters = likeSet.ToArray();
        }

        private static string[] ParseFilterStringForRegex(string filterString)
        {
            OrderedSet<string> matchSet = new OrderedSet<string>();
            OrderedSet<string> likeSet = new OrderedSet<string>();

            if (!string.IsNullOrEmpty(filterString))
            {
                string[] filters = filterString.Split('\n');

                foreach (string untrimmedFilter in filters)
                {
                    string filter = untrimmedFilter.Trim();

                    if (!matchSet.Contains(filter))
                    {
                        matchSet.Add(filter);
                    }
                }
            }

            return matchSet.ToArray();
        }

        private static List<JobFilter> ParseJobFilters(DataGridView gridView)
        {
            OpCodes opCode;
            string filterValue;
            List<JobFilter> jobFiltersList = new List<JobFilter>();


            foreach (DataGridViewRow row in gridView.Rows)
            {
                opCode = (OpCodes)Enum.Parse(typeof(OpCodes), row.Cells[0].Value.ToString());
                filterValue = row.Cells[1].Value.ToString();
                FilterSet categoryFilter = new FilterSet(opCode, filterValue);

                opCode = (OpCodes)Enum.Parse(typeof(OpCodes), row.Cells[2].Value.ToString());
                filterValue = row.Cells[3].Value.ToString();
                FilterSet jobNameFilter = new FilterSet(opCode, filterValue);

                opCode = (OpCodes)Enum.Parse(typeof(OpCodes), row.Cells[4].Value.ToString());
                filterValue = row.Cells[5].Value.ToString();
                FilterSet stepFilter = new FilterSet(opCode, filterValue);

                jobFiltersList.Add(new JobFilter(jobNameFilter, categoryFilter, stepFilter));
            }

            return jobFiltersList;
        }


        private static string[] ParseFilterInfo(JobFilter filter)
        {
            FilterSet categoryFilter = filter.Category;
            FilterSet jobFilter = filter.JobName;
            FilterSet stepFilter = filter.StepName;

            string[] newRow = new string[] {
                categoryFilter.OpCode.ToString(),
                categoryFilter.Filter.ToString(),
                jobFilter.OpCode.ToString(),
                jobFilter.Filter.ToString(),
                stepFilter.OpCode.ToString(),
                stepFilter.Filter.ToString() };

            return newRow;
        }


        private static bool SetChanged(string[] left, string[] right)
        {
            if (left == null || left.Length == 0)
            {
                return (right != null && right.Length > 0);
            }

            if (right == null || right.Length == 0 || left.Length != right.Length)
            {
                return true;
            }

            return !Algorithms.EqualSets(left, right);
        }

        private void filterDatabasesButton_Click(object sender, EventArgs e)
        {
            AlertFilterDialog dialog =
                new AlertFilterDialog(instanceId, AlertFilterType.Database, databasesExcludeFilterTextBox.Text);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                databasesExcludeFilterTextBox.Text = dialog.ExcludeFilterText;
            }
        }

        // SQLdm 9.1 (Abhishek Joshi) -FileGroup and Mount Point Monitoring Improvements -get all the filegroups in a SQL server instance
        private void filterFilegroupsButton_Click(object sender, EventArgs e)
        {
            AlertFilterDialog dialog =
                new AlertFilterDialog(instanceId, AlertFilterType.Filegroup, filegroupsExcludeFilterTextBox.Text, thresholdInstanceName);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                filegroupsExcludeFilterTextBox.Text = dialog.ExcludeFilterText;
            }
        }

        private void databaseExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(databasesFilterPanel, databasesExcludeFilterTextBox);
        }

        private void databaseExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(databasesFilterPanel, databasesExcludeFilterTextBox);
        }

        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --defining text box controls for the filegroup filters
        private void filegroupExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(filegroupsFilterPanel, filegroupsExcludeFilterTextBox);
        }

        private void filegroupExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(filegroupsFilterPanel, filegroupsExcludeFilterTextBox);
        }
        //END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --defining text box controls for the filegroup filters

        private void tableExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(tablesFilterPanel, tablesExcludeFilterTextBox);
        }

        private void tableExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(tablesFilterPanel, tablesExcludeFilterTextBox);
        }

        private static void UpdateTextBoxConrols(Control parentControl, TextBox textBox)
        {
            if (parentControl != null && textBox != null)
            {
                int lastLine = textBox.TextLength == 0
                                   ? 0
                                   : textBox.GetLineFromCharIndex(textBox.TextLength - 1);

                if (textBox.Text.EndsWith("\r\n"))
                {
                    lastLine++;
                }

                parentControl.Height = 75 + lastLine * TextRenderer.MeasureText(" ", textBox.Font).Height;
            }
        }

        private void jobsExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(jobsFilterPanel, jobsExcludeFilterTextBox);
        }

        private void jobsExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(jobsFilterPanel, jobsExcludeFilterTextBox);
        }

        private void jobCategoriesExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(jobCategoriesFilterPanel, jobCategoriesExcludeFilterTextBox);
        }

        private void jobCategoriesExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(jobCategoriesFilterPanel, jobCategoriesExcludeFilterTextBox);
        }

        private void sessionApplicationsExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(sessionApplicationsFilterPanel, sessionApplicationsExcludeFilterTextBox);
        }

        private void sessionApplicationsExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(sessionApplicationsFilterPanel, sessionApplicationsExcludeFilterTextBox);
        }

        private void sessionHostServersExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(sessionHostServersFilterPanel, sessionHostServersExcludeFilterTextBox);
        }

        private void sessionHostServersExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(sessionHostServersFilterPanel, sessionHostServersExcludeFilterTextBox);
        }

        private void sessionUsersExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(sessionUsersFilterPanel, sessionUsersExcludeFilterTextBox);
        }

        private void sessionUsersExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(sessionUsersFilterPanel, sessionUsersExcludeFilterTextBox);
        }

        private void filterJobsButton_Click(object sender, EventArgs e)
        {
            AlertFilterDialog dialog =
                new AlertFilterDialog(instanceId, AlertFilterType.Job, jobsExcludeFilterTextBox.Text);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                jobsExcludeFilterTextBox.Text = dialog.ExcludeFilterText;
            }
        }

        private void filterJobCategoriesButton_Click(object sender, EventArgs e)
        {
            AlertFilterDialog dialog =
                new AlertFilterDialog(instanceId, AlertFilterType.JobCategory, jobCategoriesExcludeFilterTextBox.Text);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                jobCategoriesExcludeFilterTextBox.Text = dialog.ExcludeFilterText;
            }
        }

        private string GetHelpTopic()
        {
            string topic = HelpTopics.AlertsConfiguration;

            switch (tabControl.SelectedTab.Key)
            {
                case "alertSuppressionTab":
                    switch (alertSettings.Metric)
                    {
                        case Metric.LongJobs:
                        case Metric.LongJobsMinutes:
                        case Metric.BombedJobs:
                        case Metric.JobCompletion:
                            topic = HelpTopics.AdvancedAlertConfig_ConfigureAlertDuration;
                            break;
                        default:
                            topic = HelpTopics.AdvancedAlertConfig_Smoothing;
                            break;
                    }

                    break;
                case "alertFiltersTab":
                    if (databasesFilterPanel.Visible)
                        topic = HelpTopics.AdvancedAlertConfig_DatabaseExclustions;
                    else
                        if ((alertSettings.Metric == Metric.ErrorLog) || (alertSettings.Metric == Metric.AgentLog))
                    {
                        topic = HelpTopics.AdvancedAlertConfig_TextAndExpression;
                    }
                    else if (sessionApplicationsFilterPanel.Visible)
                    {
                        topic = HelpTopics.AdvancedAlertConfig_SessionExclusions;
                    }
                    else if ((alertSettings.Metric == Metric.AverageDiskMillisecondsPerRead) ||
                        (alertSettings.Metric == Metric.AverageDiskMillisecondsPerTransfer) ||
                        (alertSettings.Metric == Metric.AverageDiskMillisecondsPerWrite) ||
                        (alertSettings.Metric == Metric.DiskReadsPerSecond) ||
                        (alertSettings.Metric == Metric.DiskTransfersPerSecond) ||
                        (alertSettings.Metric == Metric.DiskWritesPerSecond) ||
                        (alertSettings.Metric == Metric.OSDiskAverageDiskQueueLengthPerDisk) ||
                        (alertSettings.Metric == Metric.OSDiskFull) ||
                        (alertSettings.Metric == Metric.OSDiskPhysicalDiskTimePctPerDisk))
                    {
                        topic = HelpTopics.AdvancedAlertConfig_DiskDriveExclusions;
                    }
                    break;
                case "jobFiltersTab":
                    topic = HelpTopics.AdvancedAlertConfig_JobExclusions;
                    break;
                case "collectionFailuresTab":
                    topic = HelpTopics.AdvancedAlertConfig_CustomCounterOptions;
                    break;
                case "autogrowSettingsTab":
                    topic = HelpTopics.AdvancedAlertConfig_ConfigureSizeRemaining;
                    break;
            }

            return topic;
        }

        private void AdvancedAlertConfigurationDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(GetHelpTopic());
        }                

        private void AdvancedAlertConfigurationDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(GetHelpTopic());
        }
        
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ignore ENTER and CTRL+ENTER
            e.Handled = e.KeyChar == 10 || e.KeyChar == 13;
        }

        private void propertiesHeaderStrip13_Load(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void filterTablesButton_Click(object sender, EventArgs e)
        {
            string[] likes;
            string[] matches;
            ParseFilterString(tablesExcludeFilterTextBox.Text, out matches, out likes);

            Set<string> selected = new Set<string>(likes);
            selected.AddMany(matches);

            using (SelectTableDialog std = new SelectTableDialog(ApplicationModel.Default.ActiveInstances[instanceId], selected))
            {
                if (std.ShowDialog(this) == DialogResult.OK)
                {
                    tablesExcludeFilterTextBox.Text = BuildFilterTextFromLists(std.SelectedTables, null);
                }
            }
        }

        private string BuildFilterTextFromLists(IEnumerable<string> list1, IEnumerable<string> list2)
        {
            List<string> items = new List<string>();
            if (list1 != null)
                items.AddRange(list1);
            if (list2 != null)
                items.AddRange(list2);

            items.Sort();

            if (items.Count > 0)
            {
                StringBuilder excludeFilter = new StringBuilder();

                foreach (string item in items)
                {
                    if (excludeFilter.Length != 0)
                        excludeFilter.Append("; ");

                    excludeFilter.Append("[");
                    excludeFilter.Append(item.Trim());
                    excludeFilter.Append("]");
                }
                return excludeFilter.ToString();
            }
            return String.Empty;
        }

        private void drivesExcludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(driveFilterPanel, drivesExcludeFilterTextBox);
        }

        private void drivesExcludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(driveFilterPanel, drivesExcludeFilterTextBox);
        }

        private void filterDrivesButton_Click(object sender, EventArgs e)
        {
            string[] likes;
            string[] matches;
            ParseFilterString(drivesExcludeFilterTextBox.Text, out matches, out likes);

            Set<string> selected = new Set<string>(likes);
            selected.AddMany(matches);
            selected = fixupDriveFilterStrings(selected);

            using (SelectDrivesDialog std = new SelectDrivesDialog(instanceId, selected))
            {
                if (std.ShowDialog(this) == DialogResult.OK)
                {
                    selected = fixupDriveFilterStrings(std.SelectedDrives);
                    drivesExcludeFilterTextBox.Text = BuildFilterTextFromLists(selected, null);
                }
            }
        }

        private Set<string> fixupDriveFilterStrings(IEnumerable<string> driveList)
        {
            Set<string> result = new Set<string>();

            foreach (string item in driveList)
            {
                string drive = item.Trim();
                drive = drive.TrimEnd('\\', '/');

                switch (drive.Length)
                {
                    case 0:
                        continue;
                    case 1:
                        if (drive[0] == ':')
                            continue;
                        drive = drive.ToUpper() + ":";
                        break;
                    case 2:
                        drive = drive.ToUpper();
                        break;
                    default:
                        break;
                }
                result.Add(drive);
            }
            return result;
        }

        private void tabControl_ActiveTabChanging(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangingEventArgs e)
        {
            UltraTab tab = tabControl.ActiveTab ?? e.Tab;
            Office2007PropertyPage page = tab.TabPage.Controls[0] as Office2007PropertyPage;
            if (page != null)
            {
                foreach (Control control in page.ContentPanel.Controls)
                {
                    if (control.Visible)
                        visibleControls.Add(control.Name);
                    else
                        visibleControls.Remove(control.Name);
                }
            }
            if (checknumerictype == typeof(double) || checknumerictype == typeof(float) || checknumerictype == typeof(int) || checknumerictype == typeof(Int64))
            {
                this.FilterOptionPanel.Visible = true;
            }
            else
            {
                this.FilterOptionPanel.Visible = false;
            }
        }

        private void gridInclude_SelectionChanged(object sender, EventArgs e)
        {
            editInclude.Enabled = removeInclude.Enabled = (gridInclude.SelectedRows.Count > 0);
        }

        private void gridExclude_SelectionChanged(object sender, EventArgs e)
        {
            editExclude.Enabled = removeExclude.Enabled = (gridExclude.SelectedRows.Count > 0);
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (sender == addInclude)
                jobRules = gridInclude;
            else if (sender == addExclude)
                jobRules = gridExclude;
            else
            {
                MessageBox.Show("Not sure which set of rules we are working with here");
                return;
            }

            AddJobFilter addJobFilterDialog = new AddJobFilter(instanceId, chkAlertOnJobSteps.Checked);
            addJobFilterDialog.Text = "Add Job Filter";

            bool ruleExists = false;

            do
            {
                addJobFilterDialog.FilterExists = ruleExists;
                ruleExists = false;
                if (DialogResult.OK == addJobFilterDialog.ShowDialog(this))
                {
                    string[] newRule = new string[] {
                        addJobFilterDialog.CatOpCode,
                        addJobFilterDialog.Category,
                        addJobFilterDialog.JobOpCode,
                        addJobFilterDialog.JobName,
                        addJobFilterDialog.StepOpCode,
                        addJobFilterDialog.StepName
                        };

                    ruleExists = checkIfExists(jobRules.Rows, newRule);
                    if (ruleExists)
                        addJobFilterDialog.FilterExistsText = String.Format(FILTER_EXISTS, jobRules == gridInclude ? INCLUDE : EXCLUDE);
                    else
                    {
                        if ((jobRules == gridInclude) && checkIfExists(gridExclude.Rows, newRule))
                        {
                            ruleExists = true;
                            addJobFilterDialog.FilterExistsText = String.Format(FILTER_EXISTS, EXCLUDE);
                            continue;
                        }
                        else if ((jobRules == gridExclude) && checkIfExists(gridInclude.Rows, newRule))
                        {
                            ruleExists = true;
                            addJobFilterDialog.FilterExistsText = String.Format(FILTER_EXISTS, INCLUDE);
                            continue;
                        }
                        jobRules.Rows.Add(newRule);

                        settingsChanged = true;
                    }
                }
            } while (ruleExists);

            addJobFilterDialog.Dispose();
            jobRules = null;
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            if (sender == editInclude)
                jobRules = gridInclude;
            else if (sender == editExclude)
                jobRules = gridExclude;
            else
            {
                MessageBox.Show("Not sure which grid we are working with here");
                return;
            }


            if (jobRules.SelectedRows.Count <= 0)
            {
                MessageBox.Show("You must select a row to edit");
                return;
            }

            AddJobFilter editJobFilterDialog = new AddJobFilter(instanceId, chkAlertOnJobSteps.Checked);

            editJobFilterDialog.Text = "Edit Job Filter";

            editJobFilterDialog.CatOpCode = jobRules.SelectedRows[0].Cells[0].Value.ToString();
            editJobFilterDialog.Category = jobRules.SelectedRows[0].Cells[1].Value.ToString();
            editJobFilterDialog.JobOpCode = jobRules.SelectedRows[0].Cells[2].Value.ToString();
            editJobFilterDialog.JobName = jobRules.SelectedRows[0].Cells[3].Value.ToString();
            editJobFilterDialog.StepOpCode = jobRules.SelectedRows[0].Cells[4].Value.ToString();
            editJobFilterDialog.StepName = jobRules.SelectedRows[0].Cells[5].Value.ToString();

            bool ruleExists = false;

            do
            {
                editJobFilterDialog.FilterExists = ruleExists;
                ruleExists = false;

                if (DialogResult.OK == editJobFilterDialog.ShowDialog(this))
                {

                    string[] editedRow = new string[] { editJobFilterDialog.CatOpCode,
                                                      editJobFilterDialog.Category,
                                                      editJobFilterDialog.JobOpCode,
                                                      editJobFilterDialog.JobName,
                                                      editJobFilterDialog.StepOpCode,
                                                      editJobFilterDialog.StepName
                                                    };

                    ruleExists = checkIfExists(jobRules.Rows, editedRow);
                    // If the rule exists, but its the row that was being edited, then just go on
                    if (ruleExists &&
                        editedRow[0].Trim().ToLower() == jobRules.SelectedRows[0].Cells[0].Value.ToString().Trim().ToLower() &&
                        editedRow[1].Trim().ToLower() == jobRules.SelectedRows[0].Cells[1].Value.ToString().Trim().ToLower() &&
                        editedRow[2].Trim().ToLower() == jobRules.SelectedRows[0].Cells[2].Value.ToString().Trim().ToLower() &&
                        editedRow[3].Trim().ToLower() == jobRules.SelectedRows[0].Cells[3].Value.ToString().Trim().ToLower() &&
                        editedRow[4].Trim().ToLower() == jobRules.SelectedRows[0].Cells[4].Value.ToString().Trim().ToLower() &&
                        editedRow[5].Trim().ToLower() == jobRules.SelectedRows[0].Cells[5].Value.ToString().Trim().ToLower())
                    {
                        ruleExists = false;
                        break;
                    }
                    if (ruleExists)
                        editJobFilterDialog.FilterExistsText = String.Format(FILTER_EXISTS, jobRules == gridInclude ? INCLUDE : EXCLUDE);
                    else  // If the rule doesn't exists, make sure it doesn't match a rule in the opposite grid
                    {
                        if ((jobRules == gridInclude) && checkIfExists(gridExclude.Rows, editedRow))
                        {
                            ruleExists = true;
                            editJobFilterDialog.FilterExistsText = String.Format(FILTER_EXISTS, EXCLUDE);
                            continue;
                        }
                        else if ((jobRules == gridExclude) && checkIfExists(gridInclude.Rows, editedRow))
                        {
                            ruleExists = true;
                            editJobFilterDialog.FilterExistsText = String.Format(FILTER_EXISTS, INCLUDE);
                            continue;
                        }

                        // If you made it this far you are good to update the selected row
                        jobRules.SelectedRows[0].Cells[0].Value = editJobFilterDialog.CatOpCode;
                        jobRules.SelectedRows[0].Cells[1].Value = editJobFilterDialog.Category;
                        jobRules.SelectedRows[0].Cells[2].Value = editJobFilterDialog.JobOpCode;
                        jobRules.SelectedRows[0].Cells[3].Value = editJobFilterDialog.JobName;
                        jobRules.SelectedRows[0].Cells[4].Value = editJobFilterDialog.StepOpCode;
                        jobRules.SelectedRows[0].Cells[5].Value = editJobFilterDialog.StepName;

                        settingsChanged = true;
                    }
                }

            } while (ruleExists);

            editJobFilterDialog.Dispose();
            jobRules = null;

        }

        // I've hidden this button per a PR... didn't actually remove it because one of the people
        //  that complained about its existance was also the person that asked for it to be put in.
        //  So I have a feeling it will come back.
        private void Clear_Click(object sender, EventArgs e)
        {
            if (sender == clearInclude)
                gridInclude.Rows.Clear();
            else if (sender == clearExclude)
                gridExclude.Rows.Clear();
            settingsChanged = true;
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (sender == removeInclude)
                jobRules = gridInclude;
            else if (sender == removeExclude)
                jobRules = gridExclude;
            else
            {
                MessageBox.Show("Not sure which grid to remove item from");
                return;
            }

            jobRules.Rows.RemoveAt(jobRules.SelectedRows[0].Index);
            settingsChanged = true;
        }

        private void chkAlertOnJobSteps_CheckedChanged(object sender, EventArgs e)
        {
            gridInclude.Columns["includeStepOpCode"].Visible =
                gridInclude.Columns["includeStepName"].Visible =
                gridExclude.Columns["excludeStepOpCode"].Visible =
                gridExclude.Columns["excludeStepName"].Visible = chkAlertOnJobSteps.Checked;
            settingsChanged = true;
        }

        private bool checkIfExists(DataGridViewRowCollection filters, string[] newFilter)
        {
            bool exists = false;
            foreach (DataGridViewRow row in filters)
            {
                if (row.Cells[0].Value.ToString().Trim().ToLower() == newFilter[0].Trim().ToLower() &&
                    row.Cells[1].Value.ToString().Trim().ToLower() == newFilter[1].Trim().ToLower() &&
                    row.Cells[2].Value.ToString().Trim().ToLower() == newFilter[2].Trim().ToLower() &&
                    row.Cells[3].Value.ToString().Trim().ToLower() == newFilter[3].Trim().ToLower() &&
                    row.Cells[4].Value.ToString().Trim().ToLower() == newFilter[4].Trim().ToLower() &&
                    row.Cells[5].Value.ToString().Trim().ToLower() == newFilter[5].Trim().ToLower())
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
            if (Text.Contains("Oldest Open Transaction (Minutes)") || Text.Contains("Blocking Session Wait Time (Seconds)") || Text.Contains("Session Tempdb Space Usage (MB)"))
            {
                this.FilterOptionPanel.Location = new System.Drawing.Point(9, 222); // 3 , 74 
            }
            if (Text.Contains("Table Fragmentation (Percent)") || Text.Contains("Filegroup Space Full (Percent)") || Text.Contains("Filegroup Space Full (Size)"))
            {
                this.FilterOptionPanel.Location = new System.Drawing.Point(6, 148); // 3 , 74 
            }
            if (Text.Contains("Session CPU Time (Seconds)"))
            {
                this.FilterOptionPanel.Location = new System.Drawing.Point(12, 296); // 3 , 74 
            }

            if (Text.Contains("SQL Server Error Log") || Text.Contains("SQL Server Agent Log"))
            {
                this.FilterOptionPanel.Location = new System.Drawing.Point(3, 405); // 3 , 74 
                                                                                    // this.FilterOptionPanel.BackColor = System.Drawing.Color.Green;
            }
        }
        private void FilterOptionNumeric_ValueChanged(object sender, EventArgs e)
        {
            alertSettings.ThresholdTime = Convert.ToInt32(((NumericUpDown)sender).Value);
        }

        private void FilterOptionNumericOutOf_ValueChanged(object sender, EventArgs e)
        {
            alertSettings.ThresholdRefreshTime = Convert.ToInt32(((NumericUpDown)sender).Value);
        }

        private bool checkMetricSupportAdvancefilterTab(int metricId)
        {
            bool isMerticSupportAdvancefilter = false;

            switch (metricId)
            {               
                case (int)Metric.PreferredNodeUnavailability:
                case (int)Metric.ClusterActiveNode:
                case (int)Metric.ClusterFailover:
                case (int)Metric.ReadWriteErrors:
                case (int)Metric.MirroringSessionRoleChange:
                case (int)Metric.MirroringSessionNonPreferredConfig:
                case (int)Metric.MirroringWitnessConnection:
                case (int)Metric.ServiceTierChanges:
                case (int)Metric.BombedJobs:
                case (int)Metric.RepositoryGroomingTimedOut:
                    isMerticSupportAdvancefilter = false;
                    break;
                default:
                    isMerticSupportAdvancefilter = true;
                    break;
            }

            return isMerticSupportAdvancefilter;
        }

    }

}
