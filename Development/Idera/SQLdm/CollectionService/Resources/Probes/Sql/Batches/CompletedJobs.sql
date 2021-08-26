--  Tables: msdb..sysjobhistory, msdb..sysjobsteps, msdb..sysjobs, 
--  msdb..syscategories,
--	Variables:	[0] - last instanceID in the previous collection.
--              [1] - job filter
--				[2] - and h.step_id = h1.step_id (passed if alerting on job steps) 
--------------------------------------------------------------------------------\
declare @offset int
declare @prvInstanceId int
declare @lastInstanceId int

set @offset = datediff(mi, getdate(), getutcdate())

select @lastInstanceId = (select max(instance_id) from msdb..sysjobhistory where run_status not in (2, 4))

select @prvInstanceId = case when {0} = 0 then @lastInstanceId else {0} end

select isnull(@lastInstanceId, 0)

if (select isnull(object_id('tempdb..#completed_jobs'), 0)) <> 0
begin
	drop table tempdb..#completed_jobs
end

create table #completed_jobs
	(jobId uniqueidentifier,
	 jobName sysname,
	 stepId int,
	 stepName sysname,
	 runStatus int,
	 runDuration int,
	 startTime datetime,
	 sqlMessageId int,
	 sqlSeverity int, 
	 command nvarchar(4000),
	 message nvarchar(4000),
	 categoryName sysname
	 )

insert into #completed_jobs
	select 
		h.job_id,
		isnull(j.name,'(Unknown)'),
		h.step_id,
		isnull(h.step_name,'(Unknown)'),
		h.run_status,
		h.run_duration,
		dateadd(mi, @offset, convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)),
		h.sql_message_id,
		h.sql_severity,
		LEFT(s.command,4000),
		h.message,
		isnull(c.name,'(Unknown)')
	from msdb..sysjobhistory h
		 left join msdb..sysjobs j
			on h.job_id = j.job_id
		 left join msdb..sysjobsteps s
			on h.job_id = s.job_id and h.step_id = s.step_id
		 left join msdb..syscategories c
			on j.category_id = c.category_id
	where
		datediff(day,convert(datetime,convert(varchar(20), h.run_date)), GETDATE()) <= 1
		{1}
		and h.instance_id = 
			(
			select 
				max(h1.instance_id) 
			from 
				msdb..sysjobhistory h1 
			where 
				h.job_id = h1.job_id
				{2}
				and h1.instance_id between @prvInstanceId and @lastInstanceId
			) 		
			
select 
	t.jobId,
	t.jobName,
	t.stepId,
	t.stepName,
	t.runStatus,
	t.startTime,
	t.runDuration,
	t.sqlMessageId,
	t.sqlSeverity,
	t.command,
	t.message,
	t.categoryName,
	count(h.instance_id) as runs,
	failures = sum(case when h.run_status = 0 then 1 else 0 end),
	success = sum(case when h.run_status = 1 then 1 else 0 end),
	retries = sum(case when h.run_status = 2 then 1 else 0 end),
	canceled = sum(case when h.run_status = 3 then 1 else 0 end),
	CollectionSince = min(dateadd(hh,datediff(hh,getdate(),getutcdate()),convert(datetime, stuff(stuff(str(h.run_date, 8), 7, 0, '-'), 5, 0, '-') + ' ' + stuff(stuff(replace(str(h.run_time,6), ' ', '0'), 5, 0, ':'), 3, 0, ':'), 121)))
from
	#completed_jobs t,
	msdb..sysjobhistory h
where
	t.jobId = h.job_id
	and t.stepId = h.step_id
	and datediff(day,convert(datetime,convert(varchar(20), h.run_date)), GETDATE()) <= 1
	and h.instance_id between @prvInstanceId and @lastInstanceId
group by
	t.jobId,
	t.jobName,
	t.stepId,
	t.stepName,
	t.runStatus,
	t.startTime,
	t.runDuration,
	t.sqlMessageId,
	t.sqlSeverity,
	t.command,
	t.message,
	t.categoryName

drop table tempdb..#completed_jobs