-- Created by Aditya Shukla for SQLdm 8.6
-- This procedure is used to fetch data for the Query Waits section in Top servers report

--exec p_TopServersQueryWaits 5, '2013-7-24 00:00:00', '2014-7-24 10:00:00', 
--'<Srvrs><Srvr ID="1"/><Srvr ID="2"/><Srvr ID="3"/><Srvr ID="4"/><Srvr ID="5"/><Srvr ID="6"/><Srvr ID="7"/><Srvr ID="8"/><Srvr ID="9"/><Srvr ID="10"/><Srvr ID="11"/><Srvr ID="12"/><Srvr ID="13"/></Srvrs>',
--2000

if (object_id('p_TopServersQueryWaits') is not null)
begin
drop procedure [p_TopServersQueryWaits]
end
go

create procedure [dbo].[p_TopServersQueryWaits]
	@NumServers int,
	@UTCStart DateTime,
	@UTCEnd DateTime,
	@SQLServerIDs nvarchar(max) = null,
	@threshold bigint = 0 -- a value in milliseconds
as
begin

declare @SQLServers table(
		SQLServerID int,
		InstanceName nvarchar(256))

create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]

--Figuring out the SQL servers that are required as per the front end filter
if @SQLServerIDs is not null 
begin
	declare @xmlDoc int
	-- Parsing the xml for servers
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
		from #SecureMonitoredSQLServers smss 
			inner join MonitoredSQLServers mss (nolock) on mss.SQLServerID = smss.SQLServerID
		where Active = 1
end

--Assigning row numbers to rows of sets partitioned by sql server and ordered by wait desc 
select aws.SQLServerID,
	ms.InstanceName, 
	aws.WaitTypeID, 
	aws.LoginNameID, 
	aws.WaitDuration, 
	aws.WaitDuration - @threshold as [difference], 
	aws.StatementUTCStartTime,
	row_number() over (partition by aws.[SQLServerID] order by aws.WaitDuration desc) as rankNumber 
into #RankedData
from @SQLServers ms inner join ActiveWaitStatistics aws (nolock)
	on ms.SQLServerID = aws.SQLServerID
where aws.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
and aws.WaitDuration > @threshold

--This ensures that only @TopN rows are selected
set rowcount @NumServers

--Selecting the top row from each of the partitioned sets while the row count is less than @TopN
select 
	rd.SQLServerID [SQLServerID],
	rd.InstanceName [InstanceName], 
	wt.WaitType [WaitType],
	rd.StatementUTCStartTime [StatementUTCStartTime],
	rd.WaitDuration [WaitDuration],
	rd.[difference] [ExceededThresholdBy],
	ln.LoginName [LoginName]
from #RankedData rd left join LoginNames ln (nolock) 
on rd.LoginNameID = ln.LoginNameID
left join WaitTypes wt (nolock)
on wt.WaitTypeID = rd.WaitTypeID
where rankNumber = 1
order by rd.WaitDuration

set rowcount 0

end
GO


