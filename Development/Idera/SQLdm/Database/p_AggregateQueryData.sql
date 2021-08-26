if (object_id('p_AggregateQueryData') is not null)
begin
drop procedure p_AggregateQueryData
end
go

create procedure p_AggregateQueryData
as
begin
set nocount on
set ansi_warnings off


declare @ActivityCutoff datetime,
		@Now datetime, 
		@SQLServerID int, 
		@InstanceName nvarchar(256), 
		@AggregateQueryMonitorDays int,
		@RC int,
		@RowsAffected int,
		@run_id uniqueidentifier,
		@Sequence int,
		@TimeoutTime datetime,
		@RecordsToDelete int

set @Now = getdate()
set @TimeoutTime = dateadd(mi,120,getutcdate())
set @run_id = newid()
set @RecordsToDelete = 1000
set @Sequence = 0


exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Starting Aggregation', 0, null

declare @LoopServers 
table
(SQLServerID int)

insert into @LoopServers
select SQLServerID from MonitoredSQLServers where isnull(Deleted,0) = 0

select @SQLServerID = min(SQLServerID) from @LoopServers
	
select @AggregateQueryMonitorDays = Internal_Value
from RepositoryInfo where Name = 'GroomQueryAggregation'

if (@AggregateQueryMonitorDays is null)
	set @AggregateQueryMonitorDays = 14

while isnull(@SQLServerID,0) > 0
begin

	set ansi_warnings off
	
	select @ActivityCutoff = cast(floor(cast(dateadd(day, -(@AggregateQueryMonitorDays), @Now) as float)) as datetime)

	----------------------------------------------
	-- Begin Signature Mode
	----------------------------------------------
	if (object_id('QuerySignatureAggregationStep1' )) is null
	begin
		create table QuerySignatureAggregationStep1  
		(
			SQLServerID int,
			EventUTCStartTime datetime,	
			QueryMonitorOccurrences dec(38,0),
			WaitOcurrences dec(38,0),
			BlockingOcurrences dec(38,0),
			DeadlockOcurrences dec(38,0),
			TotalDurationMilliseconds dec(38,0),
			MaxDurationMilliseconds dec(38,0),
			TotalCPUMilliseconds dec(38,0),
			MaxCPUMilliseconds dec(38,0),
			TotalReads dec(38,0),
			MaxReads dec(38,0),	
			TotalWrites dec(38,0),
			MaxWrites dec(38,0),
			StatementType int,
			TotalBlockingDurationMilliseconds dec(38,0),
			MaxBlockingDurationMilliseconds dec(38,0),
			TotalWaitDuration dec(38,0),
			MaxWaitDuration dec(38,0),
			ApplicationNameID bigint,
			DatabaseID bigint,
			SQLSignatureID  bigint
		)
	end
	else
	begin
		truncate table QuerySignatureAggregationStep1
	end		

	insert into QuerySignatureAggregationStep1
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		TotalDurationMilliseconds,
		MaxDurationMilliseconds,
		TotalCPUMilliseconds,
		MaxCPUMilliseconds,
		TotalReads,
		MaxReads,	
		TotalWrites,
		MaxWrites,
		ApplicationNameID,
		DatabaseID,
		StatementType,
		SQLSignatureID
	)

	-- Query Monitor
	select
		SQLServerID,
		EventUTCStartTime = cast(floor(cast(isnull(StatementLocalStartTime,StatementUTCStartTime) as float)) as datetime),	
		QueryMonitorOccurrences = count(SQLServerID),
		TotalDurationMilliseconds = sum(cast(DurationMilliseconds as dec(38,0))),
		MaxDurationMilliseconds = max(DurationMilliseconds),
		TotalCPUMilliseconds = sum(cast(CPUMilliseconds as dec(38,0))),
		MaxCPUMilliseconds = max(CPUMilliseconds),
		TotalReads = sum(cast(Reads as dec(38,0))),
		MaxReads = max(Reads),	
		TotalWrites = sum(cast(Writes as dec(38,0))),
		MaxWrites = max(Writes),
		ApplicationNameID,
		DatabaseID,
		StatementType,
		qm.SQLSignatureID
	from 
		QueryMonitorStatistics qm 
		inner join AllSQLSignatures s
		on qm.SQLSignatureID = s.SQLSignatureID
	where
		SQLServerID = @SQLServerID
		and StatementUTCStartTime <= @ActivityCutoff
		and isnull(s.DoNotAggregate,0) = 0
	group by
		SQLServerID,
		cast(floor(cast(isnull(StatementLocalStartTime,StatementUTCStartTime) as float)) as datetime),
		ApplicationNameID,
		DatabaseID,
		StatementType,
		qm.SQLSignatureID
		
		


	insert into QuerySignatureAggregationStep1
	(
		SQLServerID,
		EventUTCStartTime,	
		BlockingOcurrences,
		TotalBlockingDurationMilliseconds,
		MaxBlockingDurationMilliseconds,
		ApplicationNameID,
		DatabaseID,
		SQLSignatureID
	)	
	select
		SQLServerID,
		EventUTCStartTime =  cast(floor(cast(isnull(BlockingLocalStartTime,BlockingUTCStartTime) as float)) as datetime),
		BlockingOcurrences = count(SQLServerID),
		TotalBlockingDurationMilliseconds = sum(cast(BlockingDurationMilliseconds as dec(38,0))),
		MaxBlockingDurationMilliseconds = max(BlockingDurationMilliseconds),
		ApplicationNameID,
		DatabaseID,
		b.SQLSignatureID
	from
		BlockingSessionStatistics b
		inner join AllSQLSignatures s
		on b.SQLSignatureID = s.SQLSignatureID
	where
		SQLServerID = @SQLServerID
		and BlockingUTCStartTime <= @ActivityCutoff
		and isnull(s.DoNotAggregate,0) = 0
	group by
		SQLServerID,
		cast(floor(cast(isnull(BlockingLocalStartTime,BlockingUTCStartTime) as float)) as datetime),
		ApplicationNameID,
		DatabaseID,
		b.SQLSignatureID

	-- Waits
	insert into QuerySignatureAggregationStep1
	(
		SQLServerID,
		EventUTCStartTime,	
		BlockingOcurrences,
		TotalWaitDuration,
		MaxWaitDuration,
		ApplicationNameID,
		DatabaseID,
		SQLSignatureID
	)	
	select
		SQLServerID,
		EventUTCStartTime =  cast(floor(cast(isnull(StatementLocalStartTime,StatementUTCStartTime) as float)) as datetime),
		WaitOcurrences = count(SQLServerID),
		TotalWaitDuration = sum(cast(WaitDuration as dec(38,0))),
		MaxWaitDuration = max(WaitDuration),
		ApplicationNameID,
		DatabaseID,
		a.SQLSignatureID
	from
		ActiveWaitStatistics a (nolock)
		inner join AllSQLSignatures s (nolock)
		on a.SQLSignatureID = s.SQLSignatureID
	where
		SQLServerID = @SQLServerID
		and StatementUTCStartTime <= @ActivityCutoff
		and isnull(s.DoNotAggregate,0) = 0
	group by
		SQLServerID,
		cast(floor(cast(isnull(StatementLocalStartTime,StatementUTCStartTime) as float)) as datetime),
		ApplicationNameID,
		DatabaseID,
		a.SQLSignatureID

	insert into QuerySignatureAggregationStep1
	(
		SQLServerID,
		EventUTCStartTime,	
		DeadlockOcurrences,
		ApplicationNameID,
		DatabaseID,
		SQLSignatureID
	)	
	-- Deadlocks
	select 
		SQLServerID,
		UTCOccurrenceDateTime =  cast(floor(cast(isnull(LocalOccurrenceDateTime,UTCOccurrenceDateTime) as float)) as datetime),
		Occurrences = count(SQLServerID),
		ApplicationNameID,
		DatabaseID,
		d.SQLSignatureID
	from
		DeadlockProcesses d
		inner join AllSQLSignatures s
		on d.SQLSignatureID = s.SQLSignatureID
	where
		SQLServerID = @SQLServerID
		and UTCOccurrenceDateTime <= @ActivityCutoff
		and isnull(s.DoNotAggregate,0) = 0
	group by
		SQLServerID,
		cast(floor(cast(isnull(LocalOccurrenceDateTime,UTCOccurrenceDateTime) as float)) as datetime),
		ApplicationNameID,
		DatabaseID,
		d.SQLSignatureID


	----------------------------------------------
	if (object_id('QuerySignatureAggregationStep2' )) is null
	begin
		create table QuerySignatureAggregationStep2  
		(
			SQLServerID int,
			EventUTCStartTime datetime,	
			QueryMonitorOccurrences dec(38,0),
			WaitOcurrences dec(38,0),
			BlockingOcurrences dec(38,0),
			DeadlockOcurrences dec(38,0),
			TotalDurationMilliseconds dec(38,0),
			MaxDurationMilliseconds dec(38,0),
			TotalCPUMilliseconds dec(38,0),
			MaxCPUMilliseconds dec(38,0),
			TotalReads dec(38,0),
			MaxReads dec(38,0),	
			TotalWrites dec(38,0),
			MaxWrites dec(38,0),
			StatementType int,
			TotalBlockingDurationMilliseconds dec(38,0),
			MaxBlockingDurationMilliseconds dec(38,0),
			TotalWaitDuration dec(38,0),
			MaxWaitDuration dec(38,0),
			ApplicationNameID bigint,
			DatabaseID bigint,
			SQLSignatureID  bigint
		)
		
		create index qs2SQLServerID on QuerySignatureAggregationStep2(SQLServerID)
		create index qs2EventUTCStartTime on QuerySignatureAggregationStep2(EventUTCStartTime)
		create index qs2ApplicationNameID on QuerySignatureAggregationStep2(ApplicationNameID)
		create index qs2DatabaseID on QuerySignatureAggregationStep2(DatabaseID)
		create index qs2SQLSignatureID on QuerySignatureAggregationStep2(SQLSignatureID)
	end
	else
	begin
		truncate table QuerySignatureAggregationStep2
	end
	
	insert into 
	QuerySignatureAggregationStep2
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		WaitOcurrences,
		BlockingOcurrences,
		DeadlockOcurrences,
		TotalDurationMilliseconds,
		MaxDurationMilliseconds,
		TotalCPUMilliseconds,
		MaxCPUMilliseconds,
		TotalReads,
		MaxReads,	
		TotalWrites,
		MaxWrites,
		StatementType,
		TotalBlockingDurationMilliseconds,
		MaxBlockingDurationMilliseconds,
		TotalWaitDuration,
		MaxWaitDuration,
		ApplicationNameID,
		DatabaseID,
		SQLSignatureID 
	)
	select
		qs1.SQLServerID,
		qs1.EventUTCStartTime,	
		max(qs1.QueryMonitorOccurrences),
		max(qs1.WaitOcurrences),
		max(qs1.BlockingOcurrences),
		max(qs1.DeadlockOcurrences),
		max(qs1.TotalDurationMilliseconds),
		max(qs1.MaxDurationMilliseconds),
		max(qs1.TotalCPUMilliseconds),
		max(qs1.MaxCPUMilliseconds),
		max(qs1.TotalReads),
		max(qs1.MaxReads),	
		max(qs1.TotalWrites),
		max(qs1.MaxWrites),
		max(qs1.StatementType),
		max(qs1.TotalBlockingDurationMilliseconds),
		max(qs1.MaxBlockingDurationMilliseconds),
		max(qs1.TotalWaitDuration),
		max(qs1.MaxWaitDuration),
		qs1.ApplicationNameID,
		qs1.DatabaseID,
		qs1.SQLSignatureID 
	from QuerySignatureAggregationStep1 qs1
	group by
		qs1.SQLServerID,
		qs1.EventUTCStartTime,	
		qs1.ApplicationNameID,
		qs1.DatabaseID,
		qs1.SQLSignatureID 
		


	update
	QuerySignatureAggregation
	set
		SQLServerID                                = case when qs2.SQLServerID                       > qsa.SQLServerID                       then qs2.SQLServerID                       else qsa.SQLServerID                       end,
		EventUTCStartTime                          = case when qs2.EventUTCStartTime                 > qsa.EventUTCStartTime                 then qs2.EventUTCStartTime                 else qsa.EventUTCStartTime                 end,
		QueryMonitorOccurrences                    = case when qs2.QueryMonitorOccurrences           > qsa.QueryMonitorOccurrences           then qs2.QueryMonitorOccurrences           else qsa.QueryMonitorOccurrences           end,
		WaitOcurrences                             = case when qs2.WaitOcurrences                    > qsa.WaitOcurrences                    then qs2.WaitOcurrences                    else qsa.WaitOcurrences                    end,
		BlockingOcurrences                         = case when qs2.BlockingOcurrences                > qsa.BlockingOcurrences                then qs2.BlockingOcurrences                else qsa.BlockingOcurrences                end,
		DeadlockOcurrences                         = case when qs2.DeadlockOcurrences                > qsa.DeadlockOcurrences                then qs2.DeadlockOcurrences                else qsa.DeadlockOcurrences                end,
		TotalDurationMilliseconds                  = case when qs2.TotalDurationMilliseconds         > qsa.TotalDurationMilliseconds         then qs2.TotalDurationMilliseconds         else qsa.TotalDurationMilliseconds         end,
		MaxDurationMilliseconds                    = case when qs2.MaxDurationMilliseconds           > qsa.MaxDurationMilliseconds           then qs2.MaxDurationMilliseconds           else qsa.MaxDurationMilliseconds           end,
		TotalCPUMilliseconds                       = case when qs2.TotalCPUMilliseconds              > qsa.TotalCPUMilliseconds              then qs2.TotalCPUMilliseconds              else qsa.TotalCPUMilliseconds              end,
		MaxCPUMilliseconds                         = case when qs2.MaxCPUMilliseconds                > qsa.MaxCPUMilliseconds                then qs2.MaxCPUMilliseconds                else qsa.MaxCPUMilliseconds                end,
		TotalReads                                 = case when qs2.TotalReads                        > qsa.TotalReads                        then qs2.TotalReads                        else qsa.TotalReads                        end,
		MaxReads                                   = case when qs2.MaxReads                          > qsa.MaxReads                          then qs2.MaxReads                          else qsa.MaxReads                          end,
		TotalWrites                                = case when qs2.TotalWrites                       > qsa.TotalWrites                       then qs2.TotalWrites                       else qsa.TotalWrites                       end,
		MaxWrites                                  = case when qs2.MaxWrites                         > qsa.MaxWrites                         then qs2.MaxWrites                         else qsa.MaxWrites                         end,
		StatementType                              = case when qs2.StatementType                     > qsa.StatementType                     then qs2.StatementType                     else qsa.StatementType                     end,
		TotalBlockingDurationMilliseconds          = case when qs2.TotalBlockingDurationMilliseconds > qsa.TotalBlockingDurationMilliseconds then qs2.TotalBlockingDurationMilliseconds else qsa.TotalBlockingDurationMilliseconds end,
		MaxBlockingDurationMilliseconds            = case when qs2.MaxBlockingDurationMilliseconds   > qsa.MaxBlockingDurationMilliseconds   then qs2.MaxBlockingDurationMilliseconds   else qsa.MaxBlockingDurationMilliseconds   end,
		TotalWaitDuration                          = case when qs2.TotalWaitDuration                 > qsa.TotalWaitDuration                 then qs2.TotalWaitDuration                 else qsa.TotalWaitDuration                 end,
		MaxWaitDuration                            = case when qs2.MaxWaitDuration                   > qsa.MaxWaitDuration                   then qs2.MaxWaitDuration                   else qsa.MaxWaitDuration                   end,
		ApplicationNameID                          = case when qs2.ApplicationNameID                 > qsa.ApplicationNameID                 then qs2.ApplicationNameID                 else qsa.ApplicationNameID                 end,
		DatabaseID                                 = case when qs2.DatabaseID                        > qsa.DatabaseID                        then qs2.DatabaseID                        else qsa.DatabaseID                        end,
		SQLSignatureID                             = case when qs2.SQLSignatureID                    > qsa.SQLSignatureID                    then qs2.SQLSignatureID                    else qsa.SQLSignatureID                    end
	from QuerySignatureAggregationStep2 qs2
	inner join QuerySignatureAggregation qsa
	on 
		qs2.SQLServerID = qsa.SQLServerID
		and qs2.EventUTCStartTime = qsa.EventUTCStartTime
		and qs2.ApplicationNameID = qsa.ApplicationNameID
		and qs2.DatabaseID = qsa.DatabaseID
		and qs2.SQLSignatureID = qsa.SQLSignatureID 
	where
		qsa.SQLServerID is not null


	insert into 
	QuerySignatureAggregation
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		WaitOcurrences,
		BlockingOcurrences,
		DeadlockOcurrences,
		TotalDurationMilliseconds,
		MaxDurationMilliseconds,
		TotalCPUMilliseconds,
		MaxCPUMilliseconds,
		TotalReads,
		MaxReads,	
		TotalWrites,
		MaxWrites,
		StatementType,
		TotalBlockingDurationMilliseconds,
		MaxBlockingDurationMilliseconds,
		TotalWaitDuration,
		MaxWaitDuration,
		ApplicationNameID,
		DatabaseID,
		SQLSignatureID 
	)
	select
		qs2.SQLServerID,
		qs2.EventUTCStartTime,	
		qs2.QueryMonitorOccurrences,
		qs2.WaitOcurrences,
		qs2.BlockingOcurrences,
		qs2.DeadlockOcurrences,
		qs2.TotalDurationMilliseconds,
		qs2.MaxDurationMilliseconds,
		qs2.TotalCPUMilliseconds,
		qs2.MaxCPUMilliseconds,
		qs2.TotalReads,
		qs2.MaxReads,	
		qs2.TotalWrites,
		qs2.MaxWrites,
		qs2.StatementType,
		qs2.TotalBlockingDurationMilliseconds,
		qs2.MaxBlockingDurationMilliseconds,
		qs2.TotalWaitDuration,
		qs2.MaxWaitDuration,
		qs2.ApplicationNameID,
		qs2.DatabaseID,
		qs2.SQLSignatureID 
	from QuerySignatureAggregationStep2 qs2
	left join QuerySignatureAggregation qsa
	on 
		qs2.SQLServerID = qsa.SQLServerID
		and qs2.EventUTCStartTime = qsa.EventUTCStartTime
		and qs2.ApplicationNameID = qsa.ApplicationNameID
		and qs2.DatabaseID = qsa.DatabaseID
		and qs2.SQLSignatureID = qsa.SQLSignatureID 
	where
		qsa.SQLServerID is null

	
	set @RC = 0

	select @InstanceName = InstanceName from MonitoredSQLServers where SQLServerID = @SQLServerID
	
	set ansi_warnings ON
	
	exec Grooming.p_GroomQueryMonitorStatistics 
		@run_id = @run_id,
		@Sequence = @Sequence out,
		@TimeoutTime = @TimeoutTime,
		@RecordsToDelete = @RecordsToDelete,
		@CutoffDateTime = @ActivityCutoff,
		@SQLServerID = @SQLServerID,
		@InstanceName = @InstanceName,
		@Deleted = 0

	


	----------------------------------------------
	-- End Signature Mode
	----------------------------------------------


	delete from @LoopServers where SQLServerID = @SQLServerID
	select @SQLServerID = min(SQLServerID) from @LoopServers
end	

	if (object_id('QuerySignatureAggregationStep1' )) is not null
	begin
		exec ('drop table QuerySignatureAggregationStep1')
	end
	
	if (object_id('QuerySignatureAggregationStep2' )) is not null
	begin
		exec ('drop table QuerySignatureAggregationStep2')
	end
	
	
exec Grooming.p_LogGroomingAction @run_id, @Sequence out, 'Finished with Aggregation', 0, null	

end