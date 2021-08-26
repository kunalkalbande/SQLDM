if (object_id('p_UpgradeDatabaseStatistics') is not null)
begin
drop procedure [p_UpgradeDatabaseStatistics]
end
go
IF (OBJECT_ID('DatabaseStatistics_upgrade') IS NULL)
begin
exec('
create procedure [p_UpgradeDatabaseStatistics]
	@RowsRemaining bigint output
as
begin
	set @RowsRemaining = 0
	return @RowsRemaining
end')
end
else
begin
exec('
create procedure [p_UpgradeDatabaseStatistics]
	@RowsRemaining bigint output
as	
begin
if exists(select top 1 * from DatabaseStatistics_upgrade)
begin

	declare @DatabaseID int, @MaxDate datetime, @LoopDate datetime, @delay char(12), @TimeoutTime datetime
	
	-- The service has a 10 minute timeout
	-- Since this timeout is being checked in a loop and we could get unlucky on timing, set internal timeout to 5 minutes
	-- Timeout has no bad effects - will just return to service and restart where it left off
	select @TimeoutTime = dateadd(mi,5,GetUTCDate())
	
	set statistics io off
	select @DatabaseID = min(DatabaseID) from DatabaseStatistics_upgrade
	select @LoopDate = min(UTCCollectionDateTime), @MaxDate = max(UTCCollectionDateTime) from DatabaseStatistics_upgrade where DatabaseID = @DatabaseID
	
	if not exists(select top 1 Character_Value from RepositoryInfo where Name = ''DBUpgradeDelay'')
		insert into RepositoryInfo(Name,Internal_Value,Character_Value) values (''DBUpgradeDelay'',null,''00:00:00.01'')
		
	select @delay = Character_Value from RepositoryInfo where Name = ''DBUpgradeDelay'' 
	
	while (1=1)
	begin
		-- Timeout if necessary
		if (GetUTCDate() > @TimeoutTime)           
			break;
			
		-- Prevent excessive looping
		waitfor delay @delay
		
		set @LoopDate = dateadd(dd,1,@LoopDate)

		insert into DatabaseSize(
			[DatabaseID],
			[UTCCollectionDateTime],
			[DatabaseStatus],
			[DataFileSizeInKilobytes],
			[LogFileSizeInKilobytes],
			[DataSizeInKilobytes],
			[LogSizeInKilobytes],
			[TextSizeInKilobytes],
			[IndexSizeInKilobytes],
			[LogExpansionInKilobytes],
			[DataExpansionInKilobytes],
			[PercentLogSpace],
			[PercentDataSize],
			[TimeDeltaInSeconds],
			[DatabaseStatisticsTime]
			)
		select 
			[DatabaseID],
			[UTCCollectionDateTime],
			[DatabaseStatus],
			[DataFileSizeInKilobytes],
			[LogFileSizeInKilobytes],
			[DataSizeInKilobytes],
			[LogSizeInKilobytes],
			[TextSizeInKilobytes],
			[IndexSizeInKilobytes],
			[LogExpansionInKilobytes],
			[DataExpansionInKilobytes],
			[PercentLogSpace],
			[PercentDataSize],
			[TimeDeltaInSeconds],
			[UTCCollectionDateTime]
		from DatabaseStatistics_upgrade 
		where
			DatabaseID = @DatabaseID
			and UTCCollectionDateTime <= @LoopDate
		option (recompile)

		insert into DatabaseStatistics(
			[DatabaseID],
			[UTCCollectionDateTime],
			[DatabaseStatus],
			[Transactions],
			[LogFlushWaits],
			[LogFlushes],
			[LogKilobytesFlushed],
			[LogCacheReads],
			[LogCacheHitRatio],
			[TimeDeltaInSeconds],
			[NumberReads],
			[NumberWrites],
			[BytesRead],
			[BytesWritten],
			[IoStallMS],
			[DatabaseSizeTime]
			)
		select
			[DatabaseID],
			[UTCCollectionDateTime],
			[DatabaseStatus],
			[Transactions],
			[LogFlushWaits],
			[LogFlushes],
			[LogKilobytesFlushed],
			[LogCacheReads],
			[LogCacheHitRatio],
			[TimeDeltaInSeconds],
			[NumberReads],
			[NumberWrites],
			[BytesRead],
			[BytesWritten],
			[IoStallMS],
			[UTCCollectionDateTime]
		from DatabaseStatistics_upgrade
		where
			DatabaseID = @DatabaseID
			and UTCCollectionDateTime <= @LoopDate
		option (recompile)

		delete from DatabaseStatistics_upgrade
		where
			DatabaseID = @DatabaseID
			and UTCCollectionDateTime <= @LoopDate

	
		if (@LoopDate > @MaxDate)
			break;
			
		if (@LoopDate is null)
			break;

	end

	if exists (select 1 from DatabaseSizeDateTime where DatabaseID = @DatabaseID)
	begin
		-- If the Database entry is present in the DatabaseSizeDateTime table then update other wise insert the row
		update DatabaseSizeDateTime set DatabaseID = @DatabaseID, UTCCollectionDateTime = @MaxDate where DatabaseID = @DatabaseID
	end
	else
	begin
		insert into DatabaseSizeDateTime (DatabaseID, UTCCollectionDateTime) values (@DatabaseID, @MaxDate)
	end
	
	select 
		@RowsRemaining = sum(rows)
	from
		sys.partitions
	where
		object_id = object_id(''DatabaseStatistics_upgrade'')
		and index_id < 2 
end
else
begin
	set @RowsRemaining = 0
end
	return @RowsRemaining
end
')

end