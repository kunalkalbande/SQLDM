set nocount on;


Declare @sysperfinfoname sysname

Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
If @sysperfinfoname Is Null
       Set @sysperfinfoname = N'sqlserver';
Else
       Set @sysperfinfoname = N'mssql$' + lower(@sysperfinfoname);


select 'Cache Pages', sum(convert(dec(20,0),size_in_bytes))/1024
from master.sys.dm_exec_cached_plans
union
select
 case
   when counter_name = 'Database pages'
     then 'Committed Pages'
   else
     counter_name
 end,
 cntr_value
from
	sys.dm_os_performance_counters
where
 counter_name in
   (
     'Procedure cache pages',
     'Total Server Memory (KB)',
     'Connection Memory (KB)',
     'Lock Memory (KB)',
     'Optimizer Memory (KB)',
     'Granted Workspace Memory (KB)'
   )
 or
   (
     counter_name = 'Database pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
 or
   (
     counter_name = 'Free pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
or
   (
     counter_name = 'Total pages'
     and lower(object_name) = lower(@sysperfinfoname + ':Buffer Manager')
   )
order by 1