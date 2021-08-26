--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------
-- Memory2012.sql
--------------------------------------------------------------

set nocount on;

declare @cachepages table (name nvarchar(128), val decimal);

--RRG: Condition branch if current database is User or System
--     sys.dm_exec_cached_plans can not be accessed is System
if (select database_id from sys.databases where name = DB_NAME()) > 4 
	insert into @cachepages
		select 'Cache Pages', sum(convert(dec(20,0),size_in_bytes))/1024
		from sys.dm_exec_cached_plans
else
	insert into @cachepages
		select 'Cache Pages', 0

IF 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
select * from @cachepages
union
select 'Total pages', cntr_value/8
from sys.dm_os_performance_counters
where counter_name = 'Total Server Memory (KB)'
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
from sys.dm_os_performance_counters
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
ELSE
	select * from @cachepages
