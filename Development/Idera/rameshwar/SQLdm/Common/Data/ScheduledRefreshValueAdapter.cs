//------------------------------------------------------------------------------
// <copyright file="ScheduledRefreshValueAdapter.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Thresholds;

namespace Idera.SQLdm.Common.Data
{
    using System;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Snapshots.AlwaysOn;
    using System.Collections.Generic;

    public class ScheduledRefreshValueAdapter
    {
        private static readonly Logger LOG = Logger.GetLogger(typeof(ScheduledRefreshValueAdapter));

        private ScheduledRefresh scheduledRefresh;
        private OpenTransaction oldestTransaction;
        private DatabaseSizeSnapshot dbRefresh;

        //OsStatisticAvailability possible texts
        private const string ServiceAvailableText = "available";
        private const string ServiceTimedOutText = "service timedout";
        private const string ServiceUnavailableText = "service unavailable";
        private const string ServiceDisabledText = "disabled";
        //Start - SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
        private BaselineMetricMeanCollection baselineMetricMean = null;
        //END - SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values

        public ScheduledRefreshValueAdapter(AlertableSnapshot snapshot)
        {
            if (snapshot is DatabaseSizeSnapshot)
            {
                dbRefresh = snapshot as DatabaseSizeSnapshot;
            }

            else
            {
                scheduledRefresh = snapshot as ScheduledRefresh;
            }

        }
        public ScheduledRefreshValueAdapter(AlertableSnapshot snapshot, BaselineMetricMeanCollection baselineMeanList)
        {
            if (snapshot is DatabaseSizeSnapshot)
            {
                dbRefresh = snapshot as DatabaseSizeSnapshot;
            }

            else
            {
                scheduledRefresh = snapshot as ScheduledRefresh;
            }
            this.baselineMetricMean = baselineMeanList;
        }

        //START: SQLdm 10.0 (Tarun Sapra)- getting values from adapter instead of processor
        public IComparable GetVmValue(Metric metric, bool isBaselineEnabled = false)
        {
            double? baselineMean = null;            
            try
            {
                if (baselineMetricMean != null)
                    baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out baselineMean);
                switch (metric)
                {
                    case Metric.VmCPUUtilization:
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.VMConfig.PerfStats.VmCPUUtilizationBaselineMean = baselineMean;
                            return scheduledRefresh.Server.VMConfig.PerfStats.VmCPUUtilizationAsBaselinePerc = CalculatePercentageOfBaseline(baselineMean, scheduledRefresh.Server.VMConfig.PerfStats.CpuUsage);
                        }
                        return scheduledRefresh.Server.VMConfig.PerfStats.CpuUsage;
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error getting value for metric {0}.\n{1}", metric, e.ToString());
            }
            return null;
        }
        //END: SQLdm 10.0 (Tarun Sapra)- getting values from adapter instead of processor        

        public IComparable GetServerValue(Metric metric, out double? collectedMetricValue, bool isBaselineEnabled = false)//SQLdm 10.0 (Tarun Sapra)- Added a default param for baselined threshold flag 
        {
            ServiceState? serviceState;
            IComparable comparablevalue;
            double? dblvalue;
            long? longvalue;
            int? intvalue;
            bool? boolvalue;
            double? baselineMean = null;
            collectedMetricValue = null;

            try
            {
                if (baselineMetricMean != null)
                {
                    if(dbRefresh != null || scheduledRefresh != null)
                        baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh == null ? (AlertableSnapshot)dbRefresh : (AlertableSnapshot)scheduledRefresh, (int)metric, out baselineMean);
                }
                switch (metric)
                {
                    case Metric.SQLCPUUsagePct:
                        collectedMetricValue = scheduledRefresh.Server.Statistics.CpuPercentage;
                        //START: SQLdm 10.0 (Srishti)- Calc fields when thresholds are of baselined type
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.Statistics.CpuBaseline = baselineMean;
                            scheduledRefresh.Server.Statistics.CpuBaselinePercentage = CalculatePercentageOfBaseline(baselineMean, collectedMetricValue);
                            dblvalue = scheduledRefresh.Server.Statistics.CpuBaselinePercentage;
                        }
                        //END: SQLdm 10.0 (Srishti)- Calc fields when thresholds are of baselined type
                        else
                            dblvalue = scheduledRefresh.Server.Statistics.CpuPercentage;
                        return dblvalue != null ? (IComparable)Math.Round(dblvalue.Value, 3) : null;
                    case Metric.UserConnectionPct:
                        longvalue = scheduledRefresh.Server.MaxConnections;
                        if (longvalue == null)
                            return null;
                        intvalue = scheduledRefresh.Server.SystemProcesses.CurrentUserProcesses;
                        if (intvalue == null)
                            return null;
                        collectedMetricValue = (((double)intvalue.Value * 100) / longvalue.Value);
                        //START: SQLdm 10.0 (Srishti)- Calc fields when thresholds are of baselined type
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.SystemProcesses.UserConnectionPctBaselineMean = baselineMean;
                            return scheduledRefresh.Server.SystemProcesses.UserConnectionPctAsBaselinePerc = CalculatePercentageOfBaseline(baselineMean, collectedMetricValue);
                        }
                        //END: SQLdm 10.0 (Srishti)- Calc fields when thresholds are of baselined type
                        else
                            return (((double)intvalue.Value * 100) / longvalue.Value);
                    case Metric.HighestThreadsPct:
                        return 0.0d; // TODO: Need HighestThreadsPct value
                    case Metric.CurrentThreadsPct:
                        return 0.0d; // TODO: Need CurrentThreadsPct value
                    case Metric.NonDistributedTrans:
                        longvalue = scheduledRefresh.Replication.NonDistributedTransactions;
                        if (longvalue != null)
                            collectedMetricValue = longvalue.Value;
                        //START: SQLdm 10.0 (Tarun Sapra)- Calc fields when thresholds are of baselined type
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Replication.NonDistributedTransactionsBaselineMean = baselineMean;
                            return (IComparable)(scheduledRefresh.Replication.NonDistributedTransactionsBaselinePerc =
                                (longvalue == null ? null : CalculatePercentageOfBaseline(baselineMean, longvalue.Value))
                                );
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- Calc fields when thresholds are of baselined type
                        else
                            return longvalue != null ? (IComparable)longvalue.Value : null;
                    case Metric.NonDistributedTransTime:
                        dblvalue = scheduledRefresh.Replication.Publisher.ReplicationLatency;
                        return dblvalue != null ? (IComparable)dblvalue.Value : null;
                    case Metric.NonSubscribedTransNum:
                        longvalue = scheduledRefresh.Replication.NonSubscribedTransactions;
                        if (longvalue != null)
                            collectedMetricValue = longvalue.Value;
                        //START: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baselines
                        if (isBaselineEnabled)
                        {
                            LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                            double? meanValue;
                            if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                            {
                                scheduledRefresh.Replication.NonSubscribedTransactionsBaselineMean = meanValue;
                                return (IComparable)(scheduledRefresh.Replication.NonSubscribedTransactionsAsBaselinePerc =
                                    (longvalue == null ? null : CalculatePercentageOfBaseline(baselineMean, longvalue.Value))
                                    );
                            }
                            else
                            {
                                LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                return null;
                            }
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baselines
                        return longvalue != null ? (IComparable)longvalue.Value : null;
                    case Metric.NonSubscribedTransTime:
                        dblvalue = scheduledRefresh.Replication.SubscriptionLatency.TotalSeconds;
                        collectedMetricValue = dblvalue;
                        //START: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baselines
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Replication.SubscriptionLatencyBaselineMean = baselineMean;

                            return (IComparable)(scheduledRefresh.Replication.SubscriptionLatencyBaselinePerc =
                                (dblvalue == null ? null : CalculatePercentageOfBaseline(baselineMean, dblvalue))
                                );
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baselines
                        return dblvalue;
                    case Metric.ReadWriteErrors:
                        longvalue = scheduledRefresh.Server.Statistics.DiskErrors;
                        return longvalue != null ? (IComparable)longvalue.Value : null;
                    case Metric.SQLMemoryUsagePct: // total memory / sql memory used
                        FileSize total = scheduledRefresh.Server.PhysicalMemory;
                        if (total.Bytes == null)
                            return null;
                        FileSize used = scheduledRefresh.Server.TotalServerMemory;
                        if (used.Bytes == null)
                            return null;
                        int ratio = (int)Math.Ceiling((double)(used.Bytes.Value / total.Bytes.Value * 100.0m));
                        collectedMetricValue = ratio <= 100 ? ratio : 100;
                        //START: SQLdm 10.0 (Tarun Sapra)- fill the fields for baseline mean and perc
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.SQlMemoryUsageBaselineMean = baselineMean;
                            return scheduledRefresh.Server.SqlMemUsagesPercBaseline = CalculatePercentageOfBaseline(baselineMean, (ratio <= 100 ? ratio : 100) / 100);
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- fill the fields for baseline mean and perc
                        return ratio <= 100 ? ratio : 100;
                    case Metric.DMStart:
                    case Metric.DMStop:
                    case Metric.IndexRowHits:
                        return 5; // display only metric - used for highlighting in the ui
                    case Metric.FullTextRefreshHours:
                        return 1; // display only metric - used for highlighting in the ui
                    case Metric.ServerResponseTime:
                        collectedMetricValue = scheduledRefresh.Server.ResponseTime;
                        //START: SQLdm 10.0 (Tarun Sapra)- fill the fields for baseline mean and perc
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.ResponseTimeBaselineMean = baselineMean;
                            return scheduledRefresh.Server.ResponseTimePercDeviationBaseine = CalculatePercentageOfBaseline(baselineMean, scheduledRefresh.Server.ResponseTime);
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- fill the fields for baseline mean and perc
                        return scheduledRefresh.Server.ResponseTime;
                    case Metric.OldestOpenTransMinutes:
                        if (GetOldestOpenTransaction() == null)
                            return null;
                        if (oldestTransaction.RunTime == null)
                            return null;
                        return oldestTransaction.RunTime.Value.TotalMinutes;
                    case Metric.ClientComputers:
                        intvalue = scheduledRefresh.Server.SystemProcesses.ComputersHoldingProcesses;
                        if (intvalue != null)
                            collectedMetricValue = intvalue.Value;
                        //START: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baselines
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.SystemProcesses.ComputersHoldingProcessesBaselineMean = baselineMean;
                            return scheduledRefresh.Server.SystemProcesses.ComputersHoldingProcessesAsBaselinePerc = CalculatePercentageOfBaseline(baselineMean, intvalue.Value);
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baselines	
                        return intvalue == null ? (IComparable)null : intvalue.Value;
                    case Metric.BlockedSessions:
                        //intvalue = scheduledRefresh.Server.SystemProcesses.BlockedProcesses;
                        intvalue = scheduledRefresh.BlockedSpids.Count > scheduledRefresh.Server.SystemProcesses.BlockedProcesses ?
                            scheduledRefresh.BlockedSpids.Count :
                            scheduledRefresh.Server.SystemProcesses.BlockedProcesses;
                        if (intvalue != null)
                            collectedMetricValue = intvalue.Value;
                        //START: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baselines
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.SystemProcesses.BlockedProcessesBaselineMean = baselineMean;
                            return scheduledRefresh.Server.SystemProcesses.BlockedProcessesAsBaselinePerc = CalculatePercentageOfBaseline(baselineMean, intvalue.Value);
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- Baseline mean and perc of baselines
                        return intvalue == null ? (IComparable)null : intvalue.Value;
                    case Metric.DataUsedPct:
                        if (dbRefresh == null || dbRefresh.DatabaseSummary == null || dbRefresh.DatabaseSummary.DataFileSpaceUsed == null || dbRefresh.DatabaseSummary.DataFileSpaceAllocated == null)
                            return null;
                        used = dbRefresh.DatabaseSummary.DataFileSpaceUsed;
                        if (used.Bytes == null)
                            return null;
                        FileSize allocated = dbRefresh.DatabaseSummary.DataFileSpaceAllocated;
                        if (allocated.Bytes == null)
                            return null;
                        ratio = (int)Math.Ceiling((double)(used.Bytes.Value / allocated.Bytes.Value * 100.0m));
                        collectedMetricValue = ratio <= 100 ? ratio : 100;
                        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                        if (isBaselineEnabled)
                        {
                            dbRefresh.DatabaseSummary.DataUsedPctBaselineMean = baselineMean;
                            return dbRefresh.DatabaseSummary.DataUsedPctAsBaselinePerc = CalculatePercentageOfBaseline(baselineMean, (ratio <= 100 ? ratio : 100) / 100);
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                        return ratio <= 100 ? ratio : 100;
                    case Metric.LogUsedPct:
                        if (dbRefresh == null || dbRefresh.DatabaseSummary == null || dbRefresh.DatabaseSummary.LogFileSpaceUsed == null || dbRefresh.DatabaseSummary.LogFileSpaceAllocated == null)
                            return null;
                        used = dbRefresh.DatabaseSummary.LogFileSpaceUsed;
                        if (used.Bytes == null)
                            return null;
                        allocated = dbRefresh.DatabaseSummary.LogFileSpaceAllocated;
                        if (allocated.Bytes == null)
                            return null;
                        ratio = (int)Math.Ceiling((double)(used.Bytes.Value / allocated.Bytes.Value * 100.0m));
                        collectedMetricValue = ratio <= 100 ? ratio : 100;
                        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                        if (isBaselineEnabled)
                        {
                            dbRefresh.DatabaseSummary.LogUsedPctBaselineMean = baselineMean;
                            return dbRefresh.DatabaseSummary.LogUsedPctAsBaselinePerc = CalculatePercentageOfBaseline(baselineMean, (ratio <= 100 ? ratio : 100) / 100);
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                        return ratio <= 100 ? ratio : 100;
                    case Metric.ProcCacheHitRatio:
                        dblvalue = scheduledRefresh.Server.Statistics.CacheHitRatio;
                        if(dblvalue != null)
                            collectedMetricValue = Math.Round(dblvalue.Value, 3);
                        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.Statistics.ProcCacheHitRatioBaselineMean = baselineMean;
                            scheduledRefresh.Server.Statistics.ProcCacheHitRatioAsBaselinePerc = (dblvalue == null ? null : CalculatePercentageOfBaseline(baselineMean, dblvalue.Value));
                            return scheduledRefresh.Server.Statistics.ProcCacheHitRatioAsBaselinePerc != null ? (IComparable)Math.Round(scheduledRefresh.Server.Statistics.ProcCacheHitRatioAsBaselinePerc.Value, 3) : null;
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                        return dblvalue != null ? (IComparable)Math.Round(dblvalue.Value, 3) : null;
                    case Metric.ResourceAlert:
                    case Metric.BlockingAlert:
                    case Metric.LongJobs:
                    case Metric.LongJobsMinutes:
                    case Metric.BombedJobs:
                        return null;
                    case Metric.MaintenanceMode:
                        return scheduledRefresh.MonitoredServer.MaintenanceModeEnabled ?
                            OptionStatus.Enabled : OptionStatus.Disabled;
                    case Metric.ServerOpenTransactions:
                        intvalue = scheduledRefresh.Server.SystemProcesses.OpenTransactions;
                        return intvalue != null ? (IComparable)intvalue.Value : null;
                    case Metric.CLRStatus:
                        boolvalue = scheduledRefresh.Alerts.ClrEnabled;
                        return boolvalue != null ? (IComparable)(boolvalue.Value ? OptionStatus.Enabled : OptionStatus.Disabled) : null;
                    case Metric.OLEAutomationStatus:
                        comparablevalue = GetServerOSValue(Metric.OSMetricsStatus, out collectedMetricValue);
                        if (comparablevalue == null)
                            return null;
                        return (comparablevalue.CompareTo(OSMetricsStatus.OLEAutomationUnavailable) == 0 ? OptionStatus.Disabled : OptionStatus.Enabled);
                    case Metric.QueryMonitorStatus:
                        return scheduledRefresh.QueryMonitorStatements.Count;
                    case Metric.AgentXPStatus:
                        boolvalue = scheduledRefresh.Alerts.AgentXpEnabled;
                        return boolvalue != null ? (IComparable)(boolvalue.Value ? OptionStatus.Enabled : OptionStatus.Disabled) : null;
                    case Metric.WMIStatus:
                        comparablevalue = GetServerOSValue(Metric.OSMetricsStatus, out collectedMetricValue);
                        if (comparablevalue == null)
                            return null;
                        return (comparablevalue.CompareTo(OSMetricsStatus.WMIServiceUnreachable) == 0 ? OptionStatus.Disabled : OptionStatus.Enabled);
                    case Metric.AgentServiceStatus:
                        serviceState = scheduledRefresh.Server.AgentServiceStatus;
                        return serviceState != null ? (IComparable)serviceState.Value : null;
                    case Metric.DtcServiceStatus:
                        serviceState = scheduledRefresh.Server.DtcServiceStatus;
                        return serviceState != null ? (IComparable)serviceState.Value : null;
                    case Metric.FullTextServiceStatus:
                        serviceState = scheduledRefresh.Server.FullTextServiceStatus;
                        return serviceState != null ? (IComparable)serviceState.Value : null;
                    case Metric.SqlServiceStatus:
                        serviceState = scheduledRefresh.Server.SqlServiceStatus;
                        LOG.Debug("25629 log: Metric.SqlServiceStatus " + serviceState + " Server name: " + scheduledRefresh.ServerName);
                        if (serviceState == ServiceState.UnableToMonitor && scheduledRefresh.MonitoredServer.CloudProviderId != null)//SQLdm 10.0 (Tarun Sapra)- For sql server hosted on cloud, we need to bypass the collection failed flag for now
                            return ServiceState.Running;
                        // SQLDM 10.1 Barkha Khatri SQLDM-25629 fix - adding a new state TruncatedFunctionalityAvailable
                        //else if (serviceState == ServiceState.UnableToMonitor && scheduledRefresh.MonitoredServer.IsUserSysAdmin != true)//SQLdm 10.1 (Barkha Khatri)- Suppressing unable to monitor alert for non-admin
                        //{
                        //    LOG.Debug("25629 log:in sys admin check blog --returned state:TruncatedFunctionalityAvailable");
                        //    return ServiceState.TruncatedFunctionalityAvailable;
                        //}
                        else if (serviceState == null || serviceState.Value == ServiceState.Running)
                        {
                            if (scheduledRefresh.CollectionFailed || scheduledRefresh.Server.CollectionFailed)
                            {
                                LOG.Debug("25629 log: collection service failed block-- returned state:UnableToMonitor");
                                return ServiceState.UnableToMonitor;
                            }
                        }

                        if (!scheduledRefresh.CollectionFailed && !scheduledRefresh.Server.CollectionFailed)
                        {
                            LOG.Debug("Resetting Server State to running in case of Permission Failures but no collection failures");
                            scheduledRefresh.Server.SqlServiceStatus = ServiceState.Running;
                            return ServiceState.Running;
                        }
                        LOG.Debug("25629 log:  returned state:" + serviceState.Value);
                        return serviceState.Value;
                    //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get server value for the new SQL services
                    case Metric.SQLBrowserServiceStatus:
                        serviceState = scheduledRefresh.Server.SQLBrowserServiceStatus;
                        return serviceState != null ? (IComparable)serviceState.Value : null;
                    case Metric.SQLActiveDirectoryHelperServiceStatus:
                        serviceState = scheduledRefresh.Server.SQLActiveDirectoryHelperServiceStatus;
                        return serviceState != null ? (IComparable)serviceState.Value : null;
                    //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get server value for the new SQL services
                    // os metrics
                    case Metric.OSMetricsStatus:
                    case Metric.OSCPUPrivilegedTimePct:
                    case Metric.OSCPUProcessorQueueLength:
                    case Metric.OSCPUUsagePct:
                    case Metric.OSDiskAverageDiskQueueLength:
                    case Metric.OSDiskPhysicalDiskTimePct:
                    case Metric.OSMemoryUsagePct:
                    case Metric.OSMemoryPagesPerSecond:
                    case Metric.OSUserCPUUsagePct:

                        return GetServerOSValue(metric, out collectedMetricValue, isBaselineEnabled);//SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                    case Metric.PageLifeExpectancy:
                        collectedMetricValue = scheduledRefresh.Server.Statistics.PageLifeExpectancySeconds;
                        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                        if (isBaselineEnabled)
                        {
                            scheduledRefresh.Server.Statistics.PageLifeExpectancyBaselineMean = baselineMean;
                            return scheduledRefresh.Server.Statistics.PageLifeExpectancyAsBaselinePerc = CalculatePercentageOfBaseline(baselineMean, scheduledRefresh.Server.Statistics.PageLifeExpectancySeconds);
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
                        return scheduledRefresh.Server.Statistics.PageLifeExpectancySeconds;
                    case Metric.VersionStoreGenerationRatio:
                        if (scheduledRefresh != null && scheduledRefresh.Server != null
                            && scheduledRefresh.Server.TempdbStatistics != null
                            && scheduledRefresh.Server.TempdbStatistics.VersionStoreCleanupKilobytes.HasValue
                            && scheduledRefresh.Server.TempdbStatistics.VersionStoreGenerationKilobytes.HasValue
                            && scheduledRefresh.Server.TimeDelta.HasValue
                            && scheduledRefresh.Server.TimeDelta.Value.TotalSeconds >= 60)
                        {
                            if (scheduledRefresh.Server.TempdbStatistics.VersionStoreCleanupKilobytes.Value == 0
                                && scheduledRefresh.Server.TempdbStatistics.VersionStoreGenerationKilobytes.Value > 0)
                                return 9999;


                            var genPerSecond = scheduledRefresh.Server.TempdbStatistics.VersionStoreGenerationKilobytes.Value
                                                / (decimal)scheduledRefresh.Server.TimeDelta.Value.TotalSeconds;
                            var cleanPerSecond = scheduledRefresh.Server.TempdbStatistics.VersionStoreCleanupKilobytes.Value
                                                    / (decimal)scheduledRefresh.Server.TimeDelta.Value.TotalSeconds;

                            if (cleanPerSecond == 0)
                                return 9999;

                            return ((genPerSecond / cleanPerSecond) - 1) * 100;


                        }
                        break;
                    case Metric.VersionStoreSize:
                        if (scheduledRefresh != null) return scheduledRefresh.VersionStoreSize.Megabytes;
                        break;
                    case Metric.TempdbContention:
                        if (scheduledRefresh.Server != null && scheduledRefresh.Server.TempdbStatistics != null)
                        {
                            return (scheduledRefresh.Server.TempdbStatistics.TempdbPFSWaitTime != null ? scheduledRefresh.Server.TempdbStatistics.TempdbPFSWaitTime.TotalMilliseconds : 0)
                            + (scheduledRefresh.Server.TempdbStatistics.TempdbGAMWaitTime != null ? scheduledRefresh.Server.TempdbStatistics.TempdbGAMWaitTime.TotalMilliseconds : 0)
                            + (scheduledRefresh.Server.TempdbStatistics.TempdbSGAMWaitTime != null ? scheduledRefresh.Server.TempdbStatistics.TempdbSGAMWaitTime.TotalMilliseconds : 0);
                        }
                        break;

                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error getting value for metric {0}.\n{1}", metric, e.ToString());
            }

            return null;
        }

        public object GetServerAdditionalValue(Metric metric)
        {
            try
            {
                switch (metric)
                {

                    // Version Store Data
                    case Metric.VersionStoreGenerationRatio:
                    case Metric.VersionStoreSize:
                        return GetVersionStoreAdditionalValue(scheduledRefresh);
                    case Metric.UserConnectionPct:
                        return GetUserConnectPctAdditionalValue(scheduledRefresh);
                    case Metric.TempdbContention:
                        return GetTempdbContentionAdditionalValue(scheduledRefresh);
                    // All Other Metrics
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error getting additional data for metric {0}.\n{1}", metric, e.ToString());
                return null;
            }
        }

        private OpenTransaction GetOldestOpenTransaction()
        {
            if (oldestTransaction == null)
            {
                foreach (OpenTransaction xa in scheduledRefresh.Alerts.OldestOpenTransactions)
                {
                    if (oldestTransaction == null)
                        oldestTransaction = xa;
                    else
                    {
                        if (xa.StartTime < oldestTransaction.StartTime)
                            oldestTransaction = xa;
                    }
                }
            }
            return oldestTransaction;
        }

        public static object[] GetUserConnectPctAdditionalValue(ScheduledRefresh scheduledRefresh)
        {
            object[] data = new object[] { 0 };

            if (scheduledRefresh != null)
            {
                if (scheduledRefresh.Server != null && scheduledRefresh.Server.SystemProcesses != null &&
                    scheduledRefresh.Server.SystemProcesses.CurrentUserProcesses.HasValue)
                {
                    data[0] = scheduledRefresh.Server.SystemProcesses.CurrentUserProcesses.Value;
                }
                else
                {
                    data[0] = 0;
                }
            }
            return data;
        }

        /// <summary>
        /// additional data for tempdb contention
        /// </summary>
        /// <param name="scheduledRefresh"></param>
        /// <returns></returns>
        public static object[] GetTempdbContentionAdditionalValue(ScheduledRefresh scheduledRefresh)
        {
            object[] data = new object[] { 0, 0, 0 };

            if (scheduledRefresh != null)
            {
                ServerOverview server = ((ScheduledRefresh)scheduledRefresh).Server;

                if (server == null) return data;

                if (server.TempdbStatistics != null)
                {
                    data[0] = server.TempdbStatistics.TempdbPFSWaitTime.TotalMilliseconds;
                    data[1] = server.TempdbStatistics.TempdbGAMWaitTime.TotalMilliseconds;
                    data[2] = server.TempdbStatistics.TempdbSGAMWaitTime.TotalMilliseconds;
                }
            }
            return data;
        }

        public static object[] GetVersionStoreAdditionalValue(ScheduledRefresh scheduledRefresh)
        {
            object[] tempdbData = new object[] { 0, 0, 0, 0 };
            if (scheduledRefresh != null)
            {
                if (scheduledRefresh.Server != null && scheduledRefresh.Server.TempdbStatistics != null &&
                    scheduledRefresh.Server.TempdbStatistics.VersionStoreGenerationKilobytes.HasValue &&
                    scheduledRefresh.Server.TempdbStatistics.VersionStoreCleanupKilobytes.HasValue
                    && scheduledRefresh.Server.TimeDelta.HasValue && (decimal)scheduledRefresh.Server.TimeDelta.Value.TotalSeconds != 0)
                {

                    tempdbData[0] = scheduledRefresh.Server.TempdbStatistics.VersionStoreGenerationKilobytes.Value
                                    / (decimal)scheduledRefresh.Server.TimeDelta.Value.TotalSeconds;
                    // current generation rate in kb/s
                    tempdbData[1] = scheduledRefresh.Server.TempdbStatistics.VersionStoreCleanupKilobytes.Value
                                    / (decimal)scheduledRefresh.Server.TimeDelta.Value.TotalSeconds;
                    ; // current cleanup rate in kb/s
                }

                tempdbData[2] = scheduledRefresh.VersionStoreSize.Megabytes; // version store size in MB
                tempdbData[3] = scheduledRefresh.TempdbFileSize.Megabytes; // tempdb size in MB

            }
            return tempdbData;
        }

        public IComparable GetServerOSValue(Metric metric, out double? collectedValue, bool isBaselineEnabled = false)//SQLdm 10.0 (Tarun Sapra)- Added a default param for baselined thresholds
        {
            OSMetrics osMetrics = scheduledRefresh.Server.OSMetricsStatistics;
            collectedValue = null;
            if (osMetrics != null)
            {
                try
                {
                    switch (metric)
                    {
                        case Metric.OSMetricsStatus:
                            switch (osMetrics.OsStatisticAvailability)
                            {
                                case ServiceAvailableText:
                                    return OSMetricsStatus.Available;
                                case ServiceDisabledText:
                                    return OSMetricsStatus.Disabled;
                                case ServiceUnavailableText:
                                    return OSMetricsStatus.WMIServiceUnreachable;
                                case ServiceTimedOutText:
                                    return OSMetricsStatus.WMIServiceTimedOut;
                            }
                            return OSMetricsStatus.OLEAutomationUnavailable;
                        case Metric.OSCPUPrivilegedTimePct:
                            if (osMetrics.PercentPrivilegedTime == null)
                                return null;
                            collectedValue = osMetrics.PercentPrivilegedTime.Value;
                            //START: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            if (isBaselineEnabled)
                            {
                                LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                                double? meanValue;
                                if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                                {
                                    osMetrics.OSCPUPrivilegedTimePctBaselineMean = meanValue;
                                    return osMetrics.OSCPUPrivilegedTimePctAsBaselinePerc = CalculatePercentageOfBaseline(osMetrics.OSCPUPrivilegedTimePctBaselineMean, osMetrics.PercentPrivilegedTime);
                                }
                                else
                                {
                                    LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                    return null;
                                }
                            }
                            //END: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline                            
                            return osMetrics.PercentPrivilegedTime.Value;
                        case Metric.OSCPUProcessorQueueLength:
                            if (osMetrics.ProcessorQueueLength == null)
                                return null;
                            collectedValue = osMetrics.ProcessorQueueLength.Value;
                            //START: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline 
                            if (isBaselineEnabled)
                            {
                                LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                                double? meanValue;
                                if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                                {
                                    osMetrics.OSCPUProcessorQueueLengthBaselineMean = meanValue;
                                    return osMetrics.OSCPUProcessorQueueLengthAsBaselinePerc = CalculatePercentageOfBaseline(osMetrics.OSCPUProcessorQueueLengthBaselineMean, osMetrics.ProcessorQueueLength.Value);
                                }
                                else
                                {
                                    LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                    return null;
                                }
                            }
                            //END: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline 
                            return osMetrics.ProcessorQueueLength.Value;
                        case Metric.OSCPUUsagePct:
                            if (osMetrics.PercentProcessorTime == null)
                                return null;
                            collectedValue = osMetrics.PercentProcessorTime.Value;
                            //START: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline							
                            if (isBaselineEnabled)
                            {
                                LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                                double? meanValue;
                                if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                                {
                                    osMetrics.OSCPUUsagePctBaselineMean = meanValue;
                                    return osMetrics.OSCPUUsagePctAsBaselinePerc = CalculatePercentageOfBaseline(osMetrics.OSCPUUsagePctBaselineMean, osMetrics.PercentProcessorTime.Value);
                                }
                                else
                                {
                                    LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                    return null;
                                }
                            }
                            //END: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline							
                            return osMetrics.PercentProcessorTime.Value;
                        case Metric.OSDiskAverageDiskQueueLength:
                            if (osMetrics.AvgDiskQueueLength == null)
                                return null;
                            collectedValue = osMetrics.AvgDiskQueueLength.Value;
                            //START: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline	
                            if (isBaselineEnabled)
                            {
                                LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                                double? meanValue;
                                if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                                {
                                    osMetrics.OSDiskAverageDiskQueueLengthBaselineMean = meanValue;
                                    return osMetrics.OSDiskAverageDiskQueueLengthAsBaselinePerc = CalculatePercentageOfBaseline(osMetrics.OSDiskAverageDiskQueueLengthBaselineMean, osMetrics.AvgDiskQueueLength.Value);
                                }
                                else
                                {
                                    LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                    return null;
                                }
                            }
                            //END: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline	
                            return osMetrics.AvgDiskQueueLength.Value;
                        case Metric.OSDiskPhysicalDiskTimePct:
                            if (osMetrics.PercentDiskTime == null)
                                return null;
                            collectedValue = osMetrics.PercentDiskTime.Value;
                            //START: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            if (isBaselineEnabled)
                            {
                                LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                                double? meanValue;
                                if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                                {
                                    osMetrics.OSDiskPhysicalDiskTimePctBaselineMean = meanValue;
                                    return osMetrics.OSDiskPhysicalDiskTimePctAsBaselinePerc = CalculatePercentageOfBaseline(osMetrics.OSDiskPhysicalDiskTimePctBaselineMean, osMetrics.PercentDiskTime.Value);
                                }
                                else
                                {
                                    LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                    return null;
                                }
                            }
                            //END: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            return osMetrics.PercentDiskTime.Value;
                        case Metric.OSMemoryUsagePct:
                            FileSize total = scheduledRefresh.Server.PhysicalMemory;
                            if (total.Bytes == null)
                                return null;
                            FileSize available = osMetrics.AvailableBytes;
                            if (available.Bytes == null)
                                return null;
                            double ratio = (double)((total.Bytes.Value - available.Bytes.Value) / total.Bytes.Value * 100.0m);
                            ratio = (ratio <= 100.0d ? ratio : 100.0d);
                            collectedValue = ratio;
                            //START: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            if (isBaselineEnabled)
                            {
                                LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                                double? meanValue;
                                if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                                {
                                    osMetrics.OSMemoryUsagePctBaselineMean = meanValue;
                                    return osMetrics.OSMemoryUsagePctAsBaselinePerc = CalculatePercentageOfBaseline(osMetrics.OSMemoryUsagePctBaselineMean, ratio / 100);
                                }
                                else
                                {
                                    LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                    return null;
                                }
                            }
                            //END: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            return ratio;
                        case Metric.OSMemoryPagesPerSecond:
                            if (osMetrics.PagesPersec == null)
                                return null;
                            collectedValue = osMetrics.PagesPersec.Value;
                            //START: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            if (isBaselineEnabled)
                            {
                                LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                                double? meanValue;
                                if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                                {
                                    osMetrics.OSMemoryPagesPerSecondBaselineMean = meanValue;

                                    return osMetrics.OSMemoryPagesPerSecondAsBaselinePerc = CalculatePercentageOfBaseline(osMetrics.OSMemoryPagesPerSecondBaselineMean, osMetrics.PagesPersec.Value);
                                }
                                else
                                {
                                    LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                    return null;
                                }

                            }
                            //END: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            return osMetrics.PagesPersec.Value;
                        case Metric.OSUserCPUUsagePct:
                            if (osMetrics.PercentUserTime == null)
                                return null;
                            collectedValue = osMetrics.PercentUserTime.Value;
                            //START: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            if (isBaselineEnabled)
                            {
                                LOG.Info("Getting baseline value to compare for alert for metric {0}.", (int)metric);
                                double? meanValue;
                                if (baselineMetricMean.GetBaselineMeanScheduled(scheduledRefresh, (int)metric, out meanValue))
                                {
                                    osMetrics.OSUserCPUUsagePctBaselineMean = meanValue;

                                    return osMetrics.OSUserCPUUsagePctAsBaselinePerc = CalculatePercentageOfBaseline(osMetrics.OSUserCPUUsagePctBaselineMean, osMetrics.PercentUserTime.Value);
                                }
                                else
                                {
                                    LOG.Warn("For baseline alert metric {0} key not found in BaselineMetricMean.", (int)metric);
                                    return null;
                                }
                            }
                            //END: SQLdm 10.0 (Tarun Sapra)- If baselined thresholds is enabled, calc perc of baseline
                            return osMetrics.PercentUserTime.Value;
                        case Metric.PhysicalMemory:
                            if (osMetrics.TotalPhysicalMemory == null || osMetrics.TotalPhysicalMemory.Bytes == null)
                                return null;
                            return osMetrics.TotalPhysicalMemory.Bytes.Value;
                    }
                }
                catch (Exception e)
                {
                    LOG.ErrorFormat("Error getting value for os metric {0}.\n{1}", metric, e.ToString());
                }
            }
            return null;
        }

        /// <summary>
        /// SQLdm 10.0 (Gaurav Karwal): generic function to calculate all percentage of baseline
        /// </summary>
        /// <param name="mean">baseline mean</param>
        /// <param name="value">metric value</param>
        /// <returns></returns>
        private double? CalculatePercentageOfBaseline(double? mean, double? value)
        {
            return (mean == null || value == null) ? null : (value / (mean == 0 ? 1 : mean)) * 100;
        }

        public IComparable GetDiskValue(Metric metric, DiskDrive diskDrive)
        {
            if (diskDrive == null)
                return null;

            try
            {
                switch (metric)
                {
                    case Metric.OSDiskFull:
                        return diskDrive.PercentUsed * 100;
                    case Metric.OSDiskPhysicalDiskTimePctPerDisk:
                        return diskDrive.DiskBusyPercent;
                    case Metric.OSDiskAverageDiskQueueLengthPerDisk:
                        return diskDrive.AverageDiskQueueLength;
                    case Metric.AverageDiskMillisecondsPerRead:
                        return diskDrive.AvgDiskSecPerRead;
                    case Metric.AverageDiskMillisecondsPerTransfer:
                        return diskDrive.AvgDiskSecPerTransfer;
                    case Metric.AverageDiskMillisecondsPerWrite:
                        return diskDrive.AvgDiskSecPerWrite;
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error getting value for disk metric {0}.\n{1}", metric, e.ToString());
            }
            return null;
        }

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --Match the filegroup name with exclusion filter of filegroup
        /// </summary>
        /// <param name="matchFilters"></param>
        /// <param name="likeFilters"></param>
        /// <param name="filegroup"></param>
        /// <returns></returns>
        public bool CompareFilegroupWithExcludeFilter(string[] matchFilters, string[] likeFilters, string filegroup)
        {
            bool isExcluded = false;

            if (matchFilters != null)
            {
                foreach (string pattern in matchFilters)
                {
                    if (filegroup.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        isExcluded = true;
                        break;
                    }
                }
            }

            if (likeFilters != null && !isExcluded)
            {
                foreach (string pattern in likeFilters)
                {
                    string pat;
                    int startIdx = -1, lastIdx = -1;

                    if (pattern.Length > 1)
                    {
                        if (pattern[0] == '%')
                            startIdx = 1;
                        if (pattern[pattern.Length - 1] == '%')
                            lastIdx = pattern.Length - 2;
                        if (startIdx <= lastIdx && startIdx != -1)
                        {
                            pat = pattern.Substring(startIdx, lastIdx - startIdx + 1);
                            if (filegroup.ToLower().Contains(pat.ToLower()))
                            {
                                isExcluded = true;
                                break;
                            }
                        }
                    }
                }
            }

            return isExcluded;
        }
        /// <summary>
        /// SQLdm 10.0 (Srishti Purohit) --Match the database name with exclusion filter of database
        /// </summary>
        /// <param name="matchFilters"></param>
        /// <param name="likeFilters"></param>
        /// <param name="filegroup"></param>
        /// <returns></returns>
        public bool CompareDatabaseNameWithExcludeFilter(string[] matchFilters, string[] likeFilters, string databaseName)
        {
            bool isExcluded = false;

            if (matchFilters != null)
            {
                foreach (string pattern in matchFilters)
                {
                    if (databaseName.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        isExcluded = true;
                        break;
                    }
                }
            }

            if (likeFilters != null && !isExcluded)
            {
                foreach (string pattern in likeFilters)
                {
                    string pat;
                    int startIdx = -1, lastIdx = -1;

                    if (pattern.Length > 1)
                    {
                        if (pattern[0] == '%')
                            startIdx = 1;
                        if (pattern[pattern.Length - 1] == '%')
                            lastIdx = pattern.Length - 2;
                        if (startIdx <= lastIdx && startIdx != -1)
                        {
                            pat = pattern.Substring(startIdx, lastIdx - startIdx + 1);
                            if (databaseName.ToLower().Contains(pat.ToLower()))
                            {
                                isExcluded = true;
                                break;
                            }
                        }
                    }
                }
            }

            return isExcluded;
        }

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --Calculating the values for each filegroup in the given database
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="databaseStatistics"></param>
        /// <param name="advanced"></param>
        /// <returns></returns>
        public Dictionary<string, double> GetDatabaseFilegroupValues(Metric metric, DatabaseStatistics databaseStatistics, AdvancedAlertConfigurationSettings advanced)
        {
            Dictionary<string, double> fileGroupsValues = new Dictionary<string, double>();

            if (databaseStatistics == null)
                return null;

            try
            {
                if (!(databaseStatistics == null || advanced == null || String.IsNullOrEmpty(databaseStatistics.Name)))
                {
                    List<DatabaseFileStatistics> filesStats = databaseStatistics.FileStatistics;
                    Dictionary<string, double> intermediateValues = new Dictionary<string, double>();
                    List<string> fileGroupNames = new List<string>();
                    List<string> drives = new List<string>();
                    bool isExcluded;

                    if (filesStats == null || String.IsNullOrEmpty(databaseStatistics.Name))
                        return null;

                    string[] matchFilters = advanced.FilegroupExcludeMatch;
                    string[] likeFilters = advanced.FilegroupExcludeLike;

                    foreach (DatabaseFileStatistics file in filesStats)
                    {
                        isExcluded = CompareFilegroupWithExcludeFilter(matchFilters, likeFilters, file.FileGroupName);

                        if (!file.FileGroupName.Equals("n/a", StringComparison.OrdinalIgnoreCase) && !isExcluded)  //excluding log files from the calculation
                        {
                            // sum of Used Size for all files in filegroup
                            if (fileGroupsValues.ContainsKey(file.FileGroupName))
                                fileGroupsValues[file.FileGroupName] += file.UsedSpace;
                            else
                            {
                                fileGroupsValues[file.FileGroupName] = file.UsedSpace;
                                fileGroupNames.Add(file.FileGroupName);
                            }

                            // sum of allocated file sizes for all files in filegroup
                            if (intermediateValues.ContainsKey(file.FileGroupName))
                                intermediateValues[file.FileGroupName] += file.InitialSize;
                            else
                                intermediateValues[file.FileGroupName] = file.InitialSize;
                        }
                    }

                    if (metric == Metric.FilegroupSpaceFullSize)
                        return fileGroupsValues;

                    if (metric == Metric.FilegroupSpaceFullPct && !advanced.IncludeAutogrowInDatabaseSizeCalc)
                    {
                        foreach (string filegroup in fileGroupNames)
                        {
                            fileGroupsValues[filegroup] = (fileGroupsValues[filegroup] / intermediateValues[filegroup]) * 100;
                        }
                        return fileGroupsValues;
                    }

                    if (metric == Metric.FilegroupSpaceFullPct && advanced.IncludeAutogrowInDatabaseSizeCalc)
                    {
                        intermediateValues.Clear();
                        foreach (DatabaseFileStatistics file in filesStats)
                        {
                            isExcluded = CompareFilegroupWithExcludeFilter(matchFilters, likeFilters, file.FileGroupName);

                            if (!file.FileGroupName.Equals("n/a", StringComparison.OrdinalIgnoreCase) && !isExcluded)
                            {
                                if (!drives.Contains(file.FileGroupName + file.DriveName))
                                {
                                    drives.Add(file.FileGroupName + file.DriveName);
                                    file.TotalPossibleFileSize = file.InitialSize + file.FreeDiskSpace;
                                }
                                else
                                    file.TotalPossibleFileSize = file.InitialSize;

                                if (file.MaximumSize > 0 && file.MaximumSize < (file.InitialSize + file.FreeDiskSpace))
                                    file.TotalPossibleFileSize = file.MaximumSize;

                                // sum of total possible file size for all files in filegroup
                                if (intermediateValues.ContainsKey(file.FileGroupName))
                                    intermediateValues[file.FileGroupName] += file.TotalPossibleFileSize;
                                else
                                    intermediateValues[file.FileGroupName] = file.TotalPossibleFileSize;
                            }
                        }

                        foreach (string filegroup in fileGroupNames)
                        {
                            fileGroupsValues[filegroup] = (fileGroupsValues[filegroup] / intermediateValues[filegroup]) * 100;
                        }
                        return fileGroupsValues;
                    }
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error getting value for database metric {0}.\n{1}", metric, e.ToString());
            }
            return null;
        }

        public IComparable GetDatabaseValue(Metric metric, DatabaseStatistics databaseStatistics, AdvancedAlertConfigurationSettings advanced)
        {
            if (databaseStatistics == null)
                return null;

            try
            {
                switch (metric)
                {
                    case Metric.DatabaseStatus:
                        return databaseStatistics.Status;
                    case Metric.DatabaseSizePct:
                        if (advanced == null || advanced.IncludeAutogrowInDatabaseSizeCalc)
                        {
                            if (databaseStatistics.PercentDataSize == null)
                                return null;
                            return databaseStatistics.PercentDataSize * 100;
                        }
                        else
                        {
                            if (databaseStatistics.PercentUsedSizeCurrent == null)
                                return null;
                            return databaseStatistics.PercentUsedSizeCurrent * 100;
                        }
                    case Metric.DatabaseFileSize:
                        if (databaseStatistics.DataFileSize == null)
                            return null;
                        return databaseStatistics.DataFileSize.Kilobytes;
                    case Metric.TransLogSize:
                        if (advanced == null || advanced.IncludeAutogrowInDatabaseSizeCalc)
                        {
                            if (databaseStatistics.PercentLogSpace == null)
                                return null;
                            return databaseStatistics.PercentLogSpace * 100;
                        }
                        else
                        {
                            if (databaseStatistics.PercentLogSizeCurrent == null)
                                return null;
                            return databaseStatistics.PercentLogSizeCurrent * 100;
                        }
                    case Metric.DatabaseSizeMb:
                        if (databaseStatistics.FileSpaceUsed != null)
                            return databaseStatistics.FileSpaceUsed.Megabytes;
                        else if (databaseStatistics.UsedSize != null)
                            return databaseStatistics.UsedSize.Megabytes;
                        else
                            return null;
                    case Metric.TransLogSizeMb:
                        if (databaseStatistics.LogSizeUsed != null)
                            return databaseStatistics.LogSizeUsed.Megabytes;
                        else
                            return null;
                    //SQLdm 8.6 -- (Ankit Srivastava) -- Preferred Node Feature -get the current value for preferred node unavailability metric 
                    case Metric.PreferredNodeUnavailability:
                        {
                            if (!(databaseStatistics == null || advanced == null || String.IsNullOrEmpty(databaseStatistics.Name)))
                                return GetPreferredNodeUnavailability(databaseStatistics.Name, advanced);
                            else
                                return 0;
                        }
                    // SQLdm 10.0 (Vandana Gogna) - Database backup alerts
                    case Metric.DatabaseLastBackupDate:
                        {

                            if ((databaseStatistics.Status != DatabaseStatus.ReadOnly || advanced.IncludeReadOnly) && !CompareDatabaseNameWithExcludeFilter(advanced.DatabaseExcludeMatch, advanced.DatabaseExcludeLike, databaseStatistics.Name))
                            {
                                    if (databaseStatistics.LastBackup != null)
                                    {
                                        DateTime d1 = DateTime.Now;
                                        DateTime d2 = (DateTime)databaseStatistics.LastBackup;
                                        TimeSpan d3 = (d1 - d2);
                                        //SQLDM-25439 fix (Barkha Khatri)
                                        int days = (int)Math.Floor(d3.TotalDays);
                                        LOG.DebugFormat("DatabaseLastBackupDate - database : {0} " +
                                                        ", dateLastBackup: {1} " +
                                                        ", dateNow: {2} " +
                                                        ", TotalDays: {3} " +
                                                        ", Days: {4} ."
                                                        , databaseStatistics.Name
                                                        , databaseStatistics.LastBackup
                                                        , d1
                                                        , d3.TotalDays
                                                        , days);
                                        return days;
                                    }
                                    else if (databaseStatistics.DateCreated != null) //SQLDM-28570. Implementing check condition for the databases newly created.
                                    {

                                        DateTime d1 = DateTime.Now;
                                        DateTime d2 = (DateTime)databaseStatistics.DateCreated;
                                        TimeSpan d3 = (d1 - d2);
                                        int days = (int)Math.Floor(d3.TotalDays);

                                        LOG.DebugFormat("DatabaseLastBackupDate - database : {0} " +
                                                        ", dateCreated: {1} " +
                                                        ", dateNow: {2} " +
                                                        ", TotalDays: {3} " +
                                                        ", Days: {4} ."
                                                        , databaseStatistics.Name
                                                        , databaseStatistics.DateCreated
                                                        , d1
                                                        , d3.TotalDays
                                                        , days);
                                        return days;
                                    }
                                    else
                                    {
                                        LOG.DebugFormat("DatabaseLastBackupDate - database: {0} , returning default value for days not backed: 9999999."
                                            , databaseStatistics.Name);
                                        return 9999999;
                                    }

                                }
                                else if (databaseStatistics.DateCreated != null) //SQLDM-28570. Implementing check condition for the databases newly created.
                                {

                                    DateTime d1 = DateTime.Now;
                                    DateTime d2 = (DateTime)databaseStatistics.DateCreated;
                                    TimeSpan d3 = (d1 - d2);
                                    int days = (int)Math.Floor(d3.TotalDays);

                                    LOG.DebugFormat("DatabaseLastBackupDate - database : {0} " +
                                                    ", dateCreated: {1} " +
                                                    ", dateNow: {2} " +
                                                    ", TotalDays: {3} " +
                                                    ", Days: {4} ."
                                                    , databaseStatistics.Name
                                                    , databaseStatistics.DateCreated
                                                    , d1
                                                    , d3.TotalDays
                                                    , days);
                                    return days;
                                }
                                else
                                    return null;
                            
                        }

                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error getting value for database metric {0}.\n{1}", metric, e.ToString());
            }
            return null;
        }

        //SQLdm 8.6 -- (Ankit Srivastava) -- Preferred Node Feature -get the current value for preferred node unavailability metric 
        private IComparable GetPreferredNodeUnavailability(string databaseName, AdvancedAlertConfigurationSettings advanced)
        {

            int value = 0;
            try
            {
                if (!String.IsNullOrEmpty(advanced.ReplicaInstanceName))
                {
                    LOG.InfoFormat(String.Format("Preferred Node Unavailability :starting to get the unavailability value of the preferred Node server:{0} for database:{1}"
                    , advanced.ReplicaInstanceName, databaseName));

                    foreach (AvailabilityGroup availabilityGroup in scheduledRefresh.AvailabilityGroupsSnapshot.AvailabilityGroups.Values)
                    {
                        foreach (AvailabilityReplica replica in availabilityGroup.Replicas.Values)
                        {
                            if (replica.ReplicaRole != ReplicaRole.Primary && (replica.ReplicaName.Equals(advanced.ReplicaInstanceName, StringComparison.OrdinalIgnoreCase)))
                            {
                                LOG.InfoFormat(String.Format("Preferred Node Unavailability : A non-primary replica found for the Preferred Node Server:{0}"
                                    , advanced.ReplicaInstanceName));
                                foreach (AlwaysOnDatabase db in replica.DatabaseItems)
                                {
                                    if (databaseName.Equals(db.DatabaseName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        value = 1;
                                        LOG.InfoFormat(String.Format("Preferred Node Unavailability : alert raised for preferred server:{0} not database:{1} not being available"
                                            , advanced.ReplicaInstanceName, databaseName));
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Preferred Node Unavailability :Could not find the unavailability for database:{0} due to unexpted error: /n {1}"
                    , databaseName, ex.Message);
            }
            return value;
        }

        public IComparable GetMirroredDatabaseValue(Metric metric, MirroringMetrics mirroringMetrics)
        {
            if (mirroringMetrics == null)
                return null;

            try
            {
                switch (metric)
                {
                    case Metric.UnsentLogThreshold:
                        return mirroringMetrics.UnsentLog;
                    case Metric.UnrestoredLog:
                        return mirroringMetrics.UnrestoredLog;
                    case Metric.OldestUnsentMirroringTran:
                        return (int)Math.Round(mirroringMetrics.OldestUnsentTransaction.TotalMinutes);
                    case Metric.MirrorCommitOverhead:
                        return mirroringMetrics.AverageDelay;
                    case Metric.MirroringSessionsStatus:
                        return mirroringMetrics.MirroringState;
                    case Metric.MirroringSessionNonPreferredConfig:
                    case Metric.MirroringSessionRoleChange:
                        return mirroringMetrics.Role;
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error getting value for mirrored database metric {0}.\n{1}", metric, e.ToString());
            }
            return null;
        }

        public IComparable GetTableReorgValue(Metric metric, TableReorganization tableReorganization)
        {
            if (tableReorganization == null)
                return null;
            try
            {
                switch (metric)
                {
                    case Metric.ReorganisationPct: // Table fragmentation (precent)
                        if (tableReorganization.LogicalFragmentation == null)
                            return null;
                        return tableReorganization.LogicalFragmentation.Value;
                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error getting value for table metric {0}.\n{1}", metric, e.ToString());
            }
            return null;
        }
    }
}
