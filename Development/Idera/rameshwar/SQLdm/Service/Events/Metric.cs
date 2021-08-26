//------------------------------------------------------------------------------
// <copyright file="Metric.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Service.Events
{
    using System;

    /// <summary>
    /// An enumeration for all monitoring events that occur in the system.
    /// </summary>
    [Serializable]
    public enum Metric
    {
        // Do not change the order (i.e. integer value) of the first set of enums,
        // as they are declared in the same order that the corresponding alert 
        // configurations appear in the SQLdm 4.x registry.  The import tool expects
        // the Nth item in the registry to match the Nth enum.
        SQLCPUUsagePct = 0,   // SQL CPU usage percent.
        UserConnectionPct, // User connections as a percentage of max.
        HighestThreadsPct, // Highest amount of worker threads, percentage of max.
        CurrentThreadsPct, // Current worker threads, percentage of max.
        NonDistributedTrans, // Number of transactions not yet transferred to the distribution database.
        NonSubscribedTransNum, // Number of transactions in the Distribution database not yet fully subscribed to.
        OldestOpenTransMinutes,// Longest time any transaction has been open.
        ReorganisationPct, // Table fragmentation percentage.
        DatabaseSizePct,   // Database size as a percentage of total expandable size on disk. 8
        TransLogSize,      // Transaction log size as a percentage of total expandable size on disk. 9 
        AgentServiceStatus, // SQL Server Agent status. 10 
        ReadWriteErrors,    // Failures to physically write or read from disk. 11
        SqlServiceStatus,   // Status of the SQL Server service. 12
        SQLMemoryUsagePct,  // 13
        DatabaseStatus,     // Database status. 14
        DMStart,            // Diagnostic Manager started. 15
        DMStop,             // Diatnostic Manager stopped. 16
        NonSubscribedTransTime, // Length of time (seconds) transactions remained in the Distribution database without being fully subscribed to. 17
        DtcServiceStatus,   // DTS service status. 18 
        FullTextServiceStatus, // Status of the Full Text Search service. 19
        IndexRowHits,    // Number of row hits that part of a clustered index or an entire non-clustered index returns. 20
        FullTextRefreshHours, // Hours since the Full Text catalogs were last refreshed. 21
        ServerResponseTime, // Milliseconds for server to respond to a query.22
        OSMetricsStatus,    // OS metrics collection status.23
        OSMemoryUsagePct,   //24
        OSMemoryPagesPerSecond, //25
        OSCPUUsagePct, //26
        OSCPUPrivilegedTimePct, //27
        OSUserCPUUsagePct,  //28
        OSCPUProcessorQueueLength,  //29
        OSDiskPhysicalDiskTimePct, //30
        OSDiskAverageDiskQueueLength, //31
        
        // The importer does not care about the order of the remaining values, though the next
        // set of metrics have configuration that is imported from 4.x
        
        ResourceAlert, //32
        BlockingAlert, //33
        LongJobs, //34
        BombedJobs, //35
        
        // The remaining metrics are not imported from 4.x.

        Language, //36
        LoginHasAdministratorRights, //37
        MaxConnections, //38
        PhysicalMemory, //39
        ProcessorCount, //40
        ProcessorType, //41
        ProductVersion, //42
        RealServerName, //43
        ServerOpenTransactions, //44
        ServerTime, //45
        WindowsVersion, //46
        DatabaseFileSize, //47
         
        // new for 5.0 
        MaintenanceMode,      //48              
        CLRStatus, //49
        OLEAutomationStatus, //50
        QueryMonitorStatus, //51
        SQLdmCollectionServiceStatus, //52
        AgentXPStatus, //53
        WMIStatus, //54
        //Removed in 7.0
        XPCmdShellStatus, //Do not use.  This has been left in to keep the enum value after this entry correct. 55
        
        Operational, //56

        ClientComputers, //57
        BlockedSessions, //58

        
        DataUsedPct, //59
        LogUsedPct, //60

        // Marfa - custom counter
        Custom, //61

        OSDiskPhysicalDiskTimePctPerDisk,//62
        OSDiskAverageDiskQueueLengthPerDisk,//63
        OSDiskFull, //64
        LongJobsMinutes, //65
        ErrorLog, //66
        AgentLog, //67

        //Mirroring
        UnsentLogThreshold, //68
        UnrestoredLog,
        OldestUnsentMirroringTran,
        MirrorCommitOverhead,
        MirroringSessionsStatus,
        MirroringSessionNonPreferredConfig,
        MirroringSessionRoleChange,
        MirroringWitnessConnection,

        PageLifeExpectancy, //76

        //Cluster Alerts
        ClusterFailover,
        ClusterActiveNode,


        //Enhanced replication
        NonDistributedTransTime,

        
        Deadlock, //80
        ProcCacheHitRatio,
        AverageDiskMillisecondsPerRead,
        AverageDiskMillisecondsPerTransfer,
        AverageDiskMillisecondsPerWrite, //84

        DiskReadsPerSecond,
        DiskTransfersPerSecond,
        DiskWritesPerSecond,
        JobCompletion,

        VersionStoreGenerationRatio, //89
        VersionStoreSize,
        LongRunningVersionStoreTransaction,
        SessionTempdbSpaceUsage,
        TempdbContention,
        LogFileAutogrow,
        DataFileAutogrow,

        // VM Alerts
        VmConfigChange, //96
        VmHostServerChange,
        VmCPUUtilization,
        VmESXCPUUtilization,
        VmMemoryUtilization,
        VmESXMemoryUsage,
        VmCPUReadyWaitTime,
        VmReclaimedMemory,
        VmMemorySwapDelayDetected,
        VmESXMemorySwapDetected,
        VmResourceLimits,
        VmPowerState,
        VmESXPowerState,

        // Amount of Storage Used
        DatabaseSizeMb, // 109
        TransLogSizeMb, //110
        OsDiskFreeMb, //111
        
        // SQLsafe alerts
        SSBackupOperation, //112
        SSRestoreOperation, //113
        SSDefragOperation, //114
        SSConnectionProblem, //115
        AlwaysOnAvailabilityGroupRoleChange, //116
        AlwaysOnEstimatedDataLossTime, //117
        AlwaysOnSynchronizationHealthState, //118
        AlwaysOnEstimatedRecoveryTime, //119
        AlwaysOnSynchronizationPerformance, //120
        AlwaysOnLogSendQueueSize, //121
        AlwaysOnRedoQueueSize, //122
        AlwaysOnRedoRate, //123

    }

    [Serializable]
    public class AlertingMetrics
    {

        public static readonly List<Metric> ScheduledRefreshMetrics = new List<Metric>()
                                                    {
                                                        Metric.SQLCPUUsagePct,
                                                        Metric.UserConnectionPct,
                                                        Metric.HighestThreadsPct,
                                                        Metric.CurrentThreadsPct,
                                                        Metric.NonDistributedTrans,
                                                        Metric.NonSubscribedTransNum,
                                                        Metric.OldestOpenTransMinutes,
                                                        Metric.ReorganisationPct,
                                                        Metric.AgentServiceStatus,
                                                        Metric.ReadWriteErrors,
                                                        Metric.SqlServiceStatus,
                                                        Metric.SQLMemoryUsagePct,
                                                        Metric.DatabaseStatus,
                                                        Metric.DMStart,
                                                        Metric.DMStop,
                                                        Metric.NonSubscribedTransTime,
                                                        Metric.DtcServiceStatus,
                                                        Metric.FullTextServiceStatus,
                                                        Metric.IndexRowHits,
                                                        Metric.FullTextRefreshHours,
                                                        Metric.ServerResponseTime,
                                                        Metric.OSMetricsStatus,
                                                        Metric.OSMemoryUsagePct,
                                                        Metric.OSMemoryPagesPerSecond,
                                                        Metric.OSCPUUsagePct,
                                                        Metric.OSCPUPrivilegedTimePct,
                                                        Metric.OSUserCPUUsagePct,
                                                        Metric.OSCPUProcessorQueueLength,
                                                        Metric.OSDiskPhysicalDiskTimePct,
                                                        Metric.OSDiskAverageDiskQueueLength,
                                                        Metric.ResourceAlert,
                                                        Metric.BlockingAlert,
                                                        Metric.LongJobs,
                                                        Metric.BombedJobs,
                                                        Metric.Language,
                                                        Metric.LoginHasAdministratorRights,
                                                        Metric.MaxConnections,
                                                        Metric.PhysicalMemory,
                                                        Metric.ProcessorCount,
                                                        Metric.ProcessorType,
                                                        Metric.ProductVersion,
                                                        Metric.RealServerName,
                                                        Metric.ServerOpenTransactions,
                                                        Metric.ServerTime,
                                                        Metric.WindowsVersion,
                                                        Metric.DatabaseFileSize,
                                                        Metric.MaintenanceMode,
                                                        Metric.CLRStatus,
                                                        Metric.OLEAutomationStatus,
                                                        Metric.QueryMonitorStatus,
                                                        Metric.SQLdmCollectionServiceStatus,
                                                        Metric.AgentXPStatus,
                                                        Metric.WMIStatus,
                                                        Metric.ClientComputers,
                                                        Metric.BlockedSessions,
                                                        Metric.Custom,
                                                        Metric.LongJobsMinutes,
                                                        Metric.ErrorLog,
                                                        Metric.AgentLog,
                                                        Metric.UnsentLogThreshold,
                                                        Metric.UnrestoredLog,
                                                        Metric.OldestUnsentMirroringTran,
                                                        Metric.MirrorCommitOverhead,
                                                        Metric.MirroringSessionsStatus,
                                                        Metric.MirroringSessionNonPreferredConfig,
                                                        Metric.MirroringSessionRoleChange,
                                                        Metric.MirroringWitnessConnection,
                                                        Metric.PageLifeExpectancy,
                                                        Metric.ClusterFailover,
                                                        Metric.ClusterActiveNode,
                                                        Metric.NonDistributedTransTime,
                                                        Metric.Deadlock,
                                                        Metric.ProcCacheHitRatio,
                                                        Metric.JobCompletion,
                                                        Metric.VersionStoreGenerationRatio,
                                                        Metric.VersionStoreSize,
                                                        Metric.LongRunningVersionStoreTransaction,
                                                        Metric.SessionTempdbSpaceUsage,
                                                        Metric.TempdbContention,
                                                        Metric.LogFileAutogrow,
                                                        Metric.DataFileAutogrow,
                                                        Metric.VmConfigChange,
                                                        Metric.VmHostServerChange,
                                                        Metric.VmCPUUtilization,
                                                        Metric.VmESXCPUUtilization,
                                                        Metric.VmMemoryUtilization,
                                                        Metric.VmESXMemoryUsage,
                                                        Metric.VmCPUReadyWaitTime,
                                                        Metric.VmReclaimedMemory,
                                                        Metric.VmMemorySwapDelayDetected,
                                                        Metric.VmESXMemorySwapDetected,
                                                        Metric.VmResourceLimits,
                                                        Metric.VmPowerState,
                                                        Metric.VmESXPowerState,
                                                        Metric.AverageDiskMillisecondsPerRead,
                                                        Metric.AverageDiskMillisecondsPerTransfer,
                                                        Metric.AverageDiskMillisecondsPerWrite,
                                                        Metric.DiskReadsPerSecond,
                                                        Metric.DiskTransfersPerSecond,
                                                        Metric.DiskWritesPerSecond,
                                                        Metric.OsDiskFreeMb,
                                                        Metric.OSDiskPhysicalDiskTimePctPerDisk,
                                                        Metric.OSDiskAverageDiskQueueLengthPerDisk,
                                                        Metric.OSDiskFull,
                                                        Metric.AlwaysOnAvailabilityGroupRoleChange,
                                                        Metric.AlwaysOnEstimatedDataLossTime,
                                                        Metric.AlwaysOnSynchronizationHealthState,
                                                        Metric.AlwaysOnEstimatedRecoveryTime,
                                                        Metric.AlwaysOnSynchronizationPerformance,
                                                        Metric.AlwaysOnLogSendQueueSize,
                                                        Metric.AlwaysOnRedoQueueSize,
                                                        Metric.AlwaysOnRedoRate
                                                    };

        public static readonly List<Metric> DatabaseMetrics = new List<Metric>()
                                                    {
                                                        Metric.DatabaseSizePct,
                                                        Metric.TransLogSize,
                                                        Metric.DatabaseStatus,
                                                        Metric.DataUsedPct,
                                                        Metric.LogUsedPct,
                                                        //Metric.AverageDiskMillisecondsPerRead,
                                                        //Metric.AverageDiskMillisecondsPerTransfer,
                                                        //Metric.AverageDiskMillisecondsPerWrite,
                                                        //Metric.DiskReadsPerSecond,
                                                        //Metric.DiskTransfersPerSecond,
                                                        //Metric.DiskWritesPerSecond,
                                                        Metric.DatabaseSizeMb,
                                                        Metric.TransLogSizeMb,
                                                        //Metric.OsDiskFreeMb,
                                                        //Metric.OSDiskPhysicalDiskTimePctPerDisk,
                                                        //Metric.OSDiskAverageDiskQueueLengthPerDisk,
                                                        //Metric.OSDiskFull
                                                    };

        public static List<Metric> AlertableMetrics(AlertableSnapshot snapshot)
        {
            if (snapshot is DatabaseSizeSnapshot)
                return DatabaseMetrics;

            return ScheduledRefreshMetrics;

        }
    }

}
