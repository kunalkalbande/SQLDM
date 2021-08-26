if (object_id('p_AddNotificationProvider') is not null)
begin
drop procedure [p_AddNotificationProvider]
end
go

create procedure [p_AddNotificationProvider] 
	@SerializedType nvarchar(64),
	@SerializedObject nvarchar(max),
	@ReturnProviderId uniqueidentifier output
as
begin
	DECLARE @err int
	DECLARE @id uniqueidentifier
	DECLARE @xmlobject xml
	
	-- there are certain conditions where we need to insert a 
	-- provider with a well-known id rather than assigning one.
	
	SELECT @id = @ReturnProviderId
	if (@id IS NULL)
	begin
		SELECT @id = NEWID()
		
		select @xmlobject = cast(@SerializedObject as xml)
		
		set @xmlobject.modify('replace value of (//@Id)[1] with sql:variable("@id")')
		
		select @SerializedObject = convert(nvarchar(max),@xmlobject)
	end

	INSERT	into NotificationProviders (ProviderId, SerializedType, SerializedObject) VALUES (@id, @SerializedType, @SerializedObject)

	SELECT @err = @@error

	if (@err = 0)
	BEGIN	
		SELECT @ReturnProviderId = @id
	END

	RETURN @err
end
