if (object_id('p_GetVirtualHostServers') is not null)
begin
drop procedure [p_GetVirtualHostServers]
end
go

CREATE PROCEDURE [dbo].[p_GetVirtualHostServers](
	@VcHostID int = null
)
AS
BEGIN
	DECLARE @error int

	SELECT [VHostID] as [vcHostID], [VHostName] as [vcName], [VHostAddress] as [vcAddress], [Username] as [vcUser], [Password] as [vcPassword], [ServerType] as [ServerType]
	FROM	[VirtualHostServers] 
	WHERE	(@VcHostID is null or @VcHostID = [VHostID])

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while retrieving the virtual host information.', 10, 1)
        RETURN(@error)
END
