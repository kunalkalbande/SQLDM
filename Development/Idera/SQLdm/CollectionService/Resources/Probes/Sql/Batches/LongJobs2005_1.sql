--------------------------------------------------------------------------------
--  Batch: long running jobs
--  Tables: #already_reported_steps, #sqlagent_enum_jobs, master..syscurconfigs
--  XPs: xp_sqlagent_enum_jobs, msdb..sysjobs, msdb..syscategories
--	Variables:	[0] - Long jobs by percentage segment
--		[1] - Long jobs by length segment
--------------------------------------------------------------------------------

set nocount on

--------------------------------------------------------------------------------
--  TEMP table: Already reported jobs
--  created tables: tempdb..#already_reported_steps
--  tables: none
--  Purpose: Prevent re-alerting
--------------------------------------------------------------------------------
if (select isnull(object_id('tempdb..#already_reported_steps'), 0)) = 0 
begin 
	create table #already_reported_steps (
		job_id uniqueidentifier not null, 
		job_name sysname not null, 
		job_desc nvarchar(512) null,
		current_running_step nvarchar(300) not null, 
		current_retry_attempt int not null,
		no_of_seconds_running int not null, 
		started_at datetime NULL, 
		this_entry_datetime datetime not null,
		average_job_duration float,
		job_start_timestamp nvarchar(300),
		metric_type int,
		job_category sysname,
		step_id int,
		average_step_duration float,
		step_start_datetime datetime,
		no_of_seconds_running_step int,
		job_last_run_duration float) -- SQLDM-19816: adding a new field job_last_run_duration
end


if isnull(object_id('tempdb..#sqlagent_enum_jobs'), 0) <> 0 
	truncate table #sqlagent_enum_jobs 
else 
create table #sqlagent_enum_jobs 
	(job_id uniqueidentifier not null, 
	last_run_date int not null, 
	last_run_time int not null, 
	next_run_date int not null, 
	next_run_time int not null, 
	next_run_schedule_id int not null, 
	requested_to_run int not null, 
	request_source int not null, 
	request_source_id sysname NULL,
	running int not null, 
	current_step int not null, 
	current_retry_attempt int not null, 
	job_state int not null) 


declare @running_jobs table
	(job_id uniqueidentifier not null, 
	current_step int not null, 
	current_retry_attempt int not null, 
	job_start_datetime datetime,
	job_start_timestamp nvarchar(300),
	job_category sysname,
	step_start_datetime datetime,
	average_job_duration float,
	average_step_duration float,
	job_last_run_duration float) 
 

declare @agentxpavailable int 

select @agentxpavailable = value from master..syscurconfigs where config = 16384 

if (@agentxpavailable > 0) 
begin 

	-- Not executable at AWS
	--insert into #sqlagent_enum_jobs 
	--	execute master..xp_sqlagent_enum_jobs 1, sa 

	if @@rowcount > 0 
	begin 
		insert into @running_jobs 
			select 
				e.job_id, 
				current_step, 
				current_retry_attempt, 
				case next_run_date
				when 0 then null
				else
				convert(datetime,convert(varchar(20), next_run_date) 
					+ ' ' + substring(right(replicate(0,6)+convert(varchar(6),next_run_time),6),1,2)
					+':'+substring(right(replicate(0,6)+convert(varchar(6),next_run_time),6),3,2)+':'+substring(right(replicate(0,6)
					+convert(varchar(6),next_run_time),6),5,2))
				 end,
				 cast(next_run_date as nvarchar(100)) + cast(next_run_time as nvarchar(100)),
				c.name,
				null,
				1,
				null,
                -- SQLDM-19816: calculating the job_last_run_duration based on the last run time of the job
				(SELECT run_duration
					FROM (
						   SELECT 
								run_duration,  
								Row_Number() OVER (PARTITION BY job_id ORDER BY instance_id DESC) AS rownum
							FROM msdb..sysjobhistory jh
							WHERE
							   jh.job_id = j.job_id
						  ) resultset
					WHERE  
					resultset.rownum = 1) as job_last_run_duration
			from 
				#sqlagent_enum_jobs e
				inner join msdb..sysjobs j
				on e.job_id = j.job_id
				inner join msdb..syscategories c 
				on c.category_id = j.category_id 
			where
				running = 1 
				and request_source = 1 

			if @@rowcount > 0
			begin
				update @running_jobs
				set 
					average_job_duration = job.average_job_duration
				from
					@running_jobs r inner join
					(select 
						job_id,
						average_job_duration = 
						avg(cast(substring(right(replicate(0,10)+convert(varchar(10),run_duration),10),1,6) as int) * 3600
						+ cast(substring(right(replicate(0,10)+convert(varchar(10),run_duration),10),7,2) as int) * 60
						+ cast(substring(right(replicate(0,10)+convert(varchar(10),run_duration),10),9,2) as int))
					from 
						msdb..sysjobhistory h 
					where 
						h.step_id = 0
						and h.run_status = 1
					group by job_id ) job
					on job.job_id = r.job_id
				end

	if @@rowcount > 0 
	begin 
		declare @current_datetime datetime 
		select @current_datetime = getdate() 
		
{0}

{1}
		select
			job_name, 
			job_desc,
			current_running_step, 
			current_retry_attempt, 
			no_of_seconds_running, 
			started_at,
			average_job_duration,
			job_id,
			metric_type,
			job_category,
			step_id,
			no_of_seconds_running_step,
			average_step_duration,
			job_last_run_duration -- SQLDM-19816: selecting the job_last_run_duration in the final query
		from 
			#already_reported_steps 
		where 
			this_entry_datetime = @current_datetime 
		End 
	End 
End

