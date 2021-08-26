
if (object_id('p_GetInstanceResources') is not null)
begin
	drop procedure [p_GetInstanceResources]
end
go

-- [p_GetInstanceResources] 1,null,null,null
CREATE PROCEDURE [dbo].[p_GetInstanceResources](
	@SQLServerID int,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInMinutes int = null,
	@Count int = null
)
AS
begin

declare	@StartDate datetime 
select @StartDate= null
declare @EndDate datetime 
select @EndDate= null
declare @e int

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDate = (select max(UTCCollectionDateTime) from [ServerStatistics] where [SQLServerID] = @SQLServerID)
else
	select @EndDate = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @StartDate = @EndDate
else
	select @StartDate = dateadd(n, -@HistoryInMinutes, @EndDate)

if(@Count is not null AND @Count != 0)
	Set rowcount @Count

Select MS.SQLServerID,MS.InstanceName,SS.UTCCollectionDateTime
	, OS.ProcessorTimePercent as OSCpu
	,SS.CPUActivityPercentage as SSCpu
	,OS.PrivilegedTimePercent,OS.UserTimePercent,OS.ProcessorQueueLength
	,SS.SqlMemoryAllocatedInKilobytes
	,SS.SqlMemoryUsedInKilobytes
	,OS.OSTotalPhysicalMemoryInKilobytes - OS.OSAvailableMemoryInKilobytes AS TotalServerMemoryInKilobytes
	,SS.ProcedureCacheSizeInKilobytes,SS.ConnectionMemoryInKilobytes,SS.LockMemoryInKilobytes,SS.CommittedInKilobytes
	,SS.PageLifeExpectancy,SS.BufferCacheHitRatioPercentage,SS.ProcedureCacheHitRatioPercentage
	,DD.DiskReadsPerSecond,DD.DiskWritesPerSecond,DD.DiskTransfersPerSecond --SQLdm 9.1 (Sanjali Makkar) - Adding the parameter of Disk Transfers Per Second
	,DD.AverageDiskMillisecondsPerRead,DD.AverageDiskMillisecondsPerWrite,DD.AverageDiskMillisecondsPerTransfer --SQLdm 10.2 (Nishant Adhikari) - Adding the parameter for Average Milliseconds
	,DD.DriveName 
	,SS.FreeCachePagesInKilobytes
	,SS.FreePagesInKilobytes
	,SS.OptimizerMemoryInKilobytes
	,SS.LockMemoryInKilobytes
	,SS.ConnectionMemoryInKilobytes
	,SS.GrantedWorkspaceMemoryInKilobytes
	,ROUND(isnull(SS.CheckpointWrites,0)/(case when SS.TimeDeltaInSeconds = 0 then NULL else SS.TimeDeltaInSeconds end) , 2) CheckpointWrites
	,ROUND(isnull(SS.LazyWriterWrites,0)/(case when SS.TimeDeltaInSeconds = 0 then NULL else SS.TimeDeltaInSeconds end), 2) LazyWriterWrites
	,ROUND(isnull(SS.ReadAheadPages,0)/(case when SS.TimeDeltaInSeconds = 0 then NULL else SS.TimeDeltaInSeconds end), 2) ReadAheadPages
	,ROUND(isnull(SS.PageReads,0)/(case when SS.TimeDeltaInSeconds = 0 then NULL else SS.TimeDeltaInSeconds end), 2) PageReads
	,ROUND(isnull(SS.PageWrites,0)/(case when SS.TimeDeltaInSeconds = 0 then NULL else SS.TimeDeltaInSeconds end), 2) PageWrites	
from [ServerStatistics] SS (NOLOCK)
	 join [MonitoredSQLServers] MS (NOLOCK)
	on SS.[SQLServerID] = MS.[SQLServerID]
	and MS.Active = 1
	and MS.SQLServerID = @SQLServerID
	and SS.UTCCollectionDateTime between ISNULL(@StartDate,SS.UTCCollectionDateTime) and ISNULL(@EndDate,SS.UTCCollectionDateTime)
	left join [OSStatistics] OS (NOLOCK)
	on SS.[SQLServerID] = OS.[SQLServerID]
	and SS.[UTCCollectionDateTime] = OS.[UTCCollectionDateTime]
	and MS.SQLServerID = OS.SQLServerID
	and MS.SQLServerID is not null
	left JOIN DiskDrives DD (NOLOCK)
	on DD.SQLServerID = SS.SQLServerID
	and DD.UTCCollectionDateTime = SS.UTCCollectionDateTime
	Order by SS.UTCCollectionDateTime DESC

SELECT @e = @@error

return @e

end
 

GO


