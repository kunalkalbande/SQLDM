use msdb;
select 
	name,
	value,
	value_in_use,
	is_dynamic
from 
	master.sys.configurations 
where 
	(is_dynamic = 0 and value != value_in_use)
	or configuration_id in 
	(
	106 -- locks
	,114 -- disallow results from triggers
	,503 -- max worker threads
	,505 -- network packet size (B)
	,1541 -- query wait (s)
	,1562 -- clr enabled
	,1563 -- max full-text crawl range
	,1568 -- default trace enabled
	,1569 -- blocked process threshold (s)
	,1582 -- access check cache bucket count
	,1583 -- access check cache quota
	,16391 -- Ad Hoc Distributed Queries
	)
union all
select distinct
	name = 'McAfeeBufferOverflow',
	value = 1,
	value_in_use = 1,
	is_dynamic = 1
from 
sys.dm_os_loaded_modules
where lower(name) like '%entapi.dll%' 

IF IS_SRVROLEMEMBER('sysadmin') = 1
begin
select 
	name = 'IsIntegratedSecurityOnly',
	value = serverProperty('IsIntegratedSecurityOnly')
union all
select
	name = 'BuiltinAdministratorIsSysadmin',
	value = count(1)
from
	master.dbo.syslogins 
where
	sysadmin = 1
	and lower(name) = 'builtin\administrators'
union all
select
	name = 'PublicEnabledSqlAgentProxyAccount',
	value = count(1)
from
	msdb.dbo.sysproxylogin p
	left join  sys.database_principals d
	on p.sid = d.sid
where
	d.principal_id = user_id('public')
end
else
begin
	select 
			name = 'Unable to access',
			value = 0
end

select
	username = name, policy = is_policy_checked, expire = is_expiration_checked
from 
	master.sys.sql_logins 
where
	is_disabled = 0 and
	(is_policy_checked = 0 
	or is_expiration_checked = 0)

select
	j.name,
	s.step_name
from
	msdb.dbo.sysjobs j
	inner join msdb.dbo.sysjobsteps s
	on j.job_id = s.job_id
where
	charindex('$(',command) - charindex('$(ESCAPE',command) > 0;


	--// SQLDm 10.0 -Srishti Purohit-  New Recommendations (SDR-Q37,SDR-Q38)
dbcc TRACESTATUS(4199);
select name, compatibility_level from sys.databases;

     --// SQLDm 10.0 -Srishti Purohit -  New Recommendations (SDR-M31,SDR-M32)
select value_in_use from sys.configurations where name ='max server memory (MB)';
select path, current_size_in_kb from sys.dm_os_buffer_pool_extension_configuration;

--// SQLDm 10.0 -Srishti Purohit -  New Recommendations (SDR-D23)
select serverproperty('Edition') as Edition;
select is_enabled from sys.resource_governor_configuration;
select drgrp.name from sys.dm_resource_governor_resource_pools drgrp
inner join sys.resource_governor_resource_pools rgrp 
on drgrp.name = rgrp.name
where (rgrp.max_iops_per_volume = 0) and rgrp.name not in ('internal','default') and
(drgrp.read_io_stall_total_ms > 0 or drgrp.write_io_stall_total_ms > 0);

--// SQLDm 10.0 -Srishti Purohit-  New Recommendations (SDR-Q45)
dbcc TRACESTATUS(2312);
dbcc TRACESTATUS(9481);