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
	charindex('$(',command) - charindex('$(ESCAPE',command) > 0
	

