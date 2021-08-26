--------------------------------------------------------------------------------
--  Batch: Sessions Summary 2000
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
	 case 
		when rsc_type = 2 then 'database'
		when rsc_type = 3 then 'file'
		when rsc_type = 4 then 'index'
		when rsc_type = 5 then 'table'
		when rsc_type = 6 then 'page'
		when rsc_type = 7 then 'key'
		when rsc_type = 8 then 'extent'
		when rsc_type = 9 then 'rid'
		when rsc_type = 10 then 'application'
	 end,
	case
		when req_status = 1 then 'grant'
		when req_status = 2 then 'convert'
		when req_status = 3 then 'wait'
	end,
	cast(count(convert(bigint,rsc_type)) as bigint)
from 
	master..syslockinfo
group by 
	rsc_type,
	req_status
