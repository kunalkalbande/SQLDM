--------------------------------------------------------------------------------
--  Batch: Log List 2000
--  Tables: none
--  Variables: none
--------------------------------------------------------------------------------
declare 
	@errorlog_path  nvarchar(255),
	@errorlog_file  nvarchar(255),
	@errorlog_prefix nvarchar(255),
	@archive_files nvarchar(255)

if (select isnull(object_id('tempdb..#loglist'), 0)) = 0 
	create table #loglist
		(
		ArchiveNo int, 
		CreateDate nvarchar(24), 
		Size int,
		ArchiveName nvarchar(1000)
		)
else 
		truncate table #loglist

if (select isnull(object_id('tempdb..#directoryList'), 0)) = 0 
	create table #directoryList
		(
			filename nvarchar(255), 
			depth int, 
			isfile int
		)
else 
		truncate table #directoryList

if (select isnull(object_id('tempdb..#fileDetails'), 0)) = 0 
	create table #fileDetails
	(
		AltName nvarchar(32) NULL, 
		Size int, 
		CreationDate int, 
		CreationTime int, 
		LastWrittenDate int, 
		LastWrittenTime int,  
		LastAccessedDate int, 
		LastAccessedTime int, 
		Attributes int)
else
	truncate table #fileDetails

set nocount on

execute master.dbo.xp_instance_regread N'hkey_local_machine', N'software\microsoft\mssqlserver\sqlserveragent', N'errorlogfile',@errorlog_file output,N'no_output'

select @errorlog_file = isnull(@errorlog_file,'no registry entry')

if (@errorlog_file <> 'no registry entry')
begin
	select @errorlog_path = left(@errorlog_file,len(@errorlog_file) - charindex('\',reverse(@errorlog_file)))  + '\'
	select @errorlog_prefix = left(@errorlog_file,len(@errorlog_file) - charindex('.',reverse(@errorlog_file)))
	select @errorlog_prefix = right(@errorlog_prefix,charindex('\',reverse(@errorlog_prefix))-1)
	select @errorlog_prefix
end
else
begin 
	select @errorlog_file
end

if (@errorlog_file <> 'no registry entry')
begin
	insert into #directoryList
		execute master..xp_dirtree @errorlog_path, 1, 1

	declare @agentfilename_loop nvarchar(255),
			@agentfilepath_loop nvarchar(255),
			@agentfileextension_loop int,
			@loopexit int

	select	@agentfilename_loop = null,
			@agentfileextension_loop = 0
			
		-- Find maximum archive file for loop
		select 
			@loopexit = max(cast(right(filename,charindex('.',reverse(filename))-1) as int))
		from 
			#directoryList 
		where 
			filename like @errorlog_prefix + '.[0-9]%'

		-- Loop through agent logs
		while @agentfileextension_loop < = isnull(@loopexit,0)
		begin
			select 
				@agentfilename_loop =  filename
			from #directoryList 
			where 
				(filename like @errorlog_prefix + '.[0-9]%' 
				and cast(right(filename,charindex('.',reverse(filename))-1) as int) = @agentfileextension_loop)
				or
				(filename like @errorlog_prefix + '.%'
				and @agentfileextension_loop = 0)
			order by 
				case when @agentfileextension_loop = 0 then -1 else cast(right(filename,charindex('.',reverse(filename))-1) as int) end

			-- Set up agent file path to pass to xp_getfiledetails
			set @agentfilepath_loop = @errorlog_path +@agentfilename_loop
	
			-- Look up file details
			if isnull(@agentfilename_loop,'') <> ''
				insert #fileDetails 
					execute master.dbo.xp_getfiledetails @agentfilepath_loop

			insert #loglist
			select
				@agentfileextension_loop,
				convert(datetime, stuff(stuff(str(LastWrittenDate, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(LastWrittenTime,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121),
				Size,
				@agentfilepath_loop
			from
				#fileDetails

			select @agentfileextension_loop = @agentfileextension_loop + 1
	
			set @agentfilename_loop = null			
			truncate table #fileDetails	
		end
		drop table #directoryList
		drop table #fileDetails
end

select
	ArchiveNo,
	convert(datetime, CreateDate, 101) AS [CreateDate],
	Size,
	ArchiveName
from
	#loglist 
order by
	[ArchiveNo] asc
	
drop table #loglist

if (select isnull(object_id('tempdb..#loglist'), 0)) = 0 
	create table #sqlloglist
		(
		ArchiveNo int, 
		CreateDate nvarchar(24), 
		Size int
		)
else 
		truncate table #sqlloglist 
		
--------------------------------------------------------------------------------
--  TEMP TABLE: Log List
--  Created Tables: tempdb..#loglist
--  Purpose: Populate lock list to return
--------------------------------------------------------------------------------
insert #sqlloglist 
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
	#sqlloglist 
order by
	[ArchiveNo] asc

drop table #sqlloglist

--------------------------------------------------------------------------------
--  SP: Number of SQL Server error logs
--------------------------------------------------------------------------------
declare 
	@NumErrorLogs int	

exec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'Software\Microsoft\MSSQLServer\MSSQLServer', N'NumErrorLogs', @NumErrorLogs OUTPUT

select cast(isnull(@NumErrorLogs, 0) as int)
