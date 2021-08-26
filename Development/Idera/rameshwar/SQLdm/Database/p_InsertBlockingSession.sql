if (object_id('[p_InsertBlockingSession]') is not null)
begin
drop procedure [p_InsertBlockingSession]
end
go

create  procedure [dbo].[p_InsertBlockingSession](
@SQLServerID int,
@UTCCollectionDateTime datetime,
@BlockID uniqueidentifier,
@ApplicationName nvarchar(256),
@ApplicationNameID int output,
@DatabaseName nvarchar(255),
@DatabaseID int,
@HostName nvarchar(256),
@HostNameID int output,
@LoginName nvarchar(256),                                           
@LoginNameID int output,
@SessionID smallint,
@SQLStatement nvarchar(max),
@SQLStatementHash nvarchar(30),
@SQLStatementID int output,
@SQLSignature nvarchar(max),
@SQLSignatureHash nvarchar(30),
@SQLSignatureID int output,
@BlockingUTCStartTime datetime,
@BlockingLocalStartTime datetime,
@BlockingDurationMilliseconds bigint
)
AS
begin
	declare @ReturnMessage nvarchar(128)
	
	set @BlockingUTCStartTime = dateadd(ms,datepart(ms,@BlockingUTCStartTime)*-1,@BlockingUTCStartTime)

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

	if (nullif(@SQLSignatureID,0) is null)
	begin
		exec p_InsertSQLSignature @SQLSignatureHash, @SQLSignature, @SQLStatementID, @SQLSignatureID output
	end

	-- The utc start time drifts, so treat any identical blockers starting within the same 2-second window as the same blocker
	if (exists(select [BlockingUTCStartTime] from [BlockingSessionStatistics] where ([BlockingUTCStartTime] = @BlockingUTCStartTime or dateadd(s,-1,[BlockingUTCStartTime]) = @BlockingUTCStartTime or dateadd(s,1,[BlockingUTCStartTime]) = @BlockingUTCStartTime) and [SQLServerID] = @SQLServerID))
	begin
		delete
		from [BlockingSessionStatistics]
		where 
			([BlockingUTCStartTime] = @BlockingUTCStartTime or dateadd(s,-1,[BlockingUTCStartTime]) = @BlockingUTCStartTime or dateadd(s,1,[BlockingUTCStartTime]) = @BlockingUTCStartTime)
			and [SQLServerID] = @SQLServerID            
			and [BlockingDurationMilliseconds] <= @BlockingDurationMilliseconds
			and [SessionID] = @SessionID
			and [HostNameID] = @HostNameID           
			and [ApplicationNameID] = @ApplicationNameID    
			and [LoginNameID] = @LoginNameID          
			and [DatabaseID] = @DatabaseID           
			and [SQLStatementID] = @SQLStatementID 
	end


	insert into [BlockingSessionStatistics]
	([SQLServerID]            
	,[UTCCollectionDateTime]
	,[BlockingUTCStartTime]
	,[BlockingDurationMilliseconds]
	,[SessionID]            
	,[HostNameID]           
	,[ApplicationNameID]    
	,[LoginNameID]          
	,[DatabaseID]           
	,[SQLStatementID]
	,[SQLSignatureID]
	,[BlockingLocalStartTime]
	,[BlockID]
	)
	values
	(@SQLServerID            
	,@UTCCollectionDateTime
	,@BlockingUTCStartTime
	,@BlockingDurationMilliseconds         
	,@SessionID            
	,@HostNameID           
	,@ApplicationNameID    
	,@LoginNameID          
	,@DatabaseID           
	,@SQLStatementID
	,@SQLSignatureID
	,@BlockingLocalStartTime
	,@BlockID)

END


