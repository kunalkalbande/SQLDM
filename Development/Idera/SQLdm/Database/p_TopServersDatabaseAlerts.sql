-- SQLDM 8.5:Mahesh : Added for Rest service consumption

if (object_id('p_TopServersDatabaseAlerts') is not null)
begin
drop procedure p_TopServersDatabaseAlerts
end
go
CREATE PROCEDURE [dbo].[p_TopServersDatabaseAlerts]
	@NumServers int,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@SQLServerIDs nvarchar(max) = null
AS
begin

declare @now datetime

declare @SQLServers table(
		SQLServerID int,
		InstanceName nvarchar(256),
		LastScheduledCollectionTime datetime)
		
create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]
		

if @SQLServerIDs is not null 
begin
	declare @xmlDoc int
	exec sp_xml_preparedocument @xmlDoc output, @SQLServerIDs

	insert into @SQLServers
		select ID, smss.InstanceName, LastScheduledCollectionTime
		from openxml(@xmlDoc, '//Srvr', 1) with (ID int)
			join #SecureMonitoredSQLServers smss on smss.SQLServerID = ID
			inner join MonitoredSQLServers mss (nolock) on mss.SQLServerID = smss.SQLServerID
		where mss.Active = 1
	exec sp_xml_removedocument @xmlDoc
end
else
begin
	insert into @SQLServers
		select smss.SQLServerID, smss.InstanceName, LastScheduledCollectionTime
		from #SecureMonitoredSQLServers smss 
			inner join MonitoredSQLServers mss (nolock) on mss.SQLServerID = smss.SQLServerID
		where Active = 1
end

set @now = DateAdd(second,10,GetUTCDate())

set rowcount @NumServers
select 
	ms.[SQLServerID]
	,[InstanceName]
	,a.[DatabaseName]
	,count([AlertID]) as AlertCount
	,sum(case [Severity] when 1 then 1 else 0 end) as OkCount
	,sum(case [Severity] when 2 then 1 else 0 end) as InfoCount
	,sum(case [Severity] when 4 then 1 else 0 end) as WarningCount
	,sum(case [Severity] when 8 then 1 else 0 end) as CriticalCount
from
	@SQLServers ms
	left outer join [Alerts] a (nolock)
		on [InstanceName] collate database_default = [ServerName] collate database_default
			and ms.LastScheduledCollectionTime = a.[UTCOccurrenceDateTime] 
			and	a.Active = 1
	join MetricThresholds t (nolock) on
		ms.[SQLServerID] = t.[SQLServerID] and
		a.[Metric] = t.[Metric] 	
where (t.[UTCSnoozeEnd] is null or t.[UTCSnoozeEnd] < @now)	
group by 
	ms.[SQLServerID]
	,ms.[InstanceName]
	,a.[DatabaseName]
Having a.DatabaseName is not null
order by 
	[AlertCount] DESC

set rowcount 0
end
 
