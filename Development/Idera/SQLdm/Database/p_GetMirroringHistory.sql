if (object_id('p_GetMirroringHistory') is not null)
begin
drop procedure [p_GetMirroringHistory]
end
go

CREATE PROCEDURE [dbo].[p_GetMirroringHistory] 
	-- Add the parameters for the stored procedure here
    @problemsOnly bit = 0,
	@server varchar(255) = null,
	@fromDate DateTime = null,
	@toDate DateTime = null,
	@databaseName nvarchar(255) = null,
	@SQLServerIDs nvarchar(max) = null,
	@UTCOffset int = 0
AS
BEGIN
	declare @xmlDoc int

	if @SQLServerIDs is not null 
	Begin
		declare @SQLServers table(
				SQLServerID int) 

		-- Prepare XML document if there is one
		exec sp_xml_preparedocument @xmlDoc output, @SQLServerIDs

		insert into @SQLServers
		select
			ID 
		from openxml(@xmlDoc, '//Srvr', 1)
			with (ID int)
	end

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]	
	
create table #NumericAlertThresholds (MetricID int, SQLServerID int, Op nvarchar(16), InfoThreshold int, WarningThreshold int, CriticalThreshold int) 
create table #EnumAlertThresholds (MetricID int, SQLServerID int, InfoThreshold nvarchar(200), WarningThreshold nvarchar(200), CriticalThreshold nvarchar(200)) 

insert into #NumericAlertThresholds exec p_GetAlertThresholdsForNumerics 68
insert into #NumericAlertThresholds exec p_GetAlertThresholdsForNumerics 69
insert into #NumericAlertThresholds exec p_GetAlertThresholdsForNumerics 70
insert into #NumericAlertThresholds exec p_GetAlertThresholdsForNumerics 71
insert into #NumericAlertThresholds exec p_GetAlertThresholdsForNumerics 73
insert into #EnumAlertThresholds exec p_GetAlertThresholdsForEnums 72
insert into #EnumAlertThresholds exec p_GetAlertThresholdsForEnums 75

select ms.DatabaseID, 
'role' = case ms.role when 2 then 'Mirror' when 1 then 'Principal' else 'Unknown' end, 
dbs.DatabaseName, 
svr.InstanceName,
'mirroring_state' = 
case ms.mirroring_state 
when 0 then 'Suspended' 
when 1 then 'Disconnected'
when 2 then 'Synchronizing'
when 3 then 'Pending Failover'
when 4 then 'Synchronized'
else 'Unknown' end,
'Witness Status' = case ms.witness_status
when 0 then 'No Witness'
when 1 then 'Connected'
when 2 then 'Disconnected' end,
log_generation_rate,
unsent_log,
send_rate,
unrestored_log,
recovery_rate,
transaction_delay,
transactions_per_sec,
average_delay,
dateadd(mi, @UTCOffset, UTCCollectionDateTime) as [CollectionTime],
time_behind,
isnull(m68.InfoThreshold,9999) as m68info,
isnull(m68.WarningThreshold,9999) as m68warn,
isnull(m68.CriticalThreshold,9999) as m68crit,
isnull(m69.InfoThreshold,9999) as m69info,
isnull(m69.WarningThreshold,9999) as m69warn,
isnull(m69.CriticalThreshold,9999) as m69crit,
isnull(m70.InfoThreshold,9999) as m70info,
isnull(m70.WarningThreshold,9999) as m70warn,
isnull(m70.CriticalThreshold,9999) as m70crit,
isnull(m71.InfoThreshold,9999) as m71info,
isnull(m71.WarningThreshold,9999) as m71warn,
isnull(m71.CriticalThreshold,9999) as m71crit,
isnull(m73.InfoThreshold,9999) as m73info,
isnull(m73.WarningThreshold,9999) as m73warn,
isnull(m73.CriticalThreshold,9999) as m73crit,
isnull(m72.InfoThreshold,9999) as m72info,
isnull(m72.WarningThreshold,'') as m72warn,
isnull(m72.CriticalThreshold,'') as m72crit,
isnull(m75.InfoThreshold,9999) as m75info,
isnull(m75.WarningThreshold,'') as m75warn,
isnull(m75.CriticalThreshold,'') as m75crit,
'PreferredStatus' = case mpc.NormalConfiguration 
when 1 then case ms.role 
			when 2 then case mpc.MirrorInstanceID 
						when svr.SQLServerID then 'Normal' 
						else 'Failed Over' 
						end 
			when 1 then case mpc.PrincipalInstanceID 
						when svr.SQLServerID then 'Normal' 
						else 'Failed Over' 
						end
			end
when 0 then case ms.role 
			when 1 then case mpc.MirrorInstanceID 
						when svr.SQLServerID then 'Normal' 
						else 'Failed Over' 
						end 
			when 2 then case mpc.PrincipalInstanceID 
						when svr.SQLServerID then 'Normal' 
						else 'Failed Over' 
						end
			end
end
from MirroringStatistics ms (nolock) 
inner join SQLServerDatabaseNames dbs (nolock) on ms.DatabaseID = dbs.DatabaseID
inner join #SecureMonitoredSQLServers svr (nolock) on svr.SQLServerID = dbs.SQLServerID 
left outer join #NumericAlertThresholds m68 on m68.SQLServerID = svr.SQLServerID and m68.MetricID = 68
left outer join #NumericAlertThresholds m69 on m69.SQLServerID = svr.SQLServerID and m69.MetricID = 69
left outer join #NumericAlertThresholds m70 on m70.SQLServerID = svr.SQLServerID and m70.MetricID = 70
left outer join #NumericAlertThresholds m71 on m71.SQLServerID = svr.SQLServerID and m71.MetricID = 71
left outer join #NumericAlertThresholds m73 on m73.SQLServerID = svr.SQLServerID and m73.MetricID = 73
left outer join #EnumAlertThresholds m72 on m72.SQLServerID = svr.SQLServerID and m72.MetricID = 72
left outer join #EnumAlertThresholds m75 on m75.SQLServerID = svr.SQLServerID and m75.MetricID = 75
left outer join MirroringPreferredConfig mpc (nolock) on mpc.MirroringGuid = ms.mirroring_guid
where dbo.fn_RoundDateTime(1, UTCCollectionDateTime) >= isnull(@fromDate,'1 Jan 1900') 
and dbo.fn_RoundDateTime(1, UTCCollectionDateTime) < isnull(@toDate,'1 Jan 2200') 
and Lower(svr.InstanceName) = coalesce(Lower(@server), Lower(svr.InstanceName))
and (svr.[SQLServerID] in (select SQLServerID from @SQLServers)
		or @SQLServerIDs is NULL and svr.[SQLServerID] = svr.[SQLServerID])
and dbs.DatabaseName = isnull(@databaseName, dbs.DatabaseName)
and (@problemsOnly = 1 and 
(
unsent_log >= m68.InfoThreshold 
or unrestored_log >= m69.InfoThreshold
or DateDiff(n, time_behind, time_recorded) >= m70.InfoThreshold
or average_delay >= m71.InfoThreshold
or 
(ms.mirroring_state = 0 and charindex('Suspended', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0
 or ms.mirroring_state = 1 and charindex('Disconnected', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0
or ms.mirroring_state = 2 and charindex('Synchronizing', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0
or ms.mirroring_state = 3 and charindex('PendingFailover', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0
or ms.mirroring_state = 4 and charindex('Synchronized', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0)
or
  ((m73.CriticalThreshold = 1 or m73.WarningThreshold = 1 or m73.InfoThreshold = 1) 
    and ((mpc.NormalConfiguration = 1 and not((ms.role = 2  and mpc.MirrorInstanceID = svr.SQLServerID) or (ms.role = 1 and mpc.PrincipalInstanceID = svr.SQLServerID))) 
      or(mpc.NormalConfiguration = 0 and not((ms.role = 1  and mpc.MirrorInstanceID = svr.SQLServerID) or (ms.role = 2 and mpc.PrincipalInstanceID = svr.SQLServerID)))
  ))
or ((m75.CriticalThreshold = 1 or m75.WarningThreshold = 1 or m75.InfoThreshold = 1)	and ms.witness_status = 2)) 
or (@problemsOnly = 0))
order by local_time desc

drop table #NumericAlertThresholds
drop table #EnumAlertThresholds
if @SQLServerIDs is not null
	exec sp_xml_removedocument @xmlDoc
END
 