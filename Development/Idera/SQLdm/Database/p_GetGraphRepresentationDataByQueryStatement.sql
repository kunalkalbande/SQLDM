-- SQLdm 9.0 (Abhishek Joshi)

-- Query Monitoring
-- retrieves the Query Monitor Statistics when grouped by Query Statement
-- according to the set filters for the Graphs representation on WebUI

-- exec p_GetGraphRepresentationDataByQueryStatement        @SQLServerID = 9,
--															@ViewID = 1,	
--															@ApplicationFilter = '8,11,12',
--															@DatabaseFilter = '179',
--															@UserFilter = '1',
--															@ClientFilter = '1',
--															@SQLExclude = null,
--															@SQLInclude = '%sys%',
--															@IncludeSQLStatements = '1',
--															@IncludeSQLProcedure = '1',
--															@IncludeSQLBatch = '1',
--															@IncludeIncompletedQueries = '1',
--															@IncludeTimeOverlappedQueries = '1',
--															@StartTimestamp = '2014-07-14 16:00:49.343',
--															@EndTimestamp = '2014-07-16 04:00:53.947',
--															@SQLSignatureIDFilter = 0,
--                                                          @StatementTypeIDFilter = -1

if (object_id('p_GetGraphRepresentationDataByQueryStatement') is not null)
begin
	drop procedure [p_GetGraphRepresentationDataByQueryStatement]
end
go

create procedure [dbo].[p_GetGraphRepresentationDataByQueryStatement]
	@SQLServerID int,
	@ViewID int,
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
	@SQLSignatureIDFilter int = 0,
	@StatementTypeIDFilter int = -1
as
begin
	declare @QueryInit nvarchar(100), --SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - added new query variable for Selecting querystatistics column 
			@Query nvarchar(max),
			@ParameterDefinition nvarchar(4000);

	select @ParameterDefinition = ' @SQLServerID int,
									@ViewID int,
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
									@SQLSignatureIDFilter int,
									@StatementTypeIDFilter int ';

Select @QueryInit= 'Select QMS.QueryStatisticsID as GroupByID,' --SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - Select querystatistics column which works as unique key

--START SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - Removed SELECT keyword since it would already be in the preceding string
	if @ViewID = 1         -- Duration (ms)
		select @Query = '  		QMS.StatementUTCStartTime as StartTime,
								QMS.DurationMilliseconds as YAxisValue,
								SST.StatementSQLText as GroupByName
						   from
								QueryMonitorStatistics as QMS ';
	else if @ViewID = 2      -- CPU Time (ms)
		select @Query = '		QMS.StatementUTCStartTime as StartTime,
								QMS.CPUMilliseconds as YAxisValue,
								SST.StatementSQLText as GroupByName
						   from
								QueryMonitorStatistics as QMS ';				   
	else if @ViewID = 3        -- Reads
		select @Query = '		QMS.StatementUTCStartTime as StartTime,
								QMS.Reads as YAxisValue,
								SST.StatementSQLText as GroupByName
						   from
								QueryMonitorStatistics as QMS ';				   
	else if @ViewID = 4        -- Writes
		select @Query = '       QMS.StatementUTCStartTime as StartTime,
								QMS.Writes as YAxisValue,
								SST.StatementSQLText as GroupByName
						   from
								QueryMonitorStatistics as QMS ';				   
	else if @ViewID = 5        -- I/O
		select @Query = '       QMS.StatementUTCStartTime as StartTime,
								(QMS.Reads + QMS.Writes) as YAxisValue,
								SST.StatementSQLText as GroupByName
						   from
								QueryMonitorStatistics as QMS ';				   
	else if @ViewID = 6         -- Blocking Duration (ms)
		select @Query = '       QMS.StatementUTCStartTime as StartTime,
								BDT.BlockingDuration as YAxisValue,
								SST.StatementSQLText as GroupByName
						   from
								QueryMonitorStatistics as QMS
								left join
								(
									select
										BSS.SQLStatementID,
										BSS.BlockingDurationMilliseconds as BlockingDuration
									from
										BlockingSessionStatistics as BSS
									where
										BSS.SQLServerID = @SQLServerID
								) as BDT
								on QMS.SQLStatementID = BDT.SQLStatementID ';
	else if @ViewID = 7         -- Wait Duration (ms)
		select @Query = '       QMS.StatementUTCStartTime as StartTime,
								WT.WaitDuration as YAxisValue,
								SST.StatementSQLText as GroupByName
						   from
								QueryMonitorStatistics as QMS
								left join
								(
									select
										AWS.SQLStatementID,
										AWS.WaitDuration
									from
										ActiveWaitStatistics as AWS
									where
										AWS.SQLServerID = @SQLServerID
								) as WT
								on QMS.SQLStatementID = WT.SQLStatementID ';
	else if @ViewID = 8         -- Deadlocks
		select @Query = '       QMS.StatementUTCStartTime as StartTime,
								YAxisValue = case
												when DPT.Deadlocks is null then cast(0 as bigint)
												else cast(1 as bigint) 
											 end,
								SST.StatementSQLText as GroupByName
						   from
								QueryMonitorStatistics as QMS
								left join
								(
									select
										DP.SQLStatementID,
										DP.DeadlockID as Deadlocks
									from
										DeadlockProcesses as DP
									where
										DP.SQLServerID = @SQLServerID
								) as DPT
								on QMS.SQLStatementID = DPT.SQLStatementID ';
--END SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - Removed SELECT keyword since it would already be in the preceding string	

	--SQLdm 9.0 (Ankit Srivastava) - Query Plan Grpahical View - Concotenate the query which select querystatistics column 
	select @Query = @QueryInit + @Query + ' left join
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
		select @Query = @Query + ' and QMS.SQLSignatureID = @SQLSignatureIDFilter ';

	if @StatementTypeIDFilter <> -1
		select @Query = @Query + ' and QMS.StatementType = @StatementTypeIDFilter ';

	execute sp_executesql @Query, 
                          @ParameterDefinition, 
						  @SQLServerID, 
						  @ViewID,
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
						  @SQLSignatureIDFilter,
						  @StatementTypeIDFilter

end
go
