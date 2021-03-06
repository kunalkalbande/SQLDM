if (object_id('p_TopServersMemory') is not null)
begin
drop procedure p_TopServersMemory
end
go
CREATE PROCEDURE [dbo].[p_TopServersMemory]
	@NumServers int,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@SQLServerIDs nvarchar(max) = null
AS
begin
declare @SQLServers table(
		SQLServerID int,
		InstanceName nvarchar(256))

create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]

if @SQLServerIDs is not null 
begin
	declare @xmlDoc int
	exec sp_xml_preparedocument @xmlDoc output, @SQLServerIDs

	insert into @SQLServers
		select ID, smss.InstanceName
		from openxml(@xmlDoc, '//Srvr', 1) with (ID int)
			join MonitoredSQLServers mss (nolock) on mss.SQLServerID = ID
			inner join #SecureMonitoredSQLServers smss on mss.SQLServerID = smss.SQLServerID

	exec sp_xml_removedocument @xmlDoc
end
else
begin
	insert into @SQLServers
		select smss.SQLServerID, smss.InstanceName
		from MonitoredSQLServers mss (nolock)
			inner join #SecureMonitoredSQLServers smss on mss.SQLServerID = smss.SQLServerID
		where Active = 1
end


set rowcount @NumServers

select
	ms.[SQLServerID]
	,[InstanceName]
	,avg([ResponseTimeInMilliseconds]) as [ResponseTimeInMilliseconds]
	,avg([CPUActivityPercentage]) as [CPUActivityPercentage]
	,avg([DiskTimePercent]) as [DiskTimePercent]
	,avg(cast([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes] as float)/[OSTotalPhysicalMemoryInKilobytes] * 100) as [OSMemoryUsagePercent]
from
	@SQLServers ms
	left outer join [ServerStatistics] s1 (nolock)
	on ms.[SQLServerID] = s1.[SQLServerID]
	left join [OSStatistics] o (nolock)
	on o.[SQLServerID] = ms.[SQLServerID] and o.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime]
where
	s1.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
	and
	o.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
group by
	ms.[SQLServerID], [InstanceName]
order by
	[OSMemoryUsagePercent] DESC

set rowcount 0

end
 
