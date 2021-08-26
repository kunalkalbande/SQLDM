if (object_id('Grooming.p_GroomTasks') is not null)
begin
drop procedure Grooming.p_GroomTasks
end
go
create procedure [Grooming].[p_GroomTasks]
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

Declare @KeysToDelete Table (TaskID int not null primary key)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'Tasks'
	
if (@SQLServerID is not null)
begin
	Set @cmd = N'Declare @KeysToDelete Table (TaskID int not null primary key)

				Insert Into @KeysToDelete
				Select Top(@RecordsToDelete) TaskID
				From dbo.Tasks with(nolock)
				Where Status = 16
				And ServerName = @SQLServerNameIn' +
				Case When @Deleted = 0 Then N'
				And CreatedOn <= @CutoffDateTimeIn'
				Else N''
				End + N';
			
				Delete QSA
				From dbo.Tasks QSA
				Inner Join @KeysToDelete K
					On K.TaskID = QSA.TaskID;';
	set @parms = N'@SQLServerNameIn nvarchar(256), @CutoffDateTimeIn datetime, @RecordsToDelete int'	
				
					
	if @Deleted = 1
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
			Select Top(@RecordsToDelete) TaskID
			From dbo.Tasks with(nolock)
			Where Status = 16
			And CreatedOn <= @CutoffDateTime;
			
			Delete QSA
			From dbo.Tasks QSA
			Inner Join @KeysToDelete K
				On K.TaskID = QSA.TaskID;
		end
		else
		begin
			if (@SQLServerID < 0)
			begin
				Delete From @KeysToDelete;

				Insert Into @KeysToDelete
				Select Top(@RecordsToDelete) TaskID
				From dbo.Tasks with(nolock)
				Where Status = 16
				And ServerName is null
				And CreatedOn <= @CutoffDateTime;
			
				Delete QSA
				From dbo.Tasks QSA
				Inner Join @KeysToDelete K
					On K.TaskID = QSA.TaskID;
			end
			else
			begin
				exec sp_executesql @cmd, @parms, @SQLServerNameIn = @InstanceName, @CutoffDateTimeIn = @CutoffDateTime, @RecordsToDelete = @RecordsToDelete; 
			end
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




			
			