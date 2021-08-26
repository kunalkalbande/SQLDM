if (object_id('Grooming.p_GroomReplicationTopology') is not null)
begin
drop procedure Grooming.p_GroomReplicationTopology
end
go
create procedure [Grooming].[p_GroomReplicationTopology]
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
	@BlockName = 'ReplicationTopology'
			
while (1=1)
begin
	
    if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,@BlockName);
           
	begin try
	           
		Delete from ReplicationTopology with (tablockx)
		where ReplicationType = 0
		and isnull(LastPublisherSnapshotDateTime,'1/1/1900') <= @CutoffDateTime
		and isnull(LastDistributorSnapshotDateTime, '1/1/1900') <= @CutoffDateTime
		and isnull(LastSubscriberSnapshotDateTime, '1/1/1900') <= @CutoffDateTime

		set @RowsAffected = @@ROWCOUNT
		set @RC = @RC + @RowsAffected

		Delete from ReplicationTopology with (tablockx)
		where ReplicationType <> 0
		and isnull(LastDistributorSnapshotDateTime, '1/1/1900') <= @CutoffDateTime
		
		
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
