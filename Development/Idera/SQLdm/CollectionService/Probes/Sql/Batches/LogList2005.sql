--------------------------------------------------------------------------------
--  Batch: Log List 2005
--  Tables: #loglist, master..sysconfigures
--  Variables: none
--------------------------------------------------------------------------------
declare 
	@errorlog_file  nvarchar(255)

execute master.dbo.xp_instance_regread N'hkey_local_machine', N'software\microsoft\mssqlserver\sqlserveragent', N'errorlogfile',@errorlog_file output,N'no_output'

if (@errorlog_file is not null)
begin
	select right(@errorlog_file,charindex('\',reverse(@errorlog_file))-1)
end
else
begin
	select 'no registry entry'
end


--------------------------------------------------------------------------------
--  TEMP TABLE: Log List
--  Created Tables: tempdb..#loglist
--  Purpose: Populate lock list to return
--------------------------------------------------------------------------------
if (select isnull(object_id('tempdb..#loglist'), 0)) = 0 
	create table #loglist
		(
		ArchiveNo int, 
		CreateDate nvarchar(24), 
		Size int
		)
else 
		truncate table #loglist 

insert #loglist 
	execute sp_enumerrorlogs 2

	
--------------------------------------------------------------------------------
--  QUERY: Agent Log List
--  Tables: #loglist
--  Returns:
--    archive number
--	  last modified date
--    size on disk
--------------------------------------------------------------------------------		
select
	ArchiveNo,
	convert(datetime, CreateDate, 101) AS [CreateDate],
	Size
from
	#loglist 
order by
	[ArchiveNo] asc

truncate table #loglist

insert #loglist 
	exec master..sp_enumerrorlogs

--------------------------------------------------------------------------------
--  QUERY: SQL Log List
--  Tables: #loglist
--  Returns:
--    archive number
--	  last modified date
--    size on disk
--------------------------------------------------------------------------------	
select
	ArchiveNo,
	convert(datetime, CreateDate, 101) AS [CreateDate],
	Size
from
	#loglist 
order by
	[ArchiveNo] asc

drop table #loglist

--------------------------------------------------------------------------------
--  SP: Number of SQL Server error logs
--------------------------------------------------------------------------------
declare 
	@NumErrorLogs int	

exec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\Microsoft\MSSQLServer\MSSQLServer', N'NumErrorLogs', @NumErrorLogs OUTPUT

select cast(isnull(@NumErrorLogs, 0) as int)
