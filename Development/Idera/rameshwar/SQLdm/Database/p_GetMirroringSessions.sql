IF (object_id('p_GetMirroringSessions') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetMirroringSessions
END
GO

CREATE PROCEDURE [dbo].[p_GetMirroringSessions]
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

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int, Active bit, MaintenanceModeEnabled bit)

insert into #SecureMonitoredSQLServers(InstanceName,SQLServerID)
exec [p_GetReportServers]	

update #SecureMonitoredSQLServers 
set Active = mss.Active,
MaintenanceModeEnabled = mss.MaintenanceModeEnabled
from #SecureMonitoredSQLServers smss
inner join MonitoredSQLServers mss
on smss.SQLServerID = mss.SQLServerID

-- Insert statements for procedure here
create table #EnumAlertThresholds (MetricID int, SQLServerID int, InfoThreshold nvarchar(200), WarningThreshold nvarchar(200), CriticalThreshold nvarchar(200)) 
insert into #EnumAlertThresholds exec p_GetAlertThresholdsForEnums 72
create table #NumericAlertThresholds (MetricID int, SQLServerID int, Op nvarchar(16), InfoThreshold int, WarningThreshold int, CriticalThreshold int) 
insert into #NumericAlertThresholds exec p_GetAlertThresholdsForNumerics 73

select 'Principal' = isnull(mp.InstanceName, mirrors.partner_instance),
'Mirror' = isnull(mm.InstanceName, principals.partner_instance), 
'DatabaseName' = isnull(dbs.DatabaseName, mdbs.DatabaseName),
mirrors.principal_address,
principals.Mirror_address,
'witness_address' = 
case isnull(principals.witness_address, mirrors.witness_address) 
when '' then 'No Witness' 
else isnull(principals.witness_address, mirrors.witness_address) end,
'Safety Level' = case isnull(principals.safety_level, mirrors.safety_level)
when 0 then 'Unknown'
when 1 then 'Off'
when 2 then 'Full' end,
'is_suspended' = isnull(principals.is_suspended, mirrors.is_suspended),
'mirroring_state' = 
case isnull(principals.mirroring_state, mirrors.mirroring_state)
when 0 then 'Suspended' 
when 1 then 'Disconnected'
when 2 then 'Synchronizing'
when 3 then 'Pending Failover'
when 4 then 'Synchronized'
else 'Unknown' end,
'Witness Status' = case isnull(principals.witness_status,mirrors.witness_status)
when 0 then 'No Witness'
when 1 then 'Connected'
when 2 then 'Disconnected' end,
isnull(m72p.InfoThreshold,'') as m72info,
isnull(m72p.WarningThreshold,'') as m72warn,
isnull(m72p.CriticalThreshold,'') as m72crit,
isnull(m72m.InfoThreshold,'') as m72minfo,
isnull(m72m.WarningThreshold,'') as m72mwarn,
isnull(m72m.CriticalThreshold,'') as m72mcrit,
isnull(m73.InfoThreshold,'') as m73info,
isnull(m73.WarningThreshold,'') as m73warn,
isnull(m73.CriticalThreshold,'') as m73crit,
'PrincipalMaintMode' = mp.MaintenanceModeEnabled,
'MirrorMaintMode' = mm.MaintenanceModeEnabled,
'WitnessAddress' = coalesce(principals.witness_address, mirrors.witness_address, mpc.WitnessAddress),
'Normal' = case mpc.NormalConfiguration when 1 then (mpc.MirrorInstanceID & mm.SQLServerID) | (mpc.PrincipalInstanceID & mp.SQLServerID) when 0 then (mpc.MirrorInstanceID & mp.SQLServerID) | (mpc.PrincipalInstanceID & mm.SQLServerID) end
from MirroringParticipants principals (nolock)
inner join #SecureMonitoredSQLServers mp (nolock) on principals.principal_instanceID = mp.SQLServerID and principals.role = 1
inner join SQLServerDatabaseNames dbs (nolock) on principals.DatabaseID = dbs.DatabaseID
full outer join MirroringParticipants mirrors (nolock) on principals.mirroring_guid = mirrors.mirroring_guid and mirrors.role = 2
left outer join #SecureMonitoredSQLServers mm (nolock) on mirrors.mirror_instanceID = mm.SQLServerID
left outer join SQLServerDatabaseNames mdbs (nolock) on mirrors.DatabaseID = mdbs.DatabaseID
left outer join #EnumAlertThresholds m72p on m72p.SQLServerID = mp.SQLServerID and m72p.MetricID = 72
left outer join #EnumAlertThresholds m72m on m72m.SQLServerID = mm.SQLServerID and m72m.MetricID = 72
left outer join #NumericAlertThresholds m73 on m73.SQLServerID = mp.SQLServerID and m73.MetricID = 73
left outer join MirroringPreferredConfig mpc on mpc.MirroringGuid = principals.mirroring_guid
where (principals.principal_instanceID is not null or mirrors.mirror_instanceID is not null)
and (

	  (@problemsOnly = 1 and 
	  (
	    (
		  principals.mirroring_state = 0 and charindex('Suspended', m72p.InfoThreshold + m72p.WarningThreshold + m72p.CriticalThreshold) > 0
	      or principals.mirroring_state = 1 and charindex('Disconnected', m72p.InfoThreshold + m72p.WarningThreshold + m72p.CriticalThreshold) > 0
	      or principals.mirroring_state = 2 and charindex('Synchronizing', m72p.InfoThreshold + m72p.WarningThreshold + m72p.CriticalThreshold) > 0
	      or principals.mirroring_state = 3 and charindex('PendingFailover', m72p.InfoThreshold + m72p.WarningThreshold + m72p.CriticalThreshold) > 0
	      or principals.mirroring_state = 4 and charindex('Synchronized', m72p.InfoThreshold + m72p.WarningThreshold + m72p.CriticalThreshold) > 0
		) 
		or
		 ((m73.CriticalThreshold = 1 or m73.WarningThreshold = 1 or m73.InfoThreshold = 1) 
			and (
					(
						mpc.NormalConfiguration = 1 and not((mpc.MirrorInstanceID = mm.SQLServerID) or (mpc.PrincipalInstanceID = mp.SQLServerID))) 
						or(mpc.NormalConfiguration = 0 and not((mpc.MirrorInstanceID = mp.SQLServerID) or (mpc.PrincipalInstanceID = mm.SQLServerID)))
						or(mpc.WitnessAddress <> coalesce(principals.witness_address, mirrors.witness_address, mpc.WitnessAddress)
					)
		  ))
	 )
)
		or @problemsOnly = 0
    )
and ((mp.[SQLServerID] in (select SQLServerID from @SQLServers)
		or @SQLServerIDs is NULL and mp.[SQLServerID] = mp.[SQLServerID])
or (mm.[SQLServerID] in (select SQLServerID from @SQLServers)
		or @SQLServerIDs is NULL and mm.[SQLServerID] = mm.[SQLServerID]))
and (mp.Active = 1 or mm.Active = 1)
drop table #EnumAlertThresholds 
drop table #NumericAlertThresholds
if @SQLServerIDs is not null
	exec sp_xml_removedocument @xmlDoc
END