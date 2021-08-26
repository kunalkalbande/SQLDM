if (object_id('p_GetQueryMonitorStatements') is not null)
begin
drop procedure p_GetQueryMonitorStatements
end
go 
create procedure [dbo].p_GetQueryMonitorStatements
	@SQLServerID int,
	@StartDate datetime = null,
	@EndDate datetime = null,
	@StartTime datetime = null,
	@EndTime datetime = null,
	 @IncludeSQLStatements bit = 1,
	 @IncludeStoredProcedures bit = 1,
	 @IncludeSQLBatches bit = 1,
	 @ApplicationNameExcludeFilter nvarchar(256) = null,
	 @ApplicationNameIncludeFilter nvarchar(256) = null,
	 @ClientComputerNameExcludeFilter nvarchar(255) = null,
	 @ClientComputerNameIncludeFilter nvarchar(255) = null,
	 @DatabaseNameExcludeFilter nvarchar(255) = null,
	 @DatabaseNameIncludeFilter nvarchar(255) = null,
	 @LoginNameExcludeFilter nvarchar(255) = null,
	 @LoginNameIncludeFilter nvarchar(255) = null,
	 @SqlTextExcludeFilter nvarchar(4000) = null,
	 @SqlTextIncludeFilter nvarchar(4000) = null,
	 @DurationFilter bigint = 0,
	@UTCOffset int = 0,
	 @ViewMode int = 0,
	 @SQLSignatureID int = null,
	 @SQLSignatureHash varchar(200) = null, -- SQLdm 9.1 (Ankit Srivastava) -- Rally Defect DE43572 -- Changed to varchar to avoid CONVERT_IMPLICIT
	 @ListRowcount int = 200,
	@SummaryRowcount int = 5,
	@GetFullQueryText bit = 0  -- SQLdm 9.0 (Ankit Srivastava) --(DE3932) get  complete SQLStatement Text

as
begin

set ansi_warnings off

declare 
	@SignatureMode int,
	@StatementMode int,
	@HistoryMode int

select
	@SignatureMode = 0,
	@StatementMode = 1,
	@HistoryMode = 2

declare @ReturnRowCount int

-- Do not honor these filters in this mode
if @ViewMode = @SignatureMode
select
	@ClientComputerNameExcludeFilter = null,
	@ClientComputerNameIncludeFilter = null,
	@LoginNameExcludeFilter = null,
	@LoginNameIncludeFilter = null

if (@StartTime is null)	set @StartTime = '00:00:00 AM'
if (@EndTime is null) set @EndTime = '11:59:59 PM'

select @StartDate = dateadd(ms,datediff(ms,0,dateadd(day,-datediff(day,0,@StartTime),@StartTime)),@StartDate)
select @EndDate = dateadd(ms,datediff(ms,0,dateadd(day,-datediff(day,0,@EndTime),@EndTime)),@EndDate)

declare
	@CorrectedStartTime datetime, 
	@CorrectedEndTime datetime;
	

select 
	@CorrectedStartTime = dateadd(mi,-@UTCOffset,@StartDate), 
	@CorrectedEndTime=dateadd(mi,-@UTCOffset,@EndDate)

-- SQLdm 10.1.2 (Varun Chopra) SQLDM-19787: SQL DM Performance - Query Monitor View - Cache Duration Filter
SELECT
	@DurationFilter= ISNULL(@DurationFilter,-1)

if (object_id('#QueryStatementAggregationStep1') is not null)
begin
	drop table #QueryStatementAggregationStep1
end

create table #QueryStatementAggregationStep1
	(
		SQLServerID int,
		EventUTCStartTime datetime,	
		QueryMonitorOccurrences dec(38,0),
		WaitOcurrences dec(38,0),
		BlockingOcurrences dec(38,0),
		DeadlockOcurrences dec(38,0),
		TotalDurationMilliseconds dec(38,0),
		TotalCPUMilliseconds dec(38,0),
		TotalReads dec(38,0),
		TotalWrites dec(38,0),
		StatementType int,
		TotalBlockingDurationMilliseconds dec(38,0),
		TotalWaitDuration dec(38,0),
		ApplicationNameID bigint,
		DatabaseID bigint,
		HostNameID bigint,
		LoginNameID bigint,
		SQLTextID  bigint,
		SQLSignatureID bigint,
		MaxCPUMilliseconds dec(38,0),
		MaxReads dec(38,0),
		MaxWrites dec(38,0),
		Spid int,
		QueryStatisticsID int              --SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
	)

--SQLDM-30170
create clustered index IX_QueryStatementAggregationStep1 on #QueryStatementAggregationStep1(SQLTextID)

	
	
if (object_id('#QueryStatementAggregationStep2') is not null)
begin
	drop table #QueryStatementAggregationStep2
end	

create table #QueryStatementAggregationStep2
	(
		SQLServerID int,
		EventUTCStartTime datetime,	
		QueryMonitorOccurrences dec(38,0),
		WaitOcurrences dec(38,0),
		BlockingOcurrences dec(38,0),
		DeadlockOcurrences dec(38,0),
		TotalDurationMilliseconds dec(38,0),
		TotalCPUMilliseconds dec(38,0),
		TotalReads dec(38,0),
		TotalWrites dec(38,0),
		StatementType int,
		TotalBlockingDurationMilliseconds dec(38,0),
		TotalWaitDuration dec(38,0),
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
		Spid int,
		QueryStatisticsID int              --SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
	)
	
create clustered index IX_QueryStatementAggregationStep2 on #QueryStatementAggregationStep2(SQLTextID)

if (@ViewMode = @HistoryMode and @SQLSignatureID is null and @SQLSignatureHash is not null)
begin
	select @SQLSignatureID = SQLSignatureID 
	from AllSQLSignatures
	where SQLSignatureHash = @SQLSignatureHash

	if @SQLSignatureID is null
	begin
		goto skip_ahead
	end
end

	insert into #QueryStatementAggregationStep1
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
		Spid,
		QueryStatisticsID                     --SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
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
		HostNameID = case when @ViewMode = @SignatureMode then 1 else HostNameID end,
		LoginNameID = case when @ViewMode = @SignatureMode then 1 else LoginNameID end,
		StatementType,
		SQLTextID = case when @ViewMode = @SignatureMode then SQLSignatureID else SQLStatementID end,
		SQLSignatureID,
		MaxCPUMilliseconds = max(cast(CPUMilliseconds as dec(38,0))),
		MaxReads = max(cast(Reads as dec(38,0))),
		MaxWrites = max(cast(Writes as dec(38,0))),
		Spid = case when @ViewMode = @SignatureMode then 1 else qm.SessionID end, 
		QueryStatisticsID = max(QueryStatisticsID)           --SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
	from 
		QueryMonitorStatistics qm
	where
		SQLServerID = @SQLServerID
		and DurationMilliseconds >= @DurationFilter			-- SQLdm 10.1.2 (Varun Chopra) SQLDM-19787: SQL DM Performance - Query Monitor View
		and (@SQLSignatureID is null or SQLSignatureID = @SQLSignatureID)
		and 
		(		
			(StatementUTCStartTime between @CorrectedStartTime and @CorrectedEndTime)
			or (CompletionTime between @CorrectedStartTime and @CorrectedEndTime)
		)
		and qm.StatementType in (
		case when @IncludeSQLStatements = 1 then 1 else null end,
		case when @IncludeStoredProcedures = 1 then 0 else null end,
		case when @IncludeSQLBatches = 1 then 2 else null end)
	group by
		SQLServerID,
		dateadd(ms,datepart(ms,StatementUTCStartTime)*-1,StatementUTCStartTime),
		ApplicationNameID,
		DatabaseID,
		StatementType,
		case when @ViewMode = @SignatureMode then SQLSignatureID else SQLStatementID end,
		SQLSignatureID,
		case when @ViewMode = @SignatureMode then 1 else HostNameID end,
		case when @ViewMode = @SignatureMode then 1 else LoginNameID end,
		case when @ViewMode = @SignatureMode then 1 else qm.SessionID end

	-- Treat all blocks as batches
	if (@IncludeSQLBatches = 1)
	begin
		insert into #QueryStatementAggregationStep1
		(
			SQLServerID,
			EventUTCStartTime,	
			BlockingOcurrences,
			TotalBlockingDurationMilliseconds,
			StatementType,
			ApplicationNameID,
			DatabaseID,
			HostNameID,
			LoginNameID,
			SQLTextID,
			SQLSignatureID,
			Spid
		)	
		select
			SQLServerID,
			EventUTCStartTime =  dateadd(ms,datepart(ms,BlockingUTCStartTime)*-1,BlockingUTCStartTime),
			BlockingOcurrences = count(SQLServerID),
			TotalBlockingDurationMilliseconds = sum(cast(BlockingDurationMilliseconds as dec(38,0))),
			StatementType = 2,
			ApplicationNameID,
			DatabaseID,
			HostNameID = case when @ViewMode = @SignatureMode then 1 else HostNameID end,
			LoginNameID = case when @ViewMode = @SignatureMode then 1 else LoginNameID end,
			SQLTextID = case when @ViewMode = @SignatureMode then SQLSignatureID else SQLStatementID end,
			SQLSignatureID,
			Spid = case when @ViewMode = @SignatureMode then 1 else SessionID end
		from
			BlockingSessionStatistics 
		where
			SQLServerID = @SQLServerID
			and BlockingDurationMilliseconds >= @DurationFilter			-- SQLdm 10.1.2 (Varun Chopra) SQLDM-19787: SQL DM Performance - Query Monitor View
			and (@SQLSignatureID is null or SQLSignatureID = @SQLSignatureID)
			and 
			(		
				(BlockingUTCStartTime between @CorrectedStartTime and @CorrectedEndTime)
				or (dateadd(ms,BlockingDurationMilliseconds,BlockingUTCStartTime) between @CorrectedStartTime and @CorrectedEndTime)
			)
		group by
			SQLServerID,
			dateadd(ms,datepart(ms,BlockingUTCStartTime)*-1,BlockingUTCStartTime),
			--cast(floor(cast(BlockingUTCStartTime as float)) as datetime),
			ApplicationNameID,
			DatabaseID,
			case when @ViewMode = @SignatureMode then SQLSignatureID else SQLStatementID end,
			SQLSignatureID,
			case when @ViewMode = @SignatureMode then 1 else HostNameID end,
			case when @ViewMode = @SignatureMode then 1 else LoginNameID end,
			case when @ViewMode = @SignatureMode then 1 else SessionID end
	end

	-- Treat all waits as statements
	if (@IncludeSQLStatements = 1)
	begin
		
		insert into #QueryStatementAggregationStep1
		(
			SQLServerID,
			EventUTCStartTime,	
			WaitOcurrences,
			TotalWaitDuration,
			StatementType,
			ApplicationNameID,
			DatabaseID,
			HostNameID,
			LoginNameID,
			SQLTextID,
			SQLSignatureID,
			Spid
		)	
		select
			SQLServerID,
			EventUTCStartTime =  dateadd(ms,datepart(ms,StatementUTCStartTime)*-1,StatementUTCStartTime),
			WaitOcurrences = count(StatementUTCStartTime),
			TotalWaitDuration = sum(cast(WaitDuration as dec(38,0))),
			StatementType = 1,
			ApplicationNameID,
			DatabaseID,
			HostNameID = case when @ViewMode = @SignatureMode then 1 else HostNameID end,
			LoginNameID = case when @ViewMode = @SignatureMode then 1 else LoginNameID end,
			SQLTextID = case when @ViewMode = @SignatureMode then SQLSignatureID else SQLStatementID end,
			SQLSignatureID,
			Spid = case when @ViewMode = @SignatureMode then 1 else SessionID end
		from
			ActiveWaitStatistics 
		where
			SQLServerID = @SQLServerID
			and WaitDuration >= @DurationFilter			-- SQLdm 10.1.2 (Varun Chopra) SQLDM-19787: SQL DM Performance - Query Monitor View
			and 
			(		
				(StatementUTCStartTime between @CorrectedStartTime and @CorrectedEndTime)
				or (dateadd(ms,WaitDuration,StatementUTCStartTime) between @CorrectedStartTime and @CorrectedEndTime)
			)
			and (@SQLSignatureID is null or SQLSignatureID = @SQLSignatureID)
		group by
			SQLServerID,
			dateadd(ms,datepart(ms,StatementUTCStartTime)*-1,StatementUTCStartTime),
			ApplicationNameID,
			DatabaseID,
			case when @ViewMode = @SignatureMode then SQLSignatureID else SQLStatementID end,
			SQLSignatureID,
			case when @ViewMode = @SignatureMode then 1 else HostNameID end,
			case when @ViewMode = @SignatureMode then 1 else LoginNameID end,
			case when @ViewMode = @SignatureMode then 1 else SessionID end
	end

	-- Treat deadlocks as batches.  This is debatable: we attempt to record the inputbuffer
	if (@IncludeSQLBatches = 1) and (@DurationFilter = -1 OR @DurationFilter = 0)			-- SQLdm 10.1.2 (Varun Chopra) SQLDM-19787: SQL DM Performance - Query Monitor View
	begin
		insert into #QueryStatementAggregationStep1
		(
			SQLServerID,
			EventUTCStartTime,	
			DeadlockOcurrences,
			StatementType,
			ApplicationNameID,
			DatabaseID,
			HostNameID,
			LoginNameID,
			SQLTextID,
			SQLSignatureID,
			Spid
		)	
		select
			SQLServerID,
			UTCOccurrenceDateTime =  dateadd(ms,datepart(ms,UTCOccurrenceDateTime)*-1,UTCOccurrenceDateTime),
			Occurrences = count(SQLServerID),
			StatementType = 2,
			ApplicationNameID,
			DatabaseID,
			HostNameID = case when @ViewMode = @SignatureMode then 1 else HostNameID end,
			LoginNameID = case when @ViewMode = @SignatureMode then 1 else LoginNameID end,
			SQLTextID = case when @ViewMode = @SignatureMode then SQLSignatureID else SQLStatementID end,
			SQLSignatureID,
			Spid = case when @ViewMode = @SignatureMode then 1 else SessionID end
		from
			DeadlockProcesses 
		where
			SQLServerID = @SQLServerID
			and UTCOccurrenceDateTime between @CorrectedStartTime and @CorrectedEndTime
			and (@SQLSignatureID is null or SQLSignatureID = @SQLSignatureID)
		group by
			SQLServerID,
			dateadd(ms,datepart(ms,UTCOccurrenceDateTime)*-1,UTCOccurrenceDateTime),
			ApplicationNameID,
			DatabaseID,
			case when @ViewMode = @SignatureMode then SQLSignatureID else SQLStatementID end,
			SQLSignatureID,
			case when @ViewMode = @SignatureMode then 1 else HostNameID end,
			case when @ViewMode = @SignatureMode then 1 else LoginNameID end,
			case when @ViewMode = @SignatureMode then 1 else SessionID end
	end

	

if (@ViewMode = @SignatureMode)
begin

	insert into 
	#QueryStatementAggregationStep2
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		WaitOcurrences,
		BlockingOcurrences,
		DeadlockOcurrences,
		TotalDurationMilliseconds,
		TotalCPUMilliseconds,
		TotalReads,
		TotalWrites,
		StatementType,
		TotalBlockingDurationMilliseconds,
		TotalWaitDuration,
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
		max(qs1.WaitOcurrences),
		max(qs1.BlockingOcurrences),
		max(qs1.DeadlockOcurrences),
		max(qs1.TotalDurationMilliseconds),
		max(qs1.TotalCPUMilliseconds),
		max(qs1.TotalReads),
		max(qs1.TotalWrites),
		max(qs1.StatementType),
		max(qs1.TotalBlockingDurationMilliseconds),
		max(qs1.TotalWaitDuration),
		ApplicationName,
		DatabaseName,
		null,
		null,
		qs1.SQLTextID,
		qs1.SQLSignatureID,
		0,		
		max(MaxCPUMilliseconds),
		max(MaxReads),
		max(MaxWrites),
		Spid
	from #QueryStatementAggregationStep1 qs1
		inner join SQLServerDatabaseNames dn
		on qs1.DatabaseID = dn.DatabaseID
		inner join ApplicationNames an
		on qs1.ApplicationNameID = an.ApplicationNameID
		--inner join AllSQLSignatures s
		--on qs1.SQLTextID = s.SQLSignatureID
	where
		(@ApplicationNameExcludeFilter is null or isnull(an.ApplicationName,'') not like @ApplicationNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@ApplicationNameIncludeFilter is null or isnull(an.ApplicationName,'') like @ApplicationNameIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@DatabaseNameExcludeFilter is null or isnull(dn.DatabaseName,'') not like @DatabaseNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@DatabaseNameIncludeFilter is null or isnull(dn.DatabaseName,'') like @DatabaseNameIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		--and (@SqlTextExcludeFilter is null or isnull(s.SQLSignature,'') not like @SqlTextExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		--and (@SqlTextIncludeFilter is null or isnull(s.SQLSignature,'') like @SqlTextIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	group by
		qs1.SQLServerID,
		qs1.EventUTCStartTime,	
		ApplicationName,
		DatabaseName,
		qs1.SQLTextID,
		qs1.SQLSignatureID,
		Spid

end
else
begin
	insert into 
	#QueryStatementAggregationStep2
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		WaitOcurrences,
		BlockingOcurrences,
		DeadlockOcurrences,
		TotalDurationMilliseconds,
		TotalCPUMilliseconds,
		TotalReads,
		TotalWrites,
		StatementType,
		TotalBlockingDurationMilliseconds,
		TotalWaitDuration,
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
		Spid,
		QueryStatisticsID                       --SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
	)
	select
		qs1.SQLServerID,
		qs1.EventUTCStartTime,	
		max(qs1.QueryMonitorOccurrences),
		max(qs1.WaitOcurrences),
		max(qs1.BlockingOcurrences),
		max(qs1.DeadlockOcurrences),
		max(qs1.TotalDurationMilliseconds),
		max(qs1.TotalCPUMilliseconds),
		max(qs1.TotalReads),
		max(qs1.TotalWrites),
		max(qs1.StatementType),
		max(qs1.TotalBlockingDurationMilliseconds),
		max(qs1.TotalWaitDuration),
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
		Spid,
		QueryStatisticsID = max(qs1.QueryStatisticsID)                         --SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
	from #QueryStatementAggregationStep1 qs1
		inner join SQLServerDatabaseNames dn
		on qs1.DatabaseID = dn.DatabaseID
		inner join ApplicationNames an
		on qs1.ApplicationNameID = an.ApplicationNameID
		inner join LoginNames ln
		on qs1.LoginNameID = ln.LoginNameID
		inner join HostNames hn
		on qs1.HostNameID = hn.HostNameID
		--inner join AllSQLStatements s
		--on qs1.SQLTextID = s.SQLStatementID
	where
		(@ApplicationNameExcludeFilter is null or isnull(an.ApplicationName,'') not like @ApplicationNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@ApplicationNameIncludeFilter is null or isnull(an.ApplicationName,'') like @ApplicationNameIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@DatabaseNameExcludeFilter is null or isnull(dn.DatabaseName,'') not like @DatabaseNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@DatabaseNameIncludeFilter is null or isnull(dn.DatabaseName,'') like @DatabaseNameIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@LoginNameExcludeFilter is null or isnull(ln.LoginName,'') not like @LoginNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@LoginNameIncludeFilter is null or isnull(ln.LoginName,'') like @LoginNameIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		--and (@SqlTextExcludeFilter is null or isnull(s.SQLStatement,'') not like @SqlTextExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		--and (@SqlTextIncludeFilter is null or isnull(s.SQLStatement,'') like @SqlTextIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@ClientComputerNameExcludeFilter is null or isnull(hn.HostName,'') not like @ClientComputerNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@ClientComputerNameIncludeFilter is null or isnull(hn.HostName,'') like @ClientComputerNameIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
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

end

insert into 
	#QueryStatementAggregationStep2
	(
		SQLServerID,
		EventUTCStartTime,	
		QueryMonitorOccurrences,
		WaitOcurrences,
		BlockingOcurrences,
		DeadlockOcurrences,
		TotalDurationMilliseconds,
		TotalCPUMilliseconds,
		TotalReads,
		TotalWrites,
		StatementType,
		TotalBlockingDurationMilliseconds,
		TotalWaitDuration,
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
		WaitOcurrences,
		BlockingOcurrences,
		DeadlockOcurrences,
		TotalDurationMilliseconds,
		TotalCPUMilliseconds,
		TotalReads,
		TotalWrites,
		StatementType,
		TotalBlockingDurationMilliseconds,
		TotalWaitDuration,
		ApplicationName,
		DatabaseName,
		q.SQLSignatureID,
		q.SQLSignatureID,
		1,		
		MaxCPUMilliseconds,
		MaxReads,
		MaxWrites
	from
		QuerySignatureAggregation q
		inner join SQLServerDatabaseNames dn
		on q.DatabaseID = dn.DatabaseID
		inner join ApplicationNames an
		on q.ApplicationNameID = an.ApplicationNameID
		inner join AllSQLSignatures s
		on q.SQLSignatureID = s.SQLSignatureID
	where
		q.SQLServerID = @SQLServerID
		and (@SQLSignatureID is null or q.SQLSignatureID = @SQLSignatureID)
		and (@StartDate is null or cast(floor(cast (dateadd(mi, @UTCOffset, EventUTCStartTime) as float)) as datetime) >= @StartDate)
		and (@EndDate is null or cast(floor(cast (dateadd(mi, @UTCOffset, EventUTCStartTime) as float)) as datetime) <= @EndDate)
		and (@ApplicationNameExcludeFilter is null or isnull(an.ApplicationName,'') not like @ApplicationNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@ApplicationNameIncludeFilter is null or isnull(an.ApplicationName,'') like @ApplicationNameIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@DatabaseNameExcludeFilter is null or isnull(dn.DatabaseName,'') not like @DatabaseNameExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@DatabaseNameIncludeFilter is null or isnull(dn.DatabaseName,'') like @DatabaseNameIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@SqlTextExcludeFilter is null or isnull(s.SQLSignature,'') not like @SqlTextExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@SqlTextIncludeFilter is null or isnull(s.SQLSignature,'') like @SqlTextIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)


if (@ViewMode = @HistoryMode and @SQLSignatureID is null)
begin
	select top 1 @SQLSignatureID = SQLSignatureID
	from #QueryStatementAggregationStep2
	order by
		cast(cast(TotalCPUMilliseconds as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)) desc,  -- CPU Per Second
		cast(cast(TotalReads + TotalWrites as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)) desc,  -- IO Per Second
		TotalDurationMilliseconds / nullif(QueryMonitorOccurrences,0) desc, -- Duration
		TotalReads / nullif(QueryMonitorOccurrences,0) desc, -- Reads
		TotalWrites / nullif(QueryMonitorOccurrences,0) desc,  -- Writes
		DeadlockOcurrences desc -- Deadlocks
	
	delete from #QueryStatementAggregationStep2 where @SQLSignatureID <> SQLSignatureID

end

skip_ahead:


declare @NewLine char(1), @NewLine2 char(1)

set @NewLine = char(13)
set @NewLine2 = char(10)

if (@ViewMode = @SignatureMode)
begin

	set rowcount @ListRowcount



	select --top @ListRowcount
		DatabaseName,
		UTCStartTime = min(EventUTCStartTime),
		UTCCompletionTime = max(dateadd(ms,TotalDurationMilliseconds,EventUTCStartTime)),
		DurationMilliseconds = sum(TotalDurationMilliseconds) / isnull(nullif(sum(QueryMonitorOccurrences),0),1),
		AvgCPUMilliseconds = sum(TotalCPUMilliseconds) / isnull(nullif(sum(QueryMonitorOccurrences),0),1),
		TotalCPUMilliseconds = max(TotalCPUMilliseconds),
		AvgReads = sum(TotalReads) / isnull(nullif(sum(QueryMonitorOccurrences),0),1),
		AvgWrites = sum(TotalWrites) / isnull(nullif(sum(QueryMonitorOccurrences),0),1),
		TotalReads = max(TotalReads),
		TotalWrites = max(TotalWrites),
		WaitMilliseconds = sum(TotalWaitDuration) / isnull(nullif(sum(WaitOcurrences),0),1),
		LoginName = null,
		ApplicationName = rtrim(ApplicationName),
		StatementType,
		BlockingTimeMilliseconds = sum(TotalBlockingDurationMilliseconds) / isnull(nullif(sum(BlockingOcurrences),0),1),
		Deadlocks = sum(DeadlockOcurrences),
		SQLSignatureID = q.SQLSignatureID,
		--START: SQLdm 9.0 (Ankit Srivastava) --(DE3932) get  complete SQLStatement Text
		SQLStatementText = case
								when @GetFullQueryText = 0 then left(replace(replace(s.SQLSignature,@NewLine,' '),@NewLine2,' '),255)
								else s.SQLSignature
						   end,
		--END : SQLdm 9.0 (Ankit Srivastava) --(DE3932) get  complete SQLStatement Text
		HostName = null,
		CPUPerSecond = cast(sum(TotalCPUMilliseconds) / nullif(sum(TotalDurationMilliseconds),0) as dec (15,2)),
		IOPerSecond = cast(sum(TotalReads + TotalWrites) / nullif(sum(TotalDurationMilliseconds),0) as dec (15,2)),
		Occurrences = coalesce(sum(QueryMonitorOccurrences),sum(WaitOcurrences),sum(BlockingOcurrences),sum(DeadlockOcurrences)),
		AggregationFlag = max(AggregationFlag),
		SQLTextID = q.SQLTextID,
		DoNotAggregate = isnull(s.DoNotAggregate,0),
		Spid = null
	from
		#QueryStatementAggregationStep2	q
		inner join AllSQLSignatures s
		on q.SQLTextID = s.SQLSignatureID
	where
		(@SqlTextExcludeFilter is null or isnull(s.SQLSignature,'') not like @SqlTextExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@SqlTextIncludeFilter is null or isnull(s.SQLSignature,'') like @SqlTextIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)		
	group by
		DatabaseName,
		ApplicationName,
		StatementType,
		q.SQLSignatureID,
		--START: SQLdm 9.0 (Ankit Srivastava) --(DE3932) get  complete SQLStatement Text
		case
			when @GetFullQueryText = 0 then left(replace(replace(s.SQLSignature,@NewLine,' '),@NewLine2,' '),255)
			else s.SQLSignature
		end,
		--END : SQLdm 9.0 (Ankit Srivastava) --(DE3932) get  complete SQLStatement Text
		q.SQLTextID,
		isnull(s.DoNotAggregate,0)
	order by
		cast(sum(TotalCPUMilliseconds) / nullif(sum(TotalDurationMilliseconds),0) as dec (15,2)) desc,  -- CPU Per Second
		cast(sum(TotalReads + TotalWrites) / nullif(sum(TotalDurationMilliseconds),0) as dec (15,2)) desc,  -- IO Per Second
		sum(TotalDurationMilliseconds) / isnull(nullif(sum(QueryMonitorOccurrences),0),1) desc,  -- Duration
		sum(TotalReads) / isnull(nullif(sum(QueryMonitorOccurrences),0),1) desc,  -- Reads
		sum(TotalWrites) / isnull(nullif(sum(QueryMonitorOccurrences),0),1) desc,  -- Writes
		sum(DeadlockOcurrences) desc -- Deadlocks
		
	
end 
else
begin

	set rowcount @ListRowcount

	select --top @ListRowcount
		DatabaseName,
		UTCStartTime = EventUTCStartTime,
		UTCCompletionTime = dateadd(ms,TotalDurationMilliseconds,EventUTCStartTime),
		DurationMilliseconds = TotalDurationMilliseconds / nullif(QueryMonitorOccurrences,0),
		AvgCPUMilliseconds = TotalCPUMilliseconds / nullif(QueryMonitorOccurrences,0),
		TotalCPUMilliseconds,
		AvgReads = TotalReads / nullif(QueryMonitorOccurrences,0),
		AvgWrites = TotalWrites / nullif(QueryMonitorOccurrences,0),
		TotalReads,
		TotalWrites,
		WaitMilliseconds = TotalWaitDuration / nullif(WaitOcurrences,0),
		LoginName,-- = LoginName,
		ApplicationName = rtrim(ApplicationName),
		StatementType,
		BlockingTimeMilliseconds = TotalBlockingDurationMilliseconds / nullif(BlockingOcurrences,0),
		Deadlocks = DeadlockOcurrences,
		SQLSignatureID = q.SQLSignatureID,
		--START: SQLdm 9.0 (Ankit Srivastava) --(DE3932) get  complete SQLStatement Text
		SQLStatementText = case
								when @GetFullQueryText = 0 then left(replace(replace(s.SQLStatement,@NewLine,' '),@NewLine2,' '),255)
								else s.SQLStatement
						   end,
		--END : SQLdm 9.0 (Ankit Srivastava) --(DE3932) get  complete SQLStatement Text
		HostName,-- = hn.HostName,
		CPUPerSecond = cast(cast(TotalCPUMilliseconds as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)),
		IOPerSecond = cast(cast(TotalReads + TotalWrites as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)),
		Occurrences = coalesce(QueryMonitorOccurrences,WaitOcurrences,BlockingOcurrences,DeadlockOcurrences),
		AggregationFlag,
		SQLTextID = q.SQLTextID,
		DoNotAggregate = isnull(sig.DoNotAggregate,0),
		Spid,
		QueryStatisticsID                --SQLdm 9.1 (Abhishek Joshi) -adding redirect functionality from a query in desktop client to WebUI Queries tab
	from
		#QueryStatementAggregationStep2	q
		inner join AllSQLStatements s
		on q.SQLTextID = s.SQLStatementID
		inner join AllSQLSignatures sig
		on q.SQLSignatureID  = sig.SQLSignatureID
	where
		isnull(@SQLSignatureID,q.SQLSignatureID) = q.SQLSignatureID
		and (@SqlTextExcludeFilter is null or isnull(s.SQLStatement,'') not like @SqlTextExcludeFilter collate SQL_Latin1_General_CP1_CI_AS)
		and (@SqlTextIncludeFilter is null or isnull(s.SQLStatement,'') like @SqlTextIncludeFilter collate SQL_Latin1_General_CP1_CI_AS)
	order by
		cast(cast(TotalCPUMilliseconds as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)) desc,  -- CPU Per Second
		cast(cast(TotalReads + TotalWrites as dec(15,0)) / nullif(TotalDurationMilliseconds,0) as dec (15,2)) desc,  -- IO Per Second
		TotalDurationMilliseconds / nullif(QueryMonitorOccurrences,0) desc, -- Duration
		TotalReads / nullif(QueryMonitorOccurrences,0) desc, -- Reads
		TotalWrites / nullif(QueryMonitorOccurrences,0) desc,  -- Writes
		DeadlockOcurrences desc -- Deadlocks

end

set @ReturnRowCount = @@rowcount


declare @LoopTable table
(
	Ordinal int,
	ColumnName nvarchar(255)	
)

declare @AggregationTable table
(
	ColumnName nvarchar(255),
	ColumnValue nvarchar(255),
	AvgCPUMilliseconds dec(15,2),
	AvgReads dec(15,2),
	AvgWrites  dec(15,2),
	AvgWaitMilliseconds dec(15,2),
	TotalDeadlocks dec(15,0),
	AvgBlockMilliseconds dec(15,2),
	AvgDurationMilliseconds dec(15,2),
	CPUPerSecond dec(15,2),
	IOPerSecond dec(15,2),
	MaxCPUMilliseconds dec(38,0),
	MaxReads dec(38,0),
	MaxWrites dec(38,0),
	Occurrences bigint,
	OccurrencesPerDay dec(15,2)
)

declare @ChartOutput table
(
	ColumnName nvarchar(255),
	ColumnValue nvarchar(255),
	AvgCPUMilliseconds dec(15,2),
	AvgReads dec(15,2),
	AvgWrites  dec(15,2),
	AvgWaitMilliseconds dec(15,2),
	TotalDeadlocks dec(15,0),
	AvgBlockMilliseconds dec(15,2),
	AvgDurationMilliseconds dec(15,2),
	CPUPerSecond dec(15,2),
	IOPerSecond dec(15,2),
	MaxCPUMilliseconds dec(38,0),
	MaxReads dec(38,0),
	MaxWrites dec(38,0),
	Occurrences bigint,
	OccurrencesPerDay dec(15,2)
)


if (@ViewMode = @HistoryMode)
	insert into @LoopTable
		select 0,'Total'

if (@ViewMode <> @HistoryMode)
	insert into @LoopTable
		select 1,'ApplicationName'
		union all select 2, 'DatabaseName'

if (@ViewMode = @StatementMode)
	insert into @LoopTable
		select 3,'HostName'
		union all select 4, 'LoginName'

declare @Loop int, @Exit int, @ColumnName nvarchar(255)


select @Loop = min(Ordinal), @Exit = max(Ordinal) from @LoopTable



while @Loop <= @Exit
begin
	set rowcount 0

	select @ColumnName = ColumnName 
	from @LoopTable
	where Ordinal = @Loop

	insert into 
		@AggregationTable
	select
		ColumnName = @ColumnName,
		ColumnValue = case 
			when @Loop = 0 then 'Total' 
			when @Loop = 1 then ApplicationName 
			when @Loop = 2 then DatabaseName 
			when @Loop = 3 then HostName 
			when @Loop = 4 then LoginName 
			else ' ' end,
		AvgCPUMilliseconds = sum(TotalCPUMilliseconds) / isnull(nullif(sum(QueryMonitorOccurrences),0),1),
		AvgReads = sum(TotalReads) / isnull(nullif(sum(QueryMonitorOccurrences),0),1),
		AvgWrites = sum(TotalWrites) / isnull(nullif(sum(QueryMonitorOccurrences),0),1),
		AvgWaitMilliseconds = sum(TotalWaitDuration) / isnull(nullif(sum(WaitOcurrences),0),1),
		TotalDeadlocks = sum(DeadlockOcurrences),
		AvgBlockMilliseconds = sum(TotalBlockingDurationMilliseconds) / isnull(nullif(sum(BlockingOcurrences),0),1),
		AvgDurationMilliseconds = sum(TotalDurationMilliseconds) / isnull(nullif(sum(QueryMonitorOccurrences),0),1),
		CPUPerSecond = cast(sum(TotalCPUMilliseconds) / nullif(sum(TotalDurationMilliseconds),0) as dec (15,2)),
		IOPerSecond = cast(sum(TotalReads + TotalWrites) / nullif(sum(TotalDurationMilliseconds),0) as dec (15,2)),
		MaxCPUMilliseconds = max(MaxCPUMilliseconds),
		MaxReads = max(MaxReads),
		MaxWrites = max(MaxWrites),
		Occurrences = coalesce(sum(QueryMonitorOccurrences),sum(WaitOcurrences),sum(BlockingOcurrences),sum(DeadlockOcurrences)),
		OccurrencesPerDay = 
			coalesce(sum(QueryMonitorOccurrences),sum(WaitOcurrences),sum(BlockingOcurrences),sum(DeadlockOcurrences)) /
			isnull(nullif(case when @StartDate is not null and @EndDate is not null then datediff(d,@StartDate,@EndDate) else datediff(d,min(EventUTCStartTime),max(EventUTCStartTime)) end,0),1)
	from
		#QueryStatementAggregationStep2	q
	group by
		case 
			when @Loop = 0 then 'Total' 
			when @Loop = 1 then ApplicationName 
			when @Loop = 2 then DatabaseName 
			when @Loop = 3 then HostName 
			when @Loop = 4 then LoginName 
			else ' ' end


	if @@rowcount < @SummaryRowcount
	begin
		insert into @ChartOutput
			select
				*
			from
				@AggregationTable
	end
	else
	begin

	set rowcount @SummaryRowcount

	insert into @ChartOutput
	select -- top @SummaryRowcount
		*
	from
		@AggregationTable
	order by
		AvgCPUMilliseconds desc

	insert into @ChartOutput
	select -- top @SummaryRowcount
		*
	from
		@AggregationTable
	order by
		AvgReads desc

	insert into @ChartOutput
	select -- top @SummaryRowcount
		*
	from
		@AggregationTable
	order by
		AvgWrites desc

	insert into @ChartOutput
	select -- top @SummaryRowcount
		*
	from
		@AggregationTable
	order by
		CPUPerSecond desc

	insert into @ChartOutput
	select -- top @SummaryRowcount
		*
	from
		@AggregationTable
	order by
		IOPerSecond desc

	end

	set rowcount 0

	delete from @AggregationTable	

	select @Loop = @Loop + 1

end

	set rowcount 0
	
	if @ViewMode = @HistoryMode
	begin

		select distinct *
		from @ChartOutput ,
		(select SQLSignature = left(replace(replace(SQLSignature,@NewLine,' '),@NewLine2,' '),255), SQLSignatureID = cast(SQLSignatureID as bigint), DoNotAggregate= isnull(DoNotAggregate,0)
		from AllSQLSignatures where SQLSignatureID = @SQLSignatureID)  s
	end
	else
	begin
		select distinct *, SQLSignature = null, SQLSignatureID = null, DoNotAggregate = null
		from @ChartOutput 
	end
	

declare 
	@QueryMonitorRunning bit

declare @DataExplanation table
(
	EnumValue int,
	Description nvarchar(100)
)	

if (@ReturnRowCount = 0)
	insert into @DataExplanation values ('0','No data was returned for the given filter set.')

select
@QueryMonitorRunning = QueryMonitorEnabled
from MonitoredSQLServers
where SQLServerID = @SQLServerID

if (@QueryMonitorRunning = 0)
	insert into @DataExplanation values ('1','The Query Monitor trace is not running.')

if (object_id('QueryMonitor') is not null)
begin
	if exists(select Name from RepositoryInfo where Name = 'QMUpgrading')
		insert into @DataExplanation values ('2','SQLdm is currently upgrading previously collected Query Monitor data.')
end
	
	select * from @DataExplanation

end


