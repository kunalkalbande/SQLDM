//------------------------------------------------------------------------------
// <copyright file="ClickThroughHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   26-Dec-2018
// Description          :   Done changes for Drill-Down Revisions.
//----------------------------------------------------------------------------
using System;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    public static class ClickThroughHelper
    {
        /// <summary>
        /// Helper function to get the view & view argument based on the metric.
        /// </summary>
        internal static bool GetMetricView(int instanceId, Metric metric, out ServerViews view, out object viewArg)
        {
            view = ServerViews.Overview;
            viewArg = null;
            bool isViewFound = true;
            switch (metric)
            {
                //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get metric view for the new services
                case Metric.SQLBrowserServiceStatus:
                case Metric.SQLActiveDirectoryHelperServiceStatus:
                //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get metric view for the new services
                case Metric.AgentServiceStatus:
                case Metric.FullTextServiceStatus:
                case Metric.DtcServiceStatus:
                case Metric.SqlServiceStatus:
                    view = ServerViews.ServicesSummary;
                    break;
                case Metric.BlockingAlert:
                case Metric.BlockedSessions:
                case Metric.Deadlock:           //M1
                    view = ServerViews.SessionsBlocking;
                    break;
                case Metric.LongJobs: 
                case Metric.LongJobsMinutes:
                case Metric.BombedJobs:
                case Metric.JobCompletion:
                    view = ServerViews.ServicesSqlAgentJobs;
                    break;
                // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --setting filegroup metrices to an appropriate view
                case Metric.FilegroupSpaceFullPct:
                case Metric.FilegroupSpaceFullSize:
                // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --setting filegroup metrices to an appropriate view
                case Metric.DatabaseFileSize:
                case Metric.TransLogSize:
                case Metric.TransLogSizeMb:
                case Metric.DataUsedPct:
                case Metric.LogUsedPct:
                case Metric.DataFileAutogrow:
                case Metric.LogFileAutogrow:
                case Metric.DatabaseSizeMb:     //M1
                    view = ServerViews.DatabasesFiles;
                    break;
                case Metric.DatabaseSizePct:                
                case Metric.DatabaseStatus:
                view = ServerViews.DatabasesSummary;
                    break;
                case Metric.NonDistributedTrans:
                case Metric.NonDistributedTransTime:
                case Metric.NonSubscribedTransTime:
                case Metric.NonSubscribedTransNum:
                    view = ServerViews.ServicesReplication;
                    break;
                case Metric.OSCPUPrivilegedTimePct:
                case Metric.OSCPUProcessorQueueLength:
                case Metric.OSCPUUsagePct:
                case Metric.SQLCPUUsagePct:
                case Metric.OSUserCPUUsagePct:
                case Metric.VmCPUUtilization:
                case Metric.VmESXCPUUtilization:
                case Metric.VmCPUReadyWaitTime:
                case Metric.CPUCreditBalance:
                case Metric.CPUCreditUsage:
                case Metric.CPUCreditBalanceHigh:
                    view = ServerViews.ResourcesCpu;
                    break;
				//START SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - added disk size view as the default view for OS Disk Full Metric Alert
                case Metric.OSDiskFull:
                case Metric.OsDiskFreeMb:       //M1
                    view = ServerViews.ResourcesDiskSize;
                    break;
                //END SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - added disk size view as the default view for OS Disk Full Metric Alert				               
                case Metric.OSDiskAverageDiskQueueLength:
                case Metric.OSDiskAverageDiskQueueLengthPerDisk:
                case Metric.OSDiskPhysicalDiskTimePct:
                case Metric.OSDiskPhysicalDiskTimePctPerDisk:
                case Metric.AverageDiskMillisecondsPerRead:
                case Metric.AverageDiskMillisecondsPerTransfer:
                case Metric.AverageDiskMillisecondsPerWrite:
                case Metric.DiskReadsPerSecond:
                case Metric.DiskTransfersPerSecond:
                case Metric.DiskWritesPerSecond:
                case Metric.WriteLatency:
                case Metric.WriteThroughput:
                case Metric.ReadLatency:
                case Metric.ReadThroughput:
                case Metric.DiskQueueDepth:
                case Metric.ReadLatencyLow:
                case Metric.WriteLatencyLow:
                    view = ServerViews.ResourcesDisk;
                    break;
                case Metric.OSMemoryUsagePct:
                case Metric.OSMemoryPagesPerSecond:
                case Metric.SQLMemoryUsagePct:
                case Metric.VmMemoryUtilization:
                case Metric.VmESXMemoryUsage:
                case Metric.VmReclaimedMemory:
                case Metric.VmMemorySwapDelayDetected:
                case Metric.VmESXMemorySwapDetected:
                case Metric.PageLifeExpectancy:     //M1
                case Metric.SwapUsage:
                    view = ServerViews.ResourcesMemory;
                    break;
                case Metric.AgentXPStatus:
                case Metric.CLRStatus:
                case Metric.OLEAutomationStatus:
                case Metric.SSConnectionProblem:
                    view = ServerViews.OverviewConfiguration;
                    break;
                case Metric.QueryMonitorStatus:
                    view = ServerViews.QueriesSignatures;       //M1
                    break;
                case Metric.ResourceAlert:
                    view = ServerViews.SessionsDetails;
                    viewArg = new SessionsConfiguration(instanceId, false, true, true, false, false, false, false, false, false, false, null, string.Empty, string.Empty, string.Empty, string.Empty);
                    break;
                case Metric.OldestOpenTransMinutes:               
                    view = ServerViews.SessionsDetails;
                    break;
                case Metric.ServerResponseTime:
                    view = ServerViews.SessionsSummary;
                    break;
                case Metric.SSDefragOperation:
                case Metric.ReorganisationPct:
                case Metric.IndexRowHits:           // M1
                    view = ServerViews.DatabasesTablesIndexes;
                    break;
                case Metric.MaintenanceMode:
                    view = ServerViews.Overview;
                    break;
                case Metric.Custom:
                    view = ServerViews.OverviewDetails;
                    viewArg = ServerDetailsView.Selection.Custom;
                    break;
                case Metric.AgentLog:
                case Metric.ErrorLog:
                    view = ServerViews.Logs;
                    break;
                case Metric.MirroringWitnessConnection:
                case Metric.MirrorCommitOverhead:
                case Metric.MirroringSessionNonPreferredConfig:
                case Metric.MirroringSessionRoleChange:
                case Metric.MirroringSessionsStatus:
                case Metric.UnsentLogThreshold:
                case Metric.UnrestoredLog:
                case Metric.OldestUnsentMirroringTran:
                    view = ServerViews.DatabasesMirroring;
                    break;
                case Metric.VmConfigChange:
                case Metric.VmHostServerChange:
                case Metric.VmResourceLimits:
                case Metric.VmPowerState:
                case Metric.VmESXPowerState:
                    view = ServerViews.OverviewDetails;
                    viewArg = ServerDetailsView.Selection.All;
                    break;
                case Metric.ProcCacheHitRatio:
                    view = ServerViews.ResourcesProcedureCache;
                    break;
                case Metric.VersionStoreGenerationRatio:
                case Metric.VersionStoreSize:
                case Metric.LongRunningVersionStoreTransaction:
                case Metric.SessionTempdbSpaceUsage:
                case Metric.TempdbContention:
                    view = ServerViews.DatabasesTempdbView;
                    break;
                case Metric.SSBackupOperation:
                case Metric.SSRestoreOperation:
                    view = ServerViews.DatabasesBackupRestoreHistory;
                    break;
                case Metric.AlwaysOnAvailabilityGroupRoleChange:
                case Metric.AlwaysOnEstimatedDataLossTime:
                case Metric.AlwaysOnSynchronizationHealthState:
                case Metric.AlwaysOnEstimatedRecoveryTime:
                case Metric.AlwaysOnSynchronizationPerformance:
                case Metric.AlwaysOnLogSendQueueSize:
                case Metric.AlwaysOnRedoQueueSize:
                case Metric.AlwaysOnRedoRate:
                case Metric.PreferredNodeUnavailability:
                // Start ID: M1
                // Done changes for Drill-Down Revisions
                case Metric.ClusterActiveNode:
                case Metric.ClusterFailover:
                    view = ServerViews.DatabaseAlwaysOn;
                    break;               
                case Metric.DatabaseLastBackupDate:
                    view = ServerViews.DatabasesBackupRestoreHistory;
                    break;
                case Metric.ReadWriteErrors:
                    view = ServerViews.ResourcesFileActivity;
                    break;
                case Metric.FullTextRefreshHours:
                    view = ServerViews.ServicesFullTextSearch;
                    break;
                case Metric.Operational:
                case Metric.OSMetricsStatus:
                case Metric.RepositoryGroomingTimedOut:
                case Metric.WMIStatus:
                    view = ServerViews.Overview;
                    break;
                case Metric.ClientComputers:
                case Metric.UserConnectionPct:
                    view = ServerViews.OverviewSummary;
                    break;
                // End ID: M1
                default:
                    isViewFound = false;
                    break;
            }

            return isViewFound;
        }

        /// <summary>
        /// Navigate to a view for a given server and metric. If a historical snapshot is specified, navigate to
        /// associated view in history.
        /// </summary>
        public static void NavigateToView(string instanceName, Metric metric, DateTime? historicalSnapshotDateTime)
        {
            if (historicalSnapshotDateTime.HasValue)
                NavigateToView(instanceName, metric, historicalSnapshotDateTime, null);
            else
                NavigateToView(instanceName, metric, null, null);
        }

        public static void NavigateToView(string instanceName, Metric metric, DateTime? historicalSnapshotDateTime, object argument)
        {
            if (!String.IsNullOrEmpty(instanceName))
            {
                foreach (MonitoredSqlServerWrapper wrapper in ApplicationModel.Default.ActiveInstances)
                {
                    if (wrapper.InstanceName.Equals(instanceName))
                    {
                        NavigateToView(wrapper.Id, metric, historicalSnapshotDateTime, argument);
                        Settings.Default.AddRecentServer(wrapper.Id);
                        break;
                    }
                }
            }
            else
            {
                ApplicationController.Default.ShowTodayView();
            }
        }

        /// <summary>
        /// Navigate to a view for a given server and metric.  
        /// </summary>
        public static void NavigateToView(int instanceId, Metric metric, DateTime? historicalSnapshotDateTime, object p_argument)
        {
            if (instanceId != -1)
            {
                ServerViews targetView;
                object argument;

                GetMetricView(instanceId, metric, out targetView, out argument);

                if (null == argument && null != p_argument)
                    argument = p_argument;

                ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
                ApplicationController.Default.ShowServerView(instanceId, targetView, argument, historicalSnapshotDateTime);

                if (!historicalSnapshotDateTime.HasValue)
                {
                    ApplicationController.Default.SetActiveViewToRealTimeMode();
                }
            }
        }

        public static bool ViewSupportsHistoricalSnapshots(Metric metric)
        {
            switch (metric)
            {
                case Metric.BlockingAlert:
                case Metric.BlockedSessions:
                case Metric.OSCPUPrivilegedTimePct:
                case Metric.OSCPUProcessorQueueLength:
                case Metric.OSCPUUsagePct:
                case Metric.SQLCPUUsagePct:
                case Metric.ReadWriteErrors:
                case Metric.OSDiskAverageDiskQueueLength:
                case Metric.OSDiskPhysicalDiskTimePct:
                case Metric.OSMemoryUsagePct:
                case Metric.OSMemoryPagesPerSecond:
                case Metric.SQLMemoryUsagePct:
                case Metric.OldestOpenTransMinutes:
                case Metric.UserConnectionPct:
                case Metric.ClientComputers:
                case Metric.ServerResponseTime:
                case Metric.ResourceAlert:
                case Metric.Custom:
                case Metric.PageLifeExpectancy:
                case Metric.AverageDiskMillisecondsPerRead:
                case Metric.AverageDiskMillisecondsPerTransfer:
                case Metric.AverageDiskMillisecondsPerWrite:
                case Metric.DiskReadsPerSecond:
                case Metric.DiskTransfersPerSecond:
                case Metric.DiskWritesPerSecond:
                case Metric.OSDiskAverageDiskQueueLengthPerDisk:
                case Metric.VersionStoreGenerationRatio:
                case Metric.VersionStoreSize:
                case Metric.LongRunningVersionStoreTransaction:
                case Metric.SessionTempdbSpaceUsage:
                case Metric.TempdbContention:
                case Metric.VmCPUUtilization:
                case Metric.VmESXCPUUtilization:
                case Metric.VmMemoryUtilization:
                case Metric.VmESXMemoryUsage:
                case Metric.VmCPUReadyWaitTime:
                case Metric.VmReclaimedMemory:
                case Metric.VmMemorySwapDelayDetected:
                case Metric.VmESXMemorySwapDetected:
                case Metric.OSDiskPhysicalDiskTimePctPerDisk:
                    return true;
                default:
                    return false;
            }
        }

        public static bool GetMetricHelp(Metric metric, out string helpHtm)
        {
            helpHtm = null;

            bool isHelpFound = true;
            switch (metric)
            {
                case Metric.OSDiskAverageDiskQueueLength:
                case Metric.OSDiskAverageDiskQueueLengthPerDisk:
                    helpHtm = HelpTopics.AlertDiskQueueLength;
                    break;
                case Metric.OSDiskPhysicalDiskTimePct:
                    helpHtm = HelpTopics.AlertDiskTimePercent;
                    break;
                case Metric.OSDiskPhysicalDiskTimePctPerDisk:
                    helpHtm = HelpTopics.AlertOSDiskPhysicalDiskTimePctPerDisk;
                    break;
                case Metric.OSMemoryPagesPerSecond:
                    helpHtm = HelpTopics.AlertMemoryPaging;
                    break;
                case Metric.UserConnectionPct:
                    helpHtm = HelpTopics.AlertUserConnections;
                    break;
                case Metric.DataUsedPct:
                    helpHtm = HelpTopics.AlertDataUsed;
                    break;
                case Metric.LogUsedPct:
                    helpHtm = HelpTopics.AlertLogUsed;
                    break;
                case Metric.ReadWriteErrors:
                    helpHtm = HelpTopics.AlertDatabaseReadWriteError;
                    break;
                case Metric.DatabaseStatus:
                    helpHtm = HelpTopics.AlertDatabaseStatus;
                    break;
                case Metric.DatabaseSizePct:
                    helpHtm = HelpTopics.AlertDatabaseSize;
                    break;
                case Metric.TransLogSize:
                    helpHtm = HelpTopics.AlertTransLogSize;
                    break;
                case Metric.ReorganisationPct:
                    helpHtm = HelpTopics.AlertReorganisation;
                    break;
                case Metric.CLRStatus:
                    helpHtm = HelpTopics.AlertCLRStatus;
                    break;
                case Metric.OLEAutomationStatus:
                    helpHtm = HelpTopics.AlertOleAutomationStatus;
                    break;
                case Metric.OSMetricsStatus:
                    helpHtm = HelpTopics.AlertOSMetricStatus;
                    break;
                case Metric.AgentXPStatus:
                    helpHtm = HelpTopics.AlertAgentXPStatus;
                    break;
                case Metric.WMIStatus:
                    helpHtm = HelpTopics.AlertWMIStatus;
                    break;
                case Metric.QueryMonitorStatus:
                    helpHtm = HelpTopics.AlertQueryMonitorStatus;
                    break;
                case Metric.OSDiskFull:
                    helpHtm = HelpTopics.AlertOSDiskFull;
                    break;
                case Metric.OSMemoryUsagePct:
                    helpHtm = HelpTopics.AlertOSMemoryUsage;
                    break;
                case Metric.OSCPUPrivilegedTimePct:
                    helpHtm = HelpTopics.AlertOSCPUPrivilegedTime;
                    break;
                case Metric.OSUserCPUUsagePct:
                    helpHtm = HelpTopics.AlertOSCPUUserTime;
                    break;
                case Metric.ClientComputers:
                    helpHtm = HelpTopics.AlertClientComputers;
                    break;
                case Metric.OldestOpenTransMinutes:
                    helpHtm = HelpTopics.AlertOldestOpenTrans;
                    break;
                case Metric.ResourceAlert:
                    helpHtm = HelpTopics.AlertSessionCPU;
                    break;
                case Metric.FullTextServiceStatus:
                    helpHtm = HelpTopics.AlertFullTextServiceStatus;
                    break;
                case Metric.NonDistributedTrans:
                    helpHtm = HelpTopics.AlertNonDistributedTrans;
                    break;
                case Metric.JobCompletion:
                    helpHtm = HelpTopics.AlertAgentJobCompletion;
                    break;
                case Metric.BombedJobs:
                    helpHtm = HelpTopics.AlertAgentJobFailures;
                    break;
                case Metric.LongJobsMinutes:
                    helpHtm = HelpTopics.AlertAgentLongRunningJobsInMinutes;
                    break;
                case Metric.LongJobs:
                    helpHtm = HelpTopics.AlertAgentLongRunningJobsPercent;
                    break;
                case Metric.AgentServiceStatus:
                    helpHtm = HelpTopics.AlertAgentServiceStatus;
                    break;
                case Metric.SqlServiceStatus:
                    helpHtm = HelpTopics.AlertSQLServiceStatus;
                    break;
                case Metric.NonSubscribedTransNum:
                    helpHtm = HelpTopics.AlertNonSubscribedTransNum;
                    break;
                case Metric.NonSubscribedTransTime:
                    helpHtm = HelpTopics.AlertNonSubscribedTransTime;
                    break;
                case Metric.BlockingAlert:
                    helpHtm = HelpTopics.AlertBlocking;
                    break;
                case Metric.OSCPUProcessorQueueLength:
                    helpHtm = HelpTopics.AlertOSCPUProcessorQueueLength;
                    break;
                case Metric.OSCPUUsagePct:
                    helpHtm = HelpTopics.AlertOSCPUUsage;
                    break;
                case Metric.SQLCPUUsagePct:
                    helpHtm = HelpTopics.AlertSQLCPUUsage;
                    break;
                case Metric.SQLMemoryUsagePct:
                    helpHtm = HelpTopics.AlertSQLMemoryUsage;
                    break;
                case Metric.DtcServiceStatus:
                    helpHtm = HelpTopics.AlertDTCServiceStatus;
                    break;
                case Metric.BlockedSessions:
                    helpHtm = HelpTopics.AlertBlockedSessions;
                    break;
                case Metric.ServerResponseTime:
                    helpHtm = HelpTopics.AlertServerResponseTime;
                    break;
                case Metric.MirroringWitnessConnection:
                    helpHtm = HelpTopics.AlertMirroringWitnessConnection;
                    break;
                case Metric.MirrorCommitOverhead:
                    helpHtm = HelpTopics.AlertMirrorCommitOverhead;
                    break;
                case Metric.MirroringSessionNonPreferredConfig:
                    helpHtm = HelpTopics.AlertMirroringSessionPreferredRole;
                    break;
                case Metric.MirroringSessionRoleChange:
                    helpHtm = HelpTopics.AlertMirroredServerRoleChange;
                    break;
                case Metric.MirroringSessionsStatus:
                    helpHtm = HelpTopics.AlertMirroringStatus;
                    break;
                case Metric.UnsentLogThreshold:
                    helpHtm = HelpTopics.AlertMirroringUnsentLog;
                    break;
                case Metric.UnrestoredLog:
                    helpHtm = HelpTopics.AlertMirroringUnrestoredLog;
                    break;
                case Metric.OldestUnsentMirroringTran:
                    helpHtm = HelpTopics.AlertMirroringOldestUnsent;
                    break;
                case Metric.AgentLog:
                    helpHtm = HelpTopics.AlertSQLServerAgentLog;
                    break;
                case Metric.ErrorLog:
                    helpHtm = HelpTopics.AlertSQLServerErrorLog;
                    break;
                case Metric.Deadlock:
                    helpHtm = HelpTopics.AlertDeadlock;
                    break;
                case Metric.ProcCacheHitRatio:
                    helpHtm = HelpTopics.AlertProcCacheHitRatio;
                    break;
                case Metric.PageLifeExpectancy:
                    helpHtm = HelpTopics.AlertPageLifeExpectancy;
                    break;
                case Metric.ClusterActiveNode:
                    helpHtm = HelpTopics.AlertClusterActiveNode;
                    break;
                case Metric.ClusterFailover:
                    helpHtm = HelpTopics.AlertClusterFailover;
                    break;
                case Metric.NonDistributedTransTime:
                    helpHtm = HelpTopics.AlertReplicationDistributionLatency;
                    break;
                case Metric.AverageDiskMillisecondsPerRead:
                    helpHtm = HelpTopics.AlertAvgDiskSecPerRead;
                    break;
                case Metric.AverageDiskMillisecondsPerWrite:
                    helpHtm = HelpTopics.AlertAvgDiskSecPerWrite;
                    break;
                case Metric.AverageDiskMillisecondsPerTransfer:
                    helpHtm = HelpTopics.AlertAvgDiskSecPerTransfer;
                    break;
                case Metric.DiskReadsPerSecond:
                    helpHtm = HelpTopics.AlertAvgDiskReadsPerSecond;
                    break;
                case Metric.DiskTransfersPerSecond:
                    helpHtm = HelpTopics.AlertAvgDiskTransfersPerSecond;
                    break;
                case Metric.DiskWritesPerSecond:
                    helpHtm = HelpTopics.AlertAvgDiskWritesPerSecond;
                    break;
                case Metric.VersionStoreGenerationRatio:
                    helpHtm = HelpTopics.AlertVersionStoreGenerationRatio;
                    break;
                case Metric.VersionStoreSize:
                    helpHtm = HelpTopics.AlertVersionStoreSize;
                    break;
                case Metric.LongRunningVersionStoreTransaction:
                    helpHtm = HelpTopics.AlertLongRunningVersionStoreTransaction;
                    break;
                case Metric.SessionTempdbSpaceUsage:
                    helpHtm = HelpTopics.AlertSessionTempdbSpaceUsage;
                    break;
                case Metric.TempdbContention:
                    helpHtm = HelpTopics.AlertTempdbContention;
                    break;
                case Metric.LogFileAutogrow:
                    helpHtm = HelpTopics.AlertLogFileAutogrow;
                    break;
                case Metric.DataFileAutogrow:
                    helpHtm = HelpTopics.AlertDataFileAutogrow;
                    break;
                case Metric.SQLdmCollectionServiceStatus:
                    helpHtm = HelpTopics.AlertSQLdmServiceStatus;
                    break;
                case Metric.MaintenanceMode:
                    helpHtm = HelpTopics.AlertMaintenanceMode;
                    break;
                case Metric.VmESXCPUUtilization:
                    helpHtm = HelpTopics.AlertESXCPUUtilization;
                    break;
                case Metric.VmESXMemorySwapDetected:
                    helpHtm = HelpTopics.AlertESXMemorySwapDetected;
                    break;
                case Metric.VmESXMemoryUsage:
                    helpHtm = HelpTopics.AlertESXMemoryUsage;
                    break;
                case Metric.VmESXPowerState:
                    helpHtm = HelpTopics.AlertESXPowerState;
                    break;
                case Metric.VmCPUReadyWaitTime:
                    helpHtm = HelpTopics.AlertVMCPUReadyWaitTime;
                    break;
                case Metric.VmCPUUtilization:
                    helpHtm = HelpTopics.AlertVMCPUUsage;
                    break;
                case Metric.VmHostServerChange:
                    helpHtm = HelpTopics.AlertVMHostServerChange;
                    break;
                case Metric.VmMemorySwapDelayDetected:
                    helpHtm = HelpTopics.AlertVMMemorySwapDelayDetected;
                    break;
                case Metric.VmMemoryUtilization:
                    helpHtm = HelpTopics.AlertVMMemoryUsagePercent;
                    break;
                case Metric.VmPowerState:
                    helpHtm = HelpTopics.AlertVMPowerState;
                    break;
                case Metric.VmReclaimedMemory:
                    helpHtm = HelpTopics.AlertVMReclaimedBalloonedMemory;
                    break;
                case Metric.VmConfigChange:
                    helpHtm = HelpTopics.AlertVMResourceConfigurationChange;
                    break;
                case Metric.VmResourceLimits:
                    helpHtm = HelpTopics.AlertVMResourceLimitsDetected;
                    break;
                case Metric.DatabaseSizeMb:
                    helpHtm = HelpTopics.AlertDatabaseFullSize;
                    break;
                case Metric.TransLogSizeMb:
                    helpHtm = HelpTopics.AlertTranLogFullSize;
                    break;
                case Metric.OsDiskFreeMb:
                    helpHtm = HelpTopics.AlertOsDiskFreeSize;
                    break;
                case Metric.AlwaysOnAvailabilityGroupRoleChange:
                    helpHtm = HelpTopics.AlertAlwaysOnAvailabilityGroupRoleChange;
                    break;
                case Metric.AlwaysOnEstimatedDataLossTime:
                    helpHtm = HelpTopics.AlertAlwaysOnEstimatedDataLossTime;
                    break;
                case Metric.AlwaysOnSynchronizationHealthState:
                    helpHtm = HelpTopics.AlertAlwaysOnSynchronizationHealthState;
                    break;
                case Metric.AlwaysOnEstimatedRecoveryTime:
                    helpHtm = HelpTopics.AlertAlwaysOnEstimatedRecoveryTime;
                    break;
                case Metric.AlwaysOnSynchronizationPerformance:
                    helpHtm = HelpTopics.AlertAlwaysOnSynchronizationPerformance;
                    break;
                case Metric.AlwaysOnLogSendQueueSize:
                    helpHtm = HelpTopics.AlertAlwaysOnLogSendQueueSize;
                    break;
                case Metric.AlwaysOnRedoQueueSize:
                    helpHtm = HelpTopics.AlertAlwaysOnRedoQueueSize;
                    break;
                case Metric.AlwaysOnRedoRate:
                    helpHtm = HelpTopics.AlertAlwaysOnRedoRate;
                    break;
                //[START] SQLdm9.0 (Gaurav Karwal): adding help link for the preferred node alert
                case Metric.PreferredNodeUnavailability:
                    helpHtm = HelpTopics.AlertAlwaysOnPreferredNode;
                    break;
                //[END] SQLdm9.0 (Gaurav Karwal): adding help link for the preferred node alert
                //START: SQLdm 9.1 (Abhishek Joshi) -Wiki Links --adding wiki links for the file group and new services alerts
                case Metric.FilegroupSpaceFullPct:
                    helpHtm = HelpTopics.AlertFilegroupPercent;
                    break;
                case Metric.FilegroupSpaceFullSize:
                    helpHtm = HelpTopics.AlertFilegroupSize;
                    break;
                case Metric.SQLActiveDirectoryHelperServiceStatus:
                    helpHtm = HelpTopics.AlertSQLActiveDirectoryHelper;
                    break;
                case Metric.SQLBrowserServiceStatus:
                    helpHtm = HelpTopics.AlertSQLBrowser;
                    break;
                //END: SQLdm 9.1 (Abhishek Joshi) -Wiki Links --adding wiki links for the file group and new services alerts
                // SQLdm 10.0 (Vandana Gogna) - Database backup alerts
                case Metric.DatabaseLastBackupDate:
                    helpHtm = HelpTopics.AlertLastBackup;
                    break;
                case Metric.RepositoryGroomingTimedOut:
                    helpHtm = HelpTopics.AlertGroomingTimedOut;
                    break;
                default:
                    helpHtm = HelpTopics.AlertDefault;
                    //isHelpFound = false;
                    break;

            }

            return isHelpFound;
        }
    }
}
