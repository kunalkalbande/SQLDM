if (object_id('Grooming.p_GroomDeadlocks') is not null)
begin
drop procedure Grooming.p_GroomDeadlocks
end
go
create procedure [Grooming].[p_GroomDeadlocks]
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

Declare @KeysToDelete Table (DeadlockID uniqueidentifier not null primary key)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'Deadlocks'
	
if (@SQLServerID is not null)
begin
	Set @cmd = N'Declare @KeysToDelete Table (DeadlockID uniqueidentifier not null primary key)
		
			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) DeadlockID
			From dbo.Deadlocks with(nolock)
			Where SQLServerID = @SQLServerIDIn' +
				Case When @Deleted = 0 Then N'
				And UTCCollectionDateTime <= @CutoffDateTime'
				Else N''
				End + N';
			
			-- This will cause a cascade delete in DeadlockProcesses
			Delete D
			From dbo.Deadlocks D
			Inner Join @KeysToDelete K
				On K.DeadlockID = D.DeadlockID;';
	set @parms = N'@SQLServerIDIn int,@CutoffDateTimeIn datetime,@RecordsToDelete int'	
				
					
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
			Delete From @KeysToDelete;

			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) DeadlockID
			From dbo.Deadlocks with(nolock)
			Where UTCCollectionDateTime <= @CutoffDateTime;
			
			-- This will cause a cascade delete in DeadlockProcesses
			Delete D
			From dbo.Deadlocks D
			Inner Join @KeysToDelete K
				On K.DeadlockID = D.DeadlockID;
		end
		else
		begin
			exec sp_executesql @cmd, @parms, @SQLServerIDIn=@SQLServerID,@CutoffDateTimeIn= @CutoffDateTime,@RecordsToDelete = @RecordsToDelete;
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



