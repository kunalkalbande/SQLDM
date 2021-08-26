if (object_id('Grooming.p_GroomRepositoryOrphanData') is not null)
begin
drop procedure Grooming.p_GroomRepositoryOrphanData
end
go
create procedure [Grooming].[p_GroomRepositoryOrphanData]
AS
begin

set nocount on

declare @StartTime datetime
declare @TimeoutTime datetime
declare @TimeoutMinutes int
declare @run_id uniqueidentifier
declare @Sequence int
declare @DeleteCount int
declare @RC int
declare @ErrorMessage nvarchar(2048)

set @DeleteCount = 0

begin try --Global

select 
	@StartTime = isnull(UTCActionEndDateTime,getutcdate()),
	@run_id = isnull(RunID,newid())
from 
GroomingLog
where UTCActionEndDateTime = 
	(select 
		max (UTCActionEndDateTime) 
	from 
	GroomingLog
	where Action = 'Started')
	
select @Sequence = max(Sequence) from GroomingLog where RunID = @run_id	

select @TimeoutMinutes = isnull(Internal_Value,180) from RepositoryInfo where [Name] = 'GroomingMaxNumberMinutes'	

set @TimeoutTime = dateadd(mi,@TimeoutMinutes,@StartTime)

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Starting Orphans', null, null

-- HostNames
exec @RC = Grooming.p_GroomHostNames
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 5000

select @DeleteCount = @DeleteCount + @RC
select @RC = 0 	
	
-- ApplicationNames
exec @RC = Grooming.p_GroomApplicationNames
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 5000
	
-- LoginNames
exec @RC = Grooming.p_GroomLoginNames
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 5000	

select @DeleteCount = @DeleteCount + @RC
select @RC = 0 

-- MetricMetaData
exec @RC = Grooming.p_GroomMetricMetaData
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 1000	

select @DeleteCount = @DeleteCount + @RC
select @RC = 0 

-- SQLSignatures
exec @RC = Grooming.p_GroomSQLSignatures
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 1000	

select @DeleteCount = @DeleteCount + @RC
select @RC = 0 

-- SQLStatements
exec @RC = Grooming.p_GroomSQLStatements
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 1000	

select @DeleteCount = @DeleteCount + @RC
select @RC = 0 


-- DatabaseNames
exec @RC = Grooming.p_GroomDatabaseNames
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 1000	

select @DeleteCount = @DeleteCount + @RC
select @RC = 0 

-- BaselineTemplates
exec @RC = Grooming.p_GroomBaselineTemplates
	@run_id = @run_id,
	@Sequence = @Sequence out,
	@TimeoutTime = @TimeoutTime,
	@RecordsToDelete = 1000	

select @DeleteCount = @DeleteCount + @RC
select @RC = 0 

-- Metric Threshold Instances
delete from MetricThresholdInstances 
	where InstanceID not in (select distinct ThresholdInstanceID from MetricThresholds)

set @RC = @@ROWCOUNT
select @DeleteCount = @DeleteCount + @RC
select @RC = 0

-- AnalysisConfigurations
delete from AnalysisConfiguration
	where MonitoredServerID not in (select SQLServerID from MonitoredSQLServers)
	or IsActive = 0

set @RC = @@ROWCOUNT
select @DeleteCount = @DeleteCount + @RC
select @RC = 0		

-- Default Metric Thresholds
delete from DefaultMetricThresholds
	where  ThresholdInstanceID not in (select distinct ThresholdInstanceID from MetricThresholds)
	and ThresholdInstanceID <> -1	
	
set @RC = @@ROWCOUNT
select @DeleteCount = @DeleteCount + @RC
select @RC = 0	

end try --Global
begin catch
	set @ErrorMessage = ERROR_MESSAGE()
	exec Grooming.p_LogGroomingAction @run_id, @Sequence out,  @ErrorMessage, @RC, null
	
end catch

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished Orphans', @DeleteCount, null


end

