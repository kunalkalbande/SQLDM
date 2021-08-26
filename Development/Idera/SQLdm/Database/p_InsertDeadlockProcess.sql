if (object_id('p_InsertDeadlockProcess') is not null)
begin
drop procedure p_InsertDeadlockProcess
end
go
create procedure p_InsertDeadlockProcess
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@UTCOccurrenceDateTime datetime,
	@LocalOccurrenceDateTime datetime,
	@DeadlockID uniqueidentifier,         
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
	@SQLSignatureID int output
as
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

	if (nullif(@SQLSignatureID,0) is null)
	begin
		exec p_InsertSQLSignature @SQLSignatureHash, @SQLSignature, @SQLStatementID, @SQLSignatureID output
	end

insert into [DeadlockProcesses]
   ([DeadlockID]
	,[SQLServerID]
	,[UTCCollectionDateTime]
	,[UTCOccurrenceDateTime]
	,[HostNameID]
	,[ApplicationNameID]
	,[LoginNameID]
	,[DatabaseID]
	,[SQLStatementID]
	,[SQLSignatureID] 
	,[SessionID]
	,[LocalOccurrenceDateTime]
)
values
	(@DeadlockID,
	@SQLServerID,
	@UTCCollectionDateTime,
	@UTCOccurrenceDateTime,
	@HostNameID,
	@ApplicationNameID,
	@LoginNameID,
	@DatabaseID,
	@SQLStatementID,
	@SQLSignatureID,
	@SessionID,
	@LocalOccurrenceDateTime)

end
go

