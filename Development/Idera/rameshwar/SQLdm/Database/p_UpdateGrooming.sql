if (object_id('p_UpdateGrooming') is not null)
begin
drop procedure [p_UpdateGrooming]
end
go

create procedure [p_UpdateGrooming]
	@AlertsDays int ,
	@MetricsDays int ,
	@TasksDays int ,
	@ActivityDays int,
	@AuditDays int,
	--10.0 SQLdm srishti purohit
    --Prescriptive analysis old data grooming implementation
	@PADays int,
	@StartTime int,
	@SubDayType int ,
	@QueriesDays int,
	@AggregationStartTime int,
	@AggregationSubDayType int,
	@ForecastingDays int,
	@FADays int
as
begin
	declare @rc int

	DELETE FROM RepositoryInfo WHERE Name in ('GroomAlerts', 'GroomMetrics', 'GroomTasks', 'GroomActivity','GroomQueryAggregation','GroomAudit', 'GroomPrescriptiveAnalysis', 'GroomForecasting','AggregateForecasting') 
	
	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('GroomAlerts', @AlertsDays);
	
	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('GroomMetrics', @MetricsDays);
	
	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('GroomTasks', @TasksDays);

	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('GroomActivity', @ActivityDays);

	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('GroomQueryAggregation', @QueriesDays);
	
	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('GroomAudit', @AuditDays);

	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('GroomPrescriptiveAnalysis', @PADays);
	
	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('GroomForecasting', @ForecastingDays);

		
	INSERT RepositoryInfo ([Name], Internal_Value)
	VALUES ('AggregateForecasting', @FADays);
	
	-- Now reconfigure the grooming job.
	DECLARE @schedname nvarchar(256)

	set @schedname = 'Groom ' + DB_NAME()
	-- sp_update_jobschedule is used for compatibility with SQL Server 2000
	
	if (@SubDayType is not null and @StartTime is not null)	
	begin
		if (@SubDayType = 1)
			EXEC @rc = msdb.dbo.sp_update_jobschedule 
				@job_name = @schedname, 
				@name = @schedname, 
				@enabled=1, 
				@freq_type=4, 
				@freq_interval=1, 
				@freq_subday_type=1, 
				@freq_subday_interval=0, 
				@freq_relative_interval=0, 
				@freq_recurrence_factor=0, 
				@active_start_time=@StartTime,
				@active_end_time=0,
				@active_start_date=20070511, 
				@active_end_date=99991231
		else
			EXEC @rc = msdb.dbo.sp_update_jobschedule 
				@job_name = @schedname, 
				@name = @schedname, 
				@enabled=1, 
				@freq_type=4, 
				@freq_interval=1, 
				@freq_subday_type=@SubDayType, 
				@freq_subday_interval=@StartTime, 
				@freq_relative_interval=0, 
				@freq_recurrence_factor=0, 
				@active_start_time=0,
				@active_end_time=0,
				@active_start_date=20070511, 
				@active_end_date=99991231
	end

	set @schedname = 'Aggregate Data ' + DB_NAME()
	-- sp_update_jobschedule is used for compatibility with SQL Server 2000
	
	if (@AggregationSubDayType is not null and @AggregationStartTime is not null)	
	begin
		if (@AggregationSubDayType = 1)
			EXEC @rc = msdb.dbo.sp_update_jobschedule 
				@job_name = @schedname, 
				@name = @schedname, 
				@enabled=1, 
				@freq_type=4, 
				@freq_interval=1, 
				@freq_subday_type=1, 
				@freq_subday_interval=0, 
				@freq_relative_interval=0, 
				@freq_recurrence_factor=0, 
				@active_start_time=@AggregationStartTime,
				@active_end_time=0,
				@active_start_date=20070511, 
				@active_end_date=99991231
		else
			EXEC @rc = msdb.dbo.sp_update_jobschedule 
				@job_name = @schedname, 
				@name = @schedname, 
				@enabled=1, 
				@freq_type=4, 
				@freq_interval=1, 
				@freq_subday_type=@AggregationSubDayType, 
				@freq_subday_interval=@AggregationStartTime, 
				@freq_relative_interval=0, 
				@freq_recurrence_factor=0, 
				@active_start_time=0,
				@active_end_time=0,
				@active_start_date=20070511, 
				@active_end_date=99991231
	end

	return @rc
end
