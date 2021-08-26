//------------------------------------------------------------------------------
// <copyright file="ServerStatistics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// Represents the calculated statistics for a scheduled refresh
    /// </summary>
    [Serializable]
    public class ServerStatistics
    {
        #region fields

        //private MonitoredSqlServer monitoredServer;

        private Int64? cpuBusyRaw = null;
        private Int64? idleTimeRaw = null;
        private Int64? ioTimeRaw = null;
        private Int64? packetsReceivedRaw = null;
        private Int64? packetsSentRaw = null;
        private Int64? packetErrorsRaw = null;
        private Int64? diskReadRaw = null;
        private Int64? diskWriteRaw = null;
        private Int64? diskErrorsRaw = null;
        private Int64? totalConnectionsRaw = null;

        private CpuTime cpuBusyDelta = null;
        private CpuTime idleTimeDelta = null;
        private CpuTime ioTimeDelta = null;
        private int? timeTicks = 31250;

        private double? cpuPercentage = null;
        //START - 10.0 SQLdm Srishti purohit -- Precentage deviation calculation using baseline for metric having baseline enabled
        private double? cpuBaseline = null;
        private double? cpuBaselinePercentage = null;
        //END - 10.0 SQLdm Srishti purohit -- Precentage deviation calculation using baseline for metric having baseline enabled
        private double? idlePercentage = null;
        private double? ioPercentage = null;

		//6.2.4
		private double? dataIOUsage = null;
		private double? logIOUsage = null;
		private double? dataIORate = null;
		private double? logIORate = null;
		
        private Int64? packetsReceived = null;
        private Int64? packetsSent = null;
        private Int64? packetErrors = null;
        private Int64? diskRead = null;
        private Int64? diskWrite = null;
        private Int64? diskErrors = null;
        private Int64? totalConnections = null;

        private Int64? batchRequestsRaw = null;
        private double? bufferCacheHitRatio = null;
        private Int64? bufferCacheHitRatioRaw = null;
        private Int64? bufferCacheHitRatioBaseRaw = null;
        private FileSize bufferCacheSize = new FileSize();
        private double? cacheHitRatio = null;
        private Int64? cacheHitRatioRaw = null;
        private Int64? cacheHitRatioBaseRaw = null;
        private Int64? checkpointPagesRaw = null;
        private Int64? fullScansRaw = null;
        private Int64? lazyWritesRaw = null;
        private Int64? lockWaitsRaw = null;
        private Int64? logFlushesRaw = null;
        private Int64? pageLookupsRaw = null;
        private Int64? pageReadsRaw = null;
        private Int64? pageSplitsRaw = null;
        private Int64? pageWritesRaw = null;
        private Int64? readaheadPagesRaw = null;
        private Int64? sqlCompilationsRaw = null;
        private Int64? sqlRecompilationsRaw = null;
        private Int64? tableLockEscalationsRaw = null;
        private Int64? workfilesCreatedRaw = null;
        private Int64? worktablesCreatedRaw = null;

        private Int64? batchRequests = null;
        private Int64? checkpointPages = null;
        private Int64? fullScans = null;
        private Int64? lazyWrites = null;
        private Int64? lockWaits = null;
        private Int64? logFlushes = null;
        private TimeSpan? pageLifeExpectancy = null;
        private Int64? pageLookups = null;
        private Int64? pageReads = null;
        private Int64? pageSplits = null;
        private Int64? pageWrites = null;
        private Int64? readaheadPages = null;
        private Int64? sqlCompilations = null;
        private Int64? sqlRecompilations = null;
        private Int64? tableLockEscalations = null;
        private Int64? workfilesCreated = null;
        private Int64? worktablesCreated = null;

        // SQLdm8.5 Mahesh:Additional Fields required for Rest Service
        private long? replicationUndistributed = null;
        private long? replicationSubscribed = null;
        private long? replicationUnsubscribed = null;
        private double? replicationLatencyInSeconds = null;
        private FileSize tempDBSize = new FileSize();
        private double? tempDBSizePercent = null;
        private long? oldestOpenTransactionsInMinutes = null;

        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
        private double? pageLifeExpectancyBaselineMean = null;
        private double? pageLifeExpectancyAsBaselinePerc = null;

        private double? procCacheHitRatioBaselineMean = null;
        private double? procCacheHitRatioAsBaselinePerc = null;
        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
        private int SqlProcessID;
        private int? _logicalProcessors = null;
        #endregion

        #region constructors

        public ServerStatistics()
        {
        }

        //public ServerStatistics(MonitoredSqlServer monitoredServer)
        //{
        //    //MonitoredServer = monitoredServer;
        //}

        #endregion

        #region properties

        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
        public double? PageLifeExpectancyBaselineMean
        {
            get { return pageLifeExpectancyBaselineMean; }
            internal set { pageLifeExpectancyBaselineMean = value; }
        }
        public double? PageLifeExpectancyAsBaselinePerc
        {
            get { return pageLifeExpectancyAsBaselinePerc; }
            internal set { pageLifeExpectancyAsBaselinePerc = value; }
        }

        public double? ProcCacheHitRatioBaselineMean
        {
            get { return procCacheHitRatioBaselineMean; }
            internal set { procCacheHitRatioBaselineMean = value; }
        }
        public double? ProcCacheHitRatioAsBaselinePerc
        {
            get { return procCacheHitRatioAsBaselinePerc; }
            internal set { procCacheHitRatioAsBaselinePerc = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline

        public long? ReplicationUndistributed
        {
            get { return replicationUndistributed; }
            internal set { replicationUndistributed = value; }
        }

        public long? ReplicationSubscribed
        {
            get { return replicationSubscribed; }
            internal set { replicationSubscribed = value; }
        }

        public long? ReplicationUnsubscribed
        {
            get { return replicationUnsubscribed; }
            internal set { replicationUnsubscribed = value; }
        }

        public double? ReplicationLatencyInSeconds
        {
            get { return replicationLatencyInSeconds; }
            internal set { replicationLatencyInSeconds = value; }
        }
        
        public double? TempDBSizePercent
        {
            get { return tempDBSizePercent; }
            internal set { tempDBSizePercent = value; }
        }

        public FileSize TempDBSize
        {
            get { return tempDBSize; }
            internal set { tempDBSize = value; }
        }

        public long? OldestOpenTransactionsInMinutes
        {
            get { return oldestOpenTransactionsInMinutes; }
            internal set { oldestOpenTransactionsInMinutes = value; }
        }
        
        /// <summary>
        /// CPU time elapsed as calculated against the previous successful scheduled refresh
        /// </summary>
        public CpuTime CpuBusyDelta
        {
            get { return cpuBusyDelta; }
            internal set { cpuBusyDelta = value; }
        }

        /// <summary>
        /// CPU percentage used as calculated against the previous successful scheduled refresh
        /// </summary>
        public double? CpuPercentage
        {
            get { return cpuPercentage; }
            internal set { cpuPercentage = value; }
        }
        /// <summary>
        /// CPU baseline value used to calculate cpu percentage difference respect to baseline 
        /// </summary>
        public double? CpuBaseline
        {
            get { return cpuBaseline; }
            internal set { cpuBaseline = value; }
        }
		
		//6.2.4
		public double? DataIOUsage
		{
			get { return dataIOUsage; }
			internal set { dataIOUsage = value; }
		}
		
		//6.2.4
		public double? LogIOUsage
		{
			get { return logIOUsage; }
			internal set { logIOUsage = value; }
		}
		
		//6.2.4
		public double? DataIORate
		{
			get { return dataIORate; }
			internal set { dataIORate = value; }
		}
		
		//6.2.4
		public double? LogIORate
		{
			get { return logIORate; }
			internal set { logIORate = value; }
		}
			
        /// <summary>
        /// CPU baseline percentage used as calculated against the previous successful scheduled refresh
        /// </summary>
        public double? CpuBaselinePercentage
        {
            get { return cpuBaselinePercentage; }
            internal set { cpuBaselinePercentage = value; }
        }

        /// <summary>
        /// Idle Time as calculated against the previous successful scheduled refresh
        /// </summary>
        public CpuTime IdleTimeDelta
        {
            get { return idleTimeDelta; }
            internal set { idleTimeDelta = value; }
        }


        public double? IdlePercentage
        {
            get { return idlePercentage; }
            internal set { idlePercentage = value; }
        }


        /// <summary>
        /// IO Time as calculated against the previous successful scheduled refresh
        /// </summary>
        public CpuTime IoTimeDelta
        {
            get { return ioTimeDelta; }
            internal set { ioTimeDelta = value; }
        }


        public double? IoPercentage
        {
            get { return ioPercentage; }
            internal set { ioPercentage = value; }
        }


        public int? TimeTicks
        {
            get { return timeTicks; }
            internal set { timeTicks = value; }
        }

        /// <summary>
        /// Network packets received as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? PacketsReceived
        {
            get { return packetsReceived; }
            internal set { packetsReceived = value; }
        }

        /// <summary>
        /// Network packets sent as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? PacketsSent
        {
            get { return packetsSent; }
            internal set { packetsSent = value; }
        }

        /// <summary>
        /// Packet errors as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? PacketErrors
        {
            get { return packetErrors; }
            internal set { packetErrors = value; }
        }

        /// <summary>
        /// Disk read as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? DiskRead
        {
            get { return diskRead; }
            internal set { diskRead = value; }
        }

        /// <summary>
        /// Disk write as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? DiskWrite
        {
            get { return diskWrite; }
            internal set { diskWrite = value; }
        }

        /// <summary>
        /// Disk errors as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? DiskErrors
        {
            get { return diskErrors; }
            internal set { diskErrors = value; }
        }

        /// <summary>
        /// Total connections as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? TotalConnections
        {
            get { return totalConnections; }
            internal set { totalConnections = value; }
        }

        /// <summary>
        /// Batch requests as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? BatchRequests
        {
            get { return batchRequests; }
            internal set { batchRequests = value; }
        }

        /// <summary>
        /// Calculated Buffer Cache Hit Ratio
        /// </summary>
        public double? BufferCacheHitRatio
        {
            get
            {
                if (bufferCacheHitRatio.HasValue)
                {
                    return bufferCacheHitRatio;
                }
                else if (BufferCacheHitRatioRaw.HasValue && BufferCacheHitRatioBaseRaw.HasValue &&
                    BufferCacheHitRatioBaseRaw > 0)
                {
                    return 100 * (BufferCacheHitRatioRaw.Value / (double)BufferCacheHitRatioBaseRaw.Value);
                }
                else
                    return null;
            }
            internal set { bufferCacheHitRatio = value; }
        }


        public FileSize BufferCacheSize
        {
            get { return bufferCacheSize; }
            internal set { bufferCacheSize = value; }
        }

        /// <summary>
        /// Calculated Cache Hit Ratio
        /// </summary>
        public double? CacheHitRatio
        {
            get
            {
                if (cacheHitRatio.HasValue)
                {
                    return cacheHitRatio;
                }
                else if (CacheHitRatioRaw.HasValue && CacheHitRatioBaseRaw.HasValue && CacheHitRatioBaseRaw > 0)
                {
                    return 100 * (CacheHitRatioRaw.Value / (double)CacheHitRatioBaseRaw.Value);
                }
                else
                    return null;
            }
            internal set { cacheHitRatio = value; }
        }

        /// <summary>
        /// Checkpoint pages as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? CheckpointPages
        {
            get { return checkpointPages; }
            internal set { checkpointPages = value; }
        }

        /// <summary>
        /// Full scanes as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? FullScans
        {
            get { return fullScans; }
            internal set { fullScans = value; }
        }

        /// <summary>
        /// Lazy writes as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? LazyWrites
        {
            get { return lazyWrites; }
            internal set { lazyWrites = value; }
        }

        /// <summary>
        /// Lock waits as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? LockWaits
        {
            get { return lockWaits; }
            internal set { lockWaits = value; }
        }

        /// <summary>
        /// Log flushes as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? LogFlushes
        {
            get { return logFlushes; }
            internal set { logFlushes = value; }
        }

        /// <summary>
        /// Page life expectancy as calculated against the previous successful scheduled refresh
        /// </summary>
        public TimeSpan? PageLifeExpectancy
        {
            get { return pageLifeExpectancy; }
            internal set { pageLifeExpectancy = value; }
        }

        /// <summary>
        /// Page life expectancy as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? PageLifeExpectancySeconds
        {
            get
            {
                if (pageLifeExpectancy.HasValue)
                    return (long)Math.Floor(pageLifeExpectancy.Value.TotalSeconds);
                return null;
            }
            set
            {

                {
                    if (value.HasValue)
                        pageLifeExpectancy = TimeSpan.FromSeconds(value.Value);
                    else
                        pageLifeExpectancy = null;
                }
            }
        }


        /// <summary>
        /// Page lookups as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? PageLookups
        {
            get { return pageLookups; }
            internal set { pageLookups = value; }
        }

        /// <summary>
        /// Page reads as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? PageReads
        {
            get { return pageReads; }
            internal set { pageReads = value; }
        }

        /// <summary>
        /// Page splits as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? PageSplits
        {
            get { return pageSplits; }
            internal set { pageSplits = value; }
        }

        /// <summary>
        /// Page writes as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? PageWrites
        {
            get { return pageWrites; }
            internal set { pageWrites = value; }
        }

        /// <summary>
        /// Readahead pages as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? ReadaheadPages
        {
            get { return readaheadPages; }
            internal set { readaheadPages = value; }
        }

        /// <summary>
        /// SQL compilations as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? SqlCompilations
        {
            get { return sqlCompilations; }
            internal set { sqlCompilations = value; }
        }

        public long? Transactions { get; internal set; }

        /// <summary>
        /// SQL recompilations as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? SqlRecompilations
        {
            get { return sqlRecompilations; }
            internal set { sqlRecompilations = value; }
        }

        /// <summary>
        /// Table lock escalations as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? TableLockEscalations
        {
            get { return tableLockEscalations; }
            internal set { tableLockEscalations = value; }
        }


        /// <summary>
        /// Workfiles created as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? WorkfilesCreated
        {
            get { return workfilesCreated; }
            internal set { workfilesCreated = value; }
        }


        /// <summary>
        /// Worktables created as calculated against the previous successful scheduled refresh
        /// </summary>
        public long? WorktablesCreated
        {
            get { return worktablesCreated; }
            internal set { worktablesCreated = value; }
        }




        #region raw value properties

        /// <summary>
        /// The raw CPU Busy value from SQL Server.  Not for display.
        /// </summary>
        public Int64? CpuBusyRaw
        {
            get { return cpuBusyRaw; }
            internal set { cpuBusyRaw = value; }
        }


        /// <summary>
        /// The raw Idle Time value from SQL Server.  Not for display.
        /// </summary>
        public Int64? IdleTimeRaw
        {
            get { return idleTimeRaw; }
            internal set { idleTimeRaw = value; }
        }

        /// <summary>
        /// The raw IO Time value from SQL Server.  Not for display.
        /// </summary>
        public Int64? IoTimeRaw
        {
            get { return ioTimeRaw; }
            internal set { ioTimeRaw = value; }
        }

        /// <summary>
        /// The raw Packets Received value from SQL Server.  Not for display.
        /// </summary>
        public Int64? PacketsReceivedRaw
        {
            get { return packetsReceivedRaw; }
            internal set { packetsReceivedRaw = value; }
        }

        /// <summary>
        /// The raw Packets Sent value from SQL Server.  Not for display.
        /// </summary>
        public Int64? PacketsSentRaw
        {
            get { return packetsSentRaw; }
            internal set { packetsSentRaw = value; }
        }

        /// <summary>
        /// The raw Packets Errors value from SQL Server.  Not for display.
        /// </summary>
        public Int64? PacketErrorsRaw
        {
            get { return packetErrorsRaw; }
            internal set { packetErrorsRaw = value; }
        }

        /// <summary>
        /// The raw Total Read value from SQL Server.  Not for display.
        /// </summary>
        public Int64? DiskReadRaw
        {
            get { return diskReadRaw; }
            internal set { diskReadRaw = value; }
        }

        /// <summary>
        /// The raw Total Write value from SQL Server.  Not for display.
        /// </summary>
        public Int64? DiskWriteRaw
        {
            get { return diskWriteRaw; }
            internal set { diskWriteRaw = value; }
        }

        /// <summary>
        /// The raw Total Errors value from SQL Server.  Not for display.
        /// </summary>
        public Int64? DiskErrorsRaw
        {
            get { return diskErrorsRaw; }
            internal set { diskErrorsRaw = value; }
        }

        /// <summary>
        /// The raw Total Connections value from SQL Server.  Not for display.
        /// </summary>
        public Int64? TotalConnectionsRaw
        {
            get { return totalConnectionsRaw; }
            internal set { totalConnectionsRaw = value; }
        }

        /// <summary>
        /// The raw batch requests/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? BatchRequestsRaw
        {
            get { return batchRequestsRaw; }
            internal set { batchRequestsRaw = value; }
        }

        /// <summary>
        /// The raw buffer cache hit ratio value from SQL Server.  Not for display.
        /// </summary>
        public long? BufferCacheHitRatioRaw
        {
            get { return bufferCacheHitRatioRaw; }
            internal set { bufferCacheHitRatioRaw = value; }
        }

        /// <summary>
        /// The raw buffer cache hit ratio base value from SQL Server.  Not for display.
        /// </summary>
        public long? BufferCacheHitRatioBaseRaw
        {
            get { return bufferCacheHitRatioBaseRaw; }
            internal set { bufferCacheHitRatioBaseRaw = value; }
        }

        /// <summary>
        /// The raw cache hit ratio value from SQL Server.  Not for display.
        /// </summary>
        public long? CacheHitRatioRaw
        {
            get { return cacheHitRatioRaw; }
            internal set { cacheHitRatioRaw = value; }
        }

        /// <summary>
        /// The raw cache hit ratio base value from SQL Server.  Not for display.
        /// </summary>
        public long? CacheHitRatioBaseRaw
        {
            get { return cacheHitRatioBaseRaw; }
            internal set { cacheHitRatioBaseRaw = value; }
        }

        /// <summary>
        /// The raw checkpoint pages/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? CheckpointPagesRaw
        {
            get { return checkpointPagesRaw; }
            internal set { checkpointPagesRaw = value; }
        }

        /// <summary>
        /// The raw full scans/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? FullScansRaw
        {
            get { return fullScansRaw; }
            internal set { fullScansRaw = value; }
        }

        /// <summary>
        /// The raw lazy writes/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? LazyWritesRaw
        {
            get { return lazyWritesRaw; }
            internal set { lazyWritesRaw = value; }
        }

        /// <summary>
        /// The raw lock waits/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? LockWaitsRaw
        {
            get { return lockWaitsRaw; }
            internal set { lockWaitsRaw = value; }
        }

        /// <summary>
        /// The raw log flushes/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? LogFlushesRaw
        {
            get { return logFlushesRaw; }
            internal set { logFlushesRaw = value; }
        }

        /// <summary>
        /// The raw page lookups/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? PageLookupsRaw
        {
            get { return pageLookupsRaw; }
            internal set { pageLookupsRaw = value; }
        }

        /// <summary>
        /// The raw page reads/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? PageReadsRaw
        {
            get { return pageReadsRaw; }
            internal set { pageReadsRaw = value; }
        }

        /// <summary>
        /// The raw page splits/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? PageSplitsRaw
        {
            get { return pageSplitsRaw; }
            internal set { pageSplitsRaw = value; }
        }

        /// <summary>
        /// The raw page writes/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? PageWritesRaw
        {
            get { return pageWritesRaw; }
            internal set { pageWritesRaw = value; }
        }

        /// <summary>
        /// The raw readahead pages/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? ReadaheadPagesRaw
        {
            get { return readaheadPagesRaw; }
            internal set { readaheadPagesRaw = value; }
        }

        /// <summary>
        /// The raw sql compilations/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? SqlCompilationsRaw
        {
            get { return sqlCompilationsRaw; }
            internal set { sqlCompilationsRaw = value; }
        }

        /// <summary>
        /// The raw sql re-compilations/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? SqlRecompilationsRaw
        {
            get { return sqlRecompilationsRaw; }
            internal set { sqlRecompilationsRaw = value; }
        }

        /// <summary>
        /// The raw table lock escalations/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? TableLockEscalationsRaw
        {
            get { return tableLockEscalationsRaw; }
            internal set { tableLockEscalationsRaw = value; }
        }

        /// <summary>
        /// The raw workfiles created/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? WorkfilesCreatedRaw
        {
            get { return workfilesCreatedRaw; }
            internal set { workfilesCreatedRaw = value; }
        }

        /// <summary>
        /// The raw worktables created/sec value from SQL Server.  Not for display.
        /// </summary>
        public long? WorktablesCreatedRaw
        {
            get { return worktablesCreatedRaw; }
            internal set { worktablesCreatedRaw = value; }
        }




        internal long? TransactionsRaw { get; set; }

        public int SQLProcessID
        {
            get { return SqlProcessID; }
            set { SqlProcessID = value; }
        }

        public int? LogicalProcessors
        {
            get { return _logicalProcessors; }
            set { _logicalProcessors = value; }
        }

        #endregion

        #endregion

        #region events

        #endregion

        #region methods

        private static Int64? CalculateInt64CounterDelta(Int64? previousCounter, Int64? currentCounter)
        {
            if (previousCounter.HasValue && currentCounter.HasValue)
            {
                Int64? counterDelta = (Int64?)((UInt64?)currentCounter - (UInt64?)previousCounter);
                if (counterDelta < 0)
                    return null;
                else
                    return counterDelta;
            }
            else
            {
                return null;
            }
        }

        public static Int64? CalculateCpuTime(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.CpuBusyRaw, currentRefresh.CpuBusyRaw);
        }

        public static Int64? CalculateIdleTime(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.IdleTimeRaw, currentRefresh.IdleTimeRaw);
        }

        public static Int64? CalculateIoTime(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.IoTimeRaw, currentRefresh.IoTimeRaw);
        }


        public static Int64? CalculatePacketsReceived(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.PacketsReceivedRaw, currentRefresh.PacketsReceivedRaw);
        }

        public static Int64? CalculatePacketsSent(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.PacketsSentRaw, currentRefresh.PacketsSentRaw);
        }

        public static Int64? CalculatePacketErrors(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.PacketErrorsRaw, currentRefresh.PacketErrorsRaw);
        }

        public static Int64? CalculateDiskRead(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.DiskReadRaw, currentRefresh.DiskReadRaw);
        }

        public static Int64? CalculateDiskWrite(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.DiskWriteRaw, currentRefresh.DiskWriteRaw);
        }

        public static Int64? CalculateDiskErrors(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.DiskErrorsRaw, currentRefresh.DiskErrorsRaw);
        }

        public static Int64? CalculateTotalConnections(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.TotalConnectionsRaw, currentRefresh.TotalConnectionsRaw);
        }

        public static Int64? CalculateBatchRequests(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.BatchRequestsRaw, currentRefresh.BatchRequestsRaw);
        }

        public static Int64? CalculateCheckpointPages(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.CheckpointPagesRaw, currentRefresh.CheckpointPagesRaw);
        }

        public static Int64? CalculateFullScans(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.FullScansRaw, currentRefresh.FullScansRaw);
        }

        public static Int64? CalculateLazyWrites(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.LazyWritesRaw, currentRefresh.LazyWritesRaw);
        }

        public static Int64? CalculateLockWaits(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.LockWaitsRaw, currentRefresh.LockWaitsRaw);
        }

        public static Int64? CalculateLogFlushes(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.LogFlushesRaw, currentRefresh.LogFlushesRaw);
        }

        public static Int64? CalculatePageLookups(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.PageLookupsRaw, currentRefresh.PageLookupsRaw);
        }

        public static Int64? CalculatePageReads(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.PageReadsRaw, currentRefresh.PageReadsRaw);
        }

        public static Int64? CalculatePageSplits(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.PageSplitsRaw, currentRefresh.PageSplitsRaw);
        }

        public static Int64? CalculatePageWrites(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.PageWritesRaw, currentRefresh.PageWritesRaw);
        }

        public static Int64? CalculateReadaheadPages(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.ReadaheadPagesRaw, currentRefresh.ReadaheadPagesRaw);
        }

        public static Int64? CalculateSqlCompilations(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.SqlCompilationsRaw, currentRefresh.SqlCompilationsRaw);
        }

        public static Int64? CalculateSqlRecompilations(ServerStatistics previousRefresh,
                                                        ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.SqlRecompilationsRaw, currentRefresh.SqlRecompilationsRaw);
        }

        public static Int64? CalculateTableLockEscalations(ServerStatistics previousRefresh,
                                                           ServerStatistics currentRefresh)
        {
            return
                CalculateInt64CounterDelta(previousRefresh.TableLockEscalationsRaw,
                                           currentRefresh.TableLockEscalationsRaw);
        }

        public static Int64? CalculateWorkfilesCreated(ServerStatistics previousRefresh, ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.WorkfilesCreatedRaw, currentRefresh.WorkfilesCreatedRaw);
        }

        public static Int64? CalculateWorktablesCreated(ServerStatistics previousRefresh,
                                                        ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.WorktablesCreatedRaw, currentRefresh.WorktablesCreatedRaw);
        }

        public static Int64? CalculateTransactions(ServerStatistics previousRefresh,
                                                        ServerStatistics currentRefresh)
        {
            return CalculateInt64CounterDelta(previousRefresh.TransactionsRaw, currentRefresh.TransactionsRaw);
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion


    }
}