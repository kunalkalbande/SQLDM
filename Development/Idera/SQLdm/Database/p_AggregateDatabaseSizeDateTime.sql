if (object_id('p_AggregateDatabaseSizeDateTime') is not null)
begin
drop procedure p_AggregateDatabaseSizeDateTime
end
go
CREATE procedure p_AggregateDatabaseSizeDateTime
as
begin
set nocount on
set ansi_warnings off

declare @ActivityCutoff datetime,
		@SQLServerID int, 
		@AggregateDatabaseSizeDateTimeMonitorDays int,
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

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Starting Aggregation Database Size DateTime', 0, null

declare @LoopServers 
table
(SQLServerID int)

insert into @LoopServers
select SQLServerID from MonitoredSQLServers where isnull(Deleted,0) = 0

select @SQLServerID = min(SQLServerID) from @LoopServers
	
select @AggregateDatabaseSizeDateTimeMonitorDays = Internal_Value
from RepositoryInfo where Name = 'AggregateForecasting'

if (@AggregateDatabaseSizeDateTimeMonitorDays is null)
	set @AggregateDatabaseSizeDateTimeMonitorDays = 1095

while isnull(@SQLServerID,0) > 0
begin

	set ansi_warnings off
		
	set @ActivityCutoff = cast(floor(cast(dateadd(day, -(@AggregateDatabaseSizeDateTimeMonitorDays), getdate()) as float)) as datetime)

	----------------------------------------------
	-- Begin TableGrowth Aggregation
	----------------------------------------------
	-- SQLDM-29448 (SQLDM 10.3) improving performance for Forecasting Aggregation
	;with DatabaseSizeDateTimeAggregationStepCte
    (
         [DatabaseID],
    	 [UTCCollectionDateTime]
    ) as
    ( select top (@RecordsToDelete)
	    dsdt.[DatabaseID], 
		dsdt.[UTCCollectionDateTime]
	from 
	    [DatabaseSizeDateTime] dsdt with (nolock)
	    inner join dbo.SQLServerDatabaseNames dn with (nolock) 
	    on dn.DatabaseID = dsdt.DatabaseID where SQLServerID = @SQLServerID
	    and dsdt.UTCCollectionDateTime <= @ActivityCutoff
	)
	insert into [DatabaseSizeDateTimeAggregation]
	(
	    [DatabaseID],
		[MinUTCCollectionDateTime],
		[MaxUTCCollectionDateTime]
	)
	select
	    [DatabaseID],
        min([UTCCollectionDateTime]),
	    max([UTCCollectionDateTime])
	from DatabaseSizeDateTimeAggregationStepCte
	group by [DatabaseID]

	----------------------------------------------
	-- End TableGrowth Aggregation
	----------------------------------------------
	
	set @RC = 0

	select @InstanceName = InstanceName from MonitoredSQLServers where SQLServerID = @SQLServerID

	set ansi_warnings ON
	
	-- Groom Database Size
	exec Grooming.p_GroomDatabaseSizeDateTime
		@run_id = @run_id,
		@Sequence = @Sequence out,
		@TimeoutTime = @TimeoutTime,
		@RecordsToDelete = @RecordsToDelete,
		@CutoffDateTime = @ActivityCutoff,
		@SQLServerID = @SQLServerID,
		@InstanceName = @InstanceName,
		@Deleted = 0,
		@AggregationGrooming = 1

	---------------------------------------------
	-- End TableGrowth Grooming
	---------------------------------------------


	delete from @LoopServers where SQLServerID = @SQLServerID
	select @SQLServerID = min(SQLServerID) from @LoopServers
end	

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished with Aggregation Database Size DateTime', 0, null	

end	