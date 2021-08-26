--------------------------------------------------------------------------------
--  Batch: Log Scan 2005
--	Variables:	
--	[0] - Rowcount max to return (flood control)
--	[1] - Enumeration to return for Agent Logs
--	[2] - Enumeration to return for SQL Logs
--	[3] - Skip error log (1 if true)
--	[4] - Skip agent log (1 if true)
--  [5] - Error LogSizeLimit 
--  [6] - Agent Log File Size Limit
--------------------------------------------------------------------------------
use master
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
	@now datetime,
	@errorLogSizeLimitInBytes bigint,
	@agentLogSizeLimitInBytes bigint,
	@CorrectedStartDateTime datetime
	
select @now = getdate()

declare @errorloglist table 
(
	ArchiveNo int, 
	LastEntry datetime, 
	Size bigint
)

declare @agentloglist table 
(
	ArchiveNo int, 
	LastEntry datetime, 
	Size bigint
)

declare @logreader table
(
	LogDate datetime,
	ProcessInfo nvarchar(512), 
	Text nvarchar(4000)
)

select 
	@StartDateTime = StartDateTime, 
	@rowcountmax = {0}, 
	@agentlogtype = {1},
	@errorlogtype = {2},
	@skiperrorlog = {3},
	@skipagentlog = {4},
	@noeventscheck = 0,
	@errorLogSizeLimitInBytes = {5},
	@agentLogSizeLimitInBytes = {6},
	@CorrectedStartDateTime = dateadd(mi,-62,StartDateTime)
from
	tempdb..LogScanVars
where hostname = host_name()

if @skipagentlog < 1
begin
	-- Populate list of agent logs
	insert into @agentloglist
		execute sp_enumerrorlogs 2

	 --Determine if there have been any events written in the last 62 minutes
	select @noeventscheck = count(*)
	from @agentloglist
	where LastEntry > @CorrectedStartDateTime  -- Due to daylight savings timestamps may be incorrect up to 1 hour
	and Size < @agentLogSizeLimitInBytes
end

if @skiperrorlog < 1
begin
--	-- Populate list of error logs
	insert into @errorloglist
		execute sp_enumerrorlogs

	-- Determine if there have been any events written
	select @noeventscheck = @noeventscheck + count(*)
	from @errorloglist
	where LastEntry > @CorrectedStartDateTime  -- Due to daylight savings timestamps may be incorrect up to 1 hour
	and Size < @errorLogSizeLimitInBytes
end

declare @type int, @archiveNo int, @lastEntry DateTime, @size int

declare errorLogCursor cursor LOCAL STATIC FORWARD_ONLY READ_ONLY for
select @errorlogtype as type, * 
	from @errorloglist 
	where LastEntry > @CorrectedStartDateTime 
	and Size < @errorLogSizeLimitInBytes  order by LastEntry desc -- Due to daylight savings timestamps may be incorrect up to 1 hour

declare agentLogCursor cursor LOCAL STATIC FORWARD_ONLY READ_ONLY for
select @agentlogtype as type, * 
	from @agentloglist 
	where  LastEntry > @CorrectedStartDateTime 
	and Size < @agentLogSizeLimitInBytes order by LastEntry desc   -- Due to daylight savings timestamps may be incorrect up to 1 hour


-- Return the expected logs
-- Negative maximums indicate that no log exists
;with skippedError as (select @errorlogtype as type, * 
		from @errorloglist 
		where LastEntry > @CorrectedStartDateTime 
		and Size > @errorLogSizeLimitInBytes),  -- Due to daylight savings timestamps may be incorrect up to 1 hour
skippedAgent as (select @agentlogtype as type, * 
	from @agentloglist 
	where  LastEntry > @CorrectedStartDateTime 
	and Size > @agentLogSizeLimitInBytes),
errorLogList as (select @errorlogtype as logtype, 
	count(*) as expectedlogs
	from @errorloglist 
	where  LastEntry > @CorrectedStartDateTime)

select @errorlogtype as logtype,
	count(*) as expectedlogs,
	(select count(*) from skippedError) as skipped, (Select Size from @errorloglist where ArchiveNo = 0) as CurrentLogSize
	from @errorloglist 
	where  LastEntry > @CorrectedStartDateTime 
	and Size < @errorLogSizeLimitInBytes
union
select @agentlogtype, 
	count(*),
	(select count(*) from skippedAgent), (Select Size from @agentloglist where ArchiveNo = 0)
	from @agentloglist 
	where  LastEntry > @CorrectedStartDateTime 
	and Size < @agentLogSizeLimitInBytes

if @noeventscheck = 0
begin
	-- Patch SQLDM-28314: The 'SQL Server Log size' metric is not being alerted as expected
	-- Microsoft's bug: https://connect.microsoft.com/SQLServer/feedback/details/761308/xp-enumerrorlogs-returns-incorrect-results-when-running-on-windows-server-2008-r2

	declare @enumErrorLogLastDateTime as datetime
	declare @hourDiff int

	
	set @enumErrorLogLastDateTime = (select max(LastEntry) from @errorloglist where ArchiveNo = 0)
	set @hourDiff = DateDiff(hour, @enumErrorLogLastDateTime, @now)

	-- If Microsoft's bug is occurring, we read the Log Error whenever it has passed more than 3 hours since writing operation.
	if @hourDiff >= 3 
	begin
		declare @beginDateTime datetime
		declare @endDateTime datetime
		set @beginDateTime  = DateAdd(minute, -5, @now)
		set @endDateTime  = @now

		insert into @logreader 	
		execute master.dbo.xp_readerrorlog 0, 1, NULL, NULL,  @beginDateTime, @now
		delete from @logreader
	end

    goto no_events
end

open errorLogCursor
fetch next from errorLogCursor into @type, @archiveNo, @lastEntry, @size
while @@FETCH_STATUS <> -1
Begin
	if @@FETCH_STATUS <> -2

	insert into @logreader 
		execute master.dbo.xp_readerrorlog @archiveNo,1,null,null,@StartDateTime,@now,N'asc'; 

	select top (@rowcountmax)
		@type,			-- Log Type
		@archiveNo,		-- Archive Number
		LogDate,
		ProcessInfo, 
		ISNULL(Text,'')
	from 
		@logreader
	where
		LogDate > @StartDateTime
	order by LogDate desc

	-- Manage flood control
	set @rowcountbig = rowcount_big()
	if @rowcountbig >= @rowcountmax
		begin
			close errorLogCursor
			deallocate errorLogCursor
			goto rowcount_exceeded
		end
	else
		set @rowcountmax = @rowcountmax - @rowcountbig

	set @rowcountbig = 0

	delete from @logreader

	fetch next from errorLogCursor into @type, @archiveNo, @lastEntry, @size
end
close errorLogCursor
deallocate errorLogCursor

open agentLogCursor 
fetch next from agentLogCursor  into @type, @archiveNo, @lastEntry, @size
while @@FETCH_STATUS <> -1
Begin
	if @@FETCH_STATUS <> -2
	insert into @logreader 
		execute master.dbo.xp_readerrorlog @archiveNo,2,null,null,@StartDateTime,@now,N'asc'; 

	select top (@rowcountmax)
		@type,  -- Log Type
		@archiveNo,				-- Archive Number
		LogDate,
		ProcessInfo, 
		ISNULL(Text,'')
	from 
		@logreader
	where
		LogDate > @StartDateTime
	order by LogDate desc

	-- Manage flood control
	set @rowcountbig = rowcount_big()
	if @rowcountbig >= @rowcountmax
		begin
			close agentLogCursor 
			deallocate agentLogCursor 
			goto rowcount_exceeded
		end
	else
		set @rowcountmax = @rowcountmax - @rowcountbig

	set @rowcountbig = 0

	delete from @logreader


	--select @type, @archiveNo, @lastEntry, @size
	fetch next from agentLogCursor  into @type, @archiveNo, @lastEntry, @size
end

close agentLogCursor 
deallocate agentLogCursor 

--delete from @errorloglist
--delete from @agentloglist

rowcount_exceeded:
if @rowcountbig >= @rowcountmax
	select 'Rowcount exceeded'
goto end_of_batch

no_events:
select 'No events to read'

end_of_batch:
delete from tempdb..LogScanVars where hostname = host_name()