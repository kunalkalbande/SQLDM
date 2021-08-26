-- SQLdm 9.0 (Abhishek Joshi)

-- Query Monitoring
-- retrieves the Query Monitor Statistics when grouped by Query Statement
-- according to the set filters

-- exec p_GetQueryMonitorDataByQueryStatement      @SQLServerID = 9,
--											       @ApplicationFilter = '8,11,12',
--											       @DatabaseFilter = '179',
--											       @UserFilter = '1',
--											       @ClientFilter = '1',
--											       @SQLExclude = null,
--											       @SQLInclude = '%sys%',
--											       @IncludeSQLStatements = '1',
--											       @IncludeSQLProcedure = '1',
--											       @IncludeSQLBatch = '1',
--											       @IncludeIncompletedQueries = '1',
--											       @IncludeTimeOverlappedQueries = '1',
--											       @StartTimestamp = '2014-07-14 16:00:49.343',
--											       @EndTimestamp = '2014-07-16 04:00:53.947',
--											       @SortBy = 'AvgDuration',
--											       @SortOrder = 'DESC',
--											       @RecordStartIndex = 1,
--											       @RecordsCount = 3,
--												   @SQLSignatureIDFilter = 0,
--                                                 @StatementTypeIDFilter = -1

if (object_id('p_GetQueryMonitorDataByQueryStatement') is not null)
begin
	drop procedure [p_GetQueryMonitorDataByQueryStatement]
end
go

create procedure [dbo].[p_GetQueryMonitorDataByQueryStatement]
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
	@SortBy nvarchar(50) = 'StatementSQLText',
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
			
	select @DefaultRecordsCount = '10'  -- default records count for Applications, Databases, etc for some other groupings

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
							QMS.QueryStatisticsID, -- SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - added new column to works as unique key
							QMS.SQLStatementID,
							SST.StatementSQLText,
							QMS.StatementType as EventType,
							count(QMS.SQLStatementID) as Occurrences,
							sum(QMS.DurationMilliseconds) as TotalDuration,
							avg(QMS.DurationMilliseconds) as AvgDuration,
							sum(QMS.CPUMilliseconds) as TotalCPUTime,
							sum(QMS.Reads) as TotalReads,
							sum(QMS.Writes) as TotalWrites,
							sum(WT.TotalWaitTime)/count(QMS.SQLStatementID) as TotalWaitTime,
							max(QMS.CompletionTime) as MostRecentCompletion,
							sum(BDT.TotalBlockingTime)/count(QMS.SQLStatementID) as TotalBlockingTime,
							sum(DPT.TotalDeadlocks)/count(QMS.SQLStatementID) as TotalDeadlocks,
							QMS.SessionID as Spid,
							QMS.StatementUTCStartTime as StartTime,
							SSIG.DoNotAggregate as KeepDetailedHistory,
							(1-SSIG.DoNotAggregate) as Aggregated,						
							substring(
								( select top (' + @DefaultRecordsCount + ') '','' + DT.ApplicationName as [text()]
								  from 
		                          (
										select 
											QMS.SQLStatementID, 
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
	
	select @Query = @Query + ' group by QMS.SQLStatementID, AN.ApplicationName, AN.ApplicationNameID
								) as DT
								   where DT.SQLStatementID = QMS.SQLStatementID
								   order by DT.SQLStatementID
								   for xml path ('''')
                                ), 2, 1000) as Application,
							substring(
								( select top (' + @DefaultRecordsCount + ') '','' + LT.DatabaseName as [text()]
								  from 
								  (
									select 
										QMS.SQLStatementID, 
										SSDN.DatabaseName,
										SSDN.DatabaseID,
										sum(QMS.DurationMilliseconds) as TotalDuration 
									from 
										QueryMonitorStatistics as QMS
										left join
										SQLServerDatabaseNames as SSDN
										on QMS.DatabaseID = SSDN.DatabaseID
										where QMS.SQLServerID = @SQLServerID'; 

	if @DatabaseFilter is not null
		select @Query = @Query + ' and SSDN.DatabaseID in (' + @DatabaseFilter + ') ';

	select @Query = @Query + '	group by QMS.SQLStatementID, SSDN.DatabaseName, SSDN.DatabaseID
									) as LT
								where LT.SQLStatementID = QMS.SQLStatementID
								order by LT.SQLStatementID
								for xml path ('''')
								), 2, 1000) as DatabaseName,
							substring(
								( select top (' + @DefaultRecordsCount + ') '','' + CT.HostName as [text()]
								  from 
		                          (
										select 
											QMS.SQLStatementID, 
											HN.HostName, 
											HN.HostNameID,
											sum(QMS.DurationMilliseconds) as TotalDuration 
										from 
											QueryMonitorStatistics as QMS
											left join
											HostNames as HN
											on QMS.HostNameID = HN.HostNameID
										where SQLServerID = @SQLServerID';
															
	if @ClientFilter is not null
		select @Query = @Query + ' and HN.HostNameID in (' + @ClientFilter + ') ';
	
	select @Query = @Query + ' group by QMS.SQLStatementID, HN.HostName, HN.HostNameID
								) as CT
								   where CT.SQLStatementID = QMS.SQLStatementID
								   order by CT.SQLStatementID
								   for xml path ('''')
                                ), 2, 1000) as Client,
							substring(
								( select top (' + @DefaultRecordsCount + ') '','' + UT.LoginName as [text()]
								  from 
		                          (
										select 
											QMS.SQLStatementID, 
											LN.LoginName, 
											LN.LoginNameID,
											sum(QMS.DurationMilliseconds) as TotalDuration 
										from 
											QueryMonitorStatistics as QMS
											left join
											LoginNames as LN
											on QMS.LoginNameID = LN.LoginNameID
										where SQLServerID = @SQLServerID';
															
	if @UserFilter is not null
		select @Query = @Query + ' and LN.LoginNameID in (' + @UserFilter + ') ';
	
	select @Query = @Query + ' group by QMS.SQLStatementID, LN.LoginName, LN.LoginNameID
								) as UT
								   where UT.SQLStatementID = QMS.SQLStatementID
								   order by UT.SQLStatementID
								   for xml path ('''')
                                ), 2, 1000) as UserName,
							avg(QMS.CPUMilliseconds) as AvgCPUTime		
					   from
						    QueryMonitorStatistics as QMS
						    left join
						    (
								select
									AWS.SQLStatementID,
									sum(AWS.WaitDuration) as TotalWaitTime
								from
									ActiveWaitStatistics as AWS
								where
									AWS.SQLServerID = @SQLServerID
								group by
									AWS.SQLStatementID
							) as WT
							on QMS.SQLStatementID = WT.SQLStatementID
							left join
							(
								select
									BSS.SQLStatementID,
									sum(BSS.BlockingDurationMilliseconds) as TotalBlockingTime
								from
									BlockingSessionStatistics as BSS
								where
									BSS.SQLServerID = @SQLServerID
								group by
									BSS.SQLStatementID
							) as BDT
							on QMS.SQLStatementID = BDT.SQLStatementID
							left join
							(
								select
									DP.SQLStatementID,
									count(DP.DeadlockID) as TotalDeadlocks
								from
									DeadlockProcesses as DP
								where
									DP.SQLServerID = @SQLServerID
								group by
									DP.SQLStatementID
							) as DPT
							on QMS.SQLStatementID = DPT.SQLStatementID
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
							left join
							SQLSignatures as SSIG
							on QMS.SQLSignatureID = SSIG.SQLSignatureID 
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
		select @Query = @Query + ' and QMS.SQLSignatureID = @SQLSignatureIDFilter ';

	if @StatementTypeIDFilter <> -1
		select @Query = @Query + ' and QMS.StatementType = @StatementTypeIDFilter ';

	select @Query = @Query + ' group by QMS.SQLStatementID, SST.StatementSQLText, QMS.StatementType, QMS.SessionID,	
	                                    QMS.StatementUTCStartTime, SSIG.DoNotAggregate, QMS.QueryStatisticsID ';   -- SQLdm 9.0 (Ankit Srivastava) - Query Plan Graphical View - added new column to works as unique key
	
	select @Query = 'select
						FilteredData.*,
						cast(cast(FilteredData.TotalCPUTime as dec(15,0)) / nullif(FilteredData.TotalDuration,0) as dec (15,2)) as AvgCPUPerSec,
						cast((cast(FilteredData.TotalCPUTime as dec(20,5))*100)/nullif((select sum(FilteredData.TotalCPUTime) from ( ' + @Query + ' ) as FilteredData),0) as dec(20,5)) as CPUAsPercentOfList,
						cast((cast(FilteredData.TotalReads as dec(20,5))*100)/nullif((select sum(FilteredData.TotalReads) from ( ' + @Query + ' ) as FilteredData),0) as dec(20,5)) as ReadsAsPercentOfList		
						from
						( ' + @Query + ' ) as FilteredData';

	-- Rank (sort) the records on the basis on the given filter
	select @Query = 'select
						SortedData.*,
						row_number() over (order by SortedData.' + @SortBy + ' ' + @SortOrder + ') as RecordRank 
					from
						( ' + @Query + ' ) as SortedData';

	if @RecordStartIndex <> 0 and @RecordsCount <> 0
		select @Query = 'select
							SQLStatementData.* 
						 from 
							( ' + @Query + ' ) as SQLStatementData
						 where 
							SQLStatementData.RecordRank between @RecordStartIndex and (@RecordStartIndex + @RecordsCount - 1)';

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
