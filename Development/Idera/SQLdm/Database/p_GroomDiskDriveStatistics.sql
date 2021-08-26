--SQLdm 9.1 (Ankit Srivastava) -- Filegroup and Mount Point Monitoring Improvements -- Added new procedure to groom disk drive stats
if (object_id('Grooming.p_GroomDiskDriveStatistics') is not null)
begin
drop procedure Grooming.p_GroomDiskDriveStatistics
end
go
create procedure [Grooming].[p_GroomDiskDriveStatistics]
(
@run_id uniqueidentifier,
@Sequence int out,
@TimeoutTime datetime,
@RecordsToDelete int = 1000,
@CutoffDateTime datetime,
@SQLServerID int = null,
@InstanceName sysname = null,
@Deleted bit = 0,
@AggregationGrooming bit = 0
)
as
begin

set nocount on    
set ansi_warnings off 

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'DiskDriveStatistics'

if (@SQLServerID is not null)
begin
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
                    DELETE TOP (@RecordsToDelete) dds 
                    FROM dbo.DiskDriveStatistics dds with (nolock)
                    Where dds.UTCCollectionDateTime <= @CutoffDateTime
		end
		else
		begin
                    DELETE TOP (@RecordsToDelete) dds 
                    FROM dbo.DiskDriveStatistics dds with (nolock)
                    Where dds.SQLServerID = @SQLServerID and dds.UTCCollectionDateTime <= @CutoffDateTime
		end
		
		
		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected
		if (@RowsAffected < @RecordsToDelete OR @AggregationGrooming = 1)
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
