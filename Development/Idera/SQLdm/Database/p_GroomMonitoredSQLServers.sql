if (object_id('Grooming.p_GroomMonitoredSQLServers') is not null)
begin
drop procedure Grooming.p_GroomMonitoredSQLServers
end
go
create procedure [Grooming].[p_GroomMonitoredSQLServers]
(
@run_id uniqueidentifier,
@Sequence int out,
@TimeoutTime datetime,
@SQLServerID int = null,
@InstanceName sysname = null,
@Deleted bit = 0
)
as
begin


if (@Deleted = 0)
	return;
	
declare @RowsAffected int
declare @RC int
declare @BlockName nvarchar(256)
declare @ErrorMessage nvarchar(2048)


select 
	@RowsAffected = 0, 
	@RC = 0, 
	@BlockName = 'MonitoredSQLServers (deleting)'

	
if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout in %s', 11, 1,@BlockName);
       
begin try
           
	if (@SQLServerID is not null and @Deleted = 1)
	begin
		           
		delete from MonitoredSQLServers 
			where SQLServerID = @SQLServerID
	
	end
	
	set @RowsAffected = @@ROWCOUNT
	set @RC = @RC + @RowsAffected

end try
begin catch

	set @ErrorMessage = @BlockName + ERROR_MESSAGE()
	exec Grooming.p_LogGroomingAction  @run_id, @Sequence out,@ErrorMessage, @RC, @InstanceName

end catch


exec Grooming.p_LogGroomingAction @run_id, @Sequence out, @BlockName, @RC, @InstanceName

return @RC

end
