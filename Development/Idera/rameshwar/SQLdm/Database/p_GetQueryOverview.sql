if (object_id('p_GetQueryOverviewMinutesInterval') is not null)
begin
drop procedure p_GetQueryOverviewMinutesInterval
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverviewMinutesInterval 
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@StatementType int = null
as 
begin

-- SQLDM-25858: Apply isnull condition once initially and use the assigned variables in the later portion of the query
SELECT @StartDateTime = isnull(@StartDateTime,'1 Jan 1900')
SELECT @EndDateTime = ISNULL(@EndDateTime,'1 Jan 2200')

DECLARE @StartDateTimeOffset DATETIME
DECLARE @EndDateTimeOffset DATETIME

-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
SELECT @StartDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@StartDateTime) AS DATETIME)
SELECT @EndDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@EndDateTime) AS DATETIME)
 
if (@StatementType is not null)
	SELECT
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(minute, datediff(minute, 0, max(EventTime)), 0),
		EventCount =  sum(EventCount),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		EventCount = count(*),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes)
	from
		QueryMonitorStatistics (nolock)
	WHERE
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(minute, datediff(minute, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(minute, datediff(minute, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mm,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(dd,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(hh,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mi,dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	select
		EventTime = max(EventUTCStartTime),
		EventCount = sum(QueryMonitorOccurrences),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU =  sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
		QuerySignatureAggregation (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(minute, datediff(minute, 0, EventUTCStartTime), 0) and
		@EndDateTime >= dateadd(minute, datediff(minute, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
		,datepart(mm,EventUTCStartTime)
		,datepart(dd,EventUTCStartTime)
	) as qm
	group by
		datepart(yy, dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(mm,dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(dd,dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(hh,dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(mi,dateadd(mi, @UtcOffset, [EventTime]))

else
	select
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(minute, datediff(minute, 0, max(EventTime)), 0),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(TotalProcedures),
		TotalStatements = sum(TotalStatements),
		TotalBatches = sum(TotalBatches)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes),
		TotalProcedures = sum(case when StatementType = 0 then 1 else 0 end),
		TotalStatements = sum(case when StatementType = 1 then 1 else 0 end),
		TotalBatches = sum(case when StatementType = 2 then 1 else 0 end)
	from
		QueryMonitorStatistics (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(minute, datediff(minute, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(minute, datediff(minute, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mm,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(dd,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(hh,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mi,dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	
	select
		EventTime = max(EventUTCStartTime),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU = sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(case when StatementType = 0 then QueryMonitorOccurrences else 0 end),
		TotalStatements = sum(case when StatementType = 1 then QueryMonitorOccurrences else 0 end),
		TotalBatches = sum(case when StatementType = 2 then QueryMonitorOccurrences else 0 end)
	from
		QuerySignatureAggregation (nolock)
	WHERE
		@StartDateTimeOffset <= dateadd(minute, datediff(minute, 0, EventUTCStartTime), 0) and
		@EndDateTimeOffset >= dateadd(minute, datediff(minute, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
		,datepart(mm,EventUTCStartTime)
		,datepart(dd,EventUTCStartTime)
	) as qm
	group by
		datepart(yy, [EventTime])
		,datepart(mm,[EventTime])
		,datepart(dd,[EventTime])
		,datepart(hh,[EventTime])
		,datepart(mi,[EventTime])
end

Go

grant EXECUTE on p_GetQueryOverviewMinutesInterval to [SQLdmConsoleUser]

go

if (object_id('p_GetQueryOverviewHoursInterval') is not null)
begin
drop procedure p_GetQueryOverviewHoursInterval
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverviewHoursInterval 
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@StatementType int = null
as 
begin

-- SQLDM-25858: Apply isnull condition once initially and use the assigned variables in the later portion of the query
SELECT @StartDateTime = isnull(@StartDateTime,'1 Jan 1900')
SELECT @EndDateTime = ISNULL(@EndDateTime,'1 Jan 2200')

DECLARE @StartDateTimeOffset DATETIME
DECLARE @EndDateTimeOffset DATETIME

-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
SELECT @StartDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@StartDateTime) AS DATETIME)
SELECT @EndDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@EndDateTime) AS DATETIME)
 
if (@StatementType is not null)
	SELECT
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(hour, datediff(hour, 0, max(EventTime)), 0),
		EventCount =  sum(EventCount),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		EventCount = count(*),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes)
	from
		QueryMonitorStatistics (nolock)
	WHERE
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(hour, datediff(hour, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(hour, datediff(hour, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mm,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(dd,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(hh,dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	select
		EventTime = max(EventUTCStartTime),
		EventCount = sum(QueryMonitorOccurrences),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU =  sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
		QuerySignatureAggregation (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(hour, datediff(hour, 0, EventUTCStartTime), 0) and
		@EndDateTime >= dateadd(hour, datediff(hour, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
		,datepart(mm,EventUTCStartTime)
		,datepart(dd,EventUTCStartTime)
	) as qm
	group by
		datepart(yy, dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(mm,dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(dd,dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(hh,dateadd(mi, @UtcOffset, [EventTime]))

else
	select
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(hour, datediff(hour, 0, max(EventTime)), 0),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(TotalProcedures),
		TotalStatements = sum(TotalStatements),
		TotalBatches = sum(TotalBatches)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes),
		TotalProcedures = sum(case when StatementType = 0 then 1 else 0 end),
		TotalStatements = sum(case when StatementType = 1 then 1 else 0 end),
		TotalBatches = sum(case when StatementType = 2 then 1 else 0 end)
	from
		QueryMonitorStatistics (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(hour, datediff(hour, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(hour, datediff(hour, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mm,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(dd,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(hh,dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	
	select
		EventTime = max(EventUTCStartTime),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU = sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(case when StatementType = 0 then QueryMonitorOccurrences else 0 end),
		TotalStatements = sum(case when StatementType = 1 then QueryMonitorOccurrences else 0 end),
		TotalBatches = sum(case when StatementType = 2 then QueryMonitorOccurrences else 0 end)
	from
		QuerySignatureAggregation (nolock)
	WHERE
		@StartDateTimeOffset <= dateadd(hour, datediff(hour, 0, EventUTCStartTime), 0) and
		@EndDateTimeOffset >= dateadd(hour, datediff(hour, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
		,datepart(mm,EventUTCStartTime)
		,datepart(dd,EventUTCStartTime)
	) as qm
	group by
		datepart(yy, [EventTime])
		,datepart(mm,[EventTime])
		,datepart(dd,[EventTime])
		,datepart(hh,[EventTime])
end

Go

grant EXECUTE on p_GetQueryOverviewHoursInterval to [SQLdmConsoleUser]

go

IF (object_id('p_GetQueryOverviewDaysInterval') is not null)
begin
drop procedure p_GetQueryOverviewDaysInterval
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverviewDaysInterval 
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@StatementType int = null
as 
begin

-- SQLDM-25858: Apply isnull condition once initially and use the assigned variables in the later portion of the query
SELECT @StartDateTime = isnull(@StartDateTime,'1 Jan 1900')
SELECT @EndDateTime = ISNULL(@EndDateTime,'1 Jan 2200')

DECLARE @StartDateTimeOffset DATETIME
DECLARE @EndDateTimeOffset DATETIME

-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
SELECT @StartDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@StartDateTime) AS DATETIME)
SELECT @EndDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@EndDateTime) AS DATETIME)
 
if (@StatementType is not null)
	SELECT
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(day, datediff(day, 0, max(EventTime)), 0),
		EventCount =  sum(EventCount),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		EventCount = count(*),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes)
	from
		QueryMonitorStatistics (nolock)
	WHERE
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(day, datediff(day, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(day, datediff(day, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mm,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(dd,dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	select
		EventTime = max(EventUTCStartTime),
		EventCount = sum(QueryMonitorOccurrences),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU =  sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
		QuerySignatureAggregation (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(day, datediff(day, 0, EventUTCStartTime), 0) and
		@EndDateTime >= dateadd(day, datediff(day, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
		,datepart(mm,EventUTCStartTime)
		,datepart(dd,EventUTCStartTime)
	) as qm
	group by
		datepart(yy, dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(mm,dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(dd,dateadd(mi, @UtcOffset, [EventTime]))

else
	select
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(day, datediff(day, 0, max(EventTime)), 0),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(TotalProcedures),
		TotalStatements = sum(TotalStatements),
		TotalBatches = sum(TotalBatches)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes),
		TotalProcedures = sum(case when StatementType = 0 then 1 else 0 end),
		TotalStatements = sum(case when StatementType = 1 then 1 else 0 end),
		TotalBatches = sum(case when StatementType = 2 then 1 else 0 end)
	from
		QueryMonitorStatistics (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(day, datediff(day, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(day, datediff(day, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mm,dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(dd,dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	
	select
		EventTime = max(EventUTCStartTime),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU = sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(case when StatementType = 0 then QueryMonitorOccurrences else 0 end),
		TotalStatements = sum(case when StatementType = 1 then QueryMonitorOccurrences else 0 end),
		TotalBatches = sum(case when StatementType = 2 then QueryMonitorOccurrences else 0 end)
	from
		QuerySignatureAggregation (nolock)
	WHERE
		@StartDateTimeOffset <= dateadd(day, datediff(day, 0, EventUTCStartTime), 0) and
		@EndDateTimeOffset >= dateadd(day, datediff(day, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
		,datepart(mm,EventUTCStartTime)
		,datepart(dd,EventUTCStartTime)
	) as qm
	group by
		datepart(yy, [EventTime])
		,datepart(mm,[EventTime])
		,datepart(dd,[EventTime])
end

Go

grant EXECUTE on p_GetQueryOverviewDaysInterval to [SQLdmConsoleUser]

go

if (object_id('p_GetQueryOverviewMonthsInterval') is not null)
begin
drop procedure p_GetQueryOverviewMonthsInterval
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverviewMonthsInterval 
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@StatementType int = null
as 
begin

-- SQLDM-25858: Apply isnull condition once initially and use the assigned variables in the later portion of the query
SELECT @StartDateTime = isnull(@StartDateTime,'1 Jan 1900')
SELECT @EndDateTime = ISNULL(@EndDateTime,'1 Jan 2200')

DECLARE @StartDateTimeOffset DATETIME
DECLARE @EndDateTimeOffset DATETIME

-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
SELECT @StartDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@StartDateTime) AS DATETIME)
SELECT @EndDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@EndDateTime) AS DATETIME)
 
if (@StatementType is not null)
	SELECT
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(month, datediff(month, 0, max(EventTime)), 0),
		EventCount =  sum(EventCount),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		EventCount = count(*),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes)
	from
		QueryMonitorStatistics (nolock)
	WHERE
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(month, datediff(month, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(month, datediff(month, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mm,dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	select
		EventTime = max(EventUTCStartTime),
		EventCount = sum(QueryMonitorOccurrences),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU =  sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
		QuerySignatureAggregation (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(month, datediff(month, 0, EventUTCStartTime), 0) and
		@EndDateTime >= dateadd(month, datediff(month, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
		,datepart(mm,EventUTCStartTime)
	) as qm
	group by
		datepart(yy, dateadd(mi, @UtcOffset, [EventTime]))
		,datepart(mm,dateadd(mi, @UtcOffset, [EventTime]))

else
	select
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(month, datediff(month, 0, max(EventTime)), 0),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(TotalProcedures),
		TotalStatements = sum(TotalStatements),
		TotalBatches = sum(TotalBatches)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes),
		TotalProcedures = sum(case when StatementType = 0 then 1 else 0 end),
		TotalStatements = sum(case when StatementType = 1 then 1 else 0 end),
		TotalBatches = sum(case when StatementType = 2 then 1 else 0 end)
	from
		QueryMonitorStatistics (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(month, datediff(month, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(month, datediff(month, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
		,datepart(mm,dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	
	select
		EventTime = max(EventUTCStartTime),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU = sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(case when StatementType = 0 then QueryMonitorOccurrences else 0 end),
		TotalStatements = sum(case when StatementType = 1 then QueryMonitorOccurrences else 0 end),
		TotalBatches = sum(case when StatementType = 2 then QueryMonitorOccurrences else 0 end)
	from
		QuerySignatureAggregation (nolock)
	WHERE
		@StartDateTimeOffset <= dateadd(month, datediff(month, 0, EventUTCStartTime), 0) and
		@EndDateTimeOffset >= dateadd(month, datediff(month, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
		,datepart(mm,EventUTCStartTime)
	) as qm
	group by
		datepart(yy, [EventTime])
		,datepart(mm,[EventTime])
end

Go

grant EXECUTE on p_GetQueryOverviewMonthsInterval to [SQLdmConsoleUser]

go

if (object_id('p_GetQueryOverviewYearsInterval') is not null)
begin
drop procedure p_GetQueryOverviewYearsInterval
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverviewYearsInterval 
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@StatementType int = null
as 
begin

-- SQLDM-25858: Apply isnull condition once initially and use the assigned variables in the later portion of the query
SELECT @StartDateTime = isnull(@StartDateTime,'1 Jan 1900')
SELECT @EndDateTime = ISNULL(@EndDateTime,'1 Jan 2200')

DECLARE @StartDateTimeOffset DATETIME
DECLARE @EndDateTimeOffset DATETIME

-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
SELECT @StartDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@StartDateTime) AS DATETIME)
SELECT @EndDateTimeOffset = CAST(dateadd(mi, @UtcOffset,@EndDateTime) AS DATETIME)
 
if (@StatementType is not null)
	SELECT
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(month, datediff(month, 0, max(EventTime)), 0),
		EventCount =  sum(EventCount),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		EventCount = count(*),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes)
	from
		QueryMonitorStatistics (nolock)
	WHERE
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(month, datediff(month, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(month, datediff(month, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	select
		EventTime = max(EventUTCStartTime),
		EventCount = sum(QueryMonitorOccurrences),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU =  sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites)
	from
		QuerySignatureAggregation (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(month, datediff(month, 0, EventUTCStartTime), 0) and
		@EndDateTime >= dateadd(month, datediff(month, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		StatementType = @StatementType and
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
	) as qm
	group by
		datepart(yy, dateadd(mi, @UtcOffset, [EventTime]))

else
	select
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		LastCollectionInInterval = dateadd(month, datediff(month, 0, max(EventTime)), 0),
		TotalDuration = sum(TotalDuration),
		TotalCPU = sum(TotalCPU),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(TotalProcedures),
		TotalStatements = sum(TotalStatements),
		TotalBatches = sum(TotalBatches)
	from
	(
	select
		EventTime = max(dateadd(mi, @UtcOffset, CompletionTime)),
		TotalDuration = sum(DurationMilliseconds),
		TotalCPU = sum(CPUMilliseconds),
		TotalReads = sum(Reads),
		TotalWrites = sum(Writes),
		TotalProcedures = sum(case when StatementType = 0 then 1 else 0 end),
		TotalStatements = sum(case when StatementType = 1 then 1 else 0 end),
		TotalBatches = sum(case when StatementType = 2 then 1 else 0 end)
	from
		QueryMonitorStatistics (nolock)
	where
		-- SQLDM-25858: Use CAST() instead of dbo.fn_RoundDateTime()
		@StartDateTime <= dateadd(month, datediff(month, 0, StatementUTCStartTime), 0) and
		@EndDateTime >= dateadd(month, datediff(month, 0, CompletionTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, dateadd(mi, @UtcOffset,[CompletionTime]))
	union all
	
	select
		EventTime = max(EventUTCStartTime),
		TotalDuration = sum(TotalDurationMilliseconds),
		TotalCPU = sum(TotalCPUMilliseconds),
		TotalReads = sum(TotalReads),
		TotalWrites = sum(TotalWrites),
		TotalProcedures = sum(case when StatementType = 0 then QueryMonitorOccurrences else 0 end),
		TotalStatements = sum(case when StatementType = 1 then QueryMonitorOccurrences else 0 end),
		TotalBatches = sum(case when StatementType = 2 then QueryMonitorOccurrences else 0 end)
	from
		QuerySignatureAggregation (nolock)
	WHERE
		@StartDateTimeOffset <= dateadd(month, datediff(month, 0, EventUTCStartTime), 0) and
		@EndDateTimeOffset >= dateadd(month, datediff(month, 0, EventUTCStartTime), 0) and
		-- SQLDM-25858: isnull on @StatementType is not required as it is already used in the above if condition
		SQLServerID = @SQLServerID
	group by
		datepart(yy, EventUTCStartTime)
	) as qm
	group by
		datepart(yy, [EventTime])
end

Go

grant EXECUTE on p_GetQueryOverviewYearsInterval to [SQLdmConsoleUser]

go

IF (object_id('p_GetQueryOverview') is not null)
begin
drop procedure p_GetQueryOverview
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverview 
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@Interval int,
	@StatementType int = null
as 
begin

IF(@Interval = 0)
BEGIN
	EXEC dbo.p_GetQueryOverviewMinutesInterval 
		@SQLServerID, -- int
	    @StartDateTime, -- datetime
	    @EndDateTime, -- datetime
	    @UtcOffset, -- int
	    @StatementType -- int
end
ELSE IF(@Interval = 1)
BEGIN
	EXEC dbo.p_GetQueryOverviewHoursInterval 
		@SQLServerID, -- int
	    @StartDateTime, -- datetime
	    @EndDateTime, -- datetime
	    @UtcOffset, -- int
	    @StatementType -- int
END
ELSE IF(@Interval = 2)
BEGIN
	EXEC dbo.p_GetQueryOverviewDaysInterval
		@SQLServerID, -- int
	    @StartDateTime, -- datetime
	    @EndDateTime, -- datetime
	    @UtcOffset, -- int
	    @StatementType -- int
END
ELSE IF(@Interval = 3)
BEGIN
	EXEC dbo.p_GetQueryOverviewMonthsInterval 
		@SQLServerID, -- int
	    @StartDateTime, -- datetime
	    @EndDateTime, -- datetime
	    @UtcOffset, -- int
	    @StatementType -- int
END
ELSE IF(@Interval = 4)
BEGIN
	EXEC dbo.p_GetQueryOverviewYearsInterval 
		@SQLServerID, -- int
	    @StartDateTime, -- datetime
	    @EndDateTime, -- datetime
	    @UtcOffset, -- int
	    @StatementType -- int
END
END

Go

grant EXECUTE on p_GetQueryOverview to [SQLdmConsoleUser]

go

if (object_id('p_GetQueryOverviewStoredProcedures') is not null)
begin
drop procedure p_GetQueryOverviewStoredProcedures
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverviewStoredProcedures
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@Interval int
as 
begin
	
	exec p_GetQueryOverview
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@Interval,
		0
end

Go

grant EXECUTE on p_GetQueryOverviewStoredProcedures to [SQLdmConsoleUser]

go

if (object_id('p_GetQueryOverviewStatements') is not null)
begin
drop procedure p_GetQueryOverviewStatements
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverviewStatements
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@Interval int
as 
begin
	
	exec p_GetQueryOverview
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@Interval,
		1
end

Go

grant EXECUTE on p_GetQueryOverviewStatements to [SQLdmConsoleUser]

go

if (object_id('p_GetQueryOverviewBatches') is not null)
begin
drop procedure p_GetQueryOverviewBatches
end
go
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetQueryOverviewBatches
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = 0,
	@Interval int
as 
begin
	
	exec p_GetQueryOverview
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@Interval,
		2
end

Go

grant EXECUTE on p_GetQueryOverviewBatches to [SQLdmConsoleUser]

GO
