if (object_id('p_AggregateDatabaseSize') is not null)
begin
drop procedure p_AggregateDatabaseSize
end
go
create procedure p_AggregateDatabaseSize
as
begin
set nocount on
set ansi_warnings off

declare @ActivityCutoff datetime, 
		@SQLServerID int,  
		@AggregateDatabaseSizeMonitorDays int,
		@Now datetime, 
		@InstanceName nvarchar(256), 
		@RC int,
		@RowsAffected int,
		@run_id uniqueidentifier,
		@Sequence int,
		@TimeoutTime datetime,
		@RecordsToDelete int

set @Now = getdate()
set @TimeoutTime = dateadd(mi,120,getutcdate())
set @run_id = newid()
set @Sequence = 0		
set @RecordsToDelete = 5000

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Starting Aggregation Database Size', 0, null

declare @LoopServers 
table
(SQLServerID int)

insert into @LoopServers
select SQLServerID from MonitoredSQLServers where isnull(Deleted,0) = 0

select @SQLServerID = min(SQLServerID) from @LoopServers
	
select @AggregateDatabaseSizeMonitorDays = Internal_Value
from RepositoryInfo where Name = 'AggregateForecasting'

if (@AggregateDatabaseSizeMonitorDays is null)
	set @AggregateDatabaseSizeMonitorDays = 1095

while isnull(@SQLServerID,0) > 0
begin

	set ansi_warnings off
	
	set @ActivityCutoff = cast(floor(cast(dateadd(day, -(@AggregateDatabaseSizeMonitorDays), getdate()) as float)) as datetime)

	----------------------------------------------
	-- Begin DatabaseSize Aggregation
	----------------------------------------------
	-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
    ;with DatabaseSizeAggregationStepCte
    (
         [DatabaseID]
    	,[UTCCollectionDateTime]
        ,[DataFileSizeInKilobytes]
        ,[DatabaseStatus] 
        ,[LogFileSizeInKilobytes]
        ,[DataSizeInKilobytes] 
        ,[LogSizeInKilobytes] 
        ,[TextSizeInKilobytes] 
        ,[IndexSizeInKilobytes] 
        ,[LogExpansionInKilobytes]
        ,[DataExpansionInKilobytes] 
        ,[PercentLogSpace]
        ,[PercentDataSize] 
        ,[TimeDeltaInSeconds]
        ,[DatabaseStatisticsTime]
    ) as
    ( select top (@RecordsToDelete)
    	ds.[DatabaseID]
    	,ds.[UTCCollectionDateTime]
    	,ds.[DataFileSizeInKilobytes]
    	,ds.[DatabaseStatus] 
    	,ds.[LogFileSizeInKilobytes]
    	,ds.[DataSizeInKilobytes] 
    	,ds.[LogSizeInKilobytes] 
    	,ds.[TextSizeInKilobytes] 
    	,ds.[IndexSizeInKilobytes] 
    	,ds.[LogExpansionInKilobytes]
    	,ds.[DataExpansionInKilobytes] 
    	,ds.[PercentLogSpace]
    	,ds.[PercentDataSize] 
    	,ds.[TimeDeltaInSeconds]
    	,ds.[DatabaseStatisticsTime]
    	from 
    		[DatabaseSize] ds with (nolock)
    		Inner Join dbo.SQLServerDatabaseNames dn with (nolock) 
    		on ds.DatabaseID = dn.DatabaseID 
    		Where dn.SQLServerID = @SQLServerID
    		and ds.UTCCollectionDateTime <= @ActivityCutoff
    	)
		insert into DatabaseSizeAggregation
		(
		    [DatabaseID],
            [MinUTCCollectionDateTime],
            [MaxUTCCollectionDateTime],
            [MinDataFileSizeInKilobytes],
            [MaxDataFileSizeInKilobytes],
            [TotalDataFileSizeInKilobytes],
            [DatabaseStatus],
            [MinLogFileSizeInKilobytes],
            [MaxLogFileSizeInKilobytes],
            [TotalLogFileSizeInKilobytes],
            [MinDataSizeInKilobytes],
            [MaxDataSizeInKilobytes],
            [TotalDataSizeInKilobytes],
            [MinLogSizeInKilobytes],
            [MaxLogSizeInKilobytes],
            [TotalLogSizeInKilobytes],
            [MinTextSizeInKilobytes],
            [MaxTextSizeInKilobytes],
            [TotalTextSizeInKilobytes],
            [MinIndexSizeInKilobytes],
            [MaxIndexSizeInKilobytes],
            [TotalIndexSizeInKilobytes],
            [MinLogExpansionInKilobytes],
            [MaxLogExpansionInKilobytes],
            [TotalLogExpansionInKilobytes],
            [MinDataExpansionInKilobytes],
            [MaxDataExpansionInKilobytes],
            [TotalDataExpansionInKilobytes],
            [MinPercentLogSpace],
            [MaxPercentLogSpace],
            [TotalPercentLogSpace],
            [MinPercentDataSize],
            [MaxPercentDataSize],
            [TotalPercentDataSize],
            [MinTimeDeltaInSeconds],
            [MaxTimeDeltaInSeconds],
            [TotalTimeDeltaInSeconds],
            [MinDatabaseStatisticsTime],
            [MaxDatabaseStatisticsTime]
		)
    	select
    	  [DatabaseID]
    	  ,min([UTCCollectionDateTime])
    	  ,max([UTCCollectionDateTime])
    	  ,min([DataFileSizeInKilobytes])
    	  ,max([DataFileSizeInKilobytes])
    	  ,sum([DataFileSizeInKilobytes])
    	  ,[DatabaseStatus] 
    	  ,min([LogFileSizeInKilobytes])
    	  ,max([LogFileSizeInKilobytes])
    	  ,sum([LogFileSizeInKilobytes])
    	  ,min([DataSizeInKilobytes])
    	  ,max([DataSizeInKilobytes])
    	  ,sum([DataSizeInKilobytes])
    	  ,min([LogSizeInKilobytes] )
    	  ,max([LogSizeInKilobytes] )
    	  ,sum([LogSizeInKilobytes] )
    	  ,min([TextSizeInKilobytes] )
    	  ,max([TextSizeInKilobytes] )
    	  ,sum([TextSizeInKilobytes] )
    	  ,min([IndexSizeInKilobytes])
    	  ,max([IndexSizeInKilobytes])
    	  ,sum([IndexSizeInKilobytes])
    	  ,min([LogExpansionInKilobytes])
    	  ,max([LogExpansionInKilobytes])
    	  ,sum([LogExpansionInKilobytes])
    	  ,min([DataExpansionInKilobytes])
    	  ,max([DataExpansionInKilobytes])
    	  ,sum([DataExpansionInKilobytes])
    	  ,min([PercentLogSpace] )
          ,max([PercentLogSpace] )
          ,sum([PercentLogSpace] )
    	  ,min([PercentDataSize] )
    	  ,max([PercentDataSize])
    	  ,sum([PercentDataSize])
    	  ,min([TimeDeltaInSeconds]) 
    	  ,max([TimeDeltaInSeconds] )
    	  ,sum([TimeDeltaInSeconds] )
    	  ,min([DatabaseStatisticsTime])
    	  ,max([DatabaseStatisticsTime])
    	from DatabaseSizeAggregationStepCte
    	group by DatabaseID, DatabaseStatus

	---------------------------------------------
	--  End DatabaseSize Aggregation
	---------------------------------------------

	set @RC = 0

	select @InstanceName = InstanceName from MonitoredSQLServers where SQLServerID = @SQLServerID

	set ansi_warnings ON
	
	-- Groom Database Size
	exec Grooming.p_GroomDatabaseSize
		@run_id = @run_id,
		@Sequence = @Sequence out,
		@TimeoutTime = @TimeoutTime,
		@RecordsToDelete = @RecordsToDelete,
		@CutoffDateTime = @ActivityCutoff,
		@SQLServerID = @SQLServerID,
		@InstanceName = @InstanceName,
		@Deleted = 0,
		@AggregationGrooming = 1

	----------------------------------------------
	-- End DatabaseSize Grooming
	----------------------------------------------

	delete from @LoopServers where SQLServerID = @SQLServerID
	select @SQLServerID = min(SQLServerID) from @LoopServers
end	

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished with Aggregation Database Size', 0, null	
	
end	