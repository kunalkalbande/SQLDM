IF (object_id('[p_InsertQueryMonitorStatement]') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_InsertQueryMonitorStatement]
END
GO

CREATE procedure [dbo].[p_InsertQueryMonitorStatement]
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@ApplicationName nvarchar(256),
	@ApplicationNameID int output,
	@DatabaseName nvarchar(255),
	@DatabaseID int output,
	@HostName nvarchar(256),
	@HostNameID int output,
	@LoginName nvarchar(256),                                           
	@LoginNameID int output,
	@SessionID smallint,
	@StatementType int,
	@SQLStatement nvarchar(max),
	@SQLStatementHash nvarchar(30),
	@SQLStatementID int output,
	@SQLSignature nvarchar(max),
	@SQLSignatureHash nvarchar(30),
	@SQLSignatureID int output,
	@StatementUTCStartTime datetime,
	@StatementLocalStartTime datetime,
	@DurationMilliseconds bigint,
	@CPUMilliseconds bigint,
	@Reads bigint,
	@Writes bigint,
	@QueryPlan nvarchar(4000)= null, -- --SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- Added optional QueryPlan paramater
	@IsActualPlan bit = null --SQLdm 10.0 (Tarun Sapra): Estimated Query Plan - If this flag is set to false, the query plan is estimated not actual
as
begin

	declare @ReturnMessage nvarchar(128)
	declare @SQLPlanID int

	if (nullif(@ApplicationNameID,0) is null)
	begin
		exec p_InsertApplicationName @ApplicationName, @ApplicationNameID output
	end

	if (nullif(@DatabaseID,0) is null)
	begin
		exec p_InsertDatabaseName @SQLServerID, @DatabaseName, 0, null, @DatabaseID output, @ReturnMessage output
	end

	if (nullif(@HostNameID,0) is null)
	begin
		exec p_InsertHostName @HostName, @HostNameID output
	end

	if (nullif(@LoginNameID,0) is null)
	begin
		exec p_InsertLoginName @LoginName, @LoginNameID output
	end

	if (nullif(@SQLStatementID,0) is null)
	begin
		exec p_InsertSQLStatement @SQLStatementHash, @SQLStatement, @SQLStatementID output
	end

	-- If we get this far and don't have a SQLStatementID then the record is not worth inserting
	-- We'll keep all the lookup values inserted above since they may be of value
	if (nullif(@SQLStatementID,0) is null)
		return

	if (nullif(@SQLSignatureID,0) is null)
	begin
		exec p_InsertSQLSignature @SQLSignatureHash, @SQLSignature, @SQLStatementID, @SQLSignatureID output
	end

	--SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- calling the Insert method fro query plans
	if (@SQLStatementID is not null)
	begin
		exec p_InsertSQLQueryPlan @QueryPlan, @SQLStatementID, @SQLPlanID output, @IsActualPlan
	end

insert into [QueryMonitorStatistics]
	([SQLServerID]
	,[UTCCollectionDateTime]
	,[StatementUTCStartTime] 
	,[DurationMilliseconds] 
	,[CPUMilliseconds]
	,[Reads]
	,[Writes]
	,[HostNameID]
	,[ApplicationNameID]
	,[LoginNameID]
	,[DatabaseID]
	,[StatementType]
	,[SQLStatementID]
	,[SQLSignatureID]
	,[SessionID]
	,[StatementLocalStartTime]
	,[PlanID]) --SQLdm 9.0 (Ankit Srivastava) - SQL Query Plan View - inserting new column
values
	(@SQLServerID
	,@UTCCollectionDateTime
	,@StatementUTCStartTime 
	,@DurationMilliseconds 
	,@CPUMilliseconds
	,@Reads
	,@Writes
	,@HostNameID
	,@ApplicationNameID
	,@LoginNameID
	,@DatabaseID
	,@StatementType
	,@SQLStatementID
	,@SQLSignatureID
	,@SessionID
	,@StatementLocalStartTime
	,@SQLPlanID)--SQLdm 9.0 (Ankit Srivastava) - SQL Query Plan View - inserting new column
end


GO


