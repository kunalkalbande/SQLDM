if (object_id('p_GetQueryStatistics') is not null)
begin
drop procedure p_GetQueryStatistics
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
--  5 - All Time
create procedure [dbo].p_GetQueryStatistics
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UTCOffset int = 0,
	@Interval int,
	@QueryText nvarchar(4000),
	@ApplicationName nvarchar(4000),
	@StatementType int,
	@DatabaseName sysname,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0
as 
begin

	if @UTCOffset is null
		set @UTCOffset = datediff(mi,getutcdate(),getdate())

	if @CaseInsensitive = 1
	begin
		set @ApplicationName = lower(@ApplicationName)
		set @DatabaseName = lower(@DatabaseName)
		set @QueryText = lower(@QueryText)
	end
	
	declare @LenCompare int
	set @LenCompare = len(@QueryText)
	if @LenCompare < 1000 
		set @LenCompare = 1000

	if (@SignatureMode = 1)
	begin

	select
		FirstStartTime = min(FirstStartTime),
		LastEndTime = max(LastEndTime),
		MinDuration = min(MinDuration),
		MaxDuration = max(MaxDuration),
		SumDuration = sum(SumDuration),
		AvgDuration = cast(sum(SumDuration) as float) / nullif(sum([Count]),0),
		MinCPU = min(MinCPU),
		MaxCPU = max(MaxCPU),
		SumCPU = sum(SumCPU),
		AvgCPU = cast(sum(SumCPU) as float) / nullif(sum([Count]),0),
		MinReads = min(MinReads),
		MaxReads = max(MaxReads),
		SumReads = sum(SumReads),
		AvgReads = cast(sum(SumReads) as float) / nullif(sum([Count]),0),
		MinWrites = min(MinWrites),
		MaxWrites = max(MaxWrites),
		SumWrites = sum(SumWrites),
		AvgWrites = cast(sum(SumWrites) as float) / nullif(sum([Count]),0),
		[Count] = sum([Count])
	from
	(
	select
		FirstStartTime = min(dateadd(mi,@UTCOffset,StatementUTCStartTime)),
		LastEndTime = max(dateadd(mi,@UTCOffset,CompletionTime)),
		MinDuration = min(DurationMilliseconds),
		MaxDuration = max(DurationMilliseconds),
		SumDuration = sum(DurationMilliseconds),
		MinCPU = min(CPUMilliseconds),
		MaxCPU = max(CPUMilliseconds),
		SumCPU = sum(CPUMilliseconds),
		MinReads = min(Reads),
		MaxReads = max(Reads),
		SumReads = sum(Reads),
		MinWrites = min(Writes),
		MaxWrites = max(Writes),
		SumWrites = sum(Writes),
		[Count] = count(*)
	from
		dbo.QueryMonitorStatistics qm (nolock)
		inner join dbo.SQLServerDatabaseNames d (nolock) on qm.DatabaseID = d.DatabaseID
		inner join ApplicationNames an on qm.ApplicationNameID = an.ApplicationNameID
		inner join AllSQLSignatures s on qm.SQLSignatureID = s.SQLSignatureID
	where
		qm.SQLServerID = @SQLServerID and
		rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)) like rtrim(ltrim(coalesce(@ApplicationName,case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end))) and
		rtrim(ltrim(StatementType)) like coalesce(@StatementType,StatementType) and
		case when @CaseInsensitive = 1 then lower(d.DatabaseName) else d.DatabaseName end like coalesce(@DatabaseName,case when @CaseInsensitive = 1 then lower(d.DatabaseName) else d.DatabaseName end) and
		left(ltrim(case when @CaseInsensitive = 1 then lower(SQLSignature) else SQLSignature end),@LenCompare) = left(ltrim(@QueryText),@LenCompare) and
		isnull(@StartDateTime,'1 Jan 1900') <= StatementUTCStartTime and
		isnull(@EndDateTime,'1 Jan 2200') >= CompletionTime
	group by
		 case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) else 1 end
		,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) else datepart(yy,dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) end
		,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) else datepart(yy,dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) end
		,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) else datepart(yy,dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) end
		,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) else datepart(yy,dateadd(mi, @UTCOffset, dateadd(mi,@UTCOffset,CompletionTime))) end
	union all
	select
		FirstStartTime = min(dateadd(mi,@UTCOffset,EventUTCStartTime)),
		LastEndTime = max(dateadd(mi,@UTCOffset,EventUTCStartTime)),
		MinDuration = min(TotalDurationMilliseconds),
		MaxDuration = max(TotalDurationMilliseconds),
		SumDuration = sum(TotalDurationMilliseconds),
		MinCPU = min(TotalCPUMilliseconds),
		MaxCPU = max(TotalCPUMilliseconds),
		SumCPU = sum(TotalCPUMilliseconds),
		MinReads = min(TotalReads),
		MaxReads = max(TotalReads),
		SumReads = sum(TotalReads),
		MinWrites = min(TotalWrites),
		MaxWrites = max(TotalWrites),
		SumWrites = sum(TotalWrites),
		[Count] = sum(QueryMonitorOccurrences)
	from
		dbo.QuerySignatureAggregation qm (nolock)
		inner join dbo.SQLServerDatabaseNames d (nolock) on qm.DatabaseID = d.DatabaseID
		inner join ApplicationNames an on qm.ApplicationNameID = an.ApplicationNameID
		inner join AllSQLSignatures s on qm.SQLSignatureID = s.SQLSignatureID
	where
		qm.SQLServerID = @SQLServerID and
		rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)) like rtrim(ltrim(coalesce(@ApplicationName,case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end))) and
		rtrim(ltrim(StatementType)) like coalesce(@StatementType,StatementType) and
		case when @CaseInsensitive = 1 then lower(d.DatabaseName) else d.DatabaseName end like coalesce(@DatabaseName,case when @CaseInsensitive = 1 then lower(d.DatabaseName) else d.DatabaseName end) and
		left(ltrim(case when @CaseInsensitive = 1 then lower(SQLSignature) else SQLSignature end),@LenCompare) = left(ltrim(@QueryText),@LenCompare) and
		isnull(@StartDateTime,'1 Jan 1900') <= EventUTCStartTime and
		isnull(@EndDateTime,'1 Jan 2200') >= EventUTCStartTime
	group by
		 dateadd(mi,@UTCOffset,EventUTCStartTime)
	) as q
	group by
		 case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, LastEndTime)) else 1 end
		,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, LastEndTime)) else datepart(yy,dateadd(mi, @UTCOffset, LastEndTime)) end
		,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, LastEndTime)) else datepart(yy,dateadd(mi, @UTCOffset, LastEndTime)) end
		,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, LastEndTime)) else datepart(yy,dateadd(mi, @UTCOffset, LastEndTime)) end
		,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, LastEndTime)) else datepart(yy,dateadd(mi, @UTCOffset, LastEndTime)) end
end
else
begin

	select
		FirstStartTime = min(dateadd(mi,@UTCOffset,StatementUTCStartTime)),
		LastEndTime = max(dateadd(mi,@UTCOffset,CompletionTime)),
		MinDuration = min(DurationMilliseconds),
		MaxDuration = max(DurationMilliseconds),
		SumDuration = sum(DurationMilliseconds),
		AvgDuration = avg(cast(DurationMilliseconds as float)),
		MinCPU = min(CPUMilliseconds),
		MaxCPU = max(CPUMilliseconds),
		SumCPU = sum(CPUMilliseconds),
		AvgCPU = avg(cast(CPUMilliseconds as float)),
		MinReads = min(Reads),
		MaxReads = max(Reads),
		SumReads = sum(Reads),
		AvgReads = avg(cast(Reads as float)),
		MinWrites = min(Writes),
		MaxWrites = max(Writes),
		SumWrites = sum(Writes),
		AvgWrites = avg(cast(Writes as float)),
		[Count] = count(*)
	from
		dbo.QueryMonitorStatistics qm (nolock)
		inner join dbo.SQLServerDatabaseNames d (nolock) on qm.DatabaseID = d.DatabaseID
		inner join ApplicationNames an on qm.ApplicationNameID = an.ApplicationNameID
		inner join AllSQLStatements s on qm.SQLStatementID = s.SQLStatementID
	where
		qm.SQLServerID = @SQLServerID and
		rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)) like rtrim(ltrim(coalesce(@ApplicationName,case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end))) and
		rtrim(ltrim(StatementType)) like coalesce(@StatementType,StatementType) and
		case when @CaseInsensitive = 1 then lower(d.DatabaseName) else d.DatabaseName end like coalesce(@DatabaseName,case when @CaseInsensitive = 1 then lower(d.DatabaseName) else d.DatabaseName end) and
		left(ltrim(case when @CaseInsensitive = 1 then lower(SQLStatement) else SQLStatement end),@LenCompare) = left(ltrim(@QueryText),@LenCompare) and
		isnull(@StartDateTime,'1 Jan 1900') <= StatementUTCStartTime and
		isnull(@EndDateTime,'1 Jan 2200') >= CompletionTime
	group by
		 case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, CompletionTime)) else 1 end
		,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, CompletionTime)) else datepart(yy,dateadd(mi, @UTCOffset, CompletionTime)) end
		,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, CompletionTime)) else datepart(yy,dateadd(mi, @UTCOffset, CompletionTime)) end
		,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, CompletionTime)) else datepart(yy,dateadd(mi, @UTCOffset, CompletionTime)) end


end


end

GO

grant EXECUTE on p_GetQueryStatistics to [SQLdmConsoleUser]

GO



