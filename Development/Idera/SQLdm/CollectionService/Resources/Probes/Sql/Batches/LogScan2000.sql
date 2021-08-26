--------------------------------------------------------------------------------
--  Batch: Log Scan 2000
--	Variables:	
--	[0] - Rowcount max to return (flood control)
--	[1] - Enumeration to return for Agent Logs
--	[2] - Enumeration to return for SQL Logs
--	[3] - Skip error log (1 if true)
--	[4] - Skip agent log (1 if true)
--  [5] - Error File Size Limit
--  [6] - Agent Log File Size Limit
--------------------------------------------------------------------------------
set nocount on
use master
set rowcount 0

declare 
	@StartDateTime datetime,
	@logloop int,
	@maxerrorlog int,
	@maxagentlog int,
	@rowcountmax bigint,
	@rowcountbig bigint,
	@agentlogtype int,
	@errorlogtype int,
	@noeventscheck int,
	@skiperrorlog bit,
	@skipagentlog bit,
	@agentlog_path  nvarchar(255),
	@agentlog_file  nvarchar(255),
	@agentlog_prefix nvarchar(255),
	@archive_files nvarchar(255),
	@agentLogSize int,
	@errorLogSize int,
	@errorLogSizeLimitInBytes int,
	@agentLogSizeLimitInBytes int,
	@lastEntry datetime

if (select isnull(object_id('tempdb..#loglist_errorlog'), 0)) = 0 
	create table #loglist_errorlog
	(
		ArchiveNo int, 
		LastEntry datetime, 
		Size int
	)
else
	truncate table #loglist_errorlog

if (select isnull(object_id('tempdb..#logreader'), 0)) = 0 
	create table #logreader
	(
		RowNumber int identity,
		Text nvarchar(4000),
		ContinuationRow int
	)
else
	truncate table #logreader

if (select isnull(object_id('tempdb..#loglist_agentlog'), 0)) = 0 
		create table #loglist_agentlog
		(
			ArchiveNo int, 
			FileName nvarchar(255),
			LastEntry datetime, 
			Size int
		)
	else
		truncate table #loglist_agentlog

	if (select isnull(object_id('tempdb..#agentlog_dir'), 0)) = 0 
		create table #agentlog_dir
		(
			filename nvarchar(255), 
			depth int, 
			isfile int
		)
	else
		truncate table #agentlog_dir

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


	if (select isnull(object_id('tempdb..sqldmLogScanTemp'), 0)) = 0 
		create table tempdb..sqldmLogScanTemp
		(
			CollectionService nvarchar(255),
			FirstLine nvarchar(1000),
			LastReadMarker int)
	
select 
	@StartDateTime = StartDateTime, 
	@rowcountmax = {0}, 
	@logloop = 1,
	@agentlogtype = {1},
	@errorlogtype = {2},
	@skiperrorlog = {3},
	@skipagentlog = {4},
	@noeventscheck = 0,
	@errorLogSizeLimitInBytes = {5},
	@agentLogSizeLimitInBytes = {6}
from
	tempdb..LogScanVars
where hostname = host_name()

-- Populate Agent Log List
if @skipagentlog < 1
begin

	-- Find agent log location
	execute master.dbo.xp_instance_regread N'hkey_local_machine', N'software\microsoft\mssqlserver\sqlserveragent', N'errorlogfile',@agentlog_file output,N'no_output'

	select @agentlog_file = isnull(@agentlog_file,'no registry entry')

	if (@agentlog_file <> 'no registry entry')
	begin
		select @agentlog_path = left(@agentlog_file,len(@agentlog_file) - charindex('\',reverse(@agentlog_file)))  + '\'
		select @agentlog_prefix = left(@agentlog_file,len(@agentlog_file) - charindex('.',reverse(@agentlog_file)))
		select @agentlog_prefix = right(@agentlog_prefix,charindex('\',reverse(@agentlog_prefix))-1)
	
		-- Find agent log files
		insert #agentlog_dir 
			execute master.dbo.xp_dirtree @agentlog_path, 1, 1

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
			#agentlog_dir 
		where 
			filename like @agentlog_prefix + '.[0-9]%'

		-- Loop through agent logs
		while @agentfileextension_loop < = isnull(@loopexit,0)
		begin
			select 
				@agentfilename_loop =  filename
			from #agentlog_dir 
			where 
				(filename like @agentlog_prefix + '.[0-9]%' 
				and cast(right(filename,charindex('.',reverse(filename))-1) as int) = @agentfileextension_loop)
				or
				(filename like @agentlog_prefix + '.%'
				and @agentfileextension_loop = 0)
			order by 
				case when @agentfileextension_loop = 0 then -1 else cast(right(filename,charindex('.',reverse(filename))-1) as int) end

			-- Set up agent file path to pass to xp_getfiledetails
			set @agentfilepath_loop = @agentlog_path +@agentfilename_loop
	
			-- Look up file details
			if isnull(@agentfilename_loop,'') <> ''
				insert #fileDetails 
					execute master.dbo.xp_getfiledetails @agentfilepath_loop

			-- Insert into #loglist_agentlog if we are reading the log
			-- Exit the loop when we reach an old log
			if 
				(@@rowcount > 0)
				and
				(select 
					convert(datetime, stuff(stuff(str(LastWrittenDate, 8), 7, 0, '-'), 5, 0, '-') 
					+ ' ' + stuff(stuff(replace(str(LastWrittenTime,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)
				from #fileDetails)
				>= dateadd(mi,-62,@StartDateTime)   or @agentfileextension_loop = 0  -- Due to daylight savings timestamps may be incorrect up to 1 hour
			begin
				insert #loglist_agentlog
				select
					@agentfileextension_loop,
					@agentfilepath_loop,
					convert(datetime, stuff(stuff(str(LastWrittenDate, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(LastWrittenTime,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121),
					Size
				from
					#fileDetails

				select @agentfileextension_loop = @agentfileextension_loop + 1
			end
			else
			begin
				select @agentfileextension_loop = @loopexit + 1
			end

			set @agentfilename_loop = null
			truncate table #fileDetails
		
		end 
		
		drop table #agentlog_dir
		drop table #fileDetails
	end

	-- Determine if there have been any events written
	select @noeventscheck = count(*)
	from 
		#loglist_agentlog
	where
		LastEntry > dateadd(mi,-62,@StartDateTime)  -- Due to daylight savings timestamps may be incorrect up to 1 hour

	-- Determine agent logs to read
	select 
		@maxagentlog = isnull(min(cast(ArchiveNo as int)),(select isnull(max(cast(ArchiveNo as int)),-1) from #loglist_agentlog))
	from
		#loglist_agentlog
	where
		LastEntry < dateadd(mi,-62,@StartDateTime)  -- Due to daylight savings timestamps may be incorrect up to 1 hour

end

-- Populate Error Log List
if @skiperrorlog < 1
begin
	-- Populate list of error logs
	insert into #loglist_errorlog
		execute sp_enumerrorlogs

	-- Determine error logs to read
	select 
		@maxerrorlog =isnull(min(cast(ArchiveNo as int)),(select isnull(max(cast(ArchiveNo as int)),-1) from #loglist_errorlog))
	from
		#loglist_errorlog
	where
		LastEntry < dateadd(mi,-62,@StartDateTime)  -- Due to daylight savings timestamps may be incorrect up to 1 hour

	-- Determine if there have been any events written
	select @noeventscheck = @noeventscheck + count(*)
	from 
		#loglist_errorlog
	where
		LastEntry > dateadd(mi,-62,@StartDateTime)  -- Due to daylight savings timestamps may be incorrect up to 1 hour
end

select @errorLogSize = Size from #loglist_errorlog where ArchiveNo = 0
select @lastEntry = LastEntry, @agentLogSize = Size from #loglist_agentlog where ArchiveNo = 0

-- Return the expected logs
-- Negative maximums indicate that no log exists
select @errorlogtype as logtype, isnull(@maxerrorlog, 0) as expectedlogs, @errorLogSize as CurrentLogSize
union
select @agentlogtype, isnull(@maxagentlog, 0),@agentLogSize

if @noeventscheck = 0
	goto no_events

-- Read current agent log if it exists
if @maxagentlog >= 0 and (@agentLogSize is not null and @agentLogSize < @agentLogSizeLimitInBytes and @lastEntry > dateadd(mi,-62,@StartDateTime))
begin
	set rowcount @rowcountmax

	insert into #logreader 
		execute master.dbo.xp_readerrorlog -1, @agentlog_file

	select 
		@agentlogtype,  -- Log Type
		0,				-- Archive Number
		Text,
		ContinuationRow
	from 
		#logreader

	-- Manage flood control
	select @rowcountbig = count(*) from #logreader

	if @rowcountbig >= @rowcountmax
		goto rowcount_exceeded
	else
		set @rowcountmax = @rowcountmax - @rowcountbig
	
	select @rowcountbig = 0
	
	delete from #logreader
end

-- Read current error log if it exists
if @maxerrorlog > 0 and (@errorLogSize is not null and @errorLogSize < @errorLogSizeLimitInBytes)
begin

	declare @identity bigint, @filterRowNumber bigint

	set rowcount 0

	-- Read current error log
	insert into #logreader 
		exec xp_readerrorlog

	if (select Text from #logreader where RowNumber = 1) = (select FirstLine from tempdb..sqldmLogScanTemp where CollectionService = host_name())
		select @filterRowNumber = max(LastReadMarker) from tempdb..sqldmLogScanTemp where CollectionService = host_name()
	else
		select @filterRowNumber = 0

	set rowcount @rowcountmax

	select 
		@errorlogtype,  -- Log Type
		0,				-- Archive Number
		Text,
		ContinuationRow
	from 
		#logreader
	where
		RowNumber > @filterRowNumber
	
	select @identity = scope_identity()

	delete from tempdb..sqldmLogScanTemp where CollectionService = host_name()
	insert into tempdb..sqldmLogScanTemp
		select host_name(), Text,  @identity from #logreader where RowNumber = 1

	-- Manage flood control
	select @rowcountbig = count(*) from #logreader where RowNumber > @filterRowNumber

	if @rowcountbig >= @rowcountmax
		goto rowcount_exceeded
	else
		set @rowcountmax = @rowcountmax - @rowcountbig

	select @rowcountbig = 0

	delete from #logreader
end

-- Read archive agent logs
while @logloop < @maxagentlog
begin
	set rowcount @rowcountmax

	select @agentlog_file = FileName, @agentLogSize = Size
	from #loglist_agentlog
	where ArchiveNo = @logloop
	if @agentLogSize < @agentLogSizeLimitInBytes
	begin
		insert into #logreader 
			execute master.dbo.xp_readerrorlog -1, @agentlog_file

		select 
			@agentlogtype,  -- Log Type
			@logloop,		-- Archive Number
			Text,
			ContinuationRow
		from 
			#logreader

		-- Manage flood control
		select @rowcountbig = count(*) from #logreader

		if @rowcountbig >= @rowcountmax
			goto rowcount_exceeded
		else
			set @rowcountmax = @rowcountmax - @rowcountbig

		select @rowcountbig = 0

		delete from #logreader
	end
	set @logloop = @logloop + 1
end

set @logloop = 1

-- Read archive error logs
while @logloop < @maxerrorlog
begin

	set rowcount @rowcountmax
	
	select @errorLogSize = Size 
		from #loglist_errorlog 
		where ArchiveNo = @logloop
	
	if @errorLogSize < @errorLogSizeLimitInBytes
	begin
		insert into #logreader 
			exec xp_readerrorlog @logloop

		select 
			@errorlogtype,	-- Log Type
			@logloop,		-- Archive Number
			Text,
			ContinuationRow
		from 
			#logreader

		-- Manage flood control
		select @rowcountbig = count(*) from #logreader

		if @rowcountbig >= @rowcountmax
			goto rowcount_exceeded
		else
			set @rowcountmax = @rowcountmax - @rowcountbig

		select @rowcountbig = 0

		delete from #logreader
	end

	set @logloop = @logloop + 1
end

rowcount_exceeded:
if @rowcountbig >= @rowcountmax
	select 'Rowcount exceeded'
goto end_of_batch

no_events:
select 'No events to read'

end_of_batch:
drop table #loglist_agentlog
drop table #loglist_errorlog
drop table #logreader
delete from tempdb..LogScanVars where hostname = host_name()

