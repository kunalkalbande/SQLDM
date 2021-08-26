if (object_id('Grooming.p_GroomBlocks') is not null)
begin
drop procedure Grooming.p_GroomBlocks
end
go
create procedure [Grooming].[p_GroomBlocks]
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

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'Blocks'
	
if (@SQLServerID is not null)
begin

	set @cmd = 'delete top (' + cast(@RecordsToDelete as nchar(50)) + ')
				from Blocks
				where
				SQLServerID = @SQLServerIDIn'
	set @parms = N'@SQLServerIDIn int,@CutoffDateTimeIn datetime'	
				
					
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
			--this will cascade delete all records from BlockingSessions that have corresponsing xactid's			           
			delete top (@RecordsToDelete)
			from Blocks
			where UTCCollectionDateTime <= @CutoffDateTime
		
		end
		else
		begin
			exec sp_executesql @cmd, @parms, @SQLServerIDIn=@SQLServerID,@CutoffDateTimeIn= @CutoffDateTime 
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
