
if (object_id('p_AddVMToMonitoredServer') is not null)
begin
drop procedure p_AddVMToMonitoredServer
end
go

CREATE PROCEDURE [dbo].[p_AddVMToMonitoredServer]
	@SQLServerID int,
	@HostServerID int,
	@VMUid nvarchar(256)
AS
BEGIN
	DECLARE @count int
	declare @err int
	
	select @count = COUNT(SQLServerID) from [dbo].[MonitoredSQLServers] where [SQLServerID] = @SQLServerID 
	select @err = @@ERROR

	if (@count > 0 OR @err = 0)
	begin
		update [dbo].[MonitoredSQLServers] set [VHostID] = @HostServerID, [VmUID] = @VMUid where [SQLServerID] = @SQLServerID 
		
		select @err = @@ERROR 
	end

	if @err > 0
	begin
		RAISERROR('There was an error associating the VM with is [%s] with SQL Server ID %d', 10, 1, @VMUid, @SQLServerID)
		return(@err)
	END

	return(0)

END
GO
