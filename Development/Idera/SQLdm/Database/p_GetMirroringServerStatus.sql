IF (object_id('p_GetMirroringServerStatus') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetMirroringServerStatus
END
GO

CREATE PROCEDURE [dbo].[p_GetMirroringServerStatus] 
	-- Add the parameters for the stored procedure here
	@problemsOnly bit,
	@SQLServerIDs nvarchar(max) = null
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

create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]	

create table #NumericAlertThresholds (MetricID int, SQLServerID int, Op nvarchar(16), InfoThreshold int, WarningThreshold int, CriticalThreshold int) 
create table #EnumAlertThresholds (MetricID int, SQLServerID int, InfoThreshold nvarchar(200), WarningThreshold nvarchar(200), CriticalThreshold nvarchar(200)) 

insert into #NumericAlertThresholds exec p_GetAlertThresholdsForNumerics 73
insert into #EnumAlertThresholds exec p_GetAlertThresholdsForEnums 72
insert into #EnumAlertThresholds exec p_GetAlertThresholdsForEnums 75

select mss.InstanceName, 
db.DatabaseName, 
'mirroring_state' = case mp.mirroring_state 
when 0 then 'Suspended' 
when 1 then 'Disconnected'
when 2 then 'Synchronizing'
when 3 then 'Pending Failover'
when 4 then 'Synchronized'
else 'Unknown' end, 
'role' = case mp.role 
when 1 then 'Principal' 
when 2 then 'Mirror' end,
mp.witness_address,
'witness_status' = case mp.witness_status 
when 0 then 'No Witness' 
when 1 then 'Connected' 
when 2 then 'Disconnected' end,
'Operational Status' = case mpc.NormalConfiguration 
when 0 then 'Failed Over' 
when 1 then 'Normal' 
else 'Not set' end,
'Safety Level' = case mp.safety_level 
when 0 then 'Unknown'
when 1 then 'Off'
when 2 then 'Full' end,
m73.InfoThreshold as m73info,
m73.WarningThreshold as m73warn,
m73.CriticalThreshold as m73crit,
isnull(m72.InfoThreshold,'') as m72info,
isnull(m72.WarningThreshold,'') as m72warn,
isnull(m72.CriticalThreshold,'') as m72crit,
isnull(m75.InfoThreshold,'') as m75info,
isnull(m75.WarningThreshold,'') as m75warn,
isnull(m75.CriticalThreshold,'') as m75crit,
mss.MaintenanceModeEnabled
from MirroringParticipants mp (nolock)
inner join SQLServerDatabaseNames db (nolock) on mp.DatabaseID = db.DatabaseID
left outer join #SecureMonitoredSQLServers smss (nolock) on db.SQLServerID = smss.SQLServerID
left outer join MonitoredSQLServers mss (nolock) on smss.SQLServerID = mss.SQLServerID
left outer join MirroringPreferredConfig mpc (nolock) on mpc.MirroringGuid = mp.mirroring_guid
left outer join #NumericAlertThresholds m73 on m73.SQLServerID = mss.SQLServerID and m73.MetricID = 73
left outer join #EnumAlertThresholds m72 on m72.SQLServerID = mss.SQLServerID and m72.MetricID = 72
left outer join #EnumAlertThresholds m75 on m75.SQLServerID = mss.SQLServerID and m75.MetricID = 75 
where (mss.[SQLServerID] in (select SQLServerID from @SQLServers)
		or (@SQLServerIDs is NULL and mss.[SQLServerID] = mss.[SQLServerID]))
and (@problemsOnly = 1 and 
(
(mp.mirroring_state = 0 and charindex('Suspended', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0
 or mp.mirroring_state = 1 and charindex('Disconnected', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0
or mp.mirroring_state = 2 and charindex('Synchronizing', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0
or mp.mirroring_state = 3 and charindex('PendingFailover', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0
or mp.mirroring_state = 4 and charindex('Synchronized', m72.InfoThreshold + m72.WarningThreshold + m72.CriticalThreshold) > 0)
)
or ((m73.CriticalThreshold = 1 or m73.WarningThreshold = 1 or m73.InfoThreshold = 1) 
    and ((mpc.NormalConfiguration = 1 and not((mp.role = 2  and mpc.MirrorInstanceID = mss.SQLServerID) or (mp.role = 1 and mpc.PrincipalInstanceID = mss.SQLServerID))) 
      or(mpc.NormalConfiguration = 0 and not((mp.role = 1  and mpc.MirrorInstanceID = mss.SQLServerID) or (mp.role = 2 and mpc.PrincipalInstanceID = mss.SQLServerID)))
  ))
or ((m75.CriticalThreshold = 1 or m75.WarningThreshold = 1 or m75.WarningThreshold = 1) and mp.witness_status = 2)
	or (@problemsOnly = 0))
and mss.Active = 1

order by db.DatabaseName, mss.InstanceName

drop table #EnumAlertThresholds
drop table #NumericAlertThresholds
if @SQLServerIDs is not null
	exec sp_xml_removedocument @xmlDoc
END
 
