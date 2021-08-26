if (object_id('Grooming.p_GroomAuditedEvents') is not null)
begin
drop procedure Grooming.p_GroomAuditedEvents
end
go
create procedure [Grooming].[p_GroomAuditedEvents]
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
	@BlockName = 'AuditedEvents'
	
if (@SQLServerID is not null)
begin

	set @cmd = 'delete top (' + cast(@RecordsToDelete as nchar(50)) + ')
				from AuditableEvents
				where
				Name = @SQLServerNameIn'
	set @parms = N'@SQLServerNameIn sysname, @CutoffDateTimeIn datetime'	
				
					
	if @Deleted = 0
	begin
		set @cmd = @cmd + ' and DateTime <= @CutoffDateTimeIn'
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
			delete top (@RecordsToDelete) 
			from AuditableEvents
			where DateTime <= @CutoffDateTime		
		end
		else
		begin
			exec sp_executesql @cmd, @parms, @SQLServerNameIn = @InstanceName, @CutoffDateTimeIn = @CutoffDateTime 
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



