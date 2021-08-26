--------------------------------------------------------------------------------
--  Batch: Log Scan 2005
--	Variables:	
--	[0] - Rowcount max to return (flood control)
--	[1] - Enumeration to return for Agent Logs
--	[2] - Enumeration to return for SQL Logs
--	[3] - Skip error log (1 if true)
--	[4] - Skip agent log (1 if true)
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
	@now datetime
	
select @now = getdate()

declare @loglist table 
(
	ArchiveNo int, 
	LastEntry datetime, 
	Size int
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
	@logloop = 1,
	@agentlogtype = {1},
	@errorlogtype = {2},
	@skiperrorlog = {3},
	@skipagentlog = {4},
	@noeventscheck = 0
from
	tempdb..LogScanVars
where hostname = host_name()

if @skipagentlog < 1
begin
	-- Populate list of agent logs
	insert into @loglist
		execute sp_enumerrorlogs 2

	-- Determine if there have been any events written
	select @noeventscheck = count(*)
	from 
		@loglist
	where
		LastEntry > dateadd(mi,-62,@StartDateTime)  -- Due to daylight savings timestamps may be incorrect up to 1 hour

	-- Determine agent logs to read
	select 
		@maxagentlog = isnull(min(cast(ArchiveNo as int)),(select isnull(max(cast(ArchiveNo as int)),-1) from @loglist))
	from
		@loglist
	where
		LastEntry < dateadd(mi,-62,@StartDateTime)  -- Due to daylight savings timestamps may be incorrect up to 1 hour

	delete from @loglist
end

if @skiperrorlog < 1
begin
	-- Populate list of error logs
	insert into @loglist
		execute sp_enumerrorlogs

	-- Determine error logs to read
	select 
		@maxerrorlog =isnull(min(cast(ArchiveNo as int)),(select isnull(max(cast(ArchiveNo as int)),-1) from @loglist))
	from
		@loglist
	where
		LastEntry < dateadd(mi,-62,@StartDateTime)  -- Due to daylight savings timestamps may be incorrect up to 1 hour

	-- Determine if there have been any events written
	select @noeventscheck = @noeventscheck + count(*)
	from 
		@loglist
	where
		LastEntry > dateadd(mi,-62,@StartDateTime)  -- Due to daylight savings timestamps may be incorrect up to 1 hour
end

if @noeventscheck = 0
	goto no_events

-- Return the expected logs
-- Negative maximums indicate that no log exists
select @errorlogtype as logtype, isnull(@maxerrorlog, 0) as expectedlogs
union
select @agentlogtype, isnull(@maxagentlog, 0)

-- Read current agent log if it exists
if @maxagentlog >= 0
begin
	insert into @logreader 
		execute master.dbo.xp_readerrorlog 0,2,null,null,@StartDateTime,@now,N'asc'; 

	select top (@rowcountmax)
		@agentlogtype,  -- Log Type
		0,				-- Archive Number
		LogDate,
		ProcessInfo, 
		Text
	from 
		@logreader
	where
		LogDate > @StartDateTime
	order by LogDate desc

	-- Manage flood control
	set @rowcountbig = rowcount_big()
	if @rowcountbig >= @rowcountmax
		goto rowcount_exceeded
	else
		set @rowcountmax = @rowcountmax - @rowcountbig

	set @rowcountbig = 0

	delete from @logreader
end

-- Read current error log if it exists
if @maxerrorlog > 0
begin
	-- Read current error log
	insert into @logreader 
		exec xp_readerrorlog 0, 1, null,null,@StartDateTime,@now,N'asc'

	select top (@rowcountmax) 
		@errorlogtype,  -- Log Type
		0,				-- Archive Number
		LogDate,
		ProcessInfo, 
		Text
	from 
		@logreader
	where
		LogDate > @StartDateTime
	order by LogDate desc

	-- Manage flood control
	set @rowcountbig = rowcount_big()
	if @rowcountbig >= @rowcountmax
		goto rowcount_exceeded
	else
		set @rowcountmax = @rowcountmax - @rowcountbig

	set @rowcountbig = 0

	delete from @logreader
end

-- Read archive agent logs
while @logloop < @maxagentlog
begin
	insert into @logreader 
		execute master.dbo.xp_readerrorlog @logloop,2,null,null,@StartDateTime,@now,N'asc'; 

	select top (@rowcountmax)
		@agentlogtype,	-- Log Type
		@logloop,		-- Archive Number
		LogDate,
		ProcessInfo, 
		Text
	from 
		@logreader
	where
		LogDate > @StartDateTime
	order by LogDate desc

	-- Manage flood control
	set @rowcountbig = rowcount_big()
	if @rowcountbig >= @rowcountmax
		goto rowcount_exceeded
	else
		set @rowcountmax = @rowcountmax - @rowcountbig

	set @rowcountbig = 0

	delete from @logreader

	set @logloop = @logloop + 1
end

set @logloop = 1

-- Read archive error logs
while @logloop < @maxerrorlog
begin
	insert into @logreader 
		exec xp_readerrorlog @logloop,1,null,null,@StartDateTime,@now,N'asc'; 

	select top (@rowcountmax)
		@errorlogtype,	-- Log Type
		@logloop,		-- Archive Number
		LogDate,
		ProcessInfo, 
		Text
	from 
		@logreader
	where
		LogDate > @StartDateTime
	order by LogDate desc

	-- Manage flood control
	set @rowcountbig = rowcount_big()
	if @rowcountbig >= @rowcountmax
		goto rowcount_exceeded
	else
		set @rowcountmax = @rowcountmax - @rowcountbig

	set @rowcountbig = 0

	delete from @logreader

	set @logloop = @logloop + 1
end

rowcount_exceeded:
if @rowcountbig >= @rowcountmax
	select 'Rowcount exceeded'
goto end_of_batch

no_events:
select 'No events to read'

end_of_batch:
delete from tempdb..LogScanVars where hostname = host_name()