using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;
using Idera.SQLdm.ManagementService.Monitoring;
using Microsoft.ApplicationBlocks.Data;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Snapshots.State;
using System.Reflection;
using Idera.SQLdm.Common.VMware;

namespace Idera.SQLdm.ManagementService.WebClient
{
    internal class ServerDetails
    {
        #region ServerDetailsMetrics Enum

        private enum ServerDetailsMetrics
        {
            [Description("Sql CPU Activity")]
            CPUActivity = 0, // 0
            [Description("User Connections Percent")]
            UserConnectionsPercent = 1, // 1
            [Description("DTC Service Status")]
            DtcServiceStatus, // 2
            [Description("Full-Text Service Status")]
            FullTextServiceStatus, // 3
            [Description("Sql Server Agent Service Status")]
            AgentServiceStatus, // 4
            [Description("Sql Server Service Status")]
            SqlServerServiceStatus, // 5
            [Description("Available Memory")]
            AvailableMemory, // 6
            [Description("Batches")]
            Batches, // 7
            [Description("Batches Per Second")]
            BatchesPerSecond, // 8
            [Description("Buffer Cache Hit Ratio")]
            BufferCacheHitRatio, // 9
            [Description("Buffer Cache Size")]
            BufferCacheSize, // 10
            [Description("Read Write Errors")]
            ReadWriteErrors = 11, // 11
            [Description("Checkpoint Writes")]
            CheckpointWrites, // 12
            [Description("Sql Memory Used Percent")]
            SqlMemoryUsedPercent = 13,  // 13
            [Description("Checkpoint Writes Per Second")]
            CheckpointWritesPerSecond, // 14
            [Description("Connections")]
            Connections, // 15
            [Description("Connections Per Second")]
            ConnectionsPerSecond, // 16
            [Description("Database Count")]
            DatabaseCount, // 17
            [Description("Data File Count")]
            DataFileCount, // 18
            [Description("Data File Space Allocated")]
            DataFileSpaceAllocated, // 19
            [Description("Data File Space Used")]
            DataFileSpaceUsed, // 20
            [Description("Full Scans")]
            FullScans, // 21
            [Description("Response Time")]
            ResponseTime = 22, // 22
            [Description("OS Metrics Availability")]
            OsMetricsAvailability = 23, // 23
            [Description("Used Memory")]
            MemoryUsed = 24, // 24
            [Description("Pages Per Second")]
            PagesPerSecond = 25, // 25
            [Description("OS Total Processor Activity")]
            OsTotalProcessorActivity = 26, // 26
            [Description("OS CPU Privileged Activity")]
            OsCpuPrivilegedActivity = 27, // 27
            [Description("OS CPU User Time")]
            OsCpuUserTime = 28, // 28
            [Description("Processor Queue Length")]
            ProcessorQueueLength = 29, // 29
            [Description("Disk Time (Percent)")]
            DiskTime = 30, // 30
            [Description("Disk Queue Length")]
            DiskQueueLength = 31, // 31
            [Description("Full Scans Per Second")]
            FullScansPerSecond, // 32
            [Description("Lazy Writer Writes")]
            LazyWriterWrites, // 33
            [Description("Lazy Writer Writes Per Second")]
            LazyWriterWritesPerSecond, // 34
            [Description("Lead Blockers")]
            LeadBlockers, // 35
            [Description("Lock Waits")]
            LockWaits, // 36
            [Description("Lock Waits Per Second")]
            LockWaitsPerSecond, // 37
            [Description("Log File Count")]
            LogFileCount, // 38
            [Description("Log File Space Allocated")]
            LogFileSpaceAllocated, // 39
            [Description("Log File Space Used")]
            LogFileSpaceUsed, // 40
            [Description("Log Flushes")]
            LogFlushes, // 41
            [Description("Log Flushes Per Second")]
            LogFlushesPerSecond, // 42
            [Description("Open Transactions")]
            OpenTransactions, // 43
            [Description("Packet Errors")]
            PacketErrors, // 44
            [Description("Packet Errors Per Second")]
            PacketErrorsPerSecond, // 45
            [Description("Packets Received")]
            PacketsReceived, // 46
            [Description("Packets Received Per Second")]
            PacketsReceivedPerSecond, // 47
            [Description("Maintenance Mode")]
            MaintenanceModeEnabled = 48, // 48
            [Description("Packets Sent")]
            PacketsSent, // 49
            [Description("OLE Automation Status")]
            OleAutomationStatus = 50, // 50
            [Description("Packets Sent Per Second")]
            PacketsSentPerSecond, // 51
            [Description("Page Life Expectancy")]
            PageLifeExpectancy, // 52
            [Description("Page Lookups")]
            PageLookups, // 53
            [Description("WMI Service Status")]
            WMIStatus = 54, // 54
            [Description("Page Lookups Per Second")]
            PageLookupsPerSecond, // 55
            [Description("Page Reads")]
            PageReads, // 56
            [Description("Client Computers")]
            ClientComputers = 57, // 57
            [Description("Blocked Sessions")]
            BlockedSessions = 58, // 58
            [Description("Page Reads Per Second")]
            PageReadsPerSecond, // 59
            [Description("Page Splits")]
            PageSplits, // 60
            [Description("Page Splits Per Second")]
            PageSplitsPerSecond, // 61
            [Description("Page Writes")]
            PageWrites, // 62
            [Description("Page Writes Per Second")]
            PageWritesPerSecond, // 63
            [Description("Physical Memory")]
            PhysicalMemory, // 64
            [Description("Procedure Cache Hit Ratio")]
            ProcedureCacheHitRatio, // 65
            [Description("Procedure Cache Size")]
            ProcedureCacheSize, // 66
            [Description("Read Ahead Pages")]
            ReadAheadPages, // 67
            [Description("Read Ahead Pages Per Second")]
            ReadAheadPagesPerSecond, // 68
            [Description("Running Since")]
            RunningSince, // 69
            [Description("Server Version")]
            ServerVersion, // 70
            [Description("Sql Compilations")]
            SqlCompilations, // 71
            [Description("Sql Compilations Per Second")]
            SqlCompilationsPerSecond, // 72
            [Description("Sql Memory Allocated")]
            SqlMemoryAllocated,  // 73
            [Description("Sql Memory Used")]
            SqlMemoryUsed,  // 74
            [Description("Sql Recompilations")]
            SqlRecompilations, // 75
            [Description("Sql Recompilations Per Second")]
            SqlRecompilationsPerSecond, // 76
            [Description("System Processes")]
            SystemProcesses, // 77
            [Description("System Processes Consuming CPU")]
            SystemProcessesConsumingCPU, // 78
            [Description("User Processes")]
            UserProcesses, // 79
            [Description("User Processes Consuming CPU")]
            UserProcessesConsumingCPU, // 80
            [Description("Workfiles Created")]
            WorkfilesCreated, // 81
            [Description("Workfiles Created Per Second")]
            WorkfilesCreatedPerSecond, // 82
            [Description("Worktables Created")]
            WorktablesCreated, // 83
            [Description("Worktables Created Per Second")]
            WorktablesCreatedPerSecond, // 84
            [Description("Total Deadlocks")]
            TotalDeadlocks, // 85

            [Description("Disk Reads Per Second")]
            DiskReadsPerSecondPerDisk, // 86        
            [Description("Disk Transfers Per Second")]
            DiskTransfersPerSecondPerDisk, // 87        
            [Description("Disk Writes Per Second")]
            DiskWritesPerSecondPerDisk, // 88     
            [Description("Average Milliseconds Per Read")]
            AverageMillisPerReadPerDisk, // 89        
            [Description("Average Milliseconds Per Transfer")]
            AverageMillisPerTransferPerDisk, // 90        
            [Description("Average Milliseconds Per Write")]
            AverageMillisPerWritePerDisk, // 91  

            [Description("IdleProcesses")]
            IdleProcesses, // 92  

            [Description("vCPU Usage (Percent)")]
            VmCPUUsage, // 93
            [Description("VM Memory Usage (Percent)")]
            VmMemUsage, // 94

            [Description("I/O")]
            IOWaits, // 95
            [Description("Lock")]
            LockWaits2, // 96
            [Description("Memory")]
            MemoryWaits, // 97
            [Description("Log")]
            TransactionLogWaits, // 98
            [Description("Other")]
            OtherWaits, // 99
            [Description("Signal")]
            SignalWaits, // 100

            [Description("Free")]
            BufferCacheFreePages, //101
            [Description("Database")]
            UsedDataMemory, //102
            [Description("Other")]
            LockOptimizerConnectionSortHashIndexMemory, //103
            [Description("Procedure Cache")]
            ProcedureCachePages, //104

            [Description("Database")]
            WaitTimeDatabase, //105
            [Description("Extent")]
            WaitTimeExtent, //106
            [Description("Key")]
            WaitTimeKey, //107
            [Description("Page")]
            WaitTimePage, //108
            [Description("RID")]
            WaitTimeRID, //109
            [Description("Table")]
            WaitTimeTable, //110

            [Description("Transactions Per Sec")]
            TransactionsPerSecond, //111
            [Description("LogFlushes Per Sec")]
            LogFlushesPerSecond2, //112
            [Description("NumberReads Per Sec")]
            NumberReadsPerSecond, //113
            [Description("NumberWrites Per Sec")]
            NumberWritesPerSecond, //114
            [Description("IoStallMS Per Sec")]
            IoStallMSPerSecond, //115

            [Description("Active Sessions")]
            ActiveSessions, //116

            [Description("Transactions")]
            CallRatesTransactions, //117

            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add SQL Services status
            [Description("SQL Browser Service Status")]
            SQLBrowserServiceStatus, // 118
            [Description("SQL Active Directory Helper Service Status")]
            SQLActiveDirectoryHelperServiceStatus // 119
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add SQL Services status

            // *** When adding a new entry to the end you must also change the size of the MetricsCategories array. ***
        }

        #endregion

        #region Categories

        private enum ServerDetailsMetricCategory
        {
            [Description("Memory")]
            Memory,
            [Description("Sessions")]
            Sessions,
            [Description("Services")]
            Services,
            [Description("Processor")]
            Processor,
            [Description("Disk")]
            Disk,
            [Description("SQL Activity")]
            SqlActivity,
            [Description("Network")]
            Network,
            [Description("Waits")]
            Waits,
            [Description("Cache")]
            Cache,
            [Description("Database")]
            Database,
            [Description("General")]
            General
        }

        public enum AlertArea
        {
            [Description("Sessions")]
            Sessions,
            [Description("Queries")]
            Queries,
            [Description("Resources")]
            Resources,
            [Description("Databases")]
            Databases,
            [Description("Services")]
            Services,
            [Description("Logs")]
            Logs,
            [Description("Operational")]
            Operational,
            [Description("Custom")]
            Custom
        }

        #endregion

        private static Dictionary<int, Triple<Metric?, string, Type>> lookupTable;
        private static ServerDetailsMetricCategory[] MetricsCategories;

        static ServerDetails()
        {
            lookupTable = new Dictionary<int, Triple<Metric?, string, Type>>();

            InitializeCategories();
            InitializeMetrics();
        }

        private static void InitializeCategories()
        {
            MetricsCategories = new ServerDetailsMetricCategory[(int)ServerDetailsMetrics.SQLActiveDirectoryHelperServiceStatus + 1];
            MetricsCategories[(int)ServerDetailsMetrics.AvailableMemory] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.MemoryUsed] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.Batches] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.BatchesPerSecond] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.BlockedSessions] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.BufferCacheHitRatio] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.BufferCacheSize] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.CheckpointWrites] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.CheckpointWritesPerSecond] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.ClientComputers] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.Connections] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.ConnectionsPerSecond] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.CPUActivity] = ServerDetailsMetricCategory.Processor;
            MetricsCategories[(int)ServerDetailsMetrics.DatabaseCount] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.DataFileCount] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.DataFileSpaceAllocated] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.DataFileSpaceUsed] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.DiskQueueLength] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.DiskTime] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.DtcServiceStatus] = ServerDetailsMetricCategory.Services;
            MetricsCategories[(int)ServerDetailsMetrics.FullScans] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.FullScansPerSecond] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.FullTextServiceStatus] = ServerDetailsMetricCategory.Services;
            MetricsCategories[(int)ServerDetailsMetrics.AgentServiceStatus] = ServerDetailsMetricCategory.Services;
            MetricsCategories[(int)ServerDetailsMetrics.SqlServerServiceStatus] = ServerDetailsMetricCategory.Services;
            MetricsCategories[(int)ServerDetailsMetrics.MaintenanceModeEnabled] = ServerDetailsMetricCategory.General;
            MetricsCategories[(int)ServerDetailsMetrics.OleAutomationStatus] = ServerDetailsMetricCategory.General;
            MetricsCategories[(int)ServerDetailsMetrics.WMIStatus] = ServerDetailsMetricCategory.General;
            MetricsCategories[(int)ServerDetailsMetrics.LazyWriterWrites] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.LazyWriterWritesPerSecond] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.LeadBlockers] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.LockWaits] = ServerDetailsMetricCategory.Processor;
            MetricsCategories[(int)ServerDetailsMetrics.LockWaitsPerSecond] = ServerDetailsMetricCategory.Processor;
            MetricsCategories[(int)ServerDetailsMetrics.LogFileCount] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.LogFileSpaceAllocated] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.LogFileSpaceUsed] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.LogFlushes] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.LogFlushesPerSecond] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.OpenTransactions] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.OsMetricsAvailability] = ServerDetailsMetricCategory.General;
            MetricsCategories[(int)ServerDetailsMetrics.OsCpuPrivilegedActivity] = ServerDetailsMetricCategory.Processor;
            MetricsCategories[(int)ServerDetailsMetrics.OsCpuUserTime] = ServerDetailsMetricCategory.Processor;
            MetricsCategories[(int)ServerDetailsMetrics.OsTotalProcessorActivity] = ServerDetailsMetricCategory.Processor;
            MetricsCategories[(int)ServerDetailsMetrics.PacketErrors] = ServerDetailsMetricCategory.Network;
            MetricsCategories[(int)ServerDetailsMetrics.PacketErrorsPerSecond] = ServerDetailsMetricCategory.Network;
            MetricsCategories[(int)ServerDetailsMetrics.PacketsReceived] = ServerDetailsMetricCategory.Network;
            MetricsCategories[(int)ServerDetailsMetrics.PacketsReceivedPerSecond] = ServerDetailsMetricCategory.Network;
            MetricsCategories[(int)ServerDetailsMetrics.PacketsSent] = ServerDetailsMetricCategory.Network;
            MetricsCategories[(int)ServerDetailsMetrics.PacketsSentPerSecond] = ServerDetailsMetricCategory.Network;
            MetricsCategories[(int)ServerDetailsMetrics.PageLifeExpectancy] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.PageLookups] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.PageLookupsPerSecond] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.PageReads] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.PageReadsPerSecond] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.PagesPerSecond] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.PageSplits] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.PageSplitsPerSecond] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.PageWrites] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.PageWritesPerSecond] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.PhysicalMemory] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.ProcedureCacheHitRatio] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.ProcedureCacheSize] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.ProcessorQueueLength] = ServerDetailsMetricCategory.Processor;
            MetricsCategories[(int)ServerDetailsMetrics.ReadAheadPages] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.ReadAheadPagesPerSecond] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.ReadWriteErrors] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.ResponseTime] = ServerDetailsMetricCategory.Network;
            MetricsCategories[(int)ServerDetailsMetrics.RunningSince] = ServerDetailsMetricCategory.General;
            MetricsCategories[(int)ServerDetailsMetrics.ServerVersion] = ServerDetailsMetricCategory.General;
            MetricsCategories[(int)ServerDetailsMetrics.SqlCompilations] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.SqlCompilationsPerSecond] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.SqlMemoryAllocated] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlMemoryUsed] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlMemoryUsedPercent] = ServerDetailsMetricCategory.Memory;
            MetricsCategories[(int)ServerDetailsMetrics.SqlRecompilations] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.SqlRecompilationsPerSecond] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.SystemProcesses] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.SystemProcessesConsumingCPU] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.UserConnectionsPercent] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.UserProcesses] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.UserProcessesConsumingCPU] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.WorkfilesCreated] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.WorkfilesCreatedPerSecond] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.WorktablesCreated] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.WorktablesCreatedPerSecond] = ServerDetailsMetricCategory.SqlActivity;
            MetricsCategories[(int)ServerDetailsMetrics.TotalDeadlocks] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.DiskReadsPerSecondPerDisk] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.DiskTransfersPerSecondPerDisk] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.DiskWritesPerSecondPerDisk] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.AverageMillisPerReadPerDisk] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.AverageMillisPerTransferPerDisk] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int)ServerDetailsMetrics.AverageMillisPerWritePerDisk] = ServerDetailsMetricCategory.Disk;
            MetricsCategories[(int) ServerDetailsMetrics.IdleProcesses] = ServerDetailsMetricCategory.Sessions;
            MetricsCategories[(int)ServerDetailsMetrics.VmCPUUsage] = ServerDetailsMetricCategory.Processor;
            MetricsCategories[(int)ServerDetailsMetrics.VmMemUsage] = ServerDetailsMetricCategory.Memory;

            MetricsCategories[(int)ServerDetailsMetrics.IOWaits] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.LockWaits2] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.MemoryWaits] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.TransactionLogWaits] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.OtherWaits] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.SignalWaits] = ServerDetailsMetricCategory.Waits;

            MetricsCategories[(int)ServerDetailsMetrics.BufferCacheFreePages] = ServerDetailsMetricCategory.Cache;
            MetricsCategories[(int)ServerDetailsMetrics.UsedDataMemory] = ServerDetailsMetricCategory.Cache;
            MetricsCategories[(int)ServerDetailsMetrics.LockOptimizerConnectionSortHashIndexMemory] = ServerDetailsMetricCategory.Cache;
            MetricsCategories[(int)ServerDetailsMetrics.ProcedureCachePages] = ServerDetailsMetricCategory.Cache;

            MetricsCategories[(int)ServerDetailsMetrics.WaitTimeDatabase] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.WaitTimeExtent] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.WaitTimeKey] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.WaitTimePage] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.WaitTimeRID] = ServerDetailsMetricCategory.Waits;
            MetricsCategories[(int)ServerDetailsMetrics.WaitTimeTable] = ServerDetailsMetricCategory.Waits;

            MetricsCategories[(int)ServerDetailsMetrics.TransactionsPerSecond] = ServerDetailsMetricCategory.Database;
            MetricsCategories[(int)ServerDetailsMetrics.LogFlushesPerSecond2] = ServerDetailsMetricCategory.Database;
            MetricsCategories[(int)ServerDetailsMetrics.NumberReadsPerSecond] = ServerDetailsMetricCategory.Database;
            MetricsCategories[(int)ServerDetailsMetrics.NumberWritesPerSecond] = ServerDetailsMetricCategory.Database;
            MetricsCategories[(int)ServerDetailsMetrics.IoStallMSPerSecond] = ServerDetailsMetricCategory.Database;

            MetricsCategories[(int)ServerDetailsMetrics.ActiveSessions] = ServerDetailsMetricCategory.Sessions;

            MetricsCategories[(int)ServerDetailsMetrics.CallRatesTransactions] = ServerDetailsMetricCategory.Processor;
            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update metric categories array
            MetricsCategories[(int)ServerDetailsMetrics.SQLBrowserServiceStatus] = ServerDetailsMetricCategory.Services;
            MetricsCategories[(int)ServerDetailsMetrics.SQLActiveDirectoryHelperServiceStatus] = ServerDetailsMetricCategory.Services;
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update metric categories array
        }

        private static void InitializeMetrics()
        {
            InitializeMetric(ServerDetailsMetrics.AvailableMemory,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AvailableMemory),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.MemoryUsed,
                Metric.OSMemoryUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.MemoryUsed),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.BlockedSessions,
                Metric.BlockedSessions,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BlockedSessions),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.BufferCacheHitRatio,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BufferCacheHitRatio),
                typeof(float));

            InitializeMetric(ServerDetailsMetrics.BufferCacheSize,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BufferCacheSize),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.CheckpointWrites,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.CheckpointWrites),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.CheckpointWritesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.CheckpointWritesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ClientComputers,
                Metric.ClientComputers,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ClientComputers),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.Connections,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.Connections),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ConnectionsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ConnectionsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.CPUActivity,
                Metric.SQLCPUUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.CPUActivity),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.DatabaseCount,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DatabaseCount),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.DataFileCount,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DataFileCount),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.DataFileSpaceAllocated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DataFileSpaceAllocated),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.DataFileSpaceUsed,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DataFileSpaceUsed),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.DiskQueueLength,
                Metric.OSDiskAverageDiskQueueLength,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DiskQueueLength),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.DiskTime,
                Metric.OSDiskPhysicalDiskTimePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DiskTime),
                typeof(double));

            //InitializeMetric(ServerDetailsMetrics.DtcServiceStatus,
            //    Metric.DtcServiceStatus,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DtcServiceStatus),
            //    typeof(ServiceState));

            InitializeMetric(ServerDetailsMetrics.FullScans,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.FullScans),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.FullScansPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.FullScansPerSecond),
                typeof(long));

            //InitializeMetric(ServerDetailsMetrics.FullTextServiceStatus,
            //    Metric.FullTextServiceStatus,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.FullTextServiceStatus),
            //    typeof(ServiceState));

            InitializeMetric(ServerDetailsMetrics.LazyWriterWrites,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LazyWriterWrites),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LazyWriterWritesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LazyWriterWritesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LeadBlockers,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LeadBlockers),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.LockWaits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LockWaits),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LockWaitsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LockWaitsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LogFileCount,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFileCount),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.LogFileSpaceAllocated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFileSpaceAllocated),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.LogFileSpaceUsed,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFileSpaceUsed),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.LogFlushes,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFlushes),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.LogFlushesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFlushesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.MaintenanceModeEnabled,
                Metric.MaintenanceMode,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.MaintenanceModeEnabled),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.OleAutomationStatus,
                Metric.OLEAutomationStatus,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OleAutomationStatus),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.OpenTransactions,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OpenTransactions),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.OsMetricsAvailability,
                Metric.OSMetricsStatus,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsMetricsAvailability),
                typeof(string));

            InitializeMetric(ServerDetailsMetrics.OsCpuPrivilegedActivity,
                Metric.OSCPUPrivilegedTimePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsCpuPrivilegedActivity),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.OsCpuUserTime,
                Metric.OSUserCPUUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsCpuUserTime),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.OsTotalProcessorActivity,
                Metric.OSCPUUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OsTotalProcessorActivity),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.PacketErrors,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketErrors),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketErrorsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketErrorsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketsReceived,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketsReceived),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketsReceivedPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketsReceivedPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketsSent,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketsSent),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PacketsSentPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PacketsSentPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageLifeExpectancy,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageLifeExpectancy),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageLookups,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageLookups),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageLookupsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageLookupsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageReads,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageReads),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageReadsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageReadsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PagesPerSecond,
                Metric.OSMemoryPagesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PagesPerSecond),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.PageSplits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageSplits),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageSplitsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageSplitsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageWrites,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageWrites),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PageWritesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PageWritesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.PhysicalMemory,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.PhysicalMemory),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.ProcedureCacheHitRatio,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ProcedureCacheHitRatio),
                typeof(float));

            InitializeMetric(ServerDetailsMetrics.ProcedureCacheSize,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ProcedureCacheSize),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.ProcessorQueueLength,
                Metric.OSCPUProcessorQueueLength,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ProcessorQueueLength),
                typeof(ulong));

            InitializeMetric(ServerDetailsMetrics.ReadAheadPages,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ReadAheadPages),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ReadAheadPagesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ReadAheadPagesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ReadWriteErrors,
                Metric.ReadWriteErrors,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ReadWriteErrors),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.ResponseTime,
                Metric.ServerResponseTime,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ResponseTime),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.RunningSince,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.RunningSince),
                typeof(DateTime));

            InitializeMetric(ServerDetailsMetrics.SqlCompilations,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlCompilations),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.SqlCompilationsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlCompilationsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.SqlRecompilations,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlRecompilations),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.SqlRecompilationsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlRecompilationsPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.SqlMemoryAllocated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlMemoryAllocated),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.SqlMemoryUsed,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlMemoryUsed),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.SqlMemoryUsedPercent,
                Metric.SQLMemoryUsagePct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlMemoryUsedPercent),
                typeof(decimal));

            //InitializeMetric(ServerDetailsMetrics.AgentServiceStatus,
            //    Metric.AgentServiceStatus,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AgentServiceStatus),
            //    typeof(ServiceState));

            //InitializeMetric(ServerDetailsMetrics.SqlServerServiceStatus,
            //    Metric.SqlServiceStatus,
            //    ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SqlServerServiceStatus),
            //    typeof(ServiceState));

            InitializeMetric(ServerDetailsMetrics.SystemProcesses,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SystemProcesses),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.SystemProcessesConsumingCPU,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SystemProcessesConsumingCPU),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.ServerVersion,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ServerVersion),
                typeof(string));

            InitializeMetric(ServerDetailsMetrics.Batches,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.Batches),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.BatchesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BatchesPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.UserConnectionsPercent,
                Metric.UserConnectionPct,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.UserConnectionsPercent),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.UserProcesses,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.UserProcesses),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.UserProcessesConsumingCPU,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.UserProcessesConsumingCPU),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.WMIStatus,
                Metric.WMIStatus,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WMIStatus),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.WorkfilesCreated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WorkfilesCreated),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.WorkfilesCreatedPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WorkfilesCreatedPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.WorktablesCreated,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WorktablesCreated),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.WorktablesCreatedPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WorktablesCreatedPerSecond),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.TotalDeadlocks,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.TotalDeadlocks),
                typeof(long));

            InitializeMetric(ServerDetailsMetrics.DiskReadsPerSecondPerDisk,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DiskReadsPerSecondPerDisk),
                typeof(long));
            InitializeMetric(ServerDetailsMetrics.DiskTransfersPerSecondPerDisk,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DiskTransfersPerSecondPerDisk),
                typeof(long));
            InitializeMetric(ServerDetailsMetrics.DiskWritesPerSecondPerDisk,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.DiskWritesPerSecondPerDisk),
                typeof(long));
            InitializeMetric(ServerDetailsMetrics.AverageMillisPerReadPerDisk,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AverageMillisPerReadPerDisk),
                typeof(long));
            InitializeMetric(ServerDetailsMetrics.AverageMillisPerTransferPerDisk,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AverageMillisPerTransferPerDisk),
                typeof(long));
            InitializeMetric(ServerDetailsMetrics.AverageMillisPerWritePerDisk,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.AverageMillisPerWritePerDisk),
                typeof(long));
            InitializeMetric(ServerDetailsMetrics.IdleProcesses,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.IdleProcesses),
                typeof(int));
            InitializeMetric(ServerDetailsMetrics.VmCPUUsage,
                Metric.VmCPUUtilization,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmCPUUsage),
                typeof(double));
            InitializeMetric(ServerDetailsMetrics.VmMemUsage,
                Metric.VmMemoryUtilization,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.VmMemUsage),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.IOWaits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.IOWaits),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.LockWaits2,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LockWaits2),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.MemoryWaits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.MemoryWaits),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.TransactionLogWaits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.TransactionLogWaits),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.OtherWaits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.OtherWaits),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.SignalWaits,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.SignalWaits),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.BufferCacheFreePages,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.BufferCacheFreePages),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.UsedDataMemory,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.UsedDataMemory),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.LockOptimizerConnectionSortHashIndexMemory,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LockOptimizerConnectionSortHashIndexMemory),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.ProcedureCachePages,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ProcedureCachePages),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.WaitTimeDatabase,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WaitTimeDatabase),
                typeof(double));
            InitializeMetric(ServerDetailsMetrics.WaitTimeExtent,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WaitTimeExtent),
                typeof(double));
            InitializeMetric(ServerDetailsMetrics.WaitTimeKey,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WaitTimeKey),
                typeof(double));
            InitializeMetric(ServerDetailsMetrics.WaitTimePage,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WaitTimePage),
                typeof(double));
            InitializeMetric(ServerDetailsMetrics.WaitTimeRID,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WaitTimeRID),
                typeof(double));
            InitializeMetric(ServerDetailsMetrics.WaitTimeTable,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.WaitTimeTable),
                typeof(double));

            InitializeMetric(ServerDetailsMetrics.TransactionsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.TransactionsPerSecond),
                typeof(long));
            InitializeMetric(ServerDetailsMetrics.LogFlushesPerSecond2,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.LogFlushesPerSecond2),
                typeof(long));
            InitializeMetric(ServerDetailsMetrics.NumberReadsPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.NumberReadsPerSecond),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.NumberWritesPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.NumberWritesPerSecond),
                typeof(decimal));
            InitializeMetric(ServerDetailsMetrics.IoStallMSPerSecond,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.IoStallMSPerSecond),
                typeof(decimal));

            InitializeMetric(ServerDetailsMetrics.ActiveSessions,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.ActiveSessions),
                typeof(int));

            InitializeMetric(ServerDetailsMetrics.CallRatesTransactions,
                ApplicationHelper.GetEnumDescription(ServerDetailsMetrics.CallRatesTransactions),
                typeof(long));
        }

        private static void InitializeMetric(ServerDetailsMetrics metricType, string metricName, Type dataType)
        {
            InitializeMetric(metricType, null, metricName, dataType);
        }

        private static void InitializeMetric(ServerDetailsMetrics metricType, Metric? metric, string metricName, Type dataType)
        {
            //DataRow metricRow = detailsGridDataSource.Rows.Add(new object[] { false, metricName });
            //metricRow["AlertMetric"] = metric == null ? (object)DBNull.Value : metric.Value;
            //metricRow["Metric Type"] = metricType;
            //metricRow["State"] = 0;
            //metricRow["Custom Counter"] = "No";
            //SetCategory(metricType, metricRow);
            //DataColumn metricColumn = chartRealTimeDataTable.Columns.Add(metricName, dataType);
            lookupTable.Add((int)metricType, new Triple<Metric?,string,Type>(metric, metricName, dataType));
        }

        private static DataSet CreateServerStatusDataSet()
        {
            DataSet result = new DataSet("ServerStatus");
            DataTable alerts = result.Tables.Add("Alerts");
            alerts.Columns.Add("NumberWarning", typeof (int));
            alerts.Columns.Add("NumberCritical", typeof (int));
            alerts.Columns.Add("NumberInformational", typeof (int));
            alerts.Columns.Add("LastScheduledRefresh", typeof (DateTime));

            DataTable cats = result.Tables.Add("Areas");
            cats.Columns.Add("Area", typeof (string));
            cats.Columns.Add("State", typeof (int));
            cats.Columns.Add("NumberWarning", typeof (int));
            cats.Columns.Add("NumberCritical", typeof(int));
            // cats.Columns.Add("NumberInformational", typeof (int));

            DataTable details = result.Tables.Add("Properties");
            details.Columns.Add("Name", typeof (string));
            details.Columns.Add("Value", typeof (string));

            return result;
        }

        internal static DataSet RollupServerStatus(int instanceId, ServerOverview currentSnapshot)
        {
            DataSet result = CreateServerStatusDataSet();

            MonitoredSqlServerState state = Management.ScheduledCollection.GetCachedMonitoredSqlServer(instanceId);
            if (state == null)
                throw new ApplicationException("Unable to locate monitored server definition");

            DataTable alerts = result.Tables["Alerts"];
            DataTable areas = result.Tables["Areas"];
            DataTable props = result.Tables["Properties"];

            CreateSummary(state, currentSnapshot, areas, props, alerts);

            return result;
        }

        private const string GetDatabaseCountersStoredProcedure = "p_GetDatabaseCounters";
        internal static void GetDatabaseGraphData(int instanceId, TimeSpan history, DataTable result, Dictionary<Pair<int, string>, MetricThresholdEntry> thresholds)
        {            
            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetDatabaseCountersStoredProcedure, instanceId, DateTime.UtcNow, (int)history.TotalMinutes))
                {
                    DateTime? timestamp;
                    string dbname;

                    while (dataReader.Read())
                    {
                        // no use (currently) for rows with null db names or no timestamps
                        if (dataReader["DatabaseName"] == DBNull.Value || dataReader["UTCCollectionDateTime"] == DBNull.Value)
                            continue;

                        dbname    = (string)dataReader["DatabaseName"];
                        timestamp = (DateTime)dataReader["UTCCollectionDateTime"];

                        decimal? timeDeltaInSeconds = null;
                        object timedelta = dataReader["TimeDeltaInSeconds"];
                        if (timedelta != DBNull.Value)
                        {
                            timeDeltaInSeconds = Convert.ToDecimal(timedelta);
                        }

                        bool validTimeDelta = timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0;

                        if (dataReader["Transactions"] != DBNull.Value && validTimeDelta)
                            AddDatabaseStatsRow(result, timestamp, ServerDetailsMetrics.TransactionsPerSecond, dbname, (long)dataReader["Transactions"] / timeDeltaInSeconds, MonitoredState.OK, DBNull.Value);

                        if (dataReader["LogFlushes"] != DBNull.Value && validTimeDelta)
                            AddDatabaseStatsRow(result, timestamp, ServerDetailsMetrics.LogFlushesPerSecond2, dbname, (long)dataReader["LogFlushes"] / timeDeltaInSeconds, MonitoredState.OK, DBNull.Value);

                        if (dataReader["BytesRead"] != DBNull.Value && validTimeDelta)
                            AddDatabaseStatsRow(result, timestamp, ServerDetailsMetrics.NumberReadsPerSecond, dbname, Convert.ToDecimal(dataReader["BytesRead"]) / timeDeltaInSeconds, MonitoredState.OK, DBNull.Value);

                        if (dataReader["BytesWritten"] != DBNull.Value && validTimeDelta)
                            AddDatabaseStatsRow(result, timestamp, ServerDetailsMetrics.NumberWritesPerSecond, dbname, Convert.ToDecimal(dataReader["BytesWritten"]) / timeDeltaInSeconds, MonitoredState.OK, DBNull.Value);

                        if (dataReader["IoStallMS"] != DBNull.Value && validTimeDelta)
                            AddDatabaseStatsRow(result, timestamp, ServerDetailsMetrics.IoStallMSPerSecond, dbname, Convert.ToDecimal(dataReader["IoStallMS"]) / timeDeltaInSeconds, MonitoredState.OK, DBNull.Value);
                    }
                }
            }
        }
      
        private const string GetServerSummaryStoredProcedure = "p_GetServerSummary";
        internal static DataTable GetMetricGraphData(int instanceId, TimeSpan history, DataTable result, Dictionary<Pair<int, string>, MetricThresholdEntry> thresholds, out ServerOverview lastOverview)
        {
            if (result == null)
                result = CreateServerDetailsTable();
            
            lastOverview = null;

            // execute query to get data for the time period))
            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {    
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, GetServerSummaryStoredProcedure, instanceId,
                                            DateTime.UtcNow, (int)history.TotalMinutes))
                {
                    // read past historical server status data
                    dataReader.Read();
                    
                    if (dataReader.NextResult())
                    {
                        DataTable overviewStatistics = WebClient.GetTable(dataReader, false);
                        List<string> drives = new List<string>();
                        DataTable diskDriveMetrics = null;

                        if (dataReader.NextResult())
                        {
                            while (dataReader.Read())
                            {
                                drives.Add(dataReader["DriveName"] as string);
                            }
                            
                            if (dataReader.NextResult())
                            {
                                diskDriveMetrics = WebClient.GetTable(dataReader, false);
                            }
                        }

                        foreach (DataRow rawRow in overviewStatistics.Rows)
                        {
                            string instanceName = Convert.ToString(rawRow["InstanceName"]);
                            ServerOverview snapshot = new ServerOverview(instanceName, rawRow, new string[0], true, new DataRow[0]);
                            //snapshot.CalculateWaitStatisticsSummary(ManagementService.GetWaitTypes());
                            SetMetricGraphRows(snapshot, result, thresholds);
                            lastOverview = snapshot;
                        }
                        if (diskDriveMetrics != null)
                        {
                            foreach (DataRow row in diskDriveMetrics.Rows)
                            {
                                if (row.IsNull("CollectionDateTime")) continue;
                                DateTime dateTime = (DateTime) row["CollectionDateTime"];
                                string resource = row["DriveName"] as string;
                                if (resource == null) continue;

                                if (!row.IsNull("DiskReadsPerSecond"))
                                    AddResourceRow(result, dateTime, resource, ServerDetailsMetrics.DiskReadsPerSecondPerDisk, row["DiskReadsPerSecond"]);
                                if (!row.IsNull("DiskTransfersPerSecond"))
                                    AddResourceRow(result, dateTime, resource, ServerDetailsMetrics.DiskTransfersPerSecondPerDisk, row["DiskTransfersPerSecond"]);
                                if (!row.IsNull("DiskWritesPerSecond"))
                                    AddResourceRow(result, dateTime, resource, ServerDetailsMetrics.DiskWritesPerSecondPerDisk, row["DiskWritesPerSecond"]);

                                if (!row.IsNull("AverageDiskMillisecondsPerRead"))
                                    AddResourceRow(result, dateTime, resource, ServerDetailsMetrics.AverageMillisPerReadPerDisk, row["AverageDiskMillisecondsPerRead"]);
                                if (!row.IsNull("AverageDiskMillisecondsPerTransfer"))
                                    AddResourceRow(result, dateTime, resource, ServerDetailsMetrics.AverageMillisPerTransferPerDisk, row["AverageDiskMillisecondsPerTransfer"]);
                                if (!row.IsNull("AverageDiskMillisecondsPerWrite"))
                                    AddResourceRow(result, dateTime, resource, ServerDetailsMetrics.AverageMillisPerWritePerDisk, row["AverageDiskMillisecondsPerWrite"]);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static DataRow AddResourceRow(DataTable stats, DateTime collected, string resource, ServerDetailsMetrics metricId, object value)
        {
            DataRow resultRow = stats.NewRow();
            resultRow["CollectionDateTime"] = collected;
            resultRow["Metric"] = (int)metricId;
            resultRow["Instance"] = (object)resource ?? DBNull.Value;
            resultRow["Value"] = value;
            resultRow["State"] = (byte)MonitoredState.OK;
            stats.Rows.Add(resultRow);
            return resultRow;
        }

        public static DataTable CreateServerDetailsTable()
        {
            DataTable result = new DataTable("MetricData");
            result.Columns.Add("CollectionDateTime", typeof(DateTime));
            result.Columns.Add("Metric", typeof(int));
            result.Columns.Add("Instance", typeof (string));
            result.Columns.Add("Value", typeof(object));
            result.Columns.Add("State", typeof(byte));
            result.Columns.Add("AlertMetric", typeof (int));
            return result;
        }

        private static readonly ServerDetailsMetrics[] ServerDetailsViewMetrics = 
        {
            // Sessions
            ServerDetailsMetrics.ResponseTime,
            ServerDetailsMetrics.SystemProcesses,
            ServerDetailsMetrics.OpenTransactions,
            //ServerDetailsMetrics.UserProcessesConsumingCPU,
            ServerDetailsMetrics.SystemProcessesConsumingCPU,
            ServerDetailsMetrics.BlockedSessions,
            ServerDetailsMetrics.ClientComputers,
            ServerDetailsMetrics.LeadBlockers,
            ServerDetailsMetrics.TotalDeadlocks,
            ServerDetailsMetrics.IdleProcesses,
            ServerDetailsMetrics.ActiveSessions,
            // Network
            ServerDetailsMetrics.PacketsSentPerSecond,
            ServerDetailsMetrics.PacketsReceivedPerSecond,
            //ServerDetailsMetrics.PacketErrorsPerSecond,
            ServerDetailsMetrics.BatchesPerSecond,
            // CPU
            ServerDetailsMetrics.OsTotalProcessorActivity,
            ServerDetailsMetrics.CPUActivity,
            ServerDetailsMetrics.OsCpuUserTime,
            ServerDetailsMetrics.OsCpuPrivilegedActivity,
            ServerDetailsMetrics.ProcessorQueueLength,
            ServerDetailsMetrics.SqlCompilationsPerSecond,
            ServerDetailsMetrics.SqlRecompilationsPerSecond,
            ServerDetailsMetrics.VmCPUUsage,
            ServerDetailsMetrics.CallRatesTransactions,
            // Memory
            ServerDetailsMetrics.MemoryUsed,
            ServerDetailsMetrics.SqlMemoryAllocated,
            ServerDetailsMetrics.SqlMemoryUsed,
            //ServerDetailsMetrics.SqlMemoryUsedPercent,
            ServerDetailsMetrics.PageLifeExpectancy,
            ServerDetailsMetrics.BufferCacheHitRatio,
            ServerDetailsMetrics.ProcedureCacheHitRatio,
            ServerDetailsMetrics.PagesPerSecond,
            ServerDetailsMetrics.VmMemUsage,
            // Disk
            ServerDetailsMetrics.DiskTime,
            ServerDetailsMetrics.ReadWriteErrors,
            // Following 5 are for Server PIO graph            
            ServerDetailsMetrics.PageReadsPerSecond,
            ServerDetailsMetrics.PageWritesPerSecond,
            ServerDetailsMetrics.CheckpointWritesPerSecond,
            ServerDetailsMetrics.LazyWriterWritesPerSecond,
            ServerDetailsMetrics.ReadAheadPagesPerSecond,
            // SQL Server I/O
            ServerDetailsMetrics.CheckpointWrites,
            ServerDetailsMetrics.LazyWriterWrites,
            ServerDetailsMetrics.ReadAheadPages,
            ServerDetailsMetrics.PageReads,
            ServerDetailsMetrics.PageWrites,
            // Waits
            ServerDetailsMetrics.IOWaits,
            ServerDetailsMetrics.LockWaits2,
            ServerDetailsMetrics.MemoryWaits,
            ServerDetailsMetrics.TransactionLogWaits,
            ServerDetailsMetrics.OtherWaits,
            ServerDetailsMetrics.SignalWaits,
            ServerDetailsMetrics.WaitTimeDatabase,
            ServerDetailsMetrics.WaitTimeExtent,
            ServerDetailsMetrics.WaitTimeKey,
            ServerDetailsMetrics.WaitTimePage,
            ServerDetailsMetrics.WaitTimeRID,
            ServerDetailsMetrics.WaitTimeTable,
            // Cache
            ServerDetailsMetrics.BufferCacheFreePages,
            ServerDetailsMetrics.UsedDataMemory,
            ServerDetailsMetrics.LockOptimizerConnectionSortHashIndexMemory,
            ServerDetailsMetrics.ProcedureCachePages,
            // Database
            ServerDetailsMetrics.TransactionsPerSecond,
            ServerDetailsMetrics.LogFlushesPerSecond2,
            ServerDetailsMetrics.NumberReadsPerSecond,
            ServerDetailsMetrics.NumberWritesPerSecond,
            ServerDetailsMetrics.IoStallMSPerSecond
        };

        public static void SetMetricGraphRows(ServerOverview overview, DataTable table, Dictionary<Pair<int, string>, MetricThresholdEntry> thresholds)
        {
            foreach (ServerDetailsMetrics metricId in ServerDetailsViewMetrics)
            {
                // these metrics are handled in SetDatabaseMetricGraphRows
                if (metricId == ServerDetailsMetrics.TransactionsPerSecond ||
                    metricId == ServerDetailsMetrics.LogFlushesPerSecond2 ||
                    metricId == ServerDetailsMetrics.NumberReadsPerSecond ||
                    metricId == ServerDetailsMetrics.NumberWritesPerSecond ||
                    metricId == ServerDetailsMetrics.IoStallMSPerSecond)
                {
                    continue;
                }

                DataRow resultrow = table.NewRow();
                
                SetMetricGraphRow(metricId, overview, resultrow, thresholds);
                table.Rows.Add(resultrow);
            }

            // Add db related stats
            SetDatabaseMetricGraphRows(overview, table, thresholds);
        }

        private static MetricThresholdEntry GetThresholdEntry(Metric? metricId, Dictionary<Pair<int, string>, MetricThresholdEntry> thresholds)
        {
            if (metricId == null || !metricId.HasValue)
                return null;

            foreach (Pair<int, string> key in thresholds.Keys)
            {
                if ((Metric)key.First == metricId.Value)
                    return thresholds[key];
            }

            return null;
        }

        private static void SetMetricGraphRow(ServerDetailsMetrics metricId, ServerOverview newSnapshot, DataRow resultRow, Dictionary<Pair<int, string>, MetricThresholdEntry> thresholds)
        {
            if (newSnapshot.CollectionFailed && newSnapshot.Error != null)
                throw newSnapshot.Error;            

            resultRow["CollectionDateTime"] = newSnapshot.ServerTimeUTC;
            resultRow["Metric"] = (int)metricId;
            object value = DBNull.Value;

            string columnName = metricId.ToString();

            // get the metadata for the server detail metric
            Triple<Metric?, string, Type>? metadata = null;
            if (lookupTable.ContainsKey((int)metricId))
                metadata = lookupTable[(int)metricId];

            // get the threshold entry if the server detail metric is associated with an alertable metric
            MetricThresholdEntry threshold = null;
            if (metadata.HasValue && metadata.Value.First.HasValue)
            {   // assign the alert metric if this metric has one associated with it

                threshold = GetThresholdEntry(metadata.Value.First, thresholds);

                if (threshold != null)
                    resultRow["AlertMetric"] = threshold.MetricID;
                else
                    resultRow["AlertMetric"] = DBNull.Value;
            }

            switch (metricId)
            {
                case ServerDetailsMetrics.AvailableMemory:
                    if (newSnapshot.OSMetricsStatistics.AvailableBytes.Megabytes.HasValue)
                    {
                        value = newSnapshot.OSMetricsStatistics.AvailableBytes.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.MemoryUsed:
                    if (newSnapshot.OSMetricsStatistics.AvailableBytes.Megabytes.HasValue &&
                        newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                    {
                        value = (newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.Value -
                                    newSnapshot.OSMetricsStatistics.AvailableBytes.Megabytes.Value);
                    }
                    break;
                case ServerDetailsMetrics.BlockedSessions:
                    if (newSnapshot.SystemProcesses.BlockedProcesses.HasValue)
                    {
                         value = newSnapshot.SystemProcesses.BlockedProcesses.Value;
                    }
                    break;
                case ServerDetailsMetrics.TotalDeadlocks:
                    if (newSnapshot.LockCounters.TotalCounters.Deadlocks.HasValue)
                    {
                        value = newSnapshot.LockCounters.TotalCounters.Deadlocks.Value;
                    }
                    break;
                case ServerDetailsMetrics.BufferCacheHitRatio:
                    if (newSnapshot.Statistics.BufferCacheHitRatio.HasValue)
                    {
                        value = newSnapshot.Statistics.BufferCacheHitRatio.Value;
                    }
                    break;
                case ServerDetailsMetrics.BufferCacheSize:
                    if (newSnapshot.Statistics.BufferCacheSize.Megabytes.HasValue)
                    {
                        value = newSnapshot.Statistics.BufferCacheSize.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.CheckpointWrites:
                    if (newSnapshot.Statistics.CheckpointPages.HasValue)
                    {
                        value = newSnapshot.Statistics.CheckpointPages.Value;
                    }
                    break;
                case ServerDetailsMetrics.CheckpointWritesPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.CheckpointPages.HasValue)
                    {
                        value = newSnapshot.Statistics.CheckpointPages.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.ClientComputers:
                    if (newSnapshot.SystemProcesses.ComputersHoldingProcesses.HasValue)
                    {
                        value = newSnapshot.SystemProcesses.ComputersHoldingProcesses.Value;
                    }
                    break;
                case ServerDetailsMetrics.Connections:
                     if (newSnapshot.Statistics.TotalConnections.HasValue)
                     {
                         value = newSnapshot.Statistics.TotalConnections.Value;
                     }
                    break;
                case ServerDetailsMetrics.ConnectionsPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.TotalConnections.HasValue)
                    {
                        value = newSnapshot.Statistics.TotalConnections.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.CPUActivity:
                    if (newSnapshot.Statistics.CpuPercentage.HasValue)
                    {
                        value = newSnapshot.Statistics.CpuPercentage.Value;
                    }
                    break;
                case ServerDetailsMetrics.DatabaseCount:
                    if (newSnapshot.DatabaseSummary.DatabaseCount.HasValue)
                    {
                        value = newSnapshot.DatabaseSummary.DatabaseCount.Value;
                    }
                    break;
                case ServerDetailsMetrics.DataFileCount:
                    if (newSnapshot.DatabaseSummary.DataFileCount.HasValue)
                    {
                        value = newSnapshot.DatabaseSummary.DataFileCount.Value;
                    }
                    break;
                case ServerDetailsMetrics.DataFileSpaceAllocated:
                    if (newSnapshot.DatabaseSummary.DataFileSpaceAllocated.Megabytes.HasValue)
                    {
                        value = newSnapshot.DatabaseSummary.DataFileSpaceAllocated.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.DataFileSpaceUsed:
                    if (newSnapshot.DatabaseSummary.DataFileSpaceUsed.Megabytes.HasValue)
                    {
                        value = newSnapshot.DatabaseSummary.DataFileSpaceUsed.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.DiskQueueLength:
                    if (newSnapshot.OSMetricsStatistics.AvgDiskQueueLength.HasValue)
                    {
                        value = newSnapshot.OSMetricsStatistics.AvgDiskQueueLength.Value;
                    }
                    break;
                case ServerDetailsMetrics.DiskTime:
                    if (newSnapshot.OSMetricsStatistics.PercentDiskTime.HasValue)
                    {
                        value = newSnapshot.OSMetricsStatistics.PercentDiskTime.Value;
                    }
                    break;
                case ServerDetailsMetrics.FullScans:
                    if (newSnapshot.Statistics.FullScans.HasValue)
                    {
                        value = newSnapshot.Statistics.FullScans.Value;
                    }
                    break;
                case ServerDetailsMetrics.FullScansPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.FullScans.HasValue)
                    {
                        value = newSnapshot.Statistics.FullScans.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.LazyWriterWrites:
                    if (newSnapshot.Statistics.LazyWrites.HasValue)
                    {
                        value = newSnapshot.Statistics.LazyWrites.Value;
                    }
                    break;
                case ServerDetailsMetrics.LazyWriterWritesPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.LazyWrites.HasValue)
                    {
                        value = newSnapshot.Statistics.LazyWrites.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.LeadBlockers:
                    if (newSnapshot.SystemProcesses.LeadBlockers.HasValue)
                    {
                        value = newSnapshot.SystemProcesses.LeadBlockers.Value;
                    }
                    break;
                case ServerDetailsMetrics.LockWaits:
                    if (newSnapshot.Statistics.LockWaits.HasValue)
                    {
                        value = newSnapshot.Statistics.LockWaits.Value;
                    }
                    break;
                case ServerDetailsMetrics.LockWaitsPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.LockWaits.HasValue)
                    {
                        value = newSnapshot.Statistics.LockWaits.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.LogFileCount:
                    if (newSnapshot.DatabaseSummary.LogFileCount.HasValue)
                    {
                        value = newSnapshot.DatabaseSummary.LogFileCount.Value;
                    }
                    break;
                case ServerDetailsMetrics.LogFileSpaceAllocated:
                    if (newSnapshot.DatabaseSummary.LogFileSpaceAllocated.Megabytes.HasValue)
                    {
                        value = newSnapshot.DatabaseSummary.LogFileSpaceAllocated.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.LogFileSpaceUsed:
                    if (newSnapshot.DatabaseSummary.LogFileSpaceUsed.Megabytes.HasValue)
                    {
                        value = newSnapshot.DatabaseSummary.LogFileSpaceUsed.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.LogFlushes:
                    if (newSnapshot.Statistics.LogFlushes.HasValue)
                    {
                        value =
                            newSnapshot.Statistics.LogFlushes;
                    }
                    break;
                case ServerDetailsMetrics.LogFlushesPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.LogFlushes.HasValue)
                    {
                        value = newSnapshot.Statistics.LogFlushes.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.MaintenanceModeEnabled:
                    //bool maintenanceModeEnabled =
                    //ApplicationModel.Default.ActiveInstances[instanceId].MaintenanceModeEnabled;
                    //dataRow[lookupTable[metric].First] = maintenanceModeEnabled
                    //                                         ? OptionStatus.Enabled
                    //                                         : OptionStatus.Disabled;
                    break;
                case ServerDetailsMetrics.OleAutomationStatus:
                    //OptionStatus optionStatus =
                    //(newSnapshot.OSMetricsStatistics.OsMetricsStatus == OSMetricsStatus.OLEAutomationUnavailable) ||
                    //(newSnapshot.OSMetricsStatistics.OsMetricsStatus == OSMetricsStatus.Disabled)
                    //                            ? OptionStatus.Disabled
                    //                            : OptionStatus.Enabled;
                    break;
                case ServerDetailsMetrics.OpenTransactions:
                    if (newSnapshot.SystemProcesses.OpenTransactions.HasValue)
                    {
                        value = newSnapshot.SystemProcesses.OpenTransactions.Value;
                    }
                    break;
                case ServerDetailsMetrics.OsMetricsAvailability:
                    //dataRow[lookupTable[metric].First] =
                    //newSnapshot.OSMetricsStatistics.OsStatisticAvailability;
                    break;
                case ServerDetailsMetrics.OsCpuPrivilegedActivity:
                    if (newSnapshot.OSMetricsStatistics.PercentPrivilegedTime.HasValue)
                    {
                        value = newSnapshot.OSMetricsStatistics.PercentPrivilegedTime.Value;
                    }
                    break;
                case ServerDetailsMetrics.OsCpuUserTime:
                    if (newSnapshot.OSMetricsStatistics.PercentUserTime.HasValue)
                    {
                       value = newSnapshot.OSMetricsStatistics.PercentUserTime.Value;
                    }
                    break;
                case ServerDetailsMetrics.OsTotalProcessorActivity:
                    if (newSnapshot.OSMetricsStatistics.PercentProcessorTime.HasValue)
                    {
                        value = newSnapshot.OSMetricsStatistics.PercentProcessorTime.Value;
                    }
                    break;
                case ServerDetailsMetrics.PacketErrors:
                    if (newSnapshot.Statistics.PacketErrors.HasValue)
                    {
                        value = newSnapshot.Statistics.PacketErrors.Value;
                    }
                    break;
                case ServerDetailsMetrics.PacketErrorsPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.PacketErrors.HasValue)
                    {
                        value = newSnapshot.Statistics.PacketErrors.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.PacketsReceived:
                    if (newSnapshot.Statistics.PacketsReceived.HasValue)
                    {
                        value = newSnapshot.Statistics.PacketsReceived.Value;
                    }
                    break;
                case ServerDetailsMetrics.PacketsReceivedPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.PacketsReceived.HasValue)
                    {
                        value = newSnapshot.Statistics.PacketsReceived.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.PacketsSent:
                    if (newSnapshot.Statistics.PacketsSent.HasValue)
                    {
                        value = newSnapshot.Statistics.PacketsSent.Value;
                    }
                    break;
                case ServerDetailsMetrics.PacketsSentPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.PacketsSent.HasValue)
                    {
                        value = newSnapshot.Statistics.PacketsSent.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.PageLifeExpectancy:
                    if (newSnapshot.Statistics.PageLifeExpectancySeconds.HasValue)
                    {
                        value = newSnapshot.Statistics.PageLifeExpectancySeconds.Value;
                    }
                    break;
                case ServerDetailsMetrics.PageLookups:
                    if (newSnapshot.Statistics.PageLookups.HasValue)
                    {
                        value = newSnapshot.Statistics.PageLookups.Value;
                    }
                    break;
                case ServerDetailsMetrics.PageLookupsPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.PageLookups.HasValue)
                    {
                        value = newSnapshot.Statistics.PageLookups.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.PageReads:
                    if (newSnapshot.Statistics.PageReads.HasValue)
                    {
                        value = newSnapshot.Statistics.PageReads.Value;
                    }
                    break;
                case ServerDetailsMetrics.PageReadsPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.PageReads.HasValue)
                    {
                        value = newSnapshot.Statistics.PageReads.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.PagesPerSecond:
                    if (newSnapshot.OSMetricsStatistics.PagesPersec.HasValue)
                    {
                        value = newSnapshot.OSMetricsStatistics.PagesPersec.Value;
                    }
                    break;
                case ServerDetailsMetrics.PageSplits:
                    if (newSnapshot.Statistics.PageSplits.HasValue)
                    {
                        value = newSnapshot.Statistics.PageSplits.Value;
                    }
                    break;
                case ServerDetailsMetrics.PageSplitsPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.PageSplits.HasValue)
                    {
                        value = newSnapshot.Statistics.PageSplits.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.PageWrites:
                    if (newSnapshot.Statistics.PageWrites.HasValue)
                    {
                        value = newSnapshot.Statistics.PageWrites.Value;
                    }
                    break;
                case ServerDetailsMetrics.PageWritesPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                        newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        newSnapshot.Statistics.PageWrites.HasValue)
                    {
                        value = newSnapshot.Statistics.PageWrites.Value /newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.PhysicalMemory:
                    if (newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                    {
                        value = newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.ProcedureCacheHitRatio:
                    if (newSnapshot.Statistics.CacheHitRatio.HasValue)
                    {
                        value = newSnapshot.Statistics.CacheHitRatio.Value;
                    }
                    break;
                case ServerDetailsMetrics.ProcedureCacheSize:
                    if (newSnapshot.ProcedureCacheSize.Megabytes.HasValue)
                    {
                        value = newSnapshot.ProcedureCacheSize.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.ProcessorQueueLength:
                    if (newSnapshot.OSMetricsStatistics.ProcessorQueueLength.HasValue)
                    {
                        value = newSnapshot.OSMetricsStatistics.ProcessorQueueLength.Value;
                    }
                    break;
                case ServerDetailsMetrics.ReadAheadPages:
                    if (newSnapshot.Statistics.ReadaheadPages.HasValue)
                    {
                        value = newSnapshot.Statistics.ReadaheadPages.Value;
                    }
                    break;
                case ServerDetailsMetrics.ReadAheadPagesPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                            newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                            newSnapshot.Statistics.ReadaheadPages.HasValue)
                    {
                        value = newSnapshot.Statistics.ReadaheadPages.Value /newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.ReadWriteErrors:
                    if (newSnapshot.Statistics.DiskErrors.HasValue)
                    {
                        value = newSnapshot.Statistics.DiskErrors.Value;
                    }
                    break;
                case ServerDetailsMetrics.ResponseTime:
                    value = newSnapshot.ResponseTime;
                    break;
                case ServerDetailsMetrics.SqlCompilations:
                    if (newSnapshot.Statistics.SqlCompilations.HasValue)
                    {
                        value = newSnapshot.Statistics.SqlCompilations.Value;
                    }
                    break;
                case ServerDetailsMetrics.SqlCompilationsPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                            newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                            newSnapshot.Statistics.SqlCompilations.HasValue)
                    {
                        value = newSnapshot.Statistics.SqlCompilations.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.SqlRecompilations:
                    if (newSnapshot.Statistics.SqlRecompilations.HasValue)
                    {
                        value = newSnapshot.Statistics.SqlRecompilations.Value;
                    }
                    break;
                case ServerDetailsMetrics.SqlRecompilationsPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                            newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                            newSnapshot.Statistics.SqlRecompilations.HasValue)
                    {
                        value = newSnapshot.Statistics.SqlRecompilations.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.SqlMemoryAllocated:
                    if (newSnapshot.TargetServerMemory.Megabytes.HasValue)
                    {
                        value = newSnapshot.TargetServerMemory.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.SqlMemoryUsed:
                    if (newSnapshot.TotalServerMemory.Megabytes.HasValue)
                    {
                        value = newSnapshot.TotalServerMemory.Megabytes.Value;
                    }
                    break;
                case ServerDetailsMetrics.SqlMemoryUsedPercent:
                    if (newSnapshot.TotalServerMemory.Kilobytes.HasValue &&
                    newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.HasValue)
                    {
                        double used = (double)(newSnapshot.TotalServerMemory.Kilobytes.Value /
                                               newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.Value*100m);
                        if (used > 100)
                            used = 100d;

                        value = used;
                    }
                    break;
                case ServerDetailsMetrics.SystemProcesses:
                    if (newSnapshot.SystemProcesses.CurrentSystemProcesses.HasValue)
                    {
                        value = newSnapshot.SystemProcesses.CurrentSystemProcesses.Value;
                    }
                    break;
                case ServerDetailsMetrics.IdleProcesses:
                    if (newSnapshot.SystemProcesses.CurrentProcesses.HasValue && newSnapshot.SystemProcesses.ActiveProcesses.HasValue)
                    {
                        int v = newSnapshot.SystemProcesses.CurrentProcesses.Value - newSnapshot.SystemProcesses.ActiveProcesses.Value;
                        if (v < 0) v = 0;
                        value = v;
                    }
                    break;
                case ServerDetailsMetrics.SystemProcessesConsumingCPU:
                    if (newSnapshot.SystemProcesses.SystemProcessesConsumingCpu.HasValue)
                    {
                        value = newSnapshot.SystemProcesses.SystemProcessesConsumingCpu.Value;
                    }
                    break;
                case ServerDetailsMetrics.ServerVersion:
                    break;
                case ServerDetailsMetrics.Batches:
                    if (newSnapshot.Statistics.BatchRequests.HasValue)
                    {
                        value = newSnapshot.Statistics.BatchRequests.Value;
                    }
                    break;
                case ServerDetailsMetrics.BatchesPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                   newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                   newSnapshot.Statistics.BatchRequests.HasValue)
                    {
                        value = newSnapshot.Statistics.BatchRequests.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.UserConnectionsPercent:
                    break;
                case ServerDetailsMetrics.UserProcesses:
                    if (newSnapshot.SystemProcesses.CurrentUserProcesses.HasValue)
                    {
                        value = newSnapshot.SystemProcesses.CurrentUserProcesses.Value;
                    }
                    break;
                case ServerDetailsMetrics.UserProcessesConsumingCPU:
                    if (newSnapshot.SystemProcesses.UserProcessesConsumingCpu.HasValue)
                    {
                        value = newSnapshot.SystemProcesses.UserProcessesConsumingCpu.Value;
                    }
                    break;
                case ServerDetailsMetrics.WMIStatus:
                    break;
                case ServerDetailsMetrics.WorkfilesCreated:
                    if (newSnapshot.Statistics.WorkfilesCreated.HasValue)
                    {
                        value = newSnapshot.Statistics.WorkfilesCreated.Value;
                    }
                    break;
                case ServerDetailsMetrics.WorkfilesCreatedPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.WorkfilesCreated.HasValue)
                    {
                        value = newSnapshot.Statistics.WorkfilesCreated.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.WorktablesCreated:
                    if (newSnapshot.Statistics.WorktablesCreated.HasValue)
                    {
                        value = newSnapshot.Statistics.WorktablesCreated.Value;
                    }
                    break;
                case ServerDetailsMetrics.WorktablesCreatedPerSecond:
                    if (newSnapshot.TimeDelta.HasValue &&
                    newSnapshot.TimeDelta.Value.TotalSeconds > 0 &&
                    newSnapshot.Statistics.WorktablesCreated.HasValue)
                    {
                        value = newSnapshot.Statistics.WorktablesCreated.Value / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;
                case ServerDetailsMetrics.VmCPUUsage:
                    if (newSnapshot.VMConfig != null && newSnapshot.VMConfig.PerfStats != null)
                    {
                        value = newSnapshot.VMConfig.PerfStats.CpuUsage;
                    }
                    break;
                case ServerDetailsMetrics.VmMemUsage:
                    if (newSnapshot.VMConfig != null && newSnapshot.VMConfig.PerfStats != null)
                    {
                        value = newSnapshot.VMConfig.PerfStats.MemUsage;
                    }
                    break;
                case ServerDetailsMetrics.IOWaits:
                    if (newSnapshot.WaitStatsSummary != null)
                    {
                        value = newSnapshot.WaitStatsSummary.IOWaits;
                    }
                    break;
                case ServerDetailsMetrics.LockWaits2:
                    if (newSnapshot.WaitStatsSummary != null)
                    {
                        value = newSnapshot.WaitStatsSummary.LockWaits;
                    }
                    break;
                case ServerDetailsMetrics.MemoryWaits:
                    if (newSnapshot.WaitStatsSummary != null)
                    {
                        value = newSnapshot.WaitStatsSummary.MemoryWaits;
                    }
                    break;
                case ServerDetailsMetrics.TransactionLogWaits:
                    if (newSnapshot.WaitStatsSummary != null)
                    {
                        value = newSnapshot.WaitStatsSummary.TransactionLogWaits;
                    }
                    break;
                case ServerDetailsMetrics.OtherWaits:
                    if (newSnapshot.WaitStatsSummary != null)
                    {
                        value = newSnapshot.WaitStatsSummary.OtherWaits;
                    }
                    break;
                case ServerDetailsMetrics.SignalWaits:
                    if (newSnapshot.WaitStatsSummary != null)
                    {
                        value = newSnapshot.WaitStatsSummary.SignalWaits;
                    }
                    break;

                case ServerDetailsMetrics.BufferCacheFreePages:
                    if (newSnapshot.MemoryStatistics != null && newSnapshot.MemoryStatistics.FreePages != null)
                    {
                        value = newSnapshot.MemoryStatistics.FreePages.Megabytes;
                    }
                    break;
                case ServerDetailsMetrics.UsedDataMemory:
                    if (newSnapshot.MemoryStatistics != null && newSnapshot.MemoryStatistics.CommittedPages != null)
                    {
                        value = newSnapshot.MemoryStatistics.CommittedPages.Megabytes;
                    }
                    break;
                case ServerDetailsMetrics.LockOptimizerConnectionSortHashIndexMemory:

                    bool otherSet = false;
                    decimal other = 0;

                    if (newSnapshot.MemoryStatistics != null)
                    {
                        Memory memStats = newSnapshot.MemoryStatistics;

                        // Desktop ServerSummaryHistoryData.cs line 1726 doesn't check for nulls on these, 
                        // so I assume they never are?
                        if (memStats.OptimizerMemory.Megabytes.HasValue)
                        {
                            other   += memStats.OptimizerMemory.Megabytes.Value;
                            otherSet = true;
                        }

                        if (memStats.LockMemory.Megabytes.HasValue)
                        {
                            other   += memStats.LockMemory.Megabytes.Value;
                            otherSet = true;
                        }

                        if (memStats.ConnectionMemory.Megabytes.HasValue)
                        {
                            other   += memStats.ConnectionMemory.Megabytes.Value;
                            otherSet = true;
                        }

                        if (memStats.GrantedWorkspaceMemory.Megabytes.HasValue)
                        {
                            other   += memStats.GrantedWorkspaceMemory.Megabytes.Value;
                            otherSet = true;
                        }

                        if (otherSet)
                            value = other;
                    }

                    break;
                case ServerDetailsMetrics.ProcedureCachePages:
                    if (newSnapshot.MemoryStatistics != null && newSnapshot.MemoryStatistics.CachePages != null)
                    {
                        value = newSnapshot.MemoryStatistics.CachePages.Megabytes;
                    }
                    break;

                case ServerDetailsMetrics.WaitTimeDatabase:
                    if (newSnapshot.LockCounters != null && newSnapshot.LockCounters.DatabaseCounters != null)
                    {
                        if (newSnapshot.LockCounters.DatabaseCounters.WaitTime.HasValue)
                            value = newSnapshot.LockCounters.DatabaseCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    break;
                case ServerDetailsMetrics.WaitTimeExtent:
                    if (newSnapshot.LockCounters != null && newSnapshot.LockCounters.ExtentCounters != null)
                    {
                        if (newSnapshot.LockCounters.ExtentCounters.WaitTime.HasValue)
                            value = newSnapshot.LockCounters.ExtentCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    break;
                case ServerDetailsMetrics.WaitTimeKey:
                    if (newSnapshot.LockCounters != null && newSnapshot.LockCounters.KeyCounters != null)
                    {
                        if (newSnapshot.LockCounters.KeyCounters.WaitTime.HasValue)
                            value = newSnapshot.LockCounters.KeyCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    break;
                case ServerDetailsMetrics.WaitTimePage:
                    if (newSnapshot.LockCounters != null && newSnapshot.LockCounters.PageCounters != null)
                    {
                        if (newSnapshot.LockCounters.PageCounters.WaitTime.HasValue)
                            value = newSnapshot.LockCounters.PageCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    break;
                case ServerDetailsMetrics.WaitTimeRID:
                    if (newSnapshot.LockCounters != null && newSnapshot.LockCounters.RidCounters != null)
                    {
                        if (newSnapshot.LockCounters.RidCounters.WaitTime.HasValue)
                            value = newSnapshot.LockCounters.RidCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    break;
                case ServerDetailsMetrics.WaitTimeTable:
                    if (newSnapshot.LockCounters != null && newSnapshot.LockCounters.TableCounters != null)
                    {
                        if (newSnapshot.LockCounters.TableCounters.WaitTime.HasValue)
                            value = newSnapshot.LockCounters.TableCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    break;

                case ServerDetailsMetrics.ActiveSessions:
                    if (newSnapshot.SystemProcesses != null && newSnapshot.SystemProcesses.ActiveProcesses.HasValue)
                    {
                        value = newSnapshot.SystemProcesses.ActiveProcesses.Value;
                    }
                    break;

                case ServerDetailsMetrics.CallRatesTransactions:
                    if (newSnapshot.Statistics != null && newSnapshot.Statistics.Transactions.HasValue &&
                        newSnapshot.TimeDelta.HasValue && newSnapshot.TimeDelta.Value.TotalSeconds > 0)
                    {
                        value = newSnapshot.Statistics.Transactions / newSnapshot.TimeDelta.Value.TotalSeconds;
                    }
                    break;

                default:
                    break;
            }

            if (value != null)
            {
                Type vtype = value.GetType();
                if (vtype.IsGenericType)
                {
                    if (vtype.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        PropertyInfo prop_hasvalue = vtype.GetProperty("HasValue");
                        PropertyInfo prop_value = vtype.GetProperty("Value");
                        value = ((bool)prop_hasvalue.GetValue(value, null))
                                ? prop_value.GetValue(value, null)
                                : null;
                    }
                }
            }
            if (value != null)
            {
                Type vtype = value.GetType();
                if (vtype.IsEnum)
                {
                    value = Convert.ChangeType(value, Enum.GetUnderlyingType(vtype));
                }
            }

            MonitoredState state = MonitoredState.None;
            if (threshold != null && threshold.IsEnabled)
                state = GetSeverity(threshold, value as IComparable);

            if (value == null)
            {
                value = DBNull.Value;
            }

            resultRow["Value"] = value;
            resultRow["State"] = (byte)state;
        }

        private static void SetDatabaseMetricGraphRows(ServerOverview newSnapshot, DataTable table, Dictionary<Pair<int, string>, MetricThresholdEntry> thresholds)
        {
            if (newSnapshot.DbStatistics == null)
                return;
            
            if (newSnapshot.TimeDelta == null || newSnapshot.TimeDelta.Value.TotalSeconds <= 0)
                return;

            double totalSeconds = newSnapshot.TimeDelta.Value.TotalSeconds;
            Dictionary<string, DatabaseStatistics> stats = newSnapshot.DbStatistics;

            foreach (DatabaseStatistics dbstats in stats.Values)
            {
                if (dbstats.Transactions.HasValue)
                {
                    object value = dbstats.Transactions / totalSeconds;

                    AddDatabaseStatsRow(table, newSnapshot.ServerTimeUTC, ServerDetailsMetrics.TransactionsPerSecond, dbstats.Name, value, MonitoredState.OK, DBNull.Value);
                }

                if (dbstats.LogFlushes.HasValue)
                {
                    object value = dbstats.LogFlushes / totalSeconds;

                    AddDatabaseStatsRow(table, newSnapshot.ServerTimeUTC, ServerDetailsMetrics.LogFlushesPerSecond2, dbstats.Name, value, MonitoredState.OK, DBNull.Value);
                }

                if (dbstats.Reads.HasValue)
                {
                    object value = dbstats.Reads / (decimal)totalSeconds;

                    AddDatabaseStatsRow(table, newSnapshot.ServerTimeUTC, ServerDetailsMetrics.NumberReadsPerSecond, dbstats.Name, value, MonitoredState.OK, DBNull.Value);
                }

                if (dbstats.Writes.HasValue)
                {
                    object value = dbstats.Writes / (decimal)totalSeconds;

                    AddDatabaseStatsRow(table, newSnapshot.ServerTimeUTC, ServerDetailsMetrics.NumberWritesPerSecond, dbstats.Name, value, MonitoredState.OK, DBNull.Value);
                }

                if (dbstats.IoStallMs.HasValue)
                {
                    object value = dbstats.IoStallMs / (decimal)totalSeconds;

                    AddDatabaseStatsRow(table, newSnapshot.ServerTimeUTC, ServerDetailsMetrics.IoStallMSPerSecond, dbstats.Name, value, MonitoredState.OK, DBNull.Value);
                }
            }
        }

        private static void AddDatabaseStatsRow(DataTable table, DateTime? timestamp, ServerDetailsMetrics metricId, object name, object value, MonitoredState state, object alertMetric)
        {
            #region Some stuff that was in SetMetricGraphRow, not sure why it was necessary, but copying here
            if (value != null)
            {
                Type vtype = value.GetType();
                if (vtype.IsGenericType)
                {
                    if (vtype.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        PropertyInfo prop_hasvalue = vtype.GetProperty("HasValue");
                        PropertyInfo prop_value = vtype.GetProperty("Value");
                        value = ((bool)prop_hasvalue.GetValue(value, null))
                                ? prop_value.GetValue(value, null)
                                : null;
                    }
                }
            }
            #endregion

            DataRow resultRow = table.NewRow();

            resultRow["CollectionDateTime"] = timestamp;
            resultRow["Metric"]      = (int)metricId;
            resultRow["Instance"]    = name ?? DBNull.Value;
            resultRow["Value"]       = value;
            resultRow["State"]       = (byte)state;
            resultRow["AlertMetric"] = alertMetric;

            table.Rows.Add(resultRow);
        }

        private static void AddPropertyRow(DataTable props, string name, object value)
        {
            DataRow row = props.NewRow();
            row[0] = name;
            row[1] = value != null ? (object)value.ToString() : DBNull.Value;
            props.Rows.Add(row);
        }

        private static void CreateSummary(MonitoredSqlServerState state, ServerOverview newSnapshot, DataTable areas, DataTable properties, DataTable alerts)
        {
            const int nbrAreas = ((int)AlertArea.Custom) + 1;
            int[] areaWarning  = new int[nbrAreas];
            int[] areaCritical = new int[nbrAreas];
            int[] areaInformational = new int[nbrAreas];

            if (newSnapshot != null)
            {
                if (newSnapshot.ProductVersion != null)
                    AddPropertyRow(properties, "Version", newSnapshot.ProductVersion + " (" +
                                                          newSnapshot.ProductVersion.Version + ")");
                else
                    AddPropertyRow(properties, "Version", "N/A");

                AddPropertyRow(properties, "Edition",
                               !String.IsNullOrEmpty(newSnapshot.SqlServerEdition)
                                   ? newSnapshot.SqlServerEdition
                                   : "N/A");

                AddPropertyRow(properties, "Running Since",
                               newSnapshot.RunningSince.HasValue
                                   ? newSnapshot.RunningSince.Value.ToLocalTime().ToString("G")
                                   : "N/A");

                string propertiesIsClusteredLabel = newSnapshot.IsClustered.HasValue
                                                        ? newSnapshot.IsClustered.Value ? "Yes" : "No"
                                                        : "N/A";

                if (newSnapshot.IsClustered.HasValue && newSnapshot.IsClustered.Value)
                {
                    propertiesIsClusteredLabel += " / Active Node: ";
                    propertiesIsClusteredLabel += newSnapshot.ClusterNodeName ?? "N/A";
                }
                AddPropertyRow(properties, "Clustered", propertiesIsClusteredLabel);

                if (newSnapshot.ProcessorCount.HasValue && newSnapshot.ProcessorsUsed.HasValue)
                {
                    AddPropertyRow(properties, "Processors",
                                   string.Format("{1} of {0} used", newSnapshot.ProcessorCount,
                                                 newSnapshot.ProcessorsUsed));
                }
                else
                {
                    AddPropertyRow(properties, "Processors", "N/A");
                }

                AddPropertyRow(properties, "Host", newSnapshot.ServerHostName ?? "N/A");
                AddPropertyRow(properties, "Host OS", newSnapshot.WindowsVersion ?? "N/A");

                if (newSnapshot.VMConfig != null)
                {
                    VMwareVirtualMachine vmconfig = newSnapshot.VMConfig;

                    if(vmconfig.ESXHost != null)
                        AddPropertyRow(properties, "VM ESX Host", vmconfig.ESXHost.Name ?? "N/A");

                    AddPropertyRow(properties, "VM UUID", vmconfig.InstanceUUID ?? "N/A");
                    AddPropertyRow(properties, "VM Name", vmconfig.Name ?? "N/A");
                    AddPropertyRow(properties, "VM BootTime", GetVmProperty(vmconfig.BootTime));
                    AddPropertyRow(properties, "VM Num CPUs", GetVmProperty(vmconfig.NumCPUs));
                    AddPropertyRow(properties, "VM CPU Limit", GetVmProperty(vmconfig.CPULimit));
                    AddPropertyRow(properties, "VM CPU Reserve", GetVmProperty(vmconfig.CPUReserve));

                    if(vmconfig.MemSize != null && vmconfig.MemSize.Megabytes.HasValue)
                        AddPropertyRow(properties, "VM MemSize", GetVmProperty(vmconfig.MemSize.Megabytes.Value));

                    if(vmconfig.MemLimit != null && vmconfig.MemLimit.Megabytes.HasValue)
                        AddPropertyRow(properties, "VM MemLimit", GetVmProperty(vmconfig.MemLimit.Megabytes.Value));

                    if(vmconfig.MemReserve != null && vmconfig.MemReserve.Megabytes.HasValue)
                        AddPropertyRow(properties, "VM MemReserve", GetVmProperty(vmconfig.MemReserve.Megabytes.Value));
                }

                string denomination;
                decimal? totalPhysicalMemory =
                    newSnapshot.OSMetricsStatistics.TotalPhysicalMemory.BestDenomination(out denomination);
                AddPropertyRow(properties, "Host Memory",
                               totalPhysicalMemory.HasValue
                                   ? string.Format("{0:F2} {1}", totalPhysicalMemory.Value, denomination)
                                   : "N/A");

                AddPropertyRow(properties, "Databases", newSnapshot.DatabaseSummary.DatabaseCount.HasValue
                                                            ? newSnapshot.DatabaseSummary.DatabaseCount.Value.ToString()
                                                            : "N/A");

                decimal? dataSize = newSnapshot.DatabaseSummary.DataFileSpaceUsed.BestDenomination(out denomination);
                AddPropertyRow(properties, "Data Size",
                               dataSize.HasValue
                                   ? string.Format("{0:F2} {1}", dataSize.Value, denomination)
                                   : "N/A");

                decimal? logSize = newSnapshot.DatabaseSummary.LogFileSpaceUsed.BestDenomination(out denomination);
                AddPropertyRow(properties, "Log Size",
                               logSize.HasValue
                                   ? string.Format("{0:F2} {1}", logSize.Value, denomination)
                                   : "N/A");

                OSMetricsStatus osMetricStatus = newSnapshot.OSMetricsStatistics.OsMetricsStatus;
                string status_string = "Unknown";
                switch (osMetricStatus)
                {
                    case OSMetricsStatus.Available:
                        status_string = "Available";
                        break;
                    case OSMetricsStatus.Disabled:
                        status_string = "Disabled";
                        break;
                    case OSMetricsStatus.OLEAutomationUnavailable:
                        status_string = "Ole Automation not enabled";
                        break;
                    case OSMetricsStatus.UnavailableDueToLightweightPooling:
                        status_string = "Lightweight pooling is enabled";
                        break;
                    case OSMetricsStatus.WMIServiceUnreachable:
                        status_string = "WMI not available";
                        break;
                }

                AddPropertyRow(properties, "OS Metrics Status", status_string);

                // update alert counts
                DataRow alertRow = alerts.NewRow();
                XmlDocument statusDoc = Management.ScheduledCollection.GetCachedStatusDocument(state.WrappedServer.Id);
                XmlElement element = statusDoc.DocumentElement.SelectSingleNode("/Servers/Server") as XmlElement;
                if (element == null)
                {   // no refresh available yet
                    alertRow[0] = alertRow[1] = alertRow[2] = 0;
                    alertRow[3] = DateTime.UtcNow;
                    alerts.Rows.Add(alertRow);

                    AddAreaRow(areas, AlertArea.Sessions, (int) MonitoredState.OK, 0, 0);
                    AddAreaRow(areas, AlertArea.Resources, (int) MonitoredState.OK, 0, 0);
                    AddAreaRow(areas, AlertArea.Databases, (int) MonitoredState.OK, 0, 0);
                    AddAreaRow(areas, AlertArea.Services, (int) MonitoredState.OK, 0, 0);
                }
                else
                {
                    alertRow[0] = GetIntAttribute(element.Attributes["ActiveWarningAlerts"]);
                    alertRow[1] = GetIntAttribute(element.Attributes["ActiveCriticalAlerts"]);
                    alertRow[2] = GetIntAttribute(element.Attributes["ActiveInfoAlerts"]);
                    alertRow[3] = GetDateTimeAttribute(element.Attributes["LastAlertRefreshTime"]);
                    alerts.Rows.Add(alertRow);

                    AlertArea area;
                    // roll up area counts for each area 
                    foreach (XmlElement catElement in element.SelectNodes("./Category"))
                    {
                        string catname = catElement.GetAttribute("Name");
                        if (catname == null) continue;
                        try
                        {
                            area = (AlertArea) Enum.Parse(typeof (AlertArea), catname, true);
                        }
                        catch (Exception)
                        {
                            area = AlertArea.Custom;
                        }
                        foreach (XmlElement stateElement in catElement.SelectNodes("./State"))
                        {
                            int status = GetIntAttribute(stateElement.GetAttributeNode("Severity"));
                            switch (status)
                            {
                                case (int) MonitoredState.Critical:
                                    areaCritical[(int) area]++;
                                    break;
                                case (int) MonitoredState.Warning:
                                    areaWarning[(int) area]++;
                                    break;
                                case (int) MonitoredState.Informational:
                                    areaInformational[(int) area]++;
                                    break;
                            }
                        }
                    }
                    foreach (AlertArea alertarea in Enum.GetValues(typeof (AlertArea)))
                    {
                        int nwarn = areaWarning[(int) alertarea];
                        int ncrit = areaCritical[(int) alertarea];
                        int ninfo = areaInformational[(int) alertarea];

                        int stat = 0;
                        if (ncrit > 0)
                            stat = 8;
                        else if (nwarn > 0)
                            stat = 4;
                        else if (ninfo > 0)
                            stat = 2;

                        switch (alertarea)
                        {
                                // don't show these areas if state = OK
                            case AlertArea.Queries:
                            case AlertArea.Logs:
                            case AlertArea.Operational:
                            case AlertArea.Custom:
                                if (stat == 0)
                                    continue;
                                break;
                        }
                        AddAreaRow(areas, alertarea, stat, nwarn, ncrit);
                    }
                }
            }
        }

        private static string GetVmProperty(int value)
        {
            return value < 0 ? "N/A" : value.ToString();
        }

        private static string GetVmProperty(long value) 
        {
            return value < 0 ? "N/A" : value.ToString();
        }

        private static string GetVmProperty(Decimal value)
        {
            return value < 0 ? "N/A" : value.ToString();
        }

        private static string GetVmProperty(DateTime value)
        {
            return value == DateTime.MinValue ? "N/A" : value.ToLocalTime().ToString("G");
        }

        private static void AddAreaRow(DataTable areas, AlertArea area, int monitoredState, int nwarning, int ncritical)
        {
            DataRow row = areas.NewRow();
            row[0] = ApplicationHelper.GetEnumDescription(area); 
            row[1] = (int) monitoredState;
            row[2] = nwarning;
            row[3] = ncritical;
            areas.Rows.Add(row);
        }

        private static int GetIntAttribute(XmlAttribute att)
        {
            if (att != null)
            {
                int i;
                string s = att.Value;
                if (!string.IsNullOrEmpty(s) && int.TryParse(s, out i))
                    return i;
            }
            return 0;
        }

        private static object GetDateTimeAttribute(XmlAttribute attribute)
        {
            if (attribute != null)
            {
                string timeString = attribute.Value;
                if (!String.IsNullOrEmpty(timeString))
                {
                    try
                    {
                        return DateTime.Parse(timeString);
                    }
                    catch
                    {
                    }
                }
            }
            return DBNull.Value;
        }

        private static void UpdateStatusCount(int metric, MonitoredSqlServerState state, Triple<Metric?, string, Type> metadata, int[] areaWarning, int[] areaCritical, int[] areaInformational, IComparable value)
        {
            if (!metadata.First.HasValue) return;
            MetricThresholdEntry threshold = state.GetMetricThresholdEntry((int)metadata.First.Value);
            if (threshold == null || !threshold.IsEnabled) return;
            int category = (int)MetricsCategories[metric];

            switch (GetSeverity(threshold, value))
            {
                case MonitoredState.Critical:
                    areaCritical[category]++;
                    break;
                case MonitoredState.Warning:
                    areaWarning[category]++;
                    break;
                case MonitoredState.Informational:
                    areaInformational[category]++;
                    break;
            }
        }

        private static MonitoredState GetSeverity(MetricThresholdEntry threshold, IComparable value)
        {
            if (value == null)
                return MonitoredState.None;

            Type valueType = value.GetType();
            
            // see if this is an enumerated value
            if (valueType.IsEnum)
            {
                // get the underlying type and convert value to that type
                object evalue = Convert.ChangeType(value, Enum.GetUnderlyingType(valueType));
                if (Enum.IsDefined(valueType, evalue))
                {
                    value = (IComparable)Enum.ToObject(valueType, evalue);
                }
            }
            return SnapshotState.GetState(value, threshold);
        }
    }
}
