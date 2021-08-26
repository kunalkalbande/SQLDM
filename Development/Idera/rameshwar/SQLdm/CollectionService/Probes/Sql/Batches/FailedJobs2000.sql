--------------------------------------------------------------------------------
--  Batch:  Failed Jobs 2000
--  Tables: msdb..sysjobhistory, msdb..sysjobsteps, msdb..sysjobs, 
--  msdb..syscategories,
--	Variables:	[0] - Selected job categories
--				[1] - and s.on_fail_action = 2 (if only interested in job level failures)
--				[2] - and h.step_id = 0 (if only interested in job level failures)
--------------------------------------------------------------------------------\
declare @instance int, @previnstance int, @treatfailureascritical int, @flag int 

select @previnstance = numValue from tempdb..FailedJobVars where name = '@previnstance' and hostname = host_name()
select @treatfailureascritical = numValue from tempdb..FailedJobVars where name = '@treatfailureascritical' and hostname = host_name()
select @flag  = numValue from tempdb..FailedJobVars where name = '@flag' and hostname = host_name()
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

if (select isnull(object_id('tempdb..#finalfailedjobs'), 0)) <> 0 
begin
	drop table #finalfailedjobs
end

if (select isnull(object_id('tempdb..#finalfailedjobs'), 0)) = 0 
begin
	create table #finalfailedjobs
	(
		[job id] uniqueidentifier,
		[Job Name] sysname,
		[Job Description] nvarchar(4000),
		[step name] sysname,
		[step id] int,
		id int,
		severity int,
		date datetime,
		Command nvarchar(4000),
		Message nvarchar(4000),
		category_id int
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
	'step id' = h.step_id, 
	'id' = h.sql_message_id, 
	'severity' = h.sql_severity, 
	'date' = dateadd(mi,datediff(mi,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)),
	'Command' = left(s.command collate database_default,2700),
	'Message' = h.message,
	j.category_id
into
	#tempfailedjobs
from
	msdb..sysjobs j
	left join msdb..sysjobsteps s
	on j.job_id = s.job_id
	left join msdb..sysjobhistory h
	on j.job_id = h.job_id  
	and h.step_id = s.step_id 
	left join msdb..syscategories c 
	on c.category_id = j.category_id 
where 
	h.run_status = 0 
	{1}
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
	[step id],
	[id],
	[severity],
	[date],
	[Command],
	t.[Message],
	t.category_id
from #tempfailedjobs t
left join msdb..sysjobs j
on [job id] = j.job_id
union
select 	
	j.job_id,
	'Job Name' = j.name, 
	'Job Description' = j.description,
	h.step_name,
	h.step_id, 
	h.sql_message_id, 
	h.sql_severity, 
	dateadd(mi,datediff(mi,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)),
	'Command' = null,
	'Message' = h.message,
	j.category_id
--	'Category' = c.name
from 
	msdb..sysjobhistory h, 
	msdb..sysjobs j, 
	msdb..syscategories c 
where 
	j.job_id = h.job_id 
	and h.step_id = 0
	and c.category_id = j.category_id 
	and h.run_status = 0 
	{2}
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
	collectionSince = min(dateadd(mi,datediff(mi,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121))),
	j.[step id],
	c.name
from
	#finalfailedjobs j
	left join msdb..sysjobhistory h
	on j.[job id] = h.job_id
	left join msdb..syscategories c 
	on c.category_id = j.category_id 
where
	datediff(day,convert(datetime,convert(varchar(20), h.run_date)), GETDATE()) <= 1 
	and h.instance_id between @previnstance and @instance
group by
	j.[job id],
	j.[Job Name],
	j.[Job Description],
	j.[step id],
	j.[step name],
	j.id,
	j.severity,
	j.date,
	j.Command,
	j.Message,
	c.name

select
	'job id' = j.job_id,
	'Job Name' = j.name, 
	j.description,
	'date' = dateadd(mi,datediff(mi,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)),
	h.run_duration,
	'step id' = h.step_id
from
	tempdb..FailedJobSteps fj
	left join msdb..sysjobhistory h
		on fj.guid = h.job_id and fj.stepid = h.step_id 
	left join msdb..sysjobs j
		on fj.guid = j.job_id
where
	fj.hostname = host_name()
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
			and h.step_id = fj.stepid
		) 

drop table #finalfailedjobs

delete from tempdb..FailedJobVars where hostname = host_name()
delete from tempdb..FailedJobSteps where hostname = host_name()
