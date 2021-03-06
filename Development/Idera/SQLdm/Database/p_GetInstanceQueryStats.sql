
if (object_id('p_GetInstanceQueryStats') is not null)
begin
drop procedure [p_GetInstanceQueryStats]
end
go

CREATE procedure [dbo].[p_GetInstanceQueryStats]
	@SQLServerID int,
	@NumHistoryMinutes int = 30,
	@ListRowcount int = 200,
	@StartDateTime DateTime = NULL,
	@EndDateTime DateTime = NULL
as
begin


if(@EndDateTime is null)
begin
	SET @EndDateTime = (SELECT MAX(CompletionTime) FROM QueryMonitorStatistics WHERE SQLServerID = @SQLServerID);
end
if(@StartDateTime is null)
begin
	SET @StartDateTime = (SELECT DATEADD(minute, -1*@NumHistoryMinutes, @EndDateTime));
end

declare  @QueryStatementAggregationStep1 table
	(
		SQLServerID int,
		EventUTCStartTime datetime,	
		QueryMonitorOccurrences dec(38,0),
		--WaitOcurrences dec(38,0),
		--BlockingOcurrences dec(38,0),
		--DeadlockOcurrences dec(38,0),
		TotalDurationMilliseconds dec(38,0),
		TotalCPUMilliseconds dec(38,0),
		TotalReads dec(38,0),
		TotalWrites dec(38,0),
		StatementType int,
		--TotalBlockingDurationMilliseconds dec(38,0),
		--TotalWaitDuration dec(38,0),
		ApplicationNameID bigint,
		DatabaseID bigint,
		HostNameID bigint,
		LoginNameID bigint,
		SQLTextID  bigint,
		SQLSignatureID bigint,
		MaxCPUMilliseconds dec(38,0),
		MaxReads dec(38,0),
		MaxWrites dec(38,0),
		Spid int
	)
	

Declare @QueryStatementAggregationStep2 table
	(
		SQLServerID int,
		EventUTCStartTime datetime,	
		QueryMonitorOccurrences dec(38,0),
		--WaitOcurrences dec(38,0),
		--BlockingOcurrences dec(38,0),
		--DeadlockOcurrences dec(38,0),
		TotalDurationMilliseconds dec(38,0),
		TotalCPUMilliseconds dec(38,0),
		TotalReads dec(38,0),
		TotalWrites dec(38,0),
		StatementType int,
		--TotalBlockingDurationMilliseconds dec(38,0),
		--TotalWaitDuration dec(38,0),
		ApplicationName nvarchar(256),
		DatabaseName nvarchar(255),
		HostName nvarchar(255),
		LoginName nvarchar(255),
		SQLTextID  bigint,
		SQLSignatureID bigint,
		AggregationFlag tinyint,
		MaxCPUMilliseconds dec(38,0),
		MaxReads dec(38,0),
		MaxWrites dec(38,0),
		Spid int
	)
	
--create clustered index IX_QueryStatementAggregationStep2 on @QueryStatementAggregationStep2(SQLTextID) -- can't create lustered ndex on table variable

	insert into @QueryStatementAggregationStep1
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		TotalDurationMilliseconds,
		TotalCPUMilliseconds,
		TotalReads,
		TotalWrites,
		ApplicationNameID,
		DatabaseID,
		HostNameID,
		LoginNameID,
		StatementType,
		SQLTextID,
		SQLSignatureID,
		MaxCPUMilliseconds,
		MaxReads,
		MaxWrites,
		Spid
	)

	-- Query Monitor
	select
		SQLServerID,
		EventUTCStartTime = dateadd(ms,datepart(ms,StatementUTCStartTime)*-1,StatementUTCStartTime),	
		QueryMonitorOccurrences = count(SQLServerID),
		TotalDurationMilliseconds = sum(cast(DurationMilliseconds as dec(38,0))),
		TotalCPUMilliseconds = sum(cast(CPUMilliseconds as dec(38,0))),
		TotalReads = sum(cast(Reads as dec(38,0))),
		TotalWrites = sum(cast(Writes as dec(38,0))),
		ApplicationNameID,
		DatabaseID,
		HostNameID ,
		LoginNameID,
		StatementType,
		SQLTextID =SQLStatementID,
		SQLSignatureID,
		MaxCPUMilliseconds = max(cast(CPUMilliseconds as dec(38,0))),
		MaxReads = max(cast(Reads as dec(38,0))),
		MaxWrites = max(cast(Writes as dec(38,0))),
		Spid =qm.SessionID 
	from 
		QueryMonitorStatistics qm (NOLOCK)
	where
		SQLServerID = @SQLServerID
		and 
		(		
			((StatementUTCStartTime between @StartDateTime and @EndDateTime) or (CompletionTime between @StartDateTime and @EndDateTime))
			
		)
	group by
		SQLServerID,
		dateadd(ms,datepart(ms,StatementUTCStartTime)*-1,StatementUTCStartTime),
		ApplicationNameID,
		DatabaseID,
		StatementType,
		SQLStatementID,
		SQLSignatureID,
		HostNameID,
		LoginNameID,
		qm.SessionID

	insert into 
	@QueryStatementAggregationStep2
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		--WaitOcurrences,
		--BlockingOcurrences,
		--DeadlockOcurrences,
		TotalDurationMilliseconds,
		TotalCPUMilliseconds,
		TotalReads,
		TotalWrites,
		StatementType,
		--TotalBlockingDurationMilliseconds,
		--TotalWaitDuration,
		ApplicationName,
		DatabaseName,
		HostName,
		LoginName,
		SQLTextID,
		SQLSignatureID,
		AggregationFlag,
		MaxCPUMilliseconds,
		MaxReads,
		MaxWrites,
		Spid
	)
	select
		qs1.SQLServerID,
		qs1.EventUTCStartTime,	
		max(qs1.QueryMonitorOccurrences),
		--max(qs1.WaitOcurrences),
		--max(qs1.BlockingOcurrences),
		--max(qs1.DeadlockOcurrences),
		max(qs1.TotalDurationMilliseconds),
		max(qs1.TotalCPUMilliseconds),
		max(qs1.TotalReads),
		max(qs1.TotalWrites),
		max(qs1.StatementType),
		--max(qs1.TotalBlockingDurationMilliseconds),
		--max(qs1.TotalWaitDuration),
		ApplicationName,
		DatabaseName,
		HostName,
		LoginName,
		qs1.SQLTextID,
		qs1.SQLSignatureID,
		0,		
		max(MaxCPUMilliseconds),
		max(MaxReads),
		max(MaxWrites),
		Spid
	from @QueryStatementAggregationStep1 qs1
		inner join SQLServerDatabaseNames dn (NOLOCK)
		on qs1.DatabaseID = dn.DatabaseID
		inner join ApplicationNames an (NOLOCK)
		on qs1.ApplicationNameID = an.ApplicationNameID
		inner join LoginNames ln (NOLOCK)
		on qs1.LoginNameID = ln.LoginNameID
		inner join HostNames hn (NOLOCK)
		on qs1.HostNameID = hn.HostNameID
		--inner join AllSQLStatements s
		--on qs1.SQLTextID = s.SQLStatementID
	group by
		qs1.SQLServerID,
		qs1.EventUTCStartTime,	
		ApplicationName,
		DatabaseName,
		HostName,
		LoginName,
		qs1.SQLTextID,
		qs1.SQLSignatureID,
		Spid

    insert into 
	@QueryStatementAggregationStep2
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		--WaitOcurrences,
		--BlockingOcurrences,
		--DeadlockOcurrences,
		TotalDurationMilliseconds,
		TotalCPUMilliseconds,
		TotalReads,
		TotalWrites,
		StatementType,
		--TotalBlockingDurationMilliseconds,
		--TotalWaitDuration,
		ApplicationName,
		DatabaseName,
		SQLTextID,
		SQLSignatureID,
		AggregationFlag,
		MaxCPUMilliseconds,
		MaxReads,
		MaxWrites
	)
	select
		q.SQLServerID,
		EventUTCStartTime,
		QueryMonitorOccurrences,
		--WaitOcurrences,
		--BlockingOcurrences,
		--DeadlockOcurrences,
		TotalDurationMilliseconds,
		TotalCPUMilliseconds,
		TotalReads,
		TotalWrites,
		StatementType,
		--TotalBlockingDurationMilliseconds,
		--TotalWaitDuration,
		ApplicationName,
		DatabaseName,
		q.SQLSignatureID,
		q.SQLSignatureID,
		1,		
		MaxCPUMilliseconds,
		MaxReads,
		MaxWrites
	from
		QuerySignatureAggregation q (NOLOCK)
		inner join SQLServerDatabaseNames dn (NOLOCK)
		on q.DatabaseID = dn.DatabaseID
		inner join ApplicationNames an (NOLOCK)
		on q.ApplicationNameID = an.ApplicationNameID
		inner join AllSQLSignatures s (NOLOCK)
		on q.SQLSignatureID = s.SQLSignatureID
	where
		q.SQLServerID = @SQLServerID

	declare @NewLine char(1), @NewLine2 char(1)

	set @NewLine = char(13)
	set @NewLine2 = char(10)
	set rowcount @ListRowcount

	select --top @ListRowcount
		DatabaseName,
		StatementUTCStartTime = EventUTCStartTime,
		--UTCCompletionTime = dateadd(ms,TotalDurationMilliseconds,EventUTCStartTime),
		AvgDurationMilliseconds = TotalDurationMilliseconds / nullif(QueryMonitorOccurrences,0),
		AvgCpu = TotalCPUMilliseconds / nullif(QueryMonitorOccurrences,0),
		TotalCPu=TotalCPUMilliseconds,
		AvgReads = TotalReads / nullif(QueryMonitorOccurrences,0),
		TotalReads,
		AvgWrites = TotalWrites / nullif(QueryMonitorOccurrences,0),
		TotalWrites,
		--WaitMilliseconds = TotalWaitDuration / nullif(WaitOcurrences,0),
		LoginName,-- = LoginName,
		ApplicationName = rtrim(ApplicationName),
		Case StatementType when 0 then 'StoredProcedure' when 1 then 'SingleStatement' when 2 then 'Batch' else 'none' end as StatementType,
		--BlockingTimeMilliseconds = TotalBlockingDurationMilliseconds / nullif(BlockingOcurrences,0),
		--Deadlocks = DeadlockOcurrences,
		sig.SQLSignature,
		sig.SQLSignatureID,
		SQLStatementText = left(replace(replace(s.SQLStatement,@NewLine,' '),@NewLine2,' '),255),
		HostName,-- = hn.HostName,
		AvgCPUPerSecond = cast(cast(TotalCPUMilliseconds as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)),
		--IOPerSecond = cast(cast(TotalReads + TotalWrites as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)),
		--Occurrences = coalesce(QueryMonitorOccurrences,WaitOcurrences,BlockingOcurrences,DeadlockOcurrences),
		Occurrence = QueryMonitorOccurrences,
		--AggregationFlag,
		SQLTextID = q.SQLTextID,
		--DoNotAggregate = isnull(sig.DoNotAggregate,0),
		Spid
	from
		@QueryStatementAggregationStep2	q
		inner join AllSQLStatements s (NOLOCK)
		on q.SQLTextID = s.SQLStatementID
		inner join AllSQLSignatures sig (NOLOCK)
		on q.SQLSignatureID  = sig.SQLSignatureID
	order by
		cast(cast(TotalCPUMilliseconds as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)) desc,  -- CPU Per Second
		cast(cast(TotalReads + TotalWrites as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)) desc,  -- IO Per Second
		TotalDurationMilliseconds / nullif(QueryMonitorOccurrences,0) desc, -- Duration
		TotalReads / nullif(QueryMonitorOccurrences,0) desc, -- Reads
		TotalWrites / nullif(QueryMonitorOccurrences,0) desc  -- Writes
		--DeadlockOcurrences desc -- Deadlocks

end

 
