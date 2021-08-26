if (object_id('p_AggregateTableGrowthData') is not null)
begin
drop procedure p_AggregateTableGrowthData
end
go
CREATE procedure p_AggregateTableGrowthData
as
begin
set nocount on
set ansi_warnings off

declare @ActivityCutoff datetime,
		@SQLServerID int, 
		@AggregateTableGrowthMonitorDays int,
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

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Starting Aggregation Table Growth', 0, null

declare @LoopServers 
table
(SQLServerID int)

insert into @LoopServers
select SQLServerID from MonitoredSQLServers where isnull(Deleted,0) = 0

select @SQLServerID = min(SQLServerID) from @LoopServers
	
select @AggregateTableGrowthMonitorDays = Internal_Value
from RepositoryInfo where Name = 'AggregateForecasting'

if (@AggregateTableGrowthMonitorDays is null)
	set @AggregateTableGrowthMonitorDays = 1095

while isnull(@SQLServerID,0) > 0
begin

	set ansi_warnings off
	
	set @ActivityCutoff = cast(floor(cast(dateadd(day, -(@AggregateTableGrowthMonitorDays), getdate()) as float)) as datetime)

	----------------------------------------------
	-- Begin TableGrowth Aggregation
	----------------------------------------------
	-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
	;with TableGrowthAggregationStepCte
	( 
	    [TableID],
	    [UTCCollectionDateTime],
		[NumberOfRows],
		[DataSize],
	    [TextSize],
		[IndexSize],
		[TimeDeltaInSeconds]
	)
	as
    ( select top (@RecordsToDelete)
	    tg.[TableID],
    	tg.[UTCCollectionDateTime],
		tg.[NumberOfRows],
		tg.[DataSize],
	    tg.[TextSize],
		tg.[IndexSize],
		tg.[TimeDeltaInSeconds]
    	from 
    		[TableGrowth] tg with (nolock)
    		Inner Join dbo.SQLServerTableNames tn with (nolock)
			on tg.TableID = tn.TableID
    		inner join dbo.SQLServerDatabaseNames dn with (nolock) 
    		on tn.DatabaseID = dn.DatabaseID
    		Where dn.SQLServerID = @SQLServerID
    		and tg.UTCCollectionDateTime <= @ActivityCutoff
	)
	insert into TableGrowthAggregation
	(
	   [MinUTCCollectionDateTime]
      ,[MaxUTCCollectionDateTime]
      ,[MinNumberOfRows]
      ,[MaxNumberOfRows]
      ,[TotalNumberOfRows]
      ,[MinDataSize]
      ,[MaxDataSize]
      ,[TotalDataSize]
      ,[MinTextSize]
      ,[MaxTextSize]
      ,[TotalTextSize]
      ,[MinIndexSize]
      ,[MaxIndexSize]
      ,[TotalIndexSize]
      ,[MinTimeDeltaInSeconds]
      ,[MaxTimeDeltaInSeconds]
      ,[TotalTimeDeltaInSeconds]
      ,[TableID]
	)
	select
        min(UTCCollectionDateTime),
		max(UTCCollectionDateTime),
		min([NumberOfRows]),
		max([NumberOfRows]),
		sum([NumberOfRows]),
	    Min([DataSize]),
		Max([DataSize]),
		sum([DataSize]),
	    Min([TextSize]),
		Max([TextSize]),
		sum([TextSize]),
		Min([IndexSize]),
		Max([IndexSize]),
		sum([IndexSize]),
		Min([TimeDeltaInSeconds]),
		Max([TimeDeltaInSeconds]),
		sum([TimeDeltaInSeconds]),
		TableID
	from TableGrowthAggregationStepCte
	group by TableID

	----------------------------------------------
	-- End TableGrowth Aggregation
	----------------------------------------------
	
	set @RC = 0

	select @InstanceName = InstanceName from MonitoredSQLServers where SQLServerID = @SQLServerID

	set ansi_warnings ON
	
	-- Groom TableGrowth
	exec Grooming.p_GroomTableGrowth
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
	---------------------------------------------

	delete from @LoopServers where SQLServerID = @SQLServerID
	select @SQLServerID = min(SQLServerID) from @LoopServers
end

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished with Aggregation Table Growth', 0, null	

end	