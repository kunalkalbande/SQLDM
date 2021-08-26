--Modified SQL DM 8.6 (Vineet Kumar) -- Fixing defect DE41457. After deploying on SSRS Top Queries report gives error.
--sp [p_GetTopQueries] is called by several other stored procedures at same time. [p_GetTopQueries] makes use of global temp tables. When it gets called by 5-6 other procs at same time, the scope of these temp tables gets into conflict.
--Solution
--Global temptables(##) are changed to local temp tables(#), so the scope of table exists within the proc call itself. 
--Now local temptables can not be accessed outside the p_GetTopQueries. So at the end of proc, select statement is used. In the proc(such as 'p_GetTopQueriesByFrequency')  calling it, we create a local temptable with scope to this particular proc. Then insert data of p_GetTopQueries in this table and perform the required select statement.

if (object_id('p_GetTopQueries') is not null)
begin
drop procedure p_GetTopQueries
end
go
-- @OrderBy
--	0 - Frequency
--	1 - Duration
--	2 - CPU
--	3 - Reads
--	4 - Writes
create procedure [dbo].p_GetTopQueries
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@OrderBy tinyint = 0,
	@CaseInsensitive bit = 0,
	@ApplLike nvarchar(255) = null,
	@ApplNotLike nvarchar(255) = null,
	@ClientLike nvarchar(255) = null,
	@ClientNotLike nvarchar(255) = null,
	@DatabaseLike nvarchar(255) = null,
	@DatabaseNotLike nvarchar(255) = null,
	@SQLTextLike nvarchar(255) = null,
	@SQLTextNotLike nvarchar(255) = null, 
	@UserLike nvarchar(255) = null, 
	@UserNotLike nvarchar(255) = null
as
begin
set ansi_warnings off
set ansi_nulls off

if @UtcOffset is null
	set @UtcOffset = datediff(mi,getutcdate(),getdate())

declare @IntermediateTable table(DatabaseID int, DatabaseName nvarchar(255))

declare @AllTypes bit 
if @IncludeStoredProcedures = 1 AND @IncludeSQLStatements = 1 AND @IncludeSQLBatches = 1
	set @AllTypes = 1
else
	set @AllTypes = 0

if @CaseInsensitive = 1
begin
	select @SQLTextLike = lower(@SQLTextLike),
		@SQLTextNotLike = lower(@SQLTextNotLike),
		@ApplLike = lower(@ApplLike),
		@ApplNotLike = lower(@ApplNotLike),
		@ClientLike = lower(@ClientLike),
		@ClientNotLike = lower(@ClientNotLike),
		@DatabaseLike = lower(@DatabaseLike),
		@DatabaseNotLike = lower(@DatabaseNotLike)
end
-- SQLDM-25858: Apply isnull and other conditions once initially and use the assigned variables in the later portion of the query
DECLARE @DatabaseLikeNullValue nvarchar(255)
SELECT @DatabaseLikeNullValue = CASE WHEN @DatabaseLike IS NULL THEN '%' ELSE @DatabaseLike END collate SQL_Latin1_General_CP1_CI_AS

DECLARE @ApplLikeNullValue nvarchar(255)
SELECT @ApplLikeNullValue = CASE WHEN @ApplLike IS NULL THEN '%' collate SQL_Latin1_General_CP1_CI_AS ELSE @ApplLike END

DECLARE @ClientLikeNullValue nvarchar(255)
SELECT @ClientLikeNullValue = CASE WHEN @ClientLike IS NULL THEN '%' collate SQL_Latin1_General_CP1_CI_AS ELSE @ClientLike END

DECLARE @UserLikeNullValue nvarchar(255)
SELECT @UserLikeNullValue = CASE WHEN @UserLike IS NULL THEN '%' collate SQL_Latin1_General_CP1_CI_AS ELSE @UserLike END

DECLARE @SQLTextLikeNullValue nvarchar(255)
SELECT @SQLTextLikeNullValue = CASE WHEN @SQLTextLike IS NULL THEN '%' collate SQL_Latin1_General_CP1_CI_AS ELSE @SQLTextLike END

SELECT @MinExecutions = isnull(@MinExecutions,0)
SELECT @MinDuration = isnull(@MinDuration,0)
SELECT @MinReads = isnull(@MinReads,0)
SELECT @MinWrites = isnull(@MinWrites,0)
SELECT @MinCPU = isnull(@MinCPU,0)
SELECT @StartDateTime = isnull(dateadd(minute, datediff(minute, 0, @StartDateTime), 0),'1 Jan 1900')
SELECT @EndDateTime = 	isnull(dateadd(minute, datediff(minute, 0, @EndDateTime), 0),'1 Jan 2200')
insert into @IntermediateTable(DatabaseID, DatabaseName)
	select DatabaseID, DatabaseName
	from
		[SQLServerDatabaseNames] d (nolock)
	where
		d.SQLServerID = @SQLServerID
		and (@DatabaseNotLike is null or rtrim(ltrim(case when @CaseInsensitive = 1 then lower(DatabaseName) else DatabaseName end)) not like @DatabaseNotLike collate SQL_Latin1_General_CP1_CI_AS)
		and (rtrim(ltrim(case when @CaseInsensitive = 1 then lower(DatabaseName) else DatabaseName end)) like isnull(@DatabaseLike, '%') collate SQL_Latin1_General_CP1_CI_AS)

-- Create a temporal table to contains the result of the query
--Start : Modified SQL DM 8.6 (Vineet Kumar) -- Fixing defect DE41457. -- Changed global temptable to local temptable
if (object_id('tempdb..#TmpTopQueriesCaseSensitive') is not null)
begin
    drop table #TmpTopQueriesCaseSensitive
end

-- Create a temporal table to contains the result of the query
if (object_id('tempdb..#TmpTopQueriesNoCaseSensitive') is not null)
begin
    drop table #TmpTopQueriesNoCaseSensitive
end
--End : Modified SQL DM 8.6 (Vineet Kumar) -- Fixing defect DE41457. -- Changed global temptable to local temptable

if (@SignatureMode = 1)
begin

CREATE TABLE #TmpTopQueriesCaseSensitive(
DatabaseName NVARCHAR(255) NOT NULL,
FirstStartTime DATETIME NOT NULL,
LastEndTime DATETIME NULL,
AverageDuration DECIMAL(16,2) NULL,
AverageCPU DECIMAL(16,2) NULL,
AverageReads DECIMAL(16,2) NULL,
AverageWrites DECIMAL(16,2) NULL,
ApplicationName NVARCHAR(256) NULL,
StatementType INT NULL,
StatementText NVARCHAR(MAX) NULL,
[Count] INT NOT NULL
)

INSERT INTO #TmpTopQueriesCaseSensitive
(
DatabaseName,
FirstStartTime,
LastEndTime,
AverageDuration,
AverageCPU,
AverageReads,
AverageWrites,
ApplicationName,
StatementType,
StatementText,
[Count]
)
select
	DatabaseName,
	FirstStartTime = min(FirstStartTime),
	LastEndTime = max(LastEndTime),
	AverageDuration = CAST(floor(sum(TotalDuration) / nullif(sum([Count]),0)) AS DECIMAL(16,2)),
	AverageCPU = CAST(floor(sum(TotalCPU) / nullif(sum([Count]),0)) AS DECIMAL(16,2)),
	AverageReads = CAST(floor(sum(TotalReads) / nullif(sum([Count]),0)) AS DECIMAL(16,2)),
	AverageWrites = CAST(floor(sum(TotalWrites) / nullif(sum([Count]),0)) AS DECIMAL(16,2)),
	ApplicationName,
	StatementType,
	StatementText,
	[Count] = sum([Count])
from
(
select 
	DatabaseName = d.DatabaseName,
	FirstStartTime = min(dateadd(mi,@UtcOffset,StatementUTCStartTime)),
	LastEndTime = max(dateadd(mi,@UtcOffset,CompletionTime)),
	TotalDuration = sum(DurationMilliseconds),
	TotalCPU = sum(CPUMilliseconds),
	TotalReads = sum(Reads),
	TotalWrites = sum(Writes),
	ApplicationName,
	StatementType,
	StatementText = SQLSignature,
	[Count] = count(*)
from
	dbo.QueryMonitorStatistics qm (nolock)
	inner join @IntermediateTable d on qm.DatabaseID = d.DatabaseID
	inner join ApplicationNames an (nolock) on qm.ApplicationNameID = an.ApplicationNameID
	inner join AllSQLSignatures s (nolock) on qm.SQLSignatureID = s.SQLSignatureID
	inner join HostNames hn (nolock) on qm.HostNameID = hn.HostNameID
	inner join LoginNames ln (nolock) on qm.LoginNameID = ln.LoginNameID
where
	qm.SQLServerID = @SQLServerID and
	(
		@AllTypes = 1
		OR
		qm.StatementType = 0 and @IncludeStoredProcedures = 1
		OR
		qm.StatementType = 1 and @IncludeSQLStatements = 1
		OR
		qm.StatementType = 2 and @IncludeSQLBatches = 1
	) and
	-- SQLDM-25858: isnull on @StartDateTime is not required as it is already used in the above if condition
	@StartDateTime <= StatementUTCStartTime and
	@EndDateTime >= CompletionTime
	and (@ApplNotLike is null or isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)),'') not like @ApplNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)),'') like @ApplLikeNullValue
	and (@ClientNotLike is null or isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(HostName) else HostName end)),'') not like @ClientNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(HostName) else HostName end)),'') like @ClientLikeNullValue
	and (@UserNotLike is null or isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(LoginName) else LoginName end)),'') not like @UserNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(LoginName) else LoginName end)),'') like @UserLikeNullValue
	and (@SQLTextNotLike is null or case when @CaseInsensitive = 1 then isnull(lower(SQLSignature),'') else isnull(SQLSignature,'') end not like @SQLTextNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and case when @CaseInsensitive = 1 then isnull(lower(SQLSignature),'') else isnull(SQLSignature,'') end LIKE @SQLTextLikeNullValue
group by
	StatementType,
	SQLSignature,
	d.DatabaseName,
	ApplicationName

union all

select 
	DatabaseName = d.DatabaseName,
	FirstStartTime = min(dateadd(mi,@UtcOffset,EventUTCStartTime)),
	LastEndTime = max(dateadd(mi,@UtcOffset,EventUTCStartTime)),
	TotalDuration = sum(TotalDurationMilliseconds),
	TotalCPU = sum(TotalCPUMilliseconds),
	TotalReads = sum(TotalReads),
	TotalWrites = sum(TotalWrites),
	ApplicationName,
	StatementType,
	StatementText = SQLSignature,
	sum(QueryMonitorOccurrences) AS [Count]
from
	dbo.QuerySignatureAggregation qm (nolock)
	inner join @IntermediateTable d on qm.DatabaseID = d.DatabaseID
	inner join ApplicationNames an (nolock) on qm.ApplicationNameID = an.ApplicationNameID
	inner join AllSQLSignatures s (nolock) on qm.SQLSignatureID = s.SQLSignatureID
where

	qm.SQLServerID = @SQLServerID and
	(
		@AllTypes = 1
		OR
		qm.StatementType = 0 and @IncludeStoredProcedures = 1
		OR
		qm.StatementType = 1 and @IncludeSQLStatements = 1
		OR
		qm.StatementType = 2 and @IncludeSQLBatches = 1
	) and
	-- SQLDM-25858: isnull on @StartDateTime is not required as it is already used in the above if condition
	@StartDateTime <= EventUTCStartTime and
	@EndDateTime >= EventUTCStartTime
	and (@ApplNotLike is null or isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)),'') not like @ApplNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)),'') like @ApplLikeNullValue
	and (@SQLTextNotLike is null or case when @CaseInsensitive = 1 then isnull(lower(SQLSignature),'') else isnull(SQLSignature,'') end not like @SQLTextNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and case when @CaseInsensitive = 1 then isnull(lower(SQLSignature),'') else isnull(SQLSignature,'') end like @SQLTextLikeNullValue
group by
	StatementType,
	SQLSignature,
	DatabaseName,
	ApplicationName
)
as q
group by
	StatementType,
	StatementText,
	DatabaseName,
	ApplicationName
having
	sum([Count]) >= @MinExecutions and
	(CAST((sum(TotalDuration) / nullif(sum([Count]),0)) AS BIGINT)) >= @MinDuration and
	(CAST((sum(TotalReads) / nullif(sum([Count]),0)) AS BIGINT)) >= @MinReads and
	(CAST((sum(TotalWrites) / nullif(sum([Count]),0)) AS BIGINT)) >= @MinWrites and
	(CAST((sum(TotalCPU) / nullif(sum([Count]),0)) AS BIGINT)) >= @MinCPU
order by
	case 
		when @OrderBy = 1 then (sum(TotalDuration) / nullif(sum([Count]),0))
		when @OrderBy = 2 then (sum(TotalCPU) / nullif(sum([Count]),0))
		when @OrderBy = 3 then (sum(TotalReads) / nullif(sum([Count]),0))
		when @OrderBy = 4 then (sum(TotalWrites) / nullif(sum([Count]),0))
	end 
	desc

end
else
begin

CREATE TABLE #TmpTopQueriesNoCaseSensitive(
DatabaseName NVARCHAR(255) NOT NULL,
FirstStartTime DATETIME NOT NULL,
LastEndTime DATETIME NULL,
AverageDuration DECIMAL(16,2) NULL,
AverageCPU DECIMAL(16,2) NULL,
AverageReads DECIMAL(16,2) NULL,
AverageWrites DECIMAL(16,2) NULL,
ApplicationName NVARCHAR(256) NULL,
StatementType INT NULL,
StatementText NVARCHAR(MAX) NULL,
[Count] INT NOT NULL
)

INSERT INTO #TmpTopQueriesNoCaseSensitive
(
DatabaseName,
FirstStartTime,
LastEndTime,
AverageDuration,
AverageCPU,
AverageReads,
AverageWrites,
ApplicationName,
StatementType,
StatementText,
[Count]
)
select 
	DatabaseName,
	FirstStartTime = min(dateadd(mi,@UtcOffset,StatementUTCStartTime)),
	LastEndTime = max(dateadd(mi,@UtcOffset,CompletionTime)),
	AverageDuration = CAST(avg(DurationMilliseconds) AS DECIMAL(16,2)),
	AverageCPU = CAST(avg(CPUMilliseconds) AS DECIMAL(16,2)),
	AverageReads = CAST(avg(Reads) AS DECIMAL(16,2)),
	AverageWrites = CAST(avg(Writes) AS DECIMAL(16,2)),
	ApplicationName,
	StatementType,
	StatementText = SQLStatement,
	[Count] = count(*)
from
	dbo.QueryMonitorStatistics qm (nolock)
	inner join @IntermediateTable d on qm.DatabaseID = d.DatabaseID
	inner join ApplicationNames an (nolock) on qm.ApplicationNameID = an.ApplicationNameID
	inner join AllSQLStatements s (nolock) on qm.SQLStatementID = s.SQLStatementID
	inner join HostNames hn (nolock) on qm.HostNameID = hn.HostNameID
	inner join LoginNames ln (nolock) on qm.LoginNameID = ln.LoginNameID

where
	qm.SQLServerID = @SQLServerID and
	(
		@AllTypes = 1
		OR
		qm.StatementType = 0 and @IncludeStoredProcedures = 1
		OR
		qm.StatementType = 1 and @IncludeSQLStatements = 1
		OR
		qm.StatementType = 2 and @IncludeSQLBatches = 1
	) and
	@StartDateTime <= StatementUTCStartTime and
	@EndDateTime >= CompletionTime
	and (@ApplNotLike is null or isnull(ApplicationName,'') not like @ApplNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(ApplicationName) else ApplicationName end)),'') like @ApplLikeNullValue
	and (@ClientNotLike is null or isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(HostName) else HostName end)),'') not like @ClientNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(HostName) else HostName end)),'') like @ClientLikeNullValue
	and (@UserNotLike is null or isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(LoginName) else LoginName end)),'') not like @UserNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and isnull(rtrim(ltrim(case when @CaseInsensitive = 1 then lower(LoginName) else LoginName end)),'') like @UserLikeNullValue
	and (@SQLTextNotLike is null or case when @CaseInsensitive = 1 then isnull(lower(SQLStatement),'') else isnull(SQLStatement,'') end not like @SQLTextNotLike collate SQL_Latin1_General_CP1_CI_AS)
	and case when @CaseInsensitive = 1 then isnull(lower(SQLStatement),'') else isnull(SQLStatement,'') end like @SQLTextLikeNullValue
group by
	StatementType,
	SQLStatement,
	DatabaseName,
	ApplicationName
having
	count(*) >= @MinExecutions and
	avg(DurationMilliseconds) >= @MinDuration and
	avg(Reads) >= @MinReads and
	avg(Writes) >= @MinWrites and
	avg(CPUMilliseconds) >= @MinCPU
order by
	case
		when @OrderBy = 1 then avg(DurationMilliseconds)
		when @OrderBy = 2 then avg(CPUMilliseconds)
		when @OrderBy = 3 then avg(Reads)
		when @OrderBy = 4 then avg(Writes)
	end 
	desc

end

--Start : Modified SQL DM 8.6 (Vineet Kumar) -- This select query added because local temptable cannot be accessed outside this proc
if (@SignatureMode = 1)
select * from #TmpTopQueriesCaseSensitive
else 
select * from #TmpTopQueriesNoCaseSensitive
--End : Modified SQL DM 8.6 (Vineet Kumar) -- This select query added because local temptable cannot be accessed outside this proc

end

Go

grant EXECUTE on p_GetTopQueries to [SQLdmConsoleUser]

go


if (object_id('p_GetTopQueriesByFrequency') is not null)
begin
drop procedure p_GetTopQueriesByFrequency
end
go
create procedure [dbo].p_GetTopQueriesByFrequency
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0,
	@ApplLike nvarchar(255) = null,
	@ApplNotLike nvarchar(255) = null,
	@ClientLike nvarchar(255) = null,
	@ClientNotLike nvarchar(255) = null,
	@DatabaseLike nvarchar(255) = null,
	@DatabaseNotLike nvarchar(255) = null,
	@SQLTextLike nvarchar(255) = null,
	@SQLTextNotLike nvarchar(255) = null, 
	@UserLike nvarchar(255) = null, 
	@UserNotLike nvarchar(255) = null
as
begin
--Modified SQL DM 8.6 (Vineet Kumar) -- Local temptable created and inserts output of p_GetTopQueries into this
CREATE TABLE #TempTableForData
(
DatabaseName varchar(255),
	FirstStartTime DATETIME,
	LastEndTime DATETIME,
	AverageDuration INT,
	AverageCPU INT,
	AverageReads INT,
	AverageWrites INT,
	ApplicationName varchar(255),
	StatementType BIT,
	StatementText varchar(max),
	[Count] INT

)
INSERT INTO #TempTableForData
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		0,
		@CaseInsensitive,
		@ApplLike,
		@ApplNotLike,
		@ClientLike,
		@ClientNotLike,
		@DatabaseLike,
		@DatabaseNotLike,
		@SQLTextLike,
		@SQLTextNotLike,
		@UserLike,
		@UserNotLike

    -- Sort acording [Count] column.
    set rowcount @TopN

	select distinct * from #TempTableForData order by [Count] desc

end

Go

grant EXECUTE on p_GetTopQueriesByFrequency to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByDuration') is not null)
begin
drop procedure p_GetTopQueriesByDuration
end
go
create procedure [dbo].p_GetTopQueriesByDuration
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0,
	@ApplLike nvarchar(255) = null,
	@ApplNotLike nvarchar(255) = null,
	@ClientLike nvarchar(255) = null,
	@ClientNotLike nvarchar(255) = null,
	@DatabaseLike nvarchar(255) = null,
	@DatabaseNotLike nvarchar(255) = null,
	@SQLTextLike nvarchar(255) = null,
	@SQLTextNotLike nvarchar(255) = null, 
	@UserLike nvarchar(255) = null, 
	@UserNotLike nvarchar(255) = null
as
begin
--Modified SQL DM 8.6 (Vineet Kumar) -- Local temptable created and inserts output of p_GetTopQueries into this
	CREATE TABLE #TempTableForData
(
DatabaseName varchar(255),
	FirstStartTime DATETIME,
	LastEndTime DATETIME,
	AverageDuration INT,
	AverageCPU INT,
	AverageReads INT,
	AverageWrites INT,
	ApplicationName varchar(255),
	StatementType BIT,
	StatementText varchar(max),
	[Count] INT

)
INSERT INTO #TempTableForData
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		1,
		@CaseInsensitive,
		@ApplLike,
		@ApplNotLike,
		@ClientLike,
		@ClientNotLike,
		@DatabaseLike,
		@DatabaseNotLike,
		@SQLTextLike,
		@SQLTextNotLike,
		@UserLike,
		@UserNotLike

    set rowcount @TopN

	select distinct * from #TempTableForData order by [AverageDuration] desc
end

Go

grant EXECUTE on p_GetTopQueriesByDuration to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByCPU') is not null)
begin
drop procedure p_GetTopQueriesByCPU
end
go
create procedure [dbo].p_GetTopQueriesByCPU
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0,
	@ApplLike nvarchar(255) = null,
	@ApplNotLike nvarchar(255) = null,
	@ClientLike nvarchar(255) = null,
	@ClientNotLike nvarchar(255) = null,
	@DatabaseLike nvarchar(255) = null,
	@DatabaseNotLike nvarchar(255) = null,
	@SQLTextLike nvarchar(255) = null,
	@SQLTextNotLike nvarchar(255) = null, 
	@UserLike nvarchar(255) = null, 
	@UserNotLike nvarchar(255) = null
as
begin
--Modified SQL DM 8.6 (Vineet Kumar) -- Local temptable created and inserts output of p_GetTopQueries into this
	CREATE TABLE #TempTableForData
(
DatabaseName varchar(255),
	FirstStartTime DATETIME,
	LastEndTime DATETIME,
	AverageDuration INT,
	AverageCPU INT,
	AverageReads INT,
	AverageWrites INT,
	ApplicationName varchar(255),
	StatementType BIT,
	StatementText varchar(max),
	[Count] INT

)
INSERT INTO #TempTableForData
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		2,
		@CaseInsensitive,
		@ApplLike,
		@ApplNotLike,
		@ClientLike,
		@ClientNotLike,
		@DatabaseLike,
		@DatabaseNotLike,
		@SQLTextLike,
		@SQLTextNotLike,
		@UserLike,
		@UserNotLike

    set rowcount @TopN

	select distinct * from #TempTableForData order by [AverageCPU] desc 
end

Go

grant EXECUTE on p_GetTopQueriesByCPU to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByReads') is not null)
begin
drop procedure p_GetTopQueriesByReads
end
go
create procedure [dbo].p_GetTopQueriesByReads
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0,
	@ApplLike nvarchar(255) = null,
	@ApplNotLike nvarchar(255) = null,
	@ClientLike nvarchar(255) = null,
	@ClientNotLike nvarchar(255) = null,
	@DatabaseLike nvarchar(255) = null,
	@DatabaseNotLike nvarchar(255) = null,
	@SQLTextLike nvarchar(255) = null,
	@SQLTextNotLike nvarchar(255) = null, 
	@UserLike nvarchar(255) = null, 
	@UserNotLike nvarchar(255) = null
as
begin
--Modified SQL DM 8.6 (Vineet Kumar) -- Local temptable created and inserts output of p_GetTopQueries into this
	CREATE TABLE #TempTableForData
(
DatabaseName varchar(255),
	FirstStartTime DATETIME,
	LastEndTime DATETIME,
	AverageDuration INT,
	AverageCPU INT,
	AverageReads INT,
	AverageWrites INT,
	ApplicationName varchar(255),
	StatementType BIT,
	StatementText varchar(max),
	[Count] INT

)
INSERT INTO #TempTableForData
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		3,
		@CaseInsensitive,
		@ApplLike,
		@ApplNotLike,
		@ClientLike,
		@ClientNotLike,
		@DatabaseLike,
		@DatabaseNotLike,
		@SQLTextLike,
		@SQLTextNotLike,
		@UserLike,
		@UserNotLike

    set rowcount @TopN

	select distinct * from #TempTableForData order by [AverageReads] desc 
end

Go

grant EXECUTE on p_GetTopQueriesByReads to [SQLdmConsoleUser]

go

if (object_id('p_GetTopQueriesByWrites') is not null)
begin
drop procedure p_GetTopQueriesByWrites
end
go
create procedure [dbo].p_GetTopQueriesByWrites
	@SQLServerID int,
	@StartDateTime datetime = null,
	@EndDateTime datetime = null,
	@UtcOffset int = null,
	@IncludeStoredProcedures bit = 1,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLBatches bit = 1,
	@MinExecutions bigint = null,
	@MinDuration bigint = null,
	@MinReads bigint = null,
	@MinWrites bigint = null,
	@MinCPU bigint = null,
	@TopN bigint = 10,
	@SignatureMode bit = 0,
	@CaseInsensitive bit = 0,
	@ApplLike nvarchar(255) = null,
	@ApplNotLike nvarchar(255) = null,
	@ClientLike nvarchar(255) = null,
	@ClientNotLike nvarchar(255) = null,
	@DatabaseLike nvarchar(255) = null,
	@DatabaseNotLike nvarchar(255) = null,
	@SQLTextLike nvarchar(255) = null,
	@SQLTextNotLike nvarchar(255) = null, 
	@UserLike nvarchar(255) = null, 
	@UserNotLike nvarchar(255) = null
as
begin
--Modified SQL DM 8.6 (Vineet Kumar) -- Local temptable created and inserts output of p_GetTopQueries into this
	CREATE TABLE #TempTableForData
(
DatabaseName varchar(255),
	FirstStartTime DATETIME,
	LastEndTime DATETIME,
	AverageDuration INT,
	AverageCPU INT,
	AverageReads INT,
	AverageWrites INT,
	ApplicationName varchar(255),
	StatementType BIT,
	StatementText varchar(max),
	[Count] INT

)
INSERT INTO #TempTableForData
	exec p_GetTopQueries
		@SQLServerID,
		@StartDateTime,
		@EndDateTime,
		@UtcOffset,
		@IncludeStoredProcedures,
		@IncludeSQLStatements,
		@IncludeSQLBatches,
		@MinExecutions,
		@MinDuration,
		@MinReads,
		@MinWrites,
		@MinCPU,
		@TopN,
		@SignatureMode,
		4,
		@CaseInsensitive,
		@ApplLike,
		@ApplNotLike,
		@ClientLike,
		@ClientNotLike,
		@DatabaseLike,
		@DatabaseNotLike,
		@SQLTextLike,
		@SQLTextNotLike,
		@UserLike,
		@UserNotLike

    set rowcount @TopN

	select distinct * from #TempTableForData order by [AverageWrites] desc 
end

Go

grant EXECUTE on p_GetTopQueriesByWrites to [SQLdmConsoleUser]

GO
