if (object_id('p_GetGrooming') is not null)
begin
drop procedure p_GetGrooming
end
go
CREATE PROCEDURE [dbo].[p_GetGrooming](
	@ActivityOut int output,
	@AlertsOut int output,
	@MetricsOut int output,
	@TasksOut int output,
	@StartTime int output,
	@SubDayType int output,
	@AllowScheduleChange bit output,
	@AgentIsRunning bit output,
	@JobIsRunning bit output,
	@RepositoryTime DateTime output,
	@GroomingTimeLimit int output,
	@LastRunDate int output,
	@LastRunTime int output,
	@LastRunOutcome int output,
	@QueriesOut int output,
	@AggregationStartTime int output,
	@AggregationSubDayType int output,
	@AggregationAllowScheduleChange bit output,
	@AggregationJobIsRunning bit output,
	@AggregationLastRunDate int output,
	@AggregationLastRunTime int output,
	@AggregationLastRunOutcome int output,
	@AuditOut int output,
	@PADaysOut int output,
	@ForecastingOut int output,
	@FADaysOut int output
)
AS
BEGIN
	-- Initialize the output parms with default values.
	set @ActivityOut = 31
	set @AlertsOut = 31
	set @AuditOut = 90
	set @MetricsOut = 90 
	set @TasksOut = 7
	set @JobIsRunning = 0
	set @RepositoryTime = getdate()
	set @GroomingTimeLimit = 180
	set @QueriesOut = 14
	--10.0 SQLdm srishti purohit
    --Prescriptive analysis old data grooming implementation
	set @PADaysOut = 90
    set @FADaysOut = 14
	set @ForecastingOut = 3

	SELECT @ActivityOut = Internal_Value FROM RepositoryInfo WHERE [Name] = 'GroomActivity'
	SELECT @AlertsOut = Internal_Value FROM RepositoryInfo WHERE [Name] = 'GroomAlerts'
	SELECT @AuditOut = Internal_Value FROM RepositoryInfo WHERE [Name] = 'GroomAudit'
	SELECT @MetricsOut = Internal_Value FROM RepositoryInfo WHERE [Name] = 'GroomMetrics'
	SELECT @TasksOut = Internal_Value FROM RepositoryInfo WHERE [Name] = 'GroomTasks'
	SELECT @GroomingTimeLimit = Internal_Value from RepositoryInfo where [Name] = 'GroomingMaxNumberMinutes'	
	SELECT @QueriesOut = Internal_Value from RepositoryInfo where [Name] = 'GroomQueryAggregation'	
	SELECT @PADaysOut = Internal_Value FROM RepositoryInfo WHERE [Name] = 'GroomPrescriptiveAnalysis'
	SELECT @ForecastingOut = Internal_Value FROM RepositoryInfo WHERE [Name] = 'GroomForecasting'
	SELECT @FADaysOut = Internal_Value FROM RepositoryInfo WHERE [Name] = 'AggregateForecasting'
	if (@GroomingTimeLimit is null or @GroomingTimeLimit < 1)
		set @GroomingTimeLimit = 180

	exec p_GetSqlAgentRunning @AgentIsRunning output 
	
	-- Get the job schedule from the appropriate system table.
	DECLARE @jobName nvarchar(256)
	DECLARE @job_id  UNIQUEIDENTIFIER
	DECLARE @enabled int
	DECLARE @freq_type int
	DECLARE @freq_interval int
	DECLARE @freq_subday_type int
	DECLARE @freq_subday_interval int
	DECLARE @start_time int
	DECLARE @end_time int
	DECLARE @start_date int
	DECLARE @end_date int

	DECLARE @last_run_date int
	DECLARE @last_run_time int
	DECLARE @last_run_outcome int

---
	set @StartTime = 030000 -- hhmmss
	set @SubDayType = 1 -- once daily
	set @AllowScheduleChange = 0
	set @jobName = 'Groom ' + DB_NAME()
	set @job_id = null
	

	CREATE TABLE #temp_job_schedule2000 
	(
		schedule_id int,
		schedule_name nvarchar(256),
		[enabled] int,
		freq_type int,
		freq_interval int,
		freq_subday_type int,
		freq_subday_interval int,
		freq_relative_interval int,
		freq_recurrence_factor int,
		active_start_date int,
		active_end_date int,
		active_start_time int,
		active_end_time int,
		date_created DateTime,
		schedule_description nvarchar(256),
		next_run_date int,
		next_run_time int
	)

	CREATE TABLE #temp_job_schedule2005 
	(
		schedule_id int,
		schedule_name nvarchar(256),
		[enabled] int,
		freq_type int,
		freq_interval int,
		freq_subday_type int,
		freq_subday_interval int,
		freq_relative_interval int,
		freq_recurrence_factor int,
		active_start_date int,
		active_end_date int,
		active_start_time int,
		active_end_time int,
		date_created DateTime,
		schedule_description nvarchar(256),
		next_run_date int,
		next_run_time int,
		schedule_uid uniqueidentifier,
		job_count int	
	)

	create table #temp_get_grooming_xp_results  (
		job_id                UNIQUEIDENTIFIER NOT NULL,
		last_run_date         INT              NOT NULL,
		last_run_time         INT              NOT NULL,
		next_run_date         INT              NOT NULL,
		next_run_time         INT              NOT NULL,
		next_run_schedule_id  INT              NOT NULL,
		requested_to_run      INT              NOT NULL, -- BOOL
		request_source        INT              NOT NULL,
		request_source_id     sysname          COLLATE database_default NULL,
		running               INT              NOT NULL, -- BOOL
		current_step          INT              NOT NULL,
		current_retry_attempt INT              NOT NULL,
		job_state             INT              NOT NULL)

	SELECT @job_id = job_id FROM msdb.dbo.sysjobs WHERE [name] = @jobName
	IF @job_id IS NOT NULL
	BEGIN
		if (object_id('msdb.dbo.sysschedules') is null)
		BEGIN
			INSERT #temp_job_schedule2000 
			   exec msdb.dbo.sp_help_jobschedule @job_name = @jobName			
			SELECT 
				 @enabled = [enabled],
			     @freq_type = freq_type,
				 @freq_interval = freq_interval,
				 @freq_subday_type = freq_subday_type,
				 @freq_subday_interval = freq_subday_interval,
				 @start_time = active_start_time,
				 @end_time = active_end_time,
				 @start_date = active_start_date,
				 @end_date = active_end_date
				 FROM #temp_job_schedule2000
		END
		ELSE
		BEGIN
			Select @enabled = S.[enabled],
				@freq_type = S.freq_type,
				@freq_interval = S.freq_interval,
				@freq_subday_type = S.freq_subday_type,
				@freq_subday_interval = S.freq_subday_interval,
				@start_time = S.active_start_time,
				@end_time = S.active_end_time,
				@start_date = S.active_start_date,
				@end_date = S.active_end_date
			From msdb.dbo.sysjobschedules JS
			Inner Join msdb.dbo.sysjobs J On J.job_id = JS.job_id
			Inner Join msdb.dbo.sysschedules S On S.schedule_id = JS.schedule_id
			Where J.name = @jobName;
		END

		set @SubDayType = @freq_subday_type
		-- schedule change allowed in ui if job frequency is daily and job interval is unused.  also required is that if
		-- job is not once daily it must be hourly and start at midnight and end by 11:59:59.
		if (@freq_type = 4 and @freq_interval = 1) 
		begin
			if (@freq_subday_type = 1)
			begin
				set @StartTime  = @start_time
				set @AllowScheduleChange = 1
			end
			else
			begin
				set @StartTime  = @freq_subday_interval
				if (@freq_subday_type = 8 and @start_time = 0 and @end_time = 235959)
					set @AllowScheduleChange = 1
			end	
			-- if start/end date is set recognize as changed outside of DM.
			if (@start_date <> 20070511 or @end_date <> 99991231)
				set @AllowScheduleChange = 0
			else if (@enabled = 0) -- job schedule is disabled
				set @AllowScheduleChange = 0	
		end
		else
			set @StartTime = @start_time
			
		if @AgentIsRunning = 1
		BEGIN
			-- Since the agent is running, the job might be running.
			-- Call a system stored proc to find out.
			-- Can't use a table variable with "insert into exec" in 2000,
			-- so use a temp table.
			
			insert #temp_get_grooming_xp_results exec master.dbo.xp_sqlagent_enum_jobs @is_sysadmin = 1, @job_owner = ''
			if EXISTS (SELECT 1 from #temp_get_grooming_xp_results where job_id = @job_id AND (running = 1 OR requested_to_run = 1))
				SELECT @JobIsRunning = 1
		END
	END

	IF (EXISTS (SELECT * FROM msdb.dbo.systargetservers))
	BEGIN
		SELECT	@last_run_date = sjs.last_run_date,
				@last_run_time = sjs.last_run_time,
				@last_run_outcome = sjs.last_run_outcome
		FROM  msdb.dbo.sysjobservers sjs
		WHERE (CONVERT(FLOAT, sjs.last_run_date) * 1000000) + sjs.last_run_time =
				(SELECT MAX((CONVERT(FLOAT, last_run_date) * 1000000) + last_run_time)
					FROM msdb.dbo.sysjobservers
					WHERE (job_id = sjs.job_id)) 
				AND (sjs.job_id = @job_id)
	END
	ELSE
	BEGIN
		SELECT	@last_run_date = sjs.last_run_date,
				@last_run_time = sjs.last_run_time,
				@last_run_outcome = sjs.last_run_outcome
		FROM msdb.dbo.sysjobservers sjs
		WHERE (sjs.job_id = @job_id)
	END

	set @LastRunDate	= @last_run_date
	set @LastRunTime	= @last_run_time
	set @LastRunOutcome = @last_run_outcome

---
	set @AggregationStartTime = 020000 -- hhmmss
	set @AggregationSubDayType = 1 -- once daily
	set @AggregationAllowScheduleChange = 0
	set @jobName = 'Aggregate Data ' + DB_NAME()
	set @job_id = null

	truncate table #temp_job_schedule2000
	truncate table #temp_job_schedule2005
	truncate table #temp_get_grooming_xp_results

	
	SELECT @job_id = job_id FROM msdb.dbo.sysjobs WHERE [name] = @jobName
	IF @job_id IS NOT NULL
	BEGIN
		if (object_id('msdb.dbo.sysschedules') is null)
		BEGIN
			
			INSERT #temp_job_schedule2000 
			   exec msdb.dbo.sp_help_jobschedule @job_name = @jobName			
			SELECT 
				 @enabled = [enabled],
			     @freq_type = freq_type,
				 @freq_interval = freq_interval,
				 @freq_subday_type = freq_subday_type,
				 @freq_subday_interval = freq_subday_interval,
				 @start_time = active_start_time,
				 @end_time = active_end_time,
				 @start_date = active_start_date,
				 @end_date = active_end_date
				 FROM #temp_job_schedule2000
		END
		ELSE
		BEGIN
			Select @enabled = S.[enabled],
				@freq_type = S.freq_type,
				@freq_interval = S.freq_interval,
				@freq_subday_type = S.freq_subday_type,
				@freq_subday_interval = S.freq_subday_interval,
				@start_time = S.active_start_time,
				@end_time = S.active_end_time,
				@start_date = S.active_start_date,
				@end_date = S.active_end_date
			From msdb.dbo.sysjobschedules JS
			Inner Join msdb.dbo.sysjobs J On J.job_id = JS.job_id
			Inner Join msdb.dbo.sysschedules S On S.schedule_id = JS.schedule_id
			Where J.name = @jobName;
		END

		set @AggregationSubDayType = @freq_subday_type
		-- schedule change allowed in ui if job frequency is daily and job interval is unused.  also required is that if
		-- job is not once daily it must be hourly and start at midnight and end by 11:59:59.
		if (@freq_type = 4 and @freq_interval = 1) 
		begin
			if (@freq_subday_type = 1)
			begin
				set @AggregationStartTime  = @start_time
				set @AggregationAllowScheduleChange = 1
			end
			else
			begin
				set @AggregationStartTime  = @freq_subday_interval
				if (@freq_subday_type = 8 and @start_time = 0 and @end_time = 235959)
					set @AggregationAllowScheduleChange = 1
			end	
			-- if start/end date is set recognize as changed outside of DM.
			if (@start_date <> 20070511 or @end_date <> 99991231)
				set @AggregationAllowScheduleChange = 0
			else if (@enabled = 0) -- job schedule is disabled
				set @AggregationAllowScheduleChange = 0	
		end
		else
			set @AggregationStartTime = @start_time
			
		if @AgentIsRunning = 1
		BEGIN
			-- Since the agent is running, the job might be running.
			-- Call a system stored proc to find out.
			-- Can't use a table variable with "insert into exec" in 2000,
			-- so use a temp table.

			insert #temp_get_grooming_xp_results exec master.dbo.xp_sqlagent_enum_jobs @is_sysadmin = 1, @job_owner = ''
			if EXISTS (SELECT * from #temp_get_grooming_xp_results where job_id = @job_id AND (running = 1 OR requested_to_run = 1))
				SELECT @AggregationJobIsRunning = 1
		END
	END

	IF (EXISTS (SELECT * FROM msdb.dbo.systargetservers))
	BEGIN
		SELECT	@last_run_date = sjs.last_run_date,
				@last_run_time = sjs.last_run_time,
				@last_run_outcome = sjs.last_run_outcome
		FROM  msdb.dbo.sysjobservers sjs
		WHERE (CONVERT(FLOAT, sjs.last_run_date) * 1000000) + sjs.last_run_time =
				(SELECT MAX((CONVERT(FLOAT, last_run_date) * 1000000) + last_run_time)
					FROM msdb.dbo.sysjobservers
					WHERE (job_id = sjs.job_id)) 
				AND (sjs.job_id = @job_id)
	END
	ELSE
	BEGIN
		SELECT	@last_run_date = sjs.last_run_date,
				@last_run_time = sjs.last_run_time,
				@last_run_outcome = sjs.last_run_outcome
		FROM msdb.dbo.sysjobservers sjs
		WHERE (sjs.job_id = @job_id)
	END

	set @AggregationLastRunDate	= @last_run_date
	set @AggregationLastRunTime	= @last_run_time
	set @AggregationLastRunOutcome = @last_run_outcome
END


