if (object_id('p_GetEnterpriseSummary') is not null)
begin
drop procedure [p_GetEnterpriseSummary]
end
go
create procedure [dbo].[p_GetEnterpriseSummary]
	@SQLServerIDs nvarchar(max) = null,
	@UTCOffset int
as
begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

declare @SQLServers table(
		SQLServerID int, 
		InstanceName nvarchar(256),
		LastScheduledCollectionTime datetime,
		LastDatabaseCollectionTime datetime,
		LastAlertRefreshTime datetime,
		IsMaintenanceMode bit) 

declare @xmlDoc int
declare @xml XML
set @xml = @SQLServerIDs
declare @now datetime

set @now = DateAdd(second,10,GetUTCDate())

create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]

if @SQLServerIDs is not null 
begin

	insert into @SQLServers
select doc.col.value('@ID', 'int') as ID, smss.InstanceName, LastScheduledCollectionTime, LastDatabaseCollectionTime, LastAlertRefreshTime, 0
from @xml.nodes('//Srvr') doc(col)
join MonitoredSQLServers mss (nolock) on mss.SQLServerID = doc.col.value('@ID', 'int')
inner join #SecureMonitoredSQLServers smss on mss.SQLServerID = smss.SQLServerID
where Active = 1

end
else
begin
	insert into @SQLServers
		select smss.SQLServerID, smss.InstanceName, LastScheduledCollectionTime, LastDatabaseCollectionTime, LastAlertRefreshTime, 0
		from MonitoredSQLServers mss (nolock)
			inner join #SecureMonitoredSQLServers smss on mss.SQLServerID = smss.SQLServerID
		where Active = 1
end



update @SQLServers
set IsMaintenanceMode = 1
from
@SQLServers S inner join Alerts A (NOLOCK) on A.ServerName collate database_default = S.InstanceName collate database_default and S.LastAlertRefreshTime = A.UTCOccurrenceDateTime
where A.Metric = 48 


declare @ServerStatus table (
		SQLServerID int,
		Metric int,	
		ServerStatus int)

insert into @ServerStatus
	select ms.SQLServerID
		--We only care about maintenance mode.  If the server is in maintenance mode, this will be the only active alert.
		--   Leaving out Severity = 2 because Informational Alerts should not affect server state.
		,case when IsMaintenanceMode = 1 then 48 else 0 end
		,case 
			when sum(case when Severity = 8 then 1 else 0 end) > 0 then 8
			when sum(case when Severity = 4 then 1 else 0 end) > 0 then 4
			--when sum(case when Severity = 2 then 1 else 0 end) > 0 then 2
			else 1
		end
	from @SQLServers ms 
		left outer join Alerts a (nolock) on
			ms.InstanceName collate database_default = a.ServerName collate database_default
			And (ms.LastScheduledCollectionTime = a.UTCOccurrenceDateTime
					Or ms.LastDatabaseCollectionTime = a.UTCOccurrenceDateTime)
		left join MetricThresholds t (nolock) on
			ms.[SQLServerID] = t.[SQLServerID] and
			a.[Metric] = t.[Metric] 		
	where 
		(((a.[Active] is null or a.[Active] = 1) and
		(t.[UTCSnoozeEnd] is null or t.[UTCSnoozeEnd] < @now)) or Active = 0)
		and (
		(IsMaintenanceMode = 0 
			and ((a.Metric not in (select MetricID from DBMetrics) and ms.LastScheduledCollectionTime = a.UTCOccurrenceDateTime)
			or (a.Metric  in (select MetricID from DBMetrics) and ms.LastDatabaseCollectionTime = a.UTCOccurrenceDateTime)))
		or (IsMaintenanceMode = 1 and (a.Metric = 48 and ms.LastAlertRefreshTime = a.UTCOccurrenceDateTime)))
	group by 
		ms.SQLServerID , IsMaintenanceMode
		
		

select
	ms.[SQLServerID]
	,[InstanceName]
	,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]) as CollectionDateTime
	,[AgentServiceStatus]
	,[CPUActivityPercentage]
	,[DiskTimePercent]
	,[OpenTransactions]
	,cast([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes] as float)/[OSTotalPhysicalMemoryInKilobytes] * 100 as [OSMemoryUsagePercent]  
	,[ResponseTimeInMilliseconds]
	,[ServerVersion] = dbo.fn_GetServerVersionString(s1.[ServerVersion])
	,[SqlServerServiceStatus]
	,s1.[SystemProcesses]
	,[SystemProcessesConsumingCPU]
	,[UserProcesses]
	,[UserProcessesConsumingCPU]
	,isnull(ss.ServerStatus,1) as ServerStatus
	,ss.Metric
	,s1.ProcessorCount
	,s1.ProcessorsUsed
	,WindowsVersion
	,SqlServerEdition
	,RunningSince
	,IsClustered
	,PhysicalMemoryInKilobytes
	,s1.ServerHostName
	,s1.FullTextSearchStatus
	,s1.DTCServiceStatus
	,DataFileSpaceAllocatedInKilobytes
	,DataFileSpaceUsedInKilobytes
	,LogFileSpaceAllocatedInKilobytes
	,LogFileSpaceUsedInKilobytes
	,[dbo].[fn_ServerVersionnVarcharToBigInt](s1.ServerVersion) as ServerVersionNumber
	,[SQLBrowserServiceStatus]     --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --fetch the status of SQL Browser service
	,[SQLActiveDirectoryHelperServiceStatus]     --SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --fetch the status of SQL Active Directory Helper service
from @SQLServers ms
	left outer join [ServerStatistics] s1 (nolock)
	on ms.[SQLServerID] = s1.[SQLServerID] and s1.[UTCCollectionDateTime] = ms.LastScheduledCollectionTime
	left outer join [OSStatistics] o (nolock)
	on o.[SQLServerID] = ms.[SQLServerID] and o.[UTCCollectionDateTime] = ms.LastScheduledCollectionTime
	left outer join @ServerStatus ss 
	on ms.[SQLServerID] = ss.[SQLServerID]
order by 
	[InstanceName],
	[LastScheduledCollectionTime]

end

