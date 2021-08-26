--------------------------------------------------------------------------------
--  Batch: Active Waits Batch
--  Variables: [0] - Runtime in Seconds
--  [1] - Loop Time (00:00:00.5 format)
--  [2] - Filter statements Level 1
--  [3] - Filter statements Level 2
--  [4] - Rowcount
--  [5] - top (@Rowcountmax) or null
--  [6] - Excluded wait types
--  [7] - Use tempdb for lower compatibility level databases
--------------------------------------------------------------------------------
{7}

declare 
@StartTime datetime,
@NowTicks bigint,
@PrevTicks bigint,
@Elapsed bigint,
@RunTime int,
@Rowcount bigint,
@Rowcountmax bigint

set @RunTime = {0}
set @Rowcount = 0
set @Rowcountmax = {4}

declare @ExcludedWaitTypes table(ExcludedType nvarchar(256))

{6}

create table #ActiveWaits
(
session_id smallint,
wait_duration_ms bigint,
wait_type nvarchar(120),
host_name nvarchar(256),
program_name nvarchar(256),
login_name nvarchar(256),
statement_txt nvarchar(max),
database_id smallint,
wait_started_ms_ticks bigint,
task_bound_ms_ticks bigint,
statement_start_offset decimal,
statement_end_offset decimal
)

create table #ActiveWaitsTemp
(
session_id smallint,
wait_duration_ms bigint,
wait_type nvarchar(120),
host_name nvarchar(256),
program_name nvarchar(256),
login_name nvarchar(256),
statement_txt nvarchar(max),
database_id smallint,
wait_started_ms_ticks bigint,
task_bound_ms_ticks bigint,
statement_start_offset decimal,
statement_end_offset decimal
)


select @StartTime = getdate(), @NowTicks = -1, @Elapsed = 0

while (datediff(s,@StartTime, getdate())  < @RunTime ) and (@Rowcountmax < 1 or @Rowcount < @Rowcountmax)
begin

	set @PrevTicks = @NowTicks
	select @NowTicks = ms_ticks
	from sys.dm_os_sys_info

	if @PrevTicks > 0
		set @Elapsed = @NowTicks - @PrevTicks

	insert into #ActiveWaitsTemp
	select 
		wt.session_id, 
		case when @Elapsed > isnull(wait_duration_ms,0) then @Elapsed else wait_duration_ms end, 
		wt.wait_type,
		host_name,
		program_name,
		login_name,
		statement_txt = case when isnull(is_user_process,0) = 0 then null else 
			(select substring(text, statement_start_offset/2 + 1,
			(case when statement_end_offset < 1
			then len(convert(nvarchar(max),text)) * 2
			else statement_end_offset
			end - statement_start_offset)/2 + 1)
			from sys.dm_exec_sql_text(sql_handle)) end,
		database_id,
		case when @Elapsed > isnull(wait_duration_ms,0) then @PrevTicks else wait_started_ms_ticks end,
		task_bound_ms_ticks,
		statement_start_offset,
		statement_end_offset
	from 
		sys.dm_os_waiting_tasks wt
		left join @ExcludedWaitTypes et
		on wt.wait_type collate database_default = et.ExcludedType collate database_default 
		left join sys.dm_os_workers w
		on wt.waiting_task_address = w.task_address
		left join sys.dm_exec_sessions s
		on wt.session_id = s.session_id
		left join 
		sys.dm_exec_requests r
		on r.session_id = s.session_id
	where
		et.ExcludedType is null
		and r.status <> 'sleeping'
		and r.session_id >= 50
		and s.session_id >= 50
		and isnull(is_user_process,0) = 1
		and r.session_id <> @@spid
		and program_name not like 'SQL diagnostic manager%'
		{2}


	if (datediff(s,@StartTime, getdate()) % 5) = 0
	begin
		insert into #ActiveWaits
		select {5}
			session_id,
			wait_duration_ms = max(wait_duration_ms),
			wait_type,
			host_name,
			program_name,
			login_name,
			statement_txt,
			database_id,
			wait_started_ms_ticks,
			task_bound_ms_ticks,
			statement_start_offset,
			statement_end_offset
		from 
		#ActiveWaitsTemp
		where 
			statement_txt is not null
			and len(rtrim(statement_txt)) > 0 
			{3}
		group by 
			session_id,
			wait_type,
			host_name,
			program_name,
			login_name,
			statement_txt,
			database_id,
			wait_started_ms_ticks,
			task_bound_ms_ticks,
			statement_start_offset,
			statement_end_offset

		set @Rowcount = @Rowcount + rowcount_big()

		truncate table #ActiveWaitsTemp
	end

	waitfor delay '{1}'
end

if (@Rowcountmax < 1 or @Rowcount < @Rowcountmax)
begin
insert into #ActiveWaits
	select {5}
		session_id,
		wait_duration_ms = max(wait_duration_ms),
		wait_type,
		host_name,
		program_name,
		login_name,
		statement_txt,
		database_id,
		wait_started_ms_ticks,
		task_bound_ms_ticks,
		statement_start_offset,
		statement_end_offset
	from 
		#ActiveWaitsTemp
	where statement_txt is not null
		and len(statement_txt) > 0 
		{3}
	group by 
		session_id,
		wait_type,
		host_name,
		program_name,
		login_name,
		statement_txt,
		database_id,
		wait_started_ms_ticks,
		task_bound_ms_ticks,
		statement_start_offset,
		statement_end_offset
end

set 
@StartTime = getutcdate()

select
@NowTicks = ms_ticks
from sys.dm_os_sys_info

;with ActiveWaitsGrouped(
wait_duration_ms,
session_id,
wait_type,
host_name,
program_name,
login_name,
database_id,
statement_txt,
task_bound_ms_ticks,
statement_start_offset,
statement_end_offset,
wait_started_ms_ticks
)
as
(
select 
	wait_duration_ms = max(wait_duration_ms),
	session_id,
	wait_type,
	host_name,
	program_name,
	login_name,
	database_id,
	statement_txt,
	task_bound_ms_ticks,
	statement_start_offset,
	statement_end_offset,
	wait_started_ms_ticks
from 
	#ActiveWaits	
group by 
	session_id,
	wait_type,
	task_bound_ms_ticks,
	wait_started_ms_ticks,
	host_name,
	program_name,
	login_name,
	database_id,
	statement_txt,
	statement_start_offset,
	statement_end_offset
)
select {5}
	Ticks = cast(min(wait_started_ms_ticks) - @NowTicks as bigint),
	WaitDuration = sum(wait_duration_ms),
	SessionID = session_id,
	WaitType = wait_type,
	HostName = host_name,
	ProgramName = program_name,
	LoginName = login_name,
	DatabaseName = db_name(database_id),
	StatementText = statement_txt,
	MSTicks = min(wait_started_ms_ticks),
	StartTime = @StartTime
from 
	ActiveWaitsGrouped
group by
	session_id,
	wait_type,
	task_bound_ms_ticks,
	host_name,
	program_name,
	login_name,
	database_id,
	statement_txt,
	statement_start_offset,
	statement_end_offset



drop table #ActiveWaits
drop table #ActiveWaitsTemp


