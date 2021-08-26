if (object_id('Grooming.p_GroomESXStatistics') is not null)
begin
drop procedure Grooming.p_GroomESXStatistics
end
go
create procedure [Grooming].[p_GroomESXStatistics]
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

Declare @KeysToDelete Table (SQLServerID int not null,
							UTCCollectionDateTime datetime not null,
							primary key(SQLServerID, UTCCollectionDateTime))
select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'ESXStatistics'
	
if (@SQLServerID is not null)
begin
	Set @cmd = N'Declare @KeysToDelete Table (SQLServerID int not null,
							UTCCollectionDateTime datetime not null,
							primary key(SQLServerID, UTCCollectionDateTime))

				Insert Into @KeysToDelete
				Select Top(@RecordsToDelete) SQLServerID, UTCCollectionDateTime
				From dbo.ESXStatistics with(nolock)
				Where SQLServerID = @SQLServerIDIn' +
				Case When @Deleted = 0 Then N'
				And UTCCollectionDateTime <= @CutoffDateTime'
				Else N''
				End + N';
			
				Delete ES
				From dbo.ESXStatistics ES
				Inner Join @KeysToDelete K
					On K.SQLServerID = ES.SQLServerID
						And K.UTCCollectionDateTime = ES.UTCCollectionDateTime;';
	set @parms = N'@SQLServerIDIn int,@CutoffDateTimeIn datetime,@RecordsToDelete int'	
				
					
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
			Select Top(@RecordsToDelete) SQLServerID, UTCCollectionDateTime
			From dbo.ESXStatistics with(nolock)
			Where UTCCollectionDateTime <= @CutoffDateTime;
			
			Delete ES
			From dbo.ESXStatistics ES
			Inner Join @KeysToDelete K
				On K.SQLServerID = ES.SQLServerID
					And K.UTCCollectionDateTime = ES.UTCCollectionDateTime;
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
