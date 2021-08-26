if (object_id('[p_GetServerStatistics]') is not null)
begin
drop procedure [p_GetServerStatistics]
end
go
create procedure [dbo].[p_GetServerStatistics]
	@XmlDocument nText = null,
	@IncludeActiveOnly bit = 1,
	@SQLServerID int = null,
	@StartTime datetime = null,
	@EndTime datetime = null
as
begin


declare @SQLServers table(
		SQLServerID int) 

declare @xmlDoc int

if @XmlDocument is not null 
begin
	-- Prepare XML document if there is one
	exec sp_xml_preparedocument @xmlDoc output, @XmlDocument

	insert into @SQLServers
	select
		SQLServerID 
	from openxml(@xmlDoc, '//SQLServer', 1)
		with (SQLServerID int)
end

if @SQLServerID is not null
	insert into @SQLServers values(@SQLServerID)


if (@StartTime is null) and (@EndTime is null)
begin
	-- DEBUGONLY - Return most recent record if no dates specified
	select
		s1.[SQLServerID]
		,[InstanceName]
		,[UTCCollectionDateTime]
		,[AgentServiceStatus]
		,[BlockedProcesses]
		,[BufferCacheHitRatioPercentage]
		,[CheckpointWrites]
		,[ClientComputers]
		,[CPUActivityPercentage]
		,[CPUTimeDelta]
		,[CPUTimeRaw]
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
		,[PacketErrors]
		,[PacketsReceived]
		,[PacketsSent]
		,[PageErrors]
		,[PageLifeExpectancy]
		,[PageLookups]
		,[PageReads]
		,[PageSplits]
		,[PageWrites]
		,[ProcedureCacheHitRatioPercentage]
		,[ProcedureCacheSizeInKilobytes]
		,[ProcedureCacheSizePercent]
		,[ReadAheadPages]
		,[ReplicationLatencyInSeconds]
		,[ReplicationSubscribed]
		,[ReplicationUndistributed]
		,[ReplicationUnsubscribed]
		,[ResponseTimeInMilliseconds]
		,s1.[ServerVersion]
		,[SqlCompilations]
		,[SqlMemoryAllocatedInKilobytes]
		,[SqlMemoryUsedInKilobytes]
		,[SqlRecompilations]
		,[SqlServerServiceStatus]
		,[SystemProcesses]
		,[SystemProcessesConsumingCPU]
		,[TableLockEscalations]
		,[TempDBSizeInKilobytes]
		,[TempDBSizePercent]
		,[Transactions]
		,[UserProcesses]
		,[UserProcessesConsumingCPU]
		,[WorkFilesCreated]
		,[WorkTablesCreated]
	from
		[ServerStatistics] s1
		left join [MonitoredSQLServers] m
		on m.[SQLServerID] = s1.[SQLServerID]
	where
		(s1.[SQLServerID] in (select SQLServerID from @SQLServers)
		or (@XmlDocument is null and s1.[SQLServerID] = coalesce(@SQLServerID,s1.[SQLServerID]) ))
		and m.[Active] = case when @IncludeActiveOnly = 1 then 1 else m.[Active] end 
		and [UTCCollectionDateTime] = (select max([UTCCollectionDateTime]) from [ServerStatistics] s2 where s1.[SQLServerID] = s2.[SQLServerID])
	order by 
		[InstanceName],
		[UTCCollectionDateTime]
end
else
	select
		[ServerStatistics].[SQLServerID]
		,[InstanceName]
		,[UTCCollectionDateTime]
		,[AgentServiceStatus]
		,[BlockedProcesses]
		,[BufferCacheHitRatioPercentage]
		,[CheckpointWrites]
		,[ClientComputers]
		,[CPUActivityPercentage]
		,[CPUTimeDelta]
		,[CPUTimeRaw]
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
		,[PacketErrors]
		,[PacketsReceived]
		,[PacketsSent]
		,[PageErrors]
		,[PageLifeExpectancy]
		,[PageLookups]
		,[PageReads]
		,[PageSplits]
		,[PageWrites]
		,[ProcedureCacheHitRatioPercentage]
		,[ProcedureCacheSizeInKilobytes]
		,[ProcedureCacheSizePercent]
		,[ReadAheadPages]
		,[ReplicationLatencyInSeconds]
		,[ReplicationSubscribed]
		,[ReplicationUndistributed]
		,[ReplicationUnsubscribed]
		,[ResponseTimeInMilliseconds]
		,[ServerStatistics].[ServerVersion]
		,[SqlCompilations]
		,[SqlMemoryAllocatedInKilobytes]
		,[SqlMemoryUsedInKilobytes]
		,[SqlRecompilations]
		,[SqlServerServiceStatus]
		,[SystemProcesses]
		,[SystemProcessesConsumingCPU]
		,[TableLockEscalations]
		,[TempDBSizeInKilobytes]
		,[TempDBSizePercent]
		,[Transactions]
		,[UserProcesses]
		,[UserProcessesConsumingCPU]
		,[WorkFilesCreated]
		,[WorkTablesCreated]
	from
		[ServerStatistics]
		left join [MonitoredSQLServers]
		on [ServerStatistics].[SQLServerID] = [MonitoredSQLServers].[SQLServerID]
	where
		([ServerStatistics].[SQLServerID] in (select SQLServerID from @SQLServers)
		or (@XmlDocument is null and [ServerStatistics].[SQLServerID] = coalesce(@SQLServerID,[ServerStatistics].[SQLServerID]) ))
		and [MonitoredSQLServers].[Active] = case when @IncludeActiveOnly = 1 then 1 else [MonitoredSQLServers].[Active] end
		and [UTCCollectionDateTime] <= coalesce(@EndTime,[UTCCollectionDateTime])
		and [UTCCollectionDateTime] >= coalesce(@StartTime,[UTCCollectionDateTime])
	order by 
		[InstanceName],
		[UTCCollectionDateTime]

if @XmlDocument is not null
	exec sp_xml_removedocument @xmlDoc

end
