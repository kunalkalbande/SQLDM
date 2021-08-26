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
select 'Total pages'
 ,cntr_value/8
from
 sys.dm_os_performance_counters
where
 counter_name = 'Total Server Memory (KB)'
union
select
 case
   when counter_name = 'Database pages' or counter_name = 'Database Cache Memory (KB)'
     then 'Committed Pages'
     when counter_name = 'Free Memory (KB)' then 'free pages'
   else
     counter_name
 end,
 case when counter_name = 'Free Memory (KB)' or counter_name = 'Database Cache Memory (KB)' then cntr_value/8 
 else cntr_value  
 end
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
     'Granted Workspace Memory (KB)',
	 'Database Cache Memory (KB)',
	 'Free Memory (KB)',
	 'Total Server Memory (KB)'
   )
order by 1