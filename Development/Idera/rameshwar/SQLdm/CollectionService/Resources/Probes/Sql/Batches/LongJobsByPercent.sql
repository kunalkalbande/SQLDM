
		--------------------------------------------------------------------------------
		--  Query Segment: Long Running Jobs By Percent
		--  [0] - Selected job categories
		--	[1] - Long jobs warning percentage
		--	[2] - Long jobs critical percentage
		--  [3] - Metric ID
		--	[4] - and h.step_id = r.current_step
		--	[5] - and a.current_running_step = r.current_step
		--	[6] - and h.step_id = 0
		--  [7] - Minimal Runtime seconds {4}
		--------------------------------------------------------------------------------
		insert into #already_reported_steps
			select 
					j.job_id, 
					isnull(j.name,'(Unknown)'), 
					j.description,
					isnull(s.step_name,'(Unknown)'),
					r.current_retry_attempt, 
					datediff(ss,r.job_start_datetime, getdate()), 
					dateadd(mi,datediff(mi,getdate(),getutcdate()),r.job_start_datetime), 
					@current_datetime,
					average_job_duration,
					job_start_timestamp,
					{3},
					isnull(job_category,'(Unknown)'),
					r.current_step,
					average_step_duration,
					step_start_datetime,
					no_of_seconds_running_step = datediff(ss,isnull(r.step_start_datetime,r.job_start_datetime), getdate()),
					r.job_last_run_duration
			from 
				@running_jobs r
				left join msdb..sysjobs j
				on j.job_id = r.job_id
				left join msdb..syscategories c
				on c.category_id = j.category_id 
				left join msdb..sysjobsteps s
				on s.step_id = r.current_step
			where 
				r.job_start_datetime is not null 
				and s.job_id = r.job_id
				{0}
				and datediff(ss,r.job_start_datetime, getdate()) > {4}
				and 
				(
					( -- Job Level
						(not exists 
							(
							select * 
							from 
								#already_reported_steps a 
							where 
								a.job_id = r.job_id 
								 and a.step_id = 0
								and a.job_start_timestamp = r.job_start_timestamp
							) 
							and datediff(ss,r.job_start_datetime, getdate()) > average_job_duration * {1}
						)
						or
						(
							not exists
							(
							select * 
							from 
								#already_reported_steps a 
							where 
								a.job_id = r.job_id 
								 and a.step_id = 0 
								and a.job_start_timestamp = r.job_start_timestamp
								and no_of_seconds_running >= average_job_duration * {2}
							)
							and datediff(ss,r.job_start_datetime, getdate()) > average_job_duration * {2} 
							
						)
					)
					or
					( -- Step Level
						(not exists 
							(
							select * 
							from 
								#already_reported_steps a 
							where 
								a.job_id = r.job_id 
								and a.step_id = r.current_step 
								and a.job_start_timestamp = r.job_start_timestamp
								and a.step_start_datetime = r.step_start_datetime
							) 
							and datediff(ss,isnull(r.step_start_datetime,r.job_start_datetime), getdate()) > average_step_duration * {1}
						)
						or
						(
							not exists
							(
							select * 
							from 
								#already_reported_steps a 
							where 
								a.job_id = r.job_id 
								 and a.step_id = r.current_step 
								and a.job_start_timestamp = r.job_start_timestamp
								and a.step_start_datetime = r.step_start_datetime
								and no_of_seconds_running >= average_job_duration * {2}
							)
							and datediff(ss,isnull(r.step_start_datetime,r.job_start_datetime), getdate()) > average_step_duration * {2}
							
						)
					)
				)

		--------------------------------------------------------------------------------
		--  End Query Segment: Long Running Jobs By Percent
		--------------------------------------------------------------------------------