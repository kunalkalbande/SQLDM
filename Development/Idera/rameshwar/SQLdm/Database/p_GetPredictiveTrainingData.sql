
IF (OBJECT_ID('p_GetPredictiveTrainingData') IS NOT NULL)
BEGIN
	DROP PROCEDURE [p_GetPredictiveTrainingData]
END
GO

CREATE PROCEDURE [dbo].[p_GetPredictiveTrainingData] 
	@SID int,
	@MID int,
	@SEV int,
	@OFF int,
	@PRE int,
	@CUT DateTime	
AS
BEGIN	

declare @severity   int
declare @metricid   int
declare @serverid   int
declare @servername varchar(128)
declare @maxsid     int
declare @offset     int      -- time before first alert to encode as ON
declare @preoffset  int      -- number of minutes prior to offset range to also include -- enocded as OFF
declare @cutoff     DateTime -- cutoff for obtaining data

set @serverid  = @SID            
set @metricid  = @MID
set @severity  = @SEV
set @offset    = @OFF -- offset of tminus in minutes (how many minutes prior)
set @preoffset = @PRE -- minutes prior to offset to include
set @cutoff    = @CUT

set nocount on

select @servername = InstanceName from MonitoredSQLServers with (nolock) where SQLServerID = @serverid

declare @alerttimes table
(
    idx		int identity,
    tstamp	datetime
)

insert into @alerttimes 
select UTCOccurrenceDateTime from Alerts with (nolock)
where ServerName = @servername and Metric = @metricid and Severity = @severity
and (       (StateEvent & 0x12) = 0x12 -- OK -> Warning
        or	(StateEvent & 0x14) = 0x14 -- OK -> Critical
        or	(StateEvent & 0x24) = 0x24 -- Warning -> Critical
        or  (StateEvent & 0x53) = 0x53 -- Info -> Warning
        or  (StateEvent & 0x54) = 0x54 -- Info -> Critical
    )
and UTCOccurrenceDateTime <= @cutoff            	

declare @results table
(
    STATE int NULL,
    UTCCollectionDateTime datetime NULL,
    [Day] int NULL,
    [Weekday] int NULL,
    [Hour] int NULL,
    TimeDeltaInSeconds float NULL,
    BufferCacheHitRatioPercentage float NULL,
    CheckpointWrites bigint NULL,
    ClientComputers int NULL,
    CPUActivityPercentage float NULL,
    CPUTimeDelta bigint NULL,
    CPUTimeRaw bigint NULL,
    FullScans bigint NULL,
    IdleTimeDelta bigint NULL,
    IdleTimePercentage float NULL,
    IdleTimeRaw bigint NULL,
    IOActivityPercentage float NULL,
    IOTimeDelta bigint NULL,
    IOTimeRaw bigint NULL,
    LazyWriterWrites bigint NULL,
    LockWaits bigint NULL,
    Logins bigint NULL,
    LogFlushes bigint NULL,
    SqlMemoryAllocatedInKilobytes bigint NULL,
    SqlMemoryUsedInKilobytes bigint NULL,
    OldestOpenTransactionsInMinutes bigint NULL,
    PacketErrors bigint NULL,
    PacketsReceived bigint NULL,
    PacketsSent bigint NULL,
    PageErrors bigint NULL,
    PageLifeExpectancy bigint NULL,
    PageLookups bigint NULL,
    PageReads bigint NULL,
    PageSplits bigint NULL,
    PageWrites bigint NULL,
    ProcedureCacheHitRatioPercentage float NULL,
    ProcedureCacheSizeInKilobytes bigint NULL,
    ProcedureCacheSizePercent float NULL,
    ReadAheadPages bigint NULL,
    ReplicationLatencyInSeconds float NULL,
    ResponseTimeInMilliseconds int NULL,
    SqlCompilations bigint NULL,
    SqlRecompilations bigint NULL,
    TableLockEscalations bigint NULL,
    TempDBSizeInKilobytes bigint NULL,
    TempDBSizePercent float NULL,
    Batches bigint NULL,
    UserProcesses int NULL,
    WorkFilesCreated bigint NULL,
    WorkTablesCreated bigint NULL,
    SystemProcesses int NULL,
    UserProcessesConsumingCPU int NULL,
    SystemProcessesConsumingCPU int NULL,
    BlockedProcesses int NULL,
    OpenTransactions int NULL,
    DatabaseCount int NULL,
    DataFileCount int NULL,
    LogFileCount int NULL,
    DataFileSpaceAllocatedInKilobytes decimal(18,0) NULL,
    DataFileSpaceUsedInKilobytes decimal(18,0) NULL,
    LogFileSpaceAllocatedInKilobytes decimal(18,0) NULL,
    LogFileSpaceUsedInKilobytes decimal(18,0) NULL,
    TotalLocks decimal(18,0) NULL,
    BufferCacheSizeInKilobytes bigint NULL,
    ActiveProcesses int NULL,
    LeadBlockers int NULL,
    CommittedInKilobytes bigint NULL,
    ConnectionMemoryInKilobytes bigint NULL,
    FreePagesInKilobytes bigint NULL,
    GrantedWorkspaceMemoryInKilobytes bigint NULL,
    LockMemoryInKilobytes bigint NULL,
    OptimizerMemoryInKilobytes bigint NULL,
    TotalServerMemoryInKilobytes bigint NULL,
    FreeCachePagesInKilobytes bigint NULL,
    CachePagesInKilobytes bigint NULL,
    MaxConnections bigint NULL,
    PhysicalMemoryInKilobytes bigint NULL,
    ProcessorCount int NULL,
    ProcessorsUsed int NULL,
    DistributionLatencyInSeconds float NULL
)

--SQLdm(10.2)--Tushar--Modified the while loop logic.
select top 1500
        case 
            when UTCCollectionDateTime = a.tstamp then 1
            else 0
        end as [STATE],
        UTCCollectionDateTime, DATEPART(DAY, UTCCollectionDateTime) as [Day], DATEPART(WEEKDAY, UTCCollectionDateTime) as [Weekday], DATEPART(HOUR, UTCCollectionDateTime) as [Hour],
		TimeDeltaInSeconds, BufferCacheHitRatioPercentage, CheckpointWrites, ClientComputers, CPUActivityPercentage, 
        CPUTimeDelta, CPUTimeRaw, FullScans, IdleTimeDelta, IdleTimePercentage, IdleTimeRaw, IOActivityPercentage, IOTimeDelta, IOTimeRaw, LazyWriterWrites, LockWaits, Logins, LogFlushes, SqlMemoryAllocatedInKilobytes, SqlMemoryUsedInKilobytes, OldestOpenTransactionsInMinutes, 
        PacketErrors, PacketsReceived, PacketsSent, PageErrors, PageLifeExpectancy, PageLookups, PageReads, PageSplits, PageWrites, ProcedureCacheHitRatioPercentage, ProcedureCacheSizeInKilobytes, ProcedureCacheSizePercent, ReadAheadPages, ReplicationLatencyInSeconds, ResponseTimeInMilliseconds, 
        SqlCompilations, SqlRecompilations, TableLockEscalations, TempDBSizeInKilobytes, TempDBSizePercent, Batches, UserProcesses, WorkFilesCreated, WorkTablesCreated, SystemProcesses, UserProcessesConsumingCPU, SystemProcessesConsumingCPU, BlockedProcesses, OpenTransactions, 
        DatabaseCount, DataFileCount, LogFileCount, DataFileSpaceAllocatedInKilobytes, DataFileSpaceUsedInKilobytes, LogFileSpaceAllocatedInKilobytes, LogFileSpaceUsedInKilobytes, TotalLocks, BufferCacheSizeInKilobytes, ActiveProcesses, LeadBlockers, CommittedInKilobytes, ConnectionMemoryInKilobytes, 
        FreePagesInKilobytes, GrantedWorkspaceMemoryInKilobytes, LockMemoryInKilobytes, OptimizerMemoryInKilobytes, TotalServerMemoryInKilobytes, FreeCachePagesInKilobytes, CachePagesInKilobytes, MaxConnections, PhysicalMemoryInKilobytes, ProcessorCount, ProcessorsUsed, DistributionLatencyInSeconds 
		from ServerStatistics with (nolock),@alerttimes a 
		where SQLServerID = @serverid and UTCCollectionDateTime between DATEADD(mi, -@preoffset, DATEADD(mi, -@offset, a.tstamp)) and a.tstamp
		order by UTCCollectionDateTime desc
END