--------------------------------------------------------------------------------
--  Batch: Sessions Summary 2005
--  Tables: sysperfinfo, syslockinfo
--  Variables: [0] - Session Count Segment
--	[1] - Lock Counter Statistics Segment
--------------------------------------------------------------------------------

{0}

declare 
@servername varchar(255), 
@sysperfinfoname varchar(255),
@slashpos int

select @servername = cast(serverproperty('servername') as nvarchar(255))

select @servername = upper(@servername) 

select @slashpos = charindex('\', @servername)  

if @slashpos <> 0 
	begin 
		select @sysperfinfoname = 'MSSQL$' + substring(@servername, @slashpos + 1, 30)
	end  
else 
	begin 
		select @sysperfinfoname = 'SQLSERVER'
	end

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
      cast(count(convert(bigint,resource_type)) as bigint)
from 
      master.sys.dm_tran_locks
group by 
      resource_type, 
	  request_status
