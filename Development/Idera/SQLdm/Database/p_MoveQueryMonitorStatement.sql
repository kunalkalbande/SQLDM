if (object_id('p_MoveQueryMonitorStatement') is not null)
begin
drop procedure p_MoveQueryMonitorStatement
end
go
create procedure p_MoveQueryMonitorStatement
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
	@CompletionTime datetime
as
begin
	exec p_InsertQueryMonitorStatement 
		@SQLServerID,
		@UTCCollectionDateTime,
		@ApplicationName,
		@ApplicationNameID ,
		@DatabaseName,
		@DatabaseID,
		@HostName,
		@HostNameID,
		@LoginName,                                       
		@LoginNameID,
		@SessionID,
		@StatementType,
		@SQLStatement,
		@SQLStatementHash,
		@SQLStatementID,
		@SQLSignature,
		@SQLSignatureHash,
		@SQLSignatureID,
		@StatementUTCStartTime,
		@StatementLocalStartTime,
		@DurationMilliseconds,
		@CPUMilliseconds,
		@Reads,
		@Writes

	update QueryMonitor
		set DeleteFlag = 1
	where 
		SQLServerID=@SQLServerID and
		UTCCollectionDateTime =@UTCCollectionDateTime and
		isnull(DatabaseID ,0) =isnull(@DatabaseID ,0) and
		isnull(ClientComputerName ,'') =isnull(@HostName ,0) and
		isnull(SqlUserName ,'') =isnull(@LoginName ,0) and
		isnull(Spid ,0) =isnull(@SessionID ,0) and
		isnull(StatementType ,0) =isnull(@StatementType ,0) and
		isnull(StatementText ,'') =isnull(@SQLStatement ,0) and
		isnull(DurationMilliseconds ,0) =isnull(@DurationMilliseconds ,0) and
		isnull(CPUMilliseconds ,0) =isnull(@CPUMilliseconds ,0) and
		isnull(Reads ,0) =isnull(@Reads ,0) and
		isnull(Writes ,0) =isnull(@Writes ,0) and
		(CompletionTime is null or isnull(CompletionTime,getdate()) =isnull(@CompletionTime ,getdate()))

end

go

