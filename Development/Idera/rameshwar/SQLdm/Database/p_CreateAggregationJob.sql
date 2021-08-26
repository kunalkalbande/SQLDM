if (object_id('p_CreateAggregationJob') is not null)
begin
drop procedure p_CreateAggregationJob
end
go

create procedure [dbo].[p_CreateAggregationJob] (
	@ForceRecreate bit = 0 -- Should the job be deleted and recreated even if it already exists?
)
as
begin
	DECLARE @JobName nvarchar(256)
	DECLARE @DBName nvarchar(256)
	DECLARE @SA nvarchar(256)
	DECLARE @JobExists bit
	DECLARE @jobId BINARY(16)

	SELECT @JobName = 'Aggregate Data ' + DB_NAME() -- Name used for job and schedule.
	SELECT @DBName = DB_NAME()
	SELECT @SA = suser_sname(0x1)	

	SELECT @jobId=job_id FROM msdb.dbo.sysjobs WHERE name = @JobName
	IF (@jobId is not null)
	begin
		SELECT @JobExists = 1
		-- if the job definition needs updating then add logic here
	end
	else
	begin
		-- make sure the uncategorized local category exists
		IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
			EXEC msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'

		EXEC msdb.dbo.sp_add_job 
				@job_name=@JobName, 
				@enabled=1, 
				@notify_level_eventlog=0, 
				@notify_level_email=0, 
				@notify_level_netsend=0, 
				@notify_level_page=0, 
				@delete_level=0, 
				@description=N'Aggregate data into day-length records.', 
				@category_name=N'[Uncategorized (Local)]', 
				@owner_login_name=@SA, 
				@job_id = @jobId OUTPUT
	end

	-- create or update the jobstep for query aggregation
	IF (@JobExists=1 AND EXISTS(SELECT step_name FROM msdb.dbo.sysjobsteps WHERE job_id=@jobId AND step_id = 1))
	begin -- update the jobstep
		EXEC msdb.dbo.sp_update_jobstep 
				@job_id=@jobId, 
				@step_name=N'Call Query SP', 
				@step_id=1, 
				@cmdexec_success_code=0, 
				@on_success_action=3, -- To go to the next step on success
				@on_success_step_id=0, 
				@on_fail_action=2, 
				@on_fail_step_id=0, 
				@retry_attempts=0, 
				@retry_interval=0, 
				@os_run_priority=0, 
				@subsystem=N'TSQL', 
				@command=N'EXEC [p_AggregateQueryData]',
				@database_name=@DBName, 
				@flags=0
	end
	else
	begin  -- add the jobstep
		EXEC msdb.dbo.sp_add_jobstep 
				@job_id=@jobId, 
				@step_name=N'Call Query SP', 
				@step_id=1, 
				@cmdexec_success_code=0, 
				@on_success_action=3, 
				@on_success_step_id=0, 
				@on_fail_action=2, 
				@on_fail_step_id=0, 
				@retry_attempts=0, 
				@retry_interval=0, 
				@os_run_priority=0, @subsystem=N'TSQL', 
				@command=N'EXEC [p_AggregateQueryData]',
				@database_name=@DBName, 
				@flags=0
	end

	-- create or update the jobstep for forecasting aggregation
	IF (@JobExists=1 AND EXISTS(SELECT step_name FROM msdb.dbo.sysjobsteps WHERE job_id=@jobId AND step_id = 2))
	begin -- update the jobstep
		EXEC msdb.dbo.sp_update_jobstep 
				@job_id=@jobId, 
				@step_name=N'Call Forecasting SP', 
				@step_id=2, 
				@cmdexec_success_code=0, 
				@on_success_action=1, 
				@on_success_step_id=0, 
				@on_fail_action=2, 
				@on_fail_step_id=0, 
				@retry_attempts=0, 
				@retry_interval=0, 
				@os_run_priority=0, 
				@subsystem=N'TSQL', 
				@command=N'EXEC [p_AggregateForecastingData]',
				@database_name=@DBName, 
				@flags=0
	end
	else
	begin  -- add the jobstep
		EXEC msdb.dbo.sp_add_jobstep 
				@job_id=@jobId, 
				@step_name=N'Call Forecasting SP', 
				@step_id=2, 
				@cmdexec_success_code=0, 
				@on_success_action=1, 
				@on_success_step_id=0, 
				@on_fail_action=2, 
				@on_fail_step_id=0, 
				@retry_attempts=0, 
				@retry_interval=0, 
				@os_run_priority=0, @subsystem=N'TSQL', 
				@command=N'EXEC [p_AggregateForecastingData]',
				@database_name=@DBName, 
				@flags=0
	end
	-- make step 1 the first jobstep
	EXEC msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1


	-- sp_add_jobschedule is deprecated in SQL Server 2005,
	-- but used for compatibility with SQL Server 2000.
	IF (@JobExists=0 OR NOT EXISTS(SELECT [job_id] FROM msdb.dbo.sysjobschedules WHERE job_id=@jobId AND schedule_id is not null))
	begin -- create the jobstep
		EXEC msdb.dbo.sp_add_jobschedule 
			@job_id=@jobId, 
			@name=@JobName, 
			@enabled=1, 
			@freq_type=4, 
			@freq_interval=1, 
			@freq_subday_type=1, 
			@freq_subday_interval=0, 
			@freq_relative_interval=0, 
			@freq_recurrence_factor=0, 
			@active_start_date=20070511, 
			@active_end_date=99991231, 
			@active_start_time=20000, 
			@active_end_time=235959
	end
	
	if (NOT EXISTS(select server_id from msdb.dbo.sysjobservers where job_id = @jobId))
		EXEC msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
end

-- These two lines create the job when the repository is installed.
go
EXEC [p_CreateAggregationJob] @ForceRecreate = 1