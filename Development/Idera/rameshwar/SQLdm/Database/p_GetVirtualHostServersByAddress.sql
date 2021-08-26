if (object_id('p_GetVirtualHostServersByAddress') is not null)
begin
drop procedure [p_GetVirtualHostServersByAddress]
end
go

CREATE PROCEDURE [dbo].[p_GetVirtualHostServersByAddress](
	@vcAddress nvarchar(256) = null
)
AS
BEGIN
	DECLARE @error int

	SELECT [VHostID] as [vcHostID], [VHostName] as [vcName], [VHostAddress] as [vcAddress], [Username] as [vcUser], [Password] as [vcPassword]
	FROM	[VirtualHostServers] 
	WHERE	(@vcAddress is null or @vcAddress = [VHostAddress])

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while retrieving the virtual host information.', 10, 1)
        RETURN(@error)
END
