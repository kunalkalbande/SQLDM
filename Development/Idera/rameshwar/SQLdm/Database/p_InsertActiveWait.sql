if (object_id('p_InsertActiveWait') is not null)
begin
drop procedure p_InsertActiveWait
end
go

create procedure [dbo].p_InsertActiveWait(
@SQLServerID int,
@UTCCollectionDateTime datetime,
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
@StatementUTCStartTime datetime,
@StatementLocalStartTime datetime,
@WaitDuration bigint,
@WaitType varchar(120),
@WaitTypeID int output,
@MSTicks bigint
)
AS
begin
	declare @ReturnMessage nvarchar(128)
	

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

	if (nullif(@WaitTypeID,0) is null)
	begin
		exec p_InsertWaitTypes @WaitType, @WaitTypeID output, @ReturnMessage output
	end

	if (nullif(@SQLSignatureID,0) is null)
	begin
		exec p_InsertSQLSignature @SQLSignatureHash, @SQLSignature, @SQLStatementID, @SQLSignatureID output
	end


	if (exists(select [MSTicks] from [ActiveWaitStatistics] where [MSTicks] = @MSTicks and [SQLServerID] = @SQLServerID))
	begin
		delete
		from [ActiveWaitStatistics]
		where 
			[MSTicks] = @MSTicks
			and [SQLServerID] = @SQLServerID            
			and [WaitDuration] < @WaitDuration
			and [SessionID] = @SessionID
			and [WaitTypeID] = @WaitTypeID           
			and [HostNameID] = @HostNameID           
			and [ApplicationNameID] = @ApplicationNameID    
			and [LoginNameID] = @LoginNameID          
			and [DatabaseID] = @DatabaseID           
			and [SQLStatementID] = @SQLStatementID 
	end


	insert into [ActiveWaitStatistics]
	([SQLServerID]            
	,[UTCCollectionDateTime]
	,[StatementUTCStartTime]
	,[WaitDuration]         
	,[SessionID]            
	,[WaitTypeID]           
	,[HostNameID]           
	,[ApplicationNameID]    
	,[LoginNameID]          
	,[DatabaseID]           
	,[SQLStatementID]
	,[SQLSignatureID]
	,[MSTicks]
	,[StatementLocalStartTime])
	values
	(@SQLServerID            
	,@UTCCollectionDateTime
	,@StatementUTCStartTime
	,@WaitDuration         
	,@SessionID            
	,@WaitTypeID           
	,@HostNameID           
	,@ApplicationNameID    
	,@LoginNameID          
	,@DatabaseID           
	,@SQLStatementID
	,@SQLSignatureID
	,@MSTicks
	,@StatementLocalStartTime)

END

go
