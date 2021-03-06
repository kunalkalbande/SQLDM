if (object_id('p_TopServersResponseTime') is not null)
begin
drop procedure p_TopServersResponseTime
end
go
CREATE PROCEDURE [dbo].[p_TopServersResponseTime]
	@NumServers int,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@SQLServerIDs nvarchar(max) = null
AS
BEGIN

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
			join #SecureMonitoredSQLServers smss (nolock) on SQLServerID = ID

	exec sp_xml_removedocument @xmlDoc
end
else
begin
	insert into @SQLServers
		select smss.SQLServerID, smss.InstanceName
		from #SecureMonitoredSQLServers smss 
			inner join MonitoredSQLServers mss (nolock) on mss.SQLServerID = smss.SQLServerID
		where Active = 1
end


set rowcount @NumServers

select
	ms.[SQLServerID]
	,[InstanceName]
	,avg([ResponseTimeInMilliseconds]) as [ResponseTimeInMilliseconds]
	,avg([CPUActivityPercentage]) as [CPUActivityPercentage]
	,avg([DiskTimePercent]) as [DiskTimePercent]
	,avg([OSAvailableMemoryInKilobytes]) as [OSAvailableMemoryInKilobytes]
	,avg([OSTotalPhysicalMemoryInKilobytes]) as [OSTotalPhysicalMemoryInKilobytes]
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
	[ResponseTimeInMilliseconds] DESC

set rowcount 0

end
 
