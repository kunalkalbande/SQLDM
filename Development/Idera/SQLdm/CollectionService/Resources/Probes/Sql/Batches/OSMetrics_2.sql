-- SQL Diagnostic Manager v11.0.0.0
-- Copyright © IDERA, Inc. 2003-2020 

set transaction isolation level read uncommitted 
 set lock_timeout 20000 
 set implicit_transactions off 
 if @@trancount > 0 commit transaction 
 set language us_english 
 set cursor_close_on_commit off 
 set query_governor_cost_limit 0 
 set numeric_roundabort off
--  Batch: OS Metrics
--  Variables:
--	[0] - sp_OACreate context
--	[1] - Disable OLE
--  [2] - Waitfor time
--  [3] - Direct WMI
--------------------------------------------------------------------------------

declare @curconfig int, @disableole int, @directwmi int
declare @MachineName varchar(255) 

select @MachineName = cast(serverproperty('MachineName') as varchar(255)) 

select @directwmi=0
if (@directwmi = 1)
begin
	select @MachineName, cast(serverproperty('servername')  as nvarchar(255)), getutcdate() as UTCTimeStamp
end
else
begin
	select @disableole = 1
	if (@disableole = 0)
	begin
		select 'service unavailable' 
	end 
	else 
	begin
		if @disableole = 1
			select 'ole automation disabled'
		else
			select 'procedure unavailable' 
	end
end


declare @sysperfinfoname_trans sysname
Set @sysperfinfoname_trans = Convert(nvarchar(128), ServerProperty('InstanceName'))
		
--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
BEGIN
select @sysperfinfoname_trans = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname_trans = SUBSTRING(@sysperfinfoname_trans, 1, PATINDEX('%:%',@sysperfinfoname_trans)-1)

select 
	'total pages', 
	isnull(sum(convert(bigint,cntr_value/8)),0)
from 
	sys.dm_os_performance_counters 
where (Lower(object_name) = Lower(@sysperfinfoname_trans + ':memory manager') 
	and Lower(counter_name) = 'total server memory (kb)' )
union
select 
	lower(RTRIM(counter_name)), 
	isnull(sum(convert(bigint,cntr_value)),0)
from 
	sys.dm_os_performance_counters 
where 
	(lower(object_name) = lower(@sysperfinfoname_trans + ':SQL Statistics')
		and lower(counter_name) in ('batch requests/sec', 'sql compilations/sec', 'sql re-compilations/sec') )
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':Buffer Manager') 
		and (Lower(counter_name) like 'buffer cache hit ratio%'
		or Lower(counter_name) = 'total pages')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':plan cache') 
		and Lower(counter_name) like 'cache hit ratio%' 
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':databases') 
		and Lower(counter_name) in ( 'log flushes/sec','transactions/sec')
		and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':memory manager') 
		and Lower(counter_name) in ('target server memory (kb)', 'total server memory (kb)')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':access methods') 
		and Lower(counter_name) in ('page splits/sec','full scans/sec', 'table lock escalations/sec', 'worktables created/sec', 'workfiles created/sec')) 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':locks') 
		and Lower(counter_name) = 'lock waits/sec' and Lower(instance_name) <> '_total') 
	or (Lower(object_name) = Lower(@sysperfinfoname_trans + ':buffer manager') 
		and Lower(counter_name) in ('page reads/sec', 'page writes/sec', 'lazy writes/sec','checkpoint pages/sec', 'checkpoint writes/sec', 'readahead pages/sec', 'page lookups/sec', 'page requests/sec', 'page life expectancy')) 
group by 
	counter_name 
END
ELSE
BEGIN
	SELECT NULL, NULL
END


select NULL AS avg_cpu_percent