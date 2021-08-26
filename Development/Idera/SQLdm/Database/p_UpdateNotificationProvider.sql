if (object_id('p_UpdateNotificationProvider') is not null)
begin
drop procedure [p_UpdateNotificationProvider]
end
go

create procedure [p_UpdateNotificationProvider] 
	@ProviderID uniqueidentifier,
	@SerializedType nvarchar(64),
	@SerializedObject nvarchar(max)
as
begin
	DECLARE @err int
	
	UPDATE [NotificationProviders] 
		SET [SerializedType] = @SerializedType,
			[SerializedObject] = @SerializedObject
		WHERE [ProviderId] = @ProviderID 

	SELECT @err = @@error
	RETURN @err
end
