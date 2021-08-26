--------------------------------------------------------------------------------
--  Batch: Sessions Summary 2005
--  Tables: sysperfinfo, syslockinfo
--  Variables: [0] - Session Count Segment
--	[1] - Lock Counter Statistics Segment
--------------------------------------------------------------------------------

{0}

Declare @sysperfinfoname sysname

Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
If @sysperfinfoname Is Null
       Set @sysperfinfoname = N'sqlserver';
Else
       Set @sysperfinfoname = N'mssql$' + lower(@sysperfinfoname);


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
BEGIN TRY
select 
      lower(resource_type),
      lower(request_status),
      cast(count(resource_type) as bigint)
from 
      master.sys.dm_tran_locks
group by 
      resource_type, 
	  request_status
END TRY
BEGIN CATCH
	select top 0 NULL, NULL, NULL	  
	print Error_Message()
END CATCH