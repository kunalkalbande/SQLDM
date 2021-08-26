//------------------------------------------------------------------------------
// <copyright file="ServerOverview.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.VMware;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using Idera.SQLdm.Common.Objects;
    using Cloud;
    /// <summary>
    /// Represents an on-demand server overview
    /// </summary>
    [Serializable]
    public sealed class ServerOverview : Snapshot
    {
        #region fields

        private ServiceState? agentServiceStatus = ServiceState.UnableToMonitor;
        private ServiceState? dtcServiceStatus = ServiceState.UnableToMonitor;
        private ServiceState? fullTextServiceStatus = ServiceState.UnableToMonitor;
        private ServiceState? sqlServiceStatus = ServiceState.UnableToMonitor;
        //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the status fields for new SQL services
        private ServiceState? sqlBrowserServiceStatus = ServiceState.UnableToMonitor;
        private ServiceState? sqlActiveDirectoryHelperServiceStatus = ServiceState.UnableToMonitor;
        //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the status fields for new SQL services

        private string language = null;
        private bool? loginHasAdministratorRights = null;
        private Int64? maxConnections = null;
        private FileSize physicalMemory = new FileSize();
        private int? processorCount = null;
        private int? processorsUsed = null;
        private string processorType = null;
        private string serverHostName = null;
        private string realServerName = null;
        private string windowsVersion = null;
        private string sqlServerEdition = null;
        private bool maintenanceModeEnabled = false;   //SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -add maintenance mode status when a specific instance is viewed

        private ServerStatistics statistics = null;

        private FileSize targetServerMemory = new FileSize();
        private FileSize totalServerMemory = new FileSize();
        //10.0 SQLdm Srishti Purohit -- baseline mean comparision change
        private double? sqlMemUsageMeanBaseline;
        private double? sqlMemUsagesPercBaseline;

        private bool? isClustered = null;
        private string clusterNodeName = null;

        private FileSize procedureCacheSize = new FileSize();
        private double? procedureCachePercentageUsed = null;

        private OSMetrics osMetricsStatistics = null;

        private ServerLoginConfiguration loginConfiguration;

        private long responseTime;
        //10.0 SQLdm Srishti Purohit -- baseline mean comparision change
        private double? responseTimeMinutesBaseline;
        private double? responseTimePercentage;

        private DateTime? runningSince = null;
        private ServerSystemProcesses systemProcesses = new ServerSystemProcesses();

        private ServerDatabaseSummary databaseSummary = new ServerDatabaseSummary();

        private decimal? totalLocks = null;

        private TimeSpan? timeDelta = new TimeSpan(0);

        private LockStatistics lockCounters = new LockStatistics();

        private Memory memoryStatistics = new Memory();
        private Dictionary<string, DiskDrive> diskDrives = new Dictionary<string, DiskDrive>();
        private bool isFileSystemObjectDataAvailable = true;

        private TempdbSummaryStatistics tempdbStatistics = new TempdbSummaryStatistics();

        private Dictionary<String, DatabaseStatistics> dbStatistics = new Dictionary<string, DatabaseStatistics>();

        private Dictionary<string, FileActivityFile> fileActivity = new Dictionary<string, FileActivityFile>();

        private VMwareVirtualMachine vmConfig = null;

        private WaitStatisticsSummary waitStatSummary = null;

        private bool multipleDefaultInstancesPresent = false;

        private List<Dictionary<String, String>> listOfDatabase = null;

        //6.2.4 Disk Section
        private double? dataIOUsage = null;
        private double? logIOUsage = null;
        private double? dataIORate = null;
        private double? logIORate = null;
        private List<Dictionary<String, String>> azureIOMetrics = null;
        private decimal managedInstanceStorageLimit;
        private double? pagesReadPersec;
        private double? pagesWritePersec;
        #endregion
        //5.4.1 (release 11)
        private double? readThroughput = 0;
        private double? writeThroughput = 0;
        private double? swapUsage = 0;
        private double? readLatency = 0;
        private double? writeLatency = 0;
        private double? cPUCreditBalance = 0;
        private double? cPUCreditUsage = 0;
        private double? diskQueueDepth = 0;
        #region constructors

        public ServerOverview(SqlConnectionInfo info)
            : this(info.InstanceName)
        {
        }

        public ServerOverview(string serverName)
            : base(serverName)
        {
            Statistics = new ServerStatistics();
            OSMetricsStatistics = new OSMetrics();
        }

        public ServerOverview(string serverName, DataRow dataRow, IEnumerable<string> diskDrives, bool loadLocks, DataRow[] dbDataRows)
            : this(serverName, dataRow, diskDrives, dbDataRows)
        {
            if (loadLocks)
            {
                byte[] lockBytes = dataRow["LockStatistics"] as byte[];
                if (lockBytes != null && lockBytes.Length > 0)
                {
                    lockCounters = Serialized<Object>.DeserializeCompressed<LockStatistics>(lockBytes);
                }
            }
        }

        public ServerOverview(string serverName, DataRow dataRow, IEnumerable<string> diskDrives, DataRow[] dbDataRows)
            : this(serverName)
        {
            Decimal? avgCpuPercent = null, avgDataIoPercent = null, avgLogWritePercent = null, dtuLimit=null;

            foreach (string diskDrive in diskDrives)
            {
                this.diskDrives.Add(diskDrive, null);
            }

            if (dataRow == null)
            {
                throw new ArgumentNullException("dataRow");
            }

            object value = dataRow["CollectionDateTime"];
            if (value != DBNull.Value)
            {
                TimeStamp = (DateTime)value;
            }

            value = dataRow["TimeDeltaInSeconds"];
            if (value != DBNull.Value)
            {
                TimeDelta = TimeSpan.FromSeconds((double)value);
            }

            value = dataRow["AgentServiceStatus"];
            if (value != DBNull.Value)
            {
                AgentServiceStatus = (ServiceState)((int)value);
            }

            //6.2.4
            value = dataRow["DataIOUsage"];
            if(value != DBNull.Value)
            {
                DataIOUsage = (double)value;
            }

            //6.2.4
            value = dataRow["LogIOUsage"];
            if (value != DBNull.Value)
            {
                LogIOUsage = (double)value;
            }
            
            //6.2.4
            value = dataRow["DataIORate"];
            if (value != DBNull.Value)
            {
                DataIORate = (double)value;
            }
            
            //6.2.4
            value = dataRow["LogIORate"];
            if (value != DBNull.Value)
            {
                LogIORate = (double)value;
            }

            //5.4.1 (release 11)

            value = dataRow["ReadThroughput"];
            if (value != DBNull.Value)
            {
                ReadThroughput = (double)value;
            }
            value = dataRow["WriteThroughput"];
            if (value != DBNull.Value)
            {
                WriteThroughput = (double)value;
            }
            value = dataRow["SwapUsage"];
            if (value != DBNull.Value)
            {
                SwapUsage = (double)value;
            }
            value = dataRow["ReadLatency"];
            if (value != DBNull.Value)
            {
                ReadLatency = (double)value;
            }
            value = dataRow["WriteLatency"];
            if (value != DBNull.Value)
            {
                WriteLatency = (double)value;
            }
            value = dataRow["CPUCreditBalance"];
            if (value != DBNull.Value)
            {
                CPUCreditBalance = (double)value;
            }
            value = dataRow["CPUCreditUsage"];
            if (value != DBNull.Value)
            {
                CPUCreditUsage = (double)value;
            }
            value = dataRow["DiskQueueDepth"];
            if (value != DBNull.Value)
            {
                DiskQueueDepth = (double)value;
            }

            value = dataRow["SqlServerServiceStatus"];
            if (value != DBNull.Value)
            {
                SqlServiceStatus = (ServiceState)((int)value);
            }

            value = dataRow["DTCServiceStatus"];
            if (value != DBNull.Value)
            {
                DtcServiceStatus = (ServiceState)((int)value);
            }

            value = dataRow["FullTextSearchStatus"];
            if (value != DBNull.Value)
            {
                FullTextServiceStatus = (ServiceState)((int)value);
            }

            //START : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --read current status of the services
            if (dataRow.Table.Columns.Contains("SQLBrowserServiceStatus"))
            {
                value = dataRow["SQLBrowserServiceStatus"];
                if (value != DBNull.Value)
                {
                    SQLBrowserServiceStatus = (ServiceState)((int)value);
                }
            }

            if (dataRow.Table.Columns.Contains("SQLActiveDirectoryHelperServiceStatus"))
            {
                value = dataRow["SQLActiveDirectoryHelperServiceStatus"];
                if (value != DBNull.Value)
                {
                    SQLActiveDirectoryHelperServiceStatus = (ServiceState)((int)value);
                }
            }
            //END : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --read current status of the services

            value = dataRow["BufferCacheHitRatioPercentage"];
            if (value != DBNull.Value)
            {
                Statistics.BufferCacheHitRatio = (double)value;
            }

            value = dataRow["CheckpointWrites"];
            if (value != DBNull.Value)
            {
                Statistics.CheckpointPages = (long)value;
            }

            value = dataRow["ClientComputers"];
            if (value != DBNull.Value)
            {
                SystemProcesses.ComputersHoldingProcesses = (int)value;
            }

            value = dataRow["CPUActivityPercentage"];
            if (value != DBNull.Value)
            {
                Statistics.CpuPercentage = (double)value;
            }

            value = dataRow["CPUTimeDelta"];
            if (value != DBNull.Value)
            {
                Statistics.CpuBusyDelta = new CpuTime((long)value);
            }

            value = dataRow["CPUTimeRaw"];
            if (value != DBNull.Value)
            {
                Statistics.CpuBusyRaw = (long)value;
            }

            value = dataRow["FullScans"];
            if (value != DBNull.Value)
            {
                Statistics.FullScans = (long)value;
            }

            value = dataRow["IdleTimeDelta"];
            if (value != DBNull.Value)
            {
                Statistics.IdleTimeDelta = new CpuTime((long)value);
            }

            value = dataRow["IdleTimePercentage"];
            if (value != DBNull.Value)
            {
                Statistics.IdlePercentage = (double)value;
            }

            value = dataRow["IdleTimeRaw"];
            if (value != DBNull.Value)
            {
                Statistics.IdleTimeRaw = (long)value;
            }

            value = dataRow["IOActivityPercentage"];
            if (value != DBNull.Value)
            {
                Statistics.IoPercentage = (double)value;
            }

            value = dataRow["IOTimeDelta"];
            if (value != DBNull.Value)
            {
                Statistics.IoTimeDelta = new CpuTime((long)value);
            }

            value = dataRow["IOTimeRaw"];
            if (value != DBNull.Value)
            {
                Statistics.IoTimeRaw = (long)value;
            }

            value = dataRow["LazyWriterWrites"];
            if (value != DBNull.Value)
            {
                Statistics.LazyWrites = (long)value;
            }

            value = dataRow["LockWaits"];
            if (value != DBNull.Value)
            {
                Statistics.LockWaits = (long)value;
            }

            value = dataRow["Logins"];
            if (value != DBNull.Value)
            {
                Statistics.TotalConnections = (long)value;
            }

            value = dataRow["LogFlushes"];
            if (value != DBNull.Value)
            {
                Statistics.LogFlushes = (long)value;
            }

            value = dataRow["SqlMemoryAllocatedInKilobytes"];
            if (value != DBNull.Value)
            {
                TargetServerMemory.Kilobytes = (long)value;
            }

            value = dataRow["SqlMemoryUsedInKilobytes"];
            if (value != DBNull.Value)
            {
                TotalServerMemory.Kilobytes = (long)value;
            }

            value = dataRow["ManagedInstanceStorageLimit"];
            if (value != DBNull.Value)
            {
                ManagedInstanceStorageLimit = (decimal)value;
            }

            
            value = dataRow["PacketErrors"];
            if (value != DBNull.Value)
            {
                Statistics.PacketErrors = (long)value;
            }

            value = dataRow["PacketsReceived"];
            if (value != DBNull.Value)
            {
                Statistics.PacketsReceived = (long)value;
            }

            value = dataRow["PacketsSent"];
            if (value != DBNull.Value)
            {
                Statistics.PacketsSent = (long)value;
            }

            value = dataRow["PageErrors"];
            if (value != DBNull.Value)
            {
                Statistics.DiskErrors = (long)value;
            }

            value = dataRow["PageLifeExpectancy"];
            if (value != DBNull.Value)
            {
                Statistics.PageLifeExpectancy = TimeSpan.FromSeconds((long)value);
            }

            value = dataRow["PageLookups"];
            if (value != DBNull.Value)
            {
                Statistics.PageLookups = (long)value;
            }

            value = dataRow["PageReads"];
            if (value != DBNull.Value)
            {
                Statistics.PageReads = (long)value;
            }

            value = dataRow["PageSplits"];
            if (value != DBNull.Value)
            {
                Statistics.PageSplits = (long)value;
            }

            value = dataRow["PageWrites"];
            if (value != DBNull.Value)
            {
                Statistics.PageWrites = (long)value;
            }

            value = dataRow["ProcedureCacheHitRatioPercentage"];
            if (value != DBNull.Value)
            {
                Statistics.CacheHitRatio = (double)value;
            }

            value = dataRow["ProcedureCacheSizeInKilobytes"];
            if (value != DBNull.Value)
            {
                ProcedureCacheSize.Kilobytes = (long)value;
            }

            value = dataRow["ProcedureCacheSizePercent"];
            if (value != DBNull.Value)
            {
                ProcedureCachePercentageUsed = (double)value;
            }

            value = dataRow["ReadAheadPages"];
            if (value != DBNull.Value)
            {
                Statistics.ReadaheadPages = (long)value;
            }

            value = dataRow["ResponseTimeInMilliseconds"];
            if (value != DBNull.Value)
            {
                ResponseTime = (int)value;
            }

            value = dataRow["SqlServerEdition"];
            if (value != DBNull.Value)
            {
                SqlServerEdition = (string)value;
            }

            //SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -add maintenance mode status when a specific instance is viewed
            if (dataRow.Table.Columns.Contains("MaintenanceModeEnabled"))
            {
                value = dataRow["MaintenanceModeEnabled"];
                if (value != DBNull.Value)
                {
                    maintenanceModeEnabled = (bool)value;
                }
            }

            value = dataRow["ServerVersion"];
            if (value != DBNull.Value)
            {                
                if(value.ToString() != "?")                    
                     ProductVersion = new ServerVersion((string)value);
            }

            value = dataRow["SqlCompilations"];
            if (value != DBNull.Value)
            {
                Statistics.SqlCompilations = (long)value;
            }

            value = dataRow["SqlRecompilations"];
            if (value != DBNull.Value)
            {
                Statistics.SqlRecompilations = (long)value;
            }

            value = dataRow["TableLockEscalations"];
            if (value != DBNull.Value)
            {
                Statistics.TableLockEscalations = (long)value;
            }

            value = dataRow["Batches"];
            if (value != DBNull.Value)
            {
                Statistics.BatchRequests = (long)value;
            }

            value = dataRow["Transactions"];
            if (value != DBNull.Value)
            {
                Statistics.Transactions = (long)value;
            }

            value = dataRow["UserProcesses"];
            if (value != DBNull.Value)
            {
                SystemProcesses.CurrentUserProcesses = (int)value;
            }

            value = dataRow["WorkFilesCreated"];
            if (value != DBNull.Value)
            {
                Statistics.WorkfilesCreated = (long)value;
            }

            value = dataRow["WorkTablesCreated"];
            if (value != DBNull.Value)
            {
                Statistics.WorktablesCreated = (long)value;
            }

            value = dataRow["ReplicationLatencyInSeconds"];
            if (value != DBNull.Value)
            {
                Statistics.ReplicationLatencyInSeconds = (double)value;
            }

            value = dataRow["ReplicationSubscribed"];
            if (value != DBNull.Value)
            {
                Statistics.ReplicationSubscribed = (long)value;
            }

            value = dataRow["ReplicationUndistributed"];
            if (value != DBNull.Value)
            {
                Statistics.ReplicationUndistributed = (long)value;
            }

            value = dataRow["ReplicationUnsubscribed"];
            if (value != DBNull.Value)
            {
                Statistics.ReplicationUnsubscribed = (long)value;
            }

            value = dataRow["TempDBSizeInKilobytes"];
            if (value != DBNull.Value)
            {
                Statistics.TempDBSize.Kilobytes = (long)value;
            }

            value = dataRow["TempDBSizePercent"];
            if (value != DBNull.Value)
            {
                Statistics.TempDBSizePercent = (double)value;
            }

            value = dataRow["OldestOpenTransactionsInMinutes"];
            if (value != DBNull.Value)
            {
                Statistics.OldestOpenTransactionsInMinutes = (long)value;
            }

            value = dataRow["ActiveProcesses"];
            if (value != DBNull.Value)
            {
                SystemProcesses.ActiveProcesses = (int)value;
            }

            value = dataRow["SystemProcesses"];
            if (value != DBNull.Value)
            {
                SystemProcesses.CurrentSystemProcesses = (int)value;
            }

            value = dataRow["UserProcessesConsumingCPU"];
            if (value != DBNull.Value)
            {
                SystemProcesses.UserProcessesConsumingCpu = (int)value;
            }

            value = dataRow["SystemProcessesConsumingCPU"];
            if (value != DBNull.Value)
            {
                SystemProcesses.SystemProcessesConsumingCpu = (int)value;
            }

            value = dataRow["BlockedProcesses"];
            if (value != DBNull.Value)
            {
                SystemProcesses.BlockedProcesses = (int)value;
            }

            value = dataRow["OpenTransactions"];
            if (value != DBNull.Value)
            {
                SystemProcesses.OpenTransactions = (int)value;
            }

            value = dataRow["DatabaseCount"];
            if (value != DBNull.Value)
            {
                DatabaseSummary.DatabaseCount = (int)value;
            }

            value = dataRow["DataFileCount"];
            if (value != DBNull.Value)
            {
                DatabaseSummary.DataFileCount = (int)value;
            }

            value = dataRow["LogFileCount"];
            if (value != DBNull.Value)
            {
                DatabaseSummary.LogFileCount = (int)value;
            }

            value = dataRow["DataFileSpaceAllocatedInKilobytes"];
            if (value != DBNull.Value)
            {
                DatabaseSummary.DataFileSpaceAllocated.Kilobytes = (decimal)value;
            }

            value = dataRow["DataFileSpaceUsedInKilobytes"];
            if (value != DBNull.Value)
            {
                DatabaseSummary.DataFileSpaceUsed.Kilobytes = (decimal)value;
            }

            value = dataRow["LogFileSpaceAllocatedInKilobytes"];
            if (value != DBNull.Value)
            {
                DatabaseSummary.LogFileSpaceAllocated.Kilobytes = (decimal)value;
            }

            value = dataRow["LogFileSpaceUsedInKilobytes"];
            if (value != DBNull.Value)
            {
                DatabaseSummary.LogFileSpaceUsed.Kilobytes = (decimal)value;
            }

            value = dataRow["TotalLocks"];
            if (value != DBNull.Value)
            {
                TotalLocks = (decimal)value;
            }

            value = dataRow["BufferCacheSizeInKilobytes"];
            if (value != DBNull.Value)
            {
                Statistics.BufferCacheSize.Kilobytes = (long)value;
            }

            value = dataRow["OSTotalPhysicalMemoryInKilobytes"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.TotalPhysicalMemory.Kilobytes = (long)value;
            }

            value = dataRow["OSAvailableMemoryInKilobytes"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.AvailableBytes.Kilobytes = (long)value;
            }

            value = dataRow["PagesPerSecond"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.PagesPersec = (double)value;
            }

            value = dataRow["ProcessorTimePercent"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.PercentProcessorTime = (double)value;
            }

            value = dataRow["PrivilegedTimePercent"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.PercentPrivilegedTime = (double)value;
            }

            value = dataRow["UserTimePercent"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.PercentUserTime = (double)value;
            }

            value = dataRow["ProcessorQueueLength"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.ProcessorQueueLength = (double)value;
            }

            value = dataRow["DiskTimePercent"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.PercentDiskIdleTime = 100 - (double)value;
            }

            value = dataRow["DiskQueueLength"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.AvgDiskQueueLength = (double)value;
            }

            value = dataRow["RealServerName"];
            if (value != DBNull.Value)
            {
                RealServerName = (string)value;
            }

            value = dataRow["ServerHostName"];
            if (value != DBNull.Value)
            {
                ServerHostName = (string)value;
            }

            value = dataRow["WindowsVersion"];
            if (value != DBNull.Value)
            {
                WindowsVersion = (string)value;
            }

            value = dataRow["ProcessorCount"];
            if (value != DBNull.Value)
            {
                ProcessorCount = (int)value;
            }

            value = dataRow["ProcessorsUsed"];
            if (value != DBNull.Value)
            {
                ProcessorsUsed = (int)value;
            }

            value = dataRow["IsClustered"];
            if (value != DBNull.Value)
            {
                IsClustered = (bool)value;
            }

            value = dataRow["ClusterNodeName"];
            if (value != DBNull.Value)
            {
                ClusterNodeName = (string)value;
            }

            value = dataRow["RunningSince"];
            if (value != DBNull.Value)
            {
                RunningSince = ((DateTime)value).ToUniversalTime();
            }

            value = dataRow["MaxConnections"];
            if (value != DBNull.Value)
            {
                MaxConnections = (long)value;
            }

            value = dataRow["LeadBlockers"];
            if (value != DBNull.Value)
            {
                SystemProcesses.LeadBlockers = (int)value;
            }

            value = dataRow["OsStatisticAvailability"];
            if (value != DBNull.Value)
            {
                OSMetricsStatistics.OsStatisticAvailability = (string)value;
            }

            value = dataRow["vmUUID"];

            if (value != DBNull.Value)
            {
                VMConfig = new VMwareVirtualMachine(dataRow);
            }

            foreach (DataRow dbRow in dbDataRows)
            {
                string dbName;

                value = dbRow["DatabaseName"];

                if (value != DBNull.Value)
                {

                    dbName = (string)value;

                    if (!DbStatistics.ContainsKey(dbName))
                    {
                        DbStatistics.Add(dbName, new DatabaseStatistics(serverName, dbName));
                    }

                    value = dbRow["DatabaseStatus"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].Status = (DatabaseStatus)value;
                    }

                    value = dbRow["Transactions"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].Transactions = (long)value;
                    }

                    value = dbRow["LogFlushWaits"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].LogFlushWaits = (long)value;
                    }

                    value = dbRow["LogFlushes"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].LogFlushes = (long)value;
                    }

                    value = dbRow["LogKilobytesFlushed"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].LogSizeFlushed.Kilobytes = (long)value;
                    }

                    value = dbRow["LogCacheReads"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].LogCacheReads = (long)value;
                    }

                    value = dbRow["LogCacheHitRatio"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].LogCacheHitRatio = (double)value;
                    }

                    value = dbRow["NumberReads"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].Reads = (decimal)value;
                    }

                    value = dbRow["NumberWrites"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].Writes = (decimal)value;
                    }

                    value = dbRow["BytesRead"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].BytesRead = (decimal)value;
                    }

                    value = dbRow["BytesWritten"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].BytesWritten = (decimal)value;
                    }

                    value = dbRow["IoStallMS"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].IoStallMs = (decimal)value;
                    }
                    // cpu usage chart data
                    value = dbRow["AvgCpuPercent"];
                    if (value != DBNull.Value )
                    {
                        avgCpuPercent = Convert.ToDecimal(value);
                    }
                    value = dbRow["AvgDataIoPercent"];
                    if (value != DBNull.Value)
                    {
                        avgDataIoPercent = Convert.ToDecimal(value);
                    }
                    value = dbRow["AvgLogWritePercent"];
                    if (value != DBNull.Value)
                    {
                        avgLogWritePercent = Convert.ToDecimal(value);
                    }
                    value = dbRow["DtuLimit"];
                    if (value != DBNull.Value)
                    {
                        dtuLimit = Convert.ToDecimal(value);
                    }
                    DbStatistics[dbName].AzureDbDetail = new AzureDbDetail(avgCpuPercent, avgDataIoPercent,
                        avgLogWritePercent, (int?)dtuLimit, null);

                    value = dbRow["AzureCloudAllocatedMemory"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].AzureCloudAllocatedMemory = (decimal)value;
                    }

                    value = dbRow["AzureCloudUsedMemory"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].AzureCloudUsedMemory = (decimal)value;
                    }

                    value = dbRow["AzureCloudStorageLimit"];
                    if (value != DBNull.Value)
                    {
                        DbStatistics[dbName].AzureCloudStorageLimit = (decimal)value;
                    }
                }
            }

            object iowaits = dataRow["I/O"];
            object lockwaits = dataRow["Lock"];
            object memorywaits = dataRow["Memory"];
            object transactionlogwaits = dataRow["Transaction Log"];
            object otherwaits = dataRow["Other"];
            object signalwaits = dataRow["Signal"];

            if (iowaits != DBNull.Value || lockwaits != DBNull.Value ||
                memorywaits != DBNull.Value || transactionlogwaits != DBNull.Value ||
                otherwaits != DBNull.Value ||
                signalwaits != DBNull.Value)
            {
                waitStatSummary = new WaitStatisticsSummary();

                if (iowaits != DBNull.Value)
                    waitStatSummary.IOWaits = Convert.ToDecimal(iowaits);

                if (lockwaits != DBNull.Value)
                    waitStatSummary.LockWaits = Convert.ToDecimal(lockwaits);

                if (memorywaits != DBNull.Value)
                    waitStatSummary.MemoryWaits = Convert.ToDecimal(memorywaits);

                if (transactionlogwaits != DBNull.Value)
                    waitStatSummary.TransactionLogWaits = Convert.ToDecimal(transactionlogwaits);

                if (otherwaits != DBNull.Value)
                    waitStatSummary.OtherWaits = Convert.ToDecimal(otherwaits);

                if (signalwaits != DBNull.Value)
                    waitStatSummary.SignalWaits = Convert.ToDecimal(signalwaits);
            }

            value = dataRow["FreePagesInKilobytes"];
            if (value != DBNull.Value)
            {
                if (memoryStatistics == null)
                    memoryStatistics = new Memory();

                memoryStatistics.FreePages.Kilobytes = (long)value;
            }

            value = dataRow["CommittedInKilobytes"];
            if (value != DBNull.Value)
            {
                if (memoryStatistics == null)
                    memoryStatistics = new Memory();

                memoryStatistics.CommittedPages.Kilobytes = (long)value;
            }

            value = dataRow["OptimizerMemoryInKilobytes"];
            if (value != DBNull.Value)
            {
                if (memoryStatistics == null)
                    memoryStatistics = new Memory();

                memoryStatistics.OptimizerMemory.Kilobytes = (long)value;
            }

            value = dataRow["LockMemoryInKilobytes"];
            if (value != DBNull.Value)
            {
                if (memoryStatistics == null)
                    memoryStatistics = new Memory();

                memoryStatistics.LockMemory.Kilobytes = (long)value;
            }

            value = dataRow["ConnectionMemoryInKilobytes"];
            if (value != DBNull.Value)
            {
                if (memoryStatistics == null)
                    memoryStatistics = new Memory();

                memoryStatistics.ConnectionMemory.Kilobytes = (long)value;
            }

            value = dataRow["GrantedWorkspaceMemoryInKilobytes"];
            if (value != DBNull.Value)
            {
                if (memoryStatistics == null)
                    memoryStatistics = new Memory();

                memoryStatistics.GrantedWorkspaceMemory.Kilobytes = (long)value;
            }

            value = dataRow["CachePagesInKilobytes"];
            if (value != DBNull.Value)
            {
                if (memoryStatistics == null)
                    memoryStatistics = new Memory();

                memoryStatistics.CachePages.Kilobytes = (long)value;
            }
        }

        #endregion

        #region properties

        //6.2.4 Disk Section
        public double? DataIOUsage
        {
            get { return dataIOUsage; }
            internal set { dataIOUsage = value; }
        }

        //6.2.4 Disk Section
        public double? DataIORate
        {
            get { return dataIORate; }
            internal set { dataIORate = value; }
        }

        //6.2.4 Disk Section
        public double? LogIOUsage
        {
            get { return logIOUsage; }
            internal set { logIOUsage = value; }
        }

        //6.2.4 Disk Section
        public double? LogIORate
        {
            get { return logIORate; }
            internal set { logIORate = value; }
        }

        //6.2.4
        public List<Dictionary<String, String>> AzureIOMetrics
        {
            get { return azureIOMetrics; }
            internal set { azureIOMetrics = value; }
        }
        public double? ReadThroughput
        {
            get { return readThroughput; }
            internal set { readThroughput = value; }
        }
        public double? WriteThroughput
        {
            get { return writeThroughput; }
            internal set { writeThroughput = value; }
        }
        public double? CPUCreditUsage
        {
            get { return cPUCreditUsage; }
            internal set { cPUCreditUsage = value; }
        }
        public double? CPUCreditBalance
        {
            get { return cPUCreditBalance; }
            internal set { cPUCreditBalance = value; }
        }
        public double? SwapUsage
        {
            get { return swapUsage; }
            internal set { swapUsage = value; }
        }
        public double? ReadLatency
        {
            get { return readLatency; }
            internal set { readLatency = value; }
        }
        public double? WriteLatency
        {
            get { return writeLatency; }
            internal set { writeLatency = value; }
        }
       
        public double? DiskQueueDepth
        {
            get { return diskQueueDepth; }
            internal set { diskQueueDepth = value; }
        }
        public Memory MemoryStatistics
        {
            get { return memoryStatistics; }
            internal set { memoryStatistics = value; }
        }

        public Dictionary<string, DiskDrive> DiskDrives
        {
            get { return diskDrives; }
            internal set { diskDrives = value; }
        }

        public bool IsFileSystemObjectDataAvailable
        {
            get { return isFileSystemObjectDataAvailable; }
            internal set { isFileSystemObjectDataAvailable = value; }
        }

        /// <summary>
        /// Represents the SQL Server Agent service state
        /// </summary>
        public ServiceState? AgentServiceStatus
        {
            get { return agentServiceStatus; }
            internal set { agentServiceStatus = value; }
        }

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --represents the SQL Server Browser service state
        /// </summary>
        public ServiceState? SQLBrowserServiceStatus
        {
            get { return sqlBrowserServiceStatus; }
            internal set { sqlBrowserServiceStatus = value; }
        }

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --represents the SQL Active Directory Helper service state
        /// </summary>
        public ServiceState? SQLActiveDirectoryHelperServiceStatus
        {
            get { return sqlActiveDirectoryHelperServiceStatus; }
            internal set { sqlActiveDirectoryHelperServiceStatus = value; }
        }

        /// <summary>
        /// Represents the DTC service state
        /// </summary>
        public ServiceState? DtcServiceStatus
        {
            get { return dtcServiceStatus; }
            internal set { dtcServiceStatus = value; }
        }

        /// <summary>
        /// Represents the SQL Server Full Text Search service state
        /// </summary>
        public ServiceState? FullTextServiceStatus
        {
            get { return fullTextServiceStatus; }
            internal set { fullTextServiceStatus = value; }
        }

        /// <summary>
        /// The status of the SQL Server service
        /// </summary>
        public ServiceState? SqlServiceStatus
        {
            get { return sqlServiceStatus; }
            internal set { sqlServiceStatus = value; }
        }

        /// <summary>
        /// Represents the language of the target operating system
        /// </summary>
        public string Language
        {
            get { return language; }
            internal set { language = value; }
        }

        /// <summary>
        /// Indicates whether the current login has administrative rights to run the refresh
        /// </summary>
        public bool? LoginHasAdministratorRights
        {
            get { return loginHasAdministratorRights; }
            internal set { loginHasAdministratorRights = value; }
        }

        /// <summary>
        /// Maximum connections allowed to the server, or null if no maximum
        /// </summary>
        public Int64? MaxConnections
        {
            get { return maxConnections; }
            internal set { maxConnections = value; }
        }


        /// <summary>
        /// Physical memory on the target machine
        /// </summary>
        public FileSize PhysicalMemory
        {
            get { return physicalMemory; }
            internal set { physicalMemory = value; }
        }

        /// <summary>
        /// Number of CPUs on the target machine
        /// </summary>
        public int? ProcessorCount
        {
            get { return processorCount; }
            internal set { processorCount = value; }
        }

        /// <summary>
        /// Number of CPUs in use by SQL Server on the target machine
        /// </summary>
        public int? ProcessorsUsed
        {
            get { return processorsUsed; }
            internal set { processorsUsed = value; }
        }

        /// <summary>
        /// Processor type (x86, etc) of the target server
        /// </summary>
        public string ProcessorType
        {
            get { return processorType; }
            internal set { processorType = value; }
        }

        /// <summary>
        /// The server host name
        /// </summary>
        public string ServerHostName
        {
            get { return serverHostName; }
            internal set { serverHostName = value; }
        }

        /// <summary>
        /// The full server name (including named instance)
        /// </summary>
        public string RealServerName
        {
            get { return realServerName; }
            internal set { realServerName = value; }
        }

        /// <summary>
        /// The current date/time on the server.  Obsolete - use Timestamp instead.
        /// </summary>
        [Obsolete("Use Timestamp instead.")]
        public DateTime? ServerTimeUTC
        {
            get { return TimeStamp; }
            internal set { TimeStamp = value; }
        }

        /// <summary>
        /// The version of Windows on the target server
        /// </summary>
        public string WindowsVersion
        {
            get { return windowsVersion; }
            internal set { windowsVersion = value; }
        }

        public FileSize TargetServerMemory
        {
            get { return targetServerMemory; }
            internal set { targetServerMemory = value; }
        }

        public FileSize TotalServerMemory
        {
            get { return totalServerMemory; }
            internal set { totalServerMemory = value; }
        }

        public List<Dictionary<String, String>> ListOfDatabase
        {
            get { return listOfDatabase; }
            internal set { listOfDatabase = value; }
        }

        public double? SQlMemoryUsageBaselineMean
        {
            get { return sqlMemUsageMeanBaseline; }
            internal set { sqlMemUsageMeanBaseline = value; }
        }

        public double? SqlMemUsagesPercBaseline
        {
            get { return sqlMemUsagesPercBaseline; }
            set { sqlMemUsagesPercBaseline = value; }
        }
        /// <summary>
        /// Procedure cache size 
        /// </summary>
        public FileSize ProcedureCacheSize
        {
            get { return procedureCacheSize; }
            internal set { procedureCacheSize = value; }
        }


        /// <summary>
        /// Percentage of procedure cache currently in use
        /// </summary>
        public double? ProcedureCachePercentageUsed
        {
            get { return procedureCachePercentageUsed; }
            internal set { procedureCachePercentageUsed = value; }
        }


        public ServerStatistics Statistics
        {
            get { return statistics; }
            internal set { statistics = value; }
        }


        /// <summary>
        /// The number of system processes at the time of refresh
        /// </summary>
        public ServerSystemProcesses SystemProcesses
        {
            get { return systemProcesses; }
            internal set { systemProcesses = value; }
        }


        public bool? IsClustered
        {
            get { return isClustered; }
            internal set { isClustered = value; }
        }


        public string ClusterNodeName
        {
            get { return clusterNodeName; }
            internal set { clusterNodeName = value != null ? value.ToLower() : null; }
        }

        public OSMetrics OSMetricsStatistics
        {
            get { return osMetricsStatistics; }
            internal set { osMetricsStatistics = value; }
        }


        public DateTime? RunningSince
        {
            get { return runningSince; }
            internal set { runningSince = value; }
        }


        public ServerLoginConfiguration LoginConfiguration
        {
            get { return loginConfiguration; }
            internal set { loginConfiguration = value; }
        }


        public long ResponseTime
        {
            get { return responseTime; }
            internal set { responseTime = value; }
        }
        public double? ResponseTimeBaselineMean
        {
            get { return responseTimeMinutesBaseline; }
            internal set { responseTimeMinutesBaseline = value; }
        }

        public double? ResponseTimePercDeviationBaseine
        {
            get { return responseTimePercentage; }
            set { responseTimePercentage = value; }
        }

        public ServerDatabaseSummary DatabaseSummary
        {
            get { return databaseSummary; }
            internal set { databaseSummary = value; }
        }


        public decimal? TotalLocks
        {
            get { return totalLocks; }
            internal set { totalLocks = value; }
        }

        public TimeSpan? TimeDelta
        {
            get { return timeDelta; }
            internal set { timeDelta = value; }
        }


        public string SqlServerEdition
        {
            get { return sqlServerEdition; }
            internal set { sqlServerEdition = value; }
        }

        //SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -add maintenance mode status when a specific instance is viewed
        public bool MaintenanceModeEnabled
        {
            get { return maintenanceModeEnabled; }
            internal set { maintenanceModeEnabled = value; }
        }

        public bool MultipleDefaultInstancesPresent
        {
            get { return multipleDefaultInstancesPresent; }
            internal set { multipleDefaultInstancesPresent = value; }
        }

        public LockStatistics LockCounters
        {
            get { return lockCounters; }
            internal set { lockCounters = value; }
        }

        public TempdbSummaryStatistics TempdbStatistics
        {
            get { return tempdbStatistics; }
            internal set { tempdbStatistics = value; }
        }

        public Dictionary<string, DatabaseStatistics> DbStatistics
        {
            get { return dbStatistics; }
            internal set { dbStatistics = value; }
        }

        public Dictionary<string, FileActivityFile> FileActivity
        {
            get { return fileActivity; }
            private set { fileActivity = value; }
        }

        //public bool IsVirtualized
        //{
        //    get { return isVirtualized; }
        //    set { isVirtualized = value; }
        //}

        public VMwareVirtualMachine VMConfig
        {
            get { return vmConfig; }
            set { vmConfig = value; }
        }

        public WaitStatisticsSnapshot WaitStats { get; set; }

        public WaitStatisticsSummary WaitStatsSummary
        {
            get { return waitStatSummary; }
            private set { waitStatSummary = value; }
        }

        

        public decimal ManagedInstanceStorageLimit
        {
            get { return managedInstanceStorageLimit; }
            set { managedInstanceStorageLimit = value; }
        }

        public double? PagesReadPersec
        {
            get { return pagesReadPersec; }
            set { pagesReadPersec = value; }
        }

        public double? PagesWritePersec
        {
            get { return pagesWritePersec; }
            set { pagesWritePersec = value; }
        }

        public double? PagesPersec
        {
            get { return PagesWritePersec + PagesReadPersec; }
        }

        #endregion

        #region methods

        public void CalculateWaitStatisticsSummary(Dictionary<string, WaitTypeInfo> waitTypes)
        {
            if (WaitStats == null || WaitStats.Waits == null || WaitStats.Waits.Count <= 0)
                return;

            if (TimeDelta == null || !TimeDelta.HasValue || TimeDelta.Value.TotalSeconds <= 0)
                return;

            if (waitStatSummary == null)
                waitStatSummary = new WaitStatisticsSummary();

            double IOWaits = 0;
            double LockWaits = 0;
            double MemoryWaits = 0;
            double TransactionLogWaits = 0;
            double OtherWaits = 0;
            double SignalWaits = 0;

            foreach (Wait wait in WaitStats.Waits.Values)
            {
                string category;

                // Need to get wait types array
                if (waitTypes.ContainsKey(wait.WaitType))
                {
                    category = waitTypes[wait.WaitType].CategoryName;
                }
                else
                {
                    category = "Other";
                }

                if (wait.ResourceWaitTimeDelta.HasValue)
                {
                    switch (category)
                    {
                        case "I/O":
                            IOWaits += wait.ResourceWaitTimeDelta.Value.TotalMilliseconds;
                            break;
                        case "Lock":
                            LockWaits += wait.ResourceWaitTimeDelta.Value.TotalMilliseconds;
                            break;
                        case "Memory":
                            MemoryWaits += wait.ResourceWaitTimeDelta.Value.TotalMilliseconds;
                            break;
                        case "Transaction Log":
                            TransactionLogWaits += wait.ResourceWaitTimeDelta.Value.TotalMilliseconds;
                            break;
                        case "Excluded":
                            continue;
                        default:
                            OtherWaits += wait.ResourceWaitTimeDelta.Value.TotalMilliseconds;
                            break;
                    }
                }

                if (wait.SignalWaitTimeDelta.HasValue)
                {
                    SignalWaits += wait.SignalWaitTimeDelta.Value.TotalMilliseconds;
                }

                waitStatSummary.IOWaits = (decimal)(IOWaits / TimeDelta.Value.TotalSeconds);
                waitStatSummary.LockWaits = (decimal)(LockWaits / TimeDelta.Value.TotalSeconds);
                waitStatSummary.MemoryWaits = (decimal)(MemoryWaits / TimeDelta.Value.TotalSeconds);
                waitStatSummary.TransactionLogWaits = (decimal)(TransactionLogWaits / TimeDelta.Value.TotalSeconds);
                waitStatSummary.OtherWaits = (decimal)(OtherWaits / TimeDelta.Value.TotalSeconds);
                waitStatSummary.SignalWaits = (decimal)(SignalWaits / TimeDelta.Value.TotalSeconds);
            }
        }

        #endregion
    }
}
