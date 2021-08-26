if (object_id('Grooming.p_GroomAlerts') is not null)
begin
drop procedure Grooming.p_GroomAlerts
end
go
create procedure [Grooming].[p_GroomAlerts]
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

Declare @KeysToDelete Table (AlertID int not null primary key)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'Alerts'
	
if (@SQLServerID is not null)
begin
	Set @cmd = N'Declare @KeysToDelete Table (AlertID int not null primary key)

			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) AlertID
			From dbo.Alerts with(nolock)
			Where ServerName = @SQLServerNameIn' +
				Case When @Deleted = 0 Then N'
				And UTCOccurrenceDateTime <= @CutoffDateTime'
				Else N''
				End + N';
		
			Delete A
			From dbo.Alerts A
			Where Exists (Select 1 From @KeysToDelete
						Where AlertID = A.AlertID);';
	set @parms = N'@SQLServerNameIn nvarchar(256),@CutoffDateTimeIn datetime,@RecordsToDelete int'	
				
					
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
			Select Top(@RecordsToDelete) AlertID
			From dbo.Alerts with(nolock)
			Where UTCOccurrenceDateTime <= @CutoffDateTime;
		
			Delete A
			From dbo.Alerts A
			Where Exists (Select 1 From @KeysToDelete
						Where AlertID = A.AlertID);
	
		end
		else
		begin
			if (@SQLServerID < 0)
			begin
				Delete From @KeysToDelete;

				Insert Into @KeysToDelete
				Select Top(@RecordsToDelete) AlertID
				From dbo.Alerts with(nolock)
				Where UTCOccurrenceDateTime <= @CutoffDateTime
				And ServerName Is Null;
		
				Delete A
				From dbo.Alerts A
				Where Exists (Select 1 From @KeysToDelete
							Where AlertID = A.AlertID);
			end
			else
			begin
				exec sp_executesql @cmd, @parms, @SQLServerNameIn=@InstanceName,@CutoffDateTimeIn= @CutoffDateTime,@RecordsToDelete = @RecordsToDelete;
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