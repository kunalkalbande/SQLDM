if (object_id('Grooming.p_GroomGroomingLog') is not null)
begin
drop procedure Grooming.p_GroomGroomingLog
end
go
create procedure [Grooming].[p_GroomGroomingLog]
(
@run_id uniqueidentifier,
@Sequence int out,
@TimeoutTime datetime,
@RecordsToDelete int = 1000
)
as
begin

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'GroomingLog'		

					
while (1=1)
begin
	
    if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,@BlockName);
           
	begin try
	
		delete top (@RecordsToDelete)
		from GroomingLog
				where [RunID] not in
					(select top 10 RunID from GroomingLog (nolock)
						group by RunID
						order by max(UTCActionEndDateTime) desc
					) 

		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
		if (@RowsAffected < @RecordsToDelete)
			break;
		
	end try
	begin catch

		set @ErrorMessage = @BlockName + ERROR_MESSAGE()
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, null
		break;
	
	end catch

	begin try
	
		delete top (@RecordsToDelete)
		from LatestGroomingStatus
				where [GroomingRunID] not in
					(select top 10 GroomingRunID from LatestGroomingStatus (nolock)
						group by GroomingRunID
						order by max(GroomingDateTimeUTC) desc
					) 

		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
		if (@RowsAffected < @RecordsToDelete)
			break;
		
	end try
	begin catch

		set @ErrorMessage = @BlockName + ERROR_MESSAGE()
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, null
		break;
	
	end catch


end	

set @RowsAffected = @@ROWCOUNT
set @RC = @RC + @RowsAffected	
				
exec Grooming.p_LogGroomingAction @run_id, @Sequence out, @BlockName, @RC, null

return @RC

end

