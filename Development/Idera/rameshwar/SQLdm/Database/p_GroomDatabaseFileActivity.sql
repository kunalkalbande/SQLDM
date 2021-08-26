if (object_id('Grooming.p_GroomDatabaseFileActivity') is not null)
begin
drop procedure Grooming.p_GroomDatabaseFileActivity
end
go
create procedure [Grooming].[p_GroomDatabaseFileActivity]
(
@run_id uniqueidentifier,
@Sequence int out,
@TimeoutTime datetime,
@RecordsToDelete int = 1000,
@CutoffDateTime datetime,
@SQLServerID int = null,
@InstanceName sysname = null,
@Deleted bit = 0
)
as
begin

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)
declare @cmd nvarchar(1000)
declare @parms nvarchar(1000)

Declare @KeysToDelete Table (UTCCollectionDateTime datetime not null,
						FileID int not null,
						primary key(UTCCollectionDateTime, FileID))

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'DatabaseFileActivity'
	
if (@SQLServerID is not null)
begin

	Set @cmd = N'Declare @KeysToDelete Table (UTCCollectionDateTime datetime not null,
						FileID int not null,
						primary key(UTCCollectionDateTime, FileID))
			
			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) dfa.UTCCollectionDateTime, dfa.FileID
			From dbo.DatabaseFileActivity dfa with(nolock)
			Inner Join DatabaseFiles df with(nolock) on dfa.FileID = df.FileID
			Inner Join SQLServerDatabaseNames dn with(nolock) on df.DatabaseID = dn.DatabaseID
			Where dn.SQLServerID = @SQLServerIDIn' +
				Case When @Deleted = 0 Then N'
				And dfa.UTCCollectionDateTime <= @CutoffDateTimeIn'
				Else N''
				End + N';
		
			Delete DFA
			From dbo.DatabaseFileActivity DFA
			Inner Join @KeysToDelete K
				On K.UTCCollectionDateTime = DFA.UTCCollectionDateTime
					And K.FileID = DFA.FileID;';
	set @parms = N'@SQLServerIDIn int, @CutoffDateTimeIn datetime, @RecordsToDelete int'	
				
					
	if @Deleted = 0
	begin
		set @cmd = @cmd + ' and UTCCollectionDateTime <= @CutoffDateTimeIn'
	end
	else
	begin
		set @BlockName = @BlockName + ' (deleting)'
	end
end	
			
while (1=1)
begin
	
    if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,@BlockName);
           
	begin try
	           
		if (@SQLServerID is null)
		begin
			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) UTCCollectionDateTime, FileID
			From dbo.DatabaseFileActivity with(nolock)
			Where UTCCollectionDateTime <= @CutoffDateTime;
		
			Delete DFA
			From dbo.DatabaseFileActivity DFA
			Inner Join @KeysToDelete K
				On K.UTCCollectionDateTime = DFA.UTCCollectionDateTime
					And K.FileID = DFA.FileID;
		end
		else
		begin
			exec sp_executesql @cmd, @parms, @SQLServerIDIn = @SQLServerID, @CutoffDateTimeIn = @CutoffDateTime, @RecordsToDelete = @RecordsToDelete;
		end
		
		
		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
		if (@RowsAffected < @RecordsToDelete)
			break;
		
	end try
	begin catch

		set @ErrorMessage = @BlockName + ERROR_MESSAGE()
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, @InstanceName
		break;
	
	end catch
end	

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, @BlockName, @RC, @InstanceName

return @RC

end
