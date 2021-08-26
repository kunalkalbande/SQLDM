if (object_id('p_GroomLegacyQueryMonitor') is not null)
begin
drop procedure p_GroomLegacyQueryMonitor
end
go
if (object_id('Grooming.p_GroomLegacyQueryMonitor') is not null)
begin
drop procedure Grooming.p_GroomLegacyQueryMonitor
end
go
create procedure [Grooming].[p_GroomLegacyQueryMonitor]
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

if (object_id('QueryMonitor') is null)
	return;

declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)
declare @cmd nvarchar(1000)
declare @parms nvarchar(1000)

select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'LegacyQueryMonitor'
	
set @parms = N'@SQLServerIDIn int,@CutoffDateTimeIn datetime,@RecordsToDelete int'	
	
if (@SQLServerID is not null and @Deleted = 1)
begin
	Set @cmd = N'Declare @KeysToDelete Table (StatementUTCStartTime datetime not null primary key)

				Insert Into @KeysToDelete
				Select Top(@RecordsToDelete) StatementUTCStartTime
				From dbo.QueryMonitorStatistics with(nolock)
				Where SQLServerID = @SQLServerIDIn;
			
				Delete QMS
				From dbo.QueryMonitorStatistics QMS
				Inner Join @KeysToDelete K
					On K.StatementUTCStartTime = QMS.StatementUTCStartTime;';
end	
else
begin
	Set @cmd = N'Declare @KeysToDelete Table (StatementUTCStartTime datetime not null primary key)

				Insert Into @KeysToDelete
				Select Top(@RecordsToDelete) StatementUTCStartTime
				From dbo.QueryMonitorStatistics with(nolock)
				Where DeleteFlag = 1 
				Or StatementText is null 
				Or len(StatementText) = 0 
				Or UTCCollectionDateTime <= @CutoffDateTimeIn;
			
				Delete QMS
				From dbo.QueryMonitorStatistics QMS
				Inner Join @KeysToDelete K
					On K.StatementUTCStartTime = QMS.StatementUTCStartTime;';
end
			
while (1=1)
begin
	
    if (GetUTCDate() > @TimeoutTime)           
		raiserror (N'Timeout in %s', 11, 1,@BlockName);
           
	begin try
	           
		exec sp_executesql @cmd, @parms, @SQLServerIDIn=@SQLServerID,@CutoffDateTimeIn= @CutoffDateTime,@RecordsToDelete = @RecordsToDelete;
		
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
