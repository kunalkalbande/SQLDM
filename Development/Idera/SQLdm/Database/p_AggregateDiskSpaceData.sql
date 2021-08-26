if (object_id('p_AggregateDiskSpaceData') is not null)
begin
drop procedure p_AggregateDiskSpaceData
end
go
CREATE procedure p_AggregateDiskSpaceData
as
begin
set nocount on
set ansi_warnings off

declare @ActivityCutoff datetime,
		@SQLServerID int ,
		@AggregateDiskSpaceMonitorDays int
		
declare @LoopServers 
table
(SQLServerID int)

insert into @LoopServers
select SQLServerID from MonitoredSQLServers where isnull(Deleted,0) = 0

select @SQLServerID = min(SQLServerID) from @LoopServers
	
select @AggregateDiskSpaceMonitorDays = Internal_Value
from RepositoryInfo where Name = 'AggregateForecasting'

if (@AggregateDiskSpaceMonitorDays is null)
	set @AggregateDiskSpaceMonitorDays = 14

while isnull(@SQLServerID,0) > 0
begin

	set ansi_warnings off
	
	set @ActivityCutoff = cast(floor(cast(dateadd(day, -(@AggregateDiskSpaceMonitorDays), getdate()) as float)) as datetime)

	----------------------------------------------
	-- Begin DiskSpace Aggregation
	----------------------------------------------
	if (object_id('DiskSpaceAggregationStep1' )) is null
	begin
		create table DiskSpaceAggregationStep1  
		(
		    [AggregatedDiskDriveStatisticsID] int identity NOT NULL,
			[SQLServerID] int,
            [MinUTCCollectionDateTime] datetime NOT NULL ,
			[MaxUTCCollectionDateTime] datetime NOT NULL ,
			[DriveName] nvarchar(256) NOT NULL,
			[MinUnusedSizeKB] dec(18,0),
			[MaxUnusedSizeKB] dec(18,0),
			[TotalUnusedSizeKB] dec(18,0),
			[MinTotalzSieKB] dec(18,0),
			[MaxTotalSizeKB] dec(18,0),
			[TotalSizeKBPerDay] dec(18,0),
			[MinDiskIdlePercent] bigint,
			[MaxDiskIdlePercent] bigint,
			[TotalDiskIdlePercent] bigint,
			[MinDiskReadsPerSecond] bigint,
			[MaxDiskReadsPerSecond] bigint,
			[TotalDiskReadsPerSecond] bigint,
			[MinDiskTransferPerSecond] bigint,
			[MaxDiskTransferPerSecond] bigint,
			[TotalTransferPerSecond] bigint,
			[MinDiskWritesPerSecond] bigint,
			[MaxDiskWritesPerSecond] bigint,
			[TotalDiskWritesPerSecond] bigint,
			[MinDatabaseSizeTime] datetime,
			[MaxDatabaseSizeTime] datetime,
			[TotalDatabaseSizeTime] datetime,
			[MinAverageDiskQueueLength] bigint,
		    [MaxAverageDiskQueueLength] bigint,
	        [MinAverageDiskMillisecondsPerRead] bigint,
		    [MaxAverageDiskMillisecondsPerRead] bigint,
	        [MinAverageDiskMillisecondsPerTransfer] bigint,
		    [MaxAverageDiskMillisecondsPerTransfer] bigint,
		    [MinAverageDiskMillisecondsPerWrite] bigint,
		    [MaxAverageDiskMillisecondsPerWrite] bigint
		)
	end
	else
	begin
		truncate table DiskSpaceAggregationStep1
	end		

	insert into DiskSpaceAggregationStep1
	(
		    [SQLServerID],
           [MinUTCCollectionDateTime],
			[MaxUTCCollectionDateTime],
			[DriveName],
			[MinUnusedSizeKB],
			[MaxUnusedSizeKB],
			[TotalUnusedSizeKB],
			[MinTotalzSieKB],
			[MaxTotalSizeKB],
			[TotalSizeKBPerDay],
			[MinDiskIdlePercent],
			[MaxDiskIdlePercent],
			[TotalDiskIdlePercent],
			[MinDiskReadsPerSecond],
			[MaxDiskReadsPerSecond],
			[TotalDiskReadsPerSecond],
			[MinDiskTransferPerSecond],
			[MaxDiskTransferPerSecond],
			[TotalTransferPerSecond],
			[MinDiskWritesPerSecond],
			[MaxDiskWritesPerSecond],
			[TotalDiskWritesPerSecond],
			[MinDatabaseSizeTime],
			[MaxDatabaseSizeTime],
			[MinAverageDiskQueueLength],
		    [MaxAverageDiskQueueLength],
	        [MinAverageDiskMillisecondsPerRead],
		    [MaxAverageDiskMillisecondsPerRead],
	        [MinAverageDiskMillisecondsPerTransfer],
		    [MaxAverageDiskMillisecondsPerTransfer],
		    [MinAverageDiskMillisecondsPerWrite],
		    [MaxAverageDiskMillisecondsPerWrite]
	)

	select
		SQLServerID,
          min(UTCCollectionDateTime),
		max(UTCCollectionDateTime),
		[DriveName],
		min([UnusedSizeKB]),
		max([UnusedSizeKB]),
		sum([UnusedSizeKB]),
		min([TotalSizeKB]),
		max([TotalSizeKB]),
		sum([TotalSizeKB]),
	    Min(DiskIdlePercent),
		Max(DiskIdlePercent),
		sum(DiskIdlePercent),
	    Min(DiskReadsPerSecond),
		Max(DiskReadsPerSecond),
		sum(DiskReadsPerSecond),
		Min([DiskTransfersPerSecond]),
		Max([DiskTransfersPerSecond]),
		sum([DiskTransfersPerSecond]),
		Min(DiskWritesPerSecond),
		Max(DiskWritesPerSecond),
		sum(DiskWritesPerSecond),
		Min(DatabaseSizeTime),
		Max(DatabaseSizeTime),
	    min([AverageDiskQueueLength]),
		max([AverageDiskQueueLength]),
	    min([AverageDiskMillisecondsPerRead]),
		max([AverageDiskMillisecondsPerRead]),
	    min([AverageDiskMillisecondsPerTransfer]),
		max([AverageDiskMillisecondsPerTransfer]),
		min([AverageDiskMillisecondsPerWrite]),
		max([AverageDiskMillisecondsPerWrite])
	from 
		[DiskDriveStatistics] dd 
		inner join AllSQLSignatures s
		on dd.SQLServerID = s.SQLSignatureID
	where
		SQLServerID = @SQLServerID
		and dd.UTCCollectionDateTime <= @ActivityCutoff
		and isnull(s.DoNotAggregate,0) = 0 
	group by
		SQLServerID,
		cast(floor(cast(isnull(UTCCollectionDateTime,UTCCollectionDateTime) as float)) as datetime),
		s.SQLSignatureID, DriveName
		order by DriveName

		---------------------------------------------
	--  Updating Main Table
	---------------------------------------------

	update
	DiskSpaceAggregation 
	set
		[MinUTCCollectionDateTime]    = case when ds.[MinUTCCollectionDateTime]       > dsMain.[MinUTCCollectionDateTime]                       then ds.[MinUTCCollectionDateTime]                       else dsMain.[MinUTCCollectionDateTime]                       end,
		[MaxUTCCollectionDateTime]    = case when ds.[MaxUTCCollectionDateTime]     > dsMain.[MaxUTCCollectionDateTime]                 then ds.[MaxUTCCollectionDateTime]                 else dsMain.[MaxUTCCollectionDateTime]                 end,
		[MinUnusedSizeKB]                  = case when ds.[MinUnusedSizeKB]        > dsMain.[MinUnusedSizeKB]                    then ds.[MinUnusedSizeKB]                    else dsMain.[MinUnusedSizeKB]                    end,
		[MaxUnusedSizeKB]                  = case when ds.[MaxUnusedSizeKB]        > dsMain.[MaxUnusedSizeKB]                then ds.[MaxUnusedSizeKB]                else dsMain.[MaxUnusedSizeKB]                end,
		[TotalUnusedSizeKB]                = case when ds.[TotalUnusedSizeKB]       > dsMain.[TotalUnusedSizeKB]                then ds.[TotalUnusedSizeKB]                else dsMain.[TotalUnusedSizeKB]                end,
		[MinTotalzSieKB]              = case when ds.[MinTotalzSieKB]      > dsMain.[MinTotalzSieKB]         then ds.[MinTotalzSieKB]         else dsMain.[MinTotalzSieKB]         end,
		[MaxTotalSizeKB]              = case when ds.[MaxTotalSizeKB]      > dsMain.[MaxTotalSizeKB]           then ds.[MaxTotalSizeKB]           else dsMain.[MaxTotalSizeKB]           end,
		[TotalSizeKBPerDay]            = case when ds.[TotalSizeKBPerDay]      > dsMain.[TotalSizeKBPerDay]              then ds.[TotalSizeKBPerDay]              else dsMain.[TotalSizeKBPerDay]              end,
		[MinDiskIdlePercent]                = case when ds.[MinDiskIdlePercent]          > dsMain.[MinDiskIdlePercent]                then ds.[MinDiskIdlePercent]                else dsMain.[MinDiskIdlePercent]                end,
		[MaxDiskIdlePercent]                = case when ds.[MaxDiskIdlePercent]        > dsMain.[MaxDiskIdlePercent]                        then ds.[MaxDiskIdlePercent]                        else dsMain.[MaxDiskIdlePercent]                        end,
		[TotalDiskIdlePercent]              = case when ds.[TotalDiskIdlePercent]         > dsMain.[TotalDiskIdlePercent]                          then ds.[TotalDiskIdlePercent]                          else dsMain.[TotalDiskIdlePercent]                          end,
		[MinDiskReadsPerSecond]           = case when ds.[MinDiskReadsPerSecond]     > dsMain.[MinDiskReadsPerSecond]                       then ds.[MinDiskReadsPerSecond]                       else dsMain.[MinDiskReadsPerSecond]                       end,
		[MaxDiskReadsPerSecond]           = case when ds.[MaxDiskReadsPerSecond]     > dsMain.[MaxDiskReadsPerSecond]                         then ds.[MaxDiskReadsPerSecond]                         else dsMain.[MaxDiskReadsPerSecond]                         end,
		[TotalDiskReadsPerSecond]         = case when ds.[TotalDiskReadsPerSecond]     > dsMain.[TotalDiskReadsPerSecond]                     then ds.[TotalDiskReadsPerSecond]                     else dsMain.[TotalDiskReadsPerSecond]                     end,
		[MinDiskTransferPerSecond]            = case when ds.[MinDiskTransferPerSecond] > dsMain.[MinDiskTransferPerSecond] then ds.[MinDiskTransferPerSecond] else dsMain.[MinDiskTransferPerSecond] end,
		[MaxDiskTransferPerSecond]            = case when ds.[MaxDiskTransferPerSecond]   > dsMain.[MaxDiskTransferPerSecond]   then ds.[MaxDiskTransferPerSecond]   else dsMain.[MaxDiskTransferPerSecond]   end,
		[TotalTransferPerSecond]          = case when ds.[TotalTransferPerSecond]   > dsMain.[TotalTransferPerSecond]                 then ds.[TotalTransferPerSecond]                 else dsMain.[TotalTransferPerSecond]                 end,
		[MinDiskWritesPerSecond]           = case when ds.[MinDiskWritesPerSecond]     > dsMain.[MinDiskWritesPerSecond]                       then ds.[MinDiskWritesPerSecond]                       else dsMain.[MinDiskWritesPerSecond]                       end,
		[MaxDiskWritesPerSecond]           = case when ds.[MaxDiskWritesPerSecond]     > dsMain.[MaxDiskWritesPerSecond]                         then ds.[MaxDiskWritesPerSecond]                         else dsMain.[MaxDiskWritesPerSecond]                         end,
		[TotalDiskWritesPerSecond]         = case when ds.[TotalDiskWritesPerSecond]     > dsMain.[TotalDiskWritesPerSecond]                     then ds.[TotalDiskWritesPerSecond]                     else dsMain.[TotalDiskWritesPerSecond]                     end,
		[MinDatabaseSizeTime]            = case when ds.[MinDatabaseSizeTime] > dsMain.[MinDatabaseSizeTime] then ds.[MinDatabaseSizeTime] else dsMain.[MinDatabaseSizeTime] end,
		[MaxDatabaseSizeTime]            = case when ds.[MaxDatabaseSizeTime]   > dsMain.[MaxDatabaseSizeTime]   then ds.[MaxDatabaseSizeTime]   else dsMain.[MaxDatabaseSizeTime]   end,
		[MinAverageDiskQueueLength]          = case when ds.[MinAverageDiskQueueLength]   > dsMain.[MinAverageDiskQueueLength]                 then ds.[MinAverageDiskQueueLength]                 else dsMain.[MinAverageDiskQueueLength]                 end,
		[MaxAverageDiskQueueLength]                  = case when ds.[MaxAverageDiskQueueLength]        > dsMain.[MaxAverageDiskQueueLength]                    then ds.[MaxAverageDiskQueueLength]                    else dsMain.[MaxAverageDiskQueueLength]                    end,
		[MinAverageDiskMillisecondsPerRead]                  = case when ds.[MinAverageDiskMillisecondsPerRead]        > dsMain.[MinAverageDiskMillisecondsPerRead]                then ds.[MinAverageDiskMillisecondsPerRead]                else dsMain.[MinAverageDiskMillisecondsPerRead]                end,
		[MaxAverageDiskMillisecondsPerRead]                = case when ds.[MaxAverageDiskMillisecondsPerRead]       > dsMain.[MaxAverageDiskMillisecondsPerRead]                then ds.[MaxAverageDiskMillisecondsPerRead]                else dsMain.[MaxAverageDiskMillisecondsPerRead]                end,
		[MinAverageDiskMillisecondsPerTransfer]              = case when ds.[MinAverageDiskMillisecondsPerTransfer]      > dsMain.[MinAverageDiskMillisecondsPerTransfer]         then ds.[MinAverageDiskMillisecondsPerTransfer]         else dsMain.[MinAverageDiskMillisecondsPerTransfer]         end,
		[MaxAverageDiskMillisecondsPerTransfer]              = case when ds.[MaxAverageDiskMillisecondsPerTransfer]      > dsMain.[MaxAverageDiskMillisecondsPerTransfer]           then ds.[MaxAverageDiskMillisecondsPerTransfer]           else dsMain.[MaxAverageDiskMillisecondsPerTransfer]           end,
		[MinAverageDiskMillisecondsPerWrite]            = case when ds.[MinAverageDiskMillisecondsPerWrite]      > dsMain.[MinAverageDiskMillisecondsPerWrite]              then ds.[MinAverageDiskMillisecondsPerWrite]              else dsMain.[MinAverageDiskMillisecondsPerWrite]              end,
		[MaxAverageDiskMillisecondsPerWrite]                = case when ds.[MaxAverageDiskMillisecondsPerWrite]          > dsMain.[MaxAverageDiskMillisecondsPerWrite]                then ds.[MaxAverageDiskMillisecondsPerWrite]                else dsMain.[MaxAverageDiskMillisecondsPerWrite]                end
	
		from DiskSpaceAggregationStep1 ds
	inner join DiskSpaceAggregation dsMain
	on 
		ds.SQLServerID = dsMain.SQLServerID and ds.AggregatedDiskDriveStatisticsID = dsMain.AggregatedDiskDriveStatisticsID
		and ds.[MinUTCCollectionDateTime] = dsMain.[MinUTCCollectionDateTime]
		and ds.MaxUTCCollectionDateTime = dsMain.MaxUTCCollectionDateTime
		and ds.DriveName is not null

	insert into 
	DiskSpaceAggregation
	(
		    [SQLServerID],
           [MinUTCCollectionDateTime],
			[MaxUTCCollectionDateTime],
			[DriveName],
			[MinUnusedSizeKB],
			[MaxUnusedSizeKB],
			[TotalUnusedSizeKB],
			[MinTotalzSieKB],
			[MaxTotalSizeKB],
			[TotalSizeKBPerDay],
			[MinDiskIdlePercent],
			[MaxDiskIdlePercent],
			[TotalDiskIdlePercent],
			[MinDiskReadsPerSecond],
			[MaxDiskReadsPerSecond],
			[TotalDiskReadsPerSecond],
			[MinDiskTransferPerSecond],
			[MaxDiskTransferPerSecond],
			[TotalTransferPerSecond],
			[MinDiskWritesPerSecond],
			[MaxDiskWritesPerSecond],
			[TotalDiskWritesPerSecond],
			[MinDatabaseSizeTime],
			[MaxDatabaseSizeTime],
			[MinAverageDiskQueueLength],
		    [MaxAverageDiskQueueLength],
	        [MinAverageDiskMillisecondsPerRead],
		    [MaxAverageDiskMillisecondsPerRead],
	        [MinAverageDiskMillisecondsPerTransfer],
		    [MaxAverageDiskMillisecondsPerTransfer],
		    [MinAverageDiskMillisecondsPerWrite],
		    [MaxAverageDiskMillisecondsPerWrite]
	)
	select
		    ds.[SQLServerID],
           ds.[MinUTCCollectionDateTime],
			ds.[MaxUTCCollectionDateTime],
			ds.[DriveName],
			ds.[MinUnusedSizeKB],
			ds.[MaxUnusedSizeKB],
			ds.[TotalUnusedSizeKB],
			ds.[MinTotalzSieKB],
			ds.[MaxTotalSizeKB],
			ds.[TotalSizeKBPerDay],
			ds.[MinDiskIdlePercent],
			ds.[MaxDiskIdlePercent],
			ds.[TotalDiskIdlePercent],
			ds.[MinDiskReadsPerSecond],
			ds.[MaxDiskReadsPerSecond],
			ds.[TotalDiskReadsPerSecond],
			ds.[MinDiskTransferPerSecond],
			ds.[MaxDiskTransferPerSecond],
			ds.[TotalTransferPerSecond],
			ds.[MinDiskWritesPerSecond],
			ds.[MaxDiskWritesPerSecond],
			ds.[TotalDiskWritesPerSecond],
			ds.[MinDatabaseSizeTime],
			ds.[MaxDatabaseSizeTime],
			ds.[MinAverageDiskQueueLength],
		    ds.[MaxAverageDiskQueueLength],
	        ds.[MinAverageDiskMillisecondsPerRead],
		    ds.[MaxAverageDiskMillisecondsPerRead],
	        ds.[MinAverageDiskMillisecondsPerTransfer],
		    ds.[MaxAverageDiskMillisecondsPerTransfer],
		    ds.[MinAverageDiskMillisecondsPerWrite],
		    ds.[MaxAverageDiskMillisecondsPerWrite]
	from DiskSpaceAggregationStep1 ds
	left join DiskSpaceAggregation dsMain
	on 
		ds.SQLServerID = dsMain.SQLServerID and ds.AggregatedDiskDriveStatisticsID = dsMain.AggregatedDiskDriveStatisticsID
		and ds.[MinUTCCollectionDateTime] = dsMain.[MinUTCCollectionDateTime]
		and ds.MaxUTCCollectionDateTime = dsMain.MaxUTCCollectionDateTime
		where dsMain.DriveName is null and dsMain.SQLServerID is null
	---------------------------------------------
	--  Main Table Updated
	---------------------------------------------



	----------------------------------------------
	-- End DiskSpace Aggregation
	----------------------------------------------

	delete from @LoopServers where SQLServerID = @SQLServerID
	select @SQLServerID = min(SQLServerID) from @LoopServers
end	

	if (object_id('DiskSpaceAggregationStep1' )) is not null
	begin
		exec ('drop table DiskSpaceAggregationStep1')
	end
	
	
end	