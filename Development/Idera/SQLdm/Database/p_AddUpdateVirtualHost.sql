
if (object_id('p_AddUpdateVirtualHost') is not null)
begin
drop procedure p_AddUpdateVirtualHost
end
go

create procedure [dbo].[p_AddUpdateVirtualHost]
	@VhName nvarchar(256),
	@VhAddress nvarchar(256),
	@VhUser nvarchar(256),
	@VhPassword nvarchar(256),
	@VhServerType nvarchar(256),
	@VhHostID int output
as
begin
	DECLARE @err int
	declare @vhid int 

    -- Check if Virtual Host already exists
    select @vhid = VHostID from [dbo].[VirtualHostServers] where [VHostAddress] = @VhAddress
    
    if @vhid is null
    begin
		insert into [dbo].[VirtualHostServers] ([VHostName],[VHostAddress],[Username],[Password],[ServerType])
			values(@VhName, @VhAddress, @VhUser, @VhPassword, @VhServerType)

		Select @err = @@ERROR, @vhid = @@IDENTITY
		if @err != 0 GOTO HANDLE_ADD_ERROR
	end
	else
	begin
		update [dbo].[VirtualHostServers] Set [VHostName] = @VhName, [VHostAddress] = @VhAddress, [Username] = @VhUser, [Password] = @VhPassword, [ServerType] = @VhServerType
			where [VHostID] = @vhid
			
		select @err = @@ERROR
		if @err != 0 GOTO HANDLE_UPDATE_ERROR
	end
		
	select @VhHostID = @vhid
	
	return(0)
	
	HANDLE_ADD_ERROR:
		RAISERROR('An error occured while adding the virtual host %s to the repository', 10, 1, @VhAddress)
		RETURN(@err)

	HANDLE_UPDATE_ERROR:
		RAISERROR('An error occurred while updating the virtual host %s in the repository', 10, 1, @VhAddress)
		RETURN(@err)
end

GO


