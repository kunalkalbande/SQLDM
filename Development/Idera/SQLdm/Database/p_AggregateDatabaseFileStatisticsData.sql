if (object_id('p_AggregateDatabaseFileStatisticsData') is not null)
begin
drop procedure p_AggregateDatabaseFileStatisticsData
end
go
CREATE procedure p_AggregateDatabaseFileStatisticsData
as
begin
set nocount on
set ansi_warnings off

declare @ActivityCutoff datetime,
		@SQLServerID int, 
		@AggregateDatabaseStatsMonitorDays int,
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

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Starting Aggregation Database File Statistics', 0, null
		
declare @LoopServers 
table
(SQLServerID int)

insert into @LoopServers
select SQLServerID from MonitoredSQLServers where isnull(Deleted,0) = 0

select @SQLServerID = min(SQLServerID) from @LoopServers
	
select @AggregateDatabaseStatsMonitorDays = Internal_Value
from RepositoryInfo where Name = 'AggregateForecasting'

if (@AggregateDatabaseStatsMonitorDays is null)
	set @AggregateDatabaseStatsMonitorDays = 1095

while isnull(@SQLServerID,0) > 0
begin

	set ansi_warnings off
	
	set @ActivityCutoff = cast(floor(cast(dateadd(day, -(@AggregateDatabaseStatsMonitorDays), getdate()) as float)) as datetime)

	----------------------------------------------
	-- Begin TableGrowth Aggregation
	----------------------------------------------
	-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
	;with DatabaseFileStatisticsAggregationStepCte
    (
        [FileStatisticsID]
        ,[UTCCollectionDateTime]
        ,[FileID]
        ,[MaxSize]
        ,[InitialSize]
        ,[UsedSpace]
        ,[AvailableSpace]
        ,[FreeDiskSpace]
        ,[DriveName]
    ) as
    ( select top (@RecordsToDelete)
        dfs.[FileStatisticsID]
        ,dfs.[UTCCollectionDateTime]
        ,dfs.[FileID]
        ,dfs.[MaxSize]
        ,dfs.[InitialSize]
        ,dfs.[UsedSpace]
        ,dfs.[AvailableSpace]
        ,dfs.[FreeDiskSpace]
        ,dfs.[DriveName]
	from 
		[DatabaseFileStatistics] dfs with (nolock)
        inner join dbo.DatabaseFiles df with (nolock) 
        on dfs.[FileID] = df.[FileID] 
        inner join dbo.SQLServerDatabaseNames dn with (nolock)
        on dn.[DatabaseID] = df.[DatabaseID]
        where [SQLServerID] = @SQLServerID
        and dfs.[UTCCollectionDateTime] <= @ActivityCutoff
	)
	insert into 
	DatabaseFileStatisticsAggregation
	(
	    [FileID]
	    ,[DriveName]
	    ,[MinUTCCollectionDateTime]
	    ,[MaxUTCCollectionDateTime]
	    ,[MinMaxSize]
	    ,[MaxMaxSize]
	    ,[TotalMaxSize]
	    ,[MinInitialSize]
	    ,[MaxInitialSize]
	    ,[TotalInitialSize]    
	    ,[MinUsedSpace]
	    ,[MaxUsedSpace]
	    ,[TotalUsedSpace]
	    ,[MinAvailableSpace]
	    ,[MaxAvailableSpace]
	    ,[TotalAvailableSpace]
	    ,[MinFreeDiskSpace]
	    ,[MaxFreeDiskSpace]
	    ,[TotalFreeDiskSpace]
	)
	select
	    [FileID]
	    ,[DriveName]
	    ,min(UTCCollectionDateTime)
		,max(UTCCollectionDateTime)
		,min([MaxSize])
		,max([MaxSize])
		,sum([MaxSize])
		,min([InitialSize]) 
		,max([InitialSize]) 
		,sum([InitialSize] )   
		,min([UsedSpace])
		,max([UsedSpace])
		,sum([UsedSpace])
		,min([AvailableSpace])
		,max([AvailableSpace])
		,sum([AvailableSpace])
		,min([FreeDiskSpace])
		,max([FreeDiskSpace])
		,sum([FreeDiskSpace])
	from DatabaseFileStatisticsAggregationStepCte
	group by [FileID], [DriveName]

	---------------------------------------------
	-- End TableGrowth Aggregation
	---------------------------------------------
		
	set @RC = 0

	select @InstanceName = InstanceName from MonitoredSQLServers where SQLServerID = @SQLServerID

	set ansi_warnings ON
	
	-- Groom Database Size
	exec Grooming.p_GroomDatabaseFileStatistics
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
	-- End TableGrowth Grooming
	----------------------------------------------

	delete from @LoopServers where SQLServerID = @SQLServerID
	select @SQLServerID = min(SQLServerID) from @LoopServers
end	
	
exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished with Aggregation Database File Statistics', 0, null
	
end	