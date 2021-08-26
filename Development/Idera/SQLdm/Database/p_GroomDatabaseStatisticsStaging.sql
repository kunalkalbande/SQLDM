
If OBJECT_ID('Grooming.p_GroomDatabaseStatisticsStaging') Is Not Null
	Drop Procedure Grooming.p_GroomDatabaseStatisticsStaging;
Go

Create Procedure Grooming.p_GroomDatabaseStatisticsStaging (
	@run_id uniqueidentifier,
	@Sequence int out,
	@TimeoutTime datetime,
	@RecordsToDelete int = 1000,
	@CutoffDateTime datetime,
	@SQLServerID int = null,
	@InstanceName sysname = null,
	@Deleted bit = 0)
as
begin
--Delete from stageDatabaseStatistics where  UTCCollectionDateTime < dateadd(d,-1,getdate())

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)
declare @cmd nvarchar(1000)
declare @parms nvarchar(1000)

Declare @KeysToDelete Table (SourceID uniqueidentifier not null, ServerID int, DatabaseName nvarchar(255), UTCCollectionDateTime datetime)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'DatabaseStatisticsStaging'

while (1=1)
begin
    if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,@BlockName);
           
	begin try

			Delete From @KeysToDelete;

			Insert Into @KeysToDelete
			Select Top(@RecordsToDelete) SourceID, ServerID, DatabaseName, UTCCollectionDateTime
			From [dbo].[stageDatabaseStatistics] with(nolock)
			Where UTCCollectionDateTime <= @CutoffDateTime;
			
			Delete stage
			From [dbo].[stageDatabaseStatistics] stage
			Inner Join @KeysToDelete K
				On K.SourceID = stage.SourceID 
				and K.ServerID = stage.ServerID 
				and K.DatabaseName = stage.DatabaseName
				and K.UTCCollectionDateTime = stage.UTCCollectionDateTime
		
		set @RowsAffected = @@ROWCOUNT;
		set @RC = @RC + @RowsAffected;
		if (@RowsAffected < @RecordsToDelete)
			break;
	end try
	begin catch
		set @ErrorMessage = @BlockName + ERROR_MESSAGE();
		exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, @InstanceName;
		break;
	end catch
end	

exec Grooming.p_LogGroomingAction @run_id, @Sequence out, @BlockName, @RC, @InstanceName;

return @RC;
	           
		
end
GO
