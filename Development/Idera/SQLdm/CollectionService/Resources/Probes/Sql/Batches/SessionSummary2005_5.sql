---------------------------------------
-- Azure SQL Database Managed Instance
---------------------------------------

--------------------------------------------------------------------------------
--  Batch: Sessions Summary 2005
--  Tables: sysperfinfo, syslockinfo
--  Variables: [0] - Session Count Segment
--	[1] - Lock Counter Statistics Segment
--------------------------------------------------------------------------------

{0}

--START (RRG): Get internal for Instance Name when in Azure Platform
declare @sysperfinfoname sysname
Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
		
--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)


{1}

--------------------------------------------------------------------------------
--  QUERY: Outstanding Locks
--  Tables: master..syslockinfo
--  Returns: 
--    Lock type
--    Lock Status
--    Number of locks
--  Join on: none
--------------------------------------------------------------------------------
select 
      lower(resource_type),
      lower(request_status),
      cast(count(resource_type) as bigint)
from 
      master.sys.dm_tran_locks  --(RRG): THIS MAY WORK IN MANAGED INSTANCE, if not use line below
      --sys.dm_tran_locks
group by 
      resource_type, 
	  request_status