-- SQLdm 9.0 (Abhishek Joshi)

-- Query Monitoring
-- retrieves the Query Monitor Statistics when grouped by Database
-- according to the set filters

-- exec p_GetQueryMonitorDataByDatabase     @SQLServerID = 9,
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


if (object_id('p_GetQueryMonitorDataByDatabase') is not null)
begin
	drop procedure [p_GetQueryMonitorDataByDatabase]
end
go

create procedure [dbo].[p_GetQueryMonitorDataByDatabase]
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
	@SortBy nvarchar(50) = 'DatabaseName',
	@SortOrder nvarchar(10) = 'ASC',
	@RecordStartIndex int = 0, 
	@RecordsCount int = 0,
	@SQLSignatureIDFilter int = 0,
	@StatementTypeIDFilter int = -1
as
begin
	declare @Query nvarchar(max),
			@DefaultRecordsCount nvarchar(10),
			@ParameterDefinition nvarchar(4000);
			
	select @DefaultRecordsCount = '10' -- default records count for Applications, Databases, etc for some other groupings

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
							QMS.DatabaseID,
							SSDN.DatabaseName as DatabaseName,
							count(QMS.DatabaseID) as Occurrences,
							sum(QMS.DurationMilliseconds) as TotalDuration,
							avg(QMS.DurationMilliseconds) as AvgDuration,
							sum(QMS.CPUMilliseconds) as TotalCPUTime,
							sum(QMS.Reads) as TotalReads,
							sum(QMS.Writes) as TotalWrites,
							sum(QMS.Reads + QMS.Writes) as TotalIO,
							avg(WT.TotalWaitTime)/count(QMS.DatabaseID) as AvgWaitTime,
							max(QMS.CompletionTime) as MostRecentCompletion,
							avg(BDT.TotalBlockingTime)/count(QMS.DatabaseID) as AvgBlockingTime,
							avg(DPT.TotalDeadlocks)/count(QMS.DatabaseID) as AvgDeadlocks,
							substring(
								( select top (' + @DefaultRecordsCount + ') '','' + DT.ApplicationName as [text()]
								  from 
		                          (
										select 
											QMS.DatabaseID, 
											AN.ApplicationName, 
											AN.ApplicationNameID,
											sum(QMS.DurationMilliseconds) as TotalDuration 
										from 
											QueryMonitorStatistics as QMS
											left join
											ApplicationNames as AN
											on QMS.ApplicationNameID = AN.ApplicationNameID
										where SQLServerID = @SQLServerID';					
	if @ApplicationFilter is not null
		select @Query = @Query + ' and AN.ApplicationNameID in (' + @ApplicationFilter + ') ';
	
	select @Query = @Query + ' group by QMS.DatabaseID, AN.ApplicationName, AN.ApplicationNameID
								) as DT
								   where DT.DatabaseID = QMS.DatabaseID
								   order by DT.DatabaseID
								   for xml path ('''')
                                ), 2, 1000) as Application,
							avg(QMS.CPUMilliseconds) as AvgCPUTime,
							avg(QMS.Reads) as AvgReads,
							avg(QMS.Writes) as AvgWrites,
							avg(QMS.Reads + QMS.Writes) as AvgIO,
							sum(WT.TotalWaitTime)/count(QMS.DatabaseID) as TotalWaitTime,
							sum(BDT.TotalBlockingTime)/count(QMS.DatabaseID) as TotalBlockingTime,
							sum(DPT.TotalDeadlocks)/count(QMS.DatabaseID) as TotalDeadlocks
					   from
						    QueryMonitorStatistics as QMS
						    left join
						    (
								select
									AWS.DatabaseID,
									sum(AWS.WaitDuration) as TotalWaitTime
								from
									ActiveWaitStatistics as AWS
								where
									AWS.SQLServerID = @SQLServerID
								group by
									AWS.DatabaseID
							) as WT
							on QMS.DatabaseID = WT.DatabaseID
							left join
							(
								select
									BSS.DatabaseID,
									sum(BSS.BlockingDurationMilliseconds) as TotalBlockingTime
								from
									BlockingSessionStatistics as BSS
								where
									BSS.SQLServerID = @SQLServerID
								group by
									BSS.DatabaseID
							) as BDT
							on QMS.DatabaseID = BDT.DatabaseID
							left join
							(
								select
									DP.DatabaseID,
									count(DP.DeadlockID) as TotalDeadlocks
								from
									DeadlockProcesses as DP
								where
									DP.SQLServerID = @SQLServerID
								group by
									DP.DatabaseID
							) as DPT
							on QMS.DatabaseID = DPT.DatabaseID
							left join
							SQLServerDatabaseNames as SSDN
							on QMS.DatabaseID = SSDN.DatabaseID
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
		select @Query = @Query + ' and QMS.LoginNameID in (' + @UserFilter + ')';

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

	select @Query = @Query + ' group by QMS.DatabaseID, SSDN.DatabaseName';  
	
	--SQLdm 10.2 (Tushar)--Storing results in temporary table and using this table in subsequent queries.
	IF OBJECT_ID('tempdb..#TempFilteredDataByDatabase') IS NOT NULL DROP Table #TempFilteredDataByDatabase
	CREATE TABLE #TempFilteredDataByDatabase(DatabaseID int, DatabaseName  nvarchar(max), Occurrences int, 
 	TotalDuration bigint, AvgDuration bigint, TotalCPUTime bigint, TotalReads bigint, TotalWrites bigint, TotalIO bigint,
 	AvgWaitTime bigint, MostRecentCompletion datetime, AvgBlockingTime bigint, AvgDeadlocks bigint,[Application] nvarchar(500), 
	AvgCPUTime bigint, AvgReads bigint, AvgWrites bigint,
 	AvgIO bigint, TotalWaitTime bigint, TotalBlockingTime bigint, TotalDeadlocks bigint)
 
	

	INSERT INTO #TempFilteredDataByDatabase execute sp_executesql @Query, @ParameterDefinition, @SQLServerID, @ApplicationFilter,
 						  @DatabaseFilter, @UserFilter, @ClientFilter, @SQLExclude, @SQLInclude, @IncludeSQLStatements,
 						  @IncludeSQLProcedure, @IncludeSQLBatch, @IncludeIncompletedQueries, @IncludeTimeOverlappedQueries,
 						  @StartTimestamp, @EndTimestamp, @SortBy, @SortOrder, @RecordStartIndex, @RecordsCount,
 						  @SQLSignatureIDFilter, @StatementTypeIDFilter


	select @Query = 'select
						FilteredData.*,
						cast(cast(FilteredData.TotalCPUTime as dec(15,0)) / nullif(FilteredData.TotalDuration,0) as dec (15,2)) as AvgCPUPerSec,
						cast(cast(FilteredData.TotalIO as dec(15,0)) / nullif(FilteredData.TotalDuration,0) as dec (15,2)) as AvgIOPerSec,
						cast((cast(FilteredData.TotalCPUTime as dec(20,5))*100)/nullif((select sum(FilteredData.TotalCPUTime) from #TempFilteredDataByDatabase as FilteredData),0) as dec(20,5)) as CPUAsPercentOfList,
						cast((cast(FilteredData.TotalReads as dec(20,5))*100)/nullif((select sum(FilteredData.TotalReads) from #TempFilteredDataByDatabase as FilteredData),0) as dec(20,5)) as ReadsAsPercentOfList		
						from
						#TempFilteredDataByDatabase as FilteredData';

	-- Rank (sort) the records on the basis on the given filter
	select @Query = 'select
						SortedData.*,
						row_number() over (order by SortedData.' + @SortBy + ' ' + @SortOrder + ') as RecordRank 
					from
						( ' + @Query + ' ) as SortedData';

	if @RecordStartIndex <> 0 and @RecordsCount <> 0
		select @Query = 'select
							DatabaseData.* 
						 from 
							( ' + @Query + ' ) as DatabaseData
						 where 
							DatabaseData.RecordRank between @RecordStartIndex and (@RecordStartIndex + @RecordsCount - 1)';

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
