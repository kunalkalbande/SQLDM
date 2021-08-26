if (object_id('p_DeleteVirtualHost') is not null)
begin
drop procedure [p_DeleteVirtualHost]
end
go

create procedure [dbo].[p_DeleteVirtualHost]
	@VhAddress nvarchar(256)
as
begin
	DECLARE @err int
	declare @vhid int 

	-- Check if host exists and if so, get the Host ID
    select @vhid = VHostID from [dbo].[VirtualHostServers] where [VHostAddress] = @VhAddress

	if @vhid is not null
	begin
		update [dbo].[MonitoredSQLServers] set [VHostID] = null, [VmUID] = null, [VmName] = null, [VmDomainName] = null where [VHostID] = @vhid
		
		select @err = @@ERROR
		if @err != 0 GOTO HANDLE_UPDATE_ERROR
		
		delete from [dbo].[VirtualHostServers] where [VHostID] = @vhid
				select @err = @@ERROR
		if @err != 0 GOTO HANDLE_DELETE_ERROR
	end

	return (0)

	HANDLE_UPDATE_ERROR:
		RAISERROR('An error occurred while removing the host associations from the monitored SQL Servers', 10, 1)
		RETURN (@err)
		
	HANDLE_DELETE_ERROR:
		RAISERROR('An error occurred while deleting the virtual host server', 10, 1)
		RETURN (@err)
		
END	