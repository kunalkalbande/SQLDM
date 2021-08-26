if (object_id('[p_GetServerWideStatistics]') is not null)
begin
drop procedure [p_GetServerWideStatistics]
end
go
create procedure [dbo].[p_GetServerWideStatistics]
	@XmlDocument nvarchar(max) = null,
	@IncludeActiveOnly bit = 1,
	@SQLServerID int = null,
	@StartTime datetime = null,
	@EndTime datetime = null
as
begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

declare @SQLServers table(
			SQLServerID int,
			InstanceName nvarchar(256),
			LastScheduledCollectionTime datetime,
			primary key (InstanceName)) 

declare @xmlDoc int
declare @ServerCount int
set @ServerCount = 0

if @XmlDocument is not null 
begin
	-- Prepare XML document if there is one
	exec sp_xml_preparedocument @xmlDoc output, @XmlDocument

	insert into @SQLServers
	select 
		X.[SQLServerID],MSS.[InstanceName],MSS.[LastScheduledCollectionTime]
	from MonitoredSQLServers MSS, 
		openxml(@xmlDoc, '//SQLServer', 1)
		with (SQLServerID int) X
		where X.SQLServerID = MSS.SQLServerID

	set @ServerCount = @@ROWCOUNT

	exec sp_xml_removedocument @xmlDoc
end

if @SQLServerID is not null
begin
	insert into @SQLServers 
		select [SQLServerID],[InstanceName],[LastScheduledCollectionTime] from [MonitoredSQLServers]
			where [SQLServerID] = @SQLServerID and			
				((@IncludeActiveOnly is null or @IncludeActiveOnly = 0) or ([Active] = 1))

	set @ServerCount = @ServerCount + 1
end

if (@ServerCount = 0)
begin
	insert into @SQLServers 
		select [SQLServerID],[InstanceName],[LastScheduledCollectionTime] from [MonitoredSQLServers]
			where ((@IncludeActiveOnly is null or @IncludeActiveOnly = 0) or ([Active] = 1))
end

if (@StartTime is null) and (@EndTime is null)
begin
	-- DEBUGONLY - Return most recent record if no dates specified
	select
		S.[SQLServerID]
		,[InstanceName]
		,SS.[UTCCollectionDateTime]
		,[AgentServiceStatus]
		,[BlockedProcesses]
		,[BufferCacheHitRatioPercentage]
		,[CheckpointWrites]
		,[ClientComputers]
		,[CPUActivityPercentage]
		,[CPUTimeDelta]
		,[CPUTimeRaw]
		,[DiskQueueLength]
		,[DiskTimePercent]
		,[DTCServiceStatus]
		,[FullScans]
		,[FullTextSearchStatus]
		,[IdleTimeDelta]
		,[IdleTimePercentage]
		,[IdleTimeRaw]
		,[IOActivityPercentage]
		,[IOTimeDelta]
		,[IOTimeRaw]
		,[LazyWriterWrites]
		,[LockWaits]
		,[Logins]
		,[LogFlushes]
		,[OldestOpenTransactionsInMinutes]
		,[OpenTransactions]
		,[OSAvailableMemoryInKilobytes]
		,[OSTotalPhysicalMemoryInKilobytes]  
		,[PacketErrors]
		,[PacketsReceived]
		,[PacketsSent]
		,[PageErrors]
		,[PageLifeExpectancy]
		,[PageLookups]
		,[PageReads]
		,[PagesPerSecond] 
		,[PageSplits]
		,[PageWrites]
		,[PrivilegedTimePercent]
		,[ProcedureCacheHitRatioPercentage]
		,[ProcedureCacheSizeInKilobytes]
		,[ProcedureCacheSizePercent]
		,[ProcessorQueueLength]
		,[ProcessorTimePercent]
		,[ReadAheadPages]
		,[ReplicationLatencyInSeconds]
		,[ReplicationSubscribed]
		,[ReplicationUndistributed]
		,[ReplicationUnsubscribed]
		,[ResponseTimeInMilliseconds]
		,SS.[ServerVersion]
		,[SqlCompilations]
		,([SqlMemoryAllocatedInKilobytes]/1024) as [SqlMemoryAllocatedInMegabytes]
		,([SqlMemoryUsedInKilobytes]/1024) as [SqlMemoryUsedInMegabytes]
		,[SqlRecompilations]
		,[SqlServerServiceStatus]
		,[SystemProcesses]
		,[SystemProcessesConsumingCPU]
		,[TableLockEscalations]
		,[TempDBSizeInKilobytes]
		,[TempDBSizePercent]
		,[Batches]
		,[UserProcesses]
		,[UserProcessesConsumingCPU]
		,[UserTimePercent]
		,[WorkFilesCreated]
		,[WorkTablesCreated]
		,[SQLBrowserServiceStatus]     --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --fetch the status of SQL Browser service
		,[SQLActiveDirectoryHelperServiceStatus]  --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --fetch the status of SQL Active Directory Helper service
	from
		@SQLServers S
		join [ServerStatistics] SS on S.SQLServerID = SS.SQLServerID
		left join [OSStatistics] 
			on SS.[SQLServerID] = [OSStatistics].[SQLServerID] and
			   SS.[UTCCollectionDateTime] = [OSStatistics].[UTCCollectionDateTime]
	where
		SS.[UTCCollectionDateTime] = S.[LastScheduledCollectionTime]
--		SS.[UTCCollectionDateTime] = (select max(S2.[UTCCollectionDateTime]) from [ServerStatistics] S2 where SS.[SQLServerID] = S2.[SQLServerID])
	order by 
		[InstanceName],
		SS.[UTCCollectionDateTime]
end
else
	select
		SS.[SQLServerID]
		,[InstanceName]
		,SS.[UTCCollectionDateTime]
		,[AgentServiceStatus]
		,[BlockedProcesses]
		,[BufferCacheHitRatioPercentage]
		,[CheckpointWrites]
		,[ClientComputers]
		,[CPUActivityPercentage]
		,[CPUTimeDelta]
		,[CPUTimeRaw]
		,[DiskQueueLength]
		,[DiskTimePercent]
		,[DTCServiceStatus]
		,[FullScans]
		,[FullTextSearchStatus]
		,[IdleTimeDelta]
		,[IdleTimePercentage]
		,[IdleTimeRaw]
		,[IOActivityPercentage]
		,[IOTimeDelta]
		,[IOTimeRaw]
		,[LazyWriterWrites]
		,[LockWaits]
		,[Logins]
		,[LogFlushes]
		,[OldestOpenTransactionsInMinutes]
		,[OpenTransactions]
		,[OSAvailableMemoryInKilobytes]
		,[OSTotalPhysicalMemoryInKilobytes]  
		,[PacketErrors]
		,[PacketsReceived]
		,[PacketsSent]
		,[PageErrors]
		,[PageLifeExpectancy]
		,[PageLookups]
		,[PageReads]
		,[PagesPerSecond] 
		,[PageSplits]
		,[PageWrites]
		,[PrivilegedTimePercent]
		,[ProcedureCacheHitRatioPercentage]
		,[ProcedureCacheSizeInKilobytes]
		,[ProcedureCacheSizePercent]
		,[ProcessorQueueLength]
		,[ProcessorTimePercent]
		,[ReadAheadPages]
		,[ReplicationLatencyInSeconds]
		,[ReplicationSubscribed]
		,[ReplicationUndistributed]
		,[ReplicationUnsubscribed]
		,[ResponseTimeInMilliseconds]
		,SS.[ServerVersion]
		,[SqlCompilations]
		,([SqlMemoryAllocatedInKilobytes]/1024) as [SqlMemoryAllocatedInMegabytes]
		,([SqlMemoryUsedInKilobytes]/1024) as [SqlMemoryUsedInMegabytes]
		,[SqlRecompilations]
		,[SqlServerServiceStatus]
		,[SystemProcesses]
		,[SystemProcessesConsumingCPU]
		,[TableLockEscalations]
		,[TempDBSizeInKilobytes]
		,[TempDBSizePercent]
		,[Batches]
		,[UserProcesses]
		,[UserProcessesConsumingCPU]
		,[UserTimePercent]
		,[WorkFilesCreated]
		,[WorkTablesCreated]
		,[SQLBrowserServiceStatus]     --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --fetch the status of SQL Browser service
		,[SQLActiveDirectoryHelperServiceStatus]  --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --fetch the status of SQL Active Directory Helper service
	from
		@SQLServers S
		join [ServerStatistics] SS on S.SQLServerID = SS.SQLServerID
		left join [OSStatistics] 
			on SS.[SQLServerID] = [OSStatistics].[SQLServerID] and
			   SS.[UTCCollectionDateTime] = [OSStatistics].[UTCCollectionDateTime]
	where
		SS.[UTCCollectionDateTime] <= coalesce(@EndTime,SS.[UTCCollectionDateTime]) and
		SS.[UTCCollectionDateTime] >= coalesce(@StartTime,SS.[UTCCollectionDateTime])
	order by 
		[InstanceName],
		SS.[UTCCollectionDateTime]

end

