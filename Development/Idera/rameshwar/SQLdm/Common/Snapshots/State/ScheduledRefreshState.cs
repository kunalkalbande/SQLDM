//------------------------------------------------------------------------------
// <copyright file="ServerSummaryStatus.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots.State
{
    using System;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [Serializable]
    public class ScheduledRefreshState : SnapshotState, IEquatable<ScheduledRefreshState>
    {
        #region fields

        public readonly IDictionary<int, MetricThresholdEntry> Thresholds;

        public MonitoredState SQLCPUUsage;
        public MonitoredState UserConnections;

        public MonitoredState NonDistributedTransactions;
        public MonitoredState NonSubscribedTransactionCount;
        public MonitoredState NonSubscribedTransactionTime;

        public MonitoredState SqlMemory;
        public MonitoredState ReadWriteErrors;
        public MonitoredState ServerResponseTime;

        public MonitoredState AgentServiceStatus;
        public MonitoredState DtcServiceStatus;
        public MonitoredState FullTextServiceStatus;
        public MonitoredState SqlServiceStatus;
        //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --declare object for state monitoring of new services
        public MonitoredState SQLBrowserServiceStatus;
        public MonitoredState SQLActiveDirectoryHelperServiceStatus;
        //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --declare object for state monitoring of new services

        public MonitoredState  OSMetricsStatus;
        public MonitoredState  OSMemoryAvailableBytes;
        public MonitoredState  OSMemoryPagesPerSecond;
        public MonitoredState  OSCPUProcessorTime;
        public MonitoredState  OSCPUPrivilegedTime;
        public MonitoredState  OSCPUUserTime;
        public MonitoredState  OSCPUProcessorQueueLength;
        public MonitoredState  OSDiskPhysicalDiskTime;
        public MonitoredState  OSDiskAverageDiskQueueLength;

        public MonitoredState AgentXPStatus;
        public MonitoredState CLRStatus;
        public MonitoredState OLEAutomationStatus;
        public MonitoredState QueryMonitorStatus;
        public MonitoredState WMIStatus;
        public MonitoredState XPCmdShellStatus;

        public MonitoredState ClientComputers;
        public MonitoredState BlockedSessions;
        public MonitoredState DataUsedPct;
        public MonitoredState LogUsedPct;


        // key is database name
        public Dictionary<string, MonitoredState> DbStatus;
        public Dictionary<string, MonitoredState> DbSize;
        public Dictionary<string, MonitoredState> LogSize;

        // key is Pair<database,table> 
        public Dictionary<MonitoredObjectName, MonitoredState> TbReorg;

        // key is disk name
        public Dictionary<string, MonitoredState> DiskFull;
        public Dictionary<string, MonitoredState> DiskTime;
        public Dictionary<string, MonitoredState> DiskQueue;

        #endregion

        #region constructors

        public ScheduledRefreshState()
        {
            DbStatus = new Dictionary<string, MonitoredState>();
            DbSize = new Dictionary<string, MonitoredState>();
            LogSize = new Dictionary<string, MonitoredState>();
            TbReorg = new Dictionary<MonitoredObjectName, MonitoredState>();
        }

        //This does not appear to be used anymore and is causing a problem with database size alerting
        //public ScheduledRefreshState(ScheduledRefresh snapshot, IDictionary<int, MetricThresholdEntry> thresholds) : this()
        //{
        //    Thresholds = thresholds;
        //    ScheduledRefreshValueAdapter valueAdapter = new ScheduledRefreshValueAdapter(snapshot);


        //    // always check for sql server status
        //    IComparable value = valueAdapter.GetServerValue(Metric.SqlServiceStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.SqlServiceStatus))
        //        SqlServiceStatus = GetState(value, thresholds[(int)Metric.SqlServiceStatus]);

        //    // if collection failed then there is no reason to continue
        //    if (snapshot.CollectionFailed || snapshot.Server.CollectionFailed)
        //        return;

        //    value = valueAdapter.GetServerValue(Metric.AgentServiceStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.AgentServiceStatus))
        //        AgentServiceStatus = GetState(value, thresholds[(int)Metric.AgentServiceStatus]);

        //    value = valueAdapter.GetServerValue(Metric.DtcServiceStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.DtcServiceStatus))
        //        DtcServiceStatus = GetState(value, thresholds[(int)Metric.DtcServiceStatus]);

        //    value = valueAdapter.GetServerValue(Metric.FullTextServiceStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.FullTextServiceStatus))
        //        FullTextServiceStatus = GetState(value, thresholds[(int)Metric.FullTextServiceStatus]);

        //    value = valueAdapter.GetServerValue(Metric.SQLCPUUsagePct);
        //    if (value != null && thresholds.ContainsKey((int)Metric.SQLCPUUsagePct))
        //        SQLCPUUsage = GetState(value, thresholds[(int)Metric.SQLCPUUsagePct]);

        //    value = valueAdapter.GetServerValue(Metric.UserConnectionPct);
        //    if (value != null && thresholds.ContainsKey((int)Metric.UserConnectionPct))
        //        UserConnections = GetState(value, thresholds[(int)Metric.UserConnectionPct]);

        //    if (true) // TODO: Replace with test to see if replication is enabled
        //    {
        //        value = valueAdapter.GetServerValue(Metric.NonDistributedTrans);
        //        if (value != null && thresholds.ContainsKey((int)Metric.NonDistributedTrans))
        //            NonDistributedTransactions = GetState(value, thresholds[(int)Metric.NonDistributedTrans]);

        //        value = valueAdapter.GetServerValue(Metric.NonSubscribedTransNum);
        //        if (value != null && thresholds.ContainsKey((int)Metric.NonSubscribedTransNum))
        //            NonSubscribedTransactionCount = GetState(value, thresholds[(int)Metric.NonSubscribedTransNum]);

        //        value = valueAdapter.GetServerValue(Metric.NonSubscribedTransTime);
        //        if (value != null && thresholds.ContainsKey((int)Metric.NonSubscribedTransTime))
        //            NonSubscribedTransactionTime = GetState(value, thresholds[(int)Metric.NonSubscribedTransTime]);
        //    }
            

        //    value = valueAdapter.GetServerValue(Metric.ReadWriteErrors);
        //    if (value != null && thresholds.ContainsKey((int)Metric.ReadWriteErrors))
        //        ReadWriteErrors = GetState(value, thresholds[(int)Metric.ReadWriteErrors]);

        //    value = valueAdapter.GetServerValue(Metric.SQLMemoryUsagePct);
        //    if (value != null && thresholds.ContainsKey((int)Metric.SQLMemoryUsagePct))
        //        SqlMemory = GetState(value, thresholds[(int)Metric.SQLMemoryUsagePct]);

        //    value = valueAdapter.GetServerValue(Metric.ServerResponseTime);
        //    if (value != null && thresholds.ContainsKey((int)Metric.ServerResponseTime))
        //        ServerResponseTime = GetState(value, thresholds[(int)Metric.ServerResponseTime]);

        //    value = valueAdapter.GetServerValue(Metric.AgentXPStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.AgentXPStatus))
        //        AgentXPStatus = GetState(value, thresholds[(int)Metric.AgentXPStatus]);
            
        //    value = valueAdapter.GetServerValue(Metric.CLRStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.CLRStatus))
        //        CLRStatus = GetState(value, thresholds[(int)Metric.CLRStatus]);

        //    value = valueAdapter.GetServerValue(Metric.OLEAutomationStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.OLEAutomationStatus))
        //        OLEAutomationStatus = GetState(value, thresholds[(int)Metric.OLEAutomationStatus]);

        //    value = valueAdapter.GetServerValue(Metric.QueryMonitorStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.QueryMonitorStatus))
        //        QueryMonitorStatus = GetState(value, thresholds[(int)Metric.QueryMonitorStatus]);
            
        //    value = valueAdapter.GetServerValue(Metric.WMIStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.WMIStatus))
        //        WMIStatus = GetState(value, thresholds[(int)Metric.WMIStatus]);

        //    value = valueAdapter.GetServerValue(Metric.XPCmdShellStatus);
        //    if (value != null && thresholds.ContainsKey((int)Metric.XPCmdShellStatus))
        //        XPCmdShellStatus = GetState(value, thresholds[(int)Metric.XPCmdShellStatus]);

        //    value = valueAdapter.GetServerValue(Metric.ClientComputers);
        //    if (value != null && thresholds.ContainsKey((int)Metric.ClientComputers))
        //        ClientComputers = GetState(value, thresholds[(int)Metric.ClientComputers]);

        //    value = valueAdapter.GetServerValue(Metric.BlockedSessions);
        //    if (value != null && thresholds.ContainsKey((int)Metric.BlockedSessions))
        //        BlockedSessions = GetState(value, thresholds[(int)Metric.BlockedSessions]);

        //    value = valueAdapter.GetServerValue(Metric.DataUsedPct);
        //    if (value != null && thresholds.ContainsKey((int)Metric.DataUsedPct))
        //        DataUsedPct = GetState(value, thresholds[(int)Metric.DataUsedPct]);

        //    value = valueAdapter.GetServerValue(Metric.LogUsedPct);
        //    if (value != null && thresholds.ContainsKey((int)Metric.LogUsedPct))
        //        LogUsedPct = GetState(value, thresholds[(int)Metric.LogUsedPct]);

        //    // process OSMetricsStatus
        //    value = valueAdapter.GetServerOSValue(Metric.OSMetricsStatus);
        //    if (thresholds.ContainsKey((int)Metric.OSMetricsStatus))
        //        OSMetricsStatus = GetState(value, thresholds[(int)Metric.OSMetricsStatus]);
            
        //    // if os metrics are available
        //    if (value.CompareTo(Snapshots.OSMetricsStatus.Available) == 0)
        //    {
        //        value = valueAdapter.GetServerValue(Metric.OSMemoryUsagePct);
        //        if (value != null && thresholds.ContainsKey((int)Metric.OSMemoryUsagePct))
        //            OSMemoryAvailableBytes = GetState(value, thresholds[(int)Metric.OSMemoryUsagePct]);

        //        value = valueAdapter.GetServerValue(Metric.OSMemoryPagesPerSecond);
        //        if (value != null && thresholds.ContainsKey((int)Metric.OSMemoryPagesPerSecond))
        //            OSMemoryPagesPerSecond = GetState(value, thresholds[(int)Metric.OSMemoryPagesPerSecond]);

        //        value = valueAdapter.GetServerValue(Metric.OSCPUUsagePct);
        //        if (value != null && thresholds.ContainsKey((int)Metric.OSCPUUsagePct))
        //            OSCPUProcessorTime = GetState(value, thresholds[(int)Metric.OSCPUUsagePct]);

        //        value = valueAdapter.GetServerValue(Metric.OSCPUPrivilegedTimePct);
        //        if (value != null && thresholds.ContainsKey((int)Metric.OSCPUPrivilegedTimePct))
        //            OSCPUPrivilegedTime = GetState(value, thresholds[(int)Metric.OSCPUPrivilegedTimePct]);

        //        value = valueAdapter.GetServerValue(Metric.OSUserCPUUsagePct);
        //        if (value != null && thresholds.ContainsKey((int)Metric.OSUserCPUUsagePct))
        //            OSCPUUserTime = GetState(value, thresholds[(int)Metric.OSUserCPUUsagePct]);

        //        value = valueAdapter.GetServerValue(Metric.OSCPUProcessorQueueLength);
        //        if (value != null && thresholds.ContainsKey((int)Metric.OSCPUProcessorQueueLength))
        //            OSCPUProcessorQueueLength = GetState(value, thresholds[(int)Metric.OSCPUProcessorQueueLength]);

        //        value = valueAdapter.GetServerValue(Metric.OSDiskPhysicalDiskTimePct);
        //        if (value != null && thresholds.ContainsKey((int)Metric.OSDiskPhysicalDiskTimePct))
        //            OSDiskPhysicalDiskTime = GetState(value, thresholds[(int)Metric.OSDiskPhysicalDiskTimePct]);

        //        value = valueAdapter.GetServerValue(Metric.OSDiskAverageDiskQueueLength);
        //        if (value != null && thresholds.ContainsKey((int)Metric.OSDiskAverageDiskQueueLength))
        //            OSDiskAverageDiskQueueLength = GetState(value, thresholds[(int)Metric.OSDiskAverageDiskQueueLength]);  
        //    }

        //    if (snapshot.DbStatistics != null)
        //    {
        //        MetricThresholdEntry dbStatusThresholdEntry = null;
        //        MetricThresholdEntry dbSizeThresholdEntry  = null;
        //        MetricThresholdEntry logSizeThresholdEntry = null;
        //        Threshold.ComparableList statusExcludedDatabases = null;
        //        Threshold.ComparableList dbSizeExcludedDatabases = null;
        //        Threshold.ComparableList logSizeExcludedDatabases = null;

        //        if (thresholds.TryGetValue((int)Metric.DatabaseStatus, out dbStatusThresholdEntry))
        //        {
        //            statusExcludedDatabases = dbStatusThresholdEntry.Data as Threshold.ComparableList;
        //        }

        //        if (thresholds.TryGetValue((int)Metric.DatabaseSizePct, out dbSizeThresholdEntry))
        //        {
        //            dbSizeExcludedDatabases = dbSizeThresholdEntry.Data as Threshold.ComparableList;
        //        }

        //        if (thresholds.TryGetValue((int)Metric.TransLogSize, out logSizeThresholdEntry))
        //        {
        //            logSizeExcludedDatabases = logSizeThresholdEntry.Data as Threshold.ComparableList;
        //        }

        //        // for each database 
        //        foreach (DatabaseStatistics dbstats in snapshot.DbStatistics.Values)
        //        {
        //            if (statusExcludedDatabases != null && statusExcludedDatabases.Contains(dbstats.Name))
        //            {
        //                DbStatus.Add(dbstats.Name, MonitoredState.OK);
        //                continue;
        //            }
        //            else
        //            {
        //                // Do the database status alert.
        //                value = valueAdapter.GetDatabaseValue(Metric.DatabaseStatus, dbstats);
        //                if (value != null && thresholds.ContainsKey((int) Metric.DatabaseStatus))
        //                {
        //                    MonitoredState state = GetState(value, thresholds[(int) Metric.DatabaseStatus]);
        //                    DbStatus.Add(dbstats.Name, state);
        //                }

        //                // if the database is accessible
        //                if (dbstats.IsAccessibleStatus && dbstats.DataFileSize != null && dbstats.DataFileSize.Bytes.HasValue && dbstats.DataFileSize.Bytes.Value > 0)
        //                {
        //                    // If the db is not excluded for size alert processing then figure out the alert state.
        //                    MonitoredState dbSizeState = MonitoredState.OK;
        //                    if (dbSizeExcludedDatabases == null || !dbSizeExcludedDatabases.Contains(dbstats.Name))
        //                    {
        //                        if (dbSizeThresholdEntry != null && dbSizeThresholdEntry.IsEnabled)
        //                        {
        //                            value = valueAdapter.GetDatabaseValue(Metric.DatabaseSizePct, dbstats);
        //                            if (value != null)
        //                            {
        //                                dbSizeState = GetState(value, dbSizeThresholdEntry);
        //                            }
        //                        }
        //                    }
        //                    DbSize.Add(dbstats.Name, dbSizeState);

        //                    // If the db is not excluded for log size alert processing then figure out the alert state.
        //                    MonitoredState logSizeState = MonitoredState.OK;
        //                    if (logSizeExcludedDatabases == null || !logSizeExcludedDatabases.Contains(dbstats.Name))
        //                    {
        //                        if (logSizeThresholdEntry != null && logSizeThresholdEntry.IsEnabled)
        //                        {
        //                            value = valueAdapter.GetDatabaseValue(Metric.TransLogSize, dbstats);
        //                            if (value != null)
        //                            {
        //                                logSizeState = GetState(value, logSizeThresholdEntry);
        //                            }
        //                        }
        //                    }
        //                    LogSize.Add(dbstats.Name, logSizeState);
        //                }
        //            }

        //            if (dbstats.TableReorganizations.Count > 0)
        //            {
        //                ProcessTableStatistics(dbstats, thresholds, valueAdapter);
        //            }
        //        }

        //        foreach (DiskDrive d in snapshot.DiskDrives.Values)
        //        {
        //            value = valueAdapter.GetDiskValue(Metric.OSDiskFull, d);
        //            if (value != null && thresholds.ContainsKey((int)Metric.OSDiskFull))
        //            {
        //                MonitoredState state = GetState(value, thresholds[(int)Metric.OSDiskFull]);
        //                DiskFull.Add(d.DriveLetter, state);
        //            }

        //            value = valueAdapter.GetDiskValue(Metric.OSDiskPhysicalDiskTimePctPerDisk, d);
        //            if (value != null && thresholds.ContainsKey((int)Metric.OSDiskPhysicalDiskTimePctPerDisk))
        //            {
        //                MonitoredState state = GetState(value, thresholds[(int)Metric.OSDiskPhysicalDiskTimePctPerDisk]);
        //                DiskTime.Add(d.DriveLetter, state);
        //            }

        //            value = valueAdapter.GetDiskValue(Metric.OSDiskAverageDiskQueueLengthPerDisk, d);
        //            if (value != null && thresholds.ContainsKey((int)Metric.OSDiskAverageDiskQueueLengthPerDisk))
        //            {
        //                MonitoredState state = GetState(value, thresholds[(int)Metric.DatabaseStatus]);
        //                DiskQueue.Add(d.DriveLetter, state);
        //            }
        //        }
        //    }
        //}

        private void ProcessTableStatistics(DatabaseStatistics dbstats, IDictionary<int, MetricThresholdEntry> thresholds, ScheduledRefreshValueAdapter valueAdapter)
        {
            MetricThresholdEntry reorgThresholdEntry = null;
            thresholds.TryGetValue((int)Metric.ReorganisationPct, out reorgThresholdEntry);

            foreach (TableReorganization reorg in dbstats.TableReorganizations.Values)
            {
                if (reorgThresholdEntry != null && reorgThresholdEntry.IsEnabled)
                {
                    IComparable value = valueAdapter.GetTableReorgValue(Metric.ReorganisationPct, reorg);
                    if (value != null)
                    {
                        MonitoredState state = GetState(value, reorgThresholdEntry);
                        TbReorg.Add(new MonitoredObjectName(dbstats.ServerName, dbstats.Name, reorg.Name), state);
                    }
                }
            }
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        #region IEquatable<ScheduledRefreshState> Members

        public bool Equals(ScheduledRefreshState other)
        {
            return
                (other != null &&
                AgentServiceStatus == other.AgentServiceStatus &&
                DtcServiceStatus == other.DtcServiceStatus &&
                FullTextServiceStatus == other.FullTextServiceStatus &&
                SqlServiceStatus == other.SqlServiceStatus && 
                SQLBrowserServiceStatus == other.SQLBrowserServiceStatus && //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add new sql services in the state equality
                SQLActiveDirectoryHelperServiceStatus == other.SQLActiveDirectoryHelperServiceStatus); 
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
