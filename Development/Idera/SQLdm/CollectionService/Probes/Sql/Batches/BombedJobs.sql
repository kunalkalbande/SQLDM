--------------------------------------------------------------------------------
--  Batch: bombed jobs
--  Tables: msdb..sysjobhistory, msdb..sysjobsteps, msdb..sysjobs, 
--  msdb..syscategories,
--	Variables:	[0] - Selected job categories
--	[1] - Instanceid from previous refresh
--  [2] - Treat any failure as critical
--  [3] - Jobs to clear
--  [4] - Flag to prevent first-time alerts
--------------------------------------------------------------------------------\
declare @instance int, @previnstance int, @treatfailureascritical int, @flag int 

select @previnstance = {1}, @treatfailureascritical = {2}, @flag = {4}

-- Run Status:
-- 0 = Failed
-- 1 = Succeeded
-- 2 = Retry
-- 3 = Cancelled
-- 4 = In progress
select 
	@instance = max(instance_id) 
from 
	msdb..sysjobhistory 
where 
	run_status not in (2, 4)

select isnull(@instance,0)

if (select isnull(object_id('tempdb..#tempfailedjobs'), 0)) <> 0 
begin 
	drop table #tempfailedjobs
end

if (select isnull(object_id('tempdb..#finalfailedjobs'), 0)) = 0 
begin
	create table #finalfailedjobs
	(
		[job id] uniqueidentifier,
		[Job Name] sysname,
		[Job Description] nvarchar(4000),
		[step name] sysname,
		id int,
		severity int,
		date datetime,
		Command nvarchar(4000),
		Message nvarchar(4000)
	)
end
begin 
	truncate table #finalfailedjobs
end

select 
    'instance' = h.instance_id,
	'job id' = j.job_id,
	'Job Name' = j.name, 
	'step name' = h.step_name, 
	'id' = h.sql_message_id, 
	'severity' = h.sql_severity, 
	'date' = dateadd(hh,datediff(hh,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)),
	'Command' = left(s.command collate database_default,2700),
	'Message' = h.message
into
	#tempfailedjobs
from 
	msdb..sysjobhistory h, 
	msdb..sysjobsteps s, 
	msdb..sysjobs j, 
	msdb..syscategories c 
where 
	h.job_id = s.job_id 
	and h.step_id = s.step_id 
	and j.job_id = h.job_id 
	and c.category_id = j.category_id 
	and s.on_fail_action = 2 
	and h.run_status = 0 
	{0}
	and datediff(day,convert(datetime,convert(varchar(20), h.run_date)), GETDATE()) <= 1 
	and h.instance_id = 
		(
		select 
			max(h1.instance_id) 
		from 
			msdb..sysjobhistory h1 
		where 
			h1.job_id = s.job_id 
			and h1.step_id = s.step_id 
			and h1.run_status = case when @previnstance > 1 and @treatfailureascritical = 1 then 0 else h1.run_status end
			and h1.instance_id between @previnstance and @instance
			and @flag > 0
		) 
option(robust plan)

insert into
	#finalfailedjobs
select 
	j.job_id,
	[Job Name],
	j.description as [Job Description],
	[step name],
	[id],
	[severity],
	[date],
	[Command],
	t.[Message]
from #tempfailedjobs t
left join msdb..sysjobs j
on [job id] = j.job_id
union
select 	
	j.job_id,
	'Job Name' = j.name, 
	'Job Description' = j.description,
	h.step_name, 
	h.sql_message_id, 
	h.sql_severity, 
	dateadd(hh,datediff(hh,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)),
	'Command' = null,
	'Message' = h.message
from 
	msdb..sysjobhistory h, 
	msdb..sysjobs j, 
	msdb..syscategories c 
where 
	j.job_id = h.job_id 
	and h.step_id = 0
	and c.category_id = j.category_id 
	and h.run_status = 0 
	and h.job_id not in (select [job id] from #tempfailedjobs)
	{0}
	and datediff(day,convert(datetime,convert(varchar(20), h.run_date)), GETDATE()) <= 1
	and h.instance_id = 
		(
		select 
			max(h1.instance_id) 
		from 
			msdb..sysjobhistory h1 
		where 
			h1.job_id = h.job_id
			and h1.run_status = case when @previnstance > 1 and @treatfailureascritical = 1 then 0 else h1.run_status end
			and h1.instance_id between @previnstance and @instance
			and @flag > 0
		) 

drop table #tempfailedjobs
	
select
	j.[Job Name],
	j.[Job Description],
	j.[step name],
	j.id,
	j.severity,
	j.date,
	j.Command,
	j.Message,
	j.[job id],
	runs = sum(case when step_id = 0 then 1 else 0 end),
	failures = sum(case when step_id = 0 and run_status = 0 then 1 else 0 end),
	collectionSince = min(dateadd(hh,datediff(hh,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)))
from
	#finalfailedjobs j
	left join msdb..sysjobhistory h
	on j.[job id] = h.job_id
where
	datediff(day,convert(datetime,convert(varchar(20), h.run_date)), GETDATE()) <= 1 
	and h.instance_id between @previnstance and @instance
group by
	j.[job id],
	j.[Job Name],
	j.[Job Description],
	j.[step name],
	j.id,
	j.severity,
	j.date,
	j.Command,
	j.Message

select
	'job id' = j.job_id,
	'Job Name' = j.name, 
	j.description,
	'date' = dateadd(hh,datediff(hh,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)),
	h.run_duration
from
	msdb..sysjobs j
	left join msdb..sysjobhistory h
	on j.job_id = h.job_id
where
	j.job_id in ('00000000-0000-0000-0000-000000000000'{3})
	and h.step_id = 0
	and h.run_status = 1
	and datediff(day,convert(datetime,convert(varchar(20), h.run_date)), GETDATE()) <= 1
	and h.instance_id = 
		(
		select 
			max(h1.instance_id) 
		from 
			msdb..sysjobhistory h1 
		where 
			h1.job_id = h.job_id
			and h.step_id = 0
		) 

drop table #finalfailedjobs

