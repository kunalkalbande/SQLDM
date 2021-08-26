if (object_id('p_GetNotificationProviders') is not null)
begin
drop procedure [p_GetNotificationProviders]
end
go

create procedure [p_GetNotificationProviders]
	@ProviderId uniqueidentifier
as
begin
	DECLARE @err int
	IF (@ProviderId IS NULL)
	BEGIN	
		SELECT [ProviderId], [SerializedType], [SerializedObject] FROM [NotificationProviders] ORDER BY [SerializedType] ASC
	END
	ELSE
	BEGIN
		SELECT [ProviderId], [SerializedType], [SerializedObject] from [NotificationProviders] 
			WHERE [ProviderId] = @ProviderId ORDER BY [SerializedType] ASC
	END
	SELECT @err = @@error

	RETURN @err
end
