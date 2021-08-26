-- SQLdm 9.0 (Abhishek Joshi)

-- Query Monitoring
-- retrieves the Query Monitor Statistics when grouped by Application
-- according to the set filters

-- exec p_GetQueryMonitorDataByApplication  @SQLServerID = 9,
--											@ApplicationFilter = '8,11,12',
--											@DatabaseFilter = '179',
--											@UserFilter = '1',
--											@ClientFilter = '1',
--											@SQLExclude = null,
--											@SQLInclude = '%sys%',
--											@IncludeSQLStatements = '1',
--											@IncludeSQLProcedure = '1',
--											@IncludeSQLBatch = '1',
--											@IncludeIncompletedQueries = '1',
--											@IncludeTimeOverlappedQueries = '1',
--											@StartTimestamp = '2014-07-14 16:00:49.343',
--											@EndTimestamp = '2014-07-16 04:00:53.947',
--											@SortBy = 'AvgDuration',
--											@SortOrder = 'DESC',
--											@RecordStartIndex = 1,
--											@RecordsCount = 3,
--											@SQLSignatureIDFilter = 0,
--                                          @StatementTypeIDFilter = -1

if (object_id('p_GetQueryMonitorDataByApplication') is not null)
begin
	drop procedure [p_GetQueryMonitorDataByApplication]
end
go

create procedure [dbo].[p_GetQueryMonitorDataByApplication]
	@SQLServerID int,
	@ApplicationFilter nvarchar(500) = null,
	@DatabaseFilter nvarchar(500) = null,
	@UserFilter nvarchar(500) = null,
	@ClientFilter nvarchar(500) = null,
	@SQLExclude nvarchar(200) = null,
	@SQLInclude nvarchar(200) = null,
	@IncludeSQLStatements bit = 1,
	@IncludeSQLProcedure bit = 1,
	@IncludeSQLBatch bit = 1,
	@IncludeIncompletedQueries bit = 0,
	@IncludeTimeOverlappedQueries bit = 0,
	@StartTimestamp datetime = null,     
	@EndTimestamp datetime = null,
	@SortBy nvarchar(50) = 'Application',
	@SortOrder nvarchar(10) = 'ASC',
	@RecordStartIndex int = 0, 
	@RecordsCount int = 0,
	@SQLSignatureIDFilter int = 0,
	@StatementTypeIDFilter int = -1
as
begin
	declare @Query nvarchar(max),
			@ParameterDefinition nvarchar(4000);
			
	select @ParameterDefinition = ' @SQLServerID int,
									@ApplicationFilter nvarchar(500),
									@DatabaseFilter nvarchar(500),
									@UserFilter nvarchar(500),
									@ClientFilter nvarchar(500),
									@SQLExclude nvarchar(200),
									@SQLInclude nvarchar(200),
									@IncludeSQLStatements bit,
									@IncludeSQLProcedure bit,
									@IncludeSQLBatch bit,
									@IncludeIncompletedQueries bit,
									@IncludeTimeOverlappedQueries bit,
									@StartTimestamp datetime,
									@EndTimestamp datetime,
									@SortBy nvarchar(50),
									@SortOrder nvarchar(10),
									@RecordStartIndex int,
									@RecordsCount int,
									@SQLSignatureIDFilter int,
									@StatementTypeIDFilter int ';

	select @Query = '  select
							QMS.ApplicationNameID as ApplicationID,
							AN.ApplicationName as Application,
							count(QMS.ApplicationNameID) as Occurrences,
							sum(QMS.DurationMilliseconds) as TotalDuration,
							avg(QMS.DurationMilliseconds) as AvgDuration,
							sum(QMS.CPUMilliseconds) as TotalCPUTime,
							sum(QMS.Reads) as TotalReads,
							sum(QMS.Writes) as TotalWrites,
							sum(QMS.Reads + QMS.Writes) as TotalIO,
							sum(WT.TotalWaitTime)/count(QMS.ApplicationNameID) as TotalWaitTime,
							max(QMS.CompletionTime) as MostRecentCompletion,
							sum(BDT.TotalBlockingTime)/count(QMS.ApplicationNameID) as TotalBlockingTime,
							sum(DPT.TotalDeadlocks)/count(QMS.ApplicationNameID) as TotalDeadlocks,
							avg(QMS.CPUMilliseconds) as AvgCPUTime,
							avg(QMS.Reads) as AvgReads,
							avg(QMS.Writes) as AvgWrites,
							avg(QMS.Reads + QMS.Writes) as AvgIO,
							avg(WT.TotalWaitTime)/count(QMS.ApplicationNameID) as AvgWaitTime,
							avg(BDT.TotalBlockingTime)/count(QMS.ApplicationNameID) as AvgBlockingTime,
							avg(DPT.TotalDeadlocks)/count(QMS.ApplicationNameID) as AvgDeadlocks
					   from
						    QueryMonitorStatistics as QMS
						    left join
						    (
								select
									AWS.ApplicationNameID,
									sum(AWS.WaitDuration) as TotalWaitTime
								from
									ActiveWaitStatistics as AWS
								where
									AWS.SQLServerID = @SQLServerID
								group by
									AWS.ApplicationNameID
							) as WT
							on QMS.ApplicationNameID = WT.ApplicationNameID
							left join
							(
								select
									BSS.ApplicationNameID,
									sum(BSS.BlockingDurationMilliseconds) as TotalBlockingTime
								from
									BlockingSessionStatistics as BSS
								where
									BSS.SQLServerID = @SQLServerID
								group by
									BSS.ApplicationNameID
							) as BDT
							on QMS.ApplicationNameID = BDT.ApplicationNameID
							left join
							(
								select
									DP.ApplicationNameID,
									count(DP.DeadlockID) as TotalDeadlocks
								from
									DeadlockProcesses as DP
								where
									DP.SQLServerID = @SQLServerID
								group by
									DP.ApplicationNameID
							) as DPT
							on QMS.ApplicationNameID = DPT.ApplicationNameID
							left join
							ApplicationNames as AN
							on QMS.ApplicationNameID = AN.ApplicationNameID
							left join
							(
								select
									SS.SQLStatementID,
									(isnull(SS.SQLStatement,'''') + isnull(SSO.SQLStatementOverflow,'''')) as StatementSQLText
								from	
									SQLStatements as SS
									left join
									SQLStatementsOverflow as SSO
									on SS.SQLStatementID = SSO.SQLStatementID
							) as SST
							on QMS.SQLStatementID = SST.SQLStatementID
					   where
							QMS.SQLServerID = @SQLServerID ';

	if @ApplicationFilter is not null
		select @Query = @Query + ' and QMS.ApplicationNameID in (' + @ApplicationFilter + ') ';

	if @DatabaseFilter is not null
		select @Query = @Query + ' and QMS.DatabaseID in (' + @DatabaseFilter + ') ';

	if @UserFilter is not null
		select @Query = @Query + ' and QMS.LoginNameID in (' + @UserFilter + ') ';

	if @ClientFilter is not null
		select @Query = @Query + ' and QMS.HostNameID in (' + @ClientFilter + ') ';

	if @SQLExclude is not null           -- exclude filter given priority, skip include filter if exclude is not null
		select @Query = @Query + ' and (LOWER(SST.StatementSQLText) not like ''' + LOWER(@SQLExclude) + ''' ) ';
	else if @SQLInclude is not null
		select @Query = @Query + ' and (LOWER(SST.StatementSQLText) like ''' + LOWER(@SQLInclude) + ''' ) ';

	if @IncludeSQLStatements = 0                           -- SQL Statement 
		select @Query = @Query + ' and (QMS.StatementType != 1) ';
	
	if @IncludeSQLProcedure = 0                             -- SQL Procedure   
		select @Query = @Query + ' and (QMS.StatementType != 0) ';
		
	if @IncludeSQLBatch = 0                                  -- SQL Batch
		select @Query = @Query + ' and (QMS.StatementType != 2) ';
		
	if @IncludeIncompletedQueries = 0                        -- Include Completed Queries
		select @Query = @Query + ' and (QMS.CompletionTime is not null) ';
		
	if @IncludeTimeOverlappedQueries = 1 and @StartTimestamp is not null and @EndTimestamp is not null      -- Including overlapped queries
		select @Query = @Query + ' and (not ((QMS.StatementUTCStartTime > @EndTimestamp) or (QMS.CompletionTime < @StartTimestamp)))';
	else if @IncludeTimeOverlappedQueries = 0 and @StartTimestamp is not null and @EndTimestamp is not null
		select @Query = @Query + ' and (QMS.StatementUTCStartTime >= @StartTimestamp) and (QMS.StatementUTCStartTime <=  @EndTimestamp)';

	if @SQLSignatureIDFilter <> 0
		select @Query = @Query + ' and QMS.SQLSignatureID = @SQLSignatureIDFilter';

	if @StatementTypeIDFilter <> -1
		select @Query = @Query + ' and QMS.StatementType = @StatementTypeIDFilter ';

	select @Query = @Query + ' group by QMS.ApplicationNameID, AN.ApplicationName ';  


	IF OBJECT_ID('tempdb..#TempFilteredDataByApplication') IS NOT NULL DROP Table #TempFilteredDataByApplication
 	CREATE TABLE #TempFilteredDataByApplication(ApplicationID int, [Application]  nvarchar(max), Occurrences int, 
 	TotalDuration bigint, AvgDuration bigint, TotalCPUTime bigint, TotalReads bigint, TotalWrites bigint, TotalIO bigint,
 	TotalWaitTime bigint, MostRecentCompletion datetime, TotalBlockingTime bigint,  TotalDeadlocks bigint,AvgCPUTime bigint, AvgReads bigint, AvgWrites bigint,
 	AvgIO bigint,AvgWaitTime bigint, AvgBlockingTime bigint, AvgDeadlocks bigint)

	INSERT INTO #TempFilteredDataByApplication execute sp_executesql @Query, @ParameterDefinition, @SQLServerID, @ApplicationFilter,
 						  @DatabaseFilter, @UserFilter, @ClientFilter, @SQLExclude, @SQLInclude, @IncludeSQLStatements,
 						  @IncludeSQLProcedure, @IncludeSQLBatch, @IncludeIncompletedQueries, @IncludeTimeOverlappedQueries,
 						  @StartTimestamp, @EndTimestamp, @SortBy, @SortOrder, @RecordStartIndex, @RecordsCount,
 						  @SQLSignatureIDFilter, @StatementTypeIDFilter


	select @Query = 'select
						FilteredData.*,
						cast(cast(FilteredData.TotalCPUTime as dec(15,0)) / nullif(FilteredData.TotalDuration,0) as dec (15,2)) as AvgCPUPerSec,
						cast(cast(FilteredData.TotalIO as dec(15,0)) / nullif(FilteredData.TotalDuration,0) as dec (15,2)) as AvgIOPerSec,
						cast((cast(FilteredData.TotalCPUTime as dec(20,5))*100)/nullif((select sum(FilteredData.TotalCPUTime) from #TempFilteredDataByApplication as FilteredData),0) as dec(20,5)) as CPUAsPercentOfList,
						cast((cast(FilteredData.TotalReads as dec(20,5))*100)/nullif((select sum(FilteredData.TotalReads) from #TempFilteredDataByApplication as FilteredData),0) as dec(20,5)) as ReadsAsPercentOfList		
						from
						#TempFilteredDataByApplication as FilteredData';

	-- Rank (sort) the records on the basis on the given filter
	select @Query = 'select
						SortedData.*,
						row_number() over (order by SortedData.' + @SortBy + ' ' + @SortOrder + ') as RecordRank 
					from
						( ' + @Query + ' ) as SortedData';

	if @RecordStartIndex <> 0 and @RecordsCount <> 0
		select @Query = 'select
							ApplicationData.* 
						 from 
							( ' + @Query + ' ) as ApplicationData
						 where 
							ApplicationData.RecordRank between @RecordStartIndex and (@RecordStartIndex + @RecordsCount - 1)';

	execute sp_executesql @Query, 
                          @ParameterDefinition, 
						  @SQLServerID, 
						  @ApplicationFilter,
						  @DatabaseFilter,
						  @UserFilter,
						  @ClientFilter,
						  @SQLExclude,
						  @SQLInclude,
						  @IncludeSQLStatements,
						  @IncludeSQLProcedure,
						  @IncludeSQLBatch,
						  @IncludeIncompletedQueries,
						  @IncludeTimeOverlappedQueries,
						  @StartTimestamp,
						  @EndTimestamp,
						  @SortBy,
						  @SortOrder,
						  @RecordStartIndex,
						  @RecordsCount,
						  @SQLSignatureIDFilter,
						  @StatementTypeIDFilter

end
go
