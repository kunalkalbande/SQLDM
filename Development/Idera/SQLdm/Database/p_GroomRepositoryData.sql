
if (object_id('Grooming.p_GroomRepositoryData') is not null)
begin
drop procedure Grooming.p_GroomRepositoryData
end
go
create procedure [Grooming].[p_GroomRepositoryData]
AS
begin

set nocount on

declare @ActivityCutoff datetime
declare @DeleteCount bigint
declare @RowsAffected int
declare @run_id uniqueidentifier	
declare @Sequence int	
declare @RC int
declare @InstanceName nvarchar(256)

declare @TimeoutMinutes int
declare @TimeoutTime DateTime	
declare @ErrorMessage nvarchar(2048)
declare @RecordsToDelete int

declare @SQLServerID int
declare @Active bit
declare @Deleted bit
declare @GroomActivity int
declare @GroomAlerts int
declare @GroomMetrics int
declare @GroomTasks int
declare @ActivityDays int
declare @AlertsDays int
declare @MetricsDays int 
declare @TasksDays int
declare @StartTime int
declare @SubDayType int
declare @AllowScheduleChange bit
declare @AgentIsRunning bit
declare @JobIsRunning bit
declare @RepositoryTime DateTime
declare @AlertCutoff DateTime
declare @MetricCutoff DateTime
declare @TaskCutoff DateTime
declare @Now DateTime
declare @LastReorg DateTime
declare @LastRunTime int
declare @LastRunDate int
declare @LastRunOutcome int
declare @QueriesOut int
declare @AggregationStartTime int
declare @AggregationSubDayType int
declare @AggregationAllowScheduleChange bit
declare @AggregationJobIsRunning bit
declare @AggregationLastRunDate int
declare @AggregationLastRunTime int
declare @AggregationLastRunOutcome int
declare @AuditDays int
declare @AuditCutoff DateTime
declare @GroomAudit int
-- START : SQLdm 10.0 (srishti) To support grooming
declare @PADays int
declare @PrescriptiveAnalysisCutoff DateTime
declare @GroomPrescriptiveAnalysis int
-- END : SQLdm 10.0 (srishti) To support grooming
declare @ForecastingDays int
declare @FADaysOut int
declare @ForecastingCutoff DateTime
declare @GroomForecasting int

--Start- SQLdm 9.0 - Grooming Time out- Declared the currdate variable and initialized with the current date time
declare @currDate datetime 
SET @currDate=getutcdate()
--End- SQLdm 9.0 - Grooming Time out- Declared the currdate variable and initialized with the current date time

set @run_id = newid()
set @DeleteCount = 0
set @Sequence = 0;

--Start- SQLdm 9.0 - Grooming Time out- Declared the table to collect already groomed Instance's ids and shifted the @groomingServers table declaration before TRY starts
declare @GroomedServerIds
	table( SQLServerID int)

declare @GroomingServers
	table(
	RowID int identity,
	SQLServerID int,
	InstanceName nvarchar(256), 
	Active bit,
	Deleted bit,
	GroomActivity int,
	GroomAlerts int,
	GroomMetrics int,
	GroomTasks int,
	GroomAudit int,
	GroomPrescriptiveAnalysis int,
	GroomForecasting int)	
--End- SQLdm 9.0 - Grooming Time out- Declared the table to collect already groomed Instance's ids and shifted the @groomingServers table declaration before TRY starts

begin try --Global


-- Get the age in days at which alerts, metrics, and tasks should be deleted
-- and convert those ages to DateTimes relative to now.  These are defaults
-- that may be overridden on a per-server basis.
exec [p_GetGrooming] 
	@ActivityDays output, 
	@AlertsDays output, 
	@MetricsDays output, 
	@TasksDays output, 
	@StartTime output,
	@SubDayType output,
	@AllowScheduleChange output, 
	@AgentIsRunning output, 
	@JobIsRunning output, 
	@RepositoryTime output,
	@TimeoutMinutes output,
	@LastRunDate output,
	@LastRunTime output,
	@LastRunOutcome output,
	@QueriesOut output,
	@AggregationStartTime output,
	@AggregationSubDayType output,
	@AggregationAllowScheduleChange output,
	@AggregationJobIsRunning output,
	@AggregationLastRunDate output,
	@AggregationLastRunTime output,
	@AggregationLastRunOutcome output,
	@AuditDays output,
	@PADays output,
	@ForecastingDays output,
	@FADaysOut output
	
select @Now = getutcdate()
set @TimeoutTime = dateadd(mi,@TimeoutMinutes,@Now)

exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,'Started', null, null

exec Grooming.p_GroomGroomingLog
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 1000
	

insert into @GroomingServers (SQLServerID, InstanceName, Active, Deleted, GroomActivity, GroomAlerts, GroomMetrics, GroomTasks, GroomAudit, GroomPrescriptiveAnalysis, GroomForecasting )
select 
	SQLServerID, 
	InstanceName, 
	Active, 
	Deleted, 
	GroomActivity, 
	GroomAlerts, 
	GroomMetrics, 
	GroomTasks,
	GroomAudit,
	GroomPrescriptiveAnalysis,
	@GroomForecasting
from MonitoredSQLServers 
	
		
if not exists(
	select SQLServerID from @GroomingServers 
	where GroomActivity > -1 or GroomAlerts > -1 or GroomMetrics > -1 or GroomTasks > -1 or GroomAudit > -1 or GroomPrescriptiveAnalysis > -1 or GroomForecasting > -1)
begin
	delete from @GroomingServers where Deleted = 0
	insert into @GroomingServers(SQLServerID, InstanceName, Active, Deleted, GroomActivity, GroomAlerts, GroomMetrics, GroomTasks,GroomAudit, GroomPrescriptiveAnalysis, GroomForecasting ) 
		select null,null,1,0,-1,-1,-1,-1,-1,-1,-1
end

declare @LoopRowID int

select @LoopRowID = max(RowID) from @GroomingServers

while isnull(@LoopRowID,0) > 0
begin

	select @Deleted = Deleted from @GroomingServers where RowID = @LoopRowID
	
	

	-- Each active server can override the default age cutoff as follows.  
	-- If the server value = -1, use the global default determined above.  
	-- If the server value > -1, use the server value instead of the default.
	-- If the server value < -1, do not groom that server.  
	-- All tasks, alerts, and events are deleted for inactive servers.  
	-- Left joins are used in the DELETEs to groom rows that are somehow not 
	-- associated with any server.  

	select @ActivityCutoff = dateadd(day, -@ActivityDays, @Now)
	select @AlertCutoff = dateadd(day, -@AlertsDays, @Now)
	select @MetricCutoff = dateadd(day, -@MetricsDays, @Now)
	select @TaskCutoff = dateadd(day, -@TasksDays, @Now)
	select @AuditCutoff = dateadd(day, -@AuditDays, @Now)
	select @PrescriptiveAnalysisCutoff = dateadd(day, -@PADays, @Now)
	select @ForecastingCutoff = DATEADD(day, -@ForecastingDays, @Now)
	
	
	select 
		@SQLServerID = SQLServerID,
		@InstanceName = InstanceName,
		@GroomActivity = GroomActivity,
		@GroomAlerts = GroomAlerts,
		@GroomTasks = GroomTasks, 
		@GroomMetrics = GroomMetrics,
		@Active = Active,
		@GroomAudit = GroomAudit,
		@GroomPrescriptiveAnalysis = GroomPrescriptiveAnalysis,
		@GroomForecasting = GroomForecasting
	from @GroomingServers where RowID = @LoopRowID		
	
	if (@Deleted = 0)
	begin
		if (@GroomActivity > -1)
			select @ActivityCutoff = dateadd(day, -@GroomActivity, @Now)
		if (@GroomAlerts > -1)
			select @AlertCutoff = dateadd(day, -@GroomAlerts, @Now)
		if (@GroomTasks > -1)
			select @TaskCutoff = dateadd(day, -@GroomTasks, @Now)
		if (@GroomMetrics > -1)
			select @MetricCutoff = dateadd(day, -@GroomMetrics, @Now)
		if (@GroomAudit > -1)
			select @AuditCutoff = dateadd(day, -@GroomAudit, @Now)
		if (@GroomPrescriptiveAnalysis > -1)
			select @PrescriptiveAnalysisCutoff = dateadd(day, -@GroomPrescriptiveAnalysis, @Now)
		if (@GroomForecasting > -1)
			select @ForecastingCutoff = DATEADD(day, -@GroomForecasting, @Now)
	end

	
	if (@GroomTasks >= -1 or @Deleted = 1)
	begin

	-- Tasks
	
		exec @RC = Grooming.p_GroomTasks
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @TaskCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Active -- Different for this table	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	end
	else -- Not grooming Tasks
	begin
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,'Skipping Tasks grooming', null, @InstanceName
	end
	
	if (@GroomAlerts >= -1 or @Deleted = 1)
	begin
	
	-- Alerts
	
		exec @RC = Grooming.p_GroomAlerts
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @AlertCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	
	end
	else -- Not grooming Alerts
	begin
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,'Skipping Alerts grooming', null, @InstanceName
	end

	if (@GroomAudit >= -1 or @Deleted = 1)
	begin 
		-- AuditedEvents

		exec @RC = Grooming.p_GroomAuditedEvents
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @AuditCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	end
	else -- Not grooming Alerts
	begin
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,'Skipping Audit grooming', null, @InstanceName
	end

	if (@GroomPrescriptiveAnalysis >= -1 or @Deleted = 1)
	begin 
		-- AuditedEvents

		exec @RC = Grooming.p_GroomPrescriptiveAnalysisData
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @PrescriptiveAnalysisCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	end
	else -- Not grooming Alerts
	begin
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,'Skipping Prescriptive Analysis grooming', null, @InstanceName
	end

	if (@GroomMetrics >= -1 or @Deleted = 1)
	begin
	-- OSStatistics
	
		exec @RC = Grooming.p_GroomOSStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
	-- ServerStatistics

		exec @RC = Grooming.p_GroomServerStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
	-- DiskDrives

		exec @RC = Grooming.p_GroomDiskDrives
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	
	if (object_id('DatabaseStatistics_upgrade') is not null)
	begin
	
		exec @RC = Grooming.p_GroomDatabaseStatistics_upgrade
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	end
		
	-- DatabaseStatistics
	
		exec @RC = Grooming.p_GroomDatabaseStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
	-- stageDatabaseStatistics
		declare @Yesterday datetime
		select @Yesterday = dateadd(d,-1,getdate())
		exec @RC = Grooming.p_GroomDatabaseStatisticsStaging
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 1000,
			@CutoffDateTime = @Yesterday,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 			
	
	
		
	-- DatabaseFiles
	
		exec @RC = Grooming.p_GroomDatabaseFiles
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	
	
	-- MirroringStatistics
	
		exec @RC = Grooming.p_GroomMirroringStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	
	-- ReplicationTopology
	
		exec @RC = Grooming.p_GroomReplicationTopology
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
	
		
	-- TableReorganization
	
		exec @RC = Grooming.p_GroomTableReorganization
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
	-- CustomCounterStatistics
	
		exec @RC = Grooming.p_GroomCustomCounterStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	
	-- Blocks (This will clear all parented blocking sessions too)
	
		exec @RC = Grooming.p_GroomBlocks
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted		
			
	-- BlockingSessionStatistics
	
		exec @RC = Grooming.p_GroomBlockingSessionStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 	
		
	-- VMConfigData
	
		exec @RC = Grooming.p_GroomVMConfigData
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 			
		
		
		
	-- ESXConfigData
	
		exec @RC = Grooming.p_GroomESXConfigData
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 	
	
	-- VMStatistics
	
		exec @RC = Grooming.p_GroomVMStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 			

	-- ESXStatistics
	
		exec @RC = Grooming.p_GroomESXStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 		
		
	-- BaselineStatistics
		
		exec @RC = Grooming.p_GroomBaselineStatistics	
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 	

	--AlwaysOnStatistics

		exec @RC = Grooming.p_GroomAlwaysOnStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,  --I made the limit 5000 because there are no foreign keys and this is only captured once per server and only for SQL Server 2012+
			@CutoffDateTime = @MetricCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC		
		select @RC = 0 
	end
	else -- Not grooming Metrics
	begin
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,'Skipping Metrics grooming', null, @InstanceName
	end
	
	
	if (@GroomActivity >= -1 or @Deleted = 1)
	begin
	
	
		-- TempdbFileData

		exec @RC = Grooming.p_GroomTempdbFileData
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
		
		-- ServerActivity	
		exec @RC = Grooming.p_GroomServerActivity 
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 1000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

		-- Legacy Query Monitor
		
		exec @RC = Grooming.p_GroomLegacyQueryMonitor 
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 1000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

		-- QueryMonitorStatistics
		exec @RC = Grooming.p_GroomQueryMonitorStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 3000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 


	-- QuerySignatureAggregation

		exec @RC = Grooming.p_GroomQuerySignatureAggregation
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 3000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	-- START SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
    -- DatabaseSizeAggregation

		exec @RC = Grooming.p_GroomDatabaseSizeAggregation
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 3000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	-- DiskDriveStatisticsAggregation

		exec @RC = Grooming.p_GroomDiskDriveStatisticsAggregation
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 3000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	-- DatabaseFileStatisticsAggregation

		exec @RC = Grooming.p_GroomDatabaseFileStatisticsAggregation
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 3000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	-- TableGrowthAggregation

		exec @RC = Grooming.p_GroomTableGrowthAggregation
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 3000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	-- DatabaseSizeDateTimeAggregation

		exec @RC = Grooming.p_GroomDatabaseSizeDateTimeAggregation
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 3000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

	-- DatabaseStatisticsAggregation

		exec @RC = Grooming.p_GroomDatabaseStatisticsAggregation
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 3000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	-- END SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
    	
	-- Deadlocks 

		exec @RC = Grooming.p_GroomDeadlocks
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 1000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
	-- WaitStatistics

		exec @RC = Grooming.p_GroomWaitStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 1000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	
	--  ActiveWaitStatistics
	
		exec @RC = Grooming.p_GroomActiveWaitStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 1000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
	
		-- DatabaseFileActivity

		exec @RC = Grooming.p_GroomDatabaseFileActivity
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @ActivityCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
	end
	else -- Not grooming Activity
	begin
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,'Skipping Activity grooming', null, @InstanceName
	end

	if (@GroomForecasting >= -1 or @Deleted = 1)
	begin

		-- DatabaseFileStatistics
		exec @RC = Grooming.p_GroomDatabaseFileStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @ForecastingCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted
			
		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

		-- TableGrowth
	
		exec @RC = Grooming.p_GroomTableGrowth
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @ForecastingCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC

		--DiskDriveStatistics
		exec @RC = Grooming.p_GroomDiskDriveStatistics
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @ForecastingCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
	

		-- DatabaseSize
	
		exec @RC = Grooming.p_GroomDatabaseSize
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @ForecastingCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 

		--DatabaseSizeDateTime
		exec @RC = Grooming.p_GroomDatabaseSizeDateTime
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@RecordsToDelete = 5000,
			@CutoffDateTime = @ForecastingCutoff,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	

		select @DeleteCount = @DeleteCount + @RC
		select @RC = 0 
		
	end

	
	-- MonitoredSQLServers	
	if (@SQLServerID is not null and @Deleted = 1)
	begin
		exec @RC = Grooming.p_GroomMonitoredSQLServers
			@run_id = @run_id,
			@Sequence = @Sequence out,
			@TimeoutTime = @TimeoutTime,
			@SQLServerID = @SQLServerID,
			@InstanceName = @InstanceName,
			@Deleted = @Deleted	
	end
	
	select @Deleted = Deleted from @GroomingServers where RowID = @LoopRowID
	
	INSERT INTO @GroomedServerIds Select SQLServerID from @GroomingServers where RowID = @LoopRowID -- SQLdm 9.0 (Ankit Srivastava) collect the Server IDs which are groomed successfully
	delete from @GroomingServers where RowID = @LoopRowID
	select @LoopRowID = max(RowID) from @GroomingServers
end	

-- Global Tasks
	
	exec @RC = Grooming.p_GroomTasks
		@run_id = @run_id,
		@Sequence = @Sequence out,
		@TimeoutTime = @TimeoutTime,
		@RecordsToDelete = 5000,
		@CutoffDateTime = @TasksDays,
		@SQLServerID = -1
		
	select @DeleteCount = @DeleteCount + @RC
	select @RC = 0 
	
-- Global Alerts

	exec @RC = Grooming.p_GroomAlerts
		@run_id = @run_id,
		@Sequence = @Sequence out,
		@TimeoutTime = @TimeoutTime,
		@RecordsToDelete = 5000,
		@CutoffDateTime = @AlertsDays,
		@SQLServerID = -1
		
	select @DeleteCount = @DeleteCount + @RC
	select @RC = 0 

end try --Global
begin catch

	set @ErrorMessage = ERROR_MESSAGE()
	exec Grooming.p_LogGroomingAction @run_id, @Sequence out,  @ErrorMessage, @RC, @InstanceName	

-- Start - SQLdm 9.0 (Ankit Srivastava) - Grooming Time out -- Logging the status and message of failed Instances
	Declare @isPrimary bit
	Select @isPrimary=1

	WHILE EXISTS (Select * from @GroomingServers)
	BEGIN
	
		exec [dbo].[p_AddLatestGroomingStatus] @SQLServerID,@run_id,@currDate,0,@ErrorMessage,@isPrimary

		Delete from @GroomingServers where SQLServerID=@SQLServerID or SQLServerID is null

		Select @SQLServerID=max(SQLServerID) from @GroomingServers where SQLServerID is not null
		Select @isPrimary=0
	END
-- End - SQLdm 9.0 (Ankit Srivastava) - Grooming Time out -- Logging the status and message of failed Instances

end catch

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished Metrics', @DeleteCount, @InstanceName

-- Start - SQLdm 9.0 (Ankit Srivastava) - Grooming Time out -- Logging the Grooming status of successfully groomed Instances

	WHILE EXISTS (Select * from @GroomedServerIds)
	BEGIN
		Select @SQLServerID=max(SQLServerID) from @GroomedServerIds where SQLServerID is not null

		if exists(Select * from MonitoredSQLServers where SQLServerID=@SQLServerID)
		begin
			exec [dbo].[p_AddLatestGroomingStatus] @SQLServerID,@run_id,@currDate,1,'Grooming Completed successfully',1
		end
		else
		begin
			DELETE FROM LatestGroomingStatus where SQLServerID=@SQLServerID or SQLServerID is null
		end

		Delete from @GroomedServerIds where SQLServerID=@SQLServerID or SQLServerID is null
	END
-- End - SQLdm 9.0 (Ankit Srivastava) - Grooming Time out -- Logging the Grooming status of successfully groomed Instances

end
